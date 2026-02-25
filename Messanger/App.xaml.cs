using Microsoft.Extensions.DependencyInjection;
using Messanger.Services;

namespace Messanger
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            
            // Lade gespeicherte Theme und Sprache
            ThemeService.LoadSavedTheme();
            LocalizationService.LoadSavedLanguage();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            try
            {
                var window = new Window(new AppShell());

                // Theme anwenden nachdem Window erstellt wurde
                ThemeService.ApplyTheme();

                return window;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"========== APP CRASH ==========");
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.GetType().Name}");
                System.Diagnostics.Debug.WriteLine($"Message: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner: {ex.InnerException.Message}");
                    System.Diagnostics.Debug.WriteLine($"Inner Stack: {ex.InnerException.StackTrace}");
                }
                System.Diagnostics.Debug.WriteLine($"===============================");

                // Zeige eine einfache Fehlerseite
                var errorPage = new ContentPage
                {
                    Content = new VerticalStackLayout
                    {
                        Padding = 20,
                        VerticalOptions = LayoutOptions.Center,
                        Children =
                        {
                            new Label { Text = "App Fehler beim Start", FontSize = 24, HorizontalOptions = LayoutOptions.Center },
                            new Label { Text = ex.Message, TextColor = Colors.Red, FontSize = 14 },
                            new Label { Text = ex.InnerException?.Message ?? "", TextColor = Colors.Orange, FontSize = 12 }
                        }
                    }
                };
                return new Window(errorPage);
            }
        }
    }
}