using UnityEngine;
using System.Collections;

public class UITradeDlg : MonoBehaviour
{
	public UIInvenSlot[] slots;
	public UIButton btnClose;
	public UIButton btnTradeOk;
	public UIButton btnTradeCancel;
	public SpriteText textNameL;
	public SimpleSprite imgPanelBaseL;
	public SimpleSprite imgPanelDoneL;
//	public SpriteText textGoldL;
	public UIStateToggleBtn btnLockL;
	public SpriteText textLockL;
	public SpriteText textNameR;
	public SimpleSprite imgPanelBaseR;
	public SimpleSprite imgPanelDoneR;
//	public SpriteText textGoldR;
	public UIStateToggleBtn btnLockR;
//	public UIButton btnGoldR;
	public SpriteText textLockR;
	public SpriteText m_TextTitle;
	public SpriteText m_TextCancel;
	public SpriteText m_TextOk;
	public SpriteText m_TextGuide;
	public GameObject guideBalloon;
	public GameObject slotItemParent;
	
	private GameObject m_goRoot = null;
	private UIInvenSlot m_ClickDownItemSlot;
	private static int SLOTMAX = 9;
	private bool[] m_bEmptySlot = new bool[SLOTMAX];
	private float m_fTimeBuf = 0.0f;
	private AsMessageBox m_msgboxRegistrationItemCancel = null;
	private AsMessageBox m_msgboxTradeCancel = null;
	private AsMessageBox m_msgboxTradeError = null;
	private bool m_bTradeOkLock = false;
	private Color m_color_normal = new Color( 0.0f, 0.3f, 0.8f, 1.0f);
	private Color m_color_disable = new Color( 0.58f, 0.58f, 0.58f, 1.0f);

	void Start()
	{
		m_msgboxRegistrationItemCancel = ( new GameObject( "msgboxRegistrationItemCancel")).AddComponent<AsMessageBox>();
		m_msgboxTradeCancel = ( new GameObject( "msgboxTradeCancel")).AddComponent<AsMessageBox>();
		m_msgboxTradeError = ( new GameObject( "msgboxTradeError")).AddComponent<AsMessageBox>();
		if( null != m_msgboxRegistrationItemCancel)
			m_msgboxRegistrationItemCancel.Close();
		if( null != m_msgboxTradeCancel)
			m_msgboxTradeCancel.Close();
		if( null != m_msgboxTradeError)
			m_msgboxTradeError.Close();

		btnClose.SetInputDelegate( _CloseBtnDelegate);
		btnTradeOk.SetInputDelegate( _OkBtnDelegate);
		btnTradeCancel.SetInputDelegate( _CancelBtnDelegate);
		btnLockR.SetInputDelegate( _LockBtnDelegate);
//		btnGoldR.SetInputDelegate( _GoldBtnDelegate);
		
		AsLanguageManager.Instance.SetFontFromSystemLanguage( textNameL);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( textLockL);
//		AsLanguageManager.Instance.SetFontFromSystemLanguage( textGoldL);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( textNameR);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( textLockR);
//		AsLanguageManager.Instance.SetFontFromSystemLanguage( textGoldR);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextTitle);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextCancel);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextOk);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextGuide);
		
		m_TextTitle.Text = AsTableManager.Instance.GetTbl_String( 1116);
		m_TextCancel.Text = AsTableManager.Instance.GetTbl_String( 1151);
		m_TextOk.Text = m_color_disable.ToString() + AsTableManager.Instance.GetTbl_String( 1297);
		m_TextGuide.Text = AsTableManager.Instance.GetTbl_String( 1710);
		textLockL.Text = AsTableManager.Instance.GetTbl_String( 856);
		textLockR.Text = AsTableManager.Instance.GetTbl_String( 856);
	}
	
	void Update()
	{
		if( true == gameObject.active)
		{
			if( true == _isLockAll())
			{
				if( true == m_bTradeOkLock)
					btnTradeOk.SetControlState( UIButton.CONTROL_STATE.ACTIVE);
				else
					btnTradeOk.SetControlState( UIButton.CONTROL_STATE.NORMAL);
			}
			else
				btnTradeOk.SetControlState( UIButton.CONTROL_STATE.NORMAL);
		}
	}
	
	public void Open(GameObject goRoot, string strUserPlayer, string strOtherPlayer)
	{
		gameObject.SetActiveRecursively( true);
		m_goRoot = goRoot;
		
		_InitSlotItmes();
		_InitImgPanel();
		_InitGold();
		_SetName( true, strOtherPlayer);
		_SetName( false, strUserPlayer);
		m_bTradeOkLock = false;
		
//		textGoldL.gameObject.SetActiveRecursively( false);
//		textGoldR.gameObject.SetActiveRecursively( false);

		textLockL.Text = AsTableManager.Instance.GetTbl_String( 856);
		textLockR.Text = AsTableManager.Instance.GetTbl_String( 856);
		btnLockL.SetState( 1);
		btnLockR.SetState( 1);
		
		m_TextOk.Text = m_color_disable.ToString() + AsTableManager.Instance.GetTbl_String( 1297);
	}
	
	public void Close(bool bSendTradeCancel)
	{
		gameObject.SetActiveRecursively( false);
		
		_ClearSlot();
		_CloseMsgBox_RegistrationItemCancel();
		_CloseMsgBox_TradeCancel();
		_CloseMsgBox_TradeError();
        ItemMgr.HadItemManagement.Inven.ResetInvenSlotMoveLock();
		if( true == bSendTradeCancel)
		{
			AsCommonSender.SendTradeCancel();
//			AsMessageManager.Instance.InsertMessage( AsTableManager.Instance.GetTbl_String( 26));
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(26), eCHATTYPE.eCHATTYPE_SYSTEM);
		}

		if( null != m_goRoot)
			Destroy( m_goRoot);
	}
	
	public bool IsRect( Ray inputRay)
	{
		if( null == collider)
			return false;
		
//		if( false == collider.bounds.IntersectRay( inputRay))
//			return false;
//		
//		return true;
		//return AsUtil.PtInCollider( collider, inputRay);
		return collider.bounds.IntersectRay( inputRay);
	}
	
	public void SetSlotItem(bool bOwnerIsMe, int nSlotIndex, RealItem realItem)
	{
		int nIndex = nSlotIndex;
		
		if( true == bOwnerIsMe)
		{
			nIndex = nSlotIndex + SLOTMAX;
			
			if( null != realItem)
				m_bEmptySlot[nSlotIndex] = false;
			else
				m_bEmptySlot[nSlotIndex] = true;
		}
		
		_SetSlotItem( nIndex, realItem);
	}
	
	public void SendMoveItem_InvenToTrade(RealItem realItem, int nItemCount)
	{
		if( true == _isLockR())
			return;

		int nTradeSlot = _GetEmptySlot();
		if( -1 == nTradeSlot)
		{
			//AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(30), eCHATTYPE.eCHATTYPE_SYSTEM);
			OpenMsgBox_TradeError( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(30));
			return;
		}
		
			
		if( true == realItem.item.ItemData.isTradeLimit  )
		{
			//AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(29), eCHATTYPE.eCHATTYPE_SYSTEM);
			OpenMsgBox_TradeError( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(29));
			return;
		}
		
		/*if( 0 != ((sbyte)eITEMATTRIBUTE.eITEMATTRIBUTE_TRADE_LIMIT & realItem.sItem.nAttribute) ||
			0 != ((sbyte)eITEMATTRIBUTE.eITEMATTRIBUTE_DUMP & realItem.sItem.nAttribute))*/
		if( false == realItem.sItem.IsTradeEnable() ||
			true == realItem.item.ItemData.isDump )
		{
			OpenMsgBox_TradeError( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(29));
			return;
		}
		
		int nSlot = realItem.getSlot;
		AsCommonSender.SendTradeRegistrationItem( true, nSlot, nTradeSlot, (short)nItemCount);
		AsSoundManager.Instance.PlaySound( realItem.item.ItemData.getStrDropSound, Vector3.zero, false);
	}
	
	public void SetLock(bool isLeft, bool bLock)
	{
		_SetImgPanel( isLeft, bLock);
		
		if( true == isLeft)
		{
			//UIButton.CONTROL_STATE s = UIButton.CONTROL_STATE.NORMAL;
			if( true == bLock)
			{
				//s = UIButton.CONTROL_STATE.ACTIVE;
				textLockL.Text = AsTableManager.Instance.GetTbl_String( 855);
				btnLockL.SetState( 0);
			}
			else
			{
				textLockL.Text = AsTableManager.Instance.GetTbl_String( 856);
				btnLockL.SetState( 1);
			}
			
			//btnLockL.set.SetControlState( s);
		}
		else
		{
			//UIButton.CONTROL_STATE s = UIButton.CONTROL_STATE.NORMAL;
			if( true == bLock)
			{
				//s = UIButton.CONTROL_STATE.ACTIVE;
				textLockR.Text = AsTableManager.Instance.GetTbl_String( 855);
				btnLockR.SetState( 0);
				
				guideBalloon.SetActiveRecursively( false);
			}
			else
			{
				textLockR.Text = AsTableManager.Instance.GetTbl_String( 856);
				btnLockR.SetState( 1);
				
				guideBalloon.SetActiveRecursively( true);
			}
			
			//btnLockR.SetControlState( s);
		}
		
		if( false == bLock)
		{
			btnTradeOk.SetControlState( UIButton.CONTROL_STATE.NORMAL);
			m_bTradeOkLock = false;
		}
		
		if( true == _isLockAll())
			m_TextOk.Text = m_color_normal.ToString() + AsTableManager.Instance.GetTbl_String( 1297);
		else
			m_TextOk.Text = m_color_disable.ToString() + AsTableManager.Instance.GetTbl_String( 1297);
	}
	
	public bool GetLock(bool isLeft)
	{
		if( true == isLeft)
			return _isLockL();
		return _isLockR();
	}
	
	public void SetGold(bool isLeft, long nGold)
	{
//		if( null == textGoldL || null == textGoldR)
//			return;
//		
//		if( nGold < 0)
//			return;
//		
//		if( true == isLeft)
//			textGoldL.Text = nGold.ToString();
//		else
//			textGoldR.Text = nGold.ToString();
	}

	public bool isOpenItemRegistrationCancel()
	{
		if( null == m_msgboxRegistrationItemCancel)
			return false;
		return true;
	}
	
	public bool isOpenTradeCancel()
	{
		if( null == m_msgboxTradeCancel)
			return false;
		return true;
	}
	
	public bool isOpenTradeError()
	{
		if( null == m_msgboxTradeError)
			return false;
		return true;
	}

	// < MessageBox
	public void OpenMsgBox_RegistrationItemCancel()
	{
		int nIndex = 0;

		foreach( UIInvenSlot slot in slots)
		{
			if( slot == m_ClickDownItemSlot)
			{
				if( nIndex < SLOTMAX)
					return;
			}
			
			nIndex++;
		}
		
		if( true == _isLockR())
			return;
		
		if( null != m_msgboxRegistrationItemCancel)
			return;
		
		string title = AsTableManager.Instance.GetTbl_String(1107);
		string msgRegistrationItemCancel = AsTableManager.Instance.GetTbl_String( 1299);
		m_msgboxRegistrationItemCancel = AsNotify.Instance.MessageBox( title, msgRegistrationItemCancel, this, "OnMsgBox_TradeRegistrationCancel_Ok", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);
	}

	public void OnMsgBox_TradeRegistrationCancel_Ok()
	{
		int nTradeSlot = m_ClickDownItemSlot.slotIndex;
		int nInvenSlot = m_ClickDownItemSlot.slotItem.realItem.getSlot;
		int nItemCnt = m_ClickDownItemSlot.slotItem.realItem.sItem.nOverlapped;
		
		AsCommonSender.SendTradeRegistrationItem( false, nInvenSlot, nTradeSlot, (short)nItemCnt);
	}
	
	public void OpenMsgBox_TradeCancel()
	{
		if( true == AsHudDlgMgr.Instance.IsOpenTradePopup_Gold)
			AsHudDlgMgr.Instance.CloseTradeGoldDlg();
		if( true == AsHudDlgMgr.Instance.IsOpenTradePopup_Quantity)
			AsHudDlgMgr.Instance.CloseTradeQuantityDlg();
		_CloseMsgBox_RegistrationItemCancel();
		_CloseMsgBox_TradeError();
		
		if( null != m_msgboxTradeCancel)
			return;
		
		string title = AsTableManager.Instance.GetTbl_String(1106);
		string msg = AsTableManager.Instance.GetTbl_String( 23);
		m_msgboxTradeCancel = AsNotify.Instance.MessageBox( title, msg, this, "OnMsgBox_TradeCancel_Ok", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);
	}
	
	public void OnMsgBox_TradeCancel_Ok()
	{
		AsHudDlgMgr.Instance.CloseTradeDlg( true);
	}
	
	public void OpenMsgBox_TradeError(string strTitle, string strErrMsg, bool bCloseTradeDlg = false)
	{
		if( null != m_msgboxTradeError)
			return;

		if( true == bCloseTradeDlg)
			AsHudDlgMgr.Instance.CloseTradeDlg( false);

		m_msgboxTradeCancel = AsNotify.Instance.MessageBox( strTitle, strErrMsg, this, "OnMsgBox_TradeError_Ok", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_QUESTION);
	}
	
	public void OnMsgBox_TradeError_Ok()
	{
	}
	// MessageBox >
	
	// < input
	public void InputDown( Ray inputRay)
	{			
		//TooltipMgr.Instance.Clear();
	}
	
	public void InputUp(Ray inputRay)
	{
	}
	
	public void InputMove(Ray inputRay)
	{
	}
	
	public void GuiInputDown( Ray inputRay)
	{
		if( false == _IsUseInput())
			return;		
		
		_SetClickDownSlot( null);
		
		foreach( UIInvenSlot slot in slots)
		{
			if( null != slot.slotItem && true == slot.IsIntersect( inputRay))
			{
				AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
				_SetClickDownSlot( slot);
				break;
			}
		}
	}	

	public void GuiInputMove(Ray inputRay)
	{
		if( false == _IsUseInput())
			return;

		if( null != m_ClickDownItemSlot)
		{
			if( true == m_ClickDownItemSlot.IsIntersect( inputRay) && null != m_ClickDownItemSlot.slotItem)
			{
				if( AsHudDlgMgr.Instance.invenDlg.m_fMaxItemMoveTime <= m_fTimeBuf)
				{
					OpenMsgBox_RegistrationItemCancel();
					m_fTimeBuf = 0.0f;
				}
				
				m_fTimeBuf += Time.deltaTime;
			}
		}
	}

	public void GuiInputUp(Ray inputRay)
	{
		if( false == _IsUseInput())
			return;
		
		if( null != m_ClickDownItemSlot)
		{
			if( m_ClickDownItemSlot.IsIntersect( inputRay))
			{
				_OpenTooltip();
			}
		}
	}
	
	public void GuiInputDClickUp(Ray inputRay)
	{
		if( false == _IsUseInput())
			return;
		
		if( null != m_ClickDownItemSlot && m_ClickDownItemSlot.IsIntersect( inputRay))
		{
			if( null != m_ClickDownItemSlot.slotItem)
			{
				OpenMsgBox_RegistrationItemCancel();
			}
		}
	}
	// input >
	
	// < private
	public void _InitSlotItmes()
	{
		if( null == slots)
			return;
		
		int nIndex = 0;

		foreach( UIInvenSlot slot in slots)
		{
			if( SLOTMAX == nIndex)
				nIndex = 0;
			slot.SetSlotIndex( nIndex);
			slot.DeleteSlotItem();
			nIndex++;
		}
		
		_InitSlotBuf();
	}

	private void _ClearSlot()
	{
		if( null == slots)
			return;
		
		foreach( UIInvenSlot slot in slots)
		{
			slot.DeleteSlotItem();
		}
	}
	
	private void _OpenTooltip()
	{
		if( null != m_msgboxRegistrationItemCancel)
			return;
		
		if( null != m_msgboxTradeError)
			return;
		
		if( null == m_ClickDownItemSlot || null == m_ClickDownItemSlot.slotItem)
			return;
		
		TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.left, m_ClickDownItemSlot.slotItem.realItem);
	}
	
	private bool _IsUseInput()
	{
		if( null != m_msgboxRegistrationItemCancel)
			return false;
		
		if( true == m_msgboxTradeCancel)
			return false;
		
		if( null != m_msgboxTradeError)
			return false;

		if( true == AsHudDlgMgr.Instance.IsOpenTradePopup_Gold)
			return false;
		
		if( true == AsHudDlgMgr.Instance.IsOpenTradePopup_Quantity)
			return false;
		
		return true;
	}
	
	private void _SetClickDownSlot( UIInvenSlot slot)
	{
		m_ClickDownItemSlot = slot;
	}
	
	private void _SetSlotItem(int nSlotIndex, RealItem realItem)
	{
		if( ( slots.Length - 1) < nSlotIndex || nSlotIndex < 0)
			return;
		
		slots[nSlotIndex].DeleteSlotItem();

		if( null != realItem)
		{
			slots[nSlotIndex].CreateSlotItem( realItem, slotItemParent.transform);
			Vector3 vColliderSize = slots[nSlotIndex].collider.bounds.size;
			slots[nSlotIndex].slotItem.iconImg.SetSize( vColliderSize.x, vColliderSize.y);
		}
	}
	
	// < button delegate
	private void _CloseBtnDelegate(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			OpenMsgBox_TradeCancel();
		}
	}
	
	private void _CancelBtnDelegate(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			OpenMsgBox_TradeCancel();
		}
	}

	private void _OkBtnDelegate(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if( true == _isLockAll() && false == m_bTradeOkLock)
			{
				AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
				AsCommonSender.SendTradeOk();
				m_bTradeOkLock = true;
			}
		}
	}
	
	private void _LockBtnDelegate(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			AsCommonSender.SendTradeLock( !_isLockR());
		}
	}
	
//	private void _GoldBtnDelegate(ref POINTER_INFO ptr)
//	{
//		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
//		{
//			if( false == _isLockR())
//				AsHudDlgMgr.Instance.OpenTradeGoldDlg();
//		}
//	}
	// button delegate >
	
	private bool _isLockR()
	{
		return ( imgPanelDoneR.gameObject.active);
	}
	
	private bool _isLockL()
	{
		return ( imgPanelDoneL.gameObject.active);
	}
	
	private bool _isLockAll()
	{
		return ( _isLockL() && _isLockR());
	}

	private void _SetImgPanel(bool isLeft, bool isDone)
	{
		if( true == isLeft)
		{
			imgPanelBaseL.gameObject.active = !isDone;
			//imgPanelDoneL.gameObject.active = isDone;
			imgPanelDoneL.gameObject.SetActiveRecursively( isDone);
		}
		else
		{
			imgPanelBaseR.gameObject.active = !isDone;
			//imgPanelDoneR.gameObject.active = isDone;
			imgPanelDoneR.gameObject.SetActiveRecursively( isDone);
		}
	}
	
	private void _InitImgPanel()
	{
		_SetImgPanel( true, false);
		_SetImgPanel( false, false);
	}
	
	private void _SetName(bool isLeft, string strName)
	{
		if( null == textNameL || null == textNameR)
			return;
		
		if( true == isLeft)
			textNameL.Text = strName;
		else
			textNameR.Text = strName;
	}

	private void _InitSlotBuf()
	{
		for( int i = 0; i < SLOTMAX; i++)
			m_bEmptySlot[i] = true;
	}
	
	private void _InitGold()
	{
		SetGold( true, 0);
		SetGold( false, 0);
	}
	
	private int _GetEmptySlot()
	{
		for( int i = 0; i < SLOTMAX; i++)
		{
			if( true == m_bEmptySlot[i])
				return i;
		}
		
		return -1;
	}

	private void _CloseMsgBox_RegistrationItemCancel()
	{
		if( null != m_msgboxRegistrationItemCancel)
			m_msgboxRegistrationItemCancel.Close();
	}
	
	private void _CloseMsgBox_TradeCancel()
	{
		if( null != m_msgboxTradeCancel)
			m_msgboxTradeCancel.Close();
	}
	
	private void _CloseMsgBox_TradeError()
	{
		if( null != m_msgboxTradeError)
			m_msgboxTradeError.Close();
	}
	// private >
}
