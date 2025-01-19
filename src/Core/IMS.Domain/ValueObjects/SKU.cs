using System;
using IMS.Domain.Common;
using System.Collections.Generic;

namespace IMS.Domain.ValueObjects
{
    public class SKU : ValueObject
    {
        public string Value { get; private set; }

        private SKU(string value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public static SKU Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("SKU cannot be empty", nameof(value));

            // Add validation rules for SKU format if needed
            return new SKU(value);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;
    }
}
