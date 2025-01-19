using IMS.Application.Common.Results;
using MediatR;

namespace IMS.Application.Features.Items.Commands.UpdateStockLevel;

public record UpdateStockLevelCommand : IRequest<Result>
{
    public required Guid Id { get; init; }
    public required int NewQuantity { get; init; }
    public Guid? TransactionId { get; init; }
}
