using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Messanger.Services;

namespace Messanger.ViewModels
{
    public class MapViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService;
        private int _userId;
        private bool _isSharing = false;
        private ObservableCollection<FriendLocationViewModel> _friendLocations;
        private CancellationTokenSource _updateCts;

        public bool IsSharing
        {
            get => _isSharing;
            set 
            { 
                if (_isSharing != value)
                {
                    _isSharing = value;
                    OnPropertyChanged();
                    if (value)
                        StartLocationSharing();
                    else
                        StopLocationSharing();
                }
            }
        }

        public ObservableCollection<FriendLocationViewModel> FriendLocations
        {
            get => _friendLocations;
            set { if (_friendLocations != value) { _friendLocations = value; OnPropertyChanged(); } }
        }

        public MapViewModel()
        {
            _apiService = new ApiService();
            _friendLocations = new ObservableCollection<FriendLocationViewModel>();
            _updateCts = new CancellationTokenSource();
        }

        public async void Initialize(int userId)
        {
            _userId = userId;
            await LoadFriendLocations();
        }

        public async Task LoadFriendLocations()
        {
            try
            {
                var locations = await _apiService.GetFriendsLocationsAsync(_userId);
                
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    FriendLocations.Clear();
                    
                    if (locations != null && locations.Count > 0)
                    {
                        foreach (var loc in locations)
                        {
                            var friendLoc = new FriendLocationViewModel
                            {
                                Id = loc["id"],
                                Username = loc["username"],
                                Latitude = loc["latitude"],
                                Longitude = loc["longitude"],
                                AvatarUrl = loc["avatarUrl"]
                            };
                            FriendLocations.Add(friendLoc);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MapViewModel] LoadFriendLocations error: {ex.Message}");
            }
        }

        public async void StartLocationSharing()
        {
            _updateCts = new CancellationTokenSource();
            
            while (!_updateCts.Token.IsCancellationRequested)
            {
                try
                {
                    // Hole aktuelle Location
                    var location = await Geolocation.GetLocationAsync(new GeolocationRequest
                    {
                        DesiredAccuracy = GeolocationAccuracy.Best,
                        Timeout = TimeSpan.FromSeconds(10)
                    });

                    if (location != null)
                    {
                        // Teile Location mit Server
                        await _apiService.ShareLocationAsync(_userId, location.Latitude, location.Longitude);
                    }

                    // Update Freunde-Locations alle 10 Sekunden
                    await Task.Delay(10000, _updateCts.Token);
                    await LoadFriendLocations();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[MapViewModel] Location sharing error: {ex.Message}");
                }
            }
        }

        public void StopLocationSharing()
        {
            _updateCts?.Cancel();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class FriendLocationViewModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string AvatarUrl { get; set; }
    }
}
