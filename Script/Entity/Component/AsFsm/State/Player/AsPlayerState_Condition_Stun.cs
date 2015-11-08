using UnityEngine;
using System.Collections;

public class AsPlayerState_Condition_Stun : AsBaseFsmState<AsPlayerFsm.ePlayerFsmStateType, AsPlayerFsm> {
	
	public AsPlayerState_Condition_Stun(AsPlayerFsm _fsm) : base(AsPlayerFsm.ePlayerFsmStateType.CONDITION_STUN, _fsm)
	{
		m_dicMessageTreat.Add(eMessageType.CONDITION_STUN, OnStunIndicate);
		m_dicMessageTreat.Add(eMessageType.RECOVER_CONDITION_STUN, OnRecoverCondition);
	}
	
	#region - fsm function -
	public override void Enter(AsIMessage _msg)
	{
		m_OwnerFsm.UserEntity.HandleMessage(new Msg_HitExecution());
		m_OwnerFsm.UserEntity.HandleMessage(new Msg_MoveStopIndication());
		
		if(AsNetworkMessageHandler.Instance != null)
		{
			//AsCommonSender.SendMove(eMoveType.Normal, m_OwnerFsm.transform.position, m_OwnerFsm.transform.position); 
			m_OwnerFsm.MovePacketRefresh( eMoveType.Normal, m_OwnerFsm.transform.position, m_OwnerFsm.transform.position);
		}
		
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
//		AsEffectManager.Instance.RemoveEffectEntity(m_StunEffectIndex);
		ReleaseTimer();
		
		m_OwnerFsm.ReleaseElements();
	}
	#endregion
	
	#region - stun -
	void StunBegin(AsIMessage _msg)
	{
//		Msg_ConditionIndicate_Stun stun = _msg as Msg_ConditionIndicate_Stun;
		
//		SetTimer(stun.time_);
		m_OwnerFsm.UserEntity.HandleMessage(new Msg_AnimationIndicate("ConditionStun"));
		
//		AsEffectManager.Instance.RemoveEffectEntity(m_StunEffectIndex);
//		m_StunEffectIndex = AsEffectManager.Instance.PlayEffect(
//			"FX/Effect/" + stun.effectName_,
//			m_OwnerFsm.UserEntity.ModelObject.transform, false, 0);
	}
	
	void StunEnd()
	{
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.IDLE);
	}
	#endregion
	
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
