namespace SignalScanner.Models;

public class NetworkSignal
{
    public string CarrierName { get; set; } = string.Empty;
    public string Operator { get; set; } = string.Empty;   // Jazz, Zong, Telenor, Ufone
    public string NetworkType { get; set; } = string.Empty; // 4G, 3G, 2G
    public int SignalDbm { get; set; }                      // e.g. -85
    public int SignalLevel { get; set; }                    // 0-4 (Android levels)
    public int SignalPercent { get; set; }                  // 0-100 computed
    public string Mcc { get; set; } = string.Empty;
    public string Mnc { get; set; } = string.Empty;
    public int CellId { get; set; }
    public int Lac { get; set; }
    public bool IsRegistered { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.Now;

    // Visual helpers
    public string SignalIcon => SignalPercent switch
    {
        >= 75 => "📶",
        >= 50 => "▊▊▊",
        >= 25 => "▊▊",
        _      => "▊"
    };

    public string QualityLabel => SignalPercent switch
    {
        >= 80 => "Excellent",
        >= 60 => "Good",
        >= 40 => "Fair",
        >= 20 => "Weak",
        _      => "Very Weak"
    };
}
