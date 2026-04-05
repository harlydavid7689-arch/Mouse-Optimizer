using System;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace MouseOptimizer.Views
{
    /// <summary>
    /// Splash screen — displays for ~2s with an animated progress bar,
    /// then launches MainWindow.
    /// </summary>
    public partial class SplashScreen : Window
    {
        private readonly DispatcherTimer _timer = new();
        private int _step;

        private static readonly string[] Steps =
        {
            "Checking system...", "Loading profiles...",
            "Applying settings...", "Starting UI..."
        };

        public SplashScreen()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Animate the loading bar width from 0 → 260
            var anim = new DoubleAnimation(0, 260, TimeSpan.FromSeconds(1.8))
            {
                EasingFunction = new QuarticEase { EasingMode = EasingMode.EaseInOut }
            };
            LoadBar.BeginAnimation(WidthProperty, anim);

            // Cycle status labels
            _timer.Interval = TimeSpan.FromMilliseconds(450);
            _timer.Tick += (_, _) =>
            {
                if (_step < Steps.Length)
                    LoadingLabel.Text = Steps[_step++];
                else
                {
                    _timer.Stop();
                    OpenMainWindow();
                }
            };
            _timer.Start();
        }

        private void OpenMainWindow()
        {
            var main = new MainWindow();
            Application.Current.MainWindow = main;
            main.Show();
            Close();
        }
    }
}
