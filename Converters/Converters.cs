using System.Globalization;

namespace SignalScanner.Converters;

/// <summary>Maps 0-100 signal percent to a bar width as GridLength proportion.</summary>
public class PercentToWidthConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int pct)
            return new GridLength(Math.Max(1, pct), GridUnitType.Star);
        return new GridLength(1, GridUnitType.Star);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

/// <summary>Signal percent → color (green / yellow / orange / red).</summary>
public class SignalToColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        int pct = value is int p ? p : 0;
        return pct switch
        {
            >= 75 => Color.FromArgb("#22C55E"),   // green
            >= 50 => Color.FromArgb("#3B82F6"),   // blue
            >= 30 => Color.FromArgb("#F59E0B"),   // amber
            _      => Color.FromArgb("#EF4444"),  // red
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

/// <summary>Carrier name → brand color.</summary>
public class CarrierToColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (value?.ToString() ?? "") switch
        {
            "Jazz"    => Color.FromArgb("#ED1C24"),
            "Zong"    => Color.FromArgb("#D4007A"),
            "Telenor" => Color.FromArgb("#E8000D"),
            "Ufone"   => Color.FromArgb("#006B3F"),
            "Warid"   => Color.FromArgb("#FF6A00"),
            _         => Color.FromArgb("#6B7280"),
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

/// <summary>IsRegistered → star symbol visibility.</summary>
public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is true;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

/// <summary>NetworkType string → badge background color.</summary>
public class NetworkTypeToBgConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (value?.ToString() ?? "") switch
        {
            "4G LTE"   => Color.FromArgb("#DCFCE7"),
            "5G NR"    => Color.FromArgb("#EDE9FE"),
            "3G WCDMA" => Color.FromArgb("#DBEAFE"),
            "2G GSM"   => Color.FromArgb("#FEF3C7"),
            _           => Color.FromArgb("#F3F4F6"),
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

/// <summary>NetworkType string → badge text color.</summary>
public class NetworkTypeToFgConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (value?.ToString() ?? "") switch
        {
            "4G LTE"   => Color.FromArgb("#166534"),
            "5G NR"    => Color.FromArgb("#4C1D95"),
            "3G WCDMA" => Color.FromArgb("#1E40AF"),
            "2G GSM"   => Color.FromArgb("#92400E"),
            _           => Color.FromArgb("#374151"),
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
