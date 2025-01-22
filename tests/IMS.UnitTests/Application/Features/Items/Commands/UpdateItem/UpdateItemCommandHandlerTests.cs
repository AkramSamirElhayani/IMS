using IMS.Application.Common.Results;
using IMS.Application.Features.Items.Commands.UpdateItem;
using IMS.Domain.Aggregates;
using IMS.Domain.Enums;
using IMS.Domain.ValueObjects;
using IMS.UnitTests.Application.Common;
using Moq;
using FluentAssertions;
using IMS.Application.Features.Items.Commands;

namespace IMS.UnitTests.Application.Features.Items.Commands.UpdateItem;

public class UpdateItemCommandHandlerTests : TestBase
{
    private readonly UpdateItemCommandHandler _handler;

    public UpdateItemCommandHandlerTests()
    {
        _handler = new UpdateItemCommandHandler(UnitOfWorkMock.Object, ItemRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WhenItemNotFound_ReturnsNotFoundError()
    {
        // Arrange
        var command = new UpdateItemCommand
        {
            Id = Guid.NewGuid(),
            Name = "Updated Item",
            Type = "Raw",
            IsPerishable = true
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
    public async Task Handle_WhenItemExists_UpdatesItemAndReturnsSuccess()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var command = new UpdateItemCommand
        {
            Id = itemId,
            Name = "Updated Item",
            Type = "Raw",
            IsPerishable = true
        };

        var existingItem = Item.Create(
            SKU.Create("SKU123"),
            "Original Item",
            ItemType.RawMaterial,
            false,
            StockLevel.Create(10, 0, 100, 5));

        ItemRepositoryMock.Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingItem);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        existingItem.Name.Should().Be(command.Name);
        existingItem.Type.Should().Be(ItemType.RawMaterial);
        existingItem.IsPerishable.Should().Be(command.IsPerishable);

        UnitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenExceptionOccurs_ReturnsFailure()
    {
        // Arrange
        var command = new UpdateItemCommand
        {
            Id = Guid.NewGuid(),
            Name = "Updated Item",
            Type = "Invalid Type", // This will cause Enum.Parse to throw
            IsPerishable = true
        };

        var existingItem = Item.Create(
            SKU.Create("SKU123"),
            "Original Item",
            ItemType.RawMaterial,
            false,
            StockLevel.Create(10, 0, 100, 5));

        ItemRepositoryMock.Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingItem);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("Validation");
        result.Error.Message.Should().Contain("Failed to update item");
    }
}
