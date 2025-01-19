using IMS.Application.Common.Results;
using MediatR;

namespace IMS.Application.Features.Items.Commands.DeactivateItem;

public record DeactivateItemCommand : IRequest<Result>
{
    public required Guid Id { get; init; }
}
