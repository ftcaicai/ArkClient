using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsPropertyDelayer_Combat : AsBaseComponent {
	
	#region - global state 1 -
	public eCombatStateType combat_;
	public float time_;
	
	protected Dictionary<eCombatStateType, AsCombatStateBase> m_dicCombatState =
		new Dictionary<eCombatStateType, AsCombatStateBase>();
	protected AsCombatStateBase m_CombatState;
	#endregion
	
	#region - init -
	void Awake()
	{
		m_ComponentType = eComponentType.PROPERTY_DELAYER_COMBAT;
		
		MsgRegistry.RegisterFunction(eMessageType.COMBAT_BEGIN, OnCombatBegin);
		
		m_dicCombatState.Add(eCombatStateType.IDLE, new AsCombat_Idle(this));
		m_dicCombatState.Add(eCombatStateType.COMBAT, new AsCombat_Combat(this));
	}
	
	public override void Init(AsBaseEntity _entity)
	{
		base.Init(_entity);
		
//		m_Entity = GetComponent<AsBaseEntity>();
//		if(m_Entity == null)
//		{
//			Debug.LogError("AsPropertyDelayer_Combat::Init: no entity attached");
//			return;
//		}
		
		m_CombatState = m_dicCombatState[eCombatStateType.IDLE];
	}
	
	void Start()
	{
		
	}
	#endregion
	#region - fsm -
	public void SetCombatState(eCombatStateType _type, AsIMessage _msg)
	{
//		if(m_CombatState.FsmStateType != _type)
//		{
			combat_ = _type;
			m_CombatState.Exit();
			m_CombatState = m_dicCombatState[_type];
			m_CombatState.Enter(_msg);
//		}
	}
	public void SetCombatState(eCombatStateType _type)
	{
		SetCombatState(_type, null);
	}
	#endregion
	#region - update & message -
	void Update()
	{
		m_CombatState.Update();
	}
	#endregion
	#region - process -
	void OnCombatBegin(AsIMessage _msg)
	{
		SetCombatState(eCombatStateType.COMBAT);
	}
	
	void OnCombatEnd(AsIMessage _msg)
	{
//		SetCombatState(eCombatStateType.IDLE);
	}
	#endregion - process -
}
