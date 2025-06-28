using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using STB_Bank_Transfer.Data;
using STB_Bank_Transfer.Models;
using System;
using System.Threading.Tasks;

namespace STB_Bank_Transfer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OperationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OperationController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Operation>> GetOperation(int id)
        {
            var op = await _context.Operations.FindAsync(id);
            if (op == null) return NotFound();
            return op;
        }

        [HttpPost]
        public async Task<ActionResult<Operation>> CreateOperation(Operation operation)
        {
            operation.DateOperation = DateTime.UtcNow;

            _context.Operations.Add(operation);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOperation), new { id = operation.IdOperation }, operation);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOperation(int id, Operation operation)
        {
            if (id != operation.IdOperation) return BadRequest();

            _context.Entry(operation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OperationExists(id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOperation(int id)
        {
            var op = await _context.Operations.FindAsync(id);
            if (op == null) return NotFound();

            _context.Operations.Remove(op);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OperationExists(int id)
        {
            return _context.Operations.Any(e => e.IdOperation == id);
        }
    }
}
