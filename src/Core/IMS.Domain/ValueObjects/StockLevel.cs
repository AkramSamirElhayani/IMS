using System;
using IMS.Domain.Common;
using System.Collections.Generic;

namespace IMS.Domain.ValueObjects
{
    public class StockLevel : ValueObject
    {
        public int Current { get; private set; }
        public int Minimum { get; private set; }
        public int Maximum { get; private set; }
        public int Critical { get; private set; }

        private StockLevel(int current, int minimum, int maximum, int critical)
        {
            if (current < 0)
                throw new ArgumentException("Current stock level cannot be negative", nameof(current));
            if (minimum < 0)
                throw new ArgumentException("Minimum stock level cannot be negative", nameof(minimum));
            if (maximum <= minimum)
                throw new ArgumentException("Maximum stock level must be greater than minimum", nameof(maximum));
            if (critical < minimum)
                throw new ArgumentException("Critical level must be greater than or equal to minimum", nameof(critical));
            if (critical > maximum)
                throw new ArgumentException("Critical level must be less than or equal to maximum", nameof(maximum));

            Current = current;
            Minimum = minimum;
            Maximum = maximum;
            Critical = critical;
        }

        public static StockLevel Create(int current, int minimum, int maximum, int critical)
        {
            return new StockLevel(current, minimum, maximum, critical);
        }

        public bool IsLow() => Current <= Minimum;
        public bool IsCritical() => Current <= Critical;
        public bool IsOverflow() => Current > Maximum;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Current;
            yield return Minimum;
            yield return Maximum;
            yield return Critical;
        }
    }
}
