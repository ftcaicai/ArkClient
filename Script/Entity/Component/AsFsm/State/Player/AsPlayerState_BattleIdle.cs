using UnityEngine;
using System.Collections;

public class AsPlayerState_BattleIdle : AsBaseFsmState<AsPlayerFsm.ePlayerFsmStateType, AsPlayerFsm> {
	
//	float m_AttackDistance;
	
	const string strBattleIdle = "BattleIdle{0}";
	System.Text.StringBuilder m_AnimName = new System.Text.StringBuilder();
	
	public AsPlayerState_BattleIdle(AsPlayerFsm _fsm) : base(AsPlayerFsm.ePlayerFsmStateType.BATTLE_IDLE, _fsm)
	{
		m_dicMessageTreat.Add(eMessageType.INPUT_MOVE, OnInputMove);
		m_dicMessageTreat.Add(eMessageType.OTHER_USER_CLICK, OnOtherUserClick);
		m_dicMessageTreat.Add(eMessageType.NPC_CLICK, OnNpcClick);
		m_dicMessageTreat.Add(eMessageType.OBJECT_CLICK, OnObjectClick);
		m_dicMessageTreat.Add(eMessageType.INPUT_TARGETING, OnInputTargeting);
		m_dicMessageTreat.Add(eMessageType.INPUT_ATTACK, OnInputAttack);
		m_dicMessageTreat.Add(eMessageType.PLAYER_SKILL_TARGET_MOVE, OnTargetSkillMove);
		m_dicMessageTreat.Add(eMessageType.PLAYER_SKILL_READY, OnSkillReady);
		m_dicMessageTreat.Add(eMessageType.INPUT_DASH, OnBeginDash);
		m_dicMessageTreat.Add(eMessageType.CHEAT_DEATH, OnCheatDeath);
	}
	
	#region - fsm function -
	public override void Enter(AsIMessage _msg)
	{
		m_OwnerFsm.Entity.HandleMessage(new Msg_MoveStopIndication());
		
//		#region - anim speed -
//		float moveSpeed = m_OwnerFsm.UserEntity.GetProperty<float>(eComponentProperty.MOVE_SPEED);
//		eRACE race = m_OwnerFsm.Entity.GetProperty<eRACE>(eComponentProperty.RACE);
//		eCLASS __class = m_OwnerFsm.Entity.GetProperty<eCLASS>(eComponentProperty.CLASS);
//		Tbl_Class_Record classRecord = AsTableManager.Instance.GetTbl_Class_Record(race, __class);
//		float animSpeed = moveSpeed / classRecord.MoveSpeed * 100;
//		#endregion
		
		m_OwnerFsm.PlayAction_BattleIdle();
		
		#region - stance -
		if(m_AnimName.Length > 0)
			m_AnimName.Remove(0, m_AnimName.Length);
		m_AnimName.AppendFormat(strBattleIdle, m_OwnerFsm.UserEntity.StanceNumber);
		#endregion
		
		float animSpeed = GetAnimationSpeedByMove_UserEntity("BattleIdle");
		m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate(m_AnimName.ToString(), 0.1f, animSpeed));
		
		if(AsGameMain.s_gameState == GAME_STATE.STATE_INGAME)
		{
//			if(_msg != null && _msg.MessageType == eMessageType.PLAYER_SKILL_FINISH)
//				m_OwnerFsm.MovePacketSynchronize();
//			else
				m_OwnerFsm.MovePacketRefresh( eMoveType.Sync_Stop, m_OwnerFsm.transform.position, m_OwnerFsm.transform.position);
		}
		
		SetTarget(_msg);
	}
	public override void Update()
	{
		if(CheckTargetLiving() == true)
		{
			CheckDistance();
			RefreshDirection();
		}
		
		CheckEquipment();
	}
	public override void Exit()
	{
		m_OwnerFsm.ReleaseElements();
	}
	#endregion
	
	#region - enter -	
	void SetTarget(AsIMessage _msg)
	{
		if(_msg != null)
		{
			switch(_msg.MessageType)
			{
			case eMessageType.INPUT_ATTACK:
				TargetProcess(_msg);
				break;
			case eMessageType.DELAY_BATTLE_RUN:
				SetTimer(0.5f);
				break;
			case eMessageType.OTHER_USER_CLICK:
				TargetProcess_OtherUser(_msg);
				break;
			}
		}
	}
	#endregion
	
	#region - update -
	bool CheckTargetLiving()
	{
		if(m_OwnerFsm.Target == null)
			return ReleaseCombat();
		
		if(m_OwnerFsm.Target.ContainProperty(eComponentProperty.LIVING) == true)
		{
			if(m_OwnerFsm.Target.GetProperty<bool>(eComponentProperty.LIVING) == false)
			{
				return ReleaseCombat();
			}
		}
		
		return true;
	}
	
	public void CheckDistance()
	{
		float distance = Vector3.Distance(m_OwnerFsm.UserEntity.transform.position, 
			m_OwnerFsm.Target.transform.position);
		
		SuitableBasicSkill suit = m_OwnerFsm.GetSuitableBasicSkill(distance);
		
//		if(distance > m_OwnerFsm.UserEntity.GetProperty<float>(eComponentProperty.ATTACK_DISTANCE))
		if(suit.InRange == false)
		{
			if(TimerOn == false || (TimerOn == true && TimeElapsed()))
				m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.BATTLE_RUN);// change state
		}
//		else if(m_OwnerFsm.ElapsedCoolTime_BaseAttack <= 0)
		else if(m_OwnerFsm.ElapsedCoolTime_BaseAttack <= 0)
		{
			eCLASS __class = m_OwnerFsm.UserEntity.GetProperty<eCLASS>(eComponentProperty.CLASS);
			eGENDER gender = m_OwnerFsm.UserEntity.GetProperty<eGENDER>(eComponentProperty.GENDER);
			
			Msg_Player_Skill_Ready ready = new Msg_Player_Skill_Ready(suit.Skill, gender, 1,
				m_OwnerFsm.Target, Vector3.zero,
				Vector3.zero, Vector3.zero, Vector3.zero,
				m_OwnerFsm.Target.transform.position - m_OwnerFsm.transform.position,
				eClockWise.CW);
						
//			Msg_Player_Skill_Ready ready = new Msg_Player_Skill_Ready(__class, eSKILL_TYPE.Base, eCommand_Type.NONE, gender,
//				m_OwnerFsm.Target, Vector3.zero,
//				Vector3.zero, Vector3.zero, Vector3.zero,
//				m_OwnerFsm.Target.transform.position - m_OwnerFsm.transform.position,
//				eClockWise.CW);
			
			m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.SKILL_READY, ready);
			
//			float animSpeed = m_Action.ActionSpeed * m_OwnerFsm.Entity.GetProperty<float>(eComponentProperty.ATTACK_SPEED);
			float animSpeed = GetAnimationSpeedByAttack_UserEntity(ready.actionRecord, eActionStep.Ready);
			ready.SetAnimSpeed(animSpeed);
		}
	}
	
	void RefreshDirection()
	{
		Vector3 dir = m_OwnerFsm.Target.transform.position - m_OwnerFsm.Entity.transform.position;
		
		if( 0.05f > dir.sqrMagnitude)
			return;
		
		m_OwnerFsm.Entity.transform.rotation = 
			Quaternion.Slerp(m_OwnerFsm.Entity.transform.rotation, 
				Quaternion.LookRotation(dir), 
				7.0f * Time.deltaTime);
		
		Vector3 angle = m_OwnerFsm.transform.eulerAngles;angle.x = 0f;m_OwnerFsm.transform.eulerAngles = angle;
	}
	
	void CheckEquipment()
	{
		if(m_OwnerFsm.WeaponEquip == false)
			m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.IDLE);
	}
	#endregion
	
	#region - msg -
	void OnInputMove(AsIMessage _msg)
	{
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.RUN, _msg);
	}
	
	void OnOtherUserClick(AsIMessage _msg)
	{
//		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.RUN_OTHERUSER, _msg);
	}
	
	void OnNpcClick(AsIMessage _msg)
	{
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.RUN_NPC, _msg);
	}
	
	void OnObjectClick(AsIMessage _msg)
	{
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.RUN_OBJECT, _msg);
	}
	
	void OnInputTargeting(AsIMessage _msg)
	{
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.IDLE, _msg);
	}
	
	void OnInputAttack(AsIMessage _msg)
	{
		TargetProcess(_msg);
	}
	
	void OnTargetSkillMove(AsIMessage _msg)
	{
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.BATTLE_RUN, _msg);
	}
	
	void OnSkillReady(AsIMessage _msg)
	{
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.SKILL_READY, _msg);
	}
	
	void OnBeginDash(AsIMessage _msg)
	{
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.DASH, _msg);
	}
	
	void OnCheatDeath( AsIMessage _msg)
	{
//		m_OwnerFsm.SetPlayerFsmState( AsPlayerFsm.ePlayerFsmStateType.DEATH, _msg);
		m_OwnerFsm.CheatDeath( _msg);
	}
	#endregion
	
	#region - method -
	void TargetProcess(AsIMessage _msg)
	{
		Msg_Input_Attack msg = _msg as Msg_Input_Attack;
		
		m_OwnerFsm.Target = msg.enemy_;
	}
	
	void TargetProcess_OtherUser(AsIMessage _msg)
	{
		Msg_OtherUserClick msg = _msg as Msg_OtherUserClick;
		
		AsUserEntity user = AsEntityManager.Instance.GetUserEntityByUniqueId(msg.idx_);
		m_OwnerFsm.Target = user;
	}
	
	bool ReleaseCombat()
	{
		m_OwnerFsm.Target = null;
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.IDLE);
		return false;//
	}
	#endregion
}
