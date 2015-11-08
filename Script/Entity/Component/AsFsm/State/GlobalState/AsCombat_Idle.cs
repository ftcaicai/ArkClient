using UnityEngine;
using System.Collections;

public class AsCombat_Idle : AsCombatStateBase{
	public AsCombat_Idle(AsPropertyDelayer_Combat _owner) : base(_owner, eCombatStateType.IDLE)
	{
		
	}
	
	#region - fsm function -
	public override void Enter(AsIMessage _msg)
	{
		m_OwnerFsm.Entity.SetProperty(eComponentProperty.COMBAT, false);
		m_OwnerFsm.Entity.HandleMessage(new Msg_ReleaseTension());
	}
	public override void Update()
	{
	}
	public override void Exit()
	{
	}
	public override void MessageProcess(AsIMessage _msg)
	{
	}
	#endregion
}
