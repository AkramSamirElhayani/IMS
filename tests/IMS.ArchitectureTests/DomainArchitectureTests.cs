using NetArchTest.Rules;
using FluentAssertions;
using IMS.Domain.Common;
using IMS.Domain.Aggregates;
using IMS.Domain.ValueObjects;
using IMS.Domain.Events;

namespace IMS.ArchitectureTests;

public class DomainArchitectureTests
{
    private const string DomainNamespace = "IMS.Domain";
    
    [Fact]
    public void DomainEvents_ShouldInheritFromDomainEvent()
    {
        var result = Types.InAssembly(typeof(DomainEvent).Assembly)
            .That()
            .ResideInNamespace($"{DomainNamespace}.Events")
            .Should()
            .Inherit(typeof(DomainEvent))
            .GetResult();

        result.IsSuccessful.Should().BeTrue("all domain events should inherit from DomainEvent");
    }

    [Fact]
    public void Aggregates_ShouldInheritFromEntity()
    {
        var result = Types.InAssembly(typeof(Entity).Assembly)
            .That()
            .ResideInNamespace($"{DomainNamespace}.Aggregates")
            .Should()
            .Inherit(typeof(Entity))
            .GetResult();

        result.IsSuccessful.Should().BeTrue("all aggregates should inherit from Entity");
    }

    [Fact]
    public void ValueObjects_ShouldInheritFromValueObject()
    {
        var result = Types.InAssembly(typeof(ValueObject).Assembly)
            .That()
            .ResideInNamespace($"{DomainNamespace}.ValueObjects")
            .Should()
            .Inherit(typeof(ValueObject))
            .GetResult();

        result.IsSuccessful.Should().BeTrue("all value objects should inherit from ValueObject");
        
        if (!result.IsSuccessful)
        {
            result.FailingTypeNames.Should().BeEmpty("found types that don't inherit from ValueObject: {0}", 
                string.Join(", ", result.FailingTypeNames));
        }
    }

    [Fact]
    public void DomainEvents_ShouldBeImmutable()
    {
        var result = Types.InAssembly(typeof(DomainEvent).Assembly)
            .That()
            .ResideInNamespace($"{DomainNamespace}.Events")
            .Should()
            .BeImmutable()
            .GetResult();

        result.IsSuccessful.Should().BeTrue("all domain events should be immutable");
        
        if (!result.IsSuccessful)
        {
            result.FailingTypeNames.Should().BeEmpty("found mutable domain events: {0}", 
                string.Join(", ", result.FailingTypeNames));
        }
    }

    [Fact]
    public void ValueObjects_ShouldBeSealed()
    {
        var result = Types.InAssembly(typeof(ValueObject).Assembly)
            .That()
            .ResideInNamespace($"{DomainNamespace}.ValueObjects")
            .Should()
            .BeSealed()
            .GetResult();

        result.IsSuccessful.Should().BeTrue("all value objects should be sealed");
        
        if (!result.IsSuccessful)
        {
            result.FailingTypeNames.Should().BeEmpty("found unsealed value objects: {0}", 
                string.Join(", ", result.FailingTypeNames));
        }
    }

    [Fact]
    public void Aggregates_ShouldHavePrivateSetters()
    {
        var result = Types.InAssembly(typeof(Entity).Assembly)
            .That()
            .ResideInNamespace($"{DomainNamespace}.Aggregates")
            .Should()
            .BeImmutable()
            .GetResult();

        result.IsSuccessful.Should().BeTrue("all aggregates should have private setters");
        
        if (!result.IsSuccessful)
        {
            result.FailingTypeNames.Should().BeEmpty("found aggregates with public setters: {0}", 
                string.Join(", ", result.FailingTypeNames));
        }
    }

    [Fact]
    public void DomainLayer_ShouldNotDependOnOtherLayers()
    {
        var result = Types.InAssembly(typeof(DomainEvent).Assembly)
            .Should()
            .NotHaveDependencyOn("IMS.Application")
            .And()
            .NotHaveDependencyOn("IMS.Infrastructure")
            .And()
            .NotHaveDependencyOn("IMS.API")
            .GetResult();

        result.IsSuccessful.Should().BeTrue("domain layer should not depend on other layers");
        
        if (!result.IsSuccessful)
        {
            result.FailingTypeNames.Should().BeEmpty("found types with invalid dependencies: {0}", 
                string.Join(", ", result.FailingTypeNames));
        }
    }

    [Fact]
    public void ValueObjects_ShouldNotHavePublicSetters()
    {
        var result = Types.InAssembly(typeof(ValueObject).Assembly)
            .That()
            .ResideInNamespace($"{DomainNamespace}.ValueObjects")
            .Should()
            .BeImmutable()
            .GetResult();

        result.IsSuccessful.Should().BeTrue("value objects should not have public setters");
        
        if (!result.IsSuccessful)
        {
            result.FailingTypeNames.Should().BeEmpty("found value objects with public setters: {0}", 
                string.Join(", ", result.FailingTypeNames));
        }
    }
}
