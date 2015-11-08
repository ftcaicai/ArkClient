using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class AsSynthesisEnchantDlg : AsSynthesisBaseDlg 
{	
	public UIButton btnReset;
	public SpriteText txtDest;	
	public SpriteText txtTargetName;	
	
	public override void Open()
	{
		base.Open();		
		actionSlider.Reset();
		SetEnableApply(false);
		
	}
	
	public override void Close()
	{
		base.Close();
	}
		
	protected void ResetNeedGold()
	{
		if( null == enchantSlots[0].slotItem )
		{
			SetNeedGold(0);
			return;
		}
		
		if( null == enchantSlots[1].slotItem )
		{
			SetNeedGold(0);
			return;
		}
		
		int level_1 = enchantSlots[0].slotItem.realItem.item.ItemData.levelLimit;
		int level_2 = enchantSlots[1].slotItem.realItem.item.ItemData.levelLimit;
		
		Item.eGRADE grade_1 = enchantSlots[0].slotItem.realItem.item.ItemData.grade;
		Item.eGRADE grade_2 = enchantSlots[1].slotItem.realItem.item.ItemData.grade;
		
		Tbl_SoulStoneEnchant_Record soulRecord_1 = AsTableManager.Instance.GetSoulStoneEnchantTable( enchantSlots[0].slotItem.realItem.item.ItemID );
		if( null == soulRecord_1)		
			return;
		
		
		Tbl_SoulStoneEnchant_Record soulRecord_2 = AsTableManager.Instance.GetSoulStoneEnchantTable( enchantSlots[1].slotItem.realItem.item.ItemID );
		if( null == soulRecord_2)
			return;
		
		Tbl_SoulStoneEnchant_Record.eTYPE type_1 = soulRecord_1.getEquipType;
		Tbl_SoulStoneEnchant_Record.eTYPE type_2 = soulRecord_2.getEquipType;
		
		Tbl_SynMixEnchant_Record _record 
			= AsTableManager.Instance.GetSynMixEnchantRecord( 
				EnchantDlg.IsSkillEnchant( enchantSlots[0].slotItem.realItem.item ), level_1, grade_1, type_1, level_2, grade_2, type_2 );
		if( null == _record )
			return;
		
		SetNeedGold(_record.cost);
		
		if( null == enchantSlots[0].slotItem || null == enchantSlots[1].slotItem || _record.cost > (int)AsUserInfo.Instance.SavedCharStat.nGold )
			SetEnableApply(false);
		else
			SetEnableApply(true);
	}
	
	private void CancelBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{		
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			AsHudDlgMgr.Instance.CloseSynEnchantDlg();
		}
	}
	
	private void ApplyBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{		
			if( true == actionSlider.isStop )
			{
				if( 1 > ItemMgr.HadItemManagement.Inven.GetEmptyInvenSlotCount() )
				{
					AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(118),
							null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
					return;
				}
				
				AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
				actionSlider.ActionStart();
				SetEnableApply(false);
				SetPlayEffect(false);
			}
		}
	}
	
	public override void ReceivePacket( body_SC_ITEM_MIX_RESULT _result)
	{		
		ResetSlotMoveLock();
		
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6039_Mixenchant_Complete", Vector3.zero, false);
		enchantSlots[0].DeleteSlotItem();
		enchantSlots[1].DeleteSlotItem();
		txtNeedGold.Text = string.Empty;
		
		RealItem _realItem = ItemMgr.HadItemManagement.Inven.GetRealItemInSlot(_result.nResultSlot1);
		if( null == _realItem )
		{
			Debug.LogError("AsSynthesisEnchantDlg::ReceivePacket[null == _realItem] : " + _result.nResultSlot1 );
			return;
		}
		
		targetEnchantSlot.DeleteSlotItem();
		targetEnchantSlot.CreateSlotItem( _realItem, targetEnchantSlot.transform );
		targetEnchantSlot.ResetSlotItemLocalPosition(-0.5f);
		if( null != targetEnchantSlot.slotItem )
		{
			targetEnchantSlot.slotItem.iconImg.SetSize( m_iconSize, m_iconSize );			
		}
		
		SetPlayEffect(true);
	}
	
	
	protected override void SendPacket()
	{
		if( null == enchantSlots[0].slotItem || null == enchantSlots[1].slotItem )
		{	
			Debug.LogError("AsSynthesisEnchantDlg::SendPacket()[null == enchantSlots[0].realItem || null == enchantSlots[1].realItem]");
			return;
		}
		
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6012_EFF_StrengthenSuccess", Vector3.zero, false);
		AsCommonSender.SendEnchantItemMix( (short)enchantSlots[0].slotItem.realItem.getSlot, (short)enchantSlots[1].slotItem.realItem.getSlot );
	}

	
	public override bool SetInputUpRealItem( RealItem _realItem, Ray inputRay )
	{
		if( null == _realItem )
			return false;
		
		if( false == actionSlider.isStop )
			return false;
			
		if( true == AsCommonSender.isSendItemMix )
			return false;
		
		
		int i=0;
		foreach( UIInvenSlot _slot in enchantSlots )
		{
			if( true == _slot.IsIntersect( inputRay ) )
			{
				if( false == IsCheckItemType(_realItem.item ) || 
					false == IsCheckSkillEnchant(0, _realItem.item) || 
					false == IsCheckSkillEnchant(1, _realItem.item))
				{
					AsNotify.Instance.MessageBox( 
						AsTableManager.Instance.GetTbl_String(126), 
						AsTableManager.Instance.GetTbl_String(101), 
						AsNotify.MSG_BOX_TYPE.MBT_OK, 
						AsNotify.MSG_BOX_ICON.MBI_NOTICE);
					return false;
				}
				
				if( null != _slot.slotItem )
				{
					if( null != _slot.slotItem.realItem )
					{
						ItemMgr.HadItemManagement.Inven.SetSlotMoveLock( _slot.slotItem.realItem.getSlot, false );
					}
				}
				
				SetItemInSlot( _slot, _realItem );
				if( null != _slot.slotItem && null != _slot.slotItem.itemCountText )
					_slot.slotItem.itemCountText.Text = "1";
				ResetNeedGold();
				++i;
				return true;
			}
		}
		
		return false;
	}
	
	public virtual bool SetDClickRealItem( RealItem _realItem )
	{
		if( null == _realItem )
			return false;	
		
		if( false == actionSlider.isStop )
			return false;
			
		if( true == AsCommonSender.isSendItemMix )
			return false;
		
		if( false == IsCheckItemType(_realItem.item ) || 
			false == IsCheckSkillEnchant(0, _realItem.item) || 
			false == IsCheckSkillEnchant(1, _realItem.item))
		{
			
			AsNotify.Instance.MessageBox( 
				AsTableManager.Instance.GetTbl_String(126), 
				AsTableManager.Instance.GetTbl_String(101), 
				AsNotify.MSG_BOX_TYPE.MBT_OK, 
				AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			return false;
		}
	
		SetItemInSlot(_realItem);
		ResetNeedGold();
		return true;
	}
	
	protected override void SetItemInSlot( RealItem _realItem )
	{
		UIInvenSlot _slot = null;
		
		if( null == enchantSlots[0].slotItem ) 	 
		{
			_slot = enchantSlots[0];		
		}
		else if( null == enchantSlots[1].slotItem ) 	 
		{
			_slot = enchantSlots[1];			
		}
		else
		{
			_slot = enchantSlots[0];
		}
		
		if( null != _slot.slotItem )
		{
			if( null != _slot.slotItem.realItem )
			{
				ItemMgr.HadItemManagement.Inven.SetSlotMoveLock( _slot.slotItem.realItem.getSlot, false );
			}
		}
		
		SetItemInSlot( _slot, _realItem );
		if( null != _slot.slotItem && null != _slot.slotItem.itemCountText )
			_slot.slotItem.itemCountText.Text = "1";
	}
	
	protected bool IsCheckItemType( Item _item )
	{
		if( null == _item )
		{
			Debug.LogError("AsSynthesisEnchantDlg::IsCheckItemType()[ null == _item ]");
			return false;
		}
		
		if( Item.eUSE_TIME_TYPE.NONE != _item.ItemData.m_eUseTimeType )
			return false;
		
		if( Item.eITEM_TYPE.EtcItem != _item.ItemData.GetItemType() )
			return false;
		
		if( Item.eEtcItem.Enchant != (Item.eEtcItem)_item.ItemData.GetSubType() )
			return false;	
		
		if( false == _item.ItemData.m_Item_MixEnchant )
		{
			return false;
		}
		
		return true;
	}
	
	protected bool IsCheckSkillEnchant( int islot, Item _item )
	{
		if( enchantSlots.Length <= islot )
			return false;
		
		bool isHaveItemSkillEnchat = false;
		if( null != enchantSlots[islot].slotItem ) 	 
		{
			isHaveItemSkillEnchat = EnchantDlg.IsSkillEnchant( enchantSlots[islot].slotItem.realItem.item );
		}	
		else
		{
			return true;
		}
		
		bool isNewItemSkillEnchant = EnchantDlg.IsSkillEnchant( _item );
		if( isHaveItemSkillEnchat != isNewItemSkillEnchant )
		{
			return false;
		}	
		
		return true;
	}
	
	private void ResetBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{		
			if( false == actionSlider.isStop )
				return;
			
			if( true == AsCommonSender.isSendItemMix )
				return;
			
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			
			ResetSlotMoveLock();		
	
			txtNeedGold.Text = string.Empty;
			enchantSlots[0].DeleteSlotItem();
			enchantSlots[1].DeleteSlotItem();		
			targetEnchantSlot.DeleteSlotItem();
			SetEnableApply(false);
		}
	}
	
	// Use this for initialization
	void Start () 
	{
		txtTitle.Text = AsTableManager.Instance.GetTbl_String(2150);
		txtDest.Text = AsTableManager.Instance.GetTbl_String(2151);
		txtEnchantSlotName[0].Text = AsTableManager.Instance.GetTbl_String(2152);
		txtEnchantSlotName[1].Text = AsTableManager.Instance.GetTbl_String(2153);
		btnApply.spriteText.Text = AsTableManager.Instance.GetTbl_String(2154);
		btnApply.SetInputDelegate(ApplyBtnDelegate);
		btnClose.SetInputDelegate(CancelBtnDelegate);
		txtTargetName.Text = string.Empty;
		txtNeedGold.Text = string.Empty;
		btnReset.spriteText.Text = AsTableManager.Instance.GetTbl_String(1221);
		btnReset.SetInputDelegate(ResetBtnDelegate);
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
