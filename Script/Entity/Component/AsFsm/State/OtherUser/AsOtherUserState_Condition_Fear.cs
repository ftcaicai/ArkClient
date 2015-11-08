using UnityEngine;
using System.Collections;

public class AsOtherUserState_Condition_Fear : AsBaseFsmState<AsOtherUserFsm.eOtherUserFsmStateType, AsOtherUserFsm> {
	
	public AsOtherUserState_Condition_Fear(AsOtherUserFsm _fsm) : base(AsOtherUserFsm.eOtherUserFsmStateType.CONDITION_FEAR, _fsm)
	{
		m_dicMessageTreat.Add(eMessageType.CONDITION_FEAR, OnFearIndicate);
		m_dicMessageTreat.Add(eMessageType.RECOVER_CONDITION_FEAR, OnRecoverCondition);
		m_dicMessageTreat.Add(eMessageType.MOVE_OTHER_USER_INDICATION, OnMoveOtherUserIndicate);
		m_dicMessageTreat.Add(eMessageType.MOVE_END_INFORM, OnMoveEnd);
	}
	
	#region - fsm function -
	public override void Enter(AsIMessage _msg)
	{
		m_OwnerFsm.Entity.HandleMessage(new Msg_HitExecution());
		m_OwnerFsm.Entity.HandleMessage(new Msg_MoveStopIndication());
		
		FearBegin(_msg);
	}
	public override void Update()
	{
	}
	public override void Exit()
	{
		ReleaseTimer();
		
		m_OwnerFsm.ReleaseElements();
	}
	#endregion
	
	#region - fear -
	void FearBegin(AsIMessage _msg)
	{
	}
	
	void FearEnd()
	{
		m_OwnerFsm.SetOtherUserFsmState(AsOtherUserFsm.eOtherUserFsmStateType.IDLE);
	}
	#endregion
	
	#region - msg -
	void OnMoveOtherUserIndicate(AsIMessage _msg)
	{
		m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("Run"));
		
		Msg_OtherUserMoveIndicate move = _msg as Msg_OtherUserMoveIndicate;
		m_OwnerFsm.Entity.HandleMessage(new Msg_MoveInfo(move.destPosition_));
	}
	
	void OnMoveEnd(AsIMessage _msg)
	{
		m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("Idle"));
	}
		
	void OnFearIndicate(AsIMessage _msg)
	{
		FearBegin(_msg);
	}
	
	void OnRecoverCondition(AsIMessage _msg)
	{
		FearEnd();
	}
	#endregion
	
	#region - timer -
	#endregion
}
