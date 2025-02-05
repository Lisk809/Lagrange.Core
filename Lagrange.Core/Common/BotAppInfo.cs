#pragma warning disable CS8618

namespace Lagrange.Core.Common;

public class BotAppInfo
{
    public string Os { get; private set; }
    
    public string Kernel { get; private set; }
    
    public string BaseVersion { get; private set; }
    
    public string CurrentVersion { get; private set; }
    
    public int BuildVersion { get; private set; }
    
    public int MiscBitmap { get; private set; }
    
    public string PtVersion { get; private set; }
    
    public int PtOsVersion { get; private set; }
    
    public string PackageName { get; private set; }
    
    public string WtLoginSdk { get; private set; }
    
    /// <summary>Or Known as QUA</summary>
    public string PackageSign { get; private set; }
    
    public int UnknownIdentifier { get; private set; }
    
    public int AppId { get; private set; }
    
    /// <summary>Or known as pubId in tencent log</summary>
    public int SubAppId { get; private set; }
    
    public int AppIdQrCode { get; private set; }
    
    public ushort AppClientVersion { get; private set; }
    
    public uint MainSigMap { get; private set; }
    
    public ushort SubSigMap { get; private set; }

    private static readonly BotAppInfo Linux = new()
    {
        Os = "Linux",
        Kernel = "Linux",
        
        BaseVersion = "3.1.1-11223",
        CurrentVersion = "3.1.2-13107",
        BuildVersion = 13107,
        MiscBitmap = 32764,
        PtVersion = "2.0.0",
        PtOsVersion = 19,
        PackageName = "com.tencent.qq",
        WtLoginSdk = "nt.wtlogin.0.0.1",
        PackageSign = "V1_LNX_NQ_3.1.2-13107_RDM_B",
        
        UnknownIdentifier = 70644224,
        AppId = 1600001615,
        SubAppId = 537146866,
        AppIdQrCode = 13697054,
        AppClientVersion = 13172,
        
        MainSigMap = 169742560,
        SubSigMap = 0
    };
    
    private static readonly BotAppInfo MacOs = new()
    {
        Os = "Mac",
        Kernel = "Darwin",

        BaseVersion = "6.9.17-12118",
        CurrentVersion = "6.9.17-12118",
        BuildVersion = 12118,
        PtVersion = "2.0.0",
        MiscBitmap = 32764,
        PtOsVersion = 23,
        PackageName = "com.tencent.qq",
        WtLoginSdk = "nt.wtlogin.0.0.1",
        PackageSign = "V1_MAC_NQ_6.9.17-12118_RDM_B",
        
        UnknownIdentifier = 70644224,
        AppId = 1600001602,
        SubAppId = 537138182,
        AppIdQrCode = 537138182,
        AppClientVersion = 13172,
        
        MainSigMap = 169742560,
        SubSigMap = 0
    };
    
    private static readonly BotAppInfo Windows = new()
    {
        Os = "Windows",
        Kernel = "Windows",

        BaseVersion = "9.8.3-13183",
        CurrentVersion = "9.8.3-13183",
        BuildVersion = 13183,
        PtVersion = "2.0.0",
        MiscBitmap = 32764,
        PtOsVersion = 23,
        PackageName = "com.tencent.qq",
        WtLoginSdk = "nt.wtlogin.0.0.1",
        PackageSign = "V1_WIN_NQ_9.8.3-13183_RDM_B",
        
        UnknownIdentifier = 70644224,
        AppId = 1600001604,
        SubAppId = 537138217,
        AppIdQrCode = 537138217,
        AppClientVersion = 13172,
        
        MainSigMap = 169742560,
        SubSigMap = 0
    };
    
    public static readonly Dictionary<Protocols, BotAppInfo> ProtocolToAppInfo = new()
    {
        { Protocols.Windows, Windows },
        { Protocols.Linux, Linux },
        { Protocols.MacOs, MacOs },
    };
}