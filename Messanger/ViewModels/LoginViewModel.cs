using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Messanger.Services;
using Microsoft.Maui.Controls;

namespace Messanger.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private string username;
        private string email;
        private string password;
        private string message;
        private bool isBusy;

        private readonly ApiService apiService;
        private readonly Command loginCommand;

        public event PropertyChangedEventHandler PropertyChanged;

        public LoginViewModel()
        {
            apiService = new ApiService();

            loginCommand = new Command(
                async () => await LoginAsync(),
                () => !IsBusy);

            Message = string.Empty;
        }

        public string Username
        {
            get => username;
            set
            {
                if (username == value)
                {
                    return;
                }

                username = value;
                OnPropertyChanged();
            }
        }

        public string Email
        {
            get => email;
            set
            {
                if (email == value)
                {
                    return;
                }

                email = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get => password;
            set
            {
                if (password == value)
                {
                    return;
                }

                password = value;
                OnPropertyChanged();
            }
        }

        public string Message
        {
            get => message;
            set
            {
                if (message == value)
                {
                    return;
                }

                message = value;
                OnPropertyChanged();
            }
        }

        public bool IsBusy
        {
            get => isBusy;
            set
            {
                if (isBusy == value)
                {
                    return;
                }

                isBusy = value;
                OnPropertyChanged();
                loginCommand.ChangeCanExecute();
            }
        }

        public ICommand LoginCommand => loginCommand;

        private async Task LoginAsync()
        {
            if (IsBusy)
            {
                return;
            }

            Message = string.Empty;

            if (string.IsNullOrWhiteSpace(Username))
            {
                Message = "Bitte einen Benutzernamen eingeben.";
                return;
            }

            if (string.IsNullOrWhiteSpace(Email))
            {
                Message = "Bitte eine E-Mail-Adresse eingeben.";
                return;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                Message = "Bitte ein Passwort eingeben.";
                return;
            }

            try
            {
                IsBusy = true;

                var user = await apiService.LoginAsync(Username, Password, Email);

                if (user != null)
                {
                    Message = "Login erfolgreich!";

                    // User-Session speichern
                    UserSession.Login(user);

                    await Shell.Current.GoToAsync("///MainPage");

                    Username = string.Empty;
                    Email = string.Empty;
                    Password = string.Empty;
                }
                else
                {
                    Message = "Login fehlgeschlagen.";
                }
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

        private void OnPropertyChanged(
            [CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName));
        }
    }
}
