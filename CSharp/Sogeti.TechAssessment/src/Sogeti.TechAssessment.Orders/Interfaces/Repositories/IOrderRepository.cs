using System;
using System.Threading.Tasks;
using Sogeti.TechAssessment.Orders.Domain;

namespace Sogeti.TechAssessment.Orders.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task<Order[]> GetByCustomerAsync(Guid customerId);

        Task<Order?> GetById(Guid orderId);

        Task AddAsync(Order order);

        Task UpdateAsync(Order order);
    }
}