using System.Threading.Tasks;

namespace IMS.Services.Interfaces
{
    public interface IDialogService
    {
        Task<bool> ShowConfirmationAsync(string title, string message);
        Task ShowErrorAsync(string title, string message);
        Task ShowInformationAsync(string title, string message);
        Task ShowWarningAsync(string title, string message);
        Task<string> ShowInputDialogAsync(string title, string message, string defaultValue = "");
    }
}
