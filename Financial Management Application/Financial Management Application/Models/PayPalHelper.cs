using System.Collections.Generic;
using PayPal.Api;

namespace Financial_Management_Application.Models
{
    public class PayPalHelper
    {
        public readonly static string ClientId;
        public readonly static string ClientSecret;
        // Static constructor for setting the readonly static members.
        static PayPalHelper()
        {
            var config = GetConfig();
            ClientId = config["clientId"];
            ClientSecret = config["clientSecret"];
        }


        // Create the configuration map that contains mode and other optional configuration details.
        public static Dictionary<string, string> GetConfig()
        {
            return PayPal.Api.ConfigManager.Instance.GetProperties();
        }
        public static APIContext GetAPIContext()
        {
            APIContext apiContext = new APIContext(GetAccessToken());
            apiContext.Config = GetConfig();
            return apiContext;
        }
        public static Payment ExecutePayment(APIContext apiContext,PayPal.Api.Payment payment, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution() { payer_id = payerId };
            payment = new Payment() { id = paymentId };
            return payment.Execute(apiContext, paymentExecution);
        }

        public static Payment CreatePayment(APIContext apiContext,string redirectUrl,string returnUrl, PayPal.Api.Transaction transaction)
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
                    return_url = returnUrl
                }
            };

            return payment.Create(apiContext);
        }

        private static string GetAccessToken()
        {
            string accessToken = new OAuthTokenCredential(ClientId, ClientSecret, GetConfig()).GetAccessToken();
            return accessToken;
        }
    }
}