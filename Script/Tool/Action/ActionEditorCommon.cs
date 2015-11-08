using UnityEngine;
using System.Collections;

public enum eCHARACTER_CHANGE_STATE
{
	None,		
	ActionEditor_OK,
	ActionMakeFiled_OK,
};

public enum eACTION_STATE
{
	None,
	RedayAction,
	HitAction,
	FinishAction		
};

public enum eACTION_CLASS_TYPE
{
	PC,
	MONSTER,
	NPC		
};

public class ActionEditorCommon : MonoBehaviour 
{
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
