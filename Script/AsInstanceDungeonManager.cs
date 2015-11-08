#define INTEGRATED_ARENA_MATCHING
#define INDUN_TIME
#define INDUN_EXCHANGE_GOLD
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsInstanceDungeonManager : MonoBehaviour
{
	static AsInstanceDungeonManager m_instance;
	public static AsInstanceDungeonManager Instance{ get{  return m_instance;}}

	private int m_nIndunTableIndex = 0;
//	private int m_nIndunDuplicationIndex = 0;
	private int m_nIndunBranchTableIndex = 0;
	private bool m_isMatching = false;
	private float m_fGoIntoDelayTime = 0.0f;
	private float m_fGoIntoTimeBuf = 0.0f;
	private int m_nGoIntoCountCheck = 0;
	private bool m_isCounting = false;
	private AsMessageBox m_msgboxIndunMatchingError = null;
	private AsMessageBox m_msgboxIndunMatchingError_Cash = null;
	private AsMessageBox m_msgboxIndunMatchingCancel = null;
	private AsMessageBox m_msgboxIndunExit = null;
	private AsIndunRewardDlg m_IndunRewardDlg = null;
	private AsIndunQuestInfoDlg m_IndunQuestInfoDlg = null;
	private AsIndunFirstRewardDlg m_FirstRewardDlg = null;
	private ushort m_nGameUserSessionIndexBuff = 0;
	private body_SC_INDUN_QUEST_PROGRESS_INFO m_nIndunQuestProgressInfoBuff = new body_SC_INDUN_QUEST_PROGRESS_INFO();
	private bool m_bLimitCountCoroutine = false;
	private short m_LimitCount = 0;
	private bool m_bStartOnce = false;
	private uint m_nCurClearCharUniqKey = 0;
	private List<int> m_listClearBranchIndex = new List<int>();
	private bool m_bOpenIndunQuestInfoDlgCoroutine = false;
	private bool m_bFirstRewardDlgOpen = false;
	private Dictionary<int, int> m_dicHellModeCount = new Dictionary<int, int>();
	private bool m_bReconnect = false;

	public short LimitCount { get{ return m_LimitCount;}}
	public PackedSprite m_MatchingEff = null;
	public bool bIndunLogoutCoroutine = false;

	public bool isMatching
	{
		get
		{
			return m_isMatching;
		}
		set
		{
			m_isMatching = value;
			int nStringIndex = m_isMatching ? 900 : 1689;

			if( AsHudDlgMgr.Instance != null )
				AsHudDlgMgr.Instance.SetIndunBtnText( AsTableManager.Instance.GetTbl_String( nStringIndex));
			
			if( false == m_isMatching)
			{
				if( GAME_STATE.STATE_WARP_SEND == AsGameMain.s_gameState)
					AsGameMain.s_gameState = GAME_STATE.STATE_INGAME;
				AsInputManager.Instance.m_Activate = true;
			}
			
			SetActiveMatchingEff( m_isMatching);
		}
	}

	void Awake()
	{
		m_instance = this;
	}

	void Start()
	{
		m_msgboxIndunMatchingCancel = ( new GameObject( "msgboxIndunMatchingCancel")).AddComponent<AsMessageBox>();
		CloseIndunMatchingCancelMsgBox();

		m_msgboxIndunMatchingError = ( new GameObject( "msgboxIndunMatchingError")).AddComponent<AsMessageBox>();
		CloseIndunMatchingErrorMsgBox();
		
		m_msgboxIndunMatchingError_Cash = ( new GameObject( "msgboxIndunMatchingError_Cash")).AddComponent<AsMessageBox>();
		CloseIndunMatchingErrorMsgBox_Cash();

		m_msgboxIndunExit = ( new GameObject( "msgboxIndunExit")).AddComponent<AsMessageBox>();
		CloseIndunExitMsgBox();
	}
	
	void Update()
	{
		/*
		// test
		if( true == TargetDecider.CheckCurrentMapIsIndun())
		{
			if( Input.GetKeyUp( KeyCode.Alpha1))
				Send_InDun_Exit();
		}
		if( Input.GetKeyUp( KeyCode.Alpha1))
			OpenFirstRewardDlg_Coroutine();
		*/

		if( true == TargetDecider.CheckCurrentMapIsIndun())
		{
			if( false == IsOpenIndunQuestInfoDlg && m_nIndunQuestProgressInfoBuff.nInsQuestGroupTableUniqCode > 0 && false == m_bOpenIndunQuestInfoDlgCoroutine)
				StartCoroutine( OpenIndunQuestInfoDlg());

			if( true == IsOpenIndunRewardDlg)
				CloseIndunExitMsgBox();
		}
		else
		{
			if( true == IsOpenIndunQuestInfoDlg)
				CloseIndunQuestInfoDlg();

			if( true == IsOpenIndunRewardDlg)
				CloseIndunRewardDlg();

			CloseIndunExitMsgBox();
		}
	}
	
	public void Switch_GameUserSessionIndex(ushort IntegratedServer_SessionIndex = 0)
	{
		if( 0 == IntegratedServer_SessionIndex)
		{
			AsUserInfo.Instance.GamerUserSessionIdx = m_nGameUserSessionIndexBuff;
			AsUserInfo.Instance.SavedCharStat.sessionKey_ = m_nGameUserSessionIndexBuff;
		}
		else
		{
			m_nGameUserSessionIndexBuff = AsUserInfo.Instance.GamerUserSessionIdx;
			AsUserInfo.Instance.GamerUserSessionIdx = IntegratedServer_SessionIndex;
			AsUserInfo.Instance.SavedCharStat.sessionKey_ = IntegratedServer_SessionIndex;
		}
	}
	
	public void Send_InDun_Create(int nInDunTableIndex, int nIndunBranchTableIndex)
	{
#if INTEGRATED_ARENA_MATCHING
		if( false == _CheckSendIndunMatching())
		{
			OpenIndunMatchingErrorMsgBox( AsTableManager.Instance.GetTbl_String( 1894));
			return;
		}
		
		body_CS_INTEGRATED_INDUN_MATCHING_REQUEST data = new body_CS_INTEGRATED_INDUN_MATCHING_REQUEST( true, nInDunTableIndex, nIndunBranchTableIndex);
		byte[] sendData = data.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( sendData);
#else
		body_CS_INSTANCE_CREATE data = new body_CS_INSTANCE_CREATE( nInDunTableIndex);
		byte[] sendData = data.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( sendData);
#endif
		
		m_nIndunTableIndex = nInDunTableIndex;
		m_nIndunBranchTableIndex = nIndunBranchTableIndex;
		InitIndunQuestProgressInfoBuff();
	}
	
	/* Not Used
	public void Send_InDun_Enter()
	{
		if( 0 == m_nIndunTableIndex || 0 == m_nIndunDuplicationIndex)
		{
			return;
		}
		
		body_CS_ENTER_INSTANCE data = new body_CS_ENTER_INSTANCE( m_nIndunTableIndex, m_nIndunDuplicationIndex);
		byte[] sendData = data.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( sendData);
	}
	*/
	
	public void Send_InDun_Enter_End()
	{
#if INTEGRATED_ARENA_MATCHING
		body_CS_INTEGRATED_INDUN_LOGIN_END data = new body_CS_INTEGRATED_INDUN_LOGIN_END( m_bReconnect);
		byte[] sendData = data.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( sendData);

		_RequestDimpleLog_Indun( 160405);

		if( true == TargetDecider.CheckCurrentMapIsIndun())
		{
			if( AsHudDlgMgr.Instance != null )
				AsHudDlgMgr.Instance.SetIndunBtnText( AsTableManager.Instance.GetTbl_String( 2346)); // indun exit text
		}
#else
		body_CS_ENTER_INSTANCE_END data = new body_CS_ENTER_INSTANCE_END();
		byte[] sendData = data.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( sendData);
#endif
	}
	
	public void Send_InDun_Exit()
	{
#if INTEGRATED_ARENA_MATCHING
		body_CS_INTEGRATED_INDUN_LOGOUT data = new body_CS_INTEGRATED_INDUN_LOGOUT();
		byte[] sendData = data.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( sendData);

		_RequestDimpleLog_Indun( 160406);


		// disconnect integrated server
		AsNetworkIntegratedManager.Instance.SwitchServer();
		
		if( false == AsNetworkIntegratedManager.Instance.IsConnected())
		{
			_RequestDimpleLog_Indun( 160407);

			body_CS_RETURN_WORLD data2 = new body_CS_RETURN_WORLD();
			byte[] sendData2 = data2.ClassToPacketBytes();
			AsNetworkMessageHandler.Instance.Send( sendData2);

			_RequestDimpleLog_Indun( 160408);
		}

		
		AsPartyManager.Instance.PartyUserRemoveAll();
#else
		body_CS_EXIT_INSTANCE data = new body_CS_EXIT_INSTANCE();
		byte[] sendData = data.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( sendData);
#endif
	}

	public IEnumerator Send_InDun_Exit_Coroutine()
	{
		bIndunLogoutCoroutine = true;
		
		yield return new WaitForSeconds( 0.3f);
		
		// disconnect integrated server
		AsNetworkIntegratedManager.Instance.SwitchServer();
		AsNetworkManager.Instance.Stop_SendAlive_GameServer();
		
		yield return new WaitForSeconds( 0.3f);
		
		if( false == AsNetworkIntegratedManager.Instance.IsConnected())
		{
			_RequestDimpleLog_Indun( 160407);
			
			body_CS_RETURN_WORLD data2 = new body_CS_RETURN_WORLD();
			byte[] sendData2 = data2.ClassToPacketBytes();
			AsNetworkMessageHandler.Instance.Send( sendData2);
			
			_RequestDimpleLog_Indun( 160408);
		}
		
		CloseIndunRewardDlg();
		bIndunLogoutCoroutine = false;
	}

	public void Send_InDun_Exit_End()
	{
#if INTEGRATED_ARENA_MATCHING
		body_CS_RETURN_WORLD_END data = new body_CS_RETURN_WORLD_END();
		byte[] sendData = data.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( sendData);

		_RequestDimpleLog_Indun( 160410);

		OpenFirstRewardDlg_Coroutine();
//		AsCommonSender.SendLevelActiveWaypoint();
#else
		body_CS_EXIT_INSTANCE_END data = new body_CS_EXIT_INSTANCE_END();
		byte[] sendData = data.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( sendData);
#endif
	}
	
	public void Send_InDun_Reward_GetItem()
	{
#if INTEGRATED_ARENA_MATCHING
		body_CS_INDUN_REWARD_GETITEM data = new body_CS_INDUN_REWARD_GETITEM();
		byte[] sendData = data.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( sendData);
#endif
	}

	/* Not Used
	public void Recv_InDun_Create_Result(body_SC_INSTANCE_CREATE_RESULT data)
	{
		// InDun create failed
		_Error( data.eResult);
		Debug.Log( "Recv_InDun_Create_Result() - result code: " + data.eResult + ", InDunTableIndex: " + data.nIndunTableIndex);
	}
	
	public void Recv_InDun_Create(body_SC_INSTANCE_CREATE data)
	{
		m_nIndunTableIndex = data.nIndunTableIndex;
		m_nIndunDuplicationIndex = data.nIndunDuplicationIndex;
		
		// test, you want a indun
		Send_InDun_Enter();
	}
	*/
	
	public void Recv_InDun_Enter(body_SC_ENTER_INSTANCE_RESULT data, int nIndunTableIndex, bool bReconnect)
	{
		_RequestDimpleLog_Indun( 160404);

		// map load( field -> InDun)
		if( data.eResult == eRESULTCODE.eRESULT_SUCC)
		{
			m_bReconnect = bReconnect;
			AsUserInfo.Instance.SetCurrentUserCharacterPosition( data.vPosition);
			
			GameObject go = GameObject.Find( "SceneLoader");
			AsSceneLoader sceneLoader = go.GetComponent<AsSceneLoader>() as AsSceneLoader;
			
			Tbl_InDun_Record record = AsTableManager.Instance.GetInDunRecord( nIndunTableIndex);
			if( null == record)
			{
				Debug.LogError( "AsInstanceDungeonManager::Recv_InDun_Enter(), null == record, nIndunTableIndex: " + nIndunTableIndex);
				return;
			}
			
			if( 0 == record.IndunType) // 0: indun, 1: pvp
				sceneLoader.Load( data.nMapIndex, AsSceneLoader.eLoadType.INSTANCE_DUNGEON_ENTER);
			else
				sceneLoader.Load( data.nMapIndex, AsSceneLoader.eLoadType.ARENA_ENTER);
		}
		else
		{
			_Error( data.eResult);
			Debug.Log( "Recv_InDun_Enter() - result code: " + data.eResult + ", MapIndex: " + data.nMapIndex);
		}
	}
	
	public void Recv_InDun_Exit(body_SC_EXIT_INSTANCE_RESULT data)
	{
		_RequestDimpleLog_Indun( 160409);

		// map load( InDun -> field)
		if( data.eResult == eRESULTCODE.eRESULT_SUCC)
		{
			StartCoroutine( InDunExitLoadCheck(data) );
		}
		else
		{
			_Error( data.eResult);
			Debug.Log( "Recv_InDun_Exit() - result code: " + data.eResult + ", MapIndex: " + data.nMapIndex);
		}
	}

	IEnumerator InDunExitLoadCheck( body_SC_EXIT_INSTANCE_RESULT data )
	{
		GameObject go = GameObject.Find( "SceneLoader");
		AsSceneLoader sceneLoader = go.GetComponent<AsSceneLoader>() as AsSceneLoader;

		while( sceneLoader.ActiveCoroutineMapCheck == true )
		{
			yield return null;
		}

		AsUserInfo.Instance.SetCurrentUserCharacterPosition( data.vPosition);

		sceneLoader.Load( data.nMapIndex, AsSceneLoader.eLoadType.INSTANCE_DUNGEON_EXIT);
	}
	
	public void SetActiveMatchingEff(bool bActive)
	{
		if( null != m_MatchingEff)
		{
			m_MatchingEff.enabled = bActive;
			m_MatchingEff.renderer.enabled = bActive;
		}
	}

	public void OnBtnIndunInfo()
	{
		if( false == m_isMatching && null == m_msgboxIndunMatchingCancel)
		{
//			if( false == CheckInIndun())
//				Send_Indun_Matching_Info();
			if( true == TargetDecider.CheckCurrentMapIsIndun())
			{
				OpenIndunExitMsgBox();
			}
			else
			{
				Send_Indun_Matching_Info();
			}
		}
		else
		{
			OpenIndunMatchingCancelMsgBox();
		}
	}
	
	public bool CheckMatching()
	{
		if( true == m_isMatching)
		{
			AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage( AsTableManager.Instance.GetTbl_String( 2283));
			return true;
		}
		
		return false;
	}
	
	public bool CheckInIndun()
	{
		if( true == TargetDecider.CheckCurrentMapIsIndun())
		{
			AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage( AsTableManager.Instance.GetTbl_String( 2286));
			
			AsSoundManager.Instance.PlaySound_VoiceBattle(eVoiceBattle.str999_Cannot_Use_In_Raid);
			
			return true;
		}
		
		return false;
	}

	public IEnumerator IndunMatchingGoInto_Coroutine()
	{
		m_isCounting = true;
		
		if( 0.0f == m_fGoIntoDelayTime)
		{
			Tbl_GlobalWeight_Record record = AsTableManager.Instance.GetTbl_GlobalWeight_Record( 88);
			if( null != record)
				m_fGoIntoDelayTime = record.Value * 0.001f;
			
			if( 0.0f == m_fGoIntoDelayTime)
			{
				Debug.LogWarning( "IndunMatchingGoInto_Coroutine(), 0.0f == m_fGoIntoDelayTime");
				m_fGoIntoDelayTime = 5.0f; // default
			}
		}

		Tbl_InDun_Record record2 = AsTableManager.Instance.GetInDunRecord( m_nIndunTableIndex);
		string strName = "";
		if( null != record2)
			strName = AsTableManager.Instance.GetTbl_String( record2.Name);
		
		string str = string.Format( AsTableManager.Instance.GetTbl_String( 1863), (int)m_fGoIntoDelayTime, strName);
		AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage( str);

		while( Time.realtimeSinceStartup < m_fGoIntoTimeBuf + m_fGoIntoDelayTime)
		{
			yield return new WaitForSeconds( 0.2f);
			
			if( false == m_isCounting)
				yield break;
			
			int nTime = (int)( Time.realtimeSinceStartup - m_fGoIntoTimeBuf);
			
			if( nTime > m_nGoIntoCountCheck)
			{
				if( (int)( m_fGoIntoDelayTime - nTime) > 0)
				{
					string strMsg = string.Format( AsTableManager.Instance.GetTbl_String( 1863), (int)m_fGoIntoDelayTime - nTime, strName);
					AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage( strMsg);
					m_nGoIntoCountCheck = nTime;
				}
			}
		}
		
		Send_Indun_Matching_GoInto( true);
		m_isCounting = false;
	}

	public void OpenIndunMatchingErrorMsgBox(string strMsg)
	{
		if( null != m_msgboxIndunMatchingError)
			return;

		string strTitle = AsTableManager.Instance.GetTbl_String( 2279);
		OpenIndunMatchingErrorMsgBox( strTitle, strMsg);
	}

	public void OpenIndunMatchingErrorMsgBox(string strTitle, string strMsg)
	{
		if( null != m_msgboxIndunMatchingError)
			return;

		m_msgboxIndunMatchingError = AsNotify.Instance.MessageBox( strTitle, strMsg, this, "OnMsgBox_IndunMatchingError_Ok", "OnMsgBox_IndunMatchingError_Ok", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
	}

	public void CloseIndunMatchingErrorMsgBox()
	{
		if( null != m_msgboxIndunMatchingError)
			m_msgboxIndunMatchingError.Close();
	}
	
	public void OnMsgBox_IndunMatchingError_Ok()
	{
		CloseIndunMatchingErrorMsgBox();
	}
	
	public void OpenIndunMatchingErrorMsgBox_Cash(int nIndunTableIndex)
	{
		if( null != m_msgboxIndunMatchingError_Cash)
			return;
		
		string strTitle = AsTableManager.Instance.GetTbl_String( 2279);
		string strMsg = AsTableManager.Instance.GetTbl_String( 2287);
		
		m_msgboxIndunMatchingError_Cash = AsNotify.Instance.MessageBox( strTitle, strMsg, this, "OnMsgBox_IndunMatchingError_Cash_Ok", "OnMsgBox_IndunMatchingError_Cash_Cancel", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
	}
	
	public void CloseIndunMatchingErrorMsgBox_Cash()
	{
		if( null != m_msgboxIndunMatchingError_Cash)
			m_msgboxIndunMatchingError_Cash.Close();
	}
	
	public void OnMsgBox_IndunMatchingError_Cash_Ok()
	{
		CloseIndunMatchingErrorMsgBox_Cash();

		if( true == AsHudDlgMgr.Instance.IsOpenInstantDungeonDlg)
			AsHudDlgMgr.Instance.CloseInstantDungeonDlg();
		
		// open cash shop
//		AsHudDlgMgr.Instance.NotEnoughMiracleProcessInGame();
	}

	public void OnMsgBox_IndunMatchingError_Cash_Cancel()
	{
		CloseIndunMatchingErrorMsgBox_Cash();
	}

	public void OpenIndunMatchingCancelMsgBox()
	{
		if( null != m_msgboxIndunMatchingCancel)
			return;
		
		Tbl_InDun_Record record = AsTableManager.Instance.GetInDunRecord( m_nIndunTableIndex);

		if( null == record)
		{
			Debug.LogError( "AsInstanceDungeonManager::OpenIndunMatchingCancelMsgBox(), null == record, m_nIndunTableIndex: " + m_nIndunTableIndex);
			return;
		}
		
		string strTitle = AsTableManager.Instance.GetTbl_String( 126);
		string strName = AsTableManager.Instance.GetTbl_String( record.Name);
		string strMsg = string.Format( AsTableManager.Instance.GetTbl_String( 2292), strName);
		m_msgboxIndunMatchingCancel = AsNotify.Instance.MessageBox( strTitle, strMsg, this, "OnMsgBox_IndunMatchingCancel_Ok", "OnMsgBox_IndunMatchingCancel_Cancel", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
	}
	
	public void CloseIndunMatchingCancelMsgBox()
	{
		if( null != m_msgboxIndunMatchingCancel)
			m_msgboxIndunMatchingCancel.Close();
	}

	public void OnMsgBox_IndunMatchingCancel_Ok()
	{
		CloseIndunMatchingCancelMsgBox();
		Send_Indun_Matching_Request( false, 0, 0); // matching cancel
		
		isMatching = false;
	}

	public void OnMsgBox_IndunMatchingCancel_Cancel()
	{
		CloseIndunMatchingCancelMsgBox();
	}

	public void OpenIndunExitMsgBox()
	{
		if( null != m_msgboxIndunExit)
			return;
		
		string strTitle = AsTableManager.Instance.GetTbl_String( 126);
		string strMsg = AsTableManager.Instance.GetTbl_String( 2347);
		m_msgboxIndunExit = AsNotify.Instance.MessageBox( strTitle, strMsg, this, "OnMsgBox_IndunExit_Ok", "OnMsgBox_IndunExit_Cancel", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
	}
	
	public void CloseIndunExitMsgBox()
	{
		if( null != m_msgboxIndunExit)
			m_msgboxIndunExit.Close();
	}
	
	public void OnMsgBox_IndunExit_Ok()
	{
		Send_InDun_Exit();
		CloseIndunExitMsgBox();
	}
	
	public void OnMsgBox_IndunExit_Cancel()
	{
		CloseIndunExitMsgBox();
	}

	public void Send_Indun_Matching_Request(bool isMatching, int nIndunTableIndex, int nIndunBranchTableIndex)
	{
#if INTEGRATED_ARENA_MATCHING
		body_CS_INTEGRATED_INDUN_MATCHING_REQUEST data = new body_CS_INTEGRATED_INDUN_MATCHING_REQUEST( isMatching, nIndunTableIndex, nIndunBranchTableIndex);
		byte[] sendData = data.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( sendData);
#endif
	}

	public void Send_Indun_Matching_Info()
	{
#if INTEGRATED_ARENA_MATCHING
		body_CS_INTEGRATED_INDUN_MATCHING_INFO data = new body_CS_INTEGRATED_INDUN_MATCHING_INFO();
		byte[] sendData = data.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( sendData);
#endif
	}

	public void Send_Indun_Matching_GoInto(bool bEnter)
	{
#if INTEGRATED_ARENA_MATCHING
		body_CS_INTEGRATED_INDUN_MATCHING_GOINTO data = new body_CS_INTEGRATED_INDUN_MATCHING_GOINTO( bEnter);
		byte[] sendData = data.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( sendData);
		
//		if( AsUserInfo.Instance.SavedCharStat.hpCur_ > 0 && true == bEnter)
//			_IndunGoIntoEnter();
		
		Debug.Log( "Send_Indun_Matching_GoInto(), bEnter: " + bEnter);
		
#endif
	}
	
#if INTEGRATED_ARENA_MATCHING
	public void Recv_Integrated_Indun_Matching_Info_Result(body_SC_INTEGRATED_INDUN_MATCHING_INFO_RESULT data)
	{
		if( eRESULTCODE.eRESULT_SUCC != data.eResult)
		{
			Debug.LogError( "Error: Recv_Integrated_Indun_Matching_Info_Result(): ResultCode: " + data.eResult);
			return;
		}

		// open Indun dlg
		AsHudDlgMgr.Instance.OpenInstantDungeonDlg_Coroutine( data.nIndunBranchTable_LastEnter);
	}
	
	public void Recv_Integrated_Indun_Matching_Request_Result(body_SC_INTEGRATED_INDUN_MATCHING_REQUEST_RESULT data)
	{
		switch( data.eResult)
		{
		case eRESULTCODE.eRESULT_SUCC:
			if( true == AsHudDlgMgr.Instance.IsOpenInstantDungeonDlg)
				AsHudDlgMgr.Instance.CloseInstantDungeonDlg();
			break;
		case eRESULTCODE.eRESULT_FAIL_INDUNMATCHING_CANNOTREADY:
			OpenIndunMatchingErrorMsgBox( AsTableManager.Instance.GetTbl_String(2280));
			break;
		case eRESULTCODE.eRESULT_FAIL_INDUNMATCHING_LIMITCOUNT_ZERO:
			OpenIndunMatchingErrorMsgBox_Cash( m_nIndunTableIndex);
			break;
		case eRESULTCODE.eRESULT_FAIL_INDUNMATCHING_NOTOPEN:
			OpenIndunMatchingErrorMsgBox( AsTableManager.Instance.GetTbl_String(914));
			break;
		case eRESULTCODE.eRESULT_FAIL_INDUNMATCHING_PARTYUSER_LOGOUT:
			OpenIndunMatchingErrorMsgBox( AsTableManager.Instance.GetTbl_String( 1900));
			break;
		case eRESULTCODE.eRESULT_FAIL_INDUN_NOTEMPTYINDUN:
			OpenIndunMatchingErrorMsgBox( AsTableManager.Instance.GetTbl_String( 1794));
			break;
		case eRESULTCODE.eRESULT_FAIL_INDUNMATCHING_SYSTEMFAILURE:
			OpenIndunMatchingErrorMsgBox( AsTableManager.Instance.GetTbl_String( 1794));
			break;
		default:
			Debug.Log( "Recv_Integrated_Indun_Matching_Request_Result() : " + data.eResult);
			break;
		}
		
//		m_bSendMatching = false;
	}

	public void Recv_Integrated_Indun_Matching_GoInto_Result(body_SC_INTEGRATED_INDUN_MATCHING_GOINTO_RESULT data)
	{
		if( eRESULTCODE.eRESULT_FAIL_INDUNMATCHING_NOTMATCHING == data.eResult)
		{
			// remove matching room
			isMatching = false;
			
			AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage( AsTableManager.Instance.GetTbl_String( 2291));
			
			if( true == m_isCounting)
				m_isCounting = false;
		}
		
		Debug.Log( "Recv_Integrated_Indun_Matching_GoInto_Result(): eResult: " + data.eResult);
	}

	public void Recv_Integrated_Indun_Matching_Notify(body_SC_INTEGRATED_INDUN_MATCHING_NOTIFY data)
	{
		switch( data.eResult)
		{
		case eRESULTCODE.eRESULT_FAIL_INDUNMATCHING_CANNOTREADY:
			OpenIndunMatchingErrorMsgBox( AsTableManager.Instance.GetTbl_String( 2280));
			break;
			
		case eRESULTCODE.eRESULT_FAIL_INDUNMATCHING_CANNOTCANCEL:
			OpenIndunMatchingErrorMsgBox( AsTableManager.Instance.GetTbl_String( 956));
			break;
			
		case eRESULTCODE.eRESULT_FAIL_INDUNMATCHING_EMPTY:
			OpenIndunMatchingErrorMsgBox( AsTableManager.Instance.GetTbl_String( 957));
			break;
			
		case eRESULTCODE.eRESULT_FAIL_INDUNMATCHING_SYSTEMFAILURE:
			// remove matching room
			OpenIndunMatchingErrorMsgBox( AsTableManager.Instance.GetTbl_String( 957));
			isMatching = false;
			break;
			
		case eRESULTCODE.eRESULT_FAIL_INDUNMATCHING_NOTPARTY: break;
			
		case eRESULTCODE.eRESULT_FAIL_INDUNMATCHING_LEVEL:
			OpenIndunMatchingErrorMsgBox( AsTableManager.Instance.GetTbl_String( 2281));
			break;
			
		case eRESULTCODE.eRESULT_FAIL_INDUNMATCHING_LIMITCOUNT_ZERO:
			OpenIndunMatchingErrorMsgBox_Cash( data.nIndunTableIndex);
			break;
			
		case eRESULTCODE.eRESULT_FAIL_INDUNMATCHING_NOTMATCHING: break;
			
		case eRESULTCODE.eRESULT_FAIL_INDUNMATCHING_ALREADY:
			OpenIndunMatchingErrorMsgBox( AsTableManager.Instance.GetTbl_String( 2289));
			break;
			
		case eRESULTCODE.eRESULT_FAIL_INDUNMATCHING_CANCEL_ALREADYPLAY:
			OpenIndunMatchingErrorMsgBox( AsTableManager.Instance.GetTbl_String( 956));
			break;
			
		case eRESULTCODE.eRESULT_FAIL_INDUNMATCHING_PLAY:
			OpenIndunMatchingErrorMsgBox( AsTableManager.Instance.GetTbl_String( 956));
			break;
			
		case eRESULTCODE.eRESULT_FAIL_INDUNMATCHING_PARTYUSERCOUNTOVER:
			OpenIndunMatchingErrorMsgBox( AsTableManager.Instance.GetTbl_String( 2282));
			break;
			
		case eRESULTCODE.eRESULT_FAIL_INDUNMATCHING_NOTOPEN:
			OpenIndunMatchingErrorMsgBox( AsTableManager.Instance.GetTbl_String( 914));
			break;
			
		case eRESULTCODE.eRESULT_FAIL_INDUNMATCHING_PARTYUSER_LOGOUT:
			OpenIndunMatchingErrorMsgBox( AsTableManager.Instance.GetTbl_String( 1900));
			break;
		
		case eRESULTCODE.eRESULT_NOTIFY_INDUNMATCHING_START:
			isMatching = true;
			m_nIndunTableIndex = data.nIndunTableIndex;
			m_nIndunBranchTableIndex = data.nIndunBranchTableIndex;
			Tbl_InDun_Record record = AsTableManager.Instance.GetInDunRecord( data.nIndunTableIndex);
			string strName = AsTableManager.Instance.GetTbl_String( record.Name);
			string strMsg = string.Format( AsTableManager.Instance.GetTbl_String( 1853), strName);
			AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage( strMsg);
			
			if( true == AsHudDlgMgr.Instance.IsOpenInstantDungeonDlg)
				AsHudDlgMgr.Instance.CloseInstantDungeonDlg();
			if( true == _CloseDontUseUI())
				AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage( AsTableManager.Instance.GetTbl_String( 2283));
			break;
			
		case eRESULTCODE.eRESULT_NOTIFY_INDUNMATCHING_RESTART:
			AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage( AsTableManager.Instance.GetTbl_String( 2290));
			if( true == m_isCounting)
				m_isCounting = false;
			break;
			
		case eRESULTCODE.eRESULT_NOTIFY_INDUNMATCHING_CENCEL_MEORMYPARTY:
			// remove matching room
			_OpenIndunMatchingCancelParty( data.nCharUniqKey, data.szCharName);
			isMatching = false;
			if( true == m_isCounting)
				m_isCounting = false;
			break;
			
		case eRESULTCODE.eRESULT_NOTIFY_INDUNMATCHING_COMPLETE:
			m_nGoIntoCountCheck = 0;
			m_fGoIntoTimeBuf = Time.realtimeSinceStartup;
			StartCoroutine( "IndunMatchingGoInto_Coroutine");
			break;
			
		case eRESULTCODE.eRESULT_NOTIFY_INDUNMATCHING_GOINTO_YES: break;

		case eRESULTCODE.eRESULT_NOTIFY_INDUNMATCHING_GOINTO_NO:
			if( AsUserInfo.Instance.SavedCharStat.uniqKey_ == data.nCharUniqKey)
			{
				AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage( AsTableManager.Instance.GetTbl_String( 2291));
				isMatching = false; // remove matching room
			}
			
			_IndunMatchingCancelParty( data.nCharUniqKey);
			
			if( true == m_isCounting)
				m_isCounting = false;
			break;

		case eRESULTCODE.eRESULT_FAIL_INDUNMATCHING_NO_CLEAR_NORMALMODE:
			Tbl_InDun_Record record2 = AsTableManager.Instance.GetInDunRecord( data.nIndunTableIndex);
			string strIndunName = AsTableManager.Instance.GetTbl_String( record2.Name);
			string strMsg2 = string.Format( AsTableManager.Instance.GetTbl_String( 1899), data.szCharName, strIndunName);
			AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage( strMsg2);
			break;
		}
		
		Debug.Log( "Error: Recv_Integrated_Indun_Matching_Notify(): ResultCode: " + data.eResult);
	}

	public void Recv_Integrated_Indun_Gogogo(body_SC_INTEGRATED_INDUN_GOGOGO data)
	{
		_RequestDimpleLog_Indun( 160401);

		if( 0 == data.nIndunTableIndex)
		{
//			_IndunGoIntoEnd();
			return;
		}

		StartCoroutine( Integrated_Indun_Gogogo( data));
	}

	private IEnumerator Integrated_Indun_Gogogo(body_SC_INTEGRATED_INDUN_GOGOGO data)
	{
		if( true == data.bReconnect)
		{
			AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage( AsTableManager.Instance.GetTbl_String( 2818));
			yield return new WaitForSeconds( 3.0f);
		}
		
		// connect integrated server
		AsNetworkIntegratedManager.Instance.ConnectToServer( data.szIpAddress, data.nPort);
		if( false == AsNetworkIntegratedManager.Instance.IsConnected())
		{
			Debug.LogError( "AsNetworkIntegratedManager.Instance.ConnectToServer(): Failed");
			Debug.Log( "ip: " + data.szIpAddress + ", port: " + data.nPort + ", id: " + data.nIndunTableIndex + ", did: " + data.nIndunDuplicationIndex);
//			_IndunGoIntoEnd();
			yield break;
		}

		AsNetworkManager.Instance.Start_SendAlive_GameServer();
		
		_RequestDimpleLog_Indun( 160402);
		
		// login possible
		body_CG_LOGIN_POSSIBLE possible = new body_CG_LOGIN_POSSIBLE();
		byte[] possibleData = possible.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( possibleData);
		
		// indun login
		uint uiUserUniqKey = AsUserInfo.Instance.LoginUserUniqueKey;
		body_CS_INTEGRATED_INDUN_LOGIN data2 = new body_CS_INTEGRATED_INDUN_LOGIN( uiUserUniqKey, data.nIndunTableIndex, data.nIndunDuplicationIndex, data.bReconnect);
		byte[] sendData = data2.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( sendData);
		
		_RequestDimpleLog_Indun( 160403);
		
		// remove matching room
		isMatching = false;
		m_bStartOnce = false;
		
		m_nIndunTableIndex = data.nIndunTableIndex;
		
		if( true == data.bReconnect)
			m_nIndunBranchTableIndex = data.nReconnectBranchTableIdx;
		
		Debug.Log( "ip: " + data.szIpAddress + ", port: " + data.nPort + ", id: " + data.nIndunTableIndex + ", did: " + data.nIndunDuplicationIndex);
	}
	
	public void Recv_Integrated_Indun_RewardInfo(body_SC_INTEGRATED_INDUN_REWARDINFO data)
	{
		// indun end message
		_ShowIndunMsg( false);

		int nPlayTimeSec = 0;
#if INDUN_TIME
		nPlayTimeSec = _GetTimeSec( data.nIndunClearTime - data.nIndunStartTime);
#endif

#if INDUN_EXCHANGE_GOLD
		StartCoroutine( OpenIndunRewardDlg( nPlayTimeSec, data.nCompleteQuest_Exp, data.nSubQuest_Exp, data.nCompleteQuest_Gold, data.nSubQuest_Gold, data.nCompleteQuest_ExchangeGold, data.nSubQuest_ExchangeGold));
#else
		StartCoroutine( OpenIndunRewardDlg( nPlayTimeSec, data.nCompleteQuest_Exp, data.nSubQuest_Exp, data.nCompleteQuest_Gold, data.nSubQuest_Gold, 0, 0));
#endif

		if( false == m_listClearBranchIndex.Contains( m_nIndunBranchTableIndex))
		{
			m_listClearBranchIndex.Add( m_nIndunBranchTableIndex);
			m_bFirstRewardDlgOpen = true;
		}
		else
			m_bFirstRewardDlgOpen = false;
	}
	
	private int _GetTimeSec(long lTime)
	{
		System.DateTime dt = new System.DateTime(1970, 1, 1, 9, 0, 0);
		dt = dt.AddSeconds( lTime);
		return ( dt.Minute * 60 + dt.Second);
	}

	public void Recv_Integrated_Indun_GetItem_Result(body_SC_INDUN_REWARD_GETITEM_RESULT data)
	{
		if( eRESULTCODE.eRESULT_SUCC == data.eResult)
		{
			// 0: get item
			m_IndunRewardDlg.Recv_Integrated_Indun_GetItem_Result( data.sIndunRewardItem_0, data.sIndunRewardItem_1, data.sIndunRewardItem_2);
		}
		else if( eRESULTCODE.eRESULT_FAIL_INDUN_FULL_INVEN == data.eResult)
		{
			if( null != m_IndunRewardDlg)
			{
				AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage( AsTableManager.Instance.GetTbl_String( 4122));
				m_IndunRewardDlg.Close();
				StartCoroutine( "Send_InDun_Exit_Delay");
			}
		}
		else
		{
			Debug.LogError( "Recv_Integrated_Indun_GetItem_Result(), result: " + data.eResult);
		}
	}
	
	public IEnumerator Send_InDun_Exit_Delay()
	{
		yield return new WaitForSeconds( 3.0f);
		
		Send_InDun_Exit();
	}

	public void Recv_Integrated_Indun_Quest_Progress_Info(body_SC_INDUN_QUEST_PROGRESS_INFO data)
	{
		m_nIndunQuestProgressInfoBuff = data;
		
		// indun starg message
		if( false == m_bStartOnce)
		{
			if( false == IsOpenIndunQuestInfoDlg)
				StartCoroutine( OpenIndunQuestInfoDlg());
			
			_ShowIndunMsg( true);
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6142_EFF_Indun_Start", Vector3.zero, false);
			
			m_bStartOnce = true;
		}

		if( null != m_IndunQuestInfoDlg)
			m_IndunQuestInfoDlg.SetIndunQuestInfo( data);
	}
	
	public void Recv_Integrated_Indun_Enter_LimitCount(body_SC_INDUN_ENTER_LIMITCOUNT data)
	{
		Debug.Log( "--------------------Recv_Integrated_Indun_Enter_LimitCount: " + data.nLimitCount);
		
		m_LimitCount = data.nLimitCount;
		
		if( true == m_bLimitCountCoroutine)
			StopCoroutine( "SetIndunLimitCountCoroutine");
		
		StartCoroutine( "SetIndunLimitCountCoroutine");
	}

#if INDUN_TIME
	public void Recv_Integrated_Indun_ClearInfo(body1_SC_INDUN_CLEARINFO data)
	{
//		if( data.nCnt <= 0)
//			return;

		if( m_nCurClearCharUniqKey != data.nCharUniqKey)
		{
			m_nCurClearCharUniqKey = data.nCharUniqKey;
			m_listClearBranchIndex.Clear();
		}

		for( int i = 0; i < data.nCnt; i++)
		{
			body2_SC_INDUN_CLEARINFO body = data.body[i];
			m_listClearBranchIndex.Add( body.nIndunBranchTableIndex);

			Debug.Log( "___________________Indun_ClearInfo, nIndunTableIndex: " + body.nIndunTableIndex + ", nIndunBranchTableIndex: " + body.nIndunBranchTableIndex +
			          ", ClearTime: " + _GetTimeSec( body.nIndunClearTime - body.nIndunStartTime));
		}
	}
#endif

	public void Recive_Integrated_Indun_Re_Connect_Fail(body_SC_INTEGRATED_INDUN_RE_CONNECT_FAIL data)
	{
		StartCoroutine( Integrated_Indun_Re_Connect_Fail( data));
	}

	private IEnumerator Integrated_Indun_Re_Connect_Fail(body_SC_INTEGRATED_INDUN_RE_CONNECT_FAIL data)
	{
		AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage( AsTableManager.Instance.GetTbl_String( 2819));

		yield return new WaitForSeconds( 3.0f);

		body_CS_INTEGRATED_INDUN_RE_CONNECT_CANCEL send = new body_CS_INTEGRATED_INDUN_RE_CONNECT_CANCEL();
		byte[] sendData = send.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( sendData);
		
		// disconnect integrated server
		AsNetworkIntegratedManager.Instance.SwitchServer();
		
		if( false == AsNetworkIntegratedManager.Instance.IsConnected())
		{
			_RequestDimpleLog_Indun( 160407);
			
			body_CS_RETURN_WORLD data2 = new body_CS_RETURN_WORLD();
			byte[] sendData2 = data2.ClassToPacketBytes();
			AsNetworkMessageHandler.Instance.Send( sendData2);
			
			_RequestDimpleLog_Indun( 160408);
		}
		
		AsPartyManager.Instance.PartyUserRemoveAll();
	}

	public void Recv_Integrated_Indun_Enter_HellModeCount(body_SC_INDUN_ENTER_HELLLIMITCOUNT data)
	{
		int nIndunID = data.nIndunTableIndex;
		int nCount = data.nLimitCount;

		if( false == m_dicHellModeCount.ContainsKey( nIndunID))
		{
			m_dicHellModeCount.Add( nIndunID, nCount);
		}
		else
		{
			m_dicHellModeCount[nIndunID] = nCount;
		}
	}

	public int GetHellLimitCount(int nIndunID)
	{
		if( true == m_dicHellModeCount.ContainsKey( nIndunID))
			return m_dicHellModeCount[nIndunID];

		return 0;
	}

	public Dictionary<int, int> GetHellLimitCountList()
	{
		return m_dicHellModeCount;
	}

	public bool isClearPrevMode(int nIndunBranchTableIndex)
	{
#if INDUN_TIME
		/*
		Tbl_InsDungeonReward_Record record = AsTableManager.Instance.GetInsRewardRecord( nIndunBranchTableIndex);

		if( null == record)
		{
			Debug.Log( "AsInstanceDungeonManager::isClearPrevMode(), null == record, nIndunBranchTableIndex: " + nIndunBranchTableIndex);
			return false;
		}

		string strBuff = record.Grade.ToLower();

		if( true == record.Grade.ToLower().Contains( "normal"))
			return true;
		else if( true == record.Grade.ToLower().Contains( "hard"))
			strBuff = "normal";
		else if( true == record.Grade.ToLower().Contains( "hell"))
			strBuff = "hard";

		foreach( int n in m_listClearBranchIndex)
		{
			Tbl_InsDungeonReward_Record data = AsTableManager.Instance.GetInsRewardRecord( n);

			if( null != data)
			{
				if( data.InstanceTableIdx == record.InstanceTableIdx && true == data.Grade.ToLower().Contains( strBuff))
					return true;
			}
		}

		return false;
		*/

		Tbl_InsDungeonReward_Record record = AsTableManager.Instance.GetInsRewardRecord( nIndunBranchTableIndex);
		
		if( null == record)
		{
			Debug.Log( "AsInstanceDungeonManager::isClearPrevMode(), null == record, nIndunBranchTableIndex: " + nIndunBranchTableIndex);
			return false;
		}
		
		if( true == record.Grade.ToLower().Contains( "normal") || 0 == record.Precede)
			return true;

		Tbl_InsDungeonReward_Record record2 = AsTableManager.Instance.GetInsRewardRecord( record.Precede);
		if( null != record2)
		{
			if( 0 == record2.Use)
				return true;
		}

		foreach( int n in m_listClearBranchIndex)
		{
			/*
			Tbl_InsDungeonReward_Record data = AsTableManager.Instance.GetInsRewardRecord( n);

			if( null != data)
			{
				if( data.ID == nIndunBranchTableIndex)
					return true;
			}
			*/
			if( n == record.Precede)
				return true;
		}

		return false;
#else
		return true;
#endif
	}

	private IEnumerator SetIndunLimitCountCoroutine()
	{
		m_bLimitCountCoroutine = true;
		
		while( null == AsHudDlgMgr.Instance)
			yield return null;
		
		AsHudDlgMgr.Instance.SetIndunLimitCount( m_LimitCount);
		
		m_bLimitCountCoroutine = false;
	}
#endif
	
	public bool IsOpenIndunRewardDlg
	{
		get
		{
			if( null == m_IndunRewardDlg)
				return false;
			return m_IndunRewardDlg.gameObject.active;
		}
	}

	public IEnumerator OpenIndunRewardDlg(int nPlayTimeSec, int nExp, int nAddExp, long lGold, long lAddGold, long lExchangeGold, long lExchangeAddGold)
	{
		yield return new WaitForSeconds( 3.0f);
		
		if( false == IsOpenIndunRewardDlg)
		{
			if( null == m_IndunRewardDlg)
			{
				GameObject obj = ResourceLoad.LoadGameObject( "UI/AsGUI/GUI_InDeonReward");
				yield return obj;

				GameObject go = GameObject.Instantiate( obj) as GameObject;
				m_IndunRewardDlg = go.GetComponentInChildren<AsIndunRewardDlg>();

				if( null != m_IndunRewardDlg)
				{
					m_IndunRewardDlg.Open( go, nPlayTimeSec, nExp, nAddExp, lGold, lAddGold, lExchangeGold, lExchangeAddGold, m_nIndunQuestProgressInfoBuff);
				}
			}
		}
		else
		{
			CloseIndunRewardDlg();
		}

		CloseIndunExitMsgBox();
	}

	public void InitIndunQuestProgressInfoBuff()
	{
		m_nIndunQuestProgressInfoBuff.nInsQuestGroupTableUniqCode = 0;
	}

	public void CloseIndunRewardDlg()
	{
		if( null == m_IndunRewardDlg)
			return;

		m_IndunRewardDlg.Close();
	}

	public void OpenFirstRewardDlg_Coroutine()
	{
		if( true == m_bFirstRewardDlgOpen)
		{
			StartCoroutine( OpenFirstRewardDlg());
			m_bFirstRewardDlgOpen = false;
		}
	}

	public IEnumerator OpenFirstRewardDlg()
	{
		if( null == m_FirstRewardDlg)
		{
			yield return new WaitForSeconds( 1.0f);

			GameObject obj = ResourceLoad.LoadGameObject( "UI/AsGUI/GUI_InDeonFirstRewardPopup");
			yield return obj;

			GameObject go = GameObject.Instantiate( obj) as GameObject;
			m_FirstRewardDlg = go.GetComponentInChildren<AsIndunFirstRewardDlg>();

			if( null != m_FirstRewardDlg)
			{
				m_FirstRewardDlg.Open( go, m_nIndunBranchTableIndex);
			}
		}
	}

	public void CloseFirstRewardDlg()
	{
		if( null == m_FirstRewardDlg)
			return;

		m_FirstRewardDlg.Close();
	}

	public bool IsOpenIndunQuestInfoDlg
	{
		get
		{
			if( null == m_IndunQuestInfoDlg)
				return false;
			return m_IndunQuestInfoDlg.gameObject.active;
		}
	}

	public IEnumerator OpenIndunQuestInfoDlg()
	{
		m_bOpenIndunQuestInfoDlgCoroutine = true;

		if( false == IsOpenIndunQuestInfoDlg)
		{
			if( null == m_IndunQuestInfoDlg)
			{
				GameObject obj = ResourceLoad.LoadGameObject( "UI/AsGUI/GUI_InDeonMission");
				yield return obj;

				GameObject go = GameObject.Instantiate( obj) as GameObject;
				m_IndunQuestInfoDlg = go.GetComponentInChildren<AsIndunQuestInfoDlg>();

				if( null != m_IndunQuestInfoDlg)
				{
					m_IndunQuestInfoDlg.Open( go, m_nIndunQuestProgressInfoBuff);
					m_IndunQuestInfoDlg.SetIndunQuestInfo( m_nIndunQuestProgressInfoBuff);
				}
			}
		}
		else
		{
			CloseIndunQuestInfoDlg();
		}

		m_bOpenIndunQuestInfoDlgCoroutine = false;
	}

	public void CloseIndunQuestInfoDlg()
	{
		if( null == m_IndunQuestInfoDlg)
			return;

		m_IndunQuestInfoDlg.Close();
	}

	#region -private
	private void _Error(eRESULTCODE result)
	{
		switch( result)
		{
		case eRESULTCODE.eRESULT_FAIL_INDUN_NOTPARTY: break;
		case eRESULTCODE.eRESULT_FAIL_INDUN_OVERMEMBER: break;
		case eRESULTCODE.eRESULT_FAIL_INDUN_LEVEL: break;
		case eRESULTCODE.eRESULT_FAIL_INDUN_NOTEMPTYINDUN: break;
		case eRESULTCODE.eRESULT_FAIL_INDUN_SYSTEMWAIT: break;
		case eRESULTCODE.eRESULT_FAIL_INDUN_CHARACTER_DEATH: break;
		case eRESULTCODE.eRESULT_FAIL_INDUN_NOTCREATED: break;
		case eRESULTCODE.eRESULT_FAIL_INDUN_POSSIBLECONTENT: break;
		}
	}

	private bool _CloseDontUseUI()
	{
		bool isClose = false;
		if( null != AsHudDlgMgr.Instance.ChannelSelectDlg)
		{
			AsHudDlgMgr.Instance.CloseChannelSelectDlg();
			isClose = true;
		}
		
		if( null != AsHudDlgMgr.Instance.worldMapDlg && true == AsHudDlgMgr.Instance.worldMapDlg.IsCurStateWaypointDlg())
		{
			AsHudDlgMgr.Instance.CloseWaypointMapDlg();
			isClose = true;
		}

		if( null != AsHudDlgMgr.Instance.productionDlg)
		{
			AsHudDlgMgr.Instance.CloseProductionDlg();
			isClose = true;
		}
		
		if( true == AsHudDlgMgr.Instance.IsOpenCashStore)
		{
			AsHudDlgMgr.Instance.CloseCashStore();
			isClose = true;
		}

		if( true == AsPartyManager.Instance.PartyUI.IsOpenPartyDlg || true == AsPartyManager.Instance.PartyUI.IsOpenPartyList
			|| true == AsPartyManager.Instance.PartyUI.IsOpenPartyMatching || true == AsPartyManager.Instance.PartyUI.IsOpenPartyCreate || true == AsPartyManager.Instance.PartyUI.IsOpenPartyPRDlg)
		{
			AsPartyManager.Instance.PartyUI.ClosePartyList();
			isClose = true;
		}
		
		if( null != AsPartyManager.Instance.PartyInfoDlg)
		{
			AsPartyManager.Instance.ClosePartyInfoDlg();
			isClose = true;
		}
		
		if( null != AsPartyManager.Instance.GetPartyDiceDlg())
		{
			if( true == AsPartyManager.Instance.GetPartyDiceDlg().IsUseing)
			{
				AsPartyManager.Instance.ClosePartyDiceDlg();
				isClose = true;
			}
		}
		
		return isClose;
	}
	
	public void _OpenIndunMatchingCancelParty(uint nCharUniqKey, string strCharName)
	{
		AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
		
		if( null == userEntity)
		{
			Debug.LogError( "AsInstanceDungeonManager::_OpenIndunMatchingCancelParty(): null == userEntity");
			return;
		}
		
		AS_PARTY_USER partyUser = AsPartyManager.Instance.GetPartyMember( userEntity.UniqueId);
		
		if( null == partyUser)
			return;

		if( nCharUniqKey == userEntity.UniqueId)
			return;
		
		string strTitle = AsTableManager.Instance.GetTbl_String( 2284);
		string strMsg = string.Format( AsTableManager.Instance.GetTbl_String( 2285), strCharName);
		OpenIndunMatchingErrorMsgBox( strTitle, strMsg);
	}
	
	public void _IndunMatchingCancelParty(uint nCharUniqKey)
	{
		uint uiUserUniqKey = AsUserInfo.Instance.SavedCharStat.uniqKey_;
		
		AS_PARTY_USER partyUser = AsPartyManager.Instance.GetPartyMember( uiUserUniqKey);
		
		if( null == partyUser)
			return;
		
		if( nCharUniqKey != uiUserUniqKey)
		{
			partyUser = AsPartyManager.Instance.GetPartyMember( nCharUniqKey);
			
			if( null != partyUser)
			{
				// remove matching room
				isMatching = false;
				AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage( AsTableManager.Instance.GetTbl_String( 2291));
			}
		}
	}
	
	// isStart: true(start msg), false(end msg)
	private void _ShowIndunMsg(bool isStart)
	{
		Tbl_InDun_Record record = AsTableManager.Instance.GetInDunRecord( m_nIndunTableIndex);

		if( null == record)
		{
			Debug.LogError( "AsInstanceDungeonManager::_ShowIndunMsg(), null == record, m_nIndunTableIndex: " + m_nIndunTableIndex);
			return;
		}
		
		int nIndex = 0;
		if( true == isStart)
			nIndex = record.StartMsg;
		else
			nIndex = record.EndMsg;

		AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage( AsTableManager.Instance.GetTbl_String( nIndex));
	}
	
	private bool _CheckSendIndunMatching()
	{
		AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
		
		if( null == userEntity)
		{
			Debug.LogError( "AsInstanceDungeonManager::_CheckPartyOrCaptain(): null == userEntity");
			return false;
		}
		
		// party check
		AS_PARTY_USER partyUser = AsPartyManager.Instance.GetPartyMember( userEntity.UniqueId);
		
		if( null != partyUser)
		{
			if( true == partyUser.isCaptain)
				return true;
			else
				return false;
		}

		return true;
	}

	private void _RequestDimpleLog_Indun(int nSubKey)
	{
#if ( ( UNITY_IPHONE || UNITY_ANDROID) && ( !UNITY_EDITOR))
		AsWebRequest.Instance.Request_Indun( nSubKey, AsUserInfo.Instance.LoginUserUniqueKey, AsUserInfo.Instance.SavedCharStat.uniqKey_, m_nIndunTableIndex, m_nIndunBranchTableIndex);
#endif
	}

	/*
	private void _IndunGoIntoEnter()
	{
		AsGameMain.s_gameState = GAME_STATE.STATE_WARP_SEND;

		AsPlayerFsm player = AsEntityManager.Instance.GetPlayerCharFsm();
		if( player != null)
		{
			player.SetPlayerFsmState( AsPlayerFsm.ePlayerFsmStateType.IDLE);
			player.ClearMovePacket();
		}
		
		AsInputManager.Instance.m_Activate = false;
	}
	
	private void _IndunGoIntoEnd()
	{
		if( GAME_STATE.STATE_WARP_SEND == AsGameMain.s_gameState)
		{
			AsGameMain.s_gameState = GAME_STATE.STATE_INGAME;
			AsInputManager.Instance.m_Activate = true;
		}
	}
	*/
	#endregion
}
