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
        /// <summary>
        /// Allows the viewbag to pass
        /// </summary>
        /// <returns></returns>
        public ActionResult IndexView()
        {
            // return index view
            List<Category> categoriesview;
            SessionSaver.Load.categories(TempData, out categoriesview);
            return View("Index", new Models.CategoryVM.IndexViewModel()
            {
                categories = categoriesview
            });
        }

        public ActionResult Index()
        {
            List<Category> categories;
            SessionSaver.Load.categories(TempData, out categories);

            return View(new Models.CategoryVM.IndexViewModel()
            {
                categories = categories
            });
        }
        [HttpPost]
        public ActionResult Index(long? Id)
        {
            using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
            {
                if (Session[AppSettings.SessionVariables.CATEGORY] != null)
                { // if session var exists remove product from session var
                    List<Category> products = (List<Category>)Session[AppSettings.SessionVariables.CATEGORY];
                    products.Remove(products.FirstOrDefault(m => m.Id == Id));
                    Session[AppSettings.SessionVariables.CATEGORY] = products;
                }
                Category category = db_manager.Categories.Single(m => m.Id == Id);
                db_manager.Categories.Remove(category);
                db_manager.SaveChanges();
            }
            return View();
        }
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateViewModel model)
        {
            Category newCategory;
            newCategory = await SessionSaver.Add.category(TempData, model.category);

            ModelState.Clear();
            ViewBagHelper.setMessage(ViewBag, ViewBagHelper.MessageType.SuccessMsgBox, "Category '" + model.category.name + "' Created Successfully");
            return View();
        }
        public ActionResult Edit(long? Id)
        {
            if (Id == null)
            {
                return new HttpNotFoundResult();
            }

            Category category;
            using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
            {
                category = db_manager.Categories.FirstOrDefault(m => m.Id == Id);
            }

            return View(new EditViewModel()
            {
                category = category
            });
        }

        [HttpPost]
        public ActionResult Edit(long? Id, EditViewModel model)
        {
            if (Id == null)
                return new HttpNotFoundResult();

            using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
            {
                model.category.Id = (long)Id;
                SessionSaver.Update.category(TempData, model.category);
            }

            ViewBagHelper.setMessage(ViewBag, ViewBagHelper.MessageType.SuccessMsgBox, "Category " + model.category.name + " updated successfully");
            return IndexView();
        }


        public async Task<ActionResult> Delete(long? Id)
        {
            if (Id == null)
                return new HttpNotFoundResult();

            List<SelectListItem> categories;
            SessionSaver.Load.categoriesCombobox(TempData, out categories);
            categories.Remove(categories.FirstOrDefault(m => m.Value == Id.ToString())); // remove category currently being deleted
            if (categories.Count == 0)
            {
                using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
                {
                    if(db_manager.Products.Count() == 0)
                    {
                        ViewBagHelper.setMessage(ViewBag, ViewBagHelper.MessageType.WarningMsgBox, "No other categories available to transfer products to. Please create a <a href='" + Url.Action("Create", "Category") + "'>category</a>");
                        return IndexView();
                    }
                }
            }

            List<Product> products;
            Category category;
            int categoryProductCount = 0;
            using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
            {
                category = db_manager.Categories.FirstOrDefault(m => m.Id == Id);
                if(category == null)
                { // if item has already been deleted return the category view and display that the value had previously been deleted
                    ViewBagHelper.setMessage(ViewBag, ViewBagHelper.MessageType.WarningMsgBox, "Category was already deleted");
                    return Redirect(Url.Action("Index", "Category"));
                }
                products = category.Products.ToList();

                if(products.Count == 0)
                {
                    await SessionSaver.Remove.category(TempData, (long)Id);
                    ViewBagHelper.setMessage(ViewBag, ViewBagHelper.MessageType.SuccessMsgBox, "No products are linked to category. \"" + category.name + "\" has been deleted.");
                    return IndexView();
                }
            }
            categoryProductCount = products.Count;
            long[] productDefCategory = new long[categoryProductCount];

           
            return View(new DeleteViewModel()
            {
                products = products,
                categories = categories,
                productDefCategory = productDefCategory
            });
        }

        [HttpPost]
        public async Task<ActionResult> Delete(long? Id, DeleteViewModel model)
        {
            if (Id == null)
                return new HttpNotFoundResult();

            if(model.allProducts != 0)
            {
                await SessionSaver.Remove.category(TempData, (long) Id, model.allProducts);
            }
            else
            {
                await SessionSaver.Remove.category(TempData, (long) Id, model.productDefCategory);
            }

            return RedirectToAction("Index");
        }
    }
}