using System;

namespace Sogeti.TechAssessment.Orders.Domain
{
    public abstract class EditableEntity
    {
        protected EditableEntity()
        {
        }

        protected EditableEntity(string addUser, DateTimeOffset? addDate = null)
        {
            var resolvedAddDate = addDate.GetValueOrDefault(DateTimeOffset.Now);
            var resolvedAddUser = ValidateRequiredStringAndLength(addUser, DomainConstants.UserNameMaxLength, "Add User");
            ValidateDateTimeOffset(resolvedAddDate, "Add Date");
            
            Id = Guid.NewGuid();
            AddUser = resolvedAddUser.Trim();
            AddDate = resolvedAddDate;
        }
        
        public Guid Id { get; private set; }
        
        public string AddUser { get; private set; }
        
        public DateTimeOffset AddDate { get; private set; }
        
        public string? UpdateUser { get; private set; }
        
        public DateTimeOffset? UpdateDate { get; private set; }

        protected void MarkUpdated(string updateUser, DateTimeOffset? updateDate = null)
        {
            var resolvedUpdateDate = updateDate.GetValueOrDefault(DateTimeOffset.Now);
            var resolvedUpdateUser = ValidateRequiredStringAndLength(updateUser, DomainConstants.UserNameMaxLength, "Update User");
            ValidateDateTimeOffset(resolvedUpdateDate, "Add Date");

            UpdateUser = resolvedUpdateUser;
            UpdateDate = resolvedUpdateDate;
        }

        protected string ValidateRequiredStringAndLength(string? value, int maxLength, string propertyName,
            bool allowEmptyString = false)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value), $"{propertyName} value is NULL");
            }

            if (!allowEmptyString && string.IsNullOrEmpty(value))
            {
                throw new ArgumentException($"{propertyName} is empty", nameof(value));
            }

            var resolvedValue = value.Trim();
            if (resolvedValue.Length > maxLength)
            {
                throw new ArgumentException(
                    $"{propertyName} is {resolvedValue.Length} characters but only {maxLength} characters are allowed",
                    nameof(value));
            }

            return resolvedValue;
        }

        protected void ValidateDateTimeOffset(DateTimeOffset value, string propertyName)
        {
            if (value == DateTimeOffset.MinValue || value == DateTimeOffset.MaxValue)
            {
                throw new ArgumentException($"{propertyName} is an invalid date/time", nameof(value));
            }
        }

        protected void ValidateGuid(Guid value, string propertyName)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException($"Invalid {propertyName}", nameof(value));
            }
        }

        protected void ValidateValueRange<T>(T value, string propertyName, T minValue, T maxValue)
            where T : IComparable<T>
        {
            if (value.CompareTo(minValue) < 0)
            {
                throw new ArgumentException($"{propertyName} is less than {minValue}", nameof(value));
            }

            if (value.CompareTo(maxValue) > 0)
            {
                throw new ArgumentException($"{propertyName} is greater than {minValue}", nameof(value));
            }
        }
    }
}