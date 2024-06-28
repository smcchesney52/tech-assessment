using System;
using System.Collections.Generic;
using System.Linq;
using Sogeti.TechAssessment.Orders.Domain.Enums;

namespace Sogeti.TechAssessment.Orders.Domain
{
    public class Order : EditableEntity
    {
        private readonly List<OrderItem> _items = new();
        
        public Order() : base()
        {
        }

        public Order(Guid customerId, DateTimeOffset orderDate, string addUser, DateTimeOffset? addDate = null) 
            : base(addUser, addDate)
        {
            ValidateGuid(customerId, "Customer ID");
            ValidateDateTimeOffset(orderDate, "Order Date");

            CustomerId = customerId;
            OrderDate = orderDate;
            Status = OrderStatus.Submitted;
        }
        
        public Guid CustomerId { get; private set; }
        
        public DateTimeOffset OrderDate { get; private set; }
        
        public OrderStatus Status { get; private set; }

        public decimal OrderTotal => _items.Sum(i => i.TotalPrice);

        public IEnumerable<OrderItem> Items => _items;

        public void AddItem(Guid productId, int quantity, decimal unitPrice, string addUser,
            DateTimeOffset? addDate = null)
        {
            var itemNumber = (byte)(_items.Count + 1);
            _items.Add(new OrderItem(Id, productId, itemNumber, quantity, unitPrice, addUser, addDate));
        }

        public void RemoveItem(Guid orderItemId)
        {
            var index = _items.FindIndex(i => i.Id == orderItemId);
            if (index != -1)
            {
                _items.RemoveAt(index);
            }
        }

        public void Cancel(string updateUser, DateTimeOffset? updateDate = null)
        {
            if (Status == OrderStatus.Shipped)
            {
                throw new InvalidOperationException("Cannot cancel order - already shipped");
            }

            Status = OrderStatus.Cancelled;
            MarkUpdated(updateUser, updateDate);
        }
    }
}