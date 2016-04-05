namespace Financial_Management_Application.Identity
{
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;

    public class AccountUser : IdentityUser<long, AccountLogin, AccountUserRole, AccountClaim>
    {
        #region properties

        public string ActivationToken { get; set; }

        public string PasswordAnswer { get; set; }

        public string PasswordQuestion { get; set; }

        #endregion

        #region methods

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(AccountUserManager userManager)
        {
            var userIdentity = await userManager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        #endregion
    }
}
