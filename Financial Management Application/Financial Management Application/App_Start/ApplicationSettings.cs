using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Financial_Management_Application
{
    public static class ApplicationSettings
    {
        public static readonly List<SelectListItem> Roles = new List<SelectListItem>()
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
            }
        };
    }
}