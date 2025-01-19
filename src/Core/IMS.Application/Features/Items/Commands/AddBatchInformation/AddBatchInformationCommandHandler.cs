using IMS.Application.Common.Interfaces;
using IMS.Application.Common.Results;
using IMS.Domain.ValueObjects;
using MediatR;

namespace IMS.Application.Features.Items.Commands.AddBatchInformation;

public class AddBatchInformationCommandHandler : IRequestHandler<AddBatchInformationCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IItemRepository _itemRepository;

    public AddBatchInformationCommandHandler(
        IUnitOfWork unitOfWork,
        IItemRepository itemRepository)
    {
        _unitOfWork = unitOfWork;
        _itemRepository = itemRepository;
    }

    public async Task<Result> Handle(AddBatchInformationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var item = await _itemRepository.GetByIdAsync(request.Id, cancellationToken);
            if (item == null)
                return Result.Failure(Error.NotFound("Item not found"));

            var batchInfo = BatchInformation.Create(
                request.BatchNumber,
                request.ManufacturingDate,
                request.ExpiryDate);

            item.AddBatchInformation(batchInfo);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(Error.Validation($"Failed to add batch information: {ex.Message}"));
        }
    }
}
