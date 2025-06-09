using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RotaryAdminAPI.Data;
using RotaryAdminAPI.Models;
using System.Security.Claims;

namespace RotaryAdminAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]  // Protect all endpoints
    public class ContactController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ContactController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/contact
        [HttpGet]
        public async Task<ActionResult<Contact>> GetContact()
        {
            var adminUsername = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.Identity?.Name;

            if (string.IsNullOrEmpty(adminUsername))
                return Unauthorized();

            var contact = await _context.Contacts.FirstOrDefaultAsync(c => c.AdminUsername == adminUsername);

            if (contact == null)
                return NotFound();

            return Ok(contact);
        }

        // POST: api/contact
        [HttpPost]
        public async Task<ActionResult<Contact>> CreateContact(Contact contact)
        {
            var adminUsername = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.Identity?.Name;
            if (string.IsNullOrEmpty(adminUsername))
                return Unauthorized();

            contact.AdminUsername = adminUsername;

            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetContact), contact);
        }

        // PUT: api/contact
        [HttpPut]
        public async Task<IActionResult> UpdateContact(Contact updatedContact)
        {
            var adminUsername = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.Identity?.Name;

            if (string.IsNullOrEmpty(adminUsername))
                return Unauthorized();
            if (updatedContact == null)
                return BadRequest("Updated contact cannot be null");
            var contact = await _context.Contacts.FirstOrDefaultAsync(c => c.AdminUsername == adminUsername);

            if (contact == null)
                return NotFound();

            // Update contact properties
            contact.Name = updatedContact.Name;
            contact.Email = updatedContact.Email;
            contact.Phone = updatedContact.Phone;
            contact.Additionalph = updatedContact.Additionalph;
            contact.Address = updatedContact.Address;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
