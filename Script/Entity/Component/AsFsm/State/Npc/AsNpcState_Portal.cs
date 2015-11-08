using UnityEngine;
using System.Collections;

public class AsNpcState_Portal : AsBaseFsmState<AsNpcFsm.eNpcFsmStateType, AsNpcFsm> 
{
	bool m_isSendPorta = false;
	
	public AsNpcState_Portal(AsNpcFsm _fsm) : base(AsNpcFsm.eNpcFsmStateType.PORTAL, _fsm)
	{		
	}
	
	#region - fsm function -
	public override void Enter(AsIMessage _msg)
	{
		//m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("Idle"));
	}
	public override void Update()
	{
		if( true == m_isSendPorta )
			return;
		
		AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
		if( null == userEntity)
			return;
		
	
		float fRadius = 1f;
		if( null != this.m_OwnerFsm.Entity.characterController )
		{
			fRadius = this.m_OwnerFsm.Entity.characterController.radius * 0.5f;
		}
		
		float dist = Vector3.Distance(userEntity.transform.position, m_OwnerFsm.transform.position);
		
		if( fRadius >= dist )
		{			
			AsCommonSender.SendWarp(m_OwnerFsm.NpcEntity.npcWarpIndex, m_OwnerFsm.NpcEntity.SessionId );
			m_isSendPorta = true;			
		}
	}
	public override void Exit()
	{
	}
	#endregion
	
	#region - update -
	void CheckPlayerDistance()
	{
//		float distance = Vector3.Distance(m_OwnerFsm.transform.position, m_OwnerFsm.Target.transform.position);
//		
//		if(distance < 3f)
//		{
//			// change state
//			m_OwnerFsm.SetNpcFsmState(AsNpcFsm.eNpcFsmStateType.ACCOST);
//		}
	}
	#endregion
	
}
