using Messanger.ViewModels;

namespace Messanger.Views
{
    public partial class RegistrationPage : ContentPage
    {
        public RegistrationPage()
        {
            InitializeComponent();

            BindingContext = new RegistrationViewModel();
        }

        private async void OnBackToLoginClicked(
          object sender,
          EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
