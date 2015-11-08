using UnityEngine;
using System.Collections;

public enum eCombatStateType {IDLE, COMBAT}

public abstract class AsCombatStateBase {
	
	protected eCombatStateType m_FsmStateType;public eCombatStateType FsmStateType{get{return m_FsmStateType;}}
	protected AsPropertyDelayer_Combat m_OwnerFsm;//public T_Fsm OwnerFsm{get{return m_OwnerFsm;}}
	
	bool m_TimerOn = false;
	float m_BeginTime = 0;
	float m_Elapsed = 0;
	float m_Timer = 0;
	
	public AsCombatStateBase(AsPropertyDelayer_Combat _owner, eCombatStateType _type)
	{
		m_OwnerFsm = _owner;
		m_FsmStateType = _type;
	}
	
	#region - fsm function -
	public abstract void Enter(AsIMessage _msg);
	public abstract void Update();
//	public virtual void FixedUpdate(){}
	public abstract void Exit();
	public abstract void MessageProcess(AsIMessage _msg);
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
}
