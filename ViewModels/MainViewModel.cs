using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SignalScanner.Models;
using System.Collections.ObjectModel;

#if ANDROID
using SignalScanner.Services;
#endif

namespace SignalScanner.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<NetworkSignal> _signals = new();
    [ObservableProperty] private bool _isScanning;
    [ObservableProperty] private string _statusText = "Tap Scan to start";
    [ObservableProperty] private string _lastUpdated = "--:--:--";
    [ObservableProperty] private bool _hasPermission;
    [ObservableProperty] private bool _isEmpty = true;

    private CancellationTokenSource? _cts;

#if ANDROID
    private readonly CellSignalService _service = new();
#endif

    [RelayCommand]
    private async Task StartScanAsync()
    {
        if (IsScanning)
        {
            StopScan();
            return;
        }

        HasPermission = await RequestPermissionsAsync();
        if (!HasPermission)
        {
            StatusText = "Location permission required";
            return;
        }

        IsScanning = true;
        StatusText = "Scanning...";
        _cts = new CancellationTokenSource();

        _ = Task.Run(async () =>
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                await RefreshSignalsAsync();
                await Task.Delay(3000, _cts.Token).ContinueWith(_ => { });
            }
        });
    }

    private void StopScan()
    {
        _cts?.Cancel();
        IsScanning = false;
        StatusText = "Scan stopped";
    }

    private async Task RefreshSignalsAsync()
    {
        List<NetworkSignal> fresh = new();

#if ANDROID
        try
        {
            fresh = _service.GetAllCellSignals();
        }
        catch (Exception ex)
        {
            MainThread.BeginInvokeOnMainThread(() =>
                StatusText = $"Error: {ex.Message}");
            return;
        }
#else
        // Desktop/simulator demo data
        fresh = GetDemoSignals();
        await Task.Delay(500);
#endif

        MainThread.BeginInvokeOnMainThread(() =>
        {
            Signals.Clear();
            foreach (var s in fresh)
                Signals.Add(s);

            IsEmpty = Signals.Count == 0;
            LastUpdated = DateTime.Now.ToString("HH:mm:ss");
            StatusText = Signals.Count == 0
                ? "No cell towers found nearby"
                : $"Found {Signals.Count} network{(Signals.Count == 1 ? "" : "s")}";
        });
    }

    private static async Task<bool> RequestPermissionsAsync()
    {
        var coarse = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        return coarse == PermissionStatus.Granted;
    }

    // Demo data for Windows/simulator testing
    private static List<NetworkSignal> GetDemoSignals() =>
    [
        new() { Operator="Jazz",    CarrierName="Jazz / Mobilink", NetworkType="4G LTE",  SignalDbm=-72,  SignalPercent=82, SignalLevel=4, Mcc="410", Mnc="01", IsRegistered=true  },
        new() { Operator="Zong",    CarrierName="Zong / CMPAK",    NetworkType="4G LTE",  SignalDbm=-85,  SignalPercent=64, SignalLevel=3, Mcc="410", Mnc="08", IsRegistered=false },
        new() { Operator="Telenor", CarrierName="Telenor PK",      NetworkType="4G LTE",  SignalDbm=-97,  SignalPercent=41, SignalLevel=2, Mcc="410", Mnc="06", IsRegistered=false },
        new() { Operator="Ufone",   CarrierName="Ufone / PTCL",    NetworkType="3G WCDMA",SignalDbm=-102, SignalPercent=28, SignalLevel=1, Mcc="410", Mnc="03", IsRegistered=false },
        new() { Operator="Zong",    CarrierName="Zong / CMPAK",    NetworkType="2G GSM",  SignalDbm=-108, SignalPercent=15, SignalLevel=1, Mcc="410", Mnc="08", IsRegistered=false },
    ];
}
