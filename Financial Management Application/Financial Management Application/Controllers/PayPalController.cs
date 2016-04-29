using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PayPal.Api;
using Model = Financial_Management_Application.Models;



namespace Financial_Management_Application.Controllers
{
    public class PayPalController : Controller
    {
        // GET: PayPal
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PaymentWithCreditCard()
        {
            Item item = new Item();
            item.name = "Demo Item";
            item.currency = "USD";
            item.price = "5";
            item.quantity = "1";
            item.sku = "sku";

            List<Item> itms = new List<Item>();
            itms.Add(item);
            ItemList itemList = new ItemList();
            itemList.items = itms;

            Address billingAddress = new Address();
            billingAddress.city = "Baltimore";
            billingAddress.country_code = "US";
            billingAddress.line1 = "7800 York Road";
            billingAddress.postal_code = "21286";
            billingAddress.state = "MD";

            CreditCard crdtCard = new CreditCard();
            crdtCard.billing_address = billingAddress;
            crdtCard.cvv2 = "874";  //card cvv2 number
            crdtCard.expire_month = 4; //card expire date
            crdtCard.expire_year = 2021; //card expire year
            crdtCard.first_name = "Patrick";
            crdtCard.last_name = "Lenhart";
            crdtCard.number = "4032035141244814"; //enter your credit card number here
            crdtCard.type = "visa"; //credit card type here paypal allows 4 types

            Details details = new Details();
            details.shipping = "1";
            details.subtotal = "5";
            details.tax = "1";

            Amount amnt = new Amount();
            amnt.currency = "USD";
            // Total = shipping tax + subtotal.
            amnt.total = "7";
            amnt.details = details;

            Transaction tran = new Transaction();
            tran.amount = amnt;
            tran.description = "Total of Items.";
            tran.item_list = itemList;
            tran.invoice_number = "0000000002";

            List<Transaction> transactions = new List<Transaction>();
            transactions.Add(tran);

            FundingInstrument fundInstrument = new FundingInstrument();
            fundInstrument.credit_card = crdtCard;

            List<FundingInstrument> fundingInstrumentList = new List<FundingInstrument>();
            fundingInstrumentList.Add(fundInstrument);

            Payer payr = new Payer();
            payr.funding_instruments = fundingInstrumentList;
            payr.payment_method = "credit_card";

            Payment pymnt = new Payment();
            pymnt.intent = "sale";
            pymnt.payer = payr;
            pymnt.transactions = transactions;

            try
            {
                APIContext apiContext = Model.Configuration.GetAPIContext();
                Payment newPayment = pymnt.Create(apiContext);
                if(newPayment.state.ToLower() != "approved")
                {
                    return View("FailureView");
                }
            }
            catch(PayPal.PayPalException ex)
            {
                Model.Logger.Log("Error: " + ex.Message);
                return View("FailureView");
            }

            return View("SuccessView");

        }

        public ActionResult PaymentWithPayPal()
        {
            
            APIContext apiContext = Model.Configuration.GetAPIContext();

            try
            {
                string payerId = Request.Params["PayerID"];

                if (string.IsNullOrEmpty(payerId))
                {
                   

                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/Paypal/PaymentWithPayPal?";

                   
                    var guid = Convert.ToString((new Random()).Next(100000));
                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid);
                    var links = createdPayment.links.GetEnumerator();

                    string paypalRedirectUrl = null;

                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;

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
                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);

                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return View("FailureView");
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Model.Logger.Log("Error" + ex.Message);
                return View("FailureView");
            }

            return View("SuccessView");
        }

        private PayPal.Api.Payment payment;

        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            
            var paymentExecution = new PaymentExecution() { payer_id = payerId };
            this.payment = new Payment() { id = paymentId };
            return this.payment.Execute(apiContext, paymentExecution);
        }

        private Payment CreatePayment(APIContext apiContext, string redirectUrl)
        {
           
            var itemList = new ItemList() { items = new List<Item>() };

            itemList.items.Add(new Item()
            {
                name = "Item Name",
                currency = "USD",
                price = "5",
                quantity = "1",
                sku = "sku"
            });

            var payer = new Payer() { payment_method = "paypal" };

            
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl,
                return_url = redirectUrl
            };

            
            var details = new Details()
            {
                tax = "1",
                shipping = "1",
                subtotal = "5"
            };

           
            var amount = new Amount()
            {
                currency = "USD",
                total = "7", 
                details = details
            };

            var transactionList = new List<Transaction>();

            transactionList.Add(new Transaction()
            {
                description = "Transaction description.",
                invoice_number = "000000003",
                amount = amount,
                item_list = itemList
            });

            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };

            
            return this.payment.Create(apiContext);

        }
    }
    
}