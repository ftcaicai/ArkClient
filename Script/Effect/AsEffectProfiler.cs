using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsEffectDestroyChecker
{
	static int index_ = 0;
	public string log_;
	
	~AsEffectDestroyChecker()
	{
	//	Debug.Log(index_ + "'th AsEffectDestroyChecker: " + log_);
		index_++;
	}
}

public class AsEffectProfiler : MonoBehaviour {
	
//	static int index_ = 0;
//	
//	string name_;
//	AsEffectDestroyChecker checker = new AsEffectDestroyChecker();
//	List<ParticleSystem> m_listParticleSystem = new List<ParticleSystem>();
//	
//	void Start () {
//		name_ = name;
//		checker.log_ = name;
////		m_listParticleSystem.AddRange(GetComponentsInChildren<ParticleSystem>());
//	}
//	
//	// Update is called once per frame
//	void Update () {
//	
//	}
//	
//	void OnDisable()
//	{
////		foreach(ParticleSystem system in m_listParticleSystem)
////		{
////			Debug.Log("OnDisable: effect = " + system);
////		}
//	}
//	
//	void OnDestroy()
//	{
////		foreach(ParticleSystem system in m_listParticleSystem)
////		{
////			Debug.Log("OnDestroy: effect = " + system);
////		}
//	}
//	
//	~AsEffectProfiler()
//	{
//		Debug.Log(index_ + "'th AsEffectProfiler: " + name_);
//		index_++;
//	}
}
