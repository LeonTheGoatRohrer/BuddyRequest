using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Controls;

namespace Messanger.Views
{
    public partial class MapPage : ContentPage
    {
        public MapPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await ShowUserLocationAsync();
        }

        private async Task ShowUserLocationAsync()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                    if (status != PermissionStatus.Granted)
                    {
                        await DisplayAlert("Hinweis", "Standortberechtigung wurde nicht erteilt.", "OK");
                        return;
                    }
                }

                var location = await Geolocation.GetLastKnownLocationAsync();
                
                if (location == null)
                {
                    location = await Geolocation.GetLocationAsync(new GeolocationRequest
                    {
                        DesiredAccuracy = GeolocationAccuracy.Medium,
                        Timeout = TimeSpan.FromSeconds(10)
                    });
                }

                if (location != null)
                {
                    var position = new Location(location.Latitude, location.Longitude);
                    
                    UserMap.Pins.Clear();
                    UserMap.Pins.Add(new Pin
                    {
                        Label = "Dein Standort",
                        Location = position
                    });
                    UserMap.MoveToRegion(MapSpan.FromCenterAndRadius(position, Distance.FromKilometers(1)));
                }
                else
                {
                    // Fallback: Zeige Standardort (z.B. Wien)
                    var defaultPosition = new Location(48.2082, 16.3738);
                    UserMap.MoveToRegion(MapSpan.FromCenterAndRadius(defaultPosition, Distance.FromKilometers(10)));
                }
            }
            catch (FeatureNotSupportedException)
            {
                await DisplayAlert("Fehler", "GPS wird auf diesem Gerät nicht unterstützt.", "OK");
            }
            catch (FeatureNotEnabledException)
            {
                await DisplayAlert("Fehler", "Bitte aktiviere GPS in den Einstellungen.", "OK");
            }
            catch (PermissionException)
            {
                await DisplayAlert("Fehler", "Standortberechtigung fehlt.", "OK");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MapPage] Fehler: {ex.Message}");
                // Fallback: Zeige Standardort
                var defaultPosition = new Location(48.2082, 16.3738);
                UserMap.MoveToRegion(MapSpan.FromCenterAndRadius(defaultPosition, Distance.FromKilometers(10)));
            }
        }
    }
}
