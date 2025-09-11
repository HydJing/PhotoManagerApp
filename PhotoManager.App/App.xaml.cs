using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Windows;
using PhotoManager.App.ViewModels; // Ensure this is present

namespace PhotoManager.App
{
    public partial class App : Application
    {
        public static IHost? AppHost { get; private set; }

        public App()
        {
            AppHost = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.SetBasePath(AppContext.BaseDirectory);
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<PhotoGalleryViewModel>();
                    services.AddSingleton<SidebarViewModel>();
                    services.AddSingleton<ToolbarViewModel>();
                    services.AddSingleton<StatusBarViewModel>();

                    // Register MainWindowViewModel after children (order not strictly required but clear)
                    services.AddSingleton<MainWindowViewModel>();

                    // Register MainWindow itself so DI constructs it with injected MainWindowViewModel
                    services.AddSingleton<MainWindow>();
                })
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await AppHost!.StartAsync();

            // Let DI create MainWindow (it will inject MainWindowViewModel)
            var mainWindow = AppHost.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await AppHost!.StopAsync();
            AppHost!.Dispose();
            base.OnExit(e);
        }
    }
}