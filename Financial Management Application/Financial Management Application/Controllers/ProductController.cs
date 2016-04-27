﻿using Financial_Management_Application.Models.ProductVM;
using Financial_Management_Application.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity.Infrastructure;

namespace Financial_Management_Application.Controllers
{
    public class ProductController : ControllerModel
    {
        public ActionResult Index()
        {
            List<Product> products = new SessionSaver<List<Product>>().use(Session, AppSettings.SessionVariables.PRODUCT, (out List<Product> saveobject) =>
            {
                using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
                {
                    DbQuery<Product> queryProducts = db_manager.Products.Include(AppSettings.Includes.Category);
                    saveobject = queryProducts.ToList();
                }
            });

            return View(new Models.ProductVM.IndexViewModel()
            {
                products = products
            });
        }
        [HttpPost]
        public ActionResult Index(long? Id)
        {
            using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
            {
                if (Session[AppSettings.SessionVariables.PRODUCT] != null)
                { // if session var exists remove product from session var
                    List<Product> products = (List<Product>)Session[AppSettings.SessionVariables.PRODUCT];
                    products.Remove(products.FirstOrDefault(m => m.Id == Id));
                    Session[AppSettings.SessionVariables.PRODUCT] = products;
                }
                db_manager.Products.Remove(db_manager.Products.FirstOrDefault(m => m.Id == Id));
                db_manager.SaveChanges();
            }
            return View();
        }
        public ActionResult Create()
        {
            List<SelectListItem> categories = new SessionSaver<List<SelectListItem>>().use(Session, AppSettings.SessionVariables.CATEGORYCOMBOBOX, (out List<SelectListItem> saveobject) =>
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

            model.product.categoryId = model.categoryId;
            using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
            {
                db_manager.Products.Add(model.product);
                db_manager.SaveChanges();
                ViewBag.SuccessMessage = "Added product '" + model.product.name + "'";
            }
            List<SelectListItem> categories = new SessionSaver<List<SelectListItem>>().use(Session, AppSettings.SessionVariables.CATEGORYCOMBOBOX, (out List<SelectListItem> saveobject) =>
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


            model.categories = categories;
            model.product.name = "";
            model.product.price = 0;

            return View(model);
        }


        public ActionResult Edit(long? Id)
        {
            if(Id == null)
            {
                return new HttpNotFoundResult();
            }

            Product product;
            using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
            {
                product = db_manager.Products.Include(AppSettings.Includes.Category).FirstOrDefault(m => m.Id == Id);
            }

            // create category list
            List<SelectListItem> categories = new SessionSaver<List<SelectListItem>>().use(Session, AppSettings.SessionVariables.CATEGORYCOMBOBOX, (out List<SelectListItem> saveobject) =>
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

            return View(new EditViewModel()
            {
                product = product,
                categories = categories
            });
        }
        [HttpPost]
        public ActionResult Edit(long? Id, EditViewModel model)
        {
            using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
            {
                Category newCategory = db_manager.Categories.FirstOrDefault(m => m.Id == model.categoryId);
                if (Session[AppSettings.SessionVariables.PRODUCT] != null)
                { // if session var exists edit product from session var
                    List<Product> products = (List<Product>)Session[AppSettings.SessionVariables.PRODUCT];
                    int index = products.IndexOf(products.FirstOrDefault(m => m.Id == Id));

                    products[index].name = model.product.name;
                    products[index].price = model.product.price;
                    products[index].Category = newCategory;
                    Session[AppSettings.SessionVariables.PRODUCT] = products;
                }

                Product tmpProd = db_manager.Products.FirstOrDefault(m => m.Id == Id);
                tmpProd.name = model.product.name;
                tmpProd.price = model.product.price;
                tmpProd.Category = newCategory;

                db_manager.Entry(tmpProd);
                db_manager.SaveChanges();
            }
            return Redirect(Url.Action("Index"));
        }
    }
}