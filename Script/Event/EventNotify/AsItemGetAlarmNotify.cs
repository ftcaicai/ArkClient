using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsItemGetAlarmNotify : MonoBehaviour
{
	float m_GAP_TIME = 1.0f;

	#region - m_NotifyItem -
	private List<sITEM> m_listNotifyItem = new List<sITEM>();
	private float m_fTime = 0.0f;
	#endregion

	// Use this for initialization
	void Start()
	{
		m_GAP_TIME  =  AsTableManager.Instance.GetTbl_GlobalWeight_Record(113).Value;// * 0.001f;
	}

	// Update is called once per frame
	void Update()
	{
	}

	public void AddListItem( sITEM _sitem)
	{
		if( null == _sitem)
			return;

		m_listNotifyItem.Add(_sitem);
	}

	public void UpdateData()
	{
		if( false == AsHudDlgMgr.Instance.isOpenItemGetEquipAlarmDlg)
		{
			if( m_listNotifyItem.Count > 0)
			{
				AsHudDlgMgr.Instance.OpenItemGetEquipAlarmDlg( m_listNotifyItem[0]);
				m_listNotifyItem.RemoveAt( 0);
				m_fTime = 0f;
			}
			return;
		}

		m_fTime += Time.deltaTime;

		if( m_GAP_TIME > m_fTime)
			return;

		if( m_listNotifyItem.Count <= 0)
		{
			AsHudDlgMgr.Instance.DeleteItemGetEquipAlramDlg();
			return;
		}

		AsHudDlgMgr.Instance.OpenItemGetEquipAlarmDlg( m_listNotifyItem[0]);
		m_listNotifyItem.RemoveAt( 0);
		m_fTime = 0f;
	}

	public static bool CheckEquipEnable( sITEM _sitem)
	{
		if( null == _sitem)
			return false;

		return true;
	}
}
