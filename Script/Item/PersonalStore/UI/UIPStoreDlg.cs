
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

public class UIPStoreDlg : MonoBehaviour
{
	#region - serialized & public -
	[SerializeField] UIPStoreMsgDlg m_PStoreMsgDlg = null;
	[SerializeField] UIPStoreGoodsBox m_PStoreGoodsBox = null; public bool GoodsBox{get{return m_PStoreGoodsBox.gameObject.active;}}
	[SerializeField] UIPStoreQuantityDlg m_PStoreQuantityDlg = null;
	
	[SerializeField] int slotCount_; public int slotCount{get{return slotCount_;}}
	[SerializeField] GameObject slotItem;
	
	[SerializeField] UIScrollList scrollList; public UIScrollList ScrollList{get{return scrollList;}}
	
	[SerializeField] UIButton btnClose;
	[SerializeField] UIButton btnOk;
	[SerializeField] UIButton btnMsg;
	[SerializeField] UIButton btnStop;
	[SerializeField] UIButton btnQuit;
	[SerializeField] SpriteText textName;
	[SerializeField] SpriteText textTime;
	[SerializeField] SpriteText textMessage;
	[SerializeField] SpriteText textOk;
	[SerializeField] SpriteText textStop;
	[SerializeField] GameObject slotItemParent;
	
//	public readonly static int SLOTMAX = 6;
	public float m_fMaxItemMoveTime = 0.5f;
	#endregion
	#region - private -
	List<UIPStoreSlot> slots = new List<UIPStoreSlot>();
	
	private UIPStoreSlot m_ClickedSlot;
	[SerializeField] private UIPStoreSlot m_MovingSlot;
	
//	private float m_fTimeBuf = 0.0f;
		
	float m_RemainTime = 0;
	
	string strOk;
	string strMsg;
	string strStop;
	
	Color m_OkTextColor;
	#endregion
	#region - init & release -
	void Awake()
	{
		Debug.LogWarning("UIPStoreDlg::Awake: private shop dialog is instantiated");
		
		m_PStoreMsgDlg.Close();
		m_PStoreGoodsBox.Close();
		m_PStoreQuantityDlg.Close();
		
		textMessage.Text = AsTableManager.Instance.GetTbl_String( 1232);
		textOk.Text = AsTableManager.Instance.GetTbl_String( 1234);
		textStop.Text = AsTableManager.Instance.GetTbl_String( 1235);
		btnQuit.Text = AsTableManager.Instance.GetTbl_String( 1317);
		
		strMsg = AsTableManager.Instance.GetTbl_String( 1232);
		strOk = AsTableManager.Instance.GetTbl_String( 1234);
		strStop = AsTableManager.Instance.GetTbl_String( 1235);
		
		slots.Clear();
		
		for(int i=0; i<slotCount_; ++i)
		{
			UIListItemContainer item = scrollList.CreateItem( slotItem) as UIListItemContainer;
//			UIPStoreSlot slot = item.gameObject.GetComponentInChildren<UIPStoreSlot>();
			UIPStoreSlot slot = item.transform.GetChild(0).GetComponent<UIPStoreSlot>();
			
			if(slot == null)
			{
				Debug.LogError("UIPStoreDlg:: Awake: UIPStoreSlot is not attached at prefab");
				return;
			}
			else
			{
				slot.Init(i);
				slots.Add(slot);
			}
		}
		
		m_OkTextColor = textOk.Color;
		textOk.SetColor(Color.white);
		textOk.Text = m_OkTextColor + strOk;
	}
	
	void Start()
	{
		#region - basic -
		btnClose.SetInputDelegate( _CloseBtnDelegate);
		btnOk.SetInputDelegate( _OpenBtnDelegate);
		btnMsg.SetInputDelegate( _MsgBtnDelegate);
		btnStop.SetInputDelegate( _StopBtnDelegate);
		btnQuit.SetInputDelegate( _CloseBtnDelegate);
		
//		btnLockR.SetInputDelegate( _LockBtnDelegate);
//		btnGoldR.SetInputDelegate( _GoldBtnDelegate);
//		AsLanguageManager.Instance.SetFontFromSystemLanguage( textNameL);
//		AsLanguageManager.Instance.SetFontFromSystemLanguage( textGoldL);
//		AsLanguageManager.Instance.SetFontFromSystemLanguage( textNameR);
//		AsLanguageManager.Instance.SetFontFromSystemLanguage( textGoldR);
		
		AsLanguageManager.Instance.SetFontFromSystemLanguage( textName);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( textTime);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( textMessage);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( textOk);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( textStop);
		#endregion
	}
	#endregion
	#region - update -
	void Update()
	{
		switch(AsPStoreManager.Instance.storeState)
		{
		case ePStoreState.User_Opening:
			if(m_RemainTime > 0)
			{
				m_RemainTime -= Time.deltaTime;
				
				float hr = m_RemainTime / 3600.0f;
				float min = ( m_RemainTime % 3600.0f) / 60.0f;
				float sec = ( m_RemainTime % 3600.0f) % 60.0f;
				
				textTime.Text = string.Format( "{0:D2}:{1:D2}:{2:D2}", (int)hr, (int)min, (int)sec);
			}
			break;
		}
	}
	#endregion
	#region - open & close -
	public void Open()
	{
		gameObject.SetActiveRecursively( true);
		_ClearSlots();
		
		SetBtnByState();
		SetDefaultTitle();
		SetRemainTime();
		SetSlotByUsedItem();
		SetSlotsByList();
		SetCurScrollPosition();
		
		if(AsHudDlgMgr.Instance.IsOpenInven == false) AsHudDlgMgr.Instance.OpenInven();
		AsUserEntity player = AsUserInfo.Instance.GetCurrentUserEntity();
		player.HandleMessage(new Msg_MoveEndInform());
	}
	
	public void Close()
	{
		m_PStoreMsgDlg.Close();
		m_PStoreGoodsBox.Close();
		m_PStoreQuantityDlg.Close();
		
		AsPStoreManager.Instance.Close();
	}
	#endregion
	#region - input -
	public bool IsRect( Ray inputRay )
	{
		if( null == collider )
			return false;
		
		return collider.bounds.IntersectRay( inputRay );
//		return AsUtil.PtInCollider( collider, inputRay);
	}
	
	public void InputDown( Ray inputRay)
	{			
//		TooltipMgr.Instance.Clear();
	}
	
	public void InputUp(Ray inputRay)
	{
		if( null != m_MovingSlot.slotItem )
		{
			RaycastHit hit;
			if(Physics.Raycast(inputRay, out hit, 10000f, 1<<LayerMask.NameToLayer("GUI")) == false)		
			{
				switch(AsPStoreManager.Instance.storeState)
				{
				case ePStoreState.User_Standby:
					TryRemovePStoreItem(m_MovingSlot);
					SetRestoreSlot();
					break;
				case ePStoreState.Another_Opened:
					SetRestoreSlot();
					break;
				}
				
			}			
		}
	}
	
	public void InputMove(Ray inputRay)
	{
		if(null != m_MovingSlot.slotItem )
		{
			Vector3 pt = inputRay.origin;
			pt.z = m_MovingSlot.gameObject.transform.position.z - 10.0f;
			m_MovingSlot.SetSlotItemPosition( pt);			
		}
	}
	
	float m_fItemMoveTime = 0.0f;
	public void GuiInputDown( Ray inputRay)
	{
		if( false == _IsUseInput())
			return;
		
		m_fItemMoveTime = 0.0f;
		
		foreach( UIPStoreSlot slot in slots)
		{
			slot.ReleaseSelection();
			
//			if( null != slot.slotItem && true == slot.collider.bounds.IntersectRay( inputRay))
			if( ( null != slot.slotItem) && ( true == slot.IsIntersect(inputRay)))
			{
				AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);				
				
				m_ClickedSlot = slot;
				m_ClickedSlot.ActiveSelection();
			}
		}
	}	

	public void GuiInputMove(Ray inputRay)
	{
		if( false == _IsUseInput() || m_PStoreGoodsBox.gameObject.active == true )
			return;
		
		if( scrollList != null && true == scrollList.IsScrolling)
			return;
		
		// exist move item 
		if( null == m_MovingSlot.slotItem && ( AsPStoreManager.Instance.storeState == ePStoreState.User_Standby ||
			AsPStoreManager.Instance.storeState == ePStoreState.Another_Opened))
		{
			if( null != m_ClickedSlot && null != m_ClickedSlot.slotItem )
			{
//				if( true == m_ClickedSlot.IconSlot.collider.bounds.IntersectRay( inputRay ))// && false == m_ClickedSlot.iconSlot.isMoveLock )
				if( true == m_ClickedSlot.IsIntersect(inputRay))
				{				
					if( m_fMaxItemMoveTime <= m_fItemMoveTime )
					{
						m_MovingSlot.SetMovingSlot(m_ClickedSlot);
						m_ClickedSlot.SetSlotItem(null);
						m_ClickedSlot.collider.enabled = true;
						scrollList.LockScroll = true;
						m_ClickedSlot.ActiveSelection();
						m_fItemMoveTime = 0.0f;
						
//						Debug.Log("UIPStoreDlg::GuiInputMove: m_MovingSlot is set(" + m_MovingSlot.slotItem.realItem.item.ItemData.nameId + ")");
					}
					
					m_fItemMoveTime += Time.deltaTime;
				}
			}
		}
	}

	public void GuiInputUp(Ray inputRay)
	{
		if( false == _IsUseInput() || m_PStoreGoodsBox.gameObject.active == true)
			return;
		
		if( null == m_MovingSlot.slotItem)
		{
			if( null != m_ClickedSlot)
			{
				if( m_ClickedSlot.IsIntersect(inputRay) == true)//  m_ClickedSlot.collider.bounds.IntersectRay( inputRay))
				{
					m_ClickedSlot.ActiveSelection();
					_OpenTooltip();
				}
			}
			//else
			//	TooltipMgr.Instance.Clear();
		}
		else if( null != m_MovingSlot.slotItem)
		{			
			switch(AsPStoreManager.Instance.storeState)
			{
			case ePStoreState.User_Standby:
				if(IsRect(inputRay) != true)
					TryRemovePStoreItem(m_MovingSlot);
				SetRestoreSlot();
				break;
			case ePStoreState.User_Opening:
				break;
			case ePStoreState.Another_Opened:
				if( true == AsHudDlgMgr.Instance.IsOpenInven &&  AsHudDlgMgr.Instance.invenDlg.IsRect(inputRay) == true)
					TryBuyPStoreItem(m_MovingSlot);
				SetRestoreSlot();
				break;
			}
		}
		
	}
	
	public void GuiInputDClickUp(Ray inputRay)
	{
		if( false == _IsUseInput() || m_PStoreGoodsBox.gameObject.active == true)
			return;
		
//		if( null != m_ClickedSlot && m_ClickedSlot.collider.bounds.IntersectRay( inputRay) && m_ClickedSlot.state == UIPStoreSlot.eState.Filled)
		if( ( null != m_ClickedSlot) && true == m_ClickedSlot.IsIntersect(inputRay) && ( UIPStoreSlot.eState.Filled == m_ClickedSlot.state))
		{
			switch(AsPStoreManager.Instance.storeState)
			{
			case ePStoreState.User_Standby:
				TryRemovePStoreItem(m_ClickedSlot);
				break;
			case ePStoreState.Another_Opened:
				TryBuyPStoreItem(m_ClickedSlot);
				break;
			}
			
			m_ClickedSlot.ReleaseSelection();
			m_ClickedSlot = null;
		}
	}
	#endregion
	#region - operate -
	public bool CheckPickingGoodsBox(Ray _ray)
	{
		if( null == m_PStoreGoodsBox.collider)
			return false;
		
//		if( false == m_PStoreGoodsBox.collider.bounds.IntersectRay( _ray))
//			return false;
//		
//		return true;
		return AsUtil.PtInCollider( m_PStoreGoodsBox.collider, _ray);
	}
	
	public void SendMoveItem_InvenToPStore(UIInvenSlot _slot)
	{
		int slotIndex = AsPStoreManager.Instance.GetProperBlankSlotIndex();
		bool tradable = _slot.slotItem.realItem.item.ItemData.isTradeLimit;
		bool standby = AsPStoreManager.Instance.storeState == ePStoreState.User_Standby;
		
		if(slotIndex == -1 || tradable == true || standby == false ||
			//0 != ((sbyte)eITEMATTRIBUTE.eITEMATTRIBUTE_DUMP & _slot.slotItem.realItem.sItem.nAttribute) ||
			//0 != ((sbyte)eITEMATTRIBUTE.eITEMATTRIBUTE_TRADE_LIMIT & _slot.slotItem.realItem.sItem.nAttribute))
			true == _slot.slotItem.realItem.item.ItemData.isDump ||
			false == _slot.slotItem.realItem.sItem.IsTradeEnable() )
		{
			if(tradable == true ||
				true == _slot.slotItem.realItem.item.ItemData.isDump ||
				false == _slot.slotItem.realItem.sItem.IsTradeEnable() )
				//0 != ((sbyte)eITEMATTRIBUTE.eITEMATTRIBUTE_DUMP & _slot.slotItem.realItem.sItem.nAttribute) ||
				//0 != ((sbyte)eITEMATTRIBUTE.eITEMATTRIBUTE_TRADE_LIMIT & _slot.slotItem.realItem.sItem.nAttribute))
				AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(29));
			
			if( AsHudDlgMgr.Instance.IsOpenInven )
				AsHudDlgMgr.Instance.invenDlg.SetRestoreSlot();
		}
		else
		{
			if(AsPStoreManager.Instance.PStoreItemUsedSlot != null &&
				AsPStoreManager.Instance.PStoreItemUsedSlot.getSlot == _slot.slotItem.realItem.getSlot)
			{
				AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(29));
				Debug.LogWarning("UIPStoreDlg:: SendMoveItem_InvenToPStore: trying to register item opening shop.");
			}
			else
				m_PStoreGoodsBox.Open(_slot, slotIndex, StoreGoods_OkBtnClick_, StoreGoods_CancelBtnClick_);
		}
	}
	
	public void SendMoveItem_InvenToPStore(Ray _ray, UIInvenSlot _slot)
	{
//		int slotIndex = GetSlotIndex(_ray);
		int slotIndex = AsPStoreManager.Instance.GetProperBlankSlotIndex();
		bool tradable = _slot.slotItem.realItem.item.ItemData.isTradeLimit;
		bool standby = AsPStoreManager.Instance.storeState == ePStoreState.User_Standby;
		
		if(slotIndex == -1 || tradable == true || standby == false ||
			//0 != ((sbyte)eITEMATTRIBUTE.eITEMATTRIBUTE_DUMP & _slot.slotItem.realItem.sItem.nAttribute) ||
			true == _slot.slotItem.realItem.item.ItemData.isDump || 
			false == _slot.slotItem.realItem.sItem.IsTradeEnable() )
			//0 != ((sbyte)eITEMATTRIBUTE.eITEMATTRIBUTE_TRADE_LIMIT & _slot.slotItem.realItem.sItem.nAttribute))
		{
			if(tradable == true ||
				true == _slot.slotItem.realItem.item.ItemData.isDump ||
				false == _slot.slotItem.realItem.sItem.IsTradeEnable() )
				//0 != ((sbyte)eITEMATTRIBUTE.eITEMATTRIBUTE_DUMP & _slot.slotItem.realItem.sItem.nAttribute) ||
				//0 != ((sbyte)eITEMATTRIBUTE.eITEMATTRIBUTE_TRADE_LIMIT & _slot.slotItem.realItem.sItem.nAttribute))
				AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(29));
			
			if( AsHudDlgMgr.Instance.IsOpenInven )
				AsHudDlgMgr.Instance.invenDlg.SetRestoreSlot();
		}
		else
		{
			if(AsPStoreManager.Instance.PStoreItemUsedSlot != null &&
				AsPStoreManager.Instance.PStoreItemUsedSlot.getSlot == _slot.slotItem.realItem.getSlot)
			{
				AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(29));
				Debug.LogWarning("UIPStoreDlg:: SendMoveItem_InvenToPStore: trying to register item opening shop.");
			}
			else
				m_PStoreGoodsBox.Open(_slot, slotIndex, StoreGoods_OkBtnClick_, StoreGoods_CancelBtnClick_);
		}
	}
	
	void StoreGoods_OkBtnClick_(int _idx, int _quantity, UInt64 _gold, int _slotIndex)
	{
		body_CS_PRIVATESHOP_REGISTRATION_ITEM registration = new body_CS_PRIVATESHOP_REGISTRATION_ITEM(true,
			_slotIndex, (byte)_idx, (Int16)_quantity, _gold);
			
		RealItem _realItem = ItemMgr.HadItemManagement.Inven.GetRealItemInSlot( _slotIndex );
		if( null != _realItem )
			AsSoundManager.Instance.PlaySound( _realItem.item.ItemData.getStrDropSound, Vector3.zero, false);
		
		AsPStoreManager.Instance.Request_Registraion_Item(registration);
	}
	
	void StoreGoods_CancelBtnClick_()
	{
		m_PStoreGoodsBox.Close();
	}
	
	void TryRemovePStoreItem(UIPStoreSlot pstoreSlot)
	{
		body_CS_PRIVATESHOP_REGISTRATION_ITEM register = new body_CS_PRIVATESHOP_REGISTRATION_ITEM(
			false, pstoreSlot.InvenIdx, (byte)pstoreSlot.slotIndex, (short)pstoreSlot.slotItem.realItem.sItem.nOverlapped, pstoreSlot.gold);
//		AsCommonSender.Send(register.ClassToPacketBytes());		
		AsPStoreManager.Instance.Request_Registraion_Item(register);
	}
	
	void TryBuyPStoreItem(UIPStoreSlot pstoreSlot)
	{
		if(pstoreSlot.slotItem.realItem.sItem.nOverlapped == 1)
		{
//			m_SavedSlotIdx = pstoreSlot.slotIndex;
//			m_SavedCount = 1;
			
			PurchaseConfirmed(pstoreSlot, 1);
		}
		else
		{
			m_PStoreQuantityDlg.Open(pstoreSlot, PurchaseConfirmed);
		}
	}
	
	int m_SavedSlotIdx;
	int m_SavedCount;
	void PurchaseConfirmed(UIPStoreSlot _slot, int _count)
	{
		m_SavedSlotIdx = _slot.slotIndex;
		m_SavedCount = _count;
		
		string title = AsTableManager.Instance.GetTbl_String(126);
		string content = AsTableManager.Instance.GetTbl_String(407);
		content = string.Format( content, _slot.TextName + Color.white, _count.ToString(), ( _slot.gold * (ulong)_count).ToString( "#,#0", CultureInfo.InvariantCulture));
		string left = AsTableManager.Instance.GetTbl_String(1151);
		string right = AsTableManager.Instance.GetTbl_String(1152);
		
		AsNotify.Instance.MessageBox(title, content, right, left, this, "SendPurchaseInfo", "", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL);
	}
	
	void SendPurchaseInfo()
	{
		body_CS_PRIVATESHOP_ITEMBUY buy = new body_CS_PRIVATESHOP_ITEMBUY(AsPStoreManager.Instance.curShopUId,
			(byte)m_SavedSlotIdx, (byte)m_SavedCount);
		AsPStoreManager.Instance.Request_ItemBuy(buy);
	}
	#endregion
	#region - public -
	public int GetSlotIndex( Ray inputRay)
	{
		foreach( UIPStoreSlot slot in slots )
		{
			if(slot.IsIntersect(inputRay) == true)
			{
				if(slot.slotItem == null && slot.state != UIPStoreSlot.eState.Closed)
					return slot.slotIndex;
				else
					return -1;
			}
		}
		
		return -1;
	}
	public void SetPStoreItem(body_SC_PRIVATESHOP_REGISTRATION_ITEM _item)
	{
		if( ( slots.Count - 1) < _item.nPrivateShopSlot || _item.nPrivateShopSlot < 0)
			return;
		
		slots[_item.nPrivateShopSlot].Clear();
		
		if( null != _item)
		{
			slots[_item.nPrivateShopSlot].CreateSlotItem(_item);//, slotItemParent.transform);
		}
	}
	public void SetPlayerPStoreItem()
	{
		foreach(KeyValuePair<int, body2_SC_PRIVATESHOP_OWNER_ITEMLIST> pair in AsPStoreManager.Instance.dicPlayerShopItem)
		{
			if( ( slots.Count - 1) < pair.Value.nPrivateShopSlot || pair.Value.nPrivateShopSlot < 0)
				return;
			
			slots[pair.Value.nPrivateShopSlot].Clear();
			
			if( null != pair.Value && pair.Value.sItem.nOverlapped > 0)
			{
				slots[pair.Value.nPrivateShopSlot].CreateSlotItem(pair.Value);//, slotItemParent.transform);
			}
		}
	}
	public void SetOtherPStoreItem(body1_SC_PRIVATESHOP_ITEMLIST _list)
	{
		for(int i=0; i<slotCount_; ++i)
		{
			slots[i].SetClosed();
		}
		
		int firstIdx = int.MaxValue;
		foreach(body2_SC_PRIVATESHOP_ITEMLIST item in _list.body)
		{
			Debug.Log("SetOtherPStoreItem: item = " + item.nPrivateShopSlot);
			
			if( ( slots.Count - 1) < item.nPrivateShopSlot || item.nPrivateShopSlot < 0)
				return;
			
			slots[item.nPrivateShopSlot].Clear();
			
			if( null != item)
			{
				slots[item.nPrivateShopSlot].CreateSlotItem(item);//, slotItemParent.transform);
			}
			
			if(item.nPrivateShopSlot < firstIdx && item.sItem.nOverlapped > 0)
			{
				firstIdx = item.nPrivateShopSlot;
			}
		}
		
		if(firstIdx != int.MaxValue)
		{
			float unit_Y = 3.2f;
			float total = slotCount_;
			
			float length = ((unit_Y * total) + (0.01f * (total - 1)) - scrollList.viewableArea.y);
			float curPos = (firstIdx * unit_Y) / length; if(curPos > 1) curPos = 1;
			
			scrollList.ScrollListTo(curPos);
		}
	}
	public void RemovePStoreItem(body_SC_PRIVATESHOP_REGISTRATION_ITEM _item)
	{
		if( ( slots.Count - 1) < _item.nPrivateShopSlot || _item.nPrivateShopSlot < 0)
			return;
		
		slots[_item.nPrivateShopSlot].Clear();
		
		SetRestoreSlot();
	}
	public void RemovePStoreItem(body_SC_PRIVATESHOP_ITEMBUY _item)
	{
		if( ( slots.Count - 1) < _item.nPrivateShopSlot || _item.nPrivateShopSlot < 0)
			return;
		
		slots[_item.nPrivateShopSlot].Clear();
		
		SetRestoreSlot();
	}
	public void RemovePStoreItem(body_SC_PRIVATESHOP_OWNER_ITEMBUY _item)
	{
		if( ( slots.Count - 1) < _item.nPrivateShopSlot || _item.nPrivateShopSlot < 0)
			return;
		
		slots[_item.nPrivateShopSlot].Clear();
		
		SetRestoreSlot();
	}
	public void ItemSold(body_SC_PRIVATESHOP_ITEMBUY _item)
	{
		Debug.Log("UIPStoreDlg::ItemSold: _item.nPrivateShopSlot = "  + _item.nPrivateShopSlot + ", _item.nItemCount = " + _item.nItemCount);
		
		string name = AsTableManager.Instance.GetTbl_String( slots[_item.nPrivateShopSlot].slotItem.realItem.item.ItemData.nameId);
		string header = AsTableManager.Instance.GetTbl_String(1145);
		string body = string.Format(AsTableManager.Instance.GetTbl_String(376), name);
		AsNotify.Instance.MessageBox(header, body);
		
		int curQuantity = slots[_item.nPrivateShopSlot].slotItem.realItem.sItem.nOverlapped;
		int resultCount = curQuantity - _item.nItemCount;
		Debug.Log("UIPStoreDlg::ItemSold: resultCount = "  + resultCount);
		if(resultCount < 1)
			slots[_item.nPrivateShopSlot].Clear();
		else
		{
			slots[_item.nPrivateShopSlot].slotItem.realItem.sItem.nOverlapped = resultCount; 
			slots[_item.nPrivateShopSlot].slotItem.SetItemCountText(resultCount);
			scrollList.ClipItems();
		}
	}
	public void ItemSold(body_SC_PRIVATESHOP_OWNER_ITEMBUY _item)
	{
		Debug.Log("UIPStoreDlg::ItemSold: _item.nPrivateShopSlot = "  + _item.nPrivateShopSlot + ", _item.nItemCount = " + _item.nItemCount);
				
		int curQuantity = slots[_item.nPrivateShopSlot].slotItem.realItem.sItem.nOverlapped;
		int resultCount = curQuantity - _item.nItemCount;
		if(resultCount < 1)
			slots[_item.nPrivateShopSlot].Clear();
		else
		{
//			slots[_item.nPrivateShopSlot].slotItem.realItem.sItem.nOverlapped = resultCount;
			slots[_item.nPrivateShopSlot].slotItem.SetItemCountText(resultCount);
		}
	}
	public void SetBtnByState()
	{
		Debug.Log("UIPStoreDlg:: SetBtnByState: state = " + AsPStoreManager.Instance.storeState);
		
		switch(AsPStoreManager.Instance.storeState)
		{
		case ePStoreState.User_Standby:
			btnStop.gameObject.SetActiveRecursively(false);
			
			btnMsg.gameObject.SetActiveRecursively(true);
//			btnMsg.renderer.material.SetColor("_Color", Color.white);
			btnMsg.Text = Color.black + strMsg;
			btnQuit.gameObject.SetActiveRecursively(false);
			
			btnOk.gameObject.SetActiveRecursively(true);
			if(AsPStoreManager.Instance.OpenCoolRemained == true)
				btnOk.Text = Color.gray + strOk;
			break;
		case ePStoreState.User_Opening:
			btnMsg.gameObject.SetActiveRecursively(true);
//			btnMsg.renderer.material.SetColor("_Color", Color.gray);
			btnMsg.Text = Color.gray + strMsg;
			
			btnOk.gameObject.SetActiveRecursively(false);
			btnStop.gameObject.SetActiveRecursively(true);
			btnQuit.gameObject.SetActiveRecursively(false);
			break;
		case ePStoreState.Another_Opened:
			btnMsg.gameObject.SetActiveRecursively(false);
			btnOk.gameObject.SetActiveRecursively(false);
			btnStop.gameObject.SetActiveRecursively(false);
			btnQuit.gameObject.SetActiveRecursively(true);
			break;
		}
	}
	public void OpenCoolFinished()
	{
		btnOk.Text = m_OkTextColor + strOk;
	}
	public void BeginTimeProcess()
	{
		m_RemainTime = AsPStoreManager.Instance.RemainTime;
	}
	public void StopTimeProcess()
	{
		m_RemainTime = 0;
		textTime.Text = "";
	}
	public void SetDefaultTimeProcess()
	{
		SetDefaultTime();
	}
	#endregion
	#region - focus -
	public void AttachFocusImg( UIPStoreSlot storageslot )
	{
//		if( null == focusImg )
//		{
//			Debug.LogError("UIStorageDlg::AttachFocusImg() [ null == focusImg ]");
//			return;
//		}
//		
//		if( null == storageslot )
//			return;
//		
//		
//		focusImg.gameObject.active = true;
//		Vector3 vec3SlotPos = storageslot.transform.position;
//		vec3SlotPos.z -= 1.0f;
//		focusImg.transform.position = vec3SlotPos;
	}
	
	public void DetachFocusImg()
	{
//		if( null == focusImg )
//		{
//			Debug.LogError("UIStorageDlg::AttachFocusImg() [ null == focusImg ]");
//			return;
//		}
//		
//		focusImg.gameObject.active = false;
	}
	#endregion
	#region - private -
	void SetDefaultTitle()
	{
		Debug.Log("UIPStoreDlg:: SetDefaultTitle: state = " + AsPStoreManager.Instance.storeState);
		
		switch(AsPStoreManager.Instance.storeState)
		{
		case ePStoreState.User_Standby:
		case ePStoreState.User_Opening:
			string name = AsUserInfo.Instance.GetCurrentUserEntity().GetProperty<string>(eComponentProperty.NAME);
			textName.text = string.Format(AsTableManager.Instance.GetTbl_String(1231), name);
			break;
		case ePStoreState.Another_Opened:
			textName.Text = AsPStoreManager.Instance.strTitle;
			break;
		}
	}
	
	void SetRemainTime()
	{
		Debug.Log("UIPStoreDlg:: SetRemainTime: state = " + AsPStoreManager.Instance.storeState);
		
		textTime.Text = "";
		
		switch(AsPStoreManager.Instance.storeState)
		{
		case ePStoreState.User_Standby:
			SetDefaultTime();
			break;
		case ePStoreState.User_Opening:
			m_RemainTime = AsPStoreManager.Instance.RemainTime;
			break;
		case ePStoreState.Another_Opened:
			break;
		}
	}
	
	void SetSlotByUsedItem()
	{
		switch(AsPStoreManager.Instance.storeState)
		{
		case ePStoreState.User_Standby:
		case ePStoreState.User_Opening:
			InitPlayerPStoreSlot();
			break;
		case ePStoreState.Another_Opened:
//			InitOtherUserPStoreSlot();
			break;
		}
	}
	
	void InitPlayerPStoreSlot()
	{
		int count = (int)AsPStoreManager.Instance.PStoreEnableSlot;
//		for(int i=slotCount_ - 1; i>count; --i)
//		{
//			slots[i].SetClosed();
//		}
		for(int i=count; i<slotCount_; ++i)
		{
			slots[i].SetClosed();
		}
	}
	
	void SetDefaultTime()
	{
		float defaultTime = 0;
			
		switch(AsPStoreManager.Instance.PStoreEnableSlot)
		{
		case ePStoreEnableSlot.Ticket_1:
//			defaultTime = 3600f * 2f;
			defaultTime = 60f;
			break;
		case ePStoreEnableSlot.Ticket_2:
			defaultTime = 3600f * 4f;
			break;
		case ePStoreEnableSlot.Ticket_3:
			defaultTime = 3600f * 8f;
			break;
		case ePStoreEnableSlot.Ticket_4:
			defaultTime = 3600f * 12f;
			break;
		case ePStoreEnableSlot.Ticket_5:
			defaultTime = 3600f * 24f;
			break;
		}
		
		float hr = defaultTime / 3600.0f;
		float min = ( defaultTime % 3600.0f) / 60.0f;
		float sec = ( defaultTime % 3600.0f) % 60.0f;
		
		textTime.Text = string.Format( "{0:D2}:{1:D2}:{2:D2}", (int)hr, (int)min, (int)sec);
	}
	
	void SetSlotsByList()
	{
		Debug.Log("UIPStoreDlg:: SetSlotsByList: state = " + AsPStoreManager.Instance.storeState);
		
		switch(AsPStoreManager.Instance.storeState)
		{
		case ePStoreState.User_Standby:
			break;
		case ePStoreState.User_Opening:
			SetPlayerPStoreItem();
			break;
		case ePStoreState.Another_Opened:
			break;
		}
	}
	
	void SetCurScrollPosition()
	{
		switch(AsPStoreManager.Instance.storeState)
		{
		case ePStoreState.User_Standby:
			break;
		case ePStoreState.User_Opening:
			int idx = AsPStoreManager.Instance.GetFirstFilledSlotIndex();
			
			float unit_Y = 3.2f;
			float total = slotCount_;
			
			float length = ((unit_Y * total) + (0.01f * (total - 1)) - scrollList.viewableArea.y);
			float curPos = (idx * unit_Y) / length; if(curPos > 1) curPos = 1;
			
			scrollList.ScrollListTo(curPos);
			break;
		case ePStoreState.Another_Opened:
			break;
		}
	}
	
	private void SetRestoreSlot()
	{
		if( null == m_ClickedSlot || null == m_MovingSlot.slotItem )
			return;
		
		if( null != m_ClickedSlot.slotItem )
		{
			m_MovingSlot.Clear();
//			DetachFocusImg();	
			return;
		}
		
		PlayDropSound( m_MovingSlot.slotItem.realItem.item );
		m_ClickedSlot.SetSlotItem( m_MovingSlot.slotItem );
		m_ClickedSlot.ResetSlotItemPosition();
		m_ClickedSlot.collider.enabled = false;
		scrollList.LockScroll = false;
		m_MovingSlot.SetSlotItem(null);
		m_ClickedSlot.ReleaseSelection();
		
//		m_MovingSlot = null;
//		DetachFocusImg();		
	}
	
	private void _ClearSlots()
	{
		if( null == slots)
			return;
		
		foreach( UIPStoreSlot slot in slots)
		{
			slot.Clear();
		}
	}
	
	private void _OpenTooltip()
	{
		if( null == m_ClickedSlot || UIPStoreSlot.eState.Filled != m_ClickedSlot.state)
			return;
		
		RealItem haveitem = null;
		
		if (Item.eITEM_TYPE.EquipItem  == m_ClickedSlot.slotItem.realItem.item.ItemData.GetItemType() )
			 haveitem = ItemMgr.HadItemManagement.Inven.GetEquipItem(m_ClickedSlot.slotItem.realItem.item.ItemData.GetSubType());
		else if( Item.eITEM_TYPE.CosEquipItem == m_ClickedSlot.slotItem.realItem.item.ItemData.GetItemType())
            haveitem = ItemMgr.HadItemManagement.Inven.GetCosEquipItem(m_ClickedSlot.slotItem.realItem.item.ItemData.GetSubType());
		
		switch(AsPStoreManager.Instance.storeState)
		{
		case ePStoreState.User_Standby:
		case ePStoreState.User_Opening:
			if (null == haveitem)
               TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.left, m_ClickedSlot.slotItem.realItem);
            else
               TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.left, haveitem, m_ClickedSlot.slotItem.realItem );
			
			break;
		case ePStoreState.Another_Opened:
			if (null == haveitem)
               TooltipMgr.Instance.OpenTooltip(TooltipMgr.eOPEN_DLG.left, m_ClickedSlot.slotItem.realItem, this, "_BuyItemViaTooltip",TooltipMgr.eCommonState.Buy);
            else
               TooltipMgr.Instance.OpenTooltip(TooltipMgr.eOPEN_DLG.left, haveitem, m_ClickedSlot.slotItem.realItem, this, "_BuyItemViaTooltip",
					TooltipMgr.eCommonState.NONE, TooltipMgr.eCommonState.Buy);
			break;
		}
	}
	
	
	private void _BuyItemViaTooltip()
	{
		TryBuyPStoreItem(m_ClickedSlot);
	}
	
	private bool _IsUseInput()
	{
		if( true == AsHudDlgMgr.Instance.IsOpenTradePopup_Gold)
			return false;
		
		if( true == AsHudDlgMgr.Instance.IsOpenTradePopup_Quantity)
			return false;
		
		return true;
	}
	
	void PlayDropSound( Item _item )	
	{
		if( null == _item)
		{
			Debug.LogError("UIStorageDlg::PlayDropSound() [ null == _item ]");
			return;
		}
		
		if(string.Compare("NONE", _item.ItemData.getStrDropSound, true) == 0)
		{
			Debug.LogError("UIStorageDlg::PlayDropSound()[getStrDropSound == NONE] id : " + _item.ItemID );
			return;
		}
		
		AsSoundManager.Instance.PlaySound( _item.ItemData.getStrDropSound, Vector3.zero, false  );
	}
	#endregion
	#region - delegate -
	private void _CloseBtnDelegate(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
//			Close();
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			
			AsHudDlgMgr.Instance.ClosePStore();
		}
	}
	
	private void _OpenBtnDelegate(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			Debug.Log("UIPStoreDlg:: _OpenBtnDelegate: state = " + AsPStoreManager.Instance.storeState);
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			
			switch(AsPStoreManager.Instance.storeState)
			{
			case ePStoreState.User_Standby:
				if(AsPStoreManager.Instance.OpenCoolRemained == false)
					AsPStoreManager.Instance.Request_Open();
				break;
			case ePStoreState.User_Opening:
				break;
			case ePStoreState.Another_Opened:
				break;
			}
		}
	}

	private void _MsgBtnDelegate(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			Debug.Log("UIPStoreDlg:: _MsgBtnDelegate: state = " + AsPStoreManager.Instance.storeState);
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			
			switch(AsPStoreManager.Instance.storeState)
			{
			case ePStoreState.User_Standby:
				m_PStoreMsgDlg.Open(AsPStoreManager.Instance.strContent);
				break;
			case ePStoreState.User_Opening:
				break;
			case ePStoreState.Another_Opened:
				break;
			}
		}
	}
	
	void _StopBtnDelegate(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if(m_Closing == true)
				return;
			
			StartCoroutine(WaitForClosing());
			
			Debug.Log("UIPStoreDlg:: _StopBtnDelegate: state = " + AsPStoreManager.Instance.storeState);
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			
			switch(AsPStoreManager.Instance.storeState)
			{
			case ePStoreState.User_Standby:
				break;
			case ePStoreState.User_Opening:
				AsPStoreManager.Instance.Request_Close();
				AsUserInfo.Instance.GetCurrentUserEntity().HandleMessage(new Msg_ClosePrivateShop());
				break;
			case ePStoreState.Another_Opened:
				break;
			}
		}
	}
	
	bool m_Closing = false;
	[SerializeField] float m_ClosingTime = 1f;
	IEnumerator WaitForClosing()
	{
		m_Closing = true;
		
		yield return new WaitForSeconds(m_ClosingTime);
		
		m_Closing = false;
	}
	
//	private void _LockBtnDelegate(ref POINTER_INFO ptr)
//	{
////		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
////		{
////			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
////			AsCommonSender.SendTradeLock( !_isLockR());
////		}
//		
//		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
//		{
//		}
//	}
//	
//	private void _GoldBtnDelegate(ref POINTER_INFO ptr)
//	{
////		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
////		{
////			if( false == _isLockR())
////				AsHudDlgMgr.Instance.OpenTradeGoldDlg();
////		}
//		
//		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
//		{
//		}
//	}
	#endregion
}
