using UnityEngine;
using System.Collections;

public class AsPlayerState_Condition_Transform : AsBaseFsmState<AsPlayerFsm.ePlayerFsmStateType, AsPlayerFsm> {

	GameObject m_TrnObject = null;

	public AsPlayerState_Condition_Transform(AsPlayerFsm _fsm) : base(AsPlayerFsm.ePlayerFsmStateType.CONDITION_TRANSFORM, _fsm)
	{
		m_dicMessageTreat.Add(eMessageType.CONDITION_TRANSFORM, OnTransformIndicate);
		m_dicMessageTreat.Add(eMessageType.RECOVER_CONDITION_TRANSFORM, OnRecoverCondition);
	}
	
	#region - fsm function -
	public override void Enter(AsIMessage _msg)
	{
		m_OwnerFsm.UserEntity.HandleMessage(new Msg_HitExecution());
		m_OwnerFsm.UserEntity.HandleMessage(new Msg_MoveStopIndication());
		
		if(AsNetworkMessageHandler.Instance != null)
		{
			m_OwnerFsm.MovePacketRefresh( eMoveType.Normal, m_OwnerFsm.transform.position, m_OwnerFsm.transform.position);
		}
		
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
			Debug.LogError("AsPlayerState_Condition_Transform:: TransformBegin: no object is cached. idx = " + msg.idx_);
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
	}
	
	void TransformEnd()
	{
		m_OwnerFsm.TransformEnd();

		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.IDLE);

		if(m_TrnObject != null)
		{
			GameObject.Destroy(m_TrnObject);
		}
	}
	#endregion
	
	#region - msg -
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
