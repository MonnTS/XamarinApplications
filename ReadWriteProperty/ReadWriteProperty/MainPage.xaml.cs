using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;

namespace ReadWriteProperty
{
    public partial class MainPage : ContentPage
    {
        private string 
            _url = "https://pswbp.percallgroup.com/Thingworx/Things/PSW.DZ20319.Thing.MojSensor01/Properties/";

        private string
            _appkey = "1931e71b-7300-4e90-871d-98dcd5d64873";
        
        private const string Status = "true";

        private const string Json = "application/json";
        

        public MainPage()
        {
            InitializeComponent();
            EntryConnection.Text = _url;
            ApplicationKey.Text = _appkey;
        }

        private async void btn_ClickedRead(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(VerName.Text))
            {
                await DisplayAlert("Warning", "Provide data for property", "OK");
            }
            else
            {
                try
                {
                    var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Add("x-thingworx-session", Status);
                    httpClient.DefaultRequestHeaders.Add("appkey", _appkey);
                    httpClient.DefaultRequestHeaders.Add("Accept", Json);
                    var response = await httpClient.GetAsync(_url);

                    try
                    {
                        if (!response.IsSuccessStatusCode)
                        {
                            await DisplayAlert("Error", $"Cannot send a request. Code status is {response.StatusCode}", "OK");
                        }
                        
                        var result = await response.Content.ReadAsStringAsync();
                        var jObject = JObject.Parse(result);
                        var propertyName = VerName.Text;
                        ValLab.Text = jObject["rows"][0][propertyName].ToString();
                    }

                    catch (HttpRequestException)
                    {
                        await DisplayAlert("Error", "Cannot send a request.", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", ex.Message, "OK");
                }
            }
        }

        private async void btn_ClickedUpdate(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(EntryUpdate.Text))
            {
                await DisplayAlert("Warning", "Provide data to a new value", "OK");
                return;
            }

            try
            {
                var jsonFile = new JsonData();
                var propertyName = VerName.Text;
                var updatedValue = EntryUpdate.Text;
                jsonFile.JsonObject.Add(new JObject());
                jsonFile.JsonObject[0][propertyName] = updatedValue;
                var jsond = JsonConvert.SerializeObject(jsonFile.JsonObject[0]);
                
                var httpWebRequest = (HttpWebRequest) WebRequest.Create(_url + propertyName + "/");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "PUT";
                httpWebRequest.Headers.Add("appkey", _appkey);

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    await streamWriter.WriteAsync(jsond);
                }
    
                var httpResponse = (HttpWebResponse) httpWebRequest.GetResponse();
                
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var responseText = await streamReader.ReadToEndAsync();
                }
            }

            catch (Exception ex)
            {
                await DisplayAlert("Warning", ex.Message, "OK");
            }
        }
    }
}
