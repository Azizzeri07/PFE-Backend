// Data/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using STB_Bank_Transfer.Models;

namespace STB_Bank_Transfer.Data  // Changé de MyApiProject.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Banquier> Banquiers { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Compte> Comptes { get; set; }
        public DbSet<Virement> Virements { get; set; }
        public DbSet<Operation> Operations { get; set; }
    }
}