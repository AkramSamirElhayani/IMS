using IMS.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace IMS.Application.EventHandlers.Items;

public class CriticalStockLevelReachedEventHandler : INotificationHandler<CriticalStockLevelReachedEvent>
{
    private readonly ILogger<CriticalStockLevelReachedEventHandler> _logger;

    public CriticalStockLevelReachedEventHandler(ILogger<CriticalStockLevelReachedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(CriticalStockLevelReachedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogWarning("Item {ItemId} has reached critical stock level. Current quantity: {CurrentQuantity}, Critical level: {CriticalLevel}",
            notification.ItemId,
            notification.CurrentQuantity,
            notification.CriticalLevel);

        // TODO: Add additional handling like:
        // - Send email notifications
        // - Create automatic reorder
        // - Update dashboard statistics

        return Task.CompletedTask;
    }
}
