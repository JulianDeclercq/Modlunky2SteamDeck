namespace Modlunky2SteamDeck.Models.Config;

public class UI
{
    public Window Window { get; set; }
    public Display display { get; set; } // capitalization is the same as in the original config.vdf
}

public class Window
{
    public Sp sp { get; set; }
}

public class Sp
{
    public int X { get; set; }
    public int Y { get; set; }
    public int width { get; set; } // capitalization is the same as in the original config.vdf
    public int height { get; set; } // capitalization is the same as in the original config.vdf
}

public class Display
{
    public Current Current { get; set; }
}

public class Current
{
    // the scale factors here are doubles but the parser doesn't serialize that well, so handle them as strings as to not lose data
    public string MinScaleFactor { get; set; }
    public string MaxScaleFactor { get; set; }
    public bool IsExternalDisplay { get; set; }
    public string name { get; set; } // capitalization is the same as in the original config.vdf
    public string AutoScaleFactor { get; set; }
    public string ScaleFactor { get; set; }
}