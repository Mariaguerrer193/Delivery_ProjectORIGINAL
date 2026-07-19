using Microsoft.EntityFrameworkCore;
using Delivery_Models;

namespace Delivery.API.Data
{
    public class DeliveryAPIContext : DbContext
    {
        public DeliveryAPIContext(DbContextOptions<DeliveryAPIContext> options)
            : base(options)
        {
        }

        public DbSet<Administrator> Administrators { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Delivery_Models.Delivery> Deliveries { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<DetailOrder> DetailOrders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<PayMethod> PayMethods { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Person> Persons { get; set; }

        // --- NUEVO ---
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<Review> Reviews { get; set; }

        public DbSet<Report> Reports { get; set; }
    }
}