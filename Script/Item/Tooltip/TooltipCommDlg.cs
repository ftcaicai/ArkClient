using UnityEngine;
using System.Collections;
using System.Globalization;


public class TooltipCommDlg : TooltipDlg
{
	public SpriteText sellPrice;
	public SpriteText contentText_1;
	public SpriteText contentText_2;
	public UIButton btnButton;
	public UIButton btnSocket;
	public UIButton btnStrength;
	public UIButton btnBuy;
	public SpriteText textBtnTitle;
	public SimpleSprite imgGold;
	public SpriteText tradeCount;

	private MonoBehaviour m_script;
	private string m_method;
	private RealItem m_RealItemp;


	public UIButton btnEquip;
	public UIButton btnUse;


	private System.Text.StringBuilder m_sbtemp = new System.Text.StringBuilder();

	protected void SetTradeCount( sbyte _count)
	{
		if( null == tradeCount)
			return;

		if( -1 == _count)
			tradeCount.Text = string.Empty;
		else if( 0 == _count)
			tradeCount.Text = string.Empty;
		else
			tradeCount.Text = string.Format( AsTableManager.Instance.GetTbl_String(4047), _count);
	}
	
	private void SetEnableStrength( bool isEnable )
	{
		if( null == btnStrength)
			return;
	
		if( false == isEnable )
		{
			btnStrength.spriteText.Color = Color.gray;			
		}
		else
		{
			btnStrength.spriteText.Color = Color.black;	
		}
		
		btnStrength.controlIsEnabled = isEnable;
	}
	
	private void SetEnableEnchant( bool isEnable )
	{
		if( null == btnSocket)
			return;
	
		if( false == isEnable )
		{
			btnSocket.spriteText.Color = Color.gray;			
		}
		else
		{
			btnSocket.spriteText.Color = Color.black;	
		}
		
		btnSocket.controlIsEnabled = isEnable;
	}
	
	private void SetEnableEquip( bool isEnable )
	{
		if( null == btnEquip)
			return;
	
		if( false == isEnable )
		{
			btnEquip.spriteText.Color = Color.gray;			
		}
		else
		{
			btnEquip.spriteText.Color = Color.black;	
		}
		
		btnEquip.controlIsEnabled = isEnable;
	}
	
	
	
	
	public void Open( RealItem _item, MonoBehaviour btnScript, string btnMethod, TooltipMgr.eCommonState commonState)
	{
		if( false == SetItem( _item.item))
			return;

		m_RealItemp = _item;

		m_script = btnScript;
		m_method = btnMethod;

		//dopamin
		string strGold = string.Empty;
		int Amount = 0;
		switch( commonState)
		{
		case TooltipMgr.eCommonState.Equip:
			strGold = AsTableManager.Instance.GetTbl_String(1062);
			btnButton.gameObject.SetActiveRecursively( false);
			Set3BtnShow( true );
			SetUseBtnState( _item.item, _item.sItem);
			Amount = getItem.ItemData.sellAmount;	
			
			SetEnableStrength(false);			
			SetEnableEnchant(false);
			SetEquipBtnState(_item); 
			break;
		case TooltipMgr.eCommonState.NONE:
			Set3BtnShow( false);
			SetBtnUseShow( false);
			btnButton.gameObject.SetActiveRecursively( false);
			break;
		case TooltipMgr.eCommonState.Buy:
			strGold = AsTableManager.Instance.GetTbl_String(1290);
			textBtnTitle.Text = AsTableManager.Instance.GetTbl_String(1145);
			Set3BtnShow( false);
			SetBtnUseShow( false);
			Amount = getItem.ItemData.buyAmount;
			break;
		case TooltipMgr.eCommonState.Sell:
			strGold = AsTableManager.Instance.GetTbl_String(1062);
			btnButton.gameObject.SetActiveRecursively( false);
			Set3BtnShow( false);
			SetUseBtnState( _item.item, _item.sItem);
			Amount = getItem.ItemData.sellAmount;
			break;
		case TooltipMgr.eCommonState.Sell_Btn:
			strGold = AsTableManager.Instance.GetTbl_String(1062);
			btnButton.spriteText.Text = AsTableManager.Instance.GetTbl_String(1146);			
			Set3BtnShow( false);
			SetBtnUseShow( false);
			Amount = getItem.ItemData.sellAmount;
			break;
		case TooltipMgr.eCommonState.Socket:
			strGold = AsTableManager.Instance.GetTbl_String(1062);
			Amount = getItem.ItemData.sellAmount;
			btnButton.gameObject.SetActiveRecursively( false);
			SetEnableStrength(false);
			SetUseBtnState( _item.item, _item.sItem);
			SetEquipBtnState( _item);
			break;

		case TooltipMgr.eCommonState.Strength:
			strGold = AsTableManager.Instance.GetTbl_String(1062);
			Amount = getItem.ItemData.sellAmount;
			btnButton.gameObject.SetActiveRecursively( false);
			SetEnableEnchant(false);
			SetUseBtnState( _item.item, _item.sItem);
			SetEquipBtnState( _item);
			break;

		case TooltipMgr.eCommonState.Socket_Strength:
			strGold = AsTableManager.Instance.GetTbl_String(1062);
			Amount = getItem.ItemData.sellAmount;
			btnButton.gameObject.SetActiveRecursively( false);
			SetUseBtnState( _item.item, _item.sItem);
			SetEquipBtnState( _item);
			break;
		}

		SetContentText1( _item.sItem, getItem);
		SetContentText2( _item.sItem, getItem);
		SetPrice( Amount, strGold, commonState);
		SetTradeCount( _item.sItem.nTradeCount);

		if( AsHudDlgMgr.Instance.IsOpenedPostBox || 
		   AsHudDlgMgr.Instance.IsOpenSynthesisDlg || 
		   AsHudDlgMgr.Instance.IsOpenEnchantDlg ||
			AsHudDlgMgr.Instance.IsOpenStrengthenDlg || 
		   AsHudDlgMgr.Instance.IsOpenTrade || 
		   AsHudDlgMgr.Instance.IsOpenStorage ||
		   AsHudDlgMgr.Instance.IsOpenSynDisDlg ||
		   AsHudDlgMgr.Instance.IsOpenSynOptionDlg ||
		   AsHudDlgMgr.Instance.IsOpenSynCosDlg ||
		   (null != AsPetManager.Instance.PetSynthesisDlg) || 
			AsPStoreManager.Instance.UnableActionByPStore())
		{
			Set3BtnShow( false);
			SetBtnUseShow( false);
		}
		
		CheckUseBtnPvp( _item.item );
		CheckUseBtnRaid( _item.item );
        CheckUseBtnField(_item.item);
		CheckUseBtnIndun( _item.item);
	}
	
	private void CheckUseBtnPvp( Item _item )
	{
		if( null == btnUse )
			return;
		
		if( false == btnUse.gameObject.active )
			return;
		
		if( null == _item )
			return;
		
		if( false == TargetDecider.CheckCurrentMapIsArena() )
			return;
		
		if( _item.ItemData.GetItemType() != Item.eITEM_TYPE.ActionItem )
			return;
			
		bool isDisableSkill_PvP = false;
		Tbl_Skill_Record skillrecord = AsTableManager.Instance.GetTbl_Skill_Record( _item.ItemData.itemSkill);
		if( null != skillrecord)
			isDisableSkill_PvP = (skillrecord.DisableInPvP == eDisableInPvP.Disable);
		
		if( true == isDisableSkill_PvP )
		{
			SetBtnUseShow( false );
		}
	}
	
	private void CheckUseBtnRaid( Item _item )
	{
		if( null == btnUse )
			return;
		
		if( false == btnUse.gameObject.active )
			return;
		
		if( null == _item )
			return;
		
		if( false == TargetDecider.CheckCurrentMapIsRaid() )
			return;
		
		if( _item.ItemData.GetItemType() != Item.eITEM_TYPE.ActionItem )
			return;
			
		bool isDisableSkill_Raid = false;
		Tbl_Skill_Record skillrecord = AsTableManager.Instance.GetTbl_Skill_Record( _item.ItemData.itemSkill);
		if( null != skillrecord)
			isDisableSkill_Raid = (skillrecord.DisableInRaid == eDisableInRaid.Disable);
		
		if( true == isDisableSkill_Raid )
		{
			SetBtnUseShow( false );
		}
	}

    private void CheckUseBtnField(Item _item)
    {
        if (null == btnUse)
            return;

        if (false == btnUse.gameObject.active)
            return;

        if (null == _item)
            return;

        if (false == TargetDecider.CheckCurrentMapIsField())
            return;

        if (_item.ItemData.GetItemType() != Item.eITEM_TYPE.ActionItem)
            return;

        bool isDisableSkill_Field = false;
        Tbl_Skill_Record skillrecord = AsTableManager.Instance.GetTbl_Skill_Record(_item.ItemData.itemSkill);
        if (null != skillrecord)
            isDisableSkill_Field = (skillrecord.DisableInField == eDisableInRaid.Disable);

        if (true == isDisableSkill_Field)
        {
            SetBtnUseShow(false);
        }
    }

	private void CheckUseBtnIndun( Item _item )
	{
		if( null == btnUse )
			return;
		
		if( false == btnUse.gameObject.active )
			return;
		
		if( null == _item )
			return;
		
		if( false == TargetDecider.CheckCurrentMapIsIndun() )
			return;
		
		if( _item.ItemData.GetItemType() != Item.eITEM_TYPE.ActionItem )
			return;
		
		bool isDisableSkill_Indun = false;
		Tbl_Skill_Record skillrecord = AsTableManager.Instance.GetTbl_Skill_Record( _item.ItemData.itemSkill);
		if( null != skillrecord)
			isDisableSkill_Indun = (skillrecord.DisableInInDun == eDisableInInDun.Disable);
		
		if( true == isDisableSkill_Indun )
		{
			SetBtnUseShow( false );
		}
	}

	public void Open( Item _item, sITEM _sitem, MonoBehaviour btnScript, string btnMethod, TooltipMgr.eCommonState commonState)
	{
		if( false == SetItem( _item))
			return;

		m_script = btnScript;
		m_method = btnMethod;
		//dopamin
		string strGold = string.Empty;
		int Amount = 0;
		switch( commonState)
		{
		case TooltipMgr.eCommonState.Equip:
		case TooltipMgr.eCommonState.NONE:
			Set3BtnShow( false);
			SetBtnUseShow( false);
			btnButton.gameObject.SetActiveRecursively( false);
			Amount = getItem.ItemData.sellAmount;
			break;
		case TooltipMgr.eCommonState.Buy:
			strGold = AsTableManager.Instance.GetTbl_String(1290);
			textBtnTitle.Text = AsTableManager.Instance.GetTbl_String(1145);
			Set3BtnShow( false);
			SetBtnUseShow( false);
			Amount = getItem.ItemData.buyAmount;
			break;
		case TooltipMgr.eCommonState.Sell:
			strGold = AsTableManager.Instance.GetTbl_String(1062);
			btnButton.gameObject.SetActiveRecursively( false);
			Set3BtnShow( false);
			SetBtnUseShow( false);
			Amount = getItem.ItemData.sellAmount;
			break;
		case TooltipMgr.eCommonState.Sell_Btn:
			strGold = AsTableManager.Instance.GetTbl_String(1062);
			btnButton.spriteText.Text = AsTableManager.Instance.GetTbl_String(1146);
			Set3BtnShow( false);
			SetBtnUseShow( false);
			Amount = getItem.ItemData.sellAmount;
			break;
		case TooltipMgr.eCommonState.Socket:
		case TooltipMgr.eCommonState.Strength:
		case TooltipMgr.eCommonState.Socket_Strength:
			Set3BtnShow( false);
			SetBtnUseShow( false);
			strGold = AsTableManager.Instance.GetTbl_String(1062);
			Amount = getItem.ItemData.sellAmount;
			btnButton.gameObject.SetActiveRecursively( false);
			break;
		}

		SetContentText1( _sitem, getItem);
		SetContentText2( _sitem, getItem);
		SetPrice( Amount, strGold, commonState);

		if( null != _sitem)
			SetTradeCount( _sitem.nTradeCount);
		else if( null != getItem)
			SetTradeCount( getItem.ItemData.m_sbItemTradeLimit);
	}

	private void Set3BtnShow( bool _isEnable)
	{
		if( null!= btnEquip)
			btnEquip.gameObject.SetActiveRecursively( _isEnable);

		if( null != btnSocket)
			btnSocket.gameObject.SetActiveRecursively( _isEnable);

		if( null != btnStrength)
			btnStrength.gameObject.SetActiveRecursively( _isEnable);
	}

	private void SetBtnUseShow( bool _isEnable)
	{
		if( null!= btnUse)
			btnUse.gameObject.SetActiveRecursively( _isEnable);
	}

	private void SetEquipBtnState( RealItem _realItem)
	{
		if( null == _realItem)
		{
			Set3BtnShow( false);
			return;
		}
		
		if( _realItem.getSlot < Inventory.useInvenSlotBeginIndex)
			btnEquip.spriteText.Text = AsTableManager.Instance.GetTbl_String(1356);
		else
			btnEquip.spriteText.Text = AsTableManager.Instance.GetTbl_String(961);
	
		if(  (null!=AsUserInfo.Instance.GetCurrentUserEntity()&& AsUserInfo.Instance.GetCurrentUserEntity().IsCheckEquipEnable( _realItem.item ) )
			&& (Item.eITEM_TYPE.EquipItem == _realItem.item.ItemData.GetItemType() || Item.eITEM_TYPE.CosEquipItem == _realItem.item.ItemData.GetItemType()) )
		{
			SetEnableEquip(true);
		}
		else
		{
			SetEnableEquip(false);
		}
	}

	private void SetUseBtnState( Item _item, sITEM _sitem)
	{
		if( null == _sitem || null == _item)
		{
			SetBtnUseShow( false);
			return;
		}

		if( Item.eITEM_TYPE.UseItem == _item.ItemData.GetItemType() ||
			Item.eITEM_TYPE.ActionItem == _item.ItemData.GetItemType())
		{
		}
		else
		{
			if( null!= btnUse)
				btnUse.gameObject.SetActiveRecursively( false);
		}
	}

	public void SetBuyPrice( int iSellAmount)
	{
		if( null != imgGold)
			imgGold.gameObject.active = true;

		if( null == sellPrice)
			return;

		sellPrice.gameObject.active = true;

		m_sbtemp.Remove( 0, m_sbtemp.Length);

		m_sbtemp.Append( AsTableManager.Instance.GetTbl_String(1290));
		m_sbtemp.Append( " ");
		m_sbtemp.Append( iSellAmount.ToString(  "#,#0", CultureInfo.InvariantCulture));
		sellPrice.Text = m_sbtemp.ToString();
	}

	protected void SetPrice( int iSellAmount, string strGold, TooltipMgr.eCommonState commonState)
	{
		if( null != getItem && false == getItem.ItemData.isShopSell && commonState != TooltipMgr.eCommonState.Buy)
		{
			sellPrice.Text = string.Empty;
			if( null != imgGold)
				imgGold.gameObject.active = false;
			return;
		}
		m_sbtemp.Remove( 0, m_sbtemp.Length);
		m_sbtemp.Append( strGold);
		m_sbtemp.Append( " ");
		m_sbtemp.Append( iSellAmount.ToString(  "#,#0", CultureInfo.InvariantCulture));
		sellPrice.Text = m_sbtemp.ToString();
	}

	protected void SetContentText2( sITEM _sitem, Item _item)
	{
		if( null == _item)
		{
			contentText_2.Text = string.Empty;
			return;
		}

		if( Item.eUSE_TIME_TYPE.NONE == _item.ItemData.m_eUseTimeType)
		{
			contentText_2.Text = string.Empty;
			return;
		}

		if( null != _sitem)
		{
			System.DateTime dt = new System.DateTime( 1970, 1, 1, 9, 0, 0);
			dt = dt.AddSeconds( _sitem.nExpireTime);
			contentText_2.Text = string.Format( AsTableManager.Instance.GetTbl_String(384), dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute);
		}
		else
		{
			System.DateTime dt = new System.DateTime( 
				System.DateTime.Now.Year,
				System.DateTime.Now.Month,
				System.DateTime.Now.Day,
				System.DateTime.Now.Hour,
				System.DateTime.Now.Minute,
				System.DateTime.Now.Second);
			dt = dt.AddSeconds( ( long)_item.ItemData.m_iItemUseTime);
			contentText_2.Text = string.Format( AsTableManager.Instance.GetTbl_String(384), dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute);
		}
	}

	protected void SetContentText1( sITEM _sitem, Item _item)
	{
		bool isUseSeal = true;
		bool isUseDump = true;
		bool isUseTrade = true;
		bool isUseStorge = true;
		bool isUseCosSynthesis = true;
		if( null != _sitem)
		{
			/*if( 0 != ( ( sbyte)eITEMATTRIBUTE.eITEMATTRIBUTE_SEAL & _sitem.nAttribute))
				isUseSeal = false;
			if( 0 != ( ( sbyte)eITEMATTRIBUTE.eITEMATTRIBUTE_DUMP & _sitem.nAttribute))
				isUseDump = false;
			if( 0 != ( ( sbyte)eITEMATTRIBUTE.eITEMATTRIBUTE_TRADE_LIMIT & _sitem.nAttribute))
				isUseTrade = false;
			if( 0 != ( ( sbyte)eITEMATTRIBUTE.eITEMATTRIBUTE_STORAGE_LIMIT & _sitem.nAttribute))
				isUseStorge = false;*/

			isUseTrade = _sitem.IsTradeEnable();

		}

		if( null != _item)
		{
			isUseSeal = _item.ItemData.isShopSell;
			isUseDump = !_item.ItemData.isDump;
			isUseStorge = !_item.ItemData.m_bItem_Storage_Limit;
		}

		m_sbtemp.Remove( 0, m_sbtemp.Length);

		bool bcheck = false;
		if ( true == _item.ItemData.isTradeLimit || false == isUseTrade)
		{
			bcheck = true;
			m_sbtemp.Append( AsTableManager.Instance.GetTbl_String(1116));
		}

		if ( false == _item.ItemData.isShopSell || false == isUseSeal)
		{
			if( true == bcheck)
				m_sbtemp.Append( ", ");
			m_sbtemp.Append( AsTableManager.Instance.GetTbl_String(1146));
			bcheck = true;
		}

		if ( true == _item.ItemData.isDump || false == isUseDump)
		{
			if( true == bcheck)
				m_sbtemp.Append( ", ");
			m_sbtemp.Append( AsTableManager.Instance.GetTbl_String(383));
			bcheck = true;
		}

		if ( true == _item.ItemData.m_bItem_Storage_Limit || false == isUseStorge)
		{
			if( true == bcheck)
				m_sbtemp.Append( ", ");
			m_sbtemp.Append( AsTableManager.Instance.GetTbl_String(405));
			bcheck = true;
		}

		if ( _item.ItemData.GetItemType () == Item.eITEM_TYPE.CosEquipItem && _item.ItemData.m_Item_MixEnchant == false) 
		{
			if( true == bcheck)
				m_sbtemp.Append( ", ");
			m_sbtemp.Append( AsTableManager.Instance.GetTbl_String(2427));
			bcheck = true;

			isUseCosSynthesis = false;
		}

		if( true == _item.ItemData.isTradeLimit ||
			false == _item.ItemData.isShopSell ||
			true == _item.ItemData.isDump ||
			true == _item.ItemData.m_bItem_Storage_Limit ||
			false == isUseTrade ||
			false == isUseSeal ||
			false == isUseDump ||
			false == isUseStorge || 
		    false == isUseCosSynthesis )
		{
			m_sbtemp.Append( " ");
			m_sbtemp.Append( AsTableManager.Instance.GetTbl_String(382));
		}

		contentText_1.Text = m_sbtemp.ToString();
	}

	/*protected string GetStrTradeLimit()
	{
		Tbl_String_Record strRecord = AsTableManager.Instance.GetTbl_String_Record( 1049);
		if( null == strRecord)
		{
			Debug.LogError( "TooltipCommDlg::GetTradeLimit() [ null == strRecord ] ");
			return string.Empty;
		}

		return strRecord.String;
	}*/

	/*protected string GetStrDump()
	{
		Tbl_String_Record strRecord = AsTableManager.Instance.GetTbl_String_Record( 1050);
		if( null == strRecord)
		{
			Debug.LogError( "TooltipCommDlg::GetTradeLimit() [ null == strRecord ] ");
			return string.Empty;
		}

		return strRecord.String;
	}*/

	private void ButtonDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if( AsPStoreManager.Instance.storeState != ePStoreState.Closed
				&& AsPStoreManager.Instance.storeState != ePStoreState.Another_Opened)
			{
				AsHudDlgMgr.Instance.SetMsgBox( 
					AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(365), AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_ERROR));
				return;
			}

			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			if( null != m_script && null != m_method)
			{
				if( m_method.Length > 0)
					m_script.Invoke( m_method, 0);
			}

			TooltipMgr.Instance.Clear();
		}
	}

	private void ButtonSocketDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if( null != m_RealItemp)
			{
				if( true == AsInstanceDungeonManager.Instance.CheckInIndun() || true == AsPvpManager.Instance.CheckInArena())
					return;

				if( AsHudDlgMgr.Instance.IsOpenEnchantDlg)
				{
					AsHudDlgMgr.Instance.enchantDlg.SetEquipItem( m_RealItemp);
				}
				else
				{
					AsHudDlgMgr.Instance.OpenEnchantDlg( m_RealItemp);
				}

			}
			TooltipMgr.Instance.Clear();
		}
	}

	private void ButtonStrengthDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if( null != m_RealItemp)
			{
				if( true == AsInstanceDungeonManager.Instance.CheckInIndun() || true == AsPvpManager.Instance.CheckInArena())
					return;

				if( AsHudDlgMgr.Instance.IsOpenStrengthenDlg)
					AsHudDlgMgr.Instance.strengthenDlg.ResetUIInvenItem( m_RealItemp);
				else
				{
					AsHudDlgMgr.Instance.OpenStrengthenDlg();
					if( AsHudDlgMgr.Instance.IsOpenStrengthenDlg)
						AsHudDlgMgr.Instance.strengthenDlg.ResetUIInvenItem( m_RealItemp);
				}
			}
			TooltipMgr.Instance.Clear();
		}
	}

	private void ButtonUseDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if( AsPStoreManager.Instance.UnableActionByPStore() == true)
			{
				AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(365),
					null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE));
				return;
			}
			
			if( null != m_RealItemp)
			{
				if( m_RealItemp.item.ItemData.GetItemType() == Item.eITEM_TYPE.ActionItem)//$yde
				{
					if( m_RealItemp.IsCanCoolTimeActive() == false)
					{
						AsUserEntity user = AsUserInfo.Instance.GetCurrentUserEntity();
						if( null != user)
							user.HandleMessage( new Msg_Player_Use_ActionItem( m_RealItemp));
					}
				}
				else if( m_RealItemp.item.ItemData.GetItemType() == Item.eITEM_TYPE.UseItem && m_RealItemp.item.ItemData.GetSubType() == ( int)Item.eUSE_ITEM.Random)
				{
					AsHudDlgMgr.Instance.OpenRandomItemDlg( m_RealItemp);
				}
				else if (m_RealItemp.item.ItemData.GetItemType() == Item.eITEM_TYPE.UseItem && m_RealItemp.item.ItemData.GetSubType() == (int)Item.eUSE_ITEM.QuestRandom)
				{
					AsHudDlgMgr.Instance.OpenRandomItemDlg(m_RealItemp);
				}
				else if( m_RealItemp.item.ItemData.CheckPStoreOpenItem() == true)
				{
					AsPStoreManager.Instance.ItemUsed( m_RealItemp);
				}
				else if( m_RealItemp.item.ItemData.GetItemType() == Item.eITEM_TYPE.UseItem && m_RealItemp.item.ItemData.GetSubType() == ( int)Item.eUSE_ITEM.ConsumeHair )
				{
					if( true == AsHudDlgMgr.Instance.IsOpenInven )
						AsHudDlgMgr.Instance.invenDlg.SetUseHairItem( m_RealItemp, 109, 2124 );		
				}
				else if (m_RealItemp.item.ItemData.GetItemType() == Item.eITEM_TYPE.UseItem && m_RealItemp.item.ItemData.GetSubType() == (int)Item.eUSE_ITEM.SkillReset)
				{
					AsHudDlgMgr.Instance.invenDlg.ConfirmSkillReset(m_RealItemp);
				}
				else
				{
					m_RealItemp.SendUseItem();
				}
			}
			TooltipMgr.Instance.Clear();
		}
	}

	private void ButtonEquipDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if( null != m_RealItemp)
			{
				if( m_RealItemp.getSlot < Inventory.useInvenSlotBeginIndex && AsHudDlgMgr.Instance.IsOpenPlayerStatus)
				{
					AsHudDlgMgr.Instance.playerStatusDlg.SetEquipToInven( m_RealItemp);
				}
				else
				{
					if( AsHudDlgMgr.Instance.IsOpenInven)
					{
						switch( m_RealItemp.item.ItemData.GetItemType())
						{
						case Item.eITEM_TYPE.CosEquipItem:
							AsHudDlgMgr.Instance.invenDlg.SendDClickCosEquipItem( m_RealItemp);
							break;
						case Item.eITEM_TYPE.EquipItem:
							AsHudDlgMgr.Instance.invenDlg.SendDClickEquipItem( m_RealItemp);
							break;
						}
					}
				}
			}

			TooltipMgr.Instance.Clear();
		}
	}

	void Awake()
	{
		// < ilmeda, 20120822
		sellPrice.Text = "";
		contentText_1.Text = "";
		contentText_2.Text = "";
		AsLanguageManager.Instance.SetFontFromSystemLanguage( sellPrice);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( contentText_1);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( contentText_2);
		// ilmeda, 20120822

		btnButton.SetInputDelegate( ButtonDelegate);
		if( null != btnSocket)
		{
			btnSocket.SetInputDelegate( ButtonSocketDelegate);
			btnSocket.spriteText.Text = AsTableManager.Instance.GetTbl_String(1360);
		}

		if( null != btnStrength)
		{
			btnStrength.SetInputDelegate( ButtonStrengthDelegate);
			btnStrength.spriteText.Text = AsTableManager.Instance.GetTbl_String(1348);
		}

		if( null != btnUse)
		{
			btnUse.SetInputDelegate( ButtonUseDelegate);
			btnUse.spriteText.Text = AsTableManager.Instance.GetTbl_String(962);
		}

		if( null != btnEquip)
		{
			btnEquip.SetInputDelegate( ButtonEquipDelegate);
		}
	}

	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
	}
}
