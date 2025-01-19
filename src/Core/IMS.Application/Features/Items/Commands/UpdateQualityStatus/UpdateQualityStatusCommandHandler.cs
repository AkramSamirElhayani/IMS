using IMS.Application.Common.Interfaces;
using IMS.Application.Common.Results;
using MediatR;

namespace IMS.Application.Features.Items.Commands.UpdateQualityStatus;

public class UpdateQualityStatusCommandHandler : IRequestHandler<UpdateQualityStatusCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IItemRepository _itemRepository;

    public UpdateQualityStatusCommandHandler(
        IUnitOfWork unitOfWork,
        IItemRepository itemRepository)
    {
        _unitOfWork = unitOfWork;
        _itemRepository = itemRepository;
    }

    public async Task<Result> Handle(UpdateQualityStatusCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var item = await _itemRepository.GetByIdAsync(request.Id, cancellationToken);
            if (item == null)
                return Result.Failure(Error.NotFound("Item not found"));

            item.UpdateQualityStatus(request.NewStatus);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(Error.Validation($"Failed to update quality status: {ex.Message}"));
        }
    }
}
