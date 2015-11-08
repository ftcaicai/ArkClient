using UnityEngine;
using System.Collections;

public class AsOtherUserState_Condition_ForcedMove : AsBaseFsmState<AsOtherUserFsm.eOtherUserFsmStateType, AsOtherUserFsm> {
	
	Msg_ForcedMoveIndication m_ForcedMove = null;
	
	public AsOtherUserState_Condition_ForcedMove(AsOtherUserFsm _fsm) : base(AsOtherUserFsm.eOtherUserFsmStateType.CONDITION_FORCEDMOVE, _fsm)
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
			m_OwnerFsm.SetOtherUserFsmState(AsOtherUserFsm.eOtherUserFsmStateType.IDLE);
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
		m_ForcedMove = null;
		
		m_OwnerFsm.Entity.HandleMessage(new Msg_MoveStopIndication());
		m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("BattleIdle"));
		
		Msg_ConditionIndicate_ForcedMove msg = _msg as Msg_ConditionIndicate_ForcedMove;
		
		m_ForcedMove = new Msg_ForcedMoveIndication(msg.duration_, msg.destination_);
		Debug.Log("AsOtherUserState_Condition_ForcedMove::ForcedMoveProcess: duration=" + msg.duration_ + ", destination=" + msg.destination_);
		
		m_OwnerFsm.Entity.HandleMessage(m_ForcedMove);
		
		SetTimer(msg.duration_ * 0.001f);
	}
	#endregion
	#region - msg -
	void OnStunIndicate(AsIMessage _msg)
	{
		m_OwnerFsm.SetOtherUserFsmState(AsOtherUserFsm.eOtherUserFsmStateType.CONDITION_STUN, m_ForcedMove);
	}
	#endregion
}
