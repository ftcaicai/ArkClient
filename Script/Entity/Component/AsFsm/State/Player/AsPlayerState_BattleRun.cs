using UnityEngine;
using System.Collections;

public class AsPlayerState_BattleRun : AsBaseFsmState<AsPlayerFsm.ePlayerFsmStateType, AsPlayerFsm> {
	
	public enum eRunType {Target, NonTarget};
	
	eRunType m_RunType = eRunType.Target;
	Msg_Player_Skill_Target_Move m_TargetMove;
	
	public AsPlayerState_BattleRun(AsPlayerFsm _fsm) : base(AsPlayerFsm.ePlayerFsmStateType.BATTLE_RUN, _fsm)
	{
		m_dicMessageTreat.Add(eMessageType.INPUT_MOVE, OnInputMove);
		m_dicMessageTreat.Add(eMessageType.INPUT_AUTO_MOVE, OnInputAutoMove);
		m_dicMessageTreat.Add(eMessageType.OTHER_USER_CLICK, OnOtherUserClick);
		m_dicMessageTreat.Add(eMessageType.NPC_CLICK, OnNpcClick);
		m_dicMessageTreat.Add(eMessageType.OBJECT_CLICK, OnObjectClick);
		m_dicMessageTreat.Add(eMessageType.MOVE_END_INFORM, OnMoveEndInform);
		m_dicMessageTreat.Add(eMessageType.INPUT_TARGETING, OnInputTargeting);
		m_dicMessageTreat.Add(eMessageType.INPUT_ATTACK, OnInputAttack);
		m_dicMessageTreat.Add(eMessageType.PLAYER_SKILL_TARGET_MOVE, OnTargetSkillMove);
		m_dicMessageTreat.Add(eMessageType.PLAYER_SKILL_READY, OnSkillReady);
		m_dicMessageTreat.Add(eMessageType.INPUT_DASH, OnBeginDash);
		m_dicMessageTreat.Add(eMessageType.CONDITION_BINDING, OnBinding);
	}
	
	#region - fsm function -
	public override void Enter(AsIMessage _msg)
	{
		m_OwnerFsm.UserEntity.HandleMessage(new Msg_CombatBegin());
		
		m_RunType = eRunType.Target;	
		
//		#region - anim speed -
//		float moveSpeed = m_OwnerFsm.UserEntity.GetProperty<float>(eComponentProperty.MOVE_SPEED);
//		eRACE race = m_OwnerFsm.Entity.GetProperty<eRACE>(eComponentProperty.RACE);
//		eCLASS __class = m_OwnerFsm.Entity.GetProperty<eCLASS>(eComponentProperty.CLASS);
//		Tbl_Class_Record classRecord = AsTableManager.Instance.GetTbl_Class_Record(race, __class);
//		float animSpeed = moveSpeed / classRecord.MoveSpeed * 100;
//		#endregion
		
		m_OwnerFsm.PlayAction_BattleRun();
		
		float animSpeed = GetAnimationSpeedByMove_UserEntity("BattleRun");
		m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("BattleRun", 0.1f, animSpeed));
		
		SetTarget(_msg);
		
//		if(SetTarget(_msg) == true)
//			//walk order
//			BattleMove(m_OwnerFsm.Target.transform.position);
	}
	float m_Timing = 0;
	float m_Cycle = 0.1f;
	public override void Update()
	{
		RefreshPosition();
		
		if(Time.time - m_Timing > m_Cycle)
		{
			m_Timing = Time.time;
			
			switch(m_RunType)
			{
			case eRunType.Target:
				if(CheckTargetLiving() == true)
				{
					CheckDistance();
				}
				break;
			case eRunType.NonTarget:
				CheckDistance2();
				break;
			}
		}
		
		CheckEquipment();
	}
	public override void Exit()
	{
		m_TargetMove = null;
		
		m_OwnerFsm.ReleaseElements();
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
	
	void CheckDistance()
	{
		float distance = m_OwnerFsm.Entity.GetNavPathDistance(
			m_OwnerFsm.UserEntity.transform.position, m_OwnerFsm.Target.transform.position);
		
		if(m_TargetMove == null)
		{
			SuitableBasicSkill suit = m_OwnerFsm.GetSuitableBasicSkill(distance);
			
//			if(distance < m_OwnerFsm.UserEntity.GetProperty<float>(eComponentProperty.ATTACK_DISTANCE))
			if(suit.InRange == true)
			{
				// change state
				m_OwnerFsm.Entity.HandleMessage(new Msg_MoveStopIndication());
	//			m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.BASIC_ATTACK);
	//			m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.SKILL_READY, new Msg_Player_Skill_Ready(eSKILL_TYPE.Base, eSKILL_INPUT_TYPE.NONE));
				m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.BATTLE_IDLE, new Msg_Delay_BattleRun());
			}
		}
		else
		{
			if(distance <
				m_TargetMove.Ready.skillLvRecord.Usable_Distance * 0.01f)// * m_OwnerFsm.Character_Size)
			{
				if( AsCommonSender.CheckSkillUseProcessing() == false)
					m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.SKILL_READY, m_TargetMove.Ready);
			}
		}
	}
	
	void CheckDistance2()
	{
		float distance = m_OwnerFsm.Entity.GetNavPathDistance(
			m_OwnerFsm.UserEntity.transform.position, m_TargetMove.Ready.picked);
		
		if(m_TargetMove == null)
		{
			ReleaseCombat();
			return;
		}
		else
		{
			if(distance <
				m_TargetMove.Ready.skillLvRecord.Usable_Distance * 0.01f)// * m_OwnerFsm.Character_Size)
			{
				m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.SKILL_READY, m_TargetMove.Ready);
			}
		}
	}
	
	float m_Timer = 1;
	float m_Interval = 1;
	void RefreshPosition()
	{
		Vector3 targetPos = Vector3.zero;
		
		switch(m_RunType)
		{
		case eRunType.Target:
			if(m_OwnerFsm.targetDistance != 0 && m_OwnerFsm.Target != null)
				targetPos = m_OwnerFsm.Target.transform.position;
			else
			{
				Debug.LogWarning("AsPlayerState_BattleRun::RefreshPosition: m_RunType = eRunType.Target, and m_OwnerFsm.Target = null");
				return;
			}
			break;
		case eRunType.NonTarget:
			if(m_TargetMove != null)
				targetPos = m_TargetMove.Ready.picked;
			else
			{
				Debug.LogWarning("AsPlayerState_BattleRun::RefreshPosition: m_RunType = eRunType.NonTarget, and m_TargetMove = null");
				return;
			}
			break;
		}
		
		if(Time.time - m_Timer > m_Interval)
		{
			m_Timer = Time.time;
			
			m_OwnerFsm.UserEntity.HandleMessage(new Msg_MoveInfo(targetPos));
			//AsCommonSender.SendMove(eMoveType.Combat, m_OwnerFsm.transform.position, targetPos);
			m_OwnerFsm.MovePacketRefresh( eMoveType.Combat, m_OwnerFsm.transform.position, targetPos);
		}
	}
	
	void CheckEquipment()
	{
		if(m_OwnerFsm.WeaponEquip == false)
			m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.IDLE);
	}
	#endregion
	
	#region - enter -
	void BattleMove(Vector3 _pos)
	{
		m_OwnerFsm.UserEntity.HandleMessage(new Msg_MoveInfo(_pos));
		
		if(AsNetworkMessageHandler.Instance != null)
		{
            //AsCommonSender.SendMove(eMoveType.Combat, m_OwnerFsm.transform.position, _pos);                   
			m_OwnerFsm.MovePacketRefresh( eMoveType.Combat, m_OwnerFsm.transform.position, _pos);
		}
		
//		m_MoveInfo = new Msg_MoveInfo(_pos);
	}
	#endregion
	
	#region - msg -
	void OnInputMove(AsIMessage _msg)
	{
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.RUN, _msg);
	}
	
	void OnInputAutoMove(AsIMessage _msg)
	{
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.RUN, _msg);
	}
	
	void OnOtherUserClick(AsIMessage _msg)
	{
//		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.RUN_OTHERUSER, _msg);
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.IDLE);
	}
	
	void OnNpcClick(AsIMessage _msg)
	{
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.RUN_NPC, _msg);
	}
	
	void OnObjectClick(AsIMessage _msg)
	{
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.RUN_OBJECT, _msg);
	}
	
	void OnMoveEndInform(AsIMessage _msg)
	{
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.IDLE, _msg);	
//		AsCommonSender.SendMove(eMoveType.Combat, m_OwnerFsm.transform.position, m_OwnerFsm.transform.position);//stop
	}
	
	void OnInputTargeting(AsIMessage _msg)
	{
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.IDLE, _msg);
	}
	
	void OnInputAttack(AsIMessage _msg)
	{
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.BATTLE_IDLE, _msg);
	}
	
	void OnTargetSkillMove(AsIMessage _msg)
	{
		SetTarget(_msg);
//		if(SetTarget(_msg) == true)
//			BattleMove(m_OwnerFsm.Target.transform.position);
	}
	
	void OnSkillReady(AsIMessage _msg)
	{
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.SKILL_READY, _msg);
	}
	
	void OnBeginDash(AsIMessage _msg)
	{
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.DASH, _msg);
	}
	
	void OnBinding(AsIMessage _msg)
	{
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.CONDITION_BINDING, _msg);
	}
	#endregion
	
	#region - method -
	bool SetTarget(AsIMessage _msg)
	{
		m_TargetMove = null;
		
		Msg_Player_Skill_Target_Move skill = _msg as Msg_Player_Skill_Target_Move;
		Vector3 targetPos = Vector3.zero;
		
		if(skill == null)
		{
			if(m_OwnerFsm.Target == null)
			{
				Debug.LogError("AsPlayerState_BattleRun::Enter: Invalid id");
				m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.IDLE, null);
				return false;
			}
			else
			{
				targetPos = m_OwnerFsm.Target.transform.position;
			}
		}
		else
		{
			m_TargetMove = skill;
			m_RunType = skill.RunType;
						
			switch(m_RunType)
			{
			case eRunType.Target:
				targetPos = m_OwnerFsm.Target.transform.position;
				break;
			case eRunType.NonTarget:
				targetPos = m_TargetMove.Ready.picked;
				break;
			}
		}
		
		m_OwnerFsm.UserEntity.HandleMessage(new Msg_MoveInfo(targetPos));
		
		if(AsNetworkMessageHandler.Instance != null)
		{
            //AsCommonSender.SendMove(eMoveType.Combat, m_OwnerFsm.transform.position, targetPos);
			m_OwnerFsm.MovePacketRefresh( eMoveType.Combat, m_OwnerFsm.transform.position, targetPos);
		}
		
		return true;
	}
	
	bool ReleaseCombat()
	{
		m_OwnerFsm.Target = null;
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.IDLE);
		return false;//
	}
	#endregion
}

