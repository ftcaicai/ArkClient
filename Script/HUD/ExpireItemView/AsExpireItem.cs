using UnityEngine;
using System.Collections;

public class AsExpireItem : MonoBehaviour 
{
	public UIButton btnList;
	public SpriteText textItemName;
	public SimpleSprite iconImgPos;
	
	private sITEM m_sItem = null;
	private Item m_item = null;
	private UISlotItem m_SlotItem;	
	public Vector2 minusItemSize;
	
//	float fOffsetToolTip = -3.0f;
	
	public void Open( sITEM _sitem )
	{
		m_sItem = _sitem;
		
		if( null == m_sItem )
		{
			Debug.LogError("AsExpireItem::Open()[ null == sITEM ) ");
			return;
		}
		
		m_item = ItemMgr.ItemManagement.GetItem( m_sItem.nItemTableIdx );
		
		if( null == m_item )
		{
			Debug.LogError("AsExpireItem::Open()[ null == Item ) id: " + m_sItem.nItemTableIdx );
			return;
		}
		
		if( null != textItemName )
			textItemName.Text = AsTableManager.Instance.GetTbl_String( m_item.ItemData.nameId );	
		
		
		if( null != m_SlotItem )
		{
			GameObject.DestroyObject( m_SlotItem.gameObject );
		}
		
		m_SlotItem = ResourceLoad.CreateItemIcon( m_item.GetIcon(), iconImgPos, Vector3.back, minusItemSize, false );		
		UIListItem _listitem = gameObject.GetComponent<UIListItem>();		
		
		if( null != m_SlotItem && null != _listitem )
		{
			m_SlotItem.iconImg.renderer.enabled = false;
			_listitem.layers[0] = m_SlotItem.iconImg;
			m_SlotItem.slotType = UISlotItem.eSLOT_TYPE.NO_COOLTIME;
		}		
	}
	
	// Use this for initialization
	void Start () 
	{
		btnList.SetInputDelegate(ClickBtnDelegate);
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	private void ClickBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{				
			if( null != TooltipMgr.Instance.tooltipOnePosition )
			{
				if( TooltipMgr.Instance.tooltipOnePosition.isOpen )
					return;
			}
			if( null == m_sItem )
				return;
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);			
			TooltipMgr.Instance.OpenTooltip(TooltipMgr.eOPEN_DLG.normal, m_sItem, 0f);
		}
	}
}
