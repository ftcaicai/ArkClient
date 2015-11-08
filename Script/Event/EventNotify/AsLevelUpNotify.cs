using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class AsLevelUpNotify : MonoBehaviour
{
	List<string> m_list = new List<string>();

	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
	}

	public void LevelUpProcess()
	{
		if( m_list.Count > 0)
		{
			GameObject obj = ResourceLoad.LoadGameObject( "UI/AsGUI/GUI_LevelUpNotify");
			GameObject levelUpNotifyObject = GameObject.Instantiate( obj)  as GameObject;
			AsLevelUpNotifyItem levelUpNotifyItem = levelUpNotifyObject.GetComponentInChildren<AsLevelUpNotifyItem>();
			levelUpNotifyItem.SetText( m_list, levelUpNotifyObject);

			m_list.Clear();
		}
	}

	public void SetLearnSkill( string str)
	{
		StringBuilder sb = new StringBuilder();
		sb.Insert( 0, "skill:");
		sb.AppendFormat( "{0}", str);
		m_list.Add( string.Format( AsTableManager.Instance.GetTbl_String(851),sb.ToString()));
	}

	public void SetWayPoint( string str)
	{
		StringBuilder sb = new StringBuilder();
		sb.Insert( 0, "waypoint:");
		sb.AppendFormat( "{0}", str);
		m_list.Add( string.Format( AsTableManager.Instance.GetTbl_String(852), sb.ToString()));
	}
}
