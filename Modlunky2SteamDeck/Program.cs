using System.Reflection;
using Modlunky2SteamDeck.Models;
using ValveKeyValue;

namespace Modlunky2SteamDeck;

internal abstract class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("Modlunky2SteamDeck started");

        const string steamPath = "/home/deck/.local/share/Steam";
        const string configPath = $"{steamPath}/config/config.vdf";
        const string spelunky2Path = $"{steamPath}/steamapps/common/Spelunky 2";
        
        if (!Path.Exists(spelunky2Path))
            throw new Exception("Spelunky 2 is not installed.");

        const string modlunkyPath = $"{spelunky2Path}/modlunky2.exe";
        if (Path.Exists(modlunkyPath))
            throw new Exception("Modlunky2 is already installed.");
        
        // TODO: Support entering your steam id, this just picks the first one.
        var userPath = Directory.GetDirectories($"{steamPath}/userdata")[0];
        var shortcutsPath = $"{userPath}/config/shortcuts.vdf";

        await GithubApi.DownloadLatestRelease("spelunky-fyi", "modlunky2", modlunkyPath); 
        
        var binarySerializer = KVSerializer.Create(KVSerializationFormat.KeyValues1Binary);
        var shortcuts = LoadShortcuts(binarySerializer, shortcutsPath);
        var modlunkyEntry = LoadModlunkyEntry(binarySerializer);

        if (shortcuts.Any(s => s.exe.Equals(modlunkyEntry.exe)))
            throw new Exception("Shortcut with modlunky exe already exists.");

        shortcuts.Add(modlunkyEntry);

        SaveShortcuts(binarySerializer, shortcuts, shortcutsPath);
        AddCompatToolMapping(configPath);

        Console.WriteLine("Modlunky2SteamDeck finished successfully.");
    }

    private static List<Shortcut> LoadShortcuts(KVSerializer binarySerializer, string shortcutsPath)
    {
        using var shortcutsInputStream = File.OpenRead(shortcutsPath);
        var shortcuts = binarySerializer.Deserialize<List<Shortcut>>(shortcutsInputStream);

        Console.WriteLine("Successfully read existing shortcuts from stream.");
        return shortcuts;
    }

    private static Shortcut LoadModlunkyEntry(KVSerializer binarySerializer)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var modlunkyStream = assembly.GetManifestResourceStream("Modlunky2SteamDeck.modlunky_shortcut.vdf");

        var modlunkyEntry = binarySerializer.Deserialize<Shortcut>(modlunkyStream);
        if (modlunkyEntry == null)
            throw new Exception("Failed to read modlunky entry");

        Console.WriteLine("Successfully read modlunky entry from resource file.");
        return modlunkyEntry;
    }

    private static void SaveShortcuts(KVSerializer binarySerializer, List<Shortcut> shortcuts, string shortcutsPath)
    {
        Backup(shortcutsPath);

        using var outputStream = File.OpenWrite(shortcutsPath);
        binarySerializer.Serialize(outputStream, shortcuts, "Shortcuts"); // not sure about name 
        Console.WriteLine("Successfully wrote modlunky shortcut to stream");
    }

    private static void AddCompatToolMapping(string configPath)
    {
        var textSerializer = KVSerializer.Create(KVSerializationFormat.KeyValues1Text);
        var options = new KVSerializerOptions
        {
            HasEscapeSequences = true
        };
        options.Conditions.Clear(); // Remove default conditionals set by the library

        using var configInputStream = File.OpenRead(configPath);
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

        Backup(configPath);
        using var stream = new FileStream(configPath, FileMode.Truncate, FileAccess.Write, FileShare.None);
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