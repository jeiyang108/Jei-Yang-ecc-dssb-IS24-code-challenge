namespace ECC.API.Models
{
    public class Product
    {
        public int Id { get; set; }
        public int ProductNumber { get; set; }
        public string ProductName { get; set; }
        public User ProductOwner { get; set; }
        public int ProductOwnerId { get; set; }
        public IEnumerable<ProductDeveloper> ProductDevelopers { get; set; }
        public User ScrumMaster { get; set; }
        public int ScrumMasterId { get; set; }
        public DateTime StartDate { get; set; }
        public string Methodology { get; set; }
        public string Location { get; set; }
    }
}
