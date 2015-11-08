using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class Arealogic 
{	
	protected int m_iAreaIdx;
	protected WorldMapDlg m_WorldMapDlg;	
	protected Tbl_AreaMap_Record m_AreaMapRecord;
	protected int m_iFocusMapIdx  = 0;
	private Dictionary<int, int> m_PartyList = new Dictionary<int, int>(); // mapid, num
	private float m_fMaxPartySendTime = 15.0f;
	private float m_fTime = 0f;
	body2_SC_PARTY_USER_POSITION[] m_ServerPartyUserList;
	
	protected int iUpIndex = -1;
	protected int iDownIndex = -1;
	protected int iRightIndex = -1;
	protected int iLeftIndex = -1;
	
	public int getAreaMapIdx
	{
		get
		{
			return m_iAreaIdx;
		}
	}
	
	public Arealogic( WorldMapDlg _worldMapDlg )
	{
		m_WorldMapDlg = _worldMapDlg;
		if( null == m_WorldMapDlg )
		{
			AsUtil.ShutDown(" null == m_WorldMapDlg ");
			return;
		}		
	}		
	
	
	public virtual void ClickArrayBtn( UIWorldBackGround.eAREAMAP_ARRAW _state )
	{
		switch( _state )
		{
		case UIWorldBackGround.eAREAMAP_ARRAW.left:
			if( -1 != iLeftIndex )
			{
				m_WorldMapDlg.CloseDlg( m_WorldMapDlg.getCurDlgState );
				m_WorldMapDlg.OpenDlg( WorldMapDlg.eDLG_STATE.AREA_DLG, iLeftIndex );
			}
			break;
			
		case UIWorldBackGround.eAREAMAP_ARRAW.right:
			if( -1 != iRightIndex )
			{
				m_WorldMapDlg.CloseDlg( m_WorldMapDlg.getCurDlgState );
				m_WorldMapDlg.OpenDlg( WorldMapDlg.eDLG_STATE.AREA_DLG, iRightIndex );
			}
			break;
			
		case UIWorldBackGround.eAREAMAP_ARRAW.up:
			if( -1 != iUpIndex )
			{
				m_WorldMapDlg.CloseDlg( m_WorldMapDlg.getCurDlgState );
				m_WorldMapDlg.OpenDlg( WorldMapDlg.eDLG_STATE.AREA_DLG, iUpIndex );
			}
			break;
			
		case UIWorldBackGround.eAREAMAP_ARRAW.down:
			if( -1 != iDownIndex )
			{
				m_WorldMapDlg.CloseDlg( m_WorldMapDlg.getCurDlgState );
				m_WorldMapDlg.OpenDlg( WorldMapDlg.eDLG_STATE.AREA_DLG, iDownIndex );
			}
			break;
		}		
	}
	
	
	public void SetParyPositionInfo( body2_SC_PARTY_USER_POSITION[] _info )
	{
		if( false == m_WorldMapDlg.isOpenAreaMap )
			return;
		
		m_ServerPartyUserList = _info;
		
		m_PartyList.Clear();
		m_WorldMapDlg.areaMap.ClearPartyImg();
		
		
		foreach( body2_SC_PARTY_USER_POSITION _data in _info )
		{
			if( m_PartyList.ContainsKey( _data.nMapIdx ) )
			{
				m_PartyList[_data.nMapIdx] += 1;
			}
			else
			{
				m_PartyList.Add( _data.nMapIdx, 1 );
			}
		}
		
		if( m_PartyList.Count > 0 )		
		{
			foreach( KeyValuePair<int, int> pair in m_PartyList )
			{
				m_WorldMapDlg.areaMap.SetPartyImg( pair.Key, pair.Value, TerrainMgr.Instance.GetCurMapID() );
			}
		}
		else
		{
			SetNoPartyImg();
		}
		
		ResetPartyNameList();
	}
	
	public void ResetPartyNameList()
	{		
		if( null == m_ServerPartyUserList )
			return;
		
		m_WorldMapDlg.SetHidePartyName();
		int i = 0;
		foreach( body2_SC_PARTY_USER_POSITION _data in m_ServerPartyUserList )
		{
			if( _data.nMapIdx == m_iFocusMapIdx )
			{
				AS_PARTY_USER user = AsPartyManager.Instance.GetPartyMember( _data.nCharUniqKey );
				if( null != user )
				{					
					m_WorldMapDlg.SetPartiName( i++,  user.strCharName );
				}
			}
		}
	}
		
	
	public virtual void Close()
	{
		m_WorldMapDlg.DeleteAreaMap();
	}
	
	// virtual 
	public virtual void ClickBtnMsg( int iMapIdx )
	{
		if( iMapIdx == m_iFocusMapIdx )
		{
			AsHudDlgMgr.Instance.worldMapDlg.CloseDlg();
			AsHudDlgMgr.Instance.worldMapDlg.OpenDlg( WorldMapDlg.eDLG_STATE.ZONE_DLG, iMapIdx );
		}
		else
		{
			m_iFocusMapIdx = iMapIdx;
			if( false == m_WorldMapDlg.isOpenAreaMap )
			{
				AsUtil.ShutDown("AreaLogic::ClickBtnMsg()[ false == m_WorldMapDlg.isOpenAreaMap ]");
				return;
			}
			
			m_WorldMapDlg.areaMap.SetFocusZoneMap( m_iFocusMapIdx );	
			ResetPartyNameList();
		}
	}
	
	public virtual void ClickBtnMsg( int iMapIdx, int iWarpIdx )
	{
		Debug.LogError("use error");
	}
	
	
	public virtual void Open( int _iAreaIdx )
	{
		m_iAreaIdx = _iAreaIdx;
		m_AreaMapRecord = AsTableManager.Instance.GetAreaMapRecord( m_iAreaIdx );
		if( null == m_AreaMapRecord )
		{
			AsUtil.ShutDown("AreaLogic::Open()[ null == Tbl_AreaMap_Record ] area index : " + m_iAreaIdx );
			return;
		}
		
		m_iFocusMapIdx = TerrainMgr.Instance.GetCurMapID();		
		m_WorldMapDlg.CreateAreaMap( m_AreaMapRecord.getStrImgPath, m_iFocusMapIdx );					
		m_WorldMapDlg.SetShowUpBtn(true);		
		m_WorldMapDlg.SetTitleName( AsTableManager.Instance.GetTbl_String( m_AreaMapRecord.getTitleStrIdx) );
		
		m_fTime = 0f;
		if( true == AsPartyManager.Instance.IsPartying )
		{
			AsPartySender.SendPartyUserPosition();
		}
		else
		{
			SetNoPartyImg();
		}
		
		ResetPartyNameList();		
		
		CheckArrayState();
	}
	
	public void CheckArrayState()
	{
		
		
		iUpIndex = m_iAreaIdx - 4;
		iDownIndex = m_iAreaIdx + 4;
		iRightIndex = m_iAreaIdx + 1;
		iLeftIndex = m_iAreaIdx - 1;
		
		if( iUpIndex >= 1 && iUpIndex <= 8 ) 	
		{
			if( IsEnableAreaMap( iUpIndex ) )
				m_WorldMapDlg.SetAreaMapArrawBtn(UIWorldBackGround.eAREAMAP_ARRAW.up, true );
			else
				m_WorldMapDlg.SetAreaMapArrawBtn(UIWorldBackGround.eAREAMAP_ARRAW.up, false );
		}
		else
			m_WorldMapDlg.SetAreaMapArrawBtn(UIWorldBackGround.eAREAMAP_ARRAW.up, false );		
		
		if( iDownIndex >= 1 && iDownIndex <= 8 ) 	
		{
			if( IsEnableAreaMap( iDownIndex ) )
				m_WorldMapDlg.SetAreaMapArrawBtn(UIWorldBackGround.eAREAMAP_ARRAW.down, true );
			else
				m_WorldMapDlg.SetAreaMapArrawBtn(UIWorldBackGround.eAREAMAP_ARRAW.down, false );
		}
		else
			m_WorldMapDlg.SetAreaMapArrawBtn(UIWorldBackGround.eAREAMAP_ARRAW.down, false );
		
		if( iRightIndex >= 1 && iRightIndex <= 8 ) 	
		{
			if( IsEnableAreaMap( iRightIndex ) )
				m_WorldMapDlg.SetAreaMapArrawBtn(UIWorldBackGround.eAREAMAP_ARRAW.right, true );
			else
				m_WorldMapDlg.SetAreaMapArrawBtn(UIWorldBackGround.eAREAMAP_ARRAW.right, false );
		}
		else
			m_WorldMapDlg.SetAreaMapArrawBtn(UIWorldBackGround.eAREAMAP_ARRAW.right, false );
		
		if( iLeftIndex >= 1 && iLeftIndex <= 8 ) 	
		{
			if( IsEnableAreaMap( iLeftIndex ) )
				m_WorldMapDlg.SetAreaMapArrawBtn(UIWorldBackGround.eAREAMAP_ARRAW.left, true );
			else
				m_WorldMapDlg.SetAreaMapArrawBtn(UIWorldBackGround.eAREAMAP_ARRAW.left, false );
		}
		else
			m_WorldMapDlg.SetAreaMapArrawBtn(UIWorldBackGround.eAREAMAP_ARRAW.left, false );
		
	}
	
	private bool IsEnableAreaMap( int iIndex ) 
	{
		Tbl_AreaMap_Record _record = AsTableManager.Instance.GetAreaMapRecord( iIndex );
		if( null == _record )
		{
			return false;
		}
		
		return _record.isActive;
	}
	
	private void SetNoPartyImg()
	{		
		if( false == m_WorldMapDlg.isOpenAreaMap )
			return;
		
		m_WorldMapDlg.areaMap.ClearPartyImg();		
		m_WorldMapDlg.areaMap.SetNoPartyImg( TerrainMgr.Instance.GetCurMapID() );		
	}
	
	public virtual void Update()
	{	
		if( true == AsPartyManager.Instance.IsPartying )
		{
			m_fTime += Time.deltaTime;
			
			if( m_fTime >= m_fMaxPartySendTime )
			{
				AsPartySender.SendPartyUserPosition();
				m_fTime = 0;
			}
		}	
		else
		{
			SetNoPartyImg();
		}
	}	
	
	public virtual void GuiInputDown( Ray inputRay )
	{	
	}
	
	public virtual void GuiInputMove( Ray inputRay )
	{
		
	}
	
	public virtual void GuiInputUp( Ray inputRay )
	{
		
	}
}
