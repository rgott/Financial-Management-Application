using Financial_Management_Application.Models.ProductVM;
using Financial_Management_Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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

        public ActionResult CreatePartial(CreatePartialViewModel Id)
        {
            // if id is not null then add item to database
            //if()
            //{

            //}
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

            return PartialView(new CreatePartialViewModel()
            {
                categories = categories
            });
        }

        public ActionResult ViewPartial()
        {
            List<Product> products = new SessionSaver<List<Product>>().use(Session, ProductSessionVar, (out List<Product> saveobject) =>
            {
                using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
                {
                    saveobject = db_manager.Products.ToList();
                }
            });

            return PartialView(new ViewPartialViewModel()
            {
                products = products
            });
        }

        public ActionResult CategoryPartial()
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

            return PartialView(new CategoryPartialViewModel()
            {
                categories = categories
            });
        }

    }
}