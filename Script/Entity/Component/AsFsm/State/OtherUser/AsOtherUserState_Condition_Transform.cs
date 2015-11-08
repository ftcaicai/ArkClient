using UnityEngine;
using System.Collections;

public class AsOtherUserState_Condition_Transform : AsBaseFsmState<AsOtherUserFsm.eOtherUserFsmStateType, AsOtherUserFsm> {

	GameObject m_TrnObject = null;

	public AsOtherUserState_Condition_Transform(AsOtherUserFsm _fsm) : base(AsOtherUserFsm.eOtherUserFsmStateType.CONDITION_TRANSFORM, _fsm)
	{
		m_dicMessageTreat.Add(eMessageType.CONDITION_TRANSFORM, OnTransformIndicate);
		m_dicMessageTreat.Add(eMessageType.RECOVER_CONDITION_TRANSFORM, OnRecoverCondition);
	}
	
	#region - fsm function -
	public override void Enter(AsIMessage _msg)
	{
		m_OwnerFsm.OtherUserEntity.HandleMessage(new Msg_HitExecution());
		m_OwnerFsm.OtherUserEntity.HandleMessage(new Msg_MoveStopIndication());
		
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
			Debug.LogError("AsOtherUserState_Condition_Transform:: TransformBegin: no object is cached. idx = " + msg.idx_);
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

		m_OwnerFsm.SetOtherUserFsmState(AsOtherUserFsm.eOtherUserFsmStateType.IDLE);
		
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
