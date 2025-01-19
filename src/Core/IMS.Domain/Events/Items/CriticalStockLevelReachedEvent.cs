using IMS.Domain.Common;

namespace IMS.Domain.Events
{
    public record CriticalStockLevelReachedEvent(Guid ItemId, int CurrentQuantity, int CriticalLevel) : DomainEvent;
}
