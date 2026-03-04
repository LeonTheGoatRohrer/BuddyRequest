using Messanger.Views;

namespace Messanger
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Nur Seiten registrieren die NICHT in der Shell-Hierarchie sind
            Routing.RegisterRoute("ChatPage", typeof(ChatPage));
        }
    }
}
