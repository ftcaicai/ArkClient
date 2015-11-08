using UnityEngine;
using System.Collections;

public class AsMonsterState_Run : AsBaseFsmState<AsMonsterFsm.eMonsterFsmStateType, AsMonsterFsm> {
	
	public AsMonsterState_Run(AsMonsterFsm _fsm) : base(AsMonsterFsm.eMonsterFsmStateType.RUN, _fsm)
	{
		m_dicMessageTreat.Add(eMessageType.MOVE_NPC_INDICATION, OnBeginMove);
		m_dicMessageTreat.Add(eMessageType.MOVE_END_INFORM, OnEndMove);
		m_dicMessageTreat.Add(eMessageType.NPC_ATTACK_CHAR1, OnBeginAttack);
	}
	
	#region - fsm function -
	public override void Enter(AsIMessage _msg)
	{
		MoveProcess(_msg);
	}
	public override void Update()
	{
	}
	public override void Exit()
	{
//		Debug.Log("AsMonsterState_Run::Exit: m_OwnerFsm.transform.forward = " + m_OwnerFsm.transform.forward);
		m_OwnerFsm.ReleaseElements();
	}
	#endregion
	
	#region - move -
	void MoveProcess(AsIMessage _msg)
	{
		Msg_NpcMoveIndicate msg = _msg as Msg_NpcMoveIndicate;
		
		m_OwnerFsm.MonsterEntity.SetProperty(eComponentProperty.MOVE_SPEED, msg.moveSpeed_);
		
//		float moveSpeed = msg.moveSpeed_;
//		int monsterId = m_OwnerFsm.Entity.GetProperty<int>(eComponentProperty.MONSTER_ID);
//		Tbl_Monster_Record monsterRecord = AsTableManager.Instance.GetTbl_Monster_Record(monsterId);
		
		m_OwnerFsm.MonsterEntity.SetProperty(eComponentProperty.COMBAT, msg.combat_);
		
		if(msg.forceMove_ == true)
		{
			Msg_ConditionIndicate_ForcedMove forcedMove = new Msg_ConditionIndicate_ForcedMove(
				msg.targetPosition_, 0, PotencyProcessor.s_ForcedMoveDuratioin);
			
			m_OwnerFsm.SetMonsterFsmState(AsMonsterFsm.eMonsterFsmStateType.CONDITION_FORCEDMOVE, forcedMove);
		}
		else
		{
//			string __class = m_OwnerFsm.MonsterEntity.GetProperty<string>(eComponentProperty.CLASS);
			
			float animSpeed = 1f;
			
			if(msg.combat_ == true)
			{
				animSpeed = GetAnimationSpeedByRun_NpcEntity("Run");
				m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("Run", 0.1f, animSpeed));
				m_Action = GetAction_NpcEntity("Run");
			}
			else
			{
				animSpeed = GetAnimationSpeedByWalk_NpcEntity("Walk");
				m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("Walk", 0.1f, animSpeed));
				m_Action = GetAction_NpcEntity("Walk");
			}
			
			m_OwnerFsm.SetAction(m_Action);
			m_OwnerFsm.PlayElements(eActionStep.Entire, animSpeed);
			
			m_OwnerFsm.Entity.HandleMessage(new Msg_MoveInfo(msg.targetPosition_));
		}
	}
	#endregion
	
	#region - msg -
	void OnBeginMove(AsIMessage _msg)
	{
		MoveProcess(_msg);
	}
	
	void OnEndMove(AsIMessage _msg)
	{
		m_OwnerFsm.SetMonsterFsmState(AsMonsterFsm.eMonsterFsmStateType.IDLE);
	}
	
	void OnBeginAttack(AsIMessage _msg)
	{
//		Msg_NpcAttackChar1 attack = _msg as Msg_NpcAttackChar1;
		m_OwnerFsm.SetMonsterFsmState(AsMonsterFsm.eMonsterFsmStateType.SKILL_READY, _msg);
	}
	#endregion
}
