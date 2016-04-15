using Financial_Management_Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Financial_Management_Application.Controllers
{
    public class TransactionController : ControllerModel
    {
        const string TransactionSessionVar = "transactions";

        public ActionResult RequestTransaction()
        {
            return View(); 
        }

        ///// <summary>
        ///// Querys database for transactions and 
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult ViewTransaction()
        //{
        //    List<Transaction> transactions;

        //    if (Session[TransactionSessionVar] == null || !(Session[TransactionSessionVar] is List<Transaction>))
        //    {
        //        using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
        //        {
        //            transactions = db_manager.Transactions.ToList();
        //        }
        //        Session.Add(TransactionSessionVar, transactions);
        //    }
        //    else
        //    {
        //        transactions = (List<Transaction>)Session[TransactionSessionVar];
        //    }

        //    return View(new TViewViewModel()
        //    {
        //        transactions = transactions
        //    });
        //}
    }
}