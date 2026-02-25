using System.ComponentModel;
using System.Runtime.CompilerServices;
using Messanger.Services;

namespace Messanger.Views
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            BindingContext = new ViewModels.SettingsViewModel();
        }
    }
}

namespace Messanger.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        private string selectedLanguage;
        private string selectedTheme;

        public SettingsViewModel()
        {
            var index = LocalizationService.AvailableLanguageCodes.IndexOf(LocalizationService.CurrentLanguage);
            selectedLanguage = index >= 0 ? LocalizationService.AvailableLanguages[index] : "Deutsch";
            
            selectedTheme = ThemeService.CurrentTheme == ThemeService.AppTheme.Light ? "Light" : "Dark";
        }

        public List<string> Languages => LocalizationService.AvailableLanguages;
        public List<string> Themes => new List<string> { "Light", "Dark" };

        public string SelectedLanguage
        {
            get => selectedLanguage;
            set
            {
                if (selectedLanguage != value && value != null)
                {
                    selectedLanguage = value;
                    OnPropertyChanged();

                    var index = Languages.IndexOf(value);
                    if (index >= 0)
                    {
                        LocalizationService.CurrentLanguage = LocalizationService.AvailableLanguageCodes[index];
                    }
                }
            }
        }

        public string SelectedTheme
        {
            get => selectedTheme;
            set
            {
                if (selectedTheme != value && value != null)
                {
                    selectedTheme = value;
                    OnPropertyChanged();

                    var theme = value == "Light" ? ThemeService.AppTheme.Light : ThemeService.AppTheme.Dark;
                    ThemeService.CurrentTheme = theme;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
