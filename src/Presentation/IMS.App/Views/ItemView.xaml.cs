using System.Windows;
using IMS.Presentation.ViewModels;

namespace IMS.Presentation.Views
{
    public partial class ItemView : Window
    {
        public ItemView(ItemViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
