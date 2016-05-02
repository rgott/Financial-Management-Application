using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Financial_Management_Application.Models;
using Financial_Management_Application.Identity;
using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;

namespace Financial_Management_Application.Controllers
{
    [Authorize]
    public class ManageController : ControllerModel
    {

        public ManageController()
        {
        }

        public ManageController(AccountUserManager userManager)
        {
            UserManager = userManager;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }
        //
        // GET: /Manage/Index
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
                : message == ManageMessageId.Error ? "An error has occurred."
                : message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
                : message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
                : "";

            FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF();
            long notification = db_manager.Notifications.LongCount();
            db_manager.Dispose();

            var userId = long.Parse(User.Identity.GetUserId());

            var model = new IndexViewModel
            {
                HasPassword = HasPassword(),
                PhoneNumber = await UserManager.GetPhoneNumberAsync(userId),
                TwoFactor = await UserManager.GetTwoFactorEnabledAsync(userId),
                Logins = await UserManager.GetLoginsAsync(userId),
                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId.ToString()),
                Notifications = notification
            };


            return View(model);
        }

        ////
        //// POST: /Manage/RemoveLogin
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        //{
        //    ManageMessageId? message;
        //    var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
        //    if (result.Succeeded)
        //    {
        //        var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
        //        if (user != null)
        //        {
        //            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
        //        }
        //        message = ManageMessageId.RemoveLoginSuccess;
        //    }
        //    else
        //    {
        //        message = ManageMessageId.Error;
        //    }
        //    return RedirectToAction("ManageLogins", new { Message = message });
        //}

        ////
        //// GET: /Manage/AddPhoneNumber
        //public ActionResult AddPhoneNumber()
        //{
        //    return View();
        //}

        ////
        //// POST: /Manage/AddPhoneNumber
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }
        //    // Generate the token and send it
        //    var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), model.Number);
        //    if (UserManager.SmsService != null)
        //    {
        //        var message = new IdentityMessage
        //        {
        //            Destination = model.Number,
        //            Body = "Your security code is: " + code
        //        };
        //        await UserManager.SmsService.SendAsync(message);
        //    }
        //    return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.Number });
        //}

        ////
        //// POST: /Manage/EnableTwoFactorAuthentication
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> EnableTwoFactorAuthentication()
        //{
        //    await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), true);
        //    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
        //    if (user != null)
        //    {
        //        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
        //    }
        //    return RedirectToAction("Index", "Manage");
        //}

        ////
        //// POST: /Manage/DisableTwoFactorAuthentication
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> DisableTwoFactorAuthentication()
        //{
        //    await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), false);
        //    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
        //    if (user != null)
        //    {
        //        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
        //    }
        //    return RedirectToAction("Index", "Manage");
        //}

        ////
        //// GET: /Manage/VerifyPhoneNumber
        //public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
        //{
        //    var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), phoneNumber);
        //    // Send an SMS through the SMS provider to verify the phone number
        //    return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        //}

        ////
        //// POST: /Manage/VerifyPhoneNumber
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }
        //    var result = await UserManager.ChangePhoneNumberAsync(User.Identity.GetUserId(), model.PhoneNumber, model.Code);
        //    if (result.Succeeded)
        //    {
        //        var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
        //        if (user != null)
        //        {
        //            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
        //        }
        //        return RedirectToAction("Index", new { Message = ManageMessageId.AddPhoneSuccess });
        //    }
        //    // If we got this far, something failed, redisplay form
        //    ModelState.AddModelError("", "Failed to verify phone");
        //    return View(model);
        //}

        ////
        //// GET: /Manage/RemovePhoneNumber
        //public async Task<ActionResult> RemovePhoneNumber()
        //{
        //    var result = await UserManager.SetPhoneNumberAsync(User.Identity.GetUserId(), null);
        //    if (!result.Succeeded)
        //    {
        //        return RedirectToAction("Index", new { Message = ManageMessageId.Error });
        //    }
        //    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
        //    if (user != null)
        //    {
        //        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
        //    }
        //    return RedirectToAction("Index", new { Message = ManageMessageId.RemovePhoneSuccess });
        //}

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(getUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(getUserId());
                if (user != null)
                {
                    await UserManager.FindAsync(user.Email,model.NewPassword);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }
        
        //
        // GET: /Manage/SetPassword
        public ActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.AddPasswordAsync(getUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(getUserId());
                    if (user != null)
                    {
                        await UserManager.FindAsync(user.Email,model.NewPassword);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult Notify()
        {
            FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF();
            var notifyListDB = db_manager.Notifications.ToList();

            // get division list
            List<SelectListItem> RolesList = new List<SelectListItem>();
            new SelectList(db_manager.Addresses, "Id", "addressLine1");
            foreach (var item in db_manager.Roles.ToList())
            {
                RolesList.Add(new SelectListItem()
                {
                    Text = item.Name,
                    Value = item.Id.ToString() //  will be used to get id later
                });
            }
            long? roleResult = db_manager.Roles.FirstOrDefault(m => m.Name == AppSettings.Roles.APPROVEDUSER).Id;
            db_manager.Dispose();
            Session.Add("notifyListDB", notifyListDB);
            Session.Add("roleResult", roleResult);
            Session.Add("RolesList", RolesList);
            return View(new NotifyViewModel()
            {
                Role = roleResult,
                Roles = RolesList,
                notifyList = notifyListDB
            });
        }

        [HttpPost]
        public ActionResult Notify(NotifyViewModel model, string submitButton, string id)
        {
            if(model.Role == null)
            {
                return View(model); // redisplay the view if error
            }
            long role = (long)model.Role;
            model.notifyList = (List<Notification>)Session["notifyListDB"];
            model.Role = (long)Session["roleResult"];
            model.Roles = (List<SelectListItem>)Session["RolesList"];
            bool continueRemove = true;
            long result;
            if(!long.TryParse(id, out result))
                return View(model);

            FM_Datastore_Entities_EF db_model = new FM_Datastore_Entities_EF();

            // get notification
            Notification oldNotify = db_model.Notifications.FirstOrDefault(m => m.Id == result);
            switch (submitButton)
            {
                case "Accept":
                    //send email to new user
                    Mail.send(
                        oldNotify.Email,
                        "Access Approved",
                        "here is the link to sign up this link will only be available for so long - "
                            + Request.Url.GetLeftPart(UriPartial.Authority)
                            + Url.Action("Register", "Account")
                            + "?rqst="
                            + UrlEncryption.Encrypt(
                                DateTime.UtcNow,
                                oldNotify.Email,
                                oldNotify.AddressId,
                                oldNotify.DivisionId,
                                role));
                    break;
                case "Deny":
                    // send denial email to user
                    Mail.send(oldNotify.Email,"Denied Access", "Appologies user you have been denied access by administration to the application.");
                    break;
                default:
                    break;
            }

            if (continueRemove)
            {
                model.notifyList.Remove(model.notifyList.First(m => m.Id == result)); // remove from current model
                db_model.Notifications.Remove(oldNotify);
            }
            db_model.SaveChanges();
            db_model.Dispose();
            return View(model);
        }

        //
        // GET: /Manage/ManageLogins
        public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            var user = await UserManager.FindByIdAsync(getUserId());
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await UserManager.GetLoginsAsync(getUserId());
            var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
            ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
            return View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        //
        // POST: /Manage/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
        }


        /// <summary>
        /// This method can only be run once to initialize the applications database with an admin to login
        /// </summary>
        /// <returns>not found if more then 0 records are in the database otherwise returns to login page on successful completion</returns>
        [AllowAnonymous]
        public ActionResult Setup()
        {
            // dispose of quickly
            using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
            {
                if (db_manager.Users.Count() != 0)
                {
                    return new HttpNotFoundResult();
                }

                //remove after databaseModel.sql
                if(db_manager.Roles.Count() == 0)
                {
                    foreach (var item in AppSettings.Roles.ComboBox)
                    {
                        db_manager.Roles.Add(new Role() { Name = item.Value });
                    }
                }

            }
            if (TempData.ContainsKey("Setup") && Session["SetupUser"] == null)
            {
                return new HttpNotFoundResult();
            }
            if(!TempData.Keys.Contains("Setup"))
            {
               TempData.Add("Setup", true);
            }
            Session.Add("SetupUser", true); 
            return View(new SetupViewModel()
            {
                Role = AppSettings.Roles.CONGRESS
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Setup(SetupViewModel model)
        {
            if (ModelState.IsValid)
            {
                // dispose of quickly
                using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
                {
                    if (db_manager.Users.Count() != 0)
                    {
                        return new HttpNotFoundResult();
                    }
                }
                if (TempData.ContainsKey("Setup") && Session["SetupUser"] == null)
                {
                    return new HttpNotFoundResult();
                }
                Address address;
                Division division;
                using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
                {
                    address = db_manager.Addresses.Add(model.Address);
                    division = db_manager.Divisions.Add(model.Division);
                    foreach (var item in AppSettings.Roles.ComboBox)
                    {
                        db_manager.Roles.Add(new Role() { Name = item.Value });
                    }
                    try
                    {
                        db_manager.SaveChanges();
                    }
                    catch (DbEntityValidationException dbEx)
                    {
                        foreach (var validationErrors in dbEx.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                            }
                        }
                    }
                }
                AccountUser user = new AccountUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Address = address.Id,
                    Division = division.Id,
                    TimeZoneOffset = DateTime.UtcNow,// TODO: change to hours of offset
                    CreationDate = DateTime.UtcNow
                };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // create or add role
                    UserManager.AddToRole(UserManager.FindByEmail(model.Email).Id, AppSettings.Roles.CONGRESS);

                    await SignInAsync(user, false);

                    return RedirectToAction("Index", "Home"); // redirect to user creation
                }
                AddErrors(result);
                return View(model);
            }
            return Setup();
        }


        //
        // GET: /Manage/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(getUserId(), loginInfo.Login);
            return result.Succeeded ? RedirectToAction("ManageLogins") : RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
        }
    }
}