using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECC.API.Data;
using ECC.API.Models;
using Microsoft.AspNetCore.SignalR;
using Swashbuckle.AspNetCore.Annotations;

namespace ECC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : Controller
    {
        private readonly EccDbContext _context;

        public ProductController(EccDbContext context)
        {
            _context = context;
        }

        // GET: api/Product
        [SwaggerOperation(
            Summary = "Get All Products", Description = "The GET endpoint retrieves all project records, along with product owner, scrum master and developer users that are associated to each project. As your front-end application call http://localhost:3000/api/product, an array of Product will be returned in JSON format.")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductViewModel>>> GetAllProducts()
        {

            var products = await (from product in _context.Products
                                   join po in _context.Users
                                    on product.ProductOwnerId equals po.Id
                                   join sm in _context.Users
                                    on product.ScrumMasterId equals sm.Id
                                  join pd in _context.ProductDevelopers
                                    on product.Id equals pd.ProductId into productDevelopers
                                  select (new ProductViewModel
                                   {
                                       ProductId = product.Id,
                                       ProductName = product.ProductName,
                                       ProductOwner = new UserViewModel { UserId = po.Id, UserName = po.Name, UserRoleCode = po.RoleCode },
                                       ProductNumber = product.ProductNumber,
                                       ScrumMaster = new UserViewModel { UserId = sm.Id, UserName= sm.Name, UserRoleCode= sm.RoleCode },
                                       Location = product.Location,
                                       Methodology = product.Methodology,
                                       Developers = productDevelopers.Select(pd => new UserViewModel
                                       {
                                           UserId = pd.Developer.Id,
                                           UserName = pd.Developer.Name,
                                           UserRoleCode = pd.Developer.RoleCode
                                       }).ToList(),
                                       StartDate = product.StartDate
                                  })).ToListAsync();

            return Ok(products);
        }

        // GET: api/Product/5
        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Get Product by ID", Description = "The GET endpoint retireves details of a product with the given ID. As your front-end application call http://localhost:3000/api/product/id, a Product will be returned in JSON format.")]
        public async Task<ActionResult<ProductViewModel>> GetProduct(int id)
        {
            var viewModel = await (from product in _context.Products
                                   join po in _context.Users
                                    on product.ProductOwnerId equals po.Id
                                   join sm in _context.Users
                                    on product.ScrumMasterId equals sm.Id
                                   join pd in _context.ProductDevelopers
                                    on product.Id equals pd.ProductId into productDevelopers
                                   where product.Id == id
                                   select (new ProductViewModel
                                   {
                                       ProductId = product.Id,
                                       ProductName = product.ProductName,
                                       ProductOwner = new UserViewModel { UserId = po.Id, UserName = po.Name, UserRoleCode = po.RoleCode },
                                       ProductNumber = product.ProductNumber,
                                       ScrumMaster = new UserViewModel { UserId = sm.Id, UserName = sm.Name, UserRoleCode = sm.RoleCode },
                                       Location = product.Location,
                                       Methodology = product.Methodology,
                                       Developers = productDevelopers.Select(pd => new UserViewModel
                                       {
                                           UserId = pd.Developer.Id,
                                           UserName = pd.Developer.Name,
                                           UserRoleCode = pd.Developer.RoleCode
                                       }).ToList(),
                                       StartDate = product.StartDate
                                   })).FirstAsync();

            if (viewModel == null)
            {
                return NotFound();
            }

            return Ok(viewModel);
        }

        // PUT: api/Product/5
        [HttpPut("{id:int}")]
        [SwaggerOperation(
            Summary = "Update an existing Product", Description = "The PUT request requires ID and the body. Product ID is as part of the URL(http://localhost:3000/api/product/id) and the updated product data should be passed as the request body. The provided product data is compared to the previous version stored in the database, and all columns and associated user entities are updated based on the request.")]
        public async Task<IActionResult> UpdateProduct([FromRoute] int id, [FromBody] ProductViewModel product)
        {
            // Validation
            if (product.ProductName == "" || product.Location == "")
            {
                return BadRequest("Make sure to enter Product Name and Location");
            }
            if (!product.Location.StartsWith("http") && !product.Location.StartsWith("github.com/") && !product.Location.StartsWith("www."))
            {
                return BadRequest("Please provide a valid URL for location");
            }
            if (product.Developers.Count > 5)
            {
                return BadRequest("You can add up to 5 developers");
            }
            var dbProduct = await _context.Products.FindAsync(id);
            
            if (dbProduct != null)
            {
                dbProduct.Location = product.Location;
                dbProduct.Methodology = product.Methodology;
                var sm = await _context.Users.FirstOrDefaultAsync(u => u.Id == product.ScrumMaster.UserId && u.Name == product.ScrumMaster.UserName);
                if (sm == null)
                {
                    sm = new User
                    {
                        Id = _context.GetUserIdCounter(),
                        Name = product.ScrumMaster.UserName,
                        RoleCode = "SM"
                    };
                    await _context.Users.AddAsync(sm);
                }
                dbProduct.ScrumMasterId = sm.Id;
                var po = await _context.Users.FirstOrDefaultAsync(u => u.Id == product.ProductOwner.UserId && u.Name == product.ProductOwner.UserName);
                if (po == null)
                {
                    po = new User
                    {
                        Id = _context.GetUserIdCounter(),
                        Name = product.ProductOwner.UserName,
                        RoleCode = "PO"
                    };
                    await _context.Users.AddAsync(po);
                }
                dbProduct.ProductOwnerId = po.Id;
                dbProduct.ProductName = product.ProductName;
                dbProduct.StartDate = product.StartDate;
                dbProduct.ProductDevelopers = await _context.ProductDevelopers.Where(pd => pd.ProductId == id).ToListAsync();

                // Iterate through the developer list of DB
                foreach (var dev in dbProduct.ProductDevelopers)
                {
                    // The developer needs to be removed from DB as it does not exist in the view model.
                    if (!product.Developers.Any(d => d.UserId == dev.DeveloperId))
                    {
                        _context.ProductDevelopers.Remove(dev);
                    }
                }
                // Interate through the developer list provided by front-end
                foreach (var dev in product.Developers)
                {
                    if (dev.UserId is 0)
                    {
                        // The developer is new. Add this new Developer to db and associate to the product
                        var newUser = new User
                        {
                            Id = _context.GetUserIdCounter(),
                            Name = dev.UserName,
                            RoleCode = "DEV"
                        };
                        await _context.Users.AddAsync(newUser);
                        var newProdDev = new ProductDeveloper
                        {
                            ProductId = id,
                            DeveloperId = newUser.Id,
                            IsActive = true
                        };
                        await _context.ProductDevelopers.AddAsync(newProdDev);
                    }
                    else
                    {
                        var dbDevUser = await _context.Users.FindAsync(dev.UserId);
                        if (dbDevUser == null)
                        {
                            return NotFound();
                        }

                        if (!dbProduct.ProductDevelopers.Any(pd => pd.DeveloperId == dev.UserId))
                        {
                            // Associate the existing user to the product
                            var newProductDev = new ProductDeveloper
                            {
                                DeveloperId = dbDevUser.Id,
                                ProductId = dbProduct.Id,
                                IsActive = true
                            };
                            await _context.ProductDevelopers.AddAsync(newProductDev);
                        }
                    }
                }

            }
            _context.Update(dbProduct);
            await _context.SaveChangesAsync();
            return Ok();
        }


        // POST: api/Product
        [HttpPost]
        [SwaggerOperation(Summary = "Create a new Product", Description = "As your front-end application call http://localhost:3000/api/product along with the request body, a new Product record is created using the provided data, and added to the database. After a successful creation, it will return a Product data including a newly generated Product ID, which can be used to reload the front-end interface.")]
        public async Task<ActionResult<ProductViewModel>> AddProduct([FromBody] ProductViewModel product)
        {
            // Validation
            if (product.ProductName == "" || product.Location == "")
                {
                return BadRequest("Make sure to enter Product Name and Location");
            }
            if (!product.Location.StartsWith("http") && !product.Location.StartsWith("github.com/") && !product.Location.StartsWith("www."))
            {
                return BadRequest("Please provide a valid URL for location");
            }
            if (product.Developers.Count > 5)
            {
                return BadRequest("You can add up to 5 developers");
            }

            // Adding Product Owner, Scrum Master and Product to DB
            var po = await _context.Users.FirstOrDefaultAsync(u => u.Name == product.ProductOwner.UserName);
            if (po == null)
            {
                po = new User
                {
                    Id = _context.GetUserIdCounter(),
                    Name = product.ProductOwner.UserName,
                    RoleCode = "PO"
                };
                await _context.Users.AddAsync(po);
            }
            var sm = await _context.Users.FirstOrDefaultAsync(u => u.Name == product.ScrumMaster.UserName);
            if (sm == null)
            {
                sm = new User
                {
                    Id = _context.GetUserIdCounter(),
                    Name = product.ScrumMaster.UserName,
                    RoleCode = "SM"
                };
                await _context.Users.AddAsync(sm);
            }
            var newProduct = new Product
            {
                Id = _context.GetProductIdCounter(),
                ProductName = product.ProductName,
                ProductNumber = _context.GetProductNumberCounter(),
                ProductOwnerId = po.Id,
                ScrumMasterId = sm.Id,
                StartDate = product.StartDate,
                Location = product.Location,
                Methodology = product.Methodology
            };
            await _context.Products.AddAsync(newProduct);

            // Adding Developers to DB
            foreach (var dev in product.Developers)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Name == dev.UserName);
                if (user == null)
                {
                    user = new User
                    {
                        Id = _context.GetUserIdCounter(),
                        Name = dev.UserName,
                        RoleCode = "DEV"
                    };
                    await _context.Users.AddAsync(user);
                }
                await _context.ProductDevelopers.AddAsync(new ProductDeveloper
                {
                    ProductId = newProduct.Id,
                    DeveloperId = user.Id,
                    IsActive = true
                });

            }
            await _context.SaveChangesAsync();
            return Ok(new ProductViewModel { ProductId = newProduct.Id });
        }



    }
}
