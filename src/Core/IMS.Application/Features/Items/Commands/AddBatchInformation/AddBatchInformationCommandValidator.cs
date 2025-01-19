using FluentValidation;

namespace IMS.Application.Features.Items.Commands.AddBatchInformation;

public class AddBatchInformationCommandValidator : AbstractValidator<AddBatchInformationCommand>
{
    public AddBatchInformationCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Item Id is required");

        RuleFor(x => x.BatchNumber)
            .NotEmpty().WithMessage("Batch number is required")
            .MaximumLength(50).WithMessage("Batch number must not exceed 50 characters");

        RuleFor(x => x.ManufacturingDate)
            .NotEmpty().WithMessage("Manufacturing date is required")
            .LessThan(x => x.ExpiryDate)
            .WithMessage("Manufacturing date must be before expiry date");

        RuleFor(x => x.ExpiryDate)
            .NotEmpty().WithMessage("Expiry date is required")
            .GreaterThan(x => x.ManufacturingDate)
            .WithMessage("Expiry date must be after manufacturing date")
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Expiry date must be in the future");
    }
}
