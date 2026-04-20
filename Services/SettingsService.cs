using Microsoft.Win32;
using VsCodeProfilePicker.Models;

namespace VsCodeProfilePicker.Services;

public static class SettingsService
{
    private const string RegKey = @"Software\VsCodeProfilePicker";
    private const string DirsValue = "ProfileDirectories";
    private const string VsCodeValue = "VsCodeExePath";

    public static AppSettings Load()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(RegKey);
            if (key is null) return new AppSettings();

            var dirs = (key.GetValue(DirsValue) as string[] ?? [])
                       .Where(d => !string.IsNullOrWhiteSpace(d))
                       .ToList();

            var vscodePath = key.GetValue(VsCodeValue) as string ?? "code";

            return new AppSettings
            {
                ProfileDirectories = dirs,
                VsCodeExePath = vscodePath
            };
        }
        catch { }

        return new AppSettings();
    }

    public static void Save(AppSettings settings)
    {
        using var key = Registry.CurrentUser.CreateSubKey(RegKey);
        key.SetValue(DirsValue, settings.ProfileDirectories.ToArray(), RegistryValueKind.MultiString);
        key.SetValue(VsCodeValue, settings.VsCodeExePath, RegistryValueKind.String);
    }
}
