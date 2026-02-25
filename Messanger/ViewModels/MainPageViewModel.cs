using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Messanger.Services;
using Models;

namespace Messanger.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private string username;
        private int userCount = 0;
        private int friendsCount = 0;
        private int appUses = 0;
        private string appLoad = "n/a";

        public string Username { get => username; set { username = value; OnPropertyChanged(); } }
        public int UserCount { get => userCount; set { userCount = value; OnPropertyChanged(); } }
        public int FriendsCount { get => friendsCount; set { friendsCount = value; OnPropertyChanged(); } }
        public int AppUses { get => appUses; set { appUses = value; OnPropertyChanged(); } }
        public string AppLoad { get => appLoad; set { appLoad = value; OnPropertyChanged(); } }

        public MainPageViewModel()
        {
            try
            {
                Username = UserSession.IsLoggedIn ? UserSession.CurrentUsername : "Gast";
                MainThread.BeginInvokeOnMainThread(async () => await LoadStatsAsync());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MainPageViewModel] Constructor Error: {ex.Message}");
                Username = "Gast";
            }
        }

        private async Task LoadStatsAsync()
        {
            try
            {
                if (!UserSession.IsLoggedIn)
                {
                    return;
                }

                var api = new ApiService();

                try
                {
                    var users = await api.GetAllUsersAsync(UserSession.CurrentUserId);
                    UserCount = users?.Count ?? 0;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[MainPageViewModel] GetAllUsers Error: {ex.Message}");
                    UserCount = 0;
                }

                try
                {
                    var friends = await api.GetFriendsAsync(UserSession.CurrentUserId);
                    FriendsCount = friends?.Count ?? 0;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[MainPageViewModel] GetFriends Error: {ex.Message}");
                    FriendsCount = 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MainPageViewModel] LoadStats Error: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
