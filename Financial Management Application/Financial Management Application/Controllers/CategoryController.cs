using Financial_Management_Application.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Threading.Tasks;
using Financial_Management_Application.Models.CategoryVM;

namespace Financial_Management_Application.Controllers
{
    [Authorize]
    public class CategoryController : ControllerModel
    {
        const string CategorySessionVar = "category";
        const string ProductSessionVar = "products";

        public ActionResult Index()
        {
            List<Category> categories = new SessionSaver<List<Category>>().use(Session, CategorySessionVar, (out List<Category> saveobject) =>
            {
                using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
                {
                    saveobject = db_manager.Categories.ToList();
                }
            });

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
                if (Session[CategorySessionVar] != null)
                { // if session var exists remove product from session var
                    List<Category> products = (List<Category>)Session[CategorySessionVar];
                    products.Remove(products.First(m => m.Id == Id));
                    Session[CategorySessionVar] = products;
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
            using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
            {
                newCategory = db_manager.Categories.Add(model.category);
               

                await db_manager.SaveChangesAsync();
            }

            List<Category> categories = new SessionSaver<List<Category>>().use(Session, CategorySessionVar, (out List<Category> saveobject) =>
            {
                using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
                {
                    saveobject = db_manager.Categories.ToList();
                }
            });
            categories.Add(newCategory);
            Session[CategorySessionVar] = categories;

            ViewBag.StatusMessage = "Category '" + model.category.name + "' Created Successfully";
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
                category = db_manager.Categories.First(m => m.Id == Id);
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
                Category newCategory = db_manager.Categories.First(m => m.Id == Id);
                if (Session[CategorySessionVar] != null)
                { // if session var exists edit product from session var
                    List<Category> categories = (List<Category>)Session[CategorySessionVar];
                    if(categories != null)
                    {
                        int index = categories.IndexOf(categories.First(m => m.Id == Id));
                        categories[index].name = model.category.name;
                    }

                    Session[CategorySessionVar] = categories;
                }

                Category tmpCategory = db_manager.Categories.First(m => m.Id == Id);
                tmpCategory.name = model.category.name;

                db_manager.Entry(tmpCategory);
                db_manager.SaveChanges();
            }
            return Redirect(Url.Action("Index"));
        }


        public ActionResult Delete(long? Id)
        {
            if (Id == null)
                return new HttpNotFoundResult();

            List<Product> products;
            int categoryProductCount = 0;
            using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
            {
                var db_productList = db_manager.Categories.First(m => m.Id == Id).Products;
                if(db_productList == null)
                { // if item has already been deleted return the category view and display that the value had previously been deleted
                    ViewBag.alreadyDeletedMessage = "Category was already deleted";
                    return Redirect(Url.Action("Index", "Category"));
                }
                products = db_productList.ToList();

                if(products.Count == 0)
                {
                    // remove from database
                    db_manager.Categories.Remove(db_manager.Categories.First(m => m.Id == Id));
                    db_manager.SaveChanges();

                    // remove from session
                    List<Category> categoriesList = (List<Category>)Session[CategorySessionVar];
                    if(categoriesList != null)
                    {
                        categoriesList.Remove(categoriesList.First(m => m.Id == Id));
                    }
                }
            }
            categoryProductCount = products.Count;
            long[] productDefCategory = new long[categoryProductCount];

            List<SelectListItem> categories = new SessionSaver<List<SelectListItem>>().use(Session, ProductSessionVar, (out List<SelectListItem> saveobject) =>
            {
                List<SelectListItem> CategoriesList = new List<SelectListItem>();
                using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
                {
                    foreach (var item in db_manager.Categories.ToList())
                    {
                        CategoriesList.Add(new SelectListItem()
                        {
                            Text = item.name,
                            Value = item.Id.ToString() //  will be used to get id later
                        });
                    }
                }
                saveobject = CategoriesList;
            });

            return View(new DeleteViewModel()
            {
                products = products,
                categories = categories,
                productDefCategory = productDefCategory
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="RId">Replacement Id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(DeleteViewModel model)
        {
            //using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
            //{
            //    //RId = 6;
            //    Category category = db_manager.Categories.First(m => m.Id == Id);
            //    Category newCategory = db_manager.Categories.First(m => m.Id == (long)RId);
            //    List<Product> prod = category.Products.ToList();
            //    foreach (var item in prod)
            //    {
            //        item.categoryId = (long)RId;
            //        db_manager.Entry(item).Property(m => m.categoryId).CurrentValue = (long)RId;
            //    }
            //    Session[ProductSessionVar] = prod;
            //    db_manager.Categories.Remove(category);
            //    db_manager.SaveChanges();

            //    List<Category> categories = (List<Category>)Session[CategorySessionVar];
            //    categories.RemoveAt(categories.IndexOf(categories.First(m => m.Id == Id)));                   
            //}
            return View();
        }

    }
}