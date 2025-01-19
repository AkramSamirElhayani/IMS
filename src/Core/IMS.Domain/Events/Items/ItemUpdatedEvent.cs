using IMS.Domain.Common;

namespace IMS.Domain.Events;

public sealed record ItemUpdatedEvent : DomainEvent
{
    public Guid ItemId { get; }

    public ItemUpdatedEvent(Guid itemId)
    {
        ItemId = itemId;
    }
}
