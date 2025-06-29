using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using STB_Bank_Transfer.Data;
using STB_Bank_Transfer.Helpers;
using STB_Bank_Transfer.Models;
using System.Linq;
using System.Threading.Tasks;
using BCrypt.Net;

namespace STB_Bank_Transfer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtHelper _jwtHelper;
        public AuthController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _jwtHelper = new JwtHelper(configuration);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel user)
        {
            string role = user.Role;
            string email = user.Email;
            string password = user.MotDePasse;
            string nom = user.Nom;

            if (role == "Client")
            {
                if (await _context.Clients.AnyAsync(u => u.Email == email))
                    return BadRequest("Email already exists");
                var client = new Client { Nom = nom, Email = email, MotDePasse = BCrypt.Net.BCrypt.HashPassword(password) };
                _context.Clients.Add(client);
                await _context.SaveChangesAsync();
                return Ok("Client registered successfully");
            }
            else if (role == "Banquier")
            {
                if (await _context.Banquiers.AnyAsync(u => u.Email == email))
                    return BadRequest("Email already exists");
                var banquier = new Banquier { Nom = nom, Email = email, MotDePasse = BCrypt.Net.BCrypt.HashPassword(password) };
                _context.Banquiers.Add(banquier);
                await _context.SaveChangesAsync();
                return Ok("Banquier registered successfully");
            }
            else if (role == "Admin")
            {
                if (await _context.Banquiers.AnyAsync(u => u.Email == email))
                    return BadRequest("Email already exists");
                var admin = new Banquier { Nom = nom, Email = email, MotDePasse = BCrypt.Net.BCrypt.HashPassword(password) };
                _context.Banquiers.Add(admin);
                await _context.SaveChangesAsync();
                return Ok("Admin registered successfully");
            }
            else
            {
                return BadRequest("Invalid role");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel login)
        {
            string role = login.Role;
            string email = login.Email;
            string password = login.MotDePasse;

            if (role == "Client")
            {
                var client = await _context.Clients.FirstOrDefaultAsync(u => u.Email == email);
                if (client == null || !BCrypt.Net.BCrypt.Verify(password, client.MotDePasse))
                    return Unauthorized("Invalid credentials");
                var token = _jwtHelper.GenerateToken(client.IdClient.ToString(), client.Email, "Client");
                return Ok(new { client.IdClient, client.Nom, client.Email, role = "Client", token });
            }
            else if (role == "Banquier" || role == "Admin")
            {
                var banquier = await _context.Banquiers.FirstOrDefaultAsync(u => u.Email == email);
                if (banquier == null || !BCrypt.Net.BCrypt.Verify(password, banquier.MotDePasse))
                    return Unauthorized("Invalid credentials");
                var token = _jwtHelper.GenerateToken(banquier.IdBanquier.ToString(), banquier.Email, role);
                return Ok(new { banquier.IdBanquier, banquier.Nom, banquier.Email, role, token });
            }
            else
            {
                return BadRequest("Invalid role");
            }
        }
    }
}
