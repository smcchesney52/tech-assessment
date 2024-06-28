namespace Sogeti.TechAssessment.Orders.Domain
{
    public class Customer : EditableEntity
    {
        public string? FirstName { get; set; }
        
        public string LastName { get; set; }
    }
}