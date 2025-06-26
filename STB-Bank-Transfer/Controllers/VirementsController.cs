using Microsoft.AspNetCore.Mvc;
using STB_Bank_Transfer.Models;

namespace STB_Bank_Transfer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VirementsController : ControllerBase
    {
        private static List<Virement> virements = new List<Virement>();
        private static int nextVirementId = 1;

        public static int GetNextVirementId() => nextVirementId++;

        [HttpGet]
        public ActionResult<List<Virement>> GetVirementsEnAttente()
        {
            return virements.Where(v => v.Statut == StatutVirement.EN_ATTENTE).ToList();
        }

        [HttpGet("{id}")]
        public ActionResult<Virement> GetVirement(int id)
        {
            var virement = virements.FirstOrDefault(v => v.IdVirement == id);
            if (virement == null) return NotFound();
            return virement;
        }

        [HttpPost("{id}/Valider")]
        public IActionResult ValiderVirement(int id)
        {
            var virement = virements.FirstOrDefault(v => v.IdVirement == id);
            if (virement == null) return NotFound();

            virement.Statut = StatutVirement.APPROUVE;
            virement.DateValidation = DateTime.Now;
            return NoContent();
        }

        [HttpPost("{id}/Rejeter")]
        public IActionResult RejeterVirement(int id, [FromBody] string raison)
        {
            var virement = virements.FirstOrDefault(v => v.IdVirement == id);
            if (virement == null) return NotFound();

            virement.Statut = StatutVirement.REJETE;
            virement.RaisonRejet = raison;
            virement.DateValidation = DateTime.Now;
            return NoContent();
        }
    }
}