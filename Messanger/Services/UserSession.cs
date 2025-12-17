using Models;

namespace Messanger.Services
{
    public static class UserSession
    {
        private static User? currentUser;

        public static User? CurrentUser
        {
            get => currentUser;
            private set => currentUser = value;
        }

        public static int CurrentUserId => CurrentUser?.Id ?? 0;

        public static bool IsLoggedIn => CurrentUser != null && CurrentUser.Id > 0;

        public static void Login(User user)
        {
            CurrentUser = user;
            
            // Optional: In SecureStorage speichern für App-Neustart
            Preferences.Set("user_id", user.Id);
            Preferences.Set("username", user.Username);
            Preferences.Set("email", user.Email);
            Preferences.Set("key", user.Key ?? string.Empty);
        }

        public static void Logout()
        {
            CurrentUser = null;
            
            // Aus Storage löschen
            Preferences.Remove("user_id");
            Preferences.Remove("username");
            Preferences.Remove("email");
            Preferences.Remove("key");
        }

        public static void LoadFromStorage()
        {
            var userId = Preferences.Get("user_id", 0);
            
            if (userId > 0)
            {
                CurrentUser = new User
                {
                    Id = userId,
                    Username = Preferences.Get("username", string.Empty),
                    Email = Preferences.Get("email", string.Empty),
                    Key = Preferences.Get("key", string.Empty)
                };
            }
        }
    }
}
