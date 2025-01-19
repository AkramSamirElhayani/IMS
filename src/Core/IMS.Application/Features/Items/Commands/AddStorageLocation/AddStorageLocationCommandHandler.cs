using IMS.Application.Common.Interfaces;
using IMS.Application.Common.Results;
using MediatR;

namespace IMS.Application.Features.Items.Commands.AddStorageLocation;

public class AddStorageLocationCommandHandler : IRequestHandler<AddStorageLocationCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IItemRepository _itemRepository;

    public AddStorageLocationCommandHandler(
        IUnitOfWork unitOfWork,
        IItemRepository itemRepository)
    {
        _unitOfWork = unitOfWork;
        _itemRepository = itemRepository;
    }

    public async Task<Result> Handle(AddStorageLocationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var item = await _itemRepository.GetByIdAsync(request.Id, cancellationToken);
            if (item == null)
                return Result.Failure(Error.NotFound("Item not found"));

            item.AddStorageLocation(request.Location);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(Error.Validation($"Failed to add storage location: {ex.Message}"));
        }
    }
}
