using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Financial_Management_Application
{
    public static class AppSettings
    {
        // session variables
        public static class SessionVariables
        {
            public const string CATEGORY = "category";
            public const string PRODUCT = "products";
            public const string CATEGORYCOMBOBOX = "categorycombobox";
            public const string PRODUCTCOMBOBOX = "productcombobox";
            public const string TRANSACTION = "transaction";
        }

        // Includes for databases
        public static class Includes
        {
            public const string Category = "Category";
            public const string Product = "Product";
        }

        // Roles
        public static class Roles
        {
            public const string CONGRESS = "Congress";
            public const string AUDITORS = "Auditors";
            public const string ADMIN = "Administrators";
            public const string MANAGER = "Manager";
            public const string SUPERVISOR = "Supervisors";
            public const string PURCHASINGAGENT = "Purchasing Agent";
            public const string APPROVEDUSER = "Approved User";

            public static readonly List<SelectListItem> ComboBox = new List<SelectListItem>()
            {
                new SelectListItem()
                {
                    Text = "Auditors",
                    Value = "Auditors"
                },
                new SelectListItem()
                {
                    Text = "Administrator",
                    Value = "Administrator"
                },
                new SelectListItem()
                {
                    Text = "Manager",
                    Value = "Manager"
                },
                new SelectListItem()
                {
                    Text = "Supervisors",
                    Value = "Supervisors"
                },
                new SelectListItem()
                {
                    Text = "Purchasing Agent",
                    Value = "Purchasing Agent"
                },
                new SelectListItem()
                {
                    Text = "Approved User",
                    Value = "Approved User"
                },
                new SelectListItem()
                {
                    Text = "Congress",
                    Value = "Congress"
                }
            };
        }

        public static class Notify
        {
            public const string newUser = "newUser";
            public const string pendingUser = "pendingUser";
            public const string pendingTransactionRequest = "Pending Transaction Request";

        }

        public static class Mail
        {
            public const string displayName = "Gold Team Technologies";
            public const string email = "GoldTeamTechnologies@gmail.com";
            public const string password = "GoldTeam1";
        }

        public static class ComboboxLists
        {
            public static readonly List<SelectListItem> Countries = new List<SelectListItem>()
            {
                new SelectListItem()
                {
                    Text = "United States",
                    Value = "United States"
                }                
            };

            public class Countires
            {
                public class UnitedStates
                {
                    public static readonly List<SelectListItem> Countries = new List<SelectListItem>()
                    {
                        new SelectListItem()
                        {
                            Text = "Maryland",
                            Value = "United States"
                        }
                    };
                }
            }
        }

    }
}