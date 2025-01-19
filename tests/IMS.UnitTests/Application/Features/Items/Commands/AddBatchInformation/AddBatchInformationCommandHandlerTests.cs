using IMS.Application.Common.Results;
using IMS.Application.Features.Items.Commands.AddBatchInformation;
using IMS.Domain.Aggregates;
using IMS.Domain.Enums;
using IMS.Domain.ValueObjects;
using IMS.UnitTests.Application.Common;
using Moq;
using FluentAssertions;

namespace IMS.UnitTests.Application.Features.Items.Commands.AddBatchInformation;

public class AddBatchInformationCommandHandlerTests : TestBase
{
    private readonly AddBatchInformationCommandHandler _handler;

    public AddBatchInformationCommandHandlerTests()
    {
        _handler = new AddBatchInformationCommandHandler(UnitOfWorkMock.Object, ItemRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WhenItemNotFound_ReturnsNotFoundError()
    {
        // Arrange
        var command = new AddBatchInformationCommand
        {
            Id = Guid.NewGuid(),
            BatchNumber = "BATCH123",
            ManufacturingDate = DateTime.UtcNow.AddDays(-10),
            ExpiryDate = DateTime.UtcNow.AddDays(90)
        };

        ItemRepositoryMock.Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Item?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("NotFound");
        result.Error.Message.Should().Be("Item not found");
    }

    [Fact]
    public async Task Handle_WhenItemIsNotPerishable_ReturnsFailure()
    {
        // Arrange
        var command = new AddBatchInformationCommand
        {
            Id = Guid.NewGuid(),
            BatchNumber = "BATCH123",
            ManufacturingDate = DateTime.UtcNow.AddDays(-10),
            ExpiryDate = DateTime.UtcNow.AddDays(90)
        };

        var existingItem = Item.Create(
            SKU.Create("SKU123"),
            "Test Item",
            ItemType.RawMaterial,
            isPerishable: false, // Non-perishable item
            StockLevel.Create(10, 0, 100, 5));

        ItemRepositoryMock.Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingItem);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("Validation");
        result.Error.Message.Should().Contain("Failed to add batch information");
    }

    [Fact]
    public async Task Handle_WhenItemIsPerishable_UpdatesAndReturnsSuccess()
    {
        // Arrange
        var command = new AddBatchInformationCommand
        {
            Id = Guid.NewGuid(),
            BatchNumber = "BATCH123",
            ManufacturingDate = DateTime.UtcNow.AddDays(-10),
            ExpiryDate = DateTime.UtcNow.AddDays(90)
        };

        var existingItem = Item.Create(
            SKU.Create("SKU123"),
            "Test Item",
            ItemType.RawMaterial,
            isPerishable: true, // Perishable item
            StockLevel.Create(10, 0, 100, 5));

        ItemRepositoryMock.Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingItem);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        existingItem.BatchInformation.Should().NotBeNull();
        existingItem.BatchInformation!.BatchNumber.Should().Be(command.BatchNumber);
        existingItem.BatchInformation.ManufacturingDate.Should().Be(command.ManufacturingDate);
        existingItem.BatchInformation.ExpiryDate.Should().Be(command.ExpiryDate);

        UnitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
