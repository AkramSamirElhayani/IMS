using IMS.Application.Common.Results;
using MediatR;

namespace IMS.Application.Features.Items.Commands.AddBatchInformation;

public record AddBatchInformationCommand : IRequest<Result>
{
    public required Guid Id { get; init; }
    public required string BatchNumber { get; init; }
    public required DateTime ManufacturingDate { get; init; }
    public required DateTime ExpiryDate { get; init; }
}
