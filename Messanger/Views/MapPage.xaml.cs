using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Controls;
using Messanger.ViewModels;
using Messanger.Services;

namespace Messanger.Views
{
    public partial class MapPage : ContentPage
    {
        private MapViewModel _viewModel;

        public MapPage()
        {
            InitializeComponent();
            _viewModel = new MapViewModel();
            BindingContext = _viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            
            // Hole UserId aus UserSession
            var userId = UserSession.CurrentUserId;
            if (userId > 0)
            {
                _viewModel.Initialize(userId);
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _viewModel.IsSharing = false;
        }
    }
}
