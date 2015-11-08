using UnityEngine;
using System.Collections;

public class AsNpcState_Walk : AsBaseFsmState<AsNpcFsm.eNpcFsmStateType, AsNpcFsm> {
	public AsNpcState_Walk(AsNpcFsm _fsm) : base(AsNpcFsm.eNpcFsmStateType.WALK, _fsm)
	{
		m_dicMessageTreat.Add(eMessageType.MOVE_NPC_INDICATION, OnBeginMove);
		m_dicMessageTreat.Add(eMessageType.MOVE_END_INFORM, OnEndMove);
		m_dicMessageTreat.Add(eMessageType.NPC_BEGIN_DIALOG, OnNpcBeginDialog);
	}
	
	#region - fsm function -
	public override void Enter(AsIMessage _msg)
	{
		MoveProcess(_msg);
	}
	public override void Update()
	{
	}
	public override void Exit()
	{
	}
	#endregion
	
	#region - move -
	void MoveProcess(AsIMessage _msg)
	{
		Msg_NpcMoveIndicate msg = _msg as Msg_NpcMoveIndicate;
		
		m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("Walk"));
		
		m_OwnerFsm.Entity.SetProperty(eComponentProperty.MOVE_SPEED, msg.moveSpeed_);
		m_OwnerFsm.Entity.HandleMessage(new Msg_MoveInfo(msg.targetPosition_));
	}
	#endregion
	
	#region - msg -
	void OnBeginMove(AsIMessage _msg)
	{
		MoveProcess(_msg);
	}

	void OnEndMove(AsIMessage _msg)
	{
		m_OwnerFsm.SetNpcFsmState(AsNpcFsm.eNpcFsmStateType.IDLE);
	}
	
	void OnNpcBeginDialog(AsIMessage _msg)
	{
		m_OwnerFsm.SetNpcFsmState(AsNpcFsm.eNpcFsmStateType.POPUP, _msg);
	}
	#endregion
}
