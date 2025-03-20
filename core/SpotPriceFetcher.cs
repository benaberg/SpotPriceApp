using Newtonsoft.Json;
using SpotPriceApp.model;
using System.Net.Http;

namespace SpotPriceApp.core
{
    internal static class SpotPriceFetcher
    {
        public static List<SpotPriceReading> FetchPrices()
        {
            List<SpotPriceReading>? _readings = null;
            HttpClientHandler Handler = new()
            {
                AutomaticDecompression = System.Net.DecompressionMethods.All
            };
            HttpClient Client = new(Handler)
            {
                BaseAddress = new Uri(ApplicationResource.SpotPrice_API_BaseAddress)
            };
            using (Client)
            {
                int RetryInterval = int.Parse(ApplicationResource.SpotPrice_HTTP_RetryInterval);
                do
                {
                    try
                    {
                        _readings = PerformFetch(Client);
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("Request failed! Reason: " + e.Message);
                        System.Diagnostics.Debug.WriteLine("Retrying in " + RetryInterval + " seconds...");
                        Thread.Sleep(RetryInterval * 1000);
                    }
                }
                while (_readings == null);
            }
            return _readings;
        }

        private static List<SpotPriceReading> PerformFetch(HttpClient Client)
        {
            System.Diagnostics.Debug.WriteLine("Fetching API...");
            HttpResponseMessage Response = Client.GetAsync(ApplicationResource.SpotPrice_API_RequestPath).Result;
            Response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<List<SpotPriceReading>>(Response.Content.ReadAsStringAsync().Result);
        }

        public static async void InitUpdate(int Seconds, List<SpotPriceReading>? _readings, Action<LabelContent> LabelAction)
        {
            if (Seconds <= 0 || _readings == null) 
            { 
                return; 
            }
            var PeriodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(Seconds));
            while (await PeriodicTimer.WaitForNextTickAsync())
            {
                UpdateLabel(_readings, LabelAction);
            }
        }

        private static void UpdateLabel(List<SpotPriceReading> Readings, Action<LabelContent> LabelAction)
        {
            if (DateTime.Now.Hour == 0 && DateTime.Now.Minute == 00)
            {
                Readings = FetchPrices();
                System.Diagnostics.Debug.WriteLine("Readings updated!");
            }
            LabelContent Content = new(Readings.Count);
            Readings.ForEach(Reading =>
            {
                if (Reading.Value > Content.Max)
                {
                    Content.Max = Reading.Value;
                    Content.MaxTime = Reading.Time;
                }
                if (Reading.Value < Content.Min)
                {
                    Content.Min = Reading.Value;
                    Content.MinTime = Reading.Time;
                }
                if (Reading.Time.Hour == DateTime.Now.Hour)
                {
                    string PriceString = Reading.Value.ToString("0.00");
                    Content.LabelPrice = PriceString + " c/kWh";
                    string[] PriceSplit = PriceString.Split(",");
                    Content.IconPrice = PriceSplit[0] + "\n." + PriceSplit[1];
                    Content.Color = ColorUtil.GetColor(int.Parse(PriceSplit[0]));
                }
                Content.AddReading(Reading.Value);
            });
            LabelAction.Invoke(Content);
        }
    }
}
