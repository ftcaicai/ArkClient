using UnityEngine;
using System.Collections;

public class AsPlayerState_Condition_Freeze : AsBaseFsmState<AsPlayerFsm.ePlayerFsmStateType, AsPlayerFsm> {
	
	public AsPlayerState_Condition_Freeze(AsPlayerFsm _fsm) : base(AsPlayerFsm.ePlayerFsmStateType.CONDITION_FREEZE, _fsm)
	{
		m_dicMessageTreat.Add(eMessageType.CONDITION_FREEZE, OnFreezeIndicate);
		m_dicMessageTreat.Add(eMessageType.RECOVER_CONDITION_FREEZE, OnRecoverCondition);
	}
	
	#region - fsm function -
	public override void Enter(AsIMessage _msg)
	{
		m_OwnerFsm.UserEntity.HandleMessage(new Msg_HitExecution());
		m_OwnerFsm.UserEntity.HandleMessage(new Msg_MoveStopIndication());
		
		if(AsNetworkMessageHandler.Instance != null)
		{
			//AsCommonSender.SendMove(eMoveType.Normal, m_OwnerFsm.transform.position, m_OwnerFsm.transform.position);
			m_OwnerFsm.MovePacketRefresh( eMoveType.Normal, m_OwnerFsm.transform.position, m_OwnerFsm.transform.position);
		}
		
		FreezeBegin(_msg);
	}
	public override void Update()
	{
		if(TimeElapsed() == true)
		{
			FreezeEnd();
		}
	}
	public override void Exit()
	{
//		AsEffectManager.Instance.RemoveEffectEntity(m_FreezeEffectIndex);
		ReleaseTimer();
		
		m_OwnerFsm.ReleaseElements();
	}
	#endregion
	
	#region - freeze -
	void FreezeBegin(AsIMessage _msg)
	{
		m_OwnerFsm.UserEntity.HandleMessage(new Msg_AnimationIndicate("Idle", 0.1f, 0));
	}
	
	void FreezeEnd()
	{
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.IDLE);
	}
	#endregion
	
	#region - msg -
	void OnFreezeIndicate(AsIMessage _msg)
	{
		FreezeBegin(_msg);
	}
	
	void OnRecoverCondition(AsIMessage _msg)
	{
		FreezeEnd();
	}
	#endregion
}
