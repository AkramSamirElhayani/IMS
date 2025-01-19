using IMS.Domain.Common;
using IMS.Domain.Enums;

namespace IMS.Domain.Events
{
    public record QualityStatusChangedEvent(Guid ItemId, QualityStatus OldStatus, QualityStatus NewStatus) : DomainEvent;
}
