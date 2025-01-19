using FluentValidation;

namespace IMS.Application.Features.Items.Commands.RemoveStorageLocation;

public class RemoveStorageLocationCommandValidator : AbstractValidator<RemoveStorageLocationCommand>
{
    public RemoveStorageLocationCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Item Id is required");

        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("Location is required")
            .MaximumLength(100).WithMessage("Location must not exceed 100 characters");
    }
}
