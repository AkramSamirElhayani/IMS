using FluentValidation;
using FluentValidation.Internal;

using IMS.Presentation.ViewModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Presentation.Validation;

public interface IModelValidator<T>
{
    ValidationResult Validate(T model);
}

public class ValidationResult
{
    public bool IsValid => Errors.Count == 0;
    public Dictionary<string, List<string>> Errors { get; } = new();
}

public class ItemValidator : AbstractValidator<ItemViewModel>, IModelValidator<ItemViewModel>
{
    public ItemValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MinimumLength(3).WithMessage("Name must be at least 3 characters");

        RuleFor(x => x.Sku)
            .NotEmpty().WithMessage("SKU is required")
            .Matches(@"^[A-Z0-9]{6,10}$")
            .WithMessage("SKU must be 6-10 characters, uppercase letters and numbers only");

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Quantity cannot be negative");

        RuleFor(x => x.MinimumQuantity)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Minimum quantity cannot be negative")
            .LessThanOrEqualTo(x => x.MaximumQuantity)
            .WithMessage("Minimum quantity must be less than maximum quantity");

        RuleFor(x => x.ReorderPoint)
            .GreaterThanOrEqualTo(x => x.MinimumQuantity)
            .WithMessage("Reorder point must be greater than minimum quantity")
            .LessThanOrEqualTo(x => x.MaximumQuantity)
            .WithMessage("Reorder point must be less than maximum quantity");
    }

    public new  ValidationResult Validate(ItemViewModel model)
    {
        var fluentResult = base.Validate(model);
        return ConvertToValidationResult(fluentResult);
    }




    private ValidationResult ConvertToValidationResult(FluentValidation.Results.ValidationResult fluentResult)
    {
        var result = new ValidationResult();
        foreach (var error in fluentResult.Errors.GroupBy(e => e.PropertyName))
        {
            result.Errors[error.Key] = error.Select(e => e.ErrorMessage).ToList();
        }
        return result;
    }
}