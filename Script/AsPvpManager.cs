#define INTEGRATED_ARENA_MATCHING
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum eARENATEAMTYPE
{
	eARENATEAMTYPE_A = 0,
	eARENATEAMTYPE_B,

	eARENATEAMTYPE_NOTHING,
	eARENATEAMTYPE_MAX = eARENATEAMTYPE_NOTHING,
};


public class AsPvpManager : MonoBehaviour
{
	static AsPvpManager m_instance;
	public static AsPvpManager Instance{ get{  return m_instance;}}
	
	private AsMessageBox m_msgboxPvpMatchingError = null;
	private AsMessageBox m_msgboxPvpMatchingError_Cash = null;
	private AsMessageBox m_msgboxPvpMatchingCancel = null;
	private bool m_isMatching = false;
	private float m_fGoIntoDelayTime = 0.0f;
	private float m_fGoIntoTimeBuf = 0.0f;
	private int m_nGoIntoCountCheck = 0;
	private bool m_isCounting = false;
	private int m_nGogogoIndunTableIndex = 0;
	private long m_lStartServerTime = 0;

//	[HideInInspector] public int m_nLimitCount1;
//	[HideInInspector] public int m_nLimitCount2;
//	[HideInInspector] public int m_nLimitCount3;
//	[HideInInspector] public int m_nLimitCount4;
	[HideInInspector] public int m_nPvpPoint;
	[HideInInspector] public uint m_nBattleAllCount;
	[HideInInspector] public uint m_nVictoryCount;
	[HideInInspector] public uint m_nLoseCount;
	[HideInInspector] public uint m_nDrawCount;
	//[HideInInspector] public uint m_nRank = 0;
	[HideInInspector] public bool m_bSendMatching = false;
	[HideInInspector] public List<AsPvpKillMessage> m_listKillMsg = new List<AsPvpKillMessage>();
	public PackedSprite m_MatchingEff = null;
	
	[SerializeField] float m_WaitTimeBeforeBegan = 10f;
	bool m_PetSkillAvailable = false; public bool PetSkillAvailable{get{return m_PetSkillAvailable;}}
	
	public bool isMatching
	{
		get
		{
			return m_isMatching;
		}
		set
		{
			m_isMatching = value;
			int nStringIndex = m_isMatching ? 900 : 883;

			if( AsHudDlgMgr.Instance != null )
				AsHudDlgMgr.Instance.SetPvpBtnText( AsTableManager.Instance.GetTbl_String( nStringIndex));
			
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
		m_msgboxPvpMatchingError = ( new GameObject( "msgboxPvpMatchingError")).AddComponent<AsMessageBox>();
		ClosePvpMatchingErrorMsgBox();
		
		m_msgboxPvpMatchingError_Cash = ( new GameObject( "msgboxPvpMatchingError_Cash")).AddComponent<AsMessageBox>();
		ClosePvpMatchingErrorMsgBox_Cash();
		
		m_msgboxPvpMatchingCancel= ( new GameObject( "msgboxPvpMatchingCancel")).AddComponent<AsMessageBox>();
		ClosePvpMatchingCancelMsgBox();

//		Tbl_GlobalWeight_Record record = AsTableManager.Instance.GetTbl_GlobalWeight_Record( 88);
//		if( null != record)
//			m_fGoIntoDelayTime = record.Value * 0.001f;
	}
	
	void Update()
	{
	}
	
	#region -send
	public void Send_Pvp_Matching_Info()
	{
		body_CS_ARENA_MATCHING_INFO data = new body_CS_ARENA_MATCHING_INFO();
		byte[] sendData = data.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( sendData);
	}
	
	public void Send_Pvp_Matching_Request(bool isMatching, int nIndunTableIndex, bool bTrainningMode)
	{
#if INTEGRATED_ARENA_MATCHING
		body_CS_INTEGRATED_ARENA_MATCHING_REQUEST data = new body_CS_INTEGRATED_ARENA_MATCHING_REQUEST( isMatching, nIndunTableIndex, bTrainningMode);
#else
		body_CS_ARENA_MATCHING_REQUEST data = new body_CS_ARENA_MATCHING_REQUEST( isMatching, nIndunTableIndex, bTrainningMode);
#endif
		byte[] sendData = data.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( sendData);
	}

	public void Send_Pvp_Matching_GoInto(bool bEnter)
	{
#if INTEGRATED_ARENA_MATCHING
		body_CS_INTEGRATED_ARENA_MATCHING_GOINTO data = new body_CS_INTEGRATED_ARENA_MATCHING_GOINTO( bEnter);
#else
		body_CS_ARENA_MATCHING_GOINTO data = new body_CS_ARENA_MATCHING_GOINTO( bEnter);
#endif
		byte[] sendData = data.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( sendData);
		
		if( AsUserInfo.Instance.SavedCharStat.hpCur_ > 0 && true == bEnter)
			_PvpGoIntoEnter();
		
		Debug.Log( "Send_Pvp_Matching_GoInto(), bEnter: " + bEnter);
	}
	#endregion
	
	#region -recive
	public void Recv_Pvp_Matching_Info_Result(body_SC_ARENA_MATCHING_INFO_RESULT data)
	{
		if( eRESULTCODE.eRESULT_SUCC != data.eResult)
		{
			Debug.LogError( "Error: Recv_Pvp_Matching_Info_Result(): ResultCode: " + data.eResult);
			return;
		}
		
//		m_nLimitCount1 = data.nLimitCount1;
//		m_nLimitCount2 = data.nLimitCount2;
//		m_nLimitCount3 = data.nLimitCount3;
//		m_nLimitCount4 = data.nLimitCount4;
		m_nPvpPoint = data.nPvpPoint;
		m_nBattleAllCount = data.nBattleAllCount;
		m_nVictoryCount = data.nVictoryCount;
		m_nLoseCount = data.nLoseCount;
		m_nDrawCount = data.nDrawCount;
		
		// open pvp dlg
		AsPvpDlgManager.Instance.OpenPvpDlg_Coroutine();
	}

#if INTEGRATED_ARENA_MATCHING
	public void Receive_Integrated_Arena_Matching_Request_Result( body_SC_INTEGRATED_ARENA_MATCHING_REQUEST_RESULT data)
	{
		switch( data.eResult)
		{
		case eRESULTCODE.eRESULT_SUCC:
			if( true == AsPvpDlgManager.Instance.IsOpenPvpDlg)
				AsPvpDlgManager.Instance.ClosePvpDlg();
			break;
		case eRESULTCODE.eRESULT_FAIL_ARENAMATCHING_CANNOTREADY:
			OpenPvpMatchingErrorMsgBox( AsTableManager.Instance.GetTbl_String(889));
			break;
		case eRESULTCODE.eRESULT_FAIL_ARENAMATCHING_LIMITCOUNT_ZERO:
			OpenPvpMatchingErrorMsgBox_Cash( AsPvpDlgManager.Instance.m_PvpDlg.GetCurIndunTableIndex());
			break;
		case eRESULTCODE.eRESULT_FAIL_ARENAMATCHING_NOTOPEN:
			OpenPvpMatchingErrorMsgBox( AsTableManager.Instance.GetTbl_String(914));
			break;
		default:
			Debug.LogError( "Receive_Integrated_Arena_Matching_Request_Result : " + data.eResult);
			break;
		}
		
		m_bSendMatching = false;
	}
#else
	public void Recv_Pvp_Matching_Request_Result( body_SC_ARENA_MATCHING_REQUEST_RESULT data)
	{
		switch( data.eResult)
		{
		case eRESULTCODE.eRESULT_SUCC:
			if( true == AsPvpDlgManager.Instance.IsOpenPvpDlg)
				AsPvpDlgManager.Instance.ClosePvpDlg();
			break;
		case eRESULTCODE.eRESULT_FAIL_ARENAMATCHING_CANNOTREADY:
			OpenPvpMatchingErrorMsgBox( AsTableManager.Instance.GetTbl_String(889));
			break;
		case eRESULTCODE.eRESULT_FAIL_ARENAMATCHING_LIMITCOUNT_ZERO:
			OpenPvpMatchingErrorMsgBox_Cash( AsPvpDlgManager.Instance.m_PvpDlg.GetCurIndunTableIndex());
			break;
		case eRESULTCODE.eRESULT_FAIL_ARENAMATCHING_NOTOPEN:
			OpenPvpMatchingErrorMsgBox( AsTableManager.Instance.GetTbl_String(914));
			break;
		default:
			Debug.LogError( "Recv_Pvp_Matching_Request_Result : " + data.eResult);
			break;
		}
		
		m_bSendMatching = false;
	}
#endif
	
#if INTEGRATED_ARENA_MATCHING
	public void Receive_Integrated_Arena_Matching_GoInto_Result(body_SC_INTEGRATED_ARENA_MATCHING_GOINTO_RESULT data)
	{
		if( eRESULTCODE.eRESULT_FAIL_ARENAMATCHING_NOTMATCHING == data.eResult)
		{
			// remove matching room
			isMatching = false;
			
			AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage( AsTableManager.Instance.GetTbl_String( 965));
			
			if( true == m_isCounting)
				m_isCounting = false;
		}
		
		Debug.Log( "Receive_Integrated_Arena_Matching_GoInto_Result(): eResult: " + data.eResult);
	}
#else
	public void Recv_Pvp_Matching_GoInto_Result(body_SC_ARENA_MATCHING_GOINTO_RESULT data)
	{
		if( eRESULTCODE.eRESULT_FAIL_ARENAMATCHING_NOTMATCHING == data.eResult)
		{
			// remove matching room
			isMatching = false;
			
			AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage( AsTableManager.Instance.GetTbl_String( 965));
			
			if( true == m_isCounting)
				m_isCounting = false;
		}
		
		Debug.Log( "Recv_Pvp_Matching_GoInto_Result(): eResult: " + data.eResult);
	}
#endif
	
#if INTEGRATED_ARENA_MATCHING
	public void Receive_Integrated_Arena_Matching_Notify(body_SC_INTEGRATED_ARENA_MATCHING_NOTIFY data)
	{
		switch( data.eResult)
		{
		case eRESULTCODE.eRESULT_FAIL_ARENAMATCHING_CANNOTREADY:
			OpenPvpMatchingErrorMsgBox( AsTableManager.Instance.GetTbl_String( 889));
			break;
			
		case eRESULTCODE.eRESULT_FAIL_ARENAMATCHING_CANNOTCANCEL:
			OpenPvpMatchingErrorMsgBox( AsTableManager.Instance.GetTbl_String( 956));
			break;
			
		case eRESULTCODE.eRESULT_FAIL_ARENAMATCHING_EMPTY:
			OpenPvpMatchingErrorMsgBox( AsTableManager.Instance.GetTbl_String( 957));
			break;
			
		case eRESULTCODE.eRESULT_FAIL_ARENAMATCHING_SYSTEMFAILURE:
			// remove matching room
			OpenPvpMatchingErrorMsgBox( AsTableManager.Instance.GetTbl_String( 957));
			isMatching = false;
			break;
			
		case eRESULTCODE.eRESULT_FAIL_ARENAMATCHING_NOTPARTY: break;
			
		case eRESULTCODE.eRESULT_FAIL_ARENAMATCHING_LEVEL:
			OpenPvpMatchingErrorMsgBox( AsTableManager.Instance.GetTbl_String( 891));
			break;
			
		case eRESULTCODE.eRESULT_FAIL_ARENAMATCHING_LIMITCOUNT_ZERO:
			OpenPvpMatchingErrorMsgBox_Cash( data.nIndunTableIndex);
			break;
			
		case eRESULTCODE.eRESULT_FAIL_ARENAMATCHING_NOTMATCHING:
			//OpenPvpMatchingErrorMsgBox( AsTableManager.Instance.GetTbl_String( 959));
			break;
			
		case eRESULTCODE.eRESULT_FAIL_ARENAMATCHING_ALREADY:
			OpenPvpMatchingErrorMsgBox( AsTableManager.Instance.GetTbl_String( 958));
			break;
			
		case eRESULTCODE.eRESULT_FAIL_ARENAMATCHING_CANCEL_ALREADYPLAY:
			OpenPvpMatchingErrorMsgBox( AsTableManager.Instance.GetTbl_String( 956));
			break;
			
		case eRESULTCODE.eRESULT_FAIL_ARENAMATCHING_PLAY:
			OpenPvpMatchingErrorMsgBox( AsTableManager.Instance.GetTbl_String( 956));
			break;
			
		case eRESULTCODE.eRESULT_FAIL_ARENAMATCHING_PARTYUSERCOUNTOVER:
			OpenPvpMatchingErrorMsgBox( AsTableManager.Instance.GetTbl_String( 892));
			break;
			
		case eRESULTCODE.eRESULT_FAIL_ARENAMATCHING_NOTOPEN:
			OpenPvpMatchingErrorMsgBox( AsTableManager.Instance.GetTbl_String( 914));
			break;
			
		case eRESULTCODE.eRESULT_FAIL_ARENAMATCHING_PARTYUSER_LOGOUT:
			OpenPvpMatchingErrorMsgBox( AsTableManager.Instance.GetTbl_String( 916));
			break;
		
		case eRESULTCODE.eRESULT_NOTIFY_ARENAMATCHING_START:
			isMatching = true;
			AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage( AsTableManager.Instance.GetTbl_String( 977));
			if( true == AsPvpDlgManager.Instance.IsOpenPvpDlg)
				AsPvpDlgManager.Instance.ClosePvpDlg();
			if( true == _CloseDontUseUI())
				AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage( AsTableManager.Instance.GetTbl_String( 894));
			break;
			
		case eRESULTCODE.eRESULT_NOTIFY_ARENAMATCHING_RESTART:
			AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage( AsTableManager.Instance.GetTbl_String( 964));
			if( true == m_isCounting)
				m_isCounting = false;
			break;
			
		case eRESULTCODE.eRESULT_NOTIFY_ARENAMATCHING_CENCEL_MEORMYPARTY:
			// remove matching room
			_OpenPvpMatchingCancelParty( data.nCharUniqKey, data.szCharName);
			isMatching = false;
			if( true == m_isCounting)
				m_isCounting = false;
			break;
			
		case eRESULTCODE.eRESULT_NOTIFY_ARENAMATCHING_COMPLETE:
			//Send_Pvp_Matching_GoInto( true);
			m_nGoIntoCountCheck = 0;
			m_fGoIntoTimeBuf = Time.realtimeSinceStartup;
			StartCoroutine( "PvpMatchingGoInto_Coroutine");
			break;
			
		case eRESULTCODE.eRESULT_NOTIFY_ARENAMATCHING_GOINTO_YES: break;

		case eRESULTCODE.eRESULT_NOTIFY_ARENAMATCHING_GOINTO_NO:
//			AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
//			if( null != userEntity && data.nCharUniqKey == userEntity.UniqueId)
//			{
//				AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage( AsTableManager.Instance.GetTbl_String( 965));
//				
//				// remove matching room
//				isMatching = false;
//			}
			if( AsUserInfo.Instance.SavedCharStat.uniqKey_ == data.nCharUniqKey)
			{
				AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage( AsTableManager.Instance.GetTbl_String( 965));
				isMatching = false; // remove matching room
			}
			
			_PvpMatchingCancelParty( data.nCharUniqKey);
			
			if( true == m_isCounting)
				m_isCounting = false;
			break;
		}
		
		Debug.Log( "Receive_Integrated_Arena_Matching_Notify(), eResult: " + data.eResult);
	}
#else
	public void Recv_Pvp_Matching_Notity(body_SC_ARENA_MATCHING_NOTIFY data)
	{
		switch( data.eResult)
		{
		case eRESULTCODE.eRESULT_FAIL_ARENAMATCHING_CANNOTREADY:
			OpenPvpMatchingErrorMsgBox( AsTableManager.Instance.GetTbl_String( 889));
			break;
			
		case eRESULTCODE.eRESULT_FAIL_ARENAMATCHING_CANNOTCANCEL:
			OpenPvpMatchingErrorMsgBox( AsTableManager.Instance.GetTbl_String( 956));
			break;
			
		case eRESULTCODE.eRESULT_FAIL_ARENAMATCHING_EMPTY:
			OpenPvpMatchingErrorMsgBox( AsTableManager.Instance.GetTbl_String( 957));
			break;
			
		case eRESULTCODE.eRESULT_FAIL_ARENAMATCHING_SYSTEMFAILURE:
			// remove matching room
			OpenPvpMatchingErrorMsgBox( AsTableManager.Instance.GetTbl_String( 957));
			isMatching = false;
			break;
			
		case eRESULTCODE.eRESULT_FAIL_ARENAMATCHING_NOTPARTY: break;
			
		case eRESULTCODE.eRESULT_FAIL_ARENAMATCHING_LEVEL:
			OpenPvpMatchingErrorMsgBox( AsTableManager.Instance.GetTbl_String( 891));
			break;
			
		case eRESULTCODE.eRESULT_FAIL_ARENAMATCHING_LIMITCOUNT_ZERO:
			OpenPvpMatchingErrorMsgBox_Cash( data.nIndunTableIndex);
			break;
			
		case eRESULTCODE.eRESULT_FAIL_ARENAMATCHING_NOTMATCHING:
			//OpenPvpMatchingErrorMsgBox( AsTableManager.Instance.GetTbl_String( 959));
			break;
			
		case eRESULTCODE.eRESULT_FAIL_ARENAMATCHING_ALREADY:
			OpenPvpMatchingErrorMsgBox( AsTableManager.Instance.GetTbl_String( 958));
			break;
			
		case eRESULTCODE.eRESULT_FAIL_ARENAMATCHING_CANCEL_ALREADYPLAY:
			OpenPvpMatchingErrorMsgBox( AsTableManager.Instance.GetTbl_String( 956));
			break;
			
		case eRESULTCODE.eRESULT_FAIL_ARENAMATCHING_PLAY:
			OpenPvpMatchingErrorMsgBox( AsTableManager.Instance.GetTbl_String( 956));
			break;
			
		case eRESULTCODE.eRESULT_FAIL_ARENAMATCHING_PARTYUSERCOUNTOVER:
			OpenPvpMatchingErrorMsgBox( AsTableManager.Instance.GetTbl_String( 892));
			break;
			
		case eRESULTCODE.eRESULT_FAIL_ARENAMATCHING_NOTOPEN:
			OpenPvpMatchingErrorMsgBox( AsTableManager.Instance.GetTbl_String( 914));
			break;
			
		case eRESULTCODE.eRESULT_FAIL_ARENAMATCHING_PARTYUSER_LOGOUT:
			OpenPvpMatchingErrorMsgBox( AsTableManager.Instance.GetTbl_String( 916));
			break;
		
		case eRESULTCODE.eRESULT_NOTIFY_ARENAMATCHING_START:
			isMatching = true;
			AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage( AsTableManager.Instance.GetTbl_String( 977));
			if( true == AsPvpDlgManager.Instance.IsOpenPvpDlg)
				AsPvpDlgManager.Instance.ClosePvpDlg();
			if( true == _CloseDontUseUI())
				AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage( AsTableManager.Instance.GetTbl_String( 894));
			break;
			
		case eRESULTCODE.eRESULT_NOTIFY_ARENAMATCHING_RESTART:
			AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage( AsTableManager.Instance.GetTbl_String( 964));
			if( true == m_isCounting)
				m_isCounting = false;
			break;
			
		case eRESULTCODE.eRESULT_NOTIFY_ARENAMATCHING_CENCEL_MEORMYPARTY:
			// remove matching room
			_OpenPvpMatchingCancelParty( data.nCharUniqKey, data.szCharName);
			isMatching = false;
			if( true == m_isCounting)
				m_isCounting = false;
			break;
			
		case eRESULTCODE.eRESULT_NOTIFY_ARENAMATCHING_COMPLETE:
			//Send_Pvp_Matching_GoInto( true);
			m_nGoIntoCountCheck = 0;
			m_fGoIntoTimeBuf = Time.realtimeSinceStartup;
			StartCoroutine( "PvpMatchingGoInto_Coroutine");
			break;
			
		case eRESULTCODE.eRESULT_NOTIFY_ARENAMATCHING_GOINTO_YES: break;

		case eRESULTCODE.eRESULT_NOTIFY_ARENAMATCHING_GOINTO_NO:
//			AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
//			if( null != userEntity && data.nCharUniqKey == userEntity.UniqueId)
//			{
//				AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage( AsTableManager.Instance.GetTbl_String( 965));
//				
//				// remove matching room
//				isMatching = false;
//			}
			if( AsUserInfo.Instance.SavedCharStat.uniqKey_ == data.nCharUniqKey)
			{
				AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage( AsTableManager.Instance.GetTbl_String( 965));
				isMatching = false; // remove matching room
			}
			
			_PvpMatchingCancelParty( data.nCharUniqKey);
			
			if( true == m_isCounting)
				m_isCounting = false;
			break;
		}
		
		Debug.Log( "Recv_Pvp_Matching_Notity(), eResult: " + data.eResult);
	}
#endif
	
	public void Recv_Pvp_Matching_LimitCount(body_SC_ARENA_MATCHING_LIMITCOUNT data)
	{
	}
	
#if INTEGRATED_ARENA_MATCHING
	public void Receive_Integrated_Arena_Gogogo(body_SC_INTEGRATED_ARENA_GOGOGO data)
	{
		AsWebRequest.Instance.Request_Indun( 160411, AsUserInfo.Instance.LoginUserUniqueKey, AsUserInfo.Instance.SavedCharStat.uniqKey_, data.nIndunTableIndex, data.nIndunDuplicationIndex);

		if( 0 == data.nIndunTableIndex)
		{
			_PvpGoIntoEnd();
			return;
		}
		
		// connect integrated pvp server
		AsNetworkIntegratedManager.Instance.ConnectToServer( data.szIpAddress, data.nPort);
		if( false == AsNetworkIntegratedManager.Instance.IsConnected())
		{
			Debug.LogError( "AsNetworkIntegratedManager.Instance.ConnectToServer(): Failed");
			Debug.Log( "ip: " + data.szIpAddress + ", port: " + data.nPort + ", id: " + data.nIndunTableIndex + ", did: " + data.nIndunDuplicationIndex);
			_PvpGoIntoEnd();
			return;
		}

		AsWebRequest.Instance.Request_Indun( 160402, AsUserInfo.Instance.LoginUserUniqueKey, AsUserInfo.Instance.SavedCharStat.uniqKey_, data.nIndunTableIndex, data.nIndunDuplicationIndex);
		
		// login possible
		body_CG_LOGIN_POSSIBLE possible = new body_CG_LOGIN_POSSIBLE();
		byte[] possibleData = possible.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( possibleData);
		
		// indun login
		uint uiUserUniqKey = AsUserInfo.Instance.LoginUserUniqueKey;
		body_CS_INTEGRATED_INDUN_LOGIN data2 = new body_CS_INTEGRATED_INDUN_LOGIN( uiUserUniqKey, data.nIndunTableIndex, data.nIndunDuplicationIndex, false);
		byte[] sendData = data2.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( sendData);

		AsWebRequest.Instance.Request_Indun( 160403, AsUserInfo.Instance.LoginUserUniqueKey, AsUserInfo.Instance.SavedCharStat.uniqKey_, data.nIndunTableIndex, data.nIndunDuplicationIndex);

		// remove matching room
		isMatching = false;
		
		m_nGogogoIndunTableIndex = data.nIndunTableIndex;
		Debug.Log( "ip: " + data.szIpAddress + ", port: " + data.nPort + ", id: " + data.nIndunTableIndex + ", did: " + data.nIndunDuplicationIndex);
	}
#else
	public void Recv_Pvp_Gogogo(body_SC_ARENA_GOGOGO data)
	{
		if( 0 == data.nIndunTableIndex)
		{
			_PvpGoIntoEnd();
			return;
		}
		
		body_CS_ENTER_INSTANCE data2 = new body_CS_ENTER_INSTANCE( data.nIndunTableIndex, data.nIndunDuplicationIndex);
		byte[] sendData = data2.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( sendData);

		// remove matching room
		isMatching = false;
		
		m_nGogogoIndunTableIndex = data.nIndunTableIndex;
	}
#endif
	
	public void Recv_Pvp_UserInfo_List(body_SC_ARENA_USERINFO_LIST data)
	{
		m_dicArenaUserInfoList.Clear();//$yde
		foreach(sARENAUSERINFO node in data.sInfo)
		{
			if(node.nCharUniqKey != 0)
				m_dicArenaUserInfoList.Add(node.nCharUniqKey, node);
		}
		
		AsPvpDlgManager.Instance.OpenPvpUserInfoDlg_Coroutine();
	}
	
	public void Recv_Pvp_EnterUser(body_SC_ARENA_ENTERUSER data)
	{
		if(m_dicArenaUserInfoList.ContainsKey(data.nCharUniqKey) == true)
		{
			m_dicArenaUserInfoList[data.nCharUniqKey].nEnterState = data.nEnterState;
			Debug.Log("AsPvpManager::Recv_Pvp_EnterUser: user[" + data.nCharUniqKey + "] entered arena");
		}
	}
	
	public void Recv_Pvp_RewardInfo(body_SC_ARENA_REWARDINFO data)
	{
		AsPvpDlgManager.Instance.OpenPvpRewardDlg_Coroutine( data, 3.0f);
	}
	
	public void Recv_Pvp_Arena_Ready(body_SC_ARENA_READY data)
	{
		AsPvpDlgManager.Instance.ClosePvpUserInfoDlg();
		AsPvpDlgManager.Instance.OpenPvpCountDlg_Coroutine();
	}
	
	public void Recv_Pvp_Arena_Start(body_SC_ARENA_START data)
	{
		//AsPvpDlgManager.Instance.ClosePvpUserInfoDlg();
		AsPvpDlgManager.Instance.ClosePvpCountDlg();
		
		AsPvpDlgManager.Instance.OpenPvpFightDlg_Coroutine();
		
		//int nCurIndunTableIndex = AsPvpDlgManager.Instance.m_PvpDlg.GetCurIndunTableIndex();
		Tbl_InDun_Record record = AsTableManager.Instance.GetInDunRecord( m_nGogogoIndunTableIndex);
		int nTimeDurationSec = 0;
		
		if( null != record)
			nTimeDurationSec = record.PvPDuration;
		else
			Debug.LogError( "AsPvpManager::Recv_Pvp_Arena_Start(): null == Tbl_InDun_Record, IndunTableIndex: " + m_nGogogoIndunTableIndex);
		
		AsPvpDlgManager.Instance.OpenPvpTimeDlg_Coroutine( nTimeDurationSec);
		
		m_lStartServerTime = data.nStartTime;
		
		StartCoroutine(_PetSkillAvailableProc_CR());
	}
	
	IEnumerator _PetSkillAvailableProc_CR()
	{
		yield return new WaitForSeconds(m_WaitTimeBeforeBegan);
		
		m_PetSkillAvailable = true;
	}
	#endregion

	public void SetBackGroundDelayTime(long lCurServerTime)
	{
		if( m_lStartServerTime > 0)
		{
			long lTimeDelay = lCurServerTime - m_lStartServerTime;
			
			int nBackGroundDelayTime = _GetTimeSec( lTimeDelay);
			AsPvpDlgManager.Instance.m_PvpTimeDlg.SetBackGroundDelayTime( nBackGroundDelayTime);
			
			m_lStartServerTime = 0;
		}
	}
	
	private int _GetTimeSec(long lTime)
	{
		System.DateTime dt = new System.DateTime(1970, 1, 1, 9, 0, 0);
		dt = dt.AddSeconds( lTime);
		return ( dt.Minute * 60 + dt.Second);
	}

	#region -private
	public void _OpenPvpMatchingCancelParty(uint nCharUniqKey, string strCharName)
	{
		AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
		
		if( null == userEntity)
		{
			Debug.LogError( "AsPvpManager::_OpenPvpMatchingCancelParty(): null == userEntity");
			return;
		}
		
		AS_PARTY_USER partyUser = AsPartyManager.Instance.GetPartyMember( userEntity.UniqueId);
		
		if( null == partyUser)
			return;

		if( nCharUniqKey == userEntity.UniqueId)
			return;
		
		string strTitle = AsTableManager.Instance.GetTbl_String( 897);
		string strMsg = string.Format( AsTableManager.Instance.GetTbl_String( 898), strCharName);
		OpenPvpMatchingErrorMsgBox( strTitle, strMsg);
	}
	
	public void _PvpMatchingCancelParty(uint nCharUniqKey)
	{
//		AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
//		
//		if( null == userEntity)
//		{
//			Debug.LogError( "AsPvpManager::_PvpMatchingCancelParty(): null == userEntity");
//			return;
//		}
		
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
				AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage( AsTableManager.Instance.GetTbl_String( 965));
			}
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
	
	private void _PvpGoIntoEnter()
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
	
	private void _PvpGoIntoEnd()
	{
		if( GAME_STATE.STATE_WARP_SEND == AsGameMain.s_gameState)
		{
			AsGameMain.s_gameState = GAME_STATE.STATE_INGAME;
			AsInputManager.Instance.m_Activate = true;
		}
	}
	#endregion
	
	#region - public -
	Dictionary<uint, sARENAUSERINFO> m_dicArenaUserInfoList = new Dictionary<uint, sARENAUSERINFO>(); //$yde
	public Dictionary<uint, sARENAUSERINFO> GetUserInfoList { get{ return m_dicArenaUserInfoList;}}
	
	public bool CheckUsersAreHostile(uint _user1, uint _user2)
	{
		if(CheckOneToOne() == true)
		{
			if(_user1 != _user2)
				return true;
			else
				return false;
		}
		else
		{
			int playerTeam = GetTeamType(_user1);
			int userTeam = GetTeamType(_user2);
			
			if(playerTeam != -1 && userTeam != -1 &&
				playerTeam != userTeam)
				return true;
			else
				return false;
		}
	}
	
	bool CheckOneToOne()
	{
		if(m_dicArenaUserInfoList.Count == 2)
			return true;
		else
			return false;
	}
	
	public int GetTeamType(uint _uniqKey)//$yde
	{
		if(m_dicArenaUserInfoList.ContainsKey(_uniqKey) == true)
			return m_dicArenaUserInfoList[_uniqKey].nTeamType;
		else
		{
//			Debug.Log("AsPvpManager::GetTeamType: uniq key is not found[" + _uniqKey + "]");
			return -1;
		}
	}
	
	public int GetMyTeamType()
	{
		AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
		if( null == userEntity)
		{
			Debug.Log( "AsPvpManager::GetMyTeamType(): null == userEntity");
			return -1;
		}
		
		return GetTeamType( userEntity.UniqueId);
	}
	
	public IEnumerator PvpMatchingGoInto_Coroutine()
	{
		m_isCounting = true;
		
		if( 0.0f == m_fGoIntoDelayTime)
		{
			Tbl_GlobalWeight_Record record = AsTableManager.Instance.GetTbl_GlobalWeight_Record( 88);
			if( null != record)
				m_fGoIntoDelayTime = record.Value * 0.001f;
			
			if( 0.0f == m_fGoIntoDelayTime)
			{
				Debug.LogWarning( "PvpMatchingGoInto_Coroutine(), 0.0f == m_fGoIntoDelayTime");
				m_fGoIntoDelayTime = 5.0f; // default
			}
		}

		string str = string.Format( AsTableManager.Instance.GetTbl_String( 899), (int)m_fGoIntoDelayTime);
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
					string strMsg = string.Format( AsTableManager.Instance.GetTbl_String( 899), (int)m_fGoIntoDelayTime - nTime);
					AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage( strMsg);
					m_nGoIntoCountCheck = nTime;
				}
			}
		}
		
		Send_Pvp_Matching_GoInto( true);
		m_isCounting = false;
	}

	public void OpenPvpMatchingErrorMsgBox(string strMsg)
	{
		if( null != m_msgboxPvpMatchingError)
			return;

		string strTitle = AsTableManager.Instance.GetTbl_String( 888);
		OpenPvpMatchingErrorMsgBox( strTitle, strMsg);
	}

	public void OpenPvpMatchingErrorMsgBox(string strTitle, string strMsg)
	{
		if( null != m_msgboxPvpMatchingError)
			return;

		m_msgboxPvpMatchingError = AsNotify.Instance.MessageBox( strTitle, strMsg, this, "OnMsgBox_PvpMatchingError_Ok", "OnMsgBox_PvpMatchingError_Ok", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
	}

	public void ClosePvpMatchingErrorMsgBox()
	{
		if( null != m_msgboxPvpMatchingError)
			m_msgboxPvpMatchingError.Close();
	}
	
	public void OnMsgBox_PvpMatchingError_Ok()
	{
		ClosePvpMatchingErrorMsgBox();
	}
	
	public void OpenPvpMatchingErrorMsgBox_Cash(int nIndunTableIndex)
	{
		if( null != m_msgboxPvpMatchingError_Cash)
			return;
		
		Tbl_InDun_Record record = AsTableManager.Instance.GetInDunRecord( nIndunTableIndex);
		if( null == record)
		{
			Debug.LogError( "AsPvpManager::OpenPvpMatchingErrorMsgBox_Cash(): null == Tbl_InDun_Record");
			return;
		}
		
		string strPvpModeName = AsTableManager.Instance.GetTbl_String( record.Name);

		string strTitle = AsTableManager.Instance.GetTbl_String( 888);
		string strMsg = string.Format( AsTableManager.Instance.GetTbl_String( 915), strPvpModeName, strPvpModeName);
		
		m_msgboxPvpMatchingError_Cash = AsNotify.Instance.MessageBox( strTitle, strMsg, this, "OnMsgBox_PvpMatchingError_Cash_Ok", "OnMsgBox_PvpMatchingError_Cash_Cancel", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
	}
	
	public void ClosePvpMatchingErrorMsgBox_Cash()
	{
		if( null != m_msgboxPvpMatchingError_Cash)
			m_msgboxPvpMatchingError_Cash.Close();
	}
	
	public void OnMsgBox_PvpMatchingError_Cash_Ok()
	{
		ClosePvpMatchingErrorMsgBox_Cash();

		if( true == AsPvpDlgManager.Instance.IsOpenPvpDlg)
			AsPvpDlgManager.Instance.ClosePvpDlg();
		
		// open cash shop
		AsHudDlgMgr.Instance.NotEnoughMiracleProcessInGame();
	}

	public void OnMsgBox_PvpMatchingError_Cash_Cancel()
	{
		ClosePvpMatchingErrorMsgBox_Cash();
	}
	
	public void OpenPvpMatchingCancelMsgBox()
	{
		if( null != m_msgboxPvpMatchingCancel)
			return;
		
		string strTitle = AsTableManager.Instance.GetTbl_String( 897);
		string strMsg = AsTableManager.Instance.GetTbl_String( 919);
		
		m_msgboxPvpMatchingCancel = AsNotify.Instance.MessageBox( strTitle, strMsg, this, "OnMsgBox_PvpMatchingCancel_Ok", "OnMsgBox_PvpMatchingCancel_Cancel", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
	}
	
	public void ClosePvpMatchingCancelMsgBox()
	{
		if( null != m_msgboxPvpMatchingCancel)
			m_msgboxPvpMatchingCancel.Close();
	}
	
	public void OnMsgBox_PvpMatchingCancel_Ok()
	{
		ClosePvpMatchingCancelMsgBox();
		Send_Pvp_Matching_Request( false, 0, false); // matching cancel
		
		isMatching = false;
	}

	public void OnMsgBox_PvpMatchingCancel_Cancel()
	{
		ClosePvpMatchingCancelMsgBox();
	}

	public void OnBtnPvpInfo()
	{
		if( false == m_isMatching && null == m_msgboxPvpMatchingCancel)
		{
			if( false == CheckInArena())
				Send_Pvp_Matching_Info();
		}
		else
		{
			OpenPvpMatchingCancelMsgBox();
		}
	}
	
	public bool CheckMatching()
	{
		if( true == m_isMatching)
		{
			AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage( AsTableManager.Instance.GetTbl_String( 894));
			return true;
		}
		
		return false;
	}
	
	public bool CheckInArena()
	{
		if( true == TargetDecider.CheckCurrentMapIsArena())
		{
			AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage( AsTableManager.Instance.GetTbl_String( 903));
			
			AsSoundManager.Instance.PlaySound_VoiceBattle(eVoiceBattle.str903_Cannot_Use_In_Duel);
			
			return true;
		}
		
		return false;
	}
	
//	public bool CheckPetSkillUsableInArena()
//	{
//		if( false == AsPvpManager.m_instance.PetSkillAvailable)
//			return true;
//		else
//			return false;
//	}

//	public int FindPvpGradeID(int nPvpPoint)
//	{
//		Tbl_PvpGrade_Table table = AsTableManager.Instance.GetPvpGradeTable();
//		
//		if( null == table)
//		{
//			Debug.Log( "AsPvpDlg::_FindPvpGradeID(): Tbl_PvpGrade_Table table == null");
//			return 0;
//		}
//
//		for( int i = 1; i < table.GetCount() + 1; i++)
//		{
//			Tbl_PvpGrade_Record record = AsTableManager.Instance.GetPvpGradeRecord( i);
//			if( null != record)
//			{
//				if( record.GradePoint > nPvpPoint)
//					return i - 1;
//			}
//			else
//			{
//				Debug.Log( "AsPvpDlg::_FindPvpGradeID(): Tbl_PvpGrade_Record record == null, ID: " + i);
//			}
//		}
//		
//		return 0;
//	}
	
	public int FindPvpGradeID(int nYesterdayPvpPoint, uint nYesterdayPvpRate)
	{
		Tbl_PvpGrade_Table table = AsTableManager.Instance.GetPvpGradeTable();
		
		if( null == table)
		{
			Debug.Log( "AsPvpDlg::_FindPvpGradeID(): Tbl_PvpGrade_Table table == null");
			return 0;
		}
		
		for( int i = table.GetCount(); i > 0; i--)
		{
			Tbl_PvpGrade_Record record = AsTableManager.Instance.GetPvpGradeRecord(i);
			if( null != record)
			{
				if( record.GradePoint <= nYesterdayPvpPoint)
				{
					for( int k = 0; k < i; k++)
					{
						Tbl_PvpGrade_Record record2 = AsTableManager.Instance.GetPvpGradeRecord( i - k);
						if( record2.GradeProp >= (int)nYesterdayPvpRate)
						{
							return ( i - k);
//							int res = i - k;
//							if( 0 == k && i < table.GetCount())
//								res -= 1;
//							if( res <= 0)
//								return 1;
//							else
//								return res;
						}
					}
				}
			}
			else
			{
				Debug.Log( "AsPvpDlg::_FindPvpGradeID(): Tbl_PvpGrade_Record record == null, ID: " + i);
			}
		}
#if false		
		for( int i = 1; i <= table.GetCount(); i++)
		{
			Tbl_PvpGrade_Record record = AsTableManager.Instance.GetPvpGradeRecord( i);
			if( null != record)
			{
				if( record.GradePoint > nYesterdayPvpPoint || table.GetCount() == i)
				{
					for( int k = 0; k < i; k++)
					{
						Tbl_PvpGrade_Record record2 = AsTableManager.Instance.GetPvpGradeRecord( i - k);
						if( record2.GradeProp >= (int)nYesterdayPvpRate)
						{
							int res = i - k;
							if( 0 == k && i < table.GetCount())
								res -= 1;
							if( res <= 0)
								return 1;
							else
								return res;
						}
					}
				}
			}
			else
			{
				Debug.Log( "AsPvpDlg::_FindPvpGradeID(): Tbl_PvpGrade_Record record == null, ID: " + i);
			}
		}
#endif
		
		return 0;
	}
	
	public void SetBackGroundProc(bool pause)
	{
		if( true == pause)
		{
			if( true == m_isCounting)
			{
				Send_Pvp_Matching_Request( false, 0, false); // matching cancel
				m_isCounting = false;
				isMatching = false;
				StopCoroutine( "PvpMatchingGoInto_Coroutine");
			}
			else
			{
				if( true == isMatching)
				{
					Send_Pvp_Matching_Request( false, 0, false); // matching cancel
					isMatching = false;
				}
			}
		}
	}
	
	public void SetActiveMatchingEff(bool bActive)
	{
		if( AsHudDlgMgr.Instance != null )
			AsHudDlgMgr.Instance.SetPvpBtnRollingEffect( bActive );
/*		
		if( null != m_MatchingEff)
		{
			m_MatchingEff.enabled = bActive;
			m_MatchingEff.renderer.enabled = bActive;
		}
*/		
	}
	
	public void ShowKillMessage(int nCharUniqKey_Death, int nCharUniqKey_AttackUser)
	{
		GameObject obj = ResourceLoad.LoadGameObject( "UI/AsGUI/PVP/GUI_PVP_KilledMessage");
		GameObject goKillMsg = GameObject.Instantiate( obj)  as GameObject;
		AsPvpKillMessage killMsg = goKillMsg.GetComponentInChildren<AsPvpKillMessage>();
		killMsg.ShowKillMessage( nCharUniqKey_Death, nCharUniqKey_AttackUser);
		
		foreach( AsPvpKillMessage kill in m_listKillMsg)
			kill.PositionUp();
		
		m_listKillMsg.Add( killMsg);
	}
	
	public void EnterArena()
	{
		m_PetSkillAvailable = false;
	}
	#endregion
	
//	void OnGUI()//$yde
//	{
//		if(GUI.Button(new Rect(900, 500, 80, 30), "pvp") == true)
//		{
//			OnBtnPvpInfo();
//		}
//	}
}
