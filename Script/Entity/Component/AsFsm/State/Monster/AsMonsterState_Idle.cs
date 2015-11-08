using UnityEngine;
using System.Collections;

public class AsMonsterState_Idle : AsBaseFsmState<AsMonsterFsm.eMonsterFsmStateType, AsMonsterFsm> {
	
	AsTimer m_IdleActionTimer = null;
	
	bool m_ViewHold = false;
	
	public AsMonsterState_Idle(AsMonsterFsm _fsm) : base(AsMonsterFsm.eMonsterFsmStateType.IDLE, _fsm)
	{		
		m_dicMessageTreat.Add(eMessageType.MOVE_NPC_INDICATION, OnBeginMove);
		m_dicMessageTreat.Add(eMessageType.NPC_ATTACK_CHAR1, OnBeginAttack);
		m_dicMessageTreat.Add(eMessageType.ANIMATION_END, OnAnimationEnd);
	}
	
	#region - fsm function -
	public override void Enter(AsIMessage _msg)
	{
		m_ViewHold = m_OwnerFsm.Entity.GetProperty<bool>(eComponentProperty.VIEW_HOLD);
		
		EnterIdle();
		
		m_OwnerFsm.SetAction(m_Action);
		m_OwnerFsm.PlayElements(eActionStep.Entire, m_OwnerFsm.Entity.GetProperty<float>(eComponentProperty.ATTACK_SPEED));
	}
	public override void Update()
	{
		if(CheckTargetLiving() == true)
			RefreshDirection();
	}
	public override void Exit()
	{
		m_OwnerFsm.ReleaseElements();
	}
	#endregion
	#region - process -
	#region - enter idle -
	void EnterIdle()
	{
		float animSpeed = 1f;
		
		bool combat = m_OwnerFsm.MonsterEntity.GetProperty<bool>(eComponentProperty.COMBAT);
		if(combat == true)
		{
			animSpeed = GetAnimationSpeedByAttack_NpcEntity("BattleIdle", eActionStep.Ready);
			m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("BattleIdle", 0.1f, animSpeed));
			
			m_Action = GetAction_NpcEntity("BattleIdle");
		}
		else
		{
			animSpeed = GetAnimationSpeedByAttack_NpcEntity("Idle", eActionStep.Ready);
			m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("Idle", 0.1f, animSpeed));
			
			m_Action = GetAction_NpcEntity("Idle");
			
			SetIdleAction();
		}
		
		m_OwnerFsm.SetAction(m_Action);
		m_OwnerFsm.PlayElements(eActionStep.Entire, animSpeed);
	}
	#endregion
	#region - msg -
	void OnBeginMove(AsIMessage _msg)
	{
		m_OwnerFsm.SetMonsterFsmState(AsMonsterFsm.eMonsterFsmStateType.RUN, _msg);
	}
	void OnBeginAttack(AsIMessage _msg)
	{
		m_OwnerFsm.SetMonsterFsmState(AsMonsterFsm.eMonsterFsmStateType.SKILL_READY, _msg);
	}
	void OnAnimationEnd(AsIMessage _msg)
	{
		Msg_AnimationEnd end = _msg as Msg_AnimationEnd;
		if(end.animName_ != "Idle")
		{
//			#region - anim speed -
//			float moveSpeed = m_OwnerFsm.Entity.GetProperty<float>(eComponentProperty.MOVE_SPEED);
//			Tbl_Monster_Record monster = AsTableManager.Instance.GetTbl_Monster_Record(m_OwnerFsm.MonsterEntity.TableIdx);
//			float animSpeed = moveSpeed / monster.WalkSpeed * 100f;
//			if( 0 == animSpeed)
//				animSpeed = 1.0f;
//			#endregion
			
			float animSpeed = GetAnimationSpeedByWalk_NpcEntity("Idle");
			m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("Idle", 0.1f, animSpeed));
			
//			SetIdleAction();
		}
	}
	#endregion
	#region - monster is clicked -
	void ClickedProcess(AsIMessage _msg)
	{
//		Msg_Input input= _msg as Msg_Input;
//		
//		switch(input.type_)
//		{
//		case eInputType.SINGLE_UP:
//			
//			
//			
////			if(_msg.worldPosition_ != Vector3.zero)
////			{
////				if(_msg.inputObject_.layer == LayerMask.NameToLayer("Terrain"))
////				{
////					m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.RUN, _msg);
////				}
////			}
//			break;
//		}
	}
	#endregion
	#region - check target living -
	bool CheckTargetLiving()
	{
		if(m_OwnerFsm.Target == null)
			return false;
		
		int monsterId = m_OwnerFsm.Entity.GetProperty<int>(eComponentProperty.MONSTER_ID);
		Tbl_Monster_Record monsterRecord = AsTableManager.Instance.GetTbl_Monster_Record(monsterId);
		
		if(monsterRecord != null &&
			(monsterRecord.Grade == eMonster_Grade.DObject || monsterRecord.Grade == eMonster_Grade.QObject))
		{
//			Debug.Log("AsMonsterState_Idle:: monster grade = " + monsterRecord.Grade + ". direction will not change.");
			return false;
		}
		
		if(m_OwnerFsm.Target.ContainProperty(eComponentProperty.LIVING) == true)
		{
			if(m_OwnerFsm.Target.GetProperty<bool>(eComponentProperty.LIVING) == false)
			{
				m_OwnerFsm.Target = null;
				return false;
			}
		}
		
		return true;
	}
	#endregion
	#region - refresh direction -
	void RefreshDirection()
	{
		if( m_ViewHold == true)
			return;
		
		Vector3 dir = m_OwnerFsm.Target.transform.position - m_OwnerFsm.Entity.transform.position;
		dir.y = 0;
		
		if( Vector3.zero == dir)
			return;
		
		m_OwnerFsm.Entity.transform.rotation = 
			Quaternion.Slerp(m_OwnerFsm.Entity.transform.rotation, 
				Quaternion.LookRotation(dir), 
				10.0f * Time.deltaTime);
	}
	#endregion
	
	#endregion
	#region - idle action -
	void Timer_IdleAction(System.Object _obj)
	{
		if(m_OwnerFsm.CurrnetFsmStateType == AsMonsterFsm.eMonsterFsmStateType.IDLE &&
			m_OwnerFsm.Entity.GetProperty<bool>(eComponentProperty.COMBAT) == false)
		{
//			#region - anim speed -
//			float moveSpeed = m_OwnerFsm.Entity.GetProperty<float>(eComponentProperty.MOVE_SPEED);
//			Tbl_Monster_Record monster = AsTableManager.Instance.GetTbl_Monster_Record(m_OwnerFsm.MonsterEntity.TableIdx);
//			float animSpeed = moveSpeed / monster.WalkSpeed * 100f;
//			if( 0 == animSpeed)
//				animSpeed = 1.0f;
//			#endregion
			
			float animSpeed = GetAnimationSpeedByWalk_NpcEntity("IdleAction");
			m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("IdleAction", 0.1f, animSpeed));
		}
	}
	
	void SetIdleAction()
	{
		ReleaseIdleAction();
		
		Tbl_Monster_Record monster = AsTableManager.Instance.GetTbl_Monster_Record(m_OwnerFsm.MonsterEntity.TableIdx);
		if(monster.Grade == eMonster_Grade.DObject)
			return;
		
		float probability = AsTableManager.Instance.GetTbl_GlobalWeight_Record("MonsterActionIdle").Value;
		float seed = Random.Range(0f, 1000f);
		if(seed < probability)
		{
			float startTime = monster.StopTime_Min * 0.5f * 0.001f;
			startTime = startTime * Random.Range(0f, 1f);
			
			m_IdleActionTimer = AsTimerManager.Instance.SetTimer(startTime, Timer_IdleAction);
		}
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
