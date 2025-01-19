using IMS.Domain.Common;

namespace IMS.Domain.Events
{
    public record ItemActivatedEvent(Guid ItemId) : DomainEvent;
}
