using FluentValidation;

namespace IMS.Application.Features.Transactions.Commands.CreateTransaction;

public class CreateTransactionCommandValidator : AbstractValidator<CreateTransactionCommand>
{
    public CreateTransactionCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty();

        RuleFor(x => x.Quantity)
            .GreaterThan(0);

        RuleFor(x => x.Type)
            .NotEmpty()
            .Must(x => x is "IN" or "OUT")
            .WithMessage("Transaction type must be either 'IN' or 'OUT'");

        RuleFor(x => x.BatchNumber)
            .NotEmpty()
            .When(x => x.Type == "IN")
            .WithMessage("Batch number is required for incoming transactions");

        RuleFor(x => x.ExpiryDate)
            .NotNull()
            .When(x => x.Type == "IN")
            .WithMessage("Expiry date is required for incoming transactions")
            .Must(x => x > DateTimeOffset.UtcNow)
            .When(x => x.ExpiryDate.HasValue && x.Type == "IN")
            .WithMessage("Expiry date must be in the future");
    }
}
