namespace Modlunky2SteamDeck.Models;

public class Customization
{
    public StartupMovie StartupMovie { get; set; }
}

public class StartupMovie
{
    string MovieID { get; set; }
    string LocalPath { get; set; }
}
