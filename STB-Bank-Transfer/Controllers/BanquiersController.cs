using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using STB_Bank_Transfer.Data;
using STB_Bank_Transfer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace STB_Bank_Transfer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BanquierController : ControllerBase
    {
        private static readonly List<Banquier> _banquiers = new List<Banquier>();
        private static readonly List<Virement> _virements = new List<Virement>();
        private readonly ApplicationDbContext _context;

        public BanquierController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Méthode pour hacher le mot de passe avec SHA256
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        [HttpPost("Clients")]
        public async Task<ActionResult<Client>> CreateClient([FromBody] Client client)
        {
            if (client == null)
                return BadRequest("Client data is required");

            if (string.IsNullOrWhiteSpace(client.Nom))
                return BadRequest("Client name is required");

            if (string.IsNullOrWhiteSpace(client.Email))
                return BadRequest("Email is required");

            if (string.IsNullOrWhiteSpace(client.MotDePasse))
                return BadRequest("Password is required");

            // Vérification de l'email unique
            if (_banquiers.Any(b => b.Email == client.Email) ||
                await _context.Clients.AnyAsync(c => c.Email == client.Email))
            {
                return Conflict("Email already exists");
            }

            // Hachage du mot de passe avant sauvegarde
            client.MotDePasse = HashPassword(client.MotDePasse);

            // Ajout au contexte
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetClient), new { id = client.IdClient }, client);
        }

        [HttpGet("Clients/{id}")]
        public async Task<ActionResult<Client>> GetClient(int id)
        {
            // First check database
            var client = await _context.Clients.FindAsync(id);

            // If not found in database, check static list (if needed)
            // client = client ?? _clients.FirstOrDefault(c => c.IdClient == id);

            return client == null ? NotFound() : client;
        }



        [HttpGet("Virements/EnAttente")]
        public ActionResult<IEnumerable<Virement>> GetPendingTransfers()
        {
            return _virements
                .Where(v => v.Statut == StatutVirement.EN_ATTENTE)
                .OrderBy(v => v.DateCreation)
                .ToList();
        }

        [HttpGet("Virements/Rejetes")]
        public ActionResult<IEnumerable<Virement>> GetRejectedTransfers()
        {
            return _virements
                .Where(v => v.Statut == StatutVirement.REJETE)
                .OrderByDescending(v => v.DateValidation)
                .ToList();
        }

        [HttpPut("Virements/{id}/Approuver")]
        public IActionResult ApproveTransfer(int id)
        {
            var transfer = _virements.FirstOrDefault(v => v.IdVirement == id);
            if (transfer == null) return NotFound("Transfer not found");
            if (transfer.Statut != StatutVirement.EN_ATTENTE)
                return BadRequest("Only pending transfers can be approved");

            transfer.Statut = StatutVirement.APPROUVE;
            transfer.DateValidation = DateTime.Now;
            return Ok(new { Message = "Transfer approved", Transfer = transfer });
        }

        [HttpPut("Virements/{id}/Rejeter")]
        public IActionResult RejectTransfer(int id, [FromBody] string raison)
        {
            var transfer = _virements.FirstOrDefault(v => v.IdVirement == id);
            if (transfer == null) return NotFound("Transfer not found");
            if (transfer.Statut != StatutVirement.EN_ATTENTE)
                return BadRequest("Only pending transfers can be rejected");
            if (string.IsNullOrWhiteSpace(raison))
                return BadRequest("Rejection reason is required");

            transfer.Statut = StatutVirement.REJETE;
            transfer.DateValidation = DateTime.Now;
            transfer.RaisonRejet = raison;
            return Ok(new { Message = "Transfer rejected", Transfer = transfer });
        }
    }
}