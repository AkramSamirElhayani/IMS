using System;

using IMS.ViewModels.Base;
using IMS.ViewModels.Base;

namespace IMS.Services.Interfaces
{
    public interface INavigationService
    {
        ViewModelBase CurrentView { get; }
        void NavigateTo<T>() where T : ViewModelBase;
        void NavigateToWithParameter<T>(object parameter) where T : ViewModelBase;
        event Action CurrentViewChanged;
    }
}
