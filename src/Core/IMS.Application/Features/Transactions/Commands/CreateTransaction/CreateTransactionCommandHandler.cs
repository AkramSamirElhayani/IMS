using IMS.Application.Common.Interfaces;
using IMS.Application.Common.Results;
using IMS.Domain.Aggregates;
using IMS.Domain.Enums;
using IMS.Domain.ValueObjects;
using MediatR;

namespace IMS.Application.Features.Transactions.Commands.CreateTransaction;

public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITransactionRepository _transactionRepository;

    public CreateTransactionCommandHandler(
        IUnitOfWork unitOfWork,
        ITransactionRepository transactionRepository)
    {
        _unitOfWork = unitOfWork;
        _transactionRepository = transactionRepository;
    }

    public async Task<Result<Guid>> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var transactionType = Enum.Parse<TransactionType>(request.Type);
            var batchInfo = request.BatchNumber != null && request.ManufactureDate.HasValue && request.ExpiryDate.HasValue
                ? BatchInformation.Create(request.BatchNumber!, request.ManufactureDate.Value.DateTime, request.ExpiryDate.Value.DateTime)
                : null;

            // Create transaction based on type (inbound/outbound)
            var transaction = transactionType.GetStockImpactMultiplier() > 0
                ? Transaction.CreateInbound(
                    Guid.Parse(request.ProductId),
                    request.Quantity,
                    "Warehouse",
                    transactionType,
                    batchInfo,
                    request.TransactionDate)
                : Transaction.CreateOutbound(
                    Guid.Parse(request.ProductId),
                    request.Quantity,
                    "Warehouse",
                    transactionType,
                    batchInfo,
                    request.TransactionDate);

            await _transactionRepository.AddAsync(transaction);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(transaction.Id);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure(Error.FromException(ex));
        }
    }
}
