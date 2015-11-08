using UnityEngine;
using System.Collections;

public class AsSynthesisOptionDlg : AsSynthesisBaseDlg 
{	  
	public UIButton btnReset;
	public SpriteText txtDest;
	public SpriteText txtMatCount;
	public EnchantSlot _enchantSlot;
	public SpriteText targetItemName;
	Tbl_SynOptionChange_Record m_record; 
	
	
	
	public override void Open ()
	{
		base.Open ();
		SetEnableApply(false);
		targetItemName.Text = string.Empty;
	}
	
	private void CancelBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{		
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			AsHudDlgMgr.Instance.CloseSynOptionDlg();
		}
	}
	
	private void ApplyBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{		
			if( true == actionSlider.isStop )
			{
				AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
				actionSlider.ActionStart();	
				SetEnableApply(false);
				SetPlayEffect(false);
			}
		}
	}
	
	protected void SetTargetItemName( Item _item )
	{
		if( null == targetItemName )
			return;
		
		targetItemName.Text = AsTableManager.Instance.GetTbl_String( _item.ItemData.nameId );
	}
	
	public override bool SetInputUpRealItem( RealItem _realItem, Ray inputRay )
	{
		if( null == _realItem )
			return false;
		
		if( false == actionSlider.isStop )
			return false;
			
		if( true == AsCommonSender.isSendItemMix )
			return false;
		
		if( true == enchantSlots[0].IsIntersect(inputRay) )
		{
			if( false == IsCheckItemType( _realItem.item ) )				
			{
				AsNotify.Instance.MessageBox(
					AsTableManager.Instance.GetTbl_String(126), 
					AsTableManager.Instance.GetTbl_String(101), 
					AsNotify.MSG_BOX_TYPE.MBT_OK, 
					AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			}
			else			
			{
				ResetSlotMoveLock();
				SetItemInSlot( enchantSlots[0], _realItem );				
				ResetNeedGold();
				targetItemName.Text = string.Empty;
				targetEnchantSlot.DeleteSlotItem();			
			}
			return true;
		}	
		
		
		return false;
	}
	
	public override bool SetDClickRealItem( RealItem _realItem )
	{
		if( null == _realItem )
			return false;
		
		if( false == actionSlider.isStop )
			return false;
			
		if( true == AsCommonSender.isSendItemMix )
			return false;
		
		if( false == IsCheckItemType( _realItem.item ) )				
		{
			AsNotify.Instance.MessageBox( 
				AsTableManager.Instance.GetTbl_String(126), 
				AsTableManager.Instance.GetTbl_String(101), 
				AsNotify.MSG_BOX_TYPE.MBT_OK, 
				AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			
			return true;
		}
		else			
		{
			ResetSlotMoveLock();
			SetItemInSlot( enchantSlots[0], _realItem );			
			ResetNeedGold();
			targetItemName.Text = string.Empty;
			targetEnchantSlot.DeleteSlotItem();		
			return true;
		}
		return false;
	}
	
	
	protected void ResetNeedGold()
	{
		if( null == enchantSlots[0].slotItem )
		{
			SetNeedGold(0);			
			return;
		}		
		
		if( null == m_record)		
			return;	
		
		int iHaveCount = 0;
		Item _item = null;
		if( IsUseEmerald( enchantSlots[0].slotItem.realItem.item.ItemData.GetItemType(), enchantSlots[0].slotItem.realItem.item.ItemData.grade ) )
		{
			iHaveCount = ItemMgr.HadItemManagement.Inven.GetEmeraldItemTotalCount();
			int _itemidx = ItemMgr.HadItemManagement.Inven.GetEmeraldItemID(0);
			_item = ItemMgr.ItemManagement.GetItem( _itemidx );
			if( null == _item )
			{
				Debug.LogError("AsSynthesisOptionDlg()::ResetNeedGold()[ item not find : " + _itemidx);
				return;
			}
			
			ItemMgr.HadItemManagement.Inven.SetRealItemMoveLock(_itemidx, true );
			ItemMgr.HadItemManagement.Inven.SetRealItemMoveLock(ItemMgr.HadItemManagement.Inven.GetEmeraldItemID(1), true );
		}
		else
		{
			iHaveCount = ItemMgr.HadItemManagement.Inven.GetSapphireItemTotalCount();
			int _itemidx = ItemMgr.HadItemManagement.Inven.GetSapphireItemID(0);
			_item = ItemMgr.ItemManagement.GetItem( _itemidx );
			if( null == _item )
			{
				Debug.LogError("AsSynthesisOptionDlg()::ResetNeedGold()[ item not find : " + _itemidx);
				return;
			}
			
			ItemMgr.HadItemManagement.Inven.SetRealItemMoveLock(_itemidx, true );
			ItemMgr.HadItemManagement.Inven.SetRealItemMoveLock(ItemMgr.HadItemManagement.Inven.GetSapphireItemID(1), true );
		}	
		
		System.Text.StringBuilder _sb = new System.Text.StringBuilder();
		_sb.Append( iHaveCount );
		_sb.Append( "/" );
		_sb.Append( m_record.stuffCount );
		
		
		SetNeedGold(m_record.cost);
		_enchantSlot.CreateSlotItem( _item, _enchantSlot.transform );
		_enchantSlot.ResetSlotItemLocalPosition(-0.5f);
		txtMatCount.Text = _sb.ToString();
		txtEnchantSlotName[1].Text = AsTableManager.Instance.GetTbl_String( _item.ItemData.nameId );
		
		
		if( AsHudDlgMgr.Instance.IsOpenInven )
			AsHudDlgMgr.Instance.invenDlg.ApplySlotMoveLock();
		
		if( iHaveCount >= m_record.stuffCount && m_record.cost <= (int)AsUserInfo.Instance.SavedCharStat.nGold )
			SetEnableApply(true);
		else			
			SetEnableApply(false);
	}
	
	
	
	protected bool IsCheckItemType( Item _item )
	{
		if( null == _item )
		{
			Debug.LogError("AsSynthesisEnchantDlg::IsCheckItemType()[ null == _item ]");
			return false;
		}
		
		if( Item.eITEM_TYPE.EquipItem != _item.ItemData.GetItemType() )
			return false;
		
		if( true == _item.ItemData.isItem_OptionType )
			return false;		
		
		if( Item.eUSE_TIME_TYPE.NONE != _item.ItemData.m_eUseTimeType )
			return false;
		
		int _level = _item.ItemData.levelLimit;
		Item.eGRADE _grade = _item.ItemData.grade;
		Item.eEQUIP _equip = (Item.eEQUIP)_item.ItemData.GetSubType();	
		
		m_record = AsTableManager.Instance.GetSynOptionChangeRecord( _level, _grade, _equip );
		if( null == m_record )
			return false;	
		
		return true;
	}
	
	
	protected override void SendPacket()
	{
		if( null == enchantSlots[0].slotItem )
		{	
			Debug.LogError("AsSynthesisOptionDlg::SendPacket()[null == enchantSlots[0].slotItem]");
			return;
		}
				
		AsCommonSender.SendOptionItemMix( (short)enchantSlots[0].slotItem.realItem.getSlot );
	}
	
	public override void ReceivePacket( body_SC_ITEM_MIX_RESULT _result)
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6038_OptionChange_Complete", Vector3.zero, false);
		ResetSlotMoveLock();
		enchantSlots[0].DeleteSlotItem();
		_enchantSlot.DeleteSlotItem();
		txtMatCount.Text = string.Empty;
		SetEnableApply(false);
		SetPlayEffect(true);
		txtNeedGold.Text = string.Empty;
		txtMatCount.Text = string.Empty;	
		txtEnchantSlotName[1].Text = string.Empty;
		
		RealItem _realItem = ItemMgr.HadItemManagement.Inven.GetRealItemInSlot(_result.nResultSlot1);
		if( null != _realItem )
		{
			targetEnchantSlot.DeleteSlotItem();
			targetEnchantSlot.CreateSlotItem( _realItem, targetEnchantSlot.transform );
			targetEnchantSlot.ResetSlotItemLocalPosition(-0.5f);
			if( null != targetEnchantSlot.slotItem )
			{
				targetEnchantSlot.slotItem.iconImg.SetSize( m_iconSize, m_iconSize );			
			}
			SetTargetItemName( _realItem.item );
		}
		
	}
	
	public override void GuiInputUp(Ray inputRay)
	{ 	
		if( false == m_bInputDown )
			return;
		
		base.GuiInputUp(inputRay);	
		
		if( true == _enchantSlot.IsIntersect(inputRay) )
		{
			if( null != _enchantSlot.getItem )
			{
				TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.normal, _enchantSlot.getItem );
			}
			return;
		}		
	}
	
	private void ResetBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{	
			if( false == actionSlider.isStop )
				return;
			
			if( true == AsCommonSender.isSendItemMix )
				return;
			
			SetEnableApply(false);
			txtEnchantSlotName[1].Text = string.Empty;
			targetItemName.Text = string.Empty;
			txtMatCount.Text = string.Empty;
			txtNeedGold.Text = string.Empty;
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			ResetSlotMoveLock();
			enchantSlots[0].DeleteSlotItem();
			_enchantSlot.DeleteSlotItem();
			targetEnchantSlot.DeleteSlotItem();
			
		}
	}
	
	// Use this for initialization
	void Start () 
	{
		txtTitle.Text = AsTableManager.Instance.GetTbl_String(2144);
		txtDest.Text = AsTableManager.Instance.GetTbl_String(2145);
		txtEnchantSlotName[0].Text = AsTableManager.Instance.GetTbl_String(2146);
		txtEnchantSlotName[1].Text = string.Empty;
		btnApply.spriteText.Text = AsTableManager.Instance.GetTbl_String(2147);
		btnApply.SetInputDelegate(ApplyBtnDelegate);
		btnClose.SetInputDelegate(CancelBtnDelegate);
		txtMatCount.Text = string.Empty;	
		targetItemName.Text = string.Empty;
		txtNeedGold.Text = string.Empty;		
		btnReset.spriteText.Text = AsTableManager.Instance.GetTbl_String(1221);
		btnReset.SetInputDelegate(ResetBtnDelegate);
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
