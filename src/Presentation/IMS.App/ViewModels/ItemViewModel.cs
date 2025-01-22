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
using IMSresentation.ViewModels;

namespace IMS.Presentation.ViewModels
{
     
    public partial class ItemViewModel : ViewModelBase,  IParameterizedViewModel
    {
    
        private readonly IMediator _mediator;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
    

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
        [Required(ErrorMessage = "Type is required")]
        private string _type = string.Empty;

        [ObservableProperty]
        [Required(ErrorMessage = "SKU is required")]
        [RegularExpression(@"^[A-Z0-9]{6,10}$", ErrorMessage = "SKU must be 6-10 characters, uppercase letters and numbers only")]
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

        // Observable collection for validation errors
        [ObservableProperty]
        private Dictionary<string, List<string>> _validationErrors = new();

        public ItemViewModel(
            IMediator mediator,
            IMS.Services.Interfaces.IDialogService dialogService,
            INavigationService navigationService)
        {
            _mediator = mediator;
            _dialogService = dialogService;
            _navigationService = navigationService;
        }

        [RelayCommand(CanExecute = nameof(CanSaveItem))]
        private async Task SaveAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                if (!ValidateItem())
                {
                    return;
                }

                if (_itemId != Guid.Empty)
                {
                    var command = new  UpdateItemCommand
                    {
                        Id = _itemId,
                        Name = Name, 
                        Type = Type,
                        IsPerishable = IsPerishable
                    };

                    await _mediator.Send(command);
                    await _dialogService.ShowInformationAsync("Success", "Item updated successfully.");
                }
                else
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

        private bool CanSaveItem()
        {
            return !IsBusy && !string.IsNullOrWhiteSpace(Name) && 
                   !string.IsNullOrWhiteSpace(Type) && !string.IsNullOrWhiteSpace(Sku);
        }

        [RelayCommand]
        private void CancelEdit()
        {
            _navigationService.NavigateTo<ItemsListViewModel>();
        }

      

        private bool ValidateItem()
        {
            ValidationErrors.Clear();

            // Existing quantity validations
            if (MinimumQuantity > MaximumQuantity)
            {
                ValidationErrors.Add(nameof(MinimumQuantity),
                    new List<string> { "Minimum quantity cannot be greater than maximum quantity" });
                return false;
            }

            // Add validation context check
            var validationContext = new ValidationContext(this);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(this, validationContext, validationResults, true);

            if (!isValid)
            {
                foreach (var validationResult in validationResults)
                {
                    foreach (var memberName in validationResult.MemberNames)
                    {
                        if (!ValidationErrors.ContainsKey(memberName))
                            ValidationErrors[memberName] = new List<string>();
                        ValidationErrors[memberName].Add(validationResult.ErrorMessage);
                    }
                }
            }

            return isValid;

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

        partial void OnNameChanged(string value)
        {
            ValidateItem();
        }

        partial void OnTypeChanged(string value)
        {
            ValidateItem();
        }

        partial void OnSkuChanged(string value)
        {
            ValidateItem();
        }

        partial void OnMinimumQuantityChanged(int value)
        {
            ValidateItem();
        }

        partial void OnMaximumQuantityChanged(int value)
        {
            ValidateItem();
        }

        partial void OnReorderPointChanged(int value)
        {
            ValidateItem();
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
}
