using IMS.Application.Common.Results;
using MediatR;

namespace IMS.Application.Features.Items.Commands.AddStorageLocation;

public record AddStorageLocationCommand : IRequest<Result>
{
    public required Guid Id { get; init; }
    public required string Location { get; init; }
}
