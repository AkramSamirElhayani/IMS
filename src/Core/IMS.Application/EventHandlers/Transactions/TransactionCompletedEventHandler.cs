using IMS.Application.Common.Interfaces;
using IMS.Domain.Events;
using IMS.Domain.Events.Transactions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace IMS.Application.EventHandlers.Transactions;

public class TransactionCompletedEventHandler : INotificationHandler<TransactionCompletedEvent>
{
    private readonly ILogger<TransactionCompletedEventHandler> _logger;
    private readonly IItemRepository _itemRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TransactionCompletedEventHandler(
        ILogger<TransactionCompletedEventHandler> logger,
        IItemRepository itemRepository,
        ITransactionRepository transactionRepository,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _itemRepository = itemRepository;
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(TransactionCompletedEvent notification, CancellationToken cancellationToken)
    {
        var item = await _itemRepository.GetByIdAsync(notification.ItemId, cancellationToken);
        if (item == null)
        {
            _logger.LogError("Item {ItemId} not found while processing TransactionCompletedEvent", notification.ItemId);
            return;
        }

        var totalStock = await _transactionRepository.CalculateTotalStockForItemAsync(notification.ItemId, cancellationToken);
        item.UpdateStockLevel(totalStock, notification.TransactionId);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Updated stock level for Item {ItemId}. New stock level: {CurrentStock}",
            item.Id,
            item.StockLevel.Current);
    }
}
