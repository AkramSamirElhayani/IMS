using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using IMS.ViewModels; 
using IMS.Infrastructure;
using IMS.Application;
using IMS.Presentation.ViewModels;
using IMS.Presentation.Views;
using IMS.Services.Interfaces;
using IMS.Services;

namespace IMS
{
    public partial class App : System.Windows.Application
    {
        private IHost _host;

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // Register Application and Infrastructure services
                    services.AddApplication();
                    services.AddInfrastructure(context.Configuration);

                    //Register Validation 
                    //services.AddSingleton<IModelValidator<ItemViewModel>, ItemValidator>();

                    // Register Services 
                    services.AddScoped<IDialogService , DialogService>();
                    services.AddScoped<INavigationService , NavigationService>();
                    // Register ViewModels
                    services.AddTransient<MainWindowViewModel>();
                    services.AddTransient<ItemViewModel>(); 

                    // Register Views
                    services.AddScoped<MainWindow>();
                    services.AddTransient<ItemView>();

                  
                })
                .Build();
        }

        protected override async void OnStartup(  StartupEventArgs e)
        {
            await _host.StartAsync();

            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await _host.StopAsync();
            _host.Dispose();

            base.OnExit(e);
        }
    }
}
