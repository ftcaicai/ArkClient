using UnityEngine;
using System.Collections;

public class AsMonsterState_SkillHit : AsBaseFsmState<AsMonsterFsm.eMonsterFsmStateType, AsMonsterFsm> {
	
	#region - member -
	string m_CurPlayingAnimName;
	
	Msg_NpcAttackChar_Hit m_HitMsg;
	
	Tbl_MonsterSkillLevel_Record m_Skill;
	
	float m_fMaxMove = 0.045f;
	
	Tbl_Action_HitAnimation m_HitAnimation;
	
//	Msg_NpcMoveIndicate m_ReservedMove;
	
//	public static Vector3 debug_MoveBeganPos;
	#endregion
	
	public AsMonsterState_SkillHit(AsMonsterFsm _fsm) : base(AsMonsterFsm.eMonsterFsmStateType.SKILL_HIT, _fsm)
	{
		m_dicMessageTreat.Add(eMessageType.MOVE_NPC_INDICATION, OnBeginMove);
		m_dicMessageTreat.Add(eMessageType.NPC_ATTACK_CHAR1, OnAttackPacketArrived);
		m_dicMessageTreat.Add(eMessageType.ANIMATION_END, OnAnimationEnd);
	}
	
	#region - fsm function -
	public override void Enter(AsIMessage _msg)
	{
		m_OwnerFsm.Entity.HandleMessage(new Msg_MoveStopIndication());
		
		HitProcess(_msg);
	}
	public override void Update()
	{
		if(m_Action != null)
		{
			CheckAnimDuration();
//			AdjustmentMove();
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
		switch(m_HitAnimation.LoopType)
		{
		case eLoopType.Loop:
		case eLoopType.TimeLoop:
		case eLoopType.Cast:
			if(TimeElapsed() == true)
				ChangeStateToFinish();
			break;
		}
	}
	
	
	void AdjustmentMove()
	{				
		Vector3 vec3TempPos = m_OwnerFsm.transform.position + GetMoveDelta();	
		//if( true == TerrainMgr.Instance.IsContainNavMesh( vec3TempPos ) )
			m_OwnerFsm.transform.position = vec3TempPos;
	}
	
	Vector3 GetMoveDelta()
	{
		if( null == m_OwnerFsm.Target )
			return Vector3.zero;
			
		CharacterController charControl = m_OwnerFsm.Target.characterController;
		CharacterController mobControl = m_OwnerFsm.MonsterEntity.characterController;
		float fMobRadius = m_OwnerFsm.MonsterEntity.fCollisionRadius;
		
		Vector3 vec3Diff = charControl.transform.position - mobControl.transform.position;
		if( vec3Diff.magnitude < fMobRadius + charControl.radius )
		{
			vec3Diff.y = 0.0f;
			float fdelta = (fMobRadius + charControl.radius) - vec3Diff.magnitude;
			if( fdelta > m_fMaxMove )
				fdelta = m_fMaxMove;
			 
			return -( vec3Diff.normalized * fdelta );
		}
		
		return Vector3.zero;
	}
	#endregion
	
	#region - ready to HIT -
	void HitProcess(AsIMessage _msg)
	{
		m_HitMsg = _msg as Msg_NpcAttackChar_Hit;
		m_OwnerFsm.Target = AsEntityManager.Instance.GetUserEntityByUniqueId(m_HitMsg.attack_.charUniqKey_[0]);
		
		m_Action = m_HitMsg.action_;
		m_HitAnimation = m_Action.HitAnimation;
		m_Skill = m_HitMsg.skill_;
		
		if(m_HitAnimation != null)
		{
			float animSpeed = GetAnimationSpeedByAttack_NpcEntity(m_Action, eActionStep.Hit);
			m_HitMsg.attack_.SetAnimSpeed( animSpeed);
			
			float timing = m_Action.HitAnimation.hitInfo.HitTiming * 0.001f / m_HitMsg.attack_.animSpeed_;
			AsTimerManager.Instance.SetTimer(timing, SkillNameShout);
			
			switch(m_HitAnimation.LoopType)
			{
			case eLoopType.Once:
			case eLoopType.TargetLoop:
			case eLoopType.Charge:
				break;
			case eLoopType.Loop:
			case eLoopType.TimeLoop:
			case eLoopType.Cast:
				SetTimer(m_HitAnimation.LoopDuration * 0.001f / m_HitMsg.attack_.animSpeed_);
				break;
			}
			
			Msg_AnimationIndicate anim = null;
			switch(m_HitAnimation.LoopType)
			{
			case eLoopType.Once:
			case eLoopType.Loop:
			case eLoopType.TimeLoop:
			case eLoopType.Cast:
			case eLoopType.Charge:
				anim = new Msg_AnimationIndicate(m_HitAnimation.FileName, 0.1f, m_HitMsg.attack_.animSpeed_);
				break;
			case eLoopType.TargetLoop:
				anim = new Msg_AnimationIndicate(m_HitAnimation.FileName, 0.1f, m_HitMsg.attack_.animSpeed_,
					m_HitAnimation.LoopTargetTime, m_HitAnimation.LoopDuration);
				break;
			default:
				Debug.LogError("AsMonsterState_SkillHit::HitProcess: invalid hit animation process. m_HitAnimation.LoopType = " + m_HitAnimation.LoopType);
				ChangeStateToFinish();
				break;
			}
			
			m_CurPlayingAnimName = m_HitAnimation.FileName;
			anim.SetActionAnimation(m_HitAnimation);
			m_OwnerFsm.MonsterEntity.HandleMessage(anim);
			
			if(m_Action.AniBlendStep == eActionStep.Hit)
				m_OwnerFsm.Entity.HandleMessage(new Msg_FadeTimeIndicate(m_Action.AniBlendingDuration * 0.001f));
			
			for( int i=0; i<m_HitMsg.attack_.charUniqKey_.Length; ++i)
			{
				uint node = m_HitMsg.attack_.charUniqKey_[i];
				AsUserEntity user = AsEntityManager.Instance.GetUserEntityByUniqueId( node);

				if( user != null)
					m_OwnerFsm.PlayElements(eActionStep.Hit, m_HitMsg.attack_.targetPos_, user.transform, m_HitMsg.attack_.animSpeed_);
				else if( i == 0)
					m_OwnerFsm.PlayElements(eActionStep.Hit, m_HitMsg.attack_.targetPos_, null, m_HitMsg.attack_.animSpeed_);
			}
		}
		else
		{
			ChangeStateToFinish();
		}
		
		ActionMove();
		
		GenerateMissile();
	}
	
	void ActionMove()
	{
//		return;
		
		Msg_DashIndication dashIndication = null;
		
		float attSpeed = m_HitMsg.attack_.animSpeed_;
			
		switch(m_Action.HitAnimation.MoveType)
		{
		case eHitMoveType.Dash:
			Debug.Log("AsMonsterState_SkillReady::ActionMove: m_Action.HitAnimation.MoveType == " + m_Action.HitAnimation.MoveType + ". m_OwnerFsm.transform.forward = " + m_OwnerFsm.transform.forward);
			dashIndication = new Msg_DashIndication(
				m_Action.HitAnimation.LoopDuration / attSpeed, m_Action.HitAnimation.MoveDistance * 0.01f,
				m_OwnerFsm.transform.forward, m_OwnerFsm.transform.position);
			Debug.Log("AsMonsterState_SkillReady::ActionMove: ");
			break;
		case eHitMoveType.TargetDash:
			if(m_OwnerFsm.Target != null)
			{
				if(m_OwnerFsm.Entity.ModelObjectAnimation == null)
					return;
				
				AnimationState animState = m_OwnerFsm.Entity.ModelObjectAnimation[m_Action.HitAnimation.FileName];
				if(animState != null)
				{
					dashIndication = new Msg_DashIndication(
						animState.length * 1000 / attSpeed, m_OwnerFsm.targetDistance - 1,
						m_OwnerFsm.transform.forward, m_OwnerFsm.transform.position);
				}
			}
			break;
		case eHitMoveType.BackTargetDash:
			if(m_OwnerFsm.Target != null)
			{
				if(m_OwnerFsm.Entity.ModelObjectAnimation == null)
					return;
				
				AnimationState animState = m_OwnerFsm.Entity.ModelObjectAnimation[m_Action.HitAnimation.FileName];
				if(animState != null)
				{
					dashIndication = new Msg_DashBackIndication(
						animState.length * 1000 / attSpeed,m_Action.HitAnimation.MoveDistance * 0.01f,
						-m_OwnerFsm.transform.forward, m_OwnerFsm.transform.position);
				}
			}
			break;
		default:
			return;
		}
		
		if(dashIndication != null)
			m_OwnerFsm.Entity.HandleMessage(dashIndication);
	}
	#endregion
	
	#region - msg -
	void OnBeginMove(AsIMessage _msg)
	{
//		m_ReservedMove = _msg as Msg_NpcMoveIndicate;
//		m_OwnerFsm.SetLastMoveIndication(_msg as Msg_NpcMoveIndicate);
		
		Msg_NpcMoveIndicate move = _msg as Msg_NpcMoveIndicate;
		if(move.forceMyself_ == true)
			m_OwnerFsm.SetLastMoveIndication(_msg as Msg_NpcMoveIndicate);
		else
			m_OwnerFsm.SetMonsterFsmState(AsMonsterFsm.eMonsterFsmStateType.RUN, _msg);
		
//		float attSpeed = m_HitMsg.attack_.animSpeed_;
//		
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
		m_OwnerFsm.MonsterEntity.HandleMessage(new Msg_HitExecution());
		m_OwnerFsm.SetMonsterFsmState(AsMonsterFsm.eMonsterFsmStateType.SKILL_READY, _msg);
	}
	
	void OnAnimationEnd(AsIMessage _msg)
	{
		Msg_AnimationEnd end = _msg as Msg_AnimationEnd;
		
//		float time = Time.time;
//		string __class = m_OwnerFsm.Entity.GetProperty<string>(eComponentProperty.CLASS);
//		if((__class == "Gust Harpy" || __class == "Breeze Harpy") && m_Action.ActionName == "SkillTornado")
//		{
//			Debug.Log("AsMonsterState_SkillHit::OnAnimationEnd: animation ended");
//			Debug.Log("end.animName_ = " + end.animName_ + ", m_CurPlayingAnimName = " + m_CurPlayingAnimName);
//			Debug.Log("m_HitAnimation.LoopType = " + m_HitAnimation.LoopType);
//		}
		
		if(end.animName_ == m_CurPlayingAnimName && 
//			(m_Action.HitAni_LoopType == eHitAniLoopType.NONE ||
//			m_Action.HitAni_LoopType == eHitAniLoopType.Once))
			(m_HitAnimation.LoopType & (eLoopType.NONE | eLoopType.Once | eLoopType.TargetLoop)) != 0)
		{
//			if((__class == "Gust Harpy" || __class == "Breeze Harpy") && m_Action.ActionName == "SkillTornado")
//			Debug.Log("AsMonsterState_SkillHit::OnAnimationEnd: change state to FINISH. " + (time - Time.time) + " passed");
			
			ChangeStateToFinish();
		}
	}
	#endregion
	
	#region - method -
	void ChangeStateToFinish()
	{
		ReleaseTimer();
		Msg_NpcAttackChar_Finish finish = new Msg_NpcAttackChar_Finish(m_HitMsg.attack_, m_Action, m_Skill);
		m_OwnerFsm.SetMonsterFsmState(AsMonsterFsm.eMonsterFsmStateType.SKILL_FINISH, finish);
	}
	
	bool ReleaseCombat()
	{
		m_OwnerFsm.Target = null;
		m_OwnerFsm.SetMonsterFsmState(AsMonsterFsm.eMonsterFsmStateType.IDLE);
		return false;
	}
	#endregion
	
	#region - timer -
	void Timer_HitExecution(System.Object _obj)
	{
	}
	
	void Timer_GenerateMissile(System.Object _obj)
	{
		if( null == m_HitMsg)
		{
			Debug.LogError( "m_HitMsg ================= null");
			return;
		}
		
		if(m_OwnerFsm.Target != null && m_OwnerFsm.Target.ModelObject != null &&
			m_OwnerFsm.Entity != null && m_OwnerFsm.Entity.ModelObject != null)
		{
			Tbl_Action_HitInfo hitInfo = m_HitMsg.action_.HitAnimation.hitInfo;
			
//			AsEffectManager.Instance.ShootingEffect(
//				hitInfo.HitProjectileName, m_OwnerFsm.Entity.ModelObject.transform, m_OwnerFsm.Target.ModelObject.transform, Missile_HitExecution,
//				hitInfo.HitProjectileSpeed, hitInfo.HitProjectileAccel, hitInfo.HitProjectilePath, m_OwnerFsm.Entity.transform.localScale.y, hitInfo.HitProjectileHitSoundPath);
			
			foreach(uint node in m_HitMsg.attack_.charUniqKey_)
			{
				AsUserEntity user = AsEntityManager.Instance.GetUserEntityByUniqueId( node);
				if( user != null)
				{
					if( user.ModelObject != null)
					{
						AsEffectManager.Instance.ShootingEffect(
							hitInfo.HitProjectileName, m_OwnerFsm.Entity.ModelObject.transform, user.ModelObject.transform, Missile_HitExecution,
							hitInfo.HitProjectileSpeed, hitInfo.HitProjectileAccel, hitInfo.HitProjectilePath, m_OwnerFsm.Entity.transform.localScale.y, hitInfo.HitProjectileHitSoundPath);
					}
				}
			}
		}
	}
	
	//missile
	void GenerateMissile()
	{
		// projectile target
		Tbl_Action_HitInfo hitInfo = m_HitMsg.action_.HitAnimation.hitInfo;
		
		if(hitInfo.HitType == eHitType.ProjectileTarget)
		{
			AsTimerManager.Instance.SetTimer(hitInfo.HitTiming * 0.001f, Timer_GenerateMissile);
		}
	}
	
	void Missile_HitExecution(System.Object _obj)
	{
		m_OwnerFsm.Entity.HandleMessage(new Msg_HitExecution());
	}
	#endregion
	
	#region - skill name shout -
	void SkillNameShout(System.Object _obj)
	{
		if(m_HitMsg.attack_.skill_.SkillName_Print != eSkillNamePrint.None)
		{
			string skillName = AsTableManager.Instance.GetTbl_String(m_HitMsg.attack_.skill_.SkillName_Index);
			m_OwnerFsm.Entity.SkillNameShout(eSkillNameShoutType.Self, skillName);
		}
	}
	#endregion
}
