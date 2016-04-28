using Financial_Management_Application.Identity;
using Financial_Management_Application.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Financial_Management_Application
{
    public abstract class ControllerModel : Controller
    {
        private AccountUserManager _userManager;
        protected AccountUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<AccountUserManager>();
            }
            set
            {
                _userManager = value;
            }
        }

        private ApplicationRoleManager _roleManager;
        protected ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationRoleManager>();
            }
            set
            {
                _roleManager = value;
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }
        public class SessionSaver<T>
        {
            #region basic methods
            public delegate void useSessionFunc(out T savedObject);

            /// <summary>
            /// Ensures the right type is set and returned from session
            /// </summary>
            /// <param name="Session"></param>
            /// <param name="sessionVarName"></param>
            /// <param name="methodsetObject">method where the T value is set to the session with key param:sessionVarName. If null then automatically loads value from storage</param>
            /// <returns></returns>
            public T use(HttpSessionStateBase Session, string sessionVarName, useSessionFunc methodsetObject)
            {
                T savedObject = default(T);
                object sessionVar = Session[sessionVarName];

                if ((sessionVar == null || !(sessionVar.GetType() == typeof(T))) && methodsetObject != null)
                {
                    methodsetObject(out savedObject); // set saved to value
                    Session.Add(sessionVarName, savedObject);
                }
                else
                    savedObject = (T)Session[sessionVarName];

                return savedObject;
            }

            public T use(ViewDataDictionary ViewData, string sessionVarName, useSessionFunc methodsetObject)
            {
                T savedObject = default(T);
                object sessionVar = ViewData[sessionVarName];

                if ((sessionVar == null || !(sessionVar.GetType() == typeof(T))) && methodsetObject != null)
                {
                    methodsetObject(out savedObject); // set saved to value
                    ViewData.Add(sessionVarName, savedObject);
                }
                else
                    savedObject = (T)ViewData[sessionVarName];

                return savedObject;
            }

            internal List<SelectListItem> use(HttpSessionStateBase session, object productSessionVar, object p)
            {
                throw new NotImplementedException();
            }
            #endregion
        }
        public static class SessionSaver
        {
            public static class Load
            {
                public static List<Category> categories(HttpSessionStateBase Session)
                {
                    return new SessionSaver<List<Category>>().use(Session, AppSettings.SessionVariables.CATEGORY, (out List<Category> saveobject) =>
                    {
                        using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
                        {
                            saveobject = db_manager.Categories.ToList();
                        }
                    });
                }
                public static List<SelectListItem> categoriesCombobox(HttpSessionStateBase Session)
                {
                    return new SessionSaver<List<SelectListItem>>().use(Session, AppSettings.SessionVariables.CATEGORYCOMBOBOX, (out List<SelectListItem> saveobject) =>
                    {
                        List<SelectListItem> CategoriesList = new List<SelectListItem>();
                        using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
                        {
                            foreach (var item in db_manager.Categories.ToList())
                            {
                                CategoriesList.Add(new SelectListItem()
                                {
                                    Text = item.name,
                                    Value = item.Id.ToString() //  will be used to get id later
                                });
                            }
                        }
                        saveobject = CategoriesList;
                    });
                }

                public static List<Category> products(HttpSessionStateBase Session)
                {
                    return new SessionSaver<List<Category>>().use(Session, AppSettings.SessionVariables.CATEGORY, (out List<Category> saveobject) =>
                    {
                        using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
                        {
                            saveobject = db_manager.Categories.ToList();
                        }
                    });
                }
                public static List<SelectListItem> productsCombobox(HttpSessionStateBase Session)
                {
                    return new SessionSaver<List<SelectListItem>>().use(Session, AppSettings.SessionVariables.PRODUCTCOMBOBOX, (out List<SelectListItem> saveobject) =>
                    {
                        List<SelectListItem> ProductsList = new List<SelectListItem>();
                        using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
                        {
                            foreach (var item in db_manager.Products.ToList())
                            {
                                ProductsList.Add(new SelectListItem()
                                {
                                    Text = item.name,
                                    Value = item.Id.ToString() //  will be used to get id later
                                });
                            }
                        }
                        saveobject = ProductsList;
                    });
                }
            }
        }

        #region Helpers

        #region Model

        // Used for XSRF protection when adding external logins
        protected const string XsrfKey = "XsrfId";

        public long getUserId()
        {
            return long.Parse(User.Identity.GetUserId());
        }

        protected IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        protected void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        protected ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        protected async Task SignInAsync(AccountUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            AuthenticationManager.SignIn(
                new AuthenticationProperties
                {
                    IsPersistent = isPersistent
                },
                await user.GenerateUserIdentityAsync(UserManager));
        }
        #endregion
        
        #region Users

        protected bool HasPassword()
        {
            var user = UserManager.FindById(long.Parse(User.Identity.GetUserId()));
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        protected bool HasPhoneNumber()
        {
            var user = UserManager.FindById(long.Parse(User.Identity.GetUserId()));
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        #endregion

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        #endregion
    }
}