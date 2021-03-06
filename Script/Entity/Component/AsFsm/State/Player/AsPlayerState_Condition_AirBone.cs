using UnityEngine;
using System.Collections;

public class AsPlayerState_Condition_AirBone : AsBaseFsmState<AsPlayerFsm.ePlayerFsmStateType, AsPlayerFsm> {
	
	eAirboneState m_State = eAirboneState.Ready;
	Msg_ConditionIndicate_AirBone m_Msg;
	float m_InitHeight;
	
	public AsPlayerState_Condition_AirBone(AsPlayerFsm _fsm) : base(AsPlayerFsm.ePlayerFsmStateType.CONDITION_AIRBONE, _fsm)
	{
		m_dicMessageTreat.Add(eMessageType.CONDITION_AIRBONE, OnAirBoneIndicate);
		m_dicMessageTreat.Add(eMessageType.RECOVER_CONDITION_AIRBONE, OnRecoverCondition);
	}
	
	#region - fsm function -
	public override void Enter(AsIMessage _msg)
	{
		m_Msg = _msg as Msg_ConditionIndicate_AirBone;
		
		m_OwnerFsm.UserEntity.HandleMessage(new Msg_HitExecution());
		m_OwnerFsm.UserEntity.HandleMessage(new Msg_MoveStopIndication());
		
		if(AsNetworkMessageHandler.Instance != null)
		{
			m_OwnerFsm.MovePacketRefresh( eMoveType.Normal, m_OwnerFsm.transform.position, m_OwnerFsm.transform.position);
		}
		
		m_State = eAirboneState.Ready;
		
		AirBoneBegin(_msg);
	}
	public override void Update()
	{
		if(TimeElapsed() == true)
		{
			switch( m_State)
			{
			case eAirboneState.Up:
				m_State = eAirboneState.Float;
				float floatTime = m_Msg.body_.duration_ * 0.001f - airboneTime_Up - airboneTime_Down;
				SetTimer( floatTime);
				break;
			case eAirboneState.Float:
				m_State = eAirboneState.Down;
				m_AirboneVelocity = 0f;
				SetTimer( airboneTime_Down);
				break;
			case eAirboneState.Down:
				AirBoneEnd();
				break;
			}
		}
		else
		{
			switch( m_State)
			{
			case eAirboneState.Up:
				if( m_AirboneVelocity > 0)
				{
					m_AirboneVelocity -= m_AirboneAccel * Time.deltaTime;
					Vector3 pos = m_OwnerFsm.transform.position;
					pos.y += m_AirboneVelocity * Time.deltaTime;
					m_OwnerFsm.transform.position = pos;
				}
				break;
			case eAirboneState.Float:
				break;
			case eAirboneState.Down:
				if( m_OwnerFsm.transform.position.y > m_InitHeight)
				{
					m_AirboneVelocity -= m_AirboneAccel * Time.deltaTime;
					Vector3 pos = m_OwnerFsm.transform.position;
					pos.y += m_AirboneVelocity * Time.deltaTime;
					m_OwnerFsm.transform.position = pos;
				}
				break;
			}
		}
	}
	public override void Exit()
	{
		ReleaseTimer();
		
		m_OwnerFsm.ReleaseElements();
	}
	#endregion
	
	#region - stun -
	void AirBoneBegin(AsIMessage _msg)
	{
		m_State = eAirboneState.Up;
		SetTimer( airboneTime_Up);
		
		m_InitHeight = m_OwnerFsm.transform.position.y;
		
		m_AirboneVelocity = 8f;
		m_AirboneAccel = 16f;
		
		m_OwnerFsm.Entity.HandleMessage( new Msg_AnimationIndicate("BattleRun"));
		m_OwnerFsm.Entity.HandleMessage( new Msg_ShadowControl( false));
	}
	
	void AirBoneEnd()
	{
		Vector3 pos = m_OwnerFsm.transform.position;
		pos.y = m_InitHeight;
		m_OwnerFsm.transform.position = pos;
		
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.IDLE);
		m_OwnerFsm.Entity.HandleMessage( new Msg_ShadowControl( true));
	}
	#endregion
	
	#region - msg -
	void OnAirBoneIndicate(AsIMessage _msg)
	{
		AirBoneBegin(_msg);
	}
	
	void OnRecoverCondition(AsIMessage _msg)
	{
		AirBoneEnd();
	}
	#endregion
}
