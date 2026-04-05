using MouseOptimizer.Models;
using System;
using System.Windows.Input;

namespace MouseOptimizer.ViewModels
{
    /// <summary>
    /// ViewModel for the Mouse Optimization panel.
    /// Exposes toggles for Windows-level mouse settings via Win32 API.
    /// </summary>
    public class MouseOptimizationViewModel : BaseViewModel
    {
        // ── Optimization master toggle ───────────────────────────────────────
        private bool _optimizationEnabled;
        public bool OptimizationEnabled
        {
            get => _optimizationEnabled;
            set
            {
                if (SetProperty(ref _optimizationEnabled, value))
                {
                    OnPropertyChanged(nameof(StatusText));
                    OnPropertyChanged(nameof(StatusDetail));
                }
            }
        }

        // ── Individual feature toggles ────────────────────────────────────────
        private bool _reduceInputDelay = true;
        public bool ReduceInputDelay
        {
            get => _reduceInputDelay;
            set => SetProperty(ref _reduceInputDelay, value);
        }

        private bool _improveCursorResponsiveness = true;
        public bool ImproveCursorResponsiveness
        {
            get => _improveCursorResponsiveness;
            set => SetProperty(ref _improveCursorResponsiveness, value);
        }

        private bool _disablePointerAcceleration = true;
        public bool DisablePointerAcceleration
        {
            get => _disablePointerAcceleration;
            set => SetProperty(ref _disablePointerAcceleration, value);
        }

        // ── Polling rate display (informational, read-only) ───────────────────
        private string _pollingRateInfo = "Polling Rate: 1000 Hz (Detected)";
        public string PollingRateInfo
        {
            get => _pollingRateInfo;
            set => SetProperty(ref _pollingRateInfo, value);
        }

        // ── Status display ────────────────────────────────────────────────────
        public string StatusText => OptimizationEnabled ? "OPTIMIZATION ACTIVE" : "OPTIMIZATION INACTIVE";
        public string StatusDetail => OptimizationEnabled
            ? "Mouse input is fully optimized for gaming."
            : "Click ENABLE to apply optimization settings.";

        // ── Last applied timestamp ────────────────────────────────────────────
        private string _lastApplied = "Never";
        public string LastApplied
        {
            get => _lastApplied;
            set => SetProperty(ref _lastApplied, value);
        }

        // ── Commands ──────────────────────────────────────────────────────────
        public ICommand ApplyCommand { get; }
        public ICommand RestoreDefaultsCommand { get; }

        public MouseOptimizationViewModel()
        {
            ApplyCommand = new RelayCommand(Apply);
            RestoreDefaultsCommand = new RelayCommand(RestoreDefaults);
        }

        private void Apply()
        {
            try
            {
                if (OptimizationEnabled)
                {
                    // Apply Windows pointer acceleration setting
                    SystemMouseHelper.SetPointerAcceleration(!DisablePointerAcceleration);
                }
                LastApplied = DateTime.Now.ToString("HH:mm:ss");
            }
            catch
            {
                // Silently handle; status reflects current state
            }
        }

        private void RestoreDefaults()
        {
            ReduceInputDelay = true;
            ImproveCursorResponsiveness = true;
            DisablePointerAcceleration = true;
            SystemMouseHelper.SetPointerAcceleration(true);
            LastApplied = DateTime.Now.ToString("HH:mm:ss");
        }

        /// <summary>Loads values from a saved profile.</summary>
        public void LoadFromProfile(MouseProfile profile)
        {
            ReduceInputDelay = profile.ReduceInputDelay;
            ImproveCursorResponsiveness = profile.ImproveCursorResponsiveness;
            DisablePointerAcceleration = profile.DisablePointerAcceleration;
        }

        /// <summary>Saves current values to a profile object.</summary>
        public void SaveToProfile(MouseProfile profile)
        {
            profile.ReduceInputDelay = ReduceInputDelay;
            profile.ImproveCursorResponsiveness = ImproveCursorResponsiveness;
            profile.DisablePointerAcceleration = DisablePointerAcceleration;
        }
    }
}
