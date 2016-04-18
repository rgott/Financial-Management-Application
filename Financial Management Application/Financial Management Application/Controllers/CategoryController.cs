using Financial_Management_Application.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Threading.Tasks;
using Financial_Management_Application.Models.CategoryVM;

namespace Financial_Management_Application.Controllers
{
    public class CategoryController : ControllerModel
    {
        const string CategorySessionVar = "category";

       
        public ActionResult Peek()
        {
            List<Category> categories = new SessionSaver<List<Category>>().use(Session, CategorySessionVar, (out List<Category> saveobject) =>
            {
                using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
                {
                    saveobject = db_manager.Categories.ToList();
                }
            });

            return View(new PeekViewModel()
            {
                categories = categories
            });
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateViewModel model)
        {
            using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
            {
                Category cat = db_manager.Categories.FirstOrDefault(m => m.Id == model.category.Id);
                
                await db_manager.SaveChangesAsync();
            }
            ViewBag.StatusMessage = "Category '" + model.category.name + "' Created Successfully";
            return View();
        }
    }
}