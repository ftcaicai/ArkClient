//#define _USE_LOGIN_UI
//#define _USE_FACEBOOK
//#define _USE_NAVER
//#define _USE_GOOGLE
//#define _USE_BAND
//#define _USE_YAHOO_JP
//#define _USE_GUESTLOGIN
#define _USE_DEVICELOGIN
#define _USE_TWITTERLOGIN

using UnityEngine;
using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Text;
using WmWemeSDK.JSON;


public class AsLoginScene : AsSceneBase
{
	[HideInInspector] public string ip;
	[HideInInspector] public string id;
	[HideInInspector] public string password;

	[SerializeField] UITextField editID = null;
	[SerializeField] UITextField editPass = null;
	[SerializeField] UIButton btnLogin = null;
	[SerializeField] UIButton btnWemeLogin = null;
	[SerializeField] UIButton btnGuestLogin = null;
	[SerializeField] UIButton btnFacebookLogin = null;
	[SerializeField] UIButton btnNaverLogin = null;
	[SerializeField] UIButton btnGoogleLogin = null;
	[SerializeField] UIButton btnBandLogin = null;	
	[SerializeField] UIButton btnYahooJpLogin = null;	
	[SerializeField] UIButton btnDeviceLogin = null;	
	[SerializeField] UIButton btnTwitterLogin = null;	
	
	#region funtion button
	[SerializeField] UIButton btnNewInfo = null;
	[SerializeField] UIButton btnWebSite = null;
	[SerializeField] UIButton btnInquiry = null;
	[SerializeField] UIButton btnDeviceChange = null;
	[SerializeField] UIButton btnAccountSetting = null;
	[SerializeField] UIButton btnTouch = null;

	
	private  GameObject accountDlg = null;
	private GameObject withdrawDlg = null;
	#endregion
	[SerializeField] SimpleSprite centerWithBand = null;
	[SerializeField] SimpleSprite withBand = null;
	
	
	private Color currentColor;
	private string strID = AsNetworkDefine.DEFAULT_LOGIN_ID;
	private string strPass = AsNetworkDefine.DEFAULT_LOGIN_PASS;
	private string gameServerIP;
	private ushort gameServerPort;



	#if UNITY_ANDROID
	const string market = "google";
	#elif UNITY_IPHONE
	const string market = "appstore";
	#elif ( UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN)
	const string market = "google";
	#endif
	const string domain = "wemejp";

	
	public string logString { get;set; }

	public string GameServerIP
	{
		get	{ return gameServerIP; }
		set	{ gameServerIP = value; }
	}

	public ushort GameServerPort
	{
		get	{ return gameServerPort; }
		set	{ gameServerPort = value; }
	}

	public string IP
	{
		get	{ return ip; }
	}

	public string ID
	{
		get	{ return strID; }
	}

	public string Password
	{
		get	{ return strPass; }
	}

	#if( !_USE_LOGIN_UI && !UNITY_EDITOR && !UNITY_STANDALONE_OSX && !UNITY_STANDALONE_WIN)
	private void _DisableLoginUI()
	{
		SimpleSprite bg = gameObject.GetComponent<SimpleSprite>();
		bg.renderer.enabled = false;
		editID.gameObject.SetActiveRecursively( false);
		editPass.gameObject.SetActiveRecursively( false);
		btnLogin.gameObject.SetActiveRecursively( false);
		
		#if _USE_BAND	
		centerWithBand.gameObject.SetActiveRecursively( true);
		withBand.gameObject.SetActiveRecursively( true);		
		#else
		centerWithBand.gameObject.SetActiveRecursively( false);	
		withBand.gameObject.SetActiveRecursively( false);
		#endif
	}
	#endif

	void SetVisibleLoginBtn()
	{
		#if( UNITY_EDITOR || _USE_LOGIN_UI || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN)
		btnGuestLogin.gameObject.SetActiveRecursively( false);
		btnWemeLogin.gameObject.SetActiveRecursively( false);
		btnFacebookLogin.gameObject.SetActiveRecursively( false);
		btnNaverLogin.gameObject.SetActiveRecursively( false);
		btnGoogleLogin.gameObject.SetActiveRecursively( false);
		btnBandLogin.gameObject.SetActiveRecursively( false);
		btnYahooJpLogin.gameObject.SetActiveRecursively( false);
		btnDeviceLogin.gameObject.SetActiveRecursively( false);
		btnTwitterLogin.gameObject.SetActiveRecursively( false);
		return;
		#endif
		
		#if _USE_BAND || _USE_YAHOO_JP
		btnWemeLogin.gameObject.SetActiveRecursively( false);
		#else
		btnWemeLogin.gameObject.SetActiveRecursively( !UserData.Instance.isAuthorized);
		btnWemeLogin.SetInputDelegate( OnWemeLoginBtn);
		#endif
		
		#if _USE_FACEBOOK
		btnFacebookLogin.gameObject.SetActiveRecursively( !UserData.Instance.isAuthorized);
		btnFacebookLogin.SetInputDelegate( OnFacebookLoginBtn);
		#else
		btnFacebookLogin.gameObject.SetActiveRecursively( false);
		#endif

		#if _USE_NAVER
		btnNaverLogin.gameObject.SetActiveRecursively( !UserData.Instance.isAuthorized);
		btnNaverLogin.SetInputDelegate( OnNaverLoginBtn);
		#else
		btnNaverLogin.gameObject.SetActiveRecursively( false);
		#endif

		#if _USE_GOOGLE
		btnGoogleLogin.gameObject.SetActiveRecursively( !UserData.Instance.isAuthorized);
		btnGoogleLogin.SetInputDelegate( OnGoogleLoginBtn);
		#else
		btnGoogleLogin.gameObject.SetActiveRecursively( false);
		#endif
		
	
		#if _USE_BAND	
		centerWithBand.gameObject.SetActiveRecursively( true);
		#if( UNITY_EDITOR || _USE_LOGIN_UI || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN)
		withBand.gameObject.SetActiveRecursively( true);
		#else
		withBand.gameObject.SetActiveRecursively( false);
		#endif
		btnBandLogin.gameObject.SetActiveRecursively( !UserData.Instance.isAuthorized);
		btnBandLogin.SetInputDelegate( OnBandLoginBtn);
		#else
		centerWithBand.gameObject.SetActiveRecursively( false);
		btnBandLogin.gameObject.SetActiveRecursively( false);
		withBand.gameObject.SetActiveRecursively( false);
		#endif
		
		
		#if _USE_YAHOO_JP
		btnYahooJpLogin.gameObject.SetActiveRecursively( !UserData.Instance.isAuthorized);
		btnYahooJpLogin.SetInputDelegate( OnYahooJpLoginBtn);
		#else
		btnYahooJpLogin.gameObject.SetActiveRecursively( false);
		#endif
		
		
		#if _USE_DEVICELOGIN
		btnDeviceLogin.gameObject.SetActiveRecursively( !UserData.Instance.isAuthorized);
		btnDeviceLogin.SetInputDelegate(OnDeviceLoginBtn);
		#else
		btnDeviceLogin.gameObject.SetActiveRecursively( false);
		#endif
		
		#if _USE_GUESTLOGIN 
			#if UNITY_ANDROID
			btnGuestLogin.gameObject.SetActiveRecursively( false);
			#endif
			
			#if UNITY_IPHONE
			btnGuestLogin.gameObject.SetActiveRecursively( !UserData.Instance.isAuthorized);
			btnGuestLogin.SetInputDelegate( OnGuestLoginBtn);
			#endif
		#else
		btnGuestLogin.gameObject.SetActiveRecursively( false);
		#endif
		
		#if _USE_TWITTERLOGIN
			btnTwitterLogin.gameObject.SetActiveRecursively( !UserData.Instance.isAuthorized);
			btnTwitterLogin.SetInputDelegate(OnTwitterLoginBtn);
		#else
			btnTwitterLogin.gameObject.SetActiveRecursively( false);
		#endif
	}

	void Awake()
	{
		btnGuestLogin.gameObject.SetActiveRecursively( false);
		btnWemeLogin.gameObject.SetActiveRecursively( false);
		btnFacebookLogin.gameObject.SetActiveRecursively( false);
		btnNaverLogin.gameObject.SetActiveRecursively( false);
		btnGoogleLogin.gameObject.SetActiveRecursively( false);
	
		btnYahooJpLogin.gameObject.SetActiveRecursively( false);
		btnDeviceLogin.gameObject.SetActiveRecursively( false);
		btnTwitterLogin.gameObject.SetActiveRecursively( false);
	}
	
	
	void SetLanguage()
	{
		AsLanguageManager.Instance.SetFontFromSystemLanguage( editID);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( editPass);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( btnWemeLogin.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( btnGuestLogin.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( btnFacebookLogin.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( btnNaverLogin.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( btnGoogleLogin.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( btnBandLogin.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( btnYahooJpLogin.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( btnDeviceLogin.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( btnTwitterLogin.spriteText);
		
		btnWemeLogin.Text = AsTableManager.Instance.GetTbl_String(1712);
		btnGuestLogin.Text = AsTableManager.Instance.GetTbl_String(4024);
		btnFacebookLogin.Text = AsTableManager.Instance.GetTbl_String(431);
		btnNaverLogin.Text = AsTableManager.Instance.GetTbl_String(433);
		btnYahooJpLogin.Text = AsTableManager.Instance.GetTbl_String(436);
		btnDeviceLogin.Text = AsTableManager.Instance.GetTbl_String(437);
		btnTwitterLogin.Text = AsTableManager.Instance.GetTbl_String(438);
		
		btnGoogleLogin.Text = AsTableManager.Instance.GetTbl_String(432);
		btnBandLogin.Text = AsTableManager.Instance.GetTbl_String(434);
		btnLogin.Text = AsTableManager.Instance.GetTbl_String(1378);
	}
	
	static public bool isFirst = true;
	// Use this for initialization
	void Start()
	{
		SetLanguage();
		SetFuntionButton();
		#if( !_USE_LOGIN_UI && !UNITY_EDITOR && !UNITY_STANDALONE_OSX && !UNITY_STANDALONE_WIN)
		_DisableLoginUI();
		#endif

		btnGuestLogin.gameObject.SetActiveRecursively( false);
		btnWemeLogin.gameObject.SetActiveRecursively( false);
		btnFacebookLogin.gameObject.SetActiveRecursively( false);
		btnNaverLogin.gameObject.SetActiveRecursively( false);
		btnGoogleLogin.gameObject.SetActiveRecursively( false);
		btnYahooJpLogin.gameObject.SetActiveRecursively( false);
		btnDeviceLogin.gameObject.SetActiveRecursively( false);
		btnTwitterLogin.gameObject.SetActiveRecursively( false);
		
		Debug.Log( "AsLoginScene::Start()");
		ip = AsNetworkDefine.LOGIN_SERVER_IP; //#19820
		AsNetworkManager.Instance.InitSocket();

		if( false == WemeSdkManager.Instance.IsStartButtonClicked)
		{
			Debug.Log( "!WemeSdkManager.Instance.IsStartButtonClicked");
			#if !( UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN)
			Debug.Log( "Start::startButtonClicked()");
			AsLoadingIndigator.Instance.ShowIndigator( "");
			WemeSdkManager.Instance.IsStartButtonClicked = true;
			WemeSdkManager.GetMainGameObject.startButtonClicked();
			#endif
		}
		else
		{
		
			#if !_USE_LOGIN_UI
			if( true == UserData.Instance.isAuthorized)
			{
				#if _USE_BAND			
					BandSdkManager.Instance.getProfile(null, BandProfileHandler );
				#else
				//	WemeLogin();
				SetVisibleTouchBtn(true);
				#endif
				
				if( true == AssetbundleManager.Instance.useAssetbundle)
					AssetbundleManager.Instance.SceneAssetbundleLoadCache( "ServerSelect");
			}
			#endif
		}

		if( false == isFirst)
			SetVisibleLoginBtn();

		if( null != AsEntityManager.Instance)
			AsEntityManager.Instance.RemoveAllEntities();

		if( null != AsUserInfo.Instance)
			AsUserInfo.Instance.Clear();

		
		
		editID.Text = strID;
		editPass.Text = strPass;

		editID.SetValidationDelegate( IdValidator);
		editID.SetValueChangedDelegate( EditValueChangeDel);
		editPass.SetValidationDelegate( PasswordValidator);
		editPass.SetValueChangedDelegate( EditValueChangeDel);

		btnLogin.SetControlState( UIButton.CONTROL_STATE.DISABLED);

		strID = PlayerPrefs.GetString( "ArkSphereID");
		editID.Text = strID;
		strPass = PlayerPrefs.GetString( "ArkSpherePass");
		editPass.Text = strPass;

		AsUserInfo.Instance.isFirstConnect = true;
		#region -AccountGender
		AsUserInfo.Instance.accountGender = eGENDER.eGENDER_NOTHING;
		#endregion
		#region -GMMark
		AsUserInfo.Instance.isGM = false;
		#endregion

		//$yde
		AsPetManager.Instance.Reserve_LoggedIn();

		if (AsNetworkDefine.GAME_MODE_REVIEW == true) 
		{
			btnInquiry.gameObject.SetActive(false);
			btnNewInfo.gameObject.SetActive(false);
			btnWebSite.gameObject.SetActive(false);
		}
	}

	public void WemeLogoutHandler( string resultString)
	{



		if( true == WemeManager.isSuccess( resultString))
		{
			UserData.Instance.isAuthorized = false;
			UserData.Instance.authData="";

			SetVisibleTouchBtn(false);

			SetVisibleLoginBtn();
			SetDisabledLoginBtn(false);
		}
		else
		{
			AsNotify.Instance.MessageBox( "WemeLogoutHandler", AsTableManager.Instance.GetTbl_String(1660),
				 AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
		}

		logString = "WemeLogoutHandler : " + " result : " + resultString;
	}

	public void OnWemeWithdraw( )
	{

		SetVisibleTouchBtn(false);
		
		SetVisibleLoginBtn();
		SetDisabledLoginBtn(false);

		AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126) , AsTableManager.Instance.GetTbl_String(4286),
		                             AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
	}


	public void InitButtonState()
	{
		AsLoadingIndigator.Instance.HideIndigator();
		UserData.Instance.isAuthorized = false;
		UserData.Instance.authData = "";
		SetVisibleLoginBtn();
		SetDisabledLoginBtn(false);
		
		AsNetworkManager.Instance.InitSocket();
		WemeSdkManager.GetMainGameObject.logoutWemeSdk( WemeLogoutHandler);
	}

	void EditValueChangeDel( IUIObject obj)
	{
		UpdateLoginBtnState();
	}

	string IdValidator( UITextField field, string text, ref int pos)
	{
		return Regex.Replace( text, "[^0-9,A-Z,a-z]", "");
	}

	string PasswordValidator( UITextField field, string text, ref int pos)
	{
		return Regex.Replace( text, "[^0-9,A-Z,a-z]", "");
	}

	void UpdateLoginBtnState()
	{
		#if( _USE_LOGIN_UI || UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN)
		if( ( 2 <= editID.text.Length) && ( 1 <= editPass.text.Length))
		{
			btnLogin.SetControlState( UIButton.CONTROL_STATE.NORMAL);
			btnLogin.spriteText.Color = Color.black;
		}
		else
		{
			btnLogin.SetControlState( UIButton.CONTROL_STATE.DISABLED);
			btnLogin.spriteText.Color = Color.gray;
		}
		#endif
	}

	// Update is called once per frame
	void Update()
	{
		#if( _USE_LOGIN_UI || UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN)
		strID = editID.Text;
		strPass = editPass.Text;
		#endif
	}

	private void ApplicationQuit()
	{
		AsUtil.Quit();
	}

	public void StartWeme( string resultString)
	{
		Debug.Log( "StartWeme()");
		AsLoadingIndigator.Instance.HideIndigator();

		#if( UNITY_EDITOR || _USE_LOGIN_UI || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN)
		btnGuestLogin.gameObject.SetActiveRecursively( false);
		#else
		if( false == WemeManager.isSuccess( resultString))
		{
			Debug.LogError( "WemeManager start failed!!!");
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), "WemeManager start failed!!!",
						this, "ApplicationQuit", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
		}
		SetVisibleLoginBtn();
		#endif

		#if !_USE_LOGIN_UI
		if( true == UserData.Instance.isAuthorized)
		{
			#if _USE_BAND			
					BandSdkManager.Instance.getProfile(null, BandProfileHandler );
			#else
			//		WemeLogin();
			SetVisibleTouchBtn(true);
			#endif
			if( true == AssetbundleManager.Instance.useAssetbundle)
				AssetbundleManager.Instance.SceneAssetbundleLoadCache( "ServerSelect");
		}
		#endif
	}

	void WemeOauthWemeHandler( string resultString)
	{
		Debug.Log( "WemeOauthWemeHandler()");
		if( true == WemeManager.isSuccess( resultString))
		{
			UserData.Instance.authData = JSONObject.Parse( resultString).GetObject( "authData").ToString();
			WemeSdkManager.GetMainGameObject.loginWemeSdk( WemeLoginHandler);
		}
		else
		{
			SetDisabledLoginBtn(false);
		}

		logString = "WemeOauthWemeHandler : " + " result : " + resultString;
		Debug.Log( logString);
	}

	void FacebookOauthWemeHandler( string resultString)
	{
		Debug.Log( "FacebookOauthWemeHandler()");
		if( true == WemeManager.isSuccess( resultString))
		{
			UserData.Instance.authData = JSONObject.Parse( resultString).GetObject( "authData").ToString();
			WemeSdkManager.GetMainGameObject.loginWemeSdk( WemeLoginHandler);
		}
		else
		{
			SetDisabledLoginBtn(false);
		}

		logString = "FacebookOauthWemeHandler : " + " result : " + resultString;
		Debug.Log( logString);
	}

	void NaverOauthWemeHandler( string resultString)
	{
		Debug.Log( "NaverOauthWemeHandler()");
		if( true == WemeManager.isSuccess( resultString))
		{
			UserData.Instance.authData = JSONObject.Parse( resultString).GetObject( "authData").ToString();
			WemeSdkManager.GetMainGameObject.loginWemeSdk( WemeLoginHandler);
		}
		else
		{
			SetDisabledLoginBtn(false);
		}

		logString = "NaverOauthWemeHandler : " + " result : " + resultString;
		Debug.Log( logString);
	}

	void GoogleOauthWemeHandler( string resultString)
	{
		Debug.Log( "GoogleOauthWemeHandler()");
		if( true == WemeManager.isSuccess( resultString))
		{
			UserData.Instance.authData = JSONObject.Parse( resultString).GetObject( "authData").ToString();
			WemeSdkManager.GetMainGameObject.loginWemeSdk( WemeLoginHandler);
		}
		else
		{
			SetDisabledLoginBtn(false);
		}

		logString = "GoogleOauthWemeHandler : " + " result : " + resultString;
		Debug.Log( logString);
	}
	
	void YahooJpOauthWemeHandler( string resultString)
	{
		Debug.Log( "YahooJpOauthWemeHandler()");
		if( true == WemeManager.isSuccess( resultString))
		{
			UserData.Instance.authData = JSONObject.Parse( resultString).GetObject( "authData").ToString();
			WemeSdkManager.GetMainGameObject.loginWemeSdk( WemeLoginHandler);
		}
		else
		{
			SetDisabledLoginBtn(false);
		}

		logString = "YahooJpOauthWemeHandler : " + " result : " + resultString;
		Debug.Log( logString);
	}
	//weme Login handler
	void WemeLoginHandler( string resultString)
	{
		if( true == WemeManager.isSuccess( resultString))
		{
			UserData.Instance.isAuthorized = true;
			WemeLogin();
		}
		else
		{
			if( true == WemeManager.isBanPlayer( resultString))
			{
				string restrCode = WemeManager.getObject( "error",resultString).GetObject( "detail").GetString( "restrCode");
				string restrReason = WemeManager.getObject( "error",resultString).GetObject( "detail").GetString( "restrReason");
				string permitTime = WemeManager.getObject( "error",resultString).GetObject( "detail").GetString( "permitTime");
				Debug.Log( "WemeLoginHandler : ban player code : "+ restrCode + " restrReason : " + restrReason + " permitTime : " + permitTime);

				AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(1584),
					this, "InitButtonState", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			}
		}

		logString = "WemeLoginHandler : " + " result : " + resultString;
		Debug.Log( logString);
	}

	private void _loginFail()
	{
		AsUtil.ShutDown( "Login server connect failed...!");
	}

	public void WemeLogin()
	{
		
		Debug.Log( "Connect to login server... " + ip + " " + AsNetworkDefine.LOGIN_SERVER_PORT);
		try
		{
			AsNetworkManager.Instance.ConnectToServer( IP, AsNetworkDefine.LOGIN_SERVER_PORT, SOCKET_STATE.SS_LOGIN);
		}
		catch( Exception e)
		{
			Debug.LogWarning( "WemeLogin : " + e);
			AsMessageBox msgBox = AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(1446), AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_ERROR);
			msgBox.SetOkDelegate = _loginFail;
			msgBox.SetCancelDelegate = _loginFail;
			return;
		}

		if( true == AsNetworkManager.Instance.IsConnected())
		{
			AsLoadingIndigator.Instance.ShowIndigator( "");
			string strAuthData = WemeManager.Instance.authData();
			if( null == strAuthData)
			{
				InitButtonState();
				return;
			}

			JSONObject jsonObject = JSONObject.Parse( strAuthData);
			JSONObject authDataJosn = jsonObject.GetObject( "authData");
			Debug.Log( "WemeLogin()strAuthData:"+ strAuthData );
			if( null == authDataJosn)
			{
				InitButtonState();
				Debug.Log( "WemeLogin()"+ jsonObject.ToString());
				return;
			}

			body_CL_WEMELOGIN wemeLogin = new body_CL_WEMELOGIN();
			JSONObject checkAuthJson = new JSONObject();
			JSONObject recordJson = new JSONObject();

			#if UNITY_EDITOR
			recordJson.Add( "os", "window");
			#elif UNITY_ANDROID
			recordJson.Add( "os", "android");
			#elif UNITY_IPHONE
			recordJson.Add( "os", "ios");
			#endif
			
			//2014.05.16
			//if( WemeSdkManager.Instance.IsGuest)
			//	recordJson.Add( "platform", "guest");
			//else
				recordJson.Add( "platform", AsGameDefine.WEME_DOMAIN);

			if( authDataJosn["authProvider"].Str.CompareTo( "weme") == 0)
			{
				wemeLogin.eLoginType = ( int)eLOGINTYPE.eLOGINTYPE_WEME;
				checkAuthJson.Add( "wemeMemberNo", authDataJosn["wemeMemberNo"].Str);
				checkAuthJson.Add( "accessToken", authDataJosn["accessToken"].Str);

				recordJson.Add( "platform", AsGameDefine.WEME_DOMAIN);
				recordJson.Add( "device", authDataJosn["uuid"].Str);
			}
			else if( authDataJosn["authProvider"].Str.CompareTo( "facebook") == 0)
			{
				wemeLogin.eLoginType = ( int)eLOGINTYPE.eLOGINTYPE_FACEBOOK;
				checkAuthJson.Add( "fbUserId", authDataJosn["fbUserId"].Str);
				checkAuthJson.Add( "accessToken", authDataJosn["accessToken"].Str);

				recordJson.Add( "platform", "facebook");
				recordJson.Add( "device", "");
			}
			else if( authDataJosn["authProvider"].Str.CompareTo( "google") == 0)
			{
				wemeLogin.eLoginType = ( int)eLOGINTYPE.eLOGINTYPE_GOOGLEPLUS;
				checkAuthJson.Add( "googleUserId", authDataJosn["googleUserId"].Str);
				checkAuthJson.Add( "accessToken", authDataJosn["accessToken"].Str);

				recordJson.Add( "platform", "google");
				recordJson.Add( "device", "");
			}
			else if( authDataJosn["authProvider"].Str.CompareTo( "naver") == 0)
			{
				wemeLogin.eLoginType = ( int)eLOGINTYPE.eLOGINTYPE_NAVER;
				checkAuthJson.Add( "naverUserId", authDataJosn["naverUserId"].Str);
				checkAuthJson.Add( "naverAliasId", "");
				checkAuthJson.Add( "accessToken", authDataJosn["accessToken"].Str);

				recordJson.Add( "platform", "naver");
				recordJson.Add( "device", "");
			}
			else if( authDataJosn["authProvider"].Str.CompareTo( "band") == 0)
			{
				wemeLogin.eLoginType = ( int)eLOGINTYPE.eLOGINTYPE_NAVER_BAND;
				checkAuthJson.Add( "bandUserId",  authDataJosn["bandUserId"].Str);	
				checkAuthJson.Add( "accessToken", authDataJosn["accessToken"].Str);
			
				
				recordJson.Add( "platform", "band");
				recordJson.Add( "device", "");
			}	
			else if( authDataJosn["authProvider"].Str.CompareTo( "device") == 0)
			{
				wemeLogin.eLoginType = ( int)eLOGINTYPE.eLOGINTYPE_DEVICE;
				checkAuthJson.Add( "deviceId",  authDataJosn["deviceId"].Str);	
				checkAuthJson.Add( "deviceSeq", authDataJosn["deviceSeq"].Str);
			
				
				recordJson.Add( "platform", "device");
				recordJson.Add( "device", "");
			}
			else if( authDataJosn["authProvider"].Str.CompareTo( "yahoojp") == 0)
			{
				wemeLogin.eLoginType = ( int)eLOGINTYPE.eLOGINTYPE_YAHOOJP;
				checkAuthJson.Add( "yahoojpUserId",  authDataJosn["yahoojpUserId"].Str);	
				checkAuthJson.Add( "yahoojpIdToken", authDataJosn["yahoojpIdToken"].Str);
				checkAuthJson.Add( "yahoojpNonce",   authDataJosn["yahoojpNonce"].Str);
				
				recordJson.Add( "platform", "yahoojp");
				recordJson.Add( "device", "");
			}	
			else if( authDataJosn["authProvider"].Str.CompareTo( "twitter") == 0)
			{
				wemeLogin.eLoginType = ( int)eLOGINTYPE.eLOGINTYPE_TWITTER;
				checkAuthJson.Add( "twitterUserId", 	  authDataJosn["twitterUserId"].Str);	
				checkAuthJson.Add( "accessToken",   	  authDataJosn["accessToken"].Str);
				checkAuthJson.Add( "accessTokenSecret",   authDataJosn["accessTokenSecret"].Str);
				checkAuthJson.Add( "consumerKey",  	      authDataJosn["consumerKey"].Str);
				checkAuthJson.Add( "consumerSecret",  	  authDataJosn["consumerSecret"].Str);
				
				recordJson.Add( "platform", "twitter");
				recordJson.Add( "device", "");
			}	
			
			byte[] srcAuth = System.Text.UTF8Encoding.UTF8.GetBytes( checkAuthJson.ToString());
			System.Buffer.BlockCopy( srcAuth, 0, wemeLogin.strCheckAuth, 0, srcAuth.Length);
			byte[] srcRecord = System.Text.UTF8Encoding.UTF8.GetBytes( recordJson.ToString());
			System.Buffer.BlockCopy( srcRecord, 0, wemeLogin.strRecord, 0, srcRecord.Length);

			Debug.Log( "strCheckAuth()" + checkAuthJson.ToString() + "srcRecord" + recordJson.ToString());
			wemeLogin.bIsGuest = false;

			wemeLogin.bIsAccountDeleteCancel = AsUserInfo.Instance.isAccountDeleteCancel;
			AsUserInfo.Instance.isAccountDeleteCancel = false;

			byte[] packet = wemeLogin.ClassToPacketBytes();
			AsNetworkMessageHandler.Instance.Send( packet);

			WemeSdkManager.Instance.IsWemeLogin = true;
			string resultString = WemeManager.Instance.isAllowPushMessage();
			if( false == AsGameMain.GetOptionState( OptionBtnType.OptionBtnType_Push))
			{
				if( true == WemeManager.isSuccess( resultString))
				{
					bool allow = JSONObject.Parse( resultString).GetBoolean( "allow");
					if( true == allow)
						WemeSdkManager.GetMainGameObject.setPushAllow();
				}
			}
			else
			{
				if( true == WemeManager.isSuccess( resultString))
				{
					bool allow = JSONObject.Parse( resultString).GetBoolean( "allow");
					if( false == allow)
						WemeSdkManager.GetMainGameObject.setPushAllow();
				}
			}

			AsPartyTrackManager.Instance.SetEvent(AsPartyTrackManager.ePartyTrackEvent.eCompletion_Loading);
			#if ( !UNITY_EDITOR && !UNITY_STANDALONE_OSX && !UNITY_STANDALONE_WIN)
			UserData.Instance.playerKey = WemeManager.getString( "playerKey", WemeManager.Instance.playerKey(), "");
			#endif
		}
	}

	void SetDisabledLoginBtn( bool disabled)
	{
		if( true == disabled)
		{
			#if !_USE_BAND		
			btnWemeLogin.SetControlState( UIButton.CONTROL_STATE.DISABLED);
			btnWemeLogin.controlIsEnabled = false;
			#endif
			#if _USE_FACEBOOK
			btnFacebookLogin.SetControlState( UIButton.CONTROL_STATE.DISABLED);
			btnFacebookLogin.controlIsEnabled = false;
			#endif
			#if _USE_NAVER
			btnNaverLogin.SetControlState( UIButton.CONTROL_STATE.DISABLED);
			btnNaverLogin.controlIsEnabled = false;
			#endif
			#if _USE_GOOGLE
			btnGoogleLogin.SetControlState( UIButton.CONTROL_STATE.DISABLED);
			btnGoogleLogin.controlIsEnabled = false;
			#endif			
			#if _USE_BAND
			btnBandLogin.SetControlState( UIButton.CONTROL_STATE.DISABLED);
			btnBandLogin.controlIsEnabled = false;
			#endif
			
			#if _USE_GUESTLOGIN && UNITY_IPHONE
			btnGuestLogin.SetControlState( UIButton.CONTROL_STATE.DISABLED);
			btnGuestLogin.controlIsEnabled = false;
			#endif		
			
			#if _USE_YAHOO_JP
			btnYahooJpLogin.SetControlState( UIButton.CONTROL_STATE.DISABLED);
			btnYahooJpLogin.controlIsEnabled = false;
			#endif
			
			#if _USE_DEVICELOGIN
			btnDeviceLogin.SetControlState( UIButton.CONTROL_STATE.DISABLED);
			btnDeviceLogin.controlIsEnabled = false;
			#endif
			
			
			#if _USE_TWITTERLOGIN
			btnTwitterLogin.SetControlState( UIButton.CONTROL_STATE.DISABLED);
			btnTwitterLogin.controlIsEnabled = false;
			#endif
			
		}
		else
		{
			#if !_USE_BAND			
			btnWemeLogin.SetControlState( UIButton.CONTROL_STATE.NORMAL);
			btnWemeLogin.controlIsEnabled = true;
			#endif
			
			#if _USE_FACEBOOK
			btnFacebookLogin.SetControlState( UIButton.CONTROL_STATE.NORMAL);
			btnFacebookLogin.controlIsEnabled = true;
			#endif
			#if _USE_NAVER
			btnNaverLogin.SetControlState( UIButton.CONTROL_STATE.NORMAL);
			btnNaverLogin.controlIsEnabled = true;
			#endif
			#if _USE_GOOGLE
			btnGoogleLogin.SetControlState( UIButton.CONTROL_STATE.NORMAL);
			btnGoogleLogin.controlIsEnabled = true;
			#endif
			
			#if _USE_BAND
			btnBandLogin.SetControlState( UIButton.CONTROL_STATE.NORMAL);
			btnBandLogin.controlIsEnabled = true;
			#endif
			
			#if _USE_GUESTLOGIN && UNITY_IPHONE
			btnGuestLogin.SetControlState( UIButton.CONTROL_STATE.NORMAL);
			btnGuestLogin.controlIsEnabled = true;
			#endif		
			
			#if _USE_YAHOO_JP
			btnYahooJpLogin.SetControlState( UIButton.CONTROL_STATE.NORMAL);
			btnYahooJpLogin.controlIsEnabled = true;
			#endif
			
			#if _USE_DEVICELOGIN
			btnDeviceLogin.SetControlState( UIButton.CONTROL_STATE.NORMAL);
			btnDeviceLogin.controlIsEnabled = true;
			#endif
			
			#if _USE_TWITTERLOGIN
			btnTwitterLogin.SetControlState( UIButton.CONTROL_STATE.NORMAL);
			btnTwitterLogin.controlIsEnabled = true;
			#endif
		}
	}

	private void OnWemeLoginBtn( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			Debug.Log( "OnWemeLoginBtn()");
			#if !UNITY_EDITOR
			SetDisabledLoginBtn(true);
			if( false == UserData.Instance.isAuthorized)
			{
				Debug.Log( "OnWemeLoginBtn::loginWeme()");
				WemeSdkManager.GetMainGameObject.loginWeme( WemeOauthWemeHandler);
			}
			else
			{
				Debug.Log( "OnWemeLoginBtn::WemeLogin()");
				WemeLogin();
			}

			if( true == AssetbundleManager.Instance.useAssetbundle)
				AssetbundleManager.Instance.SceneAssetbundleLoadCache( "ServerSelect");
			#endif
		}
	}

	private void OnFacebookLoginBtn( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			Debug.Log( "OnFacebookLoginBtn()");
			#if !UNITY_EDITOR
			SetDisabledLoginBtn(true);
			if( false == UserData.Instance.isAuthorized)
			{
				Debug.Log( "OnFacebookLoginBtn::loginFacebook()");
				WemeSdkManager.GetMainGameObject.loginFacebook( FacebookOauthWemeHandler);
			}
			else
			{
				Debug.Log( "OnFacebookLoginBtn::WemeLogin()");
				WemeLogin();
			}

			if( true == AssetbundleManager.Instance.useAssetbundle)
				AssetbundleManager.Instance.SceneAssetbundleLoadCache( "ServerSelect");
			#endif
		}
	}

	private void OnNaverLoginBtn( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			Debug.Log( "OnNaverLoginBtn()");
			#if !UNITY_EDITOR
			SetDisabledLoginBtn(true);

			if( false == UserData.Instance.isAuthorized)
			{
				Debug.Log( "OnNaverLoginBtn::loginNaver()");
				WemeSdkManager.GetMainGameObject.loginNaver( NaverOauthWemeHandler);
			}
			else
			{
				Debug.Log( "OnNaverLoginBtn::WemeLogin()");
				WemeLogin();
			}

			if( true == AssetbundleManager.Instance.useAssetbundle)
				AssetbundleManager.Instance.SceneAssetbundleLoadCache( "ServerSelect");
			#endif
		}
	}
	
	private void OnGoogleLoginBtn( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			Debug.Log( "OnGoogleLoginBtn()");
			#if !UNITY_EDITOR
			SetDisabledLoginBtn(true);

			if( false == UserData.Instance.isAuthorized)
			{
				Debug.Log( "OnGoogleLoginBtn::loginGoogle()");
				WemeSdkManager.GetMainGameObject.loginGoogle( GoogleOauthWemeHandler);
			}
			else
			{
				Debug.Log( "OnGoogleLoginBtn::WemeLogin()");
				WemeLogin();
			}

			if( true == AssetbundleManager.Instance.useAssetbundle)
				AssetbundleManager.Instance.SceneAssetbundleLoadCache( "ServerSelect");
			#endif
		}
	}
	
	private void OnYahooJpLoginBtn( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			Debug.Log( "OnYahooJpLoginBtn()");
			#if !UNITY_EDITOR
			SetDisabledLoginBtn(true);

			if( false == UserData.Instance.isAuthorized)
			{
				Debug.Log( "OnYahooJpLoginBtn::loginYahooJp()");
				WemeSdkManager.GetMainGameObject.loginYahoo( YahooJpOauthWemeHandler);
			}
			else
			{
				Debug.Log( "OnGoogleLoginBtn::WemeLogin()");
				WemeLogin();
			}

			if( true == AssetbundleManager.Instance.useAssetbundle)
				AssetbundleManager.Instance.SceneAssetbundleLoadCache( "ServerSelect");
			#endif
		}
	}
	private void OnBandLoginBtn( ref POINTER_INFO ptr)
	{
		#if _USE_BAND
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			Debug.Log( "OnBandLoginBtn()");
			#if !UNITY_EDITOR
			SetDisabledLoginBtn(true);

			Debug.Log( "OnBandLoginBtn::getProfile()");
			BandSdkManager.Instance.getProfile(null, BandProfileHandler );

			if( true == AssetbundleManager.Instance.useAssetbundle)
				AssetbundleManager.Instance.SceneAssetbundleLoadCache( "ServerSelect");
			#endif
		}
		#endif
	}
	#if _USE_BAND
	public void BandProfileHandler( string resultString)
	{
		logString = "BandProfileHandler : " + " result : " + resultString;
		Debug.Log( logString);
		
		if(resultString == "Fail")
		{	
			//잠시후에 다시 시도...
			SetDisabledLoginBtn(false);
			return; 
		}			
	
		JSONObject authDataJson = new JSONObject();
		authDataJson.Add("authProvider","band");
		authDataJson.Add("accessToken",BandSdkManager.Instance.AccessToken);
		authDataJson.Add("bandUserId",BandSdkManager.Instance.BandUserKey);
		authDataJson.Add("bandClientId",BandSdkManager.Instance.BAND_CLIENT_ID);
		authDataJson.Add("bandClientSecret",BandSdkManager.Instance.BAND_CLIENT_SECRET);
		UserData.Instance.authData = authDataJson.ToString();			
		Debug.Log( "BandProfileHandler authDataJson ::" + authDataJson.ToString());
		WemeSdkManager.GetMainGameObject.loginWemeSdk( WemeLoginHandler);
			
	}
	#endif
	public override void Enter()
	{
//		btnGuestLogin.gameObject.SetActiveRecursively( false);
//		btnWemeLogin.gameObject.SetActiveRecursively( false);
//		btnFacebookLogin.gameObject.SetActiveRecursively( false);
//		btnNaverLogin.gameObject.SetActiveRecursively( false);
//		btnGoogleLogin.gameObject.SetActiveRecursively( false);
	}

	public override void Exit()
	{
		SetFadeMode( transform, FADEMODE.FM_OUT);

		AsFadeObject fo = transform.parent.parent.GetComponent<AsFadeObject>();
		if( null != fo)
			fo.SetFadeMode( FADEMODE.FM_OUT);
	}

	private void SetFadeMode( Transform trans, FADEMODE mode)
	{
		AsFadeObject fo = trans.GetComponent<AsFadeObject>();
		if( null != fo)
			fo.SetFadeMode( mode);

		int numChild = trans.childCount;
		for( int i = 0; i < numChild; i++)
		{
			Transform childTrans = trans.GetChild( i);

			fo = childTrans.GetComponent<AsFadeObject>();
			if( null != fo)
				fo.SetFadeMode( mode);

			SetFadeMode( childTrans, mode);
		}
	}

	private void OnBtnLogin()
	{
		AsSoundManager.Instance.PlaySound( "S6002_EFF_Button", Vector3.zero, false);

		Debug.Log( "Connect to login server... " + ip + " " + AsNetworkDefine.LOGIN_SERVER_PORT);

		AsNetworkManager.Instance.ConnectToServer( IP, AsNetworkDefine.LOGIN_SERVER_PORT, SOCKET_STATE.SS_LOGIN);
		if( true == AsNetworkManager.Instance.IsConnected())
		{
			AsUserInfo.curID = ID;
			AsUserInfo.curPass = Password;

			AS_CL_LOGIN login = new AS_CL_LOGIN( ID, Password,AsUserInfo.Instance.isAccountDeleteCancel);
			byte[] data = login.ClassToPacketBytes();
			AsNetworkMessageHandler.Instance.Send( data);

			if( true == AssetbundleManager.Instance.useAssetbundle)
				AssetbundleManager.Instance.SceneAssetbundleLoadCache( "ServerSelect");

			AsUserInfo.Instance.isAccountDeleteCancel = false;

			PlayerPrefs.SetString( "ArkSphereID", ID);
			PlayerPrefs.SetString( "ArkSpherePass", Password);
			PlayerPrefs.Save();

			btnLogin.SetControlState( UIButton.CONTROL_STATE.DISABLED);
		}
		else {
			Debug.Log("couldn't connect server!!!");
		}
	}
	
	#region -_USE_DEVICELOGIN -_USE_GUESTLOGIN 
	private void OnDeviceLoginBtn( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			#if !UNITY_EDITOR
			string strTitle = AsTableManager.Instance.GetTbl_String(2792);
			string strMsg = AsTableManager.Instance.GetTbl_String(2793);
			AsMessageBox	box = AsNotify.Instance.MessageBox( strTitle, strMsg, this, "OnMsgBox_DeviceLogin_Ok", "OnMsgBox_DeviceLogin_Cancel", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);
			BoxCollider col = box.gameObject.GetComponent<BoxCollider> ();
			if (col != null) 
			{
				Vector3	boxSize = new Vector3(80,60,0);
				col.size = boxSize;
			}
			#endif
		}
	}

	private void OnMsgBox_DeviceLogin_Ok()
	{
		if( false == UserData.Instance.isAuthorized)
		{
			SetDisabledLoginBtn(true);
			WemeSdkManager.GetMainGameObject.loginDevice(WemeOauthDeviceHandler);
		}
		else
		{
			WemeLogin();
		}
		
		if( true == AssetbundleManager.Instance.useAssetbundle)
			AssetbundleManager.Instance.SceneAssetbundleLoadCache( "ServerSelect");		
	}

	private void OnMsgBox_DeviceLogin_Cancel()
	{
	}

	private void OnGuestLoginBtn( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			SetDisabledLoginBtn(true);
			
			#if !UNITY_EDITOR
			GuestLogin();
			#endif
		}
	}

	

	private void GuestLogin()
	{
		WemeSdkManager.Instance.IsGuest = true;
		//디바이스 로그인.		
		if( false == UserData.Instance.isAuthorized)
		{
			Debug.Log( "btnGuestLogin::GuestLogin()");
			WemeSdkManager.GetMainGameObject.loginDevice(WemeOauthDeviceHandler);
		}
		else
		{
			Debug.Log( "btnGuestLogin::WemeLogin()");
			WemeLogin();
		}
		
		if( true == AssetbundleManager.Instance.useAssetbundle)
				AssetbundleManager.Instance.SceneAssetbundleLoadCache( "ServerSelect");		
	
	}
	
	void WemeOauthDeviceHandler(string resultString){
		if(WemeManager.isSuccess(resultString)){
			UserData.Instance.authData = JSONObject.Parse(resultString).GetObject("authData").ToString();
			WemeSdkManager.GetMainGameObject.loginWemeSdk( WemeLoginHandler);
		}
		else {
			SetDisabledLoginBtn(false);
		}
		logString = "WemeOauthDeviceHandler : " + " result : "  + resultString;
		Debug.Log( logString);
	}
	#endregion
	
	#region -_USE_TWITTERLOGIN 
	private void OnTwitterLoginBtn( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			#if !UNITY_EDITOR
			if( false == UserData.Instance.isAuthorized)
			{
				SetDisabledLoginBtn(true);
				Debug.Log( "OnTwitterLoginBtn()");
				WemeSdkManager.GetMainGameObject.loginTwitter(TwitterOauthWemeHandler);
			}
			else
			{
				Debug.Log( "TwitterLoginBtn::WemeLogin()");
				WemeLogin();
			}
			
			if( true == AssetbundleManager.Instance.useAssetbundle)
					AssetbundleManager.Instance.SceneAssetbundleLoadCache( "ServerSelect");		
			#endif
		}
	}
	
	void TwitterOauthWemeHandler( string resultString)
	{
		Debug.Log( "TwitterOauthWemeHandler()");
		if( true == WemeManager.isSuccess( resultString))
		{
			UserData.Instance.authData = JSONObject.Parse( resultString).GetObject( "authData").ToString();
			WemeSdkManager.GetMainGameObject.loginWemeSdk( WemeLoginHandler);
		}
		else
		{
			SetDisabledLoginBtn(false);
		}

		logString = "NaverOauthWemeHandler : " + " result : " + resultString;
		Debug.Log( logString);
	}
	#endregion
	
	#region funtion button
	
	private void SetFuntionButton()
	{
		AsLanguageManager.Instance.SetFontFromSystemLanguage( btnNewInfo.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( btnWebSite.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( btnInquiry.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( btnDeviceChange.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( btnAccountSetting.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( btnTouch.spriteText);

		
		btnNewInfo.Text = AsTableManager.Instance.GetTbl_String(445);
		btnWebSite.Text = AsTableManager.Instance.GetTbl_String(443);
		btnInquiry.Text = AsTableManager.Instance.GetTbl_String(444);
		btnDeviceChange.Text = AsTableManager.Instance.GetTbl_String(435);
		btnAccountSetting.Text = AsTableManager.Instance.GetTbl_String(4013);
//		btnTouch.Text = AsTableManager.Instance.GetTbl_String(2324);
		
		btnNewInfo.SetInputDelegate(OnNewInfoBtn);
		btnWebSite.SetInputDelegate(OnWebSiteBtn);
		btnInquiry.SetInputDelegate(OnInquiryBtn);
		btnDeviceChange.SetInputDelegate(OnDeviceChangeBtn);
		btnAccountSetting.SetInputDelegate(OnAccountSettingBtn);
		btnTouch.SetInputDelegate(OnTouchBtn);

		btnTouch.SetControlState( UIButton.CONTROL_STATE.NORMAL);
		btnTouch.controlIsEnabled = true;
		
		SetVisibleTouchBtn(false);
		
	
	}
	
	private void SetVisibleTouchBtn(bool bVisible)
	{
		btnTouch.gameObject.SetActiveRecursively(bVisible);

	}
	
	private void OnNewInfoBtn( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			#if UNITY_IPHONE || UNITY_ANDROID
			string url = string.Empty;
			url = AsTableManager.Instance.GetTbl_String(446);
			WemeSdkManager.GetMainGameObject.showPlatformViewWeb( url);
			#endif
		}
	}
	
	private void OnWebSiteBtn( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			#if UNITY_IPHONE || UNITY_ANDROID
			string url = string.Empty;
			url = AsTableManager.Instance.GetTbl_String(439);
			WemeSdkManager.GetMainGameObject.showPlatformViewWeb( url);
			#endif
		}
	}
	
	private void OnInquiryBtn( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			if(!UserData.Instance.isAuthorized)
			{
				AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(2358),
					 AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
				return;
			}
			#if ( !UNITY_EDITOR && !UNITY_STANDALONE_OSX && !UNITY_STANDALONE_WIN)
				WemeSdkManager.GetMainGameObject.showPlatformViewMain();	
			#endif
		/*	StringBuilder sb1 = new StringBuilder();
			#if ( !UNITY_EDITOR && !UNITY_STANDALONE_OSX && !UNITY_STANDALONE_WIN)
					StringBuilder sb = new StringBuilder();
					sb.AppendFormat( "mailto:Arkhelp@cs.wemade.com?subject={0}&body=Wemeplayerkey: {1}", string.Empty, UserData.Instance.playerKey);
					Application.OpenURL( sb.ToString());
			#endif
		*/	
		}
	}
	
	
	private void OnDeviceChangeBtn( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			if( !UserData.Instance.isDeviceLogin  ) //#27188
			{
				AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(2323),
					 AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
				return;
			}

			#if ( !UNITY_EDITOR && !UNITY_STANDALONE_OSX && !UNITY_STANDALONE_WIN)
			if(WemeSdkManager.Instance.IsDeviceLogin())
				WemeSdkManager.GetMainGameObject.requestDeviceChange(WemeOauthRequestDeviceChangeHandler);
			#endif
		}
	}
	
	void WemeOauthRequestDeviceChangeHandler(string resultString){
		Debug.Log("WemeOauthRequestDeviceChangeHandler "+resultString);
		if(WemeManager.isSuccess(resultString)){
			//if device change ->>> logut and reLogin
			WemeSdkManager.GetMainGameObject.logoutWemeSdk( WemeLogoutHandler);

			/*27173//if success logout and wemeSDKLogin
			string authData = JSONObject.Parse(resultString).GetObject("authData").ToString();
			if(authData.Length>0){
				UserData.Instance.authData = authData;
			//if device change ->>> logut and reLogin
				WemeSdkManager.GetMainGameObject.logoutWemeSdk( WemeLogoutHandler);
			}
			*/
		}
//		else
//		{
//			AsNotify.Instance.MessageBox( "WemeOauthRequestDeviceChangeHandler", AsTableManager.Instance.GetTbl_String(1660),
//			                             AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
//		}
	}
	
	private void OnAccountSettingBtn( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			if(!UserData.Instance.isAuthorized)
			{
				AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(2358),
					 AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
				return;
			}
			if( null != accountDlg)
				return;
	
			accountDlg = Instantiate( ResourceLoad.LoadGameObject( "UI/AsGUI/GUI_AccountEdit_Popup")) as GameObject;
			AsAccountDlg dlg = accountDlg.GetComponentInChildren<AsAccountDlg>();
			dlg.ParentObject = gameObject;
		
		}
	}
	
	private void CloseAccountDlg()
	{
		if( null == accountDlg)
			return;

		GameObject.DestroyImmediate( accountDlg);
		accountDlg = null;
	}
	
	private void OpenWithdrawDlg()
	{
		if( null != withdrawDlg)
			return;

		withdrawDlg = Instantiate( ResourceLoad.LoadGameObject( "UI/KAKAO/Prefab/GUI_KAKAOedit_Withdraw")) as GameObject;
		AsWithdrawDlg dlg = withdrawDlg.GetComponentInChildren<AsWithdrawDlg>();
		dlg.ParentObject = gameObject;
	}

	private void CloseWithdrawDlg()
	{
		if( null == withdrawDlg)
			return;

		GameObject.DestroyImmediate( withdrawDlg);
		withdrawDlg = null;
	}
	
	private void OnTouchBtn( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			StartCoroutine( CheckMaintenance());
//			if( false == CheckMaintenance())
//			   return;
//
//			btnTouch.SetControlState( UIButton.CONTROL_STATE.DISABLED);
//			btnTouch.controlIsEnabled = false;
//			WemeLogin();
		}
	}
	
	IEnumerator CheckMaintenance()
	{
		// FT MODIFY
		/*
#if ( !UNITY_EDITOR && ( UNITY_ANDROID || UNITY_IPHONE))
		StringBuilder sb = new StringBuilder();
		sb.AppendFormat( "{0}{1}&domain={2}&game={3}&version={4}", AsNetworkDefine.WemeGateBaseUrl, market, domain, AsGameDefine.GAME_CODE, AsGameDefine.BUNDLE_VERSION);
		
		WWW www = new WWW( sb.ToString());
		yield return www;

		if( null != www.error)
		{
			Debug.Log( "WemeGateManager ShutDown");
			AsUtil.ShutDown( string.Empty);
			yield break;
		}
		
		JSONObject topObj = JSONObject.Parse( www.text);
		www.Dispose();
		www = null;

		JSONObject maintenance = topObj.GetObject( "maintenance");
		if( null != maintenance)
		{
			UInt64 curTime = UInt64.Parse( topObj[ "timestamp"].Str);
			string maintenanceBegin = maintenance[ "begin"].Str;
			string maintenanceEnd = maintenance[ "end"].Str;
			UInt64 beginTime = UInt64.Parse( maintenanceBegin);
			UInt64 endTime = UInt64.Parse( maintenanceEnd);
			
			if( ( curTime >= beginTime) && ( curTime <= endTime))
				yield break;
		}
#endif
*/
		btnTouch.SetControlState( UIButton.CONTROL_STATE.DISABLED);
		btnTouch.controlIsEnabled = false;
		WemeLogin();
		yield break;
	}

	#endregion
}