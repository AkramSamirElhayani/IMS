using System;
using System.Threading.Tasks;
using FluentAssertions;

using IMS.Domain.Aggregates;
using IMS.Domain.Enums;
using IMS.Infrastructure.Persistence;
using IMS.Infrastructure.Persistence.Repositories;
using IMS.IntegrationTests.Helpers;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace IMS.IntegrationTests.Repositories
{
    public class TransactionRepositoryTests : IClassFixture<TestDatabaseFixture>, IAsyncLifetime
    {
        private readonly TestDatabaseFixture _fixture;
        private readonly IMSDbContext _context;
        private readonly TransactionRepository _repository;
        private readonly Guid _itemId;
        private readonly Item _testItem;

        public TransactionRepositoryTests(TestDatabaseFixture fixture)
        {
            _fixture = fixture;
            _context = fixture.CreateDbContext();

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"ConnectionStrings:DefaultConnection", fixture.ConnectionString}
                })
                .Build();

            _repository = new TransactionRepository(_context, configuration);
            _itemId = Guid.NewGuid();
            _testItem = TestDataFactory.Items.Create( 
                name: "Test Item",
                sku: "TEST001"
            );
            _itemId=_testItem.Id;
        }

        public async Task InitializeAsync()
        {
            await _context.Database.EnsureDeletedAsync();
            await _context.Database.MigrateAsync();
            await SeedTestDataAsync();
        }

        public async Task DisposeAsync()
        {
            await _context.DisposeAsync();
        }

        private async Task SeedTestDataAsync()
        {
            // Add test item
            _context.Items.Add(_testItem);

            // Add transactions that affect stock
            var transactions = new[]
            {
                TestDataFactory.Transactions.CreatePurchase(
                    _itemId, 
                    100, 
                    transactionDate: DateTimeOffset.UtcNow.AddDays(-5)),

                TestDataFactory.Transactions.CreateSale(
                    _itemId, 
                    30, 
                    transactionDate: DateTimeOffset.UtcNow.AddDays(-4)),

                TestDataFactory.Transactions.CreateReturn(
                    _itemId, 
                    10, 
                    transactionDate: DateTimeOffset.UtcNow.AddDays(-3)),

                TestDataFactory.Transactions.CreateSale(
                    _itemId, 
                    20, 
                    transactionDate: DateTimeOffset.UtcNow.AddDays(-2)),

                TestDataFactory.Transactions.CreatePurchase(
                    _itemId, 
                    15, 
                    isCompleted: false,
                    transactionDate: DateTimeOffset.UtcNow.AddDays(-1))
            };

            await _context.Transactions.AddRangeAsync(transactions);
            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task CalculateTotalStockForItem_WithMixedTransactions_ShouldReturnCorrectBalance()
        {
            // Act
            var totalStock = await _repository.CalculateTotalStockForItemAsync(_itemId);

            // Assert
            // Purchase(+100) + Sale(-30) + Return(+10) + Sale(-20) = 60
            // Pending Purchase(+15) should not be counted
            totalStock.Should().Be(60, "because completed transactions sum up to 60");
        }

        [Fact]
        public async Task CalculateTotalStockForItem_WithNonExistentItem_ShouldReturnZero()
        {
            // Act
            var totalStock = await _repository.CalculateTotalStockForItemAsync(Guid.NewGuid());

            // Assert
            totalStock.Should().Be(0, "because non-existent items should have zero stock");
        }

        [Fact]
        public async Task CalculateTotalStockForItem_WithOnlyPendingTransactions_ShouldReturnZero()
        {
            // Arrange
            var item = TestDataFactory.Items.Create();
            _context.Items.Add(item);

            var pendingTransaction = TestDataFactory.Transactions.CreatePurchase(
                item.Id,
                50,
                isCompleted: false,
                transactionDate: DateTimeOffset.UtcNow);

            await _context.Transactions.AddAsync(pendingTransaction);
            await _context.SaveChangesAsync();

            // Act
            var totalStock = await _repository.CalculateTotalStockForItemAsync(item.Id);

            // Assert
            totalStock.Should().Be(0, "because pending transactions should not affect the stock level");
        }

        [Fact]
        public async Task CalculateTotalStockForItem_WithMultipleTransactionTypes_ShouldCalculateCorrectly()
        {
            // Arrange
            var item = TestDataFactory.Items.Create();
            _context.Items.Add(item);

            var transactions = new[]
            {
                TestDataFactory.Transactions.CreatePurchase(
                    item.Id,
                    50,
                    transactionDate: DateTimeOffset.UtcNow.AddMinutes(-30)),

                TestDataFactory.Transactions.CreateTransferIn(
                    item.Id,
                    20,
                    transactionDate: DateTimeOffset.UtcNow.AddMinutes(-20)),

                TestDataFactory.Transactions.CreateSale(
                    item.Id,
                    30,
                    transactionDate: DateTimeOffset.UtcNow.AddMinutes(-10))
            };

            await _context.Transactions.AddRangeAsync(transactions);
            await _context.SaveChangesAsync();

            // Act
            var totalStock = await _repository.CalculateTotalStockForItemAsync(item.Id);

            // Assert
            totalStock.Should().Be(40, "because Purchase(+50) + TransferIn(+20) + Sale(-30) = 40");
        }
    }
}
