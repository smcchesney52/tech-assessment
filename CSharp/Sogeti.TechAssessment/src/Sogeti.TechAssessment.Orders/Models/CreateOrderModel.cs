using System;

namespace Sogeti.TechAssessment.Orders.Models
{
    public class CreateOrderModel
    {
        public Guid CustomerId { get; set; }
        
        public DateTimeOffset OrderDate { get; set; }
        
        public CreateOrderOrderItemModel[] Items { get; set; }
        
        public string AddUser { get; set; }
    }
}