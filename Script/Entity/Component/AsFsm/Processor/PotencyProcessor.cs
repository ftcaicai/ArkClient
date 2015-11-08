using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PotencyProcessor
{
	public static readonly float s_ForcedMoveDuratioin = 1200f;
	
	AsBaseEntity m_Owner;public AsBaseEntity Owner{get{return m_Owner;}}
	
	//special case
	Msg_OtherCharAttackNpc1 m_OtherCharAttackNpc1 = null;
	
	float m_MaxSize = 1f;

	public PotencyProcessor(AsBaseEntity _entity)
	{
		m_Owner = _entity;
		
		m_MaxSize = AsTableManager.Instance.GetTbl_GlobalWeight_Record("PotencyEffectMaxSize").Value * 0.001f;
	}
	
	#region - process -
	public void PotencyProcess(Msg_NpcAttackChar2 _attack)
	{
		Tbl_MonsterSkill_Record skill = _attack.parent_.skill_;
		
		for(int i=0; i<skill.listSkillPotency.Count; ++i)
		{
			Tbl_Skill_Potency potency = skill.listSkillPotency[i];
			Tbl_SkillLevel_Potency lvPotency = _attack.parent_.skillLv_.listSkillLevelPotency[i];
			
			if(potency.Potency_DurationType == ePotency_DurationType.Moment)
			{
				Tbl_MonsterSkillLevel_Record skillLv = _attack.parent_.skillLv_;
				Tbl_Action_Record action = _attack.parent_.action_;
				
				if(m_Owner.FsmType != eFsmType.MONSTER)
				{
					switch(potency.Potency_Target)
					{
					case ePotency_Target.Enemy:
					case ePotency_Target.All:
					default:
						if(PlayPotency(action.HitAnimation.hitInfo, skillLv.listSkillLevelPotency[i].Potency_EffectIndex, _attack.eDamageType_, lvPotency) == false)
						{
							Debug.LogError("PotencyProcessor: delivered message is Msg_OtherCharAttackNpc2(Skill level table record:[index:" + skillLv.Index +
								"][action index:" + skillLv.SkillAction_Index + "][skill index:" + skillLv.Skill_GroupIndex + "]");
						}
						break;
					}
				}
				
				if(_attack.knockBack_ == true && _attack.parent_.attacker_ != null)
				{
					switch(potency.Potency_Type)
					{
					case ePotency_Type.Move:
						ForcedMoveProcess(lvPotency, _attack.parent_.attacker_.transform.position);
						break;
					case ePotency_Type.PointMove:
						ForcedMoveProcess(lvPotency, _attack.parent_.targetPos_);
						break;
					}
				}
			}
		}
		
		BalloonProcess( _attack.parent_.skill_, _attack.parent_.skillLv_);
	}
	
	public void PotencyProcess(Msg_NpcAttackChar3 _attack)
	{
		Tbl_MonsterSkill_Record skill = _attack.parent_.skill_;
		
		for(int i=0; i<skill.listSkillPotency.Count; ++i)
		{
			Tbl_Skill_Potency potency = skill.listSkillPotency[i];
			Tbl_SkillLevel_Potency lvPotency = _attack.parent_.skillLv_.listSkillLevelPotency[i];
			
			if(potency.Potency_DurationType == ePotency_DurationType.Moment)
			{
				Tbl_MonsterSkillLevel_Record skillLv = _attack.parent_.skillLv_;
				Tbl_Action_Record action = _attack.parent_.action_;
				
				if(m_Owner.FsmType == eFsmType.MONSTER)
				{
					switch(potency.Potency_Target)
					{
					case ePotency_Target.Enemy:
					case ePotency_Target.All:
					default:
						if(PlayPotency(action.HitAnimation.hitInfo, skillLv.listSkillLevelPotency[i].Potency_EffectIndex, lvPotency) == false)
						{
							Debug.LogError("PotencyProcessor: delivered message is Msg_OtherCharAttackNpc2(Skill level table record:[index:" + skillLv.Index +
								"][action index:" + skillLv.SkillAction_Index + "][skill index:" + skillLv.Skill_GroupIndex + "]");
						}
						break;
					}
				}
			}
		}
		
		BalloonProcess( _attack.parent_.skill_, _attack.parent_.skillLv_);
	}
	
	public void PotencyProcess(Msg_OtherCharAttackNpc2 _attack)
	{
		Tbl_Skill_Record skill = _attack.parent_.skill_;
		m_OtherCharAttackNpc1 = _attack.parent_;
		
		for(int i=0; i<skill.listSkillPotency.Count; ++i)
		{
			Tbl_Skill_Potency potency = skill.listSkillPotency[i];
			Tbl_SkillLevel_Potency lvPotency = _attack.parent_.skillLv_.listSkillLevelPotency[i];
			
			if(potency.Potency_DurationType == ePotency_DurationType.Moment)
			{
				Tbl_SkillLevel_Record skillLv = _attack.parent_.skillLv_;
				Tbl_Action_Record action = _attack.parent_.action_;
				
				if(m_Owner.FsmType == eFsmType.MONSTER)
				{
					switch(potency.Potency_Target)
					{
					case ePotency_Target.Enemy:
					case ePotency_Target.All:
						if(PlayPotency(action.HitAnimation.hitInfo, skillLv.listSkillLevelPotency[i].Potency_EffectIndex, _attack.eDamageType_, lvPotency) == false)
						{
							Debug.LogError("PotencyProcessor: delivered message is Msg_OtherCharAttackNpc2(Skill level table record:[index:" + skillLv.Index +
								"][action index:" + skillLv.SkillAction_Index + "][skill index:" + skillLv.Skill_GroupIndex + "]");
						}
						break;
					}
				}
			}
		}
		
		BalloonProcess( _attack.parent_.skill_, _attack.parent_.skillLv_);
	}
	
	public void PotencyProcess(Msg_OtherCharAttackNpc3 _attack)
	{
		Tbl_Skill_Record skill = _attack.parent_.skill_;
		m_OtherCharAttackNpc1 = _attack.parent_;
		
		for(int i=0; i<skill.listSkillPotency.Count; ++i)
		{
			Tbl_Skill_Potency potency = skill.listSkillPotency[i];
			Tbl_SkillLevel_Potency lvPotency = _attack.parent_.skillLv_.listSkillLevelPotency[i];
			
			if(potency.Potency_DurationType == ePotency_DurationType.Moment)
			{
				Tbl_SkillLevel_Record skillLv = _attack.parent_.skillLv_;
				Tbl_Action_Record action = _attack.parent_.action_;
				
				if(TargetDecider.CheckAffectingUserIsEnemy(m_Owner, _attack.parent_.charUniqKey_) == true) // attack from user
				{
					switch(potency.Potency_Target)
					{
					case ePotency_Target.Enemy:
					case ePotency_Target.All:
						if(PlayPotency(_attack, action.HitAnimation.hitInfo, skillLv.listSkillLevelPotency[i].Potency_EffectIndex, lvPotency) == false)
						{
							Debug.LogError("PotencyProcessor: delivered message is Msg_OtherCharAttackNpc2(Skill level table record:[index:" + skillLv.Index +
								"][action index:" + skillLv.SkillAction_Index + "][skill index:" + skillLv.Skill_GroupIndex + "]");
						}
						break;
					}
					
					if(_attack.knockBack_ == true && _attack.parent_.attacker_ != null)
					{
						switch(potency.Potency_Type)
						{
						case ePotency_Type.Move:
							ForcedMoveProcess(lvPotency, _attack.parent_.attacker_.transform.position);
							break;
						case ePotency_Type.PointMove:
							ForcedMoveProcess(lvPotency, _attack.parent_.targeting_);
							break;
						}
					}
				}
				else
				{
					switch(potency.Potency_Target)
					{
					case ePotency_Target.Alliance:
					case ePotency_Target.Party:
					case ePotency_Target.Self:
					case ePotency_Target.All:
						if(PlayPotency(action.HitAnimation.hitInfo, skillLv.listSkillLevelPotency[i].Potency_EffectIndex, lvPotency) == false)
						{
							Debug.LogError("PotencyProcessor: delivered message is Msg_OtherCharAttackNpc2(Skill level table record:[index:" + skillLv.Index +
								"][action index:" + skillLv.SkillAction_Index + "][skill index:" + skillLv.Skill_GroupIndex + "]");
						}
						break;
					}
				}
			}
		}
		
		BalloonProcess( _attack.parent_.skill_, _attack.parent_.skillLv_);
	}
	
	public void PotencyProcess(Msg_OtherCharSkillEffect _skill)
	{
		Tbl_Skill_Record skillRecord = AsTableManager.Instance.GetTbl_Skill_Record( _skill.skillTableIdx_);
		Tbl_SkillLevel_Record skillLvRecord = AsTableManager.Instance.GetTbl_SkillLevel_Record(_skill.skillLevel_, _skill.skillTableIdx_, _skill.chargeStep_);
		if( skillRecord != null && skillLvRecord != null)
		{
			Tbl_Skill_Potency potency = skillRecord.listSkillPotency[_skill.potencyIdx_];
			Tbl_SkillLevel_Potency lvPotency = skillLvRecord.listSkillLevelPotency[_skill.potencyIdx_];
			int potencyIdx = lvPotency.Potency_EffectIndex;
			if(PlayPotency(potencyIdx) == false)
			{
				Debug.LogError("PotencyProcessor: delivered message is Msg_OtherCharSkillEffect(Skill level table record:[index:" + skillLvRecord.Index +
					"][skill level:" + _skill.skillLevel_ + "][charge step:" + _skill.chargeStep_ + "]");
			}
			
			if( potency.Potency_Type == ePotency_Type.Balloon)
				m_Owner.HandleMessage( new Msg_BalloonIndicate( lvPotency));
		}
	}
	
	public void PotencyProcess(Msg_NpcSkillEffect _skill)
	{
		Tbl_MonsterSkill_Record skillRecord = AsTableManager.Instance.GetTbl_MonsterSkill_Record( _skill.skillTableIdx_);
		Tbl_MonsterSkillLevel_Record skillLvRecord = AsTableManager.Instance.GetTbl_MonsterSkillLevel_Record(_skill.skillLevel_, _skill.skillTableIdx_);//, _skill.chargeStep_);
		if( skillRecord != null && skillLvRecord != null)
		{
			Tbl_Skill_Potency potency = skillRecord.listSkillPotency[_skill.potencyIdx_];
			Tbl_SkillLevel_Potency lvPotency = skillLvRecord.listSkillLevelPotency[_skill.potencyIdx_];
			int potencyIdx = lvPotency.Potency_EffectIndex;
			if(PlayPotency(potencyIdx) == false)
			{
				Debug.LogError("PotencyProcessor: delivered message is Msg_NpcSkillEffect(Skill level table record:[index:" + skillLvRecord.Index +
					"][skill level:" + _skill.skillLevel_ + "]");
			}
			
			if( potency.Potency_Type == ePotency_Type.Balloon)
				m_Owner.HandleMessage( new Msg_BalloonIndicate( lvPotency));
		}
	}
	
	public void PotencyProcess(Msg_OtherCharSkillStance _stance)
	{		
		Tbl_Skill_Record skillRecord = AsTableManager.Instance.GetTbl_Skill_Record( _stance.stanceSkill_);
		Tbl_SkillLevel_Record skillLvRecord = AsTableManager.Instance.GetTbl_SkillLevel_Record( _stance.stanceLevel_, _stance.stanceSkill_);
		if( skillRecord != null && skillLvRecord != null)
		{
			Tbl_Skill_Potency potency = skillRecord.listSkillPotency[_stance.stancePotencyIdx_];
			Tbl_SkillLevel_Potency lvPotency = skillLvRecord.listSkillLevelPotency[_stance.stancePotencyIdx_];
			int potencyIdx = lvPotency.Potency_EffectIndex;
			if(PlayPotency(potencyIdx) == false)
			{
				Debug.LogError("PotencyProcessor: delivered message is Msg_OtherCharSkillStance(Skill level table record:[Skill_GroupIndex:" + skillLvRecord.Skill_GroupIndex +
					"][skill level:" + skillLvRecord.Skill_Level + "]");
			}
			
			if( potency.Potency_Type == ePotency_Type.Balloon)
				m_Owner.HandleMessage( new Msg_BalloonIndicate( lvPotency));
		}
	}
	#endregion
	
	public void PlayerPotencyRefresh()
	{
	}
	
	#region - play potency -
	bool PlayPotency(int _idx)
	{
		return PlayPotency(null, null, _idx, eDAMAGETYPE.eDAMAGETYPE_NORMAL, null);
	}
	
	bool PlayPotency(Tbl_Action_HitInfo _hitInfo, int _idx, Tbl_SkillLevel_Potency _lvPotency)
	{
		return PlayPotency(null, _hitInfo, _idx, eDAMAGETYPE.eDAMAGETYPE_NORMAL, _lvPotency);
	}
	
	bool PlayPotency(Msg_OtherCharAttackNpc3 _attack, Tbl_Action_HitInfo _hitInfo, int _idx, Tbl_SkillLevel_Potency _lvPotency)
	{
		return PlayPotency(_attack, _hitInfo, _idx, eDAMAGETYPE.eDAMAGETYPE_NORMAL, _lvPotency);
	}
	
	bool PlayPotency(Tbl_Action_HitInfo _hitInfo, int _idx, eDAMAGETYPE _type, Tbl_SkillLevel_Potency _lvPotency)
	{
		return PlayPotency(null, _hitInfo, _idx, _type, _lvPotency);
	}
	
	bool PlayPotency(Msg_OtherCharAttackNpc3 _attack, Tbl_Action_HitInfo _hitInfo, int _idx, eDAMAGETYPE _type, Tbl_SkillLevel_Potency _lvPotency)
	{
		if(m_Owner == null)
			return false;
		
		if(_idx == int.MaxValue || m_Owner.ModelObject == null)
			return true;
		
		Tbl_SkillPotencyEffect_Record effect = AsTableManager.Instance.GetTbl_SkillPotencyEffect_Record(_idx);
		if(effect == null)
			return false;
		
		string effectPath = "";
		string soundPath = "";
		
		effectPath = effect.PotencyEffect_FileName;
		soundPath = effect.PotencySound_FileName;
		
		switch(_type)
		{
		case eDAMAGETYPE.eDAMAGETYPE_NORMAL:
			break;
		case eDAMAGETYPE.eDAMAGETYPE_CRITICAL:
			if(effect.PotencyCriticalEffect_FileName != "NONE")
				effectPath = effect.PotencyCriticalEffect_FileName;
			if(effect.PotencyCriticalSound_FileName != "NONE")
				soundPath = effect.PotencyCriticalSound_FileName;
			break;
		case eDAMAGETYPE.eDAMAGETYPE_MISS:
//			effectPath = "Miss";
//			soundPath = "Miss";			
			break;
		case eDAMAGETYPE.eDAMAGETYPE_DODGE:
//			effectPath = "Dodge";
//			soundPath = "Dodge";
			break;
		case eDAMAGETYPE.eDAMAGETYPE_NOTHING:
//			effectPath = "Nothing";
//			soundPath = "Nothing";
			break;
		default:
//			effectPath = "etc";
//			soundPath = "etc";	
//			if(m_Owner.FsmType == eFsmType.MONSTER)
//				Debug.Log("PotencyProcessor::PlayPotency: monster [class:" + m_Owner.GetProperty<string>(eComponentProperty.CLASS) + "] attack type is " + _type);
			break;
		}
		
		if(_hitInfo != null && _hitInfo.HitValueLookType == eValueLookType.Duration)
		{
			float interval = (_hitInfo.HitValueLookDuration / _hitInfo.HitValueLookCount) * 0.0005f;
			
			SequentPotency sequent = new SequentPotency(_attack, effectPath, soundPath, effect);
			
			for(int i=0; i<_hitInfo.HitValueLookCount; ++i)
			{
				AsTimerManager.Instance.SetTimer(interval * i + interval * 0.5f, Timer_SequentEffect, sequent);
//				Debug.Log("PotencyProcessor::PlayPotency: interval=" + interval * i);
			}
		}
		else
		{
			float lifeTime = 0;
//			
			if(effect.PotencyEffect_Timing == eEFFECT_TIMING.Duration && _lvPotency != null)
				lifeTime = _lvPotency.Potency_Duration;
			
			if( null != AsEffectManager.Instance && null != AsSoundManager.Instance)
			{
				float size = 1;
				if(effect.Effect_Size != float.MaxValue)
					size = effect.Effect_Size * 0.001f * m_Owner.characterController.height;
				
				if(size > m_MaxSize)
					size = m_MaxSize;
					
				AsEffectManager.Instance.PlayEffect(effectPath, m_Owner.ModelObject.transform, false, lifeTime, size);
				AsSoundManager.Instance.PlaySound(soundPath, m_Owner.ModelObject.transform.position, false);
				
				if(effect.HitShake != float.MaxValue)
				{
					if(m_Owner.FsmType == eFsmType.MONSTER ||
						(m_Owner.FsmType == eFsmType.OTHER_USER && _attack != null))
						ShakeProcess(effect.HitShake);
				}
			}
			else
			{
				Debug.Log("PotencyProcessor::PlayPotency: AsEffectManager or AsSoundManager instance is null.");
				return false;
			}
		}
		
		return true;
	}
	#endregion
	
	public void Update()
	{
		ShakeUpdate();
	}
	
	#region - sequent potency -
	public class SequentPotency
	{
	//	AsBaseEntity m_Owner;
		
		public float time_;
		public int count_;
		
		public Msg_OtherCharAttackNpc3 attack_;
		
		public string effect_;
		public string sound_;
		
		Transform m_Trn;
		
		float m_Interval;
	//	int m_CurCount = 0;
		
		float m_Timing;
		public Tbl_SkillPotencyEffect_Record potency_;
		
		public SequentPotency(Msg_OtherCharAttackNpc3 _attack, string _effect, string _sound, Tbl_SkillPotencyEffect_Record _potency)
		{
	//		m_Owner = _entity;
			
			attack_ = _attack;
			
			effect_ = _effect;
			sound_ = _sound;
			
			potency_ = _potency;
		}
		
	//	public SequentPotency(float _time, int _count, string _effect, string _sound, Transform _trn)
	//	{
	//		effect_ = _effect;
	//		sound_ = _sound;
	//		
	//		count_ = _count;
	//		
	//		m_Interval = (_time / _count) * 0.001f;
	//		
	//		m_Timing = Time.time;
	//		m_Trn = _trn;
	//	}
		
		public bool Update()
		{
	//		if(m_CurCount == count_)
	//			return true;
	//		else if(m_Interval < Time.time - m_Timing)
	//		{
	//			if( null != AsEffectManager.Instance)
	//			{
	//				AsEffectManager.Instance.PlayEffect(effect_, m_Trn, false, 0, size_);// * 0.001f * m_Owner.characterController.height);
	//			}
	//			if( null != AsSoundManager.Instance)
	//				AsSoundManager.Instance.PlaySound(sound_, m_Trn.position, false);
	//			
	//			m_Timing = Time.time;
	//			++m_CurCount;
	//		}
			
			return false;
		}
	}
	
	void Timer_SequentEffect(System.Object _obj)
	{
		if(m_Owner != null && m_Owner.ModelObject != null)
		{
			SequentPotency sequent = _obj as SequentPotency;
//			Debug.Log("PotencyProcessor::Timer_SequentEffect: effect=" + sequent.effect_ + ", sound=" + sequent.sound_);
			
			if( null != AsEffectManager.Instance && null != AsSoundManager.Instance)
			{
				float size = 1;
				if(sequent.potency_.Effect_Size != float.MaxValue)
					size = sequent.potency_.Effect_Size * 0.001f * m_Owner.characterController.height;
				if(size > m_MaxSize)
					size = m_MaxSize;
				
				AsEffectManager.Instance.PlayEffect(sequent.effect_, m_Owner.ModelObject.transform, false, 0, size);
				AsSoundManager.Instance.PlaySound(sequent.sound_, m_Owner.ModelObject.transform.position, false);
				
				if(sequent.potency_.HitShake != float.MaxValue)
				{
//					if(m_Owner.FsmType == eFsmType.MONSTER && m_Owner.FsmType == eFsmType.OTHER_USER)
//						ShakeProcess(sequent.potency_.HitShake);
					if(m_Owner.FsmType == eFsmType.MONSTER ||
						(m_Owner.FsmType == eFsmType.OTHER_USER && sequent.attack_ != null))
						ShakeProcess(sequent.potency_.HitShake);
				}
			}
		}
	}
	#endregion
	#region - shake -
	static float s_ShakingTime = 0.1f;
	bool m_Shaking = false;
	float m_ShakingStartedTime = 0;
	Vector3 m_ShakeSpeed =Vector3.zero;
	void ShakeProcess(float _shake)
	{
		if(m_Owner.ModelObject == null)
			return;
		
		if(m_OtherCharAttackNpc1 == null)
			return;
		
		s_ShakingTime = AsTableManager.Instance.GetTbl_GlobalWeight_Record(56).Value * 0.001f;
		
		m_Shaking = true;
		m_ShakingStartedTime = Time.time;
		m_Owner.ModelObject.transform.localPosition = Vector3.zero;
		
		AsBaseEntity user = AsEntityManager.Instance.GetUserEntityByUniqueId(m_OtherCharAttackNpc1.charUniqKey_);
		if(user != null)
		{
			Vector3 dir = m_Owner.transform.position - user.transform.position;
			m_ShakeSpeed = dir.normalized * _shake * 0.01f / s_ShakingTime;
		}
		else
			Debug.LogWarning("PotencyProcessor::ShakeProcess: user is not found");
		
//		m_other
	}
	
	void ShakeUpdate()
	{
		if(m_Shaking == true && null != m_Owner.ModelObject )
		{
			float curElapsedTime = Time.time - m_ShakingStartedTime;
			
			if(curElapsedTime < s_ShakingTime * 0.5f)
				m_Owner.ModelObject.transform.position += m_ShakeSpeed * Time.deltaTime;
//				m_Owner.ModelObject.transform.Translate(m_ShakeSpeed * Time.deltaTime);
			else
				m_Owner.ModelObject.transform.position -= m_ShakeSpeed * Time.deltaTime;
//				m_Owner.ModelObject.transform.Translate(-m_ShakeSpeed * Time.deltaTime);
			
			if(s_ShakingTime < curElapsedTime)
			{
				m_Shaking = false;
				m_Owner.ModelObject.transform.localPosition = Vector3.zero;
			}
		}
	}
	#endregion
	#region - forced move -
	void ForcedMoveProcess(Tbl_SkillLevel_Potency _lvPotency, Vector3 _originPos)
	{
		if(_originPos == Vector3.zero)
			Debug.LogWarning("PotencyProcessor::ForcedMoveProcess: indicated position is {0,0,0}");
		
		Vector3 dir = -_originPos + m_Owner.transform.position;
		
		float distReal = dir.magnitude * 0.8f;
		float distCalcul = _lvPotency.Potency_IntValue * 0.01f;
		
		float dist = distCalcul;
		if(distCalcul < 0 && -distCalcul > distReal) dist = -distReal;
		
		if(Mathf.Abs(dist) > 0.2f)
		{
			Vector3 dest = m_Owner.transform.position + dir.normalized * dist;
			
			Msg_ConditionIndicate_ForcedMove indication = new Msg_ConditionIndicate_ForcedMove(dest, dist, s_ForcedMoveDuratioin);
//					Msg_ForcedMoveUserIndication indication = new Msg_ForcedMoveUserIndication(s_ForcedMoveDuratioin, dest);
			m_Owner.HandleMessage(indication);
		}
	}
	#endregion
	#region - word balloon -
	void BalloonProcess( Tbl_Skill_Record _skill, Tbl_SkillLevel_Record _skillLv)
	{
		List<Tbl_SkillLevel_Potency> list = new List<Tbl_SkillLevel_Potency>();
		
		for(int i=0; i<_skill.listSkillPotency.Count; ++i)
		{
			Tbl_Skill_Potency potency = _skill.listSkillPotency[i];
			Tbl_SkillLevel_Potency lvPotency = _skillLv.listSkillLevelPotency[i];
			
			if(potency.Potency_Type == ePotency_Type.Balloon)
				list.Add( lvPotency);
		}
		
		if( list.Count > 0)
			m_Owner.HandleMessage( new Msg_BalloonIndicate( list[ Random.Range( 0, list.Count)]));
	}
	
	void BalloonProcess( Tbl_MonsterSkill_Record _skill, Tbl_MonsterSkillLevel_Record _skillLv)
	{
		List<Tbl_SkillLevel_Potency> list = new List<Tbl_SkillLevel_Potency>();
		
		for(int i=0; i<_skill.listSkillPotency.Count; ++i)
		{
			Tbl_Skill_Potency potency = _skill.listSkillPotency[i];
			Tbl_SkillLevel_Potency lvPotency = _skillLv.listSkillLevelPotency[i];
			
			if(potency.Potency_Type == ePotency_Type.Balloon)
				list.Add( lvPotency);
		}
		
		if( list.Count > 0)
			m_Owner.HandleMessage( new Msg_BalloonIndicate( list[ Random.Range( 0, list.Count)]));
	}
	#endregion
}

