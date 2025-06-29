using System.Collections.Generic;
using System.Linq;
using STB_Bank_Transfer.Models;

namespace STB_Bank_Transfer.Services
{
    public static class VirementMapper
    {
        public static VirementDto ToDto(Virement virement)
        {
            return new VirementDto
            {
                IdVirement = virement.IdVirement,
                NumCompteSource = virement.NumCompteSource,
                NumCompteDestination = virement.NumCompteDestination,
                Montant = virement.Montant,
                Motif = virement.Motif,
                DateCreation = virement.DateCreation,
                DateValidation = virement.DateValidation,
                Statut = virement.Statut.ToString(),
                RaisonRejet = virement.RaisonRejet
            };
        }

        public static List<VirementDto> ToDtoList(IEnumerable<Virement> virements)
        {
            return virements.Select(ToDto).ToList();
        }
    }
}
