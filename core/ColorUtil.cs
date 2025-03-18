namespace SpotPriceApp.core
{
    class ColorUtil
    {
        public static Color GetColor(int cents)
        {
            Color color;

            if (cents < 25)
            {
                color = Color.LimeGreen;
            }
            else if (cents < 50)
            {
                color = Color.Yellow;
            }
            else
            {
                color = Color.Red;
            }
            return color;
        }
    }
}
