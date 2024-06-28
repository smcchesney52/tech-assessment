using System;

namespace Sogeti.TechAssessment.Orders.Models
{
    public class UpdateOrderOrderItemModel
    {
        public Guid? OrderItemId { get; set; }
        
        public Guid? ProductId { get; set; }
        
        public bool Delete { get; set; }
        
        public int Quantity { get; set; }
        
        public decimal UnitPrice { get; set; }
    }
}