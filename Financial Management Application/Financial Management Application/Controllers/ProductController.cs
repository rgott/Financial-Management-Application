using Financial_Management_Application.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Threading.Tasks;
using Financial_Management_Application.Models.ProductVM;

namespace Financial_Management_Application.Controllers
{

    public class ProductController : ControllerModel
    {
        /// <summary>
        /// Allows the viewbag to pass
        /// </summary>
        /// <returns></returns>
        public ActionResult IndexView()
        {
            // return index view
            List<Product> productsView;
            SessionSaver.Load.products(TempData,out productsView);

            return View("Index", new Models.ProductVM.IndexViewModel()
            {
                products = productsView
            });
        }

        public ActionResult Index()
        {
            List<Product> products;
            SessionSaver.Load.products(TempData,out products);

            for (int i = 0; i < products.Count; i++)
            {
                if(products[i].Category == null)
                {
                    return new HttpNotFoundResult();
                }
            }

            return View(new Models.ProductVM.IndexViewModel()
            {
                products = products
            });
        }

        public ActionResult Create()
        {
            List<SelectListItem> categories;
            SessionSaver.Load.categoriesCombobox(TempData, out categories);
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

            List<SelectListItem> categories;
            SessionSaver.Load.categoriesCombobox(TempData, out categories);
            model.categories = categories;

            ViewBagHelper.setMessage(ViewBag, ViewBagHelper.MessageType.SuccessMsgBox, "Product '" + model.product.name + "' Created Successfully");
            return IndexView();
        }

        public ActionResult Edit(long? Id)
        {
            if (Id == null)
                return new HttpNotFoundResult();

            Product product;
            using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
            {
                product = db_manager.Products.FirstOrDefault(m => m.Id == Id);
            }

            List<SelectListItem> categories;
            SessionSaver.Load.categoriesCombobox(TempData, out categories);
            return View(new EditViewModel()
            {
                product = product,
                categories = categories
            });
        }

        [HttpPost]
        public ActionResult Edit(long? Id, EditViewModel model)
        {
            if (Id == null)
                return new HttpNotFoundResult();

            using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
            {
                model.product.Id = (long)Id;
                model.product.categoryId = model.categoryId;
                model.product.Category = db_manager.Categories.FirstOrDefault(m => m.Id == model.categoryId);
            }
            SessionSaver.Update.product(TempData, model.product);

            ViewBagHelper.setMessage(ViewBag, ViewBagHelper.MessageType.SuccessMsgBox, "Product " + model.product.name + " updated successfully");
            return IndexView();
        }

        

        public async Task<ActionResult> Delete(long? Id)
        {
            if (Id == null)
                return new HttpNotFoundResult();

            List<SelectListItem> products;
            SessionSaver.Load.productsCombobox(TempData, out products);
            products.Remove(products.FirstOrDefault(m => m.Value == Id.ToString())); // remove product currently being deleted
            List<Transaction> transCount;
            SessionSaver.Load.transactions(Session,out transCount, false);
            if (products.Count == 0 && transCount.Count != 0)
            {
                ViewBagHelper.setMessage(ViewBag, ViewBagHelper.MessageType.WarningMsgBox, "No other products available to transfer transactions to. Please create a <a href='" + Url.Action("Create", "Product") + "'>product</a>");
                return IndexView();
            }


            List<Transaction> transactions;
            Product product;
            int productTransactionCount = 0;
            using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
            {
                product = db_manager.Products.FirstOrDefault(m => m.Id == Id);
                if (product == null)
                { // if item has already been deleted return the product view and display that the value had previously been deleted
                    ViewBagHelper.setMessage(ViewBag, ViewBagHelper.MessageType.WarningMsgBox, "Product was already deleted");
                    return Redirect(Url.Action("Index", "Product"));
                }
                transactions = product.Transactions.ToList();

                if (transactions.Count == 0)
                {
                    await SessionSaver.Remove.product(TempData, (long)Id);


                    // return index view
                    ViewBagHelper.setMessage(ViewBag, ViewBagHelper.MessageType.SuccessMsgBox, "No Conflicts Found Product has been deleted");
                    return IndexView();
                }
            }
            productTransactionCount = transactions.Count;
            long[] transactionDefProduct = new long[productTransactionCount];


            return View(new DeleteViewModel()
            {
                transactions = transactions,
                products = products,
                transactionDefProduct = transactionDefProduct
            });
        }

        [HttpPost]
        public async Task<ActionResult> Delete(long? Id, DeleteViewModel model)
        {
            if (Id == null)
                return new HttpNotFoundResult();

            if (model.allTransactions != 0)
            {
                await SessionSaver.Remove.product(TempData, Session, (long)Id, model.allTransactions);
            }
            else
            {
                await SessionSaver.Remove.product(TempData, Session, (long)Id, model.transactionDefProduct);
            }

            return Redirect(Url.Action("Index", "Product"));
        }
    }
}