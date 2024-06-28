using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sogeti.TechAssessment.Orders.Domain;
using Sogeti.TechAssessment.Orders.Interfaces.Repositories;
using Sogeti.TechAssessment.Orders.Interfaces.Services;
using Sogeti.TechAssessment.Orders.Models;

namespace Sogeti.TechAssessment.Orders.Services
{
    public class OrderService : IOrderService
    {
        private readonly ILogger<OrderService> _logger;
        private readonly IOrderRepository _orderRepository;

        public OrderService(ILogger<OrderService> logger, IOrderRepository orderRepository)
        {
            _logger = logger;
            _orderRepository = orderRepository;
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

                await _orderRepository.AddAsync(newOrder);
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
            var orders = await _orderRepository.GetByCustomerAsync(customerId);
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
                var order = await _orderRepository.GetById(model.OrderId);
                if (order is not null)
                {
                    UpdateOrderFromModel(order, model);
                    await _orderRepository.UpdateAsync(order);
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
                var order = await _orderRepository.GetById(model.OrderId);
                if (order is not null)
                {
                    order.Cancel(model.CancelUser);
                    await _orderRepository.UpdateAsync(order);
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