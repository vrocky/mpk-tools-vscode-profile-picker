using System.IO;
using VsCodeProfilePicker.Models;

namespace VsCodeProfilePicker.Services;

public static class ProfileScanService
{
    private static readonly string[] AvatarColors =
    [
        "#007acc", "#c586c0", "#4ec9b0", "#ce9178",
        "#569cd6", "#dcdcaa", "#6a9955", "#f44747",
        "#9cdcfe", "#d7ba7d", "#b5cea8", "#608b4e"
    ];

    public static List<VsCodeProfile> ScanAll(IEnumerable<string> directories)
    {
        var profiles = new List<VsCodeProfile>();
        int colorIndex = 0;

        foreach (var dir in directories)
        {
            if (!Directory.Exists(dir)) continue;

            var subDirs = Directory.GetDirectories(dir)
                .OrderBy(d => Path.GetFileName(d), StringComparer.OrdinalIgnoreCase);

            foreach (var sub in subDirs)
            {
                var name = Path.GetFileName(sub);
                var userDataPath = Path.Combine(sub, "user-data");
                var extensionsPath = Path.Combine(sub, "extensions");

                // Ensure subdirs exist so VS Code can use them immediately
                Directory.CreateDirectory(userDataPath);
                Directory.CreateDirectory(extensionsPath);

                profiles.Add(new VsCodeProfile
                {
                    Name = name,
                    FullPath = sub,
                    SourceDirectory = dir,
                    Initials = GetInitials(name),
                    AvatarColor = AvatarColors[colorIndex % AvatarColors.Length],
                    UserDataPath = userDataPath,
                    ExtensionsPath = extensionsPath,
                    LastModified = Directory.GetLastWriteTime(sub)
                });
                colorIndex++;
            }
        }

        return profiles;
    }

    private static string GetInitials(string name)
    {
        var parts = name.Split([' ', '-', '_', '.'], StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 2)
            return $"{char.ToUpper(parts[0][0])}{char.ToUpper(parts[1][0])}";
        if (parts.Length == 1 && parts[0].Length > 0)
            return parts[0].Length >= 2
                ? $"{char.ToUpper(parts[0][0])}{char.ToUpper(parts[0][1])}"
                : char.ToUpper(parts[0][0]).ToString();
        return "?";
    }
}
