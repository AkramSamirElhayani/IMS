using NetArchTest.Rules;
using FluentAssertions;
using IMS.Domain.Common;
using IMS.Domain.Aggregates;
using IMS.Domain.ValueObjects;
using IMS.Domain.Events;
using System.Runtime.CompilerServices;
using System.Reflection;

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

        result.IsSuccessful.Should().BeTrue(
            "all domain events should inherit from DomainEvent. Failing types: {0}",
            result.FailingTypeNames?.Any() == true 
                ? string.Join(", ", result.FailingTypeNames)
                : "none");
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

        result.IsSuccessful.Should().BeTrue(
            "all aggregates should inherit from Entity. Failing types: {0}",
            result.FailingTypeNames?.Any() == true 
                ? string.Join(", ", result.FailingTypeNames)
                : "none");
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

        result.IsSuccessful.Should().BeTrue(
            "all value objects should inherit from ValueObject. Failing types: {0}",
            result.FailingTypeNames?.Any() == true 
                ? string.Join(", ", result.FailingTypeNames)
                : "none");
    }

    //[Fact]
    //public void DomainEvents_ShouldBeImmutable()
    //{
    //    var assembly = typeof(DomainEvent).Assembly;
    //    var eventTypes = assembly.GetTypes()
    //        .Where(t => t.Namespace?.StartsWith($"{DomainNamespace}.Events") == true)
    //        .Where(t => t != typeof(DomainEvent));

    //    var mutableEvents = eventTypes
    //        .Where(t => t.GetProperties()
    //            .Any(p => p.SetMethod != null && p.SetMethod.IsPublic))
    //        .Select(t => t.FullName)
    //        .ToList();

    //    mutableEvents.Should().BeEmpty("all domain events should be immutable. Events with public setters: {0}",
    //        string.Join("\n", mutableEvents));
    //}

    [Fact]
    public void ValueObjects_ShouldBeSealed()
    {
        var result = Types.InAssembly(typeof(ValueObject).Assembly)
            .That()
            .ResideInNamespace($"{DomainNamespace}.ValueObjects")
            .Should()
            .BeSealed()
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            "all value objects should be sealed. Failing types: {0}",
            result.FailingTypeNames?.Any() == true 
                ? string.Join(", ", result.FailingTypeNames)
                : "none");
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

        result.IsSuccessful.Should().BeTrue(
            "all aggregates should have private setters. Failing types: {0}",
            result.FailingTypeNames?.Any() == true 
                ? string.Join(", ", result.FailingTypeNames)
                : "none");
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

        result.IsSuccessful.Should().BeTrue(
            "domain layer should not depend on other layers. Failing types: {0}",
            result.FailingTypeNames?.Any() == true 
                ? string.Join(", ", result.FailingTypeNames)
                : "none");
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

        result.IsSuccessful.Should().BeTrue(
            "value objects should not have public setters. Failing types: {0}",
            result.FailingTypeNames?.Any() == true 
                ? string.Join(", ", result.FailingTypeNames)
                : "none");
    }

}
