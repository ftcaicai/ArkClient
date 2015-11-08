using UnityEngine;
using System.Collections;

public class AsPetState_SkillReady : AsBaseFsmState<AsPetFsm.ePetFsmStateType, AsPetFsm> {//
	
	#region - member -
	string m_CurPlayingAnimName;
	
	Msg_Pet_Skill_Ready m_Ready;
	
	Tbl_Action_Animation m_ReadyAnimation;
	#endregion
	
	public AsPetState_SkillReady(AsPetFsm _fsm) : base(AsPetFsm.ePetFsmStateType.SKILL_READY, _fsm)
	{
		m_dicMessageTreat.Add(eMessageType.PET_SKILL_READY, OnBeginReady);
		m_dicMessageTreat.Add(eMessageType.ANIMATION_END, OnAnimationEnd);
		m_dicMessageTreat.Add(eMessageType.PET_HATCH, OnHatchIndicate);
	}
	
	#region - fsm function -
	public override void Enter(AsIMessage _msg)
	{
		m_OwnerFsm.Entity.HandleMessage(new Msg_MoveStopIndication());
		
		ReadyProcess(_msg);
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
		if((m_ReadyAnimation.LoopType & (eLoopType.TimeLoop | eLoopType.TimeLoop | eLoopType.Cast)) != 0)
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
		bool succeed = false;
		succeed = ReadyMsgDecode(_msg as Msg_Pet_Skill_Ready);
		
		if(succeed == false)
		{
			Debug.LogError("AsPetState_SkillReady::ReadyProcess: ready msg decoding is failed.");
			m_OwnerFsm.SetPetFsmState(AsPetFsm.ePetFsmStateType.IDLE);
			return;
		}
		
		m_OwnerFsm.SetAction(m_Action);
		
		if(m_ReadyAnimation != null)
		{
			float animSpeed = 1f;
			
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
				ChangeStateToHit();
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
				anim = new Msg_AnimationIndicate(m_ReadyAnimation.FileName, 0.1f, animSpeed);
				break;
			case eLoopType.TargetLoop:
				anim = new Msg_AnimationIndicate(m_ReadyAnimation.FileName, 0.1f, animSpeed,
					m_ReadyAnimation.LoopTargetTime, m_ReadyAnimation.LoopDuration);
				break;
			default:
				Debug.LogError("AsPetState_SkillReady::ReadyProcess: invalid ready animation process. m_ReadyAnimation.LoopType = " + m_ReadyAnimation.LoopType);
				ChangeStateToHit();
				break;
			}
			
			m_CurPlayingAnimName = m_ReadyAnimation.FileName;
			anim.SetActionAnimation(m_ReadyAnimation);
			m_OwnerFsm.Entity.HandleMessage(anim);
			
//			m_OwnerFsm.PlayElements(eActionStep.Ready, m_OwnerFsm.PetOwner.transform.position, m_OwnerFsm.PetOwner.transform, animSpeed);
			m_OwnerFsm.PlayElements(eActionStep.Finish, m_OwnerFsm.transform.position, m_OwnerFsm.transform, animSpeed);
			
			if(m_Action.AniBlendStep == eActionStep.Ready)
				m_OwnerFsm.Entity.HandleMessage(new Msg_FadeTimeIndicate(m_Action.AniBlendingDuration * 0.001f));
		}
		else
		{
			ChangeStateToHit();
		}
	}
	
	bool ReadyMsgDecode( Msg_Pet_Skill_Ready _ready)
	{
		m_Ready = _ready;
		m_Action = _ready.actionRecord_;
		
		if(m_Action == null)
		{
			Debug.LogError("AsPetState_SkillReady::ReadyMsgDecode: such indexed action is not exist on Pet<" + _ready.skillRecord_.Index + ">");
			return false;
		}
		
		m_ReadyAnimation = m_Action.ReadyAnimation;
		
		return true;
	}
	#endregion
	
	#region - msg -
	void OnBeginReady(AsIMessage _msg)
	{
		ReadyProcess(_msg);
	}
	
	void OnAnimationEnd(AsIMessage _msg)
	{
		Msg_AnimationEnd end = _msg as Msg_AnimationEnd;
		if(end.animName_ == m_CurPlayingAnimName &&
			(m_ReadyAnimation.LoopType & (eLoopType.NONE | eLoopType.Once | eLoopType.TargetLoop)) != 0)
		{
			ChangeStateToHit();
		}
	}

	void OnHatchIndicate( AsIMessage _msg)
	{
		m_OwnerFsm.SetPetFsmState(AsPetFsm.ePetFsmStateType.HATCH, _msg);
	}
	#endregion
	
	#region - method -
	void ChangeStateToHit()
	{
		ReleaseTimer();
		m_OwnerFsm.SetPetFsmState(AsPetFsm.ePetFsmStateType.SKILL_HIT, m_Ready);
	}
	#endregion
}
