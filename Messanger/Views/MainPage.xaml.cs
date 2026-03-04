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

        private void OnChatsClicked(object sender, EventArgs e)
        {
            // Freunde-Seite ist Index 2
            if (Shell.Current.Items.Count > 2)
                Shell.Current.CurrentItem = Shell.Current.Items[2];
        }

        private void OnFriendsClicked(object sender, EventArgs e)
        {
            if (Shell.Current.Items.Count > 2)
                Shell.Current.CurrentItem = Shell.Current.Items[2];
        }

        private void OnProfilClicked(object sender, EventArgs e)
        {
            // Profil ist Index 3
            if (Shell.Current.Items.Count > 3)
                Shell.Current.CurrentItem = Shell.Current.Items[3];
        }
    }
}
