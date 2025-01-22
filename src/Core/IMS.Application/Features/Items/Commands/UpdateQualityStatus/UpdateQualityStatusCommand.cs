using IMS.Application.Common.Results;
using IMS.Domain.Enums;
using MediatR;

namespace IMS.Application.Features.Items.Commands;

public record UpdateQualityStatusCommand : IRequest<Result>
{
    public required Guid Id { get; init; }
    public required QualityStatus NewStatus { get; init; }
}
