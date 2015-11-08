using UnityEngine;
using System.Collections;

public class AsPlayerState_Product : AsBaseFsmState<AsPlayerFsm.ePlayerFsmStateType, AsPlayerFsm>
{
	AsTimer m_IdleActionTimer = null;
	
	#region - init -
	public AsPlayerState_Product(AsPlayerFsm _fsm) : base(AsPlayerFsm.ePlayerFsmStateType.PRODUCT, _fsm)
	{				
		m_dicMessageTreat.Add(eMessageType.INPUT_MOVE, OnBeginMove);
		m_dicMessageTreat.Add(eMessageType.INPUT_AUTO_MOVE, OnBeginAutoMove);	
		m_dicMessageTreat.Add(eMessageType.OTHER_USER_CLICK, OnOtherUserClick);
		m_dicMessageTreat.Add(eMessageType.NPC_CLICK, OnNpcClick);
		m_dicMessageTreat.Add(eMessageType.COLLECT_CLICK, OnCollectClick);
		m_dicMessageTreat.Add(eMessageType.OBJECT_CLICK, OnObjectClick);
		m_dicMessageTreat.Add(eMessageType.CHEAT_DEATH, OnCheatDeath);		
		m_dicMessageTreat.Add(eMessageType.EMOTION_INDICATION, OnEmotionIndicate);		
		m_dicMessageTreat.Add(eMessageType.ANIMATION_END, OnAnimationEnd);
	}
	
	~AsPlayerState_Product()
	{
		ReleaseIdleAction();
	}
	#endregion
	#region - fsm function -
	public override void Enter(AsIMessage _msg)
	{
		//m_OwnerFsm.Entity.HandleMessage(new Msg_MoveStopIndication());
		//m_OwnerFsm.UserEntity.HandleMessage(new Msg_CombatEnd());
		
		//if(AsGameMain.s_gameState == GAME_STATE.STATE_INGAME)
		//	AsCommonSender.SendMove(eMoveType.Sync_Stop, m_OwnerFsm.transform.position, m_OwnerFsm.transform.position);
	
		m_OwnerFsm.UserEntity.ShowProductImg( true );
	}
	public override void Update()
	{
		if( false == AsUserInfo.Instance.isProductionProgress )
		{
			m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.IDLE);
		}
	}
	public override void Exit()
	{
		m_OwnerFsm.UserEntity.ShowProductImg( false );
		ReleaseIdleAction();		
		m_OwnerFsm.ReleaseElements();
	}
	#endregion
	
	//ACT
	#region - begin move -
	void OnBeginMove(AsIMessage _msg)
	{
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.RUN, _msg);
	}
	#endregion
	#region - begin auto move -
	void OnBeginAutoMove(AsIMessage _msg)
	{
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.RUN, _msg);
	}
	#endregion
	#region - other user click -
	void OnOtherUserClick(AsIMessage _msg)
	{	
//		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.RUN_OTHERUSER, _msg);
	}
	#endregion
	#region - npc click -
	void OnNpcClick(AsIMessage _msg)
	{
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.RUN_NPC, _msg);
	}	
	
	#endregion
	void OnCollectClick(AsIMessage _msg)
	{
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.RUN_COLLECT, _msg);
	}
	
	#region - object click -
	void OnObjectClick(AsIMessage _msg)
	{
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.RUN_OBJECT, _msg);
	}
	#endregion
	#region - begin attack -
	void OnBeginAttack(AsIMessage _msg)
	{
//		if(m_OwnerFsm.WeaponEquip == true)
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.BATTLE_IDLE, _msg);
	}
	#endregion
	#region - release tension -
	void OnReleaseTension(AsIMessage _msg)
	{
		#region - anim speed -
		float moveSpeed = m_OwnerFsm.Entity.GetProperty<float>(eComponentProperty.MOVE_SPEED);
		eRACE race = m_OwnerFsm.Entity.GetProperty<eRACE>(eComponentProperty.RACE);
		eCLASS __class = m_OwnerFsm.Entity.GetProperty<eCLASS>(eComponentProperty.CLASS);
		Tbl_Class_Record classRecord = AsTableManager.Instance.GetTbl_Class_Record(race, __class);
		float animSpeed = moveSpeed / classRecord.MoveSpeed * 100;
		#endregion
		
		m_OwnerFsm.PlayAction_Idle();
		m_OwnerFsm.UserEntity.HandleMessage(new Msg_AnimationIndicate("Idle", 0.1f, animSpeed));
		
		SetIdleAction();
	}
	#endregion
	
	//SKILL
	#region - skill
	void OnTargetSkillMove(AsIMessage _msg)
	{
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.BATTLE_RUN, _msg);
	}
	
	void OnSkillReady(AsIMessage _msg)
	{
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.SKILL_READY, _msg);
	}
	#endregion
	#region - skill(obsolete) -
	#region - begin charge -
	void OnBeginCharge(AsIMessage _msg)
	{
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.SKILL_READY, _msg);
	}
	#endregion
	#region - cancel charge -
	void OnCancelCharge( AsIMessage _msg)
	{
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.BATTLE_IDLE);
	}
	#endregion
	#region - active -
	void OnBeginActiveSkill(AsIMessage _msg)
	{
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.SKILL_READY, _msg);
	}
	#endregion
	#region - drag dash -
	void OnBeginDragStraightSkill(AsIMessage _msg)
	{
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.SKILL_READY, _msg);
	}
	#endregion
	#region - arc -
	void OnBeginArcSkill(AsIMessage _msg)
	{
//		eCLASS __class = m_OwnerFsm.UserEntity.GetProperty<eCLASS>(eComponentProperty.CLASS);
//		Msg_Input_Arc msg = _msg as Msg_Input_Arc;
//		Msg_Player_Skill_Ready ready = new Msg_Player_Skill_Ready(
//			__class, eSKILL_TYPE.Command, eSKILL_INPUT_TYPE.Arc, 1,
//			msg.head_, msg.center_, msg.tail_, msg.direction_);
//		
//		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.SKILL_READY, ready);
		
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.SKILL_READY, _msg);
	}
	#endregion
	#endregion
	
	//ACT
	#region - dash -
	void OnBeginDash(AsIMessage _msg)
	{
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.DASH, _msg);
	}
	#endregion
	
	//EMOTION
	#region - emotion -
	void OnEmotionIndicate(AsIMessage _msg)
	{
		if(m_OwnerFsm.Entity.GetProperty<bool>(eComponentProperty.COMBAT) == true)
			return;
		
		Msg_EmotionIndicate emotion = _msg as Msg_EmotionIndicate;
		eCLASS __class = m_OwnerFsm.Entity.GetProperty<eCLASS>(eComponentProperty.CLASS);
		eGENDER gender = m_OwnerFsm.Entity.GetProperty<eGENDER>(eComponentProperty.GENDER);
		
		int actionIdx = -1;
		
		switch(__class)
		{
		case eCLASS.DIVINEKNIGHT:
			if(gender == eGENDER.eGENDER_MALE)
				actionIdx = emotion.record_.DivineKnightAction_Male;
			else if(gender == eGENDER.eGENDER_FEMALE)
				actionIdx = emotion.record_.DivineKnightAction_Female;
			break;
		case eCLASS.CLERIC:
			if(gender == eGENDER.eGENDER_MALE)
				actionIdx = emotion.record_.ClericAction_Male;
			else if(gender == eGENDER.eGENDER_FEMALE)
				actionIdx = emotion.record_.ClericAction_Female;
			break;
		case eCLASS.MAGICIAN:
			if(gender == eGENDER.eGENDER_MALE)
				actionIdx = emotion.record_.MagicianAction_Male;
			else if(gender == eGENDER.eGENDER_FEMALE)
				actionIdx = emotion.record_.MagicianAction_Female;
			break;
		case eCLASS.HUNTER:
			if(gender == eGENDER.eGENDER_MALE)
				actionIdx = emotion.record_.HunterAction_Male;
			else if(gender == eGENDER.eGENDER_FEMALE)
				actionIdx = emotion.record_.HunterAction_Female;
			break;
		}
		
		Tbl_Action_Record action = AsTableManager.Instance.GetTbl_Action_Record(actionIdx);
		if(action != null)
		{
			m_OwnerFsm.SetAction(action);
			m_OwnerFsm.PlayElements(eActionStep.Entire);
			m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate(action.ReadyAnimation.FileName));
		}
		else
			Debug.Log("AsPlayerState_Idle::OnEmotionIndicate: action record is not found. emotion is " + emotion.record_.EmotionName + ", index = " + actionIdx);
	}
	#endregion
	
	//DAMAGED
	#region - damaged -
	void OnDamaged(AsIMessage _msg)
	{
		if(m_OwnerFsm.WeaponEquip == true)
		{
			Msg_NpcAttackChar2 damaged = _msg as Msg_NpcAttackChar2;
			
			if(m_OwnerFsm.Target == null)
				m_OwnerFsm.Target = damaged.parent_.attacker_;
			
			if(m_OwnerFsm.Entity.GetProperty<bool>(eComponentProperty.COMBAT) == true)
			{
				m_OwnerFsm.PlayAction_BattleIdle();
				m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("BattleIdle"));
			}
			
			if(m_OwnerFsm.Target.FsmType == eFsmType.MONSTER)
				m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.BATTLE_IDLE);
		}
	}
	#endregion
	
	#region - anim end -
	void OnAnimationEnd(AsIMessage _msg)
	{
		Msg_AnimationEnd end = _msg as Msg_AnimationEnd;
		if(end.animName_ == "IdleAction")
		{
			#region - anim speed -
			float moveSpeed = m_OwnerFsm.Entity.GetProperty<float>(eComponentProperty.MOVE_SPEED);
			eRACE race = m_OwnerFsm.Entity.GetProperty<eRACE>(eComponentProperty.RACE);
			eCLASS __class = m_OwnerFsm.Entity.GetProperty<eCLASS>(eComponentProperty.CLASS);
			Tbl_Class_Record classRecord = AsTableManager.Instance.GetTbl_Class_Record(race, __class);
			float animSpeed = moveSpeed / classRecord.MoveSpeed * 100;
			#endregion
			
			m_OwnerFsm.PlayAction_Idle();
			m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("Idle", 0.1f, animSpeed));
			
			SetIdleAction();
		}
	}
	#endregion
	
	// CHEAT
	#region - cheat death -
	void OnCheatDeath( AsIMessage _msg)
	{
//		m_OwnerFsm.SetPlayerFsmState( AsPlayerFsm.ePlayerFsmStateType.DEATH, _msg);
		m_OwnerFsm.CheatDeath( _msg);
	}
	#endregion
//	#endregion
	
	#region - method -
	bool CheckTarget()
	{
		if(m_OwnerFsm.WeaponEquip == true &&
			(m_OwnerFsm.Target != null && m_OwnerFsm.Target.FsmType == eFsmType.MONSTER))
		{
			float dist = Vector3.Distance(m_OwnerFsm.Target.transform.position, m_OwnerFsm.Entity.transform.position);
			float viewRange = m_OwnerFsm.UserEntity.GetProperty<float>(eComponentProperty.VIEW_DISTANCE);
			if(dist < viewRange)
			{
				float attackRange = m_OwnerFsm.UserEntity.GetProperty<float>(eComponentProperty.ATTACK_DISTANCE);
				if(dist < attackRange)
					m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.BATTLE_IDLE);
				else
					m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.BATTLE_RUN);
			}
			
			return true;
		}
		else
			return false;
	}
	#endregion
	
	#region - idle action -
	void Timer_IdleAction(System.Object _obj)
	{
		if(m_OwnerFsm.CurrnetFsmStateType == AsPlayerFsm.ePlayerFsmStateType.IDLE &&
			m_OwnerFsm.Entity.GetProperty<bool>(eComponentProperty.COMBAT) == false)
		{
			#region - anim speed -
			float moveSpeed = m_OwnerFsm.Entity.GetProperty<float>(eComponentProperty.MOVE_SPEED);
			eRACE race = m_OwnerFsm.Entity.GetProperty<eRACE>(eComponentProperty.RACE);
			eCLASS __class = m_OwnerFsm.Entity.GetProperty<eCLASS>(eComponentProperty.CLASS);
			Tbl_Class_Record classRecord = AsTableManager.Instance.GetTbl_Class_Record(race, __class);
			float animSpeed = moveSpeed / classRecord.MoveSpeed * 100;
			#endregion
			
			m_OwnerFsm.PlayAction_IdleAction();
			m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("IdleAction", 0.1f, animSpeed));
		}
	}
	
	void SetIdleAction()
	{
		ReleaseIdleAction();
		
		float cycle = m_OwnerFsm.IdleActionCycle + Random.Range(-m_OwnerFsm.IdleActionRevision, m_OwnerFsm.IdleActionRevision);
		m_IdleActionTimer = AsTimerManager.Instance.SetTimer(cycle, Timer_IdleAction);
	}
	
	void ReleaseIdleAction()
	{
		if(m_IdleActionTimer != null)
		{
			AsTimerManager.Instance.ReleaseTimer(m_IdleActionTimer);
			m_IdleActionTimer = null;
		}
	}
	#endregion
	
}
