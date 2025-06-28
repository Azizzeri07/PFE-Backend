using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using STB_Bank_Transfer.Data;
using STB_Bank_Transfer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace STB_Bank_Transfer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VirementController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VirementController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Virement>>> GetVirements()
        {
            return await _context.Virements.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Virement>> GetVirement(int id)
        {
            var virement = await _context.Virements.FindAsync(id);
            if (virement == null) return NotFound();
            return virement;
        }

        [HttpPost]
        public async Task<ActionResult<Virement>> CreateVirement(Virement virement)
        {
            virement.DateCreation = System.DateTime.UtcNow;
            virement.Statut = StatutVirement.EN_ATTENTE;

            _context.Virements.Add(virement);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetVirement), new { id = virement.IdVirement }, virement);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVirement(int id, Virement virement)
        {
            if (id != virement.IdVirement) return BadRequest();

            _context.Entry(virement).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VirementExists(id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVirement(int id)
        {
            var virement = await _context.Virements.FindAsync(id);
            if (virement == null) return NotFound();

            _context.Virements.Remove(virement);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VirementExists(int id)
        {
            return _context.Virements.Any(e => e.IdVirement == id);
        }
    }
}
