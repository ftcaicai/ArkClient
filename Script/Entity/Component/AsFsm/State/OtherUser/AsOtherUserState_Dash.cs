using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsOtherUserState_Dash : AsBaseFsmState<AsOtherUserFsm.eOtherUserFsmStateType, AsOtherUserFsm> {
	
	enum eDashStep {NONE, READY, DASH, END}
	eDashStep m_DashStep = eDashStep.NONE;
	
	string m_CurPlayingAnim;
	
	Msg_OtherUserMoveIndicate m_MoveIndication;
	
	public AsOtherUserState_Dash(AsOtherUserFsm _fsm) : base(AsOtherUserFsm.eOtherUserFsmStateType.DASH, _fsm)
	{
//		eCLASS __class = m_OwnerFsm.OtherUserEntity.GetProperty<eCLASS>(eComponentProperty.CLASS);
//		eGENDER gender = m_OwnerFsm.Entity.GetProperty<eGENDER>(eComponentProperty.GENDER);
//		m_Action = AsTableManager.Instance.GetTbl_Action_Record(__class, gender, "Dash");
		
		m_dicMessageTreat.Add(eMessageType.MOVE_OTHER_USER_INDICATION, OnBeginMove);
		m_dicMessageTreat.Add(eMessageType.OTHER_CHAR_ATTACK_READY, OnBeginSkill);
		m_dicMessageTreat.Add(eMessageType.COLLECT_INFO, OnCollectInfo);
	}
	
	#region - fsm function -
	public override void Enter(AsIMessage _msg)
	{
		DashProcess(_msg);
	}
	
	public override void Update()
	{
		if(TimeElapsed() == true)
		{
			switch(m_DashStep)
			{
			case eDashStep.READY:
				Dashing();
				break;
			case eDashStep.DASH:
				DashEnd();
				break;
			case eDashStep.END:
				Msg_CombatBegin combat = new Msg_CombatBegin();
				m_OwnerFsm.Entity.HandleMessage(combat);
				m_OwnerFsm.SetOtherUserFsmState(AsOtherUserFsm.eOtherUserFsmStateType.IDLE);
				break;
			}
		}
	}
	public override void Exit()
	{
		m_DashStep = eDashStep.NONE;
	}
	#endregion
	
	#region - dash -
	void DashProcess(AsIMessage _msg)
	{
		m_DashStep = eDashStep.READY;
		SetTimer(m_Action.ReadyAnimation.LoopDuration * 0.001f);
		
		m_MoveIndication = _msg as Msg_OtherUserMoveIndicate;
		
		Vector3 lookDir = m_MoveIndication.destPosition_ - m_MoveIndication.curPosition_;
		m_OwnerFsm.transform.LookAt(m_OwnerFsm.transform.position + lookDir);
		
		Msg_AnimationIndicate anim = new Msg_AnimationIndicate(m_Action.ReadyAnimation.FileName);
		m_OwnerFsm.OtherUserEntity.HandleMessage(anim);
		
		m_OwnerFsm.SetAction(m_Action);
		m_OwnerFsm.PlayElements(eActionStep.Ready);
	}
	
	void Dashing()
	{
		m_DashStep = eDashStep.DASH;
		Msg_AnimationIndicate anim = new Msg_AnimationIndicate(m_Action.HitAnimation.FileName);
		m_OwnerFsm.OtherUserEntity.HandleMessage(anim);
		
		switch(m_Action.HitAnimation.MoveType)
		{
		case eHitMoveType.Dash:
			SetTimer(m_Action.HitAnimation.LoopDuration * 0.001f);
			Msg_DashIndication dashIndication = new Msg_DashIndication(
				m_Action.HitAnimation.LoopDuration, m_Action.HitAnimation.MoveDistance * 0.01f,
				m_MoveIndication.destPosition_ - m_MoveIndication.curPosition_, m_OwnerFsm.transform.position);
			m_OwnerFsm.OtherUserEntity.HandleMessage(dashIndication);
			break;
		case eHitMoveType.Warp:
			SetTimer(0.001f);
			Msg_WarpIndication warpIndication = new Msg_WarpIndication(
				m_Action.HitAnimation.MoveDistance,
				m_MoveIndication.destPosition_ - m_MoveIndication.curPosition_, m_OwnerFsm.transform.position);
			m_OwnerFsm.OtherUserEntity.HandleMessage(warpIndication);
			break;
		}
		
		m_OwnerFsm.PlayElements(eActionStep.Hit);
	}
	
	void DashEnd()
	{
		m_DashStep = eDashStep.END;
		SetTimer(m_Action.FinishAnimation.LoopDuration * 0.001f);
		
		Msg_AnimationIndicate anim = new Msg_AnimationIndicate(m_Action.FinishAnimation.FileName);
		m_OwnerFsm.OtherUserEntity.HandleMessage(anim);
		
		m_OwnerFsm.Entity.HandleMessage(new Msg_MoveStopIndication());
		
		m_OwnerFsm.PlayElements(eActionStep.Finish);
	}
	#endregion
	
	#region - msg -
	void OnBeginMove(AsIMessage _msg)
	{
		Msg_OtherUserMoveIndicate msg = _msg as Msg_OtherUserMoveIndicate;
		if(msg.moveType_ != eMoveType.Dash)
			m_OwnerFsm.SetOtherUserFsmState(AsOtherUserFsm.eOtherUserFsmStateType.RUN, _msg);
		else
		{
			if(m_DashStep == eDashStep.END)
			{
				m_MoveIndication = _msg as Msg_OtherUserMoveIndicate;
				m_OwnerFsm.transform.position = m_MoveIndication.destPosition_;
			}
		}
	}
	
	void OnBeginSkill(AsIMessage _msg)
	{
		m_OwnerFsm.SetOtherUserFsmState(AsOtherUserFsm.eOtherUserFsmStateType.SKILL_READY, _msg);
	}
	
	void OnCollectInfo(AsIMessage _msg)
	{
		m_OwnerFsm.SetOtherUserFsmState(AsOtherUserFsm.eOtherUserFsmStateType.COLLECT_WORK, _msg);
	}
	#endregion

//	#region - effect -
//	void SetEffect(List<Tbl_Action_Effect> _listEffect)
//	{
//		foreach(Tbl_Action_Effect effect in _listEffect)
//		{
//			AsEffectManager.Instance.PlayEffect( "FX/Effect/" + effect.FileName,
//				m_OwnerFsm.Entity.ModelObject.transform, false, effect.LoopDuration * 0.001f);
//		}
//		
//		AsSoundManager.Instance.PlaySound( m_Action.SoundName, m_OwnerFsm.Entity.ModelObject.transform.position, false);
//	}
//	
//	void EffectProcess()
//	{
//		foreach(Tbl_Action_Effect effect in _listEffect)
//		{
//			AsEffectManager.Instance.PlayEffect( "FX/Effect/" + effect.FileName,
//				m_OwnerFsm.Entity.ModelObject.transform, false, effect.LoopDuration * 0.001f);
//		}
//		
//		AsSoundManager.Instance.PlaySound( m_Action.SoundName, m_OwnerFsm.Entity.ModelObject.transform.position, false);
//	}
//	#endregion
}
