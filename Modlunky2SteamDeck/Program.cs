using Modlunky2SteamDeck.Models.Config;
using Modlunky2SteamDeck.Services;
using ValveKeyValue;

namespace Modlunky2SteamDeck;

internal abstract class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("Modlunky2SteamDeck started");
        
        const string steamPath = "/home/deck/.local/share/Steam";
        const string configPath = $"{steamPath}/config";
        const string configFilePath = $"{configPath}/config.vdf";
        const string gridPath = $"{configPath}/grid";
        const string spelunky2Path = $"{steamPath}/steamapps/common/Spelunky 2";

        if (!Path.Exists(spelunky2Path))
            throw new Exception("Spelunky 2 is not installed.");

        const string modlunkyPath = $"{spelunky2Path}/modlunky2.exe";
        if (Path.Exists(modlunkyPath))
            throw new Exception("Modlunky2 is already installed.");

        // TODO: Support entering your steam id, this just picks the first one.
        var userPath = Directory.GetDirectories($"{steamPath}/userdata")[0];
        var shortcutsPath = $"{userPath}/config/shortcuts.vdf";

        await GithubService.DownloadLatestRelease("spelunky-fyi", "modlunky2", modlunkyPath);

        var shortcuts = ShortcutService.LoadShortcuts(shortcutsPath);
        var modlunkyEntry = ShortcutService.LoadModlunkyEntry();

        if (shortcuts.Any(s => s.exe.Equals(modlunkyEntry.exe)))
            throw new Exception("Shortcut with modlunky exe already exists.");

        shortcuts.Add(modlunkyEntry);
        Backup(shortcutsPath);
        ShortcutService.SaveShortcuts(shortcuts, shortcutsPath);

        AddCompatToolMapping(configFilePath);

        ZipService.UnzipEmbeddedResourceInto("Modlunky2SteamDeck.modlunky_grid.zip", gridPath);
        Console.WriteLine("Modlunky2SteamDeck finished successfully.");
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
        {
            var kv = KVSerializer.Create(KVSerializationFormat.KeyValues1Text);
            kv.Serialize(stream, config, "InstallConfigStore");
        }
    }

    private static void Backup(string path)
    {
        var fileName = Path.GetFileNameWithoutExtension(path);
        if (fileName == null)
            throw new Exception("Failed to get file name without extension");

        var backupPath = $"{fileName}{DateTime.UtcNow.ToString("ddMMMyyHHmmss")}.backup";
        File.Copy(path, backupPath);
    }
}