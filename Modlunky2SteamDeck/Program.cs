using Modlunky2SteamDeck.Models.Config;
using Modlunky2SteamDeck.Utils;
using ValveKeyValue;

namespace Modlunky2SteamDeck;

internal abstract class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("Modlunky2SteamDeck started");

        const string steamPath = "/home/deck/.local/share/Steam";
        const string userDataPath = $"{steamPath}/userdata";
        const string configPath = $"{steamPath}/config";
        const string configFilePath = $"{configPath}/config.vdf";
        const string spelunky2Path = $"{steamPath}/steamapps/common/Spelunky 2";

        if (!Path.Exists(spelunky2Path))
            throw new Exception("Spelunky 2 is not installed.");

        const string modlunkyPath = $"{spelunky2Path}/modlunky2.exe";
        if (Path.Exists(modlunkyPath))
            throw new Exception("Modlunky2 is already installed.");

        Console.WriteLine("Downloading latest Modlunky2 release from GitHub..");
        await GithubUtil.DownloadLatestRelease("spelunky-fyi", "modlunky2", modlunkyPath);

        Console.WriteLine("Adding Modlunky2 shortcut and grid images for all Steam users");
        foreach (var userPath in Directory.GetDirectories(userDataPath))
        {
            ZipUtil.UnzipEmbeddedResourceInto("Modlunky2SteamDeck.modlunky_grid.zip", $"{userPath}/config/grid");
            AddModlunky2Shortcut($"{userPath}/config/shortcuts.vdf");
        }

        AddCompatToolMapping(configFilePath);

        Console.WriteLine("Modlunky2SteamDeck installed successfully!");
    }

    private static void AddModlunky2Shortcut(string shortcutsPath)
    {
        var shortcuts = ShortcutUtil.LoadShortcuts(shortcutsPath);
        var modlunkyEntry = ShortcutUtil.LoadModlunkyEntry();

        if (shortcuts.Any(s => s.exe.Equals(modlunkyEntry.exe)))
        {
            Console.WriteLine("Shortcut with modlunky exe already exists for user, skipping..");
            return;
        }

        shortcuts.Add(modlunkyEntry);
        Backup(shortcutsPath);
        ShortcutUtil.SaveShortcuts(shortcuts, shortcutsPath);
    }

    private static void AddCompatToolMapping(string configFilePath)
    {
        var textSerializer = KVSerializer.Create(KVSerializationFormat.KeyValues1Text);
        var options = new KVSerializerOptions
        {
            HasEscapeSequences = true
        };
        options.Conditions.Clear(); // Remove default conditionals set by the library

        using var configInputStream = File.OpenRead(configFilePath);
        var config = textSerializer.Deserialize<InstallConfigStore>(configInputStream, options);

        const string modlunkyAppId = "3921648026";

        Console.WriteLine("Adding Modlunky compat tool mapping..");
        var compatToolMapping = config.Software.Valve.Steam.CompatToolMapping;
        if (compatToolMapping.ContainsKey(modlunkyAppId))
        {
            Console.WriteLine("Modlunky compat tool mapping already exists");
            return;
        }

        compatToolMapping[modlunkyAppId] = new CompatToolMappingEntry
        {
            name = "proton_experimental",
            config = "",
            priority = 250
        };

        Backup(configFilePath);
        using var stream = new FileStream(configFilePath, FileMode.Truncate, FileAccess.Write, FileShare.None);
        var kv = KVSerializer.Create(KVSerializationFormat.KeyValues1Text);
        kv.Serialize(stream, config, "InstallConfigStore");
    }

    private static void Backup(string path)
    {
        var fileName = Path.GetFileNameWithoutExtension(path);
        if (fileName == null)
            throw new Exception("Failed to get file name without extension");

        var directory = Path.GetDirectoryName(path);
        if (directory == null)
            throw new Exception("Failed to get directory");

        var backupPath = Path.Combine(directory, $"{fileName}{DateTime.Now:ddMMMyyHHmmss}.backup");
        File.Copy(path, backupPath);
    }
}