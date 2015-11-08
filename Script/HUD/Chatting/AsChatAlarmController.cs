using UnityEngine;
using System.Collections;

public class AsChatAlarmController : MonoBehaviour
{
	[HideInInspector]
	public bool newMsgExist = false;
	
	void OnEnable()
	{
		gameObject.SetActiveRecursively( newMsgExist);
	}
	
	// Use this for initialization
	void Start()
	{
	}
	
	// Update is called once per frame
	void Update()
	{
	}
}
