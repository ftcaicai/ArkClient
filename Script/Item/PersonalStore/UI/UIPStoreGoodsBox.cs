using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

public class UIPStoreGoodsBox : MonoBehaviour
{
	public SpriteText textTitle;
	public SpriteText textItemName;
	public SpriteText textCount;
	public UITextField textFieldGold;
	public SpriteText textGold;
	public UIButton buttonPlus;
	public UIButton buttonMinus;
	public UIButton buttonPlus_1;
	public UIButton buttonMinus_1;

	public UIButton buttonOK;
	public SpriteText textOK;
	public UIButton buttonCancel;
	public Transform iconSlot;

	public SpriteText textSellAmountLimit;
	public SpriteText textSellAmountLimit_Gold;

	UISlotItem m_SlotItem;

	public int CurItemCount
	{
		get { return itemCount; }
	}

	private int itemCount = 1;
	private int itemMax = 1;

	private ulong m_ulCurSellAmountLimit = 0;

	int pstoreSlot = -1;
	UIInvenSlot invenSlot = null;
	int invenSlotIdx = -1;

	AsMessageBox m_MsgBox = null;

	OkBtnClickDelegate m_delOk;
	CancelBtnClickDelegate m_delCancel;

	string m_DefaultPrice;
//	bool m_Activated = false;

	public Color colorStrength = Color.white;
	#region delegate
	public delegate void OkBtnClickDelegate( int _idx, int _quantity, UInt64 _gold, int _slotIndex);
	public delegate void CancelBtnClickDelegate();
	#endregion

	void Start()
	{
		AsLanguageManager.Instance.SetFontFromSystemLanguage( textTitle);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( textItemName);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( textCount);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( textFieldGold.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( textGold);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( buttonPlus.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( buttonMinus.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( buttonOK.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( textOK);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( buttonCancel.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( textSellAmountLimit);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( textSellAmountLimit_Gold);

		textTitle.Text = AsTableManager.Instance.GetTbl_String(1155);
		buttonOK.Text = AsTableManager.Instance.GetTbl_String(1152);
		buttonCancel.Text = AsTableManager.Instance.GetTbl_String(1151);
		textSellAmountLimit.Text = AsTableManager.Instance.GetTbl_String(2005);

		buttonPlus.AddInputDelegate( PlusCountDelegate);
		buttonMinus.AddInputDelegate( MinusCountDelegate);

		if( null != buttonPlus_1)
			buttonPlus_1.AddInputDelegate( PlusCountDelegate_1);

		if( null != buttonMinus_1)
			buttonMinus_1.AddInputDelegate( MinusCountDelegate_1);

		buttonOK.AddInputDelegate( OkDelegate);
		buttonCancel.AddInputDelegate( CancelDelegate);

		textFieldGold.SetValidationDelegate( KeyInputDelegate);
		textFieldGold.SetFocusDelegate( FocusDelegate);
	}

	public void Open( UIInvenSlot _invenSlot, int _pstoreSlot, OkBtnClickDelegate _delOk, CancelBtnClickDelegate _delCancel)
	{
		gameObject.SetActiveRecursively( true);

		invenSlot = _invenSlot;
		invenSlotIdx = _invenSlot.slotIndex;
		pstoreSlot = _pstoreSlot;

		itemCount = _invenSlot.slotItem.realItem.sItem.nOverlapped;
		itemMax = _invenSlot.slotItem.realItem.sItem.nOverlapped;
		string str = _invenSlot.slotItem.realItem.item.ItemData.GetGradeColor() + AsTableManager.Instance.GetTbl_String(_invenSlot.slotItem.realItem.item.ItemData.nameId);
		if( _invenSlot.slotItem.realItem.sItem.nStrengthenCount > 0)
			str = colorStrength + "+" + _invenSlot.slotItem.realItem.sItem.nStrengthenCount + " " + str;
		textItemName.Text = str;
		textFieldGold.Text = "";
		textGold.Text = "";
		textCount.Text = _invenSlot.slotItem.realItem.sItem.nOverlapped.ToString();

		DeactivateOkBtn();

		GameObject resGo = _invenSlot.slotItem.realItem.item.GetIcon();
		if( null == resGo)
			Debug.LogError( "UIPStoreGoodsBox::Open() [ null == resGo ] item id : " + _invenSlot.slotItem.realItem.item.ItemID);

		GameObject go = GameObject.Instantiate( resGo) as GameObject;
		go.transform.localPosition = Vector3.zero;
		go.transform.localRotation = Quaternion.identity;
		go.transform.localScale = Vector3.one;

		m_SlotItem = go.GetComponent<UISlotItem>();
		if( null == m_SlotItem)
		{
			Debug.LogError( "UIPStoreGoodsBox::Open() [ null == slotItem");
			Destroy( go);
		}

		m_SlotItem.SetItem( _invenSlot.slotItem.realItem);

		m_SlotItem.iconImg.SetSize( iconSlot.collider.bounds.size.x, iconSlot.collider.bounds.size.y);
		Vector3 pos = iconSlot.position; pos.z -= 0.5f;
		m_SlotItem.transform.position = pos;

		m_delOk = _delOk;
		m_delCancel = _delCancel;

		//fixed
		m_DefaultPrice = AsTableManager.Instance.GetTbl_String(1705);
		textFieldGold.maxLength = 0;
		textFieldGold.Text = m_DefaultPrice;

		// sell amount limit
		m_ulCurSellAmountLimit = GetMaxPStoreSellAmount( _invenSlot.slotItem.realItem);
		textSellAmountLimit_Gold.Text = m_ulCurSellAmountLimit.ToString( "#,#0", CultureInfo.InvariantCulture);
	}

	public void Close()
	{
		gameObject.SetActiveRecursively( false);

		if( m_SlotItem != null)
			Destroy( m_SlotItem.gameObject);

		if( invenSlot != null && AsHudDlgMgr.Instance.invenDlg != null)
			AsHudDlgMgr.Instance.invenDlg.SetRestoreSlot();
	}

	public void SetCount( int iData)
	{
		itemCount = iData;

		if( 1 > itemCount)
			itemCount = 1;

		if( itemMax < itemCount)
			itemCount = itemMax;

		textCount.Text = itemCount.ToString();
	}

	void PlusCountDelegate( ref POINTER_INFO ptr)
	{
		if( CheckMsgBoxPopUp() == true)
			return;

		if ( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			SetCount( itemCount + 1);
		}
	}

	void MinusCountDelegate( ref POINTER_INFO ptr)
	{
		if( CheckMsgBoxPopUp() == true)
			return;

		if ( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			SetCount( itemCount - 1);
		}
	}

	void PlusCountDelegate_1( ref POINTER_INFO ptr)
	{
		if( CheckMsgBoxPopUp() == true)
			return;

		if ( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			SetCount( itemCount + 10);
		}
	}

	void MinusCountDelegate_1( ref POINTER_INFO ptr)
	{
		if( CheckMsgBoxPopUp() == true)
			return;

		if ( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			SetCount( itemCount - 10);
		}
	}

	void OkDelegate( ref POINTER_INFO ptr)
	{
		if( CheckMsgBoxPopUp() == true)
			return;

		if ( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP && m_OkBtnActivate == true)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			if ( m_delOk != null)
			{
				UInt64 calculated = 0;
				if( UInt64.TryParse( textGold.text, out calculated) == true)
				{
					UInt64 gold = calculated * (ulong)itemCount;

					foreach( KeyValuePair<int, body2_SC_PRIVATESHOP_OWNER_ITEMLIST> pair in AsPStoreManager.Instance.dicPlayerShopItem)
					{
						gold += pair.Value.nItemGold * (ulong)pair.Value.nMaxOverlapped;
					}

					if( calculated > m_ulCurSellAmountLimit)
					{
						string strTitle = AsTableManager.Instance.GetTbl_String(126);
						string strMsg = AsTableManager.Instance.GetTbl_String(2006);
						m_MsgBox = AsNotify.Instance.MessageBox( strTitle, strMsg, this, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_ERROR);
						textGold.Text = AsTableManager.Instance.GetTbl_String(1705);
						return;
					}
					else
					{
						if( gold + AsUserInfo.Instance.SavedCharStat.nGold < AsGameDefine.MAX_GOLD + 1)
							m_delOk( pstoreSlot, itemCount, calculated, invenSlotIdx);
						else
						{
							Debug.Log( "UIPstoreGoodsBox::OkDelegate: too high price is set( " + calculated + " + " + AsUserInfo.Instance.SavedCharStat.nGold + ")");
							string result = AsTableManager.Instance.GetTbl_String(261);
							m_MsgBox = AsNotify.Instance.MessageBox( "", result, this, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_ERROR);
						}
					}
				}
			}

			textGold.Text = "";
			Close();
		}
	}

	void CancelDelegate( ref POINTER_INFO ptr)
	{
		if( CheckMsgBoxPopUp() == true)
			return;

		if ( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			m_delCancel();

			gameObject.SetActiveRecursively( false);
			Close();
		}
	}

	string KeyInputDelegate( UITextField _field, string _text, ref int _insertionPoint)
	{
		Debug.Log( "UIPStoreGoodsBox::KeyInputDelegate: price is input.");

		for( int k = 0; k < _text.Length; ++k)
		{
			char c =_text[k];
			if( c < '0' || c > '9')
				return "";
		}

		string inputUlong = "";
		UInt64 num = 0;
		for( int i = 0; i < _text.Length; ++i)
		{
			if( UInt64.TryParse( _text[i].ToString(), out num) == true)
			{
				inputUlong += num.ToString();
			}
		}

		if( inputUlong.Length == 0)
			DeactivateOkBtn();
		else
			ActivateOkBtn();

		return inputUlong;
	}

	void FocusDelegate( UITextField _field)
	{
		textFieldGold.maxLength = 10;
		textFieldGold.Text = "";
	}

	bool m_OkBtnActivate = true;
	void ActivateOkBtn()
	{
		if( m_OkBtnActivate == false)
		{
			m_OkBtnActivate = true;
			textOK.Color = Color.black;
			Debug.Log( "UIPStoreGoodsBox::ActivateOkBtn: m_OkBtnActivate = " + m_OkBtnActivate + ", textOK.Color = " + textOK.Color);
		}
	}

	void DeactivateOkBtn()
	{
		if( m_OkBtnActivate == true)
		{
			m_OkBtnActivate = false;
			textOK.Color = Color.gray;
			Debug.Log( "UIPStoreGoodsBox::DeactivateOkBtn: m_OkBtnActivate = " + m_OkBtnActivate + ", textOK.Color = " + textOK.Color);
		}
	}

	bool CheckMsgBoxPopUp()
	{
		if( m_MsgBox != null)
			return true;
		else
			return false;
	}

	void ClearMsgBox()
	{
		if( m_MsgBox != null)
		{
			Destroy( m_MsgBox.gameObject);
		}
		m_MsgBox = null;
	}

	ulong GetMaxPStoreSellAmount( RealItem realItem)
	{
		if( null == realItem || null == realItem.item || null == realItem.sItem)
		{
			Debug.LogError( "UIPStoreGoodsBox::GetMaxPStoreSellAmount(): null == realItem || null == realItem.item || null == realItem.sItem");
			return 0;
		}

		// buy amount
		float fBuyAmount = ( float)( realItem.item.ItemData.buyAmount);

		// goods type
		float fGoodsTypeValue = 1.0f;
		if( Item.eGOODS_TYPE.Cash == realItem.item.ItemData.getGoodsType)
			fGoodsTypeValue = GetPStoreValue_GlobalWeightTable( 90, false);
		else if( Item.eGOODS_TYPE.Gold == realItem.item.ItemData.getGoodsType)
			fGoodsTypeValue = GetPStoreValue_GlobalWeightTable( 89, false);
		else if( Item.eGOODS_TYPE.Point == realItem.item.ItemData.getGoodsType)
			fGoodsTypeValue = GetPStoreValue_GlobalWeightTable( 91, false);

		// grade
		float fGradeValue = 1.0f;
		if( Item.eGRADE.Normal == realItem.item.ItemData.grade)
			fGradeValue = GetPStoreValue_GlobalWeightTable( 93, true);
		else if( Item.eGRADE.Magic == realItem.item.ItemData.grade)
			fGradeValue = GetPStoreValue_GlobalWeightTable( 94, true);
		else if( Item.eGRADE.Rare == realItem.item.ItemData.grade)
			fGradeValue = GetPStoreValue_GlobalWeightTable( 95, true);
		else if( Item.eGRADE.Epic == realItem.item.ItemData.grade)
			fGradeValue = GetPStoreValue_GlobalWeightTable( 96, true);
		else if( Item.eGRADE.Ark == realItem.item.ItemData.grade)
			fGradeValue = GetPStoreValue_GlobalWeightTable( 97, true);

		// strengthen increase ratio
		float fStrengthenRatio = 1.0f;
		if( realItem.sItem.nStrengthenCount > 0)
		{
			Tbl_Strengthen_Record strengthenRecord = AsTableManager.Instance.GetStrengthenTable().GetStrengthenRecord( realItem.item.ItemData.GetItemType(),
				(Item.eEQUIP)realItem.item.ItemData.GetSubType(), realItem.item.ItemData.grade, realItem.item.ItemData.levelLimit, realItem.sItem.nStrengthenCount);

			if( null != strengthenRecord)
			{
				fStrengthenRatio = ( strengthenRecord.getStrengthenRatiog * 0.001f) + 1.0f;
				if( fStrengthenRatio <= 0.0f)
				{
					Debug.Log( "UIPStoreGoodsBox::GetMaxPStoreSellAmount(): fStrengthenRatio <= 0.0f");
					fStrengthenRatio = 1.0f;
				}
			}
			else
			{
				Debug.Log( "UIPStoreGoodsBox::GetMaxPStoreSellAmount(): null == strengthenRecord");
			}
		}

		ulong nRes = (ulong)( fBuyAmount * fGoodsTypeValue * fGradeValue * fStrengthenRatio);

		if( nRes < (ulong)fBuyAmount)
			nRes = (ulong)fBuyAmount;

		if( nRes > AsGameDefine.MAX_GOLD)
			nRes = AsGameDefine.MAX_GOLD;

		return nRes;
	}

	float GetPStoreValue_GlobalWeightTable( int nID, bool isPermille)
	{
		Tbl_GlobalWeight_Record record = AsTableManager.Instance.GetTbl_GlobalWeight_Record( nID);

		if( null != record)
		{
			if( true == isPermille)
				return ( record.Value * 0.001f);
			else
				return record.Value;
		}

		return 1.0f;
	}
}
