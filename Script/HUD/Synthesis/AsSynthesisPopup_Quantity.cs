using UnityEngine;
using System.Collections;
//
//public class AsSynthesisPopup_Quantity : MonoBehaviour
//{
//	public UIButton cancelButton;
//	public UIButton okButton;
//	public UIButton minusButton;
//	public UIButton plusButton;
//	public UIButton minusButton_1;
//	public UIButton plusButton_1;
//	
//	public SpriteText titleText;
//	public SpriteText itemText;
//	public SpriteText inputText;
//	public GameObject iconParent;
//	public SpriteText m_TextCancel;
//	public SpriteText m_TextOk;
//	
//	private int m_nCurCount = 0;
//	private int m_nMaxCount = 0;
//	private RealItem m_realItemBuf = null;
//	private GameObject m_goIconImg = null;
//
//	void Start()
//	{
//	}
//
//	void Update()
//	{
//	}
//
//	public void Init()
//	{
//		AsLanguageManager.Instance.SetFontFromSystemLanguage( titleText);
//		AsLanguageManager.Instance.SetFontFromSystemLanguage( itemText);
//		AsLanguageManager.Instance.SetFontFromSystemLanguage( inputText);
//		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextCancel);
//		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextOk);
//
//		okButton.SetInputDelegate( _OkBtnDelegate);
//		cancelButton.SetInputDelegate( _CancelBtnDelegate);
//		plusButton.SetInputDelegate( _PlusBtnDelegate);
//		minusButton.SetInputDelegate( _MinusDataBtnDelegate);
//		
//		plusButton_1.SetInputDelegate( _PlusBtnDelegate_1);
//		minusButton_1.SetInputDelegate( _MinusDataBtnDelegate_1);
//		
//		_SetTitleText( AsTableManager.Instance.GetTbl_String( 1111));
//		m_TextCancel.Text = AsTableManager.Instance.GetTbl_String( 1151);
//		m_TextOk.Text = AsTableManager.Instance.GetTbl_String( 1152);
//	}
//
//	public void Open(RealItem realItem)
//	{
//		if( null == realItem)
//			return;
//		
//		m_realItemBuf = realItem;
//		m_nCurCount = m_nMaxCount = realItem.sItem.nOverlapped;
//		_SetItemText( realItem.item.ItemData);
//		_SetCount( m_nCurCount);
//		_SetIcon( realItem.item);
//
//		gameObject.SetActiveRecursively( true);
//	}
//	
//	public void Close()
//	{
//		gameObject.SetActiveRecursively( false);
//	}
//
//	// < private
//	private void _OkBtnDelegate(ref POINTER_INFO ptr)
//	{
//		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
//		{
//			if( null != m_realItemBuf)
//				AsHudDlgMgr.Instance.m_SynthesisDlg.SetSlotItem( m_realItemBuf, m_nCurCount, false);
//			
//			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
//			Close();
//		}
//	}
//	
//	private void _CancelBtnDelegate(ref POINTER_INFO ptr)
//	{
//		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
//		{
//			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
//			Close();
//		}
//	}
//	
//	private void _PlusBtnDelegate(ref POINTER_INFO ptr)
//	{
//		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
//		{
//			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
//			_SetCount( m_nCurCount + 1);
//		}
//	}
//	
//	private void _MinusDataBtnDelegate(ref POINTER_INFO ptr)
//	{
//		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
//		{
//			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
//			_SetCount( m_nCurCount - 1);
//		}
//	}
//	
//	
//	private void _PlusBtnDelegate_1(ref POINTER_INFO ptr)
//	{
//		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
//		{
//			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
//			_SetCount( m_nCurCount + 10);
//		}
//	}
//	
//	private void _MinusDataBtnDelegate_1(ref POINTER_INFO ptr)
//	{
//		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
//		{
//			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
//			_SetCount( m_nCurCount - 10);
//		}
//	}
//
//	private void _SetTitleText(string str)
//	{
//		if( null == titleText)
//			return;
//
//		titleText.Text = str;
//	}
//	
//	private void _SetItemText(string str)
//	{
//		if( null == itemText)
//			return;
//
//		itemText.Text = str;
//	}
//	
//	private void _SetItemText(ItemData itemData)
//	{
//		Tbl_String_Record strRecord = AsTableManager.Instance.GetTbl_String_Record( itemData.nameId);
//		
//		string strBuf = "";
//		if( null != strRecord)
//			strBuf = itemData.GetGradeColor() + strRecord.String;
//		
//		_SetItemText( strBuf);
//	}
//	
//	private void _SetInputText(string str)
//	{
//		if( null == inputText)
//			return;
//
//		inputText.Text = str;
//	}
//
//	private void _SetCount(int nCount)
//	{
//		m_nCurCount = nCount;
//
//		if( nCount < 1)
//			m_nCurCount = 1;
//		if( nCount > m_nMaxCount)
//			m_nCurCount = m_nMaxCount;
//
//		_SetInputText( m_nCurCount.ToString());
//	}
//	
//	private void _SetIcon(Item item)
//	{
//		Destroy( m_goIconImg);
//		
//		m_goIconImg = GameObject.Instantiate( item.GetIcon()) as GameObject;
//		m_goIconImg.transform.parent = iconParent.transform;
//		m_goIconImg.transform.localPosition = Vector3.zero;
//		m_goIconImg.transform.localRotation = Quaternion.identity;
//		m_goIconImg.transform.localScale = Vector3.one;
//	}
//	// private >
//}
