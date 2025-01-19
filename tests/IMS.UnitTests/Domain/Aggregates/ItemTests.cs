using IMS.Domain.Aggregates;
using IMS.Domain.Events;
using IMS.Domain.ValueObjects;
using IMS.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace IMS.UnitTests.Domain.Aggregates;

public class ItemTests
{
    [Fact]
    public void Create_ShouldCreateItemWithCorrectProperties()
    {
        // Arrange
        var sku = SKU.Create("TEST-123");
        var name = "Test Item";
        var type = ItemType.FinishedGood;
        var isPerishable = true;
        var stockLevel = StockLevel.Create(10, 5, 100, 7); // Added critical level

        // Act
        var item = Item.Create(
            sku,
            name,
            type,
            isPerishable,
            stockLevel);

        // Assert
        item.Should().NotBeNull();
        item.SKU.Should().Be(sku);
        item.Name.Should().Be(name);
        item.Type.Should().Be(type);
        item.IsPerishable.Should().BeTrue();
        item.StockLevel.Should().Be(stockLevel);
        item.IsActive.Should().BeTrue();
        item.QualityStatus.Should().Be(QualityStatus.Good);
        item.StorageLocations.Should().BeEmpty();
        
        var createdEvent = item.DomainEvents.Should()
            .ContainSingle(e => e is ItemCreatedEvent)
            .Subject as ItemCreatedEvent;
        
        createdEvent.Should().NotBeNull();
        createdEvent!.ItemId.Should().Be(item.Id);
        createdEvent.SKU.Should().Be(sku.Value);
        createdEvent.Type.Should().Be(type);
    }

    [Fact]
    public void UpdateStockLevel_WhenBelowCritical_ShouldRaiseCriticalStockEvent()
    {
        // Arrange
        var item = CreateTestItem(stockLevel: StockLevel.Create(10, 5, 100, 7));
        
        // Act
        item.UpdateStockLevel(3); // Reduce below critical level

        // Assert
        item.StockLevel.Current.Should().Be(3);
        
        var stockChangedEvent = item.DomainEvents.Should()
            .ContainSingle(e => e is StockLevelChangedEvent)
            .Subject as StockLevelChangedEvent;
        
        var criticalStockEvent = item.DomainEvents.Should()
            .ContainSingle(e => e is CriticalStockLevelReachedEvent)
            .Subject as CriticalStockLevelReachedEvent;

        stockChangedEvent.Should().NotBeNull();
        stockChangedEvent!.ItemId.Should().Be(item.Id);
        stockChangedEvent.OldQuantity.Should().Be(10);
        stockChangedEvent.NewQuantity.Should().Be(3);
        
        criticalStockEvent.Should().NotBeNull();
        criticalStockEvent!.ItemId.Should().Be(item.Id);
        criticalStockEvent.CurrentQuantity.Should().Be(3);
        criticalStockEvent.CriticalLevel.Should().Be(7);
    }

    [Fact]
    public void UpdateQualityStatus_ShouldRaiseQualityStatusChangedEvent()
    {
        // Arrange
        var item = CreateTestItem();
        
        // Act
        item.UpdateQualityStatus(QualityStatus.Damaged);

        // Assert
        item.QualityStatus.Should().Be(QualityStatus.Damaged);
        
        var qualityChangedEvent = item.DomainEvents.Should()
            .ContainSingle(e => e is QualityStatusChangedEvent)
            .Subject as QualityStatusChangedEvent;

        qualityChangedEvent.Should().NotBeNull();
        qualityChangedEvent!.ItemId.Should().Be(item.Id);
        qualityChangedEvent.NewStatus.Should().Be(QualityStatus.Damaged);
    }

    [Theory]
    [InlineData(-1, 5, 10, 7, "Current stock level cannot be negative")]
    [InlineData(5, -1, 10, 7, "Minimum stock level cannot be negative")]
    [InlineData(5, 10, -1, 7, "Maximum stock level must be greater than minimum")]
    [InlineData(10, 5, 3, 4, "Maximum stock level must be greater than minimum")]
    [InlineData(5, 10, 8, 9, "Maximum stock level must be greater than minimum")]
    [InlineData(5, 3, 10, 2, "Critical level must be greater than or equal to minimum")]
    [InlineData(5, 3, 10, 11, "Critical level must be less than or equal to maximum")]
    public void Create_WithInvalidStockLevels_ShouldThrowException(int current, int min, int max, int critical, string expectedMessage)
    {
        // Act & Assert
        var action = () => StockLevel.Create(current, min, max, critical);

        action.Should().Throw<ArgumentException>()
            .WithMessage($"{expectedMessage}*");
    }

    private static Item CreateTestItem(StockLevel? stockLevel = null)
    {
        return Item.Create(
            SKU.Create("TEST-123"),
            "Test Item",
            ItemType.FinishedGood,
            false,
            stockLevel ?? StockLevel.Create(10, 5, 100, 7));
    }
}
