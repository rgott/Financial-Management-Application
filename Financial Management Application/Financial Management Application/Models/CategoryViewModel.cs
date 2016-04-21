using System.Collections.Generic;
using System.Web.Mvc;

namespace Financial_Management_Application.Models.CategoryVM
{
    public class CreateViewModel
    {
        public Category category { get; set; }
    }
    public class EditViewModel
    {
        public long categoryId { get; set; }
        public Category category { get; set; }
    }
    public class IndexViewModel
    {
        public List<Category> categories { get; set; }
    }

    public class DeleteViewModel
    {
        public long[] productDefCategory { get; set; }
        public long categoryId { get; set; }
        public long categoryReplacementId { get; set; }
        public List<Product> products { get; set; }
        public long allProducts { get; set; }
        public List<SelectListItem> categories { get; set; }
    }
}
