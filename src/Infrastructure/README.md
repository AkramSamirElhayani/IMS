# IMS.Infrastructure

## Overview
The Infrastructure project provides implementations for all external concerns including data access, external services, and cross-cutting concerns.

## Purpose
- Implement data persistence
- Provide external service integrations
- Handle cross-cutting concerns
- Implement repository interfaces

## Key Components

### Data Access
- Entity Framework Core configuration
- Repository implementations
- Database migrations
- Query specifications

### External Services
- Email service implementation
- File storage service
- Integration event service
- External API clients

### Cross-Cutting Concerns
- Logging implementation
- Caching services
- Authentication/Authorization
- Transaction management

## Features

1. **Repository Implementation**
```csharp
public class ItemRepository : Repository<Item>, IItemRepository
{
    private readonly IMSDbContext _context;
    
    public async Task<Item> GetBySkuAsync(string sku)
    {
        return await _context.Items
            .FirstOrDefaultAsync(i => i.SKU.Value == sku);
    }
}
```

2. **Specification Pattern**
```csharp
public class InStockItemsSpecification : Specification<Item>
{
    public override Expression<Func<Item, bool>> Criteria =>
        item => item.StockLevel.Current > 0;
}
```

## Database Configuration

1. **Entity Configurations**
```csharp
public class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Name).HasMaxLength(200);
        builder.OwnsOne(i => i.SKU);
    }
}
```

2. **Value Object Conversions**
```csharp
public class MoneyValueConverter : ValueConverter<Money, decimal>
{
    public MoneyValueConverter()
        : base(
            money => money.Amount,
            value => Money.Create(value))
    {
    }
}
```

## Integration Events Implementation

```csharp
public class IntegrationEventService : IIntegrationEventService
{
    private readonly IEventBus _eventBus;
    
    public async Task PublishAsync(IntegrationEvent @event)
    {
        // Ensure transactional consistency
        // Publish to message broker
        await _eventBus.PublishAsync(@event);
    }
}
```

## Dependencies
- Entity Framework Core
- Azure Storage (optional)
- RabbitMQ.Client (optional)
- Serilog

## Configuration
- Database connection strings
- External service endpoints
- Authentication settings
- Logging configuration

## Best Practices
1. Use specification pattern for queries
2. Implement proper unit of work
3. Handle transactional consistency
4. Implement retry policies
5. Proper error handling and logging

## Deployment Considerations
- Database migrations
- Connection string management
- Service dependencies
- Infrastructure as Code support
