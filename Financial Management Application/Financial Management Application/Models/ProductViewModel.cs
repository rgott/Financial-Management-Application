using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Financial_Management_Application.Models.ProductVM
{
    public class CreateViewModel
    {
        [Display(Name ="Category")]
        public long categoryId { get; set; }
        public List<SelectListItem> categories { get; set; }
        public Product product { get; set; }
    }
    public class EditViewModel
    {
        [Display(Name ="Category")]
        public long categoryId { get; set; }
        public List<SelectListItem> categories { get; set; }
        public Product product { get; set; }
    }
    public class IndexViewModel
    {
        public string productUpdateTargetId { get; set; }
        public List<Product> products { get; set; }
    }

    public class DeleteViewModel
    {
        public long[] transactionDefProduct { get; set; }
        public long categoryId { get; set; }
        public long categoryReplacementId { get; set; }
        public List<Transaction> transactions { get; set; }
        public long allTransactions { get; set; }
        public List<SelectListItem> products { get; set; }
    }
}
