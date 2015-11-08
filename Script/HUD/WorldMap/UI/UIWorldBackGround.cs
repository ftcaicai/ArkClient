using UnityEngine;
using System.Collections;


public class UIWorldBackGround : MonoBehaviour 
{		
	public enum eAREAMAP_ARRAW
	{
		left,
		right,
		up,
		down,
	}
	public SpriteText textTitle;
	public UIButton btnUpState;	
	public UIButton btnClose;	
	public UIButton btnMapTouch;
	public UIButton btnGoWorldMap;
	public UIButton btnGoWaypointMap;
	public UIButton btnSetting;
	
	public UIButton btnLeft;
	public UIButton btnRight;
	public UIButton btnUp;
	public UIButton btnDown;
	
		
	
	public SimpleSprite[] partyImgList;
	public SpriteText[] partyTextList;
	
	//public Collider collierBox;	
	public GameObject dlgPos;

    //size
    public GameObject worldmapPos;
    private float m_fLeftMove = 7.9f;
    private Vector3 m_initTitlePos;
    private Vector3 m_initClosePos;
    private Vector3 m_initWorldMapPos;
    private bool m_isMapSizeBig = true;
   

    public void SetMapSize(bool _isBig)
    {
        if ( _isBig == m_isMapSizeBig )
            return;

        if (true == _isBig)
        {
            worldmapPos.transform.localScale = Vector3.one;
            btnClose.transform.localPosition = m_initClosePos;
            textTitle.transform.localPosition = m_initTitlePos;
            worldmapPos.transform.localPosition = m_initWorldMapPos;
        }
        else
        {
            worldmapPos.transform.localScale = new Vector3( 0.5f, 0.5f, 1f );
           	btnClose.transform.localPosition = new Vector3(8.6f, m_initClosePos.y, m_initClosePos.z);
           	textTitle.transform.localPosition = new Vector3(m_initTitlePos.x - (m_fLeftMove*0.25f), m_initTitlePos.y, m_initTitlePos.z);
            worldmapPos.transform.localPosition = new Vector3(m_initWorldMapPos.x + m_fLeftMove, m_initWorldMapPos.y-3.25f, m_initWorldMapPos.z);
        }
		m_isMapSizeBig = _isBig;
    }
	
	public void SetViewBtnSetting( bool _isView )
	{
		if( btnSetting.gameObject.active == _isView ) 
			return;
		
		btnSetting.gameObject.SetActiveRecursively( _isView );
	}
	
	
	public void SetAreaMapArrawBtn( eAREAMAP_ARRAW _type, bool isView )
	{
		switch( _type )
		{
		case eAREAMAP_ARRAW.left:
			btnLeft.gameObject.SetActiveRecursively( isView );
			break;
			
		case eAREAMAP_ARRAW.right:
			btnRight.gameObject.SetActiveRecursively( isView );
			break;
			
		case eAREAMAP_ARRAW.up:
			btnUp.gameObject.SetActiveRecursively( isView );
			break;
			
		case eAREAMAP_ARRAW.down:
			btnDown.gameObject.SetActiveRecursively( isView );
			break;
		}
	}
	
	public void SetActiveBtnMapTouch( bool isActive )
	{
        if (true == TerrainMgr.Instance.IsCurMapType(eMAP_TYPE.Pvp))
            isActive = false;
		
        if (true == TerrainMgr.Instance.IsCurMapType(eMAP_TYPE.Indun))
            isActive = false;
		
		if( false == isActive )
		{
			btnMapTouch.Text = string.Empty;
		}
		else
		{
			SetMapTouchText( AsTableManager.Instance.GetTbl_String( 868 ) );
		}
		btnMapTouch.gameObject.active = isActive;
	}
	
	
	public void SetMapTouchText( string str )
	{
		btnMapTouch.Text = str;
	}
	
	
	
	public void SetHidePartyName()
	{
		foreach( SimpleSprite _sprite in partyImgList )
		{
			_sprite.gameObject.SetActiveRecursively(false);
		}
	}
	/*public void SetEnableCollider( bool isEnable )
	{
		if( null == collierBox )
			return;
		
		collierBox.enabled = isEnable;
	}
	
	public bool IsRect( Ray _ray )
	{
		if( null == collierBox )
			return false;
		
		return AsUtil.PtInCollider( collierBox, _ray );		
	}*/
	
	/*public void SetShowPlayerPosition( bool bShow )
	{
		textPlayerPos.gameObject.active = bShow;
	}
	
	public void SetShowPlayerPositionText( bool isShow )
	{
		textPlayerPos.gameObject.active = isShow;
	}
	
	public void SetPlayerPosition( int x, int y )
	{
		if( false == textPlayerPos.gameObject.active )
			return;
		
		textPlayerPos.Text = " x : " + x.ToString() + " y : " + y.ToString();
	}*/
	
	public void SetPartyName( int iIndex, string strName )
	{
		if( null == strName || 1 > strName.Length || 
			iIndex >= partyImgList.Length || iIndex >= partyTextList.Length )
		{
			return;			
		}
		
		//SimpleSprite _sprite = partyImgList[iIndex];		
			
		partyImgList[iIndex].gameObject.SetActiveRecursively(true);
		partyTextList[iIndex].Text = strName;		
	}
	
	
	public void SetUpStateBtnShow( bool bShow )
	{
		if( null == btnUpState )
			return;

        if (true == TerrainMgr.Instance.IsCurMapType(eMAP_TYPE.Pvp))
            bShow = false;
		
        if (true == TerrainMgr.Instance.IsCurMapType(eMAP_TYPE.Indun))
            bShow = false;

		if( false == bShow )
		{
			btnUpState.Text = string.Empty;
			btnUpState.gameObject.active = false;
			return;
		}

        
		
		btnUpState.Text = AsTableManager.Instance.GetTbl_String( 869 );		
		btnUpState.gameObject.active = bShow;
	}
	
	public void SetTitleName( string strName )
	{
		if( null == textTitle )
			return;
		
		textTitle.Text = strName;
	}
	
	
	
	
	private void UpStateBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if( AsHudDlgMgr.Instance.IsOpenWorldMapDlg )
				AsHudDlgMgr.Instance.worldMapDlg.UpBtnMessage();
			
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		}
	}
	
	private void CloseBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if( AsHudDlgMgr.Instance.IsOpenWorldMapDlg )
			{
				AsHudDlgMgr.Instance.worldMapDlg.OpenWorldMap();
			}

            QuestTutorialMgr.Instance.ProcessQuestTutorialMsg(new QuestTutorialMsgInfo(QuestTutorialMsg.CLOSE_WORLDMAP));
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		}
	}
	
	
	private void MapTouchBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
            if (AsHudDlgMgr.Instance.IsOpenWorldMapDlg)
            {
                AsHudDlgMgr.Instance.worldMapDlg.MapTouch();

                if (ArkQuestmanager.instance.CheckHaveOpenUIType(OpenUIType.OPEN_LOCATION_MAP) != null)
                    AsCommonSender.SendClearOpneUI(OpenUIType.OPEN_LOCATION_MAP);
            }
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		}
	}
	
	private void StateChangeBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if( false == AsHudDlgMgr.Instance.IsOpenWorldMapDlg )
				return;
			
			if( true == AsHudDlgMgr.Instance.worldMapDlg.IsCurStateWaypointDlg() )
			{
				AsHudDlgMgr.Instance.worldMapDlg.CloseWaypointMap();
				AsHudDlgMgr.Instance.worldMapDlg.OpenDlg( WorldMapDlg.eDLG_STATE.ZONE_DLG, TerrainMgr.Instance.GetCurMapID() );	
			}
			else
			{
				AsHudDlgMgr.Instance.worldMapDlg.CloseWaypointMap();
				AsHudDlgMgr.Instance.OpenWaypointMapDlg();
			}
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		}
	}
	
	private void SettingBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			
			if( true == AsHudDlgMgr.Instance.isOpenWorldMapSettingDlg )
			{
				AsHudDlgMgr.Instance.CloseWorldMapSettingDlg();
				return;
			}
			
			AsHudDlgMgr.Instance.OpenWorldMapSettingDlg();
		}
	}
	
	
	
	private void leftBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if( AsHudDlgMgr.Instance.IsOpenWorldMapDlg )
				AsHudDlgMgr.Instance.worldMapDlg.ArrayBtnMessage(eAREAMAP_ARRAW.left );
			
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		}
	}
	
	private void rightBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if( AsHudDlgMgr.Instance.IsOpenWorldMapDlg )
				AsHudDlgMgr.Instance.worldMapDlg.ArrayBtnMessage(eAREAMAP_ARRAW.right );
			
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		}
	}
	
	private void upBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if( AsHudDlgMgr.Instance.IsOpenWorldMapDlg )
				AsHudDlgMgr.Instance.worldMapDlg.ArrayBtnMessage(eAREAMAP_ARRAW.up );
			
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		}
	}
	
	private void downBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if( AsHudDlgMgr.Instance.IsOpenWorldMapDlg )
				AsHudDlgMgr.Instance.worldMapDlg.ArrayBtnMessage(eAREAMAP_ARRAW.down );
			
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		}
	}

    void Awake()
    {
        m_initWorldMapPos = worldmapPos.transform.localPosition;
        m_initTitlePos = textTitle.transform.localPosition;
        m_initClosePos = btnClose.transform.localPosition;
    }

	// Use this for initialization
	void Start () 
	{
		btnUpState.SetInputDelegate(UpStateBtnDelegate);
		btnClose.SetInputDelegate(CloseBtnDelegate);
		btnMapTouch.SetInputDelegate(MapTouchBtnDelegate);
		btnGoWorldMap.SetInputDelegate(StateChangeBtnDelegate);
		
		
		btnSetting.SetInputDelegate(SettingBtnDelegate);
		btnSetting.Text = AsTableManager.Instance.GetTbl_String(996);		
		btnGoWorldMap.Text = AsTableManager.Instance.GetTbl_String(867);
		
		
		btnLeft.SetInputDelegate(leftBtnDelegate);
		btnRight.SetInputDelegate(rightBtnDelegate);
		btnUp.SetInputDelegate(upBtnDelegate);
		btnDown.SetInputDelegate(downBtnDelegate);
		
		
		btnGoWaypointMap.SetInputDelegate(StateChangeBtnDelegate);
		btnGoWaypointMap.Text = AsTableManager.Instance.GetTbl_String(866);
		
		if( AsHudDlgMgr.Instance.IsOpenWorldMapDlg )
		{
			if( true == AsHudDlgMgr.Instance.worldMapDlg.IsCurStateWaypointDlg() )
			{
				btnGoWorldMap.gameObject.SetActiveRecursively(true);
				//btnGoWaypointMap.gameObject.SetActiveRecursively(false);
				
			}
			else
			{
				btnGoWorldMap.gameObject.SetActiveRecursively(false);
				//btnGoWaypointMap.gameObject.SetActiveRecursively(true);
			}
		}
		btnGoWaypointMap.gameObject.SetActiveRecursively(false);
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
