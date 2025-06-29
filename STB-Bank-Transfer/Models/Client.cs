using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace STB_Bank_Transfer.Models
{
    public class Client
    {
        [Key]
        public int IdClient { get; set; }
        public string Nom { get; set; }
        public string Email { get; set; }
        public string MotDePasse { get; set; }
        public UserRole Role { get; set; } = UserRole.Client;
        public List<Virement> Virements { get; set; } = new List<Virement>();
        public int? BanquierId { get; set; }
        public Banquier Banquier { get; set; }
        public string IdCompte { get; set; } // Foreign key to Compte
        public Compte Compte { get; set; } // Navigation property
    }

    public class CreateClientRequest
    {
        [Key]
        public string Nom { get; set; }
        public string Email { get; set; }
        public string MotDePasse { get; set; }
        public UserRole Role { get; set; } = UserRole.Client;
        public int BanquierId { get; set; } // Now required
        public string TypeCompte { get; set; } = "Courant"; // Default to Courant
    }
}
