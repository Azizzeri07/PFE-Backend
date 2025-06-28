using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace STB_Bank_Transfer.Models
{
    public class Client
    {
        [Key]
        public int IdClient { get; set; }

        [Required]
        [StringLength(100)]
        public string Nom { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        public string MotDePasse { get; set; }
        public string Role { get; set; } = UserRoles.Client;

        public void SetPassword(string password)
        {
            MotDePasse = BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, MotDePasse);
        }


        public int BanquierId { get; set; }

        [ForeignKey("BanquierId")]
        public Banquier Banquier { get; set; }

        public int IdCompte { get; set; }

        [ForeignKey("IdCompte")]
        public Compte CompteS { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Solde { get; set; }

        [Required]
        [StringLength(50)]
        public string TypeCompte { get; set; }



    }
}