namespace SpotPriceApp.core
{
    class ColorUtil
    {
        public static Color GetColor(int Cents)
        {
            Color Color;

            if (Cents < 25)
            {
                Color = Color.LimeGreen;
            }
            else if (Cents < 50)
            {
                Color = Color.Yellow;
            }
            else
            {
                Color = Color.Red;
            }
            return Color;
        }
    }
}
