using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using IMS.ViewModels; 
using IMS.Infrastructure;
using IMS.Application;
using IMS.Presentation.ViewModels;

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

                    // Register ViewModels
                    services.AddTransient<MainViewModel>();
                    services.AddTransient<ItemViewModel>(); 


                  
                })
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
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
