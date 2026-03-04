using Messanger.ViewModels;
using System;

namespace Messanger.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainPageViewModel();
        }

        private async void OnRequestsClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("///RequestsPage");
        }

        private async void OnFriendsClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("///FriendsPage");
        }

        private async void OnProfilClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("///ProfilPage");
        }
    }
}
