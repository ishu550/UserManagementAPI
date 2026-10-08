using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagementAPI.Data;
using UserManagementAPI.Models;

namespace UserManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public UsersController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _db.Users.AsNoTracking().ToListAsync();
        }

        // GET: api/users/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();
            return user;
        }

        // POST: api/users
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (user == null)
                return BadRequest("User payload is required.");

            if (string.IsNullOrWhiteSpace(user.FirstName) ||
                string.IsNullOrWhiteSpace(user.LastName) ||
                string.IsNullOrWhiteSpace(user.Email))
            {
                return BadRequest("FirstName, LastName and Email are required.");
            }

            var emailValidator = new EmailAddressAttribute();
            if (!emailValidator.IsValid(user.Email))
            {
                return BadRequest("Email is not in a valid format.");
            }

            var emailExists = await _db.Users.AnyAsync(u => u.Email == user.Email);
            if (emailExists)
            {
                return Conflict("Email already in use.");
            }

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        // PUT: api/users/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User user)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (user == null)
                return BadRequest("User payload is required.");

            if (string.IsNullOrWhiteSpace(user.FirstName) ||
                string.IsNullOrWhiteSpace(user.LastName) ||
                string.IsNullOrWhiteSpace(user.Email))
            {
                return BadRequest("FirstName, LastName and Email are required.");
            }

            var emailValidator = new EmailAddressAttribute();
            if (!emailValidator.IsValid(user.Email))
            {
                return BadRequest("Email is not in a valid format.");
            }

            if (id != user.Id) return BadRequest();

            var exists = await _db.Users.AnyAsync(u => u.Id == id);
            if (!exists) return NotFound();

            _db.Entry(user).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/users/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}