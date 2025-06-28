using System.ComponentModel.DataAnnotations;

namespace STB_Bank_Transfer.Models
{
    public class RegisterBanquier
    {
        [Required(ErrorMessage = "Le nom est obligatoire")]
        [StringLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères")]
        public string Nom { get; set; }

        [Required(ErrorMessage = "L'email est obligatoire")]
        [EmailAddress(ErrorMessage = "Format d'email invalide")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Le mot de passe est obligatoire")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Le mot de passe doit contenir entre 8 et 100 caractères")]
        [DataType(DataType.Password)]
        public string MotDePasse { get; set; }

        [Required(ErrorMessage = "La confirmation du mot de passe est obligatoire")]
        [Compare("MotDePasse", ErrorMessage = "Les mots de passe ne correspondent pas")]
        [DataType(DataType.Password)]
        public string ConfirmMotDePasse { get; set; }

        // Le rôle sera défini par l'administrateur ou le système
        public string Role { get; set; } = UserRoles.Banquier;
    }
}