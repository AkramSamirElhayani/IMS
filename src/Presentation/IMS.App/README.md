# IMS.App (Presentation Layer)

## Overview
The Presentation layer implements the user interface using WPF with the MVVM pattern, providing a rich and responsive user experience.

## Purpose
- Implement user interface
- Handle user interactions
- Implement MVVM pattern
- Provide data visualization

## Key Components

### ViewModels
- `ItemViewModel`: Manages item data and operations
- `MainViewModel`: Main application coordination
- `StockLevelViewModel`: Stock management

### Views
- `ItemView`: Item management interface
- `MainWindow`: Main application window
- `StockLevelView`: Stock level monitoring

### Services
- `DialogService`: User interaction
- `NavigationService`: View navigation
- `ValidationService`: UI validation

## MVVM Implementation

1. **ViewModels with CommunityToolkit.Mvvm**
```csharp
public partial class ItemViewModel : ViewModelBase
{
    [ObservableProperty]
    private string name;

    [RelayCommand]
    private async Task SaveAsync()
    {
        // Implementation
    }
}
```

2. **View Bindings**
```xaml
<TextBox Text="{Binding Name, UpdateSourceTrigger=PropertyChanged}"
         Validation.ErrorTemplate="{StaticResource ErrorTemplate}"/>
```

## Features

### 1. Validation
- Real-time input validation
- Error templates
- Validation summary

### 2. Commands
- Async command support
- Command enabling/disabling
- Progress indication

### 3. Navigation
- View injection
- Navigation history
- Deep linking support

## UI/UX Design

1. **Styles and Themes**
```xaml
<Style TargetType="Button" x:Key="PrimaryButton">
    <Setter Property="Background" Value="{StaticResource PrimaryColor}"/>
    <Setter Property="Padding" Value="20,10"/>
</Style>
```

2. **Data Templates**
```xaml
<DataTemplate DataType="{x:Type vm:ItemViewModel}">
    <views:ItemView/>
</DataTemplate>
```

## Dependencies
- CommunityToolkit.Mvvm
- Microsoft.Xaml.Behaviors
- MaterialDesignThemes

## Best Practices

1. **MVVM Pattern**
   - Use commands for actions
   - Implement INotifyPropertyChanged
   - Avoid code-behind

2. **Performance**
   - Virtualization for large lists
   - Async operations
   - Resource management

3. **User Experience**
   - Consistent error handling
   - Progress indication
   - Responsive design

## Testing Considerations
- MVVM facilitates testing
- UI automation support
- Mocked services

## Example Implementations

### 1. Dialog Service
```csharp
public class DialogService : IDialogService
{
    public async Task<bool> ShowConfirmationAsync(string title, string message)
    {
        // Implementation
    }
}
```

### 2. Navigation
```csharp
public class NavigationService : INavigationService
{
    public void NavigateTo<TViewModel>() where TViewModel : ViewModelBase
    {
        // Implementation
    }
}
```

## Error Handling
- User-friendly error messages
- Logging of UI errors
- Global exception handling

## Localization Support
- Resource files
- Culture-specific formatting
- Right-to-left support
