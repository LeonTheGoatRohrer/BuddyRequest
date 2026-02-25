using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Messanger.Services;
using Microsoft.Maui.Controls;
using Models;

namespace Messanger.ViewModels
{
    public class ChatViewModel : INotifyPropertyChanged
    {
        private User friendUser;
        private string messageText;
        private string chatTitle;
        private bool isBusy;
        private ObservableCollection<Models.Messages> messages;
        private readonly ApiService apiService;

        public event PropertyChangedEventHandler PropertyChanged;

        public ChatViewModel()
        {
            apiService = new ApiService();
            Messages = new ObservableCollection<Models.Messages>();
            SendMessageCommand = new Command(async () => await SendMessageAsync(), () => !string.IsNullOrWhiteSpace(MessageText) && !IsBusy);
            LoadMessagesCommand = new Command(async () => await LoadMessagesAsync());
        }

        public void Initialize(User friend)
        {
            FriendUser = friend;
            ChatTitle = $"Chat mit {friend.Username}";
            Task.Run(async () => await LoadMessagesAsync());
        }

        public User FriendUser
        {
            get => friendUser;
            set { friendUser = value; OnPropertyChanged(); }
        }

        public string MessageText
        {
            get => messageText;
            set
            {
                if (messageText == value) return;
                messageText = value;
                OnPropertyChanged();
                ((Command)SendMessageCommand).ChangeCanExecute();
            }
        }

        public string ChatTitle
        {
            get => chatTitle;
            set { chatTitle = value; OnPropertyChanged(); }
        }

        public bool IsBusy
        {
            get => isBusy;
            set { isBusy = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Models.Messages> Messages
        {
            get => messages;
            set { messages = value; OnPropertyChanged(); }
        }

        public ICommand SendMessageCommand { get; }
        public ICommand LoadMessagesCommand { get; }

        private async Task SendMessageAsync()
        {
            if (string.IsNullOrWhiteSpace(MessageText) || FriendUser == null || !UserSession.IsLoggedIn)
                return;

            try
            {
                IsBusy = true;

                var success = await apiService.SendMessageAsync(
                    UserSession.CurrentUserId,
                    FriendUser.Id,
                    MessageText);

                if (success)
                {
                    var newMessage = new Models.Messages
                    {
                        SenderId = UserSession.CurrentUserId,
                        EmpfaengerId = FriendUser.Id,
                        Message = MessageText,
                        SentAt = DateTime.Now,
                        SenderName = "Sie",
                        IsOwnMessage = true
                    };

                    Messages.Add(newMessage);
                    MessageText = string.Empty;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ChatViewModel] SendMessage Error: {ex}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LoadMessagesAsync()
        {
            if (FriendUser == null || !UserSession.IsLoggedIn)
                return;

            try
            {
                IsBusy = true;

                var chatHistory = await apiService.GetChatHistoryAsync(
                    UserSession.CurrentUserId,
                    FriendUser.Id);

                Messages.Clear();
                foreach (var msg in chatHistory)
                {
                    msg.IsOwnMessage = msg.SenderId == UserSession.CurrentUserId;
                    msg.SenderName = msg.IsOwnMessage ? "Sie" : FriendUser.Username;
                    Messages.Add(msg);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ChatViewModel] LoadMessages Error: {ex}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
