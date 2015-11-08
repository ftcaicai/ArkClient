using UnityEngine;
using System.Collections;

public class AsOtherUserState_Death : AsBaseFsmState<AsOtherUserFsm.eOtherUserFsmStateType, AsOtherUserFsm> {
	
	public AsOtherUserState_Death(AsOtherUserFsm _fsm) : base(AsOtherUserFsm.eOtherUserFsmStateType.DEATH, _fsm)
	{
		m_dicMessageTreat.Add(eMessageType.CHAR_RESURRECTION, OnResurrection);
	}
	
	public override void Init ()
	{
		eCLASS __class = m_OwnerFsm.Entity.GetProperty<eCLASS>(eComponentProperty.CLASS);
		eGENDER gender = m_OwnerFsm.Entity.GetProperty<eGENDER>(eComponentProperty.GENDER);
		m_Action = AsTableManager.Instance.GetTbl_Action_Record(__class, gender, "Death");
	}
	
	#region - fsm function -
	public override void Enter(AsIMessage _msg)
	{
		m_OwnerFsm.Entity.HandleMessage(new Msg_MoveStopIndication());
		m_OwnerFsm.Entity.SetProperty(eComponentProperty.LIVING, false);
		
		if(_msg != null)
		{
			m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("Death", true));
		}
		else
		{
			m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("Death"));
			
			m_OwnerFsm.SetAction(m_Action);
			m_OwnerFsm.PlayElements(eActionStep.Entire);
		}
	}
	public override void Update()
	{
	}
	public override void Exit()
	{
		m_OwnerFsm.ReleaseElements();
	}
	#endregion
	
	void OnResurrection(AsIMessage _msg)
	{
		m_OwnerFsm.Entity.HandleMessage( new Msg_BuffRefresh());
		
		m_OwnerFsm.Entity.SetProperty(eComponentProperty.LIVING, true);
		m_OwnerFsm.SetOtherUserFsmState(AsOtherUserFsm.eOtherUserFsmStateType.IDLE);
		m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate( "Idle"));
	}
}
