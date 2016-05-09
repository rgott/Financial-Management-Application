using Financial_Management_Application.Models;
using Financial_Management_Application.Models.TransactionVM;
using paypal = PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Financial_Management_Application.Controllers
{
    public class TransactionController : ControllerModel
    {
        /// <summary>
        /// Displays the transactions view as well as acts as post back to add items
        /// </summary>
        /// <param name="model"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        public ActionResult RequestTransaction(RequestTransactionViewModel model, string productId)
        {
            long parsed;
            if (productId != null && long.TryParse(productId, out parsed))
            {
                model.productId = parsed;

                // get unit price from database
                Product db_product;
                decimal db_unitPrice;
                using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
                {
                    db_unitPrice = db_manager.Products.FirstOrDefault(m => m.Id == model.productId).price;
                    db_product = db_manager.Products.Include(AppSettings.Includes.Category).FirstOrDefault(m => m.Id == model.productId);
                }

                // create transaction (still need to add cartId on checkout)
                Transaction transaction = new Transaction()
                {
                    purchaseDate = DateTime.UtcNow,
                    requestedForUserId = null,
                    quantity = model.quantity,
                    unitPrice = db_unitPrice,
                    productId = model.productId,
                    Product = db_product,
                    isDeleted = false // on creating transaction it starts as not deleted
                };

                SessionSaver.Add.transaction(Session, transaction); // add to session
            }
            else
            {
                model = new RequestTransactionViewModel();
            }

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

        /// <summary>
        /// Add post to request transaction
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RequestTransaction()
        {
            PayPalHelper.CreatePayment(PayPalHelper.apiContext, Url.Action(""),
            new PayPal.Api.Transaction()
            {
                description = "User added description",
                invoice_number = "000000003",
                amount = new paypal.Amount()
                {
                    currency = "USD",
                    total = "7",
                    details = new paypal.Details()
                    {
                        tax = "1",
                        shipping = "1",
                        subtotal = "5"
                    }
                },
                item_list = new paypal.ItemList()
                {
                    items = new List<paypal.Item>()
                    {
                        new paypal.Item()
                        {
                            name = "Item Name",
                            currency = "USD",
                            price = "5",
                            quantity = "1",
                            sku = "sku"
                        }
                    }
                }
            });
            
            List <Product> ProductTable;
            using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
            {
                ProductTable = db_manager.Products.Include(AppSettings.Includes.Category).ToList();
            }

            //model.ProductTable = ProductTable;
            //var transactions = SessionSaver.Load.transactions(Session);
            //model.SelectedProductTable = transactions;

            return View();
        }
        public ActionResult Trans()
        {
            return null;
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}