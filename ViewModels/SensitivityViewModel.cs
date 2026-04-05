using MouseOptimizer.Models;
using System;
using System.Windows.Input;

namespace MouseOptimizer.ViewModels
{
    /// <summary>
    /// ViewModel for the Sensitivity Customization panel.
    /// </summary>
    public class SensitivityViewModel : BaseViewModel
    {
        // ── Main sensitivity (0.1–10.0) ───────────────────────────────────────
        private double _sensitivity = 5.0;
        public double Sensitivity
        {
            get => _sensitivity;
            set => SetProperty(ref _sensitivity, Math.Round(Math.Clamp(value, 0.1, 10.0), 1));
        }

        // ── H/V sensitivity multipliers (0.1–3.0) ────────────────────────────
        private double _horizontalSensitivity = 1.0;
        public double HorizontalSensitivity
        {
            get => _horizontalSensitivity;
            set => SetProperty(ref _horizontalSensitivity, Math.Round(Math.Clamp(value, 0.1, 3.0), 2));
        }

        private double _verticalSensitivity = 1.0;
        public double VerticalSensitivity
        {
            get => _verticalSensitivity;
            set => SetProperty(ref _verticalSensitivity, Math.Round(Math.Clamp(value, 0.1, 3.0), 2));
        }

        // ── ADS (Aim Down Sights) sensitivity multiplier (0.1–2.0) ───────────
        private double _adsSensitivity = 0.7;
        public double AdsSensitivity
        {
            get => _adsSensitivity;
            set => SetProperty(ref _adsSensitivity, Math.Round(Math.Clamp(value, 0.1, 2.0), 2));
        }

        // ── DPI input (display/preset, not hardware-written) ──────────────────
        private int _dpi = 800;
        public int Dpi
        {
            get => _dpi;
            set => SetProperty(ref _dpi, Math.Clamp(value, 100, 25600));
        }

        // ── Status messages ───────────────────────────────────────────────────
        private string _statusMessage = "Settings not applied yet";
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        // ── Commands ──────────────────────────────────────────────────────────
        public ICommand SetDefaultCommand { get; }
        public ICommand ApplyCommand { get; }

        public SensitivityViewModel()
        {
            SetDefaultCommand = new RelayCommand(SetDefault);
            ApplyCommand      = new RelayCommand(Apply);
        }

        private void SetDefault()
        {
            Sensitivity           = 5.0;
            HorizontalSensitivity = 1.0;
            VerticalSensitivity   = 1.0;
            AdsSensitivity        = 0.7;
            Dpi                   = 800;
            StatusMessage         = "Defaults restored";
        }

        private void Apply()
        {
            try
            {
                // Apply Windows-level mouse speed (maps 0.1–10 → 1–20)
                SystemMouseHelper.SetMouseSpeed(Sensitivity);
                StatusMessage = $"Applied at {DateTime.Now:HH:mm:ss}";
            }
            catch
            {
                StatusMessage = "Could not apply — try running as Administrator";
            }
        }

        public void LoadFromProfile(MouseProfile profile)
        {
            Sensitivity           = profile.Sensitivity;
            HorizontalSensitivity = profile.HorizontalSensitivity;
            VerticalSensitivity   = profile.VerticalSensitivity;
            AdsSensitivity        = profile.AdsSensitivity;
            Dpi                   = profile.Dpi;
        }

        public void SaveToProfile(MouseProfile profile)
        {
            profile.Sensitivity           = Sensitivity;
            profile.HorizontalSensitivity = HorizontalSensitivity;
            profile.VerticalSensitivity   = VerticalSensitivity;
            profile.AdsSensitivity        = AdsSensitivity;
            profile.Dpi                   = Dpi;
        }
    }
}
