using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class StorageSlot
{
	private RealItem m_realItem;
	private bool m_isMoveLock = false;
	
	public StorageSlot( RealItem _realItem )
	{
		m_realItem = _realItem;
	}
	
	public void SetItem(  sITEM sitem, int iSlot  )
	{		
		if( null == m_realItem )
		{
			m_realItem = new RealItem( sitem, iSlot );
			return;
		}
		m_realItem.SetItem(sitem, iSlot );
	}
	
	
	public RealItem realItem
	{
		get
		{
			return m_realItem;
		}
	}
	
	public bool isMoveLock
	{
		get
		{
			return m_isMoveLock;
		}
	}
	
	public void SetRealItem( RealItem _realItem )
	{
		m_realItem = _realItem;
	}
	
	public void SetMoveLock( bool bLock )
	{
		m_isMoveLock = bLock;
	}
	
	public void Clear()
	{
		m_realItem = null;
		m_isMoveLock = false;
	}
}

public class Storage
{	
	//----------------------------------------------------------
	/* Public const Variable */
	//----------------------------------------------------------
	public const int useStorageSlotNumInPage = 25;
	public const int useStoragePageNum = 10;	
	public const int maxStorageLine = 50;
	
	//----------------------------------------------------------
	/* Private Variable */
	//----------------------------------------------------------
	private StorageSlot[] m_ItemList = new StorageSlot[ useStoragePageNum * useStorageSlotNumInPage ];
	
	private int m_PageOpen = 0;
	private int m_LineOpen = 5;
	
	public static readonly int s_StorageLineCount = 5;
	public static readonly int s_DefaultLine = 0;
	
	//----------------------------------------------------------
	/* public Function */
	//----------------------------------------------------------
	public StorageSlot[] storageSlots
	{
		get
		{
			return m_ItemList;
		}
	}
	
	public int pageOpen{get{return m_PageOpen;}}
	public int lineOpen{get{return m_LineOpen;}}
	
	public void SetPageOpen( int iPage )
	{
		m_PageOpen = iPage;
		
		if( AsHudDlgMgr.Instance.IsOpenStorage )
		{
			AsHudDlgMgr.Instance.storageDlg.ResetMiracleText();
		}
	}
	
	public int curLineOpen{get{
			return m_LineOpen - m_PageOpen * s_StorageLineCount;
						
//			#region - current opened line -
//			int line = m_LineOpen % s_StorageLineCount;
//			if(line == 0) line = Storage.s_StorageLineCount;
//			return line;
//			#endregion
	}}
	
	public void ClearStorageItems()
	{
		for( int i=0; i<m_ItemList.Length; ++ i )
		{
			if( null == m_ItemList[i] )
				continue;
			
			m_ItemList[i].Clear();		
		}
	}
	
    public void ReceiveItemList(body1_SC_STORAGE_LIST _list)
    {
        if (null == _list)                   
            return;
		
//		ClearStorageItems();
		
		m_LineOpen = (int)_list.nExtendCount + s_DefaultLine;
		//m_PageOpen = m_LineOpen / s_StorageLineCount;
		SetPageOpen( m_LineOpen / s_StorageLineCount );
		
		foreach( body2_SC_STORAGE_LIST sitem in _list.body)
		{
			if( null == sitem )
				continue;
			
			SetItem( sitem.sItem, sitem.nSlot );			
		}  
    }
	
	public void ReceiveItem( body_SC_STORAGE_SLOT sitem )
	{
		if( null == sitem )
		{
			Debug.LogError("Storagetory::ReceiveItem() [ null == sitem] ");
			return;
		}
		
		SetItem(sitem.sItem, sitem.nSlot );
		
		if( null != AsHudDlgMgr.Instance && true == AsHudDlgMgr.Instance.IsOpenStorage)
			AsHudDlgMgr.Instance.storageDlg.SetSlotItem( sitem.nSlot, GetRealItemInSlot( sitem.nSlot ) );
	}
	
		
	private void SetItem( sITEM sitem, int iSlot )
	{	
		if( m_ItemList.Length <= iSlot )
		{
			Debug.LogError("Storagetory::SetItem() [m_ItemList.Length <= iSlot] [ index : " + iSlot );
			return;
		}
		
		if ( 0 == sitem.nItemTableIdx ) 
		{
			if( null != m_ItemList[ iSlot ] )
			{
				m_ItemList[ iSlot ].Clear();	
			}
		}
		else
		{
			if( null == m_ItemList[ iSlot ] )
			{
				m_ItemList[ iSlot ] = new StorageSlot( new RealItem( sitem, iSlot) );
			}
			else
			{
				m_ItemList[ iSlot ].SetItem( sitem, iSlot );
			}
		}	
	}	
	
	
	public StorageSlot GetStorageSlotItem( int iIndex )
	{
		if (iIndex >= m_ItemList.Length)
        {
            Debug.LogError("Storagetory::GetStorageSlotItem() [ iIndex >= m_ItemList.Length ] ");
            return null;
        }
		
		return m_ItemList[iIndex];
	}

    public void ResetStorageSlotMoveLock()
    {
        for (int i = 0; i < m_ItemList.Length; ++i)
        {
            if (null == m_ItemList[i])
                continue;

            m_ItemList[i].SetMoveLock(false);
        }
    }
	
	
    public RealItem GetStorageItem(int iIndex)
    {    
		return GetRealItemInSlot(iIndex);
    }
	
	public RealItem GetRealItem( int iItemId )
	{
		for( int i=0; i<m_ItemList.Length; ++i )
		{
			if( false == IsExistRealItem(i) )
				continue;
			
			if( m_ItemList[i].realItem.item.ItemID == iItemId )
				return m_ItemList[i].realItem;
		}
		
		return null;
	}
	
	private bool IsExistRealItem( int iSlotIdx )
	{
		if( null == m_ItemList[iSlotIdx] )
			return false;
		
		return (null != m_ItemList[iSlotIdx].realItem);
	}
	
	public RealItem GetRealItemInSlot( int iSlotIdx )
	{
		if( null == m_ItemList[iSlotIdx] )
			return null;
		
		return m_ItemList[iSlotIdx].realItem;
	}
	
	public List<RealItem> GetHaveStorageItemsID( int iItemID )
	{
		List<RealItem> tempItems = new List<RealItem>();
		
		for( int i=0; i<m_ItemList.Length; ++i )
		{
			if( false == IsExistRealItem(i) )
				continue;
			
			if( m_ItemList[i].realItem.item.ItemID == iItemID )
			{
				tempItems.Add( m_ItemList[i].realItem );				
			}
		}
		
		return tempItems;
	}
	
	public List<RealItem> GetHaveCashItemInStorage()
	{
		List<RealItem> tempItems = new List<RealItem>();
		
		for( int i=0; i<m_ItemList.Length; ++i )
		{
			if( false == IsExistRealItem(i) )
				continue;
			
			if( m_ItemList[i].realItem.item.ItemData.GetItemType() == Item.eITEM_TYPE.EtcItem &&
				m_ItemList[i].realItem.item.ItemData.getGoodsType == Item.eGOODS_TYPE.Cash )
			{		
				
				tempItems.Add( m_ItemList[i].realItem );				
			}
		}
		
		return tempItems;
	}

    public int GetItemTotalCount(int iItemID)
    {
        int totalCount = 0;
        for (int i = 0; i < m_ItemList.Length; ++i)
        {
            if( false == IsExistRealItem(i) )
                continue;

            if (m_ItemList[i].realItem.item.ItemID == iItemID)
            {
                totalCount +=m_ItemList[i].realItem.sItem.nOverlapped;
            }
        }
        return totalCount;
    }
	
	public int GetStrengthenItemTotalCount()
	{
		int totalCount = 0;
        for (int i = 0; i < m_ItemList.Length; ++i)
        {
            if( false == IsExistRealItem(i) )
                continue;

            if (m_ItemList[i].realItem.item.ItemID == getStrengthenItemID )
            {
                totalCount +=m_ItemList[i].realItem.sItem.nOverlapped;
            }
        }
        return totalCount;
	}
	
	public int getStrengthenItemID
	{
		get
		{
			return 120000;
		}
	}
	
	public bool IsHaveItem( int iItemID )
	{
		for( int i=0; i<m_ItemList.Length; ++i )
		{
			if( false == IsExistRealItem(i) )
				continue;
			
			if( m_ItemList[i].realItem.item.ItemID == iItemID )
			{
				return true;				
			} 
		}
		
		return false;
	}
	
	public List<int> GetHaveItems()
	{
		List<int> tempItems = new List<int>();
		
		foreach(StorageSlot _storageslot in m_ItemList )
		{
			if( null == _storageslot.realItem )
				continue;
			
			tempItems.Add( _storageslot.realItem.item.ItemID );
		}
		
		return tempItems;
	}
	
	public bool IsHaveEquipItem( int iItemID )
	{
		for( int i=0; i<10; ++i )	
		{
			if( false == IsExistRealItem(i) )
				continue;
			
			RealItem _realItem = GetRealItemInSlot(i);
			if( null == _realItem )
				continue;
			
			if( _realItem.item.ItemID == iItemID )
				return true;
		}
		
		return false;
	}
	
	public List<int> GetHaveEquipItemIndex()
	{
		List<int> _itemEquipIndex = null;
		
		for(int i=0; i<10; ++i)
		{
			RealItem _realItem = GetRealItemInSlot(i);
			if( null == _realItem )
				continue;
			
			if( null == _itemEquipIndex )
				_itemEquipIndex = new List<int>();
			
			_itemEquipIndex.Add( _realItem.item.ItemID );
		}	
		
		return _itemEquipIndex;
	}
	
	
//	public RealItem GetEquipItem( int iEquip )
//	{
//		int iTempEquip = iEquip -1;
//		if( false == Item.IsCheckEquipIndex( iTempEquip )) 
//		{
//			Debug.LogError("Storagetory::GetEquipItem() [ false == Item.IsCheckEquipIndex( iEquip ) ] equip : " + iTempEquip );
//			return null;
//		}
//		
//		return GetRealItemInSlot(iTempEquip);
//	}
//	
//	public RealItem GetCosEquipItem( int iEquip )
//	{
//		int iTempEquip = Item.equipMaxCount + (iEquip-1);
//		if( false == Item.IsCheckCosEquipIndex( iTempEquip ) ) 
//		{
//			Debug.LogError("Storagetory::GetCosEquipItem() [ false == Item.IsCheckCosEquipIndex( iTempEquip ) ] equip : " + iTempEquip );
//			return null;
//		}
//		
//		return GetRealItemInSlot(iTempEquip);
//	}
	
	public int GetEmptyStorageSlot()
	{
		for( int i=0; i<m_ItemList.Length; ++i )
		{
			if( null == GetRealItemInSlot(i) )
			{
				return i;
			}
		}
		
		return -1;
	}
	
	public bool IsSlotEmpty( int iIndex )
	{
		if (iIndex >= m_ItemList.Length)
        {
            Debug.LogError("Storagetory::IsSlotEmpty() [ iIndex >= m_ItemList.Length ] ");
            return false;
        }
		
		if( null == m_ItemList[iIndex] )
			return true;
		
		return m_ItemList[iIndex].realItem == null;
	}
	
	public void Update()
	{
		
	}
	
	public void ResultProcess(body_SC_STORAGE_MOVE_RESULT _result)
	{
		AsMessageBox popup = null;
		
		switch(_result.eResult)
		{
        case eRESULTCODE.eRESULT_SUCC:
            ArkQuestmanager.instance.UpdateQuestItem();
            break;
		case eRESULTCODE.eRESULT_FAIL_STORAGE:
			break;
		case eRESULTCODE.eRESULT_FAIL_STORAGE_FULL:
			popup = AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(143));
			break;
		case eRESULTCODE.eRESULT_FAIL_IVNENTORY_FULL:
			popup = AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(20));
			break;
		}
		
		if(popup != null)
			AsHudDlgMgr.Instance.storageDlg.MessageBoxPopup(popup);
	}
	
	public void ResultProcess(body_SC_STORAGE_COUNT_UP_RESULT _result)
	{
		AsMessageBox popup = null;
		switch(_result.eResult)
		{
		case eRESULTCODE.eRESULT_SUCC:
			//popup = AsNotify.Instance.MessageBox("", AsTableManager.Instance.GetTbl_String(347));
			
			m_LineOpen = (int)_result.nExtendCount + s_DefaultLine;
			//m_PageOpen = m_LineOpen / s_StorageLineCount;			
			SetPageOpen( m_LineOpen / s_StorageLineCount );
			
			if(m_LineOpen % s_StorageLineCount == 0)
				AsHudDlgMgr.Instance.storageDlg.page.SetPage(m_PageOpen - 1);
			else
				AsHudDlgMgr.Instance.storageDlg.page.SetPage(m_PageOpen);
			
			AsHudDlgMgr.Instance.storageDlg.PlayLineEffect(curLineOpen);
			AsHudDlgMgr.Instance.storageDlg.PageLock();
			AsHudDlgMgr.Instance.storageDlg.ResetSlotItmes();
			break;
		case eRESULTCODE.eRESULT_FAIL_STORAGE_UP_MAX_COUNT:
			popup = AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(218));
//			popup = AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(325));
			break;
		case eRESULTCODE.eRESULT_FAIL_STORAGE_UP_NEED_GOLD:
			
			// cash store required
			AsHudDlgMgr.Instance.OpenNeedSphereDlg();
			
//			string text = AsTableManager.Instance.GetTbl_String(209);
//			text = string.Format(text, Color.yellow.ToString() + record.Value + "G" + Color.white.ToString());
//			AsNotify.Instance.MessageBox( "Addition", text,
//				null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_ERROR);			
			break;
		case eRESULTCODE.eRESULT_FAIL:
			popup = AsNotify.Instance.MessageBox("", AsTableManager.Instance.GetTbl_String(325));
			break;
		}
		
		if(popup != null)
			AsHudDlgMgr.Instance.storageDlg.MessageBoxPopup(popup);
	}
}
