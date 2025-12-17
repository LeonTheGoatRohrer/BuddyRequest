using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Messanger.Services;
using Microsoft.Maui.Controls;
using Models;

namespace Messanger.ViewModels
{
    public class FriendsViewModel : INotifyPropertyChanged
    {
        private string searchQuery;
        private ObservableCollection<User> searchResults;
        private ObservableCollection<User> friends;
        private string message;
        private bool isBusy;

        private readonly ApiService apiService;

        public event PropertyChangedEventHandler PropertyChanged;

        public FriendsViewModel()
        {
            apiService = new ApiService();

            SearchResults = new ObservableCollection<User>();
            Friends = new ObservableCollection<User>();

            SearchCommand = new Command(async () => await SearchAsync());
            SendRequestCommand = new Command<User>(async (user) => await SendRequestAsync(user));
            LoadFriendsCommand = new Command(async () => await LoadFriendsAsync());
            LoadPendingRequestsCommand = new Command(async () => await LoadPendingRequestsAsync());

            if (UserSession.IsLoggedIn)
            {
                Message = $"Eingeloggt als: {UserSession.CurrentUser.Username} (ID: {UserSession.CurrentUserId})";
            }
            else
            {
                Message = "?? Nicht eingeloggt! Bitte zuerst anmelden.";
            }
            
            System.Diagnostics.Debug.WriteLine($"[FriendsViewModel] Initialized with currentUserId = {UserSession.CurrentUserId}");
        }

        public string SearchQuery
        {
            get => searchQuery;
            set
            {
                if (searchQuery == value) return;
                searchQuery = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<User> SearchResults
        {
            get => searchResults;
            set
            {
                if (searchResults == value) return;
                searchResults = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasSearchResults));
            }
        }

        public ObservableCollection<User> Friends
        {
            get => friends;
            set
            {
                if (friends == value) return;
                friends = value;
                OnPropertyChanged();
            }
        }

        public string Message
        {
            get => message;
            set
            {
                if (message == value) return;
                message = value;
                OnPropertyChanged();
            }
        }

        public bool IsBusy
        {
            get => isBusy;
            set
            {
                if (isBusy == value) return;
                isBusy = value;
                OnPropertyChanged();
            }
        }

        public bool HasSearchResults => SearchResults != null && SearchResults.Count > 0;

        public ICommand SearchCommand { get; }
        public ICommand SendRequestCommand { get; }
        public ICommand LoadFriendsCommand { get; }
        public ICommand LoadPendingRequestsCommand { get; }

        private async Task SearchAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                Message = "Bitte einen Suchbegriff eingeben.";
                return;
            }

            if (!UserSession.IsLoggedIn)
            {
                Message = "?? Bitte zuerst anmelden!";
                return;
            }

            try
            {
                IsBusy = true;
                Message = string.Empty;

                var results = await apiService.SearchUsersAsync(SearchQuery);
                
                SearchResults.Clear();
                foreach (var user in results)
                {
                    if (user.Id != UserSession.CurrentUserId)
                    {
                        SearchResults.Add(user);
                    }
                }

                if (SearchResults.Count == 0)
                {
                    Message = "Keine Benutzer gefunden.";
                }
            }
            catch (Exception ex)
            {
                Message = "Fehler bei der Suche: " + ex.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SendRequestAsync(User user)
        {
            if (user == null) return;

            if (!UserSession.IsLoggedIn)
            {
                Message = "?? Bitte zuerst anmelden!";
                return;
            }

            try
            {
                IsBusy = true;
                Message = $"Sende Anfrage an {user.Username} (ID: {user.Id})...";

                System.Diagnostics.Debug.WriteLine($"[FriendsViewModel] Sending request from User {UserSession.CurrentUserId} to User {user.Id}");

                var success = await apiService.SendFriendRequestAsync(UserSession.CurrentUserId, user.Id);

                if (success)
                {
                    Message = $"? Freundschaftsanfrage an {user.Username} gesendet!";
                    System.Diagnostics.Debug.WriteLine($"[FriendsViewModel] Request sent successfully");
                }
                else
                {
                    Message = "? Anfrage konnte nicht gesendet werden.";
                    System.Diagnostics.Debug.WriteLine($"[FriendsViewModel] Request failed (returned false)");
                }
            }
            catch (Exception ex)
            {
                Message = $"? Fehler: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"[FriendsViewModel] Exception: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[FriendsViewModel] StackTrace: {ex.StackTrace}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LoadFriendsAsync()
        {
            if (!UserSession.IsLoggedIn)
            {
                Message = "?? Bitte zuerst anmelden!";
                return;
            }

            try
            {
                IsBusy = true;
                Message = string.Empty;

                var friendsList = await apiService.GetFriendsAsync(UserSession.CurrentUserId);

                Friends.Clear();
                foreach (var friend in friendsList)
                {
                    Friends.Add(friend);
                }

                Message = $"{Friends.Count} Freunde geladen.";
            }
            catch (Exception ex)
            {
                Message = "Fehler beim Laden: " + ex.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LoadPendingRequestsAsync()
        {
            if (!UserSession.IsLoggedIn)
            {
                Message = "?? Bitte zuerst anmelden!";
                return;
            }

            try
            {
                IsBusy = true;
                Message = string.Empty;

                var requests = await apiService.GetPendingRequestsAsync(UserSession.CurrentUserId);

                Message = $"{requests.Count} offene Anfragen vorhanden.";
                // TODO: Anfragen anzeigen und Accept/Decline Buttons
            }
            catch (Exception ex)
            {
                Message = "Fehler: " + ex.Message;
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
