# IMS.Application

## Overview
The Application project implements application-specific business rules and orchestrates the flow of data between the UI and domain layers using the CQRS pattern with MediatR.

## Purpose
- Implement application-specific business rules
- Define commands and queries (CQRS)
- Handle domain events
- Provide data transformation (DTOs)

## Key Components

### Commands
- `CreateItemCommand`: Create new inventory items
- `UpdateItemCommand`: Modify existing items
- `DeleteItemCommand`: Remove items

### Queries
- `GetItemQuery`: Retrieve specific items
- `GetItemsListQuery`: List items with filtering
- `GetStockLevelQuery`: Get current stock levels

### Event Handlers
- `ItemCreatedEventHandler`
- `StockLevelChangedEventHandler`
- `ItemDeletedEventHandler`

### DTOs
- `ItemDto`
- `StockLevelDto`
- `CategoryDto`

## Features

1. **CQRS Implementation**
   ```csharp
   public class CreateItemCommandHandler : IRequestHandler<CreateItemCommand, Result<Guid>>
   {
       private readonly IItemRepository _repository;
       
       public async Task<Result<Guid>> Handle(CreateItemCommand command)
       {
           var item = Item.Create(command.Name, command.SKU, command.Type);
           await _repository.AddAsync(item);
           return Result.Success(item.Id);
       }
   }
   ```

2. **Validation**
   ```csharp
   public class CreateItemCommandValidator : AbstractValidator<CreateItemCommand>
   {
       public CreateItemCommandValidator()
       {
           RuleFor(x => x.Name).NotEmpty().MinimumLength(3);
           RuleFor(x => x.SKU).NotEmpty().Matches(@"^[A-Z0-9]{6,10}$");
       }
   }
   ```

## Design Patterns

1. **CQRS**
   - Separate command and query models
   - Optimized read and write operations
   - Clear separation of concerns

2. **Mediator**
   - Decoupled command/query handlers
   - Centralized request processing
   - Easy to add cross-cutting concerns

3. **Repository Abstractions**
   - Interface-based design
   - Technology-agnostic data access
   - Easy to mock for testing

## Dependencies
- MediatR for CQRS
- FluentValidation for validation
- AutoMapper for object mapping

## Best Practices
1. Keep commands and queries focused
2. Validate all incoming commands
3. Use proper error handling and Result types
4. Implement proper logging and monitoring
5. Follow SOLID principles

## Integration Events
The application layer also handles the transformation of domain events to integration events for cross-module communication:

```csharp
public class StockLevelChangedEventHandler : 
    INotificationHandler<StockLevelChangedEvent>
{
    private readonly IIntegrationEventService _integrationEvents;
    
    public async Task Handle(StockLevelChangedEvent notification)
    {
        var integrationEvent = new StockLevelChangedIntegrationEvent(
            notification.ItemId,
            notification.NewLevel
        );
        
        await _integrationEvents.PublishAsync(integrationEvent);
    }
}
```
