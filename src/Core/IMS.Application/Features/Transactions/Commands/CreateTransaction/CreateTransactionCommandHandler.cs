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

            var transaction = transactionType switch
            {
                TransactionType.TransferIn or TransactionType.Purchase => 
                    Transaction.CreateInbound(
                        Guid.Parse(request.ProductId),
                        request.Quantity,
                        "Warehouse", // We'll need to add this to the command later
                        transactionType,
                        batchInfo,
                        request.TransactionDate),

                TransactionType.TransferOut or TransactionType.Sale => 
                    Transaction.CreateOutbound(
                        Guid.Parse(request.ProductId),
                        request.Quantity,
                        "Warehouse", // We'll need to add this to the command later
                        transactionType,
                        batchInfo,
                        request.TransactionDate),

                _ => throw new ArgumentException("Invalid transaction type", nameof(request.Type))
            };

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            await _transactionRepository.AddAsync(transaction, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Result.Success(transaction.Id);
        }
        catch (ArgumentException ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return Result.Failure<Guid>(Error.Validation(ex.Message));
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return Result.Failure<Guid>(new Error("UnexpectedError", ex.Message));
        }
    }
}
