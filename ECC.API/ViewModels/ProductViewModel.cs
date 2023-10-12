namespace ECC.API.Models
{
    public class ProductViewModel
    {
        public int ProductId { get; set; }
        public int ProductNumber { get; set; }
        public string ProductName { get; set; }
        public UserViewModel ProductOwner { get; set; }
        public ICollection<UserViewModel> Developers { get; set; }
        public UserViewModel ScrumMaster { get; set; }
        public DateTime StartDate { get; set; }
        public string Methodology { get; set; }
        public string Location { get; set; }

    }
}
