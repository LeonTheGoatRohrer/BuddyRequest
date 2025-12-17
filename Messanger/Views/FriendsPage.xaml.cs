using Messanger.ViewModels;

namespace Messanger.Views
{
    public partial class FriendsPage : ContentPage
    {
        public FriendsPage()
        {
            InitializeComponent();
            BindingContext = new FriendsViewModel();
        }
    }
}
