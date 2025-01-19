using IMS.Domain.Aggregates;
using IMS.Domain.Events.Transactions;
using IMS.Domain.ValueObjects;
using IMS.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace IMS.UnitTests.Domain.Aggregates;

public class TransactionTests
{
    [Fact]
    public void CreateInbound_ShouldCreateTransactionWithCorrectProperties()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var quantity = 10;
        var destinationLocation = "Warehouse A";
        var type = TransactionType.Purchase;
        var manufacturingDate = DateTime.Now.AddDays(-30);
        var expiryDate = DateTime.Now.AddDays(180);
        var batchInfo = BatchInformation.Create("BATCH-001", manufacturingDate, expiryDate);

        // Act
        var transaction = Transaction.CreateInbound(
            itemId,
            quantity,
            destinationLocation,
            type,
            batchInfo);

        // Assert
        transaction.Should().NotBeNull();
        transaction.ItemId.Should().Be(itemId);
        transaction.Type.Should().Be(type);
        transaction.Quantity.Should().Be(quantity);
        transaction.DestinationLocation.Should().Be(destinationLocation);
        transaction.BatchInfo.Should().Be(batchInfo);
        transaction.SourceLocation.Should().BeNull();
        
        var createdEvent = transaction.DomainEvents.Should()
            .ContainSingle(e => e is TransactionCreatedEvent)
            .Subject as TransactionCreatedEvent;
        
        createdEvent.Should().NotBeNull();
        createdEvent!.TransactionId.Should().Be(transaction.Id);
        createdEvent.ItemId.Should().Be(itemId);
        createdEvent.Type.Should().Be(type);
        createdEvent.Quantity.Should().Be(quantity);
    }

    [Fact]
    public void CreateOutbound_ShouldCreateTransactionWithCorrectProperties()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var quantity = 5;
        var sourceLocation = "Warehouse A";
        var type = TransactionType.Sale;

        // Act
        var transaction = Transaction.CreateOutbound(
            itemId,
            quantity,
            sourceLocation,
            type);

        // Assert
        transaction.Should().NotBeNull();
        transaction.ItemId.Should().Be(itemId);
        transaction.Type.Should().Be(type);
        transaction.Quantity.Should().Be(quantity);
        transaction.SourceLocation.Should().Be(sourceLocation);
        transaction.DestinationLocation.Should().BeNull();
        transaction.BatchInfo.Should().BeNull();
        
        var createdEvent = transaction.DomainEvents.Should()
            .ContainSingle(e => e is TransactionCreatedEvent)
            .Subject as TransactionCreatedEvent;
        
        createdEvent.Should().NotBeNull();
        createdEvent!.TransactionId.Should().Be(transaction.Id);
        createdEvent.ItemId.Should().Be(itemId);
        createdEvent.Type.Should().Be(type);
        createdEvent.Quantity.Should().Be(quantity);
    }

    [Fact]
    public void CreateLocationTransfer_ShouldCreateTransactionWithCorrectProperties()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var quantity = 3;
        var sourceLocation = "Warehouse A";
        var destinationLocation = "Warehouse B";

        // Act
        var transaction = Transaction.CreateInternal(
            itemId,
            quantity,
            sourceLocation,
            destinationLocation,
            TransactionType.LocationTransfer);

        // Assert
        transaction.Should().NotBeNull();
        transaction.ItemId.Should().Be(itemId);
        transaction.Type.Should().Be(TransactionType.LocationTransfer);
        transaction.Quantity.Should().Be(quantity);
        transaction.SourceLocation.Should().Be(sourceLocation);
        transaction.DestinationLocation.Should().Be(destinationLocation);
        transaction.BatchInfo.Should().BeNull();
        
        var createdEvent = transaction.DomainEvents.Should()
            .ContainSingle(e => e is TransactionCreatedEvent)
            .Subject as TransactionCreatedEvent;
        
        createdEvent.Should().NotBeNull();
        createdEvent!.TransactionId.Should().Be(transaction.Id);
        createdEvent.ItemId.Should().Be(itemId);
        createdEvent.Type.Should().Be(TransactionType.LocationTransfer);
        createdEvent.Quantity.Should().Be(quantity);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_WithInvalidQuantity_ShouldThrowException(int invalidQuantity)
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var sourceLocation = "Warehouse A";
        var type = TransactionType.Sale;

        // Act
        var action = () => Transaction.CreateOutbound(
            itemId,
            invalidQuantity,
            sourceLocation,
            type);

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("Quantity must be greater than zero*");
    }
}
