using System;

using IMS.Domain.Common;

namespace IMS.Domain.Events
{

    public record ItemDeactivatedEvent(Guid ItemId) : DomainEvent;
}
