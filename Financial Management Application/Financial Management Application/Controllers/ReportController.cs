﻿using System;
using Financial_Management_Application.Models;
using Financial_Management_Application.Models.TransactionVM;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Financial_Management_Application.Models.ReportVM;
using System.Text;

namespace Financial_Management_Application.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        // GET: Report
        public ActionResult barChart()
        {
            List<Transaction> trans = new List<Transaction>();
            List<Product> prods = new List<Product>();
            List<Category> cats = new List<Category>();
            using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
            {
                trans = db_manager.Transactions.ToList();
                prods = db_manager.Products.ToList();
                cats = db_manager.Categories.ToList();
            }

            // transactions
            Dictionary<string, int> transData = new Dictionary<string, int>();
            foreach (var item in trans)
            {
                transData.Add(item.productId.ToString(), item.quantity);
            }
            string transactions = printJSArray("ProductID", "Quantity", transData);

            // products
            Dictionary<string, int> prodsData = new Dictionary<string, int>();
            foreach (var item in prods)
            {
                prodsData.Add(item.Id.ToString(), (int)item.price);
            }
            string products = printJSArray("ProductID", "Price", prodsData);

            // categories
            Dictionary<string, int> catsData = new Dictionary<string, int>();
            foreach (var item in cats)
            {
                catsData.Add(item.Id.ToString(), item.name.Length);
            }
            string categories = printJSArray("category", "name length", catsData);

            return View(new ReportItemsViewModel()
            {
                transactions = transactions,
                products = products,
                categories = categories
            });
        }
        /// <summary>
        /// Prints javascript array [['text', 'Text']['Name', data as int]]
        /// </summary>
        public static string printJSArray(string titleOfStr,string titleOfData, Dictionary<string,int> data)
        {
            StringBuilder jsScript = new StringBuilder(String.Format("[['{0}','{1}']",titleOfStr,titleOfData));
            foreach (var item in data)
            {
                jsScript.Append(String.Format(",['{0}',{1}]", item.Key, item.Value));
            }
            jsScript.Append("]");
            return jsScript.ToString();

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