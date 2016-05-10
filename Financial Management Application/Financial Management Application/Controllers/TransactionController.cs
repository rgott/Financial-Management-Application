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
        public ActionResult RequestTransaction(RequestTransactionViewModel model)
        {
            
             if (model.productId != null && model.quantity != null)
            {
                if (model.quantity <= 0)
                {
                    ViewBagHelper.setMessage(ViewBag, ViewBagHelper.MessageType.WarningMsgBox, "Cannot create a product with a quantity of " + model.quantity);
                }
                else
                {
                    List<Transaction> trans;
                    SessionSaver.Load.transactions(Session, out trans);
                    Transaction transactionValidate = trans.FirstOrDefault(m => m.productId == model.productId);
                    if(transactionValidate != null)
                    {
                        transactionValidate.quantity += (int)model.quantity;
                    }
                    else
                    {

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
                            quantity = (int)model.quantity,
                            unitPrice = db_unitPrice,
                            productId = (long)model.productId,
                            Product = db_product,
                            isDeleted = false // on creating transaction it starts as not deleted
                        };

                        SessionSaver.Add.transaction(Session, transaction); // add to session
                    }
                }
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
            List<Transaction> transactions;
            SessionSaver.Load.transactions(Session,out transactions);
            model.SelectedProductTable = transactions;

            return View(model);
        }
        static long getRandomPosLong(Random rand)
        {
            byte[] buf = new byte[8]; // size of long
            rand.NextBytes(buf);
            return Math.Abs(BitConverter.ToInt64(buf, 0));
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
                    randCartId = getRandomPosLong(random);
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
            long cartId = getCartId();

            List<Transaction> trans;
            SessionSaver.Load.transactions(Session, out trans);
            paypal.ItemList paypalItems = new paypal.ItemList();
            paypalItems.items = new List<PayPal.Api.Item>();
            decimal sum = 0;
            for (int i = 0; i < trans.Count; i++)
            {
                trans[i].cartId = cartId;
                trans[i].unitPrice = trans[i].Product.price;
                sum += (trans[i].unitPrice * trans[i].quantity);
                paypalItems.items.Add(new PayPal.Api.Item()
                {
                    name = trans[i].Product.name,
                    currency = "USD",
                    price = trans[i].unitPrice.ToString("0.00"),
                    tax = "0",
                    sku = trans[i].productId.ToString(),
                    quantity = trans[i].quantity.ToString()
                });
            }

            paypal.Transaction transaction = new PayPal.Api.Transaction()
            {
                description = "User added description",
                invoice_number = cartId.ToString(),
                amount = new paypal.Amount()
                {
                    currency = "USD",
                    total = sum.ToString("0.00"),
                    details = new paypal.Details()
                    {
                        tax = "0",
                        shipping = "0",
                        subtotal = sum.ToString("0.00")
                    }
                },
                item_list = paypalItems
            };

            paypal.APIContext apiContext = PayPalHelper.GetAPIContext();
            try
            {
                string payerId = Request.Params["PayerID"];

                if (string.IsNullOrEmpty(payerId))
                {
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority;
                    var guid = Convert.ToString((new Random()).Next(100000));
                    var createdPayment = PayPalHelper.CreatePayment(apiContext, baseURI + Url.Action("RequestTransaction", "Transaction") + "?guid=" + guid, baseURI + Url.Action("Completed","Transaction"), transaction);
                    var links = createdPayment.links.GetEnumerator();

                    string paypalRedirectUrl = null;

                    while (links.MoveNext())
                    {
                        paypal.Links lnk = links.Current;
                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {

                            paypalRedirectUrl = lnk.href;
                        }
                    }
                    Session.Add(guid, createdPayment.id);

                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    var guid = Request.Params["guid"];
                    var s = Session[guid];
                    var executedPayment = PayPalHelper.ExecutePayment(apiContext, null,payerId, Session[guid] as string);

                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return View("PaypalFatal");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error" + ex.Message);
                return View("PaypalFatal");
            }

            return View("Completed");
        } 
        public ActionResult Trans()
        {
            return null;
        }
        public ActionResult Completed()
        {
            if (HttpContext.Request.UrlReferrer != null) // comes from another site
            {
                return new HttpNotFoundResult();
            }

            List<Transaction> boughtItems;
            SessionSaver.Load.transactions(Session, out boughtItems);
            for (int i = 0; i < boughtItems.Count; i++)
            {
                SessionSaver.Update.transaction(Session, boughtItems[i]);
            }
            Session[AppSettings.SessionVariables.TRANSACTION] = null; // reset the cart

            return View(new CompletedViewModel()
            {
                boughtItems = boughtItems
            });
        }

        public ActionResult Remove(int? Id)
        {
            if(Id == null)
            {
                return new HttpNotFoundResult();
            }
            SessionSaver.Remove.transaction(Session, (int)Id);
            return Redirect(Url.Action("RequestTransaction"));
        }
        public ActionResult Index()
        {
            return View();
        }
    }
}