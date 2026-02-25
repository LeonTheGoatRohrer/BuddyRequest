using Messanger.ViewModels;
using Models;

namespace Messanger.Views
{
    [QueryProperty(nameof(Friend), "Friend")]
    public partial class ChatPage : ContentPage
    {
        private readonly ChatViewModel viewModel;
        private User friend;

        public User Friend
        {
            get => friend;
            set
            {
                friend = value;
                if (friend != null && viewModel != null)
                {
                    viewModel.Initialize(friend);
                }
            }
        }

        public ChatPage()
        {
            InitializeComponent();
            viewModel = new ChatViewModel();
            BindingContext = viewModel;
        }
    }
}
