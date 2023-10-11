namespace ECC.API.Models
{
    public class ProductDeveloper
    {
        public Product Product { get; set; }
        public User Developer { get; set; }
        public int ProductId { get; set; }
        public int DeveloperId { get; set; }
        public bool IsActive { get; set; }
    }
}
