namespace DummyProject.Entity
{
    public class Product
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public byte Status { get; set; } // 1: Aktif, 0: Pasif
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
