using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ECC.API.Models;
using System.Security.Cryptography.X509Certificates;

namespace ECC.API.Data
{
    public class EccDbContext : DbContext
    {
        private static int productNumberCounter = 1; // Initialize the counter
        private static int userIdCounter = 1;
        private static int productIdCounter = 1;

        public EccDbContext (DbContextOptions<EccDbContext> options) : base(options)
        {
            this.Database.EnsureCreated();
        }

        /*
            Since database implementation was not part of the requirements, mock data will be directly added to the Models.
            When database is implemented, SeedData function can be commented out.
            ViewModels represent how data is displayed or captured in the user interface.
         */

        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ProductDeveloper> ProductDevelopers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed some mock data for Products and Users
            SeedData(modelBuilder); 
        }

        public static void SeedData(ModelBuilder modelBuilder)
        {
            // Seed some mock data for Products and Users

            modelBuilder.Entity<ProductDeveloper>().HasKey(pd => new { pd.ProductId, pd.DeveloperId });


            var users = new List<User> {
                new User { Id = userIdCounter++, Name = "Elvina Markwell", RoleCode = "PO" }, //Product Owner
                new User { Id = userIdCounter++, Name = "Johnson Smith", RoleCode = "PO" },
                new User { Id = userIdCounter++, Name = "Adora Dunning", RoleCode = "PO" },
                new User { Id = userIdCounter++, Name = "Nevsa Pietruszewicz", RoleCode = "PO" },
                new User { Id = userIdCounter++, Name = "John Allen Frede", RoleCode = "SM" }, // Scrum Master
                new User { Id = userIdCounter++, Name = "MaryJane Smith", RoleCode = "SM" },
                new User { Id = userIdCounter++, Name = "Mamie Milmore", RoleCode = "SM" },
                new User { Id = userIdCounter++, Name = "Raymond Markwell", RoleCode = "SM" },
                new User { Id = userIdCounter++, Name = "Bobbi Dubble", RoleCode = "DEV" }, // Developer
                new User { Id = userIdCounter++, Name = "Rayna Greening", RoleCode = "DEV" },
                new User { Id = userIdCounter++, Name = "Raymond Tompson", RoleCode = "DEV" },
                new User { Id = userIdCounter++, Name = "Mitchel Buttner", RoleCode = "DEV" },
                new User { Id = userIdCounter++, Name = "Anya Gravie", RoleCode = "DEV" },
                new User { Id = userIdCounter++, Name = "Sara Burth", RoleCode = "DEV" },
                new User { Id = userIdCounter++, Name = "Maddie Griswaite", RoleCode = "DEV" },
                new User { Id = userIdCounter++, Name = "Hope Johnson", RoleCode = "DEV" },
                new User { Id = userIdCounter++, Name = "Sam Collin", RoleCode = "DEV" }
            };

            modelBuilder.Entity<User>().HasData(users);

            

            for (int i = 0; i < 4; i++)
            {
                var random = new Random();
                var product = new Product
                {
                    Id = productIdCounter++,
                    ProductNumber = productNumberCounter,
                    ProductName = $"Product {productNumberCounter++}",
                    ProductOwnerId = users.Where(u => u.RoleCode == "PO").ElementAt(i).Id,
                    ScrumMasterId = users.Where(u => u.RoleCode == "SM").ElementAt(i).Id,
                    StartDate = DateTime.Today.AddMonths(-3).AddDays(i),
                    Methodology = "Agile",
                    Location = "https://github.com/bcgov/BC-Policy-Framework-For-GitHub"
                };
                var developers = users.Where(u => u.RoleCode == "DEV").OrderBy(x => random.Next()).Take(4).ToList();
                modelBuilder.Entity<Product>().HasData(product);
                foreach (var developer in developers)
                {
                    var productDeveloper = new ProductDeveloper
                    {
                        ProductId = product.Id,
                        DeveloperId = developer.Id,
                        IsActive = true
                    };
                    modelBuilder.Entity<ProductDeveloper>().HasData(productDeveloper);
                }
                
                product = new Product
                {
                    Id = productIdCounter++,
                    ProductNumber = productNumberCounter,
                    ProductName = $"Product {productNumberCounter++}",
                    ProductOwnerId = users.Where(u => u.RoleCode == "PO").ElementAt(i).Id,
                    ScrumMasterId = users.Where(u => u.RoleCode == "SM").ElementAt((i+1)%3).Id,
                    StartDate = DateTime.Today.AddMonths(-2),
                    Methodology = "Waterfall",
                    Location = "https://github.com/bcgov/foi-flow"
                };
                developers = users.Where(u => u.RoleCode == "DEV").OrderBy(x => random.Next()).Take(2).ToList();
                modelBuilder.Entity<Product>().HasData(product);
                foreach (var developer in developers)
                {
                    var productDeveloper = new ProductDeveloper
                    {
                        ProductId = product.Id,
                        DeveloperId = developer.Id,
                        IsActive = true
                    };
                    modelBuilder.Entity<ProductDeveloper>().HasData(productDeveloper);
                }
                random = new Random();
                product = new Product
                {
                    Id = productIdCounter++,
                    ProductNumber = productNumberCounter,
                    ProductName = $"Product {productNumberCounter++}",
                    ProductOwnerId = users.Where(u => u.RoleCode == "PO").ElementAt(i).Id,
                    ScrumMasterId = users.Where(u => u.RoleCode == "SM").ElementAt((i+2)%4).Id,
                    StartDate = DateTime.Today.AddMonths(-4).AddDays(-i),
                    Methodology = "Agile",
                    Location = "https://github.com/bcgov/nr-results-exam"
                };
                developers = users.Where(u => u.RoleCode == "DEV").OrderBy(x => random.Next()).Take(3).ToList();
                modelBuilder.Entity<Product>().HasData(product);
                foreach (var developer in developers)
                {
                    var productDeveloper = new ProductDeveloper
                    {
                        ProductId = product.Id,
                        DeveloperId = developer.Id,
                        IsActive = true
                    };
                    modelBuilder.Entity<ProductDeveloper>().HasData(productDeveloper);
                }

                product = new Product
                {
                    Id = productIdCounter++,
                    ProductNumber = productNumberCounter,
                    ProductName = $"Product {productNumberCounter++}",
                    ProductOwnerId = users.Where(u => u.RoleCode == "PO").ElementAt(i).Id,
                    ScrumMasterId = users.Where(u => u.RoleCode == "SM").ElementAt(i).Id,
                    StartDate = DateTime.Today.AddMonths(-5).AddDays(4 + i),
                    Methodology = "Agile",
                    Location = "https://github.com/bcgov/traction"
                };
                developers = users.Where(u => u.RoleCode == "DEV").OrderBy(x => random.Next()).Take(2).ToList();
                modelBuilder.Entity<Product>().HasData(product);
                foreach (var developer in developers)
                {
                    var productDeveloper = new ProductDeveloper
                    {
                        ProductId = product.Id,
                        DeveloperId = developer.Id,
                        IsActive = true
                    };
                    modelBuilder.Entity<ProductDeveloper>().HasData(productDeveloper);
                }

                product = new Product
                {
                    Id = productIdCounter++,
                    ProductNumber = productNumberCounter,
                    ProductName = $"Product {productNumberCounter++}",
                    ProductOwnerId = users.Where(u => u.RoleCode == "PO").ElementAt(i).Id,
                    ScrumMasterId = users.Where(u => u.RoleCode == "SM").ElementAt(i).Id,
                    StartDate = DateTime.Today.AddMonths(-2 - i),
                    Methodology = "Waterfall",
                    Location = "https://github.com/bcgov/foi-docreviewer"
                };
                developers = users.Where(u => u.RoleCode == "DEV").OrderBy(x => random.Next()).Take(5).ToList();
                modelBuilder.Entity<Product>().HasData(product);
                foreach (var developer in developers)
                {
                    var productDeveloper = new ProductDeveloper
                    {
                        ProductId = product.Id,
                        DeveloperId = developer.Id,
                        IsActive = true
                    };
                    modelBuilder.Entity<ProductDeveloper>().HasData(productDeveloper);
                }
            }


        }
        public int GetProductNumberCounter()
        {
            return productNumberCounter++;
        }

        public int GetUserIdCounter()
        {
            return userIdCounter++;
        }
        public int GetProductIdCounter()
        {
            return productIdCounter++;
        }
    }
}
