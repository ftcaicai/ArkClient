using UnityEngine;
using System.Collections;

public class AsMonsterState_SkillFinish : AsBaseFsmState<AsMonsterFsm.eMonsterFsmStateType, AsMonsterFsm> {
	
	#region - member -
	string m_CurPlayingAnimName;
	
	Msg_NpcAttackChar_Finish m_FinishMsg;
	
	Tbl_Action_Animation m_FinishAnimation;
	#endregion
	
	public AsMonsterState_SkillFinish(AsMonsterFsm _fsm) : base(AsMonsterFsm.eMonsterFsmStateType.SKILL_FINISH, _fsm)
	{
		m_dicMessageTreat.Add(eMessageType.MOVE_NPC_INDICATION, OnBeginMove);
		m_dicMessageTreat.Add(eMessageType.NPC_ATTACK_CHAR1, OnAttackPacketArrived);
		m_dicMessageTreat.Add(eMessageType.ANIMATION_END, OnAnimationEnd);
	}
	
	#region - fsm function -
	public override void Enter(AsIMessage _msg)
	{
		m_OwnerFsm.Entity.HandleMessage(new Msg_MoveStopIndication());

		FinishProcess(_msg);
	}
	public override void Update()
	{
//		RefreshDirection();
		if(m_Action != null)
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
		if(m_Action.FinishAnimation.LoopType == eLoopType.TimeLoop)
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
		m_FinishMsg = _msg as Msg_NpcAttackChar_Finish;
		m_OwnerFsm.Target = AsEntityManager.Instance.GetUserEntityByUniqueId(m_FinishMsg.attack_.charUniqKey_[0]);
		
		m_Action = m_FinishMsg.action_;
		m_FinishAnimation = m_Action.FinishAnimation;
		
		if(m_Action.FinishAnimation != null)
		{
			float animSpeed = GetAnimationSpeedByAttack_NpcEntity(m_Action, eActionStep.Finish);
			m_FinishMsg.attack_.SetAnimSpeed( animSpeed);
			
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
				anim = new Msg_AnimationIndicate(m_FinishAnimation.FileName, 0.1f, m_FinishMsg.attack_.animSpeed_);
				break;
			case eLoopType.TargetLoop:
				anim = new Msg_AnimationIndicate(m_FinishAnimation.FileName, 0.1f, m_FinishMsg.attack_.animSpeed_,
					m_FinishAnimation.LoopTargetTime, m_FinishAnimation.LoopDuration);
				break;
			default:
				Debug.LogError("AsMonsterState_SkillFinish:: FinishProcess: invalid finish animation process. m_FinishAnimation.LoopType = " + m_FinishAnimation.LoopType);
				FinishStateEnd();
				break;
			}
			
			m_CurPlayingAnimName = m_FinishAnimation.FileName;
			m_OwnerFsm.MonsterEntity.HandleMessage(anim);			
			
			if(m_Action.AniBlendStep == eActionStep.Finish)
				m_OwnerFsm.Entity.HandleMessage(new Msg_FadeTimeIndicate(m_Action.AniBlendingDuration * 0.001f));
			
//			if(m_FinishMsg.attack_.target_ != null)
//				m_OwnerFsm.PlayElements(eActionStep.Finish, m_FinishMsg.attack_.targetPos_, m_OwnerFsm.Target.transform, m_FinishMsg.attack_.animSpeed_);
//			else
//				m_OwnerFsm.PlayElements(eActionStep.Finish, m_FinishMsg.attack_.animSpeed_);
			
			for( int i=0; i<m_FinishMsg.attack_.charUniqKey_.Length; ++i)
			{
				uint node = m_FinishMsg.attack_.charUniqKey_[i];
				AsUserEntity user = AsEntityManager.Instance.GetUserEntityByUniqueId( node);

				if( user != null)
					m_OwnerFsm.PlayElements(eActionStep.Finish,  m_FinishMsg.attack_.targetPos_, user.transform, m_FinishMsg.attack_.animSpeed_);
				else if( i == 0)
					m_OwnerFsm.PlayElements(eActionStep.Finish, m_FinishMsg.attack_.targetPos_, null, m_FinishMsg.attack_.animSpeed_);
			}
		}
		else
		{
			FinishStateEnd();
		}
	}
	
	void FinishStateEnd()
	{
//		if(m_FinishMsg.move_ != null && m_FinishMsg.move_.forceMove_ == true)
//		{
//			m_OwnerFsm.Entity.SetRePosition(m_FinishMsg.move_.targetPosition_);
//		}
		if(m_OwnerFsm.CheckLastMoveIndication_ForcedMove() == true)
		{
//			Debug.Log("AsMonsterState_SkillFinish::Enter: AsMonsterState_SkillHit.debug_MoveBeganPos = " + AsMonsterState_SkillHit.debug_MoveBeganPos);
//			Debug.Log("AsMonsterState_SkillFinish::Enter: last forced move indication = " + m_OwnerFsm.LastMoveIndication.targetPosition_);
//			
//			Vector3 reservedPos = AsMonsterState_SkillHit.debug_MoveBeganPos;
//			Vector3 modifiedPos = reservedPos; modifiedPos.y = 0;
//			
//			Debug.Log("AsMonsterState_SkillFinish::Enter: distance in client = " + Vector3.Distance(reservedPos, m_OwnerFsm.transform.position));
//			Debug.Log("AsMonsterState_SkillFinish::Enter: distance from server = " + Vector3.Distance(modifiedPos, m_OwnerFsm.LastMoveIndication.targetPosition_));			
			
			m_OwnerFsm.Entity.SetRePosition(m_OwnerFsm.LastMoveIndication.targetPosition_);
			m_OwnerFsm.LastMoveIndicationUsed();
		}
		
		ReleaseTimer();
		
		m_OwnerFsm.Entity.HandleMessage(new Msg_FadeTimeIndicate(m_Action.AniBlendingDuration * 0.001f));
		
		m_OwnerFsm.SetMonsterFsmState(AsMonsterFsm.eMonsterFsmStateType.IDLE);
		
//		if(m_Action.LinkActionIndex != int.MaxValue)
//		{
//			m_OwnerFsm.SetMonsterFsmState(AsMonsterFsm.eMonsterFsmStateType.SKILL_READY, new Msg_NpcAttackChar_Link( m_FinishMsg.attack_));
//		}
//		else
//		{
//			m_OwnerFsm.SetMonsterFsmState(AsMonsterFsm.eMonsterFsmStateType.IDLE);
//		}
	}
	#endregion
	
	#region - msg -
	void OnBeginMove(AsIMessage _msg)
	{
//		float attSpeed = m_FinishMsg.attack_.animSpeed_;
//		m_OwnerFsm.SetLastMoveIndication(_msg as Msg_NpcMoveIndicate);
		
		Msg_NpcMoveIndicate move = _msg as Msg_NpcMoveIndicate;
		if(move.forceMyself_ == true)
			m_OwnerFsm.SetLastMoveIndication(_msg as Msg_NpcMoveIndicate);
		else
			m_OwnerFsm.SetMonsterFsmState(AsMonsterFsm.eMonsterFsmStateType.RUN, _msg);
		
//		Msg_NpcMoveIndicate move = _msg as Msg_NpcMoveIndicate;
//		if(move.forceMove_ == true)
//		{
//			Msg_DashIndication dashIndication = new Msg_DashIndication(
//				m_Action.HitAnimation.LoopDuration / attSpeed, m_Action.HitAnimation.MoveDistance * 0.01f,
//				m_OwnerFsm.transform.forward, m_OwnerFsm.transform.position);
//			
//			m_OwnerFsm.Entity.HandleMessage(dashIndication);
//		}
//		else
//			m_OwnerFsm.SetMonsterFsmState(AsMonsterFsm.eMonsterFsmStateType.RUN, _msg);
	}
	
	void OnAttackPacketArrived(AsIMessage _msg)
	{
		m_OwnerFsm.SetMonsterFsmState(AsMonsterFsm.eMonsterFsmStateType.SKILL_READY, _msg);
	}
	
	void OnAnimationEnd(AsIMessage _msg)
	{
		Msg_AnimationEnd end = _msg as Msg_AnimationEnd;
		if(end.animName_ == m_CurPlayingAnimName && 
			(m_FinishAnimation.LoopType & (eLoopType.NONE | eLoopType.Once | eLoopType.TargetLoop)) != 0)
		{
			FinishStateEnd();
		}
	}
	#endregion
}
