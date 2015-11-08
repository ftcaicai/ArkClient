//#define USE_OLD_COSTUME
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsUserInfo
{
	static private AsUserInfo ms_UserInfo = null;
	private UInt32 nLoginUserUniqKey;	// �������� �־����� ����ũ�� Ű ��
	private UInt32 nLoginUserSessionKey;	// �α��μ����� �α��ν� ������ ����Ű
	private UInt16 nGamerUserSessionIdx;	// ���Ӽ��� ���� ���� �ε���
	#region -WEME Certify
	public bool isWemeCertified = false;

	private string strAccessToken;
	public string WemeAuthToken
	{
		set	{ strAccessToken = value; }
		get	{ return strAccessToken; }
	}
	/*private Int32 nAuthToken;
	public Int32 WemeAuthToken
	{
		set	{ nAuthToken = value; }
		get	{ return nAuthToken; }
	}*/
//	public bool isGuest = false;
	#endregion
	static public string curID = null;
	static public string curPass = null;
	private ArrayList characterInfos = new ArrayList();
	//$yde
	private sCHARVIEWDATA curUserCharacterinfo;//character selection
	private AS_GC_ENTER_WORLD_RESULT curUserEnterWorldinfo;//entered world information
#if USE_INSTANCE_SERVER
	private AS_GC_INSTANCE_CREATE_RESULT curUserInstanceInfo; //In Dungeon infomation
#endif
	private UnityEngine.Vector3 charPosition = UnityEngine.Vector3.zero;
	CharacterLoadData savedCharStat = new CharacterLoadData();
	public CharacterLoadData SavedCharStat	{ get	{ return savedCharStat; } }

#if USE_OLD_COSTUME
	private bool m_bCostumeOnOff;
#else
	private int m_bCostumeOnOff;
#endif
	private sITEMVIEW[] m_ItemViews;
	private sITEMVIEW[] m_sCosItemView;

	public int latestCharSlot = 1;
	public int currentChannel = -1;
	public string currentChannelName = string.Empty;
	public bool isFirstConnect = true;
	#region -AccountGender
	public eGENDER accountGender = eGENDER.eGENDER_NOTHING;
	#endregion
	#region -GMMark
	public bool isGM = false;
	#endregion

	#region -AccountDeleteCancel
	public bool isAccountDeleteCancel = false;
	#endregion

	public Int64 nMiracle;
	private int curConditionValue = 0;
	//private int nPvpPoint = 0;
	private UInt32 nYesterdayPvpRank = 0;
	private Int32 nYesterdayPvpPoint = 0;
	private UInt32 nYesterdayPvpRankRate = 0;
	private int nRankPoint = 0;
	private int curServerId = 0;
	private bool bSubTitleHide = false;
	public bool SubTitleHide { get{ return bSubTitleHide;} set{ bSubTitleHide = value;}}
	public int CurrentServerID	{ get { return curServerId; } set { curServerId = value; } }
	//public int PvpPoint { get{ return nPvpPoint;} set{ nPvpPoint = value;}}
	public UInt32 YesterdayPvpRank { get{ return nYesterdayPvpRank;} set{ nYesterdayPvpRank = value;}}
	public Int32 YesterdayPvpPoint { get{ return nYesterdayPvpPoint;} set{ nYesterdayPvpPoint = value;}}
	public UInt32 YesterdayPvpRankRate { get{ return nYesterdayPvpRankRate;} set{ nYesterdayPvpRankRate = value;}}
	public int RankPoint { get{ return nRankPoint;} set{ nRankPoint = value;}}
	public byte FreeGachaPoint { get; set; }

	public int CurConditionValue
	{
		get	{ return curConditionValue; }
		set
		{
			if( 0 == value)
			{
				if( 0 < curConditionValue)
				{
					#region -GameGuide_Condition
					AsGameGuideManager.Instance.CheckUp( eGameGuideType.Condition, -1);
					#endregion

					if( null != AsChatManager.Instance)
					{
						AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(863), eCHATTYPE.eCHATTYPE_SYSTEM);
						AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(2103), eCHATTYPE.eCHATTYPE_PUBLIC, true);
						AsChatManager.Instance.ShowChatBalloon( savedCharStat.uniqKey_, AsTableManager.Instance.GetTbl_String(2103), eCHATTYPE.eCHATTYPE_PUBLIC);
					}

					AsEventNotifyMgr.Instance.CenterNotify.AddTradeMessage( AsTableManager.Instance.GetTbl_String(863));
					
					//$yde
					AsPromotionManager.Instance.ConditionExhausted();
				}
			}
			else
			{
				if( 0 == curConditionValue)
				{
					if( null != AsChatManager.Instance)
						AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(862), eCHATTYPE.eCHATTYPE_SYSTEM);
				}
			}

			curConditionValue = value;

			if( 0 > curConditionValue)
			{
				curConditionValue = 0;

				if( null != AsChatManager.Instance)
					AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(863), eCHATTYPE.eCHATTYPE_SYSTEM);
			}
		}
	}

	//$yde
	#region - private shop -
	UInt32 m_nPrivateShopOpenCharUniqKey; public UInt32 nPrivateShopOpenCharUniqKey
	{
		get
		{
			Debug.Log( "AsUserInfo:: nPrivateShopOpenCharUniqKey {" + m_nPrivateShopOpenCharUniqKey + "}");
			return m_nPrivateShopOpenCharUniqKey;
		}
	}
//	Int64 m_nPrivateShopRemainingTime;// public Int64 nPrivateShopRemainingTime
//	{
//		get
//		{
//			Debug.Log( "AsUserInfo:: nPrivateShopRemainingTime {" + m_nPrivateShopRemainingTime + "}");
//			return m_nPrivateShopRemainingTime;
//		}
//	}

//	Int64 m_nPrivateShopMaxOpenTime; public Int64 nPrivateShopMaxOpenTime
//	{
//		get
//		{
//			Debug.Log( "AsUserInfo:: nPrivateShopMaxOpenTime {" + m_nPrivateShopMaxOpenTime + "}");
//			return m_nPrivateShopMaxOpenTime;
//		}
//	}
//	Int64 m_nPrivateShopOpenTime; public Int64 nPrivateShopOpenTime
//	{
//		get
//		{
//			Debug.Log( "AsUserInfo:: nPrivateShopOpenTime {" + m_nPrivateShopOpenTime + "}");
//			return m_nPrivateShopOpenTime;
//		}
//	}
	Int32 m_nPrivateShopCreateItemSlot; public Int32 nPrivateShopCreateItemSlot
	{
		get
		{
			Debug.Log( "AsUserInfo:: nPrivateShopCreateItemSlot {" + m_nPrivateShopCreateItemSlot + "}");
			return m_nPrivateShopCreateItemSlot;
		}
	}

	public bool PlayerPrivateShopOpened
	{
		get	{ return savedCharStat.shopOpening_; }
	}
	
	#endregion
	Dictionary<int, Dictionary<int, Msg_CharBuff_Body>> m_dicBuff = new Dictionary<int, Dictionary<int, Msg_CharBuff_Body>>();
	public Dictionary<int, Dictionary<int, Msg_CharBuff_Body>> dicBuff{get{return m_dicBuff;}}

	// begin kij
	List<body2_SC_WAYPOINT> m_WaypointList = new List<body2_SC_WAYPOINT>();
	public bool isReceiveWaypointList = false;
	bool m_isProductionProgress = false;
	body1_SC_ITEM_PRODUCT_INFO m_itemProductionInfo;
	//end kij
	
	
	#region -SkillReset
	public Dictionary<int,body2_SC_SKILLBOOK_LIST> resettedSkills = new Dictionary<int, body2_SC_SKILLBOOK_LIST>();
	#endregion

	public void SendLevelUpActiveWaypoint( int iCurLevel)
	{
		bool sw = false;

		if( null != AsTableManager.Instance)
		{
			foreach( KeyValuePair<int, Tbl_WarpData_Record> pair in AsTableManager.Instance.GetWarpDataTable().getWaypointList)
			{
				if( pair.Value.getActiveLevel == iCurLevel)
				{
					if( pair.Value.isActive)
					{
						// print message
						AsChatManager.Instance.InsertChat( string.Format( AsTableManager.Instance.GetTbl_String(852), pair.Value.GetName()) , eCHATTYPE.eCHATTYPE_SYSTEM);
						AsEventNotifyMgr.Instance.LevelUpNotify.SetWayPoint( pair.Value.GetName());
					}

					if( !sw)
					{
						AsCommonSender.SendLevelActiveWaypoint();
						sw = true;
					}
				}
			}
		}
	}

	public void SetProductionInfo( body1_SC_ITEM_PRODUCT_INFO _data)
	{
		m_itemProductionInfo = _data;
	}

	public bool IsHaveProductionInfoComplete()
	{
		if( null == m_itemProductionInfo || null == m_itemProductionInfo.body)
			return false;

		if( 0 >= m_itemProductionInfo.body.Length)
			return false;

		for( int i = 0; i < m_itemProductionInfo.body.Length; ++i)
		{
			body2_SC_ITEM_PRODUCT_INFO _dataslotInfo = m_itemProductionInfo.body[ i];
			if( 0 == _dataslotInfo.sSlotInfo.nProductTime && 0 != _dataslotInfo.sSlotInfo.nRecipeIndex)
				return true;
		}

		return false;
	}

	public int GetTechInfoDiffValue( body_SC_ITEM_PRODUCT_TECHNIQUE_INFO _data)
	{
		if( null == m_itemProductionInfo)
			return 0;


		if( m_itemProductionInfo.sProductInfo.Length <=_data.eProductType)
		{
			AsUtil.ShutDown( "m_itemProductionInfo.sProductInfo.Length >= iIndex [ index : " + _data.eProductType);
			return 0;
		}

		return _data.sProductInfo.nTotExp - m_itemProductionInfo.sProductInfo[ _data.eProductType ].nTotExp;
	}

	public int GetProductTechniqueLv( eITEM_PRODUCT_TECHNIQUE_TYPE _type)
	{
		int type = (int)_type;

		if( m_itemProductionInfo.sProductInfo.Length <= type)
		{
			Debug.LogError( "m_itemProductionInfo.sProductInfo.Length >= iIndex [ index : " + _type.ToString());
			return 0;
		}
	
		return m_itemProductionInfo.sProductInfo[(int)_type].nLevel;
	}

	public int GetProductTechniqueHaveCount()
	{
		if( null == m_itemProductionInfo)
			return 0;

		int iCount = 0;

		for( int i = 0; i < m_itemProductionInfo.sProductInfo.Length; ++i)
		{
			if( 0 < m_itemProductionInfo.sProductInfo[i].nLevel)
			{
				++iCount;
			}
		}

		return iCount;
	}

	public int HaveProductTechLvMax()
	{
		int lv = 0;
		foreach( sPRODUCT_INFO info in m_itemProductionInfo.sProductInfo)
		{
			if( info.nLevel >= lv)
			lv = info.nLevel;
		}

		return lv;
	}


	public void SetProductTechnique( body_SC_ITEM_PRODUCT_TECHNIQUE_INFO _data)
	{
		if( null == m_itemProductionInfo)
			return;

		if( m_itemProductionInfo.sProductInfo.Length <=_data.eProductType)
		{
			AsUtil.ShutDown( "m_itemProductionInfo.sProductInfo.Length >= iIndex [ index : " + _data.eProductType);
			return;
		}

		bool isLevelUp = false;
		if( m_itemProductionInfo.sProductInfo[_data.eProductType].nLevel < _data.sProductInfo.nLevel)
			isLevelUp = true;

		m_itemProductionInfo.sProductInfo[ _data.eProductType ] = _data.sProductInfo;

		if( isLevelUp)
		{
			QuestMessageBroadCaster.BrocastQuest( QuestMessages.QM_PRODUCE_MASTERY, new AchProductionMastery( (eITEM_PRODUCT_TECHNIQUE_TYPE)_data.eProductType, _data.sProductInfo.nLevel));
			ArkQuestmanager.instance.CheckQuestMarkAllNpcAndCollecion();

			if( null != AsUserInfo.Instance.GetCurrentUserEntity() && null != AsUserInfo.Instance.GetCurrentUserEntity().ModelObject)
			{
				AsEffectManager.Instance.PlayEffect( "Fx/Effect/COMMON/Fx_Common_Technic_LevelUp", AsUserInfo.Instance.GetCurrentUserEntity().ModelObject.transform, false, 0);
				AsSoundManager.Instance.PlaySound( "Sound/PC/Common/Se_Common_Technic_LevelUp", Vector3.zero, false);
			}


			AsChatManager.Instance.InsertChat( string.Format( AsTableManager.Instance.GetTbl_String(406),
				ProductionTechnologyItem.GetName( (eITEM_PRODUCT_TECHNIQUE_TYPE) _data.eProductType), _data.sProductInfo.nLevel),
				eCHATTYPE.eCHATTYPE_SYSTEM);
		}
	}

	public void SetProductSlotInfo( body_SC_ITEM_PRODUCT_SLOT_INFO _data)
	{
		if( null == m_itemProductionInfo)
			return;

		if( null == m_itemProductionInfo.body)
			return;

		for( int i = 0; i < m_itemProductionInfo.body.Length; ++i)
		{
			body2_SC_ITEM_PRODUCT_INFO _dataslotInfo = m_itemProductionInfo.body[ i];
			if( _dataslotInfo.nProductSlot == _data.nProductSlot)
			{
				m_itemProductionInfo.body[i].sSlotInfo = _data.sSlotInfo;
				break;
			}
		}

		if( 0 != _data.sSlotInfo.nRecipeIndex && 0 == _data.sSlotInfo.nProductTime && -1 != ItemMgr.HadItemManagement.Inven.GetEmptyInvenSlot())
		{
			AsCommonSender.SendItemProductReceive( _data.nProductSlot);
		}

		if( AsHudDlgMgr.Instance.IsOpenProductionDlg)
		{
			AsHudDlgMgr.Instance.productionDlg.CheckNewImg();
			AsHudDlgMgr.Instance.CheckNewMenuImg();
		}
	}

	public bool IsEnableProductTechniqueType( eITEM_PRODUCT_TECHNIQUE_TYPE _type, int ilevel, bool isShowMsg = false)
	{
		if( null == m_itemProductionInfo)
			return false;

		if( eITEM_PRODUCT_TECHNIQUE_TYPE.QUEST == _type)
			return true;

		int iIndex = (int)_type;
		if( m_itemProductionInfo.sProductInfo.Length <= iIndex)
		{
			AsUtil.ShutDown( "m_itemProductionInfo.sProductInfo.Length >= iIndex [ index : " + iIndex);
			return false;
		}

		sPRODUCT_INFO _data = m_itemProductionInfo.sProductInfo[ iIndex ];

		if( _data.nLevel == 0)
		{
//			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(281), eCHATTYPE.eCHATTYPE_SYSTEM);
			AsEventNotifyMgr.Instance.CenterNotify.AddTradeMessage( AsTableManager.Instance.GetTbl_String(281));
			return false;
		}

		if( ilevel > _data.nLevel)
		{
			string strTemp = string.Format( AsTableManager.Instance.GetTbl_String(282), ilevel);
//			AsChatManager.Instance.InsertChat( strTemp, eCHATTYPE.eCHATTYPE_SYSTEM);
			AsEventNotifyMgr.Instance.CenterNotify.AddTradeMessage( strTemp);
			return false;
		}

		return true;
	}

	public void SetProductionProgress( bool isProgress)
	{
		m_isProductionProgress = isProgress;
		if( null != m_itemProductionInfo)
			m_itemProductionInfo.bProgress= isProgress;

		if( true == m_isProductionProgress)
		{
			AsUserInfo.Instance.GetCurrentUserEntity().HandleMessage( new Msg_PRODUCT( m_isProductionProgress));
		}
	}

	public bool isProductionProgress
	{
		get	{ return m_isProductionProgress; }
	}


	public List<body2_SC_WAYPOINT> getWayPointList
	{
		get	{ return m_WaypointList; }
	}

	public void ReciveWaypointList( body2_SC_WAYPOINT[] waypointList)
	{
		isReceiveWaypointList = true;
		m_WaypointList.Clear();

		if( null == waypointList)
			return;

		foreach( body2_SC_WAYPOINT _data in waypointList)
		{
			m_WaypointList.Add( _data);
		}
	}


	public void InsertWapointList( int iWarpIdx)
	{
		foreach( body2_SC_WAYPOINT _body in m_WaypointList)
		{
			if( _body.nWarpTableIdx == iWarpIdx)
				return;
		}

		body2_SC_WAYPOINT _data = new body2_SC_WAYPOINT();
		_data.nWarpTableIdx = iWarpIdx;
		m_WaypointList.Add( _data);

		//AsHudDlgMgr.Instance.OpenWaypointMapDlg();
	}

	public bool IsExistWaypoint( int iWaypoint)
	{
		foreach( body2_SC_WAYPOINT _data in m_WaypointList)
		{
			if( iWaypoint == _data.nWarpTableIdx)
				return true;
		}

		return false;
	}

	public void ClickWaypointNpc( int iWarpIdx)
	{
		if( false == IsExistWaypoint( iWarpIdx))
			AsCommonSender.SendWayPointActive( iWarpIdx);
	}



	// end kij waypoint

	#region -PostBox
	private bool isExistNewMail = false;
	public bool NewMail
	{
		set	{ isExistNewMail = value; }
		get	{ return isExistNewMail; }
	}
	#endregion

	#region -Guild
	private body_SC_GUILD_LOAD_RESULT guildData = null;
	public body_SC_GUILD_LOAD_RESULT GuildData
	{
		get	{ return guildData; }
		set	{ guildData = value; }
	}
	#endregion

	//$yde cool time
	bool[] m_CommandSkillCoolTimeCompletion = new bool[(int)COMMAND_SKILL_TYPE.MAX_SKILL_TYPE/*9*/]{ true, true, true, true, true, true, true, true, true };
	public bool[] CommandSkillCoolTimeCompletion
	{
		get	{ return m_CommandSkillCoolTimeCompletion; }
	}

	public void ResetCommandSkillCoolTimeCompletion()
	{
		for( int i = 0; i < m_CommandSkillCoolTimeCompletion.Length; ++i)
		{
			m_CommandSkillCoolTimeCompletion[i] = true;
		}
	}

	

	public void SetItemViews( sITEMVIEW[] view)
	{
		m_ItemViews = view;
		savedCharStat.sCharView = view;
	}

	public void SetItemViews( int[] view)
	{
		if( null == m_ItemViews)
			m_ItemViews = new sITEMVIEW[ AsGameDefine.ITEM_SLOT_VIEW_COUNT];

		for( int i = 0; i < view.Length; ++i)
		{
			m_ItemViews[i].nItemTableIdx = view[i];
			savedCharStat.sCharView[i].nItemTableIdx = view[i];
		}
	}

	public void SetCosItemView( sITEMVIEW[] view)
	{
		m_sCosItemView = view;
		savedCharStat.sCosItemView = view;
	}
	
#if USE_OLD_COSTUME
	public bool isCostumeOnOff
	{
		get
		{
			return m_bCostumeOnOff;
		}
	}
	public void SetCostumeOnOff( bool isOnOff)
	{
		savedCharStat.bCostumeOnOff = isOnOff;
		m_bCostumeOnOff = isOnOff;
	}
	
#else
	
	public void SetCostumeOnOff( int isOnOff)
	{
		savedCharStat.bCostumeOnOff = isOnOff;
		m_bCostumeOnOff = isOnOff;
	}
	
	public int isCostumeOnOff
	{
		get
		{
			return m_bCostumeOnOff;
		}
	}
#endif
	

	public sITEMVIEW[] getItemViews
	{
		get	{ return m_ItemViews; }
	}

	public sITEMVIEW[] getCosItemView
	{
		get	{ return m_sCosItemView; }
	}


	private AsUserInfo()	{}

	static public AsUserInfo Instance
	{
		get
		{
			if( null == ms_UserInfo)
				ms_UserInfo = new AsUserInfo();

			return ms_UserInfo;
		}
	}

	public UInt32 LoginUserUniqueKey
	{
		get	{ return nLoginUserUniqKey; }
		set	{ nLoginUserUniqKey = value; }
	}

	public UInt32 LoginUserSessionKey
	{
		get	{ return nLoginUserSessionKey; }
		set { nLoginUserSessionKey = value; }
	}

	public UInt16 GamerUserSessionIdx
	{
		get	{ return nGamerUserSessionIdx; }
		set	{ nGamerUserSessionIdx = value; }
	}

	public Int32 CharacterCount
	{
		get	{ return characterInfos.Count; }
	}

	public Int32 CreatedCharacterCount
	{
		get
		{
			int count = 0;

			foreach( AsPacketHeader charData in characterInfos)
			{
				if( charData.GetType() == typeof( sCHARVIEWDATA))
					count++;
			}

			return count;
		}
	}

	public void AddCharacter( sCHARVIEWDATA info)
	{
		if( false == characterInfos.Contains( info))
		{
			characterInfos.Add( info);
		}
	}

	public void ReplaceSlotData( sCHARVIEWDATA data)
	{
		latestCharSlot = data.nCharSlot;
		characterInfos[ data.nCharSlot] = data;
	}

	public void AddEmptySlot()
	{
		AS_GC_CHAR_LOAD_RESULT_EMPTY empty = new AS_GC_CHAR_LOAD_RESULT_EMPTY();
		characterInfos.Add( empty);
	}

	public void Clear()
	{
		#region -SkillReset
		resettedSkills.Clear();
		#endregion
		m_WaypointList.Clear();
		characterInfos.Clear();
		latestCharSlot = 1;
		currentChannel = -1;
		guildData = null;
		AsEventManager.Instance.Clear();
		
		NewMail = false;
	}

	public void Init()
	{
		guildData = null;
		currentChannel = -1;
	}

	public AsPacketHeader GetCharacter( int index)
	{
		if( index > characterInfos.Count - 1)
			return null;

		return characterInfos[ index] as AsPacketHeader;
	}

	//$yde
	public void SetCurrentUserCharacterInfo( int _index)
	{
		SetCurrentUserCharacterInfo( GetCharacter( _index) as sCHARVIEWDATA);

	}

	public void SetCurrentUserCharacterInfo( sCHARVIEWDATA _info)
	{
		curUserCharacterinfo = _info;
	}

	public sCHARVIEWDATA GetCurrentUserCharacterInfo()
	{
		return curUserCharacterinfo;
	}

	public string GetCurrentUserClassName()
	{
		eCLASS usrClass = curUserCharacterinfo.eClass;

		string returnName = "None";

//		if( usrClass == null)
		if( eCLASS.NONE == usrClass)
			return returnName;
	
		returnName = AsTableManager.Instance.GetTbl_String(305 + (int)usrClass);
	
		return returnName;
	}

	public void SaveCurCharStat( body_GC_CHAR_SELECT_RESULT_1 _result)
	{
		savedCharStat = new CharacterLoadData( nGamerUserSessionIdx, _result.body);

		AsDeathDlg.SetResurrectCnt(_result.nResurrectionCnt);
	}

	public void SaveCurCharStat( body_SC_ATTR_LEVELUP _result)
	{
		savedCharStat.hpCur_ = _result.fHpCur;
		savedCharStat.mpCur_ = _result.fMpCur;

		savedCharStat.sDefaultStatus = _result.sDefaultStatus;
		savedCharStat.sFinalStatus = _result.sFinalStatus;
	}

	public void SaveCurCharStatChange( body_SC_ATTR_CHANGE _result)
	{
		if( AsUserInfo.Instance.SavedCharStat.uniqKey_ == _result.nCharUniqKey)
		{
			switch( _result.eChangeType)
			{
			case eCHANGE_INFO_TYPE.eCHANGE_INFO_HP_MAX:
				savedCharStat.sFinalStatus.fHPMax = _result.nChangeValue;
				break;
			case eCHANGE_INFO_TYPE.eCHANGE_INFO_MP_MAX:
				savedCharStat.sFinalStatus.fMPMax = _result.nChangeValue;
				break;

			case eCHANGE_INFO_TYPE.eCHANGE_INFO_PHYSIC_DMGMIN:
				savedCharStat.sFinalStatus.nPhysicDmg_Min = (int)_result.nChangeValue;
				break;
			case eCHANGE_INFO_TYPE.eCHANGE_INFO_PHYSIC_DMGMAX:
				savedCharStat.sFinalStatus.nPhysicDmg_Max = (int)_result.nChangeValue;
				break;

			case eCHANGE_INFO_TYPE.eCHANGE_INFO_MAGIC_DMGMIN:
				savedCharStat.sFinalStatus.nMagicDmg_Min = (int)_result.nChangeValue;
				break;
			case eCHANGE_INFO_TYPE.eCHANGE_INFO_MAGIC_DMGMAX:
				savedCharStat.sFinalStatus.nMagicDmg_Max = (int)_result.nChangeValue;
				break;

			case eCHANGE_INFO_TYPE.eCHANGE_INFO_PHYSIC_DEF:
				savedCharStat.sFinalStatus.nPhysic_Def = (int)_result.nChangeValue;
				break;
			case eCHANGE_INFO_TYPE.eCHANGE_INFO_MAGIC_DEF:
				savedCharStat.sFinalStatus.nMagic_Def = (int)_result.nChangeValue;
				break;

			case eCHANGE_INFO_TYPE.eCHANGE_INFO_CRITICAL_PROB:
				savedCharStat.sFinalStatus.nCriticalProb = (int)_result.nChangeValue;
				break;
			case eCHANGE_INFO_TYPE.eCHANGE_INFO_ACCURACY_PROB:
				savedCharStat.sFinalStatus.nAccuracyProb = (int)_result.nChangeValue;
				break;
			case eCHANGE_INFO_TYPE.eCHANGE_IFNO_DODGE_PROB:
				savedCharStat.sFinalStatus.nDodgeProb = (int)_result.nChangeValue;
				break;

			case eCHANGE_INFO_TYPE.eCHANGE_INFO_MOVE_SPEED:
				savedCharStat.sFinalStatus.nMoveSpeed = (int)_result.nChangeValue;
				break;
			case eCHANGE_INFO_TYPE.eCHANGE_INFO_ATTACK_SPEED:
				savedCharStat.sFinalStatus.nAtkSpeed = (int)_result.nChangeValue;
				break;

			case eCHANGE_INFO_TYPE.eCHANGE_INFO_PHYSIC_DMG_DEC:
				savedCharStat.sFinalStatus.fPhysic_Dmg_Dec = _result.nChangeValue;
				break;
			case eCHANGE_INFO_TYPE.eCHANGE_INFO_MAGIC_DMG_DEC:
				savedCharStat.sFinalStatus.fMagic_Dmg_Dec = _result.nChangeValue;
				break;
			case eCHANGE_INFO_TYPE.eCHANGE_INFO_HP_REGEN:
				savedCharStat.sFinalStatus.nHPRegen = _result.nChangeValue;
				break;
			case eCHANGE_INFO_TYPE.eCHANGE_INFO_MP_REGEN:
				savedCharStat.sFinalStatus.nMPRegen = _result.nChangeValue;
				break;
			}

			if( null != AsHudDlgMgr.Instance)
			{
				if( true == AsHudDlgMgr.Instance.IsOpenPlayerStatus && false == AsHudDlgMgr.Instance.playerStatusDlg.isOtherUser)
					AsHudDlgMgr.Instance.playerStatusDlg.ResetPageText();
			}
		}


		AsUserEntity userEntity = null;
		switch( _result.eChangeType)
		{
		case eCHANGE_INFO_TYPE.eCHANGE_INFO_MOVE_SPEED:
			userEntity = AsEntityManager.Instance.GetUserEntityByUniqueId( _result.nCharUniqKey);
			if( null != userEntity)
				userEntity.HandleMessage( new Msg_MoveSpeedRefresh( _result.nChangeValue / 100));
			break;
		case eCHANGE_INFO_TYPE.eCHANGE_INFO_ATTACK_SPEED:
			userEntity = AsEntityManager.Instance.GetUserEntityByUniqueId( _result.nCharUniqKey);
			if( null != userEntity)
				userEntity.HandleMessage( new Msg_AttackSpeedRefresh( _result.nChangeValue * AsProperty.s_BaseAttackSpeedRatio));
			break;
		}
	}

	public void SaveCurCharStatTotal( body_SC_ATTR_TOTAL _result)
	{
		AsUserEntity player = AsUserInfo.Instance.GetCurrentUserEntity();

		if( player != null)
		{
			player.SetProperty( eComponentProperty.HP_CUR, _result.fHpCur);
			player.SetProperty( eComponentProperty.HP_MAX, _result.sFinalSatus.fHPMax);
			player.SetProperty( eComponentProperty.MP_CUR, _result.fMpCur);
			player.SetProperty( eComponentProperty.MP_MAX, _result.sFinalSatus.fMPMax);
			
			savedCharStat.sFinalStatus = _result.sFinalSatus;
			
			player.HandleMessage( new Msg_MoveSpeedRefresh( _result.sFinalSatus.nMoveSpeed / 100));
			player.HandleMessage( new Msg_AttackSpeedRefresh( _result.sFinalSatus.nAtkSpeed * AsProperty.s_BaseAttackSpeedRatio));
			savedCharStat.hpCur_ = _result.fHpCur;
			savedCharStat.mpCur_ = _result.fMpCur;
		}
	}

	public void SetCurrentUserEnterWorldinfo( AS_GC_ENTER_WORLD_RESULT _info)
	{
		curUserEnterWorldinfo = _info;
		AsCommonSender.selfPosition = charPosition = _info.sPosition;
	}

	//$yde
	public void SetCurrentUserCharacterPosition( UnityEngine.Vector3 _pos)
	{
		AsCommonSender.selfPosition = charPosition = _pos;
	}

	public UnityEngine.Vector3 GetCurrentUserCharacterPosition()
	{
		return charPosition;
	}

	public AS_GC_ENTER_WORLD_RESULT GetCurrentUserEnterWorldinfo()
	{
		return curUserEnterWorldinfo;
	}

#if USE_INSTANCE_SERVER
	public void SetCurrentInstanceInfo( AS_GC_INSTANCE_CREATE_RESULT _info)
	{
		curUserInstanceInfo = _info;
	}


	public AS_GC_INSTANCE_CREATE_RESULT GetCurrentInstanceInfo()
	{
		return curUserInstanceInfo;
	}
#endif

	public AsUserEntity GetCurrentUserEntity()
	{
#if false
		List<AsUserEntity> entity = AsEntityManager.Instance.GetUserEntityBySessionId( nGamerUserSessionIdx);
		if( entity == null)
			return null;
		else if( entity.Count == 1)
			return entity[0];
		else
		{
			if( null == curUserCharacterinfo)
				return null;

			return AsEntityManager.Instance.GetUserEntityByUniqueId( curUserCharacterinfo.nCharUniqKey);
		}
#endif
		List<AsUserEntity> entity = AsEntityManager.Instance.GetUserEntityBySessionId( nGamerUserSessionIdx);
		if( null != entity)
		{
			if( 1 == entity.Count)
				return entity[0];

			if( null == curUserCharacterinfo)
				return null;

			return AsEntityManager.Instance.GetUserEntityByUniqueId( curUserCharacterinfo.nCharUniqKey);
		}

		return null;
	}

	public void ApplyInGameDataOnChannelInfo()
	{
		AsUserEntity userEntity = GetCurrentUserEntity();
		if( null == userEntity)
			return;

		sITEMVIEW[] itemViewTemp = userEntity.getCharView;
		sITEMVIEW[] itemCosViewTemp = userEntity.getCosItemView;

		savedCharStat.sCosItemView = itemCosViewTemp;
		savedCharStat.sCharView = itemViewTemp;
		savedCharStat.bCostumeOnOff = userEntity.isCostumeOnOff;;
	}

	public void ApplyInGameDataOnSelectInfo()
	{
		foreach( System.Object obj in characterInfos)
		{
			if( obj.GetType() != typeof( sCHARVIEWDATA))
				continue;

			sCHARVIEWDATA data = obj as sCHARVIEWDATA;

			if( data.nCharUniqKey == savedCharStat.uniqKey_)
			{
				data.nLevel = savedCharStat.level_;

				sITEMVIEW[] itemViewTemp = AsUserInfo.Instance.GetCurrentUserEntity().getCharView;
				sITEMVIEW[] itemCosViewTemp = AsUserInfo.Instance.GetCurrentUserEntity().getCosItemView;

				data.sNormalItemVeiw_1 = itemViewTemp[0];
				data.sNormalItemVeiw_2 = itemViewTemp[1];
				data.sNormalItemVeiw_3 = itemViewTemp[2];
				data.sNormalItemVeiw_4 = itemViewTemp[3];
				data.sNormalItemVeiw_5 = itemViewTemp[4];

				data.sCosItemView_1 = itemCosViewTemp[0];
				data.sCosItemView_2 = itemCosViewTemp[1];
				data.sCosItemView_3 = itemCosViewTemp[2];
				data.sCosItemView_4 = itemCosViewTemp[3];
				data.sCosItemView_5 = itemCosViewTemp[4];
				data.sCosItemView_6 = itemCosViewTemp[5];
				data.sCosItemView_7 = itemCosViewTemp[6];
				data.bCostumeOnOff = AsUserInfo.Instance.GetCurrentUserEntity().isCostumeOnOff;
				data.nHair = AsUserInfo.Instance.GetCurrentUserEntity().getHairItemIndex;
				data.nHairColor = 0;
//				data.ShowInfo();
				break;
			}
		}
	}

	public void SetPrivateShopInfo( body1_GC_CHAR_LOAD_RESULT _result)
	{
		m_nPrivateShopOpenCharUniqKey = _result.nPrivateShopOpenCharUniqKey;
		m_nPrivateShopCreateItemSlot = _result.nPrivateShopCreateItemSlot;

//		m_nPrivateShopRemainingTime = _result.nPrivateShopRemainingTime;

//		m_nPrivateShopMaxOpenTime = _result.nPrivateShopMaxOpenTime;
//		m_nPrivateShopOpenTime = _result.nPrivateShopOpenTime;
	}

	public void SetPrivateShopInfo( body_SC_PRIVATESHOP_OPEN _open)
	{
		m_nPrivateShopOpenCharUniqKey = _open.nCharUniqKey;
		m_nPrivateShopCreateItemSlot = _open.nOpenItemInvenSlot;

//		m_nPrivateShopRemainingTime = _open.nPrivateShopRemainingTime;

//		m_nPrivateShopMaxOpenTime = _result.nPrivateShopMaxOpenTime;
//		m_nPrivateShopOpenTime = _result.nPrivateShopOpenTime;
	}

	public void ClosePrivateShop()
	{
		m_nPrivateShopOpenCharUniqKey = 0;
		savedCharStat.shopOpening_ = false;
	}

	public void StoreBuff( Msg_CharBuff _buff)
	{
		foreach( Msg_CharBuff_Body body in _buff.listBuff_)
		{
			#region - init dictionary -
			if( m_dicBuff.ContainsKey( body.skillTableIdx_) == false)
				m_dicBuff.Add( body.skillTableIdx_, new Dictionary<int, Msg_CharBuff_Body>());

			if( m_dicBuff[body.skillTableIdx_].ContainsKey( body.potencyIdx_) == true)
			{
				m_dicBuff[body.skillTableIdx_].Remove( body.potencyIdx_);
			}
			#endregion

			m_dicBuff[body.skillTableIdx_].Add( body.potencyIdx_, body);
		}
	}

	public void ReleaseBuff( Msg_CharDeBuff _buff)
	{
		if( m_dicBuff.ContainsKey( _buff.skillTableIdx_) == true &&
			m_dicBuff[_buff.skillTableIdx_].ContainsKey( _buff.potencyIdx_) == true)
		{
			m_dicBuff[_buff.skillTableIdx_].Remove( _buff.potencyIdx_);

			if( m_dicBuff[_buff.skillTableIdx_].Count == 0)
				m_dicBuff.Remove( _buff.skillTableIdx_);
		}
	}

	public void ClearBuff()
	{
		m_dicBuff.Clear();
	}
	
	public void ApplyPlayerStat()
	{
		AsUserEntity userEntity = AsEntityManager.Instance.UserEntity;
		if( null != userEntity)
		{
			userEntity.HandleMessage( new Msg_MoveSpeedRefresh( (float)savedCharStat.sFinalStatus.nMoveSpeed * AsProperty.s_BaseMoveSpeedRatio));
			userEntity.HandleMessage( new Msg_AttackSpeedRefresh( savedCharStat.sFinalStatus.nAtkSpeed * AsProperty.s_BaseAttackSpeedRatio));
		}
	}
	//~$yde

	public bool IsEquipChaingEnable()
	{
		if( null == AsEntityManager.Instance.UserEntity)
			return false;

		bool bSkiling = AsEntityManager.Instance.UserEntity.GetProperty<bool>( eComponentProperty.COMBAT);
		if( true == bSkiling)
			return false;

		return true;
	}

	public bool IsBattle()
	{
		if( null == AsEntityManager.Instance.UserEntity)
			return false;

		return AsEntityManager.Instance.UserEntity.GetProperty<bool>( eComponentProperty.COMBAT);
	}

	public bool IsLiving()
	{
		if( null == AsEntityManager.Instance.UserEntity)
			return false;

		return AsEntityManager.Instance.UserEntity.GetProperty<bool>( eComponentProperty.LIVING);
	}

	public bool IsDied()
	{
		AsPlayerFsm playerFsm = AsEntityManager.Instance.GetPlayerCharFsm();
		if( null != playerFsm)
		{
			if( AsPlayerFsm.ePlayerFsmStateType.DEATH == playerFsm.CurrnetFsmStateType)
				return true;
		}

		return false;
	}

	public string GetCharacterName( uint _characterUniqueKey)
	{
		for( int i = 0; i < characterInfos.Count; i++)
		{
			if( characterInfos[i].GetType() == typeof( sCHARVIEWDATA))
			{
				sCHARVIEWDATA characterInfo = characterInfos[i] as sCHARVIEWDATA;
	
				return System.Text.Encoding.UTF8.GetString( characterInfo.szCharName);
			}
			else
				Debug.LogWarning( characterInfos[i].GetType());
		}

		return string.Empty;
	}

	public string GetCharacterName()
	{
		return GetCharacterName( curUserCharacterinfo.nCharUniqKey);
	}

	public string GetUserID()
	{	//2013.10.29.
		if( WemeSdkManager.Instance.IsWemeLogin == false)
			return curID;
		else
			return UserData.Instance.playerKey;
		return null;
	}

	public bool HaveCharacter( uint _uinqueKey)
	{
		for( int i = 0; i < characterInfos.Count; i++)
		{
			if( characterInfos[i].GetType() == typeof( sCHARVIEWDATA))
			{
				sCHARVIEWDATA characterInfo = characterInfos[i] as sCHARVIEWDATA;

				if( characterInfo.nCharUniqKey == _uinqueKey)
					return true;
			}
		}

		return false;
	}
	
	
	public void ChangeCharacterName(uint _uinqueKey , string szNewName)
	{
		for( int i = 0; i < characterInfos.Count; i++)
		{
			if( characterInfos[i].GetType() == typeof( sCHARVIEWDATA))
			{
				sCHARVIEWDATA characterInfo = characterInfos[i] as sCHARVIEWDATA;

				if( characterInfo.nCharUniqKey == _uinqueKey)
				{
					byte[] byteArray = System.Text.UTF8Encoding.UTF8.GetBytes(szNewName);
					Buffer.BlockCopy( byteArray, 0, characterInfo.szCharName, 0, byteArray.Length);
					characterInfo.szCharName[byteArray.Length] = 0;
					break;
				}
			}
		}
	}	
}
