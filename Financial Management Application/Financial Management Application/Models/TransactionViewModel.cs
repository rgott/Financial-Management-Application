using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace Financial_Management_Application.Models.TransactionVM
{
    public class IndexViewModel
    {
        public List<Transaction> transactions { get; set; }
    }
    public class RequestTransactionViewModel// change all *Trasactions to *
    {
        public List<Product> ProductTable { get; set; }
        public List<Transaction> SelectedProductTable { get; set; }
        
        public int? quantity { get; set; }
        public long? productId { get; set; }
    }

    public class CompletedViewModel
    {
        public List<Transaction> boughtItems { get; set; }

    }
}