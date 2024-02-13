using System.Reflection;
using Modlunky2SteamDeck.Models;
using ValveKeyValue;
using Directory = System.IO.Directory;

namespace Modlunky2SteamDeck;

internal abstract class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Modlunky2SteamDeck started");

        const string steamPath = "/home/deck/.local/share/Steam";
        const string configPath = $"{steamPath}/config/config.vdf";

        // TODO: Support entering your steam id, this just picks the first one.
        var userPath = Directory.GetDirectories($"{steamPath}/userdata")[0];
        var shortcutsPath = $"{userPath}/config/shortcuts.vdf";

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
        // TODO: Take a backup of the file before modifying it
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
        
        var backupPath = $"{configPath}{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}.backup";
        File.Copy(configPath, backupPath); 
        
        using var stream = new FileStream(configPath, FileMode.Truncate, FileAccess.Write, FileShare.None);
        {
            var kv = KVSerializer.Create(KVSerializationFormat.KeyValues1Text);
            kv.Serialize(stream, config, "InstallConfigStore");
        }
    }
}