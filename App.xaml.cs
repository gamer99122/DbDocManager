// App.xaml.cs
using System;
using System.IO;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DbDocManager.Models;
using DbDocManager.Services;
using DbDocManager.ViewModels;
using DbDocManager.Views;

namespace DbDocManager
{
    public partial class App : Application
    {
        private ServiceProvider? _serviceProvider;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                // 設置服務容器
                var services = new ServiceCollection();
                ConfigureServices(services);
                _serviceProvider = services.BuildServiceProvider();

                // 手動創建並顯示主視窗
                var viewModel = _serviceProvider.GetRequiredService<MainViewModel>();
                var mainWindow = new MainWindow(viewModel);
                MainWindow = mainWindow;
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"應用程式啟動失敗: {ex.Message}\n\n{ex.StackTrace}", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }

        private void ConfigureServices(ServiceCollection services)
        {
            try
            {
                // 配置
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build();

                // 創建並註冊 AppSettings 實例
                var appSettings = new AppSettings();
                appSettings.ConnectionStrings.DbDoc = configuration.GetConnectionString("DbDoc") ?? "Server=.;Database=DbDoc;Trusted_Connection=True;TrustServerCertificate=True";
                appSettings.Export.OutputDir = configuration.GetSection("Export:OutputDir").Value ?? "docs";
                appSettings.Logging.Path = configuration.GetSection("Logging:Path").Value ?? "logs";

                services.AddSingleton(appSettings);

                // 服務註冊
                services.AddTransient<IDataService, DataService>();
                services.AddTransient<IExportService, ExportService>();

                // ViewModels
                services.AddTransient<MainViewModel>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"配置服務失敗: {ex.Message}", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _serviceProvider?.Dispose();
            base.OnExit(e);
        }
    }
}