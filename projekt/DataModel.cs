using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRUD2
{
    public class Category
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }

    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }

    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductID { get; set; }
        [Required] public string ProductName { get; set; }
        public decimal Price { get; set; }
        public string Unit { get; set; }
        public int Quantity { get; set; }
        public int? CategoryID { get; set; }
        public virtual Category Category { get; set; }
    }

    public class StoreContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string conn = @"Data Source=localhost\SQLEXPRESS;Initial Catalog=GroceryStoreDB_Final;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";
            optionsBuilder.UseSqlServer(conn);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryID = 1, CategoryName = "Nabiał" },
                new Category { CategoryID = 2, CategoryName = "Warzywa" }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product { ProductID = 1, ProductName = "Mleko", Price = 3.50m, Unit = "szt", Quantity = 100, CategoryID = 1 },
                new Product { ProductID = 2, ProductName = "Marchew", Price = 2.99m, Unit = "kg", Quantity = 50, CategoryID = 2 }
            );
        }
    }
}