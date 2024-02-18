namespace Modlunky2SteamDeck.Models.Config;

public class Customization
{
    public StartupMovie StartupMovie { get; set; }
}

public class StartupMovie
{
    public string MovieID { get; set; }
    public string LocalPath { get; set; }
}