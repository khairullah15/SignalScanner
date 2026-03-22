using SignalScanner.Views;

namespace SignalScanner;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new NavigationPage(new Views.MainPage())
        {
            BarBackgroundColor = Color.FromArgb("#0F172A"),
            BarTextColor       = Color.FromArgb("#F1F5F9")
        };
    }
}
