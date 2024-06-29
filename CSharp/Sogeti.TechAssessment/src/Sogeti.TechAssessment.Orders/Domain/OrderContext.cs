using Microsoft.EntityFrameworkCore;

namespace Sogeti.TechAssessment.Orders.Domain
{
    public class OrderContext : DbContext
    {
        public OrderContext(DbContextOptions<OrderContext> options) : base(options)
        {
        }
        
        public DbSet<Customer> Customers { get; set; }
        
        public DbSet<Order> Orders { get; set; }
        
        public DbSet<OrderItem> OrderItems { get; set; }
    }
}