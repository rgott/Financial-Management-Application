using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace Financial_Management_Application.Models.TransactionVM
{
    public class RequestTransactionViewModel// change all *Trasactions to *
    {
        
        public List<Product> ProductTable { get; set; }
    }


}