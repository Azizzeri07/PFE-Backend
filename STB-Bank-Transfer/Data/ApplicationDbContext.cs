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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- Keep your existing configurations ---
            modelBuilder.Entity<Banquier>()
                .Property(b => b.Role)
                .HasConversion<string>();
            modelBuilder.Entity<Client>()
                .Property(c => c.Role)
                .HasConversion<string>();

            modelBuilder.Entity<Client>()
                .HasOne(c => c.Banquier)
                .WithMany(b => b.Clients)
                .HasForeignKey(c => c.BanquierId)
                .IsRequired();

            // This defines the one-to-one relationship between Client and Compte
            modelBuilder.Entity<Client>()
                .HasOne(client => client.Compte) // A Client has one Compte
                .WithOne() // The Compte is related to one Client (no navigation property back from Compte)
                .HasForeignKey<Client>(client => client.IdCompte); // The FK is the 'IdCompte' property on the Client model.
                                                                   // EF Core will map this to the 'CompteIdCompte' column by convention.
        }

    }

    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Only seed admin, do not run migrations here
            if (!context.Banquiers.Any(b => b.Email == "admin@stb.com"))
            {
                var admin = new Banquier
                {
                    Nom = "Admin",
                    Email = "admin@stb.com",
                    MotDePasse = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    Role = UserRole.Admin
                };
                context.Banquiers.Add(admin);
                context.SaveChanges();
            }
        }
    }
}