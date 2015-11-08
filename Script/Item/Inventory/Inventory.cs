using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class InvenSlot
{
	private RealItem m_realItem;
	private bool m_isMoveLock = false;
	
	public InvenSlot( RealItem _realItem)
	{
		m_realItem = _realItem;
	}
	
	public void SetItem(  sITEM sitem, int iSlot )
	{		
		if( null == m_realItem)
		{
			m_realItem = new RealItem( sitem, iSlot);
			return;
		}
		m_realItem.SetItem( sitem, iSlot);
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
	
	public void SetRealItem( RealItem _realItem)
	{
		m_realItem = _realItem;
	}
	
	public void SetMoveLock( bool bLock)
	{
		m_isMoveLock = bLock;
	}
	
	public void Clear()
	{
		m_realItem = null;
		m_isMoveLock = false;
	}
}

public class Inventory
{	
	//----------------------------------------------------------
	/* Public const Variable */
	//----------------------------------------------------------
	public const int useInvenSlotBeginIndex = 20;
	public const int useInvenSlotNumInPage = 25;
	public const int useInvenPageNum = 5;	
	public const int maxInvenSlotNum = useInvenSlotBeginIndex + ( useInvenSlotNumInPage*useInvenPageNum);
	static public byte hadBaseSlotIndex = 20 + 25;
	
	
	public static int fairyEquipSlotIdx = 6;
	public static int wingEquipSlotIdx = 5;
	
	
	
	
	//----------------------------------------------------------
	/* Private Variable */
	//----------------------------------------------------------	
	private InvenSlot[] m_ItemList = new InvenSlot[ maxInvenSlotNum ];
	private byte m_nExtendPageCount = 0;
	private System.Text.StringBuilder m_sbChatTemp = new System.Text.StringBuilder();
	public bool[] newReceiveItemslots = new bool[ maxInvenSlotNum ];	
	public bool isNewReceiveItems = false;
	private List<RealItem> m_ExpireItemList = new List<RealItem>();   
	
	private void SetExpireItem( int iSlotIndex, RealItem _realItem )
	{
		if( null == _realItem )
		{
			for( int i=0; i<m_ExpireItemList.Count; ++i )
			{
				if( null == m_ExpireItemList[i] )
					continue;
				
				if( m_ExpireItemList[i].getSlot == iSlotIndex )
				{
					m_ExpireItemList.RemoveAt(i);
					return;
				}
			}
			
			return;
		}
		
		if( iSlotIndex < useInvenSlotBeginIndex )
		{
			if( null != _realItem && Item.eUSE_TIME_TYPE.Finish == _realItem.item.ItemData.m_eUseTimeType )
			{
				m_ExpireItemList.Add( _realItem );
			}
		}
	}
	
	public enum eCOMPARE_TYPE
	{
		YEAR,
		MONTH,
		DAY,
		HOUR,
		MINUTE,
		SECOND,
	}
	
	public static bool IsSameNowDay( System.DateTime day, eCOMPARE_TYPE eCompareType )	
	{
		System.DateTime nowDay = System.DateTime.Now;
		
		switch( eCompareType )
		{
		case eCOMPARE_TYPE.YEAR:
			return 	day.Year == nowDay.Year;
		case eCOMPARE_TYPE.MONTH:
			return 	day.Year == nowDay.Year && 
					day.Month == nowDay.Month;
			
		case eCOMPARE_TYPE.DAY:
			return 	day.Year == nowDay.Year && 
					day.Month == nowDay.Month &&
					day.Day == nowDay.Day;
			
		case eCOMPARE_TYPE.HOUR:
			return 	day.Year == nowDay.Year && 
					day.Month == nowDay.Month &&
					day.Day == nowDay.Day &&
					day.Hour == nowDay.Hour;
			
		case eCOMPARE_TYPE.MINUTE:
			return 	day.Year == nowDay.Year && 
					day.Month == nowDay.Month && 
					day.Day == nowDay.Day && 
					day.Hour == nowDay.Hour &&
					day.Minute == nowDay.Minute;		
		}	
		
		// type : eCOMPARE_TYPE.SECOND
		return 	day.Year == nowDay.Year && 
				day.Month == nowDay.Month && 
				day.Day == nowDay.Day && 
				day.Hour == nowDay.Hour &&
				day.Minute == nowDay.Minute &&
				day.Second == nowDay.Second;
	}
	
	public void CheckExpireItems()
	{		
		if( GAME_STATE.STATE_INGAME != AsGameMain.s_gameState )
			return;
		int i=0;
		while( i < m_ExpireItemList.Count)
		{
			
			RealItem _realItem = m_ExpireItemList[i];	
			
			if( null != _realItem && Item.eUSE_TIME_TYPE.Finish == _realItem.item.ItemData.m_eUseTimeType )
			{
			
				System.DateTime dt = new System.DateTime(1970, 1, 1, 9, 0, 0);
				dt = dt.AddSeconds( _realItem.sItem.nExpireTime - 600 );	
				
				if( IsSameNowDay( dt, eCOMPARE_TYPE.MINUTE ) )					
				{
					
					m_sbChatTemp.Length = 0;
					m_sbChatTemp.Append( " " );
					m_sbChatTemp.Append( _realItem.item.ItemData.GetGradeColor() );
					m_sbChatTemp.Append( AsTableManager.Instance.GetTbl_String( _realItem.item.ItemData.nameId ) );
					m_sbChatTemp.Append( AsChatManager.Instance.m_Color_System_Item );
					
					AsChatManager.Instance.InsertSystemChat( 
						string.Format( AsTableManager.Instance.GetTbl_String(385), m_sbChatTemp.ToString() ), eCHATTYPE.eCHATTYPE_SYSTEM_ITEM );
					
					m_ExpireItemList.RemoveAt(i);
  					continue;
				}
			}
			else
			{
				m_ExpireItemList.RemoveAt(i);
  				continue;
			}
			
		 	++i;
		}
	}
	
	
	//----------------------------------------------------------
	/* public Function */
	//----------------------------------------------------------
	
	public long GetInvenAddSphereCost( int iPage )
	{
		int iIndex = 116 + iPage;	
		Tbl_GlobalWeight_Record _record = AsTableManager.Instance.GetTbl_GlobalWeight_Record(iIndex);
		if( null == _record)
		{
			AsUtil.ShutDown( "Inventory::GetInvenAddSphereCost()[ not find id : " +  iIndex);
			return 0;
		}
		
		return ( long)_record.Value;
	}
	
	public int GetEnableSlotIdx()
	{
		return  hadBaseSlotIndex + ( getExtendPageCount*5);
	}
	public byte getExtendPageCount
	{
		get
		{
			return m_nExtendPageCount;
		}		
	}
	
	public void SetExtendPageCount( byte nExtendPageCount)
	{
		Debug.Log( "Inventory::SetExtendPageCount()[ " + nExtendPageCount );
		m_nExtendPageCount = nExtendPageCount;
	}

	
	
	public InvenSlot[] invenSlots
	{
		get
		{
			return m_ItemList;
		}
	}	
	
	public void ClearInvenItems()
	{
		for( int i=0; i<m_ItemList.Length; ++ i)
		{
			if( null == m_ItemList[i])
				continue;
			
			m_ItemList[i].Clear();
			m_ItemList[i] = null;
		}
		
		m_ExpireItemList.Clear();
	}
	
    public void ReceiveItemList( body2_SC_ITEM_INVENTORY[] _list)
    {
        if ( null == _list)                   
            return;		
		
		foreach( body2_SC_ITEM_INVENTORY sitem in _list)
		{
			if( null == sitem)
				continue;
			
			SetItem( sitem.sItem, sitem.nSlot);			
		}  
		
		ItemMgr.HadItemManagement.Inven.isNewReceiveItems = false;
		for( int i=0; i< ItemMgr.HadItemManagement.Inven.newReceiveItemslots.Length; ++i )
		{
			ItemMgr.HadItemManagement.Inven.newReceiveItemslots[i] = false;
		}
		
    }
	
	private void SetChatMsg( body_SC_ITEM_SLOT sitem )
	{
		Item _dropItem = ItemMgr.ItemManagement.GetItem( sitem.sItem.nItemTableIdx);
			
		if( null != _dropItem)
		{
			int iHadItemCount = sitem.sItem.nOverlapped;
			
			RealItem _dropRealItem = GetRealItemInSlot( sitem.nSlot);
			if( null != _dropRealItem)
			{
				iHadItemCount = sitem.sItem.nOverlapped - _dropRealItem.sItem.nOverlapped;					
			}
			
			
			if( 1 == iHadItemCount)
			{					
				m_sbChatTemp.Remove( 0, m_sbChatTemp.Length );
				
				m_sbChatTemp.Append( _dropItem.ItemData.GetGradeColor().ToString() );
				m_sbChatTemp.Append( AsTableManager.Instance.GetTbl_String( _dropItem.ItemData.nameId ) );		
				
				
				//m_sbChatTemp.AppendFormat( AsTableManager.Instance.GetTbl_String(1236),
				//							AsTableManager.Instance.GetTbl_String(_dropItem.ItemData.nameId) );					
				
				AsChatManager.Instance.InsertSystemChat( string.Format( AsTableManager.Instance.GetTbl_String(1236), m_sbChatTemp.ToString()), eCHATTYPE.eCHATTYPE_SYSTEM_ITEM);					
			}
			else if( 0 < iHadItemCount )
			{					
				m_sbChatTemp.Remove( 0, m_sbChatTemp.Length );				
				m_sbChatTemp.Append( _dropItem.ItemData.GetGradeColor().ToString() );
				m_sbChatTemp.Append( AsTableManager.Instance.GetTbl_String( _dropItem.ItemData.nameId ) );		
				
				//m_sbChatTemp.AppendFormat( AsTableManager.Instance.GetTbl_String(1237),
				//							AsTableManager.Instance.GetTbl_String(_dropItem.ItemData.nameId), iHadItemCount );						
				
				AsChatManager.Instance.InsertSystemChat( string.Format( AsTableManager.Instance.GetTbl_String(1237), m_sbChatTemp.ToString(), iHadItemCount ), eCHATTYPE.eCHATTYPE_SYSTEM_ITEM);						
			}				
		}
	}
	
	public void ReceiveItem( body_SC_ITEM_SLOT sitem)
	{		
		if( null == sitem)
		{
			Debug.LogError( "Inventory::ReceiveItem() [ null == sitem] ");
			return;
		}	
		
        eITEMCHANGECONTENTS itemContent = ( eITEMCHANGECONTENTS)sitem.nContents;

		if (sitem.sItem.nOverlapped != 0 && itemContent != eITEMCHANGECONTENTS.eITEMCHANGECONTENTS_ITEMMOVE)
			CheckGetQuestItem(sitem.sItem.nItemTableIdx);
		
		
		if( eITEMCHANGECONTENTS.eITEMCHANGECONTENTS_RANDITEM == itemContent ||
			eITEMCHANGECONTENTS.eITEMCHANGECONTENTS_PICKUP == itemContent || 
			eITEMCHANGECONTENTS.eITEMCHANGECONTENTS_PRODUCT == itemContent )
		{		
				
			SetChatMsg( sitem );	
			
		}		 
		else if( 	eITEMCHANGECONTENTS.eITEMCHANGECONTENTS_SHOP == itemContent ||
					eITEMCHANGECONTENTS.eITEMCHANGECONTENTS_PRIVATESHOP == itemContent ||
					eITEMCHANGECONTENTS.eITEMCHANGECONTENTS_CASHSHOP == itemContent ||
					eITEMCHANGECONTENTS.eITEMCHANGECONTENTS_SOCIALSHOP == itemContent )
		{
		}
		
		if( eITEMCHANGECONTENTS.eITEMCHANGECONTENTS_PRODUCT == itemContent )
		{
			Item _productItem = ItemMgr.ItemManagement.GetItem( sitem.sItem.nItemTableIdx);
			if( null != _productItem )
				AsSoundManager.Instance.PlaySound( _productItem.ItemData.getStrDropSound, Vector3.zero, false);				
		}
		
		#region - pet -
		if( sitem.nSlot == AsPetManager.ePETINVENTORY)
			return;
		#endregion

        int prevItemID    = 0;
        RealItem prevSlot = GetRealItemInSlot(sitem.nSlot);
        if (prevSlot != null)
           prevItemID =  prevSlot.item.ItemID;


        if (sitem.nSlot < newReceiveItemslots.Length && false == AsHudDlgMgr.Instance.IsOpenInven)
        {
            if (0 == sitem.sItem.nItemTableIdx )
            {
                newReceiveItemslots[sitem.nSlot] = false;
            }
            else if (null == GetRealItemInSlot(sitem.nSlot) )
            {
                isNewReceiveItems = true;
                newReceiveItemslots[sitem.nSlot] = true;
                AsHudDlgMgr.Instance.SetNewInvenImg(true);
            }
        }
		
		
		//아이템 장착...
		if(sitem.nSlot < useInvenSlotBeginIndex && sitem.sItem.nItemTableIdx != 0 && 
			eITEMCHANGECONTENTS.eITEMCHANGECONTENTS_STRENGTHEN != itemContent && eITEMCHANGECONTENTS.eITEMCHANGECONTENTS_ENCHANT != itemContent )
		{
			AsEventNotifyMgr.Instance.CenterNotify.AddItemPutOnMessage( GetRealItemInSlot(sitem.nSlot), sitem.sItem.nItemTableIdx, sitem.sItem.nStrengthenCount, sitem.sItem );
		}
	
		SetItem( sitem.sItem, sitem.nSlot);
		
		
		if( eITEMCHANGECONTENTS.eITEMCHANGECONTENTS_ROULETTE == itemContent )
		{			
			AsHudDlgMgr.Instance.OpenRandItemUI( sitem.nSlot, GetRealItemInSlot( sitem.nSlot) );			
		}


		if (itemContent == eITEMCHANGECONTENTS.eITEMCHANGECONTENTS_PICKUP ||
			 itemContent == eITEMCHANGECONTENTS.eITEMCHANGECONTENTS_TRADE ||
			 itemContent == eITEMCHANGECONTENTS.eITEMCHANGECONTENTS_PRODUCT ||
			 itemContent == eITEMCHANGECONTENTS.eITEMCHANGECONTENTS_SHOP ||
			 itemContent == eITEMCHANGECONTENTS.eITEMCHANGECONTENTS_SOCIALSHOP ||
			 itemContent == eITEMCHANGECONTENTS.eITEMCHANGECONTENTS_CHEAT ||
			 itemContent == eITEMCHANGECONTENTS.eITEMCHANGECONTENTS_ITEMMOVE)
		{
			if (sitem.sItem.nOverlapped == 0)
			{
				if (itemContent != eITEMCHANGECONTENTS.eITEMCHANGECONTENTS_ITEMMOVE)
					ArkQuestmanager.instance.GernerateQuestItemMsg(prevItemID, GetItemTotalCount(prevItemID));
				AsUseItemToMonTriggerManager.instance.UpdateUseItemToTarget(prevItemID);
				AsUseItemToTargetPanelManager.instance.UpdateUseItemToTargetPanel(prevItemID);
			}
			else
			{
				if (itemContent != eITEMCHANGECONTENTS.eITEMCHANGECONTENTS_ITEMMOVE)
					ArkQuestmanager.instance.GernerateQuestItemMsg(sitem.sItem.nItemTableIdx, GetItemTotalCount(sitem.sItem.nItemTableIdx));
				AsUseItemToMonTriggerManager.instance.UpdateUseItemToTarget(sitem.sItem.nItemTableIdx);
				AsUseItemToTargetPanelManager.instance.UpdateUseItemToTargetPanel(sitem.sItem.nItemTableIdx);
			}

			// quest - equip item check
			if (itemContent == eITEMCHANGECONTENTS.eITEMCHANGECONTENTS_ITEMMOVE && sitem.nSlot < useInvenSlotBeginIndex)
			{
				// check equip

				// Equip
				if (invenSlots[sitem.nSlot].realItem != null)
				{
					AchEquipItem equipItem = new AchEquipItem(sitem.sItem.nItemTableIdx);
					QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_EQUIP_ITEM, equipItem);
				}
				else // unEquip
				{
					List<int> listEquipItemID = ArkQuestmanager.instance.GetAchEquipItemIDs();

					foreach (int id in listEquipItemID)
					{
						if (IsHaveEquipItem(id) == false && IsHaveCosEquipItem(id) == false)
						{
							AchEquipItem equipItem = new AchEquipItem(id);
							QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_UNEQUIP_ITEM, equipItem);
						}
					}
				}
			}

		}
		else if (itemContent == eITEMCHANGECONTENTS.eITEMCHANGECONTENTS_QUEST || itemContent == eITEMCHANGECONTENTS.eITEMCHANGECONTENTS_RANDITEM)
		{
			ArkQuestmanager.instance.UpdateQuestItem();
		}
        else if (itemContent == eITEMCHANGECONTENTS.eITEMCHANGECONTENTS_QUEST_COMPLETE_REWARD)
        {
            AsEventNotifyMgr.Instance.ShowGetItemAlrameBalloon(sitem.sItem);
        }
        else
			Debug.LogWarning(itemContent);
		
		
		if( null != AsHudDlgMgr.Instance )
		{
			if( true == AsHudDlgMgr.Instance.IsOpenInven && sitem.nSlot >= useInvenSlotBeginIndex && eITEMCHANGECONTENTS.eITEMCHANGECONTENTS_ROULETTE != itemContent)
			{
				AsHudDlgMgr.Instance.invenDlg.SetSlotItem( sitem.nSlot, GetRealItemInSlot( sitem.nSlot));
			}
			else if( true == AsHudDlgMgr.Instance.IsOpenPlayerStatus && sitem.nSlot < useInvenSlotBeginIndex)
			{
				AsHudDlgMgr.Instance.playerStatusDlg.SetSlotItem( sitem.nSlot, GetRealItemInSlot( sitem.nSlot));
			}		
			
			if( true == AsHudDlgMgr.Instance.IsOpenEnchantDlg )
			{
				AsHudDlgMgr.Instance.enchantDlg.ReciveEnchantUseItem( sitem.nSlot, GetRealItemInSlot( sitem.nSlot));
			}
			
			if ( true == AsHudDlgMgr.Instance.IsOpenStrengthenDlg)
			{			
				AsHudDlgMgr.Instance.strengthenDlg.ResetRealItems();			
			}
			
			if( true == AsHudDlgMgr.Instance.IsOpenProductionDlg)
			{
				AsHudDlgMgr.Instance.productionDlg.ResetReceiveItem();
			}	
			
			if( true == AsHudDlgMgr.Instance.IsOpenStorage )
			{
				AsHudDlgMgr.Instance.storageDlg.SetSlotItem( sitem.nSlot );
			}
			
		}	
			
		
		if( null != ItemMgr.HadItemManagement && sitem.nSlot >= useInvenSlotBeginIndex )
			ItemMgr.HadItemManagement.QuickSlot.ResetSlot();
		
		//Debug.Log( "------ total time : " + ( Time.realtimeSinceStartup - fTotalTime ) );

        if (AsHudDlgMgr.Instance.IsOpenInven)
            QuestTutorialMgr.Instance.ProcessQuestTutorialMsg(new QuestTutorialMsgInfo(QuestTutorialMsg.MOVE_INVENTORY_ITEM));
		
		
		if ( itemContent == eITEMCHANGECONTENTS.eITEMCHANGECONTENTS_PICKUP ||
             itemContent == eITEMCHANGECONTENTS.eITEMCHANGECONTENTS_TRADE  ||
             itemContent == eITEMCHANGECONTENTS.eITEMCHANGECONTENTS_POST ||             
			itemContent == eITEMCHANGECONTENTS.eITEMCHANGECONTENTS_MIXITEM ||
			itemContent == eITEMCHANGECONTENTS.eITEMCHANGECONTENTS_CHEAT ||
			itemContent == eITEMCHANGECONTENTS.eITEMCHANGECONTENTS_SHOP ||
			itemContent == eITEMCHANGECONTENTS.eITEMCHANGECONTENTS_PRIVATESHOP ||
			itemContent == eITEMCHANGECONTENTS.eITEMCHANGECONTENTS_PRODUCT ||
			itemContent == eITEMCHANGECONTENTS.eITEMCHANGECONTENTS_CASHSHOP ||
			itemContent == eITEMCHANGECONTENTS.eITEMCHANGECONTENTS_SOCIALSHOP ||
			itemContent == eITEMCHANGECONTENTS.eITEMCHANGECONTENTS_QUEST			
			)
		{
			AutoEquipItem( GetRealItemInSlot( sitem.nSlot) );
		}
	}
	

	
	private void AutoEquipItem( RealItem _realItem )
	{
		if( null == _realItem )
			return;
#if false
		Map _map = TerrainMgr.Instance.GetCurrentMap();
		if( null == _map )
			return;
		
		if( _map.MapData.getMapType == eMAP_TYPE.Tutorial )
			return;
#endif
		if(_realItem.getSlot >= useInvenSlotBeginIndex )
		{		
			int iTargetSlot = -1;
			if( Item.eITEM_TYPE.EquipItem == _realItem.item.ItemData.GetItemType() )
			{
				if( Item.eEQUIP.Ring == (Item.eEQUIP)_realItem.item.ItemData.GetSubType() )
				{
					if( null != GetRealItemInSlot( 7 ) &&
						null != GetRealItemInSlot( 8 ) )
						return;
						
				}
				else if( null != GetRealItemInSlot( _realItem.item.ItemData.GetSubType()-1 ) )
				{
					return;
				}
				
				iTargetSlot = UIInvenDlg.SendDClickEquipItem_s( _realItem );
				
			}				
			/*else if( Item.eITEM_TYPE.CosEquipItem == _realItem.item.ItemData.GetItemType() )
			{
				if( null != GetRealItemInSlot( 9 + _realItem.item.ItemData.GetSubType() ) )
					return;
				
				iTargetSlot = UIInvenDlg.SendDClickCosEquipItem_s( _realItem );
			}*/
			
			if( -1 != iTargetSlot )
			{				
				if( AsHudDlgMgr.Instance.IsOpenInven )
				{
					AsHudDlgMgr.Instance.invenDlg.SetEquipEffect(_realItem.getSlot, iTargetSlot, false );
				}
				
				if( null != AsMyProperty.Instance.equipAlertEffect && false == AsMyProperty.Instance.equipAlertEffect.gameObject.active )
				{
					AsMyProperty.Instance.equipAlertEffect.gameObject.active = true;	
					AsMyProperty.Instance.equipAlertEffect.Play();
				}
			}
		}
	}
		
	private void SetItem( sITEM sitem, int iSlot)
	{	
		if( 0 > iSlot || m_ItemList.Length <= iSlot)
		{
			Debug.LogError( "Inventory::SetItem() [m_ItemList.Length <= iSlot] [ index : " + iSlot);
			return;
		}		
		
		if( null == sitem)
		{
			Debug.LogError("Inventory::SetItem()[null == sitem ] ");
		}
		
		if ( 0 == sitem.nItemTableIdx) 
		{
			if( null != m_ItemList[ iSlot ])
				m_ItemList[ iSlot ].Clear();			
		}
		else
		{
			if( null == m_ItemList[ iSlot ])
			{
				m_ItemList[ iSlot ] = new InvenSlot( new RealItem( sitem, iSlot));
			}
			else
			{	
				if( null == m_ItemList[ iSlot ])
				{
					Debug.LogError("Inventory::SetItem()[null == m_ItemList[ iSlot ] ] slot:  " + iSlot );
				}
				m_ItemList[ iSlot ].SetItem( sitem, iSlot);
			}
		}	
		
		#region -GameGuide_Item
		AsGameGuideManager.Instance.CheckUp( eGameGuideType.Item, sitem.nItemTableIdx);
		#endregion
		
		SetExpireItem( iSlot, GetRealItemInSlot( iSlot ) );
	}	
	
	
	public InvenSlot GetInvenSlotItem( int iIndex)
	{
		if ( 0 > iIndex || iIndex >= m_ItemList.Length)
        {
            Debug.LogError( "Inventory::GetInvenSlotItem() [ iIndex( " + iIndex + ") >= m_ItemList.Length( " + m_ItemList.Length + ") ] ");
            return null;
        }
		
		return m_ItemList[iIndex];
	}

    public void ResetInvenSlotMoveLock()
    {
        for ( int i = useInvenSlotBeginIndex; i < m_ItemList.Length; ++i)
        {
            if ( null == m_ItemList[i])
                continue;

            m_ItemList[i].SetMoveLock( false);
        }
    }
	
	public void SetSlotMoveLock( int iSlot, bool bLock )//$yde
	{
		InvenSlot _invenslot = GetInvenSlotItem( iSlot );
		if( null != _invenslot ) 
			_invenslot.SetMoveLock(bLock);		
	}
	
    public RealItem GetIvenItem( int iIndex)
    {    
		return GetRealItemInSlot( iIndex);
    }
	
	public RealItem GetRealItem( int iItemId)
	{
		for( int i=useInvenSlotBeginIndex; i<m_ItemList.Length; ++i)
		{
			if( false == IsExistRealItem( i))
				continue;
			
			if( m_ItemList[i].realItem.item.ItemID == iItemId)
				return m_ItemList[i].realItem;
		}
		
		return null;
	}
	
	public void SetRealItemMoveLock( int itemId, bool _isLock )
	{
		for( int i=useInvenSlotBeginIndex; i<m_ItemList.Length; ++i)
		{
			if( false == IsExistRealItem( i))
				continue;
			
			if( m_ItemList[i].realItem.item.ItemID == itemId)
				m_ItemList[i].SetMoveLock(_isLock);
		}
	}
	
	private bool IsExistRealItem( int iSlotIdx)
	{
		if( 0 > iSlotIdx || m_ItemList.Length <= iSlotIdx )
		{
			Debug.LogError("Inventory::IsExistRealItem()[ error m_ItemList : " + iSlotIdx );
			return false;
		}
		
		if( null == m_ItemList[iSlotIdx])
			return false;
		
		return ( null != m_ItemList[iSlotIdx].realItem);
	}
	
	public RealItem GetRealItemInSlot( int iSlotIdx)
	{
		if( 0 > iSlotIdx || m_ItemList.Length <= iSlotIdx )
		{
			Debug.LogError("Inventory::GetRealItemInSlot()[ error m_ItemList : " + iSlotIdx );
			return null;
		}
		
		if( null == m_ItemList[iSlotIdx])
			return null;
		
		return m_ItemList[iSlotIdx].realItem;
	}
	
	public List<RealItem> GetHaveInvenItemsID( int iItemID)
	{
		List<RealItem> tempItems = new List<RealItem>();
		
		for( int i=useInvenSlotBeginIndex; i<m_ItemList.Length; ++i)
		{
			if( false == IsExistRealItem( i))
				continue;
			
			if( m_ItemList[i].realItem.item.ItemID == iItemID)
			{
				tempItems.Add( m_ItemList[i].realItem);				
			}
		}
		
		return tempItems;
	}
	
	public List<RealItem> GetHaveCashItemInInven()
	{
		List<RealItem> tempItems = new List<RealItem>();
		
		for( int i=useInvenSlotBeginIndex; i<m_ItemList.Length; ++i)
		{
			if( false == IsExistRealItem( i))
				continue;
			
			if( m_ItemList[i].realItem.item.ItemData.GetItemType() == Item.eITEM_TYPE.EtcItem &&
				m_ItemList[i].realItem.item.ItemData.getGoodsType == Item.eGOODS_TYPE.Cash)
			{		
				
				tempItems.Add( m_ItemList[i].realItem);				
			}
		}
		
		return tempItems;
	}

    public int GetItemTotalCount( int iItemID)
    {
        int totalCount = 0;
        for ( int i = useInvenSlotBeginIndex; i < m_ItemList.Length; ++i)
        {
            if( false == IsExistRealItem( i))
                continue;

            if ( m_ItemList[i].realItem.item.ItemID == iItemID)
            {
                totalCount +=m_ItemList[i].realItem.sItem.nOverlapped;
            }
        }
        return totalCount;
    }
	
	public int GetStrengthenItemTotalCount(int iIndex )
	{
		int totalCount = 0;
        for ( int i = useInvenSlotBeginIndex; i < m_ItemList.Length; ++i)
        {
            if( false == IsExistRealItem( i))
                continue;

            if ( m_ItemList[i].realItem.item.ItemID == GetStrengthenItemID(iIndex, 0) )
            {
                totalCount += m_ItemList[i].realItem.sItem.nOverlapped;
            }
			
			if ( m_ItemList[i].realItem.item.ItemID == GetStrengthenItemID(iIndex, 1) )
            {
                totalCount += m_ItemList[i].realItem.sItem.nOverlapped;
            }
        }
        return totalCount;
	}
	
	public int GetStrengthenItemID( int iStrengthGrade, int _index )
	{
		Tbl_GlobalWeight_Record record = null;
		if( 0 == iStrengthGrade )
		{
			if( 0 == _index )
			{
				record = AsTableManager.Instance.GetTbl_GlobalWeight_Record(19);
				if( null == record)
				{
					return 120000;
				}
			}
			else
			{
				record = AsTableManager.Instance.GetTbl_GlobalWeight_Record(160);
				if( null == record)
				{
					return 120010;
				}
			}
		}
		else
		{
			if( 0 == _index )
			{				
				record = AsTableManager.Instance.GetTbl_GlobalWeight_Record(67);
				if( null == record)
				{
					return 120001;
				}
			}
			else
			{
				record = AsTableManager.Instance.GetTbl_GlobalWeight_Record(161);
				if( null == record)
				{
					return 120011;
				}
			}
		}
		
		
		return ( int)record.Value;
	}
	
	
	public int GetSapphireItemTotalCount()
	{
		int totalCount = 0;
        for ( int i = useInvenSlotBeginIndex; i < m_ItemList.Length; ++i)
        {
            if( false == IsExistRealItem( i))
                continue;

            if ( m_ItemList[i].realItem.item.ItemID == GetSapphireItemID(0) )
            {
                totalCount += m_ItemList[i].realItem.sItem.nOverlapped;
            }
			
			if ( m_ItemList[i].realItem.item.ItemID == GetSapphireItemID(1) )
            {
                totalCount += m_ItemList[i].realItem.sItem.nOverlapped;
            }
        }
        return totalCount;
	}
	
	public int GetSapphireItemID( int _index )
	{
		Tbl_GlobalWeight_Record record = null;
		
		if( 0 == _index )
		{
			record = AsTableManager.Instance.GetTbl_GlobalWeight_Record(162);
			if( null == record)
			{
				return 120002;
			}
		}
		else
		{
			record = AsTableManager.Instance.GetTbl_GlobalWeight_Record(164);
			if( null == record)
			{
				return 120012;
			}
		}		
		
		return ( int)record.Value;
	}
	
	public int GetEmeraldItemTotalCount()
	{
		int totalCount = 0;
        for ( int i = useInvenSlotBeginIndex; i < m_ItemList.Length; ++i)
        {
            if( false == IsExistRealItem( i))
                continue;

            if ( m_ItemList[i].realItem.item.ItemID == GetEmeraldItemID(0) )
            {
                totalCount += m_ItemList[i].realItem.sItem.nOverlapped;
            }
			
			if ( m_ItemList[i].realItem.item.ItemID == GetEmeraldItemID(1) )
            {
                totalCount += m_ItemList[i].realItem.sItem.nOverlapped;
            }
        }
        return totalCount;
	}
	
	public int GetEmeraldItemID( int _index )
	{
		Tbl_GlobalWeight_Record record = null;
		
		if( 0 == _index )
		{
			record = AsTableManager.Instance.GetTbl_GlobalWeight_Record(163);
			if( null == record)
			{
				return 120003;
			}
		}
		else
		{
			record = AsTableManager.Instance.GetTbl_GlobalWeight_Record(165);
			if( null == record)
			{
				return 120013;
			}
		}		
		
		return ( int)record.Value;
	}
	
	
	public bool IsHaveItem( int iItemID)
	{
		for( int i=useInvenSlotBeginIndex; i<m_ItemList.Length; ++i)
		{
			if( false == IsExistRealItem( i))
				continue;
			
			if( m_ItemList[i].realItem.item.ItemID == iItemID)
			{
				return true;				
			} 
		}
		
		return false;
	}
	
	public List<int> GetHaveItems()
	{
		List<int> tempItems = new List<int>();
		
		foreach( InvenSlot _invenslot in m_ItemList)
		{
			if( null == _invenslot.realItem)
				continue;
			
			tempItems.Add( _invenslot.realItem.item.ItemID);
		}
		
		return tempItems;
	}
	
	public bool IsHaveEquipItem( int iItemID)
	{
		for( int i=0; i<10; ++i)	
		{
			if( false == IsExistRealItem( i))
				continue;
			
			RealItem _realItem = GetRealItemInSlot( i);
			if( null == _realItem)
				continue;
			
			if( _realItem.item.ItemID == iItemID)
				return true;
		}
		
		return false;
	}
	
	public bool IsHaveCosEquipItem( int iItemID)
	{
		for( int i=10; i<20; ++i)	
		{
			if( false == IsExistRealItem( i))
				continue;
			
			RealItem _realItem = GetRealItemInSlot( i);
			if( null == _realItem)
				continue;
			
			if( _realItem.item.ItemID == iItemID)
				return true;
		}
		
		return false;
	}
	
	
	public List<int> GetHaveEquipItemIndex()
	{
		List<int> _itemEquipIndex = null;
		
		for( int i=0; i<10; ++i)
		{
			RealItem _realItem = GetRealItemInSlot( i);
			if( null == _realItem)
				continue;
			
			if( null == _itemEquipIndex)
				_itemEquipIndex = new List<int>();
			
			_itemEquipIndex.Add( _realItem.item.ItemID);
		}	
		
		return _itemEquipIndex;
	}
	
	
	public RealItem GetEquipItem( int iEquip)
	{
		int iTempEquip = iEquip -1;
		if( false == Item.IsCheckEquipIndex( iTempEquip) &&
			(Item.eEQUIP)iEquip != Item.eEQUIP.Pet) //$yde
		{
			Debug.LogError( "Inventory::GetEquipItem() [ false == Item.IsCheckEquipIndex( iEquip) ] equip : " + iTempEquip);
			return null;
		}
		
		return GetRealItemInSlot( iTempEquip);
	}
	
	public RealItem GetCosEquipItem( int iEquip)
	{
		int iTempEquip = Item.equipMaxCount + Item.GetCosEquipSlotIdx( (Item.eEQUIP)iEquip );
		if( false == Item.IsCheckCosEquipIndex( iTempEquip)) 
		{
			Debug.LogError( "Inventory::GetCosEquipItem() [ false == Item.IsCheckCosEquipIndex( iTempEquip) ] equip : " + iTempEquip);
			return null;
		}
		
		return GetRealItemInSlot( iTempEquip);
	}
	
	public int GetEmptyInvenSlot()
	{
		for( int i = useInvenSlotBeginIndex; i < GetEnableSlotIdx(); ++i)
		{
			if( null == GetRealItemInSlot(i))
				return i;
		}
		
		return -1;
	}
	
	public int GetEmptyInvenSlot( int iPage)
	{
		int iBeginIndex = useInvenSlotNumInPage * iPage;
		int iEndIndex = useInvenSlotBeginIndex + useInvenSlotNumInPage * (iPage+1);
		
		if( iBeginIndex > GetEnableSlotIdx() )
			return -1;
		
		iEndIndex = iEndIndex > GetEnableSlotIdx() ? GetEnableSlotIdx() : iEndIndex;
		
		for( int i = iBeginIndex + useInvenSlotBeginIndex; i < iEndIndex; ++i)
		{
			if( null == GetRealItemInSlot(i))
				return i;
		}
		
		return -1;
	}
	
	public int GetEmptyInvenSlotCount()
	{
		int iTotal = 0;
		for( int i = useInvenSlotBeginIndex; i < GetEnableSlotIdx(); ++i)
		{
			if( null == GetRealItemInSlot(i))
				++iTotal;
		}
		
		return iTotal;
	}
	
	public bool IsSlotEmpty( int iIndex)
	{
		if( 0 > iIndex || iIndex >= m_ItemList.Length)
        {
            Debug.LogError( "Inventory::IsSlotEmpty() [ iIndex >= m_ItemList.Length ] : " + iIndex);
            return false;
        }
		
		if( null == m_ItemList[iIndex])
			return true;
		
		return m_ItemList[iIndex].realItem == null;
	}
	
	public void Update()
	{
		CheckExpireItems();
	}

	public void CheckGetQuestItem(int iItemID)
	{
		Item nowItem = ItemMgr.ItemManagement.GetItem(iItemID);

		if (nowItem.ItemData.GetItemType() != Item.eITEM_TYPE.UseItem)
			return;

		if (nowItem.ItemData.GetSubType() != (int)Item.eUSE_ITEM.GetQuest)
			return;

		Color textColor = new Color(1.0f, 0.717f, 0.0627f, 1.0f);

		System.Text.StringBuilder sb = new System.Text.StringBuilder(textColor.ToString());
		sb.Append(AsTableManager.Instance.GetTbl_String(2134));

		ArkQuestmanager.instance.ShowBalloonRewardMsgBox(sb.ToString(), AsTableManager.Instance.GetTbl_String(2134), 10.0f);
	}
}
