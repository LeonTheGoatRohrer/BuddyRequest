namespace Messanger.Services
{
    public static class ThemeService
    {
        public enum AppTheme
        {
            Light,
            Dark
        }

        private static AppTheme _currentTheme = AppTheme.Light;

        public static event Action ThemeChanged;

        public static AppTheme CurrentTheme
        {
            get => _currentTheme;
            set
            {
                if (_currentTheme != value)
                {
                    _currentTheme = value;
                    ApplyTheme(value);
                    Preferences.Set("AppTheme", value.ToString());
                    ThemeChanged?.Invoke();
                }
            }
        }

        public static void LoadSavedTheme()
        {
            var savedTheme = Preferences.Get("AppTheme", "Light");
            if (Enum.TryParse<AppTheme>(savedTheme, out var theme))
            {
                _currentTheme = theme;
            }
        }

        public static void ApplyTheme()
        {
            ApplyTheme(_currentTheme);
        }

        private static void ApplyTheme(AppTheme theme)
        {
            if (Application.Current == null) return;

            var resources = Application.Current.Resources;

            if (theme == AppTheme.Dark)
            {
                resources["BackgroundColor"] = Color.FromArgb("#1E1E1E");
                resources["SurfaceColor"] = Color.FromArgb("#2D2D2D");
                resources["TextColor"] = Color.FromArgb("#FFFFFF");
                resources["TextColorSecondary"] = Color.FromArgb("#AAAAAA");
                resources["BorderColor"] = Color.FromArgb("#404040");
                resources["PrimaryColor"] = Color.FromArgb("#0A84FF");
                resources["SecondaryColor"] = Color.FromArgb("#30D158");
                resources["CardColor"] = Color.FromArgb("#2D2D2D");
                resources["ButtonTextColor"] = Color.FromArgb("#FFFFFF");
            }
            else
            {
                resources["BackgroundColor"] = Color.FromArgb("#FFFFFF");
                resources["SurfaceColor"] = Color.FromArgb("#F5F5F5");
                resources["TextColor"] = Color.FromArgb("#000000");
                resources["TextColorSecondary"] = Color.FromArgb("#666666");
                resources["BorderColor"] = Color.FromArgb("#E0E0E0");
                resources["PrimaryColor"] = Color.FromArgb("#007AFF");
                resources["SecondaryColor"] = Color.FromArgb("#34C759");
                resources["CardColor"] = Color.FromArgb("#F5F5F5");
                resources["ButtonTextColor"] = Color.FromArgb("#FFFFFF");
            }
        }
    }
}
