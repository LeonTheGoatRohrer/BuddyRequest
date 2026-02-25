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
        private ObservableCollection<FriendRequestWithUser> pendingRequests;
        private string message;
        private bool isBusy;

        private readonly ApiService apiService;

        public event PropertyChangedEventHandler PropertyChanged;

        public FriendsViewModel()
        {
            apiService = new ApiService();

            SearchResults = new ObservableCollection<User>();
            Friends = new ObservableCollection<User>();
            PendingRequests = new ObservableCollection<FriendRequestWithUser>();

            SearchCommand = new Command(async () => await SearchAsync());
            ShowAllUsersCommand = new Command(async () => await ShowAllUsersAsync());
            SendRequestCommand = new Command<User>(async (user) => await SendRequestAsync(user));
            LoadFriendsCommand = new Command(async () => await LoadFriendsAsync());
            LoadPendingRequestsCommand = new Command(async () => await LoadPendingRequestsAsync());
            AcceptRequestCommand = new Command<FriendRequestWithUser>(async (request) => await AcceptRequestAsync(request));
            DeclineRequestCommand = new Command<FriendRequestWithUser>(async (request) => await DeclineRequestAsync(request));
            OpenChatCommand = new Command<User>(async (friend) => await OpenChatAsync(friend));

            if (UserSession.IsLoggedIn)
            {
                Message = $"Eingeloggt als: {UserSession.CurrentUser.Username} (ID: {UserSession.CurrentUserId})";
            }
            else
            {
                Message = "Nicht eingeloggt! Bitte zuerst anmelden.";
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

        public ObservableCollection<FriendRequestWithUser> PendingRequests
        {
            get => pendingRequests;
            set
            {
                if (pendingRequests == value) return;
                pendingRequests = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasPendingRequests));
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
        public bool HasPendingRequests => PendingRequests != null && PendingRequests.Count > 0;

        public ICommand SearchCommand { get; }
        public ICommand ShowAllUsersCommand { get; }
        public ICommand SendRequestCommand { get; }
        public ICommand LoadFriendsCommand { get; }
        public ICommand LoadPendingRequestsCommand { get; }
        public ICommand AcceptRequestCommand { get; }
        public ICommand DeclineRequestCommand { get; }
        public ICommand OpenChatCommand { get; }

        private async Task SearchAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                Message = "Bitte einen Suchbegriff eingeben.";
                return;
            }

            if (!UserSession.IsLoggedIn)
            {
                Message = "Bitte zuerst anmelden!";
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
                Message = "Bitte zuerst anmelden!";
                return;
            }

            try
            {
                IsBusy = true;
                Message = $"Sende Anfrage an {user.Username}...";

                System.Diagnostics.Debug.WriteLine($"[FriendsViewModel] Sending request from User {UserSession.CurrentUserId} to User {user.Id}");

                var success = await apiService.SendFriendRequestAsync(UserSession.CurrentUserId, user.Id);

                if (success)
                {
                    // Button-Status ändern
                    user.RequestSent = true;
                    
                    Message = $"Freundschaftsanfrage an {user.Username} gesendet!";
                    System.Diagnostics.Debug.WriteLine($"[FriendsViewModel] Request sent successfully");
                    
                    // UI aktualisieren
                    OnPropertyChanged(nameof(SearchResults));
                }
                else
                {
                    Message = "Anfrage konnte nicht gesendet werden.";
                    System.Diagnostics.Debug.WriteLine($"[FriendsViewModel] Request failed (returned false)");
                }
            }
            catch (Exception ex)
            {
                Message = $"Fehler: {ex.Message}";
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
                Message = "Bitte zuerst anmelden!";
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
                Message = "Bitte zuerst anmelden!";
                return;
            }

            try
            {
                IsBusy = true;
                Message = string.Empty;

                var requests = await apiService.GetPendingRequestsWithUserAsync(UserSession.CurrentUserId);

                PendingRequests.Clear();
                foreach (var request in requests)
                {
                    PendingRequests.Add(request);
                }

                Message = $"{PendingRequests.Count} offene Anfragen vorhanden.";
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

        private async Task ShowAllUsersAsync()
        {
            if (!UserSession.IsLoggedIn)
            {
                Message = "Bitte zuerst anmelden!";
                return;
            }

            try
            {
                IsBusy = true;
                Message = string.Empty;

                var allUsers = await apiService.GetAllUsersAsync(UserSession.CurrentUserId);

                SearchResults.Clear();
                foreach (var user in allUsers)
                {
                    if (user.Id != UserSession.CurrentUserId)
                    {
                        SearchResults.Add(user);
                    }
                }

                if (SearchResults.Count == 0)
                {
                    Message = "Keine anderen Benutzer gefunden.";
                }
            }
            catch (Exception ex)
            {
                Message = "Fehler beim Laden der Benutzer: " + ex.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task AcceptRequestAsync(FriendRequestWithUser request)
        {
            if (request == null) return;

            if (!UserSession.IsLoggedIn)
            {
                Message = "Bitte zuerst anmelden!";
                return;
            }

            try
            {
                IsBusy = true;
                Message = $"Akzeptiere Anfrage von {request.SenderUsername}...";

                var success = await apiService.AcceptFriendRequestAsync(request.RequestId);

                if (success)
                {
                    Message = $"Anfrage von {request.SenderUsername} akzeptiert!";
                    PendingRequests.Remove(request);
                }
                else
                {
                    Message = "Anfrage konnte nicht akzeptiert werden.";
                }
            }
            catch (Exception ex)
            {
                Message = $"Fehler beim Akzeptieren: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task DeclineRequestAsync(FriendRequestWithUser request)
        {
            if (request == null) return;

            if (!UserSession.IsLoggedIn)
            {
                Message = "Bitte zuerst anmelden!";
                return;
            }

            try
            {
                IsBusy = true;
                Message = $"Lehne Anfrage von {request.SenderUsername} ab...";

                var success = await apiService.DeclineFriendRequestAsync(request.RequestId);

                if (success)
                {
                    Message = $"Anfrage von {request.SenderUsername} abgelehnt.";
                    PendingRequests.Remove(request);
                }
                else
                {
                    Message = "Anfrage konnte nicht abgelehnt werden.";
                }
            }
            catch (Exception ex)
            {
                Message = $"Fehler beim Ablehnen: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task OpenChatAsync(User friend)
        {
            if (friend == null) return;

            await Shell.Current.GoToAsync(nameof(Views.ChatPage), new Dictionary<string, object>
            {
                { "Friend", friend }
            });
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
