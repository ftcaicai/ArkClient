using UnityEngine;
using System.Collections;

public class AsSelectAreaDlg : MonoBehaviour
{
	public const string WORLDPATH = "UI/AsGUI/Worldmap/Map_World";
	public GameObject dlgPos;
	public UIButton btnLeft;
	public UIButton btnRight;
	public UIButton btnUp;
	public UIButton btnDown;

	private UIWorldMap m_uiWorldMap;
	private UIAreaMap m_uiAreaMap;

	public SpriteText m_TextTitle = null;
	public UIButton btnWorldMap;
	public UIButton m_CloseBtn;
	
	int m_iCurrentAreaIdx;
	
	protected int m_iDownIndex = 0;
	protected int iUpIndex = -1;
	protected int iDownIndex = -1;
	protected int iRightIndex = -1;
	protected int iLeftIndex = -1;
	int m_iAreaIdx;

	// Use this for initialization
	void Start()
	{
		m_CloseBtn.SetInputDelegate( CloseBtnDelegate);
		btnWorldMap.SetInputDelegate( WorldMapBtnDelegate);
		btnWorldMap.Text = AsTableManager.Instance.GetTbl_String(869);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextTitle);
		m_TextTitle.Text = AsTableManager.Instance.GetTbl_String(2015);

		btnLeft.SetInputDelegate( leftBtnDelegate);
		btnRight.SetInputDelegate( rightBtnDelegate);
		btnUp.SetInputDelegate( upBtnDelegate);
		btnDown.SetInputDelegate( downBtnDelegate);
	}

	// Update is called once per frame
	void Update()
	{
		if( null == m_uiWorldMap)
			return;

#if ( UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN)
		if( true == Input.GetMouseButtonDown( 0))
		{
			Ray ray1 = UIManager.instance.rayCamera.ScreenPointToRay( Input.mousePosition);
			m_iDownIndex = GetColliderIndex( ray1);
		}
		else if( Input.GetMouseButtonUp( 0) == true)
		{
			Ray ray1 = UIManager.instance.rayCamera.ScreenPointToRay( Input.mousePosition);
			if( m_iDownIndex != 0 && m_iDownIndex == GetColliderIndex( ray1))
				CreateByAreaMapRecord( m_iDownIndex);
		}
#else
		if( 0 < Input.touchCount)
		{
			Touch touch = Input.GetTouch( 0);
			Ray ray1 = UIManager.instance.rayCamera.ScreenPointToRay( touch.position);
			if( touch.phase == TouchPhase.Began)
			{
				 m_iDownIndex = GetColliderIndex( ray1);
			}
			else if( touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
			{
				Ray ray = UIManager.instance.rayCamera.ScreenPointToRay( Input.mousePosition);
				if( m_iDownIndex !=0 && m_iDownIndex == GetColliderIndex( ray1))
					CreateByAreaMapRecord( m_iDownIndex);
			}
		}
#endif
	}

	public void Open()
	{
		CreateAreaMap( TerrainMgr.Instance.GetCurMapID());
		gameObject.SetActiveRecursively( true);
		btnWorldMap.gameObject.SetActiveRecursively( false);
		m_iCurrentAreaIdx = TerrainMgr.Instance.GetCurMapID();
		CheckArrayState();
	}

	public void SetAreaMapArrawBtn( UIWorldBackGround.eAREAMAP_ARRAW _type, bool isView)
	{
		switch( _type)
		{
		case UIWorldBackGround.eAREAMAP_ARRAW.left:
			btnLeft.gameObject.SetActiveRecursively( isView);
			break;
		case UIWorldBackGround.eAREAMAP_ARRAW.right:
			btnRight.gameObject.SetActiveRecursively( isView);
			break;
		case UIWorldBackGround.eAREAMAP_ARRAW.up:
			btnUp.gameObject.SetActiveRecursively( isView);
			break;
		case UIWorldBackGround.eAREAMAP_ARRAW.down:
			btnDown.gameObject.SetActiveRecursively( isView);
			break;
		}
	}

	private bool IsEnableAreaMap( int iIndex)
	{
		Tbl_AreaMap_Record _record = AsTableManager.Instance.GetAreaMapRecord( iIndex);
		if( null == _record)
			return false;

		return _record.isActive;
	}

	public void CheckArrayState()
	{
		iUpIndex = m_iAreaIdx - 4;
		iDownIndex = m_iAreaIdx + 4;
		iRightIndex = m_iAreaIdx + 1;
		iLeftIndex = m_iAreaIdx - 1;

		if( iUpIndex >= 1 && iUpIndex <= 8)
		{
			if( IsEnableAreaMap( iUpIndex))
				SetAreaMapArrawBtn( UIWorldBackGround.eAREAMAP_ARRAW.up, true);
			else
				SetAreaMapArrawBtn( UIWorldBackGround.eAREAMAP_ARRAW.up, false);
		}
		else
			SetAreaMapArrawBtn( UIWorldBackGround.eAREAMAP_ARRAW.up, false);

		if( iDownIndex >= 1 && iDownIndex <= 8)
		{
			if( IsEnableAreaMap( iDownIndex))
				SetAreaMapArrawBtn( UIWorldBackGround.eAREAMAP_ARRAW.down, true);
			else
				SetAreaMapArrawBtn( UIWorldBackGround.eAREAMAP_ARRAW.down, false);
		}
		else
			SetAreaMapArrawBtn( UIWorldBackGround.eAREAMAP_ARRAW.down, false);

		if( iRightIndex >= 1 && iRightIndex <= 8)
		{
			if( IsEnableAreaMap( iRightIndex))
				SetAreaMapArrawBtn( UIWorldBackGround.eAREAMAP_ARRAW.right, true);
			else
				SetAreaMapArrawBtn( UIWorldBackGround.eAREAMAP_ARRAW.right, false);
		}
		else
			SetAreaMapArrawBtn( UIWorldBackGround.eAREAMAP_ARRAW.right, false);

		if( iLeftIndex >= 1 && iLeftIndex <= 8)
		{
			if( IsEnableAreaMap( iLeftIndex))
				SetAreaMapArrawBtn( UIWorldBackGround.eAREAMAP_ARRAW.left, true);
			else
				SetAreaMapArrawBtn( UIWorldBackGround.eAREAMAP_ARRAW.left, false);
		}
		else
			SetAreaMapArrawBtn( UIWorldBackGround.eAREAMAP_ARRAW.left, false);
	}

	public void CreateByAreaMapRecord( int _AreaMapIdx)
	{
		DestroyAreaMap();
		DestroyWorldMap();

		Tbl_AreaMap_Record areaMapRecord = AsTableManager.Instance.GetAreaMapRecord( _AreaMapIdx);
		if( null == areaMapRecord)
		{
			AsUtil.ShutDown( "WayPointAreaLogic::Open()[ null == Tbl_AreaMap_Record ] area index : " + _AreaMapIdx);
			return;
		}

		CreateAreaMap( areaMapRecord.getStrImgPath, TerrainMgr.Instance.GetCurMapID());
	}

	public void CreateAreaMap( int _ZoneMapIdx)
	{
		Tbl_ZoneMap_Record record = AsTableManager.Instance.GetZoneMapRecord( _ZoneMapIdx);
		if( null == record)
		{
			AsUtil.ShutDown( "WorldMapDlg::OpenWaypointMap()[ null == Tbl_ZoneMap_Record ] id : " + _ZoneMapIdx);
			return;
		}

		CreateAreaMapAreaIndex( record.getAreaMapIdx);
	}

	public void CreateAreaMapAreaIndex( int _AreaIdx)
	{
		DestroyAreaMap();
		DestroyWorldMap();

		Tbl_AreaMap_Record areaMapRecord = AsTableManager.Instance.GetAreaMapRecord( _AreaIdx);
		if( null == areaMapRecord)
		{
			AsUtil.ShutDown( "WayPointAreaLogic::Open()[ null == Tbl_AreaMap_Record ] area index : " + _AreaIdx);
			return;
		}

		CreateAreaMap( areaMapRecord.getStrImgPath, TerrainMgr.Instance.GetCurMapID());

		m_iAreaIdx = areaMapRecord.Index;
	}

	public void CreateAreaMap( string strPath, int iCurMapIdx)
	{
		if( null == m_uiAreaMap)
		{
			GameObject goInstance = ResourceLoad.CreateGameObject( strPath, dlgPos.transform);

			m_uiAreaMap = goInstance.GetComponent<UIAreaMap>();
			if( null == m_uiAreaMap)
			{
				AsUtil.ShutDown( "WorldMapDlg::OpenAreaMap()[no find m_uiAreaMap] path : " + strPath);
				GameObject.DestroyObject( goInstance);
				return;
			}
			m_uiAreaMap.SetAreaMapClickDelegate( AsPartyManager.Instance.PartyUI.AreaMapBtnClick);
		}

		m_uiAreaMap.Open( iCurMapIdx);
	}

	public void CreateWorldMap()
	{
		DestroyAreaMap();
		DestroyWorldMap();
		if( null == m_uiWorldMap)
		{
			GameObject goInstance = ResourceLoad.CreateGameObject( WORLDPATH, dlgPos.transform);

			m_uiWorldMap = goInstance.GetComponent<UIWorldMap>();
			if( null == m_uiWorldMap)
			{
				AsUtil.ShutDown( "WorldMapDlg::OpenWorldMap()[no find UIWorldMap] path : " + WORLDPATH);
				GameObject.DestroyObject( goInstance);
				return;
			}

			m_uiWorldMap.Open();
		}
	}

	private void DestroyAreaMap()
	{
		if( null == m_uiAreaMap)
			return;

		GameObject.DestroyObject( m_uiAreaMap.gameObject);
		m_uiAreaMap = null;
	}

	private void DestroyWorldMap()
	{
		if( null == m_uiWorldMap)
			return;

		GameObject.DestroyObject( m_uiWorldMap.gameObject);
		m_uiWorldMap = null;
	}

	public void Close()
	{
		gameObject.SetActiveRecursively( false);
		gameObject.active = false;
	}

	public void SetFocusZoneMap( int iMapid)
	{
		if( null == m_uiAreaMap)
			return;
		m_iCurrentAreaIdx = iMapid;
		m_uiAreaMap.SetFocusZoneMap( iMapid) ;
	}

	private void CloseBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			Close();
		}
	}

	private void WorldMapBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			CreateWorldMap();
		}
	}

	public int GetColliderIndex( Ray inputRay)
	{
		for( int i = 0; i < m_uiWorldMap.worldBtnList.Length; ++i)
		{
			if( m_uiWorldMap.worldBtnList[i].IsRect( inputRay))
				return m_uiWorldMap.worldBtnList[i].areaIndex;
		}

		return 0;
	}

	public virtual void ClickArrayBtn( UIWorldBackGround.eAREAMAP_ARRAW _state)
	{
		switch( _state)
		{
		case UIWorldBackGround.eAREAMAP_ARRAW.left:
			if( -1 != iLeftIndex)
			{
				CreateAreaMapAreaIndex( iLeftIndex);
				CheckArrayState();
			}
			break;
		case UIWorldBackGround.eAREAMAP_ARRAW.right:
			if( -1 != iRightIndex)
			{
				CreateAreaMapAreaIndex( iRightIndex);
				CheckArrayState();
			}
			break;
		case UIWorldBackGround.eAREAMAP_ARRAW.up:
			if( -1 != iUpIndex)
			{
				CreateAreaMapAreaIndex( iUpIndex);
				CheckArrayState();
			}
			break;
		case UIWorldBackGround.eAREAMAP_ARRAW.down:
			if( -1 != iDownIndex)
			{
				CreateAreaMapAreaIndex( iDownIndex);
				CheckArrayState();
			}
			break;
		}
		
		SetFocusZoneMap(m_iCurrentAreaIdx);
	}

	private void leftBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			ClickArrayBtn( UIWorldBackGround.eAREAMAP_ARRAW.left);
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		}
	}

	private void rightBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			ClickArrayBtn( UIWorldBackGround.eAREAMAP_ARRAW.right);
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		}
	}

	private void upBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			ClickArrayBtn( UIWorldBackGround.eAREAMAP_ARRAW.up);
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		}
	}

	private void downBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			ClickArrayBtn( UIWorldBackGround.eAREAMAP_ARRAW.down);
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		}
	}
}