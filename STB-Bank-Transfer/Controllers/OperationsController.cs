using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using STB_Bank_Transfer.Data;
using STB_Bank_Transfer.Models;

namespace STB_Bank_Transfer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OperationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OperationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Operation>>> GetOperations()
        {
            return await _context.Operations.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Operation>> GetOperation(int id)
        {
            var operation = await _context.Operations.FindAsync(id);
            if (operation == null) return NotFound();
            return operation;
        }

        [HttpGet("Compte/{numeroCompte}")]
        public async Task<ActionResult<IEnumerable<Operation>>> GetOperationsByCompte(string numeroCompte)
        {
            return await _context.Operations
                .Where(o => o.NumCompte == numeroCompte)
                .OrderByDescending(o => o.DateOperation)
                .ToListAsync();
        }

        [HttpPost("Debit")]
        public async Task<ActionResult<Operation>> PostOperationDebit(Operation operation)
        {
            var compte = await _context.Comptes.FindAsync(operation.NumCompte);
            if (compte == null) return NotFound("Compte non trouvé");

            if (!compte.Debiter(operation.Montant))
                return BadRequest("Solde insuffisant");

            operation.DateOperation = DateTime.Now;
            operation.TypeOperation = "Débit";

            _context.Operations.Add(operation);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOperation), new { id = operation.IdOperation }, operation);
        }

        [HttpPost("Credit")]
        public async Task<ActionResult<Operation>> PostOperationCredit(Operation operation)
        {
            var compte = await _context.Comptes.FindAsync(operation.NumCompte);
            if (compte == null) return NotFound("Compte non trouvé");

            compte.Crediter(operation.Montant);

            operation.DateOperation = DateTime.Now;
            operation.TypeOperation = "Crédit";

            _context.Operations.Add(operation);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOperation), new { id = operation.IdOperation }, operation);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOperation(int id)
        {
            var operation = await _context.Operations.FindAsync(id);
            if (operation == null) return NotFound();

            _context.Operations.Remove(operation);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}