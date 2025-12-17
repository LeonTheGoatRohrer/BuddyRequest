namespace Messanger.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnLoginClicked(
          object sender,
          EventArgs e)
        {
            await DisplayAlertAsync(
              "Login",
              "Login-Seite wird später implementiert.",
              "OK");
        }

        private async void OnRegisterClicked(
          object sender,
          EventArgs e)
        {
            // über AppShell-Route zur Registrierungs-Seite
            await Shell.Current.GoToAsync(nameof(RegistrationPage));
        }
    }
}
