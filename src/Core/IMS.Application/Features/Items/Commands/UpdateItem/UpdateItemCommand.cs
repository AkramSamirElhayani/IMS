using IMS.Application.Common.Results;
using MediatR;

namespace IMS.Application.Features.Items.Commands;

public record UpdateItemCommand : IRequest<Result>
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Type { get; init; }
    public required bool IsPerishable { get; init; }
}
