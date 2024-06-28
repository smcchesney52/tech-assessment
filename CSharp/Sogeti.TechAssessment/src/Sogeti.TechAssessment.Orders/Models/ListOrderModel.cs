using System;

namespace Sogeti.TechAssessment.Orders.Models
{
    public class ListOrderModel
    {
        public Guid Id { get; set; }
        
        public string CustomerName { get; set; }
        
        public DateTimeOffset OrderDate { get; set; }
        
        public decimal OrderTotal { get; set; }
        
        public string CurrentStatus { get; set; }
    }
}