using Messanger.ViewModels;

namespace Messanger.Views
{
    public partial class RequestsPage : ContentPage
    {
        public RequestsPage()
        {
            InitializeComponent();
            System.Diagnostics.Debug.WriteLine("[RequestsPage] Constructor");
            System.Diagnostics.Debug.WriteLine($"[RequestsPage] BindingContext Type: {BindingContext?.GetType().Name ?? "NULL"}");
        }

        private void OnSendRequestClicked(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("[RequestsPage] OnSendRequestClicked - Button wurde geklickt!");
            System.Diagnostics.Debug.WriteLine($"[RequestsPage] BindingContext: {BindingContext?.GetType().Name ?? "NULL"}");
            
            if (BindingContext is RequestsViewModel vm)
            {
                System.Diagnostics.Debug.WriteLine($"[RequestsPage] SelectedFriend: {vm.SelectedFriend?.Username ?? "NULL"}");
                System.Diagnostics.Debug.WriteLine($"[RequestsPage] RequestMessage: {vm.RequestMessage}");
                System.Diagnostics.Debug.WriteLine($"[RequestsPage] RequestType: {vm.RequestType}");
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            System.Diagnostics.Debug.WriteLine("[RequestsPage] OnAppearing");

            if (BindingContext is RequestsViewModel viewModel)
            {
                System.Diagnostics.Debug.WriteLine("[RequestsPage] ViewModel gefunden, lade Daten...");
                await viewModel.LoadFriendsAsync();
                await viewModel.LoadPendingRequestsAsync();
                await viewModel.LoadSentRequestsAsync();
                await viewModel.LoadHistoryAsync();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("[RequestsPage] FEHLER: ViewModel ist NULL!");
            }
        }
    }
}
