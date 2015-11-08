using UnityEngine;
using System.Collections;

public class AsPlayerState_Idle : AsBaseFsmState<AsPlayerFsm.ePlayerFsmStateType, AsPlayerFsm>
{
	AsTimer m_IdleActionTimer = null;
	
	const string strBattleIdle = "BattleIdle{0}";
	System.Text.StringBuilder m_AnimName = new System.Text.StringBuilder();
	
	float m_AutoCombatRange = 1f;
	
	#region - init -
	public AsPlayerState_Idle(AsPlayerFsm _fsm) : base(AsPlayerFsm.ePlayerFsmStateType.IDLE, _fsm)
	{				
		m_dicMessageTreat.Add(eMessageType.INPUT_MOVE, OnBeginMove);
		m_dicMessageTreat.Add(eMessageType.INPUT_AUTO_MOVE, OnBeginAutoMove);
		m_dicMessageTreat.Add(eMessageType.INPUT_ATTACK, OnBeginAttack);
		m_dicMessageTreat.Add(eMessageType.OTHER_USER_CLICK, OnOtherUserClick);
		m_dicMessageTreat.Add(eMessageType.NPC_CLICK, OnNpcClick);
		m_dicMessageTreat.Add(eMessageType.COLLECT_CLICK, OnCollectClick);
		m_dicMessageTreat.Add(eMessageType.OBJECT_CLICK, OnObjectClick);
		m_dicMessageTreat.Add(eMessageType.RELEASE_TENSION, OnReleaseTension);
		m_dicMessageTreat.Add(eMessageType.PLAYER_SKILL_TARGET_MOVE, OnTargetSkillMove);
		m_dicMessageTreat.Add(eMessageType.PLAYER_SKILL_READY, OnSkillReady);
		m_dicMessageTreat.Add(eMessageType.INPUT_DASH, OnBeginDash);
		m_dicMessageTreat.Add(eMessageType.CHEAT_DEATH, OnCheatDeath);
		
		m_dicMessageTreat.Add(eMessageType.EMOTION_INDICATION, OnEmotionIndicate);
		m_dicMessageTreat.Add(eMessageType.EMOTICON_SEAT_INDICATION, OnEmotionSeatIndicate);
		
		m_dicMessageTreat.Add(eMessageType.NPC_ATTACK_CHAR2, OnDamaged);
		m_dicMessageTreat.Add(eMessageType.CHAR_ATTACK_NPC3, OnAffected);
		
		m_dicMessageTreat.Add(eMessageType.ANIMATION_END, OnAnimationEnd);
		
		m_dicMessageTreat.Add(eMessageType.CONDITION_BINDING, OnBinding);
		
		m_dicMessageTreat.Add(eMessageType.AUTOCOMBAT_SEARCH, OnAutoCombatSearch);
		
		m_AutoCombatRange = AsTableManager.Instance.GetTbl_GlobalWeight_Record(30).Value * 0.01f;
	}
	
	~AsPlayerState_Idle()
	{
		ReleaseIdleAction();
	}
	#endregion
	#region - fsm function -
	public override void Enter(AsIMessage _msg)
	{
		m_OwnerFsm.Entity.HandleMessage(new Msg_MoveStopIndication());
		m_OwnerFsm.UserEntity.HandleMessage(new Msg_CombatEnd());
		
		if(AsGameMain.s_gameState == GAME_STATE.STATE_INGAME)
		{
//			if(_msg != null && _msg.MessageType == eMessageType.PLAYER_SKILL_FINISH)
//				m_OwnerFsm.MovePacketSynchronize();
//			else
				m_OwnerFsm.MovePacketRefresh( eMoveType.Sync_Stop, m_OwnerFsm.transform.position, m_OwnerFsm.transform.position);
		}
		
		if(_msg == null || _msg.MessageType != eMessageType.PREV_STATE_PRIVATESHOP)
		{
			CheckCombat();
			SetIdleAction();
		}
	}
	public override void Update()
	{
		CheckNearMonster();
	}
	public override void Exit()
	{
		ReleaseIdleAction();
		
		m_OwnerFsm.ReleaseElements();
	}
	#endregion
	//ENTER
	#region - enter -
	void CheckCombat()
	{
		bool combat = m_OwnerFsm.UserEntity.GetProperty<bool>(eComponentProperty.COMBAT);
//		AsNotify.Log("set to IDLE. combat state is " + combat);
		
		
		
		if(combat == true && m_OwnerFsm.WeaponEquip == true)
		{
			m_OwnerFsm.PlayAction_BattleIdle();
			
			#region - stance -
			if(m_AnimName.Length > 0)
				m_AnimName.Remove(0, m_AnimName.Length);
			m_AnimName.AppendFormat(strBattleIdle, m_OwnerFsm.UserEntity.StanceNumber);
			#endregion
			
			float animSpeed = GetAnimationSpeedByMove_UserEntity("BattleIdle");
			m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate(m_AnimName.ToString(), 0.1f, animSpeed));
		}
		else
		{
			m_OwnerFsm.PlayAction_Idle();
			
			float animSpeed = GetAnimationSpeedByMove_UserEntity("Idle");
			m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("Idle", 0.1f, animSpeed));
		}
	}
	#endregion
	
	//UPDATE
	#region - check near monster -
	void CheckNearMonster()
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
					if( AutoCombatManager.Instance.Activate == true) viewRange *= 2f; // auto combat
					if(dist < viewRange)
					{
//						float attackRange = m_OwnerFsm.UserEntity.GetProperty<float>(eComponentProperty.ATTACK_DISTANCE);
//						if(dist < attackRange)
//							m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.BATTLE_IDLE);
//						else
//							m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.BATTLE_RUN);
						
						SuitableBasicSkill suit = m_OwnerFsm.GetSuitableBasicSkill(dist);
						if(suit.InRange == true)
							m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.BATTLE_IDLE);
						else
							m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.BATTLE_RUN);
					}
				}
			}
			else
				return;
		}
	}
	#endregion
	
//	#region - msg -
	
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
		Msg_OtherUserClick otherUserClick =  _msg as Msg_OtherUserClick;
		AsUserEntity entity = AsEntityManager.Instance.GetUserEntityByUniqueId(otherUserClick.idx_);	
		if(entity.GetProperty<bool>(eComponentProperty.SHOP_OPENING) == true)
		{
			m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.RUN_OTHERUSER, _msg);
		}
		else
		{
			if(entity.GetProperty<bool>(eComponentProperty.LIVING) == true)
			{			
				if(TargetDecider.CheckOtherUserIsEnemy(entity) == true)
				{
					m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.BATTLE_IDLE, _msg);
				}
			}
		}
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
//		#region - anim speed -
//		float moveSpeed = m_OwnerFsm.Entity.GetProperty<float>(eComponentProperty.MOVE_SPEED);
//		eRACE race = m_OwnerFsm.Entity.GetProperty<eRACE>(eComponentProperty.RACE);
//		eCLASS __class = m_OwnerFsm.Entity.GetProperty<eCLASS>(eComponentProperty.CLASS);
//		Tbl_Class_Record classRecord = AsTableManager.Instance.GetTbl_Class_Record(race, __class);
//		float animSpeed = moveSpeed / classRecord.MoveSpeed * 100;
//		if( 0 == animSpeed)
//			animSpeed = 1.0f;
//		#endregion
		
		m_OwnerFsm.PlayAction_Idle();
		
		float animSpeed = GetAnimationSpeedByMove_UserEntity("Idle");
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
	
	void OnEmotionSeatIndicate(AsIMessage _msg)
	{
		if(m_OwnerFsm.UserEntity.GetProperty<bool>(eComponentProperty.COMBAT) == false)
		{
			ReleaseIdleAction();
			m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("Seat_Action", 0.1f));
		}
	}
	#endregion
	
	//DAMAGED
	#region - damaged & affected -
	void OnDamaged(AsIMessage _msg)
	{
		if(m_OwnerFsm.WeaponEquip == true)
		{
			Msg_NpcAttackChar2 damaged = _msg as Msg_NpcAttackChar2;
			
			if(m_OwnerFsm.Target == null ||
				m_OwnerFsm.Target != null &&
				(m_OwnerFsm.Target.FsmType == eFsmType.NPC || m_OwnerFsm.Target.FsmType == eFsmType.COLLECTION))// exceptional targeting by npc
			{
				if( damaged.parent_.attacker_.isSegregate == false)
					m_OwnerFsm.Target = damaged.parent_.attacker_;
			}
			
			if(m_OwnerFsm.Entity.GetProperty<bool>(eComponentProperty.COMBAT) == true)
			{
				m_OwnerFsm.PlayAction_BattleIdle();
				
				float animSpeed = GetAnimationSpeedByMove_UserEntity("BattleIdle");
				m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("BattleIdle", 0.1f, animSpeed));
			}
			
			if(m_OwnerFsm.Target != null && m_OwnerFsm.Target.FsmType == eFsmType.MONSTER)
				m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.BATTLE_IDLE);
		}
	}
	
	void OnAffected(AsIMessage _msg)
	{
		if(m_OwnerFsm.WeaponEquip == true)
		{
//			Msg_NpcAttackChar3 damaged = _msg as Msg_NpcAttackChar3;
			
//			if(m_OwnerFsm.Target == null ||
//				m_OwnerFsm.Target != null &&
//				(m_OwnerFsm.Target.FsmType == eFsmType.NPC || m_OwnerFsm.Target.FsmType == eFsmType.COLLECTION))// exceptional targeting by npc
//				m_OwnerFsm.Target = damaged.parent_.attacker_;
			
			if(m_OwnerFsm.Entity.GetProperty<bool>(eComponentProperty.COMBAT) == true)
			{
				m_OwnerFsm.PlayAction_BattleIdle();
				
				float animSpeed = GetAnimationSpeedByMove_UserEntity("BattleIdle");
				m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("BattleIdle", 0.1f, animSpeed));
			}
			
//			if(m_OwnerFsm.Target.FsmType == eFsmType.MONSTER)
//				m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.BATTLE_IDLE);
		}
	}
	#endregion
	
	#region - anim end -
	void OnAnimationEnd(AsIMessage _msg)
	{
		Msg_AnimationEnd end = _msg as Msg_AnimationEnd;
		if(end.animName_ != "Idle")
		{
//			#region - anim speed -
//			float moveSpeed = m_OwnerFsm.Entity.GetProperty<float>(eComponentProperty.MOVE_SPEED);
//			eRACE race = m_OwnerFsm.Entity.GetProperty<eRACE>(eComponentProperty.RACE);
//			eCLASS __class = m_OwnerFsm.Entity.GetProperty<eCLASS>(eComponentProperty.CLASS);
//			Tbl_Class_Record classRecord = AsTableManager.Instance.GetTbl_Class_Record(race, __class);
//			float animSpeed = moveSpeed / classRecord.MoveSpeed * 100;
//			if( 0 == animSpeed)
//				animSpeed = 1.0f;
//				#endregion
			
			m_OwnerFsm.PlayAction_Idle();
			
			float animSpeed = GetAnimationSpeedByMove_UserEntity("Idle");
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
	
//	#region - method -
//	bool CheckTarget()
//	{
//		if(m_OwnerFsm.WeaponEquip == true &&
//			(m_OwnerFsm.Target != null && m_OwnerFsm.Target.FsmType == eFsmType.MONSTER))
//		{
//			float dist = Vector3.Distance(m_OwnerFsm.Target.transform.position, m_OwnerFsm.Entity.transform.position);
//			float viewRange = m_OwnerFsm.UserEntity.GetProperty<float>(eComponentProperty.VIEW_DISTANCE);
//			if(dist < viewRange)
//			{
////				float attackRange = m_OwnerFsm.UserEntity.GetProperty<float>(eComponentProperty.ATTACK_DISTANCE);
////				if(dist < attackRange)
////					m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.BATTLE_IDLE);
////				else
////					m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.BATTLE_RUN);
//				
//				SuitableBasicSkill suit = m_OwnerFsm.GetSuitableBasicSkill(dist);
//				if(suit.InRange == true)
//					
//				
//			}
//			
//			return true;
//		}
//		else
//			return false;
//	}
//	#endregion
	
	#region - idle action -
	void Timer_IdleAction(System.Object _obj)
	{
		if(m_OwnerFsm.CurrnetFsmStateType == AsPlayerFsm.ePlayerFsmStateType.IDLE &&
			m_OwnerFsm.Entity.GetProperty<bool>(eComponentProperty.COMBAT) == false)
		{
//			#region - anim speed -
//			float moveSpeed = m_OwnerFsm.Entity.GetProperty<float>(eComponentProperty.MOVE_SPEED);
//			eRACE race = m_OwnerFsm.Entity.GetProperty<eRACE>(eComponentProperty.RACE);
//			eCLASS __class = m_OwnerFsm.Entity.GetProperty<eCLASS>(eComponentProperty.CLASS);
//			Tbl_Class_Record classRecord = AsTableManager.Instance.GetTbl_Class_Record(race, __class);
//			float animSpeed = moveSpeed / classRecord.MoveSpeed * 100;
//			if( 0 == animSpeed)
//				animSpeed = 1.0f;
//			#endregion
			
			m_OwnerFsm.PlayAction_IdleAction();
			
			float animSpeed = GetAnimationSpeedByMove_UserEntity("IdleAction");
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
	
	void OnBinding(AsIMessage _msg)
	{
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.CONDITION_BINDING, _msg);
	}
	
	void OnAutoCombatSearch( AsIMessage _msg)
	{
//		eCLASS __class = m_OwnerFsm.Entity.GetProperty<eCLASS>( eComponentProperty.CLASS);
//		Tbl_Skill_Record record = AsTableManager.Instance.GetRandomBaseSkill( __class);
//		
//		TargetDecider.sTargeting_AsSlot( m_OwnerFsm, record);//$yde
//
//		if( true == TargetDecider.CheckValidSkill( m_OwnerFsm, record))
//			m_OwnerFsm.Entity.HandleMessage(new Msg_Input_Slot_Active( record.Index, 0));
		
//		if(m_OwnerFsm.Target == null)
//			m_OwnerFsm.Target = m_OwnerFsm.MonsterExistInRange(m_AutoCombatRange);
		
		Msg_AutoCombat_Search search = _msg as Msg_AutoCombat_Search;
		
		Tbl_Skill_Record record = AsTableManager.Instance.GetTbl_Skill_Record(search.skillIdx_);
		
		TargetDecider.CheckDisableSkillByCondition(m_OwnerFsm, record);
		TargetDecider.CheckDisableSkillByMap(record);
		
		TargetDecider.sTargeting_AsSlot( m_OwnerFsm, record);

		if( null == m_OwnerFsm.Target)
			AsMyProperty.Instance.AlertInvalidTarget();
		else
		{
			if( true == TargetDecider.TargetSkillCheck( record.Index, m_OwnerFsm))
			{
				if( true == TargetDecider.IsAvailableSkill( record.Index, 1))
				{
					// CHECKING COOLTIME IS EXCEPTED
					
					m_OwnerFsm.Entity.HandleMessage(new Msg_Input_Slot_Active( search.skillIdx_, search.step_));
				}
			}
		}
	}
}
