using Financial_Management_Application.Models;
using Financial_Management_Application.Models.TransactionVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Financial_Management_Application.Controllers
{
    public class TransactionController : ControllerModel
    {
        public ActionResult RequestTransaction()
        {
            List<Product>ProductTable; 
            using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
            {
                ProductTable = db_manager.Products.Include(AppSettings.Includes.Category).ToList(); 
            }

            return View(new RequestTransactionViewModel()
            {
                ProductTable = ProductTable,
                SelectedProductTable = new List<Transaction>()
            });
        }
        static long getRandomLong(Random rand)
        {
            byte[] buf = new byte[8]; // size of long
            rand.NextBytes(buf);
            return BitConverter.ToInt64(buf, 0);
        }


        public static long getCartId()
        {
            long randCartId;
            // create cart ID
            using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
            {
                Random random = new Random(DateTime.Now.Millisecond);
                Transaction cartId;
                do
                {
                    randCartId = getRandomLong(random);
                    cartId = db_manager.Transactions.FirstOrDefault(m => m.cartId == randCartId);
                }
                while (cartId != null); // if false cart id is valid

                return randCartId;
            }
        }

        // NOTE:TODO: create another view for requester users

        [HttpPost]
        public ActionResult RequestTransaction(RequestTransactionViewModel model, string productId)
        {
            long parsed;
            if(productId == null || !long.TryParse(productId, out parsed))
            {
                return new HttpNotFoundResult();
            }
            model.productId = parsed;

            // get unit price from database
            decimal unitPrice;
            using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
            {
                unitPrice = db_manager.Products.FirstOrDefault(m => m.Id == model.productId).price;
            }

            // creat transaction
            Transaction transaction = new Transaction()
            {
                cartId = getCartId(),
                purchaseDate = DateTime.UtcNow,
                requestedForUserId = null,
                quantity = model.quantity,
                unitPrice = unitPrice,
                productId = model.productId,
                isDeleted = false // on creating transaction it starts as not deleted
            };

            SessionSaver.Add.transaction(Session, transaction); // add to session


            List<Product> ProductTable;
            using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
            {
                ProductTable = db_manager.Products.Include(AppSettings.Includes.Category).ToList();
            }

            model.ProductTable = ProductTable;
            var transactions = SessionSaver.Load.transactions(Session);
            model.SelectedProductTable = transactions;

            

            return View(model);
        }


        public ActionResult Index()
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