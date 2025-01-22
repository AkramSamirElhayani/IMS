using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using IMS.Services.Interfaces;
using IMS.ViewModels.Base;
using IMS.ViewModels.Base;

namespace IMS.Services
{
    public class NavigationService : Interfaces.INavigationService
    {
        private readonly IServiceProvider _serviceProvider;
        private ViewModelBase _currentView;
        private readonly Dictionary<Type, object> _parameters;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _parameters = new Dictionary<Type, object>();
        }

        public ViewModelBase CurrentView
        {
            get => _currentView;
            private set
            {
                if (_currentView != value)
                {
                    _currentView = value;
                    CurrentViewChanged?.Invoke();
                }
            }
        }

        public event Action CurrentViewChanged;

        public void NavigateTo<T>() where T : ViewModelBase
        {
            var view = _serviceProvider.GetRequiredService<T>();
            if (_parameters.ContainsKey(typeof(T)))
            {
                (view as ViewModels.Base.IParameterizedViewModel)?.Initialize(_parameters[typeof(T)]);
                _parameters.Remove(typeof(T));
            }
            CurrentView = view;
        }

        public void NavigateToWithParameter<T>(object parameter) where T : ViewModelBase
        {
            _parameters[typeof(T)] = parameter;
            NavigateTo<T>();
        }
    }
}
