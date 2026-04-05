using System.Windows.Input;

namespace MouseOptimizer.ViewModels
{
    /// <summary>
    /// ViewModel for the Mouse Smoothness Settings panel.
    /// Manages the smoothness slider and preset selection.
    /// </summary>
    public class SmoothnessViewModel : BaseViewModel
    {
        // ── Smoothness level (0–100) ──────────────────────────────────────────
        private int _smoothnessLevel = 50;
        public int SmoothnessLevel
        {
            get => _smoothnessLevel;
            set
            {
                if (SetProperty(ref _smoothnessLevel, value))
                    OnPropertyChanged(nameof(SmoothnessDescription));
            }
        }

        // ── Preset names ──────────────────────────────────────────────────────
        private string _selectedPreset = "Balanced";
        public string SelectedPreset
        {
            get => _selectedPreset;
            set
            {
                if (SetProperty(ref _selectedPreset, value))
                    OnPropertyChanged(nameof(PresetDescription));
            }
        }

        public string[] Presets { get; } = { "Low", "Balanced", "High", "Competitive" };

        // ── Computed descriptions ─────────────────────────────────────────────
        public string SmoothnessDescription => _smoothnessLevel switch
        {
            <= 25 => "Raw & Direct — No smoothing applied",
            <= 50 => "Light Smoothing — Minimal curve applied",
            <= 75 => "Moderate Smoothing — Natural movement curve",
            _      => "High Smoothing — Fluid but slightly delayed"
        };

        public string PresetDescription => _selectedPreset switch
        {
            "Low"         => "Raw input — perfect for high-DPI precision",
            "Balanced"    => "Everyday use — comfortable and responsive",
            "High"        => "Smooth cursor — ideal for creative tasks",
            "Competitive" => "Zero smoothing — pure input, maximum reaction speed",
            _             => ""
        };

        // ── Commands ──────────────────────────────────────────────────────────
        public ICommand SetPresetCommand { get; }

        public SmoothnessViewModel()
        {
            SetPresetCommand = new RelayCommand(param =>
            {
                if (param is string preset) ApplyPreset(preset);
            });
        }

        private void ApplyPreset(string preset)
        {
            SelectedPreset = preset;
            SmoothnessLevel = preset switch
            {
                "Low"         => 10,
                "Balanced"    => 50,
                "High"        => 80,
                "Competitive" => 0,
                _             => 50
            };
        }

        public void LoadFromProfile(Models.MouseProfile profile)
        {
            SmoothnessLevel = profile.SmoothnessLevel;
            SelectedPreset  = profile.SmoothnessPreset;
        }

        public void SaveToProfile(Models.MouseProfile profile)
        {
            profile.SmoothnessLevel  = SmoothnessLevel;
            profile.SmoothnessPreset = SelectedPreset;
        }
    }
}
