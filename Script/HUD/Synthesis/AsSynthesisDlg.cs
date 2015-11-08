using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//
//public class AsSynthesisDlg : MonoBehaviour
//{
//	public UIInvenSlot[] m_slots;
//	public UIButton m_BtnClose;
//	public UIButton m_BtnReset;
//	public UIButton m_BtnConfirm;
//	public GameObject m_slotItemParent;
//	public AsSynthesisPopup_Quantity m_SynthesisPopup_Quantity;
//	public SpriteText m_TextTitle;
//	public SpriteText m_TextReset;
//	public SpriteText m_TextConfirm;
//	
//	private GameObject m_goRoot = null;
//	private UIInvenSlot m_ClickDownItemSlot;
//	private float m_fTimeBuf = 0.0f;
//	private List<body2_CS_ITEM_MIX> m_listSynthesis = new List<body2_CS_ITEM_MIX>();
//	private int MAX_COUNT = 5;
//	private int[] m_nOverlappedBuf = new int[6];
//	private AsMessageBox m_msgboxSynthesisErr = null;
//	private System.Text.StringBuilder m_sbTemp = new System.Text.StringBuilder();
//	private Color enableColor = new Color( 0.0f, 0.29f, 0.84f, 1.0f);
//
//	void Start()
//	{
//		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextTitle);
//		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextReset);
//		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextConfirm);
//		
//		m_TextTitle.Text = AsTableManager.Instance.GetTbl_String( 1220);
//		m_TextReset.Text = AsTableManager.Instance.GetTbl_String( 1221);
//		m_TextConfirm.Text = AsTableManager.Instance.GetTbl_String( 1222);
//	}
//	
//	void Update()
//	{
//		if( 1 >= _CheckSynthesisItemSlotCount())
//		{
//			m_sbTemp.Length = 0;
//			m_sbTemp.Append( Color.gray);
//			m_sbTemp.Append( AsTableManager.Instance.GetTbl_String(1222));
//			m_TextConfirm.Text = m_sbTemp.ToString();
//		}
//		else
//		{
//			m_sbTemp.Length = 0;
//			m_sbTemp.Append( enableColor.ToString());
//			m_sbTemp.Append( AsTableManager.Instance.GetTbl_String(1222));
//			m_TextConfirm.Text = m_sbTemp.ToString();
//		}
//	}
//
//	public void Open(GameObject goRoot)
//	{
//		_Init();
//		gameObject.SetActiveRecursively( true);
//		m_goRoot = goRoot;
//		
//		if( null != m_SynthesisPopup_Quantity)
//		{
//			m_SynthesisPopup_Quantity.Init();
//			m_SynthesisPopup_Quantity.Close();
//		}
//
//		m_msgboxSynthesisErr = ( new GameObject( "msgboxSynthesisErr")).AddComponent<AsMessageBox>();
//		if( null != m_msgboxSynthesisErr)
//			m_msgboxSynthesisErr.Close();
//	}
//	
//	public void Close()
//	{
//		_CloseErrDlg();
//		ClosePopup_QuantityDlg();
//
//        ItemMgr.HadItemManagement.Inven.ResetInvenSlotMoveLock();
//		gameObject.SetActiveRecursively( false);
//		if( null != m_goRoot)
//			Destroy( m_goRoot);
//	}
//	
//	public bool IsRect( Ray inputRay)
//	{
//		if( null == collider)
//			return false;
//		
////		if( false == collider.bounds.IntersectRay( inputRay))
////			return false;
////		
////		return true;
//		//return AsUtil.PtInCollider( collider, inputRay);
//		return collider.bounds.IntersectRay( inputRay);
//	}
//	
//	public void SetSlotItem(RealItem realItem)
//	{
//		SetSlotItem( realItem, realItem.sItem.nOverlapped, true);
//	}
//	
//	public void SetSlotItem(RealItem realItem, int nOverlapped, bool bOpenQuantityDlg)
//	{
//		if( false == _isEmptyInvenSlot())
//			return;
//		
//		if( false == _CheckSynthesisItem( realItem))
//			return;
//		
//		if( true == bOpenQuantityDlg && realItem.sItem.nOverlapped > 1)
//		{
//			OpenPopup_QuantityDlg( realItem);
//			return;
//		}
//		else
//		{
//			foreach( UIInvenSlot slot in m_slots)
//			{
//				if( null == slot.slotItem)
//				{
//					if( slot.slotIndex < MAX_COUNT)
//					{
//						_SetSlotItem( slot.slotIndex, realItem, nOverlapped);
//						return;
//					}
//				}
//			}
//		}
//	}
//	
//	public void OpenPopup_QuantityDlg(RealItem realItem)
//	{
//		if( null != m_SynthesisPopup_Quantity)
//			m_SynthesisPopup_Quantity.Open( realItem);
//	}
//	
//	public void ClosePopup_QuantityDlg()
//	{
//		if( null != m_SynthesisPopup_Quantity)
//			m_SynthesisPopup_Quantity.Close();
//	}
//	
//	public bool IsOpenPopup_QuantityDlg()
//	{
//		if( null == m_SynthesisPopup_Quantity)
//			return false;
//		
//		return m_SynthesisPopup_Quantity.gameObject.active;
//	}
//	
//	public bool IsOpenPopup_Err()
//	{
//		if( null != m_msgboxSynthesisErr)
//			return true;
//		return false;
//	}
//
//	public void ReciveItemMixResult(eRESULTCODE eResultCode, short nInvenSlotIndex)
//	{
//		if( eRESULTCODE.eRESULT_SUCC != eResultCode)
//		{
//			Debug.Log( "Failed: ReciveItemMixResult(), ErrCode: " + eResultCode);
//			if( eRESULTCODE.eRESULT_ITEM_MIX_LACK_MIN == eResultCode || eRESULTCODE.eRESULT_ITEM_MIX_OVER_MAX == eResultCode)
//				_OpenErrDlg( 194);
//			return;
//		}
//
//        if (eRESULTCODE.eRESULT_SUCC == eResultCode)
//            QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_MIX_ITEM, new AchMixItem(1));
//		
//		RealItem realItem = ItemMgr.HadItemManagement.Inven.GetRealItemInSlot( (int)nInvenSlotIndex);
//		
//		if( null == realItem)
//		{
//			Debug.LogError( "Err: ReciveItemMixResult(): null == realItem, InvenSlotIndex: " + nInvenSlotIndex);
//			return;
//		}
//		
//		_SetSlotItem( MAX_COUNT, realItem);
//
//		if( null != m_slots[MAX_COUNT].m_SlotEff)
//			m_slots[MAX_COUNT].m_SlotEff.animation.Play();
//		
//		Debug.Log( "Succeeded: ReciveItemMixResult(), ItemID: " + realItem.item.ItemID);
//	}
//
//	// < input
//	public void InputDown( Ray inputRay)
//	{			
//		//TooltipMgr.Instance.Clear();
//	}
//	
//	public void InputUp(Ray inputRay)
//	{
//	}
//	
//	public void InputMove(Ray inputRay)
//	{
//	}
//	
//	public void GuiInputDown( Ray inputRay)
//	{
//		if( false == _IsUseInput())
//			return;
//		
//		//TooltipMgr.Instance.Clear();
//		_SetClickDownSlot( null);
//		
//		foreach( UIInvenSlot slot in m_slots)
//		{
//			if( null != slot.slotItem && true == slot.IsIntersect( inputRay))
//			{
//				_SetClickDownSlot( slot);
//				break;
//			}
//		}
//	}	
//
//	public void GuiInputMove(Ray inputRay)
//	{
//		if( false == _IsUseInput())
//			return;
//
//		if( null != m_ClickDownItemSlot)
//		{
//			if( true == m_ClickDownItemSlot.IsIntersect( inputRay) && null != m_ClickDownItemSlot.slotItem)
//			{
//				if( AsHudDlgMgr.Instance.invenDlg.m_fMaxItemMoveTime <= m_fTimeBuf)
//				{
//					int nSlotIndex = _FindSynthesisSlotIndexFromClickDownItemSlot();
//					if( nSlotIndex >= 0 && nSlotIndex < MAX_COUNT)
//					{
//						ItemMgr.HadItemManagement.Inven.SetSlotMoveLock( m_ClickDownItemSlot.slotItem.realItem.getSlot, false);
//						AsHudDlgMgr.Instance.invenDlg.ApplySlotMoveLock();
//						_SetSlotItem( nSlotIndex, null);
//					}
//					
//					m_fTimeBuf = 0.0f;
//				}
//				
//				m_fTimeBuf += Time.deltaTime;
//			}
//		}
//	}
//
//	public void GuiInputUp(Ray inputRay)
//	{
//		if( false == _IsUseInput())
//			return;
//		
//		if( null != m_ClickDownItemSlot)
//		{
//			if( m_ClickDownItemSlot.IsIntersect( inputRay))
//			{
//				_OpenTooltip();
//			}
//		}
//	}
//	
//	public void GuiInputDClickUp(Ray inputRay)
//	{
//		if( false == _IsUseInput())
//			return;
//		
//		if( null != m_ClickDownItemSlot && m_ClickDownItemSlot.IsIntersect( inputRay))
//		{
//			if( null != m_ClickDownItemSlot.slotItem)
//			{
//				int nSlotIndex = _FindSynthesisSlotIndexFromClickDownItemSlot();
//				if( nSlotIndex >= 0 && nSlotIndex < MAX_COUNT)
//				{
//					ItemMgr.HadItemManagement.Inven.SetSlotMoveLock( m_ClickDownItemSlot.slotItem.realItem.getSlot, false);
//					AsHudDlgMgr.Instance.invenDlg.ApplySlotMoveLock();
//					_SetSlotItem( nSlotIndex, null);
//				}
//			}
//		}
//	}
//	// input >
//
//	// < private
//	private void _Init()
//	{
//		m_BtnClose.SetInputDelegate( _BtnDelegate_Close);
//		m_BtnReset.SetInputDelegate( _BtnDelegate_Reset);
//		m_BtnConfirm.SetInputDelegate( _BtnDelegate_Confirm);
//
//		int nIndex = 0;
//
//		foreach( UIInvenSlot slot in m_slots)
//		{
//			slot.SetSlotIndex( nIndex);
//			slot.DeleteSlotItem();
//			m_nOverlappedBuf[nIndex] = 1;
//			nIndex++;
//		}
//	}
//
//	private void _BtnDelegate_Close(ref POINTER_INFO ptr)
//	{
//		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
//		{
//			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
//			AsHudDlgMgr.Instance.CloseSynthesisDlg();
//		}
//	}
//
//	private void _BtnDelegate_Reset(ref POINTER_INFO ptr)
//	{
//		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
//		{
//			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
//
//			foreach( UIInvenSlot slot in m_slots)
//			{
//				if( null != slot.slotItem)
//				{
//					ItemMgr.HadItemManagement.Inven.SetSlotMoveLock( slot.slotItem.realItem.getSlot, false);
//					AsHudDlgMgr.Instance.invenDlg.ApplySlotMoveLock();
//					_SetSlotItem( slot.slotIndex, null);
//				}
//			}
//		}
//	}
//
//	private void _BtnDelegate_Confirm(ref POINTER_INFO ptr)
//	{
//		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
//		{
//			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
//			
//			if( _CheckSynthesisItemSlotCount() < 2)
//			{
//				_OpenErrDlg( 192);
//				return;
//			}
//			
//			AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(408),
//				this, "Confirm", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION) );
//			
//			// kij
//			/*m_listSynthesis.Clear();
//			
//			foreach( UIInvenSlot slot in m_slots)
//			{
//				if( null != slot.slotItem)
//				{
//					short nInvenSlot = (short)( slot.slotItem.realItem.getSlot);
//					int nCount = m_nOverlappedBuf[slot.slotIndex];
//					m_listSynthesis.Add( new body2_CS_ITEM_MIX( nInvenSlot, nCount));
//					
//					if( null != slot.m_SlotEff)
//						slot.m_SlotEff.animation.Play();
//
//					AsHudDlgMgr.Instance.invenDlg.SetSlotMoveLock( slot.slotItem.realItem.getSlot, false);
//					AsHudDlgMgr.Instance.invenDlg.ApplySlotMoveLock();
//					_SetSlotItem( slot.slotIndex, null);
//				}
//			}
//			
//			if( m_listSynthesis.Count > 1)
//			{
//				AsCommonSender.SendItemMix( m_listSynthesis.Count, m_listSynthesis.ToArray());
//				m_listSynthesis.Clear();
//			}*/
//		}
//	}
//	
//	private void Confirm()
//	{
//		if( _CheckSynthesisItemSlotCount() < 2)
//		{
//			_OpenErrDlg( 192);
//			return;
//		}
//		
//		m_listSynthesis.Clear();
//			
//		foreach( UIInvenSlot slot in m_slots)
//		{
//			if( null != slot.slotItem)
//			{
//				short nInvenSlot = (short)( slot.slotItem.realItem.getSlot);
//				int nCount = m_nOverlappedBuf[slot.slotIndex];
//				m_listSynthesis.Add( new body2_CS_ITEM_MIX( nInvenSlot, nCount));
//				
//				if( null != slot.m_SlotEff)
//					slot.m_SlotEff.animation.Play();
//
//				ItemMgr.HadItemManagement.Inven.SetSlotMoveLock( slot.slotItem.realItem.getSlot, false);
//				AsHudDlgMgr.Instance.invenDlg.ApplySlotMoveLock();
//				_SetSlotItem( slot.slotIndex, null);
//			}
//		}
//		
//		if( m_listSynthesis.Count > 1)
//		{
//			AsCommonSender.SendItemMix( m_listSynthesis.Count, m_listSynthesis.ToArray());
//			m_listSynthesis.Clear();
//		}
//	}
//
//	private void _SetClickDownSlot( UIInvenSlot slot)
//	{
//		m_ClickDownItemSlot = slot;
//	}
//
//	private bool _IsUseInput()
//	{
//		if( true == IsOpenPopup_Err())
//			return false;
//		
//		if( true == IsOpenPopup_QuantityDlg())
//			return false;
//		
//		return true;
//	}
//
//	private void _OpenTooltip()
//	{
//		if( null == m_ClickDownItemSlot || null == m_ClickDownItemSlot.slotItem)
//			return;
//		
//		TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.normal, m_ClickDownItemSlot.slotItem.realItem);
//	}
//	
//	private void _SetSlotItem(int nSlotIndex, RealItem realItem)
//	{
//		_SetSlotItem( nSlotIndex, realItem, 1);
//	}
//
//	private void _SetSlotItem(int nSlotIndex, RealItem realItem, int nItemCount)
//	{
//		if( ( m_slots.Length - 1) < nSlotIndex || nSlotIndex < 0)
//			return;
//		
//		m_slots[nSlotIndex].DeleteSlotItem();
//		
//		if( null != m_slots[MAX_COUNT])
//			m_slots[MAX_COUNT].DeleteSlotItem();
//
//		if( null != realItem)
//		{
//			m_slots[nSlotIndex].CreateSlotItem( realItem, m_slotItemParent.transform);
//			Vector3 vColliderSize = m_slots[nSlotIndex].collider.bounds.size;
//			m_slots[nSlotIndex].slotItem.iconImg.SetSize( vColliderSize.x, vColliderSize.y);
//			AsSoundManager.Instance.PlaySound( realItem.item.ItemData.getStrDropSound, Vector3.zero, false);
//			
//			if( nItemCount > 0)
//			{
//				m_nOverlappedBuf[nSlotIndex] = nItemCount;
//				m_slots[nSlotIndex].slotItem.SetItemCountText( nItemCount);
//			}
//			
//			if( nSlotIndex < MAX_COUNT)
//			{
//				ItemMgr.HadItemManagement.Inven.SetSlotMoveLock( realItem.getSlot, true);
//				AsHudDlgMgr.Instance.invenDlg.ApplySlotMoveLock();
//			}
//		}
//	}
//	
//	private int _FindSynthesisSlotIndexFromClickDownItemSlot()
//	{
//		foreach( UIInvenSlot slot in m_slots)
//		{
//			if( null != slot.slotItem)
//			{
//				if( m_ClickDownItemSlot.slotItem.realItem.getSlot == slot.slotItem.realItem.getSlot)
//					return slot.slotIndex;
//			}
//		}
//		
//		return -1;
//	}
//	
//	private bool _isEmptyInvenSlot()
//	{
//		int nRes = ItemMgr.HadItemManagement.Inven.GetEmptyInvenSlot();
//		
//		if( -1 == nRes)
//		{
//			_OpenErrDlg( 191);
//			return false;
//		}
//		
//		return true;
//	}
//	
//	private bool _CheckSynthesisItem(RealItem realItem)
//	{
////		if( false == realItem.item.ItemData.isShopSell)
////		{
////			_OpenErrDlg( 190);
////			return false;
////		}
////		
////		if( realItem.item.ItemData.sellAmount < 1)
////		{
////			_OpenErrDlg( 190);
////			return false;
////		}
////		
////		return true;
//
//		if( Item.eITEM_TYPE.EquipItem == realItem.item.ItemData.GetItemType())
//		{
//			if( ItemMgr.GetRealRankPoint( realItem.sItem, realItem.item) >= 1)
//				return true;
//		}
//		
//		_OpenErrDlg( 190);
//		
//		return false;
//	}
//	
//	private int _CheckSynthesisItemSlotCount()
//	{
//		int nSlotCount = 0;
//		
//		foreach( UIInvenSlot slot in m_slots)
//		{
//			if( null != slot.slotItem)
//				nSlotCount++;
//		}
//		
//		return nSlotCount;
//	}
//	
//	private void _OpenErrDlg(int nStringTblIndex)
//	{
//		if( null != m_msgboxSynthesisErr)
//			return;
//		
//		string title = AsTableManager.Instance.GetTbl_String( 1223);
//		string msg = AsTableManager.Instance.GetTbl_String( nStringTblIndex);
//		m_msgboxSynthesisErr = AsNotify.Instance.MessageBox( title, msg, null, null, AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_ERROR);
//	}
//	
//	private void _CloseErrDlg()
//	{
//		if( null != m_msgboxSynthesisErr)
//			m_msgboxSynthesisErr.Close();
//	}
//	// private >
//}
