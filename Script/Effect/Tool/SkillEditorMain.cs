using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SkillEditorMain : MonoBehaviour {

	GameObject SkillEditorObject;
	public int CurrentMapID = 1;
	
	
	void Awake()
	{	
		Debug.Log("SkillEditorMgr Awake()");    
	//	m_instance = this;	
		SkillEditorObject = new GameObject("SkillEditor");
		if( null == AsEffectManager.Instance )
		{
			SkillEditorObject.AddComponent<AsEffectManager>();
		}
	
		
		if( null == AsSoundManager.Instance )
		{
			SkillEditorObject.AddComponent<AsSoundManager>();
		}
	
				
		if( null == ItemMgr.Instance )
		{
			SkillEditorObject.AddComponent<ItemMgr>();
		}
		
		if( null == AsEntityManager.Instance )
		{
			//SkillEditorObject.AddComponent<AsEntityManager>();
			SkillEditorObject.AddComponent<AsTableManager>();	
		
			
			//SkillEditorObject.AddComponent<AsInputManager>();//$yde//	
		}		
		
		
		if( null == TerrainMgr.Instance )
		{
			SkillEditorObject.AddComponent<TerrainMgr>();
		}
		
		/*if( null == CameraMgr.Instance )
		{			
			SkillEditorObject.AddComponent<CameraMgr>();
			
		}*/
	}
	
	
	
	// Use this for initialization
	void Start () 
	{	
		
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}