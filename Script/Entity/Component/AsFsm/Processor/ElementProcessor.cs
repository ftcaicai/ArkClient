using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum eActionStep {NONE = 0, Ready, Hit, Finish, Entire}

public class ElementProcessor
{
	Tbl_Action_Record m_Action;
	
	EffectProcessor m_Effect;
	SoundProcessor m_Sound;
	
	PotencyProcessor m_Potency;
	
	BuffProcessor m_Buff;
	
	AsBaseEntity m_Owner;public AsBaseEntity Owner{get{return m_Owner;}}
	
	eActionStep m_CurStep =  eActionStep.NONE;
	
	public ElementProcessor(AsBaseEntity _entity)
	{
		m_Owner = _entity;
		
		m_Effect = new EffectProcessor(_entity);
		m_Sound = new SoundProcessor(_entity);		
		m_Potency = new PotencyProcessor(_entity);		
		m_Buff = new BuffProcessor(_entity);
	}
	
	#region - init & release -
	public void ModelLoaded()
	{
		m_Buff.ModelLoaded();
	}
	
	public void PlayerModelLoaded()
	{
		m_Buff.PlayerModelLoaded();
//		m_Potency.PlayerPotencyRefresh();
	}
	
	public void SetAction(Tbl_Action_Record _action)
	{
//		if(m_Action != null && m_Action.ActionName == "BattleIdle" &&
//			_action != null && _action.HitAnimation != null)
//		{
//			Debug.LogError("ElementProcessor::SetAction: m_Action = " + m_Action + ", _action = " + _action);
//		}
		
		m_Action = _action;
	}
	
	public void Update()
	{
		m_Effect.Update();
		m_Sound.Update();
		m_Potency.Update();
	}
	
	public void Release(bool _canceled)
	{
		m_Effect.Release(_canceled);
		m_Sound.Release();
	}
	
	public void Cancel()
	{
		m_Effect.Cancel();
	}
	#endregion
	
	public bool GetEnableCondition(eSkillIcon_Enable_Condition _condition)
	{
		return m_Buff.GetEnableCondition(_condition);
	}
	
	#region - element -
	bool CheckPermission_NonAction()
	{
		if( m_Action == null)
		{
			bool caseMonster = m_Owner.FsmType == eFsmType.MONSTER &&
				m_Owner.ContainProperty(eComponentProperty.GRADE) == true &&
					( m_Owner.GetProperty<eMonster_Grade>(eComponentProperty.GRADE) == eMonster_Grade.DObject &&
					 m_Owner.GetProperty<eMonster_Grade>(eComponentProperty.GRADE) == eMonster_Grade.Treasure);

			if(caseMonster == true)
				return true;
			else
				return false;
		}
		else
			return true;
	}
	
	public void PlayElement(eActionStep _element, Vector3 _pos, Transform _targetTrn)
	{
		PlayElement(_element, _pos, _targetTrn, 1);
	}
	
	public void PlayElement(eActionStep _element, Vector3 _pos, Transform _targetTrn, float _animSpeed)
	{
		if( CheckPermission_NonAction() == false)
		{
			Debug.LogWarning("ElementProcessor::PlayElement: There is no [" + _element + "] action. check action initializing");
			return;
		}
		
		if(((_element == eActionStep.Ready || _element == eActionStep.Entire) && m_Action.ReadyAnimation == null) ||
				(_element == eActionStep.Hit && m_Action.HitAnimation == null) ||
				(_element == eActionStep.Finish && m_Action.FinishAnimation == null))
		{
			if(m_Owner.CheckModelLoadingState() != eModelLoadingState.Finished)
			{
				Debug.LogWarning("ElementProcessor::PlayElement: Loading ModelObject is not finished. state = " + m_Owner.CheckModelLoadingState());
				return;
			}
			
			Debug.LogWarning("ElementProcessor::PlayElement: There is no [" + _element + "] action. check action initializing");
				
			Debug.LogWarning("ElementProcessor::PlayElement: m_Action.Index == " + m_Action.Index);
			Debug.LogWarning("ElementProcessor::PlayElement: m_Action.ActionName == " + m_Action.ActionName);
			Debug.LogWarning("ElementProcessor::PlayElement: m_Action.ReadyAnimation == " + m_Action.ReadyAnimation);
			Debug.LogWarning("ElementProcessor::PlayElement: m_Action.HitAnimation == " + m_Action.HitAnimation);
			Debug.LogWarning("ElementProcessor::PlayElement: m_Action.FinishAnimation == " + m_Action.FinishAnimation);
			Debug.LogWarning("ElementProcessor::PlayElement: _pos = " + _pos + ", _targetTrn = " + _targetTrn + ", _animSpeed = " + _animSpeed);
			return;
		}
		
//		m_Effect.Release(false);
//		m_Sound.Release();
		
		bool refresh = false;
		if( m_CurStep != _element)
		{
			refresh = true;
			
			m_Effect.Release(false);
			m_Sound.Release();
		}
		
		m_CurStep = _element;
		
		switch(_element)
		{
		case eActionStep.Ready:
		case eActionStep.Entire:
			m_Effect.AddEffect(m_Action.ReadyAnimation.listEffect, _pos, _targetTrn, _animSpeed, refresh);
//			m_Sound.AddSound(m_Action.ReadyAnimation.listSound, _animSpeed);
			m_Sound.AddSound(m_Action.ReadyAnimation, _animSpeed);
			break;
		case eActionStep.Hit:
			m_Effect.AddEffect(m_Action.HitAnimation.listEffect, _pos, _targetTrn, _animSpeed, refresh);
//			m_Sound.AddSound(m_Action.HitAnimation.listSound, _animSpeed);
			m_Sound.AddSound(m_Action.HitAnimation, _animSpeed);
			break;
		case eActionStep.Finish:
			m_Effect.AddEffect(m_Action.FinishAnimation.listEffect, _pos, _targetTrn, _animSpeed, refresh);
//			m_Sound.AddSound(m_Action.FinishAnimation.listSound, _animSpeed);
			m_Sound.AddSound(m_Action.FinishAnimation, _animSpeed);
			break;
//		case eActionStep.Entire:
//			List<Tbl_Action_Effect> listEffect = new List<Tbl_Action_Effect>();
//			List<Tbl_Action_Sound> listSound = new List<Tbl_Action_Sound>();
//			
//			if(m_Action.ReadyAnimation != null)
//			{
//				listEffect.AddRange(m_Action.ReadyAnimation.listEffect);
//				listSound.AddRange(m_Action.ReadyAnimation.listSound);
//			}
//			if(m_Action.HitAnimation != null)
//			{
//				listEffect.AddRange(m_Action.HitAnimation.listEffect);
//				listSound.AddRange(m_Action.HitAnimation.listSound);
//			}
//			if(m_Action.FinishAnimation != null)
//			{
//				listEffect.AddRange(m_Action.FinishAnimation.listEffect);
//				listSound.AddRange(m_Action.FinishAnimation.listSound);
//			}
//			
//			m_Effect.AddEffect(listEffect, _pos, _targetTrn, _animSpeed);
//			m_Sound.AddSound(listSound, _animSpeed);
//			break;
		}
	}
	
	public void PlayElement(eActionStep _element)
	{
		PlayElement(_element, Vector3.zero, null);
	}
	#endregion
	#region - potency -
	public void PotencyProcess(Msg_NpcAttackChar2 _attack)
	{
		m_Potency.PotencyProcess(_attack);
	}
	
	public void PotencyProcess(Msg_NpcAttackChar3 _attack)
	{
		m_Potency.PotencyProcess(_attack);
	}
	
	public void PotencyProcess(Msg_OtherCharAttackNpc2 _attack)
	{
		m_Potency.PotencyProcess(_attack);
	}
	
	public void PotencyProcess(Msg_OtherCharAttackNpc3 _attack)
	{
		m_Potency.PotencyProcess(_attack);
	}
	
	public void PotencyProcess(Msg_NpcSkillEffect _attack)
	{
		m_Potency.PotencyProcess(_attack);
	}
	
	public void PotencyProcess(Msg_OtherCharSkillEffect _attack)
	{
		m_Potency.PotencyProcess(_attack);
	}
	
	public void PotencyProcess(Msg_OtherCharSkillStance _stance)
	{
		m_Potency.PotencyProcess(_stance);
	}
	#endregion	
	#region - buff -
	public void SetBuff(Msg_CharBuff_Body _body)
	{
		m_Buff.AddBuff(_body);
	}
	
	public void SetBuff(Msg_CharBuff _head, Msg_CharBuff_Body _body)
	{
		m_Buff.AddBuff(_body, _head.effect_);
	}
	
	public void SetBuff(body2_SC_CHAR_BUFF _body)
	{
		m_Buff.AddBuff(_body);
	}
	
//	public void RefreshBuff(body2_SC_CHAR_BUFF _body)
//	{
//		m_Buff.AddBuff(_body);///, false);
//	}
	
	public void RefreshBuff()
	{
		m_Buff.RefreshBuff();
	}
	
	public void RemoveBuff(Msg_CharDeBuff _potency)
	{
		m_Buff.RemoveBuff(_potency);
	}
	
	public void SetBuff(Msg_NpcBuff_Body _buff, bool _effect)
	{
		m_Buff.AddBuff(_buff, _effect);
	}
	
	public void RemoveBuff(Msg_NpcDeBuff _deBuff)
	{
		m_Buff.RemoveBuff(_deBuff);
	}
	
	public void ReleaseBuff()
	{
		m_Buff.Release();
	}
	
	public bool CheckBuffInclusion(int _skillIdx)
	{
		return m_Buff.CheckBuffInclusion(_skillIdx);
	}
	#endregion
	#region - soul stone -
	public void SoulStoneProc( ref Msg_CharSkillSoulStone _soul)
	{
		Tbl_SkillLevel_Record lv = AsTableManager.Instance.GetTbl_SkillLevel_Record( _soul.soul_.nSkillLevel, _soul.soul_.nSkillTableIdx);
		if( lv == null)
		{
			Debug.LogError("ElementProcessor:: SouStoneProc: skill level record is not found. _soul.soul_.nSkillLevel = " + _soul.soul_.nSkillLevel +
				"_soul.soul_.nSkillTableIdx = " + _soul.soul_.nSkillTableIdx);
			return;
		}
		
		if( m_Owner.ContainProperty(eComponentProperty.GENDER) == false)
		{
			Debug.LogError("ElementProcessor:: SouStoneProc: owner doesnt have gender property. fsm type = " + m_Owner.FsmType);
			return;
		}
		
		Tbl_Action_Record action = null;
		eGENDER gender = m_Owner.GetProperty<eGENDER>(eComponentProperty.GENDER);
		switch( gender)
		{
		case eGENDER.eGENDER_MALE:
			action = AsTableManager.Instance.GetTbl_Action_Record( lv.SkillAction_Index);
			break;
		case eGENDER.eGENDER_FEMALE:
			action = AsTableManager.Instance.GetTbl_Action_Record( lv.SkillAction_Index_Female);
			break;
		}
		
		if( action == null)
		{
			Debug.LogError("ElementProcessor:: SouStoneProc: action is not found. skill level record index = " + lv.Index);
			return;
		}
		
		float animSpeed = 1f;
		if( m_Owner.ContainProperty(eComponentProperty.ATTACK_SPEED) == true)
			animSpeed = m_Owner.GetProperty<float>(eComponentProperty.ATTACK_SPEED);
		
		if( action.HitAnimation != null)
		{
			m_Effect.AddEffect(action.HitAnimation.listEffect, Vector3.zero, _soul.player_.Target.transform, animSpeed, false);
			m_Sound.AddSound(action.HitAnimation, animSpeed);
		}
		else
			Debug.LogError("ElementProcessor:: SouStoneProc: action has no READY animation");
		
		_soul.SetActionIdx( action.Index);
	}
	#endregion
}