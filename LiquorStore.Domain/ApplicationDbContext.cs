using LiquorStore.Domain.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiquorStore.Domain
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Category> Categories { get; set; }

        public DbSet<Liquor> Liquors { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<CartLine> CartLines { get; set; }

        public ApplicationDbContext()
            : base("DefaultConnection")
        {

        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Liquor>()
                    .HasRequired(l => l.Category)
                    .WithMany(c => c.Liquors)
                    .HasForeignKey(l => l.CategoryId);

            builder.Entity<CartLine>()
                    .HasRequired(c => c.Product)
                    .WithMany(p => p.CartLines)
                    .HasForeignKey(c => c.ProductId);

            builder.Entity<CartLine>()
                   .HasRequired(c => c.Order)
                   .WithMany(o => o.CartLines)
                   .HasForeignKey(c => c.OrderId);

            builder.Entity<Order>()
                   .HasRequired(o => o.Customer)
                   .WithMany(c => c.Orders)
                   .HasForeignKey(o => o.CustomerId);
                   
        }
    }
}
