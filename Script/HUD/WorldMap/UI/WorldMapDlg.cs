using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class WorldMapDlg : UIMessageBase 
{		
	public enum eDLG_STATE
	{
		CLOSE,
		ZONE_DLG,
		AREA_DLG,
		WORLD_DLG,		
		WAYPOINT_AREA_DLG,
		WAYPOINT_WORLD_DLG,	
	};
	
	
	// path
	public string zonePath = "UI/AsGUI/Worldmap/Object_ZoneMap";	
	public string worldPath = "UI/AsGUI/Worldmap/Map_World";
	public string backgroundPath = "UI/AsGUI/Worldmap/GUI_WorldMap";
	
	// layout
	public float dlgLayout = -1f;
	public float backgroundLayout = 0f;	
	
	//btn
	//public UIStateToggleBtn btnWorldMap;
	public UIButton btnWorldOpen;
	public SimpleSprite worldImgClose;
	public SimpleSprite worldImgOpen;

    public FingerPointerController fingerPointer;
	
	// logic
	private ZoneLogic m_ZoneLogic;
	private Arealogic m_AreaLogic;
	private WorldLogic m_WorldLogic;
	private WayPointWorldLogic m_WayPointWorldLogic;
	private WayPointAreaLogic m_WayPointAreaLogic;	
	
	// ui 
	private UIZoneMap m_uiZoneMap;
	private UIAreaMap m_uiAreaMap;
	private UIWorldMap m_uiWorldMap;
	private UIWorldBackGround m_uiBackGround;
	
	// state
	private eDLG_STATE m_eDlgState = eDLG_STATE.CLOSE;
	private  AsBaseEntity m_TargetEntity;
	
	public bool isBackGround
	{
		get
		{
			if( null == m_uiBackGround )
				return false;
			
			return m_uiBackGround.gameObject.active;
		}
	}
	
	public bool IsCurStateWaypointDlg()
	{
		if( eDLG_STATE.WAYPOINT_AREA_DLG == m_eDlgState || eDLG_STATE.WAYPOINT_WORLD_DLG == m_eDlgState )
			return true;
		
		return false;
	}
	
	// Open & Close
	
	public void OpenWorldMap()
	{			
		DeleteBackground();
		CloseDlg(m_eDlgState);
		
		Map _map = TerrainMgr.Instance.GetCurrentMap();
		
		if( null != _map && eMAP_TYPE.Tutorial == TerrainMgr.Instance.GetCurrentMap().MapData.getMapType ) 
		{
			ShowWorldMapBtn( false );		
		}
		else
		{
			ShowWorldMapBtn( true );	
		}	
	}

    
	public void CloseWorldMap()
	{
		DeleteBackground();
		CloseDlg(m_eDlgState);
		ShowWorldMapBtn( false );
       
	}
	
	public void OpenWaypointMap( AsBaseEntity _targetEntity )
	{
		Tbl_ZoneMap_Record record = AsTableManager.Instance.GetZoneMapRecord( TerrainMgr.Instance.GetCurMapID() );
		if( null == record)
		{
			AsUtil.ShutDown("WorldMapDlg::OpenWaypointMap()[ null == Tbl_ZoneMap_Record ] id : " + TerrainMgr.Instance.GetCurMapID() );
			return;
		}
		m_TargetEntity = _targetEntity;
		DeleteBackground();
		OpenDlg( eDLG_STATE.WAYPOINT_AREA_DLG, record.getAreaMapIdx );

		SetWorldBtnSwitch(1);
        
		//ShowWorldMapBtn( false );
		if( null != m_uiBackGround )
			m_uiBackGround.SetTitleName( AsTableManager.Instance.GetTbl_String( 1301 ) );
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
	}
	
	public void CloseWaypointMap()
	{
		m_TargetEntity = null;
		DeleteBackground();
		CloseDlg(m_eDlgState);
		ShowWorldMapBtn( true );
	}
	
	
	public void CloseDlg()
	{
		CloseDlg(m_eDlgState);
	}
	
	public void CloseDlg( eDLG_STATE eState )
	{
		switch( eState )
		{
		case eDLG_STATE.ZONE_DLG:
			GetZoneLogic().Close();
			break;
			
		case eDLG_STATE.AREA_DLG:
			GetAreaLogic().Close();
			break;
			
		case eDLG_STATE.WORLD_DLG:
			GetWorldLogic().Close();
			break;
			
		case eDLG_STATE.WAYPOINT_AREA_DLG:
			GetWayPointAreaLogic().Close();
			break;
			
		case eDLG_STATE.WAYPOINT_WORLD_DLG:
			GetWayPointWorldLogic().Close();
			break;
		}
		
		m_eDlgState = eDLG_STATE.CLOSE;

      
	}
	
	public void OpenDlg( eDLG_STATE eState, int iIndex )
	{			
		CreateBackground();		
		m_eDlgState = eState;
		
		SetHidePartyName();
		SetShowUpBtn(false);		
		//SetBackGroundEnableCollier( true );		
		SetActiveTouchBtn( false);
		
		SetAreaMapArrawBtn( UIWorldBackGround.eAREAMAP_ARRAW.left, false );
		SetAreaMapArrawBtn( UIWorldBackGround.eAREAMAP_ARRAW.right, false );
		SetAreaMapArrawBtn( UIWorldBackGround.eAREAMAP_ARRAW.up, false );
		SetAreaMapArrawBtn( UIWorldBackGround.eAREAMAP_ARRAW.down, false );
		
		switch( m_eDlgState )
		{
		case eDLG_STATE.ZONE_DLG:              
			SetActiveTouchBtn(true);
			GetZoneLogic().Open( iIndex );	
			SetWorldBtnSwitch(1);
			SetWorldMapSize(UIWorldMapSettingDlg.s_bigMap);
			if( null != m_uiBackGround )
            	m_uiBackGround.SetViewBtnSetting( true );
			break;
			
		case eDLG_STATE.AREA_DLG:			
			GetAreaLogic().Open( iIndex );
            SetWorldMapSize(true);
			if( null != m_uiBackGround )
            	m_uiBackGround.SetViewBtnSetting( false );
			break;
			
		case eDLG_STATE.WORLD_DLG:			
			GetWorldLogic().Open(/* iIndex */);
            SetWorldMapSize(true);
			if( null != m_uiBackGround )
            	m_uiBackGround.SetViewBtnSetting( false );
			break;
			
		case eDLG_STATE.WAYPOINT_AREA_DLG:		
			GetWayPointAreaLogic().Open( iIndex );
            SetWorldMapSize(true);
			if( null != m_uiBackGround )
            	m_uiBackGround.SetViewBtnSetting( false );
			break;
			
		case eDLG_STATE.WAYPOINT_WORLD_DLG:			
			GetWayPointWorldLogic().Open(/* iIndex */);
            SetWorldMapSize(true);
			if( null != m_uiBackGround )
            	m_uiBackGround.SetViewBtnSetting( false );
			break;
		}
	}
			
	
	public eDLG_STATE getCurDlgState
	{
		get
		{
			return m_eDlgState;
		}
	}
	
	
	// Zone map 
	
	public ZoneLogic GetZoneLogic()
	{
		if( null == m_ZoneLogic )
		{
			m_ZoneLogic  = new ZoneLogic( this );
		}
		
		return m_ZoneLogic;
	}
		
	public UIZoneMap zoneMap
	{
		get
		{
			return m_uiZoneMap;
		}
		
		set
		{
			m_uiZoneMap = value;
		}
	}
	
	public bool isOpenZoneMap
	{
		get
		{
			if( null == m_uiZoneMap )
				return false;
			
			return m_uiZoneMap.gameObject.active;
		}
	}
	
	public bool CreateZoneMap( string strBackImgPath )
	{		
		if( null == m_uiZoneMap )
		{		
			GameObject goInstance = ResourceLoad.CreateGameObject( zonePath, m_uiBackGround.dlgPos.transform );		
			
			m_uiZoneMap = goInstance.GetComponent< UIZoneMap >();
			if( null == m_uiZoneMap )
			{
				AsUtil.ShutDown( "WorldMapDlg::OpenZoneMap()[no find UIZoneMap] path : "  + zonePath );
				GameObject.DestroyObject(goInstance);
				return false;
			}	
			
			goInstance.transform.localPosition = new Vector3( 0f, 0f, dlgLayout );		
		}			
		
		m_uiZoneMap.Open( strBackImgPath );		
		
		return true;
	}	

	public void UpdateZoneMapImg()
	{
		if (m_uiZoneMap != null)
			m_ZoneLogic.UpdateQuestMapMark();
	}
	
	public void DeleteZoneMap()
	{
		if( null != m_uiZoneMap )
		{
			m_uiZoneMap.Close();
			GameObject.DestroyObject( m_uiZoneMap.gameObject );			
		}			
	}
	
	
	
	
	
	// Area map
	public Arealogic GetAreaLogic()
	{
		if( null == m_AreaLogic )
		{
			m_AreaLogic  = new Arealogic( this );
		}
		
		return m_AreaLogic;
	}
	
	
	public UIAreaMap areaMap
	{
		get
		{
			return m_uiAreaMap;
		}
	}
	
	
	public void CreateAreaMap( string strPath, int iCurMapIdx )
	{		
		if( null == m_uiAreaMap )
		{						
			GameObject goInstance = ResourceLoad.CreateGameObject( strPath, m_uiBackGround.dlgPos.transform );		
			
			m_uiAreaMap = goInstance.GetComponent< UIAreaMap >();
			if( null == m_uiAreaMap )
			{
				AsUtil.ShutDown( "WorldMapDlg::OpenAreaMap()[no find m_uiAreaMap] path : "  + strPath );
				GameObject.DestroyObject(goInstance);
				return;
			}	
			m_uiAreaMap.SetAreaMapClickDelegate(AreaMapBtnClick	); 
			goInstance.transform.localPosition = new Vector3( 0f, 0f, dlgLayout );		
		}			
		
		m_uiAreaMap.Open( iCurMapIdx );		
	}
	
	public void DeleteAreaMap()
	{
		if( null != m_uiAreaMap )
		{
			m_uiAreaMap.Close();
			GameObject.DestroyObject( m_uiAreaMap.gameObject );	
			m_uiAreaMap = null;
		}	
	}
	
	public bool isOpenAreaMap
	{
		get
		{
			if( null == m_uiAreaMap )
				return false;
			
			return m_uiAreaMap.gameObject.active;		
		}
	}
	
	
		
	
	// World Map
	public WorldLogic GetWorldLogic()
	{
		if( null == m_WorldLogic )
		{
			m_WorldLogic  = new WorldLogic( this );
		}
		
		return m_WorldLogic;
	}
	
	public UIWorldMap worldMap
	{
		get
		{
			return m_uiWorldMap;
		}
	}
	
	public void CreateWorldMap(/* int iAreaMapIdx */)
	{
		if( null == m_uiWorldMap )
		{			
			
			GameObject goInstance = ResourceLoad.CreateGameObject( worldPath, m_uiBackGround.dlgPos.transform );		
			
			m_uiWorldMap = goInstance.GetComponent< UIWorldMap >();
			if( null == m_uiWorldMap )
			{
				AsUtil.ShutDown( "WorldMapDlg::OpenWorldMap()[no find UIWorldMap] path : "  + worldPath );
				GameObject.DestroyObject(goInstance);
				return;
			}	
			
			goInstance.transform.localPosition = new Vector3( 0f, 0f, dlgLayout );		
		}			
		
		m_uiWorldMap.Open(/* iAreaMapIdx */);		
	}
	
	public void DeleteWorldMap()
	{
		if( null != m_uiWorldMap )
		{
			m_uiWorldMap.Close();
			GameObject.DestroyObject( m_uiWorldMap.gameObject );
		}		
	}
	
	public bool isOpenWorldMap
	{
		get
		{
			if( null == m_uiWorldMap )
				return false;
			
			return m_uiWorldMap.gameObject.active;		
		}
	}
	
	
	
	
	// WayPointWorld Map
	public WayPointWorldLogic GetWayPointWorldLogic()
	{
		if( null == m_WayPointWorldLogic )
		{
			m_WayPointWorldLogic  = new WayPointWorldLogic(this);
		}
		
		return m_WayPointWorldLogic;
	}
	
	// WayPointArea Map
	public WayPointAreaLogic GetWayPointAreaLogic()
	{
		if( null == m_WayPointAreaLogic )
		{
			m_WayPointAreaLogic  = new WayPointAreaLogic( this );
		}
		
		return m_WayPointAreaLogic;
	}
	
	
	// Background
	
	private void CreateBackground()
	{
		if( null != m_uiBackGround )
			return;
		
		GameObject goIns = ResourceLoad.CreateGameObject( backgroundPath, transform );
		
		m_uiBackGround = goIns.GetComponent<UIWorldBackGround>();
		if( null == m_uiBackGround )
		{
			GameObject.DestroyObject(goIns);
			AsUtil.ShutDown("WorldMapDlg::CreateBackgound()[ null == UIWorldBackGround ]");
			return;
		}		
		
		goIns.transform.localPosition = new Vector3( 0f, 0f, backgroundLayout );
	}
	
	private void DeleteBackground()
	{
		if( null == m_uiBackGround )
			return;
		
		GameObject.DestroyObject( m_uiBackGround.gameObject );
		m_uiBackGround = null;
	}
	
	public void SetTouchBtnText( string strText )
	{
		if( null == m_uiBackGround )
			return;
		
		m_uiBackGround.SetMapTouchText( strText );
	}


    public void SetWorldMapSize( bool _isBig )
    {
        if (null == m_uiBackGround)
            return;

        m_uiBackGround.SetMapSize(_isBig);
		
		ShowWorldMapBtn(_isBig);
		if( true == _isBig )
		{
			SetWorldBtnSwitch( 1 );
		}		
    }
	
	
	// Set 
	
	public void ShowWorldMapBtn( bool isShow )
	{
		if( null == btnWorldOpen || null == worldImgClose || null == worldImgOpen )
		{			
			return;
		}
		
		if( true == isShow )
		{			
			btnWorldOpen.gameObject.active = true;		
			SetWorldBtnSwitch(0);		
		}
		else
		{
			worldImgClose.gameObject.active = false;
			worldImgOpen.gameObject.active = false;
			btnWorldOpen.gameObject.active = false;
		}
	}	
	
	public void MapTouch()
	{
		switch( m_eDlgState )
		{
		case eDLG_STATE.ZONE_DLG:			
			GetZoneLogic().OnTouchBtn();
			break;
			
		case eDLG_STATE.AREA_DLG:	
			
			break;
			
		case eDLG_STATE.WORLD_DLG:		
			break;
			
		case eDLG_STATE.WAYPOINT_AREA_DLG:
			break;
			
		case eDLG_STATE.WAYPOINT_WORLD_DLG:
			break;
		}
	}
	
	public void ReceiveWaypointList()
	{		
		switch( m_eDlgState )
		{
		case eDLG_STATE.ZONE_DLG:			
			
			break;
			
		case eDLG_STATE.AREA_DLG:
		
			break;
			
		case eDLG_STATE.WORLD_DLG:			
			break;
			
		case eDLG_STATE.WAYPOINT_AREA_DLG:
			GetWayPointAreaLogic().ResetAreaMapBtnState();
			break;
			
		case eDLG_STATE.WAYPOINT_WORLD_DLG:
			break;
		}
		
	}
	
	public void RecivePartyPositionInfo( body1_SC_PARTY_USER_POSITION _info )
	{	
		switch( m_eDlgState )
		{
		case eDLG_STATE.ZONE_DLG:			
			GetZoneLogic().SetParyPositionInfo( _info.body );
			break;
			
		case eDLG_STATE.AREA_DLG:	
			GetAreaLogic().SetParyPositionInfo( _info.body );
			break;
			
		case eDLG_STATE.WORLD_DLG:
			GetWorldLogic().SetParyPositionInfo( _info.body );
			break;
			
		case eDLG_STATE.WAYPOINT_AREA_DLG:
			break;
			
		case eDLG_STATE.WAYPOINT_WORLD_DLG:
			break;
		}		
	}	
	
	public void ReceiveWaypointActive()
	{
		switch( m_eDlgState )
		{
		case eDLG_STATE.WAYPOINT_AREA_DLG:
			GetWayPointAreaLogic().ResetAreaMapBtnState();
			break;
			
		}
	}
	
	public void SetHidePartyName()
	{
		if( null == m_uiBackGround )
			return;
		
		m_uiBackGround.SetHidePartyName();
	}
	
	public void SetActiveTouchBtn( bool isActive )
	{
		if( null == m_uiBackGround )
			return;
		
		m_uiBackGround.SetActiveBtnMapTouch( isActive );
	}
	
	
	public void SetShowUpBtn( bool isShow )
	{
		if( null == m_uiBackGround )
			return;
		
		m_uiBackGround.SetUpStateBtnShow(isShow);			
	}
	
	public void SetPartiName(int iIndex, string strName )
	{
		if( null == m_uiBackGround )
			return;
		
		m_uiBackGround.SetPartyName( iIndex, strName );
	}
		
	
	public void SetTitleName( string strName )
	{
		if( null == m_uiBackGround )
			return;		
		
		m_uiBackGround.SetTitleName( strName );
	}
	
	public void SetAreaMapArrawBtn( UIWorldBackGround.eAREAMAP_ARRAW _type, bool isView )
	{
		if( null == m_uiBackGround )
			return;		
		
		m_uiBackGround.SetAreaMapArrawBtn( _type, isView );
	}
	
	/*public void SetBackGroundEnableCollier( bool isEnable )
	{
		if( null == m_uiBackGround )
			return;
		
		m_uiBackGround.SetEnableCollider( isEnable );
	}*/
	
	public void GoPotal()
	{
		if( eDLG_STATE.WAYPOINT_AREA_DLG ==  m_eDlgState )
			GetWayPointAreaLogic().GoPotal();		
	}
	
	/*public bool IsBackRect( Ray _ray )
	{
		if( null == m_uiBackGround )
			return false;
		
		return m_uiBackGround.IsRect( _ray );
	}*/
	
	
	// message
	
	public void AreaMapBtnClick( int iMapId, int iWarpIdx )
	{
		switch( m_eDlgState )
		{			
		case eDLG_STATE.AREA_DLG:
			GetAreaLogic().ClickBtnMsg( iMapId );
			break;
			
		case eDLG_STATE.WAYPOINT_AREA_DLG:			
			GetWayPointAreaLogic().ClickBtnMsg( iMapId, iWarpIdx );
			break;
		}
	}
	
	public void UpBtnMessage()
	{
		switch( m_eDlgState )
		{			
		case eDLG_STATE.ZONE_DLG:
			CloseDlg( m_eDlgState );
			OpenDlg( eDLG_STATE.AREA_DLG, GetZoneLogic().getAreaIdx );				
			break;
			
		case eDLG_STATE.AREA_DLG:
			CloseDlg( m_eDlgState );
			OpenDlg( eDLG_STATE.WORLD_DLG, GetAreaLogic().getAreaMapIdx );					
			break;
			
		case eDLG_STATE.WAYPOINT_AREA_DLG:
			CloseDlg( m_eDlgState );
			OpenDlg( eDLG_STATE.WAYPOINT_WORLD_DLG, GetAreaLogic().getAreaMapIdx );
			break;
		}
	}
	
	
	public void ArrayBtnMessage( UIWorldBackGround.eAREAMAP_ARRAW _state )
	{
		switch( m_eDlgState )
		{				
		case eDLG_STATE.AREA_DLG:	
			GetAreaLogic().ClickArrayBtn( _state );
			break;
			
		case eDLG_STATE.WAYPOINT_AREA_DLG:	
			GetWayPointAreaLogic().ClickArrayBtn( _state );
			break;
		}
	}
	
	private void CloseBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{					
			CloseDlg( m_eDlgState );
			ShowWorldMapBtn(true);			
		}
	}	
	
	private void SetWorldBtnSwitch( int iState )
	{
		if( null == btnWorldOpen || false == btnWorldOpen.gameObject.active )
			return;
		
		if( 0 == iState )
		{
			worldImgClose.gameObject.active = false;
			worldImgOpen.gameObject.active = true;					
		}
		else
		{
			worldImgClose.gameObject.active = true;
			worldImgOpen.gameObject.active = false;		
		}
		
	}
	
	private void WorldBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{		
			if( null == worldImgClose )
				return;
			
			if( false == worldImgClose.gameObject.active )
			{
				if ( false == AsHudDlgMgr.Instance.IsOpenWorldMapDlg)
                {
					OpenDlg( eDLG_STATE.ZONE_DLG, TerrainMgr.Instance.GetCurMapID() );						
	
	                AchOpenUI openMinimap = ArkQuestmanager.instance.CheckHaveOpenUIType(OpenUIType.MINIMAP);
	
	                if (openMinimap != null)
	                {
	                    if (openMinimap.mapID == TerrainMgr.Instance.GetCurMapID())
	                        AsCommonSender.SendClearOpneUI(OpenUIType.MINIMAP);
	                }                
	                QuestTutorialMgr.Instance.ProcessQuestTutorialMsg(new QuestTutorialMsgInfo(QuestTutorialMsg.OPEN_WORLDMAP));
					
					SetWorldBtnSwitch( 1 );

					AsSoundManager.Instance.PlaySound(AsSoundPath.Open_MiniMap, Vector3.zero, false);
				}
			}
			else 
			{
                if (AsHudDlgMgr.Instance.IsOpenWorldMapDlg)
                {
                    QuestTutorialMgr.Instance.ProcessQuestTutorialMsg(new QuestTutorialMsgInfo(QuestTutorialMsg.CLOSE_WORLDMAP));
                    CloseWaypointMap(); 
					
					SetWorldBtnSwitch( 0 );

					AsSoundManager.Instance.PlaySound(AsSoundPath.Close_MiniMap, Vector3.zero, false);
                }
			}	
		}		
	}
	
	
	// input
	
	public void GuiInputDown( Ray inputRay )
	{
		switch( m_eDlgState )
		{
		case eDLG_STATE.ZONE_DLG:	
			GetZoneLogic().GuiInputDown(inputRay);
			break;
			
		case eDLG_STATE.AREA_DLG:
			GetAreaLogic().GuiInputDown(inputRay);
			break;
			
		case eDLG_STATE.WORLD_DLG:
			GetWorldLogic().GuiInputDown(inputRay);
			break;
			
		case eDLG_STATE.WAYPOINT_WORLD_DLG:
			GetWayPointWorldLogic().GuiInputDown(inputRay);
			break;
		}
	}
	
	public void GuiInputMove( Ray inputRay )
	{
		switch( m_eDlgState )
		{
		case eDLG_STATE.ZONE_DLG:	
			GetZoneLogic().GuiInputMove(inputRay);
			break;
			
		case eDLG_STATE.AREA_DLG:
			GetAreaLogic().GuiInputMove(inputRay);
			break;
			
		case eDLG_STATE.WORLD_DLG:			
			break;
		}
	}
	
	public void GuiInputUp( Ray inputRay )
	{		
		switch( m_eDlgState )
		{
		case eDLG_STATE.ZONE_DLG:
			GetZoneLogic().GuiInputUp(inputRay);
			break;
			
		case eDLG_STATE.AREA_DLG:
			GetAreaLogic().GuiInputUp(inputRay);
			break;
			
		case eDLG_STATE.WORLD_DLG:
			GetWorldLogic().GuiInputUp(inputRay);
			break;
					
		case eDLG_STATE.WAYPOINT_WORLD_DLG:
			GetWayPointWorldLogic().GuiInputUp(inputRay);
			break;
		}
	}
		
	// Use this for initialization
	void Start () 
	{
		if( null != btnWorldOpen )
		 	btnWorldOpen.SetInputDelegate(WorldBtnDelegate);		
	}
	
	// Update is called once per frame
	void Update () 
	{
		switch( m_eDlgState )
		{
		case eDLG_STATE.ZONE_DLG:				
			GetZoneLogic().Update();							
			break;
			
		case eDLG_STATE.AREA_DLG:
			GetAreaLogic().Update();
			break;
			
		case eDLG_STATE.WORLD_DLG:
			break;
			
		case eDLG_STATE.WAYPOINT_AREA_DLG:
			GetWayPointAreaLogic().Update();
			//UpdateTargetDistance();
			break;
			
		case eDLG_STATE.WAYPOINT_WORLD_DLG:
			GetWayPointWorldLogic().Update();
			//UpdateTargetDistance();
			break;
		}	
	}
	
	private void UpdateTargetDistance()
	{
		if( null == m_TargetEntity )
			return;	
		
		float distance = Vector3.Distance(m_TargetEntity.transform.position, AsEntityManager.Instance.UserEntity.transform.position);
		
		if(distance > AsNpcFsm.s_DialogReleaseDistance)
		{
			AsHudDlgMgr.Instance.CloseWaypointMapDlg();
		}	
	}

    public override void ProcessUIMessage(UIMessageObject message)
    {

    }
}
