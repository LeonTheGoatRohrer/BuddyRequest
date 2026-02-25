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

        private async void OnChatsClicked(object sender, EventArgs e)
        {
            // ChatPage braucht einen Freund als Parameter, daher zu FriendsPage navigieren
            await Shell.Current.GoToAsync("///FriendsPage");
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
