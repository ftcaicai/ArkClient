using UnityEngine;
using System.Collections;

public class AsOtherUserState_SkillReady : AsBaseFsmState<AsOtherUserFsm.eOtherUserFsmStateType, AsOtherUserFsm> {//
	
	#region - member -
	string m_CurPlayingAnimName;
	
	Msg_OtherCharAttackNpc_Ready m_ReadyMsg;
	
	Tbl_Action_Animation m_ReadyAnimation;
	#endregion
	
	public AsOtherUserState_SkillReady(AsOtherUserFsm _fsm) : base(AsOtherUserFsm.eOtherUserFsmStateType.SKILL_READY, _fsm)
	{
		m_dicMessageTreat.Add(eMessageType.MOVE_OTHER_USER_INDICATION, OnBeginMove);
		m_dicMessageTreat.Add(eMessageType.CHAR_ATTACK_NPC1, OnBeginSkill);
		m_dicMessageTreat.Add(eMessageType.OTHER_CHAR_ATTACK_READY, OnBeginSkill);
		m_dicMessageTreat.Add(eMessageType.ANIMATION_END, OnAnimationEnd);
	}
	
	#region - fsm function -
	public override void Enter(AsIMessage _msg)
	{
		m_OwnerFsm.Entity.HandleMessage(new Msg_MoveStopIndication());
		
		ReadyProcess(_msg);
	}
	public override void Update()
	{
//		CheckTargetLiving();
//		if(CheckTargetLiving() == true)
//			RefreshDirection();
		if(m_ReadyMsg.actionRecord.ReadyAnimation != null)
		{
			CheckAnimDuration();
		}
	}
	public override void Exit()
	{
		m_OwnerFsm.ReleaseElements();
	}
	#endregion
	
	#region - update -
	bool CheckTargetLiving()
	{
		if(m_OwnerFsm.Target == null)
			return ReleaseCombat();
		
		if(m_OwnerFsm.Target.ContainProperty(eComponentProperty.LIVING) == true)
		{
			if(m_OwnerFsm.Target.GetProperty<bool>(eComponentProperty.LIVING) == false)
			{
				return ReleaseCombat();
			}
		}
		
		return true;
	}
	
	void RefreshDirection()
	{
		Vector3 dir = m_OwnerFsm.Target.transform.position - m_OwnerFsm.Entity.transform.position;
		dir.y = 0;
		
		if( Vector3.zero == dir)
			return;
		
		m_OwnerFsm.Entity.transform.rotation = 
			Quaternion.Slerp(m_OwnerFsm.Entity.transform.rotation, 
				Quaternion.LookRotation(dir), 
				7.0f * Time.deltaTime);
	}
	
	void CheckAnimDuration()
	{
		if((m_ReadyMsg.actionRecord.ReadyAnimation.LoopType & (eLoopType.TimeLoop | 
				eLoopType.TimeLoop)) != 0)
		{
			if(TimeElapsed() == true)
			{
				ChangeStateToHit();
			}
		}
	}
	#endregion
	
	#region - ready to READY -
	void ReadyProcess(AsIMessage _msg)
	{
		switch(_msg.MessageType)
		{
		case eMessageType.OTHER_CHAR_ATTACK_READY:
			ReadyMessageDecode(_msg as Msg_OtherCharAttackNpc_Ready);
			break;
		case eMessageType.OTHER_CHAR_ATTACK_LINKACTION:
			ReadyMessageDecode(_msg as Msg_OtherCharAttackNpc_Link);
			break;
		}
		
		m_OwnerFsm.transform.LookAt(m_OwnerFsm.transform.position + m_ReadyMsg.attackMsg.direction_);	
		m_OwnerFsm.SetAction(m_Action);
		
		if(m_ReadyAnimation != null)
		{
			float animSpeed = GetAnimationSpeedByAttack_UserEntity(m_Action, eActionStep.Ready);
			m_ReadyMsg.SetAnimSpeed(animSpeed);
			
			switch(m_ReadyAnimation.LoopType)
			{
			case eLoopType.Once:
			case eLoopType.TargetLoop:
				break;
			case eLoopType.Loop:
			case eLoopType.TimeLoop:
			case eLoopType.Cast:
				SetTimer(m_ReadyAnimation.LoopDuration * 0.001f);
				break;
			case eLoopType.Charge:
				if(m_ReadyMsg.attackMsg.chargeStep_ == -1)
				{
					m_OwnerFsm.ReleaseElements();
					m_OwnerFsm.SetOtherUserFsmState(AsOtherUserFsm.eOtherUserFsmStateType.IDLE);
					return;
				}
				else
					SetTimer(float.PositiveInfinity);
				break;
			}
			
			Msg_AnimationIndicate anim = null;
			switch(m_ReadyAnimation.LoopType)
			{
			case eLoopType.Once:
			case eLoopType.Loop:
			case eLoopType.TimeLoop:
			case eLoopType.Cast:
			case eLoopType.Charge:
				anim = new Msg_AnimationIndicate(m_ReadyAnimation.FileName, 0.1f, m_ReadyMsg.animSpeed);
				break;
			case eLoopType.TargetLoop:
				anim = new Msg_AnimationIndicate(m_ReadyAnimation.FileName, 0.1f, m_ReadyMsg.animSpeed,
					m_ReadyAnimation.LoopTargetTime, m_ReadyAnimation.LoopDuration);
				break;
			default:
				Debug.LogError("AsOtherUserState_SkillReady:: ReadyProcess: invalid ready animation process. m_ReadyAnimation.LoopType = " + m_ReadyAnimation.LoopType);
				ChangeStateToHit();
				return;
			}
			
			m_CurPlayingAnimName = m_ReadyMsg.actionRecord.ReadyAnimation.FileName;
			m_OwnerFsm.OtherUserEntity.HandleMessage(anim);			
			
			if(m_Action.AniBlendStep == eActionStep.Ready)
				m_OwnerFsm.Entity.HandleMessage(new Msg_FadeTimeIndicate(m_Action.AniBlendingDuration * 0.001f));
			
			if(m_OwnerFsm.Target != null)
				m_OwnerFsm.PlayElements(eActionStep.Ready, m_ReadyMsg.attackMsg.targeting_,
					m_OwnerFsm.Target.transform, m_ReadyMsg.animSpeed);
			else
				m_OwnerFsm.PlayElements(eActionStep.Ready, m_ReadyMsg.attackMsg.targeting_,
					null, m_ReadyMsg.animSpeed);
		}
		else
		{
			ChangeStateToHit();
		}
		
		ActionMove();
	}
	
	void ActionMove()
	{
		if(m_Action.ReadyAnimation == null)
			return;
		
		Msg_DashIndication dashIndication = null;
		
		float attSpeed = m_ReadyMsg.animSpeed;// m_OwnerFsm.Entity.GetProperty<float>(eComponentProperty.ATTACK_SPEED);
			
		switch(m_Action.ReadyAnimation.MoveType)
		{
		case eReadyMoveType.Dash:
			dashIndication = new Msg_DashIndication(
				m_Action.ReadyAnimation.LoopDuration / attSpeed, m_Action.ReadyAnimation.MoveDistance * 0.01f,
				m_OwnerFsm.transform.forward, m_OwnerFsm.transform.position);
			break;
		case eReadyMoveType.TargetDash:
			if(m_OwnerFsm.Entity.ModelObjectAnimation == null)
				return;
			
			if(m_OwnerFsm.Target != null)
			{				
				AnimationState animState = m_OwnerFsm.Entity.ModelObjectAnimation[m_Action.ReadyAnimation.FileName];
				if(animState != null)
				{
					dashIndication = new Msg_DashIndication(
						animState.length * 1000 / attSpeed, m_OwnerFsm.targetDistance - 0.3f,
						m_OwnerFsm.transform.forward, m_OwnerFsm.transform.position);
				}
			}
			break;
		case eReadyMoveType.BackTargetDash:
			if(m_OwnerFsm.Entity.ModelObjectAnimation == null)
				return;
			
			if(m_OwnerFsm.Target != null)
			{
				AnimationState animState = m_OwnerFsm.Entity.ModelObjectAnimation[m_Action.HitAnimation.FileName];
				if(animState != null)
				{
					dashIndication = new Msg_DashBackIndication(
						animState.length * 1000 / attSpeed, m_Action.ReadyAnimation.MoveDistance * 0.01f,
						-m_OwnerFsm.transform.forward, m_OwnerFsm.transform.position);
				}
			}
			break;
		case eReadyMoveType.TabDash:			
			if(m_OwnerFsm.Entity.ModelObjectAnimation == null)
				return;
			
			float dist = Vector3.Distance(m_OwnerFsm.transform.position, m_ReadyMsg.attackMsg.targeting_);		
			AnimationState animState = m_OwnerFsm.Entity.ModelObjectAnimation[m_Action.ReadyAnimation.FileName];
			Debug.Log("dist:" + dist + ", animState:" + animState);
			if(animState != null)
			{
				dashIndication = new Msg_DashIndication(
					animState.length * 1000 / attSpeed, dist - 1,
					m_OwnerFsm.transform.forward, m_OwnerFsm.transform.position);
				Debug.Log("dist:" + dist + ", animState.length * 1000 / attSpeed:" + animState.length * 1000 / attSpeed);
			}
			break;
		case eReadyMoveType.TabWarp:
			dist = Vector3.Distance(m_OwnerFsm.transform.position, m_ReadyMsg.attackMsg.targeting_);
			Msg_WarpIndication warp = new Msg_WarpIndication(dist, m_OwnerFsm.transform.forward, m_OwnerFsm.transform.position);
			m_OwnerFsm.Entity.HandleMessage(warp);
			break;
		default:
			return;
		}
		
		if(dashIndication != null)
			m_OwnerFsm.Entity.HandleMessage(dashIndication);
	}
	
	void ReadyMessageDecode(Msg_OtherCharAttackNpc_Ready _attack)
	{
		m_ReadyMsg = _attack;
		
		if(m_ReadyMsg.attackMsg.npcIdx_[0] != 0)
			m_OwnerFsm.Target = AsEntityManager.Instance.GetNpcEntityBySessionId(m_ReadyMsg.attackMsg.npcIdx_[0]);
		else if(m_ReadyMsg.attackMsg.mainCharUniqKey_[0] != 0)
			m_OwnerFsm.Target = AsEntityManager.Instance.GetUserEntityByUniqueId(m_ReadyMsg.attackMsg.mainCharUniqKey_[0]);
			
		m_Action = m_ReadyMsg.actionRecord;
		m_ReadyAnimation = m_Action.ReadyAnimation;
	}
	
	void ReadyMessageDecode(Msg_OtherCharAttackNpc_Link _link)
	{
		m_ReadyMsg = _link.ready_;
		
		if(m_ReadyMsg.attackMsg.npcIdx_[0] != 0)
			m_OwnerFsm.Target = AsEntityManager.Instance.GetNpcEntityBySessionId(m_ReadyMsg.attackMsg.npcIdx_[0]);
		else if(m_ReadyMsg.attackMsg.mainCharUniqKey_[0] != 0)
			m_OwnerFsm.Target = AsEntityManager.Instance.GetUserEntityByUniqueId(m_ReadyMsg.attackMsg.mainCharUniqKey_[0]);
		
		m_Action = m_ReadyMsg.actionRecord;
		m_ReadyAnimation = m_Action.ReadyAnimation;
	}
	#endregion
	
	#region - msg -
	void OnBeginMove(AsIMessage _msg)
	{
		m_OwnerFsm.SetOtherUserFsmState(AsOtherUserFsm.eOtherUserFsmStateType.RUN, _msg);
	}
	
	void OnBeginSkill(AsIMessage _msg)
	{
		ReadyProcess(_msg);
	}
	
	void OnAnimationEnd(AsIMessage _msg)
	{
		Msg_AnimationEnd end = _msg as Msg_AnimationEnd;
		if(end.animName_ == m_CurPlayingAnimName &&
			(m_ReadyMsg.actionRecord.ReadyAnimation.LoopType & (eLoopType.NONE | eLoopType.Once | eLoopType.TargetLoop)) != 0)
		{
			ChangeStateToHit();
		}
	}
	#endregion
	
	#region - method -
	void ChangeStateToHit()
	{
		ReleaseTimer();
		Msg_OtherCharAttackNpc_Hit hit = new Msg_OtherCharAttackNpc_Hit(m_ReadyMsg);
		m_OwnerFsm.SetOtherUserFsmState(AsOtherUserFsm.eOtherUserFsmStateType.SKILL_HIT, hit);
	}
	
	bool ReleaseCombat()
	{
		m_OwnerFsm.Target = null;
//		m_OwnerFsm.SetOtherUserFsmState(AsOtherUserFsm.eOtherUserFsmStateType.IDLE);
		return false;
	}
	#endregion
	
//	#endregion
}
