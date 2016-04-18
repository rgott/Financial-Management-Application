using Financial_Management_Application.Models.ProductVM;
using Financial_Management_Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;

namespace Financial_Management_Application.Controllers
{
    public class ProductController : ControllerModel
    {
        const string ProductSessionVar = "products";
        const string CategorySessionVar = "category";

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            List<SelectListItem> categories = new SessionSaver<List<SelectListItem>>().use(Session, ProductSessionVar, (out List<SelectListItem> saveobject)=> 
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

            return View(new CreateViewModel()
            {
                categories = categories
            });
        }

        [HttpPost]
        public ActionResult Create(CreateViewModel model)
        {
            // inject category
            model.selected.categoryId = model.categoryId;

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

            using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
            {
                db_manager.Products.Add(model.selected);
                db_manager.SaveChanges();
            }

            return View(new CreateViewModel()
            {
                categories = categories
            });
        }


        public ActionResult Peek()
        {
            List<Product> products = new SessionSaver<List<Product>>().use(Session, ProductSessionVar, (out List<Product> saveobject) =>
            {
                using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
                {
                    saveobject = db_manager.Products.ToList();
                }
            });

            return View(new PeekViewModel()
            {
                products = products
            });
        }

        public ActionResult Category()
        {
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

            return View(new CategoryViewModel()
            {
                categories = categories
            });
        }


        public ActionResult CategoryCreatePartial()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> CategoryCreatePartial(CategoryCreateViewModel model)
        {
            using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
            {
                db_manager.Categories.Add(model.category);
                await db_manager.SaveChangesAsync();
            }
            ViewBag.StatusMessage = "Category '" + model.category.name + "' Created Successfully";
            return View();
        }

        public ActionResult RowPartial(long rowModel)
        {
            Product product;
            using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
            {
                product = db_manager.Products.First(m => m.Id == rowModel);
            }


            return PartialView(new RowViewModel()
            {
                productItem = product
            });
        }

        public ActionResult RowEditPartial(long rowModel)
        {
            Product product;
            using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
            {
                product = db_manager.Products.First(m => m.Id == rowModel);
            }

            return PartialView(new RowViewModel()
            {
                productItem = product
            });
        }

    }
}