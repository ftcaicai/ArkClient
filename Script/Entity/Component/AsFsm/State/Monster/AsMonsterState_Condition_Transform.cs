using UnityEngine;
using System.Collections;

public class AsMonsterState_Condition_Transform : AsBaseFsmState<AsMonsterFsm.eMonsterFsmStateType, AsMonsterFsm> {

	GameObject m_TrnObject = null;

	public AsMonsterState_Condition_Transform(AsMonsterFsm _fsm) : base(AsMonsterFsm.eMonsterFsmStateType.CONDITION_TRANSFORM, _fsm)
	{
		m_dicMessageTreat.Add(eMessageType.MOVE_NPC_INDICATION, OnMoveIndication);
		m_dicMessageTreat.Add(eMessageType.CONDITION_TRANSFORM, OnTransformIndicate);
		m_dicMessageTreat.Add(eMessageType.RECOVER_CONDITION_TRANSFORM, OnRecoverCondition);
	}
	
	#region - fsm function -
	public override void Enter(AsIMessage _msg)
	{
		m_OwnerFsm.MonsterEntity.HandleMessage(new Msg_HitExecution());
		
		if(_msg != null && _msg.MessageType == eMessageType.FORCEDMOVE_INDICATION)
		{
		}
		else
			m_OwnerFsm.MonsterEntity.HandleMessage(new Msg_MoveStopIndication());
		
		TransformBegin(_msg);
	}
	public override void Update()
	{
		if(TimeElapsed() == true)
		{
			TransformEnd();
		}
	}
	public override void Exit()
	{
		ReleaseTimer();
		
		m_OwnerFsm.ReleaseElements();
	}
	#endregion
	
	#region - transform -
	void TransformBegin(AsIMessage _msg)
	{
		m_OwnerFsm.TransformBegin();

		Msg_ConditionIndicate_Transform msg = _msg as Msg_ConditionIndicate_Transform;
		
		m_TrnObject = AsPreloadManager.Instance.GetTrnObject(msg.idx_);
		if(m_TrnObject == null)
		{
			Debug.LogError("AsMonsterState_Condition_Transform:: TransformBegin: no object is cached. idx = " + msg.idx_);
			return;
		}

		Animation anim = m_TrnObject.animation;
		if( anim == null) anim = m_TrnObject.transform.GetChild( 0).animation;
		if(anim != null)
		{
			anim["Idle"].wrapMode = WrapMode.Loop;
			anim.Play("Idle");
		}
		
		m_TrnObject.transform.parent = m_OwnerFsm.transform;
		m_TrnObject.transform.localPosition = Vector3.zero;
		m_TrnObject.transform.localRotation = Quaternion.identity;
		
		m_OwnerFsm.TransformBegin();
	}
	
	void TransformEnd()
	{
		m_OwnerFsm.TransformEnd();

		m_OwnerFsm.SetMonsterFsmState(AsMonsterFsm.eMonsterFsmStateType.IDLE);
		
		if(m_TrnObject != null)
		{
			GameObject.Destroy(m_TrnObject);
		}
	}
	#endregion
	
	#region - msg -
	void OnMoveIndication(AsIMessage _msg)
	{
		Msg_NpcMoveIndicate msg = _msg as Msg_NpcMoveIndicate;
		
		if(msg.forceMove_ == true)
		{
			m_OwnerFsm.Entity.HandleMessage(new Msg_ForcedMoveIndication(PotencyProcessor.s_ForcedMoveDuratioin, msg.targetPosition_));
		}
	}
	
	void OnTransformIndicate(AsIMessage _msg)
	{
		TransformBegin(_msg);
	}
	
	void OnRecoverCondition(AsIMessage _msg)
	{
		TransformEnd();
	}
	#endregion
}
