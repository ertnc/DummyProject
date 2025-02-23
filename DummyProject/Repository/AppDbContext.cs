using Bogus;
using DummyProject.Entity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace DummyProject.Repository
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 1000 rastgele ürün ekleme
            var fakeProducts = new Faker<Product>()
                .RuleFor(p => p.Id, f => f.IndexFaker + 1)
                .RuleFor(p => p.Description, f => f.Commerce.ProductName())
                .RuleFor(p => p.Category, f => f.Commerce.Department())
                .RuleFor(p => p.Unit, f => f.PickRandom("Adet", "Kg", "Lt"))
                .RuleFor(p => p.UnitPrice, f => f.Random.Decimal(1, 1000))
                .RuleFor(p => p.Status, f => (byte)1)
                .RuleFor(p => p.CreateDate, f => DateTime.UtcNow)
                .RuleFor(p => p.UpdateDate, f => DateTime.UtcNow)
                .Generate(1000);

            modelBuilder.Entity<Product>().HasData(fakeProducts);
        }
    }
}
