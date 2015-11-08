using UnityEngine;
using System.Collections;

public class AsOtherUserState_Condition_Freeze : AsBaseFsmState<AsOtherUserFsm.eOtherUserFsmStateType, AsOtherUserFsm> {
	
	public AsOtherUserState_Condition_Freeze(AsOtherUserFsm _fsm) : base(AsOtherUserFsm.eOtherUserFsmStateType.CONDITION_FREEZE, _fsm)
	{
		m_dicMessageTreat.Add(eMessageType.CONDITION_FREEZE, OnFreezeIndicate);
		m_dicMessageTreat.Add(eMessageType.RECOVER_CONDITION_FREEZE, OnRecoverCondition);
	}
	
	#region - fsm function -
	public override void Enter(AsIMessage _msg)
	{
		m_OwnerFsm.OtherUserEntity.HandleMessage(new Msg_HitExecution());
		m_OwnerFsm.OtherUserEntity.HandleMessage(new Msg_MoveStopIndication());
		
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
		ReleaseTimer();
		
		m_OwnerFsm.ReleaseElements();
	}
	#endregion
	
	void FreezeBegin(AsIMessage _msg)
	{
		m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("Idle", 0.1f, 0));
	}
	
	void FreezeEnd()
	{
		m_OwnerFsm.SetOtherUserFsmState(AsOtherUserFsm.eOtherUserFsmStateType.IDLE);
	}
	
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
