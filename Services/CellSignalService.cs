#if ANDROID
using Android.Content;
using Android.OS;
using Android.Telephony;
using SignalScanner.Models;
using Application = Android.App.Application;

namespace SignalScanner.Services;

public class CellSignalService
{
    private readonly TelephonyManager _telephony;

    public CellSignalService()
    {
        _telephony = (TelephonyManager)Application.Context
            .GetSystemService(Context.TelephonyService)!;
    }

    public List<NetworkSignal> GetAllCellSignals()
    {
        var results = new List<NetworkSignal>();

        try
        {
            // GetAllCellInfo() is deprecated in API 29+
            // Use synchronous fallback via the deprecated method wrapped safely
#pragma warning disable CA1416
#pragma warning disable CS0618
            var cellInfos = _telephony.AllCellInfo;
#pragma warning restore CS0618
#pragma warning restore CA1416

            if (cellInfos == null) return results;

            foreach (var cell in cellInfos)
            {
                var signal = ParseCellInfo(cell);
                if (signal != null)
                    results.Add(signal);
            }

            // Remove duplicates, keep strongest per operator+type
            results = results
                .GroupBy(s => $"{s.Operator}_{s.NetworkType}")
                .Select(g => g.OrderByDescending(x => x.SignalDbm).First())
                .OrderByDescending(s => s.SignalPercent)
                .ToList();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"CellSignalService error: {ex.Message}");
        }

        return results;
    }

    private NetworkSignal? ParseCellInfo(CellInfo cell)
    {
        return cell switch
        {
            CellInfoLte lte       => ParseLte(lte),
            CellInfoWcdma wcdma   => ParseWcdma(wcdma),
            CellInfoGsm gsm       => ParseGsm(gsm),
            CellInfoNr nr         => ParseNr(nr),
            _                     => null
        };
    }

    // ── 4G LTE ──────────────────────────────────────────────────────────────
    private NetworkSignal ParseLte(CellInfoLte cell)
    {
        var id = cell.CellIdentity as CellIdentityLte;
        var ss = cell.CellSignalStrength as CellSignalStrengthLte;

        int dbm   = ss?.Dbm ?? -120;
        int level = ss?.Level ?? 0;

#pragma warning disable CA1416
        var mcc = Build.VERSION.SdkInt >= BuildVersionCodes.P ? id?.MccString ?? "" : "";
        var mnc = Build.VERSION.SdkInt >= BuildVersionCodes.P ? id?.MncString ?? "" : "";
#pragma warning restore CA1416

        var carrier = PakistanCarriers.Resolve(mcc, mnc);

        return new NetworkSignal
        {
            CarrierName  = carrier.FullName,
            Operator     = carrier.ShortName,
            NetworkType  = "4G LTE",
            SignalDbm    = dbm,
            SignalLevel  = level,
            SignalPercent= DbmToPercent(dbm, -140, -44),
            Mcc          = mcc,
            Mnc          = mnc,
            CellId       = (int)(id?.Ci ?? 0),
            Lac          = id?.Tac ?? 0,
            IsRegistered = cell.IsRegistered,
            LastUpdated  = DateTime.Now
        };
    }

    // ── 3G WCDMA ────────────────────────────────────────────────────────────
    private NetworkSignal ParseWcdma(CellInfoWcdma cell)
    {
        var id = cell.CellIdentity as CellIdentityWcdma;
        var ss = cell.CellSignalStrength as CellSignalStrengthWcdma;

        int dbm   = ss?.Dbm ?? -120;
        int level = ss?.Level ?? 0;

#pragma warning disable CA1416
        var mcc = Build.VERSION.SdkInt >= BuildVersionCodes.P ? id?.MccString ?? "" : "";
        var mnc = Build.VERSION.SdkInt >= BuildVersionCodes.P ? id?.MncString ?? "" : "";
#pragma warning restore CA1416

        var carrier = PakistanCarriers.Resolve(mcc, mnc);

        return new NetworkSignal
        {
            CarrierName  = carrier.FullName,
            Operator     = carrier.ShortName,
            NetworkType  = "3G WCDMA",
            SignalDbm    = dbm,
            SignalLevel  = level,
            SignalPercent= DbmToPercent(dbm, -120, -25),
            Mcc          = mcc,
            Mnc          = mnc,
            CellId       = id?.Cid ?? 0,
            Lac          = id?.Lac ?? 0,
            IsRegistered = cell.IsRegistered,
            LastUpdated  = DateTime.Now
        };
    }

    // ── 2G GSM ──────────────────────────────────────────────────────────────
    private NetworkSignal ParseGsm(CellInfoGsm cell)
    {
        var id = cell.CellIdentity as CellIdentityGsm;
        var ss = cell.CellSignalStrength as CellSignalStrengthGsm;

        int dbm   = ss?.Dbm ?? -110;
        int level = ss?.Level ?? 0;

#pragma warning disable CA1416
        var mcc = Build.VERSION.SdkInt >= BuildVersionCodes.P ? id?.MccString ?? "" : "";
        var mnc = Build.VERSION.SdkInt >= BuildVersionCodes.P ? id?.MncString ?? "" : "";
#pragma warning restore CA1416

        var carrier = PakistanCarriers.Resolve(mcc, mnc);

        return new NetworkSignal
        {
            CarrierName  = carrier.FullName,
            Operator     = carrier.ShortName,
            NetworkType  = "2G GSM",
            SignalDbm    = dbm,
            SignalLevel  = level,
            SignalPercent= DbmToPercent(dbm, -113, -51),
            Mcc          = mcc,
            Mnc          = mnc,
            CellId       = id?.Cid ?? 0,
            Lac          = id?.Lac ?? 0,
            IsRegistered = cell.IsRegistered,
            LastUpdated  = DateTime.Now
        };
    }

    // ── 5G NR ───────────────────────────────────────────────────────────────
    private NetworkSignal ParseNr(CellInfoNr cell)
    {
#pragma warning disable CA1416
        if (Build.VERSION.SdkInt < BuildVersionCodes.Q)
            return null!;

        var id = cell.CellIdentity as CellIdentityNr;
        var ss = cell.CellSignalStrength as CellSignalStrengthNr;

        int dbm   = ss?.SsRsrp ?? -120;
        int level = ss?.Level ?? 0;
        var mcc   = id?.MccString ?? "";
        var mnc   = id?.MncString ?? "";
#pragma warning restore CA1416

        var carrier = PakistanCarriers.Resolve(mcc, mnc);

        return new NetworkSignal
        {
            CarrierName  = carrier.FullName,
            Operator     = carrier.ShortName,
            NetworkType  = "5G NR",
            SignalDbm    = dbm,
            SignalLevel  = level,
            SignalPercent= DbmToPercent(dbm, -140, -44),
            Mcc          = mcc,
            Mnc          = mnc,
            IsRegistered = cell.IsRegistered,
            LastUpdated  = DateTime.Now
        };
    }

    private static int DbmToPercent(int dbm, int minDbm, int maxDbm)
    {
        dbm = Math.Clamp(dbm, minDbm, maxDbm);
        return (int)Math.Round((double)(dbm - minDbm) / (maxDbm - minDbm) * 100);
    }
}
#endif
