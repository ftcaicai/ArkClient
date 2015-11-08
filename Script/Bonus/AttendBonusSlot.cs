using UnityEngine;
using System.Collections;

public class AttendBonusSlot : MonoBehaviour 
{
	public SimpleSprite 	m_itemImgPos;
	public Vector2 			m_minusItemSize;
	public GameObject		m_goEffectSelect;
	public SpriteText 		m_itemName;
	public SpriteText 		m_txtDay;
	
	private UISlotItem 		m_SlotItem;
	
	private Item 			m_Item;
	
	
	void Awake()
	{
		m_goEffectSelect.SetActive(false);
	}
	
	// Use this for initialization
	void Start () 
	{
	}
	
	public void SetAttendItem(int iItemID , string txtDay , Color _textColor , bool isFinish , bool isSelect , int nCount)
	{
		m_Item = ItemMgr.ItemManagement.GetItem( iItemID );
		if( m_Item == null )
		{
			Debug.LogError("AttendBonusSlot item is null. item id : " + iItemID.ToString() );
			return;
		}
		
		if( m_SlotItem != null )
		{
			GameObject.Destroy( m_SlotItem.gameObject );
		}
		
		m_SlotItem = ResourceLoad.CreateItemIcon( m_Item.GetIcon() , m_itemImgPos , Vector3.back , m_minusItemSize , false );
		
		m_SlotItem.SetItemCountText( nCount );
		
		m_txtDay.Text = txtDay;
		
		SetAttendItemProperty( _textColor , isFinish , isSelect );
	}
	
	public void SetAttendItemProperty( Color _textColor , bool isFinish , bool isSelect )
	{
		if( m_Item == null )
			return;
		
		if( isFinish == false )
		{
			if( isSelect == true )
				m_itemName.Text = AsTableManager.Instance.GetTbl_String( m_Item.ItemData.nameId );
			else
				m_itemName.Text = string.Empty;
		}
		else
		{
			m_itemName.Text = AsTableManager.Instance.GetTbl_String( 131 );
		}
		
		m_itemName.SetColor( _textColor );
		
		m_txtDay.SetColor( _textColor );
		
		m_goEffectSelect.SetActive( isSelect );
		
		if( isSelect == true )
		{
			Animation _animation = m_goEffectSelect.GetComponentInChildren<Animation>();
			
			foreach( AnimationState state in _animation)
			{
				state.wrapMode = WrapMode.Loop;
			}
		}
	}
	
	public bool GuiInputClickUp( Ray inputRay)
	{
		if( m_Item == null )
			return false;
		
		if( IsIntersect(inputRay) == false )
			return false;
		
		TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.right, m_Item.ItemID );
		
		return true;
	}
	
	public bool IsIntersect( Ray ray )
	{
		if( null == collider )
		{
			Debug.LogError("AttendBonusSlot::IsIntersect() [ null == collider ]");
			return false;
		}
		
		return AsUtil.PtInCollider( collider, ray, false );		
	}
	
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
