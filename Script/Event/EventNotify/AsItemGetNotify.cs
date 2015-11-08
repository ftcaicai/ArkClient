using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class AsItemGetNotify : MonoBehaviour
{
	float m_GAP_TIME = 1.0f;

	#region - m_NotifyItem -
	private List<AsItemGetNotifyItem> m_listNotifyItem = new List<AsItemGetNotifyItem>();
	private List<AsItemGetNotifyItem> m_listRemoveNotifyItem = new List<AsItemGetNotifyItem>();
	private float m_fStartTime = 0.0f;
	private float m_fStopTime = 0.5f;
	#endregion

	// Use this for initialization
	void Start()
	{
		m_fStopTime = AsTableManager.Instance.GetTbl_GlobalWeight_Record(47).Value / 1000.0f;
	}

	// Update is called once per frame
	void Update()
	{
	}

	public void UpdateData()
	{
		foreach( AsItemGetNotifyItem stData in m_listNotifyItem)
		{
			if( stData == null)
				continue;

			if( ( Time.time - m_fStartTime) > m_GAP_TIME + m_fStopTime)
			{
				if( !stData.ShowCommand)
				{
					m_fStartTime = Time.time;
					stData.Show();
					m_listRemoveNotifyItem.Add( stData);
					break;
				}
			}
		}

		if( 0 < m_listRemoveNotifyItem.Count)
		{
			foreach( AsItemGetNotifyItem stRemoveData in m_listRemoveNotifyItem)
				m_listNotifyItem.Remove( stRemoveData);

			m_listRemoveNotifyItem.Clear();
		}
	}

	public void DisplayItemPanel( int itemId)
	{
		GameObject obj = ResourceLoad.LoadGameObject( "UI/AsGUI/GUI_ItemGetAlram");
		GameObject notifyItemObject = GameObject.Instantiate( obj) as GameObject;
		AsItemGetNotifyItem notifyItem = notifyItemObject.GetComponentInChildren<AsItemGetNotifyItem>();
		notifyItem.SetData( itemId, notifyItemObject);
		m_listNotifyItem.Add( notifyItem);

		m_GAP_TIME = ( notifyItem.m_BaseDlg.height + 1.0f) / ( AsTableManager.Instance.GetTbl_GlobalWeight_Record(46).Value / 1000.0f);
	}
}
