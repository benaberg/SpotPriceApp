namespace SpotPriceApp.model
{
    internal class SpotPriceReading
    {
        public required DateTimeOffset time { get; set; }
        public required float value { get; set; }

        public override string ToString()
        {
            return PadBoth(time.Hour.ToString("D2"), 6) + "\n" + value.ToString("0.00") + " c";
        }

        private string PadBoth(string source, int length)
        {
            int spaces = length - source.Length;
            int padLeft = spaces / 2 + source.Length;
            return source.PadLeft(padLeft).PadRight(length);
        }
    }
}
