using UnityEngine;
using System.Collections;

public class AsCombat_Combat : AsCombatStateBase {
	public AsCombat_Combat(AsPropertyDelayer_Combat _owner) : base(_owner, eCombatStateType.COMBAT)
	{
	}
	
	#region - fsm function -
	public override void Enter(AsIMessage _msg)
	{
		m_OwnerFsm.Entity.SetProperty(eComponentProperty.COMBAT, true);
		
//		float time = AsTableManager.Instance.GetTbl_GlobalWeight_Record("InputwaitTime").Value;
		float time = AsTableManager.Instance.GetTbl_GlobalWeight_Record(31).Value * 0.001f;
		
		SetTimer(time);
		m_OwnerFsm.time_ = time;
	}
	public override void Update()
	{
		if(TimeElapsed() == true)
		{
			m_OwnerFsm.SetCombatState(eCombatStateType.IDLE);
		}
		else
		{
			if(m_OwnerFsm.time_ > 0)
				m_OwnerFsm.time_ -= Time.deltaTime;
		}
	}
	public override void Exit()
	{
		ReleaseTimer();
		m_OwnerFsm.time_ = 0;
	}
	public override void MessageProcess(AsIMessage _msg)
	{
	}
	#endregion
}
