using System;
using System.ComponentModel.DataAnnotations;
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
        public string TypeOperation { get; set; } // "Débit" ou "Crédit"

        internal static int GetNextOperationId()
        {
            throw new NotImplementedException();
        }
    }
}