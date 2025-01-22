using FluentAssertions;
using IMS.Presentation.ViewModels;
using MediatR;
using Moq;
using Xunit; 
using System.ComponentModel.DataAnnotations;
using IMS.Services.Interfaces;
using IMS.Application.Features.Items.Commands;
using IMS.Application.Features.Items.Common.Responses;
using IMS.Domain.Enums;
using IMS.Domain.ValueObjects;

namespace IMS.UITests.ViewModels;

public class ItemViewModelTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IDialogService> _dialogServiceMock;
    private readonly Mock<INavigationService> _navigationServiceMock;
    private readonly ItemViewModel _viewModel;

    public ItemViewModelTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _dialogServiceMock = new Mock<IDialogService>();
        _navigationServiceMock = new Mock<INavigationService>();
        
        _viewModel = new ItemViewModel(
            _mediatorMock.Object,
            _dialogServiceMock.Object,
            _navigationServiceMock.Object);
    }

    [Fact]
    public void WhenInitialized_DefaultValuesAreSet()
    {
        // Assert
        _viewModel.Name.Should().BeEmpty();
        _viewModel.Quantity.Should().Be(0);
        _viewModel.IsPerishable.Should().BeFalse();
        _viewModel.Type.Should().BeEmpty();
        _viewModel.Sku.Should().BeEmpty();
        _viewModel.MinimumQuantity.Should().Be(0);
        _viewModel.MaximumQuantity.Should().Be(0);
        _viewModel.ReorderPoint.Should().Be(0);
        _viewModel.IsBusy.Should().BeFalse();
        _viewModel.IsEditing.Should().BeFalse();
    }

    [Theory]
    [InlineData("", false)]
    [InlineData("ab", false)]
    [InlineData("abc", true)]
    [InlineData("valid name", true)]
    public void Name_Validation(string name, bool isValid)
    {
        // Act
        _viewModel.Name = name;

        // Assert
        var hasNameError = _viewModel.Errors?.Any(e => e.MemberNames.Contains(nameof(ItemViewModel.Name))) ?? false;
        hasNameError.Should().Be(!isValid);
    }

    [Theory]
    [InlineData("", false)]
    [InlineData("123", false)]
    [InlineData("ABC123", true)]
    [InlineData("ABCD123456", true)]
    [InlineData("abcd123", false)]
    [InlineData("ABC123456789", false)]
    public void SKU_Validation(string sku, bool isValid)
    {
        // Act
        _viewModel.Sku = sku;

        // Assert
        var hasSkuError = _viewModel.Errors?.Any(e => e.MemberNames.Contains(nameof(ItemViewModel.Sku))) ?? false;
        hasSkuError.Should().Be(!isValid);
    }

    [Theory]
    [InlineData(-1, false)]
    [InlineData(0, true)]
    [InlineData(100, true)]
    public void Quantity_Validation(int quantity, bool isValid)
    {
        // Act
        _viewModel.Quantity = quantity;

        // Assert
        var hasQuantityError = _viewModel.Errors?.Any(e => e.MemberNames.Contains(nameof(ItemViewModel.Quantity))) ?? false;
        hasQuantityError.Should().Be(!isValid);
    }

    [Fact]
    public void SaveCommand_WhenNameIsEmpty_CannotExecute()
    {
        // Arrange
        _viewModel.Name = string.Empty;

        // Act & Assert
        _viewModel.SaveCommand.CanExecute(null).Should().BeFalse();
    }

    [Fact]
    public void SaveCommand_WhenNameIsValid_CanExecute()
    {
        // Arrange
        _viewModel.Name = "Valid Name";

        // Act & Assert
        _viewModel.SaveCommand.CanExecute(null).Should().BeTrue();
    }

    [Fact]
    public void CancelEdit_NavigatesToItemsList()
    {
        // Act
        _viewModel.CancelEditCommand.Execute(null);

        // Assert
        _navigationServiceMock.Verify(x => x.NavigateTo<ItemsListViewModel>(), Times.Once);
    }

    [Fact]
    public async Task SaveCommand_WhenCreatingNewItem_SendsCreateCommand()
    {
        // Arrange
        _viewModel.Name = "Test Item";
        _viewModel.Sku = "ABC123";
        _viewModel.Type = "TestType";
        _viewModel.MinimumQuantity = 10;
        _viewModel.MaximumQuantity = 100;
        _viewModel.ReorderPoint = 20;

        // Act
        await _viewModel.SaveCommand.ExecuteAsync(null);

        // Assert
        _mediatorMock.Verify(x => x.Send(
            It.Is<CreateItemCommand>(cmd =>
                cmd.Name == "Test Item" &&
                cmd.SKU == "ABC123" &&
                cmd.Type == "TestType" &&
                cmd.MinimumQuantity == 10 &&
                cmd.MaximumQuantity == 100 &&
                cmd.ReorderPoint == 20),
            It.IsAny<CancellationToken>()),
            Times.Once);
        _dialogServiceMock.Verify(x => x.ShowInformationAsync("Success", "Item created successfully."), Times.Once);
        _navigationServiceMock.Verify(x => x.NavigateTo<ItemsListViewModel>(), Times.Once);
    }

    [Fact]
    public async Task SaveCommand_WhenUpdatingExistingItem_SendsUpdateCommand()
    {
        // Arrange
        var existingItem = new GetItemResponse
        {
            Id = Guid.NewGuid(),
            Name = "Existing Item",
            Type = ItemType.RawMaterial,
            IsPerishable = true,
            SKU = "ABC123",
            StockLevel = StockLevel.Create(5, 1, 10, 2)
        };
        _viewModel.InitializeForEdit(existingItem);
        _viewModel.Name = "Updated Name";

        // Act
        await _viewModel.SaveCommand.ExecuteAsync(null);

        // Assert
        _mediatorMock.Verify(x => x.Send(
            It.Is<UpdateItemCommand>(cmd =>
                cmd.Id == existingItem.Id &&
                cmd.Name == "Updated Name" &&
                cmd.Type == existingItem.Type.ToString() &&
                cmd.IsPerishable == true),
            It.IsAny<CancellationToken>()),
            Times.Once);
        _dialogServiceMock.Verify(x => x.ShowInformationAsync("Success", "Item updated successfully."), Times.Once);
    }

    [Fact]
    public async Task SaveCommand_WhenValidationErrors_ShowsErrorDialog()
    {
        // Arrange
        _viewModel.Name = ""; // This will cause validation error

        // Act
        await _viewModel.SaveCommand.ExecuteAsync(null);

        // Assert
        _dialogServiceMock.Verify(x => x.ShowErrorAsync("Validation Error", It.IsAny<string>()), Times.Once);
        _mediatorMock.Verify(x => x.Send(It.IsAny<IRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task SaveCommand_WhenExceptionOccurs_ShowsErrorDialog()
    {
        // Arrange
        _viewModel.Name = "Test Item";
        _mediatorMock.Setup(x => x.Send(It.IsAny<CreateItemCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Test error"));

        // Act
        await _viewModel.SaveCommand.ExecuteAsync(null);

        // Assert
        _dialogServiceMock.Verify(x => x.ShowErrorAsync("Error", "Failed to save item: Test error"), Times.Once);
    }

    [Fact]
    public void InitializeForEdit_SetsAllProperties()
    {
        // Arrange
        var item = new GetItemResponse
        {
            Id = Guid.NewGuid(),
            Name = "Test Item",
            Type = ItemType.RawMaterial,
            IsPerishable = true,
            SKU = "ABC123",
            StockLevel = StockLevel.Create(5, 1, 10, 2)
        }; 

        // Act
        _viewModel.InitializeForEdit(item);

        // Assert
        _viewModel.IsEditing.Should().BeTrue();
        _viewModel.Name.Should().Be("Test Item");
        _viewModel.Type.Should().Be("TestType");
        _viewModel.IsPerishable.Should().BeTrue();
        _viewModel.Sku.Should().Be("ABC123");
        _viewModel.Quantity.Should().Be(5);
        _viewModel.MinimumQuantity.Should().Be(1);
        _viewModel.MaximumQuantity.Should().Be(10);
        _viewModel.ReorderPoint.Should().Be(2);
    }

    [Theory]
    [InlineData(-1, false)]
    [InlineData(0, true)]
    [InlineData(100, true)]
    public void MinimumQuantity_Validation(int value, bool isValid)
    {
        // Act
        _viewModel.MinimumQuantity = value;

        // Assert
        var hasError = _viewModel.Errors?.Any(e => e.MemberNames.Contains(nameof(ItemViewModel.MinimumQuantity))) ?? false;
        hasError.Should().Be(!isValid);
    }

    [Theory]
    [InlineData(-1, false)]
    [InlineData(0, true)]
    [InlineData(100, true)]
    public void MaximumQuantity_Validation(int value, bool isValid)
    {
        // Act
        _viewModel.MaximumQuantity = value;

        // Assert
        var hasError = _viewModel.Errors?.Any(e => e.MemberNames.Contains(nameof(ItemViewModel.MaximumQuantity))) ?? false;
        hasError.Should().Be(!isValid);
    }

    [Theory]
    [InlineData(-1, false)]
    [InlineData(0, true)]
    [InlineData(100, true)]
    public void ReorderPoint_Validation(int value, bool isValid)
    {
        // Act
        _viewModel.ReorderPoint = value;

        // Assert
        var hasError = _viewModel.Errors?.Any(e => e.MemberNames.Contains(nameof(ItemViewModel.ReorderPoint))) ?? false;
        hasError.Should().Be(!isValid);
    }

    [Fact]
    public void Type_WhenEmpty_HasValidationError()
    {
        // Act
        _viewModel.Type = string.Empty;

        // Assert
        var hasError = _viewModel.Errors?.Any(e => e.MemberNames.Contains(nameof(ItemViewModel.Type))) ?? false;
        hasError.Should().BeTrue();
    }

    [Fact]
    public async Task SaveCommand_SetsBusyStateCorrectly()
    {
        // Arrange
        _viewModel.Name = "Test Item";
        _viewModel.Type = "TestType";
        _viewModel.Sku = "ABC123";

        // Act
        var saveTask = _viewModel.SaveCommand.ExecuteAsync(null);
        
        // Assert initial busy state
        _viewModel.IsBusy.Should().BeTrue();
        
        await saveTask;
        
        // Assert final busy state
        _viewModel.IsBusy.Should().BeFalse();
    }
}
