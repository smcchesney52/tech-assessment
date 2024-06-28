using System;

namespace Sogeti.TechAssessment.Orders.Domain
{
    public class Customer : EditableEntity
    {
        public Customer() : base()
        {
        }

        public Customer(string firstName, string lastName, string addUser, DateTimeOffset? addDate = null) 
            : base(addUser, addDate)
        {
            var resolvedFirstName = ValidateRequiredStringAndLength(firstName,
                DomainConstants.CustomerFirstNameMaxLength, "Customer First Name");
            var resolvedLastName = ValidateRequiredStringAndLength(lastName,
                DomainConstants.CustomerLastNameMaxLength, "Customer Last Name");

            FirstName = resolvedFirstName;
            LastName = resolvedLastName;
        }

        public Customer(string name, string addUser, DateTimeOffset? addDate = null) 
            : base(addUser, addDate)
        {
            var resolvedLastName = ValidateRequiredStringAndLength(name,
                DomainConstants.CustomerLastNameMaxLength, "Customer Name");

            FirstName = null;
            LastName = resolvedLastName;
        }
        
        public string? FirstName { get; set; }
        
        public string LastName { get; set; }
    }
}