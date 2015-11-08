using UnityEngine;
using System.Collections;

public class AsMonsterState_Condition_ForcedMove : AsBaseFsmState<AsMonsterFsm.eMonsterFsmStateType, AsMonsterFsm> {
	
	Msg_ForcedMoveIndication m_ForcedMove = null;
	
	public AsMonsterState_Condition_ForcedMove(AsMonsterFsm _fsm) : base(AsMonsterFsm.eMonsterFsmStateType.CONDITION_FORCEDMOVE, _fsm)
	{
		m_dicMessageTreat.Add(eMessageType.CONDITION_STUN, OnStunIndicate);
		
//		eCLASS __class = m_OwnerFsm.Entity.GetProperty<eCLASS>(eComponentProperty.CLASS);
//		eGENDER gender = m_OwnerFsm.Entity.GetProperty<eGENDER>(eComponentProperty.GENDER);
//		m_Action = AsTableManager.Instance.GetTbl_Action_Record(__class, gender, "Dash");
	}
	
	#region - fsm function -
	public override void Enter(AsIMessage _msg)
	{
		ForcedMoveProcess(_msg);
	}
	public override void Update()
	{
		if(TimeElapsed() == true)
		{
			m_OwnerFsm.SetMonsterFsmState(AsMonsterFsm.eMonsterFsmStateType.IDLE);
		}
	}
	public override void Exit()
	{
		m_OwnerFsm.ReleaseElements();
	}
	#endregion
	
	#region - dash -
	void ForcedMoveProcess(AsIMessage _msg)
	{
//		m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("Hit"));
		
		m_ForcedMove = null;
		
		if(m_Action == null || m_Action.ReadyAnimation == null)
			m_OwnerFsm.SetMonsterFsmState(AsMonsterFsm.eMonsterFsmStateType.IDLE);
		
		m_OwnerFsm.Entity.HandleMessage(new Msg_MoveStopIndication());
		
		Msg_ConditionIndicate_ForcedMove msg = _msg as Msg_ConditionIndicate_ForcedMove;
		
		m_ForcedMove = new Msg_ForcedMoveIndication(msg.duration_, msg.destination_);
		Debug.Log("AsMonsterState_Condition_ForcedMove::ForcedMoveProcess: duration=" + msg.duration_ + ", destination=" + msg.destination_);
		
//		Msg_DashIndication move = new Msg_DashIndication(
//			msg.duration_, msg.distance_ * 0.01f, msg.destination_ - m_OwnerFsm.transform.position, m_OwnerFsm.transform.position);
//		Debug.Log("AsMonsterState_Condition_ForcedMove::ForcedMoveProcess: duration=" + msg.duration_ + ", distance=" + msg.distance_ + ", destination=" + msg.destination_);
		m_OwnerFsm.Entity.HandleMessage(m_ForcedMove);
		
		SetTimer(msg.duration_);
	}
	#endregion
	#region - msg -
	void OnStunIndicate(AsIMessage _msg)
	{
		m_OwnerFsm.SetMonsterFsmState(AsMonsterFsm.eMonsterFsmStateType.CONDITION_STUN, m_ForcedMove);
	}
	#endregion
}
