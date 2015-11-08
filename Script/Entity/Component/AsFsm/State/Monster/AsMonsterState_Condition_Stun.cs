using UnityEngine;
using System.Collections;

public class AsMonsterState_Condition_Stun : AsBaseFsmState<AsMonsterFsm.eMonsterFsmStateType, AsMonsterFsm> {
	public AsMonsterState_Condition_Stun(AsMonsterFsm _fsm) : base(AsMonsterFsm.eMonsterFsmStateType.CONDITION_STUN, _fsm)
	{
		m_dicMessageTreat.Add(eMessageType.MOVE_NPC_INDICATION, OnMoveIndication);
		m_dicMessageTreat.Add(eMessageType.CONDITION_STUN, OnStunIndicate);
		m_dicMessageTreat.Add(eMessageType.RECOVER_CONDITION_STUN, OnRecoverCondition);
	}
	
	#region - member -
//	int m_StunEffectIndex;
	#endregion
	
	#region - fsm function -
	public override void Enter(AsIMessage _msg)
	{
		m_OwnerFsm.MonsterEntity.HandleMessage(new Msg_HitExecution());
		
		if(_msg != null && _msg.MessageType == eMessageType.FORCEDMOVE_INDICATION)
		{
		}
		else
			m_OwnerFsm.MonsterEntity.HandleMessage(new Msg_MoveStopIndication());
		
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
		m_OwnerFsm.MonsterEntity.HandleMessage(new Msg_AnimationIndicate("ConditionStun"));
	}
	
	void StunEnd()
	{
		m_OwnerFsm.SetMonsterFsmState(AsMonsterFsm.eMonsterFsmStateType.IDLE);
	}
	
	#region - msg -
	void OnMoveIndication(AsIMessage _msg)
	{
		Msg_NpcMoveIndicate msg = _msg as Msg_NpcMoveIndicate;
		
		if(msg.forceMove_ == true)
		{
			m_OwnerFsm.Entity.HandleMessage(new Msg_ForcedMoveIndication(PotencyProcessor.s_ForcedMoveDuratioin, msg.targetPosition_));
		}
	}
	
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
