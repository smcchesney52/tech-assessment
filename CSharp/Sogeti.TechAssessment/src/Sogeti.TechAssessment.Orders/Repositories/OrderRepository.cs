using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sogeti.TechAssessment.Orders.Domain;
using Sogeti.TechAssessment.Orders.Interfaces.Repositories;

namespace Sogeti.TechAssessment.Orders.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private static readonly List<Customer> _customers = new List<Customer>(3)
        {
            new Customer(Guid.Parse("1c9b9033-5211-4ecd-bd6d-9f6b36f5ec82"), "John", "Doe", "SYSTEM"),
            new Customer(Guid.Parse("58b0b0aa-ba35-4576-992e-db93965492c2"), "Jane", "Smith", "SYSTEM"),
            new Customer(Guid.Parse("fd7cc737-e9e5-4039-996d-6fd956303b43"), "Prince", "SYSTEM")
        };

        private static readonly List<Order> _orders = [];
        
        public Task<Order[]> GetByCustomerAsync(Guid customerId)
        {
            var customer = _customers.SingleOrDefault(c => c.Id == customerId);
            if (customer is null)
            {
                return Task.FromResult(Array.Empty<Order>());
            }

            var orders = _orders.Where(o => o.CustomerId == customerId).ToList();
            orders.ForEach(o => o.Customer = customer);
            return Task.FromResult(orders.ToArray());
        }

        public Task<Order?> GetById(Guid orderId)
        {
            var order = _orders.SingleOrDefault(o => o.Id == orderId);
            if (order is not null)
            {
                order.Customer = _customers.Single(c => c.Id == order.CustomerId);
            }

            return Task.FromResult(order);
        }

        public Task AddAsync(Order order)
        {
            _orders.Add(order);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Order order)
        {
            _orders.Remove(order);
            _orders.Add(order);
            return Task.CompletedTask;
        }
    }
}