using UnityEngine;
using System.Collections;

public class AsPlayerState_WaitDeletion : AsBaseFsmState<AsPlayerFsm.ePlayerFsmStateType, AsPlayerFsm> {
	public AsPlayerState_WaitDeletion(AsPlayerFsm _fsm) : base(AsPlayerFsm.ePlayerFsmStateType.WAIT_DELETION, _fsm)
	{
//		m_dicMessageTreat.Add(eMessageType.CHOICE, OnStunIndicate);
//		m_dicMessageTreat.Add(eMessageType.ANIMATION_END, OnAnimationEnd);
	}
	
	#region - fsm function -
	public override void Enter(AsIMessage _msg)
	{
	//	Selected(_msg);
	}
	public override void Update()
	{
	}
	public override void Exit()
	{
//		AsEffectManager.Instance.RemoveEffectEntity(m_StunEffectIndex);
	}
	#endregion
	
	#region - stun -
	void Selected(AsIMessage _msg)
	{
	}
	#endregion
	
	#region - msg -
	void OnAnimationEnd(AsIMessage _msg)
	{
//		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.IDLE);
	}
	#endregion
}
