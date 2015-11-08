using UnityEngine;
using System.Collections;

public class AsPlayerState_Condition_Sleep : AsBaseFsmState<AsPlayerFsm.ePlayerFsmStateType, AsPlayerFsm> {
	
	public AsPlayerState_Condition_Sleep(AsPlayerFsm _fsm) : base(AsPlayerFsm.ePlayerFsmStateType.CONDITION_SLEEP, _fsm)
	{
		m_dicMessageTreat.Add(eMessageType.CONDITION_SLEEP, OnSleepIndicate);
		m_dicMessageTreat.Add(eMessageType.RECOVER_CONDITION_SLEEP, OnRecoverCondition);
	}
	
	#region - member -
//	int m_SleepEffectIndex;
	#endregion
	
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
		
		SleepBegin(_msg);
	}
	public override void Update()
	{
		if(TimeElapsed() == true)
		{
			SleepEnd();
		}
	}
	public override void Exit()
	{
//		AsEffectManager.Instance.RemoveEffectEntity(m_SleepEffectIndex);
		ReleaseTimer();
		
		m_OwnerFsm.ReleaseElements();
	}
	#endregion
	
	#region - sleep -
	void SleepBegin(AsIMessage _msg)
	{
		m_OwnerFsm.UserEntity.HandleMessage(new Msg_AnimationIndicate("Idle"));
	}
	
	void SleepEnd()
	{
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.IDLE);
	}
	#endregion
	
	#region - msg -
	void OnSleepIndicate(AsIMessage _msg)
	{
		SleepBegin(_msg);
	}
	
	void OnRecoverCondition(AsIMessage _msg)
	{
		SleepEnd();
	}
	#endregion
}
