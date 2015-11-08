using UnityEngine;
using System.Collections;

public class WayPointWorldLogic : WorldLogic 
{	
	public WayPointWorldLogic(WorldMapDlg _dlg) : base(_dlg)
	{		
	}
	
	public override void Open(/* int iAreaMapIdx */)
	{
		//m_iAreaMapIdx = iAreaMapIdx;		
		m_WorldMapDlg.CreateWorldMap(/* iAreaMapIdx */);		
		
		SetNoPartyImg();		
	}
	
	
	protected override void ClickMessage( int _iIndex )
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		m_WorldMapDlg.CloseDlg(WorldMapDlg.eDLG_STATE.WAYPOINT_WORLD_DLG);
		m_WorldMapDlg.OpenDlg( WorldMapDlg.eDLG_STATE.WAYPOINT_AREA_DLG, _iIndex );		
	}
}
