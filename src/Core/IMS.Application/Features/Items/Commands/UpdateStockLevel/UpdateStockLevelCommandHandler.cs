using IMS.Application.Common.Interfaces;
using IMS.Application.Common.Results;
using MediatR;

namespace IMS.Application.Features.Items.Commands.UpdateStockLevel;

public class UpdateStockLevelCommandHandler : IRequestHandler<UpdateStockLevelCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IItemRepository _itemRepository;

    public UpdateStockLevelCommandHandler(
        IUnitOfWork unitOfWork,
        IItemRepository itemRepository)
    {
        _unitOfWork = unitOfWork;
        _itemRepository = itemRepository;
    }

    public async Task<Result> Handle(UpdateStockLevelCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var item = await _itemRepository.GetByIdAsync(request.Id, cancellationToken);
            if (item == null)
                return Result.Failure(Error.NotFound("Item not found"));

            item.UpdateStockLevel(request.NewQuantity, request.TransactionId);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(Error.Validation($"Failed to update stock level: {ex.Message}"));
        }
    }
}
