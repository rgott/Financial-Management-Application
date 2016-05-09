using System.Collections.Generic;
using PayPal.Api;

namespace Financial_Management_Application.Models
{
    public class PayPalHelper
    {
        public static APIContext apiContext = new APIContext();

        public static PayPal.Api.Payment ExecutePayment(APIContext apiContext, PayPal.Api.Payment payment, PaymentExecution paymentExecution)
        {
            return payment.Execute(apiContext, paymentExecution);
        }

        public static Payment CreatePayment(APIContext apiContext, string redirectUrl, PayPal.Api.Transaction transaction)
        {
            var payer = new Payer() { payment_method = "paypal" };

            var transactionList = new List<PayPal.Api.Transaction>();
            transactionList.Add(transaction);

            Payment payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = new RedirectUrls()
                {
                    cancel_url = redirectUrl,
                    return_url = redirectUrl
                }
            };

            return payment.Create(apiContext);
        }
    }
}