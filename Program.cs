using System;
using NLog.Web;
using System.IO;
using System.Linq;
using NorthwindConsole.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace NorthwindConsole
{
    class Program
    {
        // create static instance of Logger
        private static NLog.Logger logger = NLogBuilder.ConfigureNLog(Directory.GetCurrentDirectory() + "\\nlog.config").GetCurrentClassLogger();
        static void Main(string[] args)
        {
            logger.Info("Program started");

            try
            {
                string choice;
                do
                {
                    Console.WriteLine("1) Display Categories");
                    Console.WriteLine("2) Add Category");
                    Console.WriteLine("3) Display Products based on Category choice");
                    Console.WriteLine("4) Display Category and related products");
                    Console.WriteLine("5) Add Product");
                    Console.WriteLine("6) Edit Product");
                    Console.WriteLine("7) Display all Products");
                    Console.WriteLine("8) Display specific product");
                    Console.WriteLine("9) Edit Category");
                    Console.WriteLine("10) Display all Categories and their active products");
                    Console.WriteLine("11) Display a specific Category and its active products");
                    Console.WriteLine("12) Delete Category");
                    Console.WriteLine("13) Delete Product");
                    Console.WriteLine("\"q\" to quit");
                    choice = Console.ReadLine();
                    Console.Clear();
                    logger.Info($"Option {choice} selected");

                    //Display Categories
                    if (choice == "1")
                    {
                        var db = new NorthwindConsole_32_CDMContext();
                        var query = db.Categories.OrderBy(p => p.CategoryName);

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"{query.Count()} records returned");
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryName} - {item.Description}");
                        }
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    //Add Categories
                    else if (choice == "2")
                    {
                        var db = new NorthwindConsole_32_CDMContext();
                        Categories categories = InputCategories(db);
                        if (categories != null){
                            db.AddCategory(categories);
                            logger.Info("Category added - {name}", categories.CategoryName);
                        }
                    }
                    //Displays all categories and related products
                    else if (choice == "3")
                    {
                        var db = new NorthwindConsole_32_CDMContext();
                        var query = db.Categories.OrderBy(p => p.CategoryId);

                        Console.WriteLine("Select the category whose products you want to display:");
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryId}) {item.CategoryName}");
                        }
                        Console.ForegroundColor = ConsoleColor.White;
                        int id = int.Parse(Console.ReadLine());
                        Console.Clear();
                        logger.Info($"CategoryId {id} selected");
                        Categories category = db.Categories.Include("Products").FirstOrDefault(c => c. CategoryId== id);
                        Console.WriteLine($"{category.CategoryName} - {category.Description}");
                        foreach (Product p in category.Products)
                        {
                            Console.WriteLine(p.ProductName);
                        }
                    }
                    //Displays categories to choose from. Displays products of chosen category.
                    else if (choice == "4")
                    {
                        var db = new NorthwindConsole_32_CDMContext();
                        var query = db.Categories.Include("Products").OrderBy(p => p.CategoryId);
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryName}");
                            foreach (Product p in item.Products)
                            {
                                Console.WriteLine($"\t{p.ProductName}");
                            }
                        }
                    }
                    // 1.a   Add new records to the Products table
                    else if (choice == "5")
                    {
                        var db = new NorthwindConsole_32_CDMContext();
                        Product products = InputProduct(db);
                        if (products != null){
                            db.AddProduct(products);
                            logger.Info("Product added - {name}", products.ProductName);
                        }
                    }
                    //1.b Edit a specified record from the Products table
                    else if (choice == "6")
                    {
                        Console.WriteLine("Choose the product to edit:");
                        var db = new NorthwindConsole_32_CDMContext();
                        var product = GetProducts(db);
                        if (product != null)
                        {
                            Product EditProduct = InputProduct(db);
                            if (EditProduct != null)
                            {
                                EditProduct.ProductId = product.ProductId;
                                db.EditProduct(EditProduct);
                                logger.Info($"Product (id: {product.ProductId})");
                            }
                        }
                    }
                    //1.c  Display all records in the Products table (ProductName only) - user decides if they want to see all products, discontinued products, or active (not discontinued) products. Discontinued products should be distinguished from active products.

                    else if (choice == "7")
                    {
                        string pChoice;
                        Console.WriteLine("Please choose which products you'd like to see:");
                        Console.WriteLine("1) All Products");
                        Console.WriteLine("2) Only Discontinued Products");
                        Console.WriteLine("3) Only Active Products");
                        pChoice = Console.ReadLine();


                        if (pChoice == "1")
                        {
                            var db = new NorthwindConsole_32_CDMContext();
                            var product = db.Products.OrderBy(p => p.ProductName);
                            if (product != null)
                            {
                                Console.WriteLine($"There are {product.Count()} products in the database:");
                                foreach (var allProds in product)
                                {
                                    Console.Write(allProds.ProductId+ " ");
                                    Console.WriteLine(allProds.ProductName);
                                }
                            }
                        } 
                        else if (pChoice == "2")
                        {
                            var db = new NorthwindConsole_32_CDMContext();
                            var products = db.Products.Where(p => p.Discontinued == true).OrderBy(p => p.ProductName);
                            //var discontinued = db.Products.OrderBy(d => d.Discontinued);
                            // if (discontinued != null)
                            // {
                                Console.WriteLine($"There are {products.Count()} discontinued products in the database:");
                                foreach (var p in products)
                                {

                                    Console.WriteLine(p.ProductName);

                                }
                            // }
                        }

                        else if (pChoice == "3")
                        {
                            var db = new NorthwindConsole_32_CDMContext();
                            var products = db.Products.Where(p => p.Discontinued == false).OrderBy(p => p.ProductName);
                            Console.WriteLine($"There are {products.Count()} active products in the database:");
                                foreach (var p in products)
                                {
                                    Console.WriteLine(p.ProductName);   
                                }
                        }
                    }
                    //1.d Display a specific Product (all product fields should be displayed)
                    else if (choice == "8")
                    {
                        var db = new NorthwindConsole_32_CDMContext();
                        Console.WriteLine("Type in the name of the product to display");
                        string userInput = Console.ReadLine();
                        var products = db.Products.Where(p => p.ProductName.Contains(userInput)).OrderBy(p => p.ProductName);
                        Console.WriteLine($"There are {products.Count()} products that match your entry");
                        foreach (var p in products)
                        {
                            Console.WriteLine($"{p.ProductId} - {p.ProductName} - {p.SupplierId} - {p.CategoryId} - {p.QuantityPerUnit} - {p.UnitPrice} - {p.UnitsInStock} - {p.UnitsOnOrder} - {p.ReorderLevel} - {p.Discontinued}");
                        }
                    }
                    //"9) Edit Category"
                    else if (choice == "9")
                    {
                        Console.WriteLine("Choose the category to edit:");
                        var db = new NorthwindConsole_32_CDMContext();
                        var categories = GetCategories(db);
                        if (categories != null)
                        {
                            // input blog
                            Categories editCategory = InputCategories(db);
                            if (editCategory != null)
                            {
                                editCategory.CategoryId = categories.CategoryId;
                                db.EditCategory(editCategory);
                                logger.Info($"Category (id: {categories.CategoryId}) updated");
                            }
                        }
                    }
                    
                    // /Display all Categories and their active products
                    else if (choice == "10")
                    {
                        var db = new NorthwindConsole_32_CDMContext();
                        var query = db.Categories.Include("Products").OrderBy(p => p.CategoryId);
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryName}");
                            var products = db.Products.Where(p => p.Discontinued == false).OrderBy(p => p.ProductName);
                            foreach (Product p in item.Products.Where(p => p.Discontinued == false))
                            {
                                Console.WriteLine($"\t{p.ProductName}");
                            }
                        }
                    }
                    //Display a specific Category and its active products
                    else if (choice == "11")
                    {
                        var db = new NorthwindConsole_32_CDMContext();
                        var query = db.Categories.OrderBy(p => p.CategoryId);

                        Console.WriteLine("Select the category whose products you want to display:");
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryId}) {item.CategoryName}");
                        }
                        Console.ForegroundColor = ConsoleColor.White;
                        int id = int.Parse(Console.ReadLine());
                        Console.Clear();
                        logger.Info($"CategoryId {id} selected");
                        Categories category = db.Categories.Include("Products").FirstOrDefault(c => c. CategoryId== id);
                        Console.WriteLine($"{category.CategoryName} - {category.Description}");
                        foreach (Product p in category.Products.Where(p => p.Discontinued == false))
                        {
                            Console.WriteLine(p.ProductName);
                        }
                    }
                    //Delete Category
                    else if (choice == "12")
                    {
                        Console.WriteLine("Choose Category to delete");
                        var db = new NorthwindConsole_32_CDMContext();
                        var catDelete = GetCategories(db);
                        Categories categories = new Categories();
                            if (catDelete != null)
                            {
                                db.DeleteCategory(catDelete);
                                logger.Info($"Category (id: {catDelete.CategoryId}) deleted");
                            }
                    }
                    //Delete Product
                    else if (choice == "13")
                    {
                      Console.WriteLine("Choose Product to delete");
                        var db = new NorthwindConsole_32_CDMContext();
                        var prodDelete = GetProducts(db);
                        if (prodDelete != null)
                        {
                            db.DeleteProduct(prodDelete);
                            logger.Info($"Product (id: {prodDelete.ProductId}) deleted");
                        }
                    }

                    Console.WriteLine();

                } while (choice.ToLower() != "q");
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }

            logger.Info("Program ended");
        }
//---------------------------------------------------
        //Category and product retrieval for editing
        public static Categories GetCategories(NorthwindConsole_32_CDMContext db)
        {
            // display all blogs
            var categories = db.Categories.OrderBy(c => c.CategoryId);
            foreach (Categories c in categories)
            {
                Console.WriteLine($"{c.CategoryId}: {c.CategoryName}");
            }
            if (int.TryParse(Console.ReadLine(), out int CategoryID))
            {
                Categories category = db.Categories.FirstOrDefault(c => c.CategoryId == CategoryID);
                if (category != null)
                {
                    return category;
                }
            }
            logger.Error("Invalid Blog Id");
            return null;
        }

        public static Product GetProducts(NorthwindConsole_32_CDMContext db)
        {
            // display all blogs
            var products = db.Products.OrderBy(p => p.ProductId);
            foreach (Product p in products)
            {
                Console.WriteLine($"{p.ProductId}: {p.ProductName}");
            }
            if (int.TryParse(Console.ReadLine(), out int ProductID))
            {
                Product product = db.Products.FirstOrDefault(p => p.ProductId == ProductID);
                if (product != null)
                {
                    return product;
                }
            }
            logger.Error("Invalid Blog Id");
            return null;
        }

//------------------------------------------
//Category and Product input validation
        public static Categories InputCategories(NorthwindConsole_32_CDMContext db)
        {

            Categories categories = new Categories();
            Console.WriteLine("Enter Category Name");
            categories.CategoryName = Console.ReadLine();
            Console.WriteLine("Enter Category Descrption");
            categories.Description = Console.ReadLine();

            ValidationContext context = new ValidationContext(categories, null, null);
            List<ValidationResult> results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(categories, context, results, true);
            if (isValid)
            {
                // check for unique name
                if (db.Categories.Any(c => c.CategoryName == categories.CategoryName))
                {
                    // generate validation error
                    isValid = false;
                    results.Add(new ValidationResult("Name exists", new string[] { "CategoryName" }));
                }
                else
                {
                    logger.Info("Validation passed");

                }
            }
            if (!isValid)
            {
                foreach (var result in results)
                {
                    logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                }
                return null;
            }
            return categories;
        }

        public static Product InputProduct(NorthwindConsole_32_CDMContext db)
        {

            Product products = new Product();
            Console.WriteLine("Enter Product Name");
            products.ProductName = Console.ReadLine();

            ValidationContext context = new ValidationContext(products, null, null);
            List<ValidationResult> results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(products, context, results, true);
            if (isValid)
            {
                // check for unique name
                if (db.Products.Any(p => p.ProductName == products.ProductName))
                {
                    // generate validation error
                    isValid = false;
                    results.Add(new ValidationResult("Name exists", new string[] { "ProductName" }));
                }
                else
                {
                    logger.Info("Validation passed");

                }
            }
            if (!isValid)
            {
                foreach (var result in results)
                {
                    logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                }
                return null;
            }
            return products;
        }



    }
}