using UnityEngine;
using System.Collections;

public class AsPetState_Idle : AsBaseFsmState<AsPetFsm.ePetFsmStateType, AsPetFsm> {
	
	AsTimer m_IdleActionTimer = null;
	
	float m_HungryGrade = 0f;
	
	bool m_InTown = false;
	
	public AsPetState_Idle(AsPetFsm _fsm) : base(AsPetFsm.ePetFsmStateType.IDLE, _fsm)
	{		
		m_dicMessageTreat.Add(eMessageType.MOVE_NPC_INDICATION, OnBeginMove);
		m_dicMessageTreat.Add(eMessageType.NPC_ATTACK_CHAR1, OnBeginAttack);
		m_dicMessageTreat.Add(eMessageType.ANIMATION_END, OnAnimationEnd);
		
		m_dicMessageTreat.Add( eMessageType.PET_FEEDING, OnFeeding);
		m_dicMessageTreat.Add( eMessageType.PET_HATCH, OnHatchIndicate);
		m_dicMessageTreat.Add( eMessageType.PET_HUNGRY_INDICATE, OnHungryIndicate);
		m_dicMessageTreat.Add( eMessageType.PET_SCRIPT_INDICATE, OnScriptIndicate);
	}
	
	#region - fsm function -
	public override void Enter(AsIMessage _msg)
	{
		m_CountDlgClosed = false;
		m_ArenaBegan = false;
		
		m_OwnerFsm.Entity.HandleMessage( new Msg_MoveStopIndication());
		
//		m_HungryGrade = AsTableManager.Instance.GetTbl_GlobalWeight_Record( "Pet_Hungry").Value;
		m_HungryGrade = 5000;
		
		EnterIdle();
		
		m_OwnerFsm.SetAction(m_Action);
		m_OwnerFsm.PlayElements(eActionStep.Entire, 1f);
		
		m_InTown = TerrainMgr.Instance.IsCurMapType( eMAP_TYPE.Town);

		m_OwnerFsm.InvokeRepeating("CheckAbsoluteFollow_Inv", 0f, 5f);
	}
	public override void Update()
	{
		if( CheckOwnerInRange() == false)
		{
			m_OwnerFsm.SetPetFsmState(AsPetFsm.ePetFsmStateType.RUN);
			return;
		}
	}
	public override void Exit()
	{
		m_OwnerFsm.ReleaseElements();

		m_OwnerFsm.CancelInvoke("CheckAbsoluteFollow_Inv");
	}
	#endregion
	#region - update -
	public bool CheckOwnerInRange()
	{
		if(m_OwnerFsm.PetOwner == null)
		{
			Debug.LogWarning("AsPetState_Idle:: CheckOwnerInRange: m_OwnerFsm.PetOwner is null");
			return true;
		}

		if( m_OwnerFsm.PetOwner != null)
		{
			if( ( m_OwnerFsm.PetOwner.transform.position - m_OwnerFsm.transform.position).sqrMagnitude > AsPetFsm.s_sqrFollowOwnerRange)
				return false;
		}
		
		return true;
	}
	
	public bool CheckSkillUsable()
	{
		if(AsGameMain.s_gameState != GAME_STATE.STATE_INGAME)
			return false;

		if( m_OwnerFsm.PetOwner == null)
			return false;
		
		if( m_OwnerFsm.PetOwner.FsmType != eFsmType.PLAYER)
			return false;
		
		if( m_InTown == true)
			return false;
		
		if( true == TargetDecider.CheckCurrentMapIsArena())
		{
			if( AsPvpManager.Instance.PetSkillAvailable == false)
				return false;
		}
		
		if( m_OwnerFsm.CheckSkillUsable( ePET_SKILL_TYPE.ePET_SKILL_TYPE_ACTIVE) == true &&
			m_OwnerFsm.PetOwner.GetProperty<bool>(eComponentProperty.LIVING) == true)
			return true;
		else
			return false;
	}
	
//	void _UseActiveSkill()
//	{
//		sPETSKILL petSkill = m_OwnerFsm.GetSkill( ePET_SKILL_TYPE.ePET_SKILL_TYPE_ACTIVE);
//		if( petSkill != null && petSkill.nSkillTableIdx > 0)
//		{
//			bool skillPlay = true;
//			
//			Tbl_Skill_Record skillRec = AsTableManager.Instance.GetTbl_Skill_Record( petSkill.nSkillTableIdx);
//			if( skillRec != null)
//			{
//				foreach( Tbl_Skill_Potency node in skillRec.listSkillPotency)
//				{		
//					if( node.Potency_Type == ePotency_Type.Heal)
//					{
//						skillPlay = false;
//						
//						float curHp = m_OwnerFsm.PetOwner.GetProperty<float>(eComponentProperty.HP_CUR);
//						float maxHp = m_OwnerFsm.PetOwner.GetProperty<float>(eComponentProperty.HP_MAX);
//						if(curHp / maxHp < node.Potency_Enable_ConditionValue * 0.001f)
//							skillPlay = true;
//					}
//				}
//				
//				foreach( Tbl_Skill_Potency node in skillRec.listSkillPotency)
//				{		
//					if( node.Potency_Type == ePotency_Type.LimitShield)
//					{
//						skillPlay = false;
//						
//						float curHp = m_OwnerFsm.PetOwner.GetProperty<float>(eComponentProperty.HP_CUR);
//						float maxHp = m_OwnerFsm.PetOwner.GetProperty<float>(eComponentProperty.HP_MAX);
//						if( curHp / maxHp < node.Potency_Enable_ConditionValue * 0.001f)
//							skillPlay = true;
//					}
//				}
//			}
//			
//			if( skillPlay == true)
//			{
//				Msg_Pet_Skill_Ready ready = new Msg_Pet_Skill_Ready( petSkill);
//				if( ready.actionRecord_ == null)
//				{
//					string __class = m_OwnerFsm.Entity.GetProperty<string>(eComponentProperty.CLASS);
//					ready.actionRecord_ = AsTableManager.Instance.GetPetActionRecord( __class, "PetSkillAttack");
//				}
//				m_OwnerFsm.SetPetFsmState(AsPetFsm.ePetFsmStateType.SKILL_READY, ready);
//			}
//		}
//	}
	
	bool m_CountDlgClosed = false;
	bool m_ArenaBegan = false;
	IEnumerator _ArenaBegan_CR()
	{
		m_CountDlgClosed = true;
		
		yield return new WaitForSeconds(5f);
		
		m_ArenaBegan = true;
	}
	#endregion
	#region - process -
	void EnterIdle()
	{
		float animSpeed = 1f;
		if(m_OwnerFsm.PetOwner != null && m_OwnerFsm.PetOwner.FsmType == eFsmType.PLAYER &&
		   AsPetManager.Instance.PetHungry == ePET_HUNGRY_STATE.ePET_HUNGRY_STATE_HUNGRY)
		{
			m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("Idle", 0.1f, animSpeed));
			m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("Idle_Hungry", 0.1f, animSpeed));

			m_Action = GetAction_Pet("Idle_Hungry");
		}
		else
		{
			m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("Idle", 0.1f, animSpeed));

			m_Action = GetAction_Pet("Idle");
		}
			
		SetIdleAction();
	}
	#endregion
	#region - msg -
	void OnBeginMove(AsIMessage _msg)
	{
		m_OwnerFsm.SetPetFsmState(AsPetFsm.ePetFsmStateType.RUN, _msg);
	}
	void OnBeginAttack(AsIMessage _msg)
	{
		m_OwnerFsm.SetPetFsmState(AsPetFsm.ePetFsmStateType.SKILL_READY, _msg);
	}
	void OnAnimationEnd(AsIMessage _msg)
	{
		Msg_AnimationEnd end = _msg as Msg_AnimationEnd;
		if(end.animName_ != "Idle" && end.animName_ != "Idle_Hungry")
		{
			float animSpeed = 1f;
			if(m_OwnerFsm.PetOwner != null && m_OwnerFsm.PetOwner.FsmType == eFsmType.PLAYER &&
			   AsPetManager.Instance.PetHungry == ePET_HUNGRY_STATE.ePET_HUNGRY_STATE_HUNGRY)
			{
				m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("Idle", 0.1f, animSpeed));
				m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("Idle_Hungry", 0.1f, animSpeed));
			}
			else
				m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("Idle", 0.1f, animSpeed));
			
			SetIdleAction();
		}
	}
	
	void OnFeeding( AsIMessage _msg)
	{
		m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("Eat", 0.1f, 1f));
	}
	
	void OnHatchIndicate( AsIMessage _msg)
	{
		m_OwnerFsm.SetPetFsmState(AsPetFsm.ePetFsmStateType.HATCH, _msg);
	}
	
	void OnHungryIndicate(AsIMessage _msg)
	{
		Msg_PetHungryIndicate hungry = _msg as Msg_PetHungryIndicate;

		float animSpeed = 1f;

		if(m_OwnerFsm.PetOwner != null && m_OwnerFsm.PetOwner.FsmType == eFsmType.PLAYER &&
		   AsPetManager.Instance.PetHungry == ePET_HUNGRY_STATE.ePET_HUNGRY_STATE_HUNGRY)
			m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("Idle_Hungry", 0.1f, animSpeed));
		else
			m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("Idle", 0.1f, animSpeed));
	}

	void OnScriptIndicate(AsIMessage _msg)
	{
		if(m_OwnerFsm == null || m_OwnerFsm.PetOwner == null)
			return;

		Msg_Pet_Script_Indicate indicate = _msg as Msg_Pet_Script_Indicate;
		Tbl_PetScript_Record scriptRec = AsTableManager.Instance.GetPetScriptRecord(indicate.data_.data_.nPetPersonality_);
		if(scriptRec == null)
			return;

		string word = "not defined";
		bool combat = m_OwnerFsm.PetOwner.GetProperty<bool>(eComponentProperty.COMBAT);
		if(combat == true)
			word = scriptRec.GetScriptString(Tbl_PetScript_Record.eType.Battle);
		else if(m_OwnerFsm.PetOwner.FsmType == eFsmType.PLAYER &&
		        AsPetManager.Instance.PetHungry == ePET_HUNGRY_STATE.ePET_HUNGRY_STATE_HUNGRY)
		{
//			m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("Idle_Hungry", 0.1f, 1f));
			word = scriptRec.GetScriptString(Tbl_PetScript_Record.eType.Hungry);
		}
		else
			word = scriptRec.GetScriptString(Tbl_PetScript_Record.eType.Idle);

		Msg_Pet_Script script = new Msg_Pet_Script(word);
		m_OwnerFsm.PetOwner.HandleMessage(script);
	}
	#endregion
	#region - idle action -
	void Timer_IdleAction(System.Object _obj)
	{
		if(m_OwnerFsm.CurrnetFsmStateType == AsPetFsm.ePetFsmStateType.IDLE &&
			m_OwnerFsm.Entity.GetProperty<bool>(eComponentProperty.COMBAT) == false)
		{
			float animSpeed = GetAnimationSpeedByWalk_NpcEntity("IdleAction");
			m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("IdleAction", 0.1f, animSpeed));
		}
	}
	
	void SetIdleAction()
	{
//		ReleaseIdleAction();
//		
//		Tbl_Pet_Record monster = AsTableManager.Instance.GetTbl_Pet_Record(m_OwnerFsm.PetEntity.TableIdx);
//		if(monster.Grade == ePet_Grade.DObject)
//			return;
//		
//		float probability = AsTableManager.Instance.GetTbl_GlobalWeight_Record("PetActionIdle").Value;
//		float seed = Random.Range(0f, 1000f);
//		if(seed < probability)
//		{
//			float startTime = monster.StopTime_Min * 0.5f * 0.001f;
//			startTime = startTime * Random.Range(0f, 1f);
//			
//			m_IdleActionTimer = AsTimerManager.Instance.SetTimer(startTime, Timer_IdleAction);
//		}
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
