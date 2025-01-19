using IMS.Domain.Common;
using IMS.Domain.Enums;

namespace IMS.Domain.Events.Transactions
{
    public record TransactionCreatedEvent(
        Guid TransactionId,
        Guid ItemId,
        TransactionType Type,
        int Quantity
    ) : DomainEvent;
}
