using AutoMapper;
using DummyProject.Entity;
using DummyProject.Model;
using DummyProject.Repository;
using DummyProject.Service;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace DummyProject.Controllers
{
    
        [ApiController]
        [Route("api/[controller]")]
        public class OrderController : ControllerBase
        {
            private readonly IGenericRepository<Product> _productRepo;
            private readonly IGenericRepository<Order> _orderRepo;
            private readonly IGenericRepository<OrderDetail> _orderDetailRepo;
            private readonly ICacheService _cacheService;
            private readonly IMessageQueueService _messageQueueService;
            private readonly IMapper _mapper;

            public OrderController(
                IGenericRepository<Product> productRepo,
                IGenericRepository<Order> orderRepo,
                IGenericRepository<OrderDetail> orderDetailRepo,
                ICacheService cacheService,
                IMessageQueueService messageQueueService,
                IMapper mapper)
            {
                _productRepo = productRepo;
                _orderRepo = orderRepo;
                _orderDetailRepo = orderDetailRepo;
                _cacheService = cacheService;
                _messageQueueService = messageQueueService;
                _mapper = mapper;
            }

            [HttpGet("products")]
            public async Task<ActionResult<ApiResponse<List<ProductDto>>>> GetProducts([FromQuery] string category = null)
            {
                try
                {
                    string cacheKey = string.IsNullOrEmpty(category) ? "all_products" : $"products_{category}";
                    var cachedProducts = await _cacheService.GetAsync<List<ProductDto>>(cacheKey);

                    if (cachedProducts != null)
                    {
                        Log.Information("Ürünler Redis cache'ten alındı: {CacheKey}", cacheKey);
                        return Ok(ApiResponse<List<ProductDto>>.Success(cachedProducts, "Ürünler listelendi (cache)"));
                    }

                    var products = await _productRepo.GetAllAsync();
                    if (!string.IsNullOrEmpty(category))
                        products = products.Where(p => p.Category == category).ToList();

                    var productDtos = _mapper.Map<List<ProductDto>>(products);
                    await _cacheService.SetAsync(cacheKey, productDtos);

                    Log.Information("Ürünler veritabanından alındı ve cache'e yazıldı: {CacheKey}", cacheKey);
                    return Ok(ApiResponse<List<ProductDto>>.Success(productDtos, "Ürünler listelendi"));
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Ürünler listelenirken hata oluştu");
                    return StatusCode(500, ApiResponse<List<ProductDto>>.Failed("Bir hata oluştu", "E500"));
                }
            }

            [HttpPost("create")]
            public async Task<ActionResult<ApiResponse<int>>> CreateOrder([FromBody] CreateOrderRequest request)
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    Log.Warning("Sipariş oluşturma isteği geçersiz: {Errors}", errors);
                    return BadRequest(ApiResponse<int>.Failed("Geçersiz istek", "E400"));
                }

                try
                {
                    var order = new Order
                    {
                        CustomerName = request.CustomerName,
                        CustomerEmail = request.CustomerEmail,
                        CustomerGSM = request.CustomerGSM,
                        TotalAmount = request.Products.Sum(p => p.UnitPrice * p.Amount)
                    };

                    await _orderRepo.AddAsync(order);
                    await _orderRepo.SaveChangesAsync();

                    var orderDetails = request.Products.Select(p => new OrderDetail
                    {
                        OrderId = order.Id,
                        ProductId = p.ProductId,
                        UnitPrice = p.UnitPrice
                    }).ToList();

                    await _orderDetailRepo.AddRangeAsync(orderDetails);
                    await _orderDetailRepo.SaveChangesAsync();

                    // Mail kuyruğuna ekle
                    var mailMessage = $"Sayın {request.CustomerName}, siparişiniz (#${order.Id}) oluşturuldu.";
                    _messageQueueService.PublishToQueue("SendMail", mailMessage);

                    Log.Information("Sipariş oluşturuldu: {OrderId}", order.Id);
                    return Ok(ApiResponse<int>.Success(order.Id, "Sipariş oluşturuldu"));
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Sipariş oluşturulurken hata oluştu");
                    return StatusCode(500, ApiResponse<int>.Failed("Bir hata oluştu", "E500"));
                }
            }
        }
    }

