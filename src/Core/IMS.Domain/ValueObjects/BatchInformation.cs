using System;
using IMS.Domain.Common;
using System.Collections.Generic;

namespace IMS.Domain.ValueObjects
{
    public sealed class BatchInformation : ValueObject
    {
        public string BatchNumber { get; private set; }
        public DateTime ManufacturingDate { get; private set; }
        public DateTime ExpiryDate { get; private set; }

        private BatchInformation(string batchNumber, DateTime manufacturingDate, DateTime expiryDate)
        {
            if (string.IsNullOrWhiteSpace(batchNumber))
                throw new ArgumentException("Batch number cannot be empty", nameof(batchNumber));
            
            if (expiryDate <= manufacturingDate)
                throw new ArgumentException("Expiry date must be after manufacturing date");

            BatchNumber = batchNumber;
            ManufacturingDate = manufacturingDate;
            ExpiryDate = expiryDate;
        }

        public static BatchInformation Create(string batchNumber, DateTime manufacturingDate, DateTime expiryDate)
        {
            return new BatchInformation(batchNumber, manufacturingDate, expiryDate);
        }

        public bool IsExpired() => DateTime.UtcNow > ExpiryDate;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return BatchNumber;
            yield return ManufacturingDate;
            yield return ExpiryDate;
        }
    }
}
