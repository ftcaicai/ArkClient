using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//public class SkillViewComparer : IEqualityComparer<SkillView>
//{
//	public bool Equals(SkillView _left, SkillView _right)
//	{
//		if(_left.record_ == null || _right == null)
//			return false;
//		
//		if(_left.record_.ClassLevel > _right.record_.ClassLevel)
//			return true;
//		else
//			return false;
//	}
//	
//	public int GetHashCode(SkillView _obj)
//	{
//		return _obj.GetHashCode();
//	}
//}

public enum eSkillGoodsGroup {Learned_Base, Learned_Choice1, Learned_Choice2, UnLearned_Base, UnLearned_Choice}

public class SkillShop
{
	#region - singleton -
	private static SkillShop instance = null;
	public static SkillShop Instance
	{
		get{
			if(instance == null)
			{
				instance = new SkillShop();
			}
			return instance;
		}
	}
	#endregion
	#region - member -
	Dictionary<int, Tbl_SkillBook_Record> m_ChoiceTypeDic = new Dictionary<int, Tbl_SkillBook_Record>();
	Dictionary<int, SkillView> m_BaseTypeDic = new Dictionary<int, SkillView>();
	
	Dictionary<int, SkillView> m_dicLearnedSkill = new Dictionary<int, SkillView>();
	MultiDictionary<int, Tbl_SkillBook_Record> m_mdicSkillBookBySkillIdx = new MultiDictionary<int, Tbl_SkillBook_Record>();
	
	int m_CurNpcIdx;public int CurNpcIdx{get{return m_CurNpcIdx;}}
	#endregion
	#region - public -
	public Dictionary<int, Tbl_SkillBook_Record> ChoiceTypeDic
	{
		get	{ return m_ChoiceTypeDic; }
	}
	
	public Dictionary<int, SkillView> BaseTypeDic
	{
		get	{ return m_BaseTypeDic; }
	}
	#endregion
	#region - init & release -
	private SkillShop()
	{
		instance = this;
	}
	
	void InitSkillShop()
	{
		
	}
	
	void Start()
	{
	}
	#endregion
	
	public void RefreshSkillShop(int _idx)
	{
		m_CurNpcIdx = _idx;
		
		AsUserEntity user = AsUserInfo.Instance.GetCurrentUserEntity();
		eCLASS __class = user.GetProperty<eCLASS>(eComponentProperty.CLASS);
		
		m_dicLearnedSkill = SkillBook.Instance.dicCurSkill;
		List<Tbl_SkillBook_Record> list = AsTableManager.Instance.GetTbl_SkillBook_RecordsByClass(_idx, __class);
		m_mdicSkillBookBySkillIdx = TransferToIdxMultidic(list);
		
		m_ChoiceTypeDic.Clear();
		m_BaseTypeDic.Clear();
		
		foreach(int key in m_mdicSkillBookBySkillIdx.Keys)
		{
			foreach(Tbl_SkillBook_Record record in m_mdicSkillBookBySkillIdx[key])
			{
				Tbl_Skill_Record skill = AsTableManager.Instance.GetTbl_Skill_Record(record.Skill1_Index);
				if(skill.Skill_Type != eSKILL_TYPE.Base)
					DecideSkillGoodsGroup(record);
			}
		}
		
		SortShopDictionary();
	}
	
	#region - method -
	MultiDictionary<int, Tbl_SkillBook_Record> TransferToIdxMultidic(List<Tbl_SkillBook_Record> _list)
	{
		MultiDictionary<int, Tbl_SkillBook_Record> mdicIdx = new MultiDictionary<int, Tbl_SkillBook_Record>();
		
		foreach(Tbl_SkillBook_Record record in _list)
		{
			mdicIdx.Add(record.Skill1_Index, record);
			if(record.ChoiceType == eChoiceType.Choice)
				mdicIdx.Add(record.Skill2_Index, record);
		}
		
		return mdicIdx;
	}
	
	bool _isResettedSkill( Tbl_SkillBook_Record record)
	{
		if( ( true == AsUserInfo.Instance.resettedSkills.ContainsKey( record.Skill1_Index))
			|| ( true == AsUserInfo.Instance.resettedSkills.ContainsKey( record.Skill2_Index)))
			return true;
		
		return false;
	}
	
	void DecideSkillGoodsGroup(Tbl_SkillBook_Record _record)
	{
		if( _record.ChoiceType == eChoiceType.Choice)
		{
			if( m_dicLearnedSkill.ContainsKey(_record.Skill1_Index) == true)
			{
				SkillView learned = m_dicLearnedSkill[_record.Skill1_Index];
				if( learned.nSkillLevel + 1 == _record.Skill1_Level)
					m_BaseTypeDic.Add( _record.Index, new SkillView( _record.Skill1_Index, _record.Skill1_Level, _record));
			}
			else if( m_dicLearnedSkill.ContainsKey(_record.Skill2_Index) == true)
			{
				SkillView learned = m_dicLearnedSkill[_record.Skill2_Index];
				if( learned.nSkillLevel + 1 == _record.Skill2_Level)
					m_BaseTypeDic.Add( _record.Index, new SkillView( _record.Skill2_Index, _record.Skill2_Level, _record));
			}
			else if( ( ( m_dicLearnedSkill.ContainsKey(_record.Skill1_Index) == false)
				&& ( m_dicLearnedSkill.ContainsKey( _record.Skill2_Index) == false))
				|| ( true == _isResettedSkill( _record)))
			{
				if( m_ChoiceTypeDic.ContainsKey( _record.Index) == false && _record.Skill1_Level == 1)
					m_ChoiceTypeDic.Add( _record.Index, _record);
			}
		}
		else
		{
			if( m_dicLearnedSkill.ContainsKey(_record.Skill1_Index) == true)
			{
				SkillView learned = m_dicLearnedSkill[_record.Skill1_Index];
				if( learned.nSkillLevel + 1 == _record.Skill1_Level)
					m_BaseTypeDic.Add( _record.Index, new SkillView( _record.Skill1_Index, _record.Skill1_Level, _record));
			}
			else if( _record.Skill1_Level == 1)
			{
				m_BaseTypeDic.Add( _record.Index, new SkillView( _record.Skill1_Index, _record.Skill1_Level, _record));
			}
		}
	}
	
	void SortShopDictionary()
	{
		List<SkillView> list = new List<SkillView>();
		foreach(KeyValuePair<int, SkillView> pair in m_BaseTypeDic)
		{
			list.Add(pair.Value);
		}
		list.Sort(ComparerBase);
		
		m_BaseTypeDic.Clear();
		foreach(SkillView skillView in list)
		{
			m_BaseTypeDic.Add(skillView.nSkillTableIdx, skillView);
//			if(skillView.record_ != null)
//				Debug.Log("skill shop:: sorted base skill's class lv:" + skillView.record_.ClassLevel);
		}
		
		List<Tbl_SkillBook_Record> list2 = new List<Tbl_SkillBook_Record>();
		foreach(KeyValuePair<int, Tbl_SkillBook_Record> pair in m_ChoiceTypeDic)
		{
			list2.Add(pair.Value);
		}
		list2.Sort(ComparerChoice);
		
		m_ChoiceTypeDic.Clear();
		foreach(Tbl_SkillBook_Record record in list2)
		{
			m_ChoiceTypeDic.Add(record.Index, record);
			
//			Debug.Log("skill shop:: sorted choice skill's class lv:" + record.ClassLevel);
		}
	}
	
	int ComparerBase(SkillView _left, SkillView _right)
	{
		if(_left.getSkillRecord == null || _right.getSkillRecord == null)
		{
			Debug.LogWarning("skill book record is null(left:" + _left.nSkillTableIdx + ", right:" + _right.nSkillTableIdx);
			return -1;
		}
		
		if(_left.getSkillRecord.ClassLevel > _right.getSkillRecord.ClassLevel)
			return 1;
		else if(_left.getSkillRecord.ClassLevel == _right.getSkillRecord.ClassLevel)
			return 0;
		else
			return -1;
	}
	
	int ComparerChoice(Tbl_SkillBook_Record _left, Tbl_SkillBook_Record _right)
	{
		
		if(_left.ClassLevel > _right.ClassLevel)
			return 1;
		else if(_left.ClassLevel == _right.ClassLevel)
			return 0;
		else
			return -1;
	}
	#endregion
	#region - public -
	public void PurchaseSkill(int _skillidx, int _skillLv)
	{
		body_CS_SKILL_LEARN learn = new body_CS_SKILL_LEARN();
		learn.nNpcIdx = m_CurNpcIdx;
		
		if(m_mdicSkillBookBySkillIdx.ContainsKey(_skillidx) == true)
		{
			bool finished = false;
			foreach(Tbl_SkillBook_Record record in m_mdicSkillBookBySkillIdx[_skillidx])
			{
				if(record.Skill1_Level == _skillLv)
				{
					learn.nSkillBookIdx = record.Index;
					finished = true;
				}
				
				if(record.ChoiceType == eChoiceType.Choice)
				{
					if(record.Skill2_Level == _skillLv)
					{
						learn.nSkillBookIdx = record.Index;
						finished = true;
					}
				}
				
				if(finished == true)
					break;
			}
		}
		else
			Debug.LogError("no skill book record is found.");
		
//		learn.nSkillBookIdx = 2;
		learn.sSkill = new sSKILL( _skillidx, _skillLv);
		AsCommonSender.Send( learn.ClassToPacketBytes() );
	}
	#endregion
}