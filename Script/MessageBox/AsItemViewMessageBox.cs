using UnityEngine;
using System.Collections;

public class AsItemViewMessageBox : AsMessageBox
{
	public delegate void eventFunction();

	public SimpleSprite itemImgPos;
	public Vector2 minusItemSize;
	public SpriteText itemName;
	public SpriteText textRandLevel;
	public eventFunction m_EventFunction = null;

	private UISlotItem m_SlotItem;
	public BoxCollider m_IconCollider;
	private Item m_Item;
	private bool m_isShowTooltip = false;

	public void SetReciveItem( int iItemID, string strPreItemID, bool isShowTooltip)
	{
		if( null != textRandLevel )
			textRandLevel.Text = string.Empty;
		
		if( 0 == iItemID)
		{
			itemName.Text = string.Empty;
			
			Item _item = ItemMgr.ItemManagement.GetItem(  AsHudDlgMgr.Instance.productionItemId );
			if( null != _item )
			{
				itemName.Text = AsTableManager.Instance.GetTbl_String(_item.ItemData.nameId);
			}			
			
			return;
		}
		
		m_Item = ItemMgr.ItemManagement.GetItem( iItemID);
		if ( null == m_Item)
			return;

		if( null != m_SlotItem)
		{
			GameObject.Destroy( m_SlotItem.gameObject);
		}

		m_isShowTooltip = isShowTooltip;
		m_SlotItem = ResourceLoad.CreateItemIcon( m_Item.GetIcon(), itemImgPos, Vector3.back, minusItemSize, false);

		if( null != strPreItemID && 0 < strPreItemID.Length)
			itemName.Text = strPreItemID;
		else
			itemName.Text = AsTableManager.Instance.GetTbl_String( m_Item.ItemData.nameId);
	}

	public bool IsIconRect( Ray inputRay)
	{
		if( null == m_IconCollider)
			return false;

		return AsUtil.PtInCollider( m_IconCollider, inputRay);
	}

	public override void OnOK()
	{
		if( null != m_EventFunction)
			m_EventFunction();

		base.OnOK();
	}

	void Update()
	{
		if( false == m_isShowTooltip)
			return;

		if ( Input.touchCount == 1 || Input.GetMouseButtonDown( 0))
		{
			Ray ray = UIManager.instance.rayCamera.ScreenPointToRay( Input.mousePosition);

#if !UNITY_EDITOR
			if ( Input.GetTouch( 0).phase != TouchPhase.Began)
				return;
#endif

			if ( m_SlotItem == null || m_Item == null)
				return;

			if ( m_IconCollider == null)
				return;

			if( true == AsUtil.PtInCollider( m_IconCollider, ray))
				TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.normal, m_Item, false, -10.0f);
		}
	}
}