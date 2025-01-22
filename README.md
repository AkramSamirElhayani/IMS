# IMS (Inventory Management System)

## Project Purpose

This repository serves as a learning and demonstration project showcasing modern software architecture and development practices in .NET. While it implements a basic inventory management system, its primary focus is on demonstrating:

- Clean Architecture implementation
- Domain-Driven Design patterns
- CQRS with MediatR
- Modern WPF development with MVVM
- Testing practices and patterns

> **Important**: This project is designed for educational and demonstration purposes. The business logic is intentionally simplified to focus on architectural patterns and coding practices. It should not be considered a production-ready inventory management solution.

### Learning Objectives

1. **Architecture Patterns**
   - Implementing Clean Architecture in a real-world application
   - Separating concerns across different layers
   - Managing dependencies through dependency injection

2. **Modern .NET Development**
   - Using latest .NET features and best practices
   - Implementing MVVM pattern with modern tooling
   - Working with asynchronous programming patterns

3. **Testing Approaches**
   - Writing unit tests for different architectural layers
   - Implementing UI tests for WPF applications
   - Using mocking and test doubles effectively

## Overview
IMS is a modern, enterprise-grade Inventory Management System built with .NET 9.0. It provides a robust solution for managing inventory with a clean, maintainable architecture and a rich WPF-based user interface.

## Technology Stack

### Backend
- **.NET 9.0**: Latest .NET framework providing enhanced performance and modern language features
- **C# 12**: Utilizing the latest C# features for clean and efficient code
- **MediatR**: Implementation of the Mediator pattern for decoupled communication between components
- **Entity Framework Core**: ORM for database operations with clean separation of concerns
- **Domain-Driven Design (DDD)**: Implementation of domain-driven design principles for complex business logic

### Frontend
- **WPF (Windows Presentation Foundation)**: Rich desktop UI framework
- **MVVM Pattern**: Clean separation of concerns between View and ViewModel layers
- **CommunityToolkit.Mvvm**: Modern MVVM toolkit for clean and maintainable UI code

### Testing
- **xUnit**: Unit testing framework
- **Moq**: Mocking framework for isolated unit tests
- **FluentAssertions**: Fluent API for more readable test assertions
- **Architecture Tests**: Ensuring architectural boundaries and dependencies
- **Integration Tests**: Testing component integration
- **UI Tests**: Testing the user interface layer

## Architecture

The solution follows Clean Architecture principles with clear separation of concerns:

### Core Layer
1. **Domain (IMS.Domain)**
   - Contains enterprise/business logic
   - Implements DDD patterns (Entities, Value Objects, Aggregates)
   - Pure C# with no external dependencies
   - Business rules and invariants

2. **Application (IMS.Application)**
   - Application business rules
   - CQRS implementation with MediatR
   - Command and Query handlers
   - Interface definitions for infrastructure
   - DTOs and mapping profiles

### Infrastructure Layer
- Database access implementation
- External service integrations
- Repository implementations
- Logging, caching, and cross-cutting concerns

### Presentation Layer
- WPF UI implementation
- MVVM pattern implementation
- ViewModels with clean separation from Views
- UI-specific mapping and validation

## Modular Monolith Architecture

The project is designed to support a Modular Monolith architecture, allowing for the addition of new modules while maintaining loose coupling through event-driven communication.

### Module Communication Pattern
```
┌────────────────────────────────────────────────────────────────┐
│                     Integration Event Bus                      │
└─────────────────┬─────────────────┬──────────────┬─────────────┘
                  │                 │              │
        ┌─────────▼──────┐  ┌──────▼───────┐ ┌─────▼────────┐
        │  Inventory     │  │   Sales      │ │  Future      │
        │   Module       │  │   Module     │ │  Module      │
        └───────┬────────┘  └──────┬───────┘ └───────┬──────┘
                │                  │                 │
     ┌──────────▼──────────┐       │                 │
     │    Domain Events    │       │                 │
     └──────────┬──────────┘       │                 │
                │                  │                 │
     ┌──────────▼──────────┐       │                 │
     │ Integration Events  │       │                 │
     └──────────┬──────────┘       │                 │
                │                  │                 │
                └──────────────────┴─────────────────┘
```

### Module Independence and Communication

1. **Module Structure**
   - Each module has its own bounded context
   - Independent domain model and business rules
   - Separate data storage capabilities
   - Module-specific UI components

2. **Event Types**
   - **Domain Events**: Internal to a module
     - Handled within the same bounded context
     - Maintain module's data consistency
     - Trigger internal workflows
   
   - **Integration Events**: Between modules
     - Cross-module communication
     - Eventual consistency across modules
     - Loose coupling between modules

3. **Event Flow**
   ```
   ┌──────────┐    ┌───────────┐    ┌────────────┐    ┌──────────┐
   │ Domain   │    │Integration│    │Event Bus   │    │Receiving │
   │ Event    │───▶│Event      │───▶│            │───▶│Module    │
   └──────────┘    └───────────┘    └────────────┘    └──────────┘
   ```

### Benefits of This Approach

1. **Scalability**
   - Modules can be developed independently
   - Easy to add new modules
   - Each module can scale separately

2. **Maintainability**
   - Clear module boundaries
   - Independent deployment possible
   - Easier to manage team ownership

3. **Flexibility**
   - Modules can use different technologies
   - Easy to replace or upgrade modules
   - Support for different data storage per module

4. **Evolution Path**
   - Can evolve into microservices if needed
   - Gradual migration possibility
   - No distributed system complexity initially

### Implementation Details

1. **Event Publishing**
```csharp
// Domain Event
public class StockLevelChangedEvent : DomainEvent
{
    public Guid ItemId { get; }
    public int NewQuantity { get; }
}

// Integration Event
public class StockLevelUpdatedIntegrationEvent : IntegrationEvent
{
    public Guid ItemId { get; }
    public int NewQuantity { get; }
    public string ItemName { get; }
}
```

2. **Event Transformation**
```csharp
public class StockLevelChangedEventHandler : 
    INotificationHandler<StockLevelChangedEvent>
{
    private readonly IIntegrationEventService _integrationEventService;

    public async Task Handle(StockLevelChangedEvent event)
    {
        // Transform domain event to integration event
        var integrationEvent = new StockLevelUpdatedIntegrationEvent
        {
            ItemId = event.ItemId,
            NewQuantity = event.NewQuantity
        };

        // Publish to other modules
        await _integrationEventService.PublishThroughEventBus(integrationEvent);
    }
}
```

This architecture allows us to:
- Start with a monolithic approach for simplicity
- Maintain clear boundaries between modules
- Support future growth and complexity
- Enable easy addition of new modules
- Maintain loose coupling through events

## Key Features

1. **Clean Architecture**
   - Clear separation of concerns
   - Dependency inversion principle
   - Testable design
   - Domain-centric architecture

2. **CQRS Pattern**
   - Separate command and query models
   - Optimized read and write operations
   - Clear separation of mutation and query concerns

3. **Rich Domain Model**
   - Encapsulated business rules
   - Value objects for immutable concepts
   - Strong domain invariants
   - Rich domain events

4. **Comprehensive Testing**
   - Unit tests for business logic
   - Integration tests for infrastructure
   - UI tests for presentation layer
   - Architecture tests for maintaining boundaries

5. **Modern UI**
   - Responsive WPF interface
   - MVVM pattern for maintainable UI code
   - Rich user experience
   - Validation and error handling

## Key Architectural Decisions and Benefits

### 1. Clean Architecture
**Decision**: Implemented a strict layered architecture with Domain, Application, Infrastructure, and Presentation layers.

**Benefits**:
- ✨ Independent of frameworks, UI, and databases
- 🎯 Highly testable with business rules isolated in the domain layer
- 🔄 Easy to modify and maintain as changes are isolated to specific layers
- 📦 Better dependency management with clear boundaries
- 🚀 Ability to defer technical decisions as core business logic is isolated

### 2. Domain-Driven Design (DDD)
**Decision**: Used DDD patterns for domain modeling with entities, value objects, and aggregates.

**Benefits**:
- 🏢 Rich domain model that accurately represents business concepts
- 💡 Encapsulated business rules within domain objects
- 🛡️ Strong invariants and validation at the domain level
- 📚 Shared language between developers and domain experts
- 🔒 Better data consistency through aggregate boundaries

### 3. CQRS with MediatR
**Decision**: Separated read and write operations using CQRS pattern, implemented via MediatR.

**Benefits**:
- 📈 Optimized read and write paths independently
- 🎭 Clear separation of command and query responsibilities
- 🔍 Simplified query models for better read performance
- 📝 Easier to maintain and modify individual operations
- 🎯 Better support for eventual consistency when needed

### 4. WPF with MVVM (CommunityToolkit.Mvvm)
**Decision**: Used modern MVVM implementation with CommunityToolkit.Mvvm for UI development.

**Benefits**:
- 🎨 Clean separation of UI and business logic
- 🔄 Two-way data binding for responsive UI
- 🧪 Highly testable ViewModels
- 📱 Reusable view models across different views
- ⚡ Source generation for better performance

### 5. Comprehensive Testing Strategy
**Decision**: Implemented multiple testing layers (Unit, Integration, UI, Architecture).

**Benefits**:
- 🛡️ Early detection of issues across all layers
- 🎯 Confidence in architectural boundaries
- 🔄 Safe refactoring with test coverage
- 📊 Better code quality through TDD practices
- 🚀 Regression prevention

### 6. Value Objects for Domain Concepts
**Decision**: Used Value Objects for domain concepts like StockLevel, ItemType.

**Benefits**:
- 🎯 Encapsulated validation logic
- 🔒 Immutable by design
- 💡 Better domain modeling
- 🧪 Easier to test
- 📚 Self-documenting code

### 7. Repository Pattern with Specification
**Decision**: Implemented repositories with specification pattern for data access.

**Benefits**:
- 📦 Encapsulated data access logic
- 🔄 Easily swappable data sources
- 🎯 Reusable query specifications
- 🧪 Mockable for testing
- 📈 Optimized query composition

### 8. Fluent Validation
**Decision**: Used FluentValidation for complex validation rules.

**Benefits**:
- 📝 Readable validation rules
- 🔄 Reusable validation logic
- 🎯 Separation of validation from domain logic
- 🧪 Testable validation rules
- 💡 Easy to modify and extend

### 9. Event-Driven Design
**Decision**: Implemented domain events for cross-cutting concerns.

**Benefits**:
- 🔄 Loose coupling between components
- 📦 Better separation of concerns
- 🎯 Easier to add new functionality
- 📈 Scalable architecture
- 🧪 Testable event handlers

### 10. Dependency Injection
**Decision**: Used built-in .NET DI container with scrutor for assembly scanning.

**Benefits**:
- 🎯 Loose coupling between components
- 🧪 Easy to mock dependencies for testing
- 📦 Better lifecycle management
- 🔄 Easy to swap implementations
- 💡 Clear dependency graph

These architectural decisions were made to create a maintainable, testable, and scalable application while serving as a learning platform for modern .NET development practices.

## Design Decisions

1. **Clean Architecture**: Chosen for its clear separation of concerns and ability to evolve different layers independently.

2. **CQRS**: Implemented to separate read and write concerns, allowing for optimization of each path independently.

3. **DDD**: Used to handle complex business rules and ensure the domain model accurately reflects business requirements.

4. **WPF with MVVM**: Selected for creating a rich desktop experience while maintaining testability and separation of concerns.

5. **Comprehensive Testing Strategy**: Multiple testing layers ensure reliability and maintainability of the system.

## Architecture and Interaction Diagrams

### Clean Architecture Layers
```
┌─────────────────────────────────────────┐
│            Presentation Layer           │
│  ┌──────────────────────────────────┐   │
│  │      Views       ViewModels      │   │
│  └──────────────────────────────────┘   │
├─────────────────────────────────────────┤
│            Application Layer            │
│  ┌─────────────┐      ┌──────────────┐  │
│  │  Commands   │      │   Queries    │  │
│  │   Handlers  │      │   Handlers   │  │
│  └─────────────┘      └──────────────┘  │
├─────────────────────────────────────────┤
│              Domain Layer               │
│  ┌─────────┐ ┌─────────┐ ┌──────────┐   │
│  │Entities │ │  Value  │ │ Domain   │   │
│  │         │ │ Objects │ │ Services │   │
│  └─────────┘ └─────────┘ └──────────┘   │
├─────────────────────────────────────────┤
│          Infrastructure Layer           │
│  ┌──────────┐ ┌───────────┐ ┌────────┐  │
│  │Repository│ │Persistence│ │External│  │
│  │   Impl   │ │  Context  │ │Services│  │
│  └──────────┘ └───────────┘ └────────┘  │
└─────────────────────────────────────────┘
```

### CQRS Flow with MediatR
```
┌──────────┐     ┌───────────┐     ┌──────────┐
│          │     │           │     │Command   │
│   UI     │────▶│  MediatR  │────▶│Handler   │
│          │     │           │     │          │
└──────────┘     └───────────┘     └──────────┘
     │                │                  │
     │                │                  │
     │                ▼                  ▼
┌──────────┐     ┌───────────┐     ┌──────────┐
│          │     │           │     │   Query  │
│  View    │◀────│  Result   │◀────│  Handler │
│          │     │           │     │          │
└──────────┘     └───────────┘     └──────────┘
```

### Event Flow and Domain Events
```
┌──────────┐    ┌───────────┐    ┌──────────────┐
│Command   │    │  Domain   │    │EventHandler 1│
│Handler   │───▶│  Event    │───▶│(Notification)│
└──────────┘    └───────────┘    └──────────────┘
                      │
                      │          ┌──────────────┐
                      └─────────▶│EventHandler 2│
                      │          │(Email)       │
                      │          └──────────────┘
                      │
                      │          ┌──────────────┐
                      └─────────▶│EventHandler 3│
                                 │(Logging)     │
                                 └──────────────┘
```

### MVVM Pattern Implementation
```
┌───────────────────────────────────────┐
│               View                    │
│  ┌─────────────┐    ┌─────────────┐   │
│  │   XAML UI   │◀──▶│   Code      │   │
│  │             │    │   Behind    │   │
│  └─────────────┘    └─────────────┘   │
│           ▲                ▲          │
│           │                │          │
│           ▼                ▼          │
│  ┌─────────────────────────────────┐  │
│  │         Data Binding            │  │
│  └─────────────────────────────────┘  │
└───────────────┬───────────────────────┘
                │
                ▼
┌───────────────────────────────────────┐
│            ViewModel                  │
│  ┌─────────────┐    ┌─────────────┐   │
│  │  Properties │    │  Commands   │   │
│  │             │    │             │   │
│  └─────────────┘    └─────────────┘   │
│           ▲                ▲          │
│           │                │          │
│           ▼                ▼          │
│  ┌─────────────────────────────────┐  │
│  │      Property Changed Events    │  │
│  └─────────────────────────────────┘  │
└───────────────┬───────────────────────┘
                │
                ▼
┌───────────────────────────────────────┐
│              Model                    │
│  ┌─────────────┐    ┌─────────────┐   │
│  │   Domain    │    │  Business   │   │
│  │   Entities  │    │   Logic     │   │
│  └─────────────┘    └─────────────┘   │
└───────────────────────────────────────┘
```

### Repository Pattern with Specification
```
┌──────────┐     ┌───────────┐     ┌──────────┐
│          │     │           │     │          │
│ViewModel │────▶│Repository │────▶│  Spec    │
│          │     │           │     │          │
└──────────┘     └───────────┘     └──────────┘
                       │                 │
                       │                 ▼
                       │           ┌──────────┐
                       │           │Expression│
                       │           │  Tree    │
                       │           └──────────┘
                       ▼                 │
                 ┌───────────┐           │
                 │           │           │
                 │   EF      │◀──────────┘
                 │ Context   │
                 │           │
                 └───────────┘
```

These diagrams illustrate:
1. The layered architecture and dependencies
2. How commands and queries flow through the system
3. How domain events are published and handled
4. The MVVM pattern implementation
5. How the repository pattern works with specifications

Each component is clearly separated and shows its relationships with other parts of the system. The arrows indicate the direction of dependencies and data flow.

## Getting Started

1. Clone the repository
2. Ensure .NET 9.0 SDK is installed
3. Restore NuGet packages
4. Build the solution
5. Run the application

## Project Structure

```
IMS/
├── src/
│   ├── Core/
│   │   ├── IMS.Domain/        # Domain entities and logic
│   │   └── IMS.Application/   # Application business rules
│   ├── Infrastructure/        # External concerns implementation
│   └── Presentation/         # UI implementation
└── tests/
    ├── IMS.UnitTests/        # Unit tests
    ├── IMS.IntegrationTests/ # Integration tests
    ├── IMS.UITests/          # UI tests
    └── IMS.ArchitectureTests/# Architecture tests
```

## Project Roadmap

### Phase 1: Core Infrastructure ✅
- [x] Set up Clean Architecture project structure
- [x] Implement Domain layer with core entities and value objects
- [x] Set up CQRS with MediatR
- [x] Implement basic repository patterns
- [x] Configure Entity Framework Core
- [x] Set up basic testing infrastructure

### Phase 2: Basic Inventory Management ✅
- [x] Implement Item management (CRUD operations)
- [x] Add inventory tracking functionality
- [x] Implement stock level monitoring
- [x] Add basic validation rules
- [x] Create basic WPF UI structure
- [x] Implement MVVM pattern with CommunityToolkit.Mvvm

### Phase 3: Advanced Features 🚧
- [x] Implement user authentication and authorization
- [x] Add inventory alerts and notifications
- [ ] Implement batch operations for items
- [ ] Add reporting functionality
- [ ] Implement data export/import features
- [ ] Add inventory forecasting

### Phase 4: UI/UX Improvements 🚧
- [x] Implement modern UI design with Fluent UI
- [x] Add responsive layouts
- [x] Implement advanced validation feedback
- [ ] Add interactive dashboards
- [ ] Implement real-time updates
- [ ] Add data visualization components

### Phase 5: Testing and Quality Assurance 🚧
- [x] Unit tests for core business logic
- [ ] Integration tests for data access
- [x] Basic UI tests
- [ ] Performance testing
- [ ] Load testing
- [ ] End-to-end testing scenarios

### Phase 6: Documentation and Deployment 📝
- [x] Basic README documentation
- [ ] CI/CD pipeline setup

### Phase 7: Additional Features (Planned) 🔲
- [ ] Multi-language support
- [ ] Barcode/QR code scanning

### Legend
- ✅ Completed
- 🚧 In Progress
- 📝 Partially Complete
- 🔲 Not Started

## Contributing

Please read our contributing guidelines before submitting pull requests.

## License

This project is licensed under the MIT License - see the LICENSE file for details.
