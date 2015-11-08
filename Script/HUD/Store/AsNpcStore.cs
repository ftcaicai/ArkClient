using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System.Globalization;


public class AsNpcStore : AsStore
{
	protected int nowSessionNpcID = 0;
	protected eStore_ItemKind nowItemKind = eStore_ItemKind.WEAPON;
	public eStore_ItemKind[] kinds;
	protected Dictionary<eStore_ItemKind, UIRadioBtn> dicItemKindBtn = new Dictionary<eStore_ItemKind, UIRadioBtn>();
	public static string SaveKindKey = "NpcStoreKind";

	public SpriteText m_TextTitle;
	public GameObject listItemMiraclePrefab;
	public AsMessageBox messageBox = null;
	public Item.eGOODS_TYPE buyItemGoodsType = Item.eGOODS_TYPE.Gold;
	public int[] menuNameIDs = null;
	public GameObject loadingGear = null;
	public TextAsset itemLimitFont;
	public TextAsset itemNormalFont;
	public Material itemLimitFontMat;
	public Material itemNormalFontMat;

	public Dictionary<int, body2_SC_SHOP_INFO> dicShopInfo = new Dictionary<int, body2_SC_SHOP_INFO>();

	private System.Text.StringBuilder m_sbTemp = new System.Text.StringBuilder();

	void Awake()
	{
		Initilize();
		BuyItemEvent = BuyItem;
		SellItemEvent = SellItem;
		PrepareBuyItemEvent = PrepareBuyItem;
		ReleaseBuyItemEvent = CheckItemReleaseOnInventory;
	}

	void Start()
	{
		SetUIText();
	}

	protected void SetUIText()
	{
		SetTextAtSpriteTxt( m_TextTitle, AsTableManager.Instance.GetTbl_String(37908));

		for( int c = 0; c < radioBtns.Length; c++)
			SetTextAtSpriteTxt( radioBtns[c].spriteText, AsTableManager.Instance.GetTbl_String(menuNameIDs[c]));

		for( int c = 0; c < classBtns.Length; c++)
		{
			string szText = AsTableManager.Instance.GetTbl_String(306 + c);

			SetTextAtSpriteTxt( classBtns[c].spriteText, szText);

			if( (eCLASS)classBtns[c].data == nowUserClass)
			{
				textClass.Text = szText;
				classBtns[c].Value = true;
			}
		}
	}

	protected void QuantityOK()
	{
		Item item = ItemMgr.ItemManagement.GetItem( System.Convert.ToInt32( quantityMsgBox.QuantityInfo.itemID));
		if( null == item)
			return;

		string szMessage = string.Empty;
		m_sbTemp.Length = 0;
		m_sbTemp.Append( ItemMgr.GetGradeColor( item.ItemData.grade));
		m_sbTemp.Append( quantityMsgBox.QuantityInfo.itemName);
		m_sbTemp.Append( Color.white);

		StringBuilder sbTradeCount = new StringBuilder();
		StringBuilder sbNeedTradeCount = new StringBuilder();

		if( nowState == StoreState.QUANTITY_BUY)
		{
			Store_Item_Info_Table storeItemInfo = AsTableManager.Instance.GetStoreItem(nowNpcID, item.ItemID);

			if (storeItemInfo.TradeItemID != -1)
			{
				int totalTradeCount = ItemMgr.HadItemManagement.Inven.GetItemTotalCount(storeItemInfo.TradeItemID);

				int nameStringID = ItemMgr.ItemManagement.GetItem(storeItemInfo.TradeItemID).ItemData.nameId;

				sbNeedTradeCount.Append(AsTableManager.Instance.GetTbl_String(nameStringID));
				sbNeedTradeCount.Append(" ");
				sbNeedTradeCount.Append((storeItemInfo.TradeCount * quantityMsgBox.NowItemCount).ToString("#,#0", CultureInfo.InvariantCulture));


				sbTradeCount.Append(AsTableManager.Instance.GetTbl_String(nameStringID));
				sbTradeCount.Append(" ");
				sbTradeCount.Append(totalTradeCount.ToString("#,#0", CultureInfo.InvariantCulture));


				szMessage = string.Format(AsTableManager.Instance.GetTbl_String(2191), m_sbTemp.ToString(), quantityMsgBox.NowItemCount,
										  sbNeedTradeCount.ToString(),
										  sbTradeCount.ToString());
			}
			else if( item.ItemData.getGoodsType == Item.eGOODS_TYPE.Gold)
			{
				szMessage = string.Format( AsTableManager.Instance.GetTbl_String(407), m_sbTemp.ToString(), quantityMsgBox.NowItemCount,
					( item.ItemData.buyAmount * quantityMsgBox.NowItemCount).ToString( "#,#0", CultureInfo.InvariantCulture));
			}
			else
			{
				szMessage = string.Format( AsTableManager.Instance.GetTbl_String(409), m_sbTemp.ToString(), quantityMsgBox.NowItemCount,
					( item.ItemData.buyAmount * quantityMsgBox.NowItemCount).ToString( "#,#0", CultureInfo.InvariantCulture));
			}
		}
		else if( nowState == StoreState.QUANTITY_SELL)
		{
			szMessage = string.Format( AsTableManager.Instance.GetTbl_String(250), m_sbTemp.ToString(), quantityMsgBox.NowItemCount);
		}
		messageBox = ShowAskDealMessageBox( AsTableManager.Instance.GetTbl_String(126), szMessage, "Deal");
	}

	// weapon's overraped is 1
	void BuyItemOnlyOne( string _itemId, string _itemName, string _itemPrice, int _itemSlot)
	{
		Item item = ItemMgr.ItemManagement.GetItem( System.Convert.ToInt32( _itemId));
		if( null == item)
			return;

		m_sbTemp.Length = 0;
		m_sbTemp.Append( ItemMgr.GetGradeColor( item.ItemData.grade));
		m_sbTemp.Append( _itemName);
		m_sbTemp.Append( Color.white);

		quantityMsgBox.QuantityInfo = new StoreQuantityInfo( _itemName, _itemId, _itemPrice, _itemSlot, 1);

		string szMessage = string.Empty;

		Store_Item_Info_Table storeItemInfo = AsTableManager.Instance.GetStoreItem(nowNpcID, item.ItemID);

		if (storeItemInfo.TradeItemID != -1)
		{
			StringBuilder sbTradeCount = new StringBuilder();
			StringBuilder sbNeedTradeCount = new StringBuilder();

			int nameStringID = ItemMgr.ItemManagement.GetItem(storeItemInfo.TradeItemID).ItemData.nameId;

			int totalTradeCount = ItemMgr.HadItemManagement.Inven.GetItemTotalCount(storeItemInfo.TradeItemID);


			sbNeedTradeCount.Append(AsTableManager.Instance.GetTbl_String(nameStringID));
			sbNeedTradeCount.Append(" ");
			sbNeedTradeCount.Append((storeItemInfo.TradeCount).ToString("#,#0", CultureInfo.InvariantCulture));


			sbTradeCount.Append(AsTableManager.Instance.GetTbl_String(nameStringID));
			sbTradeCount.Append(" ");
			sbTradeCount.Append(totalTradeCount.ToString("#,#0", CultureInfo.InvariantCulture));


			szMessage = string.Format(AsTableManager.Instance.GetTbl_String(2191), m_sbTemp.ToString(), 1,
										  sbNeedTradeCount.ToString(),
										  sbTradeCount.ToString());
		}
		else if( item.ItemData.getGoodsType == Item.eGOODS_TYPE.Gold)
			szMessage = string.Format( AsTableManager.Instance.GetTbl_String(407), m_sbTemp.ToString(), 1, item.ItemData.buyAmount.ToString( "#,#0", CultureInfo.InvariantCulture));
		else
			szMessage = string.Format( AsTableManager.Instance.GetTbl_String(409), m_sbTemp.ToString(), 1, item.ItemData.buyAmount.ToString( "#,#0", CultureInfo.InvariantCulture));

		messageBox = ShowAskDealMessageBox( AsTableManager.Instance.GetTbl_String(126), szMessage, "Deal");
	}

	void SellItemOnlyOne( string _itemId, string _itemName, string _itemPrice)
	{
		Item item = ItemMgr.ItemManagement.GetItem( System.Convert.ToInt32( _itemId));
		if( null == item)
			return;

		m_sbTemp.Length = 0;
		m_sbTemp.Append( ItemMgr.GetGradeColor( item.ItemData.grade));
		m_sbTemp.Append( _itemName);
		m_sbTemp.Append( Color.white);

		quantityMsgBox.QuantityInfo = new StoreQuantityInfo( _itemName, _itemId, _itemPrice, 0, 1);
		string szMessage = string.Format( AsTableManager.Instance.GetTbl_String(250), m_sbTemp.ToString(), 1);
		messageBox = ShowAskDealMessageBox( AsTableManager.Instance.GetTbl_String(126), szMessage, "Deal");
	}

	public void SellItemFromInventory( RealItem _realItem)
	{
		// send packet to server
		ItemData itemData = ItemMgr.ItemManagement.GetItem( _realItem.sItem.nItemTableIdx).ItemData;
		string itemName = AsTableManager.Instance.GetTbl_String(itemData.nameId);

		
		nowRealItem = _realItem;

		Item.eGOODS_TYPE goodsType = itemData.getGoodsType;

        if (_realItem.sItem.nOverlapped == 1)
        {
            string itemNameCon = itemName;

            if (_realItem.sItem.nStrengthenCount > 0)
            {
                StringBuilder sb = new StringBuilder("+");
                sb.Append(_realItem.sItem.nStrengthenCount.ToString());
                sb.Append(" ");
                sb.Append(itemName);
                itemNameCon = sb.ToString();
            }

            SellItemOnlyOne(itemData.GetID().ToString(), itemNameCon, itemData.sellAmount.ToString());
        }
        else
        {
            GameObject objIcon = GetItemIcon(itemData.GetID().ToString(), 1);
            quantityMsgBox.Open(new StoreItemInfoUI(Store_Item_Type.NormalItem, _realItem.sItem.nItemTableIdx.ToString(), itemData.sellAmount.ToString(), 1, 0, itemName), _realItem.sItem.nOverlapped, QuantityOK, Cancel, objIcon, goodsType);
        }
	
		LockInput( true);
	
		nowState = StoreState.QUANTITY_SELL;
	}

	public void PlayDealSound()
	{
		if( buyItemGoodsType == Item.eGOODS_TYPE.Gold)
			AsSoundManager.Instance.PlaySound( AsSoundPath.ItemBuyResultGold, Vector3.zero, false);
		else if( buyItemGoodsType == Item.eGOODS_TYPE.Cash)
			AsSoundManager.Instance.PlaySound( AsSoundPath.itemBuyResultMiracle, Vector3.zero, false);
	}

	#region -delegate func
	public void BuyItem()
	{
		StoreQuantityInfo quantityInfo = quantityMsgBox.QuantityInfo;
		AsNpcEntity entity = AsEntityManager.Instance.GetNPCEntityByTableID( nowNpcID);

		// buy
		Debug.LogWarning( "Buy Item = " + quantityInfo.ToString());

		ItemData itemData = ItemMgr.ItemManagement.GetItem( System.Convert.ToInt32( quantityInfo.itemID)).ItemData;

		buyItemGoodsType = itemData.getGoodsType;

		AsCommonSender.BuyNpcStoreItem( entity.sessionId_, quantityInfo.itemSlot, quantityInfo.itemCount);
	}

	public void SellItem()
	{
		StoreQuantityInfo quantityInfo = quantityMsgBox.QuantityInfo;
		AsNpcEntity entity = AsEntityManager.Instance.GetNPCEntityByTableID( nowNpcID);
		// sell
		if( nowRealItem != null)
		{
			ItemData itemData = ItemMgr.ItemManagement.GetItem( System.Convert.ToInt32( quantityInfo.itemID)).ItemData;
	
			buyItemGoodsType = itemData.getGoodsType;
	
			AsCommonSender.SellInvenItem( entity.sessionId_, nowRealItem.getSlot, quantityInfo.itemCount);
			Debug.LogWarning( "Sell Item = " + quantityInfo.ToString());
		}
	}

	public void PrepareBuyItem( StoreItemInfoUI _storeItemInfo)
	{
		ItemData itemData = ItemMgr.ItemManagement.GetItem( System.Convert.ToInt32( _storeItemInfo.itemID)).ItemData;

		int itemOverrap = itemData.overlapCount;
		Item.eGOODS_TYPE goodsType = itemData.getGoodsType;

		if( itemOverrap == 1)
			BuyItemOnlyOne( _storeItemInfo.itemID, _storeItemInfo.itemName, _storeItemInfo.itemPrice, _storeItemInfo.itemSlot);
		else
		{
			GameObject objIcon = GetItemIcon( _storeItemInfo.itemID, _storeItemInfo.itemBunch);

			if ( _storeItemInfo.itemBuyMax != -1 && dicShopInfo.ContainsKey(_storeItemInfo.itemSlot))
			{
				int itemBuyMax = dicShopInfo[_storeItemInfo.itemSlot].nItemCount;

				quantityMsgBox.Open(_storeItemInfo, itemBuyMax, QuantityOK, Cancel, objIcon, goodsType);
			}
			else
				quantityMsgBox.Open(_storeItemInfo, ItemBuyMax, QuantityOK, Cancel, objIcon, goodsType);
		}
	}

	// release on inventory
	public void CheckItemReleaseOnInventory( Ray _ray)
	{
		if( false == AsHudDlgMgr.Instance.IsOpenInven)
			return;

		if( true == AsHudDlgMgr.Instance.invenDlg.IsRect( _ray))
		{
			LockInput( true);

			nowState = StoreState.QUANTITY_BUY;

			nowStoreItemInfo = nowTouchInfo.storeItemController.GetStoreItemInfo();

			ItemData itemData = ItemMgr.ItemManagement.GetItem( System.Convert.ToInt32( nowStoreItemInfo.itemID)).ItemData;

			int itemOverrap = itemData.overlapCount;
			Item.eGOODS_TYPE goodsType = itemData.getGoodsType;

			if( itemOverrap == 1)
				BuyItemOnlyOne( nowStoreItemInfo.itemID, nowStoreItemInfo.itemName, nowStoreItemInfo.itemPrice, nowStoreItemInfo.itemSlot);
			else
			{
				GameObject objIcon = GetItemIcon( nowStoreItemInfo.itemID, nowStoreItemInfo.itemBunch);
				quantityMsgBox.Open( nowStoreItemInfo, ItemBuyMax, QuantityOK, Cancel, objIcon, goodsType);
			}
		}
	}
	#endregion

	#region -override func
	public override void AddStoreItemLine( bool _isMiracle = false)
	{
		if( scrollList == null)
			return;

		IUIListObject item = null;

		if( _isMiracle == true)
			item = scrollList.CreateItem( listItemMiraclePrefab);
		else
			item = scrollList.CreateItem( listItemPrefab);

		item.gameObject.name = scrollList.Count.ToString();
		item.gameObject.layer = listItemPrefab.layer;
		scrollList.UpdateCamera();
	}

	public override void AddStoreItems( List<Store_Item_Info_Table> _storeItemList)
	{
		scrollList.ScrollWheel( 0);
		scrollList.ClearList( true);

		int line = _storeItemList.Count;
		ItemData itemData = null;

		for( int i = 0; i < line; i++)
		{
			itemData = ItemMgr.ItemManagement.GetItem( _storeItemList[i].ID).ItemData;
			if( itemData.getGoodsType == Item.eGOODS_TYPE.Cash)
				AddStoreItemLine( true);
			else
				AddStoreItemLine();
		}

		for( int i = 0; i < line; i++)
			AddStoreItem( i, _storeItemList[i]);
	}

	protected override void AddStoreItem(int _line, params Store_Item_Info_Table[] _storeItem)
	{
		IUIListObject item = scrollList.GetItem(_line);
		StoreItemController storeItemCon = item.gameObject.GetComponent<StoreItemController>();
		GameObject itemIcon = null;

		string itemID = string.Empty;
		string itemName = string.Empty;
		string itemPrice = string.Empty;
		int buyAmount = 0;

		int count = 0;
		int itemMaxCount = 0;
		foreach (Store_Item_Info_Table info in _storeItem)
		{
			if (info.Type == Store_Item_Type.NormalItem)
			{
				ItemData itemData = null;
				itemID = info.ID.ToString();
				itemData = ItemMgr.ItemManagement.GetItem(info.ID).ItemData;
				itemName = AsTableManager.Instance.GetTbl_String(itemData.nameId);
				itemPrice = (itemData.buyAmount * info.Count).ToString("#,#0", CultureInfo.InvariantCulture);
				buyAmount = itemData.buyAmount;
				itemIcon = GetItemIcon(info.ID.ToString(), info.Count, itemMaxCount);
			}

			if (info != null)
			{
				itemIcon.transform.parent = storeItemCon.items[count].itemSlotSprite.transform;
				itemIcon.transform.localPosition = new Vector3(0.0f, 0.0f, -2.0f);
				storeItemCon.items[count].itemIconObject = itemIcon;
				storeItemCon.items[count].itemName.Text = itemName;
				storeItemCon.items[count].itemPrice.Text = itemPrice;

				//data
				storeItemCon.items[count].itemUIInfoData = new StoreItemInfoUI(info.Type, itemID, buyAmount.ToString(),
					info.Count, info.Key, itemName, info.TradeItemID, info.TradeCount, info.ItemBuyMax);

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

					storeItemCon.items[count].itemPrice.Text = info.TradeCount.ToString("#,#0", CultureInfo.InvariantCulture);
				}
			}
			count++;
		}

		storeItemCon.initilized = true;

		UIListItemContainer con = item.gameObject.GetComponent<UIListItemContainer>();
		con.ScanChildren();
	}

	public override void OpenTooltip(RealItem _reamItem, Item _itemData, MonoBehaviour _script, string method, int _price = -1, int _key = -1)
	{
		if (TooltipMgr.Instance != null)
		{
			//if (dicShopInfo.ContainsKey(_key) == true)
			//    if (dicShopInfo[_key].nItemCount <= 0)
			//        return;

			Store_Item_Info_Table storeItemInfo = AsTableManager.Instance.GetStoreItem(nowNpcID, _itemData.ItemID);

			if (storeItemInfo == null)
				return;

			if (storeItemInfo.TradeItemID == -1)
				TooltipMgr.Instance.OpenShopTooltip(TooltipMgr.eOPEN_DLG.left, _reamItem, _itemData, _script, method, _price);
			else
			{
				int tradeItemCount = -1;

				if (dicShopInfo.ContainsKey(_key))
					tradeItemCount = dicShopInfo[_key].nItemCount;
				else
				{
					List<Store_Item_Info_Table> storeItemList = AsTableManager.Instance.GetStoreItems(nowClass, nowItemKind, nowNpcID);

					foreach (Store_Item_Info_Table info in storeItemList)
					{
						if (info.ID == _itemData.ItemID)
							tradeItemCount = info.TradeCount;
					}
				}

				TooltipMgr.Instance.OpenShopTooltip(TooltipMgr.eOPEN_DLG.left, _reamItem, _itemData, _script, method, storeItemInfo.TradeCount, storeItemInfo.TradeItemID, tradeItemCount);
			}
		}
	}

	public override void InputMove(Ray _ray)
	{
		StoreTouchInfo touchInfo = FindClickStoreItem(_ray);

		if (touchInfo.type == StoreTouchType.STORE_TOUCH_NONE)
			return;

		StoreItemInfoUI itemInfo = touchInfo.storeItemController.GetStoreItemInfo();

		if (dicShopInfo.ContainsKey(itemInfo.itemSlot))
			if (dicShopInfo[itemInfo.itemSlot].nResetTime > 0)
				return;

		base.InputMove(_ray);
	}

	// DUpClick
	public override void ProcessDClickUp()
	{
		if (scrollList == null || nowState == StoreState.NOTOUCH)
			return;

		if (scrollList.IsAutoScrolling == true || scrollList.IsScrolling == true)
			return;

		if (nowState == StoreState.NORMAL || nowState == StoreState.SELECT)
		{
			//// double tap check
			if (OneClickTouchInfo.idx != TwoClickTouchInfo.idx)
				return;

			if (OneClickTouchInfo.type != TwoClickTouchInfo.type)
				return;

			if (TooltipMgr.Instance != null)
				TooltipMgr.Instance.Clear();

			StoreItemInfoUI info = TwoClickTouchInfo.storeItemController.items[0].itemUIInfoData;

			if (dicShopInfo.ContainsKey(info.itemSlot))
				if (dicShopInfo[info.itemSlot].nItemCount <= 0)
					return;
				
			PrepareBuyNowTouchItem(TwoClickTouchInfo);
		}
	}

	public void UpdateStoreInfo()
	{
		List<IUIListObject> list = scrollList.GetItems();

		foreach (IUIListObject item in list)
		{
			StoreItemController controller = item.gameObject.GetComponent<StoreItemController>();

			int key = controller.items[0].itemUIInfoData.itemSlot;

			int maxBuy = dicShopInfo.ContainsKey(key) ? dicShopInfo[key].nItemCount : 0;

			StringBuilder sb = new StringBuilder();
			sb.Append("(");
			sb.Append(maxBuy.ToString());
			sb.Append(")");

			UISlotItem slotItem = controller.items[0].itemIconObject.GetComponent<UISlotItem>();
			slotItem.itemCountText.Text = maxBuy == 0 ? string.Empty : sb.ToString();

			// item buy limit count
			if (dicShopInfo.ContainsKey(key))
			{
				// change to outline font
				if (maxBuy > 0)
					slotItem.itemCountText.SetFont(itemLimitFont, itemLimitFontMat);
				else
					slotItem.itemCountText.SetFont(itemNormalFont, itemNormalFontMat);
			}

			// resetTime
			if (dicShopInfo.ContainsKey(key))
			{
				if (dicShopInfo[key].nResetTime == 0)
				{
					controller.items[0].itemResetTime.Text = string.Empty;
				}
				else
				{
					long hour = dicShopInfo[key].nResetTime / 3600;
					long minute = dicShopInfo[key].nResetTime / 60;

					if (hour != 0)
						controller.items[0].itemResetTime.Text = string.Format(AsTableManager.Instance.GetTbl_String(2194), hour);
					else if (minute != 0)
						controller.items[0].itemResetTime.Text = string.Format(AsTableManager.Instance.GetTbl_String(2195), minute);
					else
						controller.items[0].itemResetTime.Text = string.Format(AsTableManager.Instance.GetTbl_String(2192), dicShopInfo[key].nResetTime);
				}
				controller.items[0].itemResetTime.UpdateCamera();
			}
			else
				controller.items[0].itemResetTime.Text = string.Empty;

			// shade
			if (dicShopInfo.ContainsKey(key))
			{
				if (dicShopInfo[key].nItemCount == 0)
					controller.items[0].shade.gameObject.SetActive(true);
				else
					controller.items[0].shade.gameObject.SetActive(false);
			}
			else
				controller.items[0].shade.gameObject.SetActive(false);

			UIListItemContainer con = item.gameObject.GetComponent<UIListItemContainer>();
			con.ScanChildren();
		}

		scrollList.UpdateCamera();
	}

	protected override void RepositionKindBtn( int _npcId, eCLASS _eClass)
	{
		List<eStore_ItemKind> listKind = AsTableManager.Instance.GetHaveItemKind( _npcId, _eClass);

		foreach ( UIRadioBtn button in radioBtns)
			button.gameObject.SetActiveRecursively( false);

		int count = 0;
		foreach ( eStore_ItemKind kind in kinds)
		{
			if( listKind.Contains( kind))
			{
				if( count == 0)
					nowItemKind = kind;

				dicItemKindBtn[kind].gameObject.SetActiveRecursively( true);
				dicItemKindBtn[kind].transform.localPosition = radioBtnPositions[count];
				count++;
			}
		}

		dicItemKindBtn[nowItemKind].Value = true;
	}

	protected override void SetStoreItem( int _npcID, eCLASS _class)
	{
		// check saved kind
		if( PlayerPrefs.HasKey( SaveKindKey))
		{
			eStore_ItemKind savedKind = ( eStore_ItemKind)PlayerPrefs.GetInt( SaveKindKey);

			List<eStore_ItemKind> listKind = AsTableManager.Instance.GetHaveItemKind( _npcID, _class);

			if( listKind.Contains( savedKind))
			{
				nowItemKind = savedKind;
				dicItemKindBtn[nowItemKind].Value = true;
			}
		}
		else
			dicItemKindBtn[nowItemKind].Value = true;

		List<Store_Item_Info_Table> storeItemList = AsTableManager.Instance.GetStoreItems( _class, nowItemKind, _npcID);
		AddStoreItems( storeItemList);

		UpdateStoreInfo();
	}

	public void SetNpcSessionID(int _sessionID)
	{
		nowSessionNpcID = _sessionID;
	}

	public int GetNpcSessionID()
	{
		return nowSessionNpcID;
	}

	public override void InitilizeStore(int _npcID, eCLASS _eClass)
	{
		nowNpcID = _npcID;
		nowUserClass = _eClass;
		nowClass = _eClass;

		RepositionKindBtn( nowNpcID, nowUserClass);

		UIRadioBtn nowClassBtn = null;

		foreach ( UIRadioBtn radio in classBtns)
		{
			if( _eClass == (eCLASS)radio.data)
			{
				nowClassBtn = radio;
				textClass.Text = radio.Text;
			}
		}
		
		if( nowClassBtn != null )
			nowClassBtn.Value = true;

		SetStoreItem(nowNpcID, nowUserClass);

		LockInput(true);

		if (loadingGear != null)
			loadingGear.SetActive(true);

		classTypeBtn.controlIsEnabled = false;

		// request shop info
		RequestShopInfo();
	}


	/// <summary>
	/// process shop infoes from server
	/// </summary>
	/// <param name="_shopInfos"></param>
	public void ProcessNpcShopInfo(body2_SC_SHOP_INFO[] _shopInfoes)
	{
		dicShopInfo.Clear();

		foreach (body2_SC_SHOP_INFO info in _shopInfoes)
		{
			if (!dicShopInfo.ContainsKey(info.nShopItemSlot))
				dicShopInfo.Add(info.nShopItemSlot, info);

			Debug.LogWarning(info.ToString());
		}

		UpdateStoreInfo();

		classTypeBtn.controlIsEnabled = true;

		if (loadingGear != null)
			loadingGear.SetActive(false);

		LockInput(false);
	}

	public void RequestShopInfo()
	{
		AsCommonSender.RequestShopInfo(nowSessionNpcID);


		classTypeBtn.controlIsEnabled = false;

		if (loadingGear != null)
			loadingGear.SetActive(true);

		LockInput(true);
	}

	protected override void InitKindMenu()
	{
		int count = 0;
		foreach ( UIRadioBtn button in radioBtns)
		{
			dicItemKindBtn.Add( kinds[count], button);
			button.AddInputDelegate( ChangeKindTypeDelegate);
			radioBtnPositions[count] = button.gameObject.transform.localPosition;
			button.Data = kinds[count];
			button.gameObject.SetActiveRecursively( false);
			count++;
		}
	}

	protected override void ChangeKindTypeDelegate( ref POINTER_INFO ptr)
	{
		base.ChangeKindTypeDelegate( ref ptr);

		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			UIRadioBtn radio = (UIRadioBtn)ptr.targetObj;

			nowItemKind = (eStore_ItemKind)radio.Data;

			PlayerPrefs.SetInt( "NpcStoreKind", (int)nowItemKind);
			PlayerPrefs.Save();

			Debug.LogWarning( nowClass);

			SetStoreItem( nowNpcID, nowClass);
		}
	}

	protected override void Deal()
	{
		if( nowState == StoreState.QUANTITY_BUY && BuyItemEvent != null)
			BuyItemEvent();
		else if( nowState == StoreState.QUANTITY_SELL && SellItemEvent != null)
			SellItemEvent();

		nowState = StoreState.NOTOUCH;
	}

	protected override void CloseDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( AsSoundPath.ButtonClick, Vector3.zero, false);
			AsHudDlgMgr.Instance.CloseNpcStore();
			TooltipMgr.Instance.Clear();
		}
	}

	void OnDisable()
	{
		if( messageBox != null)
			messageBox.Close();
	}
	#endregion
}
