using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace STB_Bank_Transfer.Models
{


    public class Banquier
    {
        [Key]  // This attribute marks the property as primary key
        public int IdBanquier { get; set; }
        public string Nom { get; set; }
        public string Email { get; set; }
        public string MotDePasse { get; set; }
        public UserRole Role { get; set; } = UserRole.Banquier;

        // Navigation property for clients created by this banker
        public List<Client> Clients { get; set; } = new List<Client>();

        // Navigation property for pending transfers
        public List<Virement> VirementsEnAttente { get; set; } = new List<Virement>();
    }
}