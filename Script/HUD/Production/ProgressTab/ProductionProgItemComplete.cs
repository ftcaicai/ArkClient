using UnityEngine;
using System.Collections;

public class ProductionProgItemComplete : ProductionProgressItem 
{
	public UIButton btnReceive;
	public SpriteText textItemName;	
	public SimpleSprite iconImgPos;
	public Vector2 minusItemSize;
	public SpriteText textShowLevel;
	
	private UISlotItem m_SlotItem;	
	private ProductionProgData m_ProgData;
	private Item m_item;
	private UISlotItem m_DownSlotItem;
	
	public void Open( byte _iIndex, ProductionProgData _progData )
	{
		SetSlot(_iIndex);
		
		if( null != textShowLevel )
			textShowLevel.Text = string.Empty;
		
		Tbl_Production_Record record = AsTableManager.Instance.GetProductionTable().GetRecord( _progData.getServerData.nRecipeIndex );
		if( null == record )
		{
			Debug.LogError("ProductionProgItemIng::Open()[ recipe index : " + _progData.getServerData.nRecipeIndex );
			return;
		}
		
		m_item = ItemMgr.ItemManagement.GetItem( record.getItemID );
		if( null == m_item )
		{
			Debug.LogError("ProductionProgItemIng::Open()[ item index : " + record.getItemID );
			return;
		}
		
		textItemName.Text = AsTableManager.Instance.GetTbl_String( m_item.ItemData.nameId );
		
		if( null != m_SlotItem )
		{
			GameObject.DestroyObject( m_SlotItem.gameObject );
		}
		
		if( false == record.IsRandItemType() )
		{
			m_SlotItem = ResourceLoad.CreateItemIcon( m_item.GetIcon(), iconImgPos, Vector3.back, minusItemSize );
			GameObject.DestroyImmediate( m_SlotItem.coolTime.gameObject);
			GameObject.DestroyImmediate( m_SlotItem.itemCountText.gameObject);
			m_SlotItem.iconImg.renderer.enabled = false;
		}
		else
		{
			if( null != textShowLevel )
				textShowLevel.Text = string.Format( AsTableManager.Instance.GetTbl_String(901), m_item.ItemData.levelLimit );
		}
		
		UIListItem _listitem = gameObject.GetComponent<UIListItem>();
		if( null != m_SlotItem && null != _listitem )
		{
			_listitem.layers[0] = m_SlotItem.iconImg;
		}
		
		m_ProgData = _progData;	
	}
	
	
	private void ReceiveBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{		
			if( false != AsHudDlgMgr.Instance.productionDlg.isOpenMessageBox )
				return;
			
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);	
			
			if( null != m_ProgData )
			{
				m_ProgData.SetReciveBtn( m_item.ItemID, getSlot );
			}		
		}
	}
	
	
	// Use this for initialization
	void Start () 
	{
		btnReceive.SetInputDelegate(ReceiveBtnDelegate);
		btnReceive.Text= AsTableManager.Instance.GetTbl_String(1321);
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	//input	
	public override void GuiInputDown( Ray inputRay )
	{		
		m_DownSlotItem = null;		
		
//		if( null != m_SlotItem && true == iconImgPos.collider.bounds.IntersectRay( inputRay ) )
		if( ( null != m_SlotItem) && ( true == AsUtil.PtInCollider( m_SlotItem.iconImg.collider, inputRay)))
		{								
			m_DownSlotItem = m_SlotItem;		
		}						
	}	
		  
	public override void GuiInputUp(Ray inputRay)
	{ 
		if( null != m_DownSlotItem && null != m_SlotItem )
		{		
//			if( true == iconImgPos.collider.bounds.IntersectRay( inputRay ) )
			if( true == AsUtil.PtInCollider( m_SlotItem.iconImg.collider, inputRay))
			{										
				AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
				TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.normal, m_item );
			}	
		}		
	}
}
