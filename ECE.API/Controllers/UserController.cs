using ECC.API.Data;
using ECC.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
