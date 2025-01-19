using FluentValidation;

namespace IMS.Application.Features.Items.Commands.AddStorageLocation;

public class AddStorageLocationCommandValidator : AbstractValidator<AddStorageLocationCommand>
{
    public AddStorageLocationCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Item Id is required");

        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("Location is required")
            .MaximumLength(100).WithMessage("Location must not exceed 100 characters");
    }
}
