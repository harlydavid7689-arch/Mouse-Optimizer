using MouseOptimizer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Microsoft.Win32;

namespace MouseOptimizer.ViewModels
{
    /// <summary>
    /// ViewModel for the Profiles panel.
    /// Handles creating, selecting, deleting, exporting, and importing profiles.
    /// </summary>
    public class ProfilesViewModel : BaseViewModel
    {
        private static readonly string ProfilesPath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                         "MouseOptimizer", "profiles.json");

        // ── Profile collection ────────────────────────────────────────────────
        public ObservableCollection<MouseProfile> Profiles { get; } = new();

        private MouseProfile? _selectedProfile;
        public MouseProfile? SelectedProfile
        {
            get => _selectedProfile;
            set => SetProperty(ref _selectedProfile, value);
        }

        // ── New profile name input ────────────────────────────────────────────
        private string _newProfileName = "";
        public string NewProfileName
        {
            get => _newProfileName;
            set => SetProperty(ref _newProfileName, value);
        }

        // ── Status message ────────────────────────────────────────────────────
        private string _statusMessage = "";
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        // ── Commands ──────────────────────────────────────────────────────────
        public ICommand CreateProfileCommand { get; }
        public ICommand DeleteProfileCommand { get; }
        public ICommand ExportProfileCommand { get; }
        public ICommand ImportProfileCommand { get; }
        public ICommand SaveAllCommand { get; }

        // ── External setter injection (from MainViewModel) ────────────────────
        public Action<MouseProfile>? OnProfileSelected { get; set; }

        public ProfilesViewModel()
        {
            CreateProfileCommand = new RelayCommand(CreateProfile, () => !string.IsNullOrWhiteSpace(NewProfileName));
            DeleteProfileCommand = new RelayCommand(DeleteProfile, () => SelectedProfile != null && Profiles.Count > 1);
            ExportProfileCommand = new RelayCommand(ExportProfile, () => SelectedProfile != null);
            ImportProfileCommand = new RelayCommand(ImportProfile);
            SaveAllCommand       = new RelayCommand(SaveAll);

            LoadAll();
        }

        private void CreateProfile()
        {
            var profile = new MouseProfile
            {
                Name        = NewProfileName.Trim(),
                Icon        = "🎮",
                Description = "Custom profile"
            };
            Profiles.Add(profile);
            SelectedProfile = profile;
            NewProfileName  = "";
            SaveAll();
            StatusMessage = $"Profile '{profile.Name}' created";
        }

        private void DeleteProfile()
        {
            if (SelectedProfile is null) return;
            var name = SelectedProfile.Name;
            Profiles.Remove(SelectedProfile);
            SelectedProfile = Profiles.FirstOrDefault();
            SaveAll();
            StatusMessage = $"Profile '{name}' deleted";
        }

        private void ExportProfile()
        {
            if (SelectedProfile is null) return;
            var dlg = new SaveFileDialog
            {
                Filter   = "JSON Profile|*.json",
                FileName = $"{SelectedProfile.Name}.json"
            };
            if (dlg.ShowDialog() == true)
            {
                File.WriteAllText(dlg.FileName, JsonConvert.SerializeObject(SelectedProfile, Formatting.Indented));
                StatusMessage = "Profile exported";
            }
        }

        private void ImportProfile()
        {
            var dlg = new OpenFileDialog { Filter = "JSON Profile|*.json" };
            if (dlg.ShowDialog() == true)
            {
                try
                {
                    var json    = File.ReadAllText(dlg.FileName);
                    var profile = JsonConvert.DeserializeObject<MouseProfile>(json);
                    if (profile != null)
                    {
                        profile.Name = EnsureUniqueName(profile.Name);
                        Profiles.Add(profile);
                        SelectedProfile = profile;
                        SaveAll();
                        StatusMessage = $"Profile '{profile.Name}' imported";
                    }
                }
                catch
                {
                    StatusMessage = "Import failed — invalid file";
                }
            }
        }

        public void SaveAll()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ProfilesPath)!);
                var store = new ProfileStore
                {
                    Profiles          = Profiles.ToList(),
                    ActiveProfileName = SelectedProfile?.Name ?? ""
                };
                File.WriteAllText(ProfilesPath, JsonConvert.SerializeObject(store, Formatting.Indented));
            }
            catch { /* Non-critical */ }
        }

        public void LoadAll()
        {
            Profiles.Clear();
            try
            {
                if (File.Exists(ProfilesPath))
                {
                    var store = JsonConvert.DeserializeObject<ProfileStore>(File.ReadAllText(ProfilesPath));
                    if (store?.Profiles != null)
                    {
                        foreach (var p in store.Profiles) Profiles.Add(p);
                        SelectedProfile = Profiles.FirstOrDefault(x => x.Name == store.ActiveProfileName)
                                          ?? Profiles.FirstOrDefault();
                        return;
                    }
                }
            }
            catch { /* Fall through to defaults */ }

            // Seed default profiles
            var defaults = new[]
            {
                new MouseProfile { Name = "FPS",       Icon = "🎯", Sensitivity = 4.0, Dpi = 800,  SmoothnessLevel = 0,  SmoothnessPreset = "Competitive", Description = "Low sens, raw input — FPS titles" },
                new MouseProfile { Name = "Casual",    Icon = "🖱️", Sensitivity = 6.0, Dpi = 1600, SmoothnessLevel = 50, SmoothnessPreset = "Balanced",    Description = "Daily driver settings" },
                new MouseProfile { Name = "Precision", Icon = "⚡", Sensitivity = 3.0, Dpi = 400,  SmoothnessLevel = 30, SmoothnessPreset = "Low",         Description = "Ultra-precise, low DPI" },
            };
            foreach (var d in defaults) Profiles.Add(d);
            SelectedProfile = Profiles.First();
        }

        private string EnsureUniqueName(string name)
        {
            int i = 2;
            string candidate = name;
            while (Profiles.Any(p => p.Name == candidate))
                candidate = $"{name} ({i++})";
            return candidate;
        }
    }
}
