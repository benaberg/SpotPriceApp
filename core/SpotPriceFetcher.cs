using Newtonsoft.Json;
using SpotPriceApp.model;
using System.Net.Http;

namespace SpotPriceApp.core
{
    internal static class SpotPriceFetcher
    {
        public static List<SpotPriceReading> FetchPrices()
        {
            HttpClientHandler Handler = new HttpClientHandler
            {
                AutomaticDecompression = System.Net.DecompressionMethods.All
            };

            HttpClient Client = new HttpClient(Handler);

            using (Client)
            {
                System.Diagnostics.Debug.WriteLine("Fetching API...");
                Client.BaseAddress = new Uri(ApplicationResource.SpotPrice_API_BaseAddress);
                HttpResponseMessage Response = Client.GetAsync(ApplicationResource.SpotPrice_API_RequestUri).Result;
                Response.EnsureSuccessStatusCode();
                return JsonConvert.DeserializeObject<List<SpotPriceReading>>(Response.Content.ReadAsStringAsync().Result);
            }
        }

        public static async void InitUpdate(int Seconds, List<SpotPriceReading> Readings, Action<LabelContent> LabelAction)
        {
            if (Seconds <= 0 || Readings == null) { return; }

            var PeriodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(Seconds));
            while (await PeriodicTimer.WaitForNextTickAsync())
            {
                UpdateLabel(Readings, LabelAction);
            }
        }

        private static void UpdateLabel(List<SpotPriceReading> Readings, Action<LabelContent> LabelAction)
        {
            if (DateTime.Now.Hour == 0 && DateTime.Now.Minute == 00)
            {
                Readings = FetchPrices();
                System.Diagnostics.Debug.WriteLine("Readings updated!");
            }
            LabelContent Content = new LabelContent(Readings.Count);
            Readings.ForEach(Reading =>
            {
                Content.Max = Reading.Value > Content.Max ? Reading.Value : Content.Max;
                Content.Min = Reading.Value < Content.Min ? Reading.Value : Content.Min;
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
