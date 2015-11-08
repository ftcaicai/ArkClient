using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum eComponentType {NONE, ANIMATION, MODEL, MOVER, PROPERTY_PROCESSOR,
    FSM_PLAYER, FSM_NPC, FSM_MONSTER, FSM_OBJECT, FSM_OTHER_USER, FSM_COLLECTION,
	PROPERTY_DELAYER_COMBAT
}

public class MessageRegistry{
	#region - member -
	public delegate void MessageFunction(AsIMessage _msg);
	Dictionary<eMessageType, MessageFunction> m_dicFunction = new Dictionary<eMessageType, MessageFunction>();
	public Dictionary<eMessageType, MessageFunction> dicFunction{get{return m_dicFunction;}}
	#endregion
	#region - function -
	public void RegisterFunction(eMessageType _type, MessageFunction _func)
	{
		if(m_dicFunction.ContainsKey(_type) == true)
			m_dicFunction.Remove(_type);
		
		m_dicFunction.Add(_type, _func);
	}
	
	public void ReleaseFunction(eMessageType _type)
	{
		if(m_dicFunction.ContainsKey(_type) == true)
			m_dicFunction.Remove(_type);
	}
	
	public void HandleMessage(AsIMessage _msg)
	{
		if(m_dicFunction.ContainsKey(_msg.MessageType) == true)
			m_dicFunction[_msg.MessageType](_msg);
	}
	#endregion
}

public abstract class AsBaseComponent : MonoBehaviour {

	#region - member -
	protected eComponentType m_ComponentType;
	public eComponentType ComponentType{get{return m_ComponentType;}}
	
	protected AsBaseEntity m_Entity;public AsBaseEntity Entity{get{return m_Entity;}}
	
	MessageRegistry m_MessageRegistry = new MessageRegistry();public MessageRegistry MsgRegistry{get{return m_MessageRegistry;}}
	#endregion	
	#region - init -
	public virtual void Init(AsBaseEntity _entity)
	{
		m_Entity = _entity;
	}
	
	public virtual void InterInit(AsBaseEntity _entity)
	{
	}
	
	public virtual void LateInit(AsBaseEntity _entity)
	{
	}
	
	public virtual void LastInit(AsBaseEntity _entity)
	{
	}
	#endregion
}
