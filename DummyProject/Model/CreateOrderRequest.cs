using System.ComponentModel.DataAnnotations;

namespace DummyProject.Model
{
    public class CreateOrderRequest
    {
        [Required(ErrorMessage = "Müşteri adı zorunludur.")]
        [StringLength(100)]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "E-posta zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [StringLength(150)]
        public string CustomerEmail { get; set; }

        [Required(ErrorMessage = "Telefon numarası zorunludur.")]
        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
        [StringLength(15)]
        public string CustomerGSM { get; set; }

        [Required(ErrorMessage = "Ürün listesi boş olamaz.")]
        public List<ProductDetail> Products { get; set; }
    }

    public class ProductDetail
    {
        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir ürün ID giriniz.")]
        public int ProductId { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Birim fiyat pozitif olmalıdır.")]
        public decimal UnitPrice { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Miktar pozitif bir tam sayı olmalıdır.")]
        public int Amount { get; set; }
    }
}
