using IMS.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace IMS.UnitTests.Domain.ValueObjects;

public class SKUTests
{
    [Fact]
    public void Create_WithValidValue_ShouldCreateSKU()
    {
        // Arrange
        var value = "SKU-123";

        // Act
        var sku = SKU.Create(value);

        // Assert
        sku.Should().NotBeNull();
        sku.Value.Should().Be(value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData(" ")]
    public void Create_WithInvalidValue_ShouldThrowException(string invalidValue)
    {
        // Act
        var action = () => SKU.Create(invalidValue);

        // Assert
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Equals_WithSameValue_ShouldBeEqual()
    {
        // Arrange
        var sku1 = SKU.Create("SKU-123");
        var sku2 = SKU.Create("SKU-123");

        // Act & Assert
        sku1.Should().Be(sku2);
        sku1.GetHashCode().Should().Be(sku2.GetHashCode());
    }

    [Fact]
    public void Equals_WithDifferentValue_ShouldNotBeEqual()
    {
        // Arrange
        var sku1 = SKU.Create("SKU-123");
        var sku2 = SKU.Create("SKU-456");

        // Act & Assert
        sku1.Should().NotBe(sku2);
        sku1.GetHashCode().Should().NotBe(sku2.GetHashCode());
    }
}
