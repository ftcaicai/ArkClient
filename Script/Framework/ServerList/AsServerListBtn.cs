using UnityEngine;
using System.Collections;
using System;
//using WmWemeSDK.JSON;
//using System.Text;

public class AsServerListBtn : MonoBehaviour
{
//	#if UNITY_ANDROID
//	const string market = "google";
//	#elif UNITY_IPHONE
//	const string market = "appstore";
//	#elif ( UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN)
//	const string market = "google";
//	#endif
//	const string domain = "wemejp";
	public AS_LC_SERVERLIST_BODY serverData;
	public UIRadioBtn radioBtn = null;
	[SerializeField]SpriteText recommendText = null;
	public GameObject m_goRecentIcon = null;
	private int degSaturation = -1;
	public int Saturation
	{
		get	{ return degSaturation; }
		set
		{
			degSaturation = value;

			switch( degSaturation)
			{
			case 0:	// inspection
				radioBtn.controlIsEnabled = false;
				break;
			case 1:	// smooth
				break;
			case 2:	//	caotic
				break;
			case 3:	// saturation
				break;
			default:
				Debug.LogError( "Invalid degree of saturation.");
				break;
			}
		}
	}

	//$yde
	static AS_LC_SERVERLIST_BODY s_SelectedServerData;
	public static AS_LC_SERVERLIST_BODY selectedServerData	{ get { return s_SelectedServerData; } }
	public static AsServerListStandbyDlg s_ServerListStandbyDlg;
	public delegate void _ModifyServerConnection( bool _connect);
	static _ModifyServerConnection m_Modifier;

	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
		if( null == m_goRecentIcon)
		{
//			Debug.Log( "AsServerListBtn::Update(), null == m_goRecentIcon");
			return;
		}

		if( null != serverData)
		{
			if( serverData.nWorldIdx == AsLoginProcess.nLastWorldIdxBuf)
			{
				if( false == m_goRecentIcon.activeInHierarchy)
					m_goRecentIcon.SetActive( true);
			}
			else
			{
				if( true == m_goRecentIcon.activeInHierarchy)
					m_goRecentIcon.SetActive( false);
			}
		}
		else
		{
			m_goRecentIcon.SetActive( false);
		}
	}

	public void SetRecommendState( bool flag)
	{
		if( ( true == flag) && ( 0 != degSaturation))
			recommendText.Text = AsTableManager.Instance.GetTbl_String( 2039);
		else
			recommendText.Text = string.Empty;
	}

	public void CharacterCreateEnable( bool flag)
	{
		radioBtn.controlIsEnabled = flag;
	}
	
//	bool CheckMaintenance()
//	{
//		StringBuilder sb = new StringBuilder();
//		sb.AppendFormat( "{0}{1}&domain={2}&game={3}&version={4}", AsNetworkDefine.WemeGateBaseUrl, market, domain, AsGameDefine.GAME_CODE, AsGameDefine.BUNDLE_VERSION);
//		
//		WWW www = new WWW( sb.ToString());
//		//		yield return www;
//		while( false == www.isDone)	{}
//		
//		if( null != www.error)
//		{
//			Debug.Log( "WemeGateManager ShutDown");
//			AsUtil.ShutDown( string.Empty);
//		}
//		
//		JSONObject topObj = JSONObject.Parse( www.text);
//		www.Dispose();
//		www = null;
//		
//		UInt64 curTime = UInt64.Parse( topObj[ "timestamp"].Str);
//		JSONObject maintenance = topObj.GetObject( "maintenance");
//		if( null == maintenance)
//			return true;
//		
//		string maintenanceBegin = maintenance[ "begin"].Str;
//		string maintenanceEnd = maintenance[ "end"].Str;
//		UInt64 beginTime = UInt64.Parse( maintenanceBegin);
//		UInt64 endTime = UInt64.Parse( maintenanceEnd);
//		
//		if( ( curTime < beginTime) || ( curTime > endTime))
//			return true;
//		
//		return false;
//	}

	public bool ConnectServer()//$yde
	{
//		if( s_SelectedServerData != null)
//			return false;

//		if( false == CheckMaintenance())
//			return false;

		if( false == serverData.bPossibleCreate)
		{
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(2037), AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			return false;
		}

		string ip = System.Text.UTF8Encoding.UTF8.GetString( serverData.szIpaddress);
		AsNetworkManager.Instance.ConnectToServer( ip, serverData.nPort, SOCKET_STATE.SS_GAMESERVER);
		if( true == AsNetworkManager.Instance.IsConnected())
		{
			Debug.Log( "Connect game server...");
			Debug.Log( "AsUserInfo.Instance.isWemeCertified : " + AsUserInfo.Instance.isWemeCertified);

			s_SelectedServerData = serverData;

			body_CG_LOGIN_POSSIBLE possible = new body_CG_LOGIN_POSSIBLE();
			byte[] data = possible.ClassToPacketBytes();
			AsNetworkMessageHandler.Instance.Send( data);

			AsUserInfo.Instance.CurrentServerID = s_SelectedServerData.nWorldIdx;
			AsSocialManager.Instance.ServerName =  AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( s_SelectedServerData.szServerName));
			Debug.Log( "body_CG_LOGIN_POSSIBLE is send");

			return true;
		}

		return false;
	}

	public static void ConnectionConfirmed()//$yde
	{
		if( s_ServerListStandbyDlg != null)
			Destroy( s_ServerListStandbyDlg.gameObject);

		AsLoadingIndigator.Instance.ShowIndigator( "");

		if( false == AsUserInfo.Instance.isWemeCertified)
		{
			AS_CG_LOGIN gameLogin = new AS_CG_LOGIN( AsUserInfo.Instance.LoginUserUniqueKey, AsUserInfo.Instance.LoginUserSessionKey, AsNetworkDefine.PROTOCOL_VERSION);//$yde
			byte[] data = gameLogin.ClassToPacketBytes();
			AsNetworkMessageHandler.Instance.Send( data);
		}
		else
		{
			body_CG_WEMELOGIN wemeLogin = new body_CG_WEMELOGIN();
			wemeLogin.nUserUniqKey = AsUserInfo.Instance.LoginUserUniqueKey;
			Debug.Log( "AsUserInfo.Instance.LoginUserUniqueKey : " + AsUserInfo.Instance.LoginUserUniqueKey);
			byte[] src = System.Text.UTF8Encoding.UTF8.GetBytes( AsUserInfo.Instance.WemeAuthToken);
			System.Buffer.BlockCopy( src, 0, wemeLogin.strAccessToken, 0, src.Length);
			Debug.Log( "AsUserInfo.Instance.WemeAuthToken : " + AsUserInfo.Instance.WemeAuthToken);
			wemeLogin.nVersion = AsNetworkDefine.PROTOCOL_VERSION;//$yde
			wemeLogin.bIsGuest = WemeSdkManager.Instance.IsGuest;//2014.05.16
			//wemeLogin.bIsGuest = WemeSdkManager.Instance.IsConfirmGuest;

			byte[] packet = wemeLogin.ClassToPacketBytes();
			AsNetworkMessageHandler.Instance.Send( packet);
		}

		s_SelectedServerData = null;
	}

	public static void CreateStandbyDlg( body_GC_LOGIN_POSSIBLE_RESULT _result)//$yde
	{
		if( s_ServerListStandbyDlg == null)
		{
			GameObject obj = Instantiate( Resources.Load( "UI/Optimization/Prefab/MessageBox_LoginLocked")) as GameObject;
			s_ServerListStandbyDlg = obj.GetComponent<AsServerListStandbyDlg>();
		}

		s_ServerListStandbyDlg.Init( _result);
	}

	public static void CancelStandbyDlg()
	{
		if( m_Modifier != null)
			m_Modifier( false);

		s_SelectedServerData = null;
		AsNetworkManager.Instance.InitSocket();
	}

	public static void SetModifier( _ModifyServerConnection _modify)
	{
		m_Modifier = _modify;
	}
	
	public static void SetServerListBody(AS_LC_SERVERLIST_BODY _body)
	{
		s_SelectedServerData = _body;
	}
}
