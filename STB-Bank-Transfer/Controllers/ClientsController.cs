using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using STB_Bank_Transfer.Data;
using STB_Bank_Transfer.Models;
using System.Linq;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace STB_Bank_Transfer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ClientController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/Client
        [HttpPost]
        public async Task<ActionResult<object>> CreateClient([FromBody] Client client)
        {
            // Validation manuelle des champs requis
            if (string.IsNullOrWhiteSpace(client.Nom))
                return BadRequest("Le nom est requis");
            if (string.IsNullOrWhiteSpace(client.Email))
                return BadRequest("L'email est requis");
            if (string.IsNullOrWhiteSpace(client.MotDePasse))
                return BadRequest("Le mot de passe est requis");
            if (client.BanquierId <= 0)
                return BadRequest("Un banquier valide est requis");

            // Vérification de l'unicité de l'email
            if (await _context.Clients.AnyAsync(c => c.Email == client.Email))
                return Conflict("Cet email est déjà utilisé");

            // Vérification que le banquier existe
            if (!await _context.Banquiers.AnyAsync(b => b.IdBanquier == client.BanquierId))
                return BadRequest("Banquier introuvable");

            // Création du client (en ignorant d'éventuels champs supplémentaires envoyés)
            var newClient = new Client
            {
                Nom = client.Nom,
                Email = client.Email,
                MotDePasse = client.MotDePasse, // Devrait être hashé en production
                BanquierId = client.BanquierId
            };

            _context.Clients.Add(newClient);
            await _context.SaveChangesAsync();

            // Retourne seulement les champs demandés
            return CreatedAtAction(nameof(GetClient), new { id = newClient.IdClient }, new
            {
                newClient.IdClient,
                newClient.Nom,
                newClient.Email,
                newClient.BanquierId
            });
        }

        // GET: api/Client
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetClients()
        {
            return await _context.Clients
                .Select(c => new
                {
                    c.IdClient,
                    c.Nom,
                    c.Email,
                    c.BanquierId
                })
                .ToListAsync();
        }

        // GET: api/Client/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetClient(int id)
        {
            var client = await _context.Clients
                .Where(c => c.IdClient == id)
                .Select(c => new
                {
                    c.IdClient,
                    c.Nom,
                    c.Email,
                    c.BanquierId
                })
                .FirstOrDefaultAsync();

            if (client == null)
                return NotFound();

            return client;
        }

        // GET: api/Client/5/Virements
        [HttpGet("{id}/Virements")]
        public async Task<ActionResult<IEnumerable<Virement>>> GetClientVirements(int id)
        {
            var clientExists = await _context.Clients.AnyAsync(c => c.IdClient == id);
            if (!clientExists)
                return NotFound($"Client avec l'id {id} non trouvé");

            var virements = await _context.Virements
                .Where(v => v.NumCompteSource != null &&
                            _context.Comptes.Any(compte => compte.IdCompte == int.Parse(v.NumCompteSource) &&
                                                           compte.ClientId == id))
                .ToListAsync();

            return virements;
        }

        [HttpPost("{idClient}/Virements")]
        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Banquier}")]
        public async Task<ActionResult<Virement>> CreateVirementForClient(int idClient, [FromBody] Virement virementRequest)
        {
            // 1. Vérification des autorisations
            if (!User.IsInRole(UserRoles.Admin))
            {
                var currentBanquierId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var clientBanquierId = await _context.Clients
                    .Where(c => c.IdClient == idClient)
                    .Select(c => c.BanquierId)
                    .FirstOrDefaultAsync();

                if (clientBanquierId != currentBanquierId)
                    return Forbid("Vous ne pouvez créer des virements que pour vos clients");
            }

            // 2. Vérification du client
            var client = await _context.Clients
                .Include(c => c.CompteS)
                .FirstOrDefaultAsync(c => c.IdClient == idClient);

            if (client == null)
                return NotFound($"Client avec l'id {idClient} non trouvé");

            if (client.CompteS == null)
                return BadRequest("Le client n'a pas de compte associé");

            // 3. Validation du compte source
            if (client.CompteS.IdCompte.ToString() != virementRequest.NumCompteSource)
                return BadRequest("Le compte source ne correspond pas au compte du client");

            // 4. Vérification du compte destination
            var compteDestExists = await _context.Comptes
                .AnyAsync(c => c.IdCompte.ToString() == virementRequest.NumCompteDestination);

            if (!compteDestExists)
                return BadRequest("Le compte destinataire n'existe pas");

            // 5. Vérification du solde suffisant (si débit immédiat)
            if (client.CompteS.Solde < virementRequest.Montant)
                return BadRequest("Solde insuffisant pour effectuer le virement");

            // 6. Création du virement
            var virement = new Virement
            {
                NumCompteSource = virementRequest.NumCompteSource,
                NumCompteDestination = virementRequest.NumCompteDestination,
                Montant = virementRequest.Montant,
                Motif = virementRequest.Motif,
                DateCreation = DateTime.UtcNow,
                Statut = StatutVirement.EN_ATTENTE,
                IdCompte = client.CompteS.IdCompte,
                ClientId = client.IdClient
            };

            // 7. Enregistrement
            _context.Virements.Add(virement);

            // Optionnel: Débiter immédiatement si c'est le comportement souhaité
            // client.CompteS.Debiter(virementRequest.Montant);

            await _context.SaveChangesAsync();

            // 8. Retour de la réponse
            return CreatedAtAction(nameof(GetVirement), new { id = virement.IdVirement }, new
            {
                virement.IdVirement,
                virement.NumCompteSource,
                virement.NumCompteDestination,
                virement.Montant,
                virement.Motif,
                virement.DateCreation,
                virement.Statut
            });
        }

        private object GetVirement()
        {
            throw new NotImplementedException();
        }

        // PUT: api/Client/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClient(int id, [FromBody] Client clientUpdate)
        {
            if (id != clientUpdate.IdClient)
                return BadRequest("ID mismatch");

            var existingClient = await _context.Clients.FindAsync(id);
            if (existingClient == null)
                return NotFound();

            // Mise à jour seulement des champs autorisés
            existingClient.Nom = clientUpdate.Nom;
            existingClient.Email = clientUpdate.Email;

            if (!string.IsNullOrWhiteSpace(clientUpdate.MotDePasse))
                existingClient.MotDePasse = clientUpdate.MotDePasse; // Devrait être hashé

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClientExists(id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Client/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
                return NotFound();

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.IdClient == id);
        }
    }
}