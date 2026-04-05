using MouseOptimizer.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows.Input;

namespace MouseOptimizer.ViewModels
{
    /// <summary>
    /// Root ViewModel — owns navigation state and all child ViewModels.
    /// Coordinates profile load/save across panels.
    /// </summary>
    public class MainViewModel : BaseViewModel
    {
        // ── Child ViewModels ──────────────────────────────────────────────────
        public MouseOptimizationViewModel Optimization { get; } = new();
        public SmoothnessViewModel        Smoothness   { get; } = new();
        public SensitivityViewModel       Sensitivity  { get; } = new();
        public ProfilesViewModel          Profiles     { get; } = new();

        // ── Navigation ────────────────────────────────────────────────────────
        private string _currentPage = "Dashboard";
        public string CurrentPage
        {
            get => _currentPage;
            set
            {
                if (SetProperty(ref _currentPage, value))
                {
                    OnPropertyChanged(nameof(IsDashboard));
                    OnPropertyChanged(nameof(IsOptimization));
                    OnPropertyChanged(nameof(IsSmoothness));
                    OnPropertyChanged(nameof(IsSensitivity));
                    OnPropertyChanged(nameof(IsProfiles));
                    OnPropertyChanged(nameof(IsSettings));
                }
            }
        }

        public bool IsDashboard   => CurrentPage == "Dashboard";
        public bool IsOptimization => CurrentPage == "Optimization";
        public bool IsSmoothness  => CurrentPage == "Smoothness";
        public bool IsSensitivity => CurrentPage == "Sensitivity";
        public bool IsProfiles    => CurrentPage == "Profiles";
        public bool IsSettings    => CurrentPage == "Settings";

        // ── Footer status ─────────────────────────────────────────────────────
        public string FooterStatus => Optimization.OptimizationEnabled
            ? "⬤  System Ready — Optimized"
            : "⬤  System Ready — Standard Mode";

        // ── Settings ──────────────────────────────────────────────────────────
        private bool _startWithWindows;
        public bool StartWithWindows
        {
            get => _startWithWindows;
            set => SetProperty(ref _startWithWindows, value);
        }

        private bool _minimizeToTray;
        public bool MinimizeToTray
        {
            get => _minimizeToTray;
            set => SetProperty(ref _minimizeToTray, value);
        }

        // ── Navigation commands ───────────────────────────────────────────────
        public ICommand NavigateCommand  { get; }
        public ICommand ApplyProfileCommand { get; }

        // ── Settings paths ────────────────────────────────────────────────────
        private static readonly string SettingsPath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                         "MouseOptimizer", "settings.json");

        public MainViewModel()
        {
            NavigateCommand     = new RelayCommand(param => { if (param is string p) CurrentPage = p; });
            ApplyProfileCommand = new RelayCommand(ApplySelectedProfile);

            // Wire profile selection to sync all panels
            Profiles.OnProfileSelected = LoadProfileIntoAllPanels;

            LoadSettings();
            ApplySelectedProfile();

            // Re-broadcast footer when optimization state changes
            Optimization.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(MouseOptimizationViewModel.OptimizationEnabled))
                    OnPropertyChanged(nameof(FooterStatus));
            };
        }

        private void ApplySelectedProfile()
        {
            if (Profiles.SelectedProfile is MouseProfile profile)
                LoadProfileIntoAllPanels(profile);
        }

        private void LoadProfileIntoAllPanels(MouseProfile profile)
        {
            Optimization.LoadFromProfile(profile);
            Smoothness.LoadFromProfile(profile);
            Sensitivity.LoadFromProfile(profile);
        }

        /// <summary>Saves all current panel settings back into the active profile and persists.</summary>
        public void SaveCurrentToProfile()
        {
            if (Profiles.SelectedProfile is MouseProfile profile)
            {
                Optimization.SaveToProfile(profile);
                Smoothness.SaveToProfile(profile);
                Sensitivity.SaveToProfile(profile);
                Profiles.SaveAll();
            }
        }

        // ── App settings persist ──────────────────────────────────────────────
        private void LoadSettings()
        {
            try
            {
                if (File.Exists(SettingsPath))
                {
                    var s = JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText(SettingsPath));
                    if (s != null)
                    {
                        StartWithWindows = s.StartWithWindows;
                        MinimizeToTray   = s.MinimizeToTray;
                        Optimization.OptimizationEnabled = s.OptimizationEnabled;
                    }
                }
            }
            catch { /* Use defaults */ }
        }

        public void SaveSettings()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath)!);
                var s = new AppSettings
                {
                    StartWithWindows     = StartWithWindows,
                    MinimizeToTray       = MinimizeToTray,
                    OptimizationEnabled  = Optimization.OptimizationEnabled,
                    LastActiveProfile    = Profiles.SelectedProfile?.Name ?? "Default"
                };
                File.WriteAllText(SettingsPath, JsonConvert.SerializeObject(s, Formatting.Indented));
            }
            catch { /* Non-critical */ }
        }
    }
}
