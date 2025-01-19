using FluentAssertions;
using IMS.Application.Features.Items.Commands.AddBatchInformation;

namespace IMS.UnitTests.Application.Features.Items.Commands.AddBatchInformation;

public class AddBatchInformationCommandValidatorTests
{
    private readonly AddBatchInformationCommandValidator _validator;

    public AddBatchInformationCommandValidatorTests()
    {
        _validator = new AddBatchInformationCommandValidator();
    }

    [Fact]
    public void Validate_WhenBatchNumberIsEmpty_ShouldHaveError()
    {
        // Arrange
        var command = new AddBatchInformationCommand
        {
            Id = Guid.NewGuid(),
            BatchNumber = "",
            ManufacturingDate = DateTime.UtcNow.AddDays(-10),
            ExpiryDate = DateTime.UtcNow.AddDays(90)
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.BatchNumber));
    }

    [Fact]
    public void Validate_WhenManufacturingDateIsInFuture_ShouldHaveError()
    {
        // Arrange
        var command = new AddBatchInformationCommand
        {
            Id = Guid.NewGuid(),
            BatchNumber = "BATCH123",
            ManufacturingDate = DateTime.UtcNow.AddDays(1), // Future date
            ExpiryDate = DateTime.UtcNow.AddDays(90)
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.ManufacturingDate));
    }

    [Fact]
    public void Validate_WhenExpiryDateIsBeforeManufacturingDate_ShouldHaveError()
    {
        // Arrange
        var command = new AddBatchInformationCommand
        {
            Id = Guid.NewGuid(),
            BatchNumber = "BATCH123",
            ManufacturingDate = DateTime.UtcNow.AddDays(-10),
            ExpiryDate = DateTime.UtcNow.AddDays(-20) // Before manufacturing date
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.ExpiryDate));
    }

    [Fact]
    public void Validate_WhenAllPropertiesAreValid_ShouldNotHaveError()
    {
        // Arrange
        var command = new AddBatchInformationCommand
        {
            Id = Guid.NewGuid(),
            BatchNumber = "BATCH123",
            ManufacturingDate = DateTime.UtcNow.AddDays(-10),
            ExpiryDate = DateTime.UtcNow.AddDays(90)
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
}
