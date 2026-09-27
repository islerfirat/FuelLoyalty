using CommunityToolkit.Mvvm.ComponentModel;

namespace FuelLoyalty.PumpSimulator.Models
{
    /// <summary>
    /// Pompanın bağlantı ayarları. Ekrandan değiştirilebilir;
    /// GatewayClient her istekte güncel değeri buradan okur.
    /// </summary>
    public sealed partial class SimulatorSettings : ObservableObject
    {
        [ObservableProperty]
        private string _gatewayUrl = "http://localhost:5000";

        [ObservableProperty]
        private string _stationCode = "IST-001";

        [ObservableProperty]
        private int _pumpNo = 1;
    }
}
