using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuffElement
{
	static int s_Index = 0;
	int m_CurId = 0; public int CurId{get{return m_CurId;}}
	
	AsBaseEntity m_Owner;
	BuffProcessor m_Processor;
	
	Msg_CharBuff_Body m_Body; public Msg_CharBuff_Body Body{get{return m_Body;}}
	
	eBUFFTYPE m_BuffType;public eBUFFTYPE BuffType{get{return m_BuffType;}}
	Tbl_SkillPotencyEffect_Record m_Record = null;
	
	bool m_Activated = true;
	
	int m_SkillIdx;public int skillIdx{get{return m_SkillIdx;}}
	int m_PotencyIdx;public int PotencyIdx{get{return m_PotencyIdx;}}
	
	string m_SkillName = null;
	Tbl_SkillLevel_Potency m_SkillLevelPotency = null; public Tbl_SkillLevel_Potency SkillLevelPotency{get{return m_SkillLevelPotency;}}
	eSkillNamePrint m_SkillNamePrint = eSkillNamePrint.None;
	
	// calculated//
	int m_BuffIdx;
	AudioSource m_BuffSound;

	bool m_GenerattionMessageSend = false;
	
	// benefit
	bool m_Benefit = false; public bool Benefit{get{return m_Benefit;}} // for checkingg auto chat condition
	
	public BuffElement(){}
	
	public BuffElement(Msg_CharBuff_Body _body, BuffProcessor _owner, bool _effect)
	{
		m_CurId = s_Index++;
		
		m_Owner = _owner.Owner;
		m_Processor = _owner;
		
		m_Body = _body;
		
		m_BuffType = _body.type_;
		m_Activated = _effect;
		
		int effectIdx = int.MaxValue;	
		if(BuffBaseMgr.s_MonsterSkillIndexRange_Min <= _body.skillTableIdx_ && _body.skillTableIdx_ <= BuffBaseMgr.s_MonsterSkillIndexRange_Max)
		{
			#region - set skill name -
			Tbl_MonsterSkill_Record skillRecord = AsTableManager.Instance.GetTbl_MonsterSkill_Record(_body.skillTableIdx_);
			SetSkillData(skillRecord);
			#endregion
			
//			Tbl_MonsterSkillLevel_Record skill = AsTableManager.Instance.GetTbl_MonsterSkillLevel_Record(_body.skillLevel_, _body.skillTableIdx_);
			Tbl_MonsterSkillLevel_Record skill = AsTableManager.Instance.GetTbl_MonsterSkillLevel_Record(_body.skillLevelTableIdx_);
			
			try{
				effectIdx = skill.listSkillLevelPotency[_body.potencyIdx_].Potency_EffectIndex;
				m_SkillLevelPotency = skill.listSkillLevelPotency[_body.potencyIdx_];
			}
			catch(System.Exception e)
			{
				Debug.LogError(e);
			}
		}
		else
		{
			#region - set skill name -
			Tbl_Skill_Record skillRecord = AsTableManager.Instance.GetTbl_Skill_Record(_body.skillTableIdx_);
			SetSkillData(skillRecord);
			#endregion
			
			Tbl_SkillLevel_Record skill = null;
			
			if(_body.chargeStep_ == int.MaxValue)
			{
				skill = AsTableManager.Instance.GetTbl_SkillLevel_Record(_body.skillLevel_, _body.skillTableIdx_);
			}
			else
			{
				skill = AsTableManager.Instance.GetTbl_SkillLevel_Record(_body.skillLevel_, _body.skillTableIdx_, _body.chargeStep_);
			}
						
			effectIdx = skill.listSkillLevelPotency[_body.potencyIdx_].Potency_EffectIndex;
			m_SkillLevelPotency = skill.listSkillLevelPotency[_body.potencyIdx_];
		}
		
		m_SkillIdx = _body.skillTableIdx_;
		m_PotencyIdx = _body.potencyIdx_;
		m_Record = AsTableManager.Instance.GetTbl_SkillPotencyEffect_Record(effectIdx);
	}
	
	public BuffElement(Msg_NpcBuff_Body _body, BuffProcessor _owner, bool _effect)
	{
		m_CurId = s_Index++;
		
		m_Owner = _owner.Owner;
		m_Processor = _owner;
		
		m_BuffType = _body.type_;
		m_Activated = _effect;
		
		int effectIdx = int.MaxValue;	
		if(BuffBaseMgr.s_MonsterSkillIndexRange_Min <= _body.skillTableIdx_ && _body.skillTableIdx_ <= BuffBaseMgr.s_MonsterSkillIndexRange_Max)
		{
			#region - set skill name -
			Tbl_MonsterSkill_Record skillRecord = AsTableManager.Instance.GetTbl_MonsterSkill_Record(_body.skillTableIdx_);
			SetSkillData(skillRecord);
			#endregion
			
//			Tbl_MonsterSkillLevel_Record skill = AsTableManager.Instance.GetTbl_MonsterSkillLevel_Record(_body.skillLevel_, _body.skillTableIdx_);
			Tbl_MonsterSkillLevel_Record skill = AsTableManager.Instance.GetTbl_MonsterSkillLevel_Record(_body.skillLevelTableIdx_);
			if( null == skill )
			{
				AsUtil.ShutDown ( "BuffProcessor::BuffElement()[ (GetTbl_MonsterSkillLevel_Record) null == skill ] skill id : " + _body.skillTableIdx_ + " skill level : " + _body.skillLevel_ );
				return;
			}
			effectIdx = skill.listSkillLevelPotency[_body.potencyIdx_].Potency_EffectIndex;
			m_SkillLevelPotency = skill.listSkillLevelPotency[_body.potencyIdx_];
		}
		else
		{
			#region - set skill name -
			Tbl_Skill_Record skillRecord = AsTableManager.Instance.GetTbl_Skill_Record(_body.skillTableIdx_);
			SetSkillData(skillRecord);
			#endregion
			
			Tbl_SkillLevel_Record skill = null;
			
			if(_body.chargeStep_ == int.MaxValue)
			{
				skill = AsTableManager.Instance.GetTbl_SkillLevel_Record(_body.skillLevel_, _body.skillTableIdx_);
			}
			else
			{
				skill = AsTableManager.Instance.GetTbl_SkillLevel_Record(_body.skillLevel_, _body.skillTableIdx_, _body.chargeStep_);
			}
			
			if( null == skill )
			{
				AsUtil.ShutDown ( "BuffProcessor::BuffElement()[ (GetTbl_SkillLevel_Record) null == skill ] skill id : " + _body.skillTableIdx_ + " skill level : " + _body.skillLevel_ );
				return;
			}
			effectIdx = skill.listSkillLevelPotency[_body.potencyIdx_].Potency_EffectIndex;
			m_SkillLevelPotency = skill.listSkillLevelPotency[_body.potencyIdx_];
		}
		
		m_SkillIdx = _body.skillTableIdx_;
		m_PotencyIdx = _body.potencyIdx_;
		m_Record = AsTableManager.Instance.GetTbl_SkillPotencyEffect_Record(effectIdx);
	}
	
	public BuffElement(body2_SC_CHAR_BUFF _body, BuffProcessor _owner, bool _effect)
	{
		m_CurId = s_Index++;
		
		m_Owner = _owner.Owner;
		m_Processor = _owner;
		
		m_BuffType = _body.eType;
		m_Activated = _effect;
		
		int effectIdx = int.MaxValue;		
		if(BuffBaseMgr.s_MonsterSkillIndexRange_Min <= _body.nSkillTableIdx && _body.nSkillTableIdx <= BuffBaseMgr.s_MonsterSkillIndexRange_Max)
		{
			#region - set skill name -
			Tbl_MonsterSkill_Record skillRecord = AsTableManager.Instance.GetTbl_MonsterSkill_Record(_body.nSkillTableIdx);
			SetSkillData(skillRecord);
			#endregion
			
//			Tbl_MonsterSkillLevel_Record skill = AsTableManager.Instance.GetTbl_MonsterSkillLevel_Record(_body.nSkillLevel, _body.nSkillTableIdx);
			Tbl_MonsterSkillLevel_Record skill = AsTableManager.Instance.GetTbl_MonsterSkillLevel_Record(_body.nSkillLevelTableIdx);
			effectIdx = skill.listSkillLevelPotency[_body.nPotencyIdx].Potency_EffectIndex;
			m_SkillLevelPotency = skill.listSkillLevelPotency[_body.nPotencyIdx];
		}
		else
		{
			#region - set skill name -
			Tbl_Skill_Record skillRecord = AsTableManager.Instance.GetTbl_Skill_Record(_body.nSkillTableIdx);
			SetSkillData(skillRecord);
			#endregion
			
			Tbl_SkillLevel_Record skill = null;
			
			if(_body.nChargeStep == int.MaxValue)
			{
				skill = AsTableManager.Instance.GetTbl_SkillLevel_Record(_body.nSkillLevel, _body.nSkillTableIdx);
			}
			else
			{
				skill = AsTableManager.Instance.GetTbl_SkillLevel_Record(_body.nSkillLevel, _body.nSkillTableIdx, _body.nChargeStep);
			}
			
//			Tbl_SkillLevel_Record skill = AsTableManager.Instance.GetTbl_SkillLevel_Record(_body.nSkillLevel, _body.nSkillTableIdx);
			effectIdx = skill.listSkillLevelPotency[_body.nPotencyIdx].Potency_EffectIndex;
			m_SkillLevelPotency = skill.listSkillLevelPotency[_body.nPotencyIdx];
		}
		
		m_SkillIdx = _body.nSkillTableIdx;
		m_PotencyIdx = _body.nPotencyIdx;
		m_Record = AsTableManager.Instance.GetTbl_SkillPotencyEffect_Record(effectIdx);
	}
	
	void SetSkillData(Tbl_Skill_Record _record)
	{
		if(_record != null && _record.SkillName_Index != int.MaxValue)
		{
			m_SkillName = AsTableManager.Instance.GetTbl_String(_record.SkillName_Index);
			m_SkillNamePrint = _record.SkillName_Print;
		}
		else
			Debug.LogWarning("BuffElement::SetSkillData: there is no [" + _record.Index + "] skill record's skill name.");
	}
	
	void SetSkillData(Tbl_MonsterSkill_Record _record)
	{
		if(_record != null && _record.SkillName_Index != int.MaxValue)
		{
			m_SkillName = AsTableManager.Instance.GetTbl_String(_record.SkillName_Index);
			m_SkillNamePrint = _record.SkillName_Print;
		}
		else
		{
//			Debug.LogWarning("BuffElement::SetSkillData: there is no [" + _record.Index + "] skill record's skill name.");
		}
	}
	
	public void Generate()
	{
		#region - skill name shout -
		if(m_Activated == true &&
//			m_Record == eEFFECT_TIMING.Moment &&
//			m_Record.PotencyEffect_Timing == eEFFECT_TIMING.Moment &&
			m_SkillName != null)
		{
			if(m_SkillNamePrint == eSkillNamePrint.Negative)
			{
				m_Owner.SkillNameShout(eSkillNameShoutType.Harm, m_SkillName);
			}
			else
			{
				
				Tbl_BuffOverlap_Record overlap = AsTableManager.Instance.GetTbl_BuffOverlap_Record(m_BuffType);
				switch(overlap.BuffOverlapType)
				{
				case eBuffOverlapType.Buff:
					m_Owner.SkillNameShout(eSkillNameShoutType.Benefit, m_SkillName);
					m_Benefit = true;
					break;
				case eBuffOverlapType.Debuff:
					m_Owner.SkillNameShout(eSkillNameShoutType.Harm, m_SkillName);
					break;
				case eBuffOverlapType.Positive:
					if(m_SkillLevelPotency.CheckValuePositive() == true)
					{
						m_Owner.SkillNameShout(eSkillNameShoutType.Benefit, m_SkillName);
						m_Benefit = true;
					}
					else
						m_Owner.SkillNameShout(eSkillNameShoutType.Harm, m_SkillName);
					break;
				case eBuffOverlapType.Negative:
					if(m_SkillLevelPotency.CheckValuePositive() == false)
					{
						m_Owner.SkillNameShout(eSkillNameShoutType.Benefit, m_SkillName);
						m_Benefit = true;
					}
					else
						m_Owner.SkillNameShout(eSkillNameShoutType.Harm, m_SkillName);
					break;
				default:
					break;
				}
				
				#region - condition -
				if(m_Owner.FsmType == eFsmType.PLAYER && m_Benefit == true)
				{
					AsEmotionManager.Instance.IncreaseBenefitBuffCount();
					
					if(m_Body.charUniqKey_ != AsEntityManager.Instance.UserEntity.UniqueId && m_Body.charUniqKey_ != 0)
						AsEmotionManager.Instance.Event_Condition_GetBuff();
				}
				#endregion
			}
		}
		#endregion
		#region - msg to entity -
		Generation_Message();
		#endregion
		#region - effect -
		Generation_Effect();
		#endregion
		
		AsHUDController.Instance.panelManager.ShowBuffPanel( m_Owner.gameObject, m_BuffType);
	}

	public void Generation_Message()
	{
		if(m_GenerattionMessageSend == false)
			m_GenerattionMessageSend = true;
		else
			return;

		switch(m_BuffType)
		{
		case eBUFFTYPE.eBUFFTYPE_STUN_NOTHING:
			m_Owner.HandleMessage(new Msg_ConditionIndicate_Stun());
			Debug.Log("BuffElement::constructor: [" + m_Owner.name + "][buff:" + m_BuffType + "]");
			break;
		case eBUFFTYPE.eBUFFTYPE_FREEZE_PROB:
			m_Owner.HandleMessage(new Msg_ConditionIndicate_Freeze());
			Debug.Log("BuffElement::constructor: [" + m_Owner.name + "][buff:" + m_BuffType + "]");
			break;
		case eBUFFTYPE.eBUFFTYPE_SLEEP_PROB:
			m_Owner.HandleMessage(new Msg_ConditionIndicate_Sleep());
			Debug.Log("BuffElement::constructor: [" + m_Owner.name + "][buff:" + m_BuffType + "]");
			break;
		case eBUFFTYPE.eBUFFTYPE_BURNING_ADDPROB:
			m_Owner.HandleMessage(new Msg_ConditionIndicate_Burning());
			Debug.Log("BuffElement::constructor: [" + m_Owner.name + "][buff:" + m_BuffType + "]");
			break;
		case eBUFFTYPE.eBUFFTYPE_BLEEDING_ADDPROB:
			//
			Debug.Log("BuffElement::constructor: [" + m_Owner.name + "][buff:" + m_BuffType + "]");
			break;
		case eBUFFTYPE.eBUFFTYPE_SHIELD_DEFENSE_ADDPROB:
			//
			Debug.Log("BuffElement::constructor: [" + m_Owner.name + "][buff:" + m_BuffType + "]");
			break;
		case eBUFFTYPE.eBUFFTYPE_POISON_ADDPROB:
			//
			Debug.Log("BuffElement::constructor: [" + m_Owner.name + "][buff:" + m_BuffType + "]");
			break;
		case eBUFFTYPE.eBUFFTYPE_FEAR_NOTHING:
			m_Owner.HandleMessage(new Msg_ConditionIndicate_Fear());
			Debug.Log("BuffElement::constructor: [" + m_Owner.name + "][buff:" + m_BuffType + "]");
			break;
		case eBUFFTYPE.eBUFFTYPE_BIND_NOTHING:
			m_Owner.HandleMessage(new Msg_ConditionIndicate_Bind());
			Debug.Log("BuffElement::constructor: [" + m_Owner.name + "][buff:" + m_BuffType + "]");
			break;
		case eBUFFTYPE.eBUFFTYPE_BLIND_PROB:
			//
			Debug.Log("BuffElement::constructor: [" + m_Owner.name + "][buff:" + m_BuffType + "]");
			break;
		case eBUFFTYPE.eBUFFTYPE_SIZE_CONTROL_PROB:
			//			m_Owner.HandleMessage(new Msg_ConditionIndicate_SizeControl(m_SkillLevelPotency));
			m_Processor.Generate_SizeControl(this);
			break;
		case eBUFFTYPE.eBUFFTYPE_BLANK_NOTHING:
			m_Owner.HandleMessage(new Msg_ConditionIndicate_Blank());
			Debug.Log("BuffElement::constructor: [" + m_Owner.name + "][buff:" + m_BuffType + "]");
			break;
		case eBUFFTYPE.eBUFFTYPE_AIRBON_NOTHING:
			m_Owner.HandleMessage(new Msg_ConditionIndicate_AirBone( m_Body));
			Debug.Log("BuffElement::constructor: [" + m_Owner.name + "][buff:" + m_BuffType + "]");
			break;
		case eBUFFTYPE.eBUFFTYPE_TRANSFORM_ADD:
			m_Owner.HandleMessage(new Msg_ConditionIndicate_Transform((int)m_SkillLevelPotency.Potency_IntValue));
			Debug.Log("BuffElement::constructor: [" + m_Owner.name + "][buff:" + m_BuffType + "]");
			break;
		case eBUFFTYPE.eBUFFTYPE_GRAY_NOTHING:
			m_Owner.HandleMessage(new Msg_ConditionIndicate_Gray());
			Debug.Log("BuffElement::constructor: [" + m_Owner.name + "][buff:" + m_BuffType + "]");
			break;
		default:
			//			Debug.Log("BuffElement::constructor(npc): [" + m_Owner.name + "][buff:" + m_BuffType + "] not processed.");
			break;
		}
	}

	public void Generation_Effect()
	{
		if(m_Record == null)
		{
//			Debug.LogWarning("BuffProcessor::Generate: m_Record is null. m_SkillIdx = " + m_SkillIdx + ", m_PotencyIdx = " + m_PotencyIdx);
			return;
		}
		
		if(m_Owner.ModelObject == null)
		{
			Debug.LogWarning("BuffProcessor::Generate: m_Owner.ModelObject is null.");
			return;
		}
		
		if(m_Activated == false && m_Record.PotencyEffect_Timing == eEFFECT_TIMING.Moment)
		{
		}
		else
		{
			m_Activated = false;
			
			bool loop = false;
			if(m_Record.PotencyEffect_Timing == eEFFECT_TIMING.Duration)
				loop = true;
			//dopamin70 #19383
			float size = 1;
			if(m_Record.Effect_Size != float.MaxValue)
			{
				if(m_Owner.EntityType == eEntityType.USER)
					size = m_Record.Effect_Size * 0.001f *  m_Owner.transform.localScale.y;
				else
					size = m_Record.Effect_Size * 0.001f * m_Owner.transform.localScale.y * m_Owner.characterController.height;
			}
			
			float maxSize = AsTableManager.Instance.GetTbl_GlobalWeight_Record("PotencyEffectMaxSize").Value * 0.001f;
			if(size > maxSize)
				size = maxSize;
			
			m_BuffIdx = AsEffectManager.Instance.PlayEffect(m_Record.PotencyEffect_FileName_Full,
				m_Owner.ModelObject.transform, loop,0,  size );
			m_BuffSound = AsSoundManager.Instance.PlaySound(m_Record.PotencySound_FileName,
				m_Owner.ModelObject.transform.position, loop);
		}
	}
	
	public void Release_Effect()
	{
		AsEffectManager.Instance.RemoveEffectEntity(m_BuffIdx);
		AsSoundManager.Instance.StopSound(m_BuffSound);
	}
	
	public void Release()
	{
		AsEffectManager.Instance.RemoveEffectEntity(m_BuffIdx);
		AsSoundManager.Instance.StopSound(m_BuffSound);
		
		switch(m_BuffType)
		{
		case eBUFFTYPE.eBUFFTYPE_STUN_NOTHING:
			m_Owner.HandleMessage(new Msg_ConditionRecover_Stun());
			break;
		case eBUFFTYPE.eBUFFTYPE_FREEZE_PROB:
			m_Owner.HandleMessage(new Msg_ConditionRecover_Freeze());
			break;
		case eBUFFTYPE.eBUFFTYPE_SLEEP_PROB:
			m_Owner.HandleMessage(new Msg_ConditionRecover_Sleep());
			break;
		case eBUFFTYPE.eBUFFTYPE_BURNING_ADDPROB:
			m_Owner.HandleMessage(new Msg_ConditionRecover_Burning());
			break;
		case eBUFFTYPE.eBUFFTYPE_BLEEDING_ADDPROB:
			break;
		case eBUFFTYPE.eBUFFTYPE_SHIELD_DEFENSE_ADDPROB:
			break;
		case eBUFFTYPE.eBUFFTYPE_POISON_ADDPROB:
			break;
		case eBUFFTYPE.eBUFFTYPE_FEAR_NOTHING:
			m_Owner.HandleMessage(new Msg_ConditionRecover_Fear());
			break;
		case eBUFFTYPE.eBUFFTYPE_BIND_NOTHING:
			m_Owner.HandleMessage(new Msg_ConditionRecover_Bind());
			break;
		case eBUFFTYPE.eBUFFTYPE_BLIND_PROB:
			break;
		case eBUFFTYPE.eBUFFTYPE_SIZE_CONTROL_PROB:
//			m_Owner.HandleMessage(new Msg_ConditionRecover_SizeControl(m_SkillLevelPotency));
			m_Processor.Release_SizeControl(this);
			break;
		case eBUFFTYPE.eBUFFTYPE_BLANK_NOTHING:
			m_Owner.HandleMessage(new Msg_ConditionRecover_Blank());
			break;
		case eBUFFTYPE.eBUFFTYPE_AIRBON_NOTHING:
			m_Owner.HandleMessage(new Msg_ConditionRecover_AirBone());
			break;
		case eBUFFTYPE.eBUFFTYPE_TRANSFORM_ADD:
			m_Owner.HandleMessage(new Msg_ConditionRecover_Transform());
			break;
		case eBUFFTYPE.eBUFFTYPE_GRAY_NOTHING:
			m_Owner.HandleMessage(new Msg_ConditionRecover_Gray());
			break;
		default:
			break;
		}
		
		#region - condition -
		if(m_Owner.FsmType == eFsmType.PLAYER && m_Benefit == true)
		{
			AsEmotionManager.Instance.DecreaseBenefitBuffCount();
		}
		#endregion
	}
	
//	public bool CheckBuff_Benefit()
//	{
//		Tbl_BuffOverlap_Record overlap = AsTableManager.Instance.GetTbl_BuffOverlap_Record(m_BuffType);
//		if(overlap != null)
//		{
//			if(overlap.BuffOverlapType == eBuffOverlapType.Buff)
//				return true;
//			else
//				return false;
//		}
//		else
//		{
//			Debug.LogError("BuffProcessor::CheckBuff_Benefit: no BuffOverlap record is found");
//			return false;
//		}
//	}
}

public class BuffProcessor
{
	// < skill record index, potency index >
	Dictionary<int, Dictionary<int, BuffElement>> m_dicBuff = new Dictionary<int, Dictionary<int, BuffElement>>();
	MultiDictionary<eBUFFTYPE, BuffElement> m_mdicBuff = new MultiDictionary<eBUFFTYPE, BuffElement>();
	
	bool m_ModelLoaded = false;
//	List<BuffElement> m_listDelayed = new List<BuffElement>();
	
	// resurrectoin penalty
	bool m_PenaltyDisplayed = false;
	
	int m_BenefitBuffCount = 0; public int BeneffitBuffCount{get{return m_BenefitBuffCount;}}
	
	AsBaseEntity m_Owner;public AsBaseEntity Owner{get{return m_Owner;}}
	
	float m_CurSize = 1f; public float CurSize{get{return m_CurSize;}}
	
	public bool CheckBuffInclusion(int _skillIdx)
	{
		return m_dicBuff.ContainsKey(_skillIdx);
	}
	
	public bool GetEnableCondition( eSkillIcon_Enable_Condition _condition)
	{
		switch( _condition)
		{
		case eSkillIcon_Enable_Condition.Movable:
			if( ( true == m_mdicBuff.ContainsKey( eBUFFTYPE.eBUFFTYPE_STUN_NOTHING))
				|| ( true == m_mdicBuff.ContainsKey( eBUFFTYPE.eBUFFTYPE_BIND_NOTHING))
				|| ( true == m_mdicBuff.ContainsKey( eBUFFTYPE.eBUFFTYPE_FREEZE_PROB))
				|| ( true == m_mdicBuff.ContainsKey( eBUFFTYPE.eBUFFTYPE_SLEEP_PROB))
				|| ( true == m_mdicBuff.ContainsKey( eBUFFTYPE.eBUFFTYPE_FEAR_NOTHING)))
				return false;
			else
				return true;
		case eSkillIcon_Enable_Condition.Burning:
			return m_mdicBuff.ContainsKey( eBUFFTYPE.eBUFFTYPE_BURNING_ADDPROB) || m_mdicBuff.ContainsKey( eBUFFTYPE.eBUFFTYPE_BURNING_ADDPROB);
		case eSkillIcon_Enable_Condition.Stun:
			return m_mdicBuff.ContainsKey( eBUFFTYPE.eBUFFTYPE_STUN_NOTHING);
		case eSkillIcon_Enable_Condition.Freeze:
			return m_mdicBuff.ContainsKey( eBUFFTYPE.eBUFFTYPE_FREEZE_PROB);
		case eSkillIcon_Enable_Condition.ShieldDefense:
			return m_mdicBuff.ContainsKey( eBUFFTYPE.eBUFFTYPE_SHIELD_DEFENSE_ADDPROB);
		case eSkillIcon_Enable_Condition.Bleeding:
			return m_mdicBuff.ContainsKey( eBUFFTYPE.eBUFFTYPE_BLEEDING_ADDPROB);
		case eSkillIcon_Enable_Condition.LowHealth:
//			float hp = m_Owner.GetProperty<float>(eComponentProperty.HP_CUR); // to do
			return false;
		case eSkillIcon_Enable_Condition.Poison:
			return m_mdicBuff.ContainsKey( eBUFFTYPE.eBUFFTYPE_POISON_ADDPROB);
		case eSkillIcon_Enable_Condition.Bind:
			return m_mdicBuff.ContainsKey( eBUFFTYPE.eBUFFTYPE_BIND_NOTHING);
		case eSkillIcon_Enable_Condition.Blind:
			return m_mdicBuff.ContainsKey( eBUFFTYPE.eBUFFTYPE_BLIND_PROB);
		case eSkillIcon_Enable_Condition.Sleep:
			return m_mdicBuff.ContainsKey( eBUFFTYPE.eBUFFTYPE_SLEEP_PROB);
		case eSkillIcon_Enable_Condition.Fear:
			return m_mdicBuff.ContainsKey( eBUFFTYPE.eBUFFTYPE_FEAR_NOTHING);
		default:
			return false;
//			return m_mdicBuff.ContainsKey( AsEnumConverter.GetBuffTypeFromCondition( _condition));
		}
	}

	public BuffProcessor(AsBaseEntity _entity)
	{
		m_Owner = _entity;
	}
	
	public void ModelLoaded()
	{
		m_ModelLoaded = true;
		
		RefreshBuff();
		
//		foreach(BuffElement element in m_listDelayed)
//		{
//			if(m_dicBuff.ContainsKey(element.skillIdx) == false)
//				m_dicBuff.Add(element.skillIdx, new Dictionary<int, BuffElement>());
//			
//			if(m_dicBuff[element.skillIdx].ContainsKey(element.PotencyIdx) == true)
//			{
//				m_dicBuff[element.skillIdx][element.PotencyIdx].Release();
//				m_dicBuff[element.skillIdx].Remove(element.PotencyIdx);
//			}
//			
//			m_dicBuff[element.skillIdx].Add(element.PotencyIdx, element);
//			m_mdicBuff.Add(element.BuffType, element);
//			
//			element.Generate();
//		}
//		
//		m_listDelayed.Clear();
	}
	
	public void PlayerModelLoaded()
	{
	}
	
	public void AddBuff(Msg_CharBuff_Body _body)
	{
		#region - init dictionary -
		if(m_dicBuff.ContainsKey(_body.skillTableIdx_) == false)
			m_dicBuff.Add(_body.skillTableIdx_, new Dictionary<int, BuffElement>());
		
		if(m_dicBuff[_body.skillTableIdx_].ContainsKey(_body.potencyIdx_) == true)
		{
			m_dicBuff[_body.skillTableIdx_][_body.potencyIdx_].Release();
			m_dicBuff[_body.skillTableIdx_].Remove(_body.potencyIdx_);
		}
		#endregion
		
		BuffElement buff = new BuffElement(_body, this, false);
		
		m_dicBuff[_body.skillTableIdx_].Add(_body.potencyIdx_, buff);
		m_mdicBuff.Add(_body.type_, buff);
		
		if( m_ModelLoaded == true)
			buff.Generate();
		
		ResurrectionPenaltyProc( buff);
	}
	
	public void AddBuff(Msg_CharBuff_Body _body, bool _effect)
	{
		#region - init dictionary -
		if(m_dicBuff.ContainsKey(_body.skillTableIdx_) == false)
			m_dicBuff.Add(_body.skillTableIdx_, new Dictionary<int, BuffElement>());
		
		if(m_dicBuff[_body.skillTableIdx_].ContainsKey(_body.potencyIdx_) == true)
		{
			m_dicBuff[_body.skillTableIdx_][_body.potencyIdx_].Release();
			m_dicBuff[_body.skillTableIdx_].Remove(_body.potencyIdx_);
		}
		#endregion
		
		BuffElement buff = new BuffElement(_body, this, _effect);
		
		m_dicBuff[_body.skillTableIdx_].Add(_body.potencyIdx_, buff);
		m_mdicBuff.Add(_body.type_, buff);
		
		if( m_ModelLoaded == true)
			buff.Generate();
		
		
		ResurrectionPenaltyProc( buff);
	}
	
	public void AddBuff(body2_SC_CHAR_BUFF _buff)
	{
		#region - init dictionary -
		if(m_dicBuff.ContainsKey(_buff.nSkillTableIdx) == false)
			m_dicBuff.Add(_buff.nSkillTableIdx, new Dictionary<int, BuffElement>());
		
		if(m_dicBuff[_buff.nSkillTableIdx].ContainsKey(_buff.nPotencyIdx) == true)
		{
			m_dicBuff[_buff.nSkillTableIdx][_buff.nPotencyIdx].Release();
			m_dicBuff[_buff.nSkillTableIdx].Remove(_buff.nPotencyIdx);
		}
		#endregion
		
		BuffElement buff = new BuffElement(_buff, this, false);
		
		m_dicBuff[_buff.nSkillTableIdx].Add(_buff.nPotencyIdx, buff);
		m_mdicBuff.Add(_buff.eType, buff);
		
		if(m_ModelLoaded == true)
			buff.Generate();
		
		ResurrectionPenaltyProc( buff);
	}
	
	public void RemoveBuff(Msg_CharDeBuff _body)
	{
		if(m_dicBuff.ContainsKey(_body.skillTableIdx_) == true &&
			m_dicBuff[_body.skillTableIdx_].ContainsKey(_body.potencyIdx_) == true)
		{
			BuffElement element = m_dicBuff[_body.skillTableIdx_][_body.potencyIdx_];
			element.Release();
			ReleaseResurrectionPenalty( element);
			
			m_dicBuff[_body.skillTableIdx_].Remove(_body.potencyIdx_);
			
			if(m_dicBuff[_body.skillTableIdx_].Count == 0)
				m_dicBuff.Remove(_body.skillTableIdx_);
			
			foreach(eBUFFTYPE key in m_mdicBuff.Keys)
			{
				foreach(BuffElement node in m_mdicBuff[key])
				{
					if(element.Equals(node) == true)
					{
						m_mdicBuff.Remove(key, node);												
						return;
					}
				}
			}
		}
	}
	
	public void AddBuff(Msg_NpcBuff_Body _buff, bool _effect)
	{
		#region - init dictionary -
		if(m_dicBuff.ContainsKey(_buff.skillTableIdx_) == false)
			m_dicBuff.Add(_buff.skillTableIdx_, new Dictionary<int, BuffElement>());
		
		if(m_dicBuff[_buff.skillTableIdx_].ContainsKey(_buff.potencyIdx_) == true)
		{
			m_dicBuff[_buff.skillTableIdx_][_buff.potencyIdx_].Release();
			m_dicBuff[_buff.skillTableIdx_].Remove(_buff.potencyIdx_);
		}
		#endregion
		
		BuffElement buff = new BuffElement(_buff, this, _effect);
		
		m_dicBuff[_buff.skillTableIdx_].Add(_buff.potencyIdx_, buff);
		m_mdicBuff.Add(_buff.type_, buff);
		
		if(m_ModelLoaded == true)
			buff.Generate();
		
		ResurrectionPenaltyProc( buff);
	}
	
	public void RemoveBuff(Msg_NpcDeBuff _deBuff)
	{
		if(m_dicBuff.ContainsKey(_deBuff.skillTableIdx_) == true &&
			m_dicBuff[_deBuff.skillTableIdx_].ContainsKey(_deBuff.potencyIdx_) == true)
		{
			BuffElement element = m_dicBuff[_deBuff.skillTableIdx_][_deBuff.potencyIdx_];
			element.Release();
			ReleaseResurrectionPenalty( element);
			
			m_dicBuff[_deBuff.skillTableIdx_].Remove(_deBuff.potencyIdx_);
			
			foreach(eBUFFTYPE key in m_mdicBuff.Keys)
			{
				foreach(BuffElement node in m_mdicBuff[key])
				{
					if(element.Equals(node) == true)
					{
						m_mdicBuff.Remove(key, node);
						return;
					}
				}
			}
		}
		
//		List<BuffElement> listRemove = new List<BuffElement>();
//		foreach(BuffElement element in m_listDelayed)
//		{
//			if(_deBuff.skillTableIdx_ == element.skillIdx &&
//				_deBuff.potencyIdx_ == element.PotencyIdx)
//			{
//				listRemove.Add(element);
//			}
//		}
//		
//		foreach(BuffElement element in listRemove)
//		{
//			m_listDelayed.Remove(element);
//		}
	}
	
	public void RefreshBuff()
	{
		foreach(KeyValuePair<int, Dictionary<int, BuffElement>> pair1 in m_dicBuff)
		{
			foreach(KeyValuePair<int, BuffElement> pair2 in pair1.Value)
			{
				pair2.Value.Release_Effect();
				pair2.Value.Generation_Message();
				pair2.Value.Generation_Effect();
			}
		}
	}
	
	public void Release()
	{
		foreach(KeyValuePair<int, Dictionary<int, BuffElement>> pair1 in m_dicBuff)
		{
			foreach(KeyValuePair<int, BuffElement> pair2 in pair1.Value)
			{
				pair2.Value.Release();
			}
		}
		
		m_dicBuff.Clear();
		m_mdicBuff.Clear();
		
		m_PenaltyDisplayed = false;
	}
	
	List<BuffElement> m_listSizeControl_Buff = new List<BuffElement>();
	List<BuffElement> m_listSizeControl_Debuff = new List<BuffElement>();
	
	#region - size control -
	public void Generate_SizeControl(BuffElement _buff)
	{
		AddSizeControlBuff(_buff);
		SortSizeControl();
		DispatchSizeControl();
		
//		if(m_listSizeControl_Buff.Count != 0)
//			m_Owner.HandleMessage(new Msg_ConditionIndicate_SizeControl(m_listSizeControl_Buff[0].SkillLevelPotency));
	}
	
	public void Release_SizeControl(BuffElement _buff)
	{
		RemoveSizeControlBuff(_buff);
		SortSizeControl();
		DispatchSizeControl();
		
//		if(m_listSizeControl_Buff.Count != 0)
//			m_Owner.HandleMessage(new Msg_ConditionIndicate_SizeControl(m_listSizeControl_Buff[0].SkillLevelPotency));
//		else
//			m_Owner.HandleMessage(new Msg_ConditionRecover_SizeControl(_buff.SkillLevelPotency));
	}
	
	void AddSizeControlBuff(BuffElement _buff)
	{
		if(_buff.Benefit == true)
			m_listSizeControl_Buff.Add(_buff);
		else
			m_listSizeControl_Debuff.Add(_buff);
	}
	
	void RemoveSizeControlBuff(BuffElement _buff)
	{
		if(_buff.Benefit == true)
			m_listSizeControl_Buff.Remove(_buff);
		else
			m_listSizeControl_Debuff.Remove(_buff);
	}
	
	void SortSizeControl()
	{
		m_listSizeControl_Buff.Sort(delegate(BuffElement _a, BuffElement _b)
		{
			if(_a.Body.skillLevel_ > _b.Body.skillLevel_)
				return -1;
			else if(_a.Body.skillLevel_ < _b.Body.skillLevel_)
				return 1;
			else
			{
				if(_a.CurId > _b.CurId)
					return -1;
				else
					return 1;
			}
			
			return 0;
		}
		);
		
		m_listSizeControl_Debuff.Sort(delegate(BuffElement _a, BuffElement _b)
		{
			if(_a.Body.skillLevel_ > _b.Body.skillLevel_)
				return -1;
			else if(_a.Body.skillLevel_ < _b.Body.skillLevel_)
				return 1;
			else
			{
				if(_a.CurId > _b.CurId)
					return -1;
				else
					return 1;
			}
			
			return 0;
		}
		);
	}
	
	void DispatchSizeControl()
	{
		float buffSize = 0f;
		float debuffSize = 0f;
		
		if(m_listSizeControl_Buff.Count != 0)
		 	buffSize += m_listSizeControl_Buff[0].SkillLevelPotency.Potency_Value * 0.001f;
		if(m_listSizeControl_Debuff.Count != 0)
		 	debuffSize += m_listSizeControl_Debuff[0].SkillLevelPotency.Potency_Value * 0.001f;
		
		float size = 1 + (buffSize + debuffSize);
		m_Owner.HandleMessage(new Msg_ConditionIndicate_SizeControl(size));
	}
	#endregion
	
	#region - resurrection penalry -
	void ResurrectionPenaltyProc( BuffElement _element)
	{
		if( m_Owner.FsmType == eFsmType.PLAYER && m_PenaltyDisplayed == false && _element.skillIdx == 19999)
		{
			string str = AsTableManager.Instance.GetTbl_String(835);
			AsChatManager.Instance.InsertChat( str, eCHATTYPE.eCHATTYPE_SYSTEM);
			AsEventNotifyMgr.Instance.CenterNotify.AddQuestMessage( str, false);
			
			m_PenaltyDisplayed = true;
			
			m_Owner.HandleMessage( new Msg_DeathPenaltyIndicate( true));
		}
	}
	
	void ReleaseResurrectionPenalty(BuffElement _element)
	{
		if( m_Owner.FsmType == eFsmType.PLAYER && _element.skillIdx == 19999)
		{
			m_PenaltyDisplayed = false;
			
			m_Owner.HandleMessage( new Msg_DeathPenaltyIndicate( false));
		}
	}
	#endregion
}

