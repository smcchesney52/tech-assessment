using System;

namespace Sogeti.TechAssessment.Orders.Domain
{
    public class OrderItem : EditableEntity
    {
        internal OrderItem() : base()
        {
        }

        internal OrderItem(Guid orderId, Guid productId, int itemNumber, int quantity, decimal unitPrice,
            string addUser, DateTimeOffset? addDate = null) : base(addUser, addDate)
        {
            ValidateGuid(orderId, "Order ID");
            ValidateGuid(productId, "Product ID");
            ValidateValueRange(itemNumber, "Order Item Number", 1, int.MaxValue);
            ValidateValueRange(quantity, "Order Item Quantity", 1, int.MaxValue);
            ValidateValueRange(unitPrice, "Order Item Unit Price", 0.01m, decimal.MaxValue);

            OrderId = orderId;
            ProductId = productId;
            ItemNumber = itemNumber;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }

        public Guid OrderId { get; private set; }
        
        public Guid ProductId { get; private set; }
        
        public int ItemNumber { get; internal set; }
        
        public int Quantity { get; private set; }
        
        public decimal UnitPrice { get; private set; }

        public decimal TotalPrice => Quantity * UnitPrice;

        public void UpdateQuantity(int quantity, string updateUser, DateTimeOffset? updateDate = null)
        {
            ValidateValueRange(quantity, "Order Item Quantity", 1, int.MaxValue);
            Quantity = quantity;
            MarkUpdated(updateUser, updateDate);
        }

        public void UpdateUnitPrice(decimal unitPrice, string updateUser, DateTimeOffset? updateDate = null)
        {
            ValidateValueRange(unitPrice, "Order Item Unit Price", 0.01m, decimal.MaxValue);
            UnitPrice = unitPrice;
            MarkUpdated(updateUser, updateDate);
        }
    }
}