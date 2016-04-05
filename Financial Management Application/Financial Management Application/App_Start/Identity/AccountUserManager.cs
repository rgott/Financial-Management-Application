namespace Financial_Management_Application.Identity
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.Owin;

    public class AccountUserManager : UserManager<AccountUser, long>
    {
        #region constructors and destructors

        public AccountUserManager(IUserStore<AccountUser, long> store) : base(store)
        {
        }

        #endregion

        #region methods

        public static AccountUserManager Create(IdentityFactoryOptions<AccountUserManager> options, IOwinContext context)
        {
            var manager = new AccountUserManager(new UserStore<AccountUser, AccountRole, long, AccountLogin, AccountUserRole, AccountClaim>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<AccountUser, long>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };
            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug in here.
            //manager.RegisterTwoFactorProvider(
            //    "PhoneCode",
            //    new PhoneNumberTokenProvider<MyUser, long>
            //    {
            //        MessageFormat = "Your security code is: {0}"
            //    });
            //manager.RegisterTwoFactorProvider(
            //    "EmailCode",
            //    new EmailTokenProvider<MyUser, long>
            //    {
            //        Subject = "Security Code",
            //        BodyFormat = "Your security code is: {0}"
            //    });
            //manager.EmailService = new EmailService();
            //manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<AccountUser, long>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }

        #endregion
    }
}