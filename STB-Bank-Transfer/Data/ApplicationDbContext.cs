using Microsoft.EntityFrameworkCore;
using STB_Bank_Transfer.Models;

namespace STB_Bank_Transfer.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Ne gardez que les entités qui doivent être persistées
        public DbSet<Banquier> Banquiers { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Compte> Comptes { get; set; }
        public DbSet<Virement> Virements { get; set; }
        public DbSet<Operation> Operations { get; set; }

        // SUPPRIMEZ ces lignes - ce ne sont pas des entités persistantes
        // public DbSet<RegisterBanquier> registerBanquiers { get; set; }
        // public DbSet<Login> logins { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuration de la relation one-to-one entre Client et Compte
            modelBuilder.Entity<Client>()
                .HasOne(c => c.CompteS)
                .WithOne(c => c.Client)
                .HasForeignKey<Compte>(c => c.ClientId) // La clé étrangère est dans Compte
                .OnDelete(DeleteBehavior.Cascade); // Ou Restrict selon vos besoins

            // Configuration de la relation entre Client et Banquier
            modelBuilder.Entity<Client>()
                .HasOne(c => c.Banquier)
                .WithMany() // Supposant que Banquier a une propriété List<Client> Clients
                .HasForeignKey(c => c.BanquierId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuration de la relation entre Compte et Banquier
            modelBuilder.Entity<Compte>()
                .HasOne(c => c.Banquier)
                .WithMany() // Si Banquier n'a pas de collection de Comptes
                .HasForeignKey(c => c.BanquierId)
                .OnDelete(DeleteBehavior.Restrict);
        
        // Configure Banquier relationships
        modelBuilder.Entity<Compte>()
                .HasOne(c => c.Banquier)
                .WithMany()
                .HasForeignKey(c => c.BanquierId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Client>()
                .HasOne(c => c.Banquier)
                .WithMany()
                .HasForeignKey(c => c.BanquierId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Banquier>(entity =>
            {
                entity.HasKey(b => b.IdBanquier);
                entity.Property(b => b.Nom).IsRequired().HasMaxLength(100);
                entity.Property(b => b.Email).IsRequired();
                entity.Property(b => b.MotDePasse).IsRequired();
                entity.HasIndex(b => b.Email).IsUnique();
            });
        }
    }
}