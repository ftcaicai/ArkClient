using UnityEngine;
using System.Collections;

public class AsPetState_SkillFinish : AsBaseFsmState<AsPetFsm.ePetFsmStateType, AsPetFsm> {
	
	#region - member -
	string m_CurPlayingAnimName;
	
	Msg_Pet_Skill_Ready m_Ready;
	
	Tbl_Action_Animation m_FinishAnimation;
	#endregion
	
	public AsPetState_SkillFinish(AsPetFsm _fsm) : base(AsPetFsm.ePetFsmStateType.SKILL_FINISH, _fsm)
	{
		m_dicMessageTreat.Add(eMessageType.ANIMATION_END, OnAnimationEnd);
		m_dicMessageTreat.Add(eMessageType.PET_HATCH, OnHatchIndicate);
	}
	
	#region - fsm function -
	public override void Enter(AsIMessage _msg)
	{
		m_OwnerFsm.Entity.HandleMessage(new Msg_MoveStopIndication());

		FinishProcess(_msg);
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
		m_OwnerFsm.ReleaseElements();
	}
	#endregion
	
	#region - update -
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
		m_Ready = _msg as Msg_Pet_Skill_Ready;
		
		m_Action = m_Ready.actionRecord_;
		m_FinishAnimation = m_Action.FinishAnimation;
		
		if(m_Action.FinishAnimation != null)
		{
			float animSpeed = 1f;
			
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
				anim = new Msg_AnimationIndicate(m_FinishAnimation.FileName, 0.1f, animSpeed);
				break;
			case eLoopType.TargetLoop:
				anim = new Msg_AnimationIndicate(m_FinishAnimation.FileName, 0.1f, animSpeed,
					m_FinishAnimation.LoopTargetTime, m_FinishAnimation.LoopDuration);
				break;
			default:
				Debug.LogError("AsPetState_SkillFinish:: FinishProcess: invalid finish animation process. m_FinishAnimation.LoopType = " + m_FinishAnimation.LoopType);
				FinishStateEnd();
				break;
			}
			
			m_CurPlayingAnimName = m_FinishAnimation.FileName;
			m_OwnerFsm.PetEntity.HandleMessage(anim);			
			
//			m_OwnerFsm.PlayElements(eActionStep.Finish, m_OwnerFsm.PetOwner.transform.position, m_OwnerFsm.PetOwner.transform, animSpeed);
			m_OwnerFsm.PlayElements(eActionStep.Finish, m_OwnerFsm.transform.position, m_OwnerFsm.transform, animSpeed);
			
			if(m_Action.AniBlendStep == eActionStep.Finish)
				m_OwnerFsm.Entity.HandleMessage(new Msg_FadeTimeIndicate(m_Action.AniBlendingDuration * 0.001f));
		}
		else
		{
			FinishStateEnd();
		}
	}
	
	void FinishStateEnd()
	{
		ReleaseTimer();
		
		m_OwnerFsm.Entity.HandleMessage(new Msg_FadeTimeIndicate(m_Action.AniBlendingDuration * 0.001f));
		
		m_OwnerFsm.SetPetFsmState(AsPetFsm.ePetFsmStateType.IDLE);
	}
	#endregion
	
	#region - msg -
	void OnAnimationEnd(AsIMessage _msg)
	{
		Msg_AnimationEnd end = _msg as Msg_AnimationEnd;
		if(end.animName_ == m_CurPlayingAnimName && 
			(m_FinishAnimation.LoopType & (eLoopType.NONE | eLoopType.Once | eLoopType.TargetLoop)) != 0)
		{
			FinishStateEnd();
		}
	}

	void OnHatchIndicate( AsIMessage _msg)
	{
		m_OwnerFsm.SetPetFsmState(AsPetFsm.ePetFsmStateType.HATCH, _msg);
	}
	#endregion
}
