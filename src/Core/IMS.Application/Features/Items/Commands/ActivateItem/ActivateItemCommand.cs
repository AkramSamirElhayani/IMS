using IMS.Application.Common.Results;
using MediatR;

namespace IMS.Application.Features.Items.Commands;

public record ActivateItemCommand : IRequest<Result>
{
    public required Guid Id { get; init; }
}
