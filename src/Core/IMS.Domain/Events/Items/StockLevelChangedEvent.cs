using IMS.Domain.Common;

namespace IMS.Domain.Events
{
    public record StockLevelChangedEvent(Guid ItemId, int OldQuantity, int NewQuantity, Guid? TransactionId = null) : DomainEvent;
}
