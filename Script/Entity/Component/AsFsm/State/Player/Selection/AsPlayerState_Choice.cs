using UnityEngine;
using System.Collections;

public class AsPlayerState_Choice : AsBaseFsmState<AsPlayerFsm.ePlayerFsmStateType, AsPlayerFsm> {
	public AsPlayerState_Choice(AsPlayerFsm _fsm) : base(AsPlayerFsm.ePlayerFsmStateType.CHOICE, _fsm)
	{
//		m_dicMessageTreat.Add(eMessageType.CHOICE, OnStunIndicate);
		m_dicMessageTreat.Add(eMessageType.ANIMATION_END, OnAnimationEnd);
	}
	
	#region - fsm function -
	public override void Enter(AsIMessage _msg)
	{
		ChoiceBegin(_msg);
	}
	public override void Update()
	{
	}
	public override void Exit()
	{
//		AsEffectManager.Instance.RemoveEffectEntity(m_StunEffectIndex);
	}
	#endregion
	
	#region - stun -
	void ChoiceBegin(AsIMessage _msg)
	{
//		Msg_Choice stun = _msg as Msg_Choice;
		
		m_OwnerFsm.UserEntity.HandleMessage(new Msg_AnimationIndicate("Choice"));
		
//		AsEffectManager.Instance.RemoveEffectEntity(m_StunEffectIndex);
//		m_StunEffectIndex = AsEffectManager.Instance.PlayEffect(
//			"FX/Effect/" + stun.effectName_,
//			m_OwnerFsm.UserEntity.ModelObject.transform, false, 0);
	}
	#endregion
	
	#region - msg -
	void OnAnimationEnd(AsIMessage _msg)
	{
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.IDLE);
	}
	#endregion
}
