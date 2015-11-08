using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum eAirboneState { Ready, Up, Float, Down}

public delegate void MessageProcess(AsIMessage _msg);

public abstract class AsBaseFsmState<T_FsmType, T_Fsm> where T_Fsm : AsBaseComponent {
	
	#region - static number -
	protected readonly float airboneTime_Up = 0.5f;
	protected readonly float airboneTime_Down = 0.5f;
	protected readonly float airboneHeight = 2f;
	protected float m_AirboneVelocity = 0.1f;
	protected float m_AirboneAccel = 0.01f;
	#endregion
	#region - base -
	protected T_FsmType m_FsmStateType;public T_FsmType FsmStateType{get{return m_FsmStateType;}}
	protected T_Fsm m_OwnerFsm;//public T_Fsm OwnerFsm{get{return m_OwnerFsm;}}
	
	protected Dictionary<eMessageType, MessageProcess> m_dicMessageTreat = 
		new Dictionary<eMessageType, MessageProcess>();
	
	protected Tbl_Action_Record m_Action;public Tbl_Action_Record Action{get { return m_Action; }}
	#endregion
	#region - timer -
	bool m_TimerOn = false; public bool TimerOn{get{return m_TimerOn;}}
	float m_BeginTime = 0;
	float m_Elapsed = 0;
	float m_Timer = 0;
	#endregion
	
	public AsBaseFsmState(T_FsmType _type, T_Fsm _fsm)
	{
		m_FsmStateType = _type;
		m_OwnerFsm = _fsm;
		
		#region - exceptional case -
		m_MonsterFsm = m_OwnerFsm as AsMonsterFsm;
		#endregion
	}
	
	public virtual void Init(){}
	
	#region - treatable message -
	public bool MessageTreatable(eMessageType _msgType)
	{
		return m_dicMessageTreat.ContainsKey(_msgType);
	}
	#endregion
	#region - fsm function -
	public abstract void Enter(AsIMessage _msg);
	public abstract void Update();
	public virtual void FixedUpdate(){}
	public abstract void Exit();
//	public abstract void MessageProcess(AsIMessage _msg);
	public void MessageProcess(AsIMessage _msg)
	{
		if(m_dicMessageTreat.ContainsKey(_msg.MessageType) == true)
		{
			m_dicMessageTreat[_msg.MessageType](_msg);
		}
	}
	#endregion
	
	#region - timer -
	protected void SetTimer(float _time)
	{
		m_TimerOn = true;
		
		m_Timer = _time;
		m_BeginTime = Time.time;
	}
	
	protected bool TimeElapsed()
	{
		if(m_TimerOn == true)
		{
			m_Elapsed = Time.time - m_BeginTime;
			
			if(m_Elapsed > m_Timer)
				return true;
		}
		
		return false;
	}
	
	protected void ReleaseTimer()
	{
		m_TimerOn = false;
	}
	#endregion
	#region - gizmo -
	public virtual void OnDrawGizmos(){}
	#endregion
	
	#region - get user action -
	//USER ENTITY
	protected Tbl_Class_Record GetClass_UserEntity()
	{
		eRACE race = m_OwnerFsm.Entity.GetProperty<eRACE>(eComponentProperty.RACE);
		eCLASS __class = m_OwnerFsm.Entity.GetProperty<eCLASS>(eComponentProperty.CLASS);
		return AsTableManager.Instance.GetTbl_Class_Record(race, __class);
	}
	protected Tbl_Action_Record GetAction_UserEntity(string _action)
	{
		eCLASS __class = m_OwnerFsm.Entity.GetProperty<eCLASS>(eComponentProperty.CLASS);
		eGENDER gender = m_OwnerFsm.Entity.GetProperty<eGENDER>(eComponentProperty.GENDER);
		return AsTableManager.Instance.GetTbl_Action_Record(__class, gender, _action);
	}	
	#endregion
	#region - user action speed -
	protected float GetAnimationSpeedByMove_UserEntity(string _action)
	{
		Tbl_Class_Record classRecord = GetClass_UserEntity();
		Tbl_Action_Record action = GetAction_UserEntity(_action);
		float actionSpeed = 1000f;
		if( action != null && action.ReadyAnimation != null)
			actionSpeed = action.ReadyAnimation.ActionSpeed;
		
		float moveSpeed = m_OwnerFsm.Entity.GetProperty<float>(eComponentProperty.MOVE_SPEED);	
		float animSpeed = moveSpeed * actionSpeed * 0.001f / classRecord.MoveSpeed * 100;
		if( 0 == animSpeed)
			animSpeed = 1.0f;
		
		return animSpeed;
	}
	
	protected float GetAnimationSpeedByAttack_UserEntity(string _action, eActionStep _step)
	{
		Tbl_Action_Record action = GetAction_UserEntity(_action);
		float actionSpeed = 1000f;
		if( action != null)
		{
			switch( _step)
			{
			case eActionStep.Ready:
				if( action.ReadyAnimation != null)
					actionSpeed = action.ReadyAnimation.ActionSpeed;
				break;
			case eActionStep.Hit:
			case eActionStep.Entire:
				if( action.HitAnimation != null)
					actionSpeed = action.HitAnimation.ActionSpeed;
				break;
			case eActionStep.Finish:
				if( action.FinishAnimation != null)
					actionSpeed = action.FinishAnimation.ActionSpeed;
				break;
			}
		}
		
		float attackSpeed = m_OwnerFsm.Entity.GetProperty<float>(eComponentProperty.ATTACK_SPEED);	
		float animSpeed = attackSpeed * actionSpeed * 0.001f;
		if( 0 == animSpeed)
			animSpeed = 1.0f;
		
		return animSpeed;
	}	
	
	protected float GetAnimationSpeedByAttack_UserEntity(Tbl_Action_Record _action, eActionStep _step)
	{
		float actionSpeed = 1000f;
		if( _action != null)
		{
			switch( _step)
			{
			case eActionStep.Ready:
				if( _action.ReadyAnimation != null)
					actionSpeed = _action.ReadyAnimation.ActionSpeed;
				break;
			case eActionStep.Hit:
			case eActionStep.Entire:
				if( _action.HitAnimation != null)
					actionSpeed = _action.HitAnimation.ActionSpeed;
				break;
			case eActionStep.Finish:
				if( _action.FinishAnimation != null)
					actionSpeed = _action.FinishAnimation.ActionSpeed;
				break;
			}
		}
		
		float attackSpeed = m_OwnerFsm.Entity.GetProperty<float>(eComponentProperty.ATTACK_SPEED);	
		float animSpeed = attackSpeed * actionSpeed * 0.001f;
		if( 0 == animSpeed)
			animSpeed = 1.0f;
		
		return animSpeed;
	}
	#endregion
	#region - get npc action -
	//NPC ENTITY
	AsMonsterFsm m_MonsterFsm = null;
	protected Tbl_Monster_Record GetMonsterRecord_NpcEntity()
	{
		if(m_MonsterFsm != null)
			return AsTableManager.Instance.GetTbl_Monster_Record(m_MonsterFsm.MonsterEntity.TableIdx);
		else
		{
			Debug.Log("AsBaseFsmState::Tbl_Monster_Record: m_MonsterFsm == null");
			return null;
		}
	}
	
	protected string GetClass_NpcEntity()
	{
		return m_OwnerFsm.Entity.GetProperty<string>(eComponentProperty.CLASS);
	}
	
	protected Tbl_Action_Record GetAction_NpcEntity(string _action)
	{
		string __class = GetClass_NpcEntity();
		
		return AsTableManager.Instance.GetTbl_MonsterAction_Record(__class, _action);
	}

	protected Tbl_Action_Record GetAction_Pet(string _action)
	{
		string __class = GetClass_NpcEntity();
		
		return AsTableManager.Instance.GetPetActionRecord(__class, _action);
	}
	#endregion
	#region - npc action speed -
	protected float GetAnimationSpeedByWalk_NpcEntity(string _action)
	{
		Tbl_Monster_Record monster = GetMonsterRecord_NpcEntity(); if(monster == null) return 1f;
		Tbl_Action_Record action = GetAction_NpcEntity(_action);
		float actionSpeed = 1000f;
		if( action != null && action.ReadyAnimation != null)
			actionSpeed = action.ReadyAnimation.ActionSpeed;
		
		float moveSpeed = m_OwnerFsm.Entity.GetProperty<float>(eComponentProperty.MOVE_SPEED);	
		float animSpeed = moveSpeed * actionSpeed * 0.001f / monster.WalkSpeed * 100;
		if( 0 == animSpeed)
			animSpeed = 1.0f;
		
		return animSpeed;
	}
	
	protected float GetAnimationSpeedByRun_NpcEntity(string _action)
	{
		Tbl_Monster_Record monster = GetMonsterRecord_NpcEntity(); if(monster == null) return 1f;
		Tbl_Action_Record action = GetAction_NpcEntity(_action);
		float actionSpeed = 1000f;
		if( action != null && action.ReadyAnimation != null)
			actionSpeed = action.ReadyAnimation.ActionSpeed;
		
		float moveSpeed = m_OwnerFsm.Entity.GetProperty<float>(eComponentProperty.MOVE_SPEED);	
		float animSpeed = moveSpeed * actionSpeed * 0.001f / monster.RunSpeed * 100;
		if( 0 == animSpeed)
			animSpeed = 1.0f;
		
		return animSpeed;
	}
	
	protected float GetAnimationSpeedByAttack_NpcEntity(string _action, eActionStep _step)
	{
		Tbl_Monster_Record monster = GetMonsterRecord_NpcEntity(); if(monster == null) return 1f;
		Tbl_Action_Record action = GetAction_NpcEntity(_action);
		float actionSpeed = 1000f;
		if( action != null)
		{
			switch( _step)
			{
			case eActionStep.Ready:
				if( action.ReadyAnimation != null)
					actionSpeed = action.ReadyAnimation.ActionSpeed;
				break;
			case eActionStep.Hit:
			case eActionStep.Entire:
				if( action.HitAnimation != null)
					actionSpeed = action.HitAnimation.ActionSpeed;
				break;
			case eActionStep.Finish:
				if( action.FinishAnimation != null)
					actionSpeed = action.FinishAnimation.ActionSpeed;
				break;
			}
		}
		
		float attackSpeed = m_OwnerFsm.Entity.GetProperty<float>(eComponentProperty.ATTACK_SPEED);	
		float animSpeed = attackSpeed * actionSpeed * 0.001f;// / monster.AttackSpeed * 1000;
		if( 0 == animSpeed)
			animSpeed = 1.0f;
		
		return animSpeed;
	}
	
	protected float GetAnimationSpeedByAttack_NpcEntity(Tbl_Action_Record _action, eActionStep _step)
	{
		float actionSpeed = 1000f;
		if( _action != null)
		{
			switch( _step)
			{
			case eActionStep.Ready:
				if( _action.ReadyAnimation != null)
					actionSpeed = _action.ReadyAnimation.ActionSpeed;
				break;
			case eActionStep.Hit:
			case eActionStep.Entire:
				if( _action.HitAnimation != null)
					actionSpeed = _action.HitAnimation.ActionSpeed;
				break;
			case eActionStep.Finish:
				if( _action.FinishAnimation != null)
					actionSpeed = _action.FinishAnimation.ActionSpeed;
				break;
			}
		}
		
		Tbl_Monster_Record monster = GetMonsterRecord_NpcEntity(); if(monster == null) return 1f;
		float attackSpeed = m_OwnerFsm.Entity.GetProperty<float>(eComponentProperty.ATTACK_SPEED);	
		float animSpeed = attackSpeed * actionSpeed * 0.001f;// / monster.AttackSpeed * 1000;
		if( 0 == animSpeed)
			animSpeed = 1.0f;
		
		return animSpeed;
	}
	#endregion
}
