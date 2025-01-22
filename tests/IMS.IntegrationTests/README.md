# IMS.IntegrationTests

## Overview
Integration tests for the IMS application, focusing on testing components working together in a near-production environment.

## Purpose
- Test component integration
- Verify database operations
- Test external service integration
- Validate configuration

## Test Categories

### Database Integration
- Repository integration tests
- Entity Framework configuration tests
- Migration tests
- Transaction handling

### Service Integration
- External API integration
- Message broker integration
- File storage integration
- Email service integration

## Test Infrastructure

### Test Database
```csharp
public class TestDbContext : IDisposable
{
    private readonly IMSDbContext _context;
    
    public TestDbContext()
    {
        // Setup in-memory or test database
    }
    
    public void Dispose()
    {
        // Cleanup
    }
}
```

### Test Fixtures
```csharp
public class IntegrationTestFixture : IDisposable
{
    protected readonly IServiceProvider ServiceProvider;
    protected readonly IMSDbContext DbContext;
    
    public IntegrationTestFixture()
    {
        // Setup test services
    }
}
```

## Example Tests

### Repository Integration Tests
```csharp
public class ItemRepositoryTests : IClassFixture<IntegrationTestFixture>
{
    private readonly IItemRepository _repository;
    private readonly IMSDbContext _context;
    
    [Fact]
    public async Task GetBySkuAsync_ExistingItem_ReturnsItem()
    {
        // Test implementation
    }
}
```

### Message Broker Tests
```csharp
public class IntegrationEventTests : IClassFixture<IntegrationTestFixture>
{
    private readonly IIntegrationEventService _eventService;
    
    [Fact]
    public async Task PublishEvent_ValidEvent_PublishesSuccessfully()
    {
        // Test implementation
    }
}
```

## Docker Integration Testing

### Overview
We use Docker containers to run integration tests against a real SQL Server instance. This approach ensures our tests run against the same database engine used in production, particularly important for testing raw SQL queries with Dapper.

### Setup
```csharp
public class TestDatabaseFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _container;
    
    public TestDatabaseFixture()
    {
        _container = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:latest")
            .WithHostname($"integration_tests_db_{Guid.NewGuid()}")
            .WithPassword("Your_password123")
            .WithEnvironment("ACCEPT_EULA", "Y")
            .Build();
    }
}
```

### Testing Dapper Queries
We use Dapper for optimized read operations that require complex SQL queries. Here's how we test them:

1. **Container Setup**: Each test run creates a fresh SQL Server container
2. **Database Migration**: EF Core migrations are automatically applied to ensure correct schema
3. **Connection String**: The container provides a dynamic connection string used by both EF Core and Dapper
4. **Test Isolation**: Each test class gets its own database instance for true isolation

Example of a Dapper query being tested:
```csharp
// Repository implementation
public async Task<int> CalculateTotalStockForItemAsync(Guid itemId)
{
    const string sql = @"
        SELECT COALESCE(SUM(t.Quantity * 
            CASE WHEN CAST(t.Type as int) > 0 THEN 1 ELSE -1 END
        ), 0)
        FROM Transactions t
        WHERE t.ItemId = @ItemId 
        AND t.Status = TRUE";

    using var connection = new SqlConnection(_connectionString);
    return await connection.QuerySingleAsync<int>(sql, new { ItemId = itemId });
}

// Test
[Fact]
public async Task CalculateTotalStockForItem_WithMultipleTransactionTypes_ShouldCalculateCorrectly()
{
    // Arrange
    var item = TestDataFactory.Items.Create();
    _context.Items.Add(item);

    var transactions = new[] {
        TestDataFactory.Transactions.CreatePurchase(item.Id, 50),
        TestDataFactory.Transactions.CreateSale(item.Id, 30)
    };
    _context.Transactions.AddRange(transactions);
    await _context.SaveChangesAsync();

    // Act
    var totalStock = await _repository.CalculateTotalStockForItemAsync(item.Id);

    // Assert
    totalStock.Should().Be(20, "because Purchase(+50) - Sale(-30) = 20");
}
```

### Benefits
1. **Real Database Testing**: Tests run against actual SQL Server, not in-memory
2. **Isolation**: Each test runs in isolation with its own database
3. **Performance**: Docker containers start quickly and are disposed after tests
4. **Reproducibility**: Tests run consistently across different environments
5. **SQL Validation**: Raw SQL queries are validated against real SQL Server syntax

### Best Practices
1. Use `TestDataFactory` to create consistent test data
2. Clean up resources by implementing `IAsyncLifetime`
3. Use unique identifiers for each container to prevent conflicts
4. Keep SQL queries in constants for better maintainability
5. Test edge cases specific to SQL Server behavior

## Best Practices
1. Use test database
2. Clean up after tests
3. Test realistic scenarios
4. Proper error handling
5. Async/await testing

## Tools and Dependencies
- xUnit
- TestContainers
- Respawn
- WireMock.NET

## Configuration
- Test database connection
- Mock external services
- Test environment settings
- Logging configuration

## Test Data Management
1. Seed data
2. Data cleanup
3. Transaction handling
4. Database snapshots
