using UnityEngine;
using System.Collections;

public class EnchantSlot : MonoBehaviour 
{
	
	private Item  m_item;
	private GameObject m_Object;
	private UISlotItem m_uiSlotItem;
	
	public void SetItem( Item _item )
	{
		m_item = _item;		
	}
	
	public Item getItem
	{
		get
		{
			return m_item;
		}
	}
	// collder 
	
	public bool IsIntersect( Ray ray )
	{
		if( null == collider )
		{
			Debug.LogError("EnchantSlot::IsIntersect() [ null == collider ]");
			return false;
		}
		
//		return collider.bounds.IntersectRay( ray );
		return AsUtil.PtInCollider( collider, ray);
	}
	
	public void ResetSlotItemPosition()
	{
		if(null==m_item)
		{		
			Debug.LogError("EnchantSlot::ResetPosition() [ null == m_item ]");
			return;
		}
		
		Vector3 vec3Temp = transform.position;
		vec3Temp.z -= 0.5f;
		m_Object.transform.position = vec3Temp;
	}
	
	public void ResetSlotItemLocalPosition( float fz )
	{
		/*if(null==m_uiSlotItem)
		{		
			Debug.LogError("EnchantSlot::ResetPosition() [ null == m_uiSlotItem ]");
			return;
		}
		
		if( null == m_uiSlotItem.iconImg )
			return;
		
		Vector3 vec3Temp = m_uiSlotItem.iconImg.transform.localPosition;
		vec3Temp.z = fz;
		m_uiSlotItem.iconImg.transform.localPosition = vec3Temp;*/
	}
	
	public void DeleteSlotItem()
	{				
		if( null == m_Object)
			return;
		
		DestroyImmediate( m_Object );	
		SetItem( null );
	}
	
	
	
	public bool CreateSlotItem( Item _item, Transform trmParent )
	{
		if( null == _item )
		{
			Debug.LogError("EnchantSlot::CreateSlotItem() [ null == realItem ]");
			return false;		
		}
		
		GameObject resGo = _item.GetIcon();
		if( null == resGo )
		{
			Debug.LogError("EnchantSlot::CreateSlotItem() [ null == resGo ] item id : " + _item.ItemID );
			return false;
		}
		
		
		DeleteSlotItem();
		
		m_Object = GameObject.Instantiate( resGo ) as GameObject;
		m_Object.transform.parent = trmParent;
		m_Object.transform.localPosition = Vector3.zero;
		m_Object.transform.localRotation = Quaternion.identity;
		m_Object.transform.localScale = Vector3.one;
		
		m_uiSlotItem = m_Object.GetComponent<UISlotItem>();
						
		
		SetItem( _item );
		ResetSlotItemPosition();
		
		return true;
	}
	
	
	
	
	public void SetImgSize( float fwidth, float fheight )
	{
		if( null == m_uiSlotItem )
			return;
		
		if( null == m_uiSlotItem.iconImg)
			return;
		m_uiSlotItem.iconImg.SetSize( fwidth, fheight );
	}
	
	public void SetItemCountText( int iCount )
	{
		if( null != m_uiSlotItem)
			m_uiSlotItem.SetItemCountText( iCount);
	}
	
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

	
	
	
	