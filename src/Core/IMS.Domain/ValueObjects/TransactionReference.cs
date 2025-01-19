using System;
using IMS.Domain.Common;
using System.Collections.Generic;

namespace IMS.Domain.ValueObjects
{
    public sealed class TransactionReference : ValueObject
    {
        public string Value { get; private set; }

        private TransactionReference(string value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public static TransactionReference Create(DateTime transactionDate, string prefix = "TRX")
        {
            // Format: TRX-YYYYMMDD-GUID (first 8 chars)
            string uniqueId = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
            string reference = $"{prefix}-{transactionDate:yyyyMMdd}-{uniqueId}";
            return new TransactionReference(reference);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;
    }
}
