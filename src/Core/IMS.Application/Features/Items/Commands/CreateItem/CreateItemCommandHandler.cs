using IMS.Application.Common.Interfaces;
using IMS.Application.Common.Results;
using IMS.Domain.Aggregates;
using IMS.Domain.Enums;
using IMS.Domain.ValueObjects;
using MediatR;

namespace IMS.Application.Features.Items.Commands.CreateItem;

public class CreateItemCommandHandler : IRequestHandler<CreateItemCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IItemRepository _itemRepository;

    public CreateItemCommandHandler(
        IUnitOfWork unitOfWork,
        IItemRepository itemRepository)
    {
        _unitOfWork = unitOfWork;
        _itemRepository = itemRepository;
    }

    public async Task<Result<Guid>> Handle(CreateItemCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var itemType = Enum.Parse<ItemType>(request.Type);
            var sku = SKU.Create(request.SKU);
            var stockLevel = StockLevel.Create(
                0, // Initial current quantity
                request.MinimumQuantity,
                request.MaximumQuantity,
                request.ReorderPoint); // Using reorder point as critical level

            var item = Item.Create(
                sku,
                request.Name,
                itemType,
                request.IsPerishable,
                stockLevel);

            await _itemRepository.AddAsync(item, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(item.Id);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure(Error.Validation($"Failed to create item: {ex.Message}"));
        }
    }
}
