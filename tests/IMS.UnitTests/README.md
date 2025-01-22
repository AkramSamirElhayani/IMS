# IMS.UnitTests

## Overview
Unit tests for the IMS application, focusing on testing individual components in isolation.

## Test Categories

### Domain Tests
- Entity behavior tests
- Value object validation
- Domain event tests
- Business rule validation

### Application Tests
- Command handler tests
- Query handler tests
- Validation tests
- DTO mapping tests

### Infrastructure Tests
- Repository tests
- Service implementation tests
- Data access tests

## Testing Patterns

1. **Arrange-Act-Assert**
```csharp
[Fact]
public void Item_WithValidData_CreatesSuccessfully()
{
    // Arrange
    var name = "Test Item";
    var sku = "ABC123";

    // Act
    var item = Item.Create(name, sku);

    // Assert
    Assert.Equal(name, item.Name);
    Assert.Equal(sku, item.SKU.Value);
}
```

2. **Object Mother Pattern**
```csharp
public static class ItemMother
{
    public static Item CreateValidItem()
    {
        return Item.Create(
            "Test Item",
            "ABC123",
            ItemType.RawMaterial
        );
    }
}
```

## Best Practices
1. Test one thing at a time
2. Use meaningful test names
3. Keep tests independent
4. Use proper test categories
5. Mock external dependencies

## Tools and Frameworks
- xUnit
- Moq
- FluentAssertions
- AutoFixture

## Example Tests

### Value Object Tests
```csharp
public class MoneyTests
{
    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Create_WithNegativeAmount_ThrowsException(decimal amount)
    {
        Assert.Throws<InvalidOperationException>(() => 
            Money.Create(amount));
    }
}
```

### Command Handler Tests
```csharp
public class CreateItemCommandHandlerTests
{
    private readonly Mock<IItemRepository> _repository;
    private readonly CreateItemCommandHandler _handler;

    public CreateItemCommandHandlerTests()
    {
        _repository = new Mock<IItemRepository>();
        _handler = new CreateItemCommandHandler(_repository.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesItem()
    {
        // Test implementation
    }
}
```
