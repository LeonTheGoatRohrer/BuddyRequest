using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Models;
using Messanger.Services;

namespace Messanger.ViewModels
{
    public class RequestsViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService;

        public ObservableCollection<User> Friends { get; set; } = new();
        public ObservableCollection<RequestWithUser> PendingRequests { get; set; } = new();
        public ObservableCollection<RequestWithUser> SentRequests { get; set; } = new();
        public ObservableCollection<RequestWithUser> History { get; set; } = new();

        public List<string> RequestTypes { get; set; } = new()
        {
            "Wasser",
            "Lebensmittel",
            "Werkzeug",
            "Hilfe",
            "Transport",
            "Allgemein"
        };

        private User? _selectedFriend;
        public User? SelectedFriend
        {
            get => _selectedFriend;
            set
            {
                _selectedFriend = value;
                OnPropertyChanged();
                ((Command)SendRequestCommand).ChangeCanExecute();
            }
        }

        private string _requestType = "Allgemein";
        public string RequestType
        {
            get => _requestType;
            set
            {
                _requestType = value;
                OnPropertyChanged();
            }
        }

        private string _requestMessage = string.Empty;
        public string RequestMessage
        {
            get => _requestMessage;
            set
            {
                _requestMessage = value;
                OnPropertyChanged();
            }
        }

        private string _statusMessage = string.Empty;
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoadFriendsCommand { get; }
        public ICommand SendRequestCommand { get; }
        public ICommand LoadPendingRequestsCommand { get; }
        public ICommand LoadSentRequestsCommand { get; }
        public ICommand LoadHistoryCommand { get; }
        public ICommand AcceptRequestCommand { get; }
        public ICommand DeclineRequestCommand { get; }

        public RequestsViewModel()
        {
            _apiService = new ApiService();

            System.Diagnostics.Debug.WriteLine($"[RequestsViewModel] Constructor - UserId: {UserSession.CurrentUserId}");

            LoadFriendsCommand = new Command(OnLoadFriends);
            SendRequestCommand = new Command(OnSendRequest);
            LoadPendingRequestsCommand = new Command(OnLoadPendingRequests);
            LoadSentRequestsCommand = new Command(OnLoadSentRequests);
            LoadHistoryCommand = new Command(OnLoadHistory);
            AcceptRequestCommand = new Command<RequestWithUser>(OnAcceptRequest);
            DeclineRequestCommand = new Command<RequestWithUser>(OnDeclineRequest);
        }

        private async void OnLoadFriends()
        {
            await LoadFriendsAsync();
        }

        private async void OnSendRequest()
        {
            System.Diagnostics.Debug.WriteLine("[RequestsViewModel] OnSendRequest - Button geklickt!");
            await SendRequestAsync();
        }

        private async void OnLoadPendingRequests()
        {
            await LoadPendingRequestsAsync();
        }

        private async void OnLoadSentRequests()
        {
            await LoadSentRequestsAsync();
        }

        private async void OnLoadHistory()
        {
            await LoadHistoryAsync();
        }

        private async void OnAcceptRequest(RequestWithUser request)
        {
            await AcceptRequestAsync(request);
        }

        private async void OnDeclineRequest(RequestWithUser request)
        {
            await DeclineRequestAsync(request);
        }

        public async Task LoadFriendsAsync()
        {
            try
            {
                var friends = await _apiService.GetFriendsAsync(UserSession.CurrentUserId);
                Friends.Clear();
                foreach (var friend in friends)
                {
                    Friends.Add(friend);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[RequestsViewModel] LoadFriends error: {ex.Message}");
            }
        }

        public async Task SendRequestAsync()
        {
            System.Diagnostics.Debug.WriteLine("[RequestsViewModel] SendRequestAsync aufgerufen");
            System.Diagnostics.Debug.WriteLine($"[RequestsViewModel] UserId: {UserSession.CurrentUserId}");
            System.Diagnostics.Debug.WriteLine($"[RequestsViewModel] SelectedFriend: {SelectedFriend?.Username ?? "NULL"}");
            System.Diagnostics.Debug.WriteLine($"[RequestsViewModel] RequestMessage: {RequestMessage}");
            System.Diagnostics.Debug.WriteLine($"[RequestsViewModel] RequestType: {RequestType}");

            if (SelectedFriend == null)
            {
                StatusMessage = "⚠️ Bitte wähle einen Freund aus.";
                System.Diagnostics.Debug.WriteLine("[RequestsViewModel] SelectedFriend ist NULL");
                return;
            }

            if (string.IsNullOrWhiteSpace(RequestMessage))
            {
                StatusMessage = "⚠️ Bitte gib eine Nachricht ein.";
                System.Diagnostics.Debug.WriteLine("[RequestsViewModel] RequestMessage ist leer");
                return;
            }

            try
            {
                StatusMessage = "⏳ Sende Request...";
                System.Diagnostics.Debug.WriteLine($"[RequestsViewModel] Sende Request: UserId={UserSession.CurrentUserId}, FriendId={SelectedFriend.Id}, Type={RequestType}, Message={RequestMessage}");

                var success = await _apiService.CreateRequestAsync(UserSession.CurrentUserId, SelectedFriend.Id, RequestType, RequestMessage);

                System.Diagnostics.Debug.WriteLine($"[RequestsViewModel] API Response: {success}");

                if (success)
                {
                    StatusMessage = $"✅ Request an {SelectedFriend.Username} gesendet!";
                    System.Diagnostics.Debug.WriteLine("[RequestsViewModel] Request erfolgreich gesendet");
                    
                    RequestMessage = string.Empty;
                    SelectedFriend = null;
                    await LoadSentRequestsAsync();
                }
                else
                {
                    StatusMessage = "❌ Fehler beim Senden des Requests.";
                    System.Diagnostics.Debug.WriteLine("[RequestsViewModel] API gab FALSE zurück");
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"❌ Fehler: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"[RequestsViewModel] SendRequest Exception: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[RequestsViewModel] StackTrace: {ex.StackTrace}");
            }
        }

        public async Task LoadPendingRequestsAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[RequestsViewModel] LoadPendingRequestsAsync START - UserId: {UserSession.CurrentUserId}");
                var requests = await _apiService.GetPendingRequestsAsync2(UserSession.CurrentUserId);
                System.Diagnostics.Debug.WriteLine($"[RequestsViewModel] Erhaltene Pending Requests: {requests.Count}");
                
                PendingRequests.Clear();
                foreach (var request in requests)
                {
                    System.Diagnostics.Debug.WriteLine($"[RequestsViewModel] Pending Request: Id={request.Id}, Von={request.SenderUsername}, An=UserId({request.ReceiverId}), Typ={request.RequestType}");
                    PendingRequests.Add(request);
                }
                
                System.Diagnostics.Debug.WriteLine($"[RequestsViewModel] PendingRequests.Count nach Laden: {PendingRequests.Count}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[RequestsViewModel] LoadPendingRequests error: {ex.Message}");
            }
        }

        public async Task LoadSentRequestsAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[RequestsViewModel] LoadSentRequestsAsync START - UserId: {UserSession.CurrentUserId}");
                var requests = await _apiService.GetSentRequestsAsync(UserSession.CurrentUserId);
                System.Diagnostics.Debug.WriteLine($"[RequestsViewModel] Erhaltene Sent Requests: {requests.Count}");
                
                SentRequests.Clear();
                foreach (var request in requests)
                {
                    System.Diagnostics.Debug.WriteLine($"[RequestsViewModel] Sent Request: Id={request.Id}, Von=UserId({request.SenderId}), An={request.ReceiverUsername}, Typ={request.RequestType}");
                    SentRequests.Add(request);
                }
                
                System.Diagnostics.Debug.WriteLine($"[RequestsViewModel] SentRequests.Count nach Laden: {SentRequests.Count}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[RequestsViewModel] LoadSentRequests error: {ex.Message}");
            }
        }

        public async Task LoadHistoryAsync()
        {
            try
            {
                var requests = await _apiService.GetRequestHistoryAsync(UserSession.CurrentUserId);
                History.Clear();
                foreach (var request in requests)
                {
                    History.Add(request);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[RequestsViewModel] LoadHistory error: {ex.Message}");
            }
        }

        public async Task AcceptRequestAsync(RequestWithUser request)
        {
            try
            {
                var success = await _apiService.AcceptRequestAsync2(request.Id);
                if (success)
                {
                    PendingRequests.Remove(request);
                    await LoadHistoryAsync();
                    StatusMessage = "Request akzeptiert!";
                }
                else
                {
                    StatusMessage = "Fehler beim Akzeptieren.";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[RequestsViewModel] AcceptRequest error: {ex.Message}");
            }
        }

        public async Task DeclineRequestAsync(RequestWithUser request)
        {
            try
            {
                var success = await _apiService.DeclineRequestAsync2(request.Id);
                if (success)
                {
                    PendingRequests.Remove(request);
                    await LoadHistoryAsync();
                    StatusMessage = "Request abgelehnt.";
                }
                else
                {
                    StatusMessage = "Fehler beim Ablehnen.";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[RequestsViewModel] DeclineRequest error: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
