using CofeeShop.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CofeeShop.DAL
{
    public class Dal: DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().ToTable("usersTbl");
            modelBuilder.Entity<Item>().ToTable("itemsTbl");
            modelBuilder.Entity<Order>().ToTable("ordersTbl");
            modelBuilder.Entity<Table>().ToTable("tablesTbl");
            modelBuilder.Entity<Guest>().ToTable("GuestTbl");
            modelBuilder.Entity<Today>().ToTable("todayTbl");
        }
        public DbSet<User> users { get; set; }
        public DbSet<Item> items { get; set; }
        public DbSet<Order> orders { get; set; }
        public DbSet<Table> tables { get; set; }
        public DbSet<Guest> guests { get; set; }
        public DbSet<Today> todays { get; set; }
    }
}