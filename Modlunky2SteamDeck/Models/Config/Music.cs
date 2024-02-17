namespace Modlunky2SteamDeck.Models;

public class Music
{
    public LocalLibrary LocalLibrary { get; set; }
}

public class LocalLibrary
{
    public List<string> Directories { get; set; }
}