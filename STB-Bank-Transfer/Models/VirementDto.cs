using System;

namespace STB_Bank_Transfer.Models
{
    public class VirementDto
    {
        public int IdVirement { get; set; }
        public string NumCompteSource { get; set; }
        public string NumCompteDestination { get; set; }
        public decimal Montant { get; set; }
        public string Motif { get; set; }
        public DateTime DateCreation { get; set; }
        public DateTime? DateValidation { get; set; }
        public string Statut { get; set; }
        public string RaisonRejet { get; set; }
    }
}
