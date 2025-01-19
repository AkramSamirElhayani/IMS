using System;
using MediatR;

namespace IMS.Domain.Common
{
    public abstract record DomainEvent : IDomainEvent
    {
        public Guid Id { get; }
        public DateTime DateOccurred { get; }

        protected DomainEvent()
        {
            Id = Guid.NewGuid();
            DateOccurred = DateTime.UtcNow;
        }
    }
}
