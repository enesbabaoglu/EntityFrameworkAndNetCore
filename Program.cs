using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EntityFrameworkAndNetCore
{
    public class ShopContext : DbContext
    {

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }


        public static readonly ILoggerFactory MyLoggerFactory
        = LoggerFactory.Create(builder => { builder.AddConsole(); });

        protected override void OnConfiguring(DbContextOptionsBuilder options)
         => options
             .UseLoggerFactory(MyLoggerFactory)
             .UseSqlServer("Data Source=DESKTOP-AJT2GI5; Initial Catalog=ShopDb;Integrated Security=SSPI;");
            //  .UseSqlite("Data Source=shop.db");

    }

    public class Product
    {

        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public double Price { get; set; }

        public int CategoryId { get; set; } 
    }

    public class Category
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
    }

    public class Order
    {
        public int Id { get; set; }
        public string ProductId { get; set; }

        public DateTime createDate { get; set; }
    }
    public class User
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string UserName { get; set; }

       [Required]
        public string Email { get; set; }
    }  
    
    public class Adress    
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
    }
    
    class Program
    {
        static void Main(string[] args)
        {

            AddProducts();

        }

        static void DeleteProduct(){
                using(var context = new ShopContext()){
                var product = context.Products.FirstOrDefault(p => p.Id == 1);
                if (product != null)
                {
                    context.Remove(product);
                    context.SaveChanges();
                }
            }
        }
        static void UpdateProduct()
        {
                using(var context = new ShopContext()){
                var product = context.Products.Where(p => p.Id == 1).FirstOrDefault();
                if (product != null)
                {
                    product.Price *= 1.2;
                    context.Update(product);
                    context.SaveChanges();
                }
            }
            // using(var context = new ShopContext()){
            //   var entity =new Product{Id=1};
                
            //   context.Attach(entity);
            //   entity.Price=3000;
            //   context.SaveChanges();
            // }
            // using(var context = new ShopContext()){
            //     var product = context.Products.Where(p => p.Id == 1).FirstOrDefault();
            //     if (product != null)
            //     {
            //         product.Price *= 1.2;
            //         context.SaveChanges();
            //     }
            // }

        }
        static void GetAllProducts()
        {
            using(var context = new ShopContext()){

                var products = context
                                .Products
                                .Select(p => new {
                                    p.Name,
                                    p.Price
                                })
                                .ToList(); 
                foreach (var p in products)
                {
                      Console.WriteLine ($"Name = {p.Name} Price = {p.Price}");
                }             
            }
        }
        static void GetProductByName (string name){

            using(var context = new ShopContext()){

                var products = context
                                .Products
                                .Where(p => p.Name.Contains(name))
                                .Select(p => new {
                                    p.Name,
                                    p.Price
                                })
                                .ToList(); 
                foreach (var p in products)
                {
                      Console.WriteLine ($"Name = {p.Name} Price = {p.Price}");
                }             
            }
        }
        static void GetProductById(int id)
        {
            using(var context = new ShopContext()){

                var product = context
                                .Products
                                .Where(p => p.Id==id)
                                .Select(p => new {
                                    p.Name,
                                    p.Price
                                })
                                .First(); 
                
                Console.WriteLine ($"Name = {product.Name}");
            }
        }
        static void AddProducts()
        {
            using (var db = new ShopContext())
            {
                List<Product> products = new List<Product>{
                    new Product{Name="samsung2", Price=2000},
                    new Product{Name="samsung3", Price=3000},
                    new Product{Name="samsung4", Price=4000},
                    new Product{Name="samsung5", Price=5000},
                    new Product{Name="samsung6", Price=6000}
                };

                db.AddRange(products);
                db.SaveChanges();
            }
        }
        static void AddProduct()
        {
            using (var db = new ShopContext())
            {
                Product product = new Product { Name = "samsung7", Price = 2000 };
                db.Add(product);
                db.SaveChanges();
            }
        }
    }
}
