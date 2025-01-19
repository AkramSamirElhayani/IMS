using System;

using IMS.Domain.Common;
using IMS.Domain.Enums;

namespace IMS.Domain.Events.Transactions
{

    public record TransactionCompletedEvent(
        Guid TransactionId,
        Guid ItemId,
        TransactionType Type,
        int Quantity,
        string SourceLocation,
        string DestinationLocation
    ) : DomainEvent;
}
