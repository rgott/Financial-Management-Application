using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Financial_Management_Application.Models.ProductVM
{
    public class IndexViewModel
    {
        public List<Product> products { get; set; }
    }

    public class CreateViewModel
    {
        [Display(Name ="Category")]
        public long categoryId { get; set; }
        public List<SelectListItem> categories { get; set; }
        public Product selected { get; set; }
    }
    public class EditViewModel
    {
        public List<Product> products { get; set; }
    }
    public class PeekViewModel
    {
        public List<Product> products { get; set; }
    }
    public class CategoryViewModel
    {
        public List<SelectListItem> categories { get; set; }
    }

    public class CategoryCreateViewModel
    {
        public Category category { get; set; }
    }

    public class RowViewModel
    {
        public Product productItem { get; set; }
    }
}
