using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace STB_Bank_Transfer.Models
{
    public class Operation
    {
        [Key]
        public int IdOperation { get; set; }
        public string NumCompte { get; set; }
        public decimal Montant { get; set; }
        public DateTime DateOperation { get; set; }
        public string Libelle { get; set; }
        public string TypeOperation { get; set; }

        public int CompteId { get; set; } // Should be int, not string

        [ForeignKey("CompteId")]
        public Compte Compte { get; set; }
    }
}
