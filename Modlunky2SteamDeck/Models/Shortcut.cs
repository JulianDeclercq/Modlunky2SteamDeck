namespace Modlunky2SteamDeck;

public class Shortcut
{
    public string appId { get; set; }
    public string appname { get; set; } = "";
    public string exe { get; set; } = "";
    public string StartDir { get; set; } = "";
    public string icon { get; set; } = "";
    public string ShortCutPath { get; set; } = "";
    public string LaunchOptions { get; set; } = "";
    public bool IsHidden { get; set; }
    public bool AllowDesktopConfig { get; set; }
    public bool AllowOverlay { get; set; }
    public bool OpenVR { get; set; }
    public bool Devkit { get; set; }
    public string DevkitGameID { get; set; } = "";
    public bool DevkitOverrideAppID { get; set; }
    public int LastPlayTime { get; set; } // datetime but not sure how to parse (unix timestamp so int instead?)
    public string FlatpakAppID { get; set; } = "";
    public List<string> Tags { get; set; } = new();

    public override string ToString()
    {
        return $"{appname} ({exe}) {appId}";
    }
}