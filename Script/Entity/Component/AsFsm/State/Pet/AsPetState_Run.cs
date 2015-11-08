using UnityEngine;
using System.Collections;

public class AsPetState_Run : AsBaseFsmState<AsPetFsm.ePetFsmStateType, AsPetFsm> {
	
	public AsPetState_Run(AsPetFsm _fsm) : base(AsPetFsm.ePetFsmStateType.RUN, _fsm)
	{
		m_dicMessageTreat.Add(eMessageType.MOVE_NPC_INDICATION, OnBeginMove);
		m_dicMessageTreat.Add(eMessageType.MOVE_END_INFORM, OnEndMove);
		m_dicMessageTreat.Add(eMessageType.PET_HATCH, OnHatchIndicate);
	}
	
	#region - fsm function -
	public override void Enter(AsIMessage _msg)
	{
		MoveProcess(_msg);
	}
	public override void Update()
	{
		if( CheckOwnerInRange() == true)
		{
			m_OwnerFsm.SetPetFsmState(AsPetFsm.ePetFsmStateType.IDLE);
		}
	}
	public override void Exit()
	{
//		Debug.Log("AsPetState_Run::Exit: m_OwnerFsm.transform.forward = " + m_OwnerFsm.transform.forward);
		m_OwnerFsm.ReleaseElements();
	}
	#endregion
	#region - update -
	public bool CheckOwnerInRange()
	{
		if(m_OwnerFsm.PetOwner == null)
		{
			Debug.LogWarning("AsPetState_Idle:: CheckOwnerInRange: m_OwnerFsm.PetOwner is null");
			return true;
		}

		Vector3 distVec = m_OwnerFsm.PetOwner.transform.position - m_OwnerFsm.transform.position;
		if( distVec.sqrMagnitude < AsPetFsm.s_sqrStopFollowRange)
			return true;
		else
			return false;
	}
	#endregion
	
	#region - move -
	void MoveProcess(AsIMessage _msg)
	{
//		Msg_NpcMoveIndicate msg = _msg as Msg_NpcMoveIndicate;

		if(m_OwnerFsm.PetOwner == null)
		{
			Debug.LogWarning("AsPetState_Idle:: CheckOwnerInRange: m_OwnerFsm.PetOwner is null");
			m_OwnerFsm.SetPetFsmState(AsPetFsm.ePetFsmStateType.IDLE);
			return;
		}
		
		float animSpeed = 1f;
		animSpeed = m_OwnerFsm.GetOwnerMoveSpeed();
		m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("Run", 0.1f, animSpeed));

		m_Action = GetAction_Pet("Run");
			
		m_OwnerFsm.SetAction(m_Action);
		m_OwnerFsm.PlayElements(eActionStep.Entire, animSpeed);
		
		m_OwnerFsm.Entity.HandleMessage(new Msg_MoveSpeedRefresh( animSpeed));
		m_OwnerFsm.Entity.HandleMessage(new Msg_MoveInfo( m_OwnerFsm.PetOwner.GetRandomValidPosisionInRange( AsBaseEntity.s_PetRange)));
	}
	#endregion
	
	#region - msg -
	void OnBeginMove(AsIMessage _msg)
	{
		MoveProcess(_msg);
	}
	
	void OnEndMove(AsIMessage _msg)
	{
		m_OwnerFsm.SetPetFsmState(AsPetFsm.ePetFsmStateType.IDLE);
	}
	
	void OnHatchIndicate( AsIMessage _msg)
	{
		m_OwnerFsm.SetPetFsmState(AsPetFsm.ePetFsmStateType.HATCH, _msg);
	}
	#endregion
}
