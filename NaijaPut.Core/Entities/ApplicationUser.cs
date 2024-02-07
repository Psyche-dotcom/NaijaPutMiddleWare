using Microsoft.AspNetCore.Identity;

namespace NaijaPut.Core.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool SuspendUser { get; set; }
        public ConfirmEmailToken ConfirmEmailToken { get; set; }
        public Wallet Wallet { get; set; }
    }
}
