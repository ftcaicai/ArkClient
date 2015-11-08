using UnityEngine;
using System.Collections;

public class AsRankChangeAlarm : MonoBehaviour
{
	[SerializeField]AsRankChangeAlarmRanker alarm = null;
	
	// Use this for initialization
	void Start()
	{
		UIPanel[] panels = gameObject.GetComponentsInChildren<UIPanel>();
		foreach( UIPanel panel in panels)
			panel.BringIn();

		Invoke( "Dismiss", 6.0f);
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	public void Init( body_SC_RANK_CHANGE_MYRANK data)
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6035_EFF_RankingAlarm", Vector3.zero, false);
			
		//alarm.Init( data.sMyRankInfo);
		alarm.Init( data);
	}
	
	private void Dismiss()
	{
		UIPanel[] panels = gameObject.GetComponentsInChildren<UIPanel>();
		foreach( UIPanel panel in panels)
			panel.Dismiss();
		
		Invoke( "Destroy", 1.0f);
	}
	
	private void Destroy()
	{
		GameObject.Destroy( gameObject);
	}
}
