# Mouse Optimizer — Build Instructions

## Prerequisites

| Tool | Version | Download |
|------|---------|----------|
| .NET SDK | 8.0 or later | https://dotnet.microsoft.com/download |
| Visual Studio 2022 | Community (free) | https://visualstudio.microsoft.com/ |
| Git (optional) | Any | https://git-scm.com/ |

> **Note:** This is a Windows-only application (WPF requires Windows). You must compile it on a Windows machine.

---

## Method 1: Build via CLI (Fastest)

Open **Command Prompt** or **PowerShell** in the `MouseOptimizer/` folder:

```powershell
# 1. Restore NuGet packages
dotnet restore

# 2. Build Release EXE
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ./dist

# 3. Your EXE is at:
#    MouseOptimizer\dist\MouseOptimizer.exe
```

The `-p:PublishSingleFile=true` flag bundles everything into a **single standalone EXE**.

---

## Method 2: Build via Visual Studio 2022

1. Open **Visual Studio 2022**
2. **File → Open → Project/Solution** → select `MouseOptimizer.csproj`
3. Select **Release** in the build configuration dropdown (top toolbar)
4. Press **Ctrl+Shift+B** to build
5. Find the output in `bin\Release\net8.0-windows\`

To publish a standalone EXE:
- Right-click the project → **Publish**
- Choose **Folder** as target
- Set **Deployment mode** to "Self-contained"
- Check **Produce single file**
- Click **Publish**

---

## Required NuGet Packages

These are automatically restored by `dotnet restore`:

| Package | Version | Purpose |
|---------|---------|---------|
| `Newtonsoft.Json` | 13.0.3 | JSON profile save/load |
| `FontAwesome.WPF` | 4.7.0.9 | Icon font (optional, can remove if issues) |

---

## First Run — Admin Rights

The app requests **Administrator** privileges via its manifest to allow:
- Writing Windows mouse speed via `SystemParametersInfo`
- Disabling pointer acceleration

This is safe and standard for mouse utility software.

---

## Project Structure

```
MouseOptimizer/
├── App.xaml / App.xaml.cs           — Application entry point
├── MainWindow.xaml / .cs            — Main application window (MVVM root)
├── MouseOptimizer.csproj            — Project file (.NET 8 WPF)
├── app.manifest                     — Requests admin elevation
│
├── Views/
│   ├── SplashScreen.xaml / .cs      — Startup animation screen
│
├── ViewModels/
│   ├── BaseViewModel.cs             — INotifyPropertyChanged base
│   ├── RelayCommand.cs              — ICommand implementation
│   ├── MainViewModel.cs             — Root VM, navigation, settings
│   ├── MouseOptimizationViewModel.cs — Optimization panel logic
│   ├── SmoothnessViewModel.cs       — Smoothness panel logic
│   ├── SensitivityViewModel.cs      — Sensitivity panel logic
│   └── ProfilesViewModel.cs         — Profile CRUD + persistence
│
├── Models/
│   ├── MouseProfile.cs              — Profile data model
│   ├── AppSettings.cs               — App settings data model
│   └── SystemMouseHelper.cs         — Win32 API interop (safe calls only)
│
├── Styles/
│   └── Theme.xaml                   — Full red/black gaming theme
│
└── Converters/
    └── BooleanConverters.cs         — Value converters for bindings
```

---

## Data Persistence

Profiles and settings are saved to:
```
%AppData%\MouseOptimizer\
  profiles.json   — All mouse profiles
  settings.json   — Application settings
```

---

## Architecture

- **Pattern**: MVVM (Model-View-ViewModel)
- **Data binding**: WPF two-way bindings throughout
- **Commands**: RelayCommand (ICommand) for all button actions
- **No code-behind logic** beyond window chrome (drag, close, minimize)

---

## Troubleshooting

| Problem | Solution |
|---------|----------|
| "Win32 API failed" on apply | Right-click → Run as Administrator |
| FontAwesome icons missing | Remove `FontAwesome.WPF` reference from `.csproj` if not needed |
| White screen on startup | Ensure .NET 8 Desktop Runtime is installed |
| Build errors | Run `dotnet restore` first, then rebuild |
