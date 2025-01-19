using IMS.Domain.Common;
using IMS.Domain.Enums;

namespace IMS.Domain.Events
{
    public record ItemCreatedEvent(Guid ItemId, string SKU, ItemType Type) : DomainEvent;
}
