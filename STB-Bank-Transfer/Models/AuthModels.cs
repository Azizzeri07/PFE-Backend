namespace STB_Bank_Transfer.Models
{
    public class RegisterModel
    {
        public string Nom { get; set; }
        public string Email { get; set; }
        public string MotDePasse { get; set; }
        public string Role { get; set; }
    }

    public class LoginModel
    {
        public string Email { get; set; }
        public string MotDePasse { get; set; }
        public string Role { get; set; }
    }
}
