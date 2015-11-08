using UnityEngine;
using System.Collections;
using System.Globalization;


[System.Serializable]
public class StoreItemControllerInfo
{
	public GameObject slotObject;
	public UIListItemContainer uiListItemContainer;
	public StoreTouchType touchType;
	
	[HideInInspector]
	public StoreItemInfoUI itemUIInfoData;

	public SimpleSprite itemSlotSprite;
	public GameObject itemFocusObject;
	public GameObject itemIconObject;
	public BoxCollider boxCollider;
	public SimpleSprite imgMiracle;
	public SpriteText itemName;
	public SpriteText itemPrice;
	public SpriteText itemResetTime;
	public SpriteText itemDescription;
	public SimpleSprite[] highlightSprites = null;
	public SimpleSprite buyLimitSprite;
	public SimpleSprite goodSprite;
	public SimpleSprite shade;
	public SpriteText buyLimitTxt;
	public SpriteText itemCountTxt;
	public eSHOPITEMHIGHLIGHT highlight = eSHOPITEMHIGHLIGHT.eSHOPITEMHIGHLIGHT_NONE;

	[HideInInspector]
	public int buyLimit;
	public ulong bonus;

	public void OnHighlight()
	{
		if( highlight == eSHOPITEMHIGHLIGHT.eSHOPITEMHIGHLIGHT_BONUS)
			OnHighlightBonus( bonus);
		else
			OnHighlight( highlight);
	}

	public void OnHighlight( eSHOPITEMHIGHLIGHT _highlight)
	{
		highlight = _highlight;
		
		if (highlightSprites == null)
			return;

		if (_highlight == eSHOPITEMHIGHLIGHT.eSHOPITEMHIGHLIGHT_NONE)
		{
			if (highlightSprites != null)
			{
				foreach (SimpleSprite sprite in highlightSprites)
					sprite.gameObject.SetActive(false);
			}

			return;
		}

		// disable all
		foreach (SimpleSprite sprite in highlightSprites)
			sprite.gameObject.SetActive(false);

		if( _highlight != eSHOPITEMHIGHLIGHT.eSHOPITEMHIGHLIGHT_NONE && _highlight != eSHOPITEMHIGHLIGHT.eSHOPITEMHIGHLIGHT_NOTHING)
		{
			int idx = (int)_highlight - 2;

			if( highlightSprites[idx] != null)
				highlightSprites[idx].gameObject.SetActive( true);
		}
	}

	public void OnHighlightBonus( ulong _bonus)
	{
		OnHighlight( eSHOPITEMHIGHLIGHT.eSHOPITEMHIGHLIGHT_BONUS);

		bonus = _bonus;

		int idx = (int)highlight - 2;

		SpriteText text = highlightSprites[idx].transform.GetChild(0).GetComponent<SpriteText>();

		text.Text = _bonus.ToString( "#,#0", CultureInfo.InvariantCulture);
	}

	public void OnBuyLimitMark( int _buyLimit)
	{
		buyLimit = _buyLimit;

		if( buyLimitSprite != null)
		{
			buyLimitSprite.gameObject.SetActive( buyLimit > 0);
			buyLimitTxt.Text = string.Format( AsTableManager.Instance.GetTbl_String(1644), _buyLimit);
		}
	}

	public void OnBuyLimitMark()
	{
		OnBuyLimitMark( buyLimit);
	}
}


[System.Serializable]
public class StoreItemInfoUI
{
	public bool isInit = false;
	public string itemID;
	public string itemPrice;
	public int itemIconID;
	public int itemBunch;
	public int itemSlot;
	public string itemName;
	public int tradeItemID;
	public int tradeItemCount;
	public int itemBuyMax;
	public Store_Item_Type item_Type;

	public StoreItemInfoUI( Store_Item_Type _store_Item_Type, string _itemID, string _itemPrice, int _itemBunch, int _itemSlot, string _itemName, int _tradeItemID = -1, int _tradeItemCount = 0, int _itemBuyMax = -1)
	{
		itemID = _itemID;
		itemPrice = _itemPrice;
		itemName = _itemName;
		itemBunch = _itemBunch;
		itemSlot = _itemSlot;
		item_Type = _store_Item_Type;
		tradeItemID = _tradeItemID;
		tradeItemCount = _tradeItemCount;
		itemBuyMax = _itemBuyMax;
		isInit = true;
	}
}


public class StoreItemController : MonoBehaviour
{
	public StoreItemControllerInfo[] items;

	[HideInInspector]
	public bool initilized = false;

	void Start ()
	{
	}

	void OnEnable()
	{
		if( initilized)
			UpdateEnvisibleSlot();

		if( initilized)
		{
			foreach( StoreItemControllerInfo item in items)
			{
				item.OnHighlight();
				item.OnBuyLimitMark();
			}
		}
	}

	public StoreItemInfoUI GetStoreItemInfo( StoreTouchType _touchType = StoreTouchType.STORE_TOUCH_LEFT)
	{
		if( items[(int)_touchType] == null)
			return null;
		else
			return items[(int)_touchType].itemUIInfoData;
	}

	public void SetFocus( GameObject _focusObject, StoreTouchType _touchType = StoreTouchType.STORE_TOUCH_LEFT)
	{
		if( _focusObject != null)
		{
			_focusObject.SetActive( true);

			SimpleSprite sprite = _focusObject.GetComponent<SimpleSprite>();

			if( sprite != null)
				sprite.Unclip();

			if( items != null)
			{
				if( items[(int)_touchType].itemSlotSprite != null)
				{
					items[(int)_touchType].itemFocusObject = _focusObject;
					_focusObject.transform.parent = items[(int)_touchType].itemSlotSprite.transform;
					_focusObject.transform.localPosition = new Vector3( 0.0f, 0.0f, -2.0f);
	
					if( items[(int)_touchType].uiListItemContainer != null)
						items[(int)_touchType].uiListItemContainer.ScanChildren();
				}
			}
		}
	}

	public void RemoveFocus( Transform _parent, StoreTouchType _touchType = StoreTouchType.STORE_TOUCH_LEFT)
	{
		if( items[(int)_touchType] != null)
		{
			if( items[(int)_touchType].itemFocusObject != null)
			{
				items[(int)_touchType].itemFocusObject.transform.parent = _parent;
				items[(int)_touchType].itemFocusObject.SetActive( false);
				items[(int)_touchType].itemFocusObject = null;

				if( items[(int)_touchType].uiListItemContainer != null)
					items[(int)_touchType].uiListItemContainer.ScanChildren();
			}
		}
	}

	public void UpdateEnvisibleSlot()
	{
		foreach( StoreItemControllerInfo info in items)
		{
			if( info.itemUIInfoData.isInit == false)
				info.slotObject.SetActive( false);
		}
	}
}
