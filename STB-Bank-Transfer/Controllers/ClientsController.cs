using Microsoft.AspNetCore.Mvc;
using STB_Bank_Transfer.Models;

namespace STB_Bank_Transfer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private static List<Client> clients = new List<Client>();
        private static int nextClientId = 1;


        [HttpPost("{id}/Virements")]
        public ActionResult<Virement> CreateVirement(int id, [FromBody] Virement virement)
        {
            var client = clients.FirstOrDefault(c => c.IdClient == id);
            if (client == null) return NotFound();

            virement.IdVirement = VirementsController.GetNextVirementId();
            virement.DateCreation = DateTime.Now;
            virement.Statut = StatutVirement.EN_ATTENTE;

            client.Virements.Add(virement);
            return CreatedAtAction(nameof(VirementsController.GetVirement),
                               "Virements",
                               new { id = virement.IdVirement },
                               virement);
        }

        [HttpGet("{id}/Virements")]
        public ActionResult<List<Virement>> GetHistoriqueVirements(int id)
        {
            var client = clients.FirstOrDefault(c => c.IdClient == id);
            if (client == null) return NotFound();
            return client.Virements;
        }
    }
}