using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using STB_Bank_Transfer.Data;
using STB_Bank_Transfer.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

[Route("api/[controller]")]
[ApiController]
[Authorize] // Toutes les actions nécessitent une authentification
public class BanquierController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public BanquierController(ApplicationDbContext context)
    {
        _context = context;
    }

    // PUT: api/Banquier/5
    [HttpPut("{id}")]
[Authorize(Roles = $"{UserRoles.Banquier}")]
public async Task<IActionResult> UpdateBanquier(int id, [FromBody] Banquier banquierUpdate)
{
    try
    {
        // Vérification plus robuste de l'ID
        var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserIdClaim))
            return Forbid("Claim NameIdentifier manquant");

        if (!int.TryParse(currentUserIdClaim, out var currentUserId))
            return Forbid("ID utilisateur invalide");

        var isAdmin = User.IsInRole(UserRoles.Admin);

        // Un banquier ne peut modifier que son propre compte
        if (!isAdmin && currentUserId != id)
            return Forbid($"Vous ne pouvez modifier que votre propre profil (ID:{currentUserId} != {id})");

        // Récupération et vérification de l'entité
        var existingBanquier = await _context.Banquiers.FindAsync(id);
        if (existingBanquier == null)
            return NotFound();

        // Journalisation pour débogage

        // Mise à jour des champs
        existingBanquier.Nom = banquierUpdate.Nom ?? existingBanquier.Nom;
        existingBanquier.Email = banquierUpdate.Email ?? existingBanquier.Email;

        if (!string.IsNullOrWhiteSpace(banquierUpdate.MotDePasse))
            existingBanquier.SetPassword(banquierUpdate.MotDePasse);

        // Seul l'admin peut modifier le rôle
        if (isAdmin && !string.IsNullOrWhiteSpace(banquierUpdate.Role))
            existingBanquier.Role = banquierUpdate.Role;

        await _context.SaveChangesAsync();
        return NoContent();
    }
    catch (Exception ex)
    {
        return StatusCode(500, "Une erreur interne est survenue");
    }
}

    [HttpPost("Clients")]
    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Banquier}")]
    public async Task<IActionResult> RegisterClient([FromBody] Client model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (await _context.Clients.AnyAsync(c => c.Email == model.Email))
            return BadRequest("Email déjà utilisé");

        // Récupérer l'ID du banquier authentifié
        var banquierId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        // Créer le client
        var client = new Client
        {
            Nom = model.Nom,
            Email = model.Email,
            Role = UserRoles.Client,
            BanquierId = banquierId
        };
        client.SetPassword(model.MotDePasse);

        // Créer le compte associé
        var compte = new Compte
        {
            Solde = model.Solde,
            TypeCompte = model.TypeCompte ?? "Courant",
            BanquierId = banquierId
        };

        _context.Clients.Add(client);
        _context.Comptes.Add(compte);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Client et compte créés avec succès",
            clientId = client.IdClient,
            compteId = compte.IdCompte
        });
    }

    private object GetClientById()
    {
        throw new NotImplementedException();
    }

    // GET: api/Banquier/Clients/5
    [HttpGet("Clients/{id}")]
    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Banquier}")]
    public async Task<ActionResult<Client>> GetClient(int id)
    {
        var client = await _context.Clients.FindAsync(id);
        if (client == null)
            return NotFound();

        // Vérifier que le banquier a accès à ce client
        if (!User.IsInRole(UserRoles.Admin) &&
            client.BanquierId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value))
        {
            return Forbid();
        }

        // Ne pas retourner le mot de passe
        client.MotDePasse = null;
        return client;
    }

    // GET: api/Banquier/Virements/EnAttente
    [HttpGet("Virements/EnAttente")]
    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Banquier}")]
    public async Task<ActionResult<IEnumerable<Virement>>> GetPendingTransfers()
    {
        var query = _context.Virements
            .Where(v => v.Statut == StatutVirement.EN_ATTENTE)
            .Include(v => v.Comptes);

        // Un banquier ne voit que les virements de ses clients
        if (!User.IsInRole(UserRoles.Admin))
        {
            var banquierId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Virement, Compte>)query.Where(v => v.Comptes.Client.BanquierId == banquierId);
        }

        return await query.ToListAsync();
    }

    // PUT: api/Banquier/Virements/5/Approuver
    [HttpPut("Virements/{id}/Approuver")]
    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Banquier}")]
    public async Task<IActionResult> ApproveTransfer(int id)
    {
        var virement = await GetVirementWithAccessCheck(id);
        if (virement is NotFoundResult)
            return virement;
        if (virement is ForbidResult)
            return virement;

        var actualVirement = (Virement)((OkObjectResult)virement).Value;

        if (actualVirement.Statut != StatutVirement.EN_ATTENTE)
            return BadRequest("Seuls les virements en attente peuvent être approuvés");

        actualVirement.Statut = StatutVirement.APPROUVE;
        actualVirement.DateValidation = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new { Message = "Virement approuvé avec succès" });
    }

    // PUT: api/Banquier/Virements/5/Rejeter
    [HttpPut("Virements/{id}/Rejeter")]
    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Banquier}")]
    public async Task<IActionResult> RejectTransfer(int id, [FromBody] string raison)
    {
        if (string.IsNullOrWhiteSpace(raison))
            return BadRequest("La raison du rejet est requise");

        var virement = await GetVirementWithAccessCheck(id);
        if (virement is NotFoundResult)
            return virement;
        if (virement is ForbidResult)
            return virement;

        var actualVirement = (Virement)((OkObjectResult)virement).Value;

        if (actualVirement.Statut != StatutVirement.EN_ATTENTE)
            return BadRequest("Seuls les virements en attente peuvent être rejetés");

        actualVirement.Statut = StatutVirement.REJETE;
        actualVirement.DateValidation = DateTime.UtcNow;
        actualVirement.RaisonRejet = raison;

        await _context.SaveChangesAsync();

        return Ok(new { Message = "Virement rejeté" });
    }

    private async Task<IActionResult> GetVirementWithAccessCheck(int id)
    {
        var virement = await _context.Virements
            .Include(v => v.Comptes)
            .ThenInclude(c => c.Client)
            .FirstOrDefaultAsync(v => v.IdVirement == id);

        if (virement == null)
            return NotFound("Virement non trouvé");

        // Vérifier les droits d'accès
        if (!User.IsInRole(UserRoles.Admin) &&
            virement.Comptes.Client.BanquierId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value))
        {
            return Forbid();
        }

        return Ok(virement);
    }

    private bool BanquierExists(int id)
    {
        return _context.Banquiers.Any(e => e.IdBanquier == id);
    }
}