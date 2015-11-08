#define _STANCE

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillView
{
	public int	nSkillTableIdx;
	public int	nSkillLevel;
	private Tbl_SkillBook_Record record_;
	
	public Tbl_SkillBook_Record getSkillRecord
	{
		get
		{
			/*if( null == record_ )
			{
				Debug.LogError("SkillView::getSkillRecord[ null == record_ ]");
			}*/
			
			return record_;
		}
	}
	
	public SkillView(int _idx, int _lv)//, Tbl_SkillBook_Record _record)
	{
		nSkillTableIdx = _idx;
		nSkillLevel = _lv;
	}
	
	public SkillView(int _idx, int _lv, Tbl_SkillBook_Record _record)
	{
		nSkillTableIdx = _idx;
		nSkillLevel = _lv;
		record_ = _record;
		
		if( null == record_ )
		{
			Debug.LogError("null == record_ 1 ");
		}
	}
	
	public void SetSkillBookRecord(Tbl_SkillBook_Record _record)
	{
		record_ = _record;
		
		if( null == record_ )
		{
			Debug.LogError("null == record_ 2 ");
		}
		
	}

#if false
	public void SetSkillBookRecordByClass(eCLASS _class)
	{
		record_ = AsTableManager.Instance.GetTbl_SkillBook_Record(_class, nSkillTableIdx, nSkillLevel);
		if(record_ == null)
		{
			Tbl_Skill_Record skill = AsTableManager.Instance.GetTbl_Skill_Record(nSkillTableIdx);
			if(skill.Skill_Type != eSKILL_TYPE.Base)
			{
				Debug.LogWarning("SkillView::SetSkillBookRecordByClass: record is not exist. class=" + _class + 
					", skill index=" + nSkillTableIdx + ", skill level=" + nSkillLevel);
			}
		}
		
		if( null == record_ )
		{
			Debug.LogError("null == record_ 3 ");
		}
	}
#endif
}

public class StanceInfo
{
	int m_StanceSkill; public int StanceSkill{get{return m_StanceSkill;}}
	int m_SkillLevel; public int SkillLevel{get{return m_SkillLevel;}}
	int m_StancePotency; public int StancePotency{get{return m_StancePotency;}}
	
//	public StanceInfo(int _skill, int _potencyIdx)
//	{
//		m_StanceSkill = _skill;
//		m_StancePotency = _potencyIdx;
//	}
//	
//	public StanceInfo(int _skill, int _lv, int _potencyIdx)
//	{
//		m_StanceSkill = _skill;
//		m_SkillLevel = _lv;
//		m_StancePotency = _potencyIdx;
//	}
	
	public StanceInfo(body1_SC_SKILL_LIST _list)
	{
#if _STANCE
		m_StanceSkill = _list.nStanceSkill;
		m_SkillLevel = _list.nStanceSkillLevel;
		m_StancePotency = _list.nStancePotencyIdx;
#endif
	}
	public StanceInfo(body_SC_CHAR_SKILL_STANCE _stance)
	{
#if _STANCE
		m_StanceSkill = _stance.nStanceSkill;
		m_SkillLevel = _stance.nStanceSkillLevel;
		m_StancePotency = _stance.nStancePotencyIdx;
#endif
	}
	public StanceInfo(Msg_OtherCharSkillStance _stance)
	{
#if _STANCE
		m_StanceSkill = _stance.stanceSkill_;
		m_SkillLevel = _stance.stanceLevel_;
		m_StancePotency = _stance.stancePotencyIdx_;
#endif
	}
}

public class SkillBook
{
	#region - singleton -
	private static SkillBook instance = null;
	public static SkillBook Instance
	{
		get
		{
			if( null == instance)
				instance = new SkillBook();

			return instance;
		}
	}
	#endregion
	
	// begin kij
	//readonly int m_RegisterBeginIdx = 12;
	//readonly int m_RegisterSize = 12;
	// end kij
	
	Dictionary<int, SkillView> m_dicCurSkill = new Dictionary<int, SkillView>();public Dictionary<int, SkillView> dicCurSkill{get{return m_dicCurSkill;}}
	
	// begin kij
	//sQUICKSLOT[] m_QuickSlot = null;
	// end kij
	Dictionary<int, SkillView> m_dicNotSetting = new Dictionary<int, SkillView>();public Dictionary<int, SkillView> dicNotSetting{get{return m_dicNotSetting;}}
	Dictionary<int, SkillView> m_dicActive = new Dictionary<int, SkillView>();public Dictionary<int, SkillView> dicActive{get{return m_dicActive;}}
	Dictionary<int, SkillView> m_dicFinger = new Dictionary<int, SkillView>();public Dictionary<int, SkillView> dicFinger{get{return m_dicFinger;}}
	Dictionary<int, SkillView> m_dicPassive = new Dictionary<int, SkillView>();public Dictionary<int, SkillView> dicPassive{get{return m_dicPassive;}}
	
	StanceInfo m_StanceInfo = null; public StanceInfo StanceInfo{get{return m_StanceInfo;}}
	
	Dictionary<int, bool> m_newdicActive = new Dictionary<int, bool>();public Dictionary<int, bool> newdicActive{get{return m_newdicActive;}}
	Dictionary<int, bool> m_newdicFinger = new Dictionary<int, bool>();public Dictionary<int, bool> newdicFinger{get{return m_newdicFinger;}}
	Dictionary<int, bool> m_newdicPassive = new Dictionary<int, bool>();public Dictionary<int, bool> newdicPassive{get{return m_newdicPassive;}}
	
	private bool m_isNewSkillAdd = false;
	public bool isNewSkillAdd
	{
		get
		{
			return m_isNewSkillAdd;
		}
	}
	
	public void SetNewSkillInit()
	{
		m_isNewSkillAdd = false;
		m_newdicActive.Clear();
		m_newdicFinger.Clear();
		m_newdicPassive.Clear();
		if( null != AsHudDlgMgr.Instance)
		{
			AsHudDlgMgr.Instance.SetNewSkillImg(false);
			AsHudDlgMgr.Instance.CheckNewMenuImg();
		}
	}
	
	
	#region - init & release -
	private SkillBook()
	{
		instance = this;
	}
	
	public void InitSkillBook()
	{
		m_dicCurSkill.Clear();
		m_dicNotSetting.Clear();
		m_dicActive.Clear();
		m_dicFinger.Clear();
		m_dicPassive.Clear();
	}
	#endregion
	
	#region - public -
	public void SetCurrentSkill(body1_SC_SKILL_LIST _list)
	{
//		eCLASS __class = AsUserInfo.Instance.GetCurrentUserEntity().GetProperty<eCLASS>(eComponentProperty.CLASS);
		
		for(int i=0; i<_list.nSkillCnt; ++i)
		{
			SkillView curSlot = new SkillView(_list.body[i].nSkillTableIdx, _list.body[i].nSkillLevel);
//			curSlot.SetSkillBookRecordByClass(__class);
			
			Tbl_Skill_Record record = AsTableManager.Instance.GetTbl_Skill_Record(curSlot.nSkillTableIdx);
			if(record.Skill_Type == eSKILL_TYPE.Base)
				continue;
						
			m_dicCurSkill.Add(curSlot.nSkillTableIdx, curSlot);
			SortCurSkill();
			
			switch(record.Skill_Type)
			{
			case eSKILL_TYPE.Active:
			case eSKILL_TYPE.Case:
			case eSKILL_TYPE.Charge:
			case eSKILL_TYPE.Target:
			case eSKILL_TYPE.SlotBase:
			case eSKILL_TYPE.Stance:
			case eSKILL_TYPE.TargetCharge:
				m_dicNotSetting.Add(curSlot.nSkillTableIdx, curSlot);
				SortNotSetting();
			break;
			}
			
			AddSkillByType(curSlot, false);
		}
		
		SetNewSkillInit();
		
		// begin kij
		/*for(int i=m_RegisterBeginIdx; i<m_RegisterBeginIdx + m_RegisterSize; ++i)
		{			
			if(m_dicNotSetting.ContainsKey(m_QuickSlot[i].nValue) == true)
				m_dicNotSetting.Remove(m_QuickSlot[i].nValue);	
		}*/
		
		//ResetDicNotSettingData();
		// end kij
		
		InitStance(_list);
	}
	
	public void ResetDicNotSettingData()
	{
		m_dicNotSetting.Clear();
		
		foreach(KeyValuePair<int, SkillView> pair in m_dicActive)
		{
			m_dicNotSetting.Add(pair.Key, pair.Value);
			SortNotSetting();
		}
			
		if( null == ItemMgr.HadItemManagement.QuickSlot.getQuickSlots )
		{
			Debug.LogError("SkillBook::SetCurrentSkill() [ null == ItemMgr.HadItemManagement.QuickSlot.getQuickSlots ]");
			return;
		}
		
		for(int i=0; i<AsGameDefine.QUICK_SLOT_MAX; ++i)
		{
			if( (int)eQUICKSLOT_TYPE.eQUICKSLOT_TYPE_SKILL != ItemMgr.HadItemManagement.QuickSlot.getQuickSlots[i].eType )
				continue;
			
			int iKey = ItemMgr.HadItemManagement.QuickSlot.getQuickSlots[i].nValue;
			if(m_dicNotSetting.ContainsKey(iKey) == true)
				m_dicNotSetting.Remove(iKey);
		}
	}
	
	public void AddLearnSkill(body_SC_SKILL_LEARN_RESULT _learn)
	{
//		eCLASS __class = AsUserInfo.Instance.GetCurrentUserEntity().GetProperty<eCLASS>(eComponentProperty.CLASS);
		SkillView view = new SkillView(_learn.sSkill.nSkillTableIdx, _learn.sSkill.nSkillLevel);
//		view.SetSkillBookRecordByClass(__class);
		
		Tbl_Skill_Record record = AsTableManager.Instance.GetTbl_Skill_Record(view.nSkillTableIdx);
		if(record.Skill_Type == eSKILL_TYPE.Base)
			return;
		
		if(m_dicCurSkill.ContainsKey(_learn.sSkill.nSkillTableIdx) == true)
		{
			m_dicCurSkill.Remove(_learn.sSkill.nSkillTableIdx);
		}
		m_dicCurSkill.Add(_learn.sSkill.nSkillTableIdx, view);
		SortCurSkill();
		
		if(m_dicNotSetting.ContainsKey(_learn.sSkill.nSkillTableIdx) == true)
		{
			m_dicNotSetting.Remove(_learn.sSkill.nSkillTableIdx);
		}
		m_dicNotSetting.Add(_learn.sSkill.nSkillTableIdx, view);
		SortNotSetting();
		
		//	add resettedSkills
		if( AsUserInfo.Instance.resettedSkills.ContainsKey( _learn.nSkillBookIdx ) == false )
		{
			body2_SC_SKILLBOOK_LIST _skillBookList = new body2_SC_SKILLBOOK_LIST();
			_skillBookList.nSkillBookTableIdx = _learn.nSkillBookIdx;
			AsUserInfo.Instance.resettedSkills.Add( _learn.nSkillBookIdx , _skillBookList);
		}
		
		AddSkillByType(view, true);
		
		if( record.Skill_Type != eSKILL_TYPE.Command && record.Skill_Type != eSKILL_TYPE.Passive )
			ItemMgr.HadItemManagement.QuickSlot.SetLearnSkill( _learn.sSkill.nSkillTableIdx, _learn.sSkill.nSkillLevel );
		
		if(AsHudDlgMgr.Instance.IsOpenedSkillBook == true)
		{
			ResetDicNotSettingData();
			AsHudDlgMgr.Instance.skillBookDlg.RefreshSkillBookDlg();			
		}
	}
	
	public void SlotRegister(int _skillIdx)
	{
		if(m_dicNotSetting.ContainsKey(_skillIdx) == true)
		{
			m_dicNotSetting.Remove(_skillIdx);
			SortNotSetting();
		}
	}
	
	public void SlotRelease(int _skillIdx)
	{
		if(m_dicCurSkill.ContainsKey(_skillIdx) == true)
		{
			m_dicNotSetting.Add(_skillIdx, m_dicCurSkill[_skillIdx]);
			SortNotSetting();
		}
	}
	
	public void ResetSkillBook()
	{
		#region - preserve current reset disable skills -
		Dictionary<int, SkillView> dicResetDisableSkills = new Dictionary<int, SkillView>();
		
		foreach(KeyValuePair<int, SkillView> pair in m_dicCurSkill)
		{
			Tbl_Skill_Record skill = AsTableManager.Instance.GetTbl_Skill_Record(pair.Value.nSkillTableIdx);
			if(skill.SkillReset == eSkillReset.Disable)
				dicResetDisableSkills.Add(skill.Index, pair.Value);
		}
		#endregion
		
		Release();
		
		AsUserEntity user = AsUserInfo.Instance.GetCurrentUserEntity();
		eRACE race = user.GetProperty<eRACE>(eComponentProperty.RACE);
		eCLASS __class = user.GetProperty<eCLASS>(eComponentProperty.CLASS);
		
		AsCharacterCreateClassData createData = AsTableManager.Instance.GetTbl_CreateCharacter_ClassData(race, __class);
		foreach(int idx in createData.commonProp.defaultSkills)
		{
			Tbl_Skill_Record record = AsTableManager.Instance.GetTbl_Skill_Record(idx);
			if(record.Skill_Type != eSKILL_TYPE.Base)// && record.SkillReset == eSkillReset.Enable)
			{
				SkillView view = new SkillView(idx, 1);
				
				m_dicCurSkill.Add(view.nSkillTableIdx, view);
				SortCurSkill();
				m_dicNotSetting.Add(view.nSkillTableIdx, view);
				SortNotSetting();
				
				AddSkillByType(view, false);
				
//				view.SetSkillBookRecordByClass(__class);
			}
		}
		
		#region - add preserved reset disable skills -
		foreach(KeyValuePair<int, SkillView> pair1 in dicResetDisableSkills)
		{
			bool processed = false;
			foreach(KeyValuePair<int, SkillView> pair2 in m_dicCurSkill)
			{
				if(pair1.Value.nSkillTableIdx == pair2.Value.nSkillTableIdx)
				{
					pair2.Value.nSkillLevel = pair1.Value.nSkillLevel;
					processed = true;
					break;
				}
			}
			
			if(processed == false)
			{
				SkillView view = pair1.Value;
					
				m_dicCurSkill.Add(view.nSkillTableIdx, view);
				SortCurSkill();
				m_dicNotSetting.Add(view.nSkillTableIdx, view);
				SortNotSetting();
				
				AddSkillByType(view, false);
			}
		}
		#endregion
		
		SetNewSkillInit();
	}
	
	public void Release()
	{
		m_dicCurSkill.Clear();
		
		m_dicNotSetting.Clear();
		m_dicActive.Clear();
		m_dicFinger.Clear();
		m_dicPassive.Clear();
	}
	
//	Dictionary<int, Tbl_SkillBook_Record> m_ChoiceTypeDic = new Dictionary<int, Tbl_SkillBook_Record>();
//	Dictionary<int, SkillView> m_BaseTypeDic = new Dictionary<int, SkillView>();
	public void LevelUpProcess()
	{		
		int lv = AsUserInfo.Instance.GetCurrentUserEntity().GetProperty<int>(eComponentProperty.LEVEL);
		eCLASS __class = AsUserInfo.Instance.GetCurrentUserEntity().GetProperty<eCLASS>(eComponentProperty.CLASS);
		
		List<string> listChat = new List<string>();
		listChat.Add(string.Format(AsTableManager.Instance.GetTbl_String(135), ""));
		Dictionary<int, Tbl_SkillBook_Record> dicSkillBook = AsTableManager.Instance.GetTbl_SkillBook_RecordsByClass(__class);
		foreach(KeyValuePair<int, Tbl_SkillBook_Record> pair in dicSkillBook)
		{
			Tbl_SkillBook_Record book = pair.Value;
			
			if(book.ClassLevel == lv && book.Skill1_Level == 1)
			{
				if(book.ChoiceType == eChoiceType.Choice)
				{
					string str = "";
					Tbl_Skill_Record skill = AsTableManager.Instance.GetTbl_Skill_Record(book.Skill1_Index);
					str = AsTableManager.Instance.GetTbl_String(skill.SkillName_Index);
					skill = AsTableManager.Instance.GetTbl_Skill_Record(book.Skill2_Index);
					str += " / " + AsTableManager.Instance.GetTbl_String(skill.SkillName_Index);
					listChat.Add(str);
					AsEventNotifyMgr.Instance.LevelUpNotify.SetLearnSkill(str);
				}
				else
				{
					Tbl_Skill_Record skill = AsTableManager.Instance.GetTbl_Skill_Record(book.Skill1_Index);
					listChat.Add(AsTableManager.Instance.GetTbl_String(skill.SkillName_Index));
					AsEventNotifyMgr.Instance.LevelUpNotify.SetLearnSkill(AsTableManager.Instance.GetTbl_String(skill.SkillName_Index));
				}
			}		
		}
		
		if(listChat.Count > 1)
		{
			foreach(string str in listChat)
			{
				AsChatManager.Instance.InsertChat(str, eCHATTYPE.eCHATTYPE_SYSTEM);
//				AsChatManager.Instance.InsertChat(str, eCHATTYPE.eCHATTYPE_PUBLIC);
			}
		}
		
//		Debug.Log("SkillBook::LevelUpProcess: learnable skills");
//		foreach(string str in listChat)
//		{
//			Debug.Log(str);
//		}
	}
	
	void InitStance(body1_SC_SKILL_LIST _list)
	{
		m_StanceInfo = new StanceInfo(_list);
		StanceProcess();
	}
	public void StanceChanged(body_SC_CHAR_SKILL_STANCE _stance)
	{
		m_StanceInfo = new StanceInfo(_stance);
		StanceProcess();
		
		AsCommonSender.EndSkillUseProcess();
		
		Tbl_SkillLevel_Record lv = AsTableManager.Instance.GetTbl_SkillLevel_Record( m_StanceInfo.SkillLevel, m_StanceInfo.StanceSkill);
		if( lv != null)
			CoolTimeGroupMgr.Instance.SetCoolTimeGroup( m_StanceInfo.StanceSkill, lv.CoolTime);
		else
			Debug.LogError("SkillBook::StanceChanged: m_StanceInfo.SkillLevel = " + m_StanceInfo.SkillLevel);
	}
	void StanceProcess()
	{
		//AsUserInfo.Instance.GetCurrentUserEntity().SetStance(m_StanceInfo);
		AsEntityManager.Instance.UserEntity.SetStance( m_StanceInfo);
		AsQuickSlotManager.Instance.ChangeStance(m_StanceInfo);
	}
	#endregion
	#region - method -
	void AddSkillByType(SkillView _skill, bool _isNew )
	{
		Tbl_Skill_Record record = AsTableManager.Instance.GetTbl_Skill_Record(_skill.nSkillTableIdx);
		
		switch(record.Skill_Type)
		{
		case eSKILL_TYPE.Active:
		case eSKILL_TYPE.Case:
		case eSKILL_TYPE.Charge:
		case eSKILL_TYPE.Target:
		case eSKILL_TYPE.SlotBase:
		case eSKILL_TYPE.Stance:
		case eSKILL_TYPE.TargetCharge:
			if(m_dicActive.ContainsKey(_skill.nSkillTableIdx) == true)
				m_dicActive.Remove(_skill.nSkillTableIdx);
			m_dicActive.Add(_skill.nSkillTableIdx, _skill);
			
			if( null!= AsHudDlgMgr.Instance && false == AsHudDlgMgr.Instance.IsOpenedSkillBook && true == _isNew )
			{
				if( false == m_newdicActive.ContainsKey( _skill.nSkillTableIdx ) )
				{	
					m_isNewSkillAdd = true;
					m_newdicActive.Add( _skill.nSkillTableIdx, true );
					AsHudDlgMgr.Instance.CheckNewMenuImg();
				}
			}
			SortActive();
			break;
		case eSKILL_TYPE.Command:
			if(m_dicFinger.ContainsKey(_skill.nSkillTableIdx) == true)
				m_dicFinger.Remove(_skill.nSkillTableIdx);
			m_dicFinger.Add(_skill.nSkillTableIdx, _skill);
			SortFinger();
			
			if( null!= AsHudDlgMgr.Instance && false == AsHudDlgMgr.Instance.IsOpenedSkillBook && true == _isNew )
			{
				if( false == m_newdicFinger.ContainsKey( _skill.nSkillTableIdx ) )
				{
					m_isNewSkillAdd = true;
					m_newdicFinger.Add( _skill.nSkillTableIdx, true );
					AsHudDlgMgr.Instance.CheckNewMenuImg();
				}
			}
			break;
		case eSKILL_TYPE.Passive:
			if(m_dicPassive.ContainsKey(_skill.nSkillTableIdx) == true)
				m_dicPassive.Remove(_skill.nSkillTableIdx);
			m_dicPassive.Add(_skill.nSkillTableIdx, _skill);
			SortPassive();
			
			if( null!= AsHudDlgMgr.Instance && false == AsHudDlgMgr.Instance.IsOpenedSkillBook && true == _isNew )
			{
				if( false == m_newdicPassive.ContainsKey( _skill.nSkillTableIdx ) )
				{
					m_isNewSkillAdd = true;
					m_newdicPassive.Add( _skill.nSkillTableIdx, true );
					AsHudDlgMgr.Instance.CheckNewMenuImg();
				}
			}
			break;
		case eSKILL_TYPE.Base:
		default:
			break;
		}
	}
	
	void SortCurSkill()
	{
		List<SkillView> list = new List<SkillView>();
		foreach(KeyValuePair<int, SkillView> pair in m_dicCurSkill)
		{
			list.Add(pair.Value);
		}
		list.Sort(Comparer);
		
		m_dicCurSkill.Clear();
		foreach(SkillView skillView in list)
		{
			m_dicCurSkill.Add(skillView.nSkillTableIdx, skillView);
		}
	}
	
	void SortNotSetting()
	{
		List<SkillView> list = new List<SkillView>();
		foreach(KeyValuePair<int, SkillView> pair in m_dicNotSetting)
		{
			list.Add(pair.Value);
		}
		list.Sort(Comparer);
		
		m_dicNotSetting.Clear();
		foreach(SkillView skillView in list)
		{
			m_dicNotSetting.Add(skillView.nSkillTableIdx, skillView);
		}
	}
	
	void SortActive()
	{
		List<SkillView> list = new List<SkillView>();
		foreach(KeyValuePair<int, SkillView> pair in m_dicActive)
		{
			list.Add(pair.Value);
		}
		list.Sort(Comparer);
		
		m_dicActive.Clear();
		foreach(SkillView skillView in list)
		{
			m_dicActive.Add(skillView.nSkillTableIdx, skillView);
//			if(skillView.record_ != null)
//				Debug.Log("skillbook:: sorted active class lv:" + skillView.record_.ClassLevel);
		}
	}
	
	void SortFinger()
	{
		List<SkillView> list = new List<SkillView>();
		foreach(KeyValuePair<int, SkillView> pair in m_dicFinger)
		{
			list.Add(pair.Value);
		}
		list.Sort(Comparer);
		
		m_dicFinger.Clear();
		foreach(SkillView skillView in list)
		{
			m_dicFinger.Add(skillView.nSkillTableIdx, skillView);
		}
	}
	
	void SortPassive()
	{
		List<SkillView> list = new List<SkillView>();
		foreach(KeyValuePair<int, SkillView> pair in m_dicPassive)
		{
			list.Add(pair.Value);
		}
		list.Sort(Comparer);
		
		m_dicPassive.Clear();
		foreach(SkillView skillView in list)
		{
			m_dicPassive.Add(skillView.nSkillTableIdx, skillView);
		}
	}
	
	int Comparer(SkillView _left, SkillView _right)
	{
		if(_left.getSkillRecord == null || _right.getSkillRecord == null)
		{
//			Debug.LogWarning("skill book record is null(left:" + _left.nSkillTableIdx + ", right:" + _right.nSkillTableIdx);
			return -1;
		}
		
		if(_left.getSkillRecord.ClassLevel > _right.getSkillRecord.ClassLevel)
			return 1;
		else if(_left.getSkillRecord.ClassLevel == _right.getSkillRecord.ClassLevel)
			return 0;
		else
			return -1;
	}
	
//	MultiDictionary<int, Tbl_SkillBook_Record> TransferToIdxMultidic(List<Tbl_SkillBook_Record> _list)
//	{
//		MultiDictionary<int, Tbl_SkillBook_Record> mdicIdx = new MultiDictionary<int, Tbl_SkillBook_Record>();
//		
//		foreach(Tbl_SkillBook_Record record in _list)
//		{
//			mdicIdx.Add(record.Skill1_Index, record);
//			if(record.ChoiceType == eChoiceType.Choice)
//				mdicIdx.Add(record.Skill2_Index, record);
//		}
//		
//		return mdicIdx;
//	}
//	
//	void DecideLearnableSkill(Tbl_SkillBook_Record _record)
//	{
//		if(_record.ChoiceType == eChoiceType.Choice)
//		{
//			if(m_dicCurSkill.ContainsKey(_record.Skill1_Index) == true)
//			{
//				SkillView learned = m_dicCurSkill[_record.Skill1_Index];
////				if(learned.nSkillLevel + 1 == _record.ClassLevel)
//				if( learned.nSkillLevel + 1 == _record.Skill1_Level)
//					m_BaseTypeDic.Add(_record.Index, new SkillView(_record.Skill1_Index, _record.Skill1_Level, _record));
//			}
//			
//			else if(m_dicCurSkill.ContainsKey(_record.Skill2_Index) == true)
//			{
//				SkillView learned = m_dicCurSkill[_record.Skill2_Index];
////				if(learned.nSkillLevel + 1 == _record.ClassLevel)
//				if( learned.nSkillLevel + 1 == _record.Skill2_Level)
//					m_BaseTypeDic.Add(_record.Index, new SkillView(_record.Skill2_Index, _record.Skill2_Level, _record));
//			}
//			
//			else if(m_dicCurSkill.ContainsKey(_record.Skill1_Index) == false &&
//				m_dicCurSkill.ContainsKey(_record.Skill2_Index) == false)
//			{
//				if(m_ChoiceTypeDic.ContainsKey(_record.Index) == false &&
//					_record.Skill1_Level == 1)
//					m_ChoiceTypeDic.Add(_record.Index, _record);
//			}
//		}
//		else
//		{
//			if(m_dicCurSkill.ContainsKey(_record.Skill1_Index) == true)
//			{
//				SkillView learned = m_dicCurSkill[_record.Skill1_Index];
////				if(learned.nSkillLevel + 1 == _record.ClassLevel)
//				if( learned.nSkillLevel + 1 == _record.Skill1_Level)
//					m_BaseTypeDic.Add(_record.Index, new SkillView(_record.Skill1_Index, _record.Skill1_Level, _record));
//			}
//			else if(_record.Skill1_Level == 1)
//			{
//				m_BaseTypeDic.Add(_record.Index, new SkillView(_record.Skill1_Index, _record.Skill1_Level, _record));
//			}
//		}
//	}
	#endregion
	#region - getter -
	public int GetSkillLevel(int _skillIdx)
	{
		if(m_dicCurSkill.ContainsKey(_skillIdx) == true)
			return m_dicCurSkill[_skillIdx].nSkillLevel;
		else
			return 1;
	}
	
	public int GetSkillLevel(Tbl_Skill_Record _record)
	{
		if(m_dicCurSkill.ContainsKey(_record.Index) == true)
			return m_dicCurSkill[_record.Index].nSkillLevel;
		else
			return 1;
	}
	
	public Tbl_Skill_Record GetLearnedCommandSkill(eCommand_Type _type)
	{
		switch(_type)
		{
		case eCommand_Type.Straight:
		case eCommand_Type.ArcCW:
		case eCommand_Type.ArcCCW:
		case eCommand_Type.CircleCW:
		case eCommand_Type.CircleCCW:
			break;
		default:
			Debug.LogWarning("Invalid command skill request.");
			return null;
		}
		
		foreach(KeyValuePair<int, SkillView> pair in m_dicFinger)
		{
			Tbl_Skill_Record record = AsTableManager.Instance.GetTbl_Skill_Record(pair.Value.nSkillTableIdx);
			if(record.Command_Type == _type)
			{
				return record;
			}
		}
		
		return null;
	}
	
	public Tbl_Skill_Record GetLearnedDoubleTapSkill(eCommandPicking_Type _pickingType)
	{
		foreach(KeyValuePair<int, SkillView> pair in m_dicFinger)
		{
			Tbl_Skill_Record record = AsTableManager.Instance.GetTbl_Skill_Record(pair.Value.nSkillTableIdx);
			if(record.Command_Type == eCommand_Type.DoubleTab && record.CommandPicking_Type == _pickingType)
			{
				return record;
			}
		}
		
		return null;
	}

    public bool ContainSkillTableID(int _skillTableID)
    {
        bool bCheck1 = ContainSkillTableIDCore(m_dicActive , _skillTableID);
        bool bcheck2 = ContainSkillTableIDCore(m_dicFinger , _skillTableID);
        bool bcheck3 = ContainSkillTableIDCore(m_dicPassive, _skillTableID);

        return bCheck1 | bcheck2 | bcheck3;
    }

    bool ContainSkillTableIDCore(Dictionary<int, SkillView> _dic, int _skillTableID)
    {
        if (_dic == null)
            return false;

        return _dic.ContainsKey(_skillTableID);
    }

	
	public bool CheckSkillBookReset()
	{
		AsUserEntity user = AsUserInfo.Instance.GetCurrentUserEntity();
		eRACE race = user.GetProperty<eRACE>(eComponentProperty.RACE);
		eCLASS __class = user.GetProperty<eCLASS>(eComponentProperty.CLASS);
		
		List<int> listDefault = new List<int>();
		AsCharacterCreateClassData createData = AsTableManager.Instance.GetTbl_CreateCharacter_ClassData(race, __class);
		foreach(int idx in createData.commonProp.defaultSkills)
		{
			if(idx == 0)
				continue;
			
			Tbl_Skill_Record record = AsTableManager.Instance.GetTbl_Skill_Record(idx);
			if(record.Skill_Type != eSKILL_TYPE.Base)
			{
				listDefault.Add(idx);
			}
		}
		
		foreach(KeyValuePair<int, SkillView> pair in m_dicCurSkill)
		{
			Tbl_Skill_Record record = AsTableManager.Instance.GetTbl_Skill_Record(pair.Key);
			
			if(listDefault.Count == 0)
			{
				if(record.Skill_Type != eSKILL_TYPE.Base)
					return false;
			}
			else
			{
				foreach(int idx in listDefault)
				{
					if(record.Skill_Type != eSKILL_TYPE.Base && idx != pair.Key)
						return false;
				}
			}
		}
		
		return true;
	}
	#endregion
	
	public void ResetOneSkill( int nSkillTableIdx )
	{
		if( m_dicCurSkill.ContainsKey(nSkillTableIdx) == true )
			m_dicCurSkill.Remove(nSkillTableIdx);

		if( m_dicNotSetting.ContainsKey(nSkillTableIdx) == true )
			m_dicNotSetting.Remove(nSkillTableIdx);

		if( m_dicActive.ContainsKey(nSkillTableIdx) == true )
			m_dicActive.Remove(nSkillTableIdx);
		
		if( m_dicFinger.ContainsKey(nSkillTableIdx) == true )
			m_dicFinger.Remove(nSkillTableIdx);
		
		if( m_dicPassive.ContainsKey(nSkillTableIdx) == true )
			m_dicPassive.Remove(nSkillTableIdx);
	}
}