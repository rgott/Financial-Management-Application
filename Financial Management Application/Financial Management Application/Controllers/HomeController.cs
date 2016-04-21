﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Financial_Management_Application.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    
        [Authorize(Roles=ApplicationSettings.CONGRESS)]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            Session.Clear();
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

    }
}