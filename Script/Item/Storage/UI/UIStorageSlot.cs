using UnityEngine;
using System.Collections;

public class UIStorageSlot : MonoBehaviour 
{
	private int m_iSlotIndex = 0;
	private UISlotItem m_slotItem;	
	
	private bool m_isMoveLock = false;
	
	public void SetMoveLock( bool bLock )
	{		
		m_isMoveLock = bLock;	
		
		if( null == slotItem )
			return;
		
		if( true == m_isMoveLock )
		{
			Color color = Color.blue;
			color.a = 0.4f;
			if( null != slotItem.iconImg )
				slotItem.iconImg.gameObject.renderer.material.SetColor("_Color", color );
		}	
		else
		{
			if( null != slotItem.iconImg )
				slotItem.iconImg.gameObject.renderer.material.SetColor("_Color", Color.white );
			
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
	
	public void SetSlotItemPosition( Vector3 vec3Position )
	{
		if(null==slotItem)
		{		
			Debug.LogError("InvenSlot::MoveSlotItem() [ null == m_slotItem ]");
			return;
		}
		
//		Vector3 vec3Temp = Vector3.zero;
//		vec3Temp.x = vec3Position.x;
//		vec3Temp.y = vec3Position.y;
////		vec3Temp.z = transform.position.z - 1.0f;		
//		vec3Temp.z = vec3Position.z;
//		slotItem.transform.position = vec3Temp;
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
		
//		return collider.bounds.IntersectRay( ray );
		return AsUtil.PtInCollider( collider, ray);
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
		if( null == realItem )
		{
			Debug.LogError("UIStorageSlot::CreateSlotItem() [ null == realItem ]");
			return false;		
		}
		
		GameObject resGo = realItem.item.GetIcon();
		if( null == resGo )
		{
			Debug.LogError("UIStorageSlot::CreateSlotItem() [ null == resGo ] item id : " + realItem.item.ItemID );
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
		
		return true;
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
