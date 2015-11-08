using UnityEngine;
using System.Collections;

public class AsLogoutManager : MonoBehaviour 
{
	static AsLogoutManager m_instance;
	public static AsLogoutManager Instance{ get{  return m_instance;}}

	private bool m_bActiveLogoutCoroutine = false;

	public bool ActiveLogoutCoroutine
	{
		get{return m_bActiveLogoutCoroutine;}
		set{m_bActiveLogoutCoroutine = value;}
	}

	void Awake()
	{
		m_instance = this;
	}

	// Use this for initialization
	void Start () 
	{
		m_bActiveLogoutCoroutine = false;
	}

	void OnEnable()
	{		
		m_bActiveLogoutCoroutine = false;
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
			m_bActiveLogoutCoroutine = false;
		}
		
		Debug.Log( resultString);
	}

	public void ProcessLogout()
	{
		AsPvpManager.Instance.isMatching = false;
		AsInstanceDungeonManager.Instance.isMatching = false;
		
		AsLoadingIndigator.Instance.ShowIndigator("");
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		//2014.05.16 dopamin
		//if( true == AsUtil.CheckGuestUser())
		//	return;
		
		m_bActiveLogoutCoroutine = true;
		
		#if( UNITY_EDITOR || _USE_LOGIN_UI || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN)
		UserData.Instance.isAuthorized = false;
		UserData.Instance.authData = "";
		StartCoroutine( "ToLoginScene");		
		#else
		WemeSdkManager.GetMainGameObject.logoutWemeSdk( WemeLogoutHandler);	
		#endif

	}

	public void GotoLoginSceneNotLogout()
	{
		StartCoroutine( "ToLoginScene");
	}

	private IEnumerator ToLoginScene()
	{
		AsSocialManager.Instance.Clear();
		AsSocialManager.Instance.LogOut();

		if( AsChatManager.Instance != null )
			AsChatManager.Instance.ClearAllChat();

		if (AsHudDlgMgr.Instance != null) 
		{
			AsHudDlgMgr.Instance.CollapseMenuBtn ();	// #10694
			AsHudDlgMgr.Instance.CloseMsgBox();
			AsHudDlgMgr.Instance.CloseSystemDlg();
			AsHudDlgMgr.Instance.invenPageIdx = 0;
		}

		AsServerListData.Instance.Clear();
		AsNetworkManager.Instance.InitSocket();

		if( AsHUDController.Instance != null )
			AsHUDController.SetActiveRecursively( false);

		AsCommonSender.ResetSendCheck();
		PlayerBuffMgr.Instance.Clear();

		TerrainMgr.Instance.Clear();
		ItemMgr.HadItemManagement.Inven.ClearInvenItems();
		ItemMgr.HadItemManagement.Storage.ClearStorageItems();//$yde
		SkillBook.Instance.InitSkillBook();
		CoolTimeGroupMgr.Instance.Clear();
		ArkQuestmanager.instance.ResetQuestManager();
		AsPartyManager.Instance.PartyDiceRemoveAll();//#11954
		AsPartyManager.Instance.PartyUserRemoveAll();
		AsUserInfo.Instance.Clear();

		AsInGameEventManager.instance.Reset();
		AutoCombatManager.Instance.ExitInGame();//$yde
		
		AsEntityManager.Instance.RemoveAllEntities();
		AsLoadingIndigator.Instance.ShowIndigator( "");
		AsInputManager.Instance.m_Activate = false;
		
		while( true == AsEntityManager.Instance.ModelLoading || true == AsEntityManager.Instance.DestroyingEntity)
			yield return null;
		
		AsLoadingIndigator.Instance.HideIndigator();
		AsInputManager.Instance.m_Activate = true;

		
		Application.LoadLevel( "Login");
		DDOL_Tracer.BeginTrace();//$ yde
		Resources.UnloadUnusedAssets();
		
		AsLoginScene.isFirst = false;

		m_bActiveLogoutCoroutine = false;
	}

	// Update is called once per frame
	void Update () 
	{
	
	}
}
