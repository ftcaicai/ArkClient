using UnityEngine;
using System.Collections;

public class AsPlayerState_Condition_Fear : AsBaseFsmState<AsPlayerFsm.ePlayerFsmStateType, AsPlayerFsm> {
	
	public AsPlayerState_Condition_Fear(AsPlayerFsm _fsm) : base(AsPlayerFsm.ePlayerFsmStateType.CONDITION_FEAR, _fsm)
	{
		m_dicMessageTreat.Add(eMessageType.CONDITION_FEAR, OnFearIndicate);
		m_dicMessageTreat.Add(eMessageType.RECOVER_CONDITION_FEAR, OnRecoverCondition);
		m_dicMessageTreat.Add(eMessageType.MOVE_END_INFORM, OnMoveEnd);
	}
	
	#region - fsm function -
	public override void Enter(AsIMessage _msg)
	{
		m_OwnerFsm.UserEntity.HandleMessage(new Msg_HitExecution());
		m_OwnerFsm.UserEntity.HandleMessage(new Msg_MoveStopIndication());
		
		FearBegin(_msg);
	}
	public override void Update()
	{
		if(TimeElapsed() == true)
		{
			FearBegin(null);
		}
	}
	public override void Exit()
	{
		ReleaseTimer();
		
		m_OwnerFsm.ReleaseElements();
	}
	#endregion
	
	#region - fear -
	void FearBegin(AsIMessage _msg)
	{
		float refreshTime = 2f;
		SetTimer(refreshTime);
		
		BeginMoving();
	}
	
	void FearEnd()
	{
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.IDLE);
	}
	
	void BeginMoving()
	{
		m_OwnerFsm.UserEntity.HandleMessage(new Msg_AnimationIndicate("Run"));
		
		float range = 5f;
		Vector3 pos = m_OwnerFsm.transform.position;
		pos.x = pos.x + Random.Range(-range, range);
		pos.z = pos.z + Random.Range(-range, range);
		m_OwnerFsm.Entity.HandleMessage(new Msg_MoveInfo(pos));
		
		if(AsNetworkMessageHandler.Instance != null)
		{
			m_OwnerFsm.MovePacketRefresh( eMoveType.Normal, m_OwnerFsm.transform.position, pos);
		}
	}
	#endregion
	
	#region - msg -
	void OnMoveEnd(AsIMessage _msg)
	{
		m_OwnerFsm.UserEntity.HandleMessage(new Msg_AnimationIndicate("Idle"));
	}
		
	void OnFearIndicate(AsIMessage _msg)
	{
		FearBegin(_msg);
	}
	
	void OnRecoverCondition(AsIMessage _msg)
	{
		FearEnd();
	}
	#endregion
	
	#region - timer -
	#endregion
}
