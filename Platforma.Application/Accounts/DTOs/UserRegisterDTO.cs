namespace Platforma.Application.Accounts.DTOs
{
    public class UserRegisterDTO
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string RepeatedPassword { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
    }
}
