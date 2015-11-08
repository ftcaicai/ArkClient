using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZoneLogic
{
	// Data
	private Map m_curMap;
	private Tbl_ZoneMap_Record m_ZoneMapRecord;

	//Send SendPartyUserPosition
	private float m_fMaxPartySendTime = 5.0f;
	private float m_fPartyTime = 0f;

	// main dialog
	private WorldMapDlg m_WorldMapDlg;

	// npc position ratio
	private Vector2 m_RatioPos = Vector2.one;

	private bool m_isShowObject = true;

	private Dictionary<AchMapInto, UIZoneImg> dicMapMarkInfo = new Dictionary<AchMapInto, UIZoneImg>();
	private List<UIZoneImg> m_ZoneImgList = new List<UIZoneImg>();
	private Dictionary<int, UIZoneImg> m_ZoneNpcImgDic = new Dictionary<int, UIZoneImg>();

	public int getAreaIdx
	{
		get
		{
			if( null == m_ZoneMapRecord)
				return 0;

			return m_ZoneMapRecord.getAreaMapIdx;
		}
	}

	private Vector3 GetRatioPos( Vector3 _temp)
	{
		float fMapPosX = m_WorldMapDlg.zoneMap.getMapImgWidth * 0.5f;
		float fMapPosY = m_WorldMapDlg.zoneMap.getMapImgHeight * 0.5f;

		_temp.x = ( ( -m_curMap.MapData.getMinSize.x + _temp.x) * m_RatioPos.x) - fMapPosX;
		_temp.y = ( ( -m_curMap.MapData.getMinSize.y + _temp.z) * m_RatioPos.y) - fMapPosY;

		return _temp;
	}

	private Vector2 GetMapMin()
	{
		Vector2 temp = Vector2.zero;
		temp.x = m_WorldMapDlg.zoneMap.transform.localPosition.x - ( m_WorldMapDlg.zoneMap.getMapImgWidth * 0.5f);
		temp.y = m_WorldMapDlg.zoneMap.transform.localPosition.y + ( m_WorldMapDlg.zoneMap.getMapImgHeight * 0.5f);

		return temp;
	}

	private Vector2 GetMapMax()
	{
		Vector2 temp = Vector2.zero;
		temp.x = m_WorldMapDlg.zoneMap.transform.localPosition.x + ( m_WorldMapDlg.zoneMap.getMapImgWidth * 0.5f);
		temp.y = m_WorldMapDlg.zoneMap.transform.localPosition.y - ( m_WorldMapDlg.zoneMap.getMapImgHeight * 0.5f);

		return temp;
	}

	public void SetParyPositionInfo( body2_SC_PARTY_USER_POSITION[] _partyUserInfoList)
	{
		if( false == m_WorldMapDlg.isOpenZoneMap)
		{
			AsUtil.ShutDown( "ZoneLogic::SetParyPositionInfo()[ false == m_WorldMapDlg.isOpenZoneMap ]");
			return;
		}

		m_WorldMapDlg.zoneMap.ClearPartyPlayer();

		for( int i = 0; i < _partyUserInfoList.Length; ++i)
		{
			if( m_curMap.MapData.GetID() != _partyUserInfoList[i].nMapIdx)
				continue;

			AS_PARTY_USER user = AsPartyManager.Instance.GetPartyMember( _partyUserInfoList[i].nCharUniqKey);
			if( null == user)
			{
				Debug.LogError( "ZoneLogic::SetParyPositionInfo()[ not find : " + _partyUserInfoList[i].nCharUniqKey);
				continue;
			}
			m_WorldMapDlg.zoneMap.SetPartyPlayer( user.strCharName, i, GetRatioPos( _partyUserInfoList[i].sPosition), GetMapMin(), GetMapMax());
		}
	}

	private void SetNpcPosition( Tbl_ZoneMap_Record _record)
	{
		foreach( Tbl_ZoneMap_Record.NpcData _data in _record.getNpcList)
		{
			UIZoneImg img = m_WorldMapDlg.zoneMap.SetNpc( GetRatioPos( _data.position), AsTableManager.Instance.GetTbl_String(_data.iStrIdx),
				GetMapMin(), GetMapMax(), _data);

			if( null != img)
			{
				m_ZoneImgList.Add( img);

				if( !m_ZoneNpcImgDic.ContainsKey( _data.iNpcId))
					m_ZoneNpcImgDic.Add( _data.iNpcId, img);
				else
					Debug.Log( _data.iNpcId + " is already exist");
			}
		}
	}

	public void UpdateNpcQuestIcon()
	{
		if( m_ZoneMapRecord == null)
			return;

		List<Tbl_ZoneMap_Record.NpcData> npcList = m_ZoneMapRecord.getNpcList;

		foreach( Tbl_ZoneMap_Record.NpcData _data in npcList)
		{
			if( m_ZoneNpcImgDic.ContainsKey( _data.iNpcId))
				m_WorldMapDlg.zoneMap.UpdateQuestMark( m_ZoneNpcImgDic[_data.iNpcId], _data);
		}
	}

	void AddQuestMapMark()
	{
		
		List<AchMapInto> listInfo = new List<AchMapInto>();
		listInfo.AddRange( AsUseItemInMapMarkManager.instance.GetAchUseItemInMapList( m_curMap.MapData.GetID()));
		listInfo.AddRange( AsIntoMapTriggerManager.instance.GetAchUseItemInMapList( m_curMap.MapData.GetID()));

		foreach( AchMapInto useItemInMap in listInfo)
		{
			if( !dicMapMarkInfo.ContainsKey( useItemInMap))
			{
				UIZoneImg uiZoneImg = m_WorldMapDlg.zoneMap.SetUseItemInMap( GetRatioPos( new Vector3( useItemInMap.MapLocationX, 0.0f, useItemInMap.MapLocationZ)),
					GetMapMin(), GetMapMax());
				m_ZoneImgList.Add( uiZoneImg);
				dicMapMarkInfo.Add( useItemInMap, uiZoneImg);
			}
		}
	}

	public void UpdateQuestMapMark()
	{
		List<AchMapInto> listForDelete = new List<AchMapInto>();

		foreach( KeyValuePair<AchMapInto, UIZoneImg> keypair in dicMapMarkInfo)
		{
			if( keypair.Key.IsComplete == true || ArkQuestmanager.instance.IsFailQuest( keypair.Key.AssignedQuestID) || !ArkQuestmanager.instance.IsProgressQuest( keypair.Key.AssignedQuestID))
				listForDelete.Add( keypair.Key);
		}

		// delete
		foreach( AchMapInto key in listForDelete)
		{
			GameObject.DestroyImmediate( dicMapMarkInfo[key].gameObject);
			dicMapMarkInfo.Remove( key);
		}

		// add
		AddQuestMapMark();
	}

	private void SetPotalPosition( Tbl_ZoneMap_Record _record)
	{
		foreach( Tbl_ZoneMap_Record.PotalData potalData in _record.getPotalList)
		{
			UIZoneImg img = m_WorldMapDlg.zoneMap.SetPotal( GetRatioPos( potalData.position), AsTableManager.Instance.GetTbl_String(potalData.iStrIdx),
				GetMapMin(), GetMapMax(), potalData);

			if( null != img)
				m_ZoneImgList.Add( img);
		}
	}

	private void SetWayPointPosition( Tbl_ZoneMap_Record _record)
	{
		foreach( Tbl_ZoneMap_Record.WayPointData waypointData in _record.getWayPointList)
		{
			UIZoneImg img = m_WorldMapDlg.zoneMap.SetWayPoint( GetRatioPos( waypointData.position), AsTableManager.Instance.GetTbl_String(waypointData.iStrIdx),
				GetMapMin(), GetMapMax(), waypointData);

			if( null != img)
				m_ZoneImgList.Add( img);
		}
	}

	private void SetLocalPosition( Tbl_ZoneMap_Record _record)
	{
		foreach( Tbl_ZoneMap_Record.WayPointData waypointData in _record.getLocalNameList)
		{
			m_WorldMapDlg.zoneMap.SetLocal( GetRatioPos( waypointData.position), AsTableManager.Instance.GetTbl_String(waypointData.iStrIdx), GetMapMin(), GetMapMax(), waypointData);
		}
	}

	public void OnTouchBtn()
	{
		if( true == m_isShowObject)
		{
			m_isShowObject = false;
			m_WorldMapDlg.zoneMap.SetViewObject( m_isShowObject);
			m_WorldMapDlg.SetTouchBtnText( AsTableManager.Instance.GetTbl_String(881));
		}
		else
		{
			m_isShowObject = true;
			m_WorldMapDlg.zoneMap.SetViewObject( m_isShowObject);
			m_WorldMapDlg.SetTouchBtnText( AsTableManager.Instance.GetTbl_String(868));
		}
	}

	// core logic
	public ZoneLogic( WorldMapDlg _worldMapDlg)
	{
		m_WorldMapDlg = _worldMapDlg;
		if( null == m_WorldMapDlg)
		{
			AsUtil.ShutDown( "ZoneLogic::ZoneLogic()[ null == m_WorldMapDlg ]");
			return;
		}
	}

	public void Open( int iMapIdx)
	{
		// map table
		m_curMap = TerrainMgr.Instance.GetMap( iMapIdx);
		if( null == m_curMap)
		{
			AsUtil.ShutDown( "UIZoneMap::Open()[ null== map] mapIdx : " + iMapIdx);
			return;
		}

		m_isShowObject = true;

		// zone map table
		m_ZoneMapRecord = AsTableManager.Instance.GetZoneMapRecord( iMapIdx);

		// open zone map dlg
		m_WorldMapDlg.CreateZoneMap( m_ZoneMapRecord.getStrImgPath);

		m_WorldMapDlg.zoneMap.ClearPartyPlayer();

		// set position
		m_RatioPos.x = m_WorldMapDlg.zoneMap.getMapImgWidth / ( m_curMap.MapData.getMaxSize.x - m_curMap.MapData.getMinSize.x);

		m_RatioPos.y = m_WorldMapDlg.zoneMap.getMapImgHeight / ( m_curMap.MapData.getMaxSize.y - m_curMap.MapData.getMinSize.y);

		SetNpcPosition( m_ZoneMapRecord);
		SetPotalPosition( m_ZoneMapRecord);
		SetWayPointPosition( m_ZoneMapRecord);
		SetLocalPosition( m_ZoneMapRecord);
		m_WorldMapDlg.zoneMap.SetViewObject( m_isShowObject);
		UpdateQuestMapMark();

		// party send
		SendPartySender();

		// show state
		m_WorldMapDlg.SetShowUpBtn( true);
		//m_WorldMapDlg.SetPlayerPosImg( true);
		//m_WorldMapDlg.SetShowMapName( true, AsTableManager.Instance.GetTbl_String(m_curMap.MapData.GetNameStrIdx()));
		m_WorldMapDlg.SetTitleName( AsTableManager.Instance.GetTbl_String(m_curMap.MapData.GetNameStrIdx()));
		//m_WorldMapDlg.SetBackGroundEnableCollier( true);

		if( iMapIdx == TerrainMgr.Instance.GetCurMapID())
			m_WorldMapDlg.zoneMap.SetShowPlayerImg( true);
		else
			m_WorldMapDlg.zoneMap.SetShowPlayerImg( false);

		SetAlpahMap( UIWorldMapSettingDlg.s_alphaValue);
	}

	public void SetAlpahMap( int iAlphaValue)
	{
		float fRealAlpha = 1f - ( ( float)iAlphaValue*0.01f);
		foreach( Material _mat in m_WorldMapDlg.zoneMap.alphaMaterials)
		{
			if( null == _mat)
				continue;

			Color _temp = _mat.color;
			_temp.a = fRealAlpha;
			_mat.color = _temp;
		}

		foreach( UIZoneImg keypair in m_ZoneImgList)
		{
			if( null == keypair)
				continue;

			keypair.SetAlpahMap( fRealAlpha);
		}
	}

	public void SendPartySender()
	{
		m_fPartyTime = 0f;
		if( true == AsPartyManager.Instance.IsPartying)
			AsPartySender.SendPartyUserPosition();
	}

	public void Close()
	{
		m_WorldMapDlg.DeleteZoneMap();
		dicMapMarkInfo.Clear();
		m_ZoneNpcImgDic.Clear();

		if( AsHudDlgMgr.Instance.isOpenWorldMapSettingDlg)
			AsHudDlgMgr.Instance.CloseWorldMapSettingDlg();
	}

	public void Update()
	{
		if( false == m_WorldMapDlg.isOpenZoneMap)
			return;

		if( true == AsPartyManager.Instance.IsPartying)
		{
			m_fPartyTime += Time.deltaTime;

			if( m_fPartyTime >= m_fMaxPartySendTime)
				SendPartySender();
		}

		m_WorldMapDlg.zoneMap.SetPlayerPosition( GetRatioPos( AsEntityManager.Instance.UserEntity.transform.position));
	}

	public virtual void GuiInputDown( Ray inputRay)
	{
	}

	public virtual void GuiInputMove( Ray inputRay)
	{
	}

	public virtual void GuiInputUp( Ray inputRay)
	{
	}
}
