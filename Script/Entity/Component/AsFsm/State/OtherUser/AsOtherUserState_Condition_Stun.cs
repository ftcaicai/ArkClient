using UnityEngine;
using System.Collections;

public class AsOtherUserState_Condition_Stun : AsBaseFsmState<AsOtherUserFsm.eOtherUserFsmStateType, AsOtherUserFsm> {
	
	public AsOtherUserState_Condition_Stun(AsOtherUserFsm _fsm) : base(AsOtherUserFsm.eOtherUserFsmStateType.CONDITION_STUN, _fsm)
	{
		m_dicMessageTreat.Add(eMessageType.CONDITION_STUN, OnStunIndicate);
		m_dicMessageTreat.Add(eMessageType.RECOVER_CONDITION_STUN, OnRecoverCondition);
	}
	
	#region - fsm function -
	public override void Enter(AsIMessage _msg)
	{
		m_OwnerFsm.OtherUserEntity.HandleMessage(new Msg_HitExecution());
		m_OwnerFsm.OtherUserEntity.HandleMessage(new Msg_MoveStopIndication());
		
		StunBegin(_msg);
	}
	public override void Update()
	{
		if(TimeElapsed() == true)
		{
			StunEnd();
		}
	}
	public override void Exit()
	{
		ReleaseTimer();
		
		m_OwnerFsm.ReleaseElements();
	}
	#endregion
	
	void StunBegin(AsIMessage _msg)
	{
		m_OwnerFsm.OtherUserEntity.HandleMessage(new Msg_AnimationIndicate("ConditionStun"));
	}
	
	void StunEnd()
	{
		m_OwnerFsm.SetOtherUserFsmState(AsOtherUserFsm.eOtherUserFsmStateType.IDLE);
	}
	
	#region - msg -
	void OnStunIndicate(AsIMessage _msg)
	{
		StunBegin(_msg);
	}
	
	void OnRecoverCondition(AsIMessage _msg)
	{
		StunEnd();
	}
	#endregion
}
