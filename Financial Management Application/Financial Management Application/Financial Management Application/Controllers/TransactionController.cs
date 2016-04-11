using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Financial_Management_Application.Controllers
{
    public class TransactionController : Controller
    {
        public ActionResult Request()
        {
            return View(); 
        }
    }
}