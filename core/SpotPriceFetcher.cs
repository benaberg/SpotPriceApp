using Newtonsoft.Json;
using SpotPriceApp.model;
using System.Net.Http;

namespace SpotPriceApp.core
{
    internal static class SpotPriceFetcher
    {
        public static List<SpotPriceReading> FetchPrices()
        {
            HttpClientHandler handler = new HttpClientHandler
            {
                AutomaticDecompression = System.Net.DecompressionMethods.All
            };

            HttpClient client = new HttpClient(handler);

            using (client)
            {
                System.Diagnostics.Debug.WriteLine("Fetching API...");
                client.BaseAddress = new Uri(ApplicationResource.SpotPrice_API_BaseAddress);
                HttpResponseMessage response = client.GetAsync(ApplicationResource.SpotPrice_API_RequestUri).Result;
                response.EnsureSuccessStatusCode();
                return JsonConvert.DeserializeObject<List<SpotPriceReading>>(response.Content.ReadAsStringAsync().Result);
            }
        }

        public static async void InitUpdate(int seconds, List<SpotPriceReading> readings, Action<Color, string, string> labelAction)
        {
            if (seconds <= 0 || readings == null) { return; }

            var periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(seconds));
            while (await periodicTimer.WaitForNextTickAsync())
            {
                UpdateLabel(readings, labelAction);
            }
        }

        private static void UpdateLabel(List<SpotPriceReading> readings, Action<Color, string, string> labelAction)
        {
            if (DateTime.Now.Hour == 0 && DateTime.Now.Minute == 00)
            {
                readings = FetchPrices();
                System.Diagnostics.Debug.WriteLine("Readings updated!");
            }
            readings.ForEach(reading =>
            {
                if (reading.time.Hour == DateTime.Now.Hour)
                {
                    string price = reading.value.ToString("0.00");

                    string[] priceSplit = price.Split(",");
                    string iconPrice = priceSplit[0] + "\n." + priceSplit[1];

                    Color color = ColorUtil.GetColor(int.Parse(priceSplit[0]));

                    labelAction.Invoke(color, price + " c/kWh", iconPrice);
                    return;
                }
            });
        }
    }
}
