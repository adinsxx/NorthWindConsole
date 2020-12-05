using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
namespace NorthwindConsole
{
    public class NorthwindContext : DbContext
    {
        public DbSet<Categories> Categories {get; set;}
        public DbSet<Products> Products {get; set;}

    //Category CRUD
        public void AddCategory(Categories categories)
        {
            this.Categories.Add(categories);
            this.SaveChanges();
        }

        public void EditCategory(Categories UpdatedCategories)
        {
            Categories categories = this.Categories.Find(UpdatedCategories.CategoryID);
            categories.CategoryName = UpdatedCategories.CategoryName;
            this.SaveChanges();
        }

        public void DeleteCategory(Categories categories)
        {
            this.Categories.Remove(categories);
            this.SaveChanges();
        }

    //Product CRUD

        public void AddProduct(Products products)
        {
            this.Products.Add(products);
            this.SaveChanges();
        }

        public void EditProduct(Products UpdatedProducts)
        {
            Products products = this.Products.Find(UpdatedProducts.ProductID);
            products.ProductName = UpdatedProducts.ProductName;
            this.SaveChanges();
        }

        public void DeleteProduct(Products products)
        {
            this.Products.Remove(products);
            this.SaveChanges();
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();
            optionsBuilder.UseSqlServer(@config["BloggingContext:ConnectionString"]);
        }
    }
}