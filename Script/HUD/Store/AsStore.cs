using UnityEngine;
using System.Collections.Generic;
using System.Globalization;


public enum StoreTouchType
{
	STORE_TOUCH_NONE = -1,
	STORE_TOUCH_LEFT,
	STORE_TOUCH_RIGHT
}

public enum StoreState
{
	NORMAL,
	SELECT,
	DRAG,
	QUANTITY_SELL,
	QUANTITY_BUY,
	ASK,
	CHANGE_CLASS,
	NOTOUCH,
}

public class StoreTouchInfo
{
	public StoreTouchType type = StoreTouchType.STORE_TOUCH_NONE;
	public StoreItemController storeItemController = null;
	public GameObject touchObject = null;
	public int idx = -1;

	public StoreTouchInfo()
	{
		idx = -1;
		type = StoreTouchType.STORE_TOUCH_NONE;
		storeItemController = null;
		touchObject = null;
	}

	public override string ToString() { return "[" + idx + "] = " + type.ToString(); }
}


public class AsStore : MonoBehaviour
{
	#region -public fields
	public bool IsLockInput	{ get{return isLockInput;}}
	public bool useTouchOffset = false;
	public float fStationaryTime = 0.5f;
	public float fNotTouchTime = 0.5f;
	public float fDClickTime = 0.2f;
	public UIPanel classTypePanel;
	public SpriteText textClass;
	public UIScrollList scrollList;
	public UIScrollList scrollCategory;
	public UIRadioBtn[] radioBtns;
	public UIButton buttonClose;
	public GameObject listItemPrefab;
	public GameObject focusItemPrefab;
	public UIButton classTypeBtn;
	public eCLASS[] classEnums;
	public UIRadioBtn[] classBtns;
	//public Collider invenCollider;
	public StoreQuantityMessageBox quantityMsgBox;
	public bool isLockInput = false;
	#endregion

	#region -protected field
	protected eCLASS nowClass = eCLASS.DIVINEKNIGHT;
	protected RealItem nowRealItem = null;
	protected int nowNpcID = 0;
	protected eCLASS nowUserClass = eCLASS.DIVINEKNIGHT;
	protected Vector3[] radioBtnPositions = null;
	protected StoreTouchInfo nowTouchInfo = null;
	protected StoreState nowState = StoreState.NORMAL;
	protected StoreItemInfoUI nowStoreItemInfo = null;
	protected StoreTouchInfo prevTouchInfo = new StoreTouchInfo();
	protected const int ItemBuyMax = 99;
	protected AsBasePurchase purchaseManager = null;
	protected Dictionary<string, body2_SC_CHARGE_ITEMLIST> dicChargeItemInfo = new Dictionary<string, body2_SC_CHARGE_ITEMLIST>();
	#endregion

	#region -private field
	protected float fStationaryPassTime = 0.0f;
	protected float fNotTouchPassTime = 0.0f;
	protected float fClickTimePassed = 0.0f;
	protected bool isClicked = false;
	protected bool isDraingItem = false;
	protected bool isDownStoreOut = false;
	protected bool isShowClassPopup = false;
	protected Vector3 touchOffset = Vector3.zero;
	protected GameObject focusItem = null;
	protected GameObject nowSelectIcon = null;
	protected StoreTouchInfo OneClickTouchInfo = null;
	protected StoreTouchInfo TwoClickTouchInfo = null;
	#endregion

	#region -Delegate Define
	public delegate void BuyItemEventDelegate();
	public delegate void SellItemEventDelegate();
	public delegate void PrepareBuyItemEventDelegate( StoreItemInfoUI _storeItemInfo);
	public delegate void PrepItemOnInvenEventDelegate( Ray _ray);
	#endregion

	#region -Delegate
	protected BuyItemEventDelegate BuyItemEvent = null;
	protected SellItemEventDelegate SellItemEvent = null;
	protected PrepareBuyItemEventDelegate PrepareBuyItemEvent = null;
	protected PrepItemOnInvenEventDelegate ReleaseBuyItemEvent = null;
	#endregion
	
	public int getNowNpcId
	{
		get { return nowNpcID; }
	}

	void OnDestroy()
	{
		if( AsHudDlgMgr.Instance != null)
		{
			if( AsHudDlgMgr.Instance.invenDlg != null)
				AsHudDlgMgr.Instance.invenDlg.VisibleToolTip = true;
		}
	}

	protected void Initilize()
	{
		prevTouchInfo = new StoreTouchInfo();

		// create focus
		CreateFocus();

		if( scrollList != null)
			scrollList.SetInputDelegate( InputDelegate);

		if( quantityMsgBox != null)
			quantityMsgBox.Close();

		if( classTypeBtn != null)
			classTypeBtn.AddInputDelegate( ChangeClassTypeDelegate);

		if( buttonClose != null)
			buttonClose.AddInputDelegate( CloseDelegate);

		if( radioBtns != null)
			radioBtnPositions = new Vector3[radioBtns.Length];

		int count = 0;
		foreach ( UIRadioBtn button in classBtns)
		{
			button.data = classEnums[count];
			button.AddInputDelegate( ChangeClassTypeDelegate);
			count++;
		}

		InitKindMenu();

		if( classTypePanel != null)
			classTypePanel.GetTransition( UIPanelManager.SHOW_MODE.BringInBack).AddTransitionEndDelegate( TransitionEndDelegate);
	}

	public static void SetTextAtSpriteTxt( SpriteText _targetSpriteText, string _text)
	{
		if( _targetSpriteText.enabled)
		{
			//AsLanguageManager.Instance.SetFontFromSystemLanguage( _targetSpriteText);
	
			_targetSpriteText.Text = _text;
		}
	}

	public GameObject CreateFocus()
	{
		if( focusItemPrefab == null)
			return null;

		focusItem = GameObject.Instantiate( focusItemPrefab) as GameObject;
		focusItem.SetActive( false);
		focusItem.transform.parent = transform;
		Debug.Log( "Create Focus Item");
	
		return focusItem;
	}

	protected void SelectStoreItem( StoreTouchInfo _touchInfo)
	{
		try
		{
			if( prevTouchInfo != null && prevTouchInfo.idx != -1)
				prevTouchInfo.storeItemController.RemoveFocus( transform);

			if( focusItem == null)
				CreateFocus();

			if( focusItem != null)
			{
				_touchInfo.storeItemController.SetFocus( focusItem);
				scrollList.ClipItems();
			}
		}
		catch ( System.Exception e)
		{
			Debug.LogError( e.Message);
			Debug.LogError( e.Source);
		}
	}

	public void InputDelegate( ref POINTER_INFO ptr)
	{
		if( scrollList == null)
			return;

		if( scrollList.IsScrolling == false)
			return;

		if( nowState == StoreState.NORMAL || nowState == StoreState.SELECT)
		{
			if( ptr.evt == POINTER_INFO.INPUT_EVENT.DRAG && prevTouchInfo != null)
			{
				ReleaseStoreItem( prevTouchInfo);

				if( TooltipMgr.Instance != null)
					TooltipMgr.Instance.Clear();

				nowState = StoreState.NORMAL;
			}
		}
	}

	void Update()
	{
		MoveStoreItem();

		ProcessUpClick();
	
		ProcessNotTouch();
	}

	protected void ProcessNotTouch()
	{
		if( nowState == StoreState.NOTOUCH)
		{
			if( fNotTouchPassTime >= fNotTouchTime)
			{
				nowState = StoreState.NORMAL;
				fNotTouchPassTime = 0.0f;
				LockInput( false);

				if( AsCashStore.Instance != null)
					AsCashStore.Instance.HideLoadingIndigator();
			}
			else
				fNotTouchPassTime += Time.deltaTime;
		}
	}

	void OnEnable()
	{
		if( quantityMsgBox != null)
			quantityMsgBox.Close();
	}

	public virtual void AddStoreItems( List<Store_Item_Info_Table> _storeItemList)
	{
		if (scrollList.gameObject.activeInHierarchy == true && scrollList.gameObject.activeSelf == true)
		{
			scrollList.ScrollWheel(0);
			scrollList.ClearListSync(true);
		}

		int line = _storeItemList.Count;

		for ( int i = 0; i < line; i++)
			AddStoreItemLine();

		for ( int i = 0; i < line; i++)
			AddStoreItem( i, _storeItemList[i]);
	}

	public virtual void AddStoreItemLine( bool _isCharge = false)
	{
		if( scrollList == null)
			return;
	
		IUIListObject item = scrollList.CreateItem( listItemPrefab);
		item.gameObject.name = scrollList.Count.ToString();
		item.gameObject.layer = listItemPrefab.layer;
		scrollList.UpdateCamera();
	}

	protected virtual void AddStoreItem( int _line, params Store_Item_Info_Table[] _storeItem)//, Store_Item_Info_Table _storeItem)
	{
		IUIListObject item = scrollList.GetItem( _line);

		StoreItemController storeItemCon = item.gameObject.GetComponent<StoreItemController>();

		GameObject itemIcon = null;

		string itemID = string.Empty;
		string itemName = string.Empty;
		string itemPrice = string.Empty;
		int buyAmount = 0;

		int count = 0;
		foreach ( Store_Item_Info_Table info in _storeItem)
		{
			if( info.Type == Store_Item_Type.NormalItem || info.Type == Store_Item_Type.GachaItem)
			{
				ItemData itemData = null;
				itemID = info.ID.ToString();
				itemData = ItemMgr.ItemManagement.GetItem( info.ID).ItemData;
				itemName = AsTableManager.Instance.GetTbl_String(itemData.nameId);
				itemPrice = ( itemData.buyAmount * info.Count).ToString( "#,#0", CultureInfo.InvariantCulture);
				buyAmount = itemData.buyAmount;
				itemIcon = GetItemIcon( info.ID.ToString(), info.Count);
			}

			if( info != null)
			{
				itemIcon.transform.parent = storeItemCon.items[count].itemSlotSprite.transform;
				itemIcon.transform.localPosition = new Vector3( 0.0f, 0.0f, -2.0f);
				storeItemCon.items[count].itemIconObject = itemIcon;
				storeItemCon.items[count].itemName.Text = itemName;
				storeItemCon.items[count].itemPrice.Text = itemPrice;

				//data
				storeItemCon.items[count].itemUIInfoData = new StoreItemInfoUI( info.Type, itemID, buyAmount.ToString(),
					info.Count, info.Key, itemName);

				// trade item
				if (info.TradeItemID != -1)
				{
					SimpleSprite goodSprite = storeItemCon.items[count].goodSprite;

					if (goodSprite != null)
					{
						goodSprite.gameObject.SetActive(false);

						GameObject icon = GetItemIcon(info.TradeItemID.ToString(), 1);
						icon.transform.parent = goodSprite.transform.parent;
						icon.transform.localPosition = goodSprite.gameObject.transform.localPosition;
						icon.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
					}

					storeItemCon.items[count].itemPrice.Text = info.TradeCount.ToString( "#,#0", CultureInfo.InvariantCulture);
				}

				// resetTime

			}
			count++;
		}
	
		storeItemCon.initilized = true;
	
		UIListItemContainer con = item.gameObject.GetComponent<UIListItemContainer>();
		con.ScanChildren();
	}

	protected virtual void BuyItemBtnInputDelegate(ref POINTER_INFO ptr)
	{
	}

	protected virtual void Cancel()
	{
		nowState = StoreState.NOTOUCH;
	}

	public virtual AsMessageBox ShowAskDealMessageBox( string _title, string _message, string _dealFuncName)
	{
		return AsNotify.Instance.MessageBox( _title, _message, AsTableManager.Instance.GetTbl_String(1152), AsTableManager.Instance.GetTbl_String(1151),
			this, _dealFuncName, "Cancel", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);
	}

	private void ChangeClassTypeDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt != POINTER_INFO.INPUT_EVENT.TAP)
			return;

		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		if( nowState == StoreState.NORMAL || nowState == StoreState.SELECT || nowState == StoreState.CHANGE_CLASS)
		{
			if( ptr.hitInfo.collider.gameObject == classTypeBtn.gameObject)
			{
				if( isShowClassPopup == false)
				{
					isShowClassPopup = true;
					LockInput( true);
					nowState = StoreState.CHANGE_CLASS;
					classTypePanel.StartTransition( UIPanelManager.SHOW_MODE.BringInForward);
				}
				else
				{
					isShowClassPopup = false;
					classTypePanel.StartTransition( UIPanelManager.SHOW_MODE.BringInBack);
				}
			}
			else
			{
				UIRadioBtn button = ( UIRadioBtn)ptr.targetObj;
				nowClass = ( eCLASS)button.data;
				textClass.Text = button.spriteText.Text;
				isShowClassPopup = false;
				RepositionKindBtn( nowNpcID, nowClass);
				SetStoreItem( nowNpcID, nowClass);
				classTypePanel.StartTransition( UIPanelManager.SHOW_MODE.BringInBack);
			}
		}
	}

	public void TransitionEndDelegate( EZTransition transition)
	{
		nowState = StoreState.NOTOUCH;
	}

	protected void PlayItemSound( StoreItemInfoUI _storeItemInfo)
	{
		string szSoundPath = AsSoundPath.ButtonClick;

		if( _storeItemInfo.item_Type == Store_Item_Type.ChargeItem)
		{
			szSoundPath = AsSoundPath.CashItem_Miracle;
		}
		else if( _storeItemInfo.item_Type == Store_Item_Type.NormalItem)
		{
			Item item = ItemMgr.ItemManagement.GetItem( System.Convert.ToInt32( _storeItemInfo.itemID));
			Item.eITEM_TYPE itemType = item.ItemData.GetItemType();

			switch ( itemType)
			{
				case Item.eITEM_TYPE.UseItem:
					{
						Item.eUSE_ITEM useItemType = ( Item.eUSE_ITEM)item.ItemData.m_iSubType;
						if( useItemType == Item.eUSE_ITEM.Random  || useItemType == Item.eUSE_ITEM.QuestRandom)
							szSoundPath = AsSoundPath.CashItem_RandomBox;
						else if( useItemType == Item.eUSE_ITEM.PrivateStore1 || useItemType == Item.eUSE_ITEM.PrivateStore2 ||
								useItemType == Item.eUSE_ITEM.PrivateStore3 || useItemType == Item.eUSE_ITEM.PrivateStore4 || useItemType == Item.eUSE_ITEM.SkillReset)
							szSoundPath = AsSoundPath.CashItem_Etc;
					}
					break;
				case Item.eITEM_TYPE.EtcItem:
					{
						Item.eEtcItem etcItemType = ( Item.eEtcItem)item.ItemData.m_iSubType;

						if( etcItemType == Item.eEtcItem.Gold)
							szSoundPath = AsSoundPath.CashItem_Gold;
						else if( etcItemType == Item.eEtcItem.Strengthen)
							szSoundPath = AsSoundPath.CashItem_Jewel;
					}
					break;
				case Item.eITEM_TYPE.CosEquipItem:
					szSoundPath = AsSoundPath.CashItem_Costume;
					break;
				case Item.eITEM_TYPE.ActionItem:
					szSoundPath = AsSoundPath.CashItem_Use;
					break;
			}
		}

		if( szSoundPath != string.Empty)
			AsSoundManager.Instance.PlaySound( szSoundPath, Vector3.zero, false);
	}

	#region Input Process
	public virtual void ProcessUpClick()
	{
		if( scrollList == null || nowState == StoreState.NOTOUCH)
		{
			isClicked = false;
			return;
		}

		if( scrollList.IsAutoScrolling == true || scrollList.IsScrolling == true)
		{
			isClicked = false;
			return;
		}

		if( IsLockInput == true)
		{
			isClicked = false;
			return;
		}

		if( nowState == StoreState.NORMAL || nowState == StoreState.SELECT)
		{
			if( isClicked == false)
				return;

			fClickTimePassed += Time.deltaTime;

			if( fClickTimePassed >= fDClickTime)
			{
				nowTouchInfo = OneClickTouchInfo;
				SelectStoreItem( nowTouchInfo);
				prevTouchInfo = nowTouchInfo;
				fStationaryPassTime = 0.0f;
				nowState = StoreState.SELECT;

				StoreItemInfoUI storeItemInfo = nowTouchInfo.storeItemController.GetStoreItemInfo( nowTouchInfo.type);

				PlayItemSound( storeItemInfo);

				if( storeItemInfo == null)
				{
					fClickTimePassed = 0.0f;
					isClicked = false;
					return;
				}

				/*
				// exception gold item
				if( storeItemInfo.item_Type == Store_Item_Type.NormalItem)
				{
					int itemID = System.Convert.ToInt32( storeItemInfo.itemID);
					Item itemData = ItemMgr.ItemManagement.GetItem( itemID);

					if( itemData.ItemData.GetItemType() == Item.eITEM_TYPE.EtcItem)
					{
						if( itemData.ItemData.GetSubType() == ( int)Item.eEtcItem.Gold)
						{
							fClickTimePassed = 0.0f;
							isClicked = false;
							PrepareBuyNowTouchItem( nowTouchInfo);
							return;
						}
					}
				}
				*/
				// exception miracle
				if( storeItemInfo.item_Type == Store_Item_Type.ChargeItem)
				{
					fClickTimePassed = 0.0f;
					isClicked = false;
					PrepareBuyNowTouchItem( nowTouchInfo);
					return;
				}

				if( storeItemInfo.item_Type == Store_Item_Type.NormalItem)
				{
					int itemID = System.Convert.ToInt32( storeItemInfo.itemID);
					Item itemData = ItemMgr.ItemManagement.GetItem( itemID);

					if( itemData != null)
					{
						RealItem haveitem = null;

						if( Item.eITEM_TYPE.EquipItem == itemData.ItemData.GetItemType())
							haveitem = ItemMgr.HadItemManagement.Inven.GetEquipItem( itemData.ItemData.GetSubType());
						else if( Item.eITEM_TYPE.CosEquipItem == itemData.ItemData.GetItemType())
							haveitem = ItemMgr.HadItemManagement.Inven.GetCosEquipItem( itemData.ItemData.GetSubType());

						OpenTooltip( haveitem, itemData, this, "SetTooltipClick", -1, storeItemInfo.itemSlot);
					}
				}

				fClickTimePassed = 0.0f;
				isClicked = false;
			}
		}
	}
	
	public virtual void OpenTooltip(RealItem _reamItem, Item _itemData, MonoBehaviour _script, string method, int _price = -1, int _key = -1)
	{
		if (TooltipMgr.Instance != null)
			TooltipMgr.Instance.OpenShopTooltip(TooltipMgr.eOPEN_DLG.left, _reamItem, _itemData, _script, method, _price);
	}

	public void SetTooltipClick()
	{
		LockInput( true);
		isDownStoreOut = false;
		nowState = StoreState.QUANTITY_BUY;
		nowStoreItemInfo = nowTouchInfo.storeItemController.GetStoreItemInfo( nowTouchInfo.type);
		PrepareBuyItemEvent( nowStoreItemInfo);
	}

	public IUIListObject GetStoreItemInfo( int _itemID)
	{
		if( scrollList == null)
			return null;

		List<IUIListObject> items = scrollList.GetItems();

		foreach( IUIListObject item in items)
		{
			StoreItemController storeItemCon = item.gameObject.GetComponent<StoreItemController>();

			if( storeItemCon != null)
			{
				StoreItemInfoUI info = storeItemCon.GetStoreItemInfo();
				if( info.itemID == _itemID.ToString())
					return item;
			}
		}

		return null;
	}
	

	// DUpClick
	public virtual void ProcessDClickUp()
	{
		if( scrollList == null || nowState == StoreState.NOTOUCH)
			return;

		if( scrollList.IsAutoScrolling == true || scrollList.IsScrolling == true)
			return;

		if( nowState == StoreState.NORMAL || nowState == StoreState.SELECT)
		{
			//// double tap check
			if( OneClickTouchInfo.idx != TwoClickTouchInfo.idx)
				return;

			if( OneClickTouchInfo.type != TwoClickTouchInfo.type)
				return;

			if( TooltipMgr.Instance != null)
				TooltipMgr.Instance.Clear();

			PrepareBuyNowTouchItem( TwoClickTouchInfo);
		}
	}

	public void PrepareBuyNowTouchItem( StoreTouchInfo _touchInfo)
	{
		nowTouchInfo = _touchInfo;
		SelectStoreItem( nowTouchInfo);
		prevTouchInfo = nowTouchInfo;
		LockInput( true);
		isDownStoreOut = false;
		nowState = StoreState.QUANTITY_BUY;

		AsSoundManager.Instance.PlaySound( AsSoundPath.ButtonClick, Vector3.zero, false);

		nowStoreItemInfo = nowTouchInfo.storeItemController.GetStoreItemInfo( nowTouchInfo.type);

		PrepareBuyItemEvent( nowStoreItemInfo);
	}

	// Move
	protected void MoveStoreItem()
	{
		if( nowState != StoreState.DRAG)
			return;

		if( isDraingItem == true)
		{
			// move icon
			if( nowSelectIcon != null)
			{
				Vector3 vMousePos = AsInputManager.Instance.UICamera.ScreenToWorldPoint( Input.mousePosition);

				if( useTouchOffset == true)
					vMousePos += touchOffset;

				nowSelectIcon.transform.position = new Vector3( vMousePos.x, vMousePos.y, nowTouchInfo.touchObject.transform.position.z - 10.0f);
			}
		}
	}

	// DownClick
	public void InputDown( Ray _ray)
	{
		// release select
		if( nowState == StoreState.NORMAL || nowState == StoreState.SELECT)
		{
			if( true == AsHudDlgMgr.Instance.IsOpenInven)
			{
				if( true == AsHudDlgMgr.Instance.invenDlg.IsRect( _ray))
					isDownStoreOut = true;
			}
		}
	}

	public void InputOverTwoTouchUp( Ray _ray)
	{
		// cancel move
		if( nowState == StoreState.DRAG && isDraingItem == true)
		{
			//Drop 작업
			isDraingItem = false;

			LockInput( false);

			if( nowSelectIcon != null)
			{
				ReleaseStoreItem( nowTouchInfo);
				GameObject.DestroyImmediate( nowSelectIcon);
				nowSelectIcon = null;
				nowSelectIcon = null;
				nowState = StoreState.NORMAL;
	
				if( ReleaseBuyItemEvent != null)
					ReleaseBuyItemEvent( _ray);
			}
		}
	}

	// InputUp
	public virtual void InputUp( Ray _ray)
	{
		if( scrollList == null || nowState == StoreState.NOTOUCH)
			return;

		if( scrollList.IsAutoScrolling == true || scrollList.IsScrolling == true)
			return;

		if( nowState == StoreState.NORMAL || nowState == StoreState.SELECT)
		{
			StoreTouchInfo touchInfo = FindClickStoreItem( _ray);

			if( touchInfo.type != StoreTouchType.STORE_TOUCH_NONE)
			{
				if( isDownStoreOut == false)
				{
					if( isClicked == false)
					{
						OneClickTouchInfo = touchInfo;
						isClicked = true;
					}
					else // doubleClick
					{
						isClicked = false;
						fClickTimePassed = 0.0f;
						TwoClickTouchInfo = touchInfo;
						ProcessDClickUp();
					}
				}

				if( isDownStoreOut == true)
					isDownStoreOut = false;
			}
		}

		if( nowState == StoreState.DRAG && isDraingItem == true)
		{
			//Drop 작업
			isDraingItem = false;

			LockInput( false);

			if( nowSelectIcon != null)
			{
				//ReleaseStoreItem( nowTouchInfo);
				GameObject.DestroyImmediate( nowSelectIcon);
				nowSelectIcon = null;
				nowSelectIcon = null;
				nowState = StoreState.NOTOUCH;
	
				if( ReleaseBuyItemEvent != null)
					ReleaseBuyItemEvent( _ray);
			}
		}

		if( AsHudDlgMgr.Instance != null)
		{
			if( true == AsHudDlgMgr.Instance.IsOpenInven)
			{
				if( true == AsHudDlgMgr.Instance.invenDlg.IsRect( _ray))
					isDownStoreOut = false;
			}
		}
	}

	public virtual void InputMove( Ray _ray)
	{
		if (isLockInput == true)
			return;

		if( scrollList == null || nowState == StoreState.NOTOUCH)
			return;

		if( scrollList.IsAutoScrolling == true || isDownStoreOut == true || scrollList.IsScrolling == true)
			return;

		if( nowState == StoreState.NORMAL || nowState == StoreState.SELECT)
		{
			if( isDraingItem == true)
				return;

			StoreTouchInfo touchInfo = FindClickStoreItem( _ray);

			if( touchInfo.type == StoreTouchType.STORE_TOUCH_NONE)
				return;

			nowTouchInfo = touchInfo;

			fStationaryPassTime += Time.deltaTime;

			if( fStationaryPassTime >= fStationaryTime)
			{
				LockInput( true);

				SelectStoreItem( nowTouchInfo);

				prevTouchInfo = nowTouchInfo;

				StoreItemInfoUI itemInfo = touchInfo.storeItemController.GetStoreItemInfo();

				// test
				if( itemInfo.item_Type == Store_Item_Type.NormalItem)
					nowSelectIcon = GetItemIcon( itemInfo.itemID, itemInfo.itemBunch);
				else
					nowSelectIcon = GetItemIcon( "90000", 1);

				Vector3 nowMousePos = AsInputManager.Instance.UICamera.ScreenToWorldPoint( Input.mousePosition);
				touchOffset = nowTouchInfo.touchObject.transform.position - nowMousePos;
				isDraingItem = true;

				fStationaryPassTime = 0.0f;

				if( TooltipMgr.Instance != null)
					TooltipMgr.Instance.Clear();
	
				nowState = StoreState.DRAG;
			}
		}
	}

	public virtual void LockInput( bool _value)
	{
		isLockInput = _value;

		if( scrollList != null)
			scrollList.LockScroll = _value;

		if( scrollCategory != null)
			scrollCategory.LockScroll = _value;

		foreach ( UIRadioBtn radio in radioBtns)
			radio.useLockInput = _value;

		if( AsHudDlgMgr.Instance != null)
		{
			if( AsHudDlgMgr.Instance.invenDlg != null)
				AsHudDlgMgr.Instance.invenDlg.VisibleToolTip = !_value;
		}
	}

	private void ReleaseStoreItem( StoreTouchInfo _touchInfo)
	{
		if( _touchInfo != null && _touchInfo.idx != -1)
			_touchInfo.storeItemController.RemoveFocus( transform);
	}
	#endregion

	#region Util
	public static GameObject GetItemIcon( string _itemID, int _itemBunch, int _itemMax = 0)
	{
		// item
		Item itemData = ItemMgr.ItemManagement.GetItem( System.Convert.ToInt32( _itemID));
		GameObject icon = itemData.GetIcon();
		if( null == icon)
			return null;

		GameObject objIconInstantiate = GameObject.Instantiate( icon) as GameObject;

		UISlotItem slotItem = objIconInstantiate.GetComponentInChildren<UISlotItem>();

		if( _itemBunch > 1)
			slotItem.itemCountText.Text = _itemBunch.ToString();

		if (_itemMax != 0)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder("(");
			sb.Append(_itemMax.ToString());
			sb.Append(")");

			slotItem.itemCountText.Text = sb.ToString();
		}

		return objIconInstantiate;
	}

	public bool CheckItemReleaseOnNpcStore( Ray _ray)
	{
		return collider.bounds.IntersectRay( _ray);
	}

	public bool IsPossibleClose()
	{
		if( scrollList != null)
		{
			if( scrollList.LockScroll == true)
				return false;
		}
	
		return true;
	}

	public eCHARGETYPE GetChargeType( string _productID)
	{
		if( dicChargeItemInfo.ContainsKey( _productID))
			return dicChargeItemInfo[_productID].eChargeType;
		else
			return eCHARGETYPE.eCHARGETYPE_NOTHING;
	}

	/// <summary>
	/// Find 
	/// </summary>
	/// <param name="_ray"></param>
	/// <returns></returns>
	public virtual StoreTouchInfo FindClickStoreItem( Ray _ray)
	{
		StoreTouchInfo touchInfo = new StoreTouchInfo();

		if( !AsUtil.PtInCollider( scrollList.collider, _ray))
			return touchInfo;

		int count = scrollList.Count;
		for ( int i = 0; i < count; i++)
		{
			IUIListObject item = scrollList.GetItem( i);

			StoreItemController storeControl = item.gameObject.GetComponent<StoreItemController>();

			if( storeControl != null)
			{
				foreach ( StoreItemControllerInfo info in storeControl.items)
				{
					bool bCheck = false;

					// On box Collider
					if( info != null)
						info.boxCollider.enabled = true;

					if( info != null)
						bCheck = AsUtil.PtInCollider( info.boxCollider, _ray);

					// off boxCollider
					if( info != null)
						info.boxCollider.enabled = false;

					if( bCheck == true)
					{
						touchInfo.idx = i;
						touchInfo.storeItemController = storeControl;
						touchInfo.type = info.touchType;
						touchInfo.touchObject = info.itemIconObject;
						return touchInfo;
					}
				}
			}
		}

		return touchInfo;
	}
	#endregion

	#region -virtual functions
	public virtual void InitilizeStore( int _npcId, eCLASS _eClass) { }
	protected virtual void InitKindMenu() { }
	protected virtual void RepositionKindBtn( int _npcId, eCLASS _eClass) { }
	protected virtual void SetStoreItem( int _npcID, eCLASS _class) { }
	protected virtual void ChangeKindTypeDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( AsSoundPath.ButtonClick, Vector3.zero, false);

			if( prevTouchInfo != null && prevTouchInfo.idx != -1)
				prevTouchInfo.storeItemController.RemoveFocus( transform);
		}
	}
	protected virtual void Deal() { }
	protected virtual void CloseDelegate( ref POINTER_INFO ptr) { }
	#endregion
}
