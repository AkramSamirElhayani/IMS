 

using System.Windows;  
namespace IMS.Services
{
    public class DialogService : Interfaces.IDialogService
    {
        public async Task<bool> ShowConfirmationAsync(string title, string message)
        {
            return await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
            {
                var result = MessageBox.Show(
                    message,
                    title,
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
                return result == MessageBoxResult.Yes;
            });
        }

        public async Task ShowErrorAsync(string title, string message)
        {
            await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
            {
                MessageBox.Show(
                    message,
                    title,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            });
        }

        public async Task ShowInformationAsync(string title, string message)
        {
            await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
            {
                MessageBox.Show(
                    message,
                    title,
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            });
        }

        public async Task ShowWarningAsync(string title, string message)
        {
            await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
            {
                MessageBox.Show(
                    message,
                    title,
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            });
        }

        public async Task<string> ShowInputDialogAsync(string title, string message, string defaultValue = "")
        {
            // For a proper input dialog, you might want to create a custom WPF window
            // This is a simple implementation using standard dialogs
            return await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
            {
                var dialog = new TextDialog(title, message, defaultValue);
                var result = dialog.ShowDialog();
                return result == true ? dialog.ResponseText : string.Empty;
            });
        }
    }
}
