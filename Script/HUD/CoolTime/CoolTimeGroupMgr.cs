using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CoolTimeGroupMgr : MonoBehaviour 
{
	private static CoolTimeGroupMgr ms_kIstance = null;
	private SortedDictionary<int, CoolTimeGroup> m_CoolTimeGroupList = new SortedDictionary<int, CoolTimeGroup>();
	private List<Tbl_Skill_Record> m_InitSkillList = new List<Tbl_Skill_Record>();
	
	public static CoolTimeGroupMgr Instance
	{
		get	{ return ms_kIstance; }
	}
	
	private void SkillDelegatorCooltime( Tbl_Skill_Record skillRecord )
	{
		switch(skillRecord.Command_Type)
		{
		case eCommand_Type.ArcCW: 
			BeginCommandSkillCoolDown(COMMAND_SKILL_TYPE._ARC_CW); 
			break;
		case eCommand_Type.ArcCCW: 
			BeginCommandSkillCoolDown(COMMAND_SKILL_TYPE._ARC_CCW); 
			break;
		case eCommand_Type.Straight: 
			BeginCommandSkillCoolDown(COMMAND_SKILL_TYPE._STRAIGHT); 
			break;
		case eCommand_Type.CircleCW: 
			BeginCommandSkillCoolDown(COMMAND_SKILL_TYPE._CIRCLE_CW); 
			break;
		case eCommand_Type.CircleCCW: 
			BeginCommandSkillCoolDown(COMMAND_SKILL_TYPE._CIRCLE_CCW); 
			break;
		case eCommand_Type.DoubleTab:
			{
				switch( skillRecord.CommandPicking_Type)
				{
				case eCommandPicking_Type.FingerPoint:
					BeginCommandSkillCoolDown( COMMAND_SKILL_TYPE._DOUBLE_TAP_MONSTER);
					break;
				case eCommandPicking_Type.Self:
					BeginCommandSkillCoolDown( COMMAND_SKILL_TYPE._DOUBLE_TAP_PLAYER);
					break;
				}
			}
			break;
		}
	}
	
	private void BeginCommandSkillCoolDown( COMMAND_SKILL_TYPE _type)
	{
		if( null == AsSkillDelegatorManager.Instance)
			return;	
		
		AsUserInfo.Instance.CommandSkillCoolTimeCompletion[(int)_type] = false;
		AsSkillDelegatorManager.Instance.AddDelegator( _type);
	}
	
	
	public void SetSkillUseResult( AS_SC_CHAR_ATTACK_NPC_1 _charAttackNpc )
	{
		Tbl_Skill_Record skillRecord = AsTableManager.Instance.GetTbl_Skill_Record( _charAttackNpc.nSkillTableIdx );	
		if( null == skillRecord )
		{
			Debug.LogError("CooltimeGroupMgr::SetSkillUseResult()[ null != skillRecord ] id : " + _charAttackNpc.nSkillTableIdx );
			return;
		}
		
		//$yde
		if( false == _charAttackNpc.bReady &&
			(skillRecord.Skill_Type != eSKILL_TYPE.Base && skillRecord.Skill_Type != eSKILL_TYPE.SlotBase))
		{
//			Debug.LogWarning("CooltimeGroupMgr::SetSkillUseResult: [used, received] = [" + AsCommonSender.usedSkillIdx + ", " + skillRecord.Index + "]");
		}
		
		if( false == _charAttackNpc.bCasting &&
			false == _charAttackNpc.bReady &&
			-1 != _charAttackNpc.nChargeStep &&
			(skillRecord.Skill_Type != eSKILL_TYPE.Base && skillRecord.Skill_Type != eSKILL_TYPE.SlotBase))
		{
//			AsCommonSender.usedSkillIdx = -1;
			AsCommonSender.EndSkillUseProcess();
			
			if( RuntimePlatform.OSXEditor == Application.platform || RuntimePlatform.WindowsEditor == Application.platform)
			{
				AsPlayerFsm playerFsm = AsEntityManager.Instance.GetPlayerCharFsm();
				if( null != playerFsm )
				{
					if( false == playerFsm.bUseCoolTime )
					{
						_charAttackNpc.nCoolTime = 1;
					}					
				}		
			}		
			
			if( skillRecord.Skill_Type == eSKILL_TYPE.SlotBase )
			{
				return;
			}
				
			CoolTimeGroup cooltimeGrout = CoolTimeGroupMgr.Instance.GetCoolTimeGroup( _charAttackNpc.nSkillTableIdx, _charAttackNpc.nSkillLevel );
			if( null != cooltimeGrout )
			{
				if(  0  == _charAttackNpc.nChargeStep )
				{
					cooltimeGrout.RemainCooltime( (float)_charAttackNpc.nCoolTime );
					SkillDelegatorCooltime(skillRecord);
				}
				else
				{
					Tbl_SkillLevel_Record skillData = AsTableManager.Instance.GetTbl_SkillLevel_Record
						( _charAttackNpc.nSkillLevel, _charAttackNpc.nSkillTableIdx, _charAttackNpc.nChargeStep );					
										
					if( null != skillData )
					{	
						float fMaxTime = skillData.CoolTime;
						cooltimeGrout.RemainCooltime( (float)_charAttackNpc.nCoolTime, fMaxTime );	
						SkillDelegatorCooltime(skillRecord);
					}
					else
					{
						Debug.LogError("CoolTimeGroupMgr::SetSkillUseResult()[ null == Tbl_SkillLevel_Record ] id " + _charAttackNpc.nSkillTableIdx +
							" level : " + _charAttackNpc.nSkillLevel + " chargeStep : " +  _charAttackNpc.nChargeStep );
					}				
				}
			}
		}
	}
	
	public CoolTimeGroup GetCoolTimeGroup( int iSkillID, int iSkillLevelID )
	{
		Tbl_Skill_Record skillRecord = AsTableManager.Instance.GetTbl_Skill_Record( iSkillID );
		if( null == skillRecord )
			return null;		
		
		if( 0 == skillRecord.getCoolTimeGroup || int.MaxValue == skillRecord.getCoolTimeGroup )
			return null;
		
		return GetCoolTimeGroup( skillRecord.getCoolTimeGroup, iSkillID, iSkillLevelID );
	}
	
	/*public CoolTimeGroup GetCoolTimeGroup( int iGroupId )
	{
		if( false == m_CoolTimeGroupList.ContainsKey( iGroupId ) )
		{
			AsUtil.ShutDown("SetCoolTimeGroup()[ true == m_CoolTimeGroupList.ContainsKey ] id : " + iGroupId );
			return null;
		}
		
		return m_CoolTimeGroupList[iGroupId];
	}*/
	
	
	
	public CoolTimeGroup GetCoolTimeGroup( int iCoolGroupID, int iSkillID, int iSkillLevelID )
	{
		if( true == m_CoolTimeGroupList.ContainsKey( iCoolGroupID ) )
			return m_CoolTimeGroupList[iCoolGroupID];
		
		float fInitCoolTime = 1.0f;
		
		Tbl_SkillLevel_Record skillData = AsTableManager.Instance.GetTbl_SkillLevel_Record( iSkillLevelID, iSkillID );
		if( null == skillData )
		{
			Debug.LogError("CoolTimeGroupMgr::GetCoolTimeGroup() [ null == skillData ] skill id : " + iSkillID + " skilllevel id : " + iSkillLevelID + " group : " + iCoolGroupID );
			return null;
		}
		
		fInitCoolTime = skillData.CoolTime * 0.001f;
		if( 0 >= fInitCoolTime )
		{
			Debug.LogError("CoolTimeGroupMgr::GetCoolTimeGroup() [ 0 >= fInitCoolTime  ] skill id : " + iSkillID + " time :" + skillData.CoolTime + " group : " + iCoolGroupID );			
			return null;
		}
		
		
		CoolTimeGroup coolTimeGroup = new CoolTimeGroup( iCoolGroupID, fInitCoolTime );
		m_CoolTimeGroupList.Add( iCoolGroupID, coolTimeGroup );
		
		return coolTimeGroup;		
	}
	
	public void SetCoolTimeGroup( int skillId, int _time )
	{
		Tbl_Skill_Record skillRecord = AsTableManager.Instance.GetTbl_Skill_Record( skillId );
		if(null==skillRecord)
		{
			Debug.LogError("CooltimeGroupMgr::SetCoolTimeGroup()[ null==skillRecord ] id : " + skillId );
			return;
		}
		
		Tbl_SkillLevel_Record _skilllevelRecord = null;
		int coolgroup = skillRecord.getCoolTimeGroup;
		int _maxTime = 1;
		m_InitSkillList.Add(skillRecord); 	
		
		
		if( eCLASS.All == skillRecord.Class )
		{
			_skilllevelRecord = AsTableManager.Instance.GetTbl_SkillLevel_Record( 1, skillId );
			if( null != _skilllevelRecord )
			{
				_maxTime = _skilllevelRecord.CoolTime;
			}
		}
		else if( null != SkillBook.Instance )
		{
			if( SkillBook.Instance.dicActive.ContainsKey( skillId ) )
			{
				_skilllevelRecord = AsTableManager.Instance.GetTbl_SkillLevel_Record( SkillBook.Instance.dicActive[skillId].nSkillLevel, skillId );
				if( null != _skilllevelRecord )
				{
					_maxTime = _skilllevelRecord.CoolTime;
				}
			}
		}
		
		if( _maxTime < _time )
		{
			_maxTime = _time;
		}
		
		
		if( true == m_CoolTimeGroupList.ContainsKey( coolgroup ) )
		{
			//m_CoolTimeGroupList[ coolgroup ].SetInitcoolTime((float)_time * 0.001f);
			//m_CoolTimeGroupList[ coolgroup ].ActionCoolTime();		
			m_CoolTimeGroupList[ coolgroup ].RemainCooltime( _time, _maxTime );
			SkillDelegatorCooltime( skillRecord );
			return;
		}
		
		CoolTimeGroup coolTimeGroup = new CoolTimeGroup( coolgroup, (float)_time * 0.001f );
		//coolTimeGroup.ActionCoolTime();
		coolTimeGroup.RemainCooltime( _time, _maxTime );		
		m_CoolTimeGroupList.Add( coolgroup, coolTimeGroup );
		
		SkillDelegatorCooltime( skillRecord );
	}
	
	public void CheckLoadMap()
	{
		if( 0 >= m_InitSkillList.Count )
		{
			return;
		}
		
		foreach( Tbl_Skill_Record skillRecord in m_InitSkillList )
		{
			if( null == skillRecord )
				continue;
			SkillDelegatorCooltime(skillRecord);
		}
		m_InitSkillList.Clear();
	}
	
	
	public void Clear()
	{
		m_InitSkillList.Clear();			
		m_CoolTimeGroupList.Clear();
		if( null != AsQuickSlotManager.Instance )
			AsQuickSlotManager.Instance.Clear();
		if( null != AsSkillDelegatorManager.Instance)
			AsSkillDelegatorManager.Instance.Clear();
		if( null != AsUserInfo.Instance)
			AsUserInfo.Instance.ResetCommandSkillCoolTimeCompletion();
	}
	
	public void CoolTimeClear()
	{
		m_InitSkillList.Clear();	
		m_CoolTimeGroupList.Clear();
		
		if( GAME_STATE.STATE_INGAME == AsGameMain.s_gameState )
		{
			if( null != AsQuickSlotManager.Instance )
				ItemMgr.HadItemManagement.QuickSlot.ResetSlot();
		}
		else
		{
			if( null != AsQuickSlotManager.Instance )
				AsQuickSlotManager.Instance.Clear();
		}
		if( null != AsSkillDelegatorManager.Instance)
			AsSkillDelegatorManager.Instance.Clear();		
		if( null != AsUserInfo.Instance)
			AsUserInfo.Instance.ResetCommandSkillCoolTimeCompletion();
	}
	

	
	// Awake
	void Awake()
	{        
        ms_kIstance = this;
	}
	
	
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{		
		foreach(KeyValuePair<int, CoolTimeGroup> pair in m_CoolTimeGroupList)
		{
			pair.Value.Update(); 			
		}
	}
}
