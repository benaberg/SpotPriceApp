namespace SpotPriceApp.model
{
    internal class SpotPriceReading
    {
        public required DateTimeOffset time { get; set; }
        public required float value { get; set; }
    }
}
