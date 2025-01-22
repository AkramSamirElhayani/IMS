# IMS.Domain

## Overview
The Domain project is the core of the IMS application, implementing Domain-Driven Design principles. It contains all enterprise business rules and entities.

## Purpose
- Define core business concepts
- Implement business rules and invariants
- Provide domain events for state changes
- Define value objects and entities

## Key Components

### Entities
- `Item`: Core entity representing inventory items
- `StockLevel`: Entity tracking inventory levels
- `Category`: Classification of items

### Value Objects
- `Money`: Represents monetary values
- `Quantity`: Represents item quantities with validation
- `SKU`: Stock Keeping Unit identifier

### Domain Events
- `ItemCreatedEvent`
- `StockLevelChangedEvent`
- `ItemDeletedEvent`

### Enums
- `ItemType`: Types of inventory items
- `StockStatus`: Current status of stock levels

## Design Decisions

1. **Rich Domain Model**
   - Entities contain business logic
   - Value objects for immutable concepts
   - Strong encapsulation of business rules

2. **Domain Events**
   - Used for tracking important domain changes
   - Enable loose coupling between components
   - Support eventual consistency

3. **Immutable Value Objects**
   - Ensure data integrity
   - Thread-safe by design
   - Simplified testing

## Dependencies
- No external dependencies
- Pure C# implementation
- Framework-agnostic

## Best Practices
1. Always validate domain invariants
2. Use value objects for complex value types
3. Raise domain events for significant state changes
4. Keep entities focused and cohesive

## Example Usage

```csharp
// Creating a new item
var item = Item.Create(
    name: "Test Item",
    sku: SKU.Create("ABC123"),
    type: ItemType.RawMaterial
);

// Changing stock level
item.UpdateStockLevel(
    StockLevel.Create(
        current: 10,
        minimum: 5,
        maximum: 100,
        critical: 3
    )
);
```
