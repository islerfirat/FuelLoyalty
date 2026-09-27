using FuelLoyalty.PumpSimulator.Models;
using FuelLoyalty.PumpSimulator.Services;
using FuelLoyalty.PumpSimulator.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Net.Http;
using System.Windows;
using System.Windows.Markup;

namespace FuelLoyalty.PumpSimulator
{
    public partial class App : Application
    {
        private ServiceProvider? _services;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // WPF binding'leri varsayılan olarak İngilizce sayı biçimi kullanır (47.50).
            // Bilgisayarın dil ayarını (Türkçe: 47,50) kullanması için:
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            _services = ConfigureServices();

            var mainWindow = _services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _services?.Dispose();
            base.OnExit(e);
        }

        private static ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddSingleton<SimulatorSettings>();
            services.AddSingleton(new HttpClient { Timeout = TimeSpan.FromSeconds(10) });
            services.AddSingleton<IGatewayClient, GatewayClient>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<MainWindow>();

            return services.BuildServiceProvider();
        }
    }

}
