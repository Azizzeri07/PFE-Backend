using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using STB_Bank_Transfer.Data;
using STB_Bank_Transfer.Models;
using static STB_Bank_Transfer.Models.Virement;

namespace STB_Bank_Transfer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public ClientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private static List<Client> clients = new List<Client>();
        private static int nextClientId = 1;


        // Assuming this is inside your "ClientsController" or a similar controller.
        // The route parameter {id} should be the ID of the client.

        [HttpPost("{id}/Virements")]
        public async Task<ActionResult<Virement>> CreateVirement(int id, [FromBody] CreateVirementRequest virementRequest)
        {
            // 1. Find the client IN THE DATABASE using the DbContext.
            var client = await _context.Clients.FindAsync(id);

            // 2. Check if the client was found in the database.
            if (client == null)
            {
                return NotFound($"Client with ID {id} not found.");
            }

            // 3. Create a new Virement entity from the request data (DTO).
            var virement = new Virement
            {
                DateCreation = DateTime.UtcNow,
                Statut = StatutVirement.EN_ATTENTE,
                Montant = virementRequest.Montant,
                Motif = virementRequest.Motif,
                NumCompteSource = client.IdCompte, // Assuming the client's account is the source
                NumCompteDestination = virementRequest.NumCompteDestination,
                RaisonRejet = string.Empty
            };

            _context.Virements.Add(virement);
            await _context.SaveChangesAsync();

            // Return a Created response with the correct URI for the virement resource
            return Created($"/api/Virements/{virement.IdVirement}", virement);
        }

        [HttpGet("{id}/Virements")]
        public async Task<ActionResult<List<Virement>>> GetHistoriqueVirements(int id)
        {
            // Use the database context to get the client and their virements
            var client = await _context.Clients.Include(c => c.Virements).FirstOrDefaultAsync(c => c.IdClient == id);
            if (client == null) return NotFound();
            return client.Virements ?? new List<Virement>();
        }
    }
}