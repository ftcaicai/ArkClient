using UnityEngine;
using System.Collections;

public class AsDisassembleDlg : AsSynthesisBaseDlg
{
	
	public SpriteText targetItemName;
	
	public UIButton btnReset;
	
	 
	private Tbl_SynDisassemble_Record m_record;
	
	public override void Open ()
	{
		base.Open ();
		targetItemName.Text = string.Empty;
	}
	
	private void CancelBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{		
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			AsHudDlgMgr.Instance.CloseSynDisDlg();
		}
	}
	
	private void ApplyBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{		
			if( true == isMsgBox)
				return;
			
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			
			if( 2 > ItemMgr.HadItemManagement.Inven.GetEmptyInvenSlotCount() )
			{
				AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(118),
						null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
				return;
			}
			
			SetMsgBox( 
				AsNotify.Instance.MessageBox(
					AsTableManager.Instance.GetTbl_String(126), 
					AsTableManager.Instance.GetTbl_String(1852), 
					this, "SetApplyAction", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_WARNING));
		}
	}
	
	public void SetApplyAction()
	{
		if( true == actionSlider.isStop )
		{			
			actionSlider.ActionStart();
			SetEnableApply(false);
			SetPlayEffect(false);	
		}
	}
	
	private void ResetBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{		
			if( true == isMsgBox)
				return;
				
			if( false == actionSlider.isStop )
				return;
			
			if( true == AsCommonSender.isSendItemMix )
				return;
			
			targetItemName.Text = string.Empty;
			txtNeedGold.Text = string.Empty;
			ResetSlotMoveLock();
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			targetEnchantSlot.DeleteSlotItem();
			SetEnableApply(false);
			enchantSlots[0].DeleteSlotItem();
			enchantSlots[1].DeleteSlotItem();
			enchantSlots[2].DeleteSlotItem();
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
		if( true == isMsgBox)
			return false;
				
		if( null == _realItem )
			return false;
		
		if( false == actionSlider.isStop )
			return false;
			
		if( true == AsCommonSender.isSendItemMix )
			return false;
		
		
		
		if( true == targetEnchantSlot.IsIntersect( inputRay ) )
		{
			
			if( false == IsCheckItemType(_realItem ) )
			{
				AsNotify.Instance.MessageBox( 
					AsTableManager.Instance.GetTbl_String(126), 
					AsTableManager.Instance.GetTbl_String(101), 
					AsNotify.MSG_BOX_TYPE.MBT_OK, 
					AsNotify.MSG_BOX_ICON.MBI_NOTICE);
				
				return false;
			}
			
			ResetSlotMoveLock();
			SetItemInSlot( targetEnchantSlot, _realItem );
			if( null != targetEnchantSlot.slotItem )
			{
				targetEnchantSlot.slotItem.iconImg.SetSize( m_iconSize, m_iconSize );			
			}
			ResetNeedGold();
			SetTargetItemName( _realItem.item );
			enchantSlots[0].DeleteSlotItem();
			enchantSlots[1].DeleteSlotItem();
			enchantSlots[2].DeleteSlotItem();
			return true;
		}
		
		
		return false;
	}
	
	
	public virtual bool SetDClickRealItem( RealItem _realItem )
	{
		if( true == isMsgBox)
			return false;
		
		if( null == _realItem )
			return false;
		
		if( false == actionSlider.isStop )
			return false;
			
		if( true == AsCommonSender.isSendItemMix )
			return false;
		
		if( false == IsCheckItemType(_realItem ) )
		{
			AsNotify.Instance.MessageBox( 
				AsTableManager.Instance.GetTbl_String(126), 
				AsTableManager.Instance.GetTbl_String(101), 
				AsNotify.MSG_BOX_TYPE.MBT_OK, 
				AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			return false;
		}
		
		ResetSlotMoveLock();
		SetItemInSlot( targetEnchantSlot, _realItem );
		if( null != targetEnchantSlot.slotItem )
		{
			targetEnchantSlot.slotItem.iconImg.SetSize( m_iconSize, m_iconSize );			
		}
		ResetNeedGold();
		SetTargetItemName( _realItem.item );
		enchantSlots[0].DeleteSlotItem();
		enchantSlots[1].DeleteSlotItem();
		enchantSlots[2].DeleteSlotItem();
		return true;
	}
	
	protected void ResetNeedGold()
	{		
		if( null == m_record)		
			return;		
		
		SetNeedGold( m_record.cost );	
		
		if( (int)AsUserInfo.Instance.SavedCharStat.nGold > m_record.cost )
			SetEnableApply(true);
		else			
			SetEnableApply(false);
	}
	
	protected bool IsCheckItemType( RealItem _realItem )
	{
		if( null == _realItem )
		{
			Debug.LogError("AsDisassembleDlg::IsCheckItemType()[ null == _realItem ]");
			return false;
		}
		
		Item _item = _realItem.item;
		
		if( Item.eITEM_TYPE.EquipItem != _item.ItemData.GetItemType() && Item.eITEM_TYPE.CosEquipItem != _item.ItemData.GetItemType() )
			return false;

		if( Item.eUSE_TIME_TYPE.NONE != _item.ItemData.m_eUseTimeType )
			return false;

		if( false == _item.ItemData.isDisassemble )
			return false;

		m_record = AsTableManager.Instance.GetSynDisassembleRecord( _item.ItemData.levelLimit, 
			_item.ItemData.grade, 
			(Item.eEQUIP)_item.ItemData.GetSubType(), 
			_realItem.sItem.nStrengthenCount, 
			_item.ItemData.m_Item_Disassemble );
		
		if( null == m_record )
			return false;

		return true;
	}
	
	
	protected override void SendPacket()
	{
		if( null == targetEnchantSlot.slotItem )
		{	
			Debug.LogError("AsSynthesisDisassembleDlg::SendPacket()[null == targetEnchantSlot.slotItem]");
			return;
		}
				
		AsCommonSender.SendDecompositionItemMix( (short)targetEnchantSlot.slotItem.realItem.getSlot );
	}
	
	public override void ReceivePacket( body_SC_ITEM_MIX_RESULT _result)
	{		
		SetResultSlot( _result.nResultSlot1, enchantSlots[0] );
		SetResultSlot( _result.nResultSlot2, enchantSlots[1] );
		SetResultSlot( _result.nResultSlot3, enchantSlots[2] );
		targetEnchantSlot.DeleteSlotItem();	
		txtNeedGold.Text = string.Empty;
		targetItemName.Text = string.Empty;
		SetPlayEffect(true);
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6040_EFF_Disassemble_Complete", Vector3.zero, false);
		
	}
	
	protected void SetResultSlot( int _index, UIInvenSlot _slot )
	{
		if( 0 == _index || -1 == _index )
		{
			_slot.DeleteSlotItem();
			return;
		}
		
		RealItem _realItem = ItemMgr.HadItemManagement.Inven.GetRealItemInSlot( _index );
		if( null == _realItem )
		{
			Debug.LogError("AsDisassembleDlg::SetResultSlot()[null == _realItem] id " + _index );
			return;
		}		
		
		_slot.DeleteSlotItem();
		_slot.CreateSlotItem( _realItem, _slot.transform );		
		_slot.ResetSlotItemLocalPosition(-0.5f);
	}
			
	
	// Use this for initialization
	void Start () 
	{
		txtTitle.Text = AsTableManager.Instance.GetTbl_String(1846);		
		txtEnchantSlotName[0].Text = AsTableManager.Instance.GetTbl_String(1847);
		txtEnchantSlotName[1].Text = AsTableManager.Instance.GetTbl_String(1848);
		txtEnchantSlotName[2].Text = AsTableManager.Instance.GetTbl_String(1849);		
		
		
		targetItemName.Text = string.Empty;
		txtNeedGold.Text = string.Empty;
		btnApply.spriteText.Text = AsTableManager.Instance.GetTbl_String(1846);
		btnApply.SetInputDelegate(ApplyBtnDelegate);
		
		btnReset.spriteText.Text = AsTableManager.Instance.GetTbl_String(1221);
		btnReset.SetInputDelegate(ResetBtnDelegate);
		
		btnClose.SetInputDelegate(CancelBtnDelegate);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
