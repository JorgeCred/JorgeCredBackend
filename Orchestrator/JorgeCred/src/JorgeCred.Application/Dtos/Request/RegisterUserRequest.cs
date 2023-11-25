namespace JorgeCred.Application.Dtos.Request
{
    public class RegisterUserRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordConfirmation { get; set; }
        public string RoleName { get; set; }
    }
}
