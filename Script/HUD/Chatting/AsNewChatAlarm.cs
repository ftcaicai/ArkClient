using UnityEngine;
using System.Collections;

public class AsNewChatAlarm : MonoBehaviour
{
//	private bool newMsgExist = false;
	public bool NewMsgExist
	{
		set
		{
			eff.newMsgExist = value;
			eff.gameObject.SetActiveRecursively( value);
		}
		get	{ return eff.newMsgExist; }
	}
	
	public AsChatAlarmController eff = null;
	
	// Use this for initialization
	void Start()
	{
	}
	
	// Update is called once per frame
	void Update()
	{
	}
}
