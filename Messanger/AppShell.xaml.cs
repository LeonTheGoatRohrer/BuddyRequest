using Messanger.Views;
using Messanger.Services;

namespace Messanger
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Nur Seiten registrieren die NICHT in der Shell-Hierarchie sind
            Routing.RegisterRoute("ChatPage", typeof(ChatPage));
            
            // Zur LoginPage navigieren, wenn nicht eingeloggt
            if (!UserSession.IsLoggedIn)
            {
                GoToAsync("///LoginPage");
            }
        }
    }
}
