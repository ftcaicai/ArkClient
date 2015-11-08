using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ProductionListTab : MonoBehaviour 
{
	
	public GameObject goResListItem;
	public UIScrollList listList;
	public UIPanel typePanel;
	public UIButton typeChangeBtn;
	public SpriteText textChangeName;
	public SpriteText textNeedlearnShow;	

	public UIRadioBtn accessoryBtn;
	public UIRadioBtn medicineBtn;
	public UIRadioBtn materialBtn;
	public UIRadioBtn hunterBtn;
	private List<ProductionListItem> m_listItemList = new List<ProductionListItem>();
	
	private bool m_isActiveTypeChange = false;
	private body1_SC_ITEM_PRODUCT_INFO m_InfoData;
	private eITEM_PRODUCT_TECHNIQUE_TYPE m_eCurType;
	
	public void Open( body1_SC_ITEM_PRODUCT_INFO _data )
	{
		m_InfoData = _data;
		
		SetTypeChangeActive(false);		
		SetItemListState( AsHudDlgMgr.Instance.productRadioIndex );		
	}	
	
	private void SetItemListState( eITEM_PRODUCT_TECHNIQUE_TYPE _eType, bool isInit = true )
	{
		if( null == m_InfoData )
		{
			textNeedlearnShow.Text = string.Empty;
			return;
		}
		
		int iIndex = (int)_eType;
		if(  m_InfoData.sProductInfo.Length <= iIndex )
		{
			textNeedlearnShow.Text = string.Empty;
			return;
		}
		
		if( true == isInit )
			listList.ClearList( true);
		m_listItemList.Clear();		
	
		m_eCurType = _eType;
		sPRODUCT_INFO _info = m_InfoData.sProductInfo[iIndex];
		
		eCLASS tempClass = eCLASS.All;
#if OLD_PRODUCT
		if( eITEM_PRODUCT_TECHNIQUE_TYPE.WEAPON == m_eCurType || eITEM_PRODUCT_TECHNIQUE_TYPE.ARMOR == m_eCurType )
		{			
			accessoryBtn.gameObject.SetActiveRecursively( true );
			medicineBtn.gameObject.SetActiveRecursively( true );
			materialBtn.gameObject.SetActiveRecursively( true );
			hunterBtn.gameObject.SetActiveRecursively(true);
			
			accessoryBtn.SetState( 1 );
			medicineBtn.SetState( 1 );
			materialBtn.SetState( 1 );
			hunterBtn.SetState ( 1 );
			
			tempClass = AsHudDlgMgr.productRadioClassIndex;
		}
		else
		{			
			accessoryBtn.gameObject.SetActiveRecursively( false );
			medicineBtn.gameObject.SetActiveRecursively( false );
			materialBtn.gameObject.SetActiveRecursively( false );
			hunterBtn.gameObject.SetActiveRecursively( false );
		}
#else
		accessoryBtn.gameObject.SetActiveRecursively( false );
		medicineBtn.gameObject.SetActiveRecursively( false );
		materialBtn.gameObject.SetActiveRecursively( false );
		hunterBtn.gameObject.SetActiveRecursively( false );
#endif
		
		
		
		
		for( int i=0; i<=_info.nLevel; ++i )
		{
			List<Tbl_Production_Record> _recordlist = AsTableManager.Instance.GetProductionTable().GetRecordList( m_eCurType, i );
			if( null == _recordlist )
				continue;
			
			foreach( Tbl_Production_Record record in _recordlist )	
			{
				InsertItem( tempClass, record.getItemID, record );
			}				
		}			
		
		
		if( 0 == _info.nLevel )	
		{			
			textNeedlearnShow.Text = AsTableManager.Instance.GetTbl_String(1335);						
		}
		else
		{
			textNeedlearnShow.Text = string.Empty;
		}
		
	
		
		switch( tempClass )
		{
		case eCLASS.DIVINEKNIGHT:
			accessoryBtn.SetState( 0 );
			textChangeName.Text = AsTableManager.Instance.GetTbl_String(306);
			break;
			
		case eCLASS.MAGICIAN:
			medicineBtn.SetState( 0 );
			textChangeName.Text = AsTableManager.Instance.GetTbl_String(307);
			break;	
			
		case eCLASS.CLERIC:
			materialBtn.SetState( 0 );
			textChangeName.Text = AsTableManager.Instance.GetTbl_String(308);
			break;	
			
		case eCLASS.HUNTER:
			hunterBtn.SetState( 0 );
			textChangeName.Text = AsTableManager.Instance.GetTbl_String(309);
			break;	
			
		case eCLASS.All:
			textChangeName.Text = AsTableManager.Instance.GetTbl_String(1679);
			break;				
	
		default:
			textChangeName.Text =string.Empty;
			break;
		}		
	
		listList.ScrollToItem( 0, 0.0f);      
		
	}
	
	public void Close()
	{
		listList.ClearList( true);
		m_listItemList.Clear();
	}
	
	public void InsertItem( eCLASS _eclass, int _iItemId, Tbl_Production_Record _record )
	{		
		Item _item = ItemMgr.ItemManagement.GetItem( _iItemId );
		if( null == _item )
		{
			Debug.LogError("ProductionListTab::InsertItem()[ null == item ] id : " + _iItemId );
			return;
		}
		
		if( _item.ItemData.needClass != _eclass && _eclass != eCLASS.All && _item.ItemData.needClass != eCLASS.All )
		{
			return;
		}
		
		UIListItem _uiitem = listList.CreateItem( goResListItem ) as UIListItem;		
		ProductionListItem _listItem  = _uiitem.gameObject.GetComponent<ProductionListItem>();	
		_listItem.Open( _item, _record );
		m_listItemList.Add( _listItem );
	}
	
	
	private void SetTypeChangeActive( bool isActive )
	{
		m_isActiveTypeChange = isActive;
		if( false == isActive )
		{
			typePanel.StartTransition(UIPanelManager.SHOW_MODE.BringInForward);								
		}
		else
		{
			typePanel.StartTransition(UIPanelManager.SHOW_MODE.BringInBack);		
		}
		
	}
	
	private void TypeWeaponBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{			
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			SetTypeChangeActive(false);
			
			AsHudDlgMgr.productRadioClassIndex = eCLASS.DIVINEKNIGHT;
			SetItemListState( AsHudDlgMgr.Instance.productRadioIndex );
			//SetItemListState(eITEM_PRODUCT_TECHNIQUE_TYPE.WEAPON);
		}
	}
	
	private void TypeDefenceBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{			
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			SetTypeChangeActive(false);
			AsHudDlgMgr.productRadioClassIndex = eCLASS.CLERIC;
			SetItemListState( AsHudDlgMgr.Instance.productRadioIndex );
			
			//SetItemListState(eITEM_PRODUCT_TECHNIQUE_TYPE.ARMOR);
		}
	}
	
	private void TypeAccessoryBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{			
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			SetTypeChangeActive(false);
			
			AsHudDlgMgr.productRadioClassIndex = eCLASS.MAGICIAN;
			SetItemListState( AsHudDlgMgr.Instance.productRadioIndex );
			//SetItemListState(eITEM_PRODUCT_TECHNIQUE_TYPE.ACCESSORY);
		}
	}

	private void TypeHunterBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{			
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			SetTypeChangeActive(false);
			
			AsHudDlgMgr.productRadioClassIndex = eCLASS.HUNTER;
			SetItemListState( AsHudDlgMgr.Instance.productRadioIndex );
		}
	}
	
	
	private void TypeMedicineBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{			
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			SetTypeChangeActive(false);
			SetItemListState(eITEM_PRODUCT_TECHNIQUE_TYPE.POTION);
		}
	}	
	
	
	private void TypeMaterialBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{			
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			SetTypeChangeActive(false);
			SetItemListState(eITEM_PRODUCT_TECHNIQUE_TYPE.MINERAL);
			SetItemListState(eITEM_PRODUCT_TECHNIQUE_TYPE.PLANTS, false);
			SetItemListState(eITEM_PRODUCT_TECHNIQUE_TYPE.SPIRIT, false );			
		}
	}
	
	
	
	private void TypeChangeBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{	
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			if( true == typePanel.IsTransitioning )
				return;
#if OLD_PRODUCT
			if( eITEM_PRODUCT_TECHNIQUE_TYPE.WEAPON == m_eCurType || eITEM_PRODUCT_TECHNIQUE_TYPE.ARMOR == m_eCurType )
			{			
				if( false == m_isActiveTypeChange )
					SetTypeChangeActive(true);
				else
					SetTypeChangeActive(false);
			}
#endif
		}
	}
	
	public void ResetListItemInfo()
	{
		
		foreach( ProductionListItem _data in m_listItemList )
		{
			if( null == _data )
				continue;
			
			_data.IsCheckReadyMaking();
		}
	}
	
	
	public void TransitionEndDelegate(EZTransition transition)
    {
		return;
    }
	
	void Awake()
	{
		typeChangeBtn.AddInputDelegate(TypeChangeBtnDelegate);
		typePanel.GetTransition(UIPanelManager.SHOW_MODE.BringInBack).AddTransitionEndDelegate(TransitionEndDelegate);		
		
				
		accessoryBtn.AddInputDelegate(TypeWeaponBtnDelegate);	
		medicineBtn.AddInputDelegate(TypeAccessoryBtnDelegate);
		materialBtn.AddInputDelegate(TypeDefenceBtnDelegate);
		hunterBtn.AddInputDelegate(TypeHunterBtnDelegate);		
		
		accessoryBtn.Text = AsTableManager.Instance.GetTbl_String(306);		
		medicineBtn.Text = AsTableManager.Instance.GetTbl_String(307);
		materialBtn.Text = AsTableManager.Instance.GetTbl_String(308);
		hunterBtn.Text = AsTableManager.Instance.GetTbl_String(309);
	}
	
	// Use this for initialization
	void Start () 
	{
			
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	
	//input	
	public void GuiInputDown( Ray inputRay )
	{		
		for( int i = 0; i < listList.Count; i++)
		{
			UIListItem listItem = listList.GetItem(i) as UIListItem;
			
			ProductionListItem baseItem = listItem.GetComponent<ProductionListItem>();
			if( null != baseItem )
				baseItem.GuiInputDown(inputRay);
		}	
	}	
		  
	public void GuiInputUp(Ray inputRay)
	{ 
		for( int i = 0; i < listList.Count; i++)
		{
			UIListItem listItem = listList.GetItem(i) as UIListItem;
			
			ProductionListItem baseItem = listItem.GetComponent<ProductionListItem>();
			if( null != baseItem )
				baseItem.GuiInputUp(inputRay);
		}			
	}
}
