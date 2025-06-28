using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace STB_Bank_Transfer.Models
{
    public class Compte
    {
        [Key]
        [Required]
        public int IdCompte { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Solde { get; set; }

        [Required]
        [StringLength(50)]
        public string TypeCompte { get; set; }

        // Relations
        [Required]
        [ForeignKey("Client")]
        public int ClientId { get; set; }

        public Client Client { get; set; }

        public int BanquierId { get; set; }

        [ForeignKey("BanquierId")]
        public Banquier Banquier { get; set; }

        // Méthodes métier
        public bool Debiter(decimal montant)
        {
            if (montant <= 0)
                return false;

            if (Solde >= montant)
            {
                Solde -= montant;
                return true;
            }
            return false;
        }

        public void Crediter(decimal montant)
        {
            if (montant > 0)
            {
                Solde += montant;
            }
        }

        internal bool Any(Func<object, bool> value)
        {
            throw new NotImplementedException();
        }
    }
}