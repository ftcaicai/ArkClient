using UnityEngine;
using System.Collections;
using System.Net;

public class AsLoginProcess : AsProcessBase
{
	private bool m_bServerListResponse = false;
	private bool isAutoServerSelected = false;
	static public int nLastWorldIdxBuf = 0;

	public override void Process( byte[] _packet)
	{
		// 서버와의 데이터오더 문제로 3이어야 하나 2로 해야 Protocol이 얻어짐.
		switch( (PROTOCOL_LOGIN)_packet[2])
		{
		case PROTOCOL_LOGIN.LC_LIVE_ECHO:
			LiveEcho( _packet);
			break;
		case PROTOCOL_LOGIN.LC_LOGIN_RESULT:
			LoginResult( _packet);
			break;
		case PROTOCOL_LOGIN.LC_SERVERLIST_RESULT:
			ServerListResult( _packet);
			break;
		#region -WEME Certify
		case PROTOCOL_LOGIN.LC_WEMELOGIN_RESULT:
			WemeLoginResult( _packet);
			break;
		case PROTOCOL_LOGIN.LC_ACCOUNT_DELETE:
			AccountDelete( _packet);
			break;
		#endregion
		}
	}

	void Update()
	{
		if( false == m_bServerListResponse)
			return;

		if( true == AssetbundleManager.Instance.useAssetbundle)
		{
			if( false == isAutoServerSelected)
			{
				if( true == AssetbundleManager.Instance.isLoadedScene( "ServerSelect"))
				{
					Application.LoadLevel( "ServerSelect");
					DDOL_Tracer.BeginTrace();//$ yde
					Resources.UnloadUnusedAssets();
					m_bServerListResponse = false;
					isAutoServerSelected = false;
				}
			}
			else
			{
				DDOL_Tracer.BeginTrace();//$ yde
				Resources.UnloadUnusedAssets();
				m_bServerListResponse = false;
				isAutoServerSelected = false;
			}
		}
		else
		{
			if( false == isAutoServerSelected)
				Application.LoadLevel( "ServerSelect");

			DDOL_Tracer.BeginTrace();//$ yde
			Resources.UnloadUnusedAssets();
			m_bServerListResponse = false;
			isAutoServerSelected = false;
		}
	}

	private void LiveEcho( byte[] _packet)
	{
	}

	private void ServerListResult( byte[] _packet)
	{
		Debug.Log( "ServerListResult");

		AS_LC_SERVERLIST_COUNT serverCount = new AS_LC_SERVERLIST_COUNT();
		serverCount.PacketBytesToClass( _packet);

		AsServerListData.Instance.serverNewCnt = serverCount.nServerNewCnt;
		AsServerListData.Instance.Clear();
		foreach( AS_LC_SERVERLIST_BODY body in serverCount.body)
			AsServerListData.Instance.InsertData( body);

		m_bServerListResponse = true;

		AsNetworkManager.Instance.InitSocket();

		Debug.Log( "isAutoServerSelected : " + isAutoServerSelected);
		Debug.Log( "isFirstConnect : " + AsUserInfo.Instance.isFirstConnect);

		AS_LC_SERVERLIST_BODY serverData = AsServerListData.Instance.GetData(0);
		AsServerListBtn.SetServerListBody(serverData);
		Debug.Assert( null != serverData);
//		if( ( 0 <= serverData.nCurUser) && ( AsGameDefine.SERVER_USER_MAX > serverData.nCurUser) &&
//			( 0 != serverCount.nLastWorldIdx) && ( true == AsUserInfo.Instance.isFirstConnect))
//			FirstConnect( AsServerListData.Instance.GetData(0));

		nLastWorldIdxBuf = serverCount.nLastWorldIdx;
	}

	private void FirstConnect( AS_LC_SERVERLIST_BODY serverData)
	{
		string ip = System.Text.UTF8Encoding.UTF8.GetString( serverData.szIpaddress);
		AsNetworkManager.Instance.ConnectToServer( ip, serverData.nPort, SOCKET_STATE.SS_GAMESERVER);
		if( true == AsNetworkManager.Instance.IsConnected())
		{
			Debug.Log( "Connect game server...");
			Debug.Log( "AsUserInfo.Instance.isWemeCertified : " + AsUserInfo.Instance.isWemeCertified);

			body_CG_LOGIN_POSSIBLE possible = new body_CG_LOGIN_POSSIBLE();
			byte[] data = possible.ClassToPacketBytes();
			AsNetworkMessageHandler.Instance.Send( data);

			AsUserInfo.Instance.CurrentServerID = serverData.nWorldIdx;
			AsSocialManager.Instance.ServerName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( serverData.szServerName));
			Debug.Log("body_CG_LOGIN_POSSIBLE is send");

			if( true == AssetbundleManager.Instance.useAssetbundle)
			{
				AssetbundleManager.Instance.SceneAssetbundleLoadCache( "CharacterSelect");
				AssetbundleManager.Instance.SceneAssetbundleLoadCache( "CharacterCreate");
				AssetbundleManager.Instance.SceneAssetbundleLoadCache( "ChannelSelect");
			}

			isAutoServerSelected = true;
		}
	}

	private void LoginResult( byte[] _packet)
	{
		Debug.Log ("Login Result..");
		AS_LC_LOGIN_RESULT userInfo = new AS_LC_LOGIN_RESULT();
		userInfo.PacketBytesToClass( _packet);

		AsReviewConditionManager.Instance.SetCondition_LoginResult (userInfo);

		switch( userInfo.eResult)
		{
		case eRESULTCODE.eRESULT_SUCC:
			break;
		case eRESULTCODE.eRESULT_FAIL_BAN_LOGIN:
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(1653));
			return;
		case eRESULTCODE.eRESULT_FAIL_GM_EDIT:
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(1652));
			return;

		case eRESULTCODE.eRESULT_FAIL_ACCOUNT_DELETED:
			{
				//	confirm msg box , "are you cancel account delete?"
				AsMessageBox accountDeleteCancelDlg = AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(4093),
				                                                                   this, "CancelAccountDeleteOK", "CancelAccountDeleteKeep", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);
				
			}
			return;

		default:
			AsNotify.Instance.MessageBox( "Error", "Login failed...!", null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_ERROR);
			return;
		}
		WemeSdkManager.GetMainGameObject.requestGameServer();
		Debug.Log( "LoginResult LoginUserUniqueKey:" + userInfo.nUserUniqKey.ToString());
		AsUserInfo.Instance.LoginUserUniqueKey = userInfo.nUserUniqKey;
		AsUserInfo.Instance.LoginUserSessionKey = userInfo.nUserSessionKey;

		Debug.Log( "Request server list");
		AS_CL_SERVERLIST serverList = new AS_CL_SERVERLIST();
		byte[] data = serverList.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
		AsCommonSender.SetSocketState( SOCKET_STATE.SS_LOGIN);
	}

	#region -WEME Certify
	private void WemeLoginResult( byte[] _packet)
	{
		body_LC_WEMELOGIN_RESULT result = new body_LC_WEMELOGIN_RESULT();
		result.PacketBytesToClass( _packet);

		AsReviewConditionManager.Instance.SetCondition_WemeLoginResult (result);

		switch( result.eResult)
		{
		case eRESULTCODE.eRESULT_WEMEALOGIN_USERINFOREQ:
		case eRESULTCODE.eRESULT_SUCC:
			break;
		case eRESULTCODE.eRESULT_SUCC_REENTRANCE:
//				AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(4093));
			break;
		case eRESULTCODE.eRESULT_FAIL_WITHDRAWAL_ONEDAY:
			{
				AsLoadingIndigator.Instance.HideIndigator();
				GameObject go = GameObject.Find( "LoginFramework");
				if( null != go)
				{
					AsLoginScene loginScene = go.GetComponentInChildren<AsLoginScene>();
					if( null != loginScene)
					{
						AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(1584),
						loginScene, "InitButtonState", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
					}
				}
			}
			return;
		case eRESULTCODE.eRESULT_FAIL_BAN_LOGIN:
			{
				AsLoadingIndigator.Instance.HideIndigator();
				GameObject go = GameObject.Find( "LoginFramework");
				if( null != go)
				{
					AsLoginScene loginScene = go.GetComponentInChildren<AsLoginScene>();
					if( null != loginScene)
					{
						AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(1653),
						loginScene, "InitButtonState", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
					}
				}
			}
			return;
		case eRESULTCODE.eRESULT_FAIL_GM_EDIT:
			{
				AsLoadingIndigator.Instance.HideIndigator();
				GameObject go = GameObject.Find( "LoginFramework");
				if( null != go)
				{
					AsLoginScene loginScene = go.GetComponentInChildren<AsLoginScene>();
					if( null != loginScene)
					{
						AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(1652),
						loginScene, "InitButtonState", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
					}
				}
			}
			return;

		case eRESULTCODE.eRESULT_FAIL_ACCOUNT_DELETED:
			{
				AsLoadingIndigator.Instance.HideIndigator();

				//	confirm msg box , "are you cancel account delete?"
				AsMessageBox accountDeleteCancelDlg = AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(4093),
			                                                                   this, "CancelAccountDeleteOK", "CancelAccountDeleteKeep", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);

			}
			return;

		default:
			Debug.LogWarning( "WemeLoginResult : " + result.eResult);
			return;
		}

		if(WemeSdkManager.Instance.IsWithdrawLogin)
		{
			Debug.Log( "Withdraw");
			body_CL_ACCOUNT_DELETE delete = new body_CL_ACCOUNT_DELETE();
			byte[] packet = delete.ClassToPacketBytes();
			AsNetworkMessageHandler.Instance.Send( packet);
			WemeSdkManager.Instance.IsWithdrawLogin = false;
			AsLoadingIndigator.Instance.HideIndigator();//#19616.
			return;
		}

		Debug.Log( "WemeLoginResult LoginUserUniqueKey:" + result.nUserUniqKey);
		AsUserInfo.Instance.LoginUserUniqueKey = result.nUserUniqKey;
		AsUserInfo.Instance.WemeAuthToken = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( result.strAccessToken));

		AsUserInfo.Instance.isWemeCertified = true;
//		AsUserInfo.Instance.isGuest = result.bIsGuest;
		WemeSdkManager.Instance.IsConfirmGuest = result.bIsGuest; 	//2014.05.16
		Debug.Log( "result.IsConfirmGuest : " + result.bIsGuest);

		Debug.Log( "result.IsConfirmGuest : " + result.bIsGuest);

		Debug.Log( "Request server list");
		AS_CL_SERVERLIST serverList = new AS_CL_SERVERLIST();
		byte[] data = serverList.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
		AsCommonSender.SetSocketState( SOCKET_STATE.SS_LOGIN);

		AsLoadingIndigator.Instance.HideIndigator();
	}

	void CancelAccountDeleteOK()
	{
		//	account delete cancel and send login
		GameObject go = GameObject.Find( "LoginFramework");
		if( null != go)
		{
			AsLoginScene loginScene = go.GetComponentInChildren<AsLoginScene>();
			if( null != loginScene)
			{
				AsUserInfo.Instance.isAccountDeleteCancel = true;

				if( WemeSdkManager.Instance.IsWemeLogin == true )
				{
					loginScene.WemeLogin();
				}
				else
				{
					AS_CL_LOGIN login = new AS_CL_LOGIN( AsUserInfo.curID, AsUserInfo.curPass,AsUserInfo.Instance.isAccountDeleteCancel);
					byte[] data = login.ClassToPacketBytes();
					AsNetworkMessageHandler.Instance.Send( data);
					
					if( true == AssetbundleManager.Instance.useAssetbundle)
						AssetbundleManager.Instance.SceneAssetbundleLoadCache( "ServerSelect");
					
					AsUserInfo.Instance.isAccountDeleteCancel = false;
				}
			}
		}
	}
	
	void CancelAccountDeleteKeep()
	{
		//	account delete keep and stay login scene
		AsLogoutManager.Instance.ProcessLogout ();
	}	


	private void AccountDelete( byte[] _packet)
	{
		Debug.Log( "AccountDelete");
		body_LC_ACCOUNT_DELETE result = new body_LC_ACCOUNT_DELETE();
		result.PacketBytesToClass( _packet);

		if( eRESULTCODE.eRESULT_SUCC != result.eResult)
		{
			Debug.LogWarning( "AccountDelete : " + result.eResult);
			return;
		}

		WemeSdkManager.GetMainGameObject.withdrawWemeSdk(WemeWithdrawHandler);
	
	//	ToLoginScene();#26958

	
	}

	void WemeWithdrawHandler( string resultString){
		if(WemeManager.isSuccess(resultString)){
			UserData.Instance.isAuthorized = false;
			UserData.Instance.authData="";
		
			
			GameObject go = GameObject.Find( "LoginFramework");
			if( null != go)
			{
				AsLoginScene loginScene = go.GetComponentInChildren<AsLoginScene>();
				if( null != loginScene)
					loginScene.OnWemeWithdraw( );
				else
					Debug.LogError("AsLoginProcess::WemeWithdrawHandler( null != loginScene)");
			}	


		}
		Debug.Log(resultString);
	}

	/*
	private void ToLoginScene()
	{
		if( null != AsChatManager.Instance)
			AsChatManager.Instance.ClearAllChat();
		if( null != AsHudDlgMgr.Instance)
		{
			AsHudDlgMgr.Instance.CollapseMenuBtn();	// #10694
			AsHudDlgMgr.Instance.CloseMsgBox();
			AsHudDlgMgr.Instance.invenPageIdx = 0;
		}
		if( null != AsServerListData.Instance)
			AsServerListData.Instance.Clear();
		if( null != AsNetworkManager.Instance)
			AsNetworkManager.Instance.InitSocket();
		AsHUDController.SetActiveRecursively( false);
		AsCommonSender.ResetSendCheck();
		PlayerBuffMgr.Instance.Clear();


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

		Application.LoadLevel( "Login");
		DDOL_Tracer.BeginTrace();//$ yde
		Resources.UnloadUnusedAssets();
		
		AsLoginScene.isFirst = false;
	}
	*/
	#endregion
}
