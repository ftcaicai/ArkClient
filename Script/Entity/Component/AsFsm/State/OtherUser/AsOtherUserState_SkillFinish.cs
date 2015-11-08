using UnityEngine;
using System.Collections;

public class AsOtherUserState_SkillFinish : AsBaseFsmState<AsOtherUserFsm.eOtherUserFsmStateType, AsOtherUserFsm> {
	
	#region - member -
	string m_CurPlayingAnimName;
	
	Msg_OtherCharAttackNpc_Finish m_FinishMsg;
	
	Tbl_Action_Animation m_FinishAnimation;
	#endregion
	
	public AsOtherUserState_SkillFinish(AsOtherUserFsm _fsm) : base(AsOtherUserFsm.eOtherUserFsmStateType.SKILL_FINISH, _fsm)
	{
		m_dicMessageTreat.Add(eMessageType.MOVE_OTHER_USER_INDICATION, OnBeginMove);
		m_dicMessageTreat.Add(eMessageType.CHAR_ATTACK_NPC1, OnBeginSkill);
		m_dicMessageTreat.Add(eMessageType.ANIMATION_END, OnAnimationEnd);
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
//		RefreshDirection();
		if(m_FinishMsg.hit_.ready_.skillLvRecord != null)
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
		if(m_FinishMsg.hit_.ready_.actionRecord.FinishAnimation.LoopType == eLoopType.TimeLoop)
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
		m_FinishMsg = _msg as Msg_OtherCharAttackNpc_Finish;
		m_Action = m_FinishMsg.hit_.ready_.actionRecord;
		m_FinishAnimation = m_Action.FinishAnimation;
		
		if(m_FinishAnimation != null)
		{
			float animSpeed = GetAnimationSpeedByAttack_UserEntity(m_Action, eActionStep.Finish);
			m_FinishMsg.hit_.ready_.SetAnimSpeed(animSpeed);
			
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
				anim = new Msg_AnimationIndicate(m_FinishAnimation.FileName, 0.1f, m_FinishMsg.hit_.ready_.animSpeed);
				break;
			case eLoopType.TargetLoop:
				anim = new Msg_AnimationIndicate(m_FinishAnimation.FileName, 0.1f, m_FinishMsg.hit_.ready_.animSpeed,
					m_FinishAnimation.LoopTargetTime, m_FinishAnimation.LoopDuration);
				break;
			default:
				Debug.LogError("AsOtherUserState_SkillFinish:: FinishProcess: invalid finish animation process. m_FinishAnimation.LoopType = " + m_FinishAnimation.LoopType);
				FinishStateEnd();
				return;
			}
			
			m_CurPlayingAnimName = m_FinishAnimation.FileName;
			m_OwnerFsm.OtherUserEntity.HandleMessage(anim);			
			
			if(m_Action.AniBlendStep == eActionStep.Finish)
				m_OwnerFsm.Entity.HandleMessage(new Msg_FadeTimeIndicate(m_Action.AniBlendingDuration * 0.001f));
			
			if(m_OwnerFsm.Target != null)
				m_OwnerFsm.PlayElements(eActionStep.Finish, m_FinishMsg.hit_.ready_.attackMsg.targeting_,
					m_OwnerFsm.Target.transform, m_FinishMsg.hit_.ready_.animSpeed);
			else
				m_OwnerFsm.PlayElements(eActionStep.Finish, m_FinishMsg.hit_.ready_.attackMsg.targeting_,
					null, m_FinishMsg.hit_.ready_.animSpeed);
		}
		else
		{
			FinishStateEnd();
		}
	}
	#endregion
	
	#region - msg -
	void OnBeginMove(AsIMessage _msg)
	{
		m_OwnerFsm.SetOtherUserFsmState(AsOtherUserFsm.eOtherUserFsmStateType.RUN, _msg);
	}
	
	void OnBeginSkill(AsIMessage _msg)
	{
		m_OwnerFsm.SetOtherUserFsmState(AsOtherUserFsm.eOtherUserFsmStateType.SKILL_READY, _msg);
	}
	
	void OnAnimationEnd(AsIMessage _msg)
	{
		Msg_AnimationEnd end = _msg as Msg_AnimationEnd;
		if(end.animName_ == m_CurPlayingAnimName && 
			(m_FinishMsg.hit_.ready_.actionRecord.FinishAnimation.LoopType & (eLoopType.NONE | eLoopType.Once | eLoopType.TargetLoop)) != 0)
		{
			FinishStateEnd();
		}
	}
	#endregion
	
	#region - method -
	void FinishStateEnd()
	{
		ReleaseTimer();
		
		m_OwnerFsm.Entity.HandleMessage(new Msg_FadeTimeIndicate(m_Action.AniBlendingDuration * 0.001f));
		
		m_OwnerFsm.SetOtherUserFsmState(AsOtherUserFsm.eOtherUserFsmStateType.IDLE);
		
//		if(m_FinishMsg.hit_.ready_.actionRecord.LinkActionIndex != 0)
//		{
//			m_OwnerFsm.SetOtherUserFsmState(AsOtherUserFsm.eOtherUserFsmStateType.SKILL_READY,
//				new Msg_OtherCharAttackNpc_Link(m_FinishMsg.hit_.ready_));
//		}
//		else
//		{
//			m_OwnerFsm.SetOtherUserFsmState(AsOtherUserFsm.eOtherUserFsmStateType.IDLE);
//		}
	}
	#endregion
	
//	#endregion
}
