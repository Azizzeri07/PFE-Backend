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
        public List<Virement> Virements { get; set; } = new List<Virement>();
    }
}
