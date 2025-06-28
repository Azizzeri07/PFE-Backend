using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Sockets;

namespace STB_Bank_Transfer.Models
{
    public enum StatutVirement
    {
        EN_ATTENTE,
        APPROUVE,
        REJETE
    }

    public class Virement
    {
        [Key]
        public int IdVirement { get; set; }
        public string NumCompteSource { get; set; }
        public string NumCompteDestination { get; set; }
        public decimal Montant { get; set; }
        public string Motif { get; set; }
        public DateTime DateCreation { get; set; }
        public DateTime? DateValidation { get; set; }
        public StatutVirement Statut { get; set; }
        public string RaisonRejet { get; set; }

        public int IdCompte { get; set; }

        [ForeignKey("IdCompte")]
        public Compte Comptes { get; set; }

        [ForeignKey("Client")]
        public int ClientId { get; set; }

        public Client Client { get; set; }


    }
}