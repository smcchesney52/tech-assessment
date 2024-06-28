using System;

namespace Sogeti.TechAssessment.Orders.Models
{
    public class CreateOrderOrderItemModel
    {
        public Guid ProductId { get; set; }
        
        public int Quantity { get; set; }
        
        public decimal UnitPrice { get; set; }
    }
}