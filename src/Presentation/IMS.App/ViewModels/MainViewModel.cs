using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using IMS.ViewModels.Base; 
using IMS.Presentation.Views;
using Microsoft.Extensions.DependencyInjection;
using IMS.Presentation.ViewModels;

namespace IMS.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        //    private readonly IServiceProvider _serviceProvider;

        //    public MainWindowViewModel(IServiceProvider serviceProvider)
        //    {
        //        _serviceProvider = serviceProvider;
        //    }

        //    [RelayCommand]
        //    private void NavigateToItems()
        //    {
        //        var itemView = _serviceProvider.GetRequiredService<ItemView>();
        //        itemView.Show();
        //    }
        //{ 
        private readonly ItemView _itemView;

        public MainWindowViewModel( ItemView itemView )
        {
            _itemView = itemView;
        }

        [RelayCommand]
        private void NavigateToItems()
        {

            _itemView.Show();
        }


    }
}
