using System.Text.Json.Serialization;

namespace Modlunky2SteamDeck.Models;

public class ReleaseResponse
{
    public List<Asset> assets { get; set; }
}

public class Asset
{
    public string browser_download_url { get; set; }
}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(ReleaseResponse))]
internal partial class ReleaseResponseContext : JsonSerializerContext
{
}

// This ReleaseResponseContext is in a separate file because of a rider bug
// https://youtrack.jetbrains.com/issue/RIDER-84908