using UnityEngine;
using System.Collections;

public class AsMonsterState_Condition_Sleep : AsBaseFsmState<AsMonsterFsm.eMonsterFsmStateType, AsMonsterFsm> {
	public AsMonsterState_Condition_Sleep(AsMonsterFsm _fsm) : base(AsMonsterFsm.eMonsterFsmStateType.CONDITION_SLEEP, _fsm)
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
		m_OwnerFsm.MonsterEntity.HandleMessage(new Msg_HitExecution());
		m_OwnerFsm.MonsterEntity.HandleMessage(new Msg_MoveStopIndication());
		
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
		m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("Idle"));
	}
	
	void SleepEnd()
	{
		m_OwnerFsm.SetMonsterFsmState(AsMonsterFsm.eMonsterFsmStateType.IDLE);
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
