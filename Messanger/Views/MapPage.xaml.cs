using Messanger.Services;
using System.Text.Json;

namespace Messanger.Views
{
    public partial class MapPage : ContentPage
    {
        private readonly ApiService _apiService;
        private CancellationTokenSource _updateCts;
        private bool _isSharing;

        public MapPage()
        {
            InitializeComponent();
            _apiService = new ApiService();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadMap();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _updateCts?.Cancel();
        }

        private void LoadMap(double lat = 47.26, double lng = 11.39)
        {
            var html = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8' />
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <link rel='stylesheet' href='https://unpkg.com/leaflet@1.9.4/dist/leaflet.css' />
    <script src='https://unpkg.com/leaflet@1.9.4/dist/leaflet.js'></script>
    <style>
        body {{ margin: 0; padding: 0; }}
        #map {{ width: 100%; height: 100vh; }}
    </style>
</head>
<body>
    <div id='map'></div>
    <script>
        var map = L.map('map').setView([{lat.ToString(System.Globalization.CultureInfo.InvariantCulture)}, {lng.ToString(System.Globalization.CultureInfo.InvariantCulture)}], 13);

        L.tileLayer('https://{{s}}.tile.openstreetmap.org/{{z}}/{{x}}/{{y}}.png', {{
            attribution: '&copy; OpenStreetMap'
        }}).addTo(map);

        var markers = {{}};

        function updateMarkers(friends) {{
            // Alte Marker entfernen
            Object.keys(markers).forEach(function(key) {{
                map.removeLayer(markers[key]);
            }});
            markers = {{}};

            // Neue Marker setzen
            friends.forEach(function(f) {{
                if (f.latitude && f.longitude) {{
                    var fIcon = L.divIcon({{
                        className: 'friend-marker',
                        html: '<div style=""background:#007AFF;color:white;padding:5px 10px;border-radius:14px;font-size:13px;font-weight:bold;white-space:nowrap;box-shadow:0 2px 6px rgba(0,0,0,0.5);border:2px solid white;"">' + f.username + '</div>',
                        iconSize: [0, 0],
                        iconAnchor: [40, 20]
                    }});
                    var marker = L.marker([f.latitude, f.longitude], {{icon: fIcon}})
                        .addTo(map)
                        .bindPopup('<b>' + f.username + '</b><br>📍 Online');
                    markers[f.id] = marker;
                }}
            }});
        }}

        function setMyLocation(lat, lng, name) {{
            if (markers['me']) map.removeLayer(markers['me']);

            var myIcon = L.divIcon({{
                className: 'my-marker',
                html: '<div style=""background:#34C759;color:white;padding:5px 10px;border-radius:14px;font-size:13px;font-weight:bold;white-space:nowrap;box-shadow:0 2px 6px rgba(0,0,0,0.5);border:2px solid white;"">' + name + ' (Ich)</div>',
                iconSize: [0, 0],
                iconAnchor: [40, 20]
            }});

            markers['me'] = L.marker([lat, lng], {{icon: myIcon}}).addTo(map);
            map.setView([lat, lng], 14);
        }}
    </script>
</body>
</html>";

            MapWebView.Source = new HtmlWebViewSource { Html = html };
        }

        private void OnSharingToggled(object sender, ToggledEventArgs e)
        {
            _isSharing = e.Value;
            if (_isSharing)
                StartLocationSharing();
            else
                _updateCts?.Cancel();
        }

        private async void StartLocationSharing()
        {
            _updateCts = new CancellationTokenSource();
            var userId = UserSession.CurrentUserId;
            var username = UserSession.CurrentUsername;

            if (userId <= 0) return;

            while (!_updateCts.Token.IsCancellationRequested)
            {
                try
                {
                    // GPS holen
                    var location = await Geolocation.GetLocationAsync(new GeolocationRequest
                    {
                        DesiredAccuracy = GeolocationAccuracy.Best,
                        Timeout = TimeSpan.FromSeconds(10)
                    });

                    if (location != null)
                    {
                        // Eigenen Standort an Server senden
                        await _apiService.ShareLocationAsync(userId, location.Latitude, location.Longitude);

                        // Eigenen Marker auf Karte setzen
                        var latStr = location.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
                        var lngStr = location.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
                        await MapWebView.EvaluateJavaScriptAsync(
                            $"setMyLocation({latStr}, {lngStr}, '{username}')");
                    }

                    // Freunde-Locations holen
                    var friends = await _apiService.GetFriendsLocationsAsync(userId);
                    if (friends != null && friends.Count > 0)
                    {
                        var json = JsonSerializer.Serialize(friends);
                        await MapWebView.EvaluateJavaScriptAsync($"updateMarkers({json})");
                    }

                    await Task.Delay(10000, _updateCts.Token);
                }
                catch (TaskCanceledException) { break; }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[MapPage] Error: {ex.Message}");
                    await Task.Delay(5000);
                }
            }
        }
    }
}
