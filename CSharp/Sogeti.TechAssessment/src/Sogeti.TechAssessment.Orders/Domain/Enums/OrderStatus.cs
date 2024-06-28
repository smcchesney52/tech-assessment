namespace Sogeti.TechAssessment.Orders.Domain.Enums
{
    public enum OrderStatus
    {
        Invalid = 0,
        Submitted,
        InProcess,
        Backordered,
        Shipped,
        Cancelled
    }
}