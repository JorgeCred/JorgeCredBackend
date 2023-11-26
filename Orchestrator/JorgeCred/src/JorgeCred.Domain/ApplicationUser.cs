using Microsoft.AspNetCore.Identity;

namespace JorgeCred.Domain
{
    public class ApplicationUser : IdentityUser
    {
        // EF RELATED:
        public Account? Account { get; set; }
        public string PushNotificationAddress { get; set; }
    }
}
