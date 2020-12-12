using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkAndNetCore
{
    public class ShopContext : DbContext{

        DbSet<Product> Products { get; set; } 
        DbSet<Category> Categories {get;set;}

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=shop.db");

    }

    public class Product {
         public int Id { get; set; } 
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        
        [Required]    
        public double Price { get; set; }
    }
    public class Category {
         public int Id { get; set; } 
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
    }

    class Program {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}
