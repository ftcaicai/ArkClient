using UnityEngine;
using System.Collections;

public class AsNpcState_Idle : AsBaseFsmState<AsNpcFsm.eNpcFsmStateType, AsNpcFsm> {
	public AsNpcState_Idle(AsNpcFsm _fsm) : base(AsNpcFsm.eNpcFsmStateType.IDLE, _fsm)
	{
		m_dicMessageTreat.Add(eMessageType.MOVE_NPC_INDICATION, OnBeginMove);
		m_dicMessageTreat.Add(eMessageType.NPC_BEGIN_DIALOG, OnNpcBeginDialog);
	}
	
	#region - fsm function -
	public override void Enter(AsIMessage _msg)
	{
//		m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate(eCommonAnimType.IDLE));
		m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("Idle"));
	}
	public override void Update()
	{
//		CheckPlayerDistance();
	}
	public override void Exit()
	{
	}
	#endregion
	
	#region - update -
	void CheckPlayerDistance()
	{
		float distance = Vector3.Distance(m_OwnerFsm.transform.position, m_OwnerFsm.Target.transform.position);
		
		if(distance < 3f)
		{
			// change state
			m_OwnerFsm.SetNpcFsmState(AsNpcFsm.eNpcFsmStateType.ACCOST);
		}
	}
	#endregion
	
	#region - msg -
	void OnBeginMove(AsIMessage _msg)
	{
		m_OwnerFsm.SetNpcFsmState(AsNpcFsm.eNpcFsmStateType.WALK, _msg);
	}
	
	void OnNpcBeginDialog(AsIMessage _msg)
	{
		m_OwnerFsm.SetNpcFsmState(AsNpcFsm.eNpcFsmStateType.POPUP, _msg);
	}
	#endregion
}
