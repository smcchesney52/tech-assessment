using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sogeti.TechAssessment.Orders.Domain;
using Sogeti.TechAssessment.Orders.Interfaces.Services;
using Sogeti.TechAssessment.Orders.Models;

namespace Sogeti.TechAssessment.Orders.Services
{
    public class OrderService : IOrderService
    {
        private readonly ILogger<OrderService> _logger;
        private readonly OrderContext _orderContext;

        public OrderService(ILogger<OrderService> logger, OrderContext orderContext)
        {
            _logger = logger;
            _orderContext = orderContext;
        }
        
        public async Task<Order?> CreateAsync(CreateOrderModel model)
        {
            try
            {
                var newOrder = new Order(model.CustomerId, model.OrderDate, model.AddUser);
                foreach (var item in model.Items)
                {
                    newOrder.AddItem(item.ProductId, item.Quantity, item.UnitPrice, model.AddUser);
                }

                _orderContext.Orders.Add(newOrder);
                await _orderContext.SaveChangesAsync();
                return newOrder;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not create order");
                return null;
            }
        }

        public async Task<ListOrderModel[]> GetForCustomerAsync(Guid customerId)
        {
            var orders = await _orderContext.Orders
                .Include(o => o.Customer)
                .Where(o => o.CustomerId == customerId)
                .ToArrayAsync();
            return orders.Select(o => new ListOrderModel
            {
                Id = o.Id, OrderDate = o.OrderDate, CurrentStatus = o.Status.ToString(),
                CustomerName = o.Customer.FullName, OrderTotal = o.OrderTotal
            }).ToArray();
        }

        public async Task<Order?> UpdateAsync(UpdateOrderModel model)
        {
            try
            {
                var order = await _orderContext.Orders.FindAsync(model.OrderId);
                if (order is not null)
                {
                    UpdateOrderFromModel(order, model);
                    await _orderContext.SaveChangesAsync();
                }
            
                return order;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not update order {orderId}", model.OrderId);
                return null;
            }
        }

        public async Task<Order?> CancelAsync(CancelOrderModel model)
        {
            try
            {
                var order = await _orderContext.Orders.FindAsync(model.OrderId);
                if (order is not null)
                {
                    order.Cancel(model.CancelUser);
                    await _orderContext.SaveChangesAsync();
                }

                return order;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not cancel order {orderId}", model.OrderId);
                return null;
            }
        }

        private void UpdateOrderFromModel(Order order, UpdateOrderModel model)
        {
            foreach (var updatedItem in model.Items)
            {
                if (updatedItem.OrderItemId.HasValue)
                {
                    // This is an existing item
                    if (updatedItem.Delete)
                    {
                        order.RemoveItem(updatedItem.OrderItemId.Value);
                    }
                    else
                    {
                        var existing = order.Items.SingleOrDefault(i => i.Id == updatedItem.OrderItemId.Value);
                        if (existing is null)
                        {
                            throw new ArgumentException(
                                $"Invalid Order Item data - no Item on order matching Order Item ID {updatedItem.OrderItemId}");
                        }
                    
                        existing.UpdateQuantity(updatedItem.Quantity, model.UpdateUser);
                        existing.UpdateUnitPrice(updatedItem.UnitPrice, model.UpdateUser);
                    }
                }
                else if (updatedItem.ProductId.HasValue)
                {
                    // this is a new item
                    order.AddItem(updatedItem.ProductId.Value, updatedItem.Quantity, updatedItem.UnitPrice,
                        model.UpdateUser);
                }
                else
                {
                    throw new ArgumentException("Invalid Order Item data - no Item ID or Product ID");
                }
            }
            
            order.ResetItemNumbers();
        }
    }
}