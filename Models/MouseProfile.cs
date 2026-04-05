using System.Collections.Generic;

namespace MouseOptimizer.Models
{
    /// <summary>
    /// Represents a complete mouse configuration profile.
    /// </summary>
    public class MouseProfile
    {
        public string Name { get; set; } = "Default";
        public string Icon { get; set; } = "🎯";
        public double Sensitivity { get; set; } = 5.0;
        public int SmoothnessLevel { get; set; } = 50;
        public string SmoothnessPreset { get; set; } = "Balanced";
        public int Dpi { get; set; } = 800;
        public double HorizontalSensitivity { get; set; } = 1.0;
        public double VerticalSensitivity { get; set; } = 1.0;
        public double AdsSensitivity { get; set; } = 0.7;
        public bool ReduceInputDelay { get; set; } = true;
        public bool ImproveCursorResponsiveness { get; set; } = true;
        public bool DisablePointerAcceleration { get; set; } = true;
        public string Description { get; set; } = "";
    }

    /// <summary>
    /// Root container for all saved profiles, persisted to JSON.
    /// </summary>
    public class ProfileStore
    {
        public List<MouseProfile> Profiles { get; set; } = new();
        public string ActiveProfileName { get; set; } = "Default";
    }
}
