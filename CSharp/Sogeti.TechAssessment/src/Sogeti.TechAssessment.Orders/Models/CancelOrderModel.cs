using System;

namespace Sogeti.TechAssessment.Orders.Models
{
    public class CancelOrderModel
    {
        public Guid OrderId { get; set; }
        
        public string CancelUser { get; set; }
    }
}