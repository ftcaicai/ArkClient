using UnityEngine;
using System.Collections;

public class UIPStoreQuantityDlg : MonoBehaviour 
{
	public delegate void delVoid(UIPStoreSlot _slot, int _count);
	
	#region - member -
	public UIButton cancelButton;
	public UIButton okButton;
	public UIButton minusButton;
	public UIButton plusButton;
	public UIButton minusButton_1;
	public UIButton plusButton_1;	
	public SpriteText titleText;
	public SpriteText itemText;
	public SpriteText inputText;
	public GameObject iconParent;
	
//	private int m_CurSlotIndex = -1;
	private int m_nCurCount = 0;
	private int m_nMaxCount = 0;
	
	private GameObject m_goIconImg = null;
	private RealItem m_RealItem;
	private int m_RootSlotIdx;
	private int m_TargetSlotIdx;
	
	AbCalledProc m_CallProc = null;
	#endregion
	#region - init & close -
	void Start()
	{
		AsLanguageManager.Instance.SetFontFromSystemLanguage( cancelButton.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( okButton.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( minusButton.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( plusButton.spriteText);
		
		AsLanguageManager.Instance.SetFontFromSystemLanguage( titleText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( itemText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( inputText);
		
		cancelButton.Text = AsTableManager.Instance.GetTbl_String(1151);
		okButton.Text = AsTableManager.Instance.GetTbl_String(1152);
		
		_SetTitleText( AsTableManager.Instance.GetTbl_String(1111));

		okButton.SetInputDelegate( _OkBtnDelegate);
		cancelButton.SetInputDelegate( _CancelBtnDelegate);
		plusButton.SetInputDelegate( _PlusBtnDelegate);
		minusButton.SetInputDelegate( _MinusDataBtnDelegate);	
		
		plusButton_1.SetInputDelegate( _PlusBtnDelegate_1);
		minusButton_1.SetInputDelegate( _MinusDataBtnDelegate_1);	
	}

	void Update()
	{
	}

	public void Open( UIPStoreSlot _slot, delVoid _del)
	{
		m_CallProc = new CalledProc_PStore( _slot, _del);
		if( m_CallProc.CheckValid() == false)
			return;
		
//		m_CurSlotIndex = _slot.slotIndex;
				
		m_nCurCount = m_nMaxCount = _slot.slotItem.realItem.sItem.nOverlapped;
		_SetItemText( _slot.slotItem.realItem.item.ItemData);
		_SetCount( m_nCurCount);
		_SetIcon( _slot.slotItem.realItem.item);

		gameObject.SetActiveRecursively( true);
	}
	
	public void Open( UIPStoreSearchSlot _slot, delPurchaseConfirm _del)
	{
		m_CallProc = new CalledProc_Search( _slot, _del);
		if( m_CallProc.CheckValid() == false)
			return;
				
		m_nCurCount = m_nMaxCount = _slot.SearchInfo.sItem.nOverlapped;
		
		Item item = ItemMgr.ItemManagement.GetItem( _slot.SearchInfo.sItem.nItemTableIdx);
		_SetItemText( item.ItemData);
		_SetCount( 1);
		_SetIcon( item);

		gameObject.SetActiveRecursively( true);
	}
	
	public void Close()
	{
		gameObject.SetActiveRecursively( false);
//		Destroy(gameObject);
	}
	#endregion
	#region - delegate -
	// < private
	private void _OkBtnDelegate(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			m_CallProc.PurchaseConfirm( m_nCurCount);
			
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			Close();
		}
	}
	
	private void _CancelBtnDelegate(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			Close();
		}
	}
	
	private void _PlusBtnDelegate(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			_SetCount( m_nCurCount + 1);
		}
	}
	
	private void _MinusDataBtnDelegate(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			_SetCount( m_nCurCount - 1);
		}
	}
	
	
	private void _PlusBtnDelegate_1(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			_SetCount( m_nCurCount + 10);
		}
	}
	
	private void _MinusDataBtnDelegate_1(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			_SetCount( m_nCurCount - 10);
		}
	}
	#endregion
	#region - control -
	private void _SetTitleText(string str)
	{
		if( null == titleText)
			return;

		titleText.Text = str;
	}
	
	private void _SetItemText(string str)
	{
		if( null == itemText)
			return;

		itemText.Text = str;
	}
	
	private void _SetItemText(ItemData itemData)
	{
		Tbl_String_Record strRecord = AsTableManager.Instance.GetTbl_String_Record( itemData.nameId);
		
		string strBuf = "";
		if( null != strRecord)
			strBuf = itemData.GetGradeColor() + strRecord.String;
		
		_SetItemText( strBuf);
	}
	
	private void _SetInputText(string str)
	{
		if( null == inputText)
			return;

		inputText.Text = str;
	}

	private void _SetCount(int nCount)
	{
		m_nCurCount = nCount;

		if( nCount < 1)
			m_nCurCount = 1;
		if( nCount > m_nMaxCount)
			m_nCurCount = m_nMaxCount;

		_SetInputText( m_nCurCount.ToString());
	}
	
	private void _SetIcon(Item item)
	{
		Destroy( m_goIconImg);
		
		m_goIconImg = GameObject.Instantiate( item.GetIcon()) as GameObject;
		m_goIconImg.transform.parent = iconParent.transform;
		m_goIconImg.transform.localPosition = Vector3.zero;
		m_goIconImg.transform.localRotation = Quaternion.identity;
		m_goIconImg.transform.localScale = Vector3.one;
	}
	#endregion
	
	#region - inner class -
	public abstract class AbCalledProc
	{
		public abstract bool CheckValid();
		
		public abstract void PurchaseConfirm( int _count);
	}
	
	public class CalledProc_PStore : AbCalledProc
	{
		delVoid m_delConfirm;
		
		UIPStoreSlot m_InputSlot;
		
		public CalledProc_PStore(UIPStoreSlot _slot, delVoid _del)
		{
			m_InputSlot = _slot;
			m_delConfirm = _del;
		}
		
		public override bool CheckValid()
		{
			if( m_InputSlot == null || m_delConfirm == null)
				return false;
			else
				return true;
		}
		
		public override void PurchaseConfirm( int _count)
		{
			m_delConfirm( m_InputSlot, _count);
				
			Debug.Log("CalledProc_PStore:: PurchaseConfirm: m_InputSlot.slotIndex = " + m_InputSlot.slotIndex + ", _count:" + _count);
		}
	}
	
	public class CalledProc_Search : AbCalledProc
	{
		delPurchaseConfirm m_delConfirm;
		
		UIPStoreSearchSlot m_InputSlot;
		
		public CalledProc_Search( UIPStoreSearchSlot _slot, delPurchaseConfirm _del)
		{
			m_InputSlot = _slot;
			m_delConfirm = _del;
		}
		
		public override bool CheckValid()
		{
			if( m_InputSlot == null || m_delConfirm == null)
				return false;
			else
				return true;
		}
		
		public override void PurchaseConfirm( int _count)
		{
			m_delConfirm(m_InputSlot, _count);
				
			Debug.Log("CalledProc_PStore:: PurchaseConfirm: m_InputSlot.SearchInfo.nPrivateShopSlot = " + m_InputSlot.SearchInfo.nPrivateShopSlot + ", _count:" + _count);
		}
	}
	#endregion
}


