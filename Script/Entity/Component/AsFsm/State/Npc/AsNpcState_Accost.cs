using UnityEngine;
using System.Collections;

public class AsNpcState_Accost : AsBaseFsmState<AsNpcFsm.eNpcFsmStateType, AsNpcFsm> {
	public AsNpcState_Accost(AsNpcFsm _fsm) : base(AsNpcFsm.eNpcFsmStateType.ACCOST, _fsm)
	{
	}
	
	#region - fsm function -
	public override void Enter(AsIMessage _msg)
	{
//		m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate(eAnimationType.IDLE));
	}
	public override void Update()
	{
		RefreshDirection();
		CheckPlayerDistance();
	}
	public override void Exit()
	{
	}
	#endregion
	
	#region - update -
	void RefreshDirection()
	{
		Vector3 dir = m_OwnerFsm.Target.transform.position - m_OwnerFsm.transform.position;
		
		if( Vector3.zero == dir)
			return;
		
		m_OwnerFsm.Entity.transform.rotation = 
			Quaternion.Slerp(m_OwnerFsm.Entity.transform.rotation, 
				Quaternion.LookRotation(dir), 
				7.0f * Time.deltaTime);
		
		Vector3 angle = m_OwnerFsm.transform.eulerAngles;angle.x = 0f;m_OwnerFsm.transform.eulerAngles = angle;
	}
	
	void CheckPlayerDistance()
	{
		float distance = Vector3.Distance(m_OwnerFsm.transform.position, m_OwnerFsm.Target.transform.position);
		
		if(distance > 3)
		{
			// change state
			m_OwnerFsm.SetNpcFsmState(AsNpcFsm.eNpcFsmStateType.IDLE);
		}
	}
	#endregion
}
