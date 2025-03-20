﻿using System.Windows;
using SpotPriceApp.core;
using SpotPriceApp.model;

namespace SpotPriceApp
{
    public partial class MainWindow : Window
    {
        private readonly NotifyIcon _NotifyIcon = new();

        public MainWindow()
        {
            List<SpotPriceReading>? _readings = SpotPriceFetcher.FetchPrices();
            SpotPriceFetcher.InitUpdate(30, _readings, (Content) =>
            {
                PriceLabel.Content = Content.LabelPrice;
                Color Color = Content.Color;
                PriceLabel.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(Color.A, Color.R, Color.G, Color.B));
                UpdateTrayIcon(Content);
            });

            InitializeComponent();

            _NotifyIcon.Visible = true;
            _NotifyIcon.DoubleClick += 
                delegate(object? sender, EventArgs args)
                {
                    Show();
                    WindowState = WindowState.Normal;
                };

            HorizontalListBox.ItemsSource = _readings;
            HorizontalListBox.ScrollIntoView(_readings.Where(reading => reading.Time.Hour == DateTime.Now.Hour).ToList().FirstOrDefault());

            Hide();
        }

        public void UpdateTrayIcon(LabelContent Content)
        {
            Bitmap Bitmap = new(16, 16);
            Graphics Graphics = Graphics.FromImage(Bitmap);
            Font Font = new("Arial", 6, System.Drawing.FontStyle.Bold);

            Brush Brush = new SolidBrush(Content.Color);

            Graphics.DrawString(Content.IconPrice, Font, Brush, 0, 0);

            Icon Icon = System.Drawing.Icon.FromHandle(Bitmap.GetHicon());

            _NotifyIcon.Icon = Icon;
            _NotifyIcon.Text = "Today's prices:\n\nMin: " 
                + Content.Min.ToString("0.00") 
                + " c/kWh\nMax: "
                + Content.Max.ToString("0.00") 
                + " c/kWh\nAvg: "
                + Content.Avg.ToString("0.00")
                + " c/kWh";
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