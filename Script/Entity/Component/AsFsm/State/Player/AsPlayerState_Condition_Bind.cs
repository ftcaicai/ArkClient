using UnityEngine;
using System.Collections;

public class AsPlayerState_Condition_Bind : AsBaseFsmState<AsPlayerFsm.ePlayerFsmStateType, AsPlayerFsm> {
	
	public AsPlayerState_Condition_Bind(AsPlayerFsm _fsm) : base(AsPlayerFsm.ePlayerFsmStateType.CONDITION_BINDING, _fsm)
	{
		m_dicMessageTreat.Add(eMessageType.OTHER_USER_CLICK, OnOtherUserClick);
		m_dicMessageTreat.Add(eMessageType.NPC_CLICK, OnNpcClick);
		m_dicMessageTreat.Add(eMessageType.OBJECT_CLICK, OnObjectClick);
		m_dicMessageTreat.Add(eMessageType.INPUT_TARGETING, OnInputTargeting);
		m_dicMessageTreat.Add(eMessageType.INPUT_ATTACK, OnInputAttack);
		m_dicMessageTreat.Add(eMessageType.PLAYER_SKILL_TARGET_MOVE, OnTargetSkillMove);
		m_dicMessageTreat.Add(eMessageType.PLAYER_SKILL_READY, OnSkillReady);
		m_dicMessageTreat.Add(eMessageType.CHEAT_DEATH, OnCheatDeath);
		m_dicMessageTreat.Add(eMessageType.RECOVER_CONDITION_BINDING, OnRecoverCondition);
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
		
		float animSpeed = GetAnimationSpeedByMove_UserEntity("BattleIdle");
		m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("BattleIdle", 0.1f, animSpeed));
		
		if(AsGameMain.s_gameState == GAME_STATE.STATE_INGAME)
		{
			m_OwnerFsm.MovePacketRefresh( eMoveType.Sync_Stop, m_OwnerFsm.transform.position, m_OwnerFsm.transform.position);
		}
	}
	public override void Update()
	{
		if(CheckTargetLiving() == true)
		{
			CheckDistance();
			RefreshDirection();
		}
		
//		CheckEquipment();
	}
	public override void Exit()
	{
		m_OwnerFsm.ReleaseElements();
	}
	#endregion
	
	#region - update -
	bool CheckTargetLiving()
	{
		if(m_OwnerFsm.Target == null)
			return false;
		
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
		if(m_OwnerFsm.WeaponEquip == false)
			return;
		if(m_OwnerFsm.Target != null)
		{
			if(m_OwnerFsm.Target.FsmType == eFsmType.MONSTER ||
				(m_OwnerFsm.Target.FsmType == eFsmType.OTHER_USER && TargetDecider.CheckOtherUserIsEnemy(m_OwnerFsm.Target) == true))
			{
				if(m_OwnerFsm.Target.GetProperty<bool>(eComponentProperty.LIVING) == false)
					return;
				
				if(m_OwnerFsm.TargetForQuest == false)
				{
					float dist = Vector3.Distance(m_OwnerFsm.Target.transform.position, m_OwnerFsm.Entity.transform.position);
					float viewRange = m_OwnerFsm.UserEntity.GetProperty<float>(eComponentProperty.VIEW_DISTANCE);
					if(dist < viewRange)
					{
//						float attackRange = m_OwnerFsm.UserEntity.GetProperty<float>(eComponentProperty.ATTACK_DISTANCE);
//						if(dist < attackRange)
						
						SuitableBasicSkill suit = m_OwnerFsm.GetSuitableBasicSkill(dist);
						if(suit.InRange == true)
						{
							if(m_OwnerFsm.ElapsedCoolTime_BaseAttack <= 0)
							{
								eCLASS __class = m_OwnerFsm.UserEntity.GetProperty<eCLASS>(eComponentProperty.CLASS);
								eGENDER gender = m_OwnerFsm.UserEntity.GetProperty<eGENDER>(eComponentProperty.GENDER);
								
//								Msg_Player_Skill_Ready ready = new Msg_Player_Skill_Ready(__class, eSKILL_TYPE.Base, eCommand_Type.NONE, gender,
								Msg_Player_Skill_Ready ready = new Msg_Player_Skill_Ready(suit.Skill, gender, 1,
									m_OwnerFsm.Target, Vector3.zero,
									Vector3.zero, Vector3.zero, Vector3.zero,
									m_OwnerFsm.Target.transform.position - m_OwnerFsm.transform.position,
									eClockWise.CW);
								m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.SKILL_READY, ready);
								
								float animSpeed = GetAnimationSpeedByAttack_UserEntity(ready.actionRecord, eActionStep.Ready);
								ready.SetAnimSpeed(animSpeed);
								
							}
						}
					}
				}
			}
		}
	}
	
	void RefreshDirection()
	{
		if(m_OwnerFsm.Target == null)
			return;
		
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
	void OnOtherUserClick(AsIMessage _msg)
	{
		Msg_OtherUserClick click = _msg as Msg_OtherUserClick;
		m_OwnerFsm.Target = AsEntityManager.Instance.GetUserEntityByUniqueId(click.idx_);
	}
	
	void OnNpcClick(AsIMessage _msg)
	{
		Msg_NpcClick click = _msg as Msg_NpcClick;
		m_OwnerFsm.Target = AsEntityManager.Instance.GetNpcEntityBySessionId(click.idx_);
	}
	
	void OnObjectClick(AsIMessage _msg)
	{
	}
	
	void OnInputTargeting(AsIMessage _msg)
	{
		Msg_Input_Targeting targeting = _msg as Msg_Input_Targeting;
		m_OwnerFsm.Target = targeting.target_;
	}
	
	void OnInputAttack(AsIMessage _msg)
	{
		TargetProcess(_msg);
	}
	
	void OnTargetSkillMove(AsIMessage _msg)
	{
	}
	
	void OnSkillReady(AsIMessage _msg)
	{
		Msg_Player_Skill_Ready ready = _msg as Msg_Player_Skill_Ready;
		
		if(ready.skillRecord.Skill_Type == eSKILL_TYPE.Base || ready.skillRecord.Skill_Type == eSKILL_TYPE.SlotBase)
		{
			if(m_OwnerFsm.Target != null &&
				m_OwnerFsm.Entity.GetNavPathDistance(m_OwnerFsm.Entity.transform.position, m_OwnerFsm.Target.transform.position, true) >
				ready.skillLvRecord.Usable_Distance * 0.01f)// * m_OwnerFsm.Character_Size)
				m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.SKILL_READY, _msg);
		}
		else
			m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.SKILL_READY, _msg);
	}
	
	void OnCheatDeath( AsIMessage _msg)
	{
//		m_OwnerFsm.SetPlayerFsmState( AsPlayerFsm.ePlayerFsmStateType.DEATH, _msg);
		m_OwnerFsm.CheatDeath( _msg);
	}
	
	void OnRecoverCondition(AsIMessage _msg)
	{
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.IDLE);
	}
	#endregion
	
	#region - method -
	void TargetProcess(AsIMessage _msg)
	{
		Msg_Input_Attack msg = _msg as Msg_Input_Attack;
		
		m_OwnerFsm.Target = msg.enemy_;
	}
	
	bool ReleaseCombat()
	{
		m_OwnerFsm.Target = null;
//		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.IDLE);
		return false;//
	}
	#endregion
}
