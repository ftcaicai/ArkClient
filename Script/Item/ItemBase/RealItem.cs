using UnityEngine;
using System.Collections;

public class RealItem 
{
    private sITEM m_sItem = null;
    private Item m_item = null;
	private int m_nSlot;
	//private CoolTimeGroup m_CoolTimeGroup = null;
	private bool m_isExistCooltime = false;
	

    public sITEM sItem
    {
        get
        {
            return m_sItem;
        }
    }
	
	public int getSlot
	{
		get
		{
			return m_nSlot;
		}
	}


    public Item item
    {
        get
        {
            return m_item;
        }
    }

    public RealItem( sITEM sitem, int iSlot  )
    {
        SetItem(sitem, iSlot);
		m_nSlot = iSlot;
    }	
	
	public CoolTimeGroup getCoolTimeGroup
	{
		get
		{
			if( null == m_item )
				return null;
			
			if( false == m_isExistCooltime )
				return null;
			
			return CoolTimeGroupMgr.Instance.GetCoolTimeGroup( m_item.ItemData.itemSkill, m_item.ItemData.itemSkillLevel );
		}
	}
		
	public void SetItem( sITEM sitem, int iSlot )
	{
		if (null == sitem)
        {
            Debug.LogError("RealItem::SetItem() [ null== server sitem ]");
			return;
        }      

        m_item = ItemMgr.ItemManagement.GetItem(sitem.nItemTableIdx);
        if (null == m_item)
        {
            Debug.LogError("RealItem::SetItem() [ null== client item ] item id : " + sitem.nItemTableIdx);
			return;
        }
		
		m_sItem = sitem;
		m_nSlot = iSlot;
		
		m_isExistCooltime = false;
		if( Item.eITEM_TYPE.ActionItem == m_item.ItemData.GetItemType() )
		{
			m_isExistCooltime = true;
		}	
		
		if( Item.eITEM_TYPE.UseItem == m_item.ItemData.GetItemType() )
		{
			Item.eUSE_ITEM subtype = (Item.eUSE_ITEM)m_item.ItemData.GetSubType();
			if( Item.eUSE_ITEM.InfiniteQuest == subtype ||
				Item.eUSE_ITEM.ConsumeQuest == subtype)
				m_isExistCooltime = true;
			
		}
		
		
//		if( isExistCooltime )
//		{
//			m_CoolTimeGroup = CoolTimeGroupMgr.Instance.GetCoolTimeGroup( m_item.ItemData.itemSkill, m_item.ItemData.itemSkillLevel );
//		}
//		else
//		{
//			m_CoolTimeGroup = null;
//		}
	}
	
	
	public bool IsCanCoolTimeActive()
	{
		CoolTimeGroup _group = getCoolTimeGroup;
		if( null == _group )
		{
			return false;
		}
		
		return _group.isCoolTimeActive;		
	}
	
	
		
	private bool IsCanSkillUse( Item _item )	
	{
		if( null == _item )
			return false;
		
		if( _item.ItemData.GetItemType() != Item.eITEM_TYPE.ActionItem )
			return true;
		
		Tbl_Skill_Record _record = AsTableManager.Instance.GetTbl_Skill_Record( item.ItemData.itemSkill );
		if( null == _record )
		{
			Debug.LogError("RealItem::SendUseItem()[ null == Tbl_Skill_Table ] id : " + item.ItemData.itemSkill);
			return false;
		}
		
		if( 0 >= _record.listSkillPotency.Count )
		{
			Debug.LogError("RealItem::SendUseItem()[listSkillPotency.count == 0 ] id : " + item.ItemData.itemSkill);
			return false;
		}
		
		bool ishpItem = false;
		bool ismpItem = false;
		
		AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
		if( null == userEntity)
			return false;
		
		//	HP
		float maxHP = AsUserInfo.Instance.SavedCharStat.sFinalStatus.fHPMax;
		float curHP = userEntity.GetProperty<float>( eComponentProperty.HP_CUR);
		
		
		// MP
		float maxMP = AsUserInfo.Instance.SavedCharStat.sFinalStatus.fMPMax;
		float curMP = userEntity.GetProperty<float>( eComponentProperty.MP_CUR);
	
		
		foreach( Tbl_Skill_Potency _skillPotency in _record.listSkillPotency )
		{
			if( null == _skillPotency)
				continue;
			
			ePotency_Type _potencyType = _skillPotency.Potency_Type;
			
			if( ePotency_Type.NONE == _potencyType )
				continue;
			
			if( _skillPotency.Potency_Target != ePotency_Target.Self )
				continue;
			
			if( ePotency_Type.HPControl == _potencyType || ePotency_Type.HPControlRatio == _potencyType )	
			{
				ishpItem = true;
			}
			else if( ePotency_Type.MPControl == _potencyType || ePotency_Type.MPControlRatio == _potencyType )
			{ 
				ismpItem = true;
			}
		}
		
		if( ishpItem && ismpItem )
		{
			if( maxHP <= curHP &&
				maxMP <= curMP )
			{
				AsMyProperty.Instance.AlertUseless();              
				return false;
			}
		}
		
		else if( ishpItem )	
		{
			if( maxHP <= curHP )
			{
				AsMyProperty.Instance.AlertUseless();               
				return false;
			}
		}
		else if( ismpItem )
		{
			if( maxMP <= curMP )
			{
				AsMyProperty.Instance.AlertUseless();               
				return false;
			}
		}	
		
		return true;
	}
	
	public bool IsCanUseItem( Item _item )
	{
		if( null == _item )
			return false;		
		
		if( Item.eITEM_TYPE.UseItem == _item.ItemData.GetItemType() )
			return true;
		
		Debug.LogError(" RealItem::IsCanUseItem() [ id: " + _item.ItemID + " [ type: " + _item.ItemData.GetItemType() );
		return false;
	}
	
	public void SendUseItem( bool isUseRandItem = false )
	{				
		if( GAME_STATE.STATE_INGAME !=  AsGameMain.s_gameState )
			return;
		
		if( true == AsHudDlgMgr.Instance.IsOpenTrade || AsHudDlgMgr.Instance.IsOpenTradeRequestMsgBox)
			return;

        if (AsHudDlgMgr.Instance.IsOpenCashStore || AsHudDlgMgr.Instance.IsOpenNpcStore)
            return;
		
		if( true == AsHudDlgMgr.Instance.IsOpenSynthesisDlg)
			return;
		
		if( Item.eITEM_TYPE.UseItem == item.ItemData.GetItemType() && 
			Item.eUSE_ITEM.Summon == (Item.eUSE_ITEM)item.ItemData.GetSubType() )
		{
			if( false == TerrainMgr.Instance.IsCurMapType( eMAP_TYPE.Summon ) )
			{
				 AsMyProperty.Instance.AlertNotInSummon();
				return;
			}				
		}
		
		if( Item.eITEM_TYPE.UseItem == item.ItemData.GetItemType() )
		{
			if( item.ItemData.needClass != AsEntityManager.Instance.GetPlayerCharFsm().UserEntity.GetProperty<eCLASS>(eComponentProperty.CLASS) &&
				( item.ItemData.needClass != eCLASS.All && item.ItemData.needClass != eCLASS.PET)) //$yde
			{
				AsEventNotifyMgr.Instance.CenterNotify.AddTradeMessage( AsTableManager.Instance.GetTbl_String(13));
				return;
			}
		}
		
		int iCurLevel = AsEntityManager.Instance.GetPlayerCharFsm().UserEntity.GetProperty<int>(eComponentProperty.LEVEL);
        if (iCurLevel < item.ItemData.levelLimit)
		{
			AsEventNotifyMgr.Instance.CenterNotify.AddTradeMessage( AsTableManager.Instance.GetTbl_String(1651));
			return;
		}
		
		if( true == IsCanCoolTimeActive() )
			return;
		
		if( false == IsCanSkillUse( item ) )
			return;
		
		if( true == AsPStoreManager.Instance.IsCheckPStorUseItem( this ) )
			return;
		
		if( false == IsCanUseItem( item ) )
			return;
		
		// chatting item
		if( Item.eITEM_TYPE.UseItem == item.ItemData.GetItemType()
			&& ( Item.eUSE_ITEM.ChatChannel == (Item.eUSE_ITEM)item.ItemData.GetSubType() || Item.eUSE_ITEM.ChatServer == (Item.eUSE_ITEM)item.ItemData.GetSubType()))
		{
			AsHudDlgMgr.Instance.OpenChatServer( getSlot);
			return;
		}

		//	change name character item  
		if( Item.eITEM_TYPE.UseItem == item.ItemData.GetItemType() && Item.eUSE_ITEM.CharacterNameReset == (Item.eUSE_ITEM)item.ItemData.GetSubType() )
		{
			Map map = TerrainMgr.Instance.GetCurrentMap();
			eMAP_TYPE mapType = map.MapData.getMapType;

			if( eMAP_TYPE.Town == mapType)
			{
				AsHudDlgMgr.Instance.OpenReName( getSlot , AsReNameDlg.eReNameType.Character );	
			}
			else
			{
				AsEventNotifyMgr.Instance.CenterNotify.AddMessage(AsTableManager.Instance.GetTbl_String(1890));
			}
			return;
		}

		//	change name guild item  
		if( Item.eITEM_TYPE.UseItem == item.ItemData.GetItemType() && Item.eUSE_ITEM.GuildNameReset == (Item.eUSE_ITEM)item.ItemData.GetSubType() )
		{
			Map map = TerrainMgr.Instance.GetCurrentMap();
			eMAP_TYPE mapType = map.MapData.getMapType;

			if( eMAP_TYPE.Town == mapType)
			{
				if( AsUserInfo.Instance.GuildData == null )
				{
					AsEventNotifyMgr.Instance.CenterNotify.AddMessage(AsTableManager.Instance.GetTbl_String(1884));
					return;
				}
				
				if( AsUserInfo.Instance.GuildData.szGuildMaster != AsUserInfo.Instance.SavedCharStat.charName_ )
				{
					AsEventNotifyMgr.Instance.CenterNotify.AddMessage(AsTableManager.Instance.GetTbl_String(1884));
					return;
				}
				
				AsHudDlgMgr.Instance.OpenReName( getSlot , AsReNameDlg.eReNameType.Guild );
			}
			else
			{
				AsEventNotifyMgr.Instance.CenterNotify.AddMessage(AsTableManager.Instance.GetTbl_String(1890));
			}
			return;
		}

		//$ yde - pet
		if( Item.eITEM_TYPE.UseItem == item.ItemData.GetItemType())
		{
			if(Item.eUSE_ITEM.PetEgg == (Item.eUSE_ITEM)item.ItemData.GetSubType() ) // egg
			{
				string title = AsTableManager.Instance.GetTbl_String( 126);
				string content = "not set";

				bool indun = TerrainMgr.Instance.IsCurMapType(eMAP_TYPE.Indun) == true;
				bool pvp = TerrainMgr.Instance.IsCurMapType(eMAP_TYPE.Pvp) == true;
				if(indun == true || pvp == true)
				{
					content = AsTableManager.Instance.GetTbl_String(2739);
					AsHudDlgMgr.Instance.SetMsgBox(AsNotify.Instance.MessageBox( title, content, AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_QUESTION));
					return;
				}

				if(AsEntityManager.Instance.UserEntity != null &&
				   AsEntityManager.Instance.UserEntity.GetProperty<bool>(eComponentProperty.COMBAT) == true)
				{
					content = AsTableManager.Instance.GetTbl_String(2738);
					AsHudDlgMgr.Instance.SetMsgBox(AsNotify.Instance.MessageBox( title, content, AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_QUESTION));
					return;
				}
			}

			if(Item.eUSE_ITEM.PetFood == (Item.eUSE_ITEM)item.ItemData.GetSubType()) // food
			{
				if(AsPetManager.Instance.PetHungry == ePET_HUNGRY_STATE.ePET_HUNGRY_STATE_ENABLE_POPUP)
				{
					AsPetManager.Instance.PetFoodProc(this);
					return;
				}
			}
		}

		//	ImageGet Item(Portrait)
		if( Item.eITEM_TYPE.UseItem == item.ItemData.GetItemType() && Item.eUSE_ITEM.ImageGet == (Item.eUSE_ITEM)item.ItemData.GetSubType() )
		{
			AsDelegateImageManager.Instance.UseDelegateGetItem( m_nSlot );
			return;
		}

		if( true == AsCommonSender.SendUseItem( getSlot ) && true == isUseRandItem )
		{
			AsHudDlgMgr.Instance.CloseRandItemUI();
			AsHudDlgMgr.Instance.useRandItemIdx = item.ItemID;
		}	
	}
	
	public bool CheckSkillUsable()//$yde
	{
		return IsCanSkillUse(item);
	}
}
