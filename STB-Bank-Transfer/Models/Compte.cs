using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace STB_Bank_Transfer.Models
{
    public class Compte
    {
        [Key]
        public string IdCompte { get; set; }
        public decimal Solde { get; set; }
        public string TypeCompte { get; set; }
        public List<Operation> HistoriqueOperations { get; set; } = new List<Operation>();

        public bool Debiter(decimal montant)
        {
            if (Solde >= montant)
            {
                Solde -= montant;
                return true;
            }
            return false;
        }

        public void Crediter(decimal montant)
        {
            Solde += montant;
        }
    }
}