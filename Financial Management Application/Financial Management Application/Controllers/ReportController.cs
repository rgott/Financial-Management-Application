using System;
using Financial_Management_Application.Models;
using Financial_Management_Application.Models.TransactionVM;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Financial_Management_Application.Controllers
{
    public class ReportController : Controller
    {
        // GET: Report
        public ActionResult barChart()
        {
            return View();
        }

        public JsonResult getSalesData()
        {
            List <Transaction> trans = new List<Transaction>();
            using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
            {
                trans = db_manager.Transactions.OrderBy(m => m.productId).ToList(); 
            }
            var chartData = new object[trans.Count + 1];
            chartData[0] = new object[]
            { 
                "productId",
                "quantity",  
            };
            int j = 0; 
            foreach (var i in trans)
            {
                j++;
                chartData[j] = new object[] { i.productId, i.quantity };
            }
            return new JsonResult { Data = chartData, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}