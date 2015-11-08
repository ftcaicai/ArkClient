using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsOtherUserState_Run : AsBaseFsmState<AsOtherUserFsm.eOtherUserFsmStateType, AsOtherUserFsm>
{
	public AsOtherUserState_Run(AsOtherUserFsm _fsm) : base(AsOtherUserFsm.eOtherUserFsmStateType.RUN, _fsm)
	{
		m_dicMessageTreat.Add(eMessageType.MOVE_OTHER_USER_INDICATION, OnBeginMove);
		m_dicMessageTreat.Add(eMessageType.MOVE_END_INFORM, OnEndMove);
		m_dicMessageTreat.Add(eMessageType.OTHER_CHAR_ATTACK_READY, OnBeginSkill);
		m_dicMessageTreat.Add(eMessageType.COLLECT_INFO, OnCollectInfo);
		m_dicMessageTreat.Add(eMessageType.RELEASE_TENSION, OnReleaseTension);
	}
	
	public override void Init ()
	{
		eCLASS __class = m_OwnerFsm.Entity.GetProperty<eCLASS>(eComponentProperty.CLASS);
		eGENDER gender = m_OwnerFsm.Entity.GetProperty<eGENDER>(eComponentProperty.GENDER);
		m_Action = AsTableManager.Instance.GetTbl_Action_Record( __class, gender, "Run");
	}
	
	#region - fsm function -
	public override void Enter(AsIMessage _msg)
	{
		m_OwnerFsm.Entity.HandleMessage(new Msg_CombatEnd());
		CheckCombat();
		MoveProcess(_msg);
		
//		Tbl_Action_Record actionRecord = AsTableManager.Instance.GetTbl_Action_Record( m_OwnerFsm.Entity.GetProperty<eCLASS>(eComponentProperty.CLASS), "Run");
		
		m_OwnerFsm.SetAction(m_Action);
		m_OwnerFsm.PlayElements(eActionStep.Entire);
	}
	
	public override void Update()
	{
	}
	
	public override void Exit()
	{
		m_OwnerFsm.ReleaseElements();
	}
	#endregion
	#region - enter -
	void CheckCombat()
	{
//		#region - anim speed -
//		bool combat = m_OwnerFsm.Entity.GetProperty<bool>(eComponentProperty.COMBAT);
//		float moveSpeed = m_OwnerFsm.Entity.GetProperty<float>(eComponentProperty.MOVE_SPEED);
//		eRACE race = m_OwnerFsm.Entity.GetProperty<eRACE>(eComponentProperty.RACE);
//		eCLASS __class = m_OwnerFsm.Entity.GetProperty<eCLASS>(eComponentProperty.CLASS);
//		Tbl_Class_Record classRecord = AsTableManager.Instance.GetTbl_Class_Record(race, __class);
//		float animSpeed = moveSpeed / classRecord.MoveSpeed * 100;
//		#endregion
		
		bool combat = m_OwnerFsm.Entity.GetProperty<bool>(eComponentProperty.COMBAT);
		if(combat == true && m_OwnerFsm.WeaponEquip == true)
		{
			m_OwnerFsm.PlayAction_BattleRun();
			
			float animSpeed = GetAnimationSpeedByMove_UserEntity("BattleRun");
			m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("BattleRun", 0.1f, animSpeed));
		}
		else
		{
			m_OwnerFsm.PlayAction_Run();
			
			float animSpeed = GetAnimationSpeedByMove_UserEntity("Run");
			m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("Run", 0.1f, animSpeed));
		}
	}
	#endregion
	
	#region - move -
	void MoveProcess(AsIMessage _msg)
	{
		Msg_OtherUserMoveIndicate msg = _msg as Msg_OtherUserMoveIndicate;
		switch(msg.moveType_)
		{
		case eMoveType.Normal:
		case eMoveType.Combat:
			m_OwnerFsm.Entity.HandleMessage(new Msg_MoveInfo(msg.destPosition_));
			break;
		case eMoveType.Auto:
			m_OwnerFsm.Entity.HandleMessage(new Msg_AutoMove(msg.curPosition_, msg.destPosition_));
			
			if((msg.curPosition_ - m_OwnerFsm.transform.position).sqrMagnitude > 0.1f)
				m_OwnerFsm.transform.position = msg.curPosition_;
			break;
		}
	}
	#endregion
	
	#region - process -
	void OnBeginMove(AsIMessage _msg)
	{
		MoveProcess(_msg);
	}
	
	void OnEndMove(AsIMessage _msg)
	{
		m_OwnerFsm.SetOtherUserFsmState(AsOtherUserFsm.eOtherUserFsmStateType.IDLE);
	}
	
	void OnBeginSkill(AsIMessage _msg)
	{
		m_OwnerFsm.SetOtherUserFsmState(AsOtherUserFsm.eOtherUserFsmStateType.SKILL_READY, _msg);
	}
	#endregion
	#region - release tension -
	void OnReleaseTension(AsIMessage _msg)
	{
		m_OwnerFsm.PlayAction_Run();
		
		float animSpeed = GetAnimationSpeedByMove_UserEntity("Run");
		m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("Run", 0.1f, animSpeed));
	}
	#endregion
	
	
	void OnCollectInfo(AsIMessage _msg)
	{
		m_OwnerFsm.SetOtherUserFsmState(AsOtherUserFsm.eOtherUserFsmStateType.COLLECT_WORK, _msg);
	}
}
