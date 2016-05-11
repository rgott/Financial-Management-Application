using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;

namespace Financial_Management_Application.Models.ReportVM
{
    public class ReportItemsViewModel
    {
        public string transactions { get; set; }
        public string products { get; set; }
        public string categories { get; set; }
    }
}