using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace ShoppingApp.Models
{
    public partial class ShoppingDbContext : DbContext
    {
        public ShoppingDbContext()
        {
        }

        public ShoppingDbContext(DbContextOptions<ShoppingDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Cart> Carts { get; set; }

        public virtual DbSet<Category> Categories { get; set; }

        public virtual DbSet<Order> Orders { get; set; }

        public virtual DbSet<OrderItem> OrderItems { get; set; }

        public virtual DbSet<Payment> Payments { get; set; }

        public virtual DbSet<Product> Products { get; set; }

        public virtual DbSet<Review> Reviews { get; set; }

        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasKey(e => e.CartId);
                entity.ToTable("Cart");
                entity.Property(e => e.CartId).HasColumnName("CartID");
                entity.Property(e => e.ProductId).HasColumnName("ProductID");
                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Product).WithMany(p => p.Carts)
                    .HasForeignKey(d => d.ProductId);

                entity.HasOne(d => d.User).WithMany(p => p.Carts)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.CategoryId);
                entity.Property(e => e.CategoryId).HasColumnName("Id");
                entity.Property(e => e.CategoryName).HasColumnName("Name").HasMaxLength(100).IsUnicode(false);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.OrderId);
                entity.Property(e => e.OrderId).HasColumnName("Id");
                entity.Property(e => e.OrderDate).HasColumnType("datetime");
                entity.Property(e => e.Status).HasMaxLength(50).IsUnicode(false);
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.User).WithMany(p => p.Orders)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => e.OrderItemId);
                entity.Property(e => e.OrderItemId).HasColumnName("Id");
                entity.Property(e => e.OrderId).HasColumnName("OrderID");
                entity.Property(e => e.Price).HasColumnName("UnitPrice").HasColumnType("decimal(10, 2)");
                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.OrderId);

                entity.HasOne(d => d.Product).WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.ProductId);
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.PaymentId);
                entity.Property(e => e.PaymentId).HasColumnName("Id");
                entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.OrderId).HasColumnName("OrderID");
                entity.Property(e => e.PaymentDate).HasColumnName("PaidAt").HasColumnType("datetime");
                entity.Property(e => e.PaymentMethod).HasColumnName("Method").HasMaxLength(50).IsUnicode(false);
                entity.Property(e => e.PaymentStatus).HasColumnName("Status").HasMaxLength(50).IsUnicode(false);

                entity.HasOne(d => d.Order).WithMany(p => p.Payments)
                    .HasForeignKey(d => d.OrderId);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.ProductId);
                entity.Property(e => e.ProductId).HasColumnName("Id");
                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
                entity.Property(e => e.Description).HasMaxLength(255).IsUnicode(false);
                entity.Property(e => e.Name).HasMaxLength(100).IsUnicode(false);
                entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");

                entity.HasOne(d => d.Category).WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.UserId).HasColumnName("Id");
                entity.Property(e => e.Address).HasMaxLength(255).IsUnicode(false);
                entity.Property(e => e.Email).HasMaxLength(100).IsUnicode(false);
                entity.Property(e => e.FirstName).HasMaxLength(100);
                entity.Property(e => e.LastName).HasMaxLength(100);
                entity.Property(e => e.Password).HasColumnName("PasswordHash").HasMaxLength(100).IsUnicode(false);
                entity.Property(e => e.Phone).HasColumnName("PhoneNumber").HasMaxLength(15).IsUnicode(false);
                entity.Ignore(e => e.Name);
                entity.Property(e => e.Role).HasMaxLength(50).IsUnicode(false).HasDefaultValue("Customer");
            });

            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("Reviews");
                entity.Property(e => e.Rating).IsRequired();
                entity.Property(e => e.Comment).HasMaxLength(1000);
                entity.Property(e => e.CreatedAt).HasColumnType("datetime").HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Product).WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Ensure a user can only review a product once
                entity.HasIndex(e => new { e.ProductId, e.UserId }).IsUnique();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
