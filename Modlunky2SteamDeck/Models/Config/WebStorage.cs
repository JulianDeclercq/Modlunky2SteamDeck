namespace Modlunky2SteamDeck.Models.Config;

public class WebStorage
{
    // looks like it's a boolean but deserialize as a string since the value is "false" and the parser complains
    public string continuously_render_store { get; set; }

    // looks like a json object stored as a string
    public string DownloadsStoreRecentlyCompleted { get; set; }
}