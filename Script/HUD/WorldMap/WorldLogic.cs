using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class WorldLogic
{
	protected WorldMapDlg m_WorldMapDlg;	
	protected int m_iDownIndex = 0;	
	//protected int m_iAreaMapIdx = 0;
	protected Dictionary<int, int> m_PartyList = new Dictionary<int, int>(); // areaId, num
	
	public WorldLogic( WorldMapDlg _dlg )
	{
		m_WorldMapDlg = _dlg;
	}
	
	public virtual void Open(/* int iAreaMapIdx */)
	{
		//m_iAreaMapIdx = iAreaMapIdx;
		
		m_WorldMapDlg.CreateWorldMap(/* iAreaMapIdx */);
		m_WorldMapDlg.SetTitleName("");
		
		if( true == AsPartyManager.Instance.IsPartying )
		{
			AsPartySender.SendPartyUserPosition();
		}
		else
		{
			SetNoPartyImg();
		}
	}
	
	
	public void Close()
	{
		m_WorldMapDlg.DeleteWorldMap();
	}
	
	public virtual void SetParyPositionInfo( body2_SC_PARTY_USER_POSITION[] _info )
	{
		if( false == m_WorldMapDlg.isOpenWorldMap )
			return;		
		
		m_PartyList.Clear();
		m_WorldMapDlg.worldMap.ClearPartyImg();
		
		
		Tbl_ZoneMap_Record selfrecord = AsTableManager.Instance.GetZoneMapRecord( TerrainMgr.Instance.GetCurMapID() );
		if( null == selfrecord)
		{
			AsUtil.ShutDown("WorldMapDlg::OpenWaypointMap()[ null == Tbl_ZoneMap_Record ] id : " + TerrainMgr.Instance.GetCurMapID() );
			return;
		}		
		
		foreach( body2_SC_PARTY_USER_POSITION _data in _info )
		{
			Tbl_ZoneMap_Record _zoneRecord = AsTableManager.Instance.GetZoneMapRecord( _data.nMapIdx );
			if( null == _zoneRecord )
				continue;		
			
			if( m_PartyList.ContainsKey( _zoneRecord.getAreaMapIdx ) )
			{
				m_PartyList[_zoneRecord.getAreaMapIdx] += 1;
			}
			else
			{
				m_PartyList.Add( _zoneRecord.getAreaMapIdx, 1 );
			}
		}
		
		if( m_PartyList.ContainsKey( selfrecord.getAreaMapIdx ) )
		{
			m_PartyList[selfrecord.getAreaMapIdx] += 1;
		}
		else
		{
			m_PartyList.Add( selfrecord.getAreaMapIdx, 1 );
		}
		
		if( m_PartyList.Count > 0 )
		{			
			
			foreach( KeyValuePair<int, int> pair in m_PartyList )
			{
				m_WorldMapDlg.worldMap.SetPartyImg( pair.Key, pair.Value, selfrecord.getAreaMapIdx );
			}
		}
		else
		{
			SetNoPartyImg();
		}
	}
	
	
	protected void SetNoPartyImg()	
	{
		if( false == m_WorldMapDlg.isOpenWorldMap )
			return;	
		
		Tbl_ZoneMap_Record zoneRecord = AsTableManager.Instance.GetZoneMapRecord( TerrainMgr.Instance.GetCurMapID() );
		if( null == zoneRecord )
			return;
		
		m_WorldMapDlg.worldMap.SetNoPartyImg( zoneRecord.getAreaMapIdx );
	}
	
	
	public virtual void GuiInputDown( Ray inputRay )
	{
		if( false == m_WorldMapDlg.isOpenWorldMap )
			return;
		
		m_iDownIndex = m_WorldMapDlg.worldMap.GetColliderIndex( inputRay );		
	}
	
	public virtual void GuiInputUp( Ray inputRay )
	{
		if( false == m_WorldMapDlg.isOpenWorldMap || 0 == m_iDownIndex )
			return;
		
		if( m_iDownIndex == m_WorldMapDlg.worldMap.GetColliderIndex( inputRay ) )
		{
			Tbl_AreaMap_Record areaRecord = AsTableManager.Instance.GetAreaMapRecord( m_iDownIndex );
			if( null != areaRecord )
			{
				if( true == areaRecord.isActive )
					ClickMessage( m_iDownIndex );
			}			
		}
		
		m_iDownIndex = 0;
	}
	
	protected virtual void ClickMessage( int _iIndex )
	{
		m_WorldMapDlg.CloseDlg(WorldMapDlg.eDLG_STATE.WORLD_DLG);
		m_WorldMapDlg.OpenDlg( WorldMapDlg.eDLG_STATE.AREA_DLG, _iIndex );		
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
	}
	
	public virtual void Update()
	{	
	}
}
  