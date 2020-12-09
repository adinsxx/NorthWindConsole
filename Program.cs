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
                    Console.WriteLine("3) Display Category and related products");
                    Console.WriteLine("4) Display Products based on Category choice");
                    Console.WriteLine("5) Add Product");
                    Console.WriteLine("6) Edit Product");
                    Console.WriteLine("7) Display all Products");
                    Console.WriteLine("8) Display specific product");
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
                        
                    }
                    //1.c  Display all records in the Products table (ProductName only) - user decides if they want to see all products, discontinued products, or active (not discontinued) products. Discontinued products should be distinguished from active products.

                    else if (choice == "7")
                    {

                    }
                    //1.d Display a specific Product (all product fields should be displayed)
                    else if (choice == "8")
                    {

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
            Categories categories = new Categories();
            Console.WriteLine("Enter Product Name");
            products.ProductName = Console.ReadLine();
            Console.WriteLine("Enter Related Category");
            var query = db.Categories.OrderBy(p => p.CategoryName);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{query.Count()} records returned");
            Console.ForegroundColor = ConsoleColor.Magenta;
            foreach (var item in query)
                {
                    Console.WriteLine($"{item.CategoryName} - {item.Description}");
                }
            categories.CategoryName = Console.ReadLine();
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