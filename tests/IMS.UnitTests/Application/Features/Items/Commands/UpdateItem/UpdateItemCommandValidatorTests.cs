using FluentAssertions;

using IMS.Application.Features.Items.Commands;
using IMS.Application.Features.Items.Commands.UpdateItem;

namespace IMS.UnitTests.Application.Features.Items.Commands.UpdateItem;

public class UpdateItemCommandValidatorTests
{
    private readonly UpdateItemCommandValidator _validator;

    public UpdateItemCommandValidatorTests()
    {
        _validator = new UpdateItemCommandValidator();
    }

    [Fact]
    public void Validate_WhenNameIsEmpty_ShouldHaveError()
    {
        // Arrange
        var command = new UpdateItemCommand
        {
            Id = Guid.NewGuid(),
            Name = "",
            Type = "Raw",
            IsPerishable = true
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Name));
    }

    [Fact]
    public void Validate_WhenTypeIsEmpty_ShouldHaveError()
    {
        // Arrange
        var command = new UpdateItemCommand
        {
            Id = Guid.NewGuid(),
            Name = "Test Item",
            Type = "",
            IsPerishable = true
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Type));
    }

    [Fact]
    public void Validate_WhenTypeIsInvalid_ShouldHaveError()
    {
        // Arrange
        var command = new UpdateItemCommand
        {
            Id = Guid.NewGuid(),
            Name = "Test Item",
            Type = "InvalidType",
            IsPerishable = true
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Type));
    }

  
}
