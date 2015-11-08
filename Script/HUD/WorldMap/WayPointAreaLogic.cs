using UnityEngine;
using System.Collections;

public class WayPointAreaLogic : Arealogic
{
	protected AsMessageBox m_BuyAskAgin;
	protected int m_iWarpIdx = 0;
	protected ulong m_iNeedGold = 0;
	protected System.Text.StringBuilder m_sbClickText = new System.Text.StringBuilder();

	public WayPointAreaLogic( WorldMapDlg _worldMapDlg) : base( _worldMapDlg)
	{
	}

	public override void Close()
	{
		base.Close();

		if( null != m_BuyAskAgin)
			GameObject.Destroy(m_BuyAskAgin.gameObject);
	}

	public void GoPotal()
	{
		if( true == AsUserInfo.Instance.IsDied())
			return;

		if( AsPStoreManager.Instance.UnableActionByPStore() == true)
		{
			AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(365),
				null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE));
			return;
		}

		if( AsUserInfo.Instance.SavedCharStat.nGold < m_iNeedGold)
		{
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(252),
				null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			return;
		}

		if( AsUserInfo.Instance.IsBattle())
		{
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(1633),
				null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			return;
		}

		Tbl_WarpData_Record record = AsTableManager.Instance.GetWarpDataRecord(m_iWarpIdx);
		if( null == record)
			return;

		if( false == record.isActive)
		{
			if( false == AsHudDlgMgr.Instance.isOpenMsgBox)
				AsChatManager.Instance.InsertChat(AsTableManager.Instance.GetTbl_String(873), eCHATTYPE.eCHATTYPE_SYSTEM);

			return;
		}

		if( 0 >= m_iWarpIdx)
		{
			Debug.LogError("WayPointAreaLogic::GoPotal()[ 0 >= m_iWarpIdx ] m_iWarpIdx : " + m_iWarpIdx);
			return;
		}

		AsCommonSender.SendWarp( m_iWarpIdx);
	}

	public override void ClickBtnMsg( int iMapIdx, int iWarpIdx)
	{
		if( null != m_BuyAskAgin)
			return;

		m_iFocusMapIdx = iMapIdx;
		if( false == m_WorldMapDlg.isOpenAreaMap)
		{
			AsUtil.ShutDown("WayPointAreaLogic::ClickBtnMsg()[ false == m_WorldMapDlg.isOpenAreaMap ]");
			return;
		}

		m_WorldMapDlg.areaMap.SetFocusZoneMap( m_iFocusMapIdx);

		ClickBtnAction( iMapIdx, iWarpIdx);
	}

	private void ClickBtnAction( int iMapIdx, int iWarpIdx)
	{
		if( TerrainMgr.Instance.GetCurMapID() != iMapIdx)
		{
			Tbl_WarpData_Record record = AsTableManager.Instance.GetWarpDataRecord( iWarpIdx);
			if( null == record)
			{
				AsUtil.ShutDown("WayPointAreaLogic::ClickBtnMsg() [ null == Tbl_WarpData_Record ] warpIdx : " + iWarpIdx);
				return;
			}

			Map _warpMap = TerrainMgr.Instance.GetMap(record.getWarpMapId);
			if( null == _warpMap)
				return;

			Map _curMap = TerrainMgr.Instance.GetMap(TerrainMgr.Instance.GetCurMapID());
			if( null == _curMap)
				return;

			m_iNeedGold = ( 1 + (ulong)Mathf.Abs( _warpMap.MapData.getMapLevel - _curMap.MapData.getMapLevel)) * (ulong)record.getGoldCost;

			m_iWarpIdx = iWarpIdx;

			m_sbClickText.Remove( 0, m_sbClickText.Length);
			m_sbClickText.Append( AsTableManager.Instance.GetTbl_String(_warpMap.MapData.GetNameStrIdx()));
			m_sbClickText.Append( " ");
			m_sbClickText.Append(AsTableManager.Instance.GetTbl_String(251));

			m_BuyAskAgin = AsNotify.Instance.GoldMessageBox( m_iNeedGold, AsTableManager.Instance.GetTbl_String(126), m_sbClickText.ToString(),
				m_WorldMapDlg, "GoPotal", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);
		}
	}

	public override void Open( int _iAreaIdx)
	{
		m_iAreaIdx = _iAreaIdx;
		m_AreaMapRecord = AsTableManager.Instance.GetAreaMapRecord( m_iAreaIdx);
		if( null == m_AreaMapRecord)
		{
			AsUtil.ShutDown("WayPointAreaLogic::Open()[ null == Tbl_AreaMap_Record ] area index : " + m_iAreaIdx);
			return;
		}

		if( ArkQuestmanager.instance.CheckHaveOpenUIType( OpenUIType.WAYPOINT) != null)
			AsCommonSender.SendClearOpneUI( OpenUIType.WAYPOINT);

		m_iFocusMapIdx = TerrainMgr.Instance.GetCurMapID();
		m_WorldMapDlg.CreateAreaMap( m_AreaMapRecord.getStrImgPath, m_iFocusMapIdx);
		m_WorldMapDlg.SetShowUpBtn( true);
		m_WorldMapDlg.areaMap.AllCloseBtn();
		m_WorldMapDlg.areaMap.SetNoPartyImg( m_iFocusMapIdx);

		ResetAreaMapBtnState();

		CheckArrayState();
	}

	public override void ClickArrayBtn( UIWorldBackGround.eAREAMAP_ARRAW _state)
	{
		switch( _state)
		{
		case UIWorldBackGround.eAREAMAP_ARRAW.left:
			if( -1 != iLeftIndex)
			{
				m_WorldMapDlg.CloseDlg( m_WorldMapDlg.getCurDlgState);
				m_WorldMapDlg.OpenDlg( WorldMapDlg.eDLG_STATE.WAYPOINT_AREA_DLG, iLeftIndex);
			}
			break;
		case UIWorldBackGround.eAREAMAP_ARRAW.right:
			if( -1 != iRightIndex)
			{
				m_WorldMapDlg.CloseDlg( m_WorldMapDlg.getCurDlgState);
				m_WorldMapDlg.OpenDlg( WorldMapDlg.eDLG_STATE.WAYPOINT_AREA_DLG, iRightIndex);
			}
			break;
		case UIWorldBackGround.eAREAMAP_ARRAW.up:
			if( -1 != iUpIndex)
			{
				m_WorldMapDlg.CloseDlg( m_WorldMapDlg.getCurDlgState);
				m_WorldMapDlg.OpenDlg( WorldMapDlg.eDLG_STATE.WAYPOINT_AREA_DLG, iUpIndex);
			}
			break;
		case UIWorldBackGround.eAREAMAP_ARRAW.down:
			if( -1 != iDownIndex)
			{
				m_WorldMapDlg.CloseDlg( m_WorldMapDlg.getCurDlgState);
				m_WorldMapDlg.OpenDlg( WorldMapDlg.eDLG_STATE.WAYPOINT_AREA_DLG, iDownIndex);
			}
			break;
		}
	}

	public void ResetAreaMapBtnState()
	{
		foreach( UIAreaBtn btn in m_WorldMapDlg.areaMap.btnMapList)
		{
			Tbl_WarpData_Record record = AsTableManager.Instance.GetWarpDataIdxToUseMapID( btn.mapIdx);
			if( null == record)
			{
				btn.CloseBtn();
				continue;
			}

			if( true == AsUserInfo.Instance.IsExistWaypoint( record.Index))
				btn.OpenBtn();
		}
	}

	public override void Update()
	{
	}
}
