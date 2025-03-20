namespace SpotPriceApp.model
{
    internal class SpotPriceReading
    {
        public required DateTimeOffset Time { get; set; }
        public required float Value { get; set; }

        public override string ToString()
        {
            return PadBoth(Time.Hour.ToString("D2"), 6) + "\n" + Value.ToString("0.00") + " c";
        }

        private static string PadBoth(string Source, int Length)
        {
            int Spaces = Length - Source.Length;
            int PadLeft = Spaces / 2 + Source.Length;
            return Source.PadLeft(PadLeft).PadRight(Length);
        }
    }
}
