using System.Collections.Generic;
using System.Web.Mvc;

namespace Financial_Management_Application.Models.CategoryVM
{
    public class CreateViewModel
    {
        public long categoryId { get; set; }
        public Category category { get; set; }
    }
    public class EditViewModel
    {
        public List<Category> categories { get; set; }
    }
    public class PeekViewModel
    {
        public List<Category> categories { get; set; }
    }
}
