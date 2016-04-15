using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Financial_Management_Application.Models;
using Financial_Management_Application.Identity;
using System.Net;
using System.Collections.Generic;
using System.Net.Mail;
using System.Collections.Specialized;
using System.Text;

namespace Financial_Management_Application.Controllers
{
    [Authorize]
    public class AccountController : ControllerModel
    {

        public AccountController()
        {
        }

        public AccountController(AccountUserManager userManager)
        {
            UserManager = userManager;
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindAsync(model.Email, model.Password);
                if (user != null)
                {
                    await SignInAsync(user, model.RememberMe);
                    return RedirectToLocal(returnUrl);
                }
                ModelState.AddModelError("", "Invalid username or password.");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        ////
        //// GET: /Account/VerifyCode
        //[AllowAnonymous]
        //public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        //{
        //    // Require that the user has already logged in via username/password or external login
        //    if (!await SignInManager.HasBeenVerifiedAsync())
        //    {
        //        return View("Error");
        //    }
        //    return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        //}

        ////
        //// POST: /Account/VerifyCode
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }

        //    // The following code protects for brute force attacks against the two factor codes. 
        //    // If a user enters incorrect codes for a specified amount of time then the user account 
        //    // will be locked out for a specified amount of time. 
        //    // You can configure the account lockout settings in IdentityConfig
        //    var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
        //    switch (result)
        //    {
        //        case SignInStatus.Success:
        //            return RedirectToLocal(model.ReturnUrl);
        //        case SignInStatus.LockedOut:
        //            return View("Lockout");
        //        case SignInStatus.Failure:
        //        default:
        //            ModelState.AddModelError("", "Invalid code.");
        //            return View(model);
        //    }
        //}

        // TODO: Move to own controller
        #region Registration
        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult RegisterRequest()
        {
            FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF();

            // get division list
            List<SelectListItem> divisionList = new List<SelectListItem>();
            new SelectList(db_manager.Addresses, "Id", "addressLine1");
            foreach (var item in db_manager.Divisions.ToList())
            {
                divisionList.Add(new SelectListItem()
                {
                    Text = item.name,
                    Value = item.Id.ToString() //  will be used to get id later
                });
            }

            // get address list
            List<SelectListItem> addressList = new List<SelectListItem>();
            foreach (var item in db_manager.Addresses.ToList())
            {
                addressList.Add(new SelectListItem()
                {
                    Text = item.city + ", " + item.state,
                    Value = item.Id.ToString() //  will be used to get id later
                });
            }
            db_manager.Dispose(); // no need to save changes
            return View(new RegisterRequestViewModel()
            {
                addresses = addressList,
                divisions = divisionList
            });
        }

        //
        // POST: /Account/Register
        [AllowAnonymous]
        [HttpPost]
        public ActionResult RegisterRequest(RegisterRequestViewModel model)
        {
            // TODO:deny multiple requests from the same ip address

            
            FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF();

            // TODO: make temporary encrypted url that holds => timeCreated,email,role,address,division
            char separator = (char)31; // ASCII char 31 is the separator character
            string urlParamStr = DateTime.UtcNow.Ticks.ToString() + separator 
                + model.Email + separator 
                + model.address.ToString() + separator 
                + model.division.ToString();
            string urlParamStrB64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(urlParamStr));
            
            // posts notification
            db_manager.Notifications.Add(new Notification()
            {
                notifyType = "newUser",
                notifyText = model.Email,
                Email = model.Email,
                Address = db_manager.Addresses.FirstOrDefault(m => m.Id == model.address),
                Division = db_manager.Divisions.FirstOrDefault(m => m.Id == model.division),
                timeStamp = DateTime.UtcNow
            });
            db_manager.SaveChanges();

            GmailMail.send(model.Email, "Request Recieved", "Dear user your request has been recieved and an administrator will be looking at your request soon, so please be patient.");
            return Redirect(Url.Action("RegisterRequestCompletion"));
        }

        [AllowAnonymous]
        public ActionResult RegisterRequestCompletion()
        {
            // if not redirected here from another ActionResult then this page doesnt exist
            if (Request.UrlReferrer == null) 
                return new HttpNotFoundResult();
            return View();
        }
        
        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            //Only allow people to view register page if they have a valid link
            NameValueCollection query = Request.QueryString;
            string[] qresult = query.GetValues("rqst");
            if (qresult == null || qresult.Length < 1)
                return new HttpNotFoundResult();

            UrlEncryption EncryptionResult = UrlEncryption.Decrypt(qresult[0]);
            if (EncryptionResult == null || EncryptionResult.timeStamp > DateTime.UtcNow.AddHours(3)) // if null or url was created more than 3 hours ago dont accept
                return new HttpNotFoundResult();

            FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF();
            Role RoleResult = db_manager.Roles.FirstOrDefault(m => m.Id == EncryptionResult.role);
            Address AddressResult = db_manager.Addresses.FirstOrDefault(m => m.Id == EncryptionResult.address);
            Division DivisionResult = db_manager.Divisions.FirstOrDefault(m => m.Id == EncryptionResult.division);
            db_manager.Dispose();

            // store ids in session
            Session.Add("RoleResult", RoleResult.Id);
            Session.Add("AddressResult", AddressResult.Id);
            Session.Add("DivisionResult", DivisionResult.Id);
            return View(new RegisterViewModel()
            {
                Email = EncryptionResult.email,
                Role = RoleResult.Name,
                Address = AddressResult.country + ": " 
                + AddressResult.addressLine1 
                + AddressResult.addressLine2 
                + ", " + AddressResult.city 
                + ", " + AddressResult.state
                + ", " + AddressResult.postalCode,
                Division = DivisionResult.name
            });
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                AccountUser user = new AccountUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Address = (long)Session["AddressResult"],
                    Division = (long)Session["DivisionResult"],
                    TimeZoneOffset = DateTime.UtcNow,// TODO: change to hours of offset
                    CreationDate = DateTime.UtcNow
                };

                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // create or add role
                    if (!RoleManager.RoleExists(model.Role))
                    {
                        var roleResult = await RoleManager.CreateAsync(new AccountRole() { Name = model.Role });
                        if (!roleResult.Succeeded)
                        {
                            ModelState.AddModelError("", "Error in creating account please contact administrator");
                            return View(model);
                        }
                    }
                    UserManager.AddToRole(UserManager.FindByEmail(model.Email).Id, model.Role);

                    await SignInAsync(user, false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }


            // If we got this far, something failed, redisplay form
            return View(model);
        }
        #endregion

        ////
        //// GET: /Account/ConfirmEmail
        //[AllowAnonymous]
        //public async Task<ActionResult> ConfirmEmail(string userId, string code)
        //{
        //    if (userId == null || code == null)
        //    {
        //        return View("Error");
        //    }
        //    var result = await UserManager.ConfirmEmailAsync(userId, code);
        //    return View(result.Succeeded ? "ConfirmEmail" : "Error");
        //}

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        ////
        //// GET: /Account/SendCode
        //[AllowAnonymous]
        //public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        //{
        //    var userId = await SignInManager.GetVerifiedUserIdAsync();
        //    if (userId == null)
        //    {
        //        return View("Error");
        //    }
        //    var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
        //    var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
        //    return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        //}

        ////
        //// POST: /Account/SendCode
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> SendCode(SendCodeViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View();
        //    }

        //    // Generate the token and send it
        //    if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
        //    {
        //        return View("Error");
        //    }
        //    return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        //}

        ////
        //// GET: /Account/ExternalLoginCallback
        //[AllowAnonymous]
        //public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        //{
        //    var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
        //    if (loginInfo == null)
        //    {
        //        return RedirectToAction("Login");
        //    }

        //    // Sign in the user with this external login provider if the user already has a login
        //    var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
        //    switch (result)
        //    {
        //        case SignInStatus.Success:
        //            return RedirectToLocal(returnUrl);
        //        case SignInStatus.LockedOut:
        //            return View("Lockout");
        //        case SignInStatus.RequiresVerification:
        //            return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
        //        case SignInStatus.Failure:
        //        default:
        //            // If the user does not have an account, then prompt the user to create an account
        //            ViewBag.ReturnUrl = returnUrl;
        //            ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
        //            return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
        //    }
        //}

        ////
        //// POST: /Account/ExternalLoginConfirmation
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        return RedirectToAction("Index", "Manage");
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        // Get the information about the user from the external login provider
        //        var info = await AuthenticationManager.GetExternalLoginInfoAsync();
        //        if (info == null)
        //        {
        //            return View("ExternalLoginFailure");
        //        }
        //        var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
        //        var result = await UserManager.CreateAsync(user);
        //        if (result.Succeeded)
        //        {
        //            result = await UserManager.AddLoginAsync(user.Id, info.Login);
        //            if (result.Succeeded)
        //            {
        //                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
        //                return RedirectToLocal(returnUrl);
        //            }
        //        }
        //        AddErrors(result);
        //    }

        //    ViewBag.ReturnUrl = returnUrl;
        //    return View(model);
        //}

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        
    }
}