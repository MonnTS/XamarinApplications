using System;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace UpdateLocationTruck
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            MessagingCenter.Unsubscribe<Location>(this, "locationValue");
            MessagingCenter.Subscribe<Location>(this, "locationValue", value =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    latitude.Text = value.Latitude.ToString();
                    longitude.Text = value.Longitude.ToString();
                });
            });
        }

        private void StartService(object sender, EventArgs e)
        {
            MessagingCenter.Send("1", "myGpsService");
        }

        private void StopService(object sender, EventArgs e)
        {
            MessagingCenter.Send("0", "myGpsService");
        }
    }
}
