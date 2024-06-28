using System;

namespace Sogeti.TechAssessment.Orders.Models
{
    public class UpdateOrderModel
    {
        public Guid OrderId { get; set; }
        
        public UpdateOrderOrderItemModel[] Items { get; set; }
        
        public string UpdateUser { get; set; }
    }
}