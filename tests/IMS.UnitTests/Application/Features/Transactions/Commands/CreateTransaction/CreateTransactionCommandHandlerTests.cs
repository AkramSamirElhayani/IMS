using FluentAssertions;
using IMS.Application.Common.Interfaces;
using IMS.Application.Features.Transactions.Commands.CreateTransaction;
using IMS.Domain.Aggregates;
using IMS.Domain.Enums;
using IMS.Domain.ValueObjects;
using IMS.UnitTests.Application.Common;
using Moq;

namespace IMS.UnitTests.Application.Features.Transactions.Commands.CreateTransaction;

public class CreateTransactionCommandHandlerTests : TestBase
{
    private readonly Mock<ITransactionRepository> _transactionRepositoryMock;
    private readonly CreateTransactionCommandHandler _handler;

    public CreateTransactionCommandHandlerTests()
    {
        _transactionRepositoryMock = new Mock<ITransactionRepository>();
        _handler = new CreateTransactionCommandHandler(UnitOfWorkMock.Object, _transactionRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WhenOutTransactionWithInsufficientBalance_ReturnsError()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var command = new CreateTransactionCommand
        {
            ProductId = itemId.ToString(),
            Type = TransactionType.TransferOut.ToString(),
            Quantity = 100, // Trying to transfer out more than available
            TransactionDate = DateTimeOffset.UtcNow
        };

        var item = Item.Create(
            SKU.Create("SKU123"),
            "Test Item",
            ItemType.RawMaterial,
            false,
            StockLevel.Create(50, 0, 1000, 10)); // Only 50 items in stock

        ItemRepositoryMock.Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("InsufficientBalance");
        result.Error.Message.Should().Contain("Insufficient balance");
    }

    [Fact]
    public async Task Handle_WhenOutTransactionWithSufficientBalance_ReturnsSuccess()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var command = new CreateTransactionCommand
        {
            ProductId = itemId.ToString(),
            Type = TransactionType.TransferOut.ToString(),
            Quantity = 30, // Transferring out less than available
            TransactionDate = DateTimeOffset.UtcNow
        };

        var item = Item.Create(
            SKU.Create("SKU123"),
            "Test Item",
            ItemType.RawMaterial,
            false,
            StockLevel.Create(50, 0, 1000, 10)); // 50 items in stock

        ItemRepositoryMock.Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        Transaction? capturedTransaction = null;
        _transactionRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()))
            .Callback<Transaction, CancellationToken>((t, _) => capturedTransaction = t)
            .ReturnsAsync((Transaction t, CancellationToken _) => t);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(capturedTransaction!.Id);

        _transactionRepositoryMock.Verify(x => x.AddAsync(
            It.Is<Transaction>(t => 
                t.ItemId == itemId && 
                t.Type == TransactionType.TransferOut && 
                t.Quantity == 30), 
            It.IsAny<CancellationToken>()), 
            Times.Once);

        UnitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        UnitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenInTransaction_CreatesTransactionSuccessfully()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var command = new CreateTransactionCommand
        {
            ProductId = itemId.ToString(),
            Type = TransactionType.Purchase.ToString(),
            Quantity = 100,
            TransactionDate = DateTimeOffset.UtcNow,
            BatchNumber = "BATCH123",
            ManufactureDate = DateTimeOffset.UtcNow.AddDays(-10),
            ExpiryDate = DateTimeOffset.UtcNow.AddDays(90)
        };

        var item = Item.Create(
            SKU.Create("SKU123"),
            "Test Item",
            ItemType.RawMaterial,
            true,
            StockLevel.Create(50, 0, 1000, 10));

        ItemRepositoryMock.Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        Transaction? capturedTransaction = null;
        _transactionRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()))
            .Callback<Transaction, CancellationToken>((t, _) => capturedTransaction = t)
            .ReturnsAsync((Transaction t, CancellationToken _) => t);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(capturedTransaction!.Id);

        _transactionRepositoryMock.Verify(x => x.AddAsync(
            It.Is<Transaction>(t => 
                t.ItemId == itemId && 
                t.Type == TransactionType.Purchase && 
                t.Quantity == 100 &&
                t.BatchInfo != null &&
                t.BatchInfo.BatchNumber == command.BatchNumber), 
            It.IsAny<CancellationToken>()), 
            Times.Once);

        UnitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        UnitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
