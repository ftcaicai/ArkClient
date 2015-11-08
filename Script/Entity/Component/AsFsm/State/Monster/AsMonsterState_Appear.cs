using UnityEngine;
using System.Collections;

public class AsMonsterState_Appear : AsBaseFsmState<AsMonsterFsm.eMonsterFsmStateType, AsMonsterFsm> {
	public AsMonsterState_Appear(AsMonsterFsm _fsm) : base(AsMonsterFsm.eMonsterFsmStateType.APPEAR, _fsm)
	{
	}
	
	#region - fsm function -
	public override void Enter(AsIMessage _msg)
	{
		EnterAppear();
	}
	public override void Update()
	{
	}
	public override void Exit()
	{
		m_OwnerFsm.ReleaseElements();
	}
	#endregion
	#region - process -
	void EnterAppear()
	{
		m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("Idle"));
		AsTimerManager.Instance.SetTimer(1, Timer_ChangeStateToIdle);
	}
	
	void Timer_ChangeStateToIdle(System.Object _obj)
	{
		m_OwnerFsm.SetMonsterFsmState(AsMonsterFsm.eMonsterFsmStateType.IDLE);
	}
	#endregion
}
