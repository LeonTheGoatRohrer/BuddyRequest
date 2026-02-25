namespace Messanger.Services
{
    public static class LocalizationService
    {
        private static string _currentLanguage = "de";

        public static event Action LanguageChanged;

        public static string CurrentLanguage
        {
            get => _currentLanguage;
            set
            {
                if (_currentLanguage != value)
                {
                    _currentLanguage = value;
                    Preferences.Set("AppLanguage", value);
                    LanguageChanged?.Invoke();
                }
            }
        }

        public static void LoadSavedLanguage()
        {
            _currentLanguage = Preferences.Get("AppLanguage", "de");
        }

        public static string Get(string key)
        {
            if (_currentLanguage == "en" && EnStrings.TryGetValue(key, out var en))
                return en;
            if (DeStrings.TryGetValue(key, out var de))
                return de;
            return key;
        }

        public static string Get(string key, params object[] args)
        {
            return string.Format(Get(key), args);
        }

        public static List<string> AvailableLanguages => ["Deutsch", "English"];
        public static List<string> AvailableLanguageCodes => ["de", "en"];

        private static readonly Dictionary<string, string> DeStrings = new()
        {
            // Shell / Navigation
            ["Nav_Login"] = "Anmelden",
            ["Nav_Home"] = "Startseite",
            ["Nav_Friends"] = "Freunde",
            ["Nav_Profile"] = "Profil",
            ["Nav_Register"] = "Registrierung",
            ["Nav_Settings"] = "Einstellungen",

            // Login
            ["Login_Title"] = "Anmelden",
            ["Login_Username"] = "Benutzername",
            ["Login_Password"] = "Passwort",
            ["Login_Button"] = "Anmelden",
            ["Login_DemoLogin"] = "Demo-Login:",
            ["Login_Register"] = "Noch kein Konto? Registrieren",
            ["Login_ErrorUsername"] = "Bitte einen Benutzernamen eingeben.",
            ["Login_ErrorPassword"] = "Bitte ein Passwort eingeben.",
            ["Login_Success"] = "Login erfolgreich!",
            ["Login_Failed"] = "Login fehlgeschlagen.",
            ["Login_DemoRunning"] = "Demo-Login wird durchgeführt...",

            // Main
            ["Main_Greeting"] = "Hallo, {0}",
            ["Main_Stats"] = "Statistik",
            ["Main_AllUsers"] = "Alle User: {0}",
            ["Main_MyFriends"] = "Deine Freunde: {0}",
            ["Main_AppUses"] = "App-Uses: {0}",
            ["Main_AppLoad"] = "App-Auslastung: {0}",
            ["Main_Chats"] = "Chats",
            ["Main_Friends"] = "Freunde",
            ["Main_Profile"] = "Profil",

            // Friends
            ["Friends_Title"] = "Freunde verwalten",
            ["Friends_Search"] = "Freunde suchen",
            ["Friends_Placeholder"] = "Username oder Key eingeben",
            ["Friends_SearchBtn"] = "Suchen",
            ["Friends_ShowAll"] = "Alle anzeigen",
            ["Friends_Results"] = "Suchergebnisse:",
            ["Friends_SendRequest"] = "Anfrage senden",
            ["Friends_RequestSent"] = "Anfrage gesendet",
            ["Friends_MyFriends"] = "Meine Freunde:",
            ["Friends_Load"] = "Freunde laden",
            ["Friends_Chat"] = "Chat",
            ["Friends_Requests"] = "Offene Anfragen:",
            ["Friends_LoadRequests"] = "Anfragen laden",
            ["Friends_PendingTitle"] = "Ausstehende Freundschaftsanfragen:",
            ["Friends_Accept"] = "Annehmen",
            ["Friends_Decline"] = "Ablehnen",
            ["Friends_NotLoggedIn"] = "Bitte zuerst anmelden!",
            ["Friends_NoResults"] = "Keine Benutzer gefunden.",
            ["Friends_LoggedInAs"] = "Eingeloggt als: {0} (ID: {1})",

            // Profile
            ["Profile_Title"] = "Mein Profil",
            ["Profile_ChangePic"] = "Profilbild ändern",
            ["Profile_Username"] = "Benutzername:",
            ["Profile_Email"] = "E-Mail:",
            ["Profile_SaveProfile"] = "Profil speichern",
            ["Profile_Bio"] = "Bio:",
            ["Profile_BioPlaceholder"] = "Erzähle etwas über dich...",
            ["Profile_SaveBio"] = "Bio speichern",
            ["Profile_Logout"] = "Logout",
            ["Profile_NotLoggedIn"] = "Nicht eingeloggt!",

            // Registration
            ["Reg_Title"] = "Neuen Benutzer registrieren",
            ["Reg_Avatar"] = "Dein Avatar wird automatisch generiert",
            ["Reg_Username"] = "Benutzername",
            ["Reg_Email"] = "Email",
            ["Reg_Password"] = "Passwort",
            ["Reg_PasswordRepeat"] = "Passwort wiederholen",
            ["Reg_Button"] = "Registrieren",
            ["Reg_BackToLogin"] = "Zurück zum Login",

            // Settings
            ["Settings_Title"] = "Einstellungen",
            ["Settings_Language"] = "Sprache:",
            ["Settings_PickLanguage"] = "Wähle Sprache",

            // Chat
            ["Chat_Placeholder"] = "Nachricht eingeben...",
            ["Chat_With"] = "Chat mit {0}",

            // Common
            ["Common_Error"] = "Fehler: {0}",
            ["Common_Guest"] = "Gast",
        };

        private static readonly Dictionary<string, string> EnStrings = new()
        {
            // Shell / Navigation
            ["Nav_Login"] = "Sign In",
            ["Nav_Home"] = "Home",
            ["Nav_Friends"] = "Friends",
            ["Nav_Profile"] = "Profile",
            ["Nav_Register"] = "Register",
            ["Nav_Settings"] = "Settings",

            // Login
            ["Login_Title"] = "Sign In",
            ["Login_Username"] = "Username",
            ["Login_Password"] = "Password",
            ["Login_Button"] = "Sign In",
            ["Login_DemoLogin"] = "Demo Login:",
            ["Login_Register"] = "Don't have an account? Register",
            ["Login_ErrorUsername"] = "Please enter a username.",
            ["Login_ErrorPassword"] = "Please enter a password.",
            ["Login_Success"] = "Login successful!",
            ["Login_Failed"] = "Login failed.",
            ["Login_DemoRunning"] = "Demo login running...",

            // Main
            ["Main_Greeting"] = "Hello, {0}",
            ["Main_Stats"] = "Statistics",
            ["Main_AllUsers"] = "All Users: {0}",
            ["Main_MyFriends"] = "Your Friends: {0}",
            ["Main_AppUses"] = "App Uses: {0}",
            ["Main_AppLoad"] = "App Load: {0}",
            ["Main_Chats"] = "Chats",
            ["Main_Friends"] = "Friends",
            ["Main_Profile"] = "Profile",

            // Friends
            ["Friends_Title"] = "Manage Friends",
            ["Friends_Search"] = "Search Friends",
            ["Friends_Placeholder"] = "Enter username or key",
            ["Friends_SearchBtn"] = "Search",
            ["Friends_ShowAll"] = "Show All",
            ["Friends_Results"] = "Search Results:",
            ["Friends_SendRequest"] = "Send Request",
            ["Friends_RequestSent"] = "Request Sent",
            ["Friends_MyFriends"] = "My Friends:",
            ["Friends_Load"] = "Load Friends",
            ["Friends_Chat"] = "Chat",
            ["Friends_Requests"] = "Pending Requests:",
            ["Friends_LoadRequests"] = "Load Requests",
            ["Friends_PendingTitle"] = "Pending Friend Requests:",
            ["Friends_Accept"] = "Accept",
            ["Friends_Decline"] = "Decline",
            ["Friends_NotLoggedIn"] = "Please sign in first!",
            ["Friends_NoResults"] = "No users found.",
            ["Friends_LoggedInAs"] = "Logged in as: {0} (ID: {1})",

            // Profile
            ["Profile_Title"] = "My Profile",
            ["Profile_ChangePic"] = "Change Profile Picture",
            ["Profile_Username"] = "Username:",
            ["Profile_Email"] = "Email:",
            ["Profile_SaveProfile"] = "Save Profile",
            ["Profile_Bio"] = "Bio:",
            ["Profile_BioPlaceholder"] = "Tell us about yourself...",
            ["Profile_SaveBio"] = "Save Bio",
            ["Profile_Logout"] = "Logout",
            ["Profile_NotLoggedIn"] = "Not logged in!",

            // Registration
            ["Reg_Title"] = "Register New User",
            ["Reg_Avatar"] = "Your avatar is generated automatically",
            ["Reg_Username"] = "Username",
            ["Reg_Email"] = "Email",
            ["Reg_Password"] = "Password",
            ["Reg_PasswordRepeat"] = "Repeat Password",
            ["Reg_Button"] = "Register",
            ["Reg_BackToLogin"] = "Back to Login",

            // Settings
            ["Settings_Title"] = "Settings",
            ["Settings_Language"] = "Language:",
            ["Settings_PickLanguage"] = "Choose Language",

            // Chat
            ["Chat_Placeholder"] = "Enter message...",
            ["Chat_With"] = "Chat with {0}",

            // Common
            ["Common_Error"] = "Error: {0}",
            ["Common_Guest"] = "Guest",
        };
    }
}
