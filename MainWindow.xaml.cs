using MouseOptimizer.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace MouseOptimizer
{
    /// <summary>
    /// Code-behind for the main window.
    /// Wires up the ViewModel and handles window chrome interactions.
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _vm;

        public MainWindow()
        {
            InitializeComponent();
            _vm = new MainViewModel();
            DataContext = _vm;
        }

        // ── Window chrome ─────────────────────────────────────────────────────

        /// <summary>Allows dragging the custom title bar.</summary>
        private void TopBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
            => WindowState = WindowState.Minimized;

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            _vm.SaveSettings();
            _vm.SaveCurrentToProfile();
            Close();
        }

        // ── Quick-action handlers ─────────────────────────────────────────────

        private void QuickEnable_Click(object sender, RoutedEventArgs e)
        {
            _vm.Optimization.OptimizationEnabled = true;
        }

        private void QuickSave_Click(object sender, RoutedEventArgs e)
        {
            _vm.SaveCurrentToProfile();
        }

        private void LoadProfile_Click(object sender, RoutedEventArgs e)
        {
            if (_vm.Profiles.SelectedProfile is { } profile)
            {
                _vm.Optimization.LoadFromProfile(profile);
                _vm.Smoothness.LoadFromProfile(profile);
                _vm.Sensitivity.LoadFromProfile(profile);
            }
        }

        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            _vm.SaveSettings();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            _vm.SaveSettings();
            _vm.SaveCurrentToProfile();
            base.OnClosing(e);
        }
    }
}
