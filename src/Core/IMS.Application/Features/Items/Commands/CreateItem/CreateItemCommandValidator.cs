using FluentValidation;

namespace IMS.Application.Features.Items.Commands.CreateItem;

public class CreateItemCommandValidator : AbstractValidator<CreateItemCommand>
{
    public CreateItemCommandValidator()
    {
        RuleFor(x => x.SKU)
            .NotEmpty().WithMessage("SKU is required")
            .MaximumLength(50).WithMessage("SKU must not exceed 50 characters");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters");

        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Type is required")
            .Must(type => Enum.TryParse<Domain.Enums.ItemType>(type, out _))
            .WithMessage("Invalid item type");

        RuleFor(x => x.MinimumQuantity)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Minimum quantity must be greater than or equal to 0");

        RuleFor(x => x.MaximumQuantity)
            .GreaterThan(x => x.MinimumQuantity)
            .WithMessage("Maximum quantity must be greater than minimum quantity");

        RuleFor(x => x.ReorderPoint)
            .GreaterThanOrEqualTo(x => x.MinimumQuantity)
            .LessThanOrEqualTo(x => x.MaximumQuantity)
            .WithMessage("Reorder point must be between minimum and maximum quantity");
    }
}
