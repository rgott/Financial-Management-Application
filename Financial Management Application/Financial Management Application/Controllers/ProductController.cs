using Financial_Management_Application.Models.ProductVM;
using Financial_Management_Application.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;

namespace Financial_Management_Application.Controllers
{
    public class ProductController : ControllerModel
    {
        public ActionResult Index()
        {
            List<Product> products = SessionSaver.Load.products(TempData, true);

            return View(new Models.ProductVM.IndexViewModel()
            {
                products = products
            });
        }
        
        //[HttpPost]
        
        //public ActionResult Index(long? Id)
        //{
        //    using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
        //    {
        //        if (Session[AppSettings.SessionVariables.PRODUCT] != null)
        //        { // if session var exists remove product from session var
        //            List<Product> products = (List<Product>)Session[AppSettings.SessionVariables.PRODUCT];
        //            products.Remove(products.FirstOrDefault(m => m.Id == Id));
        //            Session[AppSettings.SessionVariables.PRODUCT] = products;
        //        }
        //        db_manager.Products.Remove(db_manager.Products.FirstOrDefault(m => m.Id == Id));
        //        db_manager.SaveChanges();
        //    }
        //    return View();
        //}
        public ActionResult Create()
        {
            List<SelectListItem> categories = SessionSaver.Load.categoriesCombobox(TempData);

            return View(new CreateViewModel()
            {
                categories = categories
            });
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateViewModel model)
        {
            model.product.categoryId = model.categoryId;
            await SessionSaver.Add.product(TempData, model.product);

            ViewBagHelper.setMessage(ViewBag,ViewBagHelper.MessageType.SuccessMsgBox,"Added product '" + model.product.name + "'");
            List<SelectListItem> categories = SessionSaver.Load.categoriesCombobox(TempData);
            model.categories = categories;

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

            List<SelectListItem> categories = SessionSaver.Load.categoriesCombobox(TempData);
            return View(new EditViewModel()
            {
                product = product,
                categories = categories
            });
        }
        [HttpPost]
        public ActionResult Edit(long? Id, EditViewModel model)
        {
            if(Id == null)
            {
                return new HttpNotFoundResult();
            }
            using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
            {
                model.product.categoryId = model.categoryId;
                model.product.Category = db_manager.Categories.FirstOrDefault(m => m.Id == model.categoryId);
                model.product.Id = (long)Id;
                Category newCategory = db_manager.Categories.FirstOrDefault(m => m.Id == model.categoryId);
                model.product.Category = newCategory;
                SessionSaver.Update.product(TempData, model.product);
            }
            
            return Redirect(Url.Action("Index"));
        }

        //TODO: change to Sessionsaver.remove
        public ActionResult Delete(long? Id)
        {
            if (Id == null)
                return new HttpNotFoundResult();

            //bool s = SessionSaver.Remove.product(Session, Id);

            List<Transaction> transactions;
            int productTransactionCount = 0;
            using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
            {
                var db_productList = db_manager.Products.FirstOrDefault(m => m.Id == Id);
                if (db_productList == null)
                { // if item has already been deleted return the category view and display that the value had previously been deleted
                    ViewBagHelper.setMessage(ViewBag, ViewBagHelper.MessageType.WarningMsgBox, "Product was already deleted");
                    return Redirect(Url.Action("Index", "Category"));
                }
                transactions = db_productList.Transactions.ToList();

                if (transactions.Count == 0)
                {
                    // remove from database
                    db_manager.Products.Remove(db_manager.Products.FirstOrDefault(m => m.Id == Id));
                    db_manager.SaveChanges();

                    // remove from session
                    List<Product> productsList = (List<Product>)Session[AppSettings.SessionVariables.PRODUCT];
                    if (productsList != null)
                    {
                        productsList.Remove(productsList.FirstOrDefault(m => m.Id == Id));
                    }
                    Session[AppSettings.SessionVariables.PRODUCT] = productsList;
                }
            }
            productTransactionCount = transactions.Count;
            long[] transactionDefProduct = new long[productTransactionCount];

            List<SelectListItem> products = SessionSaver.Load.productsCombobox(TempData);

            return View(new DeleteViewModel()
            {
                products = products,
                transactions = transactions,
                transactionDefProduct = transactionDefProduct
            });
        }
    }
}