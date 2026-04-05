namespace MouseOptimizer.Models
{
    /// <summary>
    /// Persistent application-level settings, saved to JSON.
    /// </summary>
    public class AppSettings
    {
        public bool OptimizationEnabled { get; set; } = false;
        public bool MinimizeToTray { get; set; } = false;
        public bool StartWithWindows { get; set; } = false;
        public bool ShowSplashScreen { get; set; } = true;
        public string LastActiveProfile { get; set; } = "Default";
        public string Theme { get; set; } = "Dark";
    }
}
