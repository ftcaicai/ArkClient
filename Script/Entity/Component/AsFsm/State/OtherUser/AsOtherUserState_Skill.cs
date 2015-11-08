//using UnityEngine;
//using System.Collections;
//
//public class AsOtherUserState_Skill : AsBaseFsmState<AsOtherUserFsm.eOtherUserFsmStateType, AsOtherUserFsm> {
//	public AsOtherUserState_Skill(AsOtherUserFsm _fsm) : base(AsOtherUserFsm.eOtherUserFsmStateType.SKILL, _fsm)
//	{
//		m_dicMessageTreat.Add(eMessageType.MOVE_OTHER_USER_INDICATION, OnBeginMove);
//		m_dicMessageTreat.Add(eMessageType.CHAR_ATTACK_NPC1, OnBeginSkill);
//	}
//	
//	#region - fsm function -
//	public override void Enter(AsIMessage _msg)
//	{
//		m_OwnerFsm.Entity.HandleMessage(new Msg_MoveStopIndication());
////		m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate(eCommonAnimType.BATTLE_IDLE_01));
//		m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("BattleIdle"));
//		
//		AttackProcess(_msg);
//		//m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate(eCommonAnimType.BASIC_ATTACK_01));
//	}
//	public override void Update()
//	{
//		if(CheckTargetLiving() == true)
//			RefreshDirection();
//	}
//	public override void Exit()
//	{
//	}
//	#endregion
//	
//	#region - update -
//	bool CheckTargetLiving()
//	{
//		if(m_OwnerFsm.Target == null)
//			return ReleaseCombat();
//		
//		if(m_OwnerFsm.Target.ContainProperty(eComponentProperty.LIVING) == true)
//		{
//			if(m_OwnerFsm.Target.GetProperty<bool>(eComponentProperty.LIVING) == false)
//			{
//				return ReleaseCombat();
//			}
//		}
//		
//		return true;
//	}
//	
//	void RefreshDirection()
//	{
//		Vector3 dir = m_OwnerFsm.Target.transform.position - m_OwnerFsm.Entity.transform.position;
//		
//		if( Vector3.zero == dir)
//			return;
//		
//		m_OwnerFsm.Entity.transform.rotation = 
//			Quaternion.Slerp(m_OwnerFsm.Entity.transform.rotation, 
//				Quaternion.LookRotation(dir), 
//				7.0f * Time.deltaTime);
//	}
//	#endregion
//	
//	#region - msg -
//	void OnBeginMove(AsIMessage _msg)
//	{
////		Msg_NpcMoveIndicate move = _msg as Msg_NpcMoveIndicate;
//		m_OwnerFsm.SetOtherUserFsmState(AsOtherUserFsm.eOtherUserFsmStateType.RUN, _msg);
//	}
//	
//	void OnBeginSkill(AsIMessage _msg)
//	{
////		m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate(eCommonAnimType.BASIC_ATTACK_01));	
//		m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("SkillFireBall"));
//		
//		Msg_OtherCharAttackNpc1 attack1 = _msg as Msg_OtherCharAttackNpc1;
//		m_OwnerFsm.Target = AsEntityManager.Instance.GetNpcEntityBySessionId(attack1.npcIdx_);
//		
//		foreach(Msg_OtherCharAttackNpc2 attack2 in attack1.body_)
//		{
//			AsEntityManager.Instance.DispatchMessageByNpcSessionId(attack2.npcIdx_, attack2);
//		}
//	}
//	#endregion
//	
//	#region - method -
//	bool ReleaseCombat()
//	{
//		m_OwnerFsm.Target = null;
//		m_OwnerFsm.SetOtherUserFsmState(AsOtherUserFsm.eOtherUserFsmStateType.IDLE);
//		return false;
//	}
//	#endregion
//}
