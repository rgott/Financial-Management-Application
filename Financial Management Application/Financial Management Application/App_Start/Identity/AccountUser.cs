namespace Financial_Management_Application.Identity
{
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    public class AccountUser : IdentityUser<long, AccountLogin, AccountUserRole, AccountClaim>
    {
        public string PasswordAnswer { get; set; }
        public string PasswordQuestion { get; set; }
        public long Address { get; set; }
        public long Division { get; set; }
        public DateTime TimeZoneOffset { get; set; }
        public DateTime ExpireDate { get; set; }
        public DateTime CreationDate { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(AccountUserManager userManager)
        {
            var userIdentity = await userManager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}
