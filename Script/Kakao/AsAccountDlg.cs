using UnityEngine;
using System.Collections;
using System.Text;
using WmWemeSDK.JSON;

public class AsAccountDlg : MonoBehaviour
{
	[SerializeField] SpriteText txtAccountManage = null;
	[SerializeField] SpriteText txtVersionInfo = null;
	[SerializeField] SpriteText txtAccountInfo = null;
	[SerializeField] UIButton btnWithdraw = null;
//	[SerializeField] UIButton btnInquiry = null;
	[SerializeField] UIButton btnLogout = null;
//	[SerializeField] UIButton btnDeviceChange = null;
	private GameObject goParent = null;
	public GameObject ParentObject
	{
		set	{ goParent = value; }
	}

	// Use this for initialization
	void Start()
	{
		string playerKey = string.Empty;
		string platform = string.Empty;

#if ( !UNITY_EDITOR && !UNITY_STANDALONE_OSX && !UNITY_STANDALONE_WIN)
		UserData.Instance.playerKey = WemeManager.getString( "playerKey", WemeManager.Instance.playerKey(), "");
#endif

		txtAccountManage.Text = AsTableManager.Instance.GetTbl_String(4013);
		txtVersionInfo.Text = "Ver : " + VersionManager.version;
		txtAccountInfo.Text = string.Format( "{0}{1}", AsTableManager.Instance.GetTbl_String(4014), UserData.Instance.playerKey);
		btnWithdraw.Text = AsTableManager.Instance.GetTbl_String(4016);
		btnLogout.Text = AsTableManager.Instance.GetTbl_String(809);
	//	btnInquiry.Text = AsTableManager.Instance.GetTbl_String(4017);
	//	btnDeviceChange.Text = AsTableManager.Instance.GetTbl_String(435);
	//	btnDeviceChange.SetInputDelegate(DeviceChange);
	//	if(WemeSdkManager.Instance.IsDeviceLogin())
	//		AsUtil.SetButtonState(btnDeviceChange ,UIButton.CONTROL_STATE.NORMAL);
	//	else
	//		AsUtil.SetButtonState(btnDeviceChange ,UIButton.CONTROL_STATE.DISABLED);
	}

	// Update is called once per frame
	void Update()
	{
	}

	private void OnCloseBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		goParent.BroadcastMessage( "CloseAccountDlg", SendMessageOptions.DontRequireReceiver);
	}

	private void OnWithdrawBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		goParent.BroadcastMessage( "CloseAccountDlg", SendMessageOptions.DontRequireReceiver);
		//2014.05.16
		if( false == WemeSdkManager.Instance.IsGuest)
			goParent.BroadcastMessage( "OpenWithdrawDlg", SendMessageOptions.DontRequireReceiver);
	}

/*	private void OnInquiryBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

#if ( !UNITY_EDITOR && !UNITY_STANDALONE_OSX && !UNITY_STANDALONE_WIN)
		StringBuilder sb = new StringBuilder();
		sb.AppendFormat( "mailto:Arkhelp@cs.wemade.com?subject={0}&body=Wemeplayerkey: {1}", string.Empty, UserData.Instance.playerKey);
		Application.OpenURL( sb.ToString());
#endif
	}
*/
	private void OnLogoutBtn()
	{
		//AsLoadingIndigator.Instance.ShowIndigator("");

		if( AsLogoutManager.Instance.ActiveLogoutCoroutine == true )
			return;

		goParent.BroadcastMessage( "CloseAccountDlg", SendMessageOptions.DontRequireReceiver);
		
//		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
//		WemeSdkManager.GetMainGameObject.logoutWemeSdk( WemeLogoutHandler);

		AsLogoutManager.Instance.ProcessLogout ();
	}

	void WemeLogoutHandler( string resultString)
	{
		AsLoginScene loginScene = goParent.GetComponentInChildren<AsLoginScene>();
		if( null != loginScene)
			loginScene.WemeLogoutHandler( resultString);
		else
			Debug.LogError("AsAccountDlg::WemeLogoutHandler( null != loginScene)");
	}

	/*
	private void DeviceChange(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			#if ( !UNITY_EDITOR && !UNITY_STANDALONE_OSX && !UNITY_STANDALONE_WIN)
			if(WemeSdkManager.Instance.IsDeviceLogin())
				WemeSdkManager.GetMainGameObject.requestDeviceChange(WemeOauthRequestDeviceChangeHandler);
			#endif
		}
	}
	
	void WemeOauthRequestDeviceChangeHandler(string resultString){
		Debug.Log("WemeOauthRequestDeviceChangeHandler "+resultString);
		if(WemeManager.isSuccess(resultString)){
			//if success logout and wemeSDKLogin
			string authData = JSONObject.Parse(resultString).GetObject("authData").ToString();
			if(authData.Length>0){
				UserData.Instance.authData = authData;
			//if device change ->>> logut and reLogin
				WemeSdkManager.GetMainGameObject.logoutWemeSdk( WemeLogoutHandler);
			}
			
		}
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
		if( null != AsSocialManager.Instance)
		{
			AsSocialManager.Instance.Clear();
			AsSocialManager.Instance.LogOut();
		}
		if( null != AsChatManager.Instance)
			AsChatManager.Instance.ClearAllChat();
		if( null != AsHudDlgMgr.Instance)
			AsHudDlgMgr.Instance.CollapseMenuBtn();	// #10694
		AsServerListData.Instance.Clear();
		AsNetworkManager.Instance.InitSocket();
		AsHUDController.SetActiveRecursively( false);
		AsCommonSender.ResetSendCheck();
		PlayerBuffMgr.Instance.Clear();
		if( null != AsHudDlgMgr.Instance)
		{
			AsHudDlgMgr.Instance.CloseMsgBox();
			AsHudDlgMgr.Instance.invenPageIdx = 0;
			AsHudDlgMgr.Instance.CloseSystemDlg();
		}
		if( null != TerrainMgr.Instance)
			TerrainMgr.Instance.Clear();
		ItemMgr.HadItemManagement.Inven.ClearInvenItems();
		ItemMgr.HadItemManagement.Storage.ClearStorageItems();//$yde
		if( null != SkillBook.Instance)
			SkillBook.Instance.InitSkillBook();
		if( null != CoolTimeGroupMgr.Instance)
			CoolTimeGroupMgr.Instance.Clear();
		if( null != ArkQuestmanager.instance)
			ArkQuestmanager.instance.ResetQuestManager();
		if( null != AsPartyManager.Instance)
		{
			AsPartyManager.Instance.PartyDiceRemoveAll();//#11954
			AsPartyManager.Instance.PartyUserRemoveAll();
		}
		if( null != AsUserInfo.Instance)
			AsUserInfo.Instance.Clear();

		AsLoadingIndigator.Instance.ShowIndigator( "");
		if( null != AsInputManager.Instance)
			AsInputManager.Instance.m_Activate = false;

		if( null != AsEntityManager.Instance)
		{
			AsEntityManager.Instance.RemoveAllEntities();
			while( true == AsEntityManager.Instance.ModelLoading || true == AsEntityManager.Instance.DestroyingEntity)
				yield return null;
		}

		AsLoadingIndigator.Instance.HideIndigator();
		if( null != AsInputManager.Instance)
			AsInputManager.Instance.m_Activate = true;

		Application.LoadLevel( "Login");
		DDOL_Tracer.BeginTrace();//$ yde
		Resources.UnloadUnusedAssets();

		AsLoginScene.isFirst = false;
	}
	*/
}
