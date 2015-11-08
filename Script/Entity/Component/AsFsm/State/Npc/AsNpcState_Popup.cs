using UnityEngine;
using System.Collections;

public class AsNpcState_Popup : AsBaseFsmState<AsNpcFsm.eNpcFsmStateType, AsNpcFsm> {
	
	eLivingType m_GazePlayer = eLivingType.Living;
	
	public AsNpcState_Popup(AsNpcFsm _fsm) : base(AsNpcFsm.eNpcFsmStateType.POPUP, _fsm)
	{
	}
	
	#region - fsm function -
	public override void Enter(AsIMessage _msg)
	{
		Msg_NpcBeginDialog msg = _msg as Msg_NpcBeginDialog;
		m_OwnerFsm.Target = msg.user_;
		
		m_GazePlayer = m_OwnerFsm.Entity.GetProperty<eLivingType>(eComponentProperty.NPC_LIVING);
		
		AsHUDController.Instance.SetTargetNpc(m_OwnerFsm.NpcEntity);
		
		CloseBobusWindow();
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
		if(m_GazePlayer == eLivingType.UnLiving)
			return;

        if (m_OwnerFsm == null)
            return;

        if (m_OwnerFsm.Target == null)
            return;
        
		
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
        if (m_OwnerFsm == null)
        {
            AsHUDController.Instance.SetTargetNpc(null);
            return;
        }

        if (m_OwnerFsm.Target == null)
        {
            AsHUDController.Instance.SetTargetNpc(null);
            return;
        }

		float distance = Vector3.Distance(m_OwnerFsm.transform.position, m_OwnerFsm.Target.transform.position);
		
		if(distance > AsNpcFsm.s_DialogReleaseDistance)
		{
			// change state
			m_OwnerFsm.SetNpcFsmState(AsNpcFsm.eNpcFsmStateType.IDLE);
			AsHUDController.Instance.SetTargetNpc(null);
		}
	}
	#endregion
	
	void CloseBobusWindow()
	{
		BonusManager.Instance.CloseBobusWindow();
	}
}
