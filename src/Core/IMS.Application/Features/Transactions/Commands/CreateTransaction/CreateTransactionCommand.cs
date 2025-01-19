using IMS.Application.Common.Results;
using IMS.Domain.Aggregates;
using MediatR;

namespace IMS.Application.Features.Transactions.Commands.CreateTransaction;

public record CreateTransactionCommand : IRequest<Result<Guid>>
{
    public required string ProductId { get; init; }
    public required int Quantity { get; init; }
    public required string Type { get; init; }
    public string? BatchNumber { get; init; }
    public DateTimeOffset? ManufactureDate { get; init; }
    public DateTimeOffset? ExpiryDate { get; init; }
    public DateTimeOffset? TransactionDate { get; init; }
}
