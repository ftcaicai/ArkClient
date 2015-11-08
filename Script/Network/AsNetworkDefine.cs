
#define _DEV_SERVER
//#define _ALPHA_SERVER
//#define _BETA_SERVER
//#define _REAL_SERVER
#define _USE_DEV_IAPSERVER
#define _USE_BETA_WEMEGATE
//#define _WINDOWS_BUILD

using System;

static public class AsNetworkDefine
{
	public const uint PROTOCOL_VERSION = 8;
#if _DEV_SERVER
	public const string ENVIRONMENT = "_Dev";
	private static string LoginServerIP = "61.147.88.31";//118.36.245.203
	public static string LOGIN_SERVER_IP
	{
		get	{ return LoginServerIP; }
		set	{}
	}
	public const Int32 LOGIN_SERVER_PORT = 2011;
	//public const string DimpleLog_url = "http://beta-dimple.cjp.wemade.com:10080/1.0/record/";
#if _WINDOWS_BUILD
	private static string PatchServerAddress = "http://we-mclient.gscdn.com/arksphere/dev/patch_jpn_win_dev/";
#else
	//private static string PatchServerAddress = "http://we-mclient.gscdn.com/arksphere/dev/patch_jpn_dev/";
	//private static string PatchServerAddress = "http://192.168.1.103:8001/arksphere/";
	private static string PatchServerAddress = "http://61.147.96.36:10086/";
#endif
	public static string PATCH_SERVER_ADDRESS
	{
		get	{ return PatchServerAddress; }
		set	{ PatchServerAddress = value; }
	}
	public const string WemeServerZone = "cjp_beta";
#elif _ALPHA_SERVER
	public const string ENVIRONMENT = "_Alpha";
	private static string LoginServerIP = "118.36.245.50";
	public static string LOGIN_SERVER_IP
	{
		get	{ return LoginServerIP; }
		set	{ /*LoginServerIP = value;*/ }
	}
	public const Int32 LOGIN_SERVER_PORT = 1001;
	public const string DimpleLog_url = "http://beta-dimple.cjp.wemade.com:10080/1.0/record/";
	private static string PatchServerAddress = "http://we-mclient.gscdn.com/arksphere/dev/patch_alpha/";
	public static string PATCH_SERVER_ADDRESS
	{
		get	{ return PatchServerAddress; }
		set	{ /*PatchServerAddress = value;*/ }
	}
	public const string WemeServerZone = "cjp_beta";
//	public const string WemeServerZone = "cjp_alpha";
#elif _BETA_SERVER
	public const string ENVIRONMENT = "_Beta";
	private static string LoginServerIP = "106.187.111.45";
	public static string LOGIN_SERVER_IP
	{
		get	{ return LoginServerIP; }
		set	{ LoginServerIP = value; }
	}
	public const Int32 LOGIN_SERVER_PORT = 1001;
	public const string DimpleLog_url = "http://beta-dimple.cjp.wemade.com:10080/1.0/record/";
#if _WINDOWS_BUILD
	private static string PatchServerAddress = "http://we-mclient.gscdn.com/arksphere/dev/patch_jpn_win_beta/";
	public static string PATCH_SERVER_ADDRESS
	{
		get	{ return PatchServerAddress; }
		set	{}
	}
#else
	private static string PatchServerAddress = "http://arksphere-patch.wemade.gscdn.com/resource/beta/";
	public static string PATCH_SERVER_ADDRESS
	{
		get	{ return PatchServerAddress; }
		set	{ PatchServerAddress = value; }
	}
#endif
	public const string WemeServerZone = "cjp_beta";
#elif _REAL_SERVER
	public const string ENVIRONMENT = "";
	private static string LoginServerIP = "login.arksphere.wemade.jp";
	public static string LOGIN_SERVER_IP
	{
		get	{ return LoginServerIP; }
		set	{ LoginServerIP = value; }
	}
	public const Int32 LOGIN_SERVER_PORT = 1001;
	public const string DimpleLog_url = "http://dimple.cjp.wemade.com:10080/1.0/record/";
#if _WINDOWS_BUILD
	private static string PatchServerAddress = "http://we-mclient.gscdn.com/arksphere/dev/patch_jpn_win_real/";
	public static string PATCH_SERVER_ADDRESS
	{
		get	{ return PatchServerAddress; }
		set	{}
	}
#else
	private static string PatchServerAddress = "http://arksphere-patch.wemade.gscdn.com/resource/real/";
	public static string PATCH_SERVER_ADDRESS
	{
		get	{ return PatchServerAddress; }
		set	{ PatchServerAddress = value; }
	}
#endif
	public const string WemeServerZone = "cjp_real";
#endif
	private static string IAPAgentServerIP = string.Empty;
	public static string IAPAGENT_SERVER_IP
	{
		get	
		{

#if _USE_DEV_IAPSERVER
			return IAPAgentServerIP;
#else
			return IAPAgentServerDomain;
#endif
		}
		set { IAPAgentServerIP = value;}
	}

	private static string IAPAgentServerDomain = string.Empty;
	public static string IAPAGENT_DOMAIN
	{
		get { return IAPAgentServerDomain; }
		set { IAPAgentServerDomain = value; }
	}

	public static UInt16 IAPAGENT_SERVER_PORT;
	public const string DEFAULT_LOGIN_ID = "";
	public const string DEFAULT_LOGIN_PASS = "";
	public const Int32 eSERVERNAME = 16;
	public const Int32 eIPADDRESS = 16;
	public const Int32 eDOMAIN = 128;
#if _USE_BETA_WEMEGATE
	public const string WemeGateBaseUrl = "http://beta-gate.cjp.wemade.com:8080/weme_lnc/service.json?market=";
#else
	public const string WemeGateBaseUrl = "http://gate.cjp.wemade.com:8080/weme_lnc/service.json?market=";
#endif
	private static string imageServerAddress = "http://arc.gamecom.jp/arcsphere/fgt/image/";
	public static string ImageServerAddress
	{
		get	{ return imageServerAddress; }
		set	{ imageServerAddress = value; }
	}

	public static bool GAME_MODE_REVIEW = false;
}





