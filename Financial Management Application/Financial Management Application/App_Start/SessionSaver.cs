using Financial_Management_Application.Models;
using System;
using System.Collections;
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
        public T use(HttpSessionStateBase Session, string sessionVarName, useSessionFunc methodsetObject)
        {
            T savedObject = default(T);
            object sessionVar = Session[sessionVarName];

            if ((sessionVar == null || !(sessionVar.GetType() == typeof(T))) && methodsetObject != null)
            {
                methodsetObject(out savedObject); // set saved to value
                Session.Add(sessionVarName, savedObject);
            }
            else
                savedObject = (T)Session[sessionVarName];

            return savedObject;
        }

        public T use(TempDataDictionary ViewData, string sessionVarName, useSessionFunc methodsetObject)
        {
            T savedObject = default(T);
            object sessionVar = ViewData[sessionVarName];

            if ((sessionVar == null || !(sessionVar.GetType() == typeof(T))) && methodsetObject != null)
            {
                methodsetObject(out savedObject); // set saved to value
                ViewData.Add(sessionVarName, savedObject);
            }
            else
                savedObject = (T)ViewData[sessionVarName];

            return savedObject;
        }

        #endregion
    }
    public static class SessionSaver
    {
        public static class Load
        {
            public static List<Category> categories(TempDataDictionary Session)
            {
                return new SessionSaver<List<Category>>().use(Session, AppSettings.SessionVariables.CATEGORY, (out List<Category> saveobject) =>
                {
                    using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
                    {
                        saveobject = db_manager.Categories.ToList();
                    }
                });
            }
            public static List<SelectListItem> categoriesCombobox(TempDataDictionary Session)
            {
                return new SessionSaver<List<SelectListItem>>().use(Session, AppSettings.SessionVariables.CATEGORYCOMBOBOX, (out List<SelectListItem> saveobject) =>
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

            public static List<Product> products(TempDataDictionary Session, bool includeCategories = false)
            {
                return new SessionSaver<List<Product>>().use(Session, AppSettings.SessionVariables.PRODUCT, (out List<Product> saveobject) =>
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

            public static List<Transaction> transactions(TempDataDictionary Session)
            {
                return new SessionSaver<List<Transaction>>().use(Session, AppSettings.SessionVariables.TRANSACTION, (out List<Transaction> saveobject) =>
                {
                    using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
                    {
                        saveobject = db_manager.Transactions.ToList();
                    }
                });
            }

            public static List<SelectListItem> productsCombobox(TempDataDictionary Session)
            {
                return new SessionSaver<List<SelectListItem>>().use(Session, AppSettings.SessionVariables.PRODUCTCOMBOBOX, (out List<SelectListItem> saveobject) =>
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
            public async static Task<Product> product(TempDataDictionary Session, Product product)
            {
                // add to database
                using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
                {
                    product = db_manager.Products.Add(product);
                    await db_manager.SaveChangesAsync();
                }

                // add to session
                List<Product> products = Load.products(Session);
                products.Add(product);

                // add to session combobox
                List<SelectListItem> comboboxItems = Load.productsCombobox(Session);
                comboboxItems.Add(new SelectListItem()
                {
                    Text = product.name,
                    Value = product.Id.ToString()
                });
                return product;
            }

            public async static Task<Category> category(TempDataDictionary Session, Category category)
            {
                // add to database
                using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
                {
                    category = db_manager.Categories.Add(category);
                    await db_manager.SaveChangesAsync();
                }

                // add to session
                List<Category> categories = Load.categories(Session);
                categories.Add(category);

                // add to session combobox
                List<SelectListItem> comboboxItems = Load.categoriesCombobox(Session);
                comboboxItems.Add(new SelectListItem()
                {
                    Text = category.name,
                    Value = category.Id.ToString()
                });
                return category;
            }
        }
        
        public static class Update
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="Session"></param>
            /// <param name="product">.Id must be valid</param>
            public static void product(TempDataDictionary Session, Product product)
            {
                // Update Database
                using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
                {
                    //Product prd = db_manager.Products.FirstOrDefault(m => m.Id == product.Id);
                    db_manager.Entry(product);
                    db_manager.SaveChanges();
                }

                // Update session
                List<Product> products = Load.products(Session);
                Product tmpProd = products.FirstOrDefault(m => m.Id == product.Id);
                if(tmpProd != null)
                {
                    int indexTmpProd = products.IndexOf(tmpProd);
                    products[indexTmpProd] = product;
                }

                // Update session combobox
                List<SelectListItem> productsCombobox = Load.productsCombobox(Session);
                SelectListItem tmpSLI = productsCombobox.FirstOrDefault(m => m.Value == product.Id.ToString());
                if (tmpSLI != null)
                {
                    int indexTmpSLI = productsCombobox.IndexOf(tmpSLI);
                    productsCombobox[indexTmpSLI] = new SelectListItem() { Text = product.name, Value = product.Id.ToString() };
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="Session"></param>
            /// <param name="category">.Id must be valid</param>
            public static void category(TempDataDictionary Session, Category category)
            {
                // Update Database
                using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
                {
                    db_manager.Entry(category);
                    db_manager.SaveChanges();
                }

                // Update session
                List<Category> categories = Load.categories(Session);
                Category tmpCategory = categories.FirstOrDefault(m => m.Id == category.Id);
                if (tmpCategory != null)
                {
                    int indexTmpProd = categories.IndexOf(tmpCategory);
                    categories[indexTmpProd] = category;
                }

                // Update session combobox
                List<SelectListItem> productsCombobox = Load.categoriesCombobox(Session);
                SelectListItem tmpSLI = productsCombobox.FirstOrDefault(m => m.Value == category.Id.ToString());
                if (tmpSLI != null)
                {
                    int indexTmpSLI = productsCombobox.IndexOf(tmpSLI);
                    productsCombobox[indexTmpSLI] = new SelectListItem() { Text = category.name, Value = category.Id.ToString() };
                }
            }

            public static void transaction(TempDataDictionary Session, Transaction transaction)
            {
                // Update Database
                using (FM_Datastore_Entities_EF db_manager = new FM_Datastore_Entities_EF())
                {
                    db_manager.Entry(transaction);
                    db_manager.SaveChanges();
                }

                // Update session
                List<Transaction> transactions = Load.transactions(Session);
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
            public async static Task product(TempDataDictionary Session, long productId, long[] transactionLinkProdId)
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
                List<Product> products = Load.products(Session);
                Product tmpProd = products.FirstOrDefault(m => m.Id == productId);
                if (tmpProd != null)
                {
                    products.Remove(tmpProd);
                }

                // remove from session combobox
                List<SelectListItem> comboboxItems = Load.productsCombobox(Session);
                SelectListItem tmpCBI = comboboxItems.FirstOrDefault(m => m.Value == productId.ToString());
                if (tmpCBI != null)
                {
                    products.Remove(tmpProd);
                }
            }
            public async static Task category(TempDataDictionary Session, long categoryId)
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
            }

            public async static Task category(TempDataDictionary Session, long categoryId, long productLinkCatId)
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

                            Update.product(Session, prod[i]);
                        }

                        db_manager.Categories.Remove(category);
                        await db_manager.SaveChangesAsync();
                    }
                }
            }

            public async static Task<bool> category(TempDataDictionary Session, long categoryId, long[] productLinkCatId)
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

                            Update.product(Session, prod[i]);
                        }

                        db_manager.Categories.Remove(category);
                        await db_manager.SaveChangesAsync();
                    }
                }

                // remove from session
                List<Category> categories = Load.categories(Session);
                Category tmpCategory = categories.FirstOrDefault(m => m.Id == categoryId);
                if (tmpCategory != null)
                {
                    categories.Remove(tmpCategory);
                }

                // remove from session combobox
                List<SelectListItem> comboboxItems = Load.categoriesCombobox(Session);
                SelectListItem tmpCBI = comboboxItems.FirstOrDefault(m => m.Value == categoryId.ToString());
                if (tmpCBI != null)
                {
                    categories.Remove(tmpCategory);
                }
                return true;
            }
        }
    }
}