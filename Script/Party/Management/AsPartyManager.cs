#define _PARTY_LOG_
#define _PARTY_ERROR_LOG_

using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.Text;


public class AS_PARTY_USER : AsPacketHeader
{
	public int nPartyIdx;
	public uint nCharUniqKey;
	public ushort nSessionIdx;//0이면 비정상 종료된 유저

	public string strCharName = string.Empty;

	public int eRace;
	public int eClass;
	public int nLevel;//캐릭터 레벨
	public int nImageTableIdx;

	public float fHpCur;
	public float fHpMax;

	public uint nCondition;
	public bool isCaptain;
	public string strChannelName = string.Empty;

	public PlayerBuffData[] m_BuffDataList;
}

public class AsSortPartyList
{
	public int m_LevelSortKey;//#21745
	public int m_MaxUserSortKey;
	public sPARTYLIST m_sPARTYLIST;
}

public class AsPartyManager : MonoBehaviour
{
	private const string m_strPartyUserUiPath = "UI/Optimization/Prefab/ListItem/GUI_PartyMember";
	private const string m_strPartyDiceUiPath = "UI/Optimization/Prefab/GUI_Dice";

	private static AsPartyManager ms_kIstance = null;
	GameObject m_RootObject;

	AsPartyUI m_PartyUI;
	AS_SC_PARTY_INVITE m_invite;
	sPARTYOPTION m_PartyOption = null;

	List<AsSortPartyList> m_SortPartyList = new List<AsSortPartyList>();
	Dictionary<int, sPARTYLIST> m_PartyList = new Dictionary<int, sPARTYLIST>();
	//Party Member
	Dictionary<uint, AS_PARTY_USER> m_PartyMembers = new Dictionary<uint, AS_PARTY_USER>();//Char unique id( user only)
	Dictionary<uint, AS_PARTY_USER> m_TempPartyMembers = new Dictionary<uint, AS_PARTY_USER>();//Char unique id( user only)

	private ArrayList m_PartyDiceDlg = new ArrayList();
	private Queue m_DiceQue = new Queue();
	private AsPartyDiceDlg m_CurDiceDlg = null;
	private	Vector3 m_MemberDlg_pos;
	bool m_bPartyNotice;
	public bool IsPartyNotice
	{
		get	{ return m_bPartyNotice; }
		set
		{
			m_bPartyNotice = value;
			m_PartyUI.PartyEditDlg.SetPartyNotice();
		}
	}
	string m_PartyNotice; public string PartyNotice{get{return m_PartyNotice;} set{ m_PartyNotice = value;}}

	public static AsPartyManager Instance
	{
		get { return ms_kIstance; }
	}

	public AsPartyUI PartyUI
	{
		get { return m_PartyUI; }
	}

	private string m_PartyName = null;
	public string PartyName
	{
		get { return m_PartyName; }
		set { m_PartyName = value; }
	}
	private int m_nPartyIdx = 0;
	public int PartyIdx
	{
		get { return m_nPartyIdx; }
		set { m_nPartyIdx = value;}
	}

	private bool m_IsReadyUI = false;

	private bool m_IsCaptain = false;
	public bool IsCaptain
	{
		get { return m_IsCaptain; }
		set { m_IsCaptain = value;
			
			#region - target mark -
			AsHUDController.Instance.ActivateTargetMarkBtn( m_IsCaptain);
			#endregion
		}
	}

	private bool m_IsPartying = false;
	public bool IsPartying
	{
		get { return m_IsPartying; }
		set { m_IsPartying = value; if( m_IsPartying == true) AutoCombatManager.Instance.PartyOrganized(); /*$yde*/}
	}

	private bool m_IsPartyJoin = false;
	public bool IsPartyJoin
	{
		get { return m_IsPartyJoin; }
		set { m_IsPartyJoin = value; }
	}

	//변경시( n), 변경하지 않음( 0)	//<-- 추가 2014.01.10.
	private int m_BeforeMapIdx = 0;
	public int BeforeMapIdx //My MapIdx
	{
		get { return m_BeforeMapIdx; }
		set { m_BeforeMapIdx = value; }
	}

	private int m_PartyMapIdx = 0;
	public int PartyMapIdx //My MapIdx
	{
		get { return m_PartyMapIdx; }
		set {
				m_BeforeMapIdx = m_PartyMapIdx;
				m_PartyMapIdx = value;
			}
	}

	private int m_PartyListUIMapIdx = 0;
	public int PartyListUIMapIdx
	{
		get { return m_PartyListUIMapIdx; }
		set { m_PartyListUIMapIdx = value; }
	}

	public sPARTYOPTION PartyOption
	{
		get { return m_PartyOption; }
		set
		{
			m_PartyOption = value;
			m_PartyName = System.Text.UTF8Encoding.UTF8.GetString( m_PartyOption.szPartyName);
			if( m_IsReadyUI)
			{
				SetPartyEditSolt();
				if( m_PartyUI.PartyCreateDlg.gameObject.active == true)
					m_PartyUI.PartyCreateDlg.SetPartyOption();
			}
		}
	}
	private int m_SendDetailPartyIdx = 0;
	public int SendDetailPartyIdx
	{
		get { return m_SendDetailPartyIdx; }
		set { m_SendDetailPartyIdx = value; }
	}

	private int m_SendNoticePartyIdx = 0;
	public int SendNoticePartyIdx
	{
		get { return m_SendNoticePartyIdx; }
		set { m_SendNoticePartyIdx = value; }
	}

	private GameObject m_PartyInfoDlg = null;
	public GameObject PartyInfoDlg
	{
		get { return m_PartyInfoDlg; }
		set { m_PartyInfoDlg = value; }
	}

	private UIInvenSlot m_ClickDownItemSlot;

	private AsMessageBox m_MsgBox_PartyExit = null;
	private AsMessageBox m_MsgBox_PartyInvite = null;
	private AsMessageBox m_MsgBox_BannedExit = null;
	private AsMessageBox m_MsgBox_PartyOption = null;
	private AsMessageBox m_MsgBox_PartySearch = null;
	private AsMessageBox m_MsgBox_PartyWarpXZ = null;
	
	public AsMessageBox MsgBox_PartySearch
	{
		get { return m_MsgBox_PartySearch; }
		set { m_MsgBox_PartySearch = value; }
	}
	private uint m_BannedExitId = 0;

	uint m_MyUniqueId = uint.MaxValue;
	public uint MyUniqueId	{ get	{ return m_MyUniqueId; } } //#20053.

	public int m_nJoinRequestCount = 0;
	public int JoinRequestCount
	{
		get { return m_nJoinRequestCount; }
		set { m_nJoinRequestCount = value; }
	}
	
	private ulong m_iNeedGold = 0;
	private body1_SC_PARTY_USER_POSITION m_UserPosition;
	private bool m_bPartyWarpXZMsgBox =false; 
	public bool IsPartyWarpXZMsgBox{
		get { return m_bPartyWarpXZMsgBox; }
		set { m_bPartyWarpXZMsgBox = value; }
	}
	
	private bool m_bDivideItemChange =false; 
	public bool IsDivideItemChange{
		get { return m_bDivideItemChange; }
		set { m_bDivideItemChange = value; }
	}
	
	private uint 	m_targetCharUniqKey;
	public uint TargetCharUniqKey
	{
		get{return m_targetCharUniqKey;}
		set{m_targetCharUniqKey = value;}
	}
	
	void Awake()
	{
		if( ms_kIstance == null)
		{
			ms_kIstance = this;

			m_RootObject = new GameObject( "Party");
			DontDestroyOnLoad( m_RootObject);
			DDOL_Tracer.RegisterDDOL( this, m_RootObject);//$ yde
			m_PartyUI = m_RootObject.AddComponent<AsPartyUI>();
		}
	}

	public void SetTargetByPartyMember( uint id)
	{
		m_PartyUI.PartyMemberUI.SetTarget(id);
	}

	public void Initilize()
	{
		PartyUserRemoveAll();
	}

	public void InitUI()
	{
		m_MsgBox_PartyExit = null;
		m_MsgBox_PartyInvite = null;
		m_MsgBox_BannedExit = null;
		m_MsgBox_PartyOption = null;
		m_MsgBox_PartySearch = null;
		m_MsgBox_PartyWarpXZ = null;//#21473.
		
		m_IsReadyUI = true;

		if( IsCaptain)//#19804.
		{
			AsMyProperty instance = FindObjectOfType( typeof( AsMyProperty)) as AsMyProperty;
			if( instance)
				AsMyProperty.Instance.SetCaptain( true);
		}

		if( IsPartyNotice)
		{
			AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
			if( null != userEntity)
				userEntity.ShowPartyPR( IsPartyNotice, PartyNotice);
		}

		m_PartyUI.ClosePartyList();


		if (AsHudDlgMgr.Instance != null)
			if (AsHudDlgMgr.Instance.partyAndQuestToggleMgr.NowState == AsPartyAndQuestToggleMgr.PartyAndQuestToggleState.OpenParty)
				m_PartyUI.PartyMemberUI.ReSetPartyMember(); //#20675
	}

	public void PartyCreate()
	{
		if( m_MsgBox_PartyInvite != null)
		{
			AsPartySender.SendPartyJoin( ( int)ePARTYJOINTYPE.ePARTYJOINTYPE_REFUSE, m_invite.nPartyIdx, m_invite.nCharUniqKey);
			m_MsgBox_PartyInvite.Close();
		}

		AsPartySender.SendPartyCreate( PartyMapIdx, m_PartyOption);
		IsPartying = true;
		IsCaptain = true;
	}

	public void SelfPartyCreate( string strPartyName)
	{
		PartyMapIdx = TerrainMgr.Instance.GetCurMapID();
		m_PartyName = strPartyName;
		CreateOption();
		AsPartySender.SendPartyCreate( PartyMapIdx, m_PartyOption);
		IsPartying = true;
		IsCaptain = true;
	}

	#region--MessageBox
	public void PartyOptionMsgBox()
	{
		if( m_MsgBox_PartyOption == null)
			m_MsgBox_PartyOption = AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(1117), AsTableManager.Instance.GetTbl_String(53), this, "OnMsgBox_PartyOption_Ok", "OnMsgBox_PartyOption_Cancel", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);
	}

	public void OnMsgBox_PartyOption_Ok()
	{
		m_PartyUI.PartyCreateDlg.ConvertToPartyOption();
		AsPartySender.SendPartySetting( PartyMapIdx, PartyOption);
		if( m_PartyUI.PartyCreateDlg.IsBack)
			m_PartyUI.OpenPartyList();
		m_PartyUI.PartyCreateDlg.Close();
	}

	public void OnMsgBox_PartyOption_Cancel()
	{
		if( m_PartyUI.PartyCreateDlg.IsBack)
			m_PartyUI.OpenPartyList();
		m_PartyUI.PartyCreateDlg.Close();
	}

	public void PartyExitMsgBox()
	{
		if( m_MsgBox_PartyExit == null)
			m_MsgBox_PartyExit = AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(1117), AsTableManager.Instance.GetTbl_String(51), this, "OnMsgBox_PartyExit_Ok", "OnMsgBox_PartyExit_Cancel", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);
	}

	public void OnMsgBox_PartyExit_Ok()
	{
		if( true == AsPartyManager.Instance.IsCaptain && AsPartyManager.Instance.PartyOption.bPublic)
		{
			if( AsPartyManager.Instance.IsPartyNotice)
				AsPartySender.SendPartyNoticeOnOff( false, "");
		}
		AsPartySender.SendPartyExit();
	}

	public void OnMsgBox_PartyExit_Cancel()
	{
	}

	public void BannedExitMsgBox( uint nCharUniqKey)
	{
		if( m_MsgBox_BannedExit == null)
		{
			m_BannedExitId = nCharUniqKey;
			AS_PARTY_USER partyUser = GetPartyMember( nCharUniqKey);
			if( partyUser == null)
			{
				StringBuilder sb = new StringBuilder();
				sb.Append( AsTableManager.Instance.GetTbl_String(117));
				sb.Append( "[P00008]");
				//string strErrMsg = AsTableManager.Instance.GetTbl_String(117) + "[P00008]";
				AsChatManager.Instance.InsertChat( sb.ToString(), eCHATTYPE.eCHATTYPE_SYSTEM);
				return;
			}

			string strMsg = string.Format( AsTableManager.Instance.GetTbl_String(52),partyUser.strCharName);
			m_MsgBox_BannedExit = AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(1117),strMsg, this, "OnMsgBox_BannedExit_Ok", "OnMsgBox_BannedExit_Cancel", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);
		}
	}

	public void OnMsgBox_BannedExit_Ok()
	{
		AS_PARTY_USER partyUser = GetPartyMember( m_BannedExitId);
		if( partyUser == null)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append( AsTableManager.Instance.GetTbl_String(117));
			sb.Append( "[P00008]");
			//string strErrMsg = AsTableManager.Instance.GetTbl_String(117) + "[P00008]";
			AsChatManager.Instance.InsertChat( sb.ToString(), eCHATTYPE.eCHATTYPE_SYSTEM);
			return;
		}

		AsPartySender.SendPartyBannedExit( partyUser.nSessionIdx, partyUser.nCharUniqKey);
	}

	public void OnMsgBox_BannedExit_Cancel()
	{
	}

	public void PartyInvite( AS_SC_PARTY_INVITE invite)
	{
		if( m_MsgBox_PartyInvite == null)
		{
			m_invite = invite;

			if( AsHudDlgMgr.Instance.IsOpenCashStore == true)
			{
				AsPartySender.SendPartyJoin( ( int)ePARTYJOINTYPE.ePARTYJOINTYPE_REFUSE, m_invite.nPartyIdx, m_invite.nCharUniqKey);
			}
			else
			{
				if( invite.onOff)
				{
					string userName = Encoding.UTF8.GetString( m_invite.szCharName);
					string strMsg = string.Format( AsTableManager.Instance.GetTbl_String(43), AsUtil.GetRealString( userName),AsUtil.GetRealString( Encoding.UTF8.GetString( invite.szPartyNotice)));
					m_MsgBox_PartyInvite = AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(1117), strMsg, this, "OnMsgBox_PartyInvite_Ok", "OnMsgBox_PartyInvite_Cancel", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);
				}
				else
				{
					string userName = Encoding.UTF8.GetString( m_invite.szCharName);
					string strMsg = string.Format( AsTableManager.Instance.GetTbl_String(43), AsUtil.GetRealString( userName),"");
					m_MsgBox_PartyInvite = AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(1117), strMsg, this, "OnMsgBox_PartyInvite_Ok", "OnMsgBox_PartyInvite_Cancel", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);
				}
			}
		}
		else
		{
			AsPartySender.SendPartyJoin( ( int)ePARTYJOINTYPE.ePARTYJOINTYPE_PLAYING, invite.nPartyIdx, invite.nCharUniqKey);
		}
	}

	public void OnMsgBox_PartyInvite_Ok()
	{
		AllClosePartyUI();
		AsPartySender.SendPartyJoin( ( int)ePARTYJOINTYPE.ePARTYJOINTYPE_ACCEPT, m_invite.nPartyIdx, m_invite.nCharUniqKey);
	}

	public void OnMsgBox_PartyInvite_Cancel()
	{
		AsPartySender.SendPartyJoin( ( int)ePARTYJOINTYPE.ePARTYJOINTYPE_REFUSE, m_invite.nPartyIdx, m_invite.nCharUniqKey);
		
		#region - emoticon -
		AsEmotionManager.Instance.Event_Condition_PartyReject(); //$yde
		#endregion
	}

	public void SetJoinRequestNotify( body_SC_PARTY_JOIN_REQUEST_NOTIFY joinRequestNotify)
	{
		GameObject go = Instantiate( Resources.Load( "UI/AsGUI/PartyMatching/GUI_PartyJoinRequestDlg")) as GameObject;
		AsPartyJoinRequestDlg partyJoinRequestDlg = go.GetComponent<AsPartyJoinRequestDlg>();
		Vector3 pos = go.transform.position;
		pos.z += JoinRequestCount * -0.5f;
		go.transform.position = pos;
		partyJoinRequestDlg.Init( joinRequestNotify);
		JoinRequestCount++;
	}
	
	private void PartyWarpXZMsgBox( )
	{	
		int iMapIdx = 0;
		int iWarpIdx = 0;
		
		if(m_MsgBox_PartyWarpXZ == null)
		{
			string channelName = string.Empty;
			
			bool 	isWarpTargetPartyUserFind = false;
			foreach( body2_SC_PARTY_USER_POSITION _data in m_UserPosition.body )
			{
				AS_PARTY_USER partyUser = GetPartyMember(_data.nCharUniqKey);
				if( m_targetCharUniqKey == 0 )
				{
					if( partyUser.isCaptain == true )
						isWarpTargetPartyUserFind = true;
				}
				else
				{
					if( partyUser.nCharUniqKey == m_targetCharUniqKey )
						isWarpTargetPartyUserFind = true;
				}
				
				if( isWarpTargetPartyUserFind == true )
				{
					iMapIdx = _data.nMapIdx;
					channelName = partyUser.strChannelName;
					break;
				}
			}
			
			//#22697
			if (iMapIdx == TerrainMgr.Instance.GetCurMapID() && channelName == AsUserInfo.Instance.currentChannelName) 
			{
				AsEventNotifyMgr.Instance.CenterNotify.AddPartyMoveNotice( AsTableManager.Instance.GetTbl_String(2157) );
				return;
			}
			
			Tbl_WarpData_Record record = AsTableManager.Instance.GetWarpDataIdxToUseMapID(iMapIdx );
			if( null == record ) return;						
			
			Map _warpMap = TerrainMgr.Instance.GetMap(record.getWarpMapId);
			if( null == _warpMap ) return;
			
			Map _curMap = TerrainMgr.Instance.GetMap(TerrainMgr.Instance.GetCurMapID());
			if( null == _curMap ) return;		
			
			
			m_iNeedGold = (1 + (ulong)Mathf.Abs( _warpMap.MapData.getMapLevel - _curMap.MapData.getMapLevel )) * (ulong)record.getGoldCost;			
		
			string msg = string.Format(AsTableManager.Instance.GetTbl_String(2045), AsTableManager.Instance.GetTbl_String(_warpMap.MapData.GetNameStrIdx()), channelName);
			
			m_MsgBox_PartyWarpXZ = AsNotify.Instance.GoldMessageBox( m_iNeedGold, AsTableManager.Instance.GetTbl_String(126), msg,
											this, "GoPotal", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);
			
			m_MsgBox_PartyWarpXZ.SetFullscreenCollider();//#21480
		}
	}
	
	private void GoPotal()
	{		
		if( true == AsUserInfo.Instance.IsDied())
			return;

		if( AsPStoreManager.Instance.UnableActionByPStore() == true)
		{
			AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(365),
									null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE));
			return;
		}		
		
		if( AsUserInfo.Instance.SavedCharStat.nGold < m_iNeedGold )
		{				
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(252),
											null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			return;
		}
		
		if( AsUserInfo.Instance.IsBattle() )			
		{
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(1633),
											null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			return;
		}		
	
		AsPartySender.SendWarpXZ( m_targetCharUniqKey );				
	}
	
	#endregion --MessageBox
	void CreateOption()
	{
		m_PartyOption = new sPARTYOPTION();

		string strPartyName= "Party";
		AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
		if( null != userEntity /*&& m_PartyName.Length > 0*/)//#21568
		{
			StringBuilder sb = new StringBuilder();
			sb.Append( userEntity.GetProperty<string>( eComponentProperty.NAME));
			sb.Append( AsTableManager.Instance.GetTbl_String(1357));			
			strPartyName = sb.ToString();
		}

		byte[] szPartyName = System.Text.UTF8Encoding.UTF8.GetBytes( strPartyName.ToCharArray(), 0, strPartyName.Length);
		int length = ( AsGameDefine.MAX_PARTYNAME_LEN <szPartyName.Length) ? AsGameDefine.MAX_PARTYNAME_LEN : szPartyName.Length;
		System.Buffer.BlockCopy( szPartyName, 0, m_PartyOption.szPartyName, 0, length);

		m_PartyOption.bPublic = true;
		m_PartyOption.ePurpose = ( int)ePARTYPURPOSE.ePARTYPURPOSE_HUNT;
		m_PartyOption.eDivide = ( int)ePARTYITEMDIVIDE.ePARTYITEMDIVIDE_EQUIP;//.ePARTYITEMDIVIDE_FREE;
		m_PartyOption.eGrade = ( int)eEQUIPITEM_GRADE.eEQUIPITEM_GRADE_MAGIC;

		m_PartyOption.nMaxUser = AsGameDefine.MAX_PARTY_USER;
	}

	// Use this for initialization
	void Start()
	{
		m_PartyName = "";
		CreateOption();
	}

	// Update is called once per frame
	void Update()
	{
		foreach( AsPartyDiceDlg diceDlg in m_PartyDiceDlg)
		{
			diceDlg.UpdateGiveUpTime();
		}
	}

	#region -PartyList
	public void ReceivePartyList( sPARTYLIST[] _list)
	{
		m_PartyList.Clear();
		m_SortPartyList.Clear();
		if( m_IsReadyUI)
			m_PartyUI.PartyListDlg.ClearPartyList();

		if( null == _list)
			return;

		foreach( sPARTYLIST data in _list)
		{
			m_PartyList.Add( data.nPartyIdx, data);
			AsSortPartyList sortData = new AsSortPartyList();
			sortData.m_sPARTYLIST = data;
			m_SortPartyList.Add( sortData);
		}

		if( m_PartyUI.PartyListDlg.eSortType == AsPartyListDlg.ePARTYLISTSORT.ePARTYLISTSORT_LEVEL)
			LevelSortList();
		else
			MaxUserSortList();

		if( m_IsReadyUI)
		{
			m_PartyUI.PartyListDlg.SetPartyList();
		}
	}

	public sPARTYLIST GetPartyList( int _id)
	{
		if( m_PartyList.ContainsKey( _id) == true)
			return m_PartyList[_id];
		else
			return null;
	}

	public Dictionary<int, sPARTYLIST> GetPartyList()
	{
		return m_PartyList;
	}

	public List<AsSortPartyList> GetSortPartyList()
	{
		return m_SortPartyList;
	}
	#endregion -PartyList

	#region - PartyMember
	public AS_PARTY_USER GetPartyMember( uint _id)
	{
		if( m_PartyMembers.ContainsKey( _id) == true)
			return m_PartyMembers[_id];
		else
			return null;
	}

	public Dictionary<uint, AS_PARTY_USER> GetPartyMemberList()
	{
		return m_PartyMembers;
	}

	//Dice
	public AS_PARTY_USER TempPartyMember( uint _id)
	{
		if( m_TempPartyMembers.ContainsKey( _id) == true)
			return m_TempPartyMembers[_id];
		else
			return null;
	}

	public void SetCaptainCenterNotify( uint _id)//#20209.
	{
		foreach( KeyValuePair<uint, AS_PARTY_USER> member in m_PartyMembers)
		{
			if( _id == member.Value.nCharUniqKey)
			{
				string strMsg = string.Format( AsTableManager.Instance.GetTbl_String(1736), member.Value.strCharName);
				AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage( strMsg);//#20058.
			}
		}
	}

	public void SetCaptain( uint _id)
	{
		foreach( KeyValuePair<uint, AS_PARTY_USER> member in m_PartyMembers)
		{
			member.Value.isCaptain = false;
			if( _id == member.Value.nCharUniqKey)
			{
				member.Value.isCaptain = true;
			}
		}

		AsMyProperty instance = FindObjectOfType( typeof( AsMyProperty)) as AsMyProperty;
		if( instance)
		AsMyProperty.Instance.SetCaptain( false);

		IsCaptain = false;
		if( _id == MyUniqueId)
		{
			IsCaptain = true;

			if( instance)
				AsMyProperty.Instance.SetCaptain( true);
			m_PartyUI.PartyCreateDlg.SetPartyOption();
		}
		else
		{
			if( m_MsgBox_BannedExit != null)
				m_MsgBox_BannedExit.Close();

			if( m_MsgBox_PartyOption != null)
				m_MsgBox_PartyOption.Close();
		}
		//
		SetPartyEditSolt();
		m_PartyUI.PartyMemberUI.SetCaptain( _id);
	}

	private void SetPartyEditSolt()
	{
		if( !m_IsReadyUI)
			return;

		m_PartyUI.PartyEditDlg.SetPartyEditSlot();
	}

	/*
	public void PartyMyAdd()
	{
		AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
		if( null != userEntity)
		{
			m_MyUniqueId = userEntity.UniqueId;
			string userName = userEntity.GetProperty<string>( eComponentProperty.NAME);
			AS_PARTY_USER new_member = new AS_PARTY_USER();

			new_member.nCharUniqKey = userEntity.UniqueId;//.nCharUniqKey;
			new_member.nSessionIdx = userEntity.SessionId;
			new_member.strCharName = userName;
			new_member.eRace = ( int)userEntity.GetProperty<eRACE>( eComponentProperty.RACE);
			new_member.eClass = ( int)userEntity.GetProperty<eCLASS>( eComponentProperty.CLASS);
			new_member.nLevel = 0;
			new_member.fHpCur = 0.0f;
			new_member.fHpMax = 0.0f;
			new_member.nCondition = 0;
			new_member.nImageTableIdx = AsDelegateImageManager.Instance.AssignedImageID;
			m_PartyMembers.Add( new_member.nCharUniqKey, new_member);

			AS_PARTY_USER tmp_member = new AS_PARTY_USER();

			tmp_member.nCharUniqKey = userEntity.UniqueId;
			tmp_member.nSessionIdx = userEntity.SessionId;
			tmp_member.strCharName = userName;
			tmp_member.eRace = ( int)userEntity.GetProperty<eRACE>( eComponentProperty.RACE);;
			tmp_member.eClass = ( int)userEntity.GetProperty<eCLASS>( eComponentProperty.CLASS);
			tmp_member.nLevel = 0;
			tmp_member.fHpCur = 0.0f;
			tmp_member.fHpMax = 0.0f;
			tmp_member.nCondition = 0;
			tmp_member.nImageTableIdx = AsDelegateImageManager.Instance.AssignedImageID;
			if( m_TempPartyMembers.ContainsKey( tmp_member.nCharUniqKey) == false)
				m_TempPartyMembers.Add( tmp_member.nCharUniqKey, tmp_member);

			QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_ADD_PARTY, new AchAddParty(m_PartyMembers.Count));
		}
	}
	*/
	public void PartyMyAdd()
	{
		StartCoroutine( "PartyMyAdd_Coroutine");
	}

	IEnumerator PartyMyAdd_Coroutine()
	{
		while( true)
		{
			if( null != AsUserInfo.Instance.GetCurrentUserEntity())
				break;

			yield return null;
		}

		AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();

		m_MyUniqueId = userEntity.UniqueId;
		string userName = userEntity.GetProperty<string>( eComponentProperty.NAME);
		AS_PARTY_USER new_member = new AS_PARTY_USER();
		
		new_member.nCharUniqKey = userEntity.UniqueId;//.nCharUniqKey;
		new_member.nSessionIdx = userEntity.SessionId;
		new_member.strCharName = userName;
		new_member.eRace = ( int)userEntity.GetProperty<eRACE>( eComponentProperty.RACE);
		new_member.eClass = ( int)userEntity.GetProperty<eCLASS>( eComponentProperty.CLASS);
		new_member.nLevel = 0;
		new_member.fHpCur = 0.0f;
		new_member.fHpMax = 0.0f;
		new_member.nCondition = 0;
		new_member.nImageTableIdx = AsDelegateImageManager.Instance.AssignedImageID;
		m_PartyMembers.Add( new_member.nCharUniqKey, new_member);
		
		AS_PARTY_USER tmp_member = new AS_PARTY_USER();
		
		tmp_member.nCharUniqKey = userEntity.UniqueId;
		tmp_member.nSessionIdx = userEntity.SessionId;
		tmp_member.strCharName = userName;
		tmp_member.eRace = ( int)userEntity.GetProperty<eRACE>( eComponentProperty.RACE);;
		tmp_member.eClass = ( int)userEntity.GetProperty<eCLASS>( eComponentProperty.CLASS);
		tmp_member.nLevel = 0;
		tmp_member.fHpCur = 0.0f;
		tmp_member.fHpMax = 0.0f;
		tmp_member.nCondition = 0;
		tmp_member.nImageTableIdx = AsDelegateImageManager.Instance.AssignedImageID;
		if( m_TempPartyMembers.ContainsKey( tmp_member.nCharUniqKey) == false)
			m_TempPartyMembers.Add( tmp_member.nCharUniqKey, tmp_member);
		
		QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_ADD_PARTY, new AchAddParty(m_PartyMembers.Count));
	}

	public void PartyUserAdd( AS_SC_PARTY_USER_ADD data, bool bOption = true)
	{
		if( null == data)
			return;

		PartyIdx = data.nPartyIdx;

		if( data.nPartyIdx == 0)
		{
			string strMsg = "Error PartyUserAdd Party Index zero !!!!";
			Debug.LogError( strMsg);
			AsChatManager.Instance.InsertChat( strMsg, eCHATTYPE.eCHATTYPE_SYSTEM);
		}

		if( 0 == m_PartyMembers.Count)
		{
			AsPartyManager.Instance.TargetCharUniqKey = 0;
			
			AsPartySender.SendPartyUserPosition();
			AsPartyManager.Instance.IsPartyWarpXZMsgBox = true;
			
			if( null != MsgBox_PartySearch)
				MsgBox_PartySearch.Close();
			PartyMyAdd();
			IsPartying = true;
			if( bOption && false == AsPvpManager.Instance.isMatching && false == AsInstanceDungeonManager.Instance.isMatching)
				AsPartySender.SendPartyDetailInfo( data.nPartyIdx);
			
		}

		if( 0 == m_PartyMembers.Count)
		{
			if( AsHudDlgMgr.Instance != null)
				if( AsHudDlgMgr.Instance.partyAndQuestToggleMgr != null)
					AsHudDlgMgr.Instance.partyAndQuestToggleMgr.AddPartyProcess();
		}

		AS_PARTY_USER member = GetPartyMember( data.nCharUniqKey);
		if( member == null)
		{
			AS_PARTY_USER new_member = new AS_PARTY_USER();

			new_member.nCharUniqKey = data.nCharUniqKey;
			new_member.nSessionIdx = data.nSessionIdx;
			new_member.strCharName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( data.szCharName));
			new_member.eRace 	= data.eRace;
			new_member.eClass = data.eClass;
			new_member.nLevel = data.nLevel;
			new_member.nImageTableIdx = data.nImageTableIdx;

			new_member.fHpCur = data.fHpCur;
			new_member.fHpMax = data.fHpMax;
			new_member.nCondition = data.nCondition;
			new_member.strChannelName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( data.szChannelName));
			m_PartyMembers.Add( new_member.nCharUniqKey, new_member);

			AS_PARTY_USER tmp_member = new AS_PARTY_USER();

			tmp_member.nCharUniqKey = data.nCharUniqKey;
			tmp_member.nSessionIdx = data.nSessionIdx;
			tmp_member.strCharName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( data.szCharName));
			tmp_member.eRace 	= data.eRace;
			tmp_member.eClass = data.eClass;
			tmp_member.nLevel = data.nLevel;
			tmp_member.nImageTableIdx = data.nImageTableIdx;

			tmp_member.fHpCur = data.fHpCur;
			tmp_member.fHpMax = data.fHpMax;
			tmp_member.nCondition = data.nCondition;
			tmp_member.strChannelName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( data.szChannelName));
			if( m_TempPartyMembers.ContainsKey( tmp_member.nCharUniqKey) == false)
				m_TempPartyMembers.Add( tmp_member.nCharUniqKey, tmp_member);

			member = new_member;
		}
		else
		{
			member.nCharUniqKey = data.nCharUniqKey;
			member.nSessionIdx = data.nSessionIdx;
			member.strCharName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( data.szCharName));
			member.eRace 	= data.eRace;
			member.eClass = data.eClass;
			member.nLevel = data.nLevel;
			member.nImageTableIdx = data.nImageTableIdx;

			member.fHpCur = data.fHpCur;
			member.fHpMax = data.fHpMax;
			member.nCondition = data.nCondition;
			member.strChannelName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( data.szChannelName));
		}

		if( AsHudDlgMgr.Instance != null)
			if( AsHudDlgMgr.Instance.partyAndQuestToggleMgr != null)
				AsHudDlgMgr.Instance.partyAndQuestToggleMgr.AddPartyProcess();

		if( 1 < m_PartyMembers.Count)
			AsPartyTrackManager.Instance.SetEvent(AsPartyTrackManager.ePartyTrackEvent.eJoin_Party);

		if( !m_IsReadyUI)
			return;

		AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
		if( null != userEntity)
		{
			if( data.nCharUniqKey == userEntity.UniqueId)
			{
				IsPartying = true;
				return;
			}
		}

		m_PartyUI.PartyMemberUI.ReSetPartyMember();

		SetPartyEditSolt();

		QuestMessageBroadCaster.BrocastQuest( QuestMessages.QM_ADD_PARTY, new AchAddParty( m_PartyMembers.Count));

		AutoDelPartyNotice();
	}

	public void PartyUserInfo( AS_SC_PARTY_USER_INFO data)
	{
		AsPartyMemberDlg memberDlg	= m_PartyUI.PartyMemberUI.GetPartyMemberDlgByUniqueId( data.nCharUniqKey);
		if( null != memberDlg)
		{
			AS_PARTY_USER member = GetPartyMember( data.nCharUniqKey);
			if( null != member)
			{
				member.nCharUniqKey = data.nCharUniqKey;
				member.nLevel = data.nLevel;
				member.fHpCur = data.fHpCur;
				member.fHpMax = data.fHpMax;
				member.nCondition 	 = data.nCondition;
				member.nImageTableIdx = data.nImageTableIdx;
				member.strChannelName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( data.szChannelName));

				memberDlg.PartyUserInfo( member);
			}
		}

		SetPartyEditSolt();
	}

	public void PartyUserDel( AS_SC_PARTY_USER_DEL data)
	{
		if( null == data)
			return;

		AS_PARTY_USER user = GetPartyMember( data.nCharUniqKey);
		if( null == user)
		{
			Debug.LogError( "PartyUserDel() user not found!!!");
			return;
		}

		AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
		if( null == userEntity)
		{
			if( MyUniqueId == data.nCharUniqKey)
				PartyUserRemoveAll();
			else
				PartyUserRealDel( data.nCharUniqKey);

			return;
		}

		if( data.bBanned)//강제 추방 여부
		{
			if( data.nCharUniqKey == userEntity.UniqueId)
				PartyUserRemoveAll();
			else
				PartyUserRealDel( data.nCharUniqKey);

			StringBuilder sb = new StringBuilder();
			sb.AppendFormat( AsTableManager.Instance.GetTbl_String(49), user.strCharName);
			AsEventNotifyMgr.Instance.CenterNotify.AddGuildNotice( sb.ToString());
			AsChatManager.Instance.InsertChat( sb.ToString(), eCHATTYPE.eCHATTYPE_SYSTEM);
//			string strMsg = string.Format( AsTableManager.Instance.GetTbl_String(49), user.strCharName);
//			AsChatManager.Instance.InsertChat( strMsg, eCHATTYPE.eCHATTYPE_SYSTEM);

			return;
		}

		if( data.bDisconnect)//비정상적인 종료( true : 비정상 종료, false : 일반 종료)
		{
			//UI작업
			AsPartyMemberDlg memberDlg	= m_PartyUI.PartyMemberUI.GetPartyMemberDlgByUniqueId( data.nCharUniqKey);
			if( null != memberDlg)
			{
				memberDlg.SetOffLine( true, memberDlg.goChild.active);
				memberDlg.PartyBuffClear();

				AS_PARTY_USER member = GetPartyMember( data.nCharUniqKey);
				if( null == member)
				{
					Debug.LogError( "PartyMember not found!!!");
				}
				else
				{
					member.nSessionIdx = 0;
					member.m_BuffDataList = null;
				}
			}
			else
			{
				AS_PARTY_USER member = GetPartyMember( data.nCharUniqKey);
				member.nSessionIdx = 0;
				member.m_BuffDataList = null;
			}
		}
		else
		{
			string strMsg = string.Format( AsTableManager.Instance.GetTbl_String(50), user.strCharName);
			AsChatManager.Instance.InsertChat( strMsg, eCHATTYPE.eCHATTYPE_SYSTEM);
			PartyUserRealDel( data.nCharUniqKey);

			m_TempPartyMembers.Remove( data.nCharUniqKey);
		}

		SetPartyEditSolt();
	}

	private void PartyUserRealDel( uint _id)
	{
		AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
		if( null != userEntity)
		{
			if( userEntity.UniqueId == _id)
			{
				PartyIdx = 0; //#19805.
				IsPartying = false;
				IsCaptain = false;
			}
		}

		m_PartyMembers.Remove( _id);
		m_PartyUI.PartyMemberUI.ReSetPartyMember();
		SetPartyEditSolt();

		if( AsHudDlgMgr.Instance != null)
			if( AsHudDlgMgr.Instance.partyAndQuestToggleMgr != null)
			{
				if (GetPartyMemberList().Count <= 1)
					AsHudDlgMgr.Instance.partyAndQuestToggleMgr.DelPartyProcess();
				else
					AsHudDlgMgr.Instance.partyAndQuestToggleMgr.AddPartyProcess();
			}

		QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_ADD_PARTY, new AchAddParty(m_PartyMembers.Count));
	}

	public void PartyDiceRemoveAll()
	{
		if( m_CurDiceDlg != null)
			m_CurDiceDlg.GiveUp();

		foreach( AsPartyDiceDlg diceDlg in m_DiceQue)
		{
			diceDlg.GiveUp();
		}

		m_CurDiceDlg = null;
	}

	private void DestroyMessageBox()
	{
		if( null != m_MsgBox_PartyExit)
			m_MsgBox_PartyExit.Close();
		
		if( null != m_MsgBox_PartyInvite)		
			m_MsgBox_PartyInvite.Close();
		
		if( null != m_MsgBox_BannedExit)
			m_MsgBox_BannedExit.Close();
		
		if( null != m_MsgBox_PartyOption)
			m_MsgBox_PartyOption.Close();
		
		if( null != m_MsgBox_PartySearch) 
			m_MsgBox_PartySearch.Close();
		
		if( null != m_MsgBox_PartyWarpXZ) //#21473
			m_MsgBox_PartyWarpXZ.Close();		
	}

	public void PartyUserRemoveAll()
	{
		DestroyMessageBox();

		PartyIdx = 0;
		IsPartying = false;
		IsCaptain = false;
		m_bPartyNotice = false;
		m_PartyNotice = string.Empty;
		m_PartyOption = null;
		CreateOption();

		AsMyProperty instance = FindObjectOfType( typeof( AsMyProperty)) as AsMyProperty;
		if( instance)
		AsMyProperty.Instance.SetCaptain( false);

		m_TempPartyMembers.Clear();
		foreach( KeyValuePair<uint, AS_PARTY_USER> member in m_PartyMembers)
		{
			AS_PARTY_USER new_member = new AS_PARTY_USER();

			new_member.nCharUniqKey = member.Value.nCharUniqKey;
			new_member.nSessionIdx = member.Value.nSessionIdx;
			new_member.strCharName = member.Value.strCharName;
			new_member.eRace = member.Value.eRace;
			new_member.eClass = member.Value.eClass;
			new_member.nLevel = member.Value.nLevel;
			new_member.fHpCur = member.Value.fHpCur;
			new_member.fHpMax = member.Value.fHpMax;

			m_TempPartyMembers.Add( new_member.nCharUniqKey, new_member);
		}

		m_PartyMembers.Clear();

		AllClosePartyUI();

		if( AsHudDlgMgr.Instance != null)
		{
			if( AsHudDlgMgr.Instance.partyAndQuestToggleMgr != null)
			{
				if (AsPartyManager.Instance.IsPartying == true)
					if( GetPartyMemberList().Count <= 1)
						AsHudDlgMgr.Instance.partyAndQuestToggleMgr.DelPartyProcess();
			}
		}

		QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_ADD_PARTY, new AchAddParty(m_PartyMembers.Count));
	}

	#endregion - PartyMember
	public void PartyUserBuff( AS_SC_PARTY_USER_BUFF data)
	{
		if( null == data.body)
			return;

		body1_SC_CHAR_BUFF userBuff = new body1_SC_CHAR_BUFF();

	//	userBuff.nCharUniqKey = data.nCharUniqKey.
		userBuff.bEffect = data.bEffect;
		userBuff.nBuffCnt = data.nBuffCnt;
//		userBuff.body = data.body;
		
		userBuff.body = new body2_SC_CHAR_BUFF[data.body.Length];
		for(int i=0; i<data.body.Length; ++i)
		{
			userBuff.body[i] = new body2_SC_CHAR_BUFF(data.body[i]);
		}
		
	//	Debug.Log( "RecivePartyUserBuff" + userBuff.nBuffCnt.ToString() + ":" + data.nCharUniqKey.ToString());
		AsPartyMemberDlg memberDlg	= m_PartyUI.PartyMemberUI.GetPartyMemberDlgByUniqueId( data.nCharUniqKey);
		if( null != memberDlg)
		{
			memberDlg.PartyUserBuff( userBuff);
			AS_PARTY_USER member = GetPartyMember( data.nCharUniqKey);
			member.m_BuffDataList = null;
			member.m_BuffDataList = new PlayerBuffData[memberDlg.m_PartyBuffUI.BuffDataList.Count];
			memberDlg.m_PartyBuffUI.BuffDataList.CopyTo( member.m_BuffDataList);
		}
	}

	public void PartyUserDeBuff( body_SC_CHAR_DEBUFF data)
	{
		AsPartyMemberDlg memberDlg	= m_PartyUI.PartyMemberUI.GetPartyMemberDlgByUniqueId( data.nCharUniqKey);
		if( null != memberDlg)
		{
			memberDlg.PartyUserDeBuff( data);
			AS_PARTY_USER member = GetPartyMember( data.nCharUniqKey);
			member.m_BuffDataList = null;
			member.m_BuffDataList = new PlayerBuffData[memberDlg.m_PartyBuffUI.BuffDataList.Count];
			memberDlg.m_PartyBuffUI.BuffDataList.CopyTo( member.m_BuffDataList);
		}
	}

	public void ReceivePartyDetailInfo( AS_SC_PARTY_DETAIL_INFO _info)
	{
		#if _PARTY_LOG_
		Debug.Log( "ReceivePartyDetailInfo");
		#endif
		if( null == _info)
			return;

		if( null != m_PartyInfoDlg)
		{
			GameObject.Destroy( m_PartyInfoDlg);
			m_PartyInfoDlg = null;
		}

		//파티 정보 처리...
		if( SendDetailPartyIdx == _info.nPartyIdx)
		{
			if( null == m_PartyInfoDlg)
				m_PartyInfoDlg = Instantiate( ResourceLoad.LoadGameObject( "UI/AsGUI/PartyMatching/GUI_PartyInfo")) as GameObject;
			AsPartyInfoDlg dlg = m_PartyInfoDlg.GetComponentInChildren<AsPartyInfoDlg>();
			dlg.SetData( _info, false);
		}
		else if( SendNoticePartyIdx == _info.nPartyIdx)
		{
			if( null == m_PartyInfoDlg)
				m_PartyInfoDlg = Instantiate( ResourceLoad.LoadGameObject( "UI/AsGUI/PartyMatching/GUI_PartyInfo")) as GameObject;
			AsPartyInfoDlg dlg = m_PartyInfoDlg.GetComponentInChildren<AsPartyInfoDlg>();
			dlg.SetData( _info, true);
		}

		if( PartyIdx == _info.nPartyIdx)
		{
			PartyMapIdx = _info.nMapIdx;//#23043
			PartyOption = _info.sOption;
		}
	}

	private AsPartyDiceDlg InstantiatePartyDice()
	{
		GameObject objPartyDice = ResourceLoad.LoadGameObject( m_strPartyDiceUiPath);
		if( objPartyDice == null)
		{
			Debug.Log( "AsPartyManager InstantiatePartyDice() Not found Resource.");
			return null;
		}

		GameObject goPartyDice = GameObject.Instantiate( objPartyDice) as GameObject;
		if( goPartyDice == null)
		{
			Debug.Log( "AsPartyManager InstantiatePartyDice() Not found Instantiate.");
			return null;
		}

		goPartyDice.transform.parent = m_RootObject.transform;
		AsPartyDiceDlg partyDiceDlg = goPartyDice.GetComponentInChildren<AsPartyDiceDlg>();
		partyDiceDlg.Hidden();
		m_PartyDiceDlg.Add( partyDiceDlg);

		return partyDiceDlg;
	}

	public AsPartyDiceDlg GetPartyDiceDlg()
	{
		foreach( AsPartyDiceDlg diceDlg in m_PartyDiceDlg)
		{
			if( !diceDlg.IsUseing)
				return diceDlg;
		}

		return null;
	}

	public sITEM GetItemByDropItemIdx( int nDropItemIdx)
	{
		foreach( AsPartyDiceDlg diceDlg in m_PartyDiceDlg)
		{
			if( diceDlg.DropItemIdx == nDropItemIdx)
				return diceDlg.m_Itemslot.slotItem.realItem.sItem;
		}

		return null;
	}

	public void ClosePartyDiceDlg()
	{
		if( m_DiceQue.Count != 0)
		{
			AsPartyDiceDlg partyDiceDlg = null;

			partyDiceDlg = m_DiceQue.Dequeue() as AsPartyDiceDlg;
			if( partyDiceDlg == null)
			{
				Debug.LogError( "AsPartyManager ClosePartyDiceDlg() Not found Resource AsPartyDiceDlg!!!");
				return;
			}
			partyDiceDlg.Open();
			m_CurDiceDlg = partyDiceDlg;
		}
		else
		{
			m_CurDiceDlg = null;
		}
	}

	public void PartyDiceItemInfo( AS_SC_PARTY_DICE_ITEM_INFO data)
	{
		AsPartyDiceDlg partyDiceDlg = null;
		partyDiceDlg = GetPartyDiceDlg();
		if( partyDiceDlg == null)
			partyDiceDlg = InstantiatePartyDice();

		if( m_CurDiceDlg == null)
		{
			partyDiceDlg.SetData( data);
			partyDiceDlg.Open();
			m_CurDiceDlg = partyDiceDlg;//#21713
		}
		else
		{
			partyDiceDlg.SetData( data);
			m_DiceQue.Enqueue( partyDiceDlg);
		}
	}

	public void PartyDiceShake( AS_SC_PARTY_DICE_SHAKE data)
	{
		AS_PARTY_USER member = GetPartyMember( data.nCharUniqKey);
		string strMsg = "PartyDiceShake()Party member not found!!!";
		if( null == member)
			member = TempPartyMember( data.nCharUniqKey);

		if( data.nDiceNumber == -1)//-1:포기
		{
			//님이 포기했습니다.//
			strMsg = string.Format( AsTableManager.Instance.GetTbl_String(61), member.strCharName, data.nDiceNumber.ToString());
		}
		else
		{
			//"님이 주사위를 굴려 %d가 나왔습니다.";
			strMsg = string.Format( AsTableManager.Instance.GetTbl_String(59), member.strCharName, data.nDiceNumber.ToString());
		}

		AsChatManager.Instance.InsertChat( strMsg,eCHATTYPE.eCHATTYPE_SYSTEM);
		AsChatManager.Instance.ShowChatBalloon( data.nCharUniqKey , strMsg, eCHATTYPE.eCHATTYPE_SYSTEM);
	}

	public void PartyDiceShakeResult( AS_SC_PARTY_DICE_SHAKE_RESULT data)
	{
		sITEM sItem = GetItemByDropItemIdx( data.nDropItemIdx);
		if( null == sItem)
		{
			#if _PARTY_ERROR_LOG_
		//	Debug.LogError( "PartyDiceShakeResult()[ null == DropItemIdx ] id : " + data.nDropItemIdx);
			#endif
			return;
		}

		Item _item = ItemMgr.ItemManagement.GetItem( sItem.nItemTableIdx);
		if( null == _item)
		{
			#if _PARTY_ERROR_LOG_
			//Debug.LogError( "PartyDiceShakeResult()[ null == _item ] id : " + data.nDropItemIdx);
			#endif
			return;
		}

		Tbl_String_Record record = AsTableManager.Instance.GetTbl_String_Record( _item.ItemData.nameId);
		if( null == record)
		{
			#if _PARTY_ERROR_LOG_
		//	Debug.LogError( "PartyDiceShakeResult()[null == record] string id :" + _item.ItemData.nameId);
			#endif
			return;
		}
		string strMsg = "PartyDiceShakeResult() Party member not found!!!";
		AS_PARTY_USER member = GetPartyMember( data.nCharUniqKey);
		if( null == member)
		{
			 member = TempPartyMember( data.nCharUniqKey);
		}
		// 님이 를 획득했습니다.
		strMsg = string.Format( AsTableManager.Instance.GetTbl_String(60),member.strCharName ,record.String);
		AsChatManager.Instance.InsertChat( strMsg,eCHATTYPE.eCHATTYPE_SYSTEM);
	}

	// < input
	public void InputDown( Ray inputRay)
	{
		//TooltipMgr.Instance.Clear();// kij
	}

	public void GuiInputDown( Ray inputRay)
	{
		if( m_CurDiceDlg == null)
		{
			return;
		}
		else
		{
			TooltipMgr.Instance.Clear();
			m_ClickDownItemSlot = null;
			if( null != m_CurDiceDlg.m_Itemslot.slotItem && true == m_CurDiceDlg.m_Itemslot.IsIntersect( inputRay))
				m_ClickDownItemSlot = m_CurDiceDlg.m_Itemslot;
		}
	}

	public void GuiInputUp( Ray inputRay)
	{
		if( null != m_ClickDownItemSlot)
		{
			if( m_ClickDownItemSlot.IsIntersect( inputRay))
			{
				TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.normal, m_ClickDownItemSlot.slotItem.realItem);
			}
		}
	}

	public void LevelSortList()
	{
		AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
		if( null == userEntity)
			return;
		
		int nlevel =  userEntity.GetProperty<int>( eComponentProperty.LEVEL);
		foreach( AsSortPartyList p in m_SortPartyList)
		{
			int firstSortKey = p.m_sPARTYLIST.nLevel - nlevel;
			if(firstSortKey > 0)
				p.m_LevelSortKey = AsGameDefine.MAKELONG( 1,Mathf.Abs(firstSortKey));
			else
				p.m_LevelSortKey = AsGameDefine.MAKELONG( 2,Mathf.Abs(firstSortKey));
			
			
		}
		m_SortPartyList.Sort( delegate( AsSortPartyList p1, AsSortPartyList p2) { return (p1.m_LevelSortKey.CompareTo( p2.m_LevelSortKey)); });
		//m_SortPartyList.Sort( delegate( AsSortPartyList p1, AsSortPartyList p2) { return -( p1.m_sPARTYLIST.nLevel.CompareTo( p2.m_sPARTYLIST.nLevel)); });
	}

	public void MaxUserSortList()
	{
		foreach( AsSortPartyList p in m_SortPartyList)
		{
			int firstSortKey = AsGameDefine.MAX_PARTY_USER - ( p.m_sPARTYLIST.sOption.nMaxUser - p.m_sPARTYLIST.nUserCnt);
			p.m_MaxUserSortKey = AsGameDefine.MAKELONG( p.m_sPARTYLIST.sOption.nMaxUser,firstSortKey);
		}

		m_SortPartyList.Sort( delegate( AsSortPartyList p1, AsSortPartyList p2) { return -( p1.m_MaxUserSortKey.CompareTo( p2.m_MaxUserSortKey)); });
	}

	public void ClosePartyInfoDlg()
	{
		if( null == m_PartyInfoDlg)
			return;

		Destroy( m_PartyInfoDlg);
	}

	public void AllClosePartyUI()
	{
		if( m_PartyUI.PartyMemberUI)
		{
			m_PartyUI.PartyMemberUI.ClearPartyBuff();
			m_PartyUI.PartyMemberUI.ClosePartyMember();
		}

		if( null != m_PartyUI.PartyEditDlg)
			m_PartyUI.PartyEditDlg.Close();

		if( null != m_PartyUI.PartyListDlg)
			m_PartyUI.PartyListDlg.Close();

		if( null != m_PartyUI.PartyEditDlg)
		{
			if( null != m_PartyUI.PartyCreateDlg)
				m_PartyUI.PartyCreateDlg.Close();
		}

		ClosePartyInfoDlg();
	}

	public void AutoDelPartyNotice()
	{
		if( IsCaptain && IsPartyNotice)
		{
			if( m_PartyMembers.Count >= m_PartyOption.nMaxUser)
				AsPartySender.SendPartyNoticeOnOff( false, "");
		}
	}

	public void SetMyPortraitImage()
	{
		AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
		if( null == userEntity)
			return;

		AS_PARTY_USER member = GetPartyMember( userEntity.UniqueId);
		if( null != member)
			member.nImageTableIdx = AsDelegateImageManager.Instance.AssignedImageID;;

		SetPartyEditSolt();
	}
	
	public void RecivePartyPositionInfo( body1_SC_PARTY_USER_POSITION _info )
	{	
		m_UserPosition = _info;
		if(IsPartyWarpXZMsgBox)
		{
			m_bPartyWarpXZMsgBox = false;
			PartyWarpXZMsgBox();			
		}
	}
	
	#region - target mark -
	public void TargetMarkBtnClicked()
	{
		AsBaseEntity target = AsHUDController.Instance.targetInfo.getCurTargetEntity;
		if( target == null)
			return;
		
		ushort charTargetIdx = 0;
		int npcTargetIdx = 0;
		
		if( target.FsmType == eFsmType.MONSTER)
		{
			AsNpcEntity monster = target as AsNpcEntity;
			npcTargetIdx = monster.SessionId;
		}
		else if( target.FsmType == eFsmType.OTHER_USER)// &&
//			TargetDecider.CheckOtherUserIsEnemy(target) == true)
		{
			AsUserEntity otherUser = target as AsUserEntity;
			charTargetIdx = otherUser.SessionId;
		}
		
		body_CS_TARGET_MARK targetMark = new body_CS_TARGET_MARK( charTargetIdx, npcTargetIdx);
		byte[] bytes = targetMark.ClassToPacketBytes();
		AsCommonSender.Send(bytes);
	}
	
	// target mark
	// 예외상황
	// 파티가 없거나 파티장이 아닌데 CS_TAGET_MARK가 오면 접속종료
	// 일반 필드에서 nCharTargetIdx값이 있으면 접속종료
	// pvp존에서 nNpcTargetIdx가 있으면 접속종료.
	int m_NpcTargetIdx = 0; public int NpcTargetIdx{get{return m_NpcTargetIdx;}}
	ushort m_CharTargetIdx = 0; public ushort CharTargetIdx{get{return m_CharTargetIdx;}}
	public void Recv_TargetMark( body_SC_TARGET_MARK _mark)
	{
		// monster
		AsNpcEntity monster = AsEntityManager.Instance.GetNpcEntityBySessionId( m_NpcTargetIdx);
		if( monster != null)
			monster.namePanel.SetTargetMark( false);

		monster = AsEntityManager.Instance.GetNpcEntityBySessionId( _mark.nNpcTargetIdx);
		if( monster != null)
			monster.namePanel.SetTargetMark( true);

		m_NpcTargetIdx = _mark.nNpcTargetIdx;
		
		// user
		List<AsUserEntity> user = AsEntityManager.Instance.GetUserEntityBySessionId( m_CharTargetIdx);
		if( user != null && user[0] != null)
			user[0].namePanel.SetTargetMark( false);
		
		user = AsEntityManager.Instance.GetUserEntityBySessionId( _mark.nCharTargetIdx);
		if( user != null && user[0] != null)
			user[0].namePanel.SetTargetMark( true);

		m_CharTargetIdx = _mark.nCharTargetIdx;
	}
	#endregion
	
	#region - party member name change -
	public void PartyMemberNameChange( uint nCharUniqKey , string changeName )
	{
		AsPartyMemberDlg memberDlg	= m_PartyUI.PartyMemberUI.GetPartyMemberDlgByUniqueId( nCharUniqKey );
		if( null != memberDlg)
		{
			memberDlg.SetName( changeName );
			
			AS_PARTY_USER member = GetPartyMember( nCharUniqKey );
			if( null != member)
			{
				member.strCharName = changeName;
			}
		}		
	}
	#endregion
	
}








