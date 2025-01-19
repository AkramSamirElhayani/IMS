using IMS.Application.Common.Interfaces;
using IMS.Application.Common.Results;
using IMS.Domain.Enums;
using IMS.Domain.ValueObjects;
using MediatR;

namespace IMS.Application.Features.Items.Commands.UpdateItem;

public class UpdateItemCommandHandler : IRequestHandler<UpdateItemCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IItemRepository _itemRepository;

    public UpdateItemCommandHandler(
        IUnitOfWork unitOfWork,
        IItemRepository itemRepository)
    {
        _unitOfWork = unitOfWork;
        _itemRepository = itemRepository;
    }

    public async Task<Result> Handle(UpdateItemCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var item = await _itemRepository.GetByIdAsync(request.Id, cancellationToken);
            if (item == null)
                return Result.Failure(Error.NotFound("Item not found"));

            var itemType = Enum.Parse<ItemType>(request.Type);

            // Update properties
            item.UpdateBasicProperties(
                request.Name,
                itemType,
                request.IsPerishable);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(Error.Validation($"Failed to update item: {ex.Message}"));
        }
    }
}
