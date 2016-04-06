using Financial_Management_Application.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Financial_Management_Application.Identity
{
    public class ApplicationRoleManager : RoleManager<AccountRole, long>
    {
        public ApplicationRoleManager(IRoleStore<AccountRole, long> roleStore)
            : base(roleStore)
        {
        }

        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            return new ApplicationRoleManager(new RoleStore<AccountRole, long, AccountUserRole>(context.Get<ApplicationDbContext>()));
        }
    }
}