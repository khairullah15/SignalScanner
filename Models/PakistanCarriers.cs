namespace SignalScanner.Models;

public static class PakistanCarriers
{
    // MCC for Pakistan = 410
    // Key = MNC string
    private static readonly Dictionary<string, CarrierInfo> Carriers = new()
    {
        { "01", new CarrierInfo("Jazz",    "Jazz/Mobilink", "#ED1C24") },
        { "03", new CarrierInfo("Ufone",   "Ufone / PTCL",  "#006B3F") },
        { "06", new CarrierInfo("Telenor", "Telenor PK",    "#E8000D") },
        { "07", new CarrierInfo("Warid",   "Warid / Jazz",  "#FF6A00") },
        { "08", new CarrierInfo("Zong",    "Zong / CMPAK",  "#D4007A") },
        { "04", new CarrierInfo("Zong",    "Zong / CMPAK",  "#D4007A") },
    };

    public static CarrierInfo Resolve(string mcc, string mnc)
    {
        // Normalise MNC to 2 digits
        var key = mnc.PadLeft(2, '0');
        if (mcc == "410" && Carriers.TryGetValue(key, out var info))
            return info;

        return new CarrierInfo("Unknown", $"MCC:{mcc} MNC:{mnc}", "#888888");
    }

    public static string ColorForCarrier(string name) => name switch
    {
        "Jazz"    => "#ED1C24",
        "Ufone"   => "#006B3F",
        "Telenor" => "#E8000D",
        "Zong"    => "#D4007A",
        "Warid"   => "#FF6A00",
        _         => "#888888"
    };
}

public record CarrierInfo(string ShortName, string FullName, string Color);
