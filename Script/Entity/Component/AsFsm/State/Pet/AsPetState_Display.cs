using UnityEngine;
using System.Collections;

public class AsPetState_Display : AsBaseFsmState<AsPetFsm.ePetFsmStateType, AsPetFsm> {
	
	AsTimer m_DisplayActionTimer = null;
	
	public AsPetState_Display(AsPetFsm _fsm) : base(AsPetFsm.ePetFsmStateType.DISPLAY, _fsm)
	{		
//		m_dicMessageTreat.Add(eMessageType.MOVE_NPC_INDICATION, OnBeginMove);
//		m_dicMessageTreat.Add(eMessageType.NPC_ATTACK_CHAR1, OnBeginAttack);
//		m_dicMessageTreat.Add(eMessageType.ANIMATION_END, OnAnimationEnd);
//		
//		m_dicMessageTreat.Add( eMessageType.PET_FEEDING, OnFeeding);
//		m_dicMessageTreat.Add( eMessageType.PET_SCRIPT, OnHatch);
//		m_dicMessageTreat.Add( eMessageType.PET_HUNGRY_INDICATE, OnHungryIndicate);
	}
	
	#region - fsm function -
	public override void Enter(AsIMessage _msg)
	{
		EnterDisplay();
	}
	public override void Update()
	{
	}
	public override void Exit()
	{
		m_OwnerFsm.ReleaseElements();
	}
	#endregion
	#region - update -
	
	#endregion
	#region - process -
	void EnterDisplay()
	{
		m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("Idle", 0.1f, 1f));
			
//		SetDisplayAction();
		
//		m_OwnerFsm.SetAction(m_Action);
//		m_OwnerFsm.PlayElements(eActionStep.Entire, animSpeed);
	}
	#endregion
//	#region - msg -
//	void OnBeginMove(AsIMessage _msg)
//	{
//		m_OwnerFsm.SetPetFsmState(AsPetFsm.ePetFsmStateType.RUN, _msg);
//	}
//	void OnBeginAttack(AsIMessage _msg)
//	{
//		m_OwnerFsm.SetPetFsmState(AsPetFsm.ePetFsmStateType.SKILL_READY, _msg);
//	}
//	void OnAnimationEnd(AsIMessage _msg)
//	{
//		Msg_AnimationEnd end = _msg as Msg_AnimationEnd;
//		if(end.animName_ != "Display" && end.animName_ != "Display_Hungry")
//		{
//			float animSpeed = 1f;
//			if( m_OwnerFsm.Hungry < m_HungryGrade)
//				m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("Display_Hungry", 0.1f, animSpeed));
//			else
//				m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("Display", 0.1f, animSpeed));
//			
//			SetDisplayAction();
//		}
//	}
//	
//	void OnFeeding( AsIMessage _msg)
//	{
//		m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("Eat", 0.1f, 1f));
//	}
//	
//	void OnHatch( AsIMessage _msg)
//	{
//		m_OwnerFsm.SetPetFsmState(AsPetFsm.ePetFsmStateType.HATCH, _msg);
//	}
//	
//	void OnHungryIndicate(AsIMessage _msg)
//	{
//		Msg_PetHungryIndicate hungry = _msg as Msg_PetHungryIndicate;
//		
//		float animSpeed = 1f;
//		if( hungry.hungry_ < m_HungryGrade)
//			m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("Display_Hungry", 0.1f, animSpeed));
//		else
//			m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("Display", 0.1f, animSpeed));
//	}
//	#endregion
	#region - idle action -
//	void Timer_DisplayAction(System.Object _obj)
//	{
//		if(m_OwnerFsm.CurrnetFsmStateType == AsPetFsm.ePetFsmStateType.IDLE &&
//			m_OwnerFsm.Entity.GetProperty<bool>(eComponentProperty.COMBAT) == false)
//		{
//			float animSpeed = GetAnimationSpeedByWalk_NpcEntity("DisplayAction");
//			m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("DisplayAction", 0.1f, animSpeed));
//		}
//	}
//	
//	void SetDisplayAction()
//	{
////		ReleaseDisplayAction();
////		
////		Tbl_Pet_Record monster = AsTableManager.Instance.GetTbl_Pet_Record(m_OwnerFsm.PetEntity.TableIdx);
////		if(monster.Grade == ePet_Grade.DObject)
////			return;
////		
////		float probability = AsTableManager.Instance.GetTbl_GlobalWeight_Record("PetActionDisplay").Value;
////		float seed = Random.Range(0f, 1000f);
////		if(seed < probability)
////		{
////			float startTime = monster.StopTime_Min * 0.5f * 0.001f;
////			startTime = startTime * Random.Range(0f, 1f);
////			
////			m_DisplayActionTimer = AsTimerManager.Instance.SetTimer(startTime, Timer_DisplayAction);
////		}
//	}
//	
//	void ReleaseDisplayAction()
//	{
//		if(m_DisplayActionTimer != null)
//		{
//			AsTimerManager.Instance.ReleaseTimer(m_DisplayActionTimer);
//			m_DisplayActionTimer = null;
//		}
//	}
	#endregion
}
