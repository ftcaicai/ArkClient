using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public abstract class PartsElement 
{		
	private Item m_Item;
	private int nItemTableIdx;
	private byte nStrengthenCount;	
	protected Item.eEQUIP m_eEquipType;
	private bool m_bNeedResetParts = false;	
	
	// get
	public Item getItem
	{
		get
		{
			return m_Item;
		}
	}
	
	protected int getItemSupType
	{
		get
		{
			if( null == m_Item )
				return -1;
			
			return	m_Item.ItemData.GetSubType();
		}
	}	
		
	public bool isNeedResetParts
	{
		get
		{
			return m_bNeedResetParts;
		}
	}	
	
	public bool isHavePartsItem
	{
		get
		{
			return null != m_Item;
		}
	}
	
	public Item.eEQUIP getEquip
	{
		get	
		{
			return m_eEquipType;
		}
	}
	
	//set		
	public void SetNeedResetParts( bool _bNeed )
	{
		m_bNeedResetParts = _bNeed;
	}		
	
	
	
	
	// virtual 
	public virtual bool SetPartsItem( int _nItemTableIdx, byte _nStrengthenCount, eGENDER _eGender, eCLASS eClass, PartsRoot _partsRoot )
	{		
		m_Item = ItemMgr.ItemManagement.GetItem( _nItemTableIdx );
		if( null == m_Item )
		{
			SetEmpty();
			Debug.LogError("SetPartsItem() [null == item] index : " + _nItemTableIdx );
			return false;
		}			
		
		
		
		if( m_Item.ItemData.GetSubType() != (int)getEquip )
		{
			SetEmpty();
			Debug.LogError("SetPartsItem() [m_Item.ItemData.GetSubType() != m_eEquipType]item id: " + m_Item.ItemID 
				+ "  cur equip : " + getEquip
				+ " [item equip : " + (Item.eEQUIP)m_Item.ItemData.GetSubType() );
			return false;
		}
		
		return true;
	}	
	
	
	public virtual void SetEmpty()
	{		
		m_Item = null;
	}
	
	public virtual void CreateData( eGENDER eGender ){}	
}
