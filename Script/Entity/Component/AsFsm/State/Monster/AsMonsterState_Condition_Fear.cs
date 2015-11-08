using UnityEngine;
using System.Collections;

public class AsMonsterState_Condition_Fear : AsBaseFsmState<AsMonsterFsm.eMonsterFsmStateType, AsMonsterFsm> {
	
	public AsMonsterState_Condition_Fear(AsMonsterFsm _fsm) : base(AsMonsterFsm.eMonsterFsmStateType.CONDITION_FEAR, _fsm)
	{
		m_dicMessageTreat.Add(eMessageType.CONDITION_FEAR, OnFearIndicate);
		m_dicMessageTreat.Add(eMessageType.RECOVER_CONDITION_FEAR, OnRecoverCondition);
		m_dicMessageTreat.Add(eMessageType.MOVE_NPC_INDICATION, OnMoveMonsterIndicate);
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
		m_OwnerFsm.SetMonsterFsmState(AsMonsterFsm.eMonsterFsmStateType.IDLE);
	}
	#endregion
	
	#region - msg -
	void OnMoveMonsterIndicate(AsIMessage _msg)
	{
		m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("Run"));
		
		Msg_NpcMoveIndicate move = _msg as Msg_NpcMoveIndicate;
		m_OwnerFsm.Entity.HandleMessage(new Msg_MoveInfo(move.targetPosition_));
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
