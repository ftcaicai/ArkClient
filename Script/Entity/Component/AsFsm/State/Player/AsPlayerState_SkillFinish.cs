using UnityEngine;
using System.Collections;

public class AsPlayerState_SkillFinish : AsBaseFsmState<AsPlayerFsm.ePlayerFsmStateType, AsPlayerFsm> {
	
	#region - member -
	string m_CurPlayingAnimName;
	
	Msg_Player_Skill_Finish m_FinishMsg;
	
	float m_BeginTime;
	
	Tbl_Action_Animation m_FinishAnimation;
	
	bool canceled = false;
	
	AsBaseEntity m_PrevTarget = null;
	#endregion
	
	public AsPlayerState_SkillFinish(AsPlayerFsm _fsm) : base(AsPlayerFsm.ePlayerFsmStateType.SKILL_FINISH, _fsm)
	{
		m_dicMessageTreat.Add(eMessageType.ANIMATION_END, OnAnimationEnd);
		m_dicMessageTreat.Add(eMessageType.INPUT_MOVE, OnBeginMove);
		m_dicMessageTreat.Add(eMessageType.INPUT_AUTO_MOVE, OnBeginAutoMove);
		m_dicMessageTreat.Add(eMessageType.OTHER_USER_CLICK, OnOtherUserClick);
		m_dicMessageTreat.Add(eMessageType.NPC_CLICK, OnNpcClick);
		m_dicMessageTreat.Add(eMessageType.COLLECT_CLICK, OnCollectClick);
		m_dicMessageTreat.Add(eMessageType.OBJECT_CLICK, OnObjectClick);
		m_dicMessageTreat.Add(eMessageType.INPUT_ATTACK, OnBeginAttack);
		m_dicMessageTreat.Add(eMessageType.PLAYER_SKILL_TARGET_MOVE, OnTargetSkillMove);
		m_dicMessageTreat.Add(eMessageType.PLAYER_SKILL_READY, OnSkillReady);
		m_dicMessageTreat.Add(eMessageType.INPUT_DASH, OnBeginDash);
		m_dicMessageTreat.Add(eMessageType.CHEAT_DEATH, OnCheatDeath);
	}
	
	#region - fsm function -
	public override void Enter(AsIMessage _msg)
	{
		m_OwnerFsm.Entity.HandleMessage(new Msg_MoveStopIndication());
//		m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("Idle"));
		
		FinishProcess(_msg);
	}
	public override void Update()
	{
		CheckTargetLiving();
//		if(CheckTargetLiving() == true)
//			RefreshDirection();
		
		if(m_FinishMsg.ready_.actionRecord != null)
		{
			CheckAnimDuration();
		}
	}
	public override void Exit()
	{
		m_OwnerFsm.ReleaseElements(canceled);
		
		m_FinishMsg = null;
	}
	#endregion
	
	#region - update -
	bool CheckTargetLiving()
	{
		if(m_PrevTarget == null)
			return ReleaseCombat();
		
		if(m_PrevTarget.ContainProperty(eComponentProperty.LIVING) == true)
		{
			if(m_PrevTarget.GetProperty<bool>(eComponentProperty.LIVING) == false)
			{
				return ReleaseCombat();
			}
		}
		
		return true;
	}
	
	void RefreshDirection()
	{
		Vector3 dir = m_PrevTarget.transform.position - m_OwnerFsm.Entity.transform.position;
		
		if( Vector3.zero == dir)
			return;
		
		m_OwnerFsm.Entity.transform.rotation = 
			Quaternion.Slerp(m_OwnerFsm.Entity.transform.rotation, 
				Quaternion.LookRotation(dir), 
				7.0f * Time.deltaTime);
	}
	
	void CheckAnimDuration()
	{	
		if(m_FinishMsg.ready_.actionRecord.FinishAnimation.LoopType == eLoopType.TimeLoop)
		{
			if(TimeElapsed() == true)
			{
				FinishStateEnd();
			}
		}
	}
	#endregion
	
	#region - ready to FINISH -
	void FinishProcess(AsIMessage _msg)
	{
		m_FinishMsg = _msg as Msg_Player_Skill_Finish;
		
		m_Action = m_FinishMsg.ready_.actionRecord;
		m_BeginTime = Time.time;
		m_FinishAnimation = m_Action.FinishAnimation;
		
		m_PrevTarget = m_FinishMsg.ready_.Target;
		
		

		if(m_FinishAnimation != null)
		{
			float animSpeed = GetAnimationSpeedByAttack_UserEntity(m_Action, eActionStep.Finish);
			m_FinishMsg.ready_.SetAnimSpeed( animSpeed);
			
			switch(m_FinishAnimation.LoopType)
			{
			case eLoopType.Once:
			case eLoopType.TargetLoop:
			case eLoopType.Charge:
				break;
			case eLoopType.Loop:
			case eLoopType.TimeLoop:
			case eLoopType.Cast:
				SetTimer(m_FinishAnimation.LoopDuration * 0.001f);
				break;
			}
			
			Msg_AnimationIndicate anim = null;		
			switch(m_FinishAnimation.LoopType)
			{
			case eLoopType.Once:
			case eLoopType.Loop:
			case eLoopType.TimeLoop:
			case eLoopType.Cast:
			case eLoopType.Charge:
				anim = new Msg_AnimationIndicate(m_FinishAnimation.FileName, 0.1f, m_FinishMsg.ready_.animSpeed);
				break;
			case eLoopType.TargetLoop:
				anim = new Msg_AnimationIndicate(m_FinishAnimation.FileName, 0.1f, m_FinishMsg.ready_.animSpeed,
					m_FinishAnimation.LoopTargetTime, m_FinishAnimation.LoopDuration);
				break;
			default:
				Debug.LogError("AsPlayerState_SkillFinish::FinishProcess: invalid finish animation process. m_FinishAnimation.LoopType = " + m_FinishAnimation.LoopType);
				FinishStateEnd();
				return;
			}
			
			m_CurPlayingAnimName = m_FinishAnimation.FileName;
			m_OwnerFsm.UserEntity.HandleMessage(anim);			
			
			if(m_Action.AniBlendStep == eActionStep.Finish)
				m_OwnerFsm.Entity.HandleMessage(new Msg_FadeTimeIndicate(m_Action.AniBlendingDuration * 0.001f));
			
			if(m_PrevTarget != null)
				m_OwnerFsm.PlayElements(eActionStep.Finish, m_FinishMsg.ready_.picked, m_PrevTarget.transform, m_FinishMsg.ready_.animSpeed);
			else
				m_OwnerFsm.PlayElements(eActionStep.Finish, m_FinishMsg.ready_.picked, null, m_FinishMsg.ready_.animSpeed);
		}
		else
		{
			FinishStateEnd();
		}
	}
	#endregion

	//MSG
	#region - msg -
	void OnAnimationEnd(AsIMessage _msg)
	{
		Msg_AnimationEnd end = _msg as Msg_AnimationEnd;
		if(end.animName_ == m_CurPlayingAnimName && 
			(m_FinishMsg.ready_.actionRecord.FinishAnimation.LoopType & (eLoopType.NONE | eLoopType.Once | eLoopType.TargetLoop)) != 0)
		{
			FinishStateEnd();
		}
	}
	
	void OnBeginMove(AsIMessage _msg)
	{
		if(m_OwnerFsm.Entity.CheckCondition(eSkillIcon_Enable_Condition.Bind) == true)
			return;
		
		if(CancelEnable() == true)
		{
			canceled = true;
			m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.RUN, _msg);
		}
	}
	
	void OnBeginAutoMove(AsIMessage _msg)
	{
		if(m_OwnerFsm.Entity.CheckCondition(eSkillIcon_Enable_Condition.Bind) == true)
			return;
		
		if(CancelEnable() == true)
		{
			canceled = true;
			m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.RUN, _msg);
		}
	}
	
	void OnOtherUserClick(AsIMessage _msg)
	{
		if(m_OwnerFsm.Entity.CheckCondition(eSkillIcon_Enable_Condition.Bind) == true)
			return;
		
//		if(CancelEnable() == true)
//		{
//			canceled = true;
//			m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.RUN_OTHERUSER, _msg);
//		}
	}
	
	void OnNpcClick(AsIMessage _msg)
	{
		if(m_OwnerFsm.Entity.CheckCondition(eSkillIcon_Enable_Condition.Bind) == true)
			return;
		
		if(CancelEnable() == true)
		{
			canceled = true;
			m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.RUN_NPC, _msg);
		}
	}
	
	void OnCollectClick(AsIMessage _msg)
	{
		if(m_OwnerFsm.Entity.CheckCondition(eSkillIcon_Enable_Condition.Bind) == true)
			return;
		
		if(CancelEnable() == true)
		{
			canceled = true;
			m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.RUN_COLLECT, _msg);		
		}
	}
	
	void OnObjectClick(AsIMessage _msg)
	{
		if(m_OwnerFsm.Entity.CheckCondition(eSkillIcon_Enable_Condition.Bind) == true)
			return;
		
		if(CancelEnable() == true)
		{
			canceled = true;
			m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.RUN_OBJECT, _msg);
		}
	}
	
	void OnBeginAttack(AsIMessage _msg)
	{
		if(m_OwnerFsm.Entity.CheckCondition(eSkillIcon_Enable_Condition.Bind) == true)
			return;
		
		Msg_Input_Attack attack = _msg as Msg_Input_Attack;
		
		if(CancelEnable() == true
			&& attack.enemy_ != m_PrevTarget)
		{
			canceled = true;
			m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.BATTLE_IDLE, _msg);
		}
	}
	
	void OnTargetSkillMove(AsIMessage _msg)
	{
		if(m_OwnerFsm.Entity.CheckCondition(eSkillIcon_Enable_Condition.Bind) == true)
			return;
		
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.BATTLE_RUN, _msg);
	}
	
	void OnSkillReady(AsIMessage _msg)
	{
		if(CancelEnable() == true)
		{
			canceled = true;
			
			m_OwnerFsm.ReleaseElements();
			m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.SKILL_READY, _msg);
		}
	}
	
	void OnBeginDash(AsIMessage _msg)
	{
		if(CancelEnable() == true)
		{
			canceled = true;
			m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.DASH, _msg);
		}
	}
	
	void OnCheatDeath( AsIMessage _msg)
	{
		m_OwnerFsm.CheatDeath( _msg);
	}
	#endregion
	
	#region - method -
	void FinishStateEnd()
	{
		ReleaseTimer();
		
		if(m_FinishMsg.ready_.actionRecord.LinkActionIndex != 0)
		{
			Tbl_Action_Record action = AsTableManager.Instance.GetTbl_Action_Record(
				m_FinishMsg.ready_.actionRecord.LinkActionIndex);
			
			m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.SKILL_READY,
				new Msg_Player_Skill_Linkaction(new Msg_Player_Skill_Ready(m_FinishMsg.ready_, action)));
		}
		else
		{
			m_OwnerFsm.Entity.HandleMessage(new Msg_FadeTimeIndicate(m_Action.AniBlendingDuration * 0.001f));
			
			m_OwnerFsm.Entity.HandleMessage(new Msg_CombatBegin());
			
			if(m_OwnerFsm.Entity.CheckCondition(eSkillIcon_Enable_Condition.Bind) == true)
			{
				m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.CONDITION_BINDING, new Msg_Player_Skill_Finish(m_FinishMsg.ready_, false));
			}
			else
			{
				if(m_OwnerFsm.Target != null &&
					(m_OwnerFsm.Target.FsmType == eFsmType.MONSTER ||
					(m_OwnerFsm.Target.FsmType == eFsmType.OTHER_USER && TargetDecider.CheckOtherUserIsEnemy(m_OwnerFsm.Target) == true)) &&
					m_OwnerFsm.Target.GetProperty<bool>(eComponentProperty.LIVING) == true &&
					(Vector3.Distance(m_OwnerFsm.transform.position, m_OwnerFsm.Target.transform.position) <
					m_OwnerFsm.Entity.GetProperty<float>(eComponentProperty.VIEW_DISTANCE)))
				{
					m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.BATTLE_IDLE,
						new Msg_Player_Skill_Finish(m_FinishMsg.ready_, false));
				}
				else
				{
					m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.IDLE,
						new Msg_Player_Skill_Finish(m_FinishMsg.ready_, false));
				}
			}
		}
	}
	
	bool ReleaseCombat()
	{
//		m_PrevTarget = null;
//		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.IDLE);
//		m_TargetReleased = true;
		return false;
	}
	
	bool CancelEnable()
	{
		if(m_Action != null)
		{
			float attSpeed = m_OwnerFsm.Entity.GetProperty<float>(eComponentProperty.ATTACK_SPEED);
			if(m_Action.GetCancelEnable(eActionStep.Finish, (Time.time - m_BeginTime) * attSpeed) == true)
				return true;
		}
		
		return false;
	}
	#endregion
}
