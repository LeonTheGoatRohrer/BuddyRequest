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

            DemoLoginCommand = new Command(
                async () => await DemoLoginAsync(),
                () => !IsBusy);

            DemoLoginFranzCommand = new Command(
                async () => await DemoLoginFranzAsync(),
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
        public ICommand DemoLoginCommand { get; }
        public ICommand DemoLoginFranzCommand { get; }

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

            if (string.IsNullOrWhiteSpace(Password))
            {
                Message = "Bitte ein Passwort eingeben.";
                return;
            }

            try
            {
                IsBusy = true;

                var user = await apiService.LoginAsync(Username, Password);

                if (user != null)
                {
                    Message = "Login erfolgreich!";

                    // User-Session speichern
                    UserSession.Login(user);

                    // Navigation zum MainPage FlyoutItem
                    var shell = Shell.Current;
                    if (shell.Items.Count > 1)
                    {
                        shell.CurrentItem = shell.Items[1]; // MainPage ist Index 1
                    }

                    Username = string.Empty;
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

        private async Task DemoLoginAsync()
        {
            if (IsBusy) return;

            // Demo-User-Daten automatisch setzen
            Username = "LeonROhrer";
            Password = "Leon Rohrer 2006";

            Message = "Demo-Login wird durchgeführt...";

            // Login ausführen
            await LoginAsync();
        }

        private async Task DemoLoginFranzAsync()
        {
            if (IsBusy) return;

            // FranzSepp Demo-User-Daten automatisch setzen
            Username = "FranzSepp";
            Password = "FranzSepp21";

            Message = "Demo-Login (FranzSepp) wird durchgeführt...";

            // Login ausführen
            await LoginAsync();
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
