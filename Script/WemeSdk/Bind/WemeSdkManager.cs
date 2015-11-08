using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using WmWemeSDK.JSON;

public class WemeSdkManager : MonoBehaviour {
	
	private static WemeSdkManager instance_ = null;
	
	public static WemeSdkManager Instance
    {
        get
        {
            if( null == instance_ )
            {
                instance_ = FindObjectOfType( typeof( WemeSdkManager ) ) as WemeSdkManager;
                if( null == instance_ )
                {
                    Debug.Log( "Fail to get WemeSdkManager Instance" );
                }
            }
            return instance_;
        }
    }
	#if UNITY_EDITOR
		[DllImport("WemeSDKPlugin")]
		private static extern string test();

	#endif	
	
	
	// kakao & weme object name
	[SerializeField] public string wemeObjectName_;
	
	// weme setting 
//	[SerializeField] public string serverZone_;
	
	// gameCode
	//[SerializeField] public string gameCode_;
		
	// gameDomain kakao,weme
//	[SerializeField] public string domain_;
	
	// gameVersion
	//[SerializeField] public string gameVersion_;
	
	// market
	//[SerializeField] public string store_;
	
	
	// auth data
	// weme
	[SerializeField] public string weme_clientId_;
	
	[SerializeField] public string weme_clientSecret_;
	
	// facebook
	[SerializeField] public string facebook_appId_;
	
	[SerializeField] public string facebook_redirectUri_;
	
	// twitter
	[SerializeField] public string twitter_consumerKey_;
	
	[SerializeField] public string twitter_consumerSecret_;
	
	[SerializeField] public string twitter_callbackUrl_;
	
	// google
	[SerializeField] public string google_clientId_;
	
	[SerializeField] public string google_clientSecret_;
	
	// naver
	[SerializeField] public string naver_clientId_;
	
	[SerializeField] public string naver_clientSecret_;
	
	[SerializeField] public string naver_redirectUri_;
	
	
	
	[SerializeField] public string yahoo_clientId_;
	

	
//	public bool CheckGuestUser
//	{
//		get { return IsConfirmGuest || IsServiceGuest; }
//	
//	}
	
	private bool isConfirmGuest = false; // true : confirm guest Mode
	public bool IsConfirmGuest
	{
		get { return isConfirmGuest; }
		set	{ isConfirmGuest = value; }
	}
	
	
	public bool IsServiceGuest
	{
		get { return !isConfirmGuest && IsGuest; }
	}
	
	private bool isGuest = false; // true : guest Mode
	public bool IsGuest
	{
		get { return isGuest; }
		set	{ isGuest = value; }
	}
	
	public bool IsDeviceLogin()
	{
		#if( UNITY_EDITOR || _USE_LOGIN_UI || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN)
		return false;
		#else
		UserData.Instance.isDeviceLogin = JSONObject.Parse(GetMainGameObject.isDeviceLogin()).GetBoolean("deviceLogin");
		 return UserData.Instance.isDeviceLogin; 
		#endif
		
	}
	
	
	
	private bool isWemeLogin = false;
	public bool IsWemeLogin
	{
		get { return isWemeLogin; }
		set	{ isWemeLogin = value; }
	}
	
	private bool isWithdrawLogin = false;
	public bool IsWithdrawLogin
	{
		get { return isWithdrawLogin; }
		set	{ isWithdrawLogin = value; }
	}
		
	
	
	private bool isStartButtonClicked = false;
	public bool IsStartButtonClicked
	{
		get { return isStartButtonClicked; }
		set	{ isStartButtonClicked = value; }
	} 
	
	public static MainGameObject GetMainGameObject
	{
		get { return MainGameObject.Instance; }	
	}
	// Use this for initialization
	
	void Start () {
		isConfirmGuest = false; 
		isWemeLogin = false;
		isGuest = false; 
		Debug.Log("WemeSdkManager Start");	
		gameObject.AddComponent<MainGameObject>();
		gameObject.AddComponent<UserData>();
		// init wemehandler 
		if(wemeObjectName_==null){
			Debug.Log("not null wemeObject name");
		}else{
			GameObject wemehandler = new GameObject( wemeObjectName_ );
			wemehandler.transform.localPosition = Vector3.zero;
			wemehandler.transform.rotation = Quaternion.identity;
			wemehandler.transform.localScale = Vector3.one;
			//add WemeManager.cs Component to handler object		
			wemehandler.AddComponent<WemeManager>();
			WemeManager.Instance.initWemeManager(wemehandler);
			//WemeManager.Instance.initWemeManager(wemeObjectName_);
			GameObject.DontDestroyOnLoad(wemehandler);
			wemehandler.transform.parent = gameObject.transform;						
		   
		}
	
	}
	
	public void WemeLogout()
	{
		
#if( UNITY_EDITOR || _USE_LOGIN_UI || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN)
		UserData.Instance.isAuthorized = false;
		UserData.Instance.authData = "";
		StartCoroutine( "ToLoginScene");		
#else
		WemeSdkManager.GetMainGameObject.logoutWemeSdk( WemeLogoutHandler);	
#endif
	
		
	}
	
	void WemeLogoutHandler( string resultString)
	{
		AsLoadingIndigator.Instance.HideIndigator();
		if( WemeManager.isSuccess( resultString))
		{
			UserData.Instance.isAuthorized = false;
			UserData.Instance.authData = "";
			StartCoroutine( "ToLoginScene");
		}
		else
		{
			if( ( RuntimePlatform.OSXEditor == Application.platform) || ( RuntimePlatform.WindowsEditor == Application.platform))
			{
				#if UNITY_EDITOR
				if( RuntimePlatform.OSXEditor == Application.platform)
				{
					UserData.Instance.isAuthorized = false;
					UserData.Instance.authData = "";
					StartCoroutine( "ToLoginScene");
					return;
				}
				#endif
			}
			
			AsNotify.Instance.MessageBox( "WemeLogoutHandler", AsTableManager.Instance.GetTbl_String(1660),
				AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
		
		}

		Debug.Log( resultString);
	}
	
	private IEnumerator ToLoginScene()
	{
		AsSocialManager.Instance.Clear();
		AsSocialManager.Instance.LogOut();
		AsChatManager.Instance.ClearAllChat();
		AsHudDlgMgr.Instance.CollapseMenuBtn();	// #10694
		AsServerListData.Instance.Clear();
		AsNetworkManager.Instance.InitSocket();
		AsHUDController.SetActiveRecursively( false);
		AsCommonSender.ResetSendCheck();
		PlayerBuffMgr.Instance.Clear();
		AsHudDlgMgr.Instance.CloseMsgBox();
		TerrainMgr.Instance.Clear();
		ItemMgr.HadItemManagement.Inven.ClearInvenItems();
		ItemMgr.HadItemManagement.Storage.ClearStorageItems();//$yde
		SkillBook.Instance.InitSkillBook();
		CoolTimeGroupMgr.Instance.Clear();
		ArkQuestmanager.instance.ResetQuestManager();
		AsPartyManager.Instance.PartyDiceRemoveAll();//#11954
		AsPartyManager.Instance.PartyUserRemoveAll();
		AsUserInfo.Instance.Clear();
		AsHudDlgMgr.Instance.invenPageIdx = 0;
		
		AsEntityManager.Instance.RemoveAllEntities();
		AsLoadingIndigator.Instance.ShowIndigator( "");
		AsInputManager.Instance.m_Activate = false;
		
		while( true == AsEntityManager.Instance.ModelLoading || true == AsEntityManager.Instance.DestroyingEntity)
			yield return null;
		
		AsLoadingIndigator.Instance.HideIndigator();
		AsInputManager.Instance.m_Activate = true;
		AsHudDlgMgr.Instance.CloseSystemDlg();

		Application.LoadLevel( "Login");
		DDOL_Tracer.BeginTrace();//$ yde
		Resources.UnloadUnusedAssets();
		
		AsLoginScene.isFirst = false;
	}
	
}
