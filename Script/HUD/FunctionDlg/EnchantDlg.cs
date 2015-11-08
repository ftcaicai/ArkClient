using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class EnchantDlg : MonoBehaviour 
{		
	
	public enum eSOCKET_IDX
	{
		LEFT = 0,
		RIGHT,
	}
	public GameObject goItemParent;
	
	public UIButton btnClose;
	
	public UIButton btnLeftSocket;
	public SpriteText textLeftSocketBtn;
	public SpriteText textLeftSocketCash;
	
	public UIButton btnRightSocket;
	public SpriteText textRightSocketBtn;
	public SpriteText textRightSocketCash;
	
	public Collider equipItemCollider;
	public Collider rectCollider;
	
	public UIInvenSlot equipItemSlot;
	public EnchantSlot[] equipItemHaveEnchantItems;
	
	public SpriteText textTitle;
	public SpriteText textSocket;
	public SpriteText textSocketTitle;
	public SpriteText textSocketTitleText;
	
	public SimpleSprite[] slotLockImg;	
	
	
	public UIProgressBar probarComplete;
	public float maxProgressTime = 2.0f;	
	private float m_fProgressTime = 0.0f;
	private bool m_bProgressStated = false;
	private eSOCKET_IDX m_SocketIdx = eSOCKET_IDX.LEFT;
	private int m_uiSocketIdx =0;
	
	
	private int m_iSocket_Lift_Miracle = 0;	
	private int m_iSocket_Right_Miracle = 0;
	private int m_iSocket_Cur_Miracle = 0;
	private bool m_bInputDown = false;		
	private AsMessageBox m_popup;
	private int[] m_slotIndex = new int[2];
	
	
	
	private EnchantPopupDlg m_EnchantPopupDlg;	
	
	public void OpenEnchantPopupDlg( Item _item, RealItem _realItem, byte enchatSlotPos, byte uienchatSlotPos )
	{
		if( true == AsCommonSender.isSendEnchant )
			return;
		
		if( null == equipItemSlot.slotItem )
			return;
		
		if( null == _realItem)
			return;
		
		
		if( null == AsEntityManager.Instance.GetPlayerCharFsm() )
			return;
		
		if( null == AsEntityManager.Instance.GetPlayerCharFsm().UserEntity )
			return;
		
		
		int iCurLevel = AsEntityManager.Instance.GetPlayerCharFsm().UserEntity.GetProperty<int>(eComponentProperty.LEVEL);
        if (iCurLevel < _realItem.item.ItemData.levelLimit)
		{
			
			AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(1722),
												null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE) );
			return;
		}
			
		if( true == IsOpenEnchantPopupDlg )
		{
			CloseEnchantPopupDlg();
		}
		
		GameObject obj = ResourceLoad.CreateGameObject( "UI/AsGUI/GUI_Socket_Popup" );
		if(null == obj )
		{
			Debug.LogError("faile resource.load [ UI/AsGUI/GUI_Socket_Popup ]");
			return;
		}

		m_EnchantPopupDlg = obj.GetComponent<EnchantPopupDlg>();			
		if( null == m_EnchantPopupDlg )
		{
			GameObject.Destroy(obj);
			return;
		}	
		
		m_SocketIdx = (eSOCKET_IDX)enchatSlotPos;	
		m_uiSocketIdx= uienchatSlotPos;
		m_EnchantPopupDlg.Open( equipItemSlot.slotItem.realItem.getSlot, enchatSlotPos, _item, _realItem, uienchatSlotPos );
		
		
		ClosePopup();
	}
	
	public void CloseEnchantPopupDlg()
	{
		if( null != m_EnchantPopupDlg )
		{
			GameObject.Destroy(m_EnchantPopupDlg.gameObject);
		}
	}
	
	public bool IsOpenEnchantPopupDlg	
	{
		get
		{
			if( null == m_EnchantPopupDlg )
			{
				return false;
			}
			
			return m_EnchantPopupDlg.gameObject.activeSelf;
		}
	}
	
	
	//
	
	
	//-------------------------------------------------------
	public bool IsRect( Ray inputRay )
	{
		if( null == rectCollider )
			return false;
		
		return rectCollider.bounds.IntersectRay( inputRay );
	}	
	
	public bool IsEquipRect( Ray inputRay )
	{
		if( null == equipItemCollider) 
		{
			Debug.LogError("EnchantDlg::IsEquipRect() [ null == equipItemCollider ]");
			return false;
		}
		
		return equipItemCollider.bounds.IntersectRay(inputRay);
	}
	
	public bool Open( RealItem _equipItem )
	{		
		return SetEquipItem(_equipItem);
	}
	
	protected void ClosePopup()
	{
		if( null != m_popup)
		{
			GameObject.Destroy(m_popup.gameObject);
		}
	}
	
	protected void OpenPopup( AsMessageBox _msgBox )
	{
		ClosePopup();
		CloseEnchantPopupDlg();
			
		m_popup = _msgBox;
	}
		
	public void Close()
	{
		Clear();	
		CloseEnchantPopupDlg();
		ClosePopup();	
	}
	
	public void Clear()
	{
		equipItemSlot.DeleteSlotItem();		
		
		foreach( EnchantSlot _slotdata  in equipItemHaveEnchantItems )
		{
			_slotdata.DeleteSlotItem();
		}
	}	
	
	private void SetEnableLeftSocketBtn( bool isEnable)
	{
		btnLeftSocket.controlIsEnabled = isEnable;
		if( false == isEnable)
		{
			textLeftSocketCash.Color = textLeftSocketBtn.Color = Color.gray;
		}
		else
		{
			textLeftSocketCash.Color = textLeftSocketBtn.Color = Color.black;
		}
	}
	
	private void SetEnableRightSocketBtn( bool isEnable)
	{
		btnRightSocket.controlIsEnabled = isEnable;
		if( false == isEnable)
		{
			textRightSocketCash.Color = textRightSocketBtn.Color = Color.gray;
		}
		else
		{
			textRightSocketCash.Color = textRightSocketBtn.Color = Color.black;
		}
	}
	
	private bool IsCheckInvenFull()
	{		
		if( -1 == ItemMgr.HadItemManagement.Inven.GetEmptyInvenSlot() )
		{
			OpenPopup( AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), 
													AsTableManager.Instance.GetTbl_String(20), 
													this, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE ) );
			
			return false;
		}
		
		return true;
	}
	private void LeftSocketBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{		
			if( true == m_bProgressStated || true == AsCommonSender.isSendEnchant )
				return;
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			if( false == IsCheckInvenFull() )
				return;
			
			StartSocketBtn( (eSOCKET_IDX)m_slotIndex[0], 0 );			
		}
	}
	
	private void RightSocketBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{		
			if( true == m_bProgressStated || true == AsCommonSender.isSendEnchant )
				return;
			
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			if( false == IsCheckInvenFull() )
				return;
			
			StartSocketBtn( (eSOCKET_IDX)m_slotIndex[1], 1 );			
		}
	}
	
	private void StartSocketBtn( eSOCKET_IDX _eSocketIdx, int uisocketIndex )
	{		
		m_SocketIdx = _eSocketIdx;
		m_uiSocketIdx = uisocketIndex;
		
		ClosePopup();		
		
		if( 0 == m_uiSocketIdx )
			m_iSocket_Cur_Miracle = m_iSocket_Lift_Miracle;
		else
			m_iSocket_Cur_Miracle = m_iSocket_Right_Miracle;
		
		
		OpenPopup( AsNotify.Instance.CashMessageBox( (long)m_iSocket_Cur_Miracle, AsTableManager.Instance.GetTbl_String(1361), 
													AsTableManager.Instance.GetTbl_String(211), 
													this, "SendSocketSend", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION ) );
	}
	
	
	public void SendSocketSend()
	{
		if( (long)m_iSocket_Cur_Miracle > AsUserInfo.Instance.nMiracle )
		{
			AsHudDlgMgr.Instance.CloseEnchantDlg();
			AsHudDlgMgr.Instance.OpenNeedSphereDlg();			
			return;
		}
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6013_EFF_SocketProgress", Vector3.zero, false);
		m_bProgressStated = true;
		m_fProgressTime = Time.time;
	}
	
	
	public void ReciveEnchantUseItem( int iSlot, RealItem _realItem )
	{
		if( null == _realItem )
		{
			if( null != equipItemSlot.slotItem && equipItemSlot.slotItem.realItem.getSlot == iSlot )
			{
                equipItemSlot.DeleteSlotItem();	
				return;
			}		
		}
		
		if( null != _realItem )
		{		
			if( null != equipItemSlot.slotItem && equipItemSlot.slotItem.realItem.getSlot == iSlot )
			{
				SetEquipItem( _realItem );
				return;
			}			
		}
	}
	
	private bool IsEnableEnchant( RealItem _realItem )
	{	
		int iIndex = -1;
		foreach( int idata in _realItem.sItem.nEnchantInfo )
		{
			++iIndex;
			
			if( ItemMgr.Instance.GetEnchantStrengthenCount() > _realItem.sItem.nStrengthenCount )
				return true;
			
			if( -1 != idata )
				return true;
		}
		
		return false;				
	}
	
	
	
	private bool IsEnableSkillEnchant( int slot )
	{		
		if( 0 > slot )
			return false;
		
		if( equipItemHaveEnchantItems.Length <= slot )
			return false;
		
		if( false == IsHaveEquipEnchantItemEnable(slot) )
			return true;
		
		EnchantSlot _slot = equipItemHaveEnchantItems[slot];
		
		if( true == IsSkillEnchant(_slot.getItem) )
			return false;
		
		return true;
	}
	
	public static bool IsSkillEnchant( Item _item )
	{
		if( null == _item)
			return false;
		
		return IsSkillEnchant( _item.ItemID );
	}
	
	public static bool IsSkillEnchant( int iItemID )
	{			
		Tbl_SoulStoneEnchant_Record _record = AsTableManager.Instance.GetSoulStoneEnchantTable( iItemID );
		if( null == _record )
			return false;
		
		if( Tbl_SoulStoneEnchant_Record.eSTATE.Skill == _record.getState )
		{
			return true;
		}	
		
		return false;
	}
	
	public bool SetEnchantItem(RealItem _realItem)
	{
		if( null == _realItem )
			return false;
		
		if ( false == IsCheckEnableReady() )
        {				
            return false;
        }
		
		if( false == IsCheckEnchatEnableEquip( _realItem.item.ItemID ) )
		{
			AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(1296),
									null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE) );
			return false;
		}		
		
//		if( false == IsCheckSkillEnchants( _realItem ) )
//		{
//			AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(1789),
//									null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE) );
//			return false;
//		}
		
		
		EnchantSlot _slot_1 = equipItemHaveEnchantItems[0];
		EnchantSlot _slot_2 = equipItemHaveEnchantItems[1];
		
		
		
		
		if( IsSkillEnchant(_realItem.item ) )			
		{
			
			if( true == IsHaveEquipEnchantItemEnable(0) && true == IsHaveEquipEnchantItemEnable(1) )
			{
				if( null != _slot_1.getItem && null != _slot_2.getItem )
				{
					if( false == IsEnableSkillEnchant(0) )
					{
						OpenEnchantPopupDlg(_slot_1.getItem, _realItem, (byte)m_slotIndex[0], (byte)0 );	
					}
					else if( false == IsEnableSkillEnchant(1) )
					{
						OpenEnchantPopupDlg(_slot_2.getItem, _realItem, (byte)m_slotIndex[1], (byte)1 );
					}
					else
					{
						OpenEnchantPopupDlg(_slot_1.getItem, _realItem, (byte)m_slotIndex[0], (byte)0 );
					}
							
				}
				else if( null == _slot_1.getItem && null == _slot_2.getItem )
				{
					OpenEnchantPopupDlg(_slot_1.getItem, _realItem, (byte)m_slotIndex[0], (byte)0 );
				}
				else if( null == _slot_1.getItem )
				{
					if( false == IsEnableSkillEnchant(1) )
					{
						AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(1789),
									null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE) );
						return false;
					}
					OpenEnchantPopupDlg(_slot_1.getItem, _realItem, (byte)m_slotIndex[0], (byte)0 );
				}
				else if( null == _slot_2.getItem )
				{
					if( false == IsEnableSkillEnchant(0) )
					{
						AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(1789),
									null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE) );
						return false;
					}
					OpenEnchantPopupDlg(_slot_2.getItem, _realItem, (byte)m_slotIndex[1], (byte)1 );
				}
			}
			else if( true == IsHaveEquipEnchantItemEnable(0) )
			{
				OpenEnchantPopupDlg(_slot_1.getItem, _realItem, (byte)m_slotIndex[0], (byte)0 );		
			}
			else if( true == IsHaveEquipEnchantItemEnable(1) )
			{
				OpenEnchantPopupDlg(_slot_2.getItem, _realItem, (byte)m_slotIndex[1], (byte)1 );
			}
			
	
		}	
		else // in no skill enchant			
		{
			if( true == IsHaveEquipEnchantItemEnable(0) && true == IsHaveEquipEnchantItemEnable(1) )
			{
				if( null == _slot_1.getItem )
				{
					OpenEnchantPopupDlg(_slot_1.getItem, _realItem, (byte)m_slotIndex[0], (byte)0 );		
				}
				else if( null == _slot_2.getItem )
				{
					OpenEnchantPopupDlg(_slot_2.getItem, _realItem, (byte)m_slotIndex[1], (byte)1 );
				}
				else
				{
					OpenEnchantPopupDlg(_slot_1.getItem, _realItem, (byte)m_slotIndex[0], (byte)0 );		
				}
			}
			else if( true == IsHaveEquipEnchantItemEnable(0) )
			{
				OpenEnchantPopupDlg(_slot_1.getItem, _realItem, (byte)m_slotIndex[0], (byte)0 );		
			}
			else if( true == IsHaveEquipEnchantItemEnable(1) )
			{
				OpenEnchantPopupDlg(_slot_2.getItem, _realItem, (byte)m_slotIndex[1], (byte)1 );
			}
		}
		
		return true;
	}
	
	public bool SetEquipItem(RealItem _realItem)
	{ 
		if( null != m_popup )
			return false;
		
		if( null == _realItem )
			return false;

        if (ArkQuestmanager.instance.CheckHaveOpenUIType(OpenUIType.SOCKET) != null)
            AsCommonSender.SendClearOpneUI(OpenUIType.SOCKET);
		
		if( false == IsEnableEnchant(_realItem) )
		{
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(74), eCHATTYPE.eCHATTYPE_SYSTEM);
			return false;
		}
		
		if( _realItem.item.ItemData.GetItemType() != Item.eITEM_TYPE.EquipItem &&
			_realItem.item.ItemData.GetItemType() != Item.eITEM_TYPE.CosEquipItem )
		{			
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(74), eCHATTYPE.eCHATTYPE_SYSTEM);
			return false;
		}	
		

		
		m_bProgressStated = false;
		probarComplete.Value = 0.0f;	
		
			
		equipItemSlot.DeleteSlotItem();
		equipItemSlot.CreateSlotItem( _realItem, goItemParent.transform );
		SetHaveEnchantItems( _realItem );	
		
		
		
		CloseEnchantPopupDlg();
			
		return true;		
	}
	
	
	public void ReciveSuccResult()
	{			
		int iIndex = m_uiSocketIdx;
		Vector3 effectPos = equipItemHaveEnchantItems[iIndex].transform.position;	
		
		
		AsSlotEffectManager.Instance.SetStrengthenSuccEffect( true, effectPos );		
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6013_EFF_SocketSuccess", Vector3.zero, false);
        QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_EXTENSTION, new AchExtension(UIExtensionType.INSERT_SOCKET));
	}
	
	
	public void ReciveOutSuccResult()
	{
		int iIndex = m_uiSocketIdx;
		Vector3 effectPos = equipItemHaveEnchantItems[iIndex].transform.position;
		
		AsSlotEffectManager.Instance.SetStrengthenSuccEffect( true, effectPos );		
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6013_EFF_SocketSuccess", Vector3.zero, false);
	}
		
	
	
	private void SetHaveEnchantItems( RealItem _realItem )
	{			
		
		textLeftSocketCash.Text = "0";
        textRightSocketCash.Text = "0";
		
				
		if( 2 > _realItem.sItem.nEnchantInfo.Length )
		{
			Debug.LogError("Enchant::SetHaveEnchantItems()[equipItemHaveEnchantItems.Length ]" +  equipItemHaveEnchantItems.Length );
			return;
		}	
		
		equipItemHaveEnchantItems[0].DeleteSlotItem();	
		equipItemHaveEnchantItems[1].DeleteSlotItem();			

		if( _realItem.sItem.nEnchantInfo[0] == -1 && _realItem.sItem.nEnchantInfo[1] != -1 )
		{					
			SetEnchantSlotIcon( 0, _realItem.sItem.nEnchantInfo[1] );
			m_slotIndex[0] = 1;
			m_slotIndex[1] = 0;
			
			slotLockImg[1].gameObject.SetActive( true );			
			SetHaveEquipEnchantItemEnable(1, false);
		}
		else if( _realItem.sItem.nEnchantInfo[0] != -1 && _realItem.sItem.nEnchantInfo[1] != -1 )
		{			
			m_slotIndex[0] = 0;
			m_slotIndex[1] = 1;
			SetEnchantSlotIcon( 0, _realItem.sItem.nEnchantInfo[0] );
			SetEnchantSlotIcon( 1, _realItem.sItem.nEnchantInfo[1] );			
		}
		else if( _realItem.sItem.nEnchantInfo[0] != -1 && _realItem.sItem.nEnchantInfo[1] == -1 )
		{
			m_slotIndex[0] = 0;
			m_slotIndex[1] = 1;
			
			SetEnchantSlotIcon( 0, _realItem.sItem.nEnchantInfo[0] );		
			slotLockImg[1].gameObject.SetActive( true );			
			SetHaveEquipEnchantItemEnable(1, false);
		}
		else
		{
			slotLockImg[0].gameObject.SetActive( true );				
			SetHaveEquipEnchantItemEnable(0, false);
			slotLockImg[1].gameObject.SetActive( true );		
			SetHaveEquipEnchantItemEnable(1, false);
		}
		
		ResetCashSocketBtn();
	}
	
	private void SetHaveEquipEnchantItemEnable( int _index, bool _isEnable )
	{
		if( _index >= equipItemHaveEnchantItems.Length )
		{
			return;
		}
		
		if( null == equipItemHaveEnchantItems[ _index ].gameObject.collider )
			return;
		
		equipItemHaveEnchantItems[ _index ].gameObject.collider.enabled = _isEnable;
	}
	
	private bool IsHaveEquipEnchantItemEnable(int _index)
	{
		if( _index >= equipItemHaveEnchantItems.Length )
		{
			return false;
		}
		
		if( null == equipItemHaveEnchantItems[ _index ].gameObject.collider )
			return false;
		
		return equipItemHaveEnchantItems[ _index ].gameObject.collider.enabled;
	}
	
	private void SetEnchantSlotIcon( int iIndex, int _socketItemIdx )
	{
		if( 0 < _socketItemIdx )			
		{
			Item _item = ItemMgr.ItemManagement.GetItem( _socketItemIdx );
			if( null == _item )
			{
				Debug.LogError("EnchantDlg::SetHaveEnchantItems()[ null == _item ] id: " + _socketItemIdx );
				return;
			}	
			
			equipItemHaveEnchantItems[iIndex].CreateSlotItem( _item, goItemParent.transform );
			
			Tbl_SoulStoneEnchant_Record soulRecord = AsTableManager.Instance.GetSoulStoneEnchantTable( _item.ItemID );
			if( null != soulRecord )
			{
				if( 0 == iIndex )
				{
					textLeftSocketCash.Text = (m_iSocket_Lift_Miracle = soulRecord.getExtractionCount).ToString();
				}
				else
				{
					textRightSocketCash.Text = (m_iSocket_Right_Miracle = soulRecord.getExtractionCount).ToString();
				}
			}
		}
		
		slotLockImg[iIndex].gameObject.SetActive(false);		
		SetHaveEquipEnchantItemEnable(iIndex, true);
	}
	
	private void ResetCashSocketBtn()
	{
		for( int i=0; i<equipItemHaveEnchantItems.Length; ++i )
		{
			switch( i )
			{
			case 0:
				if( null == equipItemHaveEnchantItems[i].getItem )
				{
					SetEnableLeftSocketBtn(false);
				}
				else
				{
					SetEnableLeftSocketBtn(!IsSkillEnchant(equipItemHaveEnchantItems[i].getItem));
				}							
				break;
				
			case 1:
				if( null == equipItemHaveEnchantItems[i].getItem )
				{
					SetEnableRightSocketBtn(false);
				}
				else
				{
					SetEnableRightSocketBtn(!IsSkillEnchant(equipItemHaveEnchantItems[i].getItem));
				}				
				break;
			}			
		}
	}
	
	
	
	public void GuiInputDown(Ray inputRay)
	{ 
		if( false == IsRect( inputRay ) )
			return;
		
		m_bInputDown = true;
	}
	
	public void GuiInputUp(Ray inputRay)
	{ 	
		if( false == m_bInputDown )
			return;
		
		m_bInputDown = false;
		
		if( true == equipItemSlot.IsIntersect(inputRay) )
		{
			if( null != equipItemSlot.slotItem )
			{
				TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.normal, equipItemSlot.slotItem.realItem );
			}
			return;
		}	
		
		foreach( EnchantSlot enchSlot in equipItemHaveEnchantItems )
		{
			if( false == enchSlot.gameObject.activeSelf )
				continue;
			
			if( true == enchSlot.IsIntersect( inputRay ) )
			{
				
				if( false == AsUserInfo.Instance.IsEquipChaingEnable() )
				{
					if( false == AsHudDlgMgr.Instance.isOpenMsgBox )
					{					
						AsHudDlgMgr.Instance.SetMsgBox(  AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(1633),
										null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE) );
					}
					
					return;
				}
				
				
				TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.normal, enchSlot.getItem );
				return;
			}
		}	
		
		
		if( IsOpenEnchantPopupDlg )
		{
			m_EnchantPopupDlg.GuiInputUp( inputRay );
		}
	}
	
	private void CancelBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{		
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			AsHudDlgMgr.Instance.CloseEnchantDlg();
		}
	}
	
	
	public void SetItemInSlot( RealItem _realItem, Ray inputRay )
	{
		if( null == _realItem )
		{
			Debug.LogError("EnchantDlg::SetItemInslot()[ null == _realItem ]");
			return;
		}
		
		if( equipItemCollider.bounds.IntersectRay( inputRay ))
		{
			if( Item.eITEM_TYPE.EquipItem == _realItem.item.ItemData.GetItemType() || Item.eITEM_TYPE.CosEquipItem == _realItem.item.ItemData.GetItemType() )
			{
				bool isHaveEnchant = TooltipMgr.IsEnableEnchant( _realItem.sItem.nEnchantInfo, _realItem.sItem.nStrengthenCount);
				if( isHaveEnchant == true )
				{
					SetEquipItem( _realItem );
					AsSoundManager.Instance.PlaySound( _realItem.item.ItemData.m_strDropSound, Vector3.zero, false );
				}
				return;
			}	
		}

        if (Item.eITEM_TYPE.EtcItem == _realItem.item.ItemData.GetItemType() &&
            (int)Item.eEtcItem.Enchant == _realItem.item.ItemData.GetSubType() )
		{

            if ( false == IsCheckEnableReady() )
            {				
                return;
            }

			for( int i=0; i<equipItemHaveEnchantItems.Length; ++i )
			{
				EnchantSlot _slot = equipItemHaveEnchantItems[i];
				if( false == _slot.gameObject.activeSelf )
					continue;
				
				if( _slot.IsIntersect( inputRay ) )
				{
					if( false == IsCheckEnchatEnableEquip( _realItem.item.ItemID ) )
					{
						AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(1296),
												null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE) );
						return;
					}
					
					if( IsSkillEnchant(_realItem.item ) )			
					{
						if( i == 0 )							
						{
							if( false == IsEnableSkillEnchant(1) )
							{
								AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(1789),
												null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE) );
								return;
							}
							
						}
						else
						{
							if( false == IsEnableSkillEnchant(0) )
							{
								AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(1789),
												null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE) );
								return;
							}
						}									
					}				
								
					OpenEnchantPopupDlg(_slot.getItem, _realItem, (byte)m_slotIndex[i], (byte)i );
					break;
				}
			}
		}
		else if( Item.eITEM_TYPE.EquipItem == _realItem.item.ItemData.GetItemType() || Item.eITEM_TYPE.CosEquipItem == _realItem.item.ItemData.GetItemType() )
		{
			if( equipItemCollider.bounds.IntersectRay( inputRay ))
			{
				SetEquipItem( _realItem );
				AsSoundManager.Instance.PlaySound( _realItem.item.ItemData.m_strDropSound, Vector3.zero, false );
			}
		}		
	}
	
	private bool IsCheckEnableReady()
	{
		if ( null != equipItemSlot.slotItem && false == IsEnableEnchant(equipItemSlot.slotItem.realItem))
        {				
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(74), eCHATTYPE.eCHATTYPE_SYSTEM);
            return false;
        }
		
		return true;
	}
	
	
	private bool IsCheckEnchatEnableEquip( int iItemIdx )
	{
		if( null == equipItemSlot.slotItem )
			return false;
		
		
		Tbl_SoulStoneEnchant_Record record = AsTableManager.Instance.GetSoulStoneEnchantTable( iItemIdx );
		if( null == record )
		{
			Debug.LogError("EnchantDlg::IsCheckEnchatEnableEquip()[ not find id : " + iItemIdx );
			return false;
		}
		
		Item.eEQUIP eSubType = (Item.eEQUIP)equipItemSlot.slotItem.realItem.item.ItemData.GetSubType();
		
		ItemData	slotItemData 	= equipItemSlot.slotItem.realItem.item.ItemData;
		if( slotItemData.GetItemType() == Item.eITEM_TYPE.CosEquipItem && record.IsCostume == false )
		{
			return false;
		}
		
		if( true == IsSkillEnchant( iItemIdx ) )
		{
			if( Item.eEQUIP.Weapon == eSubType )	
			{
				if( Tbl_SoulStoneEnchant_Record.eTYPE.WEAPON == record.getEquipType )
					return true;
			}
			else if( Item.eEQUIP.Armor == eSubType )
			{
				if( Tbl_SoulStoneEnchant_Record.eTYPE.DEFEND == record.getEquipType )
					return true;
			}
			
			return false;
		}
		
		if( Item.eEQUIP.Weapon == eSubType )	
		{
			if( Tbl_SoulStoneEnchant_Record.eTYPE.WEAPON == record.getEquipType )
				return true;
		}
		else if( Item.eEQUIP.Head == eSubType || Item.eEQUIP.Armor == eSubType ||
			Item.eEQUIP.Point == eSubType || Item.eEQUIP.Gloves == eSubType )
		{
			if( Tbl_SoulStoneEnchant_Record.eTYPE.DEFEND == record.getEquipType )
				return true;
		}
		else if( Item.eEQUIP.Earring == eSubType || 
			Item.eEQUIP.Ring == eSubType || 
			Item.eEQUIP.Necklace == eSubType ||
			Item.eEQUIP.Fairy == eSubType ||
			Item.eEQUIP.Wing == eSubType )
		{
			if( Tbl_SoulStoneEnchant_Record.eTYPE.ACCESSORY == record.getEquipType )
				return true;
		}	
		
		return false;
	}
	
		
	// Use this for initialization
	void Start () 
	{		
		
		textTitle.Text = AsTableManager.Instance.GetTbl_String(1360);
		textSocket.Text = AsTableManager.Instance.GetTbl_String(1364);
		textSocketTitle.Text = AsTableManager.Instance.GetTbl_String(1361);
		textSocketTitleText.Text = AsTableManager.Instance.GetTbl_String(1365);		
		textLeftSocketBtn.Text = AsTableManager.Instance.GetTbl_String(1362);	
		textRightSocketBtn.Text = AsTableManager.Instance.GetTbl_String(1363);
		
		
		
		btnClose.SetInputDelegate(CancelBtnDelegate);	
		btnLeftSocket.SetInputDelegate(LeftSocketBtnDelegate);
		btnRightSocket.SetInputDelegate(RightSocketBtnDelegate);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if( true == m_bProgressStated )
		{	
			float fValue = Time.time - m_fProgressTime;
			probarComplete.Value = fValue/maxProgressTime;			
			
			if( 1.0f <= probarComplete.Value )
			{
				m_bProgressStated = false;
				probarComplete.Value = 0.0f;	
				
				if( false == AsUserInfo.Instance.IsEquipChaingEnable() )
				{
					if( false == AsHudDlgMgr.Instance.isOpenMsgBox )
					{					
						AsHudDlgMgr.Instance.SetMsgBox(  AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(1633),
										null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE) );
					}
					
					return;
				}
				
				AsCommonSender.SendItemEnchantOut( (short)equipItemSlot.slotItem.realItem.getSlot, (byte)m_SocketIdx );			
			}
		}		
	}
}
