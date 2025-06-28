using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using STB_Bank_Transfer.Data;
using STB_Bank_Transfer.Models;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly JwtService _jwtService;

    public AuthController(ApplicationDbContext context, JwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    [HttpGet("Banquiers")]
    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Banquier}")]
    public async Task<IActionResult> GetAllBanquiers()
    {
        var banquiers = await _context.Banquiers
            .Select(b => new
            {
                b.IdBanquier,
                b.Nom,
                b.Email,
                b.Role
            })
            .ToListAsync();

        return Ok(banquiers);
    }

    [HttpPost("RegisterBanquier")]
    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Banquier}")]
    public async Task<IActionResult> RegisterBanquier([FromBody] RegisterBanquier model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (await _context.Banquiers.AnyAsync(b => b.Email == model.Email))
            return BadRequest("Email déjà utilisé");

        var banquier = new Banquier
        {
            Nom = model.Nom,
            Email = model.Email,
            MotDePasse = model.MotDePasse,
            Role = model.Role
        };

        banquier.SetPassword(banquier.MotDePasse);
        _context.Banquiers.Add(banquier);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Banquier enregistré avec succès" });
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] Login login)
    {
        var banquier = await _context.Banquiers.FirstOrDefaultAsync(b => b.Email == login.Email);
        if (banquier != null && banquier.VerifyPassword(login.Password))
        {
            var token = _jwtService.GenerateToken(banquier.Email, banquier.Role);
            return Ok(new { token, role = banquier.Role });
        }

        return Unauthorized("Invalid email or password");
    }

    [HttpDelete("DeleteBanquier/{id}")]
    [Authorize(Roles = UserRoles.Admin)] // Seul l'admin peut supprimer un banquier
    public async Task<IActionResult> DeleteBanquier(int id)
    {
        // Vérifier si l'utilisateur essaie de se supprimer lui-même (optionnel)
        var currentUserEmail = User.Identity.Name;
        var banquierToDelete = await _context.Banquiers.FindAsync(id);

        if (banquierToDelete == null)
        {
            return NotFound("Banquier non trouvé");
        }

        // Empêcher un admin de se supprimer lui-même
        if (banquierToDelete.Email == currentUserEmail && banquierToDelete.Role == UserRoles.Admin)
        {
            return BadRequest("Un administrateur ne peut pas se supprimer lui-même");
        }

        _context.Banquiers.Remove(banquierToDelete);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Banquier supprimé avec succès" });
    }
}