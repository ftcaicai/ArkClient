using UnityEngine;
using System.Collections;

public class AsPetState_SkillHit : AsBaseFsmState<AsPetFsm.ePetFsmStateType, AsPetFsm> {
	
	#region - member -
	string m_CurPlayingAnimName;
	
	Msg_Pet_Skill_Ready m_Ready;
	
	Tbl_Action_Animation m_HitAnimation;
	
	AsTimer m_SkillTimer = null;
	#endregion
	
	public AsPetState_SkillHit(AsPetFsm _fsm) : base(AsPetFsm.ePetFsmStateType.SKILL_HIT, _fsm)
	{
		m_dicMessageTreat.Add(eMessageType.ANIMATION_END, OnAnimationEnd);
		m_dicMessageTreat.Add(eMessageType.PET_HATCH, OnHatchIndicate);
	}
	
	#region - fsm function -
	public override void Enter(AsIMessage _msg)
	{
		m_SkillTimer = null;
		
		m_OwnerFsm.Entity.HandleMessage(new Msg_MoveStopIndication());
		
		HitProcess(_msg);
	}
	public override void Update()
	{
		if(m_Action != null)
		{
			CheckAnimDuration();
		}
	}
	public override void Exit()
	{
		if(m_SkillTimer != null) m_SkillTimer.Release();
		
		m_OwnerFsm.ReleaseElements();
	}
	#endregion

	#region - update -
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
	#endregion
	
	#region - ready to HIT -
	void HitProcess(AsIMessage _msg)
	{
		m_Ready = _msg as Msg_Pet_Skill_Ready;
		
		m_Action = m_Ready.actionRecord_;
		m_HitAnimation = m_Action.HitAnimation;
		
		if(m_HitAnimation != null)
		{
			float animSpeed = 1f;
			
			float timing = 0f;//m_Action.HitAnimation.hitInfo.HitTiming * 0.001f;
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
				SetTimer(m_HitAnimation.LoopDuration * 0.001f);
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
				anim = new Msg_AnimationIndicate(m_HitAnimation.FileName, 0.1f, animSpeed);
				break;
			case eLoopType.TargetLoop:
				anim = new Msg_AnimationIndicate(m_HitAnimation.FileName, 0.1f, animSpeed,
					m_HitAnimation.LoopTargetTime, m_HitAnimation.LoopDuration);
				break;
			default:
				Debug.LogError("AsPetState_SkillHit::HitProcess: invalid hit animation process. m_HitAnimation.LoopType = " + m_HitAnimation.LoopType);
				ChangeStateToFinish();
				break;
			}
			
			m_CurPlayingAnimName = m_HitAnimation.FileName;
			anim.SetActionAnimation(m_HitAnimation);
			m_OwnerFsm.PetEntity.HandleMessage(anim);
			
//			m_OwnerFsm.PlayElements(eActionStep.Hit, m_OwnerFsm.PetOwner.transform.position, m_OwnerFsm.PetOwner.transform, animSpeed);
			m_OwnerFsm.PlayElements(eActionStep.Finish, m_OwnerFsm.transform.position, m_OwnerFsm.transform, animSpeed);
			
			if(m_Action.AniBlendStep == eActionStep.Hit)
				m_OwnerFsm.Entity.HandleMessage(new Msg_FadeTimeIndicate(m_Action.AniBlendingDuration * 0.001f));
			
			if( m_Ready.playerPet_ == true)
				PacketDelay();
		}
		else
		{
			ChangeStateToFinish();
		}
		
		GenerateMissile();
	}
	#endregion
	
	#region - msg -
	void OnAnimationEnd(AsIMessage _msg)
	{
		Msg_AnimationEnd end = _msg as Msg_AnimationEnd;
		
		if(end.animName_ == m_CurPlayingAnimName && 
			(m_HitAnimation.LoopType & (eLoopType.NONE | eLoopType.Once | eLoopType.TargetLoop)) != 0)
		{
			ChangeStateToFinish();
		}
	}

	void OnHatchIndicate( AsIMessage _msg)
	{
		m_OwnerFsm.SetPetFsmState(AsPetFsm.ePetFsmStateType.HATCH, _msg);
	}
	#endregion
	
	#region - method -
	void ChangeStateToFinish()
	{
		ReleaseTimer();
		m_OwnerFsm.SetPetFsmState(AsPetFsm.ePetFsmStateType.SKILL_FINISH, m_Ready);
	}
	#endregion
	
	#region - timer -
	void PacketDelay()
	{
		float delay = 0.1f;
		float time = 0f;// m_Action.HitAnimation.hitInfo.HitTiming * 0.001f;// - delay;
		
		if(time < 0)
			time = 0;
		
		m_SkillTimer = AsTimerManager.Instance.SetTimer(time / 1f - delay, SendingPacket, m_Ready);
	}
	
	void SendingPacket(System.Object _obj)
	{
//		Msg_Pet_Skill_Ready attack = _obj as Msg_Pet_Skill_Ready;
//		AsPetManager.Instance.Send_PetSkillUse( m_Ready);
	}
	
	void Timer_HitExecution(System.Object _obj)
	{
	}
	
	void Timer_GenerateMissile(System.Object _obj)
	{
	}
	
	//missile
	void GenerateMissile()
	{
//		// projectile target
//		Tbl_Action_HitInfo hitInfo = m_Ready.actionRecord_.HitAnimation.hitInfo;
//		
//		if(hitInfo.HitType == eHitType.ProjectileTarget)
//		{
//			AsTimerManager.Instance.SetTimer(hitInfo.HitTiming * 0.001f, Timer_GenerateMissile);
//		}
	}
	
	void Missile_HitExecution(System.Object _obj)
	{
//		m_OwnerFsm.Entity.HandleMessage(new Msg_HitExecution());
	}
	#endregion
	
	#region - skill name shout -
	void SkillNameShout(System.Object _obj)
	{
		if( m_Ready.skillRecord_.SkillName_Print != eSkillNamePrint.None)
		{
			string skillName = AsTableManager.Instance.GetTbl_String( m_Ready.skillRecord_.SkillName_Index);
			m_OwnerFsm.Entity.SkillNameShout(eSkillNameShoutType.Self, skillName);
		}
	}
	#endregion
}
