using Financial_Management_Application.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Financial_Management_Application
{
    public class SessionSaver<T>
    {
        #region basic methods
        public delegate void useSessionFunc(out T savedObject);

        /// <summary>
        /// Ensures the right type is set and returned from session
        /// </summary>
        /// <param name="Session"></param>
        /// <param name="sessionVarName"></param>
        /// <param name="methodsetObject">method where the T value is set to the session with key param:sessionVarName. If null then automatically loads value from storage</param>
        /// <returns></returns>
        public bool use(HttpSessionStateBase Session,out T savedObject, string sessionVarName, useSessionFunc methodsetObject)
        {
            bool newData;
            savedObject = default(T);
            object sessionVar = Session[sessionVarName];

            if ((sessionVar == null || !(sessionVar.GetType() == typeof(T))) && methodsetObject != null)
            {
                newData = false;
                methodsetObject(out savedObject); // set saved to value
                Session.Add(sessionVarName, savedObject);
            }
            else
            {
                newData = false;
                savedObject = (T)Session[sessionVarName];
            }

            return newData;
        }

        public bool use(TempDataDictionary ViewData, out T savedObject, string sessionVarName, useSessionFunc methodsetObject)
        {
            bool newData;
            savedObject = default(T);
            object sessionVar = ViewData[sessionVarName];

            if ((sessionVar == null || !(sessionVar.GetType() == typeof(T))) && methodsetObject != null)
            {
                newData = true;
                methodsetObject(out savedObject); // set saved to value
                ViewData.Add(sessionVarName, savedObject);
            }
            else
            {
                newData = false;
                savedObject = (T)ViewData[sessionVarName];
            }

            return newData;
        }

        #endregion
    }
    public static class SessionSaver
    {
        public static class Load
        {
            public static bool categories(TempDataDictionary TempData,out List<Category> categories)
            {
                return new SessionSaver<List<Category>>().use(TempData,out categories, AppSettings.SessionVariables.CATEGORY, (out List<Category> saveobject) =>
                {
                    using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
                    {
                        saveobject = db_manager.Categories.ToList();
                    }
                });
            }
            public static bool categoriesCombobox(TempDataDictionary TempData, out List<SelectListItem> categoriesCombobox)
            {
                return new SessionSaver<List<SelectListItem>>().use(TempData,out categoriesCombobox, AppSettings.SessionVariables.CATEGORYCOMBOBOX, (out List<SelectListItem> saveobject) =>
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
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="Session"></param>
            /// <param name="includeCategories">load categories by default as most operations require it</param>
            /// <returns></returns>
            public static bool products(TempDataDictionary TempData, out List<Product> products, bool includeCategories = true)
            {
                return new SessionSaver<List<Product>>().use(TempData,out products, AppSettings.SessionVariables.PRODUCT, (out List<Product> saveobject) =>
                {
                    using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
                    {
                        if (includeCategories)
                        {
                            saveobject = db_manager.Products.Include(AppSettings.Includes.Category).ToList();
                        }
                        else
                        {
                            saveobject = db_manager.Products.ToList();
                        }
                    }
                });
            }

            public static bool transactions(HttpSessionStateBase Session, out List<Transaction> transactions, bool includeProduct = true)
            {
                return new SessionSaver<List<Transaction>>().use(Session,out transactions, AppSettings.SessionVariables.TRANSACTION, (out List<Transaction> saveobject) =>
                {
                    saveobject = new List<Transaction>();
                });
            }

            public static bool productsCombobox(TempDataDictionary TempData, out List<SelectListItem> productsCombobox)
            {
                return new SessionSaver<List<SelectListItem>>().use(TempData, out productsCombobox, AppSettings.SessionVariables.PRODUCTCOMBOBOX, (out List<SelectListItem> saveobject) =>
                {
                    List<SelectListItem> ProductsList = new List<SelectListItem>();
                    using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
                    {
                        foreach (var item in db_manager.Products.ToList())
                        {
                            ProductsList.Add(new SelectListItem()
                            {
                                Text = item.name,
                                Value = item.Id.ToString() //  will be used to get id later
                            });
                        }
                    }
                    saveobject = ProductsList;
                });
            }
        }

        public static class Add
        {
            public async static Task<Product> product(TempDataDictionary TempData, Product product)
            {
                // add to database
                using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
                {
                    if(product.Category == null)
                    {
                        product.Category = db_manager.Categories.FirstOrDefault(m => m.Id == product.categoryId);
                    }

                    product = db_manager.Products.Add(product);
                    await db_manager.SaveChangesAsync();
                }

                // add to session
                List<Product> products;
                if(!Load.products(TempData,out products, false))
                {
                    products.Add(product);
                }

                // add to session combobox
                List<SelectListItem> comboboxItems;
                if(!Load.productsCombobox(TempData,out comboboxItems))
                {
                    comboboxItems.Add(new SelectListItem()
                    {
                        Text = product.name,
                        Value = product.Id.ToString()
                    });
                }

                return product;
            }

            public async static Task<Category> category(TempDataDictionary TempData, Category category)
            {
                // add to database
                using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
                {
                    category = db_manager.Categories.Add(category);
                    await db_manager.SaveChangesAsync();
                }

                // add to session
                List<Category> categories;
                if(!Load.categories(TempData, out categories))
                {
                    categories.Add(category);
                }

                // add to session combobox
                List<SelectListItem> comboboxItems;
                if (!Load.categoriesCombobox(TempData,out comboboxItems))
                {
                    comboboxItems.Add(new SelectListItem()
                    {
                        Text = category.name,
                        Value = category.Id.ToString()
                    });
                }
                
                return category;
            }

            public static Transaction transaction(HttpSessionStateBase Session, Transaction transaction)
            {
                // add to session
                List<Transaction> transactions;
                if(!Load.transactions(Session,out transactions, false))
                {
                    transactions.Add(transaction);
                }

                return transaction;
            }
        }
        
        public static class Update
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="Session"></param>
            /// <param name="product">.Id must be valid</param>
            public static void product(TempDataDictionary TempData, Product product)
            {
                // Update Database
                Product db_product;
                using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
                {
                    db_product = db_manager.Products.Include(AppSettings.Includes.Category).FirstOrDefault(m => m.Id == product.Id);
                    if(db_product != null)
                    {
                        db_product.name = product.name;
                        db_product.price = product.price;
                        db_product.categoryId = product.categoryId;

                        db_manager.Entry(db_product);
                        db_manager.SaveChanges();
                    }
                }
                if(db_product != null)
                {
                    db_product.Category = product.Category;

                    // Update session
                    List<Product> products;
                    Load.products(TempData, out products);

                    Product tmpProd = products.FirstOrDefault(m => m.Id == product.Id);
                    if(tmpProd != null)
                    {
                        int indexTmpProd = products.IndexOf(tmpProd);
                        products[indexTmpProd] = db_product;
                    }

                    // Update session combobox
                    List<SelectListItem> productsCombobox;
                    Load.productsCombobox(TempData, out productsCombobox);
                    SelectListItem tmpSLI = productsCombobox.FirstOrDefault(m => m.Value == db_product.Id.ToString());
                    if (tmpSLI != null)
                    {
                        int indexTmpSLI = productsCombobox.IndexOf(tmpSLI);
                        productsCombobox[indexTmpSLI] = new SelectListItem() { Text = db_product.name, Value = db_product.Id.ToString() };
                    }
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="Session"></param>
            /// <param name="category">.Id must be valid</param>
            public static void category(TempDataDictionary TempData, Category category)
            {
                Category db_category;
                // Update Database
                using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
                {
                    db_category = db_manager.Categories.FirstOrDefault(m => m.Id == category.Id);
                    if(db_category != null)
                    {
                        db_category.name = category.name;

                        db_manager.Entry(db_category);
                        db_manager.SaveChanges();
                    }
                }

                // Update session
                List<Category> categories;
                Load.categories(TempData,out categories);
                Category tmpCategory = categories.FirstOrDefault(m => m.Id == db_category.Id);
                if (tmpCategory != null)
                {
                    int indexTmpProd = categories.IndexOf(tmpCategory);
                    categories[indexTmpProd] = db_category;
                }

                // Update session combobox
                List<SelectListItem> productsCombobox;
                Load.categoriesCombobox(TempData, out productsCombobox);
                SelectListItem tmpSLI = productsCombobox.FirstOrDefault(m => m.Value == db_category.Id.ToString());
                if (tmpSLI != null)
                {
                    int indexTmpSLI = productsCombobox.IndexOf(tmpSLI);
                    productsCombobox[indexTmpSLI] = new SelectListItem() { Text = db_category.name, Value = db_category.Id.ToString() };
                }
            }

            public static void transaction(HttpSessionStateBase Session, Transaction transaction)
            {
                // Update Database
                using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
                {
                    db_manager.Entry(transaction);
                    db_manager.SaveChanges();
                }

                // Update session
                List<Transaction> transactions;
                Load.transactions(Session,out transactions, false);
                Transaction tmpTransaction = transactions.FirstOrDefault(m => m.Id == transaction.Id);
                if (tmpTransaction != null)
                {
                    int indexTmpProd = transactions.IndexOf(tmpTransaction);
                    transactions[indexTmpProd] = transaction;
                }

                // no combobox needed
            }
        }

        public static class Remove
        {
            public async static Task product(TempDataDictionary TempData, long productId)
            {
                // remove from database
                using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
                {
                    Product product = db_manager.Products.Include(AppSettings.Includes.Category).FirstOrDefault(m => m.Id == productId);
                    if (product != null)
                    {
                        db_manager.Products.Remove(product);
                        await db_manager.SaveChangesAsync();
                    }
                }

                // remove from session
                List<Product> products;
                Load.products(TempData,out products);
                Product tmpProd = products.FirstOrDefault(m => m.Id == productId);
                if (tmpProd != null)
                {
                    products.Remove(tmpProd);
                }

                // remove from session combobox
                List<SelectListItem> comboboxItems;
                Load.productsCombobox(TempData,out comboboxItems);
                SelectListItem tmpCBI = comboboxItems.FirstOrDefault(m => m.Value == productId.ToString());
                if (tmpCBI != null)
                {
                    products.Remove(tmpProd);
                }
            }
            public async static Task product(TempDataDictionary TempData,HttpSessionStateBase Session, long productId, long transactionLinkProdId)
            {
                // remove from database
                using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
                {
                    Product product = db_manager.Products.Include(AppSettings.Includes.Category).FirstOrDefault(m => m.Id == productId);
                    if (product != null)
                    {
                        // remove transactions from products
                        List<Transaction> trans = product.Transactions.ToList();
                        Product productDef;
                        // remove transactions from products
                        for (int i = 0; i < trans.Count; i++)
                        {
                            productDef = db_manager.Products.Include(AppSettings.Includes.Category).FirstOrDefault(m => m.Id == transactionLinkProdId);
                            trans[i].productId = transactionLinkProdId;
                            trans[i].Product = productDef;

                            Update.transaction(Session, trans[i]);
                        }

                        db_manager.Products.Remove(product);
                        await db_manager.SaveChangesAsync();
                    }
                }

                // remove from session
                List<Product> products;
                Load.products(TempData,out products);
                Product tmpProd = products.FirstOrDefault(m => m.Id == productId);
                if (tmpProd != null)
                {
                    products.Remove(tmpProd);
                }

                // remove from session combobox
                List<SelectListItem> comboboxItems;
                Load.productsCombobox(TempData, out comboboxItems);
                SelectListItem tmpCBI = comboboxItems.FirstOrDefault(m => m.Value == productId.ToString());
                if (tmpCBI != null)
                {
                    products.Remove(tmpProd);
                }
            }

            public async static Task product(TempDataDictionary TempData,HttpSessionStateBase Session, long productId, long[] transactionLinkProdId)
            {
                // remove from database
                using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
                {
                    Product product = db_manager.Products.FirstOrDefault(m => m.Id == productId);
                    if(product != null)
                    {
                        // remove transactions from products
                        List<Transaction> trans = product.Transactions.ToList();
                        Product productDef;
                        // remove transactions from products
                        for (int i = 0; i < trans.Count; i++)
                        {
                            productDef = db_manager.Products.FirstOrDefault(m => m.Id == transactionLinkProdId[i]);
                            trans[i].productId = transactionLinkProdId[i];
                            trans[i].Product = productDef;

                            Update.transaction(Session, trans[i]);
                        }

                        db_manager.Products.Remove(product);
                        await db_manager.SaveChangesAsync();
                    }
                }

                // remove from session
                List<Product> products;
                Load.products(TempData,out products);
                Product tmpProd = products.FirstOrDefault(m => m.Id == productId);
                if (tmpProd != null)
                {
                    products.Remove(tmpProd);
                }

                // remove from session combobox
                List<SelectListItem> comboboxItems;
                Load.productsCombobox(TempData,out comboboxItems);
                SelectListItem tmpCBI = comboboxItems.FirstOrDefault(m => m.Value == productId.ToString());
                if (tmpCBI != null)
                {
                    products.Remove(tmpProd);
                }
            }
            public async static Task category(TempDataDictionary TempData, long categoryId)
            {
                // remove from database
                using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
                {
                    Category category = db_manager.Categories.FirstOrDefault(m => m.Id == categoryId);
                    if (category != null)
                    {
                        db_manager.Categories.Remove(category);
                        await db_manager.SaveChangesAsync();
                    }
                }
                // remove from session
                List<Category> categories;
                Load.categories(TempData, out categories);
                Category tmpCategory = categories.FirstOrDefault(m => m.Id == categoryId);
                if (tmpCategory != null)
                {
                    categories.Remove(tmpCategory);
                }

                // remove from session combobox
                List<SelectListItem> comboboxItems;
                Load.categoriesCombobox(TempData,out comboboxItems);
                SelectListItem tmpCBI = comboboxItems.FirstOrDefault(m => m.Value == categoryId.ToString());
                if (tmpCBI != null)
                {
                    categories.Remove(tmpCategory);
                }
            }

            public async static Task category(TempDataDictionary TempData, long categoryId, long productLinkCatId)
            {
                // remove from database
                using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
                {
                    Category category = db_manager.Categories.FirstOrDefault(m => m.Id == categoryId);
                    if (category != null)
                    {
                        List<Product> prod = category.Products.ToList();
                        Category categoryDef;
                        // remove products from categories
                        for (int i = 0; i < prod.Count; i++)
                        {
                            categoryDef = db_manager.Categories.FirstOrDefault(m => m.Id == productLinkCatId);
                            prod[i].categoryId = (long)productLinkCatId;
                            prod[i].Category = categoryDef;

                            Update.product(TempData, prod[i]);
                        }

                        db_manager.Categories.Remove(category);
                        await db_manager.SaveChangesAsync();
                    }
                }
                // remove from session
                List<Category> categories;
                Load.categories(TempData,out categories);
                Category tmpCategory = categories.FirstOrDefault(m => m.Id == categoryId);
                if (tmpCategory != null)
                {
                    categories.Remove(tmpCategory);
                }

                // remove from session combobox
                List<SelectListItem> comboboxItems;
                Load.categoriesCombobox(TempData, out comboboxItems);
                SelectListItem tmpCBI = comboboxItems.FirstOrDefault(m => m.Value == categoryId.ToString());
                if (tmpCBI != null)
                {
                    categories.Remove(tmpCategory);
                }
            }

            public async static Task<bool> category(TempDataDictionary TempData, long categoryId, long[] productLinkCatId)
            {
                // remove from database
                using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
                {
                    Category category = db_manager.Categories.FirstOrDefault(m => m.Id == categoryId);
                    if (category != null)
                    {
                        List<Product> prod = category.Products.ToList();
                        Category categoryDef;
                        long tmpCat;
                        // remove products from categories
                        for (int i = 0; i < prod.Count; i++)
                        {
                            tmpCat = productLinkCatId[i];
                            categoryDef = db_manager.Categories.FirstOrDefault(m => m.Id == tmpCat);
                            prod[i].categoryId = (long)productLinkCatId[i];
                            prod[i].Category = categoryDef;

                            Update.product(TempData, prod[i]);
                        }

                        db_manager.Categories.Remove(category);
                        await db_manager.SaveChangesAsync();
                    }
                }

                // remove from session
                List<Category> categories;
                Load.categories(TempData, out categories);
                Category tmpCategory = categories.FirstOrDefault(m => m.Id == categoryId);
                if (tmpCategory != null)
                {
                    categories.Remove(tmpCategory);
                }

                // remove from session combobox
                List<SelectListItem> comboboxItems;
                Load.categoriesCombobox(TempData, out comboboxItems);
                SelectListItem tmpCBI = comboboxItems.FirstOrDefault(m => m.Value == categoryId.ToString());
                if (tmpCBI != null)
                {
                    categories.Remove(tmpCategory);
                }
                return true;
            }
            public static void transaction(HttpSessionStateBase Session, int index)
            {
                // add to session
                List<Transaction> transactions;
                if (!Load.transactions(Session, out transactions, false))
                {
                    transactions.RemoveAt(index);
                }
            }
        }
    }
}