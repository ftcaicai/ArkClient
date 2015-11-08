using UnityEngine;
using System.Collections;

public class AsOtherUserState_SkillHit : AsBaseFsmState<AsOtherUserFsm.eOtherUserFsmStateType, AsOtherUserFsm> {
	
	#region - member -
	string m_CurPlayingAnimName;
	
	Msg_OtherCharAttackNpc_Hit m_HitMsg;
	
	Tbl_Action_HitAnimation m_HitAnimation;
	
	AsTimer m_SkillTimer = null;
	AsTimer m_SkillProjTimer = null;
	#endregion
	
	public AsOtherUserState_SkillHit(AsOtherUserFsm _fsm) : base(AsOtherUserFsm.eOtherUserFsmStateType.SKILL_HIT, _fsm)
	{
		m_dicMessageTreat.Add(eMessageType.MOVE_OTHER_USER_INDICATION, OnBeginMove);
		m_dicMessageTreat.Add(eMessageType.OTHER_CHAR_ATTACK_READY, OnBeginSkill);
		m_dicMessageTreat.Add(eMessageType.ANIMATION_END, OnAnimationEnd);
	}
	
	#region - fsm function -
	public override void Enter(AsIMessage _msg)
	{
		m_SkillTimer = null;
		m_SkillProjTimer = null;
		
		m_OwnerFsm.Entity.HandleMessage(new Msg_MoveStopIndication());
//		m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("Idle"));
		
		HitProcess(_msg);
	}
	
	public override void Update()
	{
		if(m_HitMsg.ready_.actionRecord != null)
		{
			CheckAnimDuration();
//			ActionMove();
		}
	}
	public override void Exit()
	{
		if(m_SkillTimer != null) m_SkillTimer.Release();
		if(m_SkillProjTimer != null) m_SkillProjTimer.Release();
		
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
		switch(m_HitMsg.ready_.actionRecord.HitAnimation.LoopType)
		{
		case eLoopType.Loop:
		case eLoopType.TimeLoop:
		case eLoopType.Cast:
			if(TimeElapsed() == true)
				ChangeStateToFinish();
			break;
		}
	}
	#endregion
	
	#region - msg -
	void OnBeginMove(AsIMessage _msg)
	{
//		Msg_NpcMoveIndicate move = _msg as Msg_NpcMoveIndicate;
//		m_OwnerFsm.SetOtherUserFsmState(AsOtherUserFsm.eOtherUserFsmStateType.RUN, _msg);
	}
	
	void OnBeginSkill(AsIMessage _msg)
	{
		m_OwnerFsm.SetOtherUserFsmState(AsOtherUserFsm.eOtherUserFsmStateType.SKILL_READY, _msg);
	}
	
	void OnAnimationEnd(AsIMessage _msg)
	{
		Msg_AnimationEnd end = _msg as Msg_AnimationEnd;
		if(end.animName_ == m_CurPlayingAnimName && 
//			(m_HitMsg.ready_.actionRecord.HitAni_LoopType == eHitAniLoopType.NONE ||
//			m_HitMsg.ready_.actionRecord.HitAni_LoopType == eHitAniLoopType.Once))
			(m_HitMsg.ready_.actionRecord.HitAnimation.LoopType & (eLoopType.NONE | eLoopType.Once | eLoopType.TargetLoop)) != 0)
//			(m_HitMsg.ready_.actionRecord.HitAni_LoopType & (eHitAniLoopType.NONE | eHitAniLoopType.Once)) != 0)
		{
//			Debug.Log(m_OwnerFsm.OtherUserEntity.SessionId + ": other use skill hit : ANIMATION END");
			ChangeStateToFinish();
		}
	}
	#endregion
	
	#region - ready to HIT -
	void HitProcess(AsIMessage _msg)
	{
		m_HitMsg = _msg as Msg_OtherCharAttackNpc_Hit;
		m_Action = m_HitMsg.ready_.actionRecord;
		m_HitAnimation = m_Action.HitAnimation;
			
		if(m_HitAnimation != null)
		{
			float animSpeed = GetAnimationSpeedByAttack_UserEntity(m_Action, eActionStep.Hit);
			m_HitMsg.ready_.SetAnimSpeed(animSpeed);
			
			float timing = m_Action.HitAnimation.hitInfo.HitTiming * 0.001f / m_HitMsg.ready_.animSpeed;
			m_SkillTimer = AsTimerManager.Instance.SetTimer(timing, SkillNameShout);
			
			switch(m_HitAnimation.LoopType)
			{
			case eLoopType.Once:
			case eLoopType.TargetLoop:
			case eLoopType.Charge:
				break;
			case eLoopType.Loop:
			case eLoopType.TimeLoop:
			case eLoopType.Cast:
				SetTimer(m_HitAnimation.LoopDuration * 0.001f / m_HitMsg.ready_.animSpeed);
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
				anim = new Msg_AnimationIndicate(m_HitAnimation.FileName, 0.1f, m_HitMsg.ready_.animSpeed);
				break;
			case eLoopType.TargetLoop:
				anim = new Msg_AnimationIndicate(m_HitAnimation.FileName, 0.1f, m_HitMsg.ready_.animSpeed,
					m_HitAnimation.LoopTargetTime, m_HitAnimation.LoopDuration);
				break;
			default:
				Debug.LogError("AsOtherUserState_SkillHit:: HitProcess: invalid hit animation process. m_HitAnimation.LoopType = " + m_HitAnimation.LoopType);
				ChangeStateToFinish();
				return;
			}
			
			m_CurPlayingAnimName = m_HitAnimation.FileName;
			m_OwnerFsm.OtherUserEntity.HandleMessage(anim);			
			
			if(m_Action.AniBlendStep == eActionStep.Hit)
				m_OwnerFsm.Entity.HandleMessage(new Msg_FadeTimeIndicate(m_Action.AniBlendingDuration * 0.001f));
			
			for( int i=0; i<m_HitMsg.ready_.attackMsg.npcIdx_.Length; ++i)
			{
				int node = m_HitMsg.ready_.attackMsg.npcIdx_[i];
				AsNpcEntity npc = AsEntityManager.Instance.GetNpcEntityBySessionId( node);

				if( npc != null)
					m_OwnerFsm.PlayElements(eActionStep.Hit, m_HitMsg.ready_.attackMsg.targeting_, npc.transform, m_HitMsg.ready_.animSpeed);
				else if( i == 0)
					m_OwnerFsm.PlayElements(eActionStep.Hit, m_HitMsg.ready_.attackMsg.targeting_, null, m_HitMsg.ready_.animSpeed);
			}

			for( int i=0; i<m_HitMsg.ready_.attackMsg.mainCharUniqKey_.Length; ++i)
			{
				uint node = m_HitMsg.ready_.attackMsg.mainCharUniqKey_[i];
				AsUserEntity user = AsEntityManager.Instance.GetUserEntityByUniqueId( node);

				if( user != null)
					m_OwnerFsm.PlayElements(eActionStep.Hit, m_HitMsg.ready_.attackMsg.targeting_, user.transform, m_HitMsg.ready_.animSpeed);
				else if( i == 0)
					m_OwnerFsm.PlayElements(eActionStep.Hit, m_HitMsg.ready_.attackMsg.targeting_, null, m_HitMsg.ready_.animSpeed);
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
		Msg_DashIndication dashIndication = null;
		
		float attSpeed = m_HitMsg.ready_.animSpeed;
			
		switch(m_Action.HitAnimation.MoveType)
		{
		case eHitMoveType.Dash:
			dashIndication = new Msg_DashIndication(
				m_Action.HitAnimation.LoopDuration / attSpeed, m_Action.HitAnimation.MoveDistance * 0.01f,
				m_OwnerFsm.transform.forward, m_OwnerFsm.transform.position);
			break;
		case eHitMoveType.TargetDash:
			if(m_OwnerFsm.Entity.ModelObjectAnimation == null)
				return;
			
			if(m_OwnerFsm.Target != null)
			{				
				AnimationState animState = m_OwnerFsm.Entity.ModelObjectAnimation[m_Action.HitAnimation.FileName];
				if(animState != null)
				{
					dashIndication = new Msg_DashIndication(
						animState.length * 1000 / attSpeed, m_OwnerFsm.targetDistance - 0.3f,
						m_OwnerFsm.transform.forward, m_OwnerFsm.transform.position);
				}
			}
			break;
		case eHitMoveType.BackTargetDash:
			if(m_OwnerFsm.Entity.ModelObjectAnimation == null)
				return;
			
			if(m_OwnerFsm.Target != null)
			{				
				AnimationState animState = m_OwnerFsm.Entity.ModelObjectAnimation[m_Action.HitAnimation.FileName];
				if(animState != null)
				{
					dashIndication = new Msg_DashBackIndication(
						animState.length * 1000 / attSpeed, m_Action.HitAnimation.MoveDistance * 0.01f,
						-m_OwnerFsm.transform.forward, m_OwnerFsm.transform.position);
				}
			}
			break;
		case eHitMoveType.TabDash:			
			if(m_OwnerFsm.Entity.ModelObjectAnimation == null)
				return;
			
			float dist = Vector3.Distance(m_OwnerFsm.transform.position, m_HitMsg.ready_.attackMsg.targeting_);		
			AnimationState animState = m_OwnerFsm.Entity.ModelObjectAnimation[m_Action.HitAnimation.FileName];
			Debug.Log("dist:" + dist + ", animState:" + animState);
			if(animState != null)
			{
				dashIndication = new Msg_DashIndication(
					animState.length * 1000 / attSpeed, dist - 1,
					m_OwnerFsm.transform.forward, m_OwnerFsm.transform.position);
				Debug.Log("dist:" + dist + ", animState.length * 1000 / attSpeed:" + animState.length * 1000 / attSpeed);
			}
			break;
		case eHitMoveType.TabWarp:
			dist = Vector3.Distance(m_OwnerFsm.transform.position, m_HitMsg.ready_.attackMsg.targeting_);
			Msg_WarpIndication warp = new Msg_WarpIndication(dist, m_OwnerFsm.transform.forward, m_OwnerFsm.transform.position);
			m_OwnerFsm.Entity.HandleMessage(warp);
			break;
		default:
			return;
		}
		
		if(dashIndication != null)
			m_OwnerFsm.Entity.HandleMessage(dashIndication);
	}
	#endregion
	
	#region - method -
	void ChangeStateToFinish()
	{
		ReleaseTimer();
		Msg_OtherCharAttackNpc_Finish finish = new Msg_OtherCharAttackNpc_Finish(m_HitMsg);
		m_OwnerFsm.SetOtherUserFsmState(AsOtherUserFsm.eOtherUserFsmStateType.SKILL_FINISH, finish);
	}
	
	bool ReleaseCombat()
	{
		m_OwnerFsm.Target = null;
//		m_OwnerFsm.SetOtherUserFsmState(AsOtherUserFsm.eOtherUserFsmStateType.IDLE);
		return false;
	}
	#endregion
	
	#region - timer -
	void Timer_HitExecution(System.Object _obj)
	{
		m_OwnerFsm.OtherUserEntity.HandleMessage(new Msg_HitExecution());
	}
	
	void Timer_GenerateMissile(System.Object _obj)
	{		
		if(m_OwnerFsm.Target != null && m_OwnerFsm.Target.ModelObject != null &&
			m_OwnerFsm.Entity != null && m_OwnerFsm.Entity.ModelObject != null)
		{
			Tbl_Action_HitInfo hitInfo = m_HitMsg.ready_.actionRecord.HitAnimation.hitInfo;
			
			try
			{				
//				AsEffectManager.Instance.ShootingEffect(
//					hitInfo.HitProjectileName, m_OwnerFsm.Entity.ModelObject.transform, m_OwnerFsm.Target.ModelObject.transform, Missile_HitExecution,
//					hitInfo.HitProjectileSpeed, hitInfo.HitProjectileAccel, hitInfo.HitProjectilePath, m_OwnerFsm.Entity.transform.localScale.y, hitInfo.HitProjectileHitSoundPath);
				
				foreach(int node in m_HitMsg.ready_.attackMsg.npcIdx_)
				{
					AsNpcEntity npc = AsEntityManager.Instance.GetNpcEntityBySessionId( node);
					if( npc != null)
					{
						if( npc.ModelObject != null)
						{
							AsEffectManager.Instance.ShootingEffect(
								hitInfo.HitProjectileName, m_OwnerFsm.Entity.ModelObject.transform, npc.ModelObject.transform, Missile_HitExecution,
								hitInfo.HitProjectileSpeed, hitInfo.HitProjectileAccel,
								hitInfo.HitProjectilePath, m_OwnerFsm.Entity.transform.localScale.y, hitInfo.HitProjectileHitSoundPath);
						}
					}
				}
				
				foreach(uint node in m_HitMsg.ready_.attackMsg.mainCharUniqKey_)
				{
					AsUserEntity user = AsEntityManager.Instance.GetUserEntityByUniqueId( node);
					if( user != null)
					{
						if( user.ModelObject != null)
						{
							AsEffectManager.Instance.ShootingEffect(
								hitInfo.HitProjectileName, m_OwnerFsm.Entity.ModelObject.transform, user.ModelObject.transform, Missile_HitExecution,
								hitInfo.HitProjectileSpeed, hitInfo.HitProjectileAccel,
								hitInfo.HitProjectilePath, m_OwnerFsm.Entity.transform.localScale.y, hitInfo.HitProjectileHitSoundPath);
						}
					}
				}
			}
			catch
			{
				Debug.LogError("AsOtherUserState_SkillHit::Timer_GenerateMissile: hitInfo.HitProjectileName = " + m_HitMsg.ready_.actionRecord.HitAnimation.FileName);
			}
		}
		
	}
	
	//missile
	void GenerateMissile()
	{
		// projectile target
		Tbl_Action_HitInfo hitInfo = m_HitMsg.ready_.actionRecord.HitAnimation.hitInfo;
		
		if(hitInfo.HitType == eHitType.ProjectileTarget)
		{
			m_SkillProjTimer = AsTimerManager.Instance.SetTimer(hitInfo.HitTiming * 0.001f, Timer_GenerateMissile);
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
		if(m_HitMsg.ready_.skillRecord.SkillName_Print != eSkillNamePrint.None)
		{
			string skillName = AsTableManager.Instance.GetTbl_String(m_HitMsg.ready_.skillRecord.SkillName_Index);
			m_OwnerFsm.Entity.SkillNameShout(eSkillNameShoutType.Self, skillName);
		}
	}
	#endregion
}
