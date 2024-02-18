using System.IO.Compression;
using System.Reflection;

namespace Modlunky2SteamDeck.Services;

public static class ZipService
{
    public static void UnzipEmbeddedResourceInto(string embeddedResource, string targetPath)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream(embeddedResource);
        if (stream == null)
            throw new Exception($"Failed to read embedded resource: {embeddedResource}");

        var archive = new ZipArchive(stream);
        archive.ExtractToDirectory(targetPath, true);

        Console.WriteLine($"Successfully unzipped {embeddedResource} into {targetPath}.");
    }
}