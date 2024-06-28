using System;

namespace Sogeti.TechAssessment.Orders.Domain
{
    public class Customer : EditableEntity
    {
        public Customer() : base()
        {
        }

        public Customer(Guid customerId, string firstName, string lastName, string addUser, DateTimeOffset? addDate = null) 
            : base(addUser, addDate)
        {
            ValidateGuid(customerId, "Customer ID");
            var resolvedFirstName = ValidateRequiredStringAndLength(firstName,
                DomainConstants.CustomerFirstNameMaxLength, "Customer First Name");
            var resolvedLastName = ValidateRequiredStringAndLength(lastName,
                DomainConstants.CustomerLastNameMaxLength, "Customer Last Name");

            Id = customerId;
            FirstName = resolvedFirstName;
            LastName = resolvedLastName;
        }

        public Customer(Guid customerId, string name, string addUser, DateTimeOffset? addDate = null) 
            : base(addUser, addDate)
        {
            ValidateGuid(customerId, "Customer ID");
            var resolvedLastName = ValidateRequiredStringAndLength(name,
                DomainConstants.CustomerLastNameMaxLength, "Customer Name");

            Id = customerId;
            FirstName = null;
            LastName = resolvedLastName;
        }
        
        public string? FirstName { get; set; }
        
        public string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}