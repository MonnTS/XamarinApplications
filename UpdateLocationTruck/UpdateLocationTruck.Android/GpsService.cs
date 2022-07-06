using System;
using Android.OS;
using System.Text;
using Android.App;
using Android.Util;
using Xamarin.Forms;
using Android.Content;
using System.Net.Http;
using Xamarin.Essentials;
using System.Threading.Tasks;

namespace UpdateLocationTruck.Droid
{
    [Service(Label = "GPsService")]
    public class GpsService : Service
    {
        private bool _isRunningTimer = true;

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override void OnCreate()
        {
            base.OnCreate();
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            Device.StartTimer(TimeSpan.FromSeconds(5), () =>
            {
                GetLocation();
                return _isRunningTimer;
            });

            return StartCommandResult.Sticky;
        }

        public override void OnDestroy()
        {
            StopSelf();
            _isRunningTimer = false;
            base.OnDestroy();
        }

        public async void GetLocation()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.High);
                var location = await Geolocation.GetLocationAsync(request);

                if (location != null)
                {
                    MessagingCenter.Send<Location>(location, "locationValue");
                    await UpdateTWXLocationAsync(location);
                }
            }
            catch (Exception ex)
            {
                // ignored
            }
        }

        public async Task UpdateTWXLocationAsync(Location location)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("appkey", "1931e71b-7300-4e90-871d-98dcd5d64873");
            httpClient.DefaultRequestHeaders.Add("x-thingworx-session", "true");
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            const string thingName = "Name...";
            const string url = "Url..." + thingName + "/Properties/location";

            var jsonBody = @"{""location"": {""latitude"":" +
                              location.Latitude.ToString().Replace(",", ".") + @",""longitude"": " +
                              location.Longitude.ToString().Replace(",", ".") + @",""elevation"": 0.0}}";

            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            var response = await httpClient.PutAsync(url, content);

            Log.Debug("GPS", await response.Content.ReadAsStringAsync());
        }
    }
}
