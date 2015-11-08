using UnityEngine;
using System.Collections;
using System.Globalization;

public class StoreQuantityInfo
{
	public string itemName = string.Empty;
	public string itemID = string.Empty;
	public int itemMax = 1;
	public int itemCount = 1;
	public int itemSlot = 0;
	public string itemPrice = string.Empty;

	public StoreQuantityInfo() { Reset(); }

	public StoreQuantityInfo( string _itemName, string _itemID, string _itemPrice, int _itemSlot, int _itemMax)
	{
		itemName = _itemName;
		itemID = _itemID;
		itemPrice = _itemPrice;
		itemMax = _itemMax;
		itemSlot = _itemSlot;
	}

	public void Reset()
	{
		itemName = string.Empty;
		itemID = string.Empty;
		itemPrice = string.Empty;
		itemMax = 1;
		itemCount = 1;
		itemSlot = 0;
	}

	public override string ToString()
	{
		return "[" + itemID + "]" + itemName + " = " + itemCount;
	}
}


public class StoreQuantityMessageBox : MonoBehaviour
{
	public GameObject objectIconParent = null;
	public SimpleSprite goldSprite;
	public SpriteText textTitle;
	public SpriteText textItemName;
	public SpriteText textCount;
	public SpriteText textGold;
	public SpriteText textCancel;
	public SpriteText textConfirm;
	public UIButton buttonPlus;
	public UIButton buttonMinus;
	public UIButton buttonPlus_1;
	public UIButton buttonMinus_1;
	public UIButton buttonOK;
	public UIButton buttonCancel;
	public GameObject objTradeIcon = null;

	public StoreQuantityInfo QuantityInfo
	{
		get { return quantityInfo; }
		set { quantityInfo = value; }
	}

	public int NowItemCount
	{
		get { return itemCount; }
	}

	private GameObject objIconImg = null;
	private int itemCount = 1;
	private int itemMax = 1;
	private int itemBunch = 1;
	private int tradeItemID = -1;
	private int tradeItemCount = 0;
	private StoreQuantityInfo quantityInfo = new StoreQuantityInfo();
	private OkDelegateFromOut OkProcessFromOut;
	private CancelDelegateFromOut CancelProcessFromOut;
	
	#region delegate
	public delegate void OkDelegateFromOut();
	public delegate void CancelDelegateFromOut();
	#endregion

	void Start()
	{
		buttonPlus.AddInputDelegate( PlusCountDelegate);
		buttonMinus.AddInputDelegate( MinusCountDelegate);
		
		if( null != buttonPlus_1)
			buttonPlus_1.AddInputDelegate( PlusCountDelegate_1);
		
		if( null != buttonMinus_1)
			buttonMinus_1.AddInputDelegate( MinusCountDelegate_1);
		
		buttonOK.AddInputDelegate( OkDelegate);
		buttonCancel.AddInputDelegate( CancelDelegate);

		AsLanguageManager.Instance.SetFontFromSystemLanguage( textTitle);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( textItemName);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( textCount);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( textGold);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( textCancel);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( textConfirm);
		
		textTitle.Text = AsTableManager.Instance.GetTbl_String(126);
		textCancel.Text = AsTableManager.Instance.GetTbl_String(1151);
		textConfirm.Text = AsTableManager.Instance.GetTbl_String(1152);
	}

	public void Open( StoreItemInfoUI _info, int _itemMax, OkDelegateFromOut _okProcess, CancelDelegateFromOut _cancelProcess, GameObject _IconImg, Item.eGOODS_TYPE _goodsType)
	{
		quantityInfo = new StoreQuantityInfo( _info.itemName, _info.itemID, _info.itemPrice, _info.itemSlot, _itemMax);
		gameObject.SetActiveRecursively( true);
		
		itemCount = 1;
		itemMax = _itemMax;
		itemBunch = _info.itemBunch;
		tradeItemID = _info.tradeItemID;
		tradeItemCount = _info.tradeItemCount;
		textItemName.Text = _info.itemName;

		if (_info.tradeItemID == -1)
			textGold.Text = ( System.Convert.ToInt32( _info.itemPrice) * _info.itemBunch).ToString( "#,#0", CultureInfo.InvariantCulture);
		else
			textGold.Text = (_info.tradeItemCount).ToString("#,#0", CultureInfo.InvariantCulture);

		textCount.Text = "1";
		
		OkProcessFromOut = _okProcess;
		CancelProcessFromOut = _cancelProcess;

		// add Icon
		if( objIconImg != null)
			GameObject.DestroyImmediate( objIconImg);

		// check goods
		if (_info.tradeItemID != -1)
		{
			goldSprite.Hide(true);

			if (objTradeIcon != null)
			{
				GameObject.Destroy(objTradeIcon);
				objTradeIcon = null;
			}

			GameObject objGoods = AsNpcStore.GetItemIcon(_info.tradeItemID.ToString(), 1);
			objGoods.transform.parent = goldSprite.transform.parent;
			objGoods.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
			objGoods.transform.localPosition = goldSprite.transform.localPosition;

			objTradeIcon = objGoods;
		}
		else
		{
			if (objTradeIcon != null)
			{
				GameObject.Destroy(objTradeIcon);
				objTradeIcon = null;
			}
		}

		if( goldSprite != null)
		{
			if( _goodsType == Item.eGOODS_TYPE.Gold)
				goldSprite.Hide( false);
			else
				goldSprite.Hide( true);
		}

		objIconImg = _IconImg;

		_IconImg.transform.parent = objectIconParent.transform;

		_IconImg.transform.localPosition = Vector3.zero;
	}

	public void Close()
	{
		gameObject.SetActiveRecursively( false);
	}
	
	public void SetCount( int iData)
	{
		itemCount = iData;
		if( 1 > itemCount)
			itemCount = 1;
		if( itemMax < itemCount)
			itemCount = itemMax;
		
		textCount.Text = itemCount.ToString();

		if (tradeItemID == -1)
			textGold.Text = ( itemCount * System.Convert.ToInt32( quantityInfo.itemPrice) * itemBunch).ToString( "#,#0", CultureInfo.InvariantCulture);
		else
			textGold.Text = ( itemCount * tradeItemCount).ToString("#,#0", CultureInfo.InvariantCulture);
	}

	void PlusCountDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			SetCount( itemCount + 1);
		}
	}

	void MinusCountDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			SetCount( itemCount - 1);
		}
	}
	
	void PlusCountDelegate_1( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			SetCount( itemCount + 10);
		}
	}

	void MinusCountDelegate_1( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			
			
			SetCount( itemCount - 10);
		}
	}

	void OkDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			if( OkProcessFromOut != null)
			{
				quantityInfo.itemCount = itemCount;
				OkProcessFromOut();
			}

			gameObject.SetActiveRecursively( false);
		}
	}

	void CancelDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			if( CancelProcessFromOut != null)
				CancelProcessFromOut();

			gameObject.SetActiveRecursively( false);
		}
	}
}
