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

        AddCompatToolMapping("./config.vdf");
        return;
        
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
        //AddCompatToolMapping(configPath);

        Console.WriteLine("Modlunky2SteamDeck finished successfully.");
    }

    private static List<Shortcut> LoadShortcuts(KVSerializer binarySerializer, string shortcutsPath)
    {
        //shortcutsPath = "../../../shortcuts.vdf";

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

        var backupPath = $"{configPath}{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}.backup";
        File.Copy(configPath, backupPath); 
        
        using var configInputStream = File.OpenRead(configPath);
        var config = textSerializer.Deserialize<InstallConfigStore>(configInputStream, options);
        
        // var modlunkyAppId = "3921648026"; // real one
        var modlunkyAppId = "1337"; // fake one for testing

        var compatToolMapping = config.Software.Valve.Steam.CompatToolMapping;
        compatToolMapping[modlunkyAppId] = new CompatToolMappingEntry
        {
            name = "proton_experimental",
            config = "",
            priority = 250
        };
        
        // TODO: write to the configPath, not to the test output
        using var stream = new FileStream("configtestout.vdf", FileMode.Truncate, FileAccess.Write, FileShare.None);
        {
            var kv = KVSerializer.Create(KVSerializationFormat.KeyValues1Text);
            kv.Serialize(stream, config, "InstallConfigStore");
        }
        return;
        
        var newSerializer = KVSerializer.Create(KVSerializationFormat.KeyValues1Text); 
        var newOptions = new KVSerializerOptions
        {
            HasEscapeSequences = true
        };
        newOptions.Conditions.Clear(); // Remove default conditionals set by the library
        
        using var newConfigInputStream = File.OpenRead(configPath);
        var newConfig = newSerializer.Deserialize(newConfigInputStream, newOptions);
        int bkpt = 5;
    }

    private static void AddCompatToolMappingOld(string configPath)
    {
        var textSerializer = KVSerializer.Create(KVSerializationFormat.KeyValues1Text);
        var options = new KVSerializerOptions
        {
            HasEscapeSequences = true
        };
        options.Conditions.Clear(); // Remove default conditionals set by the library

        using var configInputStream = File.OpenRead(configPath);
        var config = textSerializer.Deserialize(configInputStream, options);

        // For some reason, using Children is the way to go since then the return type is KVObject instead of KVValue when using e.g. config["Software"]
        var newSoftware = config.Children.Single(c => c.Name == "Software");
        var newValve = newSoftware.Children.Single(c => c.Name == "Valve");
        var newSteam = newValve.Children.Single(c => c.Name == "Steam");
        var newCompatToolMapping = newSteam.Children.Single(c => c.Name == "CompatToolMapping");

        // var modlunkyAppId = "3921648026"; // real one
        var modlunkyAppId = "1337"; // fake one for testing

        var modlunkyEntry = new KVObject(modlunkyAppId, new List<KVObject>
        {
            new("name", "proton_experimental"),
            new("config", ""),
            new("priority", 250)
        });
        newCompatToolMapping.Add(modlunkyEntry);

        using var outputStream = File.OpenWrite($"{configPath}TESTOUTPUT");
        textSerializer.Serialize(outputStream, config, ""); // not sure about name
        Console.WriteLine("Successfully wrote modlunky compat tool mapping to stream");
    }
}