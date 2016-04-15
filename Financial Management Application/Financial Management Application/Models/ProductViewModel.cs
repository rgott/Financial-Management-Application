using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Financial_Management_Application.Models.ProductVM
{
    public class IndexViewModel
    {
        public List<Product> products { get; set; }
        
    }

    public class CreatePartialViewModel
    {
        public long categoryId { get; set; }
        public List<SelectListItem> categories { get; set; }
        public Product selected { get; set; }
    }
    public class EditPartialViewModel
    {
        public List<Product> products { get; set; }
    }
    public class ViewPartialViewModel
    {
        public List<Product> products { get; set; }
    }
    public class CategoryPartialViewModel
    {
        public List<SelectListItem> categories { get; set; }
    }
}
