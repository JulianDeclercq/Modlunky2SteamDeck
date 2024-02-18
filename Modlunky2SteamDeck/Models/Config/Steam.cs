namespace Modlunky2SteamDeck.Models.Config;

public class Steam
{
    public bool AutoUpdateWindowEnabled { get; set; }
    public Perf Perf { get; set; }
    public string ipv6check_http_state { get; set; }
    public Storage Storage { get; set; }
    public bool IsSteamDeck { get; set; }
    public string ipv6check_udp_state { get; set; }
    public ShaderCacheManager ShaderCacheManager { get; set; }
    public Dictionary<string, WebSocketEntry> CMWebSocket { get; set; }
    public string RecentWebSocket443Failures { get; set; }
    public string RecentWebSocketNon443Failures { get; set; }
    public string RecentUDPFailures { get; set; }
    public string RecentTCPFailures { get; set; }
    public Dictionary<string, Account> Accounts { get; set; }
    public string CellIDServerOverride { get; set; }
    public string MTBF { get; set; }
    public string cip { get; set; }
    public SteamSystem System { get; set; }
    public string SurveyDateSteamDeckDelay { get; set; }
    public int Rate { get; set; }
    public Dictionary<string, DecryptionKeyWrapper> depots { get; set; }
    public string RecentDownloadRate { get; set; } // probably a number
    public string LastConfigstoreUploadTime { get; set; }
    public Dictionary<string, SizeOnDiskWrapper> Tools { get; set; }
    public string SurveyDate { get; set; }
    public string SurveyDateVersion { get; set; }
    public string SurveyDateType { get; set; } // probably an enum or a number
    public Dictionary<string, CompatToolMappingEntry> CompatToolMapping { get; set; }
}

public class Perf
{
    public GameProfiles GameProfiles { get; set; }
}

public class GameProfiles
{
    public List<List<string>> Global { get; set; }
    public Dictionary<string, List<List<string>>> App { get; set; }
}

public class Storage
{
    public long NextTrimRunTime { get; set; }
}

public class ShaderCacheManager
{
    public bool HasCurrentBucket { get; set; }
    public string CurrentBucketGPU { get; set; }
    public string CurrentBucketDriver { get; set; }
    public Dictionary<string, ShaderCacheSizeClass> App { get; set; }
    public string ProcessingQueue { get; set; }
    public Dictionary<string, string> CompatToolBucketHashes { get; set; }
    public Dictionary<string, string> CommittedDepotManifests { get; set; }
}

public class ShaderCacheSizeClass
{
    public long ShaderCacheSize { get; set; }
}

public class WebSocketEntry
{
    public string LastPingTimestamp { get; set; }
    public int LastPingValue { get; set; }
    public int LastLoadValue { get; set; }
}

public class Account
{
    public string SteamID { get; set; }
}

public class SteamSystem // System is already used in InstallConfigStore
{
    public string HostID { get; set; }
    public int IdleBacklightDimBatterySeconds { get; set; }
    public int IdleBacklightDimACSeconds { get; set; }
    public int IdleSuspendBatterySeconds { get; set; }
    public int IdleSuspendACSeconds { get; set; }
    public bool WifiPowerManagementEnabled { get; set; }
    public string DisplayBrightness { get; set; } // probably a double
}

public class DecryptionKeyWrapper
{
    public string DecryptionKey { get; set; }
}

public class SizeOnDiskWrapper
{
    public string SizeOnDisk { get; set; }
}

public class CompatToolMappingEntry
{
    public string name { get; set; }
    public string config { get; set; }
    public int priority { get; set; }
}