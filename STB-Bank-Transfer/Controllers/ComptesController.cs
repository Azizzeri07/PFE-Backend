using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using STB_Bank_Transfer.Data;
using STB_Bank_Transfer.Models;

namespace STB_Bank_Transfer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComptesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ComptesController(ApplicationDbContext context)
        {
            _context = context;
        }


       
        [HttpGet("{id}")]
        public async Task<ActionResult<Compte>> GetCompte(string id)
        {
            var compte = await _context.Comptes.FindAsync(id);
            if (compte == null) return NotFound();
            return compte;
        }

        [HttpGet("{id}/Operations")]
        public async Task<ActionResult<IEnumerable<Operation>>> GetHistoriqueOperations(string id)
        {
            return await _context.Operations
                .Where(o => o.NumCompte == id)
                .OrderByDescending(o => o.DateOperation)
                .ToListAsync();
        }

        [HttpPost("{id}/Debiter")]
        public async Task<IActionResult> DebiterCompte(string id, Operation operation)
        {
            var compte = await _context.Comptes.FindAsync(id);
            if (compte == null) return NotFound();

            if (!compte.Debiter(operation.Montant))
                return BadRequest("Solde insuffisant");

            operation.NumCompte = id;
            operation.DateOperation = DateTime.Now;
            operation.TypeOperation = "Débit";

            _context.Operations.Add(operation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{id}/Crediter")]
        public async Task<IActionResult> CrediterCompte(string id, Operation operation)
        {
            var compte = await _context.Comptes.FindAsync(id);
            if (compte == null) return NotFound();

            compte.Crediter(operation.Montant);

            operation.NumCompte = id;
            operation.DateOperation = DateTime.Now;
            operation.TypeOperation = "Crédit";

            _context.Operations.Add(operation);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}