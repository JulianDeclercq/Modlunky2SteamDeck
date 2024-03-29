﻿namespace Modlunky2SteamDeck.Models.Config;

public class InstallConfigStore
{
    public Software Software { get; set; }
    public System System { get; set; }
    public string SDL_GamepadBind { get; set; }
    public UI UI { get; set; }
    public WebStorage WebStorage { get; set; }
    public Streaming streaming { get; set; } // capitalization is the same as in the original config.vdf
    public Music Music { get; set; }
    public BigPicture BigPicture { get; set; }
    public string SteamDeckRegisteredSteamID { get; set; }
    public string SteamDeckRegisteredSerialNumber { get; set; }
    public Customization Customization { get; set; }
}

public class Software
{
    public Valve Valve { get; set; }
}

public class Valve
{
    public Steam Steam { get; set; }
}