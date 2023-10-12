using ECC.API.Data;
using ECC.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace ECC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly EccDbContext _context;

        public UserController(EccDbContext context)
        {
            _context = context;
        }

        // GET: api/User/Dev
        [HttpGet("Dev")]
        [SwaggerOperation(Summary = "Get all Developers", Description = "As your front-end application call http://localhost:3000/api/user/dev, an array of Users with the Developer role will be returned in JSON format. This GET request can be used to load a list of Developrs that currently exist in the system.")]
        public async Task<ActionResult<IEnumerable<UserViewModel>>> GetDevelopers()
        {
            var developers = await _context.Users.Where(u => u.RoleCode == "DEV")
                            .Select(d => new UserViewModel
                            {
                                UserId = d.Id,
                                UserName = d.Name,
                                UserRoleCode = d.RoleCode
                            }).ToListAsync();
                
            return Ok(developers);
        }
    }
}
