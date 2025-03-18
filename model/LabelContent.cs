namespace SpotPriceApp.model
{
    public class LabelContent(int Count)
    {
        private int pointer = 0;
        public float[] Values { get; private set; } = new float[Count];
        public string LabelPrice { get; set; } = string.Empty;
        public string IconPrice { get; set; } = string.Empty;
        public float Min { get; set; } = float.MaxValue;
        public float Max { get; set; } = float.MinValue;
        public Color Color { get; set; } = Color.LimeGreen;
        public float Avg
        {
            get
            {
                return Values.Average();
            }
        }

        public void AddReading(float reading)
        {
            Values[pointer++] = reading;
        }
    }
}
