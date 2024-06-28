using System;
using System.Threading.Tasks;
using Sogeti.TechAssessment.Orders.Domain;
using Sogeti.TechAssessment.Orders.Models;

namespace Sogeti.TechAssessment.Orders.Interfaces.Services
{
    public interface IOrderService
    {
        Task<Order?> CreateAsync(CreateOrderModel model);

        Task<ListOrderModel[]> GetForCustomerAsync(Guid customerId);

        Task<Order?> UpdateAsync(UpdateOrderModel model);

        Task<Order?> CancelAsync(CancelOrderModel model);
    }
}