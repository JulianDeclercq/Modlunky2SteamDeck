using System.Reflection;
using ValveKeyValue;

namespace Modlunky2SteamDeck;

internal abstract class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Modlunky2SteamDeck started");
        const string steamPath = "/home/deck/.local/share/Steam/userdata";

        // TODO: Support entering your steam id, this just picks the first one.
        var userPath = Directory.GetDirectories(steamPath)[0];
        var shortcutsPath = $"{userPath}/config/shortcuts.vdf";

        // TODO: REMOVE ME
        //shortcutsPath = "../../../shortcuts.vdf";

        List<Shortcut> shortcuts;
        var binarySerializer = KVSerializer.Create(KVSerializationFormat.KeyValues1Binary);
        using (var inputStream = File.OpenRead(shortcutsPath))
        {
            shortcuts = binarySerializer.Deserialize<List<Shortcut>>(inputStream);
        }
        Console.WriteLine("Successfully read existing shortcuts from stream");

        Shortcut? modlunkyEntry;
        var assembly = Assembly.GetExecutingAssembly();
        using (var modlunkyStream = assembly.GetManifestResourceStream("Modlunky2SteamDeck.modlunky_shortcut.vdf"))
        {
            modlunkyEntry = binarySerializer.Deserialize<Shortcut>(modlunkyStream);
            if (modlunkyEntry == null)
                throw new Exception("Failed to read modlunky entry");
        }

        Console.WriteLine("Successfully read modlunky entry from resource file.");

        if (shortcuts.Any(s => s.exe.Equals(modlunkyEntry.exe)))
            throw new Exception("Shortcut with modlunky exe already exists.");

        shortcuts.Add(modlunkyEntry);

        using var outputStream = File.OpenWrite(shortcutsPath);
        binarySerializer.Serialize(outputStream, shortcuts, ""); // not sure what to use for name
        Console.WriteLine("Successfully wrote new shortcut to stream");
    }
}