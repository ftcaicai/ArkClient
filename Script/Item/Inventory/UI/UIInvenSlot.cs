using UnityEngine;
using System.Collections;

public class UIInvenSlot : MonoBehaviour 
{
	public enum ePOS_STATE	
	{
		noraml,
		inven,
	}
	private int m_iSlotIndex = 0;
	private UISlotItem m_slotItem;	
	
	private bool m_isMoveLock = false;
	
	public GameObject m_SlotEff;
	public ePOS_STATE posState = ePOS_STATE.noraml;
	
	public void SetMoveLock( bool bLock )
	{		
		m_isMoveLock = bLock;	
		
		if( null == slotItem )
			return;
		
		if( true == m_isMoveLock )
		{
			Color color = Color.blue;
			color.a = 0.4f;
			SetColor( color );
		}	
		else
		{
			SetColor( Color.white );		
		}
	}
	
	protected void SetColor( Color _color )
	{
		if( null != slotItem.iconImg )
				slotItem.iconImg.gameObject.renderer.material.SetColor("_Color", _color );
	}
	
	public void SetNewImg( bool isActive )
	{
		if( false == isActive )
			return;
		
		if( null == m_slotItem || null == m_slotItem.iconImg )
			return;		
		
		ResourceLoad.CreateUI( "UI/AsGUI/GUI_NewImg", m_slotItem.iconImg.transform, new Vector3(0f, 1f, -0.1f ) );
	}
	
	public void SetEquipImg()
	{		
		if( null == m_slotItem || null == m_slotItem.iconImg )
			return;	
		
		if( m_slotItem.realItem.item.ItemData.GetItemType() != Item.eITEM_TYPE.EquipItem )
			return;
		
		if( null == AsUserInfo.Instance.GetCurrentUserEntity() )
			return;
		
		if( true == AsUserInfo.Instance.GetCurrentUserEntity().IsCheckEquipEnable( m_slotItem.realItem.item ) )
			return;
		
		
		eCLASS __class = AsUserInfo.Instance.GetCurrentUserEntity().GetProperty<eCLASS>( eComponentProperty.CLASS);

		if( __class != m_slotItem.realItem.item.ItemData.needClass && eCLASS.All != m_slotItem.realItem.item.ItemData.needClass)
		{
			bool isNotClassImg = true;

			if(  m_slotItem.realItem.item.ItemData.needClass == eCLASS.PET )
			{
				isNotClassImg = false;
				if( AsPetManager.Instance.CheckPetEquipEnable( m_slotItem.realItem) == false )
					isNotClassImg = true;
			}

			if( isNotClassImg == true )
				ResourceLoad.CreateUI( "UI/AsGUI/GUI_NotClassImg", m_slotItem.iconImg.transform, new Vector3(0f, 0f, -0.1f ) );
		}
		else
		{
			int iCurLevel = AsUserInfo.Instance.GetCurrentUserEntity().GetProperty<int>( eComponentProperty.LEVEL);
			if( m_slotItem.realItem.item.ItemData.levelLimit > iCurLevel)
			{
				ResourceLoad.CreateUI( "UI/AsGUI/GUI_NotUseImg", m_slotItem.iconImg.transform, new Vector3(0f, 0f, -0.1f ) );
			}
			else
			{
				ResourceLoad.CreateUI( "UI/AsGUI/GUI_NotClassImg", m_slotItem.iconImg.transform, new Vector3(0f, 0f, -0.1f ) );
			}
		}	
	}
	
	public bool isMoveLock
	{
		get
		{
			return m_isMoveLock;
		}
	}

	
	// UISlotItem	
	public UISlotItem slotItem
	{
		get
		{
			return m_slotItem;
		}
	}	
	
	public void SetSlotItem( UISlotItem slotItem )
	{
		m_slotItem = slotItem;		
	}
	
	
	public void ResetSlotItemPosition()
	{
		if(null==slotItem)
		{		
			Debug.LogError("InvenSlot::ResetPosition() [ null == m_slotItem ]");
			return;
		}
		
		Vector3 vec3Temp = transform.position;
		vec3Temp.z -= 1.0f;
		slotItem.transform.position = vec3Temp;
	}
	
	public void ResetSlotItemLocalPosition( float fZPos )
	{
		/*if(null==slotItem)
		{		
			Debug.LogError("InvenSlot::ResetPosition() [ null == m_slotItem ]");
			return;
		}
		
		if( null == slotItem.iconImg )
			return;
		
		Vector3 vec3Temp = slotItem.iconImg.transform.localPosition;
		vec3Temp.z = fZPos;
		slotItem.iconImg.transform.localPosition = vec3Temp;*/
	}
	
	public void SetSlotItemPosition( Vector3 vec3Position )
	{
		if(null==slotItem)
		{		
			Debug.LogError("InvenSlot::MoveSlotItem() [ null == m_slotItem ]");
			return;
		}

		slotItem.transform.position = vec3Position;
	}
	
	
	// collder 
	
	public bool IsIntersect( Ray ray )
	{
		if( null == collider )
		{
			Debug.LogError("InvenSlot::IsIntersect() [ null == collider ]");
			return false;
		}
		
		return AsUtil.PtInCollider( collider, ray, false );		
	}
	
	
	// slot index 
	
	public void SetSlotIndex( int iSlotIndex ) 
	{
		m_iSlotIndex = iSlotIndex;
	}
	
	public int slotIndex
	{
		get	{ return m_iSlotIndex; }
	}
	
	
	// GameObject 
	
	
	public void DeleteSlotItem()
	{		
		if( null == slotItem )
			return;
		
		DestroyImmediate( slotItem.gameObject );	
		SetSlotItem( null );	
	}
	
	public bool CreateSlotItem( RealItem realItem, Transform trmParent )
	{
		if( null == realItem  || realItem.item == null)
		{
			Debug.LogError("UIInvenSlot::CreateSlotItem() [ null == realItem ]");
			return false;		
		}
		
		GameObject resGo = realItem.item.GetIcon();
		if( null == resGo )
		{
			Debug.LogError("UIInvenSlot::CreateSlotItem() [ null == resGo ] item id : " + realItem.item.ItemID );
			return false;
		}
		
		GameObject go = GameObject.Instantiate( resGo ) as GameObject;
		go.transform.parent = trmParent;
		go.transform.localPosition = Vector3.zero;
		go.transform.localRotation = Quaternion.identity;
		go.transform.localScale = Vector3.one;
		
		
		UISlotItem slotItem = go.GetComponent<UISlotItem>();
		if( null == slotItem )
		{				
			Debug.LogError("UIInvelSlot::CreateSlotItem() [ null == slotItem");
			Destroy(go);	
			return false;
		}
			
		slotItem.SetItem( realItem );
		SetSlotItem( slotItem );
		ResetSlotItemPosition();
		
		if( ePOS_STATE.inven ==  posState )
		{
			SetEquipImg();			
		}
		
		CheckPvpIconColor();
		CheckRaidIconColor();
		CheckFieldIconColor();
		CheckIndunIconColor();
		return true;
	}
	
	public void CheckPvpIconColor()
	{
		if( null == slotItem )
			return;
		
		if( null == slotItem.realItem )
			return;
		
		Item _item = slotItem.realItem.item;
		
		if( null == _item )
			return;
		
		if( false == TargetDecider.CheckCurrentMapIsArena() )
			return;
		
		if( _item.ItemData.GetItemType() != Item.eITEM_TYPE.ActionItem )
			return;
			
		bool isDisableSkill_PvP = false;
		Tbl_Skill_Record skillrecord = AsTableManager.Instance.GetTbl_Skill_Record( _item.ItemData.itemSkill);
		if( null != skillrecord)
		{
			isDisableSkill_PvP = (skillrecord.DisableInPvP == eDisableInPvP.Disable);		
		}
		
		if( true == isDisableSkill_PvP  )
		{
			Color color = Color.red;
			color.a = 0.4f;			
			SetColor( color );
		}
		else
		{
			SetColor( Color.white );
		}
		
		
	}
	
	
	public void CheckRaidIconColor()
	{
		if( null == slotItem )
			return;
		
		if( null == slotItem.realItem )
			return;
		
		Item _item = slotItem.realItem.item;
		
		if( null == _item )
			return;
		
		if( false == TargetDecider.CheckCurrentMapIsRaid() )
			return;
		
		if( _item.ItemData.GetItemType() != Item.eITEM_TYPE.ActionItem )
			return;
			
		
		bool isDisableSkill_Raid = false;
		Tbl_Skill_Record skillrecord = AsTableManager.Instance.GetTbl_Skill_Record( _item.ItemData.itemSkill);
		if( null != skillrecord)
		{
			
			isDisableSkill_Raid = (skillrecord.DisableInRaid == eDisableInRaid.Disable);
		}
		
		if( true == isDisableSkill_Raid )
		{
			Color color = Color.red;
			color.a = 0.4f;			
			SetColor( color );
		}
		else if( false == isMoveLock )
		{
			SetColor( Color.white );
		}		
		
	}
	
	public void CheckFieldIconColor()
	{
		if( null == slotItem )
			return;
		
		if( null == slotItem.realItem )
			return;
		
		Item _item = slotItem.realItem.item;
		
		if( null == _item )
			return;
		
		if( false == TargetDecider.CheckCurrentMapIsField() )
			return;
		
		if( _item.ItemData.GetItemType() != Item.eITEM_TYPE.ActionItem )
			return;
			
		
		bool isDisableSkill_Raid = false;
		Tbl_Skill_Record skillrecord = AsTableManager.Instance.GetTbl_Skill_Record( _item.ItemData.itemSkill);
		if( null != skillrecord)
		{
			
			isDisableSkill_Raid = (skillrecord.DisableInField == eDisableInRaid.Disable);
		}
		
		if( true == isDisableSkill_Raid )
		{
			Color color = Color.red;
			color.a = 0.4f;			
			SetColor( color );
		}
		else if( false == isMoveLock )
		{
			SetColor( Color.white );
		}		
		
	}
	
	public void CheckIndunIconColor()
	{
		if( null == slotItem )
			return;
		
		if( null == slotItem.realItem )
			return;
		
		Item _item = slotItem.realItem.item;
		
		if( null == _item )
			return;
		
		if( false == TargetDecider.CheckCurrentMapIsIndun() )
			return;
		
		if( _item.ItemData.GetItemType() != Item.eITEM_TYPE.ActionItem )
			return;
		
		bool isDisableSkill_InDun = false;
		Tbl_Skill_Record skillrecord = AsTableManager.Instance.GetTbl_Skill_Record( _item.ItemData.itemSkill);
		if( null != skillrecord)
		{
			isDisableSkill_InDun = (skillrecord.DisableInInDun == eDisableInInDun.Disable);		
		}
		
		if( true == isDisableSkill_InDun  )
		{
			Color color = Color.red;
			color.a = 0.4f;			
			SetColor( color );
		}
		else
		{
			SetColor( Color.white );
		}
		
		
	}

	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
	}
}
