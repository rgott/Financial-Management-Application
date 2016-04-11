using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Financial_Management_Application.Models
{
    public class UrlEncryption
    {
        public DateTime timeStamp { get; set; }
        public string email { get; set; }
        public long address { get; set; }
        public long division { get; set; }
        public long role { get; set; }

        static char delimiter = (char)31;
        static int paramNum = 5;// 5 for the five parameters
        protected UrlEncryption(DateTime timeStamp, string email, long address, long division, long role)
        {
            this.timeStamp = timeStamp;
            this.email = email;
            this.address = address;
            this.division = division;
            this.role = role;
        }

        public static string Encrypt(DateTime timeStamp, string email, long address, long division, long role)
        {
            string urlEncoded = timeStamp.Ticks.ToString() + delimiter
                + email + delimiter
                + address.ToString() + delimiter
                + division.ToString() + delimiter
                + role.ToString();

            // TODO:encrypt string here

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(urlEncoded));

        }

        public static UrlEncryption Decrypt(string base64UrlQuery)
        {
            if (base64UrlQuery == null)
                return null;

            string urlQry = Encoding.UTF8.GetString(Convert.FromBase64String(base64UrlQuery));

            // TODO:decrypt here

            string[] urlQryArry = urlQry.Split(delimiter);


            if (urlQryArry.Length != paramNum) // is proper length
                return null;

            long timeStampResult;
            if (!long.TryParse(urlQryArry[0], out timeStampResult)) // is long
                return null;

            string emailResult;

            try
            {
                var addr = new System.Net.Mail.MailAddress(urlQryArry[1]); // confirms that address given is email
                emailResult = addr.Address;
            }
            catch { return null; }

            long divisionResult;
            if (!long.TryParse(urlQryArry[2], out divisionResult)) // is long
                return null;

            long addressResult;
            if (!long.TryParse(urlQryArry[3], out addressResult)) // is long
                return null;

            long roleResult;
            if (!long.TryParse(urlQryArry[4], out roleResult)) // is long
                return null;

            return new UrlEncryption(new DateTime(timeStampResult), emailResult, addressResult, divisionResult, roleResult);
        }
    }
}