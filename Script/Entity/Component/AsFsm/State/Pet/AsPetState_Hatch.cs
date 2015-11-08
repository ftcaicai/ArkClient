using UnityEngine;
using System.Collections;

public class  AsPetState_Hatch : AsBaseFsmState<AsPetFsm.ePetFsmStateType, AsPetFsm> {
	
	AsTimer m_HatchActionTimer = null;
	
	public AsPetState_Hatch(AsPetFsm _fsm) : base(AsPetFsm.ePetFsmStateType.HATCH, _fsm)
	{
		m_dicMessageTreat.Add(eMessageType.ANIMATION_END, OnAnimationEnd);
	}
	
	#region - fsm function -
	public override void Enter(AsIMessage _msg)
	{
		EnterHatch();
		
		m_OwnerFsm.SetAction(m_Action);
		m_OwnerFsm.PlayElements(eActionStep.Entire, 1f);
	}
	public override void Update()
	{
	}
	public override void Exit()
	{
		m_OwnerFsm.ReleaseElements();
	}
	#endregion
	#region - process -
	void EnterHatch()
	{
		float animSpeed = 1f;
		m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("Hatch", 0.1f, animSpeed));

		m_Action = GetAction_Pet("Hatch");
		
		m_OwnerFsm.SetAction(m_Action);
		m_OwnerFsm.PlayElements(eActionStep.Entire, animSpeed);
	}
	#endregion
	#region - msg -
	void OnAnimationEnd(AsIMessage _msg)
	{
		m_OwnerFsm.SetPetFsmState(AsPetFsm.ePetFsmStateType.IDLE, _msg);
	}
	#endregion
}
