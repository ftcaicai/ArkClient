using UnityEngine;
using System.Collections;




public class AsOtherUserState_Work_Collect : AsBaseFsmState<AsOtherUserFsm.eOtherUserFsmStateType, AsOtherUserFsm> 
{
	
	#region - member -
	AsTimer m_IdleActionTimer;
	
	
	public enum eState {Ready, Hit, Finish}
//	eState m_State = eState.Ready;
	
	Msg_MoveBase m_MoveInfo;
	#endregion
	
	public AsOtherUserState_Work_Collect(AsOtherUserFsm _fsm) : base(AsOtherUserFsm.eOtherUserFsmStateType.COLLECT_WORK, _fsm)
	{
		m_dicMessageTreat.Add(eMessageType.ANIMATION_END, OnAnimationEnd);
		m_dicMessageTreat.Add(eMessageType.MOVE_OTHER_USER_INDICATION, OnBeginMove);
		m_dicMessageTreat.Add(eMessageType.OTHER_CHAR_ATTACK_READY, OnBeginAttack);
		m_dicMessageTreat.Add(eMessageType.RELEASE_TENSION, OnReleaseTension);	
		m_dicMessageTreat.Add(eMessageType.COLLECT_INFO, OnCollectInfo);
	}
	
	
	
	
	#region - fsm function -
	public override void Enter(AsIMessage _msg)
	{
		m_OwnerFsm.Entity.HandleMessage(new Msg_MoveStopIndication());
		m_OwnerFsm.OtherUserEntity.HandleMessage(new Msg_CombatEnd());
		
		OnCollectInfo(_msg);	
		
//		m_State = eState.Ready;		
		m_OwnerFsm.SetAction(m_Action);
		m_OwnerFsm.PlayElements(eActionStep.Hit);		
		m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate(m_Action.HitAnimation.FileName));	
	}
	
	public override void Init ()
	{
		eCLASS __class = m_OwnerFsm.Entity.GetProperty<eCLASS>(eComponentProperty.CLASS);
		eGENDER gender = m_OwnerFsm.Entity.GetProperty<eGENDER>(eComponentProperty.GENDER);
		m_Action = AsTableManager.Instance.GetTbl_Action_Record(__class, gender, "Collection");
	}
	
	public override void Update()
	{
	}
	public override void Exit()
	{
		ReleaseIdleAction();
		
		m_OwnerFsm.ReleaseElements();
	}
	#endregion
	
	
	#region - enter -
	void CheckCombat()
	{
		bool combat = m_OwnerFsm.OtherUserEntity.GetProperty<bool>(eComponentProperty.COMBAT);
		
		if(combat == true)
		{
			m_OwnerFsm.PlayAction_BattleIdle();
			m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("BattleIdle"));
		}
		else
		{
			m_OwnerFsm.PlayAction_Idle();
			m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("Idle"));
		}
	}
	#endregion
	
	#region - msg -
	void OnBeginMove(AsIMessage _msg)
	{
		m_OwnerFsm.SetOtherUserFsmState(AsOtherUserFsm.eOtherUserFsmStateType.RUN, _msg);
	}
	
	void OnBeginAttack(AsIMessage _msg)
	{
		m_OwnerFsm.SetOtherUserFsmState(AsOtherUserFsm.eOtherUserFsmStateType.SKILL_READY, _msg);
	}
	
	void OnReleaseTension(AsIMessage _msg)
	{
		m_OwnerFsm.PlayAction_Idle();
		m_OwnerFsm.OtherUserEntity.HandleMessage(new Msg_AnimationIndicate("Idle"));
		
		SetIdleAction();
	}	
	
	void OnCollectInfo(AsIMessage _msg)
	{
		Msg_CollectInfo m_collectInfo = _msg as Msg_CollectInfo;		
		switch( (eCOLLECT_STATE)m_collectInfo.collectInfo.eCollectState )
		{
		case eCOLLECT_STATE.eCOLLECT_STATE_CANCEL:
			m_OwnerFsm.OtherUserEntity.ShowCollectImg(false);
			m_OwnerFsm.SetOtherUserFsmState(AsOtherUserFsm.eOtherUserFsmStateType.IDLE);
			
			break;
		case eCOLLECT_STATE.eCOLLECT_STATE_COMPLETE:
			m_OwnerFsm.OtherUserEntity.ShowCollectImg(false);
			BeginFinishState();	
			m_OwnerFsm.SetOtherUserFsmState(AsOtherUserFsm.eOtherUserFsmStateType.IDLE);
			break;
			
		case eCOLLECT_STATE.eCOLLECT_STATE_START:
			m_OwnerFsm.OtherUserEntity.ShowCollectImg(true);
			break;
		}
	}
			

	void OnAnimationEnd(AsIMessage _msg)
	{
//		switch(m_State)
//		{
//		case eState.Ready:
//			BeginHitState();
//			break;
//		case eState.Hit:
////			BeginFinishState();
//			break;
//		case eState.Finish:
//			EndFinishState();
//			break;
//		}
	}
	
	void BeginHitState()
	{
//		m_State = eState.Hit;
			
//		m_OwnerFsm.PlayElements(eActionStep.Hit);
//		m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate(m_Action.HitAnimation.FileName));
	}
	
	void BeginFinishState()
	{
//		m_State = eState.Finish;
		
//		m_OwnerFsm.PlayElements(eActionStep.Finish);
//		m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate(m_Action.FinishAnimation.FileName));
	}
	
	void EndFinishState()
	{
		m_OwnerFsm.SetOtherUserFsmState( AsOtherUserFsm.eOtherUserFsmStateType.IDLE);
	}
	#endregion
	
	
	
	#region - idle action -
	void Timer_IdleAction(System.Object _obj)
	{
		if(m_OwnerFsm.CurrnetFsmStateType == AsOtherUserFsm.eOtherUserFsmStateType.IDLE &&
			m_OwnerFsm.Entity.GetProperty<bool>(eComponentProperty.COMBAT) == false)
		{
			m_OwnerFsm.PlayAction_IdleAction();
			m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("IdleAction"));
		}
	}
	
	void SetIdleAction()
	{
		ReleaseIdleAction();

		float cycle = m_OwnerFsm.IdleActionCycle + Random.Range(-m_OwnerFsm.IdleActionRevision, m_OwnerFsm.IdleActionRevision);
		m_IdleActionTimer = AsTimerManager.Instance.SetTimer(cycle, Timer_IdleAction);
	}
	
	void ReleaseIdleAction()
	{
		if(m_IdleActionTimer != null)
		{
			AsTimerManager.Instance.ReleaseTimer(m_IdleActionTimer);
			m_IdleActionTimer = null;
		}
	}
	#endregion
	
//	#endregion
}

