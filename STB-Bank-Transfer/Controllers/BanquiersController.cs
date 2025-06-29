using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using STB_Bank_Transfer.Data;
using STB_Bank_Transfer.Models;
using STB_Bank_Transfer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        // In BanquierController.cs

        [HttpPost("Clients")]
        public async Task<ActionResult<Client>> CreateClient([FromBody] CreateClientRequest request)
        {
            // --- Your validation logic remains the same ---
            if (request == null)
                return BadRequest("Client data is required");

            // ... (other validations for Nom, Email, MotDePasse, BanquierId) ...

            var banker = await _context.Banquiers.FindAsync(request.BanquierId);
            if (banker == null)
                return BadRequest("Specified banker does not exist");

            if (await _context.Clients.AnyAsync(c => c.Email == request.Email))
            {
                return Conflict("Email already exists");
            }

            // --- REFACTORED LOGIC ---

            // 1. Create the new Compte and Client objects in memory.
            //    Do NOT save them yet.
            var compte = new Compte
            {
                IdCompte = Guid.NewGuid().ToString(),
                Solde = 0,
                TypeCompte = request.TypeCompte ?? "Courant"
            };

            var client = new Client
            {
                Nom = request.Nom,
                Email = request.Email,
                MotDePasse = HashPassword(request.MotDePasse),
                Role = UserRole.Client,
                BanquierId = request.BanquierId,

                // Let EF Core manage the foreign key by assigning the navigation property.
                // This is the idiomatic way. EF will see the new Compte and insert it first,
                // then use its generated ID for the Client insert.
                Compte = compte

                // You no longer need to set 'IdCompte' manually:
                // IdCompte = compte.IdCompte, // THIS LINE IS NO LONGER NEEDED
            };

            // 2. Add BOTH entities to the context.
            _context.Clients.Add(client);
            // You don't even need to add the 'compte' separately,
            // EF Core knows about it through the 'client.Compte' relationship.
            // _context.Comptes.Add(compte); // This is not required

            // 3. Save everything in a single transaction.
            await _context.SaveChangesAsync();

            // The rest of your return logic is fine.
            return CreatedAtAction(nameof(GetClient), new { id = client.IdClient }, new
            {
                client.IdClient,
                client.Nom,
                client.Email,
                client.Role,
                client.BanquierId,
                client.IdCompte, // This will be correctly populated now
                Compte = new
                {
                    compte.IdCompte,
                    compte.Solde,
                    compte.TypeCompte
                }
            });
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
        public ActionResult<IEnumerable<VirementDto>> GetPendingTransfers()
        {
            var pending = _virements
                .Where(v => v.Statut == StatutVirement.EN_ATTENTE)
                .OrderBy(v => v.DateCreation)
                .ToList();
            return VirementMapper.ToDtoList(pending);
        }

        [HttpGet("Virements/Rejetes")]
        public ActionResult<IEnumerable<VirementDto>> GetRejectedTransfers()
        {
            var rejected = _virements
                .Where(v => v.Statut == StatutVirement.REJETE)
                .OrderByDescending(v => v.DateValidation)
                .ToList();
            return VirementMapper.ToDtoList(rejected);
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