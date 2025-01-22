using System;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IMS.ViewModels.Base;
using IMS.Application.Features.Items.Common.Responses;
using IMS.Services.Interfaces;
using MediatR;
using System.ComponentModel.DataAnnotations;
using IMS.Domain.ValueObjects;
using IMS.Application.Features.Items.Commands;
using IMS.Presentation.Validation;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace IMS.Presentation.ViewModels;

 
public partial class ItemViewModel : ViewModelBase,  IParameterizedViewModel
{

    private readonly IMediator _mediator;
    private readonly IDialogService _dialogService;
    private readonly INavigationService _navigationService;
    private readonly IModelValidator<ItemViewModel> _validator;

 

    [ObservableProperty]
    [Required(ErrorMessage = "Name is required")]
    [MinLength(3, ErrorMessage = "Name must be at least 3 characters")]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string _name = string.Empty;

    [ObservableProperty]
    [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative")]
    private int _quantity;
            
    [ObservableProperty]
    private bool _isPerishable;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    [Required(ErrorMessage = "Type is required")]
    private string _type = string.Empty;

    [ObservableProperty]
    [Required(ErrorMessage = "SKU is required")]
    [RegularExpression(@"^[A-Z0-9]{6,10}$", ErrorMessage = "SKU must be 6-10 characters, uppercase letters and numbers only")]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string _sku = string.Empty;

    [ObservableProperty]
    [Range(0, int.MaxValue, ErrorMessage = "Minimum quantity cannot be negative")]
    private int _minimumQuantity;

    [ObservableProperty]
    [Range(0, int.MaxValue, ErrorMessage = "Maximum quantity cannot be negative")]
    private int _maximumQuantity;

    [ObservableProperty]
    [Range(0, int.MaxValue, ErrorMessage = "Reorder point cannot be negative")]
    private int _reorderPoint;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private bool _isEditing;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    private Guid _itemId;

    [ObservableProperty]
    private ObservableCollection<string> _errors = new();

    [ObservableProperty]
    private Dictionary<string, List<string>> _validationErrors = new();
    public ItemViewModel(
        IMediator mediator,
        IDialogService dialogService,
        INavigationService navigationService,
        IModelValidator<ItemViewModel> validator)
    {
        _mediator = mediator;
        _dialogService = dialogService;
        _navigationService = navigationService;
        _validator = validator;
        PropertyChanged += OnPropertyChanged;

    }
    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(Errors) &&
            e.PropertyName != nameof(IsBusy) &&
            e.PropertyName != nameof(ValidationErrors))
        {
            ValidateProperty(e.PropertyName); 
        }
    }

    [RelayCommand(CanExecute = nameof(CanSaveItem))]
    private async Task SaveAsync()
    {
        if (IsBusy) return;

        try
        {
            ErrorMessage = string.Empty;
          
            if (!ValidateItem())
            {
                await _dialogService.ShowErrorAsync("Validation Error", string.Join("\n", Errors));
                return;
            }

            if (_itemId != Guid.Empty)
            {
                await UpdateExistingItem();
            }
            else
            {
                await CreateNewItem();
            }

            _navigationService.NavigateTo<ItemsListViewModel>();
        }
        catch (Exception ex)
        {
            ErrorMessage = "Error saving item: " + ex.Message;
            await _dialogService.ShowErrorAsync("Error", $"Failed to save item: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }
    private async Task UpdateExistingItem()
    {
        var command = new UpdateItemCommand
        {
            Id = _itemId,
            Name = Name,
            Type = Type,
            IsPerishable = IsPerishable
        };

        await _mediator.Send(command);
        await _dialogService.ShowInformationAsync("Success", "Item updated successfully.");
    }

    private async Task CreateNewItem()
    {
        var command = new CreateItemCommand
        {
            Name = Name,
            SKU = Sku,
            Type = Type,
            IsPerishable = IsPerishable,
            MinimumQuantity = MinimumQuantity,
            MaximumQuantity = MaximumQuantity,
            ReorderPoint = ReorderPoint
        };

        await _mediator.Send(command);
        await _dialogService.ShowInformationAsync("Success", "Item created successfully.");
    }
 

    [RelayCommand]
    private void CancelEdit() => _navigationService.NavigateTo<ItemsListViewModel>();

    private bool CanSaveItem() => !IsBusy && !string.IsNullOrWhiteSpace(Name) && ValidationErrors.Count == 0;

    private void ValidateProperty(string propertyName)
    {
        var result = _validator.Validate(this);
        UpdateValidationErrors(propertyName, result.Errors.GetValueOrDefault(propertyName, new List<string>()));
    }
    private bool ValidateItem()
    {
        var result = _validator.Validate(this);
        ValidationErrors.Clear();
        foreach (var error in result.Errors)
        {
            ValidationErrors[error.Key] = error.Value;
        }
        UpdateErrorsCollection();
        return result.IsValid;
    }

    private void UpdateValidationErrors(string propertyName, List<string> propertyErrors)
    {
        if (!propertyErrors.Any())
            ValidationErrors.Remove(propertyName);
        else
            ValidationErrors[propertyName] = propertyErrors;

        UpdateErrorsCollection();
    }

    private void UpdateErrorsCollection()
    {
        Errors.Clear();
        foreach (var error in ValidationErrors.Values.SelectMany(x => x))
        {
            Errors.Add(error);
        }
    }


    public void InitializeForEdit(GetItemResponse item)
    {
        IsEditing = true;
        _itemId = item.Id;
        Name = item.Name;
        Quantity = item.StockLevel.Current;
        Type = item.Type.ToString();
        IsPerishable = item.IsPerishable;
        Sku = item.SKU;
        MinimumQuantity = (int)item.StockLevel.Minimum;
        MaximumQuantity = (int)item.StockLevel.Maximum;
        ReorderPoint = (int)item.StockLevel.Critical;
    }

    public void InitializeForCreate()
    {
        IsEditing = false;
        MinimumQuantity = 0;
        MaximumQuantity = 100;
        ReorderPoint = 20;
    } 
    public Task Initialize(object parameter)
    {
        if (parameter is GetItemResponse item)
        {
            InitializeForEdit(item);
        }
        else
        {
            InitializeForCreate();
        }
        return Task.CompletedTask;
    }

    
}
