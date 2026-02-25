using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Messanger.Services;
using Microsoft.Maui.Controls;
using Models;

namespace Messanger.ViewModels
{
    public class ProfilViewModel : INotifyPropertyChanged
    {
        private string bio;
        private string username;
        private string email;
        private string avatarUrl;
        private string message;
        private bool isBusy;
        private readonly ApiService apiService;

        public event PropertyChangedEventHandler PropertyChanged;

        public ProfilViewModel()
        {
            apiService = new ApiService();
            User = UserSession.CurrentUser;
            Bio = User?.Bio ?? string.Empty;
            Username = User?.Username ?? string.Empty;
            Email = User?.Email ?? string.Empty;
            AvatarUrl = User?.Avatar ?? string.Empty;

            SaveBioCommand = new Command(async () => await SaveBioAsync(), () => !IsBusy);
            ChangeAvatarCommand = new Command(async () => await ChangeAvatarAsync(), () => !IsBusy);
            SaveProfileCommand = new Command(async () => await SaveProfileAsync(), () => !IsBusy);
            LogoutCommand = new Command(async () => await LogoutAsync());
        }

        public User User { get; }

        public string Bio
        {
            get => bio;
            set
            {
                if (bio == value) return;
                bio = value;
                OnPropertyChanged();
            }
        }

        public string Username
        {
            get => username;
            set
            {
                if (username == value) return;
                username = value;
                OnPropertyChanged();
            }
        }

        public string Email
        {
            get => email;
            set
            {
                if (email == value) return;
                email = value;
                OnPropertyChanged();
            }
        }

        public string AvatarUrl
        {
            get => avatarUrl;
            set
            {
                if (avatarUrl == value) return;
                avatarUrl = value;
                OnPropertyChanged();
            }
        }

        public string Message
        {
            get => message;
            set { if (message == value) return; message = value; OnPropertyChanged(); }
        }

        public bool IsBusy
        {
            get => isBusy;
            set { if (isBusy == value) return; isBusy = value; OnPropertyChanged(); }
        }

        public ICommand SaveBioCommand { get; }
        public ICommand ChangeAvatarCommand { get; }
        public ICommand SaveProfileCommand { get; }
        public ICommand LogoutCommand { get; }

        private async Task SaveBioAsync()
        {
            if (User == null || !UserSession.IsLoggedIn)
            {
                Message = "Nicht eingeloggt!";
                return;
            }

            try
            {
                IsBusy = true;
                Message = "Speichere Bio...";

                var success = await apiService.UpdateBioAsync(User.Id, Bio);

                if (success)
                {
                    User.Bio = Bio;
                    UserSession.Login(User);
                    Message = "Bio gespeichert!";
                }
                else
                {
                    Message = "Fehler beim Speichern.";
                }
            }
            catch (Exception ex)
            {
                Message = $"Fehler: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"[ProfilViewModel] SaveBio Error: {ex}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task ChangeAvatarAsync()
        {
            if (User == null || !UserSession.IsLoggedIn)
            {
                Message = "Nicht eingeloggt!";
                return;
            }

            try
            {
                IsBusy = true;
                Message = "Generiere neues Profilbild...";

                var newAvatarUrl = await apiService.UpdateAvatarAsync(User.Id);

                if (newAvatarUrl != null)
                {
                    User.AvatarUrl = newAvatarUrl;
                    AvatarUrl = newAvatarUrl;
                    UserSession.Login(User);
                    Message = "Neues Profilbild generiert!";
                }
                else
                {
                    Message = "Fehler beim Generieren.";
                }
            }
            catch (Exception ex)
            {
                Message = $"Fehler: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"[ProfilViewModel] ChangeAvatar Error: {ex}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SaveProfileAsync()
        {
            if (User == null || !UserSession.IsLoggedIn)
            {
                Message = "Nicht eingeloggt!";
                return;
            }

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Email))
            {
                Message = "Username und Email dürfen nicht leer sein!";
                return;
            }

            try
            {
                IsBusy = true;
                Message = "Speichere Profil...";

                var updatedUser = await apiService.UpdateProfileAsync(User.Id, Username, Email);

                if (updatedUser != null)
                {
                    User.Username = updatedUser.Username;
                    User.Email = updatedUser.Email;
                    UserSession.Login(updatedUser);
                    Message = "Profil gespeichert!";
                }
                else
                {
                    Message = "Fehler beim Speichern (evtl. Username vergeben).";
                }
            }
            catch (Exception ex)
            {
                Message = $"Fehler: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"[ProfilViewModel] SaveProfile Error: {ex}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LogoutAsync()
        {
            UserSession.Logout();
            await Shell.Current.GoToAsync("///LoginPage");
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
