using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Delivery.API.Data;
using Delivery_Models;

namespace Delivery.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdministratorsController : ControllerBase
    {
        private readonly DeliveryAPIContext _context;

        public AdministratorsController(DeliveryAPIContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Administrator>>> GetAdministrators()
        {
            return await _context.Administrators.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Administrator>> GetAdministrator(int id)
        {
            var administrator = await _context.Administrators
                .Where(a => a.Id == id)
                .FirstOrDefaultAsync();

            if (administrator == null)
            {
                return NotFound();
            }

            return administrator;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAdministrator(int id, Administrator administrator)
        {
            if (id != administrator.Id)
            {
                return BadRequest();
            }

            _context.Entry(administrator).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdministratorExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Administrator>> PostAdministrator(Administrator administrator)
        {
            _context.Administrators.Add(administrator);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAdministrator", new { id = administrator.Id }, administrator);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdministrator(int id)
        {
            var administrator = await _context.Administrators.FindAsync(id);
            if (administrator == null)
            {
                return NotFound();
            }

            _context.Administrators.Remove(administrator);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AdministratorExists(int id)
        {
            return _context.Administrators.Any(e => e.Id == id);
        }
    }
}