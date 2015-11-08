using UnityEngine;
using System.Collections;

public class AsAnimationEventListener : MonoBehaviour {
	
	AsBaseEntity m_Entity;

	// Use this for initialization
	void Start () {
		m_Entity = transform.parent.GetComponent<AsBaseEntity>();
		if(m_Entity == null)
			Debug.LogError("AsAnimationEventListener::Start(): no parent entity");
	}
	
	void EventCallBack(int _index)
	{
		m_Entity.HandleMessage(new Msg_EffectIndication(_index));
	}
}
