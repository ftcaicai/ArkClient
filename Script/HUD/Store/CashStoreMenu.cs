using UnityEngine;
using System.Collections.Generic;


//public class RecommendCashItemCompare : IComparer<Store_Item_Info_Table>
//{
//    public int Compare( Store_Item_Info_Table _first, Store_Item_Info_Table _second)
//    {
//        if( _first.RecommendIdx > _second.RecommendIdx)
//            return 1;
//        else if( _first.RecommendIdx < _second.RecommendIdx)
//            return -1;
//        return 0;
//    }
//}


public class CashStoreMenu : MonoBehaviour
{
	public SpriteText menuButtonTitle;
	public int menuNameId;
	public bool isMainMenu = true;
	public eCashStoreMenuMode menuMode;
	public eCashStoreMainCategory mainItemKind;
	public UIScrollList mainScrollList;
	public UIScrollList listCategory;
	public UIListItemContainer mainListItem;
	public delegate void addListDelegate( List<Store_Item_Info_Table> _storeItemList);
	public addListDelegate addItemToList;
	public SimpleSprite downArrowSprite;
	public SimpleSprite upArrowSprite;

	protected eCLASS userClass;

	// set title Text
	public virtual void InitText()
	{
		if( menuButtonTitle != null)
		{
			AsLanguageManager.Instance.SetFontFromSystemLanguage( menuButtonTitle);
			menuButtonTitle.Text = AsTableManager.Instance.GetTbl_String( menuNameId);
		}
	}

	public virtual void InitMenu(eCLASS _userClass)
	{
		userClass = _userClass;

		/*if (mainItemKind == eCashStoreMainCategory.RECOMMEND)
		{
			List<Store_Item_Info_Table> listItem = AsTableManager.Instance.GetCashStoreItems( _userClass, mainItemKind);
			listItem.Sort( new RecommendCashItemCompare());
			addItemToList( listItem);
		}
		 * */

		if (menuMode == eCashStoreMenuMode.EVENT)
			addItemToList(AsTableManager.Instance.GetCashStoreItems(userClass, eCashStoreMainCategory.EVENT, eCashStoreSubCategory.NONE));
		else if (mainItemKind == eCashStoreMainCategory.MIRACLE)
		{
			AsBasePurchase purchase = AsCashStore.Instance.GetPurchaseManager();
			if( purchase != null)
			{
				addItemToList( purchase.GetStoreItemList());
			}
			else
			{
#if UNITY_EDITOR
				// test code
				AsCashStore.Instance.AddItemToListForEditor();
#endif
			}
		}
	}

	public virtual void Reset()
	{
		if (mainScrollList != null)
		{
			if (mainScrollList.gameObject.activeSelf)
				mainScrollList.ClearListSync(true);
		}
	}

	public virtual void SetCategory( eCashStoreSubCategory _eSubCategory)
	{
	}
	
	public void ActivateItem( int _idx)
	{
		if( mainScrollList == null)
			return;
		
		if( mainScrollList.Count - 1 < _idx)
			return;
		
		mainScrollList.ScrollToItem( _idx, 0.0f);
	}

	public int GetGachaLevel(int _usrLevel)
	{
        if (_usrLevel >= 1 && _usrLevel <= 9)
            return 1;
        else if (_usrLevel >= 10 && _usrLevel <= 19)
            return 10;
        else if (_usrLevel >= 20 && _usrLevel <= 29)
            return 20;
        else if (_usrLevel >= 30 && _usrLevel <= 34)
            return 30;
        else if (_usrLevel >= 35 && _usrLevel <= 39)
            return 35;
        else if (_usrLevel >= 40 && _usrLevel <= 44)
            return 40;
        else if (_usrLevel >= 45 && _usrLevel <= 49)
            return 45;
        else if (_usrLevel >= 50 && _usrLevel <= 54)
            return 50;
        else if (_usrLevel >= 55 && _usrLevel <= 59)
            return 55;
        else if (_usrLevel >= 60 && _usrLevel <= 64)
            return 60;
        else
            return 0;
	}

	void Update()
	{
		if (upArrowSprite != null && downArrowSprite != null && mainScrollList != null)
			CheckMorelistMark(mainScrollList);
	}

	public virtual void ClickMainListItemProcess(StoreItemInfoUI _storeItemInfoUI)
	{
		if (_storeItemInfoUI.item_Type == Store_Item_Type.NormalItem || _storeItemInfoUI.item_Type == Store_Item_Type.ChargeItem)
			return;

		Debug.LogWarning(_storeItemInfoUI.item_Type.ToString() + "Item = "+ _storeItemInfoUI.itemID + "("+_storeItemInfoUI.itemBunch+")");

		Item item = ItemMgr.ItemManagement.GetItem(int.Parse(_storeItemInfoUI.itemID));

		if (item != null)
			_storeItemInfoUI.itemName = AsTableManager.Instance.GetTbl_String(item.ItemData.nameId);

		AsCashStore.Instance.PrepareBuyItem(_storeItemInfoUI);
	}

	public virtual void CheckMorelistMark(UIScrollList _scrollList)
	{
		if (_scrollList == null || upArrowSprite == null || downArrowSprite == null)
			return;

		if (_scrollList.Count > 0 && _scrollList.gameObject.activeSelf)
		{
			if (!_scrollList.IsShowingLastItem())
			{
				if (downArrowSprite.IsHidden() == true)
					downArrowSprite.Hide(false);
			}
			else
			{
				if (downArrowSprite.IsHidden() == false)
					downArrowSprite.Hide(true);
			}

			if (!_scrollList.IsShowingFirstItem())
			{
				if (upArrowSprite.IsHidden() == true)
					upArrowSprite.Hide(false);
			}
			else
			{
				if (upArrowSprite.IsHidden() == false)
					upArrowSprite.Hide(true);
			}
		}
		else
		{
			if (downArrowSprite.IsHidden() == false)
				downArrowSprite.Hide(true);

			if (upArrowSprite.IsHidden() == false)
				upArrowSprite.Hide(true);
		}
	}

	public virtual int UpdateFreeGachaPoint() { return 0; }
}
