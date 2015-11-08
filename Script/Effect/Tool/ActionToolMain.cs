using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ActionToolMain : MonoBehaviour {
	
	GameObject ActionToolObject;
	public int CurrentMapID = 1;
	
	
	void Awake()
	{	
		Debug.Log("ActionToolMgr Awake()");    
		
		AsGameMain.SetOptionState( OptionBtnType.OptionBtnType_EffectShow, true);
	//	m_instance = this;	
		ActionToolObject = new GameObject("ActionToolEditor");
		if( null == AsEffectManager.Instance )
		{
			ActionToolObject.AddComponent<AsEffectManager>();
		}
	
		
		if( null == AsSoundManager.Instance )
		{
			ActionToolObject.AddComponent<AsSoundManager>();
		}
	
		
		if( null == ItemMgr.Instance )
		{
			ActionToolObject.AddComponent<ItemMgr>();
		}
		
		if( null == AsEntityManager.Instance )
		{
			ActionToolObject.AddComponent<AsEntityManager>();
			ActionToolObject.AddComponent<AsTableManager>();	
		
			
			ActionToolObject.AddComponent<AsInputManager>();//$yde//	
		}		
		
		if( null == TerrainMgr.Instance )
		{
			ActionToolObject.AddComponent<TerrainMgr>();
		}
		
		/*if( null == CameraMgr.Instance )
		{			
			SkillEditorObject.AddComponent<CameraMgr>();
			
		}*/
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}


