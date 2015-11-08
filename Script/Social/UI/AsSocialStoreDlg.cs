using UnityEngine;
using System.Collections.Generic;
using System.Text;

public class AsSocialStoreDlg : AsStore
{
	protected eStore_ItemKind nowItemKind = eStore_ItemKind.WEAPON;
	public eStore_ItemKind[] kinds;
	protected Dictionary<eStore_ItemKind, UIRadioBtn> dicItemKindBtn = new Dictionary<eStore_ItemKind, UIRadioBtn>();
	public static string SaveKindKey = "SocialStoreKind";
	
	public SpriteText m_TextTitle;
	public GameObject listItemMiraclePrefab;
	public AsMessageBox messageBox = null;
	public Item.eGOODS_TYPE buyItemGoodsType = Item.eGOODS_TYPE.Gold;
	public int[] menuNameIDs = null;
	
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
		SetTextAtSpriteTxt( m_TextTitle, AsTableManager.Instance.GetTbl_String( 37908));

		for( int c = 0; c < radioBtns.Length; c++)
			SetTextAtSpriteTxt( radioBtns[c].spriteText, AsTableManager.Instance.GetTbl_String( menuNameIDs[c]));

		for( int c = 0; c < classBtns.Length; c++)
		{
			string szText = AsTableManager.Instance.GetTbl_String( 306 + c);

			SetTextAtSpriteTxt( classBtns[c].spriteText, szText);

			if( ( eCLASS)classBtns[c].data == nowUserClass)
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
		

		if( nowState == StoreState.QUANTITY_BUY)
			szMessage = string.Format( AsTableManager.Instance.GetTbl_String( 249), m_sbTemp.ToString(), quantityMsgBox.NowItemCount);
		else if( nowState == StoreState.QUANTITY_SELL)
			szMessage = string.Format( AsTableManager.Instance.GetTbl_String( 250), m_sbTemp.ToString(), quantityMsgBox.NowItemCount);

		messageBox = ShowAskDealMessageBox( AsTableManager.Instance.GetTbl_String( 126), szMessage, "Deal");
	}

	// weapon's overraped is 1
	void BuyItemOnlyOne( string _itemId, string _itemName, string _itemPrice, int _itemSlot)
	{
		quantityMsgBox.QuantityInfo = new StoreQuantityInfo( _itemName, _itemId, _itemPrice, _itemSlot, 1);
	
		string szMessage = string.Format( AsTableManager.Instance.GetTbl_String( 249), _itemName, 1);
	
		messageBox = ShowAskDealMessageBox( AsTableManager.Instance.GetTbl_String( 126), szMessage, "Deal");
	}

	void SellItemOnlyOne( string _itemId, string _itemName, string _itemPrice)
	{
		quantityMsgBox.QuantityInfo = new StoreQuantityInfo( _itemName, _itemId, _itemPrice, 0, 1);

		string szMessage = string.Format( AsTableManager.Instance.GetTbl_String( 250), _itemName, 1);

		messageBox = ShowAskDealMessageBox( AsTableManager.Instance.GetTbl_String( 126), szMessage, "Deal");
	}

	public void SellItemFromInventory( RealItem _realItem)
	{
		// send packet to server
		ItemData itemData = ItemMgr.ItemManagement.GetItem( _realItem.sItem.nItemTableIdx).ItemData;
		string itemName = AsTableManager.Instance.GetTbl_String( itemData.nameId);

		GameObject objIcon = GetItemIcon( itemData.GetID().ToString(), 1);
		nowRealItem = _realItem;

		Item.eGOODS_TYPE goodsType = itemData.getGoodsType;

		if( _realItem.sItem.nOverlapped == 1)
		{
			string itemNameCon = itemName;

			if( _realItem.sItem.nStrengthenCount > 0)
			{
				StringBuilder sb = new StringBuilder( "+");
				sb.Append( _realItem.sItem.nStrengthenCount.ToString());
				sb.Append( " ");
				sb.Append( itemName);
				itemNameCon = sb.ToString();
			}

			SellItemOnlyOne( itemData.GetID().ToString(), itemNameCon, itemData.sellAmount.ToString());
		}
		else
			quantityMsgBox.Open( new StoreItemInfoUI( Store_Item_Type.NormalItem, _realItem.sItem.nItemTableIdx.ToString(), itemData.sellAmount.ToString(), 1, 0, itemName), _realItem.sItem.nOverlapped, QuantityOK, Cancel, objIcon, goodsType);

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
		// AsNpcEntity entity = AsEntityManager.Instance.GetNPCEntityByTableID( nowNpcID);
	
		// buy
		// Debug.LogWarning( "Buy Item = " + quantityInfo.ToString());
		AsCommonSender.SendSocialItemBuy( quantityInfo.itemSlot ,( ushort) quantityInfo.itemCount);
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
			quantityMsgBox.Open( _storeItemInfo, ItemBuyMax, QuantityOK, Cancel, objIcon, goodsType);
		}
	}

	// release on inventory
	public void CheckItemReleaseOnInventory( Ray _ray)
	{
		if( false == AsHudDlgMgr.Instance.IsOpenInven)
			return;

		if( AsHudDlgMgr.Instance.invenDlg.IsRect( _ray))
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
	}

	public override void InitilizeStore( int _npcId, eCLASS _eClass)
	{
		nowNpcID = _npcId;
		nowUserClass = _eClass;
		nowClass = _eClass;

		RepositionKindBtn( nowNpcID, nowUserClass);

		UIRadioBtn nowClassBtn = null;

		foreach ( UIRadioBtn radio in classBtns)
		{
			if( _eClass == ( eCLASS)radio.data)
			{
				nowClassBtn = radio;
				textClass.Text = radio.Text;
			}
		}

		nowClassBtn.Value = true;
	
		SetStoreItem( nowNpcID, nowUserClass);
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
			UIRadioBtn radio = ( UIRadioBtn)ptr.targetObj;
	
			nowItemKind = ( eStore_ItemKind)radio.Data;
	
			PlayerPrefs.SetInt( "NpcStoreKind", ( int)nowItemKind);
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
			AsSocialManager.Instance.SocialUI.CloseSocialStoreDlg();//#19732
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
