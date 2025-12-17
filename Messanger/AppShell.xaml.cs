namespace Messanger
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Optional, aber hilfreich für GoToAsync
            Routing.RegisterRoute(
              nameof(Views.RegistrationPage),
              typeof(Views.RegistrationPage));
        }
    }
}
