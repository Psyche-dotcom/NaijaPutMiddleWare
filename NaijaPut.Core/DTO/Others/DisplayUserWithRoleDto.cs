namespace NaijaPut.Core.DTO.Others
{
    public class DisplayUserWithRoleDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string WalletId { get; set; }
        public decimal WalletBalance { get; set; }
        public string UserRole { get; set; }
    }
}
