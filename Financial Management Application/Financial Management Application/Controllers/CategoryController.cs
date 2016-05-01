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
        public ActionResult Index()
        {
            List<Category> categories = SessionSaver.Load.categories(TempData);

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

            List<Category> categories = SessionSaver.Load.categories(TempData);

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
                category = category,
            });
        }

        [HttpPost]
        public ActionResult Edit(long? Id, EditViewModel model)
        {
            using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
            {
                Category newCategory = db_manager.Categories.FirstOrDefault(m => m.Id == Id);
                if (Session[AppSettings.SessionVariables.CATEGORY] != null)
                { // if session var exists edit product from session var
                    List<Category> categories = (List<Category>)Session[AppSettings.SessionVariables.CATEGORY];
                    if(categories != null)
                    {
                        int index = categories.IndexOf(categories.FirstOrDefault(m => m.Id == Id));
                        categories[index].name = model.category.name;
                    }

                    Session[AppSettings.SessionVariables.CATEGORY] = categories;
                }

                Category tmpCategory = db_manager.Categories.FirstOrDefault(m => m.Id == Id);
                tmpCategory.name = model.category.name;

                db_manager.Entry(tmpCategory);
                db_manager.SaveChanges();
            }
            return Redirect(Url.Action("Index"));
        }


        public async Task<ActionResult> Delete(long? Id)
        {
            if (Id == null)
                return new HttpNotFoundResult();

            List<SelectListItem> categories = SessionSaver.Load.categoriesCombobox(TempData);
            categories.Remove(categories.FirstOrDefault(m => m.Value == Id.ToString())); // remove category currently being deleted
            if (categories.Count == 0)
            {
                ViewBagHelper.setMessage(ViewBag, ViewBagHelper.MessageType.SuccessMsgBox, "No other categories available to transfer products to. Please create a <a href='" + Url.Action("Create", "Category") + "'>category</a>");

                // return index view
                List<Category> categoriesview = SessionSaver.Load.categories(TempData);
                return View("Index",new Models.CategoryVM.IndexViewModel()
                {
                    categories = categoriesview
                });
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


                    // return index view
                    ViewBagHelper.setMessage(ViewBag, ViewBagHelper.MessageType.SuccessMsgBox, "No Conflicts Found Category has been deleted");
                    List<Category> categoriesview = SessionSaver.Load.categories(TempData);
                    return View("Index", new Models.CategoryVM.IndexViewModel()
                    {
                        categories = categoriesview
                    });
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

            return Redirect(Url.Action("Index", "Category"));
        }
    }
}