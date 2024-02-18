using System.Reflection;
using Modlunky2SteamDeck.Models;
using ValveKeyValue;

namespace Modlunky2SteamDeck.Services;

public static class ShortcutService
{
    private static readonly KVSerializer BinarySerializer = KVSerializer.Create(KVSerializationFormat.KeyValues1Binary);

    public static List<Shortcut> LoadShortcuts(string shortcutsPath)
    {
        using var shortcutsInputStream = File.OpenRead(shortcutsPath);
        var shortcuts = BinarySerializer.Deserialize<List<Shortcut>>(shortcutsInputStream);

        Console.WriteLine("Successfully read existing shortcuts from stream.");
        return shortcuts;
    }

    public static Shortcut LoadModlunkyEntry()
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var modlunkyStream = assembly.GetManifestResourceStream("Modlunky2SteamDeck.modlunky_shortcut.vdf");

        var modlunkyEntry = BinarySerializer.Deserialize<Shortcut>(modlunkyStream);
        if (modlunkyEntry == null)
            throw new Exception("Failed to read modlunky entry");

        Console.WriteLine("Successfully read modlunky entry from resource file.");
        return modlunkyEntry;
    }

    public static void SaveShortcuts(List<Shortcut> shortcuts, string shortcutsPath)
    {
        using var outputStream = File.OpenWrite(shortcutsPath);
        BinarySerializer.Serialize(outputStream, shortcuts, "Shortcuts"); // not sure about name 
        Console.WriteLine("Successfully wrote modlunky shortcut to stream");
    }
}