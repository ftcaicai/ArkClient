using UnityEngine;
using System.Collections;

public class AsOtherUserState_Condition_Sleep : AsBaseFsmState<AsOtherUserFsm.eOtherUserFsmStateType, AsOtherUserFsm> {
	
	public AsOtherUserState_Condition_Sleep(AsOtherUserFsm _fsm) : base(AsOtherUserFsm.eOtherUserFsmStateType.CONDITION_SLEEP, _fsm)
	{
		m_dicMessageTreat.Add(eMessageType.CONDITION_SLEEP, OnSleepIndicate);
		m_dicMessageTreat.Add(eMessageType.RECOVER_CONDITION_SLEEP, OnRecoverCondition);
	}
	
	#region - fsm function -
	public override void Enter(AsIMessage _msg)
	{
		m_OwnerFsm.OtherUserEntity.HandleMessage(new Msg_HitExecution());
		m_OwnerFsm.OtherUserEntity.HandleMessage(new Msg_MoveStopIndication());
		
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
		ReleaseTimer();
		
		m_OwnerFsm.ReleaseElements();
	}
	#endregion
	
	void SleepBegin(AsIMessage _msg)
	{
		m_OwnerFsm.OtherUserEntity.HandleMessage(new Msg_AnimationIndicate("Idle"));
	}
	
	void SleepEnd()
	{
		m_OwnerFsm.SetOtherUserFsmState(AsOtherUserFsm.eOtherUserFsmStateType.IDLE);
	}
	
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
