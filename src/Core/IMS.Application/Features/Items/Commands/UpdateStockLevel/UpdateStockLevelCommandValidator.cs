using FluentValidation;

namespace IMS.Application.Features.Items.Commands.UpdateStockLevel;

public class UpdateStockLevelCommandValidator : AbstractValidator<UpdateStockLevelCommand>
{
    public UpdateStockLevelCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Item Id is required");

        RuleFor(x => x.NewQuantity)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Quantity must be greater than or equal to 0");
    }
}
