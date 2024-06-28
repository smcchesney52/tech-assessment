using System;

namespace Sogeti.TechAssessment.Orders.Domain
{
    public class Product : EditableEntity
    {
        public Product() : base()
        {
        }

        public Product(string name, string description, decimal unitPrice, string addUser,
            DateTimeOffset? addDate = null)
            : base(addUser, addDate)
        {
            var resolvedName =
                ValidateRequiredStringAndLength(name, DomainConstants.ProductNameMaxLength, "Product Name");
            var resolvedDescription = ValidateRequiredStringAndLength(description,
                DomainConstants.ProductDescriptionMaxLength, "Product Description", true);
            ValidateValueRange(unitPrice, "Product Unit Price", 0.01m, decimal.MaxValue);

            Name = resolvedName;
            Description = resolvedDescription;
            UnitPrice = unitPrice;
        }
        
        public string Name { get; private set; }
        
        public string Description { get; private set; }
        
        public decimal UnitPrice { get; private set; }
    }
}