using UnityEngine;
using System.Collections;

public class AsOtherUserState_Idle : AsBaseFsmState<AsOtherUserFsm.eOtherUserFsmStateType, AsOtherUserFsm> {
	
	AsTimer m_IdleActionTimer;
	
	const string strBattleIdle = "BattleIdle{0}";
	System.Text.StringBuilder m_AnimName = new System.Text.StringBuilder();
	
	public AsOtherUserState_Idle(AsOtherUserFsm _fsm) : base(AsOtherUserFsm.eOtherUserFsmStateType.IDLE, _fsm)
	{
		m_dicMessageTreat.Add(eMessageType.MOVE_OTHER_USER_INDICATION, OnBeginMove);
		m_dicMessageTreat.Add(eMessageType.OTHER_CHAR_ATTACK_READY, OnBeginAttack);
		m_dicMessageTreat.Add(eMessageType.RELEASE_TENSION, OnReleaseTension);
		m_dicMessageTreat.Add(eMessageType.ANIMATION_END, OnAnimationEnd);
		m_dicMessageTreat.Add(eMessageType.COLLECT_INFO, OnCollectInfo);
		
		m_dicMessageTreat.Add(eMessageType.EMOTION_INDICATION, OnEmotionIndicate);
		m_dicMessageTreat.Add(eMessageType.EMOTICON_SEAT_INDICATION, OnEmoticonSeatIndicate);
		
		m_dicMessageTreat.Add(eMessageType.NPC_ATTACK_CHAR2, OnDamaged);
		m_dicMessageTreat.Add(eMessageType.CHAR_ATTACK_NPC3, OnAffected);
	}
	~AsOtherUserState_Idle()
	{
//		ReleaseIdleAction();
	}
	
	#region - fsm function -
	public override void Enter(AsIMessage _msg)
	{
		m_OwnerFsm.Entity.HandleMessage(new Msg_MoveStopIndication());
		m_OwnerFsm.OtherUserEntity.HandleMessage(new Msg_CombatEnd());
		
		if(_msg == null || _msg.MessageType != eMessageType.PREV_STATE_PRIVATESHOP)
		{
			CheckCombat();
			SetIdleAction();
		}
	}
	public override void Update()
	{
	}
	public override void Exit()
	{
		ReleaseIdleAction();
		
		m_OwnerFsm.ReleaseElements();
	}
	#endregion
	
	#region - enter -
	void CheckCombat()
	{
//		#region - anim speed -
//		float moveSpeed = m_OwnerFsm.Entity.GetProperty<float>(eComponentProperty.MOVE_SPEED);
//		eRACE race = m_OwnerFsm.Entity.GetProperty<eRACE>(eComponentProperty.RACE);
//		eCLASS __class = m_OwnerFsm.Entity.GetProperty<eCLASS>(eComponentProperty.CLASS);
//		Tbl_Class_Record classRecord = AsTableManager.Instance.GetTbl_Class_Record(race, __class);
//		float animSpeed = moveSpeed / classRecord.MoveSpeed * 100;
//		#endregion
		
		bool combat = m_OwnerFsm.OtherUserEntity.GetProperty<bool>(eComponentProperty.COMBAT);
		
		if(combat == true && m_OwnerFsm.WeaponEquip == true)
		{
			m_OwnerFsm.PlayAction_BattleIdle();
			
			#region - stance -
			if(m_AnimName.Length > 0)
				m_AnimName.Remove(0, m_AnimName.Length);
			m_AnimName.AppendFormat(strBattleIdle, m_OwnerFsm.OtherUserEntity.StanceNumber);
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
	
	#region - msg -
	void OnBeginMove(AsIMessage _msg)
	{
		m_OwnerFsm.SetOtherUserFsmState(AsOtherUserFsm.eOtherUserFsmStateType.RUN, _msg);
	}
	
	void OnBeginAttack(AsIMessage _msg)
	{
//		m_OwnerFsm.SetOtherUserFsmState(AsOtherUserFsm.eOtherUserFsmStateType.SKILL, _msg);
		m_OwnerFsm.SetOtherUserFsmState(AsOtherUserFsm.eOtherUserFsmStateType.SKILL_READY, _msg);
	}
	
	void OnReleaseTension(AsIMessage _msg)
	{
		m_OwnerFsm.PlayAction_Idle();
		
		float animSpeed = GetAnimationSpeedByMove_UserEntity("Idle");
		m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("Idle", 0.1f, animSpeed));
		
		SetIdleAction();
	}
	
	void OnAnimationEnd(AsIMessage _msg)
	{
		Msg_AnimationEnd end = _msg as Msg_AnimationEnd;
		if(end.animName_ == "IdleAction")
		{
			m_OwnerFsm.PlayAction_Idle();
			
			float animSpeed = GetAnimationSpeedByMove_UserEntity("Idle");
			m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("Idle", 0.1f, animSpeed));
			
			SetIdleAction();
		}
	}
	
	
	void OnCollectInfo(AsIMessage _msg)
	{
		m_OwnerFsm.SetOtherUserFsmState(AsOtherUserFsm.eOtherUserFsmStateType.COLLECT_WORK, _msg);
	}
	
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
	
	void OnEmoticonSeatIndicate(AsIMessage _msg)
	{
		if(m_OwnerFsm.OtherUserEntity.GetProperty<bool>(eComponentProperty.COMBAT) == false)
		{
			ReleaseIdleAction();
			m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("Seat_Action", 0.1f));
		}
	}
	
	void OnDamaged(AsIMessage _msg)
	{
		if(m_OwnerFsm.WeaponEquip == true)
		{
			if(m_OwnerFsm.Entity.GetProperty<bool>(eComponentProperty.COMBAT) == true)
			{
				m_OwnerFsm.PlayAction_BattleIdle();
				m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("BattleIdle"));
			}
		}
	}
	
	void OnAffected(AsIMessage _msg)
	{
		if(m_OwnerFsm.WeaponEquip == true)
		{
			if(m_OwnerFsm.Entity.GetProperty<bool>(eComponentProperty.COMBAT) == true)
			{
				m_OwnerFsm.PlayAction_BattleIdle();
				
				float animSpeed = GetAnimationSpeedByMove_UserEntity("BattleIdle");
				m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("BattleIdle", 0.1f, animSpeed));
			}
		}
	}
	#endregion
	
	
	
	#region - idle action -
	void Timer_IdleAction(System.Object _obj)
	{
		if(m_OwnerFsm.CurrnetFsmStateType == AsOtherUserFsm.eOtherUserFsmStateType.IDLE &&
			m_OwnerFsm.Entity.GetProperty<bool>(eComponentProperty.COMBAT) == false)
		{
			m_OwnerFsm.PlayAction_IdleAction();
			
			m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("IdleAction"));
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
	
//	#endregion
}
