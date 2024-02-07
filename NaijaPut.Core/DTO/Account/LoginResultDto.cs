namespace NaijaPut.Core.DTO.Account
{
    public class LoginResultDto
    {
        public string Jwt { get; set; }
        public IList<string> UserRole { get; set; }
    }
}
