using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace STB_Bank_Transfer.Models
{
    public class Banquier
    {
        [Key]
        public int IdBanquier { get; set; }
        public string Nom { get; set; }
        public string Email { get; set; }
        public string MotDePasse { get; set; }
        public string Role { get; set; } 


        public void SetPassword(string password)
        {
            MotDePasse = BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, MotDePasse);
        }


    }
}
