using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Financial_Management_Application
{
    public static class ApplicationSettings
    {
        public const string CONGRESS = "Congress"; 
        public const string AUDITORS = "Auditors"; 
        public const string ADMIN = "Administrators"; 
        public const string MANAGER = "Manager"; 
        public const string SUPERVISOR = "Supervisors"; 
        public const string PURCHASINGAGENT = "PurchasingAgent"; 
        public const string APPROVEDUSER = "AprovedUser";
         
        public enum RoleTypes
        {
            Congress,
            Auditors,
            Administrators,
            Manager,
            Supervisors,
            PurchasingAgent,
            ApprovedUser
        }

        public static string getString(RoleTypes roles)
        {
            switch (roles)
            {
                case RoleTypes.PurchasingAgent:
                    return "Purchasing Agent";
                case RoleTypes.Auditors:
                    return "Auditors";
                case RoleTypes.Administrators:
                    return "Administrators";
                case RoleTypes.Manager:
                    return "Manager";
                case RoleTypes.Supervisors:
                    return "Supervisors";
                case RoleTypes.Congress:
                    return "Congress";
                case RoleTypes.ApprovedUser:
                default:
                    return "Approved User";
            }
        }
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
            },
            new SelectListItem()
            {
                Text = "Congress",
                Value = "Congress"
            }
        };
    }
}