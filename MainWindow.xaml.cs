using System.Windows;
using SpotPriceApp.core;
using SpotPriceApp.model;

namespace SpotPriceApp
{
    public partial class MainWindow : Window
    {
        private readonly List<SpotPriceReading>? _Readings = null;
        private NotifyIcon _NotifyIcon = new NotifyIcon();

        public MainWindow()
        {
            _Readings = SpotPriceFetcher.FetchPrices();
            SpotPriceFetcher.InitUpdate(30, _Readings, (color, labelPrice, trayIconPrice) =>
            {
                PriceLabel.Content = labelPrice;
                PriceLabel.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
                UpdateTrayIcon(color, trayIconPrice);
            });
            InitializeComponent();

            _NotifyIcon.Visible = true;
            _NotifyIcon.DoubleClick += 
                delegate(object? sender, EventArgs args)
                {
                    Show();
                    WindowState = WindowState.Normal;
                };
            Hide();
        }

        public void UpdateTrayIcon(System.Drawing.Color color, string price)
        {
            Bitmap bitmap = new Bitmap(16, 16);
            Graphics graphics = Graphics.FromImage(bitmap);
            Font font = new Font("Arial", 6, System.Drawing.FontStyle.Bold);

            System.Drawing.Brush brush = new SolidBrush(color);

            graphics.DrawString(price, font, brush, 0, 0);

            Icon icon = System.Drawing.Icon.FromHandle(bitmap.GetHicon());
            _NotifyIcon.Icon = icon;
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
            }
            base.OnStateChanged(e);
        }
    }
}