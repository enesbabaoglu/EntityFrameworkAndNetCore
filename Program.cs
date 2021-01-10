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
        public DbSet<Customer> Customers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Address> Addresses { get; set; }


        public static readonly ILoggerFactory MyLoggerFactory
        = LoggerFactory.Create(builder => { builder.AddConsole(); });

        protected override void OnConfiguring(DbContextOptionsBuilder options)
         => options
             .UseLoggerFactory(MyLoggerFactory)
             .UseSqlServer("Data Source=DESKTOP-AJT2GI5; Initial Catalog=ShopDb;Integrated Security=SSPI;");
        //  .UseSqlite("Data Source=shop.db");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductCategory>()  // Product Category tablosunda İki adet primary key olduğunu belirtiyoruz
                        .HasKey(t=> new {t.ProductId,t.CategoryId});
            
            modelBuilder.Entity<ProductCategory>()
                        .HasOne(pc=>pc.Product)
                        .WithMany(p=>p.ProductCategories)
                        .HasForeignKey(pc=>pc.ProductId);
                                                         //İki Id ninde (ProductId,CategoryId) çoklu olabileceğini ve foreign key olduğunu belirtiyoruz
            modelBuilder.Entity<ProductCategory>()
                        .HasOne(pc=>pc.Category)
                        .WithMany(c=>c.ProductCategories)
                        .HasForeignKey(pc=>pc.CategoryId);
        }

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
        
        public List<ProductCategory> ProductCategories { get; set; }
    }

    public class Category
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        public List<ProductCategory> ProductCategories { get; set; }
    }

    public class ProductCategory {
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int CategoryId { get; set; } 
        public Category Category { get; set; } 
    }

    public class User
    {
        //[Key] Eğer Id kolonunu farklı isimde örneğin "KullanıcıId " tanımlamak istersek başına bundan eklenemek gerekir.
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string UserName { get; set; }

        [Required]
        public string Email { get; set; }
        public List<Address> Addresses { get; set; } // For One to many relation -many side 
        public Customer Customer { get; set; } //One to one relation 
    }

    public class Address
    {
        public int Id { get; set; }

        [Required] //vb. data annotations denir.  
        [MaxLength(100)]
        public string FullName { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }

        public User User { get; set; } //navigation property - One to many relation - One side
        public int? UserId { get; set; }
    }
    public class Customer
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string IdentityNo { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public User User { get; set; } //navigation property - One to one relation 
        public int? UserId { get; set; } // FK for one to one relation 
    }
    class Program
    {
        static void Main(string[] args)
        {

            InsertProductCategory();

        }

        static void DeleteProduct()
        {
            using (var context = new ShopContext())
            {
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
            using (var context = new ShopContext())
            {
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
            using (var context = new ShopContext())
            {

                var products = context
                                .Products
                                .Select(p => new
                                {
                                    p.Name,
                                    p.Price
                                })
                                .ToList();
                foreach (var p in products)
                {
                    Console.WriteLine($"Name = {p.Name} Price = {p.Price}");
                }
            }
        }
        static void GetProductByName(string name)
        {

            using (var context = new ShopContext())
            {

                var products = context
                                .Products
                                .Where(p => p.Name.Contains(name))
                                .Select(p => new
                                {
                                    p.Name,
                                    p.Price
                                })
                                .ToList();
                foreach (var p in products)
                {
                    Console.WriteLine($"Name = {p.Name} Price = {p.Price}");
                }
            }
        }
        static void GetProductById(int id)
        {
            using (var context = new ShopContext())
            {

                var product = context
                                .Products
                                .Where(p => p.Id == id)
                                .Select(p => new
                                {
                                    p.Name,
                                    p.Price
                                })
                                .First();

                Console.WriteLine($"Name = {product.Name}");
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


    /// <summary>
    /// /////////////RELATION ONE-MANY 
    /// </summary>
    
        
        static void InsertAddresses()
        {

            var addresses = new List<Address>{
                new Address(){Body="İstanbul",FullName="Enes BABAOĞLU",Title="Ev Adresi",UserId=1},
                new Address(){Body="İstanbul",FullName="Test Test",Title="Ev Adresi",UserId=2},
                new Address(){Body="İstanbul",FullName="İnfo info",Title="İş Adresi",UserId=3},
                new Address(){Body="İstanbul",FullName="Enes Test",Title="Ev Adresi",UserId=4},
                new Address(){Body="İstanbul",FullName="info Test",Title="İş Adresi",UserId=5}
            };
            using (var context = new ShopContext())
            {
                context.AddRange(addresses);
                context.SaveChanges();
            }

        }
        static void InsertUsers()
        {

            var users = new List<User>{
                new User(){UserName="enesbabaoglu",Email="info@enesbabaoglu.com"},
                new User(){UserName="erenbabaoglu",Email="info@erenbabaoglu.com"},
                new User(){UserName="infoinfo",Email="info@info.com"},
                new User(){UserName="infotest",Email="info@test.com"},
                new User(){UserName="testtest",Email="info@etesttest.com"}
            };
            using (var context = new ShopContext())
            {
                context.AddRange(users);
                context.SaveChanges();
            }

        }

    /// <summary>
    /// /////////////RELATION ONE-ONE 
    /// </summary>
        
        static void InsertCustomer()
        { 
            
            //Customer eklemek ve user a bağlamak 

            var customer = new Customer()
            {
                FirstName = "deneme",
                LastName = "deneme",
                IdentityNo = "11111111",
                UserId = 1,
            };
            using (var context = new ShopContext())
            {
                // aynı userId ile birden fazla kayıt eklenmez one - to- one ilişki mevcut
             
                context.Add(customer); 
                context.SaveChanges();
            }
        }
        static void InsertUserAndCustomer(){ 
            
            // user insert edilirken customer a da aynı anda ekleme yapmak ve customer daki user Id ile bağlamak
            
            var user=new User(){
                UserName="deneme",
                Email="deneme@deneme.com",
                Customer=new Customer(){
                    FirstName="test",
                    LastName="test",
                    IdentityNo = "11111111"
                } 
            };
            using (var context = new ShopContext())
            {
                context.Add(user);
                context.SaveChanges();
            }


        }

    /// <summary>
    /// /////////////RELATION MANY_TO_MANY
    /// </summary>
        static void InsertProductCategory(){ //Product category tablosuna insert yapma 

            using(var db=new ShopContext()){

                var categories = new List<Category>(){
                    new Category {Name = "Telefon"},
                    new Category {Name = "Bilgisayar"},
                    new Category {Name = "Elektronik"},
                } ;

                db.AddRange (categories);
                var Ids = new int[2]{1,2};

                var p = db.Products.Find(1);

                p.ProductCategories = Ids.Select(cid => new ProductCategory(){
                    CategoryId = cid,
                    ProductId =p.Id
                }).ToList();

                db.SaveChanges();
            } 
        }
    
    }
}
