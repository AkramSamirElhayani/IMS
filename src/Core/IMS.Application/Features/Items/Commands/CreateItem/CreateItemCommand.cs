using IMS.Application.Common.Results;
using IMS.Domain.Enums;
using MediatR;

namespace IMS.Application.Features.Items.Commands;

public record CreateItemCommand : IRequest<Result<Guid>>
{
    public required string SKU { get; init; }
    public required string Name { get; init; }
    public required string Type { get; init; }
    public required bool IsPerishable { get; init; }
    public required int MinimumQuantity { get; init; }
    public required int MaximumQuantity { get; init; }
    public required int ReorderPoint { get; init; }
}
