using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SpotPriceApp.core
{
    public class ColorConverter : IValueConverter
    {
        public Color GreenColor { get; set; }
        public Color YellowColor { get; set; }
        public Color RedColor { get; set; }

        public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            if (Value == null) 
            { 
                return Color.Black.Name;
            }

            Color color = ColorUtil.GetColor(int.Parse(Value.ToString()!.Trim().Split("\n")[1].Split(",")[0]));
            
            if (color == Color.LimeGreen) 
            {
                return GreenColor.Name;
            }
            else if (color == Color.Yellow)
            {
                return YellowColor.Name;
            }
            else if (color == Color.Red)
            {
                return RedColor.Name;
            }
            return Color.Black.Name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
