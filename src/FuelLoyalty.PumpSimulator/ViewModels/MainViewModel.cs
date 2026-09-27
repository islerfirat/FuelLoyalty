using System.Collections.ObjectModel;
using System.Net.Http;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FuelLoyalty.Contracts;
using FuelLoyalty.PumpSimulator.Models;
using FuelLoyalty.PumpSimulator.Services;

namespace FuelLoyalty.PumpSimulator.ViewModels
{
    /// <summary>
    /// Pompa ekranının durumu ve komutları. Ekranı (XAML) tanımaz;
    /// ekran bu sınıfın özelliklerine ve komutlarına binding ile bağlanır.
    /// </summary>
    public sealed partial class MainViewModel : ObservableObject
    {
        private static readonly TimeSpan TickInterval = TimeSpan.FromMilliseconds(100);
        private const decimal LitersPerTick = 0.25m;   // Saniyede 2,5 litre

        private readonly IGatewayClient _gatewayClient;

        // Devam eden işlemin bilgileri
        private AuthorizeResponse? _authorization;
        private FuelingSession? _session;
        private string _activeCardNumber = string.Empty;
        private FuelType _activeFuelType;
        private CancellationTokenSource? _fuelingCts;

        public MainViewModel(IGatewayClient gatewayClient, SimulatorSettings settings)
        {
            _gatewayClient = gatewayClient;
            Settings = settings;

            SelectedFuelType = FuelType.Benzin;   // Birim fiyat otomatik dolar
            StatusMessage = DescribeState(State);
            AddLog("Simülatör hazır. Önce backend'in (docker compose) çalıştığından emin olun.");
        }

        public SimulatorSettings Settings { get; }

        public IReadOnlyList<FuelType> FuelTypes { get; } = Enum.GetValues<FuelType>();

        public IReadOnlyList<string> TestCards { get; } =
        [
            "1000200030004000",   // Benzin, TL limitli
        "1000200030004001",   // Motorin, litre limitli
        "1000200030004002",   // Pasif
        "1000200030004003"    // LPG, bakiye 0
        ];

        public ObservableCollection<string> Logs { get; } = [];

        // ================= Ekrana bağlanan özellikler =================

        [ObservableProperty]
        private string _cardNumber = "1000200030004000";

        [ObservableProperty]
        private FuelType _selectedFuelType;

        [ObservableProperty]
        private decimal _unitPrice;

        [ObservableProperty]
        private decimal _liters;

        [ObservableProperty]
        private decimal _amount;

        [ObservableProperty]
        private string _limitText = "-";

        [ObservableProperty]
        private string _statusMessage = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsIdle))]
        [NotifyCanExecuteChangedFor(nameof(ReadCardCommand), nameof(StartFuelingCommand),
                                    nameof(StopFuelingCommand), nameof(SendSaleCommand))]
        private PumpState _state = PumpState.Idle;

        /// <summary>Kart, yakıt ve ayarlar sadece boşta iken değiştirilebilir.</summary>
        public bool IsIdle => State == PumpState.Idle;

        partial void OnSelectedFuelTypeChanged(FuelType value) => UnitPrice = DefaultFuelPrices.For(value);

        partial void OnStateChanged(PumpState value) => StatusMessage = DescribeState(value);

        // ================= 1) KART OKUT =================

        private bool CanReadCard() => State == PumpState.Idle;

        [RelayCommand(CanExecute = nameof(CanReadCard))]
        private async Task ReadCardAsync()
        {
            if (string.IsNullOrWhiteSpace(CardNumber))
            {
                AddLog("Kart numarası girin.");
                return;
            }

            if (UnitPrice <= 0)
            {
                AddLog("Geçerli bir birim fiyat girin.");
                return;
            }

            _activeCardNumber = CardNumber.Trim();
            _activeFuelType = SelectedFuelType;

            StatusMessage = "Onay bekleniyor...";
            AddLog($"Kart okutuldu: {_activeCardNumber} ({_activeFuelType})");

            try
            {
                var request = new AuthorizeRequest(_activeCardNumber, _activeFuelType, Settings.StationCode.Trim(), Settings.PumpNo);
                var response = await _gatewayClient.AuthorizeAsync(request, CancellationToken.None);

                if (!response.Approved)
                {
                    AddLog($"RET: {response.RejectReason}");
                    ResetToIdle();
                    StatusMessage = $"Onay reddedildi: {response.RejectReason}";
                    return;
                }

                _authorization = response;
                _session = new FuelingSession(response.LimitType!.Value, response.MaxValue!.Value, UnitPrice);

                UpdateDisplay();
                LimitText = FormatLimit(_session.LimitType, _session.MaxValue);
                AddLog($"ONAY: Provizyon {response.AuthorizationId}, en fazla {LimitText}");

                State = PumpState.Authorized;
            }
            catch (Exception ex)
            {
                AddLog($"HATA: {DescribeError(ex)}");
                ResetToIdle();
            }
        }

        // ================= 2) YAKIT VER =================

        private bool CanStartFueling() => State == PumpState.Authorized;

        [RelayCommand(CanExecute = nameof(CanStartFueling))]
        private async Task StartFuelingAsync()
        {
            if (_session is null)
                return;

            State = PumpState.Fueling;
            AddLog("Yakıt verme başladı.");

            _fuelingCts = new CancellationTokenSource();
            using var timer = new PeriodicTimer(TickInterval);

            try
            {
                while (await timer.WaitForNextTickAsync(_fuelingCts.Token))
                {
                    var limitReached = _session.Advance(LitersPerTick);
                    UpdateDisplay();

                    if (limitReached)
                    {
                        FinishFueling("Limit doldu, pompa otomatik durdu.");
                        return;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Sıfırla ile iptal edildiyse durum zaten Idle'dır; sadece tabanca asıldıysa bitir.
                if (State == PumpState.Fueling)
                    FinishFueling("Tabanca asıldı.");
            }
            finally
            {
                _fuelingCts.Dispose();
                _fuelingCts = null;
            }
        }

        // ================= 3) TABANCAYI AS =================

        private bool CanStopFueling() => State == PumpState.Fueling;

        [RelayCommand(CanExecute = nameof(CanStopFueling))]
        private void StopFueling() => _fuelingCts?.Cancel();

        // ================= 4) SATIŞI GÖNDER =================

        private bool CanSendSale() => State == PumpState.Finished;

        [RelayCommand(CanExecute = nameof(CanSendSale))]
        private async Task SendSaleAsync()
        {
            if (_authorization?.AuthorizationId is not Guid authorizationId || _session is null)
                return;

            StatusMessage = "Satış gönderiliyor...";

            try
            {
                var request = new CreateSaleRequest(
                    authorizationId,
                    Settings.StationCode.Trim(),
                    _activeCardNumber,
                    _activeFuelType,
                    _session.Liters,
                    _session.Amount);

                var response = await _gatewayClient.SendSaleAsync(request, CancellationToken.None);

                AddLog($"Satış gönderildi. Satış No: {response.SaleId}");
                AddLog("Bakiye düşümü arka planda yapılacak (RabbitMQ). 'Kart Durumu' ile kontrol edebilirsiniz.");
                ResetToIdle(keepDisplay: true);
            }
            catch (Exception ex)
            {
                // Yakıt verildi, satış kaybolmamalı: bilgiler hafızada, tekrar gönderilebilir.
                AddLog($"HATA: Satış gönderilemedi. {DescribeError(ex)} Tekrar deneyebilirsiniz.");
                StatusMessage = "Satış gönderilemedi, tekrar deneyin";
            }
        }

        // ================= KART DURUMU =================

        [RelayCommand]
        private async Task GetCardStatusAsync()
        {
            var cardNumber = CardNumber.Trim();
            if (string.IsNullOrWhiteSpace(cardNumber))
                return;

            try
            {
                var card = await _gatewayClient.GetCardAsync(cardNumber, CancellationToken.None);
                if (card is null)
                {
                    AddLog($"Kart bulunamadı: {cardNumber}");
                    return;
                }

                var unit = card.LimitType == LimitType.Liters ? "L" : "TL";
                AddLog($"KART {card.CardNumber} ({card.HolderName}, {card.AllowedFuelType}, {(card.IsActive ? "Aktif" : "Pasif")}) | " +
                       $"Bakiye: {card.Balance:N2} {unit} | Bloke: {card.ReservedBalance:N2} {unit} | Kullanılabilir: {card.AvailableBalance:N2} {unit}");
            }
            catch (Exception ex)
            {
                AddLog($"HATA: Kart durumu alınamadı. {DescribeError(ex)}");
            }
        }

        // ================= SIFIRLA =================

        [RelayCommand]
        private void Reset()
        {
            var hadUnsentSale = State == PumpState.Finished;

            _fuelingCts?.Cancel();
            ResetToIdle();

            if (hadUnsentSale)
                AddLog("UYARI: Gönderilmemiş satış iptal edildi.");

            AddLog("Pompa sıfırlandı.");
        }

        // ================= YARDIMCI METOTLAR =================

        private void FinishFueling(string reason)
        {
            if (_session is null)
                return;

            AddLog($"{reason} Verilen: {_session.Liters:N2} L / {_session.Amount:N2} TL");

            if (_session.Liters <= 0)
            {
                AddLog("Yakıt verilmedi. Bloke, provizyon süresi dolunca kalkacak.");
                ResetToIdle();
                return;
            }

            State = PumpState.Finished;
        }

        private void ResetToIdle(bool keepDisplay = false)
        {
            _authorization = null;
            _session = null;
            LimitText = "-";

            if (!keepDisplay)
            {
                Liters = 0;
                Amount = 0;
            }

            State = PumpState.Idle;
            StatusMessage = DescribeState(PumpState.Idle);   // Durum zaten Idle ise OnStateChanged tetiklenmez
        }

        private void UpdateDisplay()
        {
            if (_session is null)
                return;

            Liters = _session.Liters;
            Amount = _session.Amount;
        }

        private void AddLog(string message) => Logs.Insert(0, $"{DateTime.Now:HH:mm:ss}  {message}");

        private static string FormatLimit(LimitType limitType, decimal maxValue)
            => limitType == LimitType.Liters ? $"{maxValue:N2} L" : $"{maxValue:N2} TL";

        private static string DescribeState(PumpState state) => state switch
        {
            PumpState.Idle => "Kart bekleniyor",
            PumpState.Authorized => "Onay alındı, yakıt verilebilir",
            PumpState.Fueling => "Yakıt veriliyor...",
            PumpState.Finished => "Yakıt verme bitti, satışı gönderin",
            _ => string.Empty
        };

        private static string DescribeError(Exception ex) => ex switch
        {
            GatewayException gatewayException => gatewayException.Message,
            HttpRequestException => "Gateway'e ulaşılamadı.",
            TaskCanceledException => "Gateway zaman aşımına uğradı.",
            UriFormatException => "Gateway adresi geçersiz.",
            _ => ex.Message
        };
    }
}
