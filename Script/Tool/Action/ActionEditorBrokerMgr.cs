using UnityEngine;
using System.Collections;

public class ActionEditorBrokerMgr : AsSingleton<ActionEditorBrokerMgr> 
{
	eACTION_STATE m_eActionState;
	public eACTION_STATE ActionState
	{
		get{return m_eActionState;}
		set{m_eActionState = value;}
	}

	float m_ProjectileDrawGizmosTime = 0f;
	public float ProjectileDrawGizmosTime
	{
		get{return m_ProjectileDrawGizmosTime;}
		set{m_ProjectileDrawGizmosTime = value;	}
	}

	float m_AnimationTime = 0f;
	public float AnimationTime
	{
		get{return m_AnimationTime;}
		set{m_AnimationTime = value;}
	}

	bool m_bShooting_Position = false;
	public bool ChangeShooting_Position
	{
		get{return m_bShooting_Position;}
		set{m_bShooting_Position = value;}
	}

	ActionEnemyList m_EnemyList = null;
	public ActionEnemyList EnemyList
	{
		get{return m_EnemyList;}
		set{m_EnemyList = value;}
	}

}
