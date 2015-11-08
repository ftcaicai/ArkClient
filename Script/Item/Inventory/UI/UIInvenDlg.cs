using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

public class UIInvenDlg : MonoBehaviour
{
	public UIInvenSlot moveItemSlot;
	public UIInvenSlot []invenslots;
	public SpriteText m_TextTitle;
	public SpriteText showGold;
	public GameObject slotItemParent;
	public UIInvenPage page;
	public float dragPageMoveDistance = 3.0f;
	public SimpleSprite focusImg;
	public UIInvenSortButton sortButton;
	public UIRemoveItemDlg removeItemDlg;
	public UIButton closeBtn;
	public UIButton sellBtn;
	public GameObject sellTooltip;
	public SpriteText sellTooltipText;
	public float maxShowSellTooltipTime = 3f;
	public GameObject[] noBuySlots;
	public SpriteText[] textNoBuyText;
	private GameObject m_ClickDownBuySlot;
	[SerializeField] GameObject[] effectOpen = new GameObject[5];

	// input click down information
	private Vector2 m_vec2DownPosition;
	private UIInvenSlot m_ClickDownItemSlot;
	private int m_iClickDownItemSlotIndex = 0;

	// item move active time
	public float m_fMaxItemMoveTime = 0.5f;
	private float m_fItemMoveTime = 0.0f;

	// page move active time
	public float m_fMaxPageMoveTime = 0.5f;
	private float m_fNextPageTime = 0.0f;
	private float m_fPrePageTime = 0.0f;
	private bool m_bMovePageEnable = false;

	// remove item check information
	private int m_iReallyRemoveItemSlotIndex = 0;
	private int m_iReallyRemoveItemCount = 0;
	private AsMessageBox m_curReallyRemoveItemDlg = null;

	// skill reset
	private AsMessageBox m_SkillResetMessageBox = null;
	private RealItem m_CurEnchantRealItem;
	private ulong m_iGold = 0;
	private bool m_bVisivleToolTip = true;
	private RealItem	m_CurSkillResetRealItem = null;

	private AsMessageBox m_IndunCountItemMsgBox = null;
	private AS_CS_CHAR_ATTACK_NPC m_IndunCountItemBuff;

	private float m_sellTooltipTime = 0f;

	public bool VisibleToolTip
	{
		get { return m_bVisivleToolTip; }
		set { m_bVisivleToolTip = value; }
	}
	
	public void SetUseHairItem( RealItem _realItem, int iTitle, int iText )
	{
		if( null == _realItem )
			return;
		
		m_tempRealItem = _realItem;
		SetTempMsgBox( AsNotify.Instance.MessageBox( 
			AsTableManager.Instance.GetTbl_String(iTitle), 
			AsTableManager.Instance.GetTbl_String(iText),
			this, "TempMsgEvent", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION) ); 
		
	}
	
	private RealItem m_tempRealItem = null;
	private AsMessageBox m_tempMsgBox = null;
	public void SetTempMsgBox( AsMessageBox _msg )
	{
		if( null != m_tempMsgBox )			
		{
			m_tempMsgBox.Close();
			m_tempMsgBox = null;
		}
		
		m_tempMsgBox = _msg;
	}
	
	public void CloseTempMsgBox()
	{
		if( null != m_tempMsgBox )			
		{
			m_tempMsgBox.Close();
			m_tempMsgBox = null;
		}
		
		m_tempRealItem = null;
	}
	

	// open dialog
	public void Open()
	{
		page.SetPage( AsHudDlgMgr.Instance.invenPageIdx);		// page index initialization
		ResetSlotItmes(); 		// current page slot item initialization
		DetachFocusImg();		// Detach Focus Image
		ResetMoveSlot();		// move slot initialization
		SetClickDownSlot( null); // click down slot initialization
		sortButton.Init();		// sort button initaialization
		CloseRemoveItemDlg();	// close remove item dialog

		foreach( GameObject obj in effectOpen)
		{
			obj.SetActiveRecursively( false);
		}

		if( false == AsHudDlgMgr.Instance.IsOpenNpcStore)
		{
			if( null!= sellBtn)
				sellBtn.gameObject.SetActiveRecursively( false);

			if( null != sellTooltip)
				sellTooltip.SetActiveRecursively( false);
			
			m_sellTooltipTime = 0f;
		}
		else
		{
			if( null != sortButton)
				sortButton.gameObject.SetActiveRecursively( false);
		}
	}

	// close dialog
	public bool Close()
	{
		QuestTutorialMgr.Instance.ProcessQuestTutorialMsg( new QuestTutorialMsgInfo( QuestTutorialMsg.CLOSE_INVENTORY, 0));
		ClearSlot();
		DetachFocusImg();
		moveItemSlot.DeleteSlotItem();
		ResetMoveSlot();
		SetClickDownSlot( null);
		CloseRemoveItemDlg();

		if( null != m_curReallyRemoveItemDlg)
		{
			m_curReallyRemoveItemDlg.Close();
			m_curReallyRemoveItemDlg = null;
		}

		if( null != removeItemDlg && true == removeItemDlg.IsOpen())
			removeItemDlg.Close();

		AsHudDlgMgr.Instance.SetNewInvenImg( false);
		for( int i = 0; i < ItemMgr.HadItemManagement.Inven.newReceiveItemslots.Length; ++i)
		{
			ItemMgr.HadItemManagement.Inven.newReceiveItemslots[i] = false;
		}

		ItemMgr.HadItemManagement.Inven.isNewReceiveItems = false;
		
		CloseTempMsgBox();

		//$yde
		AsPetManager.Instance.InventoryClosed();

		return true;
	}

	private void SellBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if( AsHudDlgMgr.Instance.IsOpenNpcStore)
			{
				int iCount = 0;
				ulong iGold = 0;
				string strItemName = string.Empty;

				for( int i = Inventory.useInvenSlotBeginIndex; i < ItemMgr.HadItemManagement.Inven.GetEnableSlotIdx(); ++i)
				{
					InvenSlot _slot = ItemMgr.HadItemManagement.Inven.invenSlots[i];
					if( null == _slot)
						continue;

					if( null == _slot.realItem)
						continue;
					
					if( false == _slot.realItem.item.ItemData.isShopSell )
						continue;

					if( Item.eITEM_TYPE.EtcItem == _slot.realItem.item.ItemData.GetItemType() && (int)Item.eEtcItem.Etc == _slot.realItem.item.ItemData.GetSubType())
					{
						++iCount;
						iGold += (ulong)( _slot.realItem.item.ItemData.sellAmount * _slot.realItem.sItem.nOverlapped);

						if( 1 == iCount)
							strItemName = AsTableManager.Instance.GetTbl_String(_slot.realItem.item.ItemData.nameId);
					}
					else if( Item.eITEM_TYPE.EquipItem == _slot.realItem.item.ItemData.GetItemType() && Item.eGRADE.Normal == _slot.realItem.item.ItemData.grade)
					{
						++iCount;
						iGold += (ulong)( _slot.realItem.item.ItemData.sellAmount * _slot.realItem.sItem.nOverlapped);

						if( 1 == iCount)
							strItemName = AsTableManager.Instance.GetTbl_String(_slot.realItem.item.ItemData.nameId);
					}
				}

				if( 0 >= iCount)
				{
					AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(2004),
						null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE));
					return;
				}

				string content = AsTableManager.Instance.GetTbl_String(2003);

				AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.GoldMessageBox( iGold, AsTableManager.Instance.GetTbl_String(126), content,
					this, "SendShopUseLessItemSell", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION));
			}
		}
	}

	public void SendShopUseLessItemSell()
	{
		if( false == AsHudDlgMgr.Instance.IsOpenNpcStore)
			return;

		AsNpcEntity entity = AsEntityManager.Instance.GetNPCEntityByTableID( AsHudDlgMgr.Instance.npcStore.getNowNpcId);
		if( null == entity)
			return;

		AsCommonSender.SendShopUseLessItemSell( entity.sessionId_);
	}

	public bool IsRect( Ray inputRay)
	{
		if( null == collider)
			return false;

		return collider.bounds.IntersectRay( inputRay);
	}

	public void OpenReallyRemoveItemDlg( int islotIndex, int iCount)
	{
		if( true == AsHudDlgMgr.Instance.IsOpenTrade ||
			true == AsHudDlgMgr.Instance.IsOpenEnchantDlg ||
			true == AsHudDlgMgr.Instance.IsOpenStrengthenDlg ||
			true == AsHudDlgMgr.Instance.IsOpenSynthesisDlg ||
			AsPStoreManager.Instance.UnableActionByPStore() == true)
		{

			AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(1410),
				null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE));
			return;
		}

		m_iReallyRemoveItemSlotIndex = islotIndex;
		m_iReallyRemoveItemCount = iCount;
		m_curReallyRemoveItemDlg = AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(1099), AsTableManager.Instance.GetTbl_String(6),
			this, "ReallyRemoveItem", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);
	}

	private void ReallyRemoveItem()
	{
		if( 0 == m_iReallyRemoveItemCount || 0 == m_iReallyRemoveItemSlotIndex)
			return;

		AsCommonSender.SendRemoveItem( m_iReallyRemoveItemSlotIndex, m_iReallyRemoveItemCount);

		m_iReallyRemoveItemCount = 0;
		m_iReallyRemoveItemSlotIndex = 0;
	}

	// click slot
	private void SetClickDownSlot( UIInvenSlot slot)
	{
		m_ClickDownItemSlot = slot;
		if( null != slot)
			m_iClickDownItemSlotIndex = slot.slotIndex;

		if( null != m_ClickDownItemSlot)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			AttachFocusImg( m_ClickDownItemSlot);
		}
		else
		{
			DetachFocusImg();
		}
	}

	// move slot
	private void SetMoveSlot( UIInvenSlot slot)
	{
		if( null != slot.slotItem)
			slot.slotItem.ShowCoolTime( false);

		moveItemSlot.SetSlotItem( slot.slotItem);
		moveItemSlot.SetSlotIndex( slot.slotIndex);
	}

	private void ResetMoveSlot()
	{
		if( null != moveItemSlot.slotItem)
			moveItemSlot.slotItem.ShowCoolTime( true);

		moveItemSlot.SetSlotItem( null);
		moveItemSlot.SetSlotIndex( 0);
	}

	// gold
	public void SetGold( ulong iGold)
	{
		if( null == showGold)
		{
			Debug.LogError( "UIInvenDlg::SetGold() [ null == showGold");
			return;
		}

		showGold.Text = iGold.ToString( "#,#0", CultureInfo.InvariantCulture);
	}

	// remove item dlg
	public void OpenRemoveItemDlg( int iSlotIndex, int iMaxCount, UIRemoveItemDlg.eDLG_TYPE eType)
	{
		if( true == AsHudDlgMgr.Instance.IsOpenTrade ||
			true == AsHudDlgMgr.Instance.IsOpenEnchantDlg ||
			true == AsHudDlgMgr.Instance.IsOpenStrengthenDlg ||
			true == AsHudDlgMgr.Instance.IsOpenSynthesisDlg	||
			AsPStoreManager.Instance.UnableActionByPStore() == true)
		{

			AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(1410),
				null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE));

			return;
		}

		if( null == removeItemDlg)
		{
			Debug.LogError( "UIInvenDlg::OpenRemoveItemDlg() [ null == removeItemDlg ] ");
			return;
		}

		removeItemDlg.Open( iSlotIndex, iMaxCount, eType);
	}

	public void CloseRemoveItemDlg()
	{
		if( null == removeItemDlg)
		{
			Debug.LogError( "UIInvenDlg::CloseRemoveItemDlg() [ null == removeItemDlg ] ");
			return;
		}

		removeItemDlg.Close();
	}

	// input
	public void InputDown( Ray inputRay)
	{
	}

	public void InputMove( Ray inputRay)
	{
		if( null != moveItemSlot.slotItem)
		{
			Vector3 pt = inputRay.origin;
			pt.z = moveItemSlot.gameObject.transform.position.z - 10.0f;
			moveItemSlot.SetSlotItemPosition( pt);
		}
	}

	public void SetRestoreSlot()
	{
		if( null == m_ClickDownItemSlot || null == moveItemSlot.slotItem)
			return;

		if( null != m_ClickDownItemSlot.slotItem)
		{
			moveItemSlot.DeleteSlotItem();
			DetachFocusImg();
			return;
		}

		PlayDorpSound( moveItemSlot.slotItem.realItem.item);
		m_ClickDownItemSlot.SetSlotItem( moveItemSlot.slotItem);
		m_ClickDownItemSlot.ResetSlotItemPosition();
		ResetMoveSlot();
		DetachFocusImg();
	}

	public void PlayDorpSound( Item _item)
	{
		if( null == _item)
		{
			Debug.LogError( "UIIvenDlg::PlayDorpSound() [ null == _item ]");
			return;
		}

		if( string.Compare( "NONE", _item.ItemData.getStrDropSound, true) == 0)
		{
			Debug.LogError( "UIInvenDlg::PlayDorpSound()[getStrDropSound == NONE] id : " + _item.ItemID);
			return;
		}

		AsSoundManager.Instance.PlaySound( _item.ItemData.getStrDropSound, Vector3.zero, false);
	}

	public void SetWastebasket()
	{
		if( null == moveItemSlot.slotItem)
			return;

		if( true == moveItemSlot.slotItem.realItem.item.ItemData.isDump)
		{
			AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(1645),
				null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE));
			return;
		}

		if( moveItemSlot.slotItem.realItem.sItem.nOverlapped > 1)
			OpenRemoveItemDlg( moveItemSlot.slotIndex, moveItemSlot.slotItem.realItem.sItem.nOverlapped, UIRemoveItemDlg.eDLG_TYPE.REMOVE);
		else
			OpenReallyRemoveItemDlg( moveItemSlot.slotIndex, moveItemSlot.slotItem.realItem.sItem.nOverlapped);
	}

	private bool IsUseInput()
	{
		if( true == this.removeItemDlg.IsOpen())
			return false;

		if( null != m_curReallyRemoveItemDlg)
			return false;

		if( null != m_SkillResetMessageBox)
			return false;

		if( null != m_IndunCountItemMsgBox)
			return false;

		if( true == AsHudDlgMgr.Instance.isOpenRandomItemDlg)
			return false;

		if( true == AsHudDlgMgr.Instance.IsOpenStorageQuantityDlg)
			return false;

		if( true == AsHudDlgMgr.Instance.IsOpenTradePopup_ItemRegistrationCancel
			|| true == AsHudDlgMgr.Instance.IsOpenTradePopup_TradeCancel
			|| true == AsHudDlgMgr.Instance.IsOpenTradePopup_Gold
			|| true == AsHudDlgMgr.Instance.IsOpenTradePopup_Quantity
			|| true == AsHudDlgMgr.Instance.IsOpenTradePopup_TradeError
			|| true == AsHudDlgMgr.Instance.IsOpenRandItemIsInven)
			return false;
		
		if( AsHudDlgMgr.Instance.IsOpenReNameDlg == true )
			return false;

//		if( null != AsHudDlgMgr.Instance.m_SynthesisDlg
//			&& ( true == AsHudDlgMgr.Instance.m_SynthesisDlg.IsOpenPopup_QuantityDlg() || true == AsHudDlgMgr.Instance.m_SynthesisDlg.IsOpenPopup_Err()))
//			return false;

		return true;
	}

	public void InputUp( Ray inputRay)
	{
		if( null != moveItemSlot.slotItem)
		{
			RaycastHit hit;
			if( Physics.Raycast( inputRay, out hit, 10000f, 1<<LayerMask.NameToLayer( "GUI")) == false)
			{
				if( true == AsUserInfo.Instance.IsLiving())
				{
					SetWastebasket();
					SetRestoreSlot();
					m_bMovePageEnable = false;
				}
				else
				{
					//message
					SetRestoreSlot();
					m_bMovePageEnable = false;
					Debug.Log( "Don't item wastebasket");
				}
			}
		}
	}

	public void GuiInputDown( Ray inputRay)
	{
		if( false == IsUseInput() || AsHudDlgMgr.Instance.IsOpenPStoreGoodsBox == true)	//$ yde
			return;

		if( moveItemSlot.slotItem != null)
			return;

		SetRestoreSlot();

		SetClickDownSlot( null);
		m_fItemMoveTime = 0.0f;
		m_fPrePageTime = 0.0f;
		m_fNextPageTime = 0.0f;
		m_ClickDownBuySlot = null;

		if( false == IsRect( inputRay))
			return;

		m_bMovePageEnable = true;
		m_vec2DownPosition = inputRay.origin;
		m_ClickDownBuySlot = GetClickBuySlot( inputRay);

		foreach( UIInvenSlot slot in invenslots)
		{
			if( null != slot.slotItem && true == slot.IsIntersect( inputRay))
			{
				SetClickDownSlot( slot);
				break;
			}
		}
	}

	private bool IsRectBuySlots( Ray ray)
	{
		foreach( GameObject go in noBuySlots)
		{
			if( false == go.active)
				continue;

			if( null == go.collider)
				continue;

			if( true == AsUtil.PtInCollider( go.collider, ray, false))
				return true;
		}

		return false;
	}

	private bool IsRectPage( Ray ray)
	{
		if( true == IsRectBuySlots( ray))
			return true;

		foreach( UIInvenSlot slot in invenslots)
		{
			if( true == slot.IsIntersect( ray))
				return true;
		}

		return false;
	}

	GameObject GetClickBuySlot( Ray inputRay)
	{
		foreach( GameObject go in noBuySlots)
		{
			if( null == go.collider)
				continue;

			if( true == AsUtil.PtInCollider( go.collider, inputRay, false))
				return go;
		}

		return null;
	}

	public void GuiInputMove( Ray inputRay)
	{
		if( false == IsUseInput() || AsHudDlgMgr.Instance.IsOpenPStoreGoodsBox == true)	//$ yde
			return;

		// exist move item
		if( null == moveItemSlot.slotItem)
		{
			if( null != m_ClickDownItemSlot)
			{
				if( true == m_ClickDownItemSlot.IsIntersect( inputRay) && false == m_ClickDownItemSlot.isMoveLock)
				{
					if( ( m_fMaxItemMoveTime <= m_fItemMoveTime) && CanProcessInput())
					{
						SetMoveSlot( m_ClickDownItemSlot);
						m_ClickDownItemSlot.SetSlotItem( null);
						AttachFocusImg( m_ClickDownItemSlot);
						m_fItemMoveTime = 0.0f;
						
						AsQuickSlotManager.Instance.Foward();
					}

					m_fItemMoveTime += Time.deltaTime;
				}
			}

			if( true == m_bMovePageEnable && ( true == page.IsPageeRectIntersect( inputRay) || IsRectPage( inputRay))/* && null == m_ClickDownItemSlot*/)
			{
				Vector2 vec2Direction = Vector2.zero;
				vec2Direction.x = inputRay.origin.x;

				Vector2 vec2TargetDirection = Vector2.zero;
				vec2TargetDirection.x = m_vec2DownPosition.x;

				vec2Direction = vec2Direction - vec2TargetDirection;
				if( dragPageMoveDistance < vec2Direction.magnitude)
				{
					if( 0 > Vector2.Dot( Vector2.right, vec2Direction.normalized))
						NextPage();
					else
						PrePage();

					m_bMovePageEnable = false;
				}
			}
		}
		else if( IsCanPageMoveCheck())
		{
			if( true == page.IsPageNextRectIntersect( inputRay))
			{
				if( m_fMaxPageMoveTime <= m_fNextPageTime)
				{
					NextPage();
					m_fNextPageTime = 0.0f;
				}
				m_fNextPageTime += Time.deltaTime;
			}
			else if( true == page.IsPagePreRectIntersect( inputRay))
			{
				if( m_fMaxPageMoveTime <= m_fPrePageTime)
				{
					PrePage();
					m_fPrePageTime = 0.0f;
				}
				m_fPrePageTime += Time.deltaTime;
			}
			else
			{
				m_fPrePageTime = 0.0f;
				m_fNextPageTime = 0.0f;
			}
		}
		
		SetPage(inputRay);
		

		if( null != moveItemSlot.slotItem)
		{
			TooltipMgr.Instance.Clear();
		}
	}

	bool CanProcessInput()
	{
		if( AsHudDlgMgr.Instance.IsOpenNpcStore)
		{
			if( AsHudDlgMgr.Instance.npcStore.IsLockInput)
				return false;
		}

		if( AsHudDlgMgr.Instance.IsOpenCashStore)
		{
			if( AsHudDlgMgr.Instance.cashStore.IsLockInput)
				return false;
		}

		return true;
	}

	public void SendInventoryOpen()
	{
		if( AsUserInfo.Instance.nMiracle < ItemMgr.HadItemManagement.Inven.GetInvenAddSphereCost(ItemMgr.HadItemManagement.Inven.getExtendPageCount/5))
			AsHudDlgMgr.Instance.OpenNeedSphereDlg();
		else
			AsCommonSender.SendItemInventoryOpen();
	}

	bool IsItemExistNowSlot( Ray inputRay)
	{
		bool bExistSlot = false;

		foreach( UIInvenSlot slot in invenslots)
		{
			if( slot.IsIntersect( inputRay))
			{
				if( null != m_ClickDownItemSlot && slot.slotIndex == m_iClickDownItemSlotIndex)
				{
					SetRestoreSlot();
					return true;
				}

				if( slot.slotIndex >= ItemMgr.HadItemManagement.Inven.GetEnableSlotIdx())
				{
					SetRestoreSlot();
					return true;
				}

				if( null != slot.slotItem)
				{
					if( moveItemSlot.slotItem.realItem.sItem.nItemTableIdx == slot.slotItem.realItem.sItem.nItemTableIdx)
					{
						if( slot.slotItem.realItem.sItem.nOverlapped >= slot.slotItem.realItem.item.ItemData.overlapCount)
							break;
					}
				}

				if( false == AsHudDlgMgr.Instance.IsOpenTrade &&
					false == AsHudDlgMgr.Instance.IsOpenEnchantDlg &&
					false == AsHudDlgMgr.Instance.IsOpenStrengthenDlg &&
					false == AsHudDlgMgr.Instance.IsOpenSynthesisDlg &&
					false == AsHudDlgMgr.Instance.IsOpenPStore &&
					AsPStoreManager.Instance.storeState == ePStoreState.Closed &&//$yde
				    AsPetManager.Instance.PetSynthesisDlg == null)
				{
					if( true == slot.isMoveLock )	
					{
						SetRestoreSlot();
					}
					else
					{
						PlayDorpSound( moveItemSlot.slotItem.realItem.item);
						AsCommonSender.SendMoveItem( moveItemSlot.slotIndex, slot.slotIndex);
						moveItemSlot.DeleteSlotItem();
					}
				}
				else
				{
					AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(1646),
						null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE));
					SetRestoreSlot();
				}

				bExistSlot = true;
				break;
			}
		}

		return bExistSlot;
	}

	public void GuiInputUp( Ray inputRay)
	{
		if( false == IsUseInput() || AsHudDlgMgr.Instance.IsOpenPStoreGoodsBox == true)	//$ yde
			return;

		if( true == this.removeItemDlg.IsOpen())
		{
			removeItemDlg.GuiInputUp( inputRay);
			return;
		}

		if( null != moveItemSlot.slotItem && null != m_ClickDownItemSlot)
		{
			bool bExistSlot = false;

			bExistSlot = IsItemExistNowSlot( inputRay);
			
			int iPage = page.IsPageIntersect(inputRay);
			if( -1 != iPage )
			{
				int iIndex = ItemMgr.HadItemManagement.Inven.GetEmptyInvenSlot( iPage );
				if( -1 != iIndex )
				{
					PlayDorpSound( moveItemSlot.slotItem.realItem.item);
					AsCommonSender.SendMoveItem( moveItemSlot.slotIndex, iIndex);
					moveItemSlot.DeleteSlotItem();
					bExistSlot = true;
				}
			}
			

			if( false == bExistSlot)
			{
				if( true == AsHudDlgMgr.Instance.IsOpenNpcStore && AsHudDlgMgr.Instance.npcStore.CheckItemReleaseOnNpcStore( inputRay))
				{
					AsHudDlgMgr.Instance.npcStore.SellItemFromInventory( moveItemSlot.slotItem.realItem);
					SetRestoreSlot();
				}
			}
		}
		
		AsQuickSlotManager.Instance.Backward();
	}

	public void GuiInputClickUp( Ray inputRay)
	{
		if( false == IsUseInput() || AsHudDlgMgr.Instance.IsOpenPStoreGoodsBox == true)	//$ yde
			return;

		if( true == this.removeItemDlg.IsOpen())
		{
			removeItemDlg.GuiInputUp( inputRay);
			return;
		}

		if( null != m_ClickDownBuySlot && true == m_bMovePageEnable)
		{
			if( m_ClickDownBuySlot == GetClickBuySlot( inputRay))
			{
				//2014.05.16 dopamin
				//if( true == AsUtil.CheckGuestUser())
				//	return;

				m_curReallyRemoveItemDlg = AsNotify.Instance.CashMessageBox( ItemMgr.HadItemManagement.Inven.GetInvenAddSphereCost( ItemMgr.HadItemManagement.Inven.getExtendPageCount/5), 
					AsTableManager.Instance.GetTbl_String(1632),
					AsTableManager.Instance.GetTbl_String(1351), this, "SendInventoryOpen",AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);
				AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
				m_bMovePageEnable = false;
				return;
			}
		}

		if( null != moveItemSlot.slotItem && null != m_ClickDownItemSlot)
		{
			if( m_ClickDownItemSlot.IsIntersect( inputRay))
			{
				if( m_ClickDownItemSlot.slotIndex == moveItemSlot.slotIndex)
				{
					SetRestoreSlot();
				}
				else
				{
					if( true == AsHudDlgMgr.Instance.IsDontMoveState)
					{
						if( AsHudDlgMgr.Instance.IsOpenPStore == false)
						{
                            if (true == m_ClickDownItemSlot.isMoveLock)
                            {
                                SetRestoreSlot();
                            }
                            else
                            {
                                PlayDorpSound(moveItemSlot.slotItem.realItem.item);
                                AsCommonSender.SendMoveItem(moveItemSlot.slotIndex, m_ClickDownItemSlot.slotIndex);
                                moveItemSlot.DeleteSlotItem();
                            }
						}
						else
						{
							if( IsRectBuySlots ( inputRay))
							{
								int iStartIndex = Inventory.useInvenSlotBeginIndex + page.curPage * Inventory.useInvenSlotNumInPage;
								if( iStartIndex > moveItemSlot.slotIndex)
									moveItemSlot.DeleteSlotItem();
								else
									SetRestoreSlot();
							}
							else
							{
								SetRestoreSlot();
							}
						}
					}
				}
			}
			else
			{
				bool bExistSlot = false;
				
				bExistSlot = IsItemExistNowSlot( inputRay);

				if( false == bExistSlot)
				{
					if( true == AsQuickSlotManager.Instance.SetMoveInvenSlotInItemSlot( inputRay, moveItemSlot))
					{
						SetRestoreSlot();
					}
					else if( ( true == AsHudDlgMgr.Instance.IsOpenPlayerStatus && false == AsHudDlgMgr.Instance.playerStatusDlg.isOtherUser) && true == AsHudDlgMgr.Instance.playerStatusDlg.IsRect( inputRay))
					{
						Item.eITEM_TYPE tempType = moveItemSlot.slotItem.realItem.item.ItemData.GetItemType();
						if( Item.eITEM_TYPE.CosEquipItem == tempType)
							SendDClickCosEquipItem( moveItemSlot.slotItem.realItem);
						else if( Item.eITEM_TYPE.EquipItem == tempType)
							SendClickEquipItem( moveItemSlot.slotItem.realItem, inputRay);

						SetRestoreSlot();
					}
					else if( AsHudDlgMgr.Instance.IsOpenEnchantDlg && true == AsHudDlgMgr.Instance.enchantDlg.IsRect( inputRay))
					{
						AsHudDlgMgr.Instance.enchantDlg.SetItemInSlot( moveItemSlot.slotItem.realItem, inputRay);
						SetRestoreSlot();
					}
					else if( AsHudDlgMgr.Instance.IsOpenStrengthenDlg && true == AsHudDlgMgr.Instance.strengthenDlg.IsEquipRect( inputRay))
					{
						AsHudDlgMgr.Instance.strengthenDlg.ResetUIInvenItem( moveItemSlot.slotItem.realItem);
						SetRestoreSlot();
					}
					else if( AsHudDlgMgr.Instance.IsOpenedPostBox)
					{
						AsPostBoxDlg postBox = AsHudDlgMgr.Instance.postBoxDlgObj.GetComponentInChildren<AsPostBoxDlg>();
						Debug.Assert( null != postBox);
						if( true == postBox.SetMoveInvenSlotInItemSlot( inputRay, moveItemSlot))
						{
							SetRestoreSlot();
							AsHudDlgMgr.Instance.invenDlg.ApplySlotMoveLock();
						}
						else
						{
							SetRestoreSlot();
						}
					}
					else
					// < ilmeda, 20120917
					if( true == AsHudDlgMgr.Instance.IsOpenTrade && true == AsHudDlgMgr.Instance.tradeDlg.IsRect( inputRay) && false == m_ClickDownItemSlot.isMoveLock)
					{
						AsHudDlgMgr.Instance.SendMoveItem_InvenToTrade( moveItemSlot.slotItem.realItem);
						SetRestoreSlot();
					}
					// ilmeda, 20120917 >
//					else if( true == AsHudDlgMgr.Instance.IsOpenSynthesisDlg && true == AsHudDlgMgr.Instance.m_SynthesisDlg.IsRect( inputRay) && false == m_ClickDownItemSlot.isMoveLock)
//					{
//						AsHudDlgMgr.Instance.m_SynthesisDlg.SetSlotItem( moveItemSlot.slotItem.realItem);
//						SetRestoreSlot();
//						ApplySlotMoveLock();
//					}
					//$ yde -20130118
					else if( true == AsHudDlgMgr.Instance.IsOpenSynEnchantDlg && AsHudDlgMgr.Instance.m_SynEnchantDlg.SetInputUpRealItem( moveItemSlot.slotItem.realItem, inputRay ) )						
					{
						SetRestoreSlot();
						ApplySlotMoveLock();
					}
					else if( true == AsHudDlgMgr.Instance.IsOpenSynCosDlg && AsHudDlgMgr.Instance.m_SynCosDlg.SetInputUpRealItem( moveItemSlot.slotItem.realItem, inputRay ) )						
					{
						SetRestoreSlot();
						ApplySlotMoveLock();
					}
					else if( true == AsHudDlgMgr.Instance.IsOpenSynOptionDlg && AsHudDlgMgr.Instance.m_SynOptionDlg.SetInputUpRealItem( moveItemSlot.slotItem.realItem, inputRay ) )						
					{
						SetRestoreSlot();
						ApplySlotMoveLock();
					}
					else if( true == AsHudDlgMgr.Instance.IsOpenSynDisDlg && AsHudDlgMgr.Instance.m_SynDisDlg.SetInputUpRealItem( moveItemSlot.slotItem.realItem, inputRay ) )						
					{
						SetRestoreSlot();
						ApplySlotMoveLock();
					}
					else if( true == AsHudDlgMgr.Instance.IsOpenStorage && true == AsHudDlgMgr.Instance.storageDlg.IsRect( inputRay) && AsHudDlgMgr.Instance.storageDlg.CheckPageOpened() == true)// & false == m_ClickDownItemSlot.isMoveLock)
					{
						AsHudDlgMgr.Instance.SendMoveItem_InvenToStorage( inputRay, moveItemSlot);
						SetRestoreSlot();
					}
					else if( ePStoreState.User_Standby == AsPStoreManager.Instance.storeState && true == AsHudDlgMgr.Instance.pstoreDlg.IsRect( inputRay))
					{
						AsHudDlgMgr.Instance.pstoreDlg.SendMoveItem_InvenToPStore( inputRay, moveItemSlot);
						SetRestoreSlot();
					}
					else if( null != AsPetManager.Instance.PetSynthesisDlg && true == AsPetManager.Instance.PetSynthesisDlg.IsRect( inputRay))
					{
						AsPetManager.Instance.PetSynthesisDlg.SetClickRealItem(moveItemSlot.slotItem.realItem);
						SetRestoreSlot();
						ApplySlotMoveLock();
					}
					else
					{
						SetRestoreSlot();
					}
				}
			}
		}
		else if( null != m_ClickDownItemSlot && true == m_ClickDownItemSlot.IsIntersect( inputRay))
		{
			if( m_iClickDownItemSlotIndex == m_ClickDownItemSlot.slotIndex)
			{
				if( m_bVisivleToolTip == true)
					OpenTooltip();
			}
		}
#if NEW_PAGE
		else if( true == m_bMovePageEnable)
		{
			if( true == page.IsPageeRectIntersect( inputRay))
			{

				Vector2 vec2Direction = Vector2.zero;
				vec2Direction.x = inputRay.origin.x;
				vec2Direction.y = inputRay.origin.y;

				vec2Direction = vec2Direction - m_vec2DownPosition;
				if( dragPageMoveDistance < vec2Direction.magnitude)
				{
					if( 0 > Vector2.Dot( Vector2.right, vec2Direction.normalized))
					{
						NextPage();
					}
					else
					{
						PrePage();
					}

					m_bMovePageEnable = false;
				}
			}
		}
#endif
		m_bMovePageEnable = false;
	}

	bool IsCanPageMoveCheck()
	{
		if( true == AsHudDlgMgr.Instance.IsOpenTrade ||
			true == AsHudDlgMgr.Instance.IsOpenEnchantDlg ||
			true == AsHudDlgMgr.Instance.IsOpenStrengthenDlg ||
			true == AsHudDlgMgr.Instance.IsOpenSynthesisDlg)
			return false;

		return true;
	}

	public bool SetPostBoxToInven( AsSlot _slot, Ray inputRay)
	{
		foreach( UIInvenSlot slot in invenslots)
		{
			if( true == slot.IsIntersect( inputRay))
			{
				if( null == slot.slotItem)
				{
					// send packet
					return true;
				}
				return false;
			}
		}

		return false;
	}

	private void OpenTooltip()
	{
		if( null == m_ClickDownItemSlot.slotItem)
			return;

		QuestTutorialMgr.Instance.ProcessQuestTutorialMsg( new QuestTutorialMsgInfo( QuestTutorialMsg.TAP_INVEN_ITEM, m_ClickDownItemSlot.slotItem.realItem.item.ItemData.GetID()));

		//int iSubType = m_ClickDownItemSlot.slotItem.realItem.item.ItemData.GetSubType();
		switch( m_ClickDownItemSlot.slotItem.realItem.item.ItemData.GetItemType())
		{
		case Item.eITEM_TYPE.CosEquipItem:
			OpenCosEquipItemToolTip( m_ClickDownItemSlot.slotItem.realItem);
			break;
		case Item.eITEM_TYPE.EquipItem:
			OpenEquipItemToolTip( m_ClickDownItemSlot.slotItem.realItem);
			break;		
		case Item.eITEM_TYPE.EtcItem:
			if( AsHudDlgMgr.Instance.IsOpenNpcStore)
			{
				m_CurEnchantRealItem = m_ClickDownItemSlot.slotItem.realItem;
				TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.right, m_ClickDownItemSlot.slotItem.realItem, this, "SellItemFromInventory", TooltipMgr.eCommonState.Sell_Btn);
			}
			else
				TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.right, m_ClickDownItemSlot.slotItem.realItem);
			break;
		case Item.eITEM_TYPE.UseItem:
			if( AsHudDlgMgr.Instance.IsOpenNpcStore)
			{
				m_CurEnchantRealItem = m_ClickDownItemSlot.slotItem.realItem;
				TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.right, m_ClickDownItemSlot.slotItem.realItem, this, "SellItemFromInventory", TooltipMgr.eCommonState.Sell_Btn);
			}
			else
				TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.right, m_ClickDownItemSlot.slotItem.realItem);
			break;
		default:
			if( AsHudDlgMgr.Instance.IsOpenNpcStore)
			{
				m_CurEnchantRealItem = m_ClickDownItemSlot.slotItem.realItem;
				TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.right, m_ClickDownItemSlot.slotItem.realItem, this, "SellItemFromInventory", TooltipMgr.eCommonState.Sell_Btn);
			}
			else
				TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.right, m_ClickDownItemSlot.slotItem.realItem);
			break;
		}
	}

	public void SellItemFromInventory()
	{
		if( null == m_CurEnchantRealItem)
			return;

		AsHudDlgMgr.Instance.npcStore.SellItemFromInventory( m_CurEnchantRealItem);
		m_CurEnchantRealItem = null;
	}

	private void OpenAllEquipToolTip( RealItem haveitem, RealItem _item)
	{
		if( null == _item)
			return;

		bool isHaveEnchant = TooltipMgr.IsEnableEnchant( _item.sItem.nEnchantInfo, _item.sItem.nStrengthenCount);
		bool isStrength = StrengthenDlg.IsCanStrengthenItem( _item);

		if( null == haveitem)
		{
			if( AsHudDlgMgr.Instance.IsOpenNpcStore)
			{
				m_CurEnchantRealItem = _item;
				TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.right, _item, this, "SellItemFromInventory", TooltipMgr.eCommonState.Sell_Btn);
			}
			else if( isHaveEnchant && isStrength)
			{
				TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.right, _item, null, null, TooltipMgr.eCommonState.Socket_Strength);
			}
			else if( isHaveEnchant)
			{
				TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.right, _item, null, null, TooltipMgr.eCommonState.Socket);
			}
			else if( isStrength)
			{
				TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.right, _item, null, null, TooltipMgr.eCommonState.Strength);
			}
			else
			{
				TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.right, _item, TooltipMgr.eCommonState.Equip );
			}
		}
		else
		{
			bool isHaveEnchant_2 = TooltipMgr.IsHaveEnchant( haveitem.sItem.nEnchantInfo);
			bool isStrength_2 = StrengthenDlg.IsCanStrengthenItem( haveitem);

			if( AsHudDlgMgr.Instance.IsOpenNpcStore)
			{
				m_CurEnchantRealItem = _item;
				TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.right, haveitem.sItem, _item, this, "SellItemFromInventory", TooltipMgr.eCommonState.Sell, TooltipMgr.eCommonState.Sell_Btn);
			}
			else
			{
				TooltipMgr.eCommonState _havestate = TooltipMgr.eCommonState.Sell;
				TooltipMgr.eCommonState _havestate_1 = TooltipMgr.eCommonState.Equip;

				if( isHaveEnchant && isStrength)
				{
					_havestate = TooltipMgr.eCommonState.Socket_Strength;
				}
				else if( isHaveEnchant)
				{
					_havestate = TooltipMgr.eCommonState.Socket;
				}
				else if( isStrength)
				{
					_havestate = TooltipMgr.eCommonState.Strength;
				}

				if( isHaveEnchant_2 && isStrength_2)
				{
					_havestate_1 = TooltipMgr.eCommonState.Socket_Strength;
				}
				else if( isHaveEnchant_2)
				{
					_havestate_1 = TooltipMgr.eCommonState.Socket;
				}
				else if( isStrength_2)
				{
					_havestate_1 = TooltipMgr.eCommonState.Strength;
				}

				TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.right, haveitem.sItem, _item, null, null, _havestate_1, _havestate	);
			}
		}
	}

	private void OpenCosEquipItemToolTip( RealItem _item)
	{
		RealItem haveitem = ItemMgr.HadItemManagement.Inven.GetCosEquipItem( _item.item.ItemData.GetSubType());
		OpenAllEquipToolTip( haveitem, _item);
	}

	private void OpenEquipItemToolTip( RealItem _item)
	{
		RealItem haveitem = ItemMgr.HadItemManagement.Inven.GetEquipItem( _item.item.ItemData.GetSubType());

		//	except pet compare tooltip
		if (((Item.eITEM_TYPE)_item.item.ItemData.GetItemType () == Item.eITEM_TYPE.EquipItem && (Item.eEQUIP)_item.item.ItemData.GetSubType () == Item.eEQUIP.Pet))
			haveitem = null;

		OpenAllEquipToolTip( haveitem, _item);
	}

	public void GuiInputDClickUp( Ray inputRay)
	{
		m_bMovePageEnable = false;

		if( false == IsUseInput() || AsHudDlgMgr.Instance.IsOpenPStoreGoodsBox == true)	//$ yde
			return;

		if( null != moveItemSlot.slotItem)
		{
			SetRestoreSlot();
		}
		else if( null != m_ClickDownItemSlot && m_ClickDownItemSlot.IsIntersect( inputRay))
		{
			if( null != m_ClickDownItemSlot.slotItem)
			{
				Item.eITEM_TYPE type = m_ClickDownItemSlot.slotItem.realItem.item.ItemData.GetItemType();
				int iSubType = m_ClickDownItemSlot.slotItem.realItem.item.ItemData.GetSubType();

				if( m_ClickDownItemSlot.slotItem.realItem.getSlot != m_ClickDownItemSlot.slotIndex)
				{
					Debug.LogError( "m_ClickDownItemSlot.slotItem.realItem.sItem.nSlot != m_ClickDownItemSlot.slotIndex");
				}
				else
				{
					// < ilmeda, 20120917
					if( true == AsHudDlgMgr.Instance.IsOpenTrade && false == m_ClickDownItemSlot.isMoveLock)
					{
						AsHudDlgMgr.Instance.SendMoveItem_InvenToTrade( m_ClickDownItemSlot.slotItem.realItem);
					}
					else if( true == AsHudDlgMgr.Instance.IsOpenStrengthenDlg)
					{
						switch( type)
						{
						case Item.eITEM_TYPE.CosEquipItem:
						case Item.eITEM_TYPE.EquipItem:
							AsHudDlgMgr.Instance.strengthenDlg.ResetUIInvenItem( m_ClickDownItemSlot.slotItem.realItem);
							break;
						default:
							AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(75),
								null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE));
							break;
						}
					}
					else if( true == AsHudDlgMgr.Instance.IsOpenEnchantDlg)
					{
						switch( type)
						{
						case Item.eITEM_TYPE.CosEquipItem:
						case Item.eITEM_TYPE.EquipItem:

							bool isHaveEnchant = TooltipMgr.IsEnableEnchant( m_ClickDownItemSlot.slotItem.realItem.sItem.nEnchantInfo, m_ClickDownItemSlot.slotItem.realItem.sItem.nStrengthenCount);
							if( isHaveEnchant == true )
							{
								AsHudDlgMgr.Instance.enchantDlg.SetEquipItem( m_ClickDownItemSlot.slotItem.realItem);
								AsSoundManager.Instance.PlaySound( m_ClickDownItemSlot.slotItem.realItem.item.ItemData.m_strDropSound, Vector3.zero, false);
							}
							break;
						case Item.eITEM_TYPE.EtcItem:
							if( (int)Item.eEtcItem.Enchant == iSubType)
							{
								AsHudDlgMgr.Instance.enchantDlg.SetEnchantItem( m_ClickDownItemSlot.slotItem.realItem);
								AsSoundManager.Instance.PlaySound( m_ClickDownItemSlot.slotItem.realItem.item.ItemData.m_strDropSound, Vector3.zero, false);
							}
							break;
						}
					}
					else if( true == AsHudDlgMgr.Instance.IsOpenedPostBox)
					{
						if( false == m_ClickDownItemSlot.isMoveLock)
						{
//							AsHudDlgMgr.Instance.postBoxDlg.SetDClickSlotItem( m_ClickDownItemSlot);
							AsPostBoxDlg postBox = AsHudDlgMgr.Instance.postBoxDlgObj.GetComponentInChildren<AsPostBoxDlg>();
							Debug.Assert( null != postBox);
							postBox.SetDClickSlotItem( m_ClickDownItemSlot);
							AsHudDlgMgr.Instance.invenDlg.ApplySlotMoveLock();
						}
					}
//					else if( true == AsHudDlgMgr.Instance.IsOpenSynthesisDlg)
//					{
//						if( false == m_ClickDownItemSlot.isMoveLock)
//							AsHudDlgMgr.Instance.m_SynthesisDlg.SetSlotItem( m_ClickDownItemSlot.slotItem.realItem);
//					}
					else if( true == AsHudDlgMgr.Instance.IsOpenNpcStore)
					{
						if( CanProcessInput())
							AsHudDlgMgr.Instance.npcStore.SellItemFromInventory( m_ClickDownItemSlot.slotItem.realItem);
					}
					//$ yde - 20130118
					else if( true == AsHudDlgMgr.Instance.IsOpenStorage)
					{
						if( m_ClickDownItemSlot.slotItem.realItem.item.ItemData.m_bItem_Storage_Limit == false)
						{
							body_CS_STORAGE_MOVE move = new body_CS_STORAGE_MOVE( eSTORAGE_MOVE_TYPE.eSTORAGE_MOVE_TYPE_INPUT,
								( short)m_ClickDownItemSlot.slotIndex, m_ClickDownItemSlot.slotItem.realItem.sItem.nOverlapped, 0);
							AsCommonSender.Send( move.ClassToPacketBytes());

							AsSoundManager.Instance.PlaySound( m_ClickDownItemSlot.slotItem.realItem.item.ItemData.getStrDropSound, Vector3.zero, false);
						}
						else
						{
							AsNotify.Instance.MessageBox( 
								AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(195),
								AsNotify.MSG_BOX_TYPE.MBT_OK);
						}
					}
					else if( true == AsHudDlgMgr.Instance.IsOpenSynEnchantDlg && false == m_ClickDownItemSlot.isMoveLock )						
					{
						 AsHudDlgMgr.Instance.m_SynEnchantDlg.SetDClickRealItem( m_ClickDownItemSlot.slotItem.realItem );
					}
					else if( true == AsHudDlgMgr.Instance.IsOpenSynCosDlg  && false == m_ClickDownItemSlot.isMoveLock )						
					{
						AsHudDlgMgr.Instance.m_SynCosDlg.SetDClickRealItem( m_ClickDownItemSlot.slotItem.realItem );
					}
					else if( true == AsHudDlgMgr.Instance.IsOpenSynOptionDlg && false == m_ClickDownItemSlot.isMoveLock )						
					{
						AsHudDlgMgr.Instance.m_SynOptionDlg.SetDClickRealItem( m_ClickDownItemSlot.slotItem.realItem );
					}
					else if( true == AsHudDlgMgr.Instance.IsOpenSynDisDlg && false == m_ClickDownItemSlot.isMoveLock )						
					{
						AsHudDlgMgr.Instance.m_SynDisDlg.SetDClickRealItem( m_ClickDownItemSlot.slotItem.realItem );
					}
					else if( null != AsPetManager.Instance.PetSynthesisDlg && false == m_ClickDownItemSlot.isMoveLock )						
					{
						AsPetManager.Instance.PetSynthesisDlg.SetClickRealItem( m_ClickDownItemSlot.slotItem.realItem );
						AsHudDlgMgr.Instance.invenDlg.ApplySlotMoveLock();
					}
					else if( true == AsHudDlgMgr.Instance.IsOpenPStore && false == m_ClickDownItemSlot.isMoveLock)
					{
						AsHudDlgMgr.Instance.pstoreDlg.SendMoveItem_InvenToPStore( m_ClickDownItemSlot);
					}
					else if( m_ClickDownItemSlot.isMoveLock == false)
					{
						switch( type)
						{
						case Item.eITEM_TYPE.CosEquipItem:
							SendDClickCosEquipItem( m_ClickDownItemSlot.slotItem.realItem);
							break;
						case Item.eITEM_TYPE.EquipItem:
							if( CheckPetItem( m_ClickDownItemSlot.slotItem.realItem) == false)
								SendDClickEquipItem( m_ClickDownItemSlot.slotItem.realItem);
							else
								DClickProc_Pet( m_ClickDownItemSlot.slotItem.realItem);
							break;
						case Item.eITEM_TYPE.EtcItem:
							break;
						case Item.eITEM_TYPE.UseItem:
							if( false == AsPStoreManager.Instance.UnableActionByPStore())//$yde
							{
								if (iSubType == (int)Item.eUSE_ITEM.Random || iSubType == (int)Item.eUSE_ITEM.QuestRandom)
								{
									AsHudDlgMgr.Instance.OpenRandomItemDlg( m_ClickDownItemSlot.slotItem.realItem);
								}
								else if ((int)Item.eUSE_ITEM.SkillReset == iSubType)
								{
									ConfirmSkillReset();
								}
								else if ((int)Item.eUSE_ITEM.Gold == iSubType)
								{
									if (false == AsPStoreManager.Instance.UnableActionByPStore())//$yde
										m_ClickDownItemSlot.slotItem.realItem.SendUseItem();
									break;
								}
								else if ((int)Item.eUSE_ITEM.Miracle == iSubType)
								{
									if (false == AsPStoreManager.Instance.UnableActionByPStore())//$yde
										m_ClickDownItemSlot.slotItem.realItem.SendUseItem();
									break;
								}
								else if ((int)Item.eUSE_ITEM.ConsumeQuest == iSubType)
								{
									if (false == AsPStoreManager.Instance.UnableActionByPStore())
										m_ClickDownItemSlot.slotItem.realItem.SendUseItem();
									break;
								}
								else if ((int)Item.eUSE_ITEM.InfiniteQuest == iSubType)
								{
									if (false == AsPStoreManager.Instance.UnableActionByPStore())
										m_ClickDownItemSlot.slotItem.realItem.SendUseItem();
									break;
								}
								else if ((int)Item.eUSE_ITEM.GetQuest == iSubType)
								{
									if (false == AsPStoreManager.Instance.UnableActionByPStore())
										m_ClickDownItemSlot.slotItem.realItem.SendUseItem();
									break;
								}
								else if ((int)Item.eUSE_ITEM.Summon == iSubType)
								{
									if (false == AsPStoreManager.Instance.UnableActionByPStore())
										m_ClickDownItemSlot.slotItem.realItem.SendUseItem();
									break;
								}
								else if ((int)Item.eUSE_ITEM.ConsumeHair == iSubType)
								{
									if (false == AsPStoreManager.Instance.UnableActionByPStore())
									{
										SetUseHairItem(m_ClickDownItemSlot.slotItem.realItem, 109, 2124);
									}
									//m_ClickDownItemSlot.slotItem.realItem.SendUseItem();
									break;
								}
								else if ((int)Item.eUSE_ITEM.ChatServer == iSubType)
								{
									if (false == AsPStoreManager.Instance.UnableActionByPStore())
										m_ClickDownItemSlot.slotItem.realItem.SendUseItem();
									break;
								}
								else if ((int)Item.eUSE_ITEM.ChatChannel == iSubType)
								{
									if (false == AsPStoreManager.Instance.UnableActionByPStore())
										m_ClickDownItemSlot.slotItem.realItem.SendUseItem();
									break;
								}
								else if ((int)Item.eUSE_ITEM.CharacterNameReset == iSubType)
								{
									if (false == AsPStoreManager.Instance.UnableActionByPStore())
										m_ClickDownItemSlot.slotItem.realItem.SendUseItem();
									break;
								}
								else if ((int)Item.eUSE_ITEM.GuildNameReset == iSubType)
								{
									if (false == AsPStoreManager.Instance.UnableActionByPStore())
										m_ClickDownItemSlot.slotItem.realItem.SendUseItem();
									break;
								}
								else if ((int)Item.eUSE_ITEM.PetEgg == iSubType ||
									(int)Item.eUSE_ITEM.PetFood == iSubType)
								{
									if (false == AsPStoreManager.Instance.UnableActionByPStore())
										m_ClickDownItemSlot.slotItem.realItem.SendUseItem();
									break;
								}
								else if ((int)Item.eUSE_ITEM.Event == iSubType)
								{
									if (false == AsPStoreManager.Instance.UnableActionByPStore())
										m_ClickDownItemSlot.slotItem.realItem.SendUseItem();
									break;
								}
								else if((int)Item.eUSE_ITEM.ImageGet == iSubType)
								{
									if (false == AsPStoreManager.Instance.UnableActionByPStore())
										m_ClickDownItemSlot.slotItem.realItem.SendUseItem();
									break;
								}
								else 
								{
									AsPStoreManager.Instance.ItemUsed(m_ClickDownItemSlot);
								}
							}
							else
								AsPStoreManager.Instance.ItemUsed( m_ClickDownItemSlot);
							break;
						case Item.eITEM_TYPE.ActionItem:						
							if( m_ClickDownItemSlot.slotItem.realItem.IsCanCoolTimeActive() == false)
							{
								if( false == IsPvpUseEnable(m_ClickDownItemSlot.slotItem.realItem.item))
								{
									AsMyProperty.Instance.AlertNotInPvp();
								}
								else if( false == IsRaidUseEnable(m_ClickDownItemSlot.slotItem.realItem.item) )
								{
									AsMyProperty.Instance.AlertNotInRaid();
								}
                                else if (false == IsFieldUseEnable(m_ClickDownItemSlot.slotItem.realItem.item))
                                {
                                    AsMyProperty.Instance.AlertNotInField();
                                }
								else if (false == IsIndunUseEnable(m_ClickDownItemSlot.slotItem.realItem.item))
								{
									AsMyProperty.Instance.AlertNotInIndun();
								}
								else
								{
									AsUserEntity user = AsUserInfo.Instance.GetCurrentUserEntity();
									user.HandleMessage( new Msg_Player_Use_ActionItem( m_ClickDownItemSlot.slotItem.realItem));
								}
								
								

//								AsUserEntity user = AsUserInfo.Instance.GetCurrentUserEntity();
//								user.HandleMessage( new Msg_Player_Use_ActionItem( m_ClickDownItemSlot.slotItem.realItem));
							}
							break;
						}
					}
					// ilmeda, 20120917 >
				}
			}
		}
	}
	
	public void TempMsgEvent()
	{
		if( null == m_tempRealItem )
			return;
		
		m_tempRealItem.SendUseItem();
		m_tempRealItem = null;
	}
	
	protected bool IsPvpUseEnable( Item _item )
	{
		if( null == _item )
			return false;
		
		if( false == TargetDecider.CheckCurrentMapIsArena() )
			return true;
		
		if( _item.ItemData.GetItemType() != Item.eITEM_TYPE.ActionItem )
			return false;
			
		bool isDisableSkill_PvP = false;
		Tbl_Skill_Record skillrecord = AsTableManager.Instance.GetTbl_Skill_Record( _item.ItemData.itemSkill);
		if( null != skillrecord)
			isDisableSkill_PvP = (skillrecord.DisableInPvP == eDisableInPvP.Disable);
		
		return !isDisableSkill_PvP;
		
	}
	
	
	protected bool IsRaidUseEnable( Item _item )
	{
		if( null == _item )
			return false;
		
		if( false == TargetDecider.CheckCurrentMapIsRaid() )
			return true;
		
		if( _item.ItemData.GetItemType() != Item.eITEM_TYPE.ActionItem )
			return false;
			
		bool isDisableSkill_Raid = false;
		Tbl_Skill_Record skillrecord = AsTableManager.Instance.GetTbl_Skill_Record( _item.ItemData.itemSkill);
		if( null != skillrecord)
			isDisableSkill_Raid = (skillrecord.DisableInRaid == eDisableInRaid.Disable);
		
		return !isDisableSkill_Raid;
		
	}

    protected bool IsFieldUseEnable(Item _item)
    {
        if (null == _item)
            return false;

        if (false == TargetDecider.CheckCurrentMapIsField())
            return true;

        if (_item.ItemData.GetItemType() != Item.eITEM_TYPE.ActionItem)
            return false;

        bool isDisableSkill_Field = false;
        Tbl_Skill_Record skillrecord = AsTableManager.Instance.GetTbl_Skill_Record(_item.ItemData.itemSkill);
        if (null != skillrecord)
            isDisableSkill_Field = (skillrecord.DisableInField == eDisableInRaid.Disable);

        return !isDisableSkill_Field;

    }

	protected bool IsIndunUseEnable( Item _item )
	{
		if( null == _item )
			return false;
		
		if( false == TargetDecider.CheckCurrentMapIsIndun() )
			return true;
		
		if( _item.ItemData.GetItemType() != Item.eITEM_TYPE.ActionItem )
			return false;
		
		bool isDisableSkill_InDun = false;
		Tbl_Skill_Record skillrecord = AsTableManager.Instance.GetTbl_Skill_Record( _item.ItemData.itemSkill);
		if( null != skillrecord)
			isDisableSkill_InDun = (skillrecord.DisableInInDun == eDisableInInDun.Disable);
		
		return !isDisableSkill_InDun;
		
	}

	public static int SendDClickCosEquipItem_s( RealItem _curItem)
	{
		if( null == AsEntityManager.Instance.UserEntity)
			return -1;

		if( false == AsEntityManager.Instance.UserEntity.IsCheckEquipEnable( _curItem))
			return -1;

		if( true == AsPStoreManager.Instance.UnableActionByPStore())//$yde
			return -1;

		int iTargetSlot = Item.equipMaxCount + Item.GetCosEquipSlotIdx( ( Item.eEQUIP)_curItem.item.ItemData.GetSubType());

		if( false == Item.IsCheckCosEquipIndex( iTargetSlot))
		{
			Debug.LogError( "UIInvelDlg::SendDClickCosEquipItem() [ false == Item.IsCheckCosEquipIndex( iTargetSlot) ] equip : " + iTargetSlot);
			return -1;
		}

		if( 10 == iTargetSlot && true == ItemMgr.HadItemManagement.Inven.IsSlotEmpty( 0))
			return -1;

		if( AsPStoreManager.Instance.UnableActionByPStore() == true)
			return -1;

		AsCommonSender.SendMoveItem( _curItem.getSlot, iTargetSlot);
		return iTargetSlot;
	}

	public void SendDClickCosEquipItem( RealItem _curItem)
	{
		if( null == AsEntityManager.Instance.UserEntity)
			return;

		if( false == AsEntityManager.Instance.UserEntity.IsCheckEquipEnable( _curItem))
			return;

		if( true == AsPStoreManager.Instance.UnableActionByPStore())//$yde
			return;

		int iTargetSlot = Item.equipMaxCount + Item.GetCosEquipSlotIdx( ( Item.eEQUIP)_curItem.item.ItemData.GetSubType());

		if( false == Item.IsCheckCosEquipIndex( iTargetSlot))
		{
			Debug.LogError( "UIInvelDlg::SendDClickCosEquipItem() [ false == Item.IsCheckCosEquipIndex( iTargetSlot) ] equip : " + iTargetSlot);
			return;
		}

		if( 10 == iTargetSlot && true == ItemMgr.HadItemManagement.Inven.IsSlotEmpty( 0))
		{
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(1564),
				null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			return;
		}

		if( AsPStoreManager.Instance.UnableActionByPStore() == true)
		{
			AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(365),
				null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE));
			return;
		}

		AsCommonSender.SendMoveItem( _curItem.getSlot, iTargetSlot);

		SetEquipEffect( _curItem.getSlot, iTargetSlot, true );
	}

	public void SetEquipEffect( int iCurSlot, int iTargetSlot, bool _isCosEquip )
	{
		if( null != ItemMgr.HadItemManagement.Inven.GetRealItemInSlot( iTargetSlot))
			SetCurEquipEffect( iCurSlot);

		if( true == AsHudDlgMgr.Instance.IsOpenPlayerStatus)
		{
			if( _isCosEquip == true && true == AsHudDlgMgr.Instance.playerStatusDlg.isCostumeEquipSlot )
				AsHudDlgMgr.Instance.playerStatusDlg.SetEquipEffect( iTargetSlot);
			
			if( _isCosEquip == false && false == AsHudDlgMgr.Instance.playerStatusDlg.isCostumeEquipSlot )
				AsHudDlgMgr.Instance.playerStatusDlg.SetEquipEffect( iTargetSlot);
		}
	}

	public void SetCurEquipEffect( int iCurSlot)
	{
		int iStartIndex = Inventory.useInvenSlotBeginIndex + page.curPage * Inventory.useInvenSlotNumInPage;
		int iRealCurSlot = iCurSlot-iStartIndex;
		if( iRealCurSlot < 0 || iRealCurSlot >= invenslots.Length)
			return;

		UIInvenSlot curslot = invenslots[iRealCurSlot];
		if( null == curslot)
			return;

		ResourceLoad.CreateUI( "UI/AsGUI/GUI_EquipEffect", curslot.gameObject.transform, Vector3.zero);
	}

	public void SendClickEquipItem( RealItem _curItem, Ray _inputRay)
	{
		if( null == AsEntityManager.Instance.UserEntity)
			return;

		if( false == AsEntityManager.Instance.UserEntity.IsCheckEquipEnable( _curItem))
			return;

		if( true == AsPStoreManager.Instance.UnableActionByPStore())//$yde
			return;

		int iTargetSlot = _curItem.item.ItemData.GetSubType() - 1;

		if( (int)Item.eEQUIP.Ring == _curItem.item.ItemData.GetSubType())
		{
			if( AsHudDlgMgr.Instance.IsOpenPlayerStatus)
			{
				int iIndex = AsHudDlgMgr.Instance.playerStatusDlg.GetRingItemSlotIndex( _inputRay);
				if( int.MaxValue != iIndex)
				{
					iTargetSlot = iIndex;
				}
				else if( true == ItemMgr.HadItemManagement.Inven.IsSlotEmpty( 7))
				{
					iTargetSlot = 7;
				}
				else if( true == ItemMgr.HadItemManagement.Inven.IsSlotEmpty( 8))
				{
					iTargetSlot = 8;
				}
			}
			else if( true == ItemMgr.HadItemManagement.Inven.IsSlotEmpty( 7))
			{
				iTargetSlot = 7;
			}
			else if( true == ItemMgr.HadItemManagement.Inven.IsSlotEmpty( 8))
			{
				iTargetSlot = 8;
			}
		}

		if( false == Item.IsCheckEquipIndex( iTargetSlot))
		{
			Debug.LogError( "UIInvelDlg::SendDClickCosEquipItem() [ false == Item.IsCheckEquipIndex( iTargetSlot) ] equip : " + iTargetSlot);
			return;
		}

		if( AsPStoreManager.Instance.UnableActionByPStore() == true)
		{
			AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(365),
									null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE));
			return;
		}

		AsSoundManager.Instance.PlaySound( _curItem.item.ItemData.m_strUseSound, Vector3.zero, false);
		AsCommonSender.SendMoveItem( _curItem.getSlot, iTargetSlot);

		SetEquipEffect( _curItem.getSlot, iTargetSlot, false );
	}

	public static int SendDClickEquipItem_s( RealItem _curItem)
	{
		if( null == AsEntityManager.Instance.UserEntity)
			return -1;

		if( false == AsEntityManager.Instance.UserEntity.IsCheckEquipEnable( _curItem.item))
			return -1;

		if( true == AsPStoreManager.Instance.UnableActionByPStore())//$yde
			return -1;

		int iTargetSlot = _curItem.item.ItemData.GetSubType() - 1;

		if( (int)Item.eEQUIP.Ring == _curItem.item.ItemData.GetSubType())
		{
			if( true == ItemMgr.HadItemManagement.Inven.IsSlotEmpty( 7))
			{
				iTargetSlot = 7;
			}
			else if( true == ItemMgr.HadItemManagement.Inven.IsSlotEmpty( 8))
			{
				iTargetSlot = 8;
			}
			else
			{
				RealItem _leftRing = ItemMgr.HadItemManagement.Inven.GetIvenItem( 7);
				RealItem _rightRing = ItemMgr.HadItemManagement.Inven.GetIvenItem( 8);

				if( null != _leftRing && null != _rightRing)
				{
					if( ItemMgr.GetRealRankPoint( _leftRing.sItem, _leftRing.item) < ItemMgr.GetRealRankPoint( _rightRing.sItem, _rightRing.item))
						iTargetSlot = 7;
					else
						iTargetSlot = 8;
				}
			}
		}

		if( false == Item.IsCheckEquipIndex( iTargetSlot))
		{
			Debug.LogError( "UIInvelDlg::SendDClickCosEquipItem() [ false == Item.IsCheckEquipIndex( iTargetSlot) ] equip : " + iTargetSlot);
			return -1;
		}

		if( AsPStoreManager.Instance.UnableActionByPStore() == true)
			return -1;

		AsSoundManager.Instance.PlaySound( _curItem.item.ItemData.m_strUseSound, Vector3.zero, false);
		AsCommonSender.SendMoveItem( _curItem.getSlot, iTargetSlot);

		return iTargetSlot;
	}

	public void SendDClickEquipItem( RealItem _curItem)
	{
		if( null == AsEntityManager.Instance.UserEntity)
			return;

		if( false == AsEntityManager.Instance.UserEntity.IsCheckEquipEnable( _curItem))
			return;

		if( true == AsPStoreManager.Instance.UnableActionByPStore())//$yde
			return;

		int iTargetSlot = _curItem.item.ItemData.GetSubType() - 1;

		if( (int)Item.eEQUIP.Ring == _curItem.item.ItemData.GetSubType())
		{
			if( true == ItemMgr.HadItemManagement.Inven.IsSlotEmpty( 7))
			{
				iTargetSlot = 7;
			}
			else if( true == ItemMgr.HadItemManagement.Inven.IsSlotEmpty( 8))
			{
				iTargetSlot = 8;
			}
			else
			{
				RealItem _leftRing = ItemMgr.HadItemManagement.Inven.GetIvenItem( 7);
				RealItem _rightRing = ItemMgr.HadItemManagement.Inven.GetIvenItem( 8);

				if( null != _leftRing && null != _rightRing)
				{
					if( ItemMgr.GetRealRankPoint( _leftRing.sItem, _leftRing.item) < ItemMgr.GetRealRankPoint( _rightRing.sItem, _rightRing.item))
						iTargetSlot = 7;
					else
						iTargetSlot = 8;
				}
			}
		}

		if( false == Item.IsCheckEquipIndex( iTargetSlot))
		{
			Debug.LogError( "UIInvelDlg::SendDClickCosEquipItem() [ false == Item.IsCheckEquipIndex( iTargetSlot) ] equip : " + iTargetSlot);
			return;
		}

		if( AsPStoreManager.Instance.UnableActionByPStore() == true)
		{
			AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(365),
				null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE));
			return;
		}

		AsSoundManager.Instance.PlaySound( _curItem.item.ItemData.m_strUseSound, Vector3.zero, false);
		AsCommonSender.SendMoveItem( _curItem.getSlot, iTargetSlot);

		SetEquipEffect( _curItem.getSlot, iTargetSlot, false );
	}

	// inven slot item
	public void InvenOpen()
	{
		int iopenPage = ( ItemMgr.HadItemManagement.Inven.getExtendPageCount + 4) / 5;
		if( page.curPage != iopenPage)
			page.SetPage( iopenPage);

		ResetSlotItmes();
		PlayLineEffect( ItemMgr.HadItemManagement.Inven.getExtendPageCount);
	}

	public void ResetSlotItmes()
	{
		if( null == ItemMgr.HadItemManagement)
		{
			Debug.LogError( "UIInvenDlg::ResetSlotItmes() [ null == ItemMgr.HadItemManagement ] ");
			return;
		}

		if( null == slotItemParent)
		{
			Debug.LogError( "UIInvenDlg::ResetSlotItmes() [ null == slotItemParent ]");
			return;
		}

		if( null == invenslots)
		{
			Debug.LogError( "UIInvenDlg::ResetSlotItmes() [ null == invenslots ]");
			return;
		}

		int iStartIndex = Inventory.useInvenSlotBeginIndex + page.curPage * Inventory.useInvenSlotNumInPage;
		int iIndex = iStartIndex;

		int iMoveSlotIndex = -1;
		DetachFocusImg();

		if( null != moveItemSlot && null != moveItemSlot.slotItem)
			iMoveSlotIndex = moveItemSlot.slotIndex;

		foreach( UIInvenSlot slot in invenslots)
		{
			slot.SetSlotIndex( iIndex);
			slot.DeleteSlotItem();
			slot.posState = UIInvenSlot.ePOS_STATE.inven;

			if( iIndex == iMoveSlotIndex)
			{
				AttachFocusImg( slot);
				++iIndex;
				continue;
			}

			InvenSlot _invenslot = ItemMgr.HadItemManagement.Inven.GetInvenSlotItem( iIndex);
			if( null == _invenslot || null == _invenslot.realItem)
			{
				++ iIndex;
				continue;
			}

			slot.CreateSlotItem( _invenslot.realItem, slotItemParent.transform);
			slot.SetMoveLock( _invenslot.isMoveLock);			
			if( iIndex < ItemMgr.HadItemManagement.Inven.newReceiveItemslots.Length)
			{
				slot.SetNewImg( ItemMgr.HadItemManagement.Inven.newReceiveItemslots[ iIndex ]);
				ItemMgr.HadItemManagement.Inven.newReceiveItemslots[ iIndex ] = false;
			}
			++ iIndex;
		}

		int iExtendIndex = iStartIndex;
		for( byte i = 0; i < 5; ++i)
		{
			if( iExtendIndex < ItemMgr.HadItemManagement.Inven.GetEnableSlotIdx())
				noBuySlots[i].SetActiveRecursively( false);
			else
				noBuySlots[i].SetActiveRecursively( true);

			iExtendIndex += 5;
		}

		#region - pstore items -
		foreach( KeyValuePair<int, body2_SC_PRIVATESHOP_OWNER_ITEMLIST> pair in AsPStoreManager.Instance.dicPlayerShopItem)
		{
			ItemMgr.HadItemManagement.Inven.SetSlotMoveLock( pair.Value.nInvenSlot, true);
		}

		ApplySlotMoveLock();
		#endregion
	}

	public void SetSlotItem( int iSlotIndex, RealItem realItem)
	{
		int iBeginSlotIndex = invenslots[0].slotIndex;
		int iIndex = iSlotIndex - iBeginSlotIndex;

		if( iIndex < 0 || iIndex >= invenslots.Length)
			return;

		invenslots[iIndex].DeleteSlotItem();

		if( null != realItem)
		{
			invenslots[iIndex].CreateSlotItem( realItem, slotItemParent.transform);
		}
		else if( null != moveItemSlot.slotItem)
		{
			if( null != moveItemSlot.slotItem.realItem)
			{
				if( moveItemSlot.slotItem.realItem.getSlot == iSlotIndex)
					moveItemSlot.DeleteSlotItem();
			}
		}

		if( null != m_curReallyRemoveItemDlg && iSlotIndex == m_iReallyRemoveItemSlotIndex)
		{
			m_curReallyRemoveItemDlg.Close();
			m_curReallyRemoveItemDlg = null;
		}

		if( null != removeItemDlg && true == removeItemDlg.IsOpen() && iSlotIndex == m_iReallyRemoveItemSlotIndex)
			removeItemDlg.Close();

		DetachFocusImg();
	}

	//$ yde - 20130207
	public void LockingPStoreSlot( body_SC_PRIVATESHOP_REGISTRATION_ITEM _item)
	{
	}

	public void ItemSoldByPStore( body_SC_PRIVATESHOP_OWNER_ITEMBUY _item)
	{
		Debug.Log( "UIInvenDlg::ItemSoldByPStore: _item.nInvenSlot = " + _item.nInvenSlot + ", _item.nItemCount = " + _item.nItemCount);

		int iBeginSlotIndex = invenslots[0].slotIndex;
		int iIndex = _item.nInvenSlot - iBeginSlotIndex;

		InvenSlot slot = ItemMgr.HadItemManagement.Inven.GetInvenSlotItem( _item.nInvenSlot);
		if( slot == null)
		{
			Debug.LogError( "UIInvenDlg::ItemSoldByPStore: invalid slot access. [_item.nInvenSlot = " + _item.nInvenSlot + "]");
			return;
		}

//		int curQuantity = invenslots[iIndex].slotItem.realItem.sItem.nOverlapped;
		int curQuantity = slot.realItem.sItem.nOverlapped;
		int resultCount = curQuantity - _item.nItemCount;

		if( resultCount < 1)
			invenslots[iIndex].DeleteSlotItem();
		else
		{
			invenslots[iIndex].slotItem.realItem.sItem.nOverlapped = resultCount;
			invenslots[iIndex].slotItem.SetItemCountText( resultCount);
		}
	}

	private void ClearSlot()
	{
		if( null == invenslots)
		{
			Debug.LogError( "UIInvenDlg::ClearSlot() [ null == invenslots ]");
			return;
		}

		foreach( UIInvenSlot slot in invenslots)
		{
			slot.DeleteSlotItem();
		}
	}

	// focus img
	public void AttachFocusImg( UIInvenSlot invenslot)
	{
		if( null == focusImg)
		{
			Debug.LogError( "UIInvenDlg::AttachFocusImg() [ null == focusImg ]");
			return;
		}

		if( null == invenslot)
			return;

		focusImg.gameObject.active = true;
		Vector3 vec3SlotPos = invenslot.transform.position;
		vec3SlotPos.z -= 1.0f;
		focusImg.transform.position = vec3SlotPos;
	}

	public void DetachFocusImg()
	{
		if( null == focusImg)
		{
			Debug.LogError( "UIInvenDlg::AttachFocusImg() [ null == focusImg ]");
			return;
		}

		focusImg.gameObject.active = false;
	}

	// page
	public void NextPage()
	{
		if( !CanProcessInput())
			return;

		if( true == page.NextPage())
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6004_EFF_Slide", Vector3.zero, false);
			ResetSlotItmes();
			AsHudDlgMgr.Instance.invenPageIdx = page.curPage;
			QuestTutorialMgr.Instance.ProcessQuestTutorialMsg( new QuestTutorialMsgInfo( QuestTutorialMsg.CHANGE_INVENTORY_PAGE));
		}
	}

	public void PrePage()
	{
		if( !CanProcessInput())
			return;

		if( true == page.PrePage())
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6004_EFF_Slide", Vector3.zero, false);
			ResetSlotItmes();
			AsHudDlgMgr.Instance.invenPageIdx = page.curPage;
			QuestTutorialMgr.Instance.ProcessQuestTutorialMsg( new QuestTutorialMsgInfo( QuestTutorialMsg.CHANGE_INVENTORY_PAGE));
		}
	}
	
	public void SetPage(Ray inputRay)
	{
		int iPage = page.IsPageIntersect(inputRay);
		if( -1 != iPage && page.curPage != iPage )
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6004_EFF_Slide", Vector3.zero, false);
			page.SetPage( iPage );			
			ResetSlotItmes();
			AsHudDlgMgr.Instance.invenPageIdx = page.curPage;
			QuestTutorialMgr.Instance.ProcessQuestTutorialMsg( new QuestTutorialMsgInfo( QuestTutorialMsg.CHANGE_INVENTORY_PAGE));
		}
	}

	public int GetInvenSlot( Ray inputRay, RealItem realItem)
	{
		foreach( UIInvenSlot slot in invenslots)
		{
			if( slot.IsIntersect( inputRay))
			{
				if( ItemMgr.HadItemManagement.Inven.GetEnableSlotIdx() <= slot.slotIndex)
					return -1;

				if( null != slot.slotItem)
				{
					if( slot.slotItem.realItem.item.ItemData.GetItemType() == realItem.item.ItemData.GetItemType() &&
						slot.slotItem.realItem.item.ItemData.GetSubType() == realItem.item.ItemData.GetSubType())
						return slot.slotItem.realItem.getSlot;
				}
				else
				{
					return slot.slotIndex;
				}
				break;
			}
		}

		return -1;
	}

	public void ResetSlotMoveLock()
	{
		ItemMgr.HadItemManagement.Inven.ResetInvenSlotMoveLock();
	}

	public void ApplySlotMoveLock()
	{
		int iIndex = Inventory.useInvenSlotBeginIndex + page.curPage * Inventory.useInvenSlotNumInPage;

		foreach( UIInvenSlot slot in invenslots)
		{
			InvenSlot _invenslot = ItemMgr.HadItemManagement.Inven.GetInvenSlotItem( iIndex);
			if( null == _invenslot)
			{
				++ iIndex;
				continue;
			}

			slot.SetMoveLock( _invenslot.isMoveLock);
			slot.CheckPvpIconColor();
			slot.CheckRaidIconColor();
			slot.CheckFieldIconColor();
			slot.CheckIndunIconColor();
			++ iIndex;
		}
	}

	public int GetSlotIndex( Ray _ray)
	{
		foreach( UIInvenSlot slot in invenslots)
		{
			if( slot.IsIntersect( _ray) == true)
				return slot.slotIndex;
		}

		return -1;
	}

	public void PlayLineEffect( int _line)
	{
		effectOpen[( _line + 4) % 5].SetActiveRecursively( false);
		effectOpen[( _line + 4) % 5].SetActiveRecursively( true);
	}

	private void CloseBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			AsHudDlgMgr.Instance.CloseInven();
		}
	}

	void Awake()
	{
		m_TextTitle.Text = AsTableManager.Instance.GetTbl_String(1298);
		string strSlot = AsTableManager.Instance.GetTbl_String(1350);
		closeBtn.SetInputDelegate( CloseBtnDelegate);
		foreach( SpriteText _text in textNoBuyText)
		{
			_text.Text = strSlot;
		}

		SetGold( m_iGold);

		if( null != sellBtn)
		{
			sellBtn.SetInputDelegate( SellBtnDelegate);
			sellBtn.spriteText.Text = AsTableManager.Instance.GetTbl_String(2001);
		}

		if( null != sellTooltipText)
			sellTooltipText.Text = AsTableManager.Instance.GetTbl_String(2002);
	}

	// Use this for initialization
	void Start ()
	{
		// ilmeda, 20120822
		//AsLanguageManager.Instance.SetFontFromSystemLanguage( showGold);
		//AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextTitle);
	}

	// Update is called once per frame
	void Update ()
	{
		if( m_iGold != AsUserInfo.Instance.SavedCharStat.nGold)
		{
			m_iGold = AsUserInfo.Instance.SavedCharStat.nGold;
			SetGold( m_iGold);
		}
		
		if( null != sellTooltip && true == sellTooltip.active )
		{
			m_sellTooltipTime += Time.deltaTime;
			if( m_sellTooltipTime >= maxShowSellTooltipTime )				
			{
				sellTooltip.SetActiveRecursively( false );
			}
		}
	}

	public void ConfirmSkillReset(RealItem	_curSkillResetRealItem = null)
	{
		if( true == TargetDecider.CheckCurrentMapIsIndun())
		{
			AsMyProperty.Instance.AlertNotInIndun();
			return;
		}

		if( true == TargetDecider.CheckCurrentMapIsArena())
		{
			AsMyProperty.Instance.AlertNotInPvp();
			return;
		}

		m_CurSkillResetRealItem = _curSkillResetRealItem;

		string strTitle = AsTableManager.Instance.GetTbl_String(123);
		string strMsg = AsTableManager.Instance.GetTbl_String(121);
		m_SkillResetMessageBox = AsNotify.Instance.MessageBox(strTitle, strMsg, this, "OnMsgBox_SkillReset_Ok", "OnMsgBox_SkillReset_Cancel", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);
	}

	void OnMsgBox_SkillReset_Ok()
	{
		//m_SkillResetMessageBox.Close();

		if( false == SkillBook.Instance.CheckSkillBookReset())
		{
			if( m_ClickDownItemSlot != null )
			{
				m_ClickDownItemSlot.slotItem.realItem.SendUseItem();	
			}
			else if( m_CurSkillResetRealItem != null )
			{
				m_CurSkillResetRealItem.SendUseItem();
			}
		}
		else
		{
			string strTitle = AsTableManager.Instance.GetTbl_String(123);
			string strMsg = AsTableManager.Instance.GetTbl_String(120);
			m_SkillResetMessageBox = AsNotify.Instance.MessageBox( strTitle, strMsg, this, "OnMsgBox_SkillReset_Err", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
		}
	}

	void OnMsgBox_SkillReset_Cancel()
	{
		//m_SkillResetMessageBox.Close();
	}

	void OnMsgBox_SkillReset_Err()
	{
		m_SkillResetMessageBox.Close();
	}

	void OnMsgBox_SkillReset_Succeeded()
	{
		m_SkillResetMessageBox.Close();
	}

	public void OpenSkillResetSucceeded()
	{
		string strTitle = AsTableManager.Instance.GetTbl_String(123);
		string strMsg = AsTableManager.Instance.GetTbl_String(122);
		m_SkillResetMessageBox = AsNotify.Instance.MessageBox( strTitle, strMsg, this, "OnMsgBox_SkillReset_Succeeded", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
	}

	public void ConfirmIndunCountItem(ePotency_Type potencyType, AS_CS_CHAR_ATTACK_NPC data, ItemData itemData, int nSkillTableID)
	{
		m_IndunCountItemBuff = data;

		string strItemName = AsTableManager.Instance.GetTbl_String( itemData.nameId);
		string strTitle = AsTableManager.Instance.GetTbl_String( 126);
		string strMsg = "";

		if( ePotency_Type.NormalHardPlayCount == potencyType)
		{
			strMsg = string.Format( AsTableManager.Instance.GetTbl_String( 2774), strItemName);
		}
		else if( ePotency_Type.HellPlayCount == potencyType)
		{
			Tbl_SkillLevel_Record record = AsTableManager.Instance.GetTbl_SkillLevel_Record( nSkillTableID);
			if( null != record)
			{
				int nIndunID = (int)( record.listSkillLevelPotency[0].Potency_IntValue);
				Tbl_InDun_Record indunRecord = AsTableManager.Instance.GetInDunRecord( nIndunID);
				if( null != indunRecord)
				{
					string strIndunName = AsTableManager.Instance.GetTbl_String( indunRecord.Name);
					strMsg = string.Format( AsTableManager.Instance.GetTbl_String( 2775), strItemName, strIndunName);
				}
			}
		}
		else if( ePotency_Type.HellPlayCountAll == potencyType)
		{
			strMsg = string.Format( AsTableManager.Instance.GetTbl_String( 2776), strItemName);
		}

		m_IndunCountItemMsgBox = AsNotify.Instance.MessageBox( strTitle, strMsg, this, "OnMsgBox_IndunCountItem_Ok", "OnMsgBox_IndunCountItem_Cancel", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);
	}

	void OnMsgBox_IndunCountItem_Ok()
	{
		byte[] bytes = m_IndunCountItemBuff.ClassToPacketBytes();
		AsCommonSender.Send( bytes);

		//m_IndunCountItemMsgBox.Close();
	}

	void OnMsgBox_IndunCountItem_Cancel()
	{
		//m_IndunCountItemMsgBox.Close();
	}
	
	#region - pet -
	bool CheckPetItem( RealItem _item)
	{
//		return Item.eEQUIP.Pet == (Item.eEQUIP)_item.item.ItemData.GetSubType();
		return _item.item.ItemData.needClass == eCLASS.PET;
	}
	
	void DClickProc_Pet( RealItem _curItem)
	{
		if( AsPetManager.Instance.CheckPetEquipEnable(_curItem) == false)
			return;
		
		if( AsPStoreManager.Instance.UnableActionByPStore() == true)
		{
			AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(365),
				null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE));
			return;
		}

		AsSoundManager.Instance.PlaySound( _curItem.item.ItemData.m_strUseSound, Vector3.zero, false);
		AsPetManager.Instance.Send_PetEquip(_curItem);
	}
	#endregion
}
