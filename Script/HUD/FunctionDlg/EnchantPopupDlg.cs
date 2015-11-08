using UnityEngine;
using System.Collections;

public class EnchantPopupDlg : MonoBehaviour 
{
	public SpriteText textTitle;
	
	public EnchantSlot enchantSlot;
	public SpriteText textEnchantSlot;
	
	public UIInvenSlot invenSlot;
	public SpriteText textInvenSlot;
	
	public SpriteText textText;
	  
	public UIButton btnOk;
	public SpriteText textOk;
	
	public UIButton btnCancel;
	public SpriteText textCancel;
	
	public GameObject goItemParent;   
	
	private int m_iEquipSlotIdx = 0;
	private byte m_iEnchantPos = 0;
	
	
	public void Open( int iEquipSlotIdx, byte enchantSlotPos, Item _item, RealItem _realItem, byte uiEnchantSlotPos ) 
	{
		m_iEquipSlotIdx = iEquipSlotIdx;
		m_iEnchantPos = enchantSlotPos;
		if( null != _item )
		{
			textText.Text = AsTableManager.Instance.GetTbl_String(210);
		}
		else
		{
			textText.Text = AsTableManager.Instance.GetTbl_String(74);
		}
		
		if( 0 == uiEnchantSlotPos )
		{
			textEnchantSlot.Text = AsTableManager.Instance.GetTbl_String(1358);		
		}
		else
		{
			textEnchantSlot.Text = AsTableManager.Instance.GetTbl_String(1359);
		}
		
		invenSlot.DeleteSlotItem();
		invenSlot.CreateSlotItem( _realItem, goItemParent.transform );
		
		enchantSlot.DeleteSlotItem();	
		if( null != _item )
			enchantSlot.CreateSlotItem( _item, goItemParent.transform );
	}
	
	public void GuiInputUp(Ray inputRay)
	{ 		
		if( true == invenSlot.IsIntersect(inputRay) )
		{
			if( null != invenSlot.slotItem )
			{
				TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.normal, invenSlot.slotItem.realItem );
			}
			return;
		}	
		
		
		if( true == enchantSlot.IsIntersect( inputRay ) )
		{
			if( null != enchantSlot.getItem )
			{
				TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.normal, enchantSlot.getItem );
			}	
			return;
		}		
		
	}
	

	public void Close()
	{
		invenSlot.DeleteSlotItem();
		enchantSlot.DeleteSlotItem();
	}
	
	private void ColseBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{		
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			if( AsHudDlgMgr.Instance.IsOpenEnchantDlg )
			{
				AsHudDlgMgr.Instance.enchantDlg.CloseEnchantPopupDlg();
			}
		}
	}
	
	private void OkBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			if( null == invenSlot.slotItem )
			{
				Debug.LogError("EnchantPopupDlg::OkBtnDeleget()[ null == invenslot.SlotItem ]");
				return;
			}
			
			if( false == AsUserInfo.Instance.IsEquipChaingEnable() )
			{
				if( false == AsHudDlgMgr.Instance.isOpenMsgBox )
				{					
					AsHudDlgMgr.Instance.SetMsgBox(  AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(1633),
									null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE) );
				}
				
				return;
			}
			
			AsCommonSender.SendItemEnchant( (short)m_iEquipSlotIdx, (short)invenSlot.slotItem.realItem.getSlot, m_iEnchantPos );			
			AsHudDlgMgr.Instance.enchantDlg.CloseEnchantPopupDlg();
		}
	}
	
	
	// Use this for initialization
	void Start () 
	{
		textTitle.Text = AsTableManager.Instance.GetTbl_String(1360);	
		
		textOk.Text = AsTableManager.Instance.GetTbl_String(1152);
		textCancel.Text = AsTableManager.Instance.GetTbl_String(1151);
		
		textInvenSlot.Text = AsTableManager.Instance.GetTbl_String(1300);
		
		
		btnOk.SetInputDelegate(OkBtnDelegate);
		btnCancel.SetInputDelegate(ColseBtnDelegate);
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
