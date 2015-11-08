using UnityEngine;
using System.Collections;

//public enum eDiscardType {From_Inven, From_Storage}

public class UIStorageDlg : MonoBehaviour 
{
	// member
	#region - public -
	public UIStorageSlot moveItemSlot;
	public UIStorageSlot []storageslots;
	public GameObject slotItemParent;
	public UIStoragePage page;
	public float dragPageMoveDistance = 3.0f;
	public SimpleSprite focusImg;
	
	public UIStorageQuantityDlg quantityDlg;
	public UIStorageAdditionDlg additionDlg;
	public GameObject imgLock;
	
	public UIStorageSortButton sortButton;
	public UIButton closeButton;
	public UIButton storageAddButton;
	
	public float m_fMaxItemMoveTime = 0.5f;
	public float m_fMaxPageMoveTime = 0.5f;
	
	public SpriteText m_TextTitle;
	public SpriteText m_TextAdd;
	
	[SerializeField] GameObject[] lineLock = new GameObject[5];
	[SerializeField] GameObject[] effectOpen = new GameObject[5];
	[SerializeField] SpriteText  curMiracle;
	[SerializeField] SpriteText  strSort;
	
	bool m_PageLocked; public bool PageLocked{get{return m_PageLocked;}}
	
	public static readonly int pageSlotCount = 25;
	#endregion
	#region - private -
	// input click down information
	private Vector2 m_vec2DownPosition;
	private UIStorageSlot m_ClickDownItemSlot;
	
	// item move active time 
	
	private float m_fItemMoveTime = 0.0f;
	
	// page move active time
	
	private float m_fNextPageTime = 0.0f;
	private float m_fPrePageTime = 0.0f;	
	private bool m_bMovePageEnable = false;
	
	// remove item check information
	private int m_iReallyRemoveItemSlotIndex;
	private int m_iReallyRemoveItemCount;
	private AsMessageBox m_curReallyRemoveItemDlg = null;
	private AsMessageBox m_Popup = null;
	#endregion
	
	#region - init -
	void Start()
	{
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextTitle);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextAdd);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( strSort);
		
		m_TextTitle.Text = AsTableManager.Instance.GetTbl_String( 19003);
		m_TextAdd.Text = AsTableManager.Instance.GetTbl_String( 1390);
		strSort.Text = AsTableManager.Instance.GetTbl_String(1918);
		
		foreach(GameObject obj in lineLock)
		{
			SpriteText text = obj.transform.GetChild(0).GetComponent<SpriteText>();
			text.Text = AsTableManager.Instance.GetTbl_String( 1350);
		}
		
		foreach(GameObject obj in effectOpen)
		{
			obj.SetActiveRecursively(false);
		}
		
		ResetSlotItmes();
		PageLock();
		Debug.Log("UIStorageDlg:: Start");
	}
	
//	void OnEnable()
//	{
//		ResetSlotItmes();
//		Debug.Log("UIStorageDlg:: OnEnable");
//	}
	#endregion
	
	#region - open & close -
	public void Open()
	{
		imgLock.SetActiveRecursively(false);
		
//		ResetSlotItmes(); 		// current page slot item initialization
		DetachFocusImg();		// Detach Focus Image
		ResetMoveSlot();		// move slot initialization
		SetClickDownSlot(null); // click down slot initialization
		sortButton.Init();		// sort button initaialization		
		CloseStorageQuantityDlg();
		CloseStorageAdditionDlg();
		
		closeButton.SetInputDelegate(CloseBtnDelegate);
		storageAddButton.SetInputDelegate(OpenStorageAdditionDlg);
		
		AsHudDlgMgr.Instance.storageQuantityDlg = quantityDlg;
		AsHudDlgMgr.Instance.storageAdditionDlg = additionDlg;
		
		if(AsHudDlgMgr.Instance.IsOpenInven == false) AsHudDlgMgr.Instance.OpenInven();
		
		ResetMiracleText();
		PageLock();
	}
	
	public void ResetMiracleText()
	{
		if( Storage.maxStorageLine <= ItemMgr.HadItemManagement.Storage.lineOpen )
		{
			storageAddButton.controlIsEnabled = false;
			curMiracle.Text = "0";
		}
		else
		{
			int iIndex = 120 + ItemMgr.HadItemManagement.Storage.pageOpen;
			curMiracle.Text = AsTableManager.Instance.GetTbl_GlobalWeight_Record(iIndex).Value.ToString();
			storageAddButton.controlIsEnabled = true;
		}
	}
	
	
//	// close dialog
	public void Close()
	{		
		ClearSlot();
		DetachFocusImg();
		moveItemSlot.DeleteSlotItem();
		ResetMoveSlot();
		SetClickDownSlot(null);		
		CloseStorageQuantityDlg();
		CloseStorageAdditionDlg();
        ResetSlotMoveLock();
		
		if(AsHudDlgMgr.Instance.IsOpenInven == true) 
			AsHudDlgMgr.Instance.CloseInven();
		
		
		
		if( null != m_curReallyRemoveItemDlg )
		{
			m_curReallyRemoveItemDlg.Close();
			m_curReallyRemoveItemDlg = null;
		}
		
		if( null!=m_Popup)
		{
			m_Popup.Close();
			m_Popup = null;
		}
	}
	
	private void CloseBtnDelegate(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			AsHudDlgMgr.Instance.CloseStorage();
		}
	}
	
//	bool disabled = false;
//	void OnDisable()
//	{
//		if(disabled == false)
//		{
//			disabled = true;
//			Close();
//		}
//	}
	#endregion	
	#region - sound & etc -
	public void PlayDropSound( Item _item )	
	{
		if( null == _item)
		{
			Debug.LogError("UIStorageDlg::PlayDropSound() [ null == _item ]");
			return;
		}
		
		if(string.Compare("NONE", _item.ItemData.getStrDropSound, true) == 0)
		{
			Debug.LogError("UIStorageDlg::PlayDropSound()[getStrDropSound == NONE] id : " + _item.ItemID );
			return;
		}
		
		AsSoundManager.Instance.PlaySound( _item.ItemData.getStrDropSound, Vector3.zero, false  );
	}
		
	public bool IsRect( Ray inputRay )
	{
		if( null == collider )
			return false;
		
		return collider.bounds.IntersectRay( inputRay );
//		return AsUtil.PtInCollider( collider, inputRay);
	}
	
	public void MessageBoxPopup(AsMessageBox _popup)
	{
		m_Popup = _popup;
	}
	
	private bool IsUseInput()
	{		
//		if( true == this.removeItemDlg.IsOpen() )
//			return false;
		
//		if( null != m_curReallyRemoveItemDlg )
//			return false;		
//		
//		if( null != m_SkillResetMessageBox)
//			return false;
		
		if( true == AsHudDlgMgr.Instance.IsOpenTradePopup_ItemRegistrationCancel
			|| true == AsHudDlgMgr.Instance.IsOpenTradePopup_TradeCancel
			|| true == AsHudDlgMgr.Instance.IsOpenTradePopup_Gold
			|| true == AsHudDlgMgr.Instance.IsOpenTradePopup_Quantity
			|| true == AsHudDlgMgr.Instance.IsOpenTradePopup_TradeError)
			return false;
		
		if(m_Popup != null)
			return false;
		
		if( true == AsHudDlgMgr.Instance.IsOpenStorageQuantityDlg)
			return false;
		
		return true;
	}
	#endregion		
	#region - move slot -
	private void SetMoveSlot( UIStorageSlot slot )
	{
		if( null != slot.slotItem )
			slot.slotItem.ShowCoolTime( false );
		
		moveItemSlot.SetSlotItem( slot.slotItem );
		moveItemSlot.SetSlotIndex( slot.slotIndex );
	}
	
	private void ResetMoveSlot()
	{
		if( null != moveItemSlot.slotItem )
			moveItemSlot.slotItem.ShowCoolTime( true );
		
		moveItemSlot.SetSlotItem( null );
		moveItemSlot.SetSlotIndex( 0 );
	}
	#endregion	
	#region - quantity dlg -
	public void OpenStorageQuantityDlg( int iSlotIndex, int iMaxCount)
	{
		Debug.Log("Invalid calling");
		
//		//if( true == AsHudDlgMgr.Instance.IsDontMoveState)
//		if( true == AsHudDlgMgr.Instance.IsOpenTrade || true == AsHudDlgMgr.Instance.IsOpenEnchantDlg || true == AsHudDlgMgr.Instance.IsOpenStrengthenDlg )
//			return;
//		
//		if( null == quantityDlg )
//		{
//			Debug.LogError("UIStorageDlg::OpenStorageQuantityDlg() [ null == quantityDlg ] ");
//			return;
//		}
//		
//		quantityDlg.Open(iSlotIndex, iMaxCount);
	}
	
	public void CloseStorageQuantityDlg()
	{
		if( null == quantityDlg )
		{
			Debug.LogError("UIStorageDlg::CloseStorageQuantityDlg() [ null == quantityDlg ] ");
			return;
		}
		quantityDlg.Close();
	}
	#endregion	
	#region - addition dlg -
	public void OpenStorageAdditionDlg(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			StorageAdditionDlg();			
			
//			OpenStorageAdditionDlg();
		}
	}
	
	void StorageAdditionDlg()
	{
		
		int iIndex = 120 + ItemMgr.HadItemManagement.Storage.pageOpen;	
		Tbl_GlobalWeight_Record _record = AsTableManager.Instance.GetTbl_GlobalWeight_Record(iIndex);
		if( null== _record)
		{
			Debug.LogError("StorageDlg::StorageAdditionDlg()[ not find GetTbl_GlobalWeight_Record : " + iIndex );
			return;
		}
		
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
				
		long cash = (long)_record.Value;
		string title = AsTableManager.Instance.GetTbl_String(1632);
		string content = AsTableManager.Instance.GetTbl_String(1371);
		
		m_Popup = AsNotify.Instance.CashMessageBox(cash, title, content, this, "OnAdditionConfirm", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);			
	}
	
	void OnAdditionConfirm()
	{
		int iIndex = 120 + ItemMgr.HadItemManagement.Storage.pageOpen;			
		long cash = (long)AsTableManager.Instance.GetTbl_GlobalWeight_Record(iIndex).Value;
		if(AsUserInfo.Instance.nMiracle < cash)
		{
//			string title = AsTableManager.Instance.GetTbl_String(1412);
			string content = AsTableManager.Instance.GetTbl_String(264);
			
            if (AsGameMain.useCashShop == true)
			    m_Popup = AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(126), content, this, "OpenCashShop", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);
            else
                m_Popup = AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(126), content, AsNotify.MSG_BOX_TYPE.MBT_OK);

		}
		else
		{
			body_CS_STORAGE_COUNT_UP data = new body_CS_STORAGE_COUNT_UP();
			AsCommonSender.Send(data.ClassToPacketBytes());
		}
	}
	
	private void OpenCashShop()
	{
		// cash store required
		AsHudDlgMgr.Instance.CloseStorage();

		AsHudDlgMgr.Instance.OpenCashStore(0, AsEntityManager.Instance.UserEntity.GetProperty<eCLASS>(eComponentProperty.CLASS), eCashStoreMenuMode.CHARGE_MIRACLE, eCashStoreSubCategory.NONE, 0);
	}
	
//	public void OpenStorageAdditionDlg()
//	{
//		//if( true == AsHudDlgMgr.Instance.IsDontMoveState)
//		if( true == AsHudDlgMgr.Instance.IsOpenTrade || true == AsHudDlgMgr.Instance.IsOpenEnchantDlg || true == AsHudDlgMgr.Instance.IsOpenStrengthenDlg )
//			return;
//		
//		if( null == additionDlg )
//		{
//			Debug.LogError("UIStorageDlg::OpenStorageAdditionDlg() [ null == additionDlg ] ");
//			return;
//		}
//		
//		int opened = ItemMgr.HadItemManagement.Storage.pageOpen;		
//		Tbl_GlobalWeight_Record record = AsTableManager.Instance.GetTbl_GlobalWeight_Record(opened + 9);
//		
//		if(AsUserInfo.Instance.SavedCharStat.nGold < (ulong)record.Value)
//		{
//			string text = AsTableManager.Instance.GetTbl_String(209);
//			text = string.Format(text, Color.yellow.ToString() + record.Value + "G" + Color.white.ToString());
//			AsNotify.Instance.MessageBox( "Addition", text,
//				null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_ERROR);
//		}
//		else
//		{
//			additionDlg.Open((int)record.Value);
//		}
//		
//		
//	}
	
	public void CloseStorageAdditionDlg()
	{
		if( null == additionDlg )
		{
			Debug.LogError("UIStorageDlg::CloseStorageAdditionDlg() [ null == additionDlg ] ");
			return;
		}
		additionDlg.Close();
	}
	#endregion
	#region - input -
	public void InputDown( Ray inputRay)
	{			
		//TooltipMgr.Instance.Clear();
	}
	
	public void InputMove(Ray inputRay)
	{			
		if( null != moveItemSlot.slotItem )
		{
			Vector3 pt = inputRay.origin;
			pt.z = moveItemSlot.gameObject.transform.position.z - 10.0f;
			moveItemSlot.SetSlotItemPosition( pt);			
		}		
	}
	
	public void InputUp(Ray inputRay)
	{
		if( null != moveItemSlot.slotItem )
		{
			RaycastHit hit;
			if(Physics.Raycast(inputRay, out hit, 10000f, 1<<LayerMask.NameToLayer("GUI")) == false)		
			{		
				if( true == AsUserInfo.Instance.IsLiving() )
				{	
					SetWastebasket();				
					SetRestoreSlot();
					m_bMovePageEnable = false;
				}
				else
				{
					//message 
					SetRestoreSlot();	
					m_bMovePageEnable = false;
//					Debug.Log("Don't item wastebasket");
				}
			}			
		}
	}
	
	public void GuiInputDown( Ray inputRay )
	{		
		if( false == IsUseInput() )			
			return;	
		
		//TooltipMgr.Instance.Clear();
		
		SetRestoreSlot();
		
		SetClickDownSlot( null );		
		m_fItemMoveTime = 0.0f;
		m_fPrePageTime = 0.0f;
		m_fNextPageTime = 0.0f;		
		
		if( false == IsRect( inputRay ) )
			return;
		
		
		m_bMovePageEnable = true;
		m_vec2DownPosition = inputRay.origin;
		
		
		foreach( UIStorageSlot slot in storageslots )
		{
			if( null != slot.slotItem && true == slot.IsIntersect( inputRay ) )
			{				
				AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
				SetClickDownSlot( slot );
				break;
			}
		}
	}	
	
	private bool IsRectPage( Ray ray )
	{
		
		foreach( GameObject go in lineLock )
		{
			if( false == go.active )
				continue;
			
			if( null == go.collider  )
				continue;			
			if( true == AsUtil.PtInCollider( go.collider, ray, false ) )
			{
				return true;
			}
		}
		
		
		foreach( UIStorageSlot slot in storageslots )
		{
			if( true == slot.IsIntersect( ray ) )
			{				
				return true;
			}
		}
		
		return false;
	}
	
//	
//	
//	
	public void GuiInputMove(Ray inputRay)
	{
		if( false == IsUseInput() )
			return;
		
		// exist move item 
		if( null == moveItemSlot.slotItem )
		{
			if( null != m_ClickDownItemSlot )
			{
				if( true == m_ClickDownItemSlot.IsIntersect( inputRay ) && false == m_ClickDownItemSlot.isMoveLock )
				{				
					if( m_fMaxItemMoveTime <= m_fItemMoveTime )
					{						
						SetMoveSlot( m_ClickDownItemSlot );
						m_ClickDownItemSlot.SetSlotItem(null);
						AttachFocusImg( m_ClickDownItemSlot );	
						m_fItemMoveTime = 0.0f;						
					}
					
					m_fItemMoveTime += Time.deltaTime;
				}
			}
				
			if( true == m_bMovePageEnable && ( page.IsPageeRectIntersect( inputRay ) || IsRectPage(inputRay ) ) )
			{
				
				Vector2 vec2Direction = Vector2.zero;
				vec2Direction.x =  inputRay.origin.x;
				//vec2Direction.y =  inputRay.origin.y;
				
				Vector2 vec2TargetDirection = Vector2.zero;
				vec2TargetDirection.x =  m_vec2DownPosition.x;
				
				vec2Direction = vec2Direction - vec2TargetDirection;      		
				if( dragPageMoveDistance < vec2Direction.magnitude)
				{
					if( 0 > Vector2.Dot( Vector2.right, vec2Direction.normalized ) )
					{
						NextPage();
					}
					else
					{
						PrePage();
					}
					m_bMovePageEnable = false;
				}
			} 
		}
		else
		{
			if( true == page.IsPageNextRectIntersect( inputRay ) )
			{				
				if( m_fMaxPageMoveTime <= m_fNextPageTime )
				{				
					NextPage();
					m_fNextPageTime = 0.0f;
				}	
				m_fNextPageTime += Time.deltaTime;
			}
			else if ( true == page.IsPagePreRectIntersect( inputRay ) )
			{				
				if( m_fMaxPageMoveTime <= m_fPrePageTime )
				{						
					PrePage();
					m_fPrePageTime = 0.0f;
				}	
				m_fPrePageTime += Time.deltaTime;	
			}
			else
			{
				m_fPrePageTime = 0.0f;
				m_fNextPageTime = 0.0f;
			}
		}
	}
	
	public void GuiInputUp(Ray inputRay)
	{ 			
		if( false == IsUseInput() )
			return;
		
//		if( true == this.quantityDlg.IsOpen() )
//		{
//			quantityDlg.GuiInputUp( inputRay );
//			return;
//		}
//		
//		if( true == this.additionDlg.IsOpen() )
//		{
//			additionDlg.GuiInputUp( inputRay );
//			return;
//		}
		
		sortButton.GuiInputUp( inputRay, page.curPage );
		
		if( true == CheckLineLockClicked(inputRay) )
		{
			if( null == moveItemSlot.slotItem && true == m_bMovePageEnable )
			{		
				StorageAdditionDlg();			
				
				m_bMovePageEnable = false;
				return;
			}
			
			if( null != moveItemSlot.slotItem)
			{
				SetRestoreSlot();
				return;
			}
		}
		
//		if( null == moveItemSlot.slotItem && true == m_bMovePageEnable )
//		{
//			if( true == CheckLineLockClicked(inputRay) )
//			{				
//				StorageAdditionDlg();			
//				
//				m_bMovePageEnable = false;
//				return;
//			}
//		}		
		
		if( null != moveItemSlot.slotItem && null != m_ClickDownItemSlot )
		{
			if( m_ClickDownItemSlot.IsIntersect( inputRay ) )
			{				
				if( m_ClickDownItemSlot.slotIndex == moveItemSlot.slotIndex )
				{
					SetRestoreSlot();						
				}
				else
				{
					if( true == AsHudDlgMgr.Instance.IsDontMoveState)
//					if( false == AsHudDlgMgr.Instance.IsOpenTrade && false == AsHudDlgMgr.Instance.IsOpenEnchantDlg && false == AsHudDlgMgr.Instance.IsOpenStrengthenDlg )
					{
						PlayDropSound( moveItemSlot.slotItem.realItem.item );
						
						body_CS_STORAGE_MOVE move = new body_CS_STORAGE_MOVE(eSTORAGE_MOVE_TYPE.eSTORAGE_MOVE_TYPE_INSIDE,
								(short)moveItemSlot.slotIndex, moveItemSlot.slotItem.realItem.sItem.nOverlapped, (short)m_ClickDownItemSlot.slotIndex);
						
						Debug.Log(moveItemSlot.slotIndex + " -> " + m_ClickDownItemSlot.slotIndex + "(count:" + moveItemSlot.slotItem.realItem.sItem.nOverlapped + ")");
						AsCommonSender.Send(move.ClassToPacketBytes());
						moveItemSlot.DeleteSlotItem();
					}
				}
			}
			else
			{		
				bool bExistSlot = false;
				foreach( UIStorageSlot slot in storageslots )
				{
					if( slot.IsIntersect( inputRay ) )
					{				
						if( null != slot.slotItem  )
						{
							if( moveItemSlot.slotItem.realItem.sItem.nItemTableIdx == slot.slotItem.realItem.sItem.nItemTableIdx )
							{
								if( slot.slotItem.realItem.sItem.nOverlapped >= slot.slotItem.realItem.item.ItemData.overlapCount )
								{
									break;
								}
							}
						}
						
						//if( true == AsHudDlgMgr.Instance.IsDontMoveState)
						if( false == AsHudDlgMgr.Instance.IsOpenTrade && false == AsHudDlgMgr.Instance.IsOpenEnchantDlg && false == AsHudDlgMgr.Instance.IsOpenStrengthenDlg )
						{
							PlayDropSound( moveItemSlot.slotItem.realItem.item );
							
							body_CS_STORAGE_MOVE move = new body_CS_STORAGE_MOVE(eSTORAGE_MOVE_TYPE.eSTORAGE_MOVE_TYPE_INSIDE,
								(short)moveItemSlot.slotIndex, moveItemSlot.slotItem.realItem.sItem.nOverlapped, (short)slot.slotIndex);
							
							Debug.Log(moveItemSlot.slotIndex + " -> " + slot.slotIndex + "(count:" + moveItemSlot.slotItem.realItem.sItem.nOverlapped + ")");
							AsCommonSender.Send(move.ClassToPacketBytes());
							moveItemSlot.DeleteSlotItem();
						}
						else
							SetRestoreSlot();
						
						bExistSlot = true;
						break;
					}
				}
				
				if( false == bExistSlot )
				{
					if( true == AsHudDlgMgr.Instance.SendMoveItem_StorageToInven(inputRay, moveItemSlot))  //AsQuickSlotManager.Instance.SetMoveInvenSlotInItemSlot( inputRay, moveItemSlot ) )
					{
						SetRestoreSlot();
					}
					else
					{
						SetRestoreSlot();
//						body_CS_STORAGE_MOVE move = new body_CS_STORAGE_MOVE(eSTORAGE_MOVE_TYPE.eSTORAGE_MOVE_TYPE_REMOVE,
//							(short)moveItemSlot.slotIndex, moveItemSlot.slotItem.realItem.sItem.nOverlapped, 0);
//						AsCommonSender.Send(move.ClassToPacketBytes());
//						moveItemSlot.DeleteSlotItem();
////						SetRestoreSlot();
					}
				}
			}		
		}
		else if( null != m_ClickDownItemSlot && true == m_ClickDownItemSlot.IsIntersect( inputRay ) )			
		{							
			OpenTooltip();			
		}		
//		else if( true == m_bMovePageEnable )
//		{
//			if( true == page.IsPageeRectIntersect( inputRay ) )
//			{
//				
//				Vector2 vec2Direction = Vector2.zero;
//				vec2Direction.x =  inputRay.origin.x;
//				vec2Direction.y =  inputRay.origin.y;
//				
//				vec2Direction = vec2Direction - m_vec2DownPosition;      		
//				if( dragPageMoveDistance < vec2Direction.magnitude)
//				{
//					if( 0 > Vector2.Dot( Vector2.right, vec2Direction.normalized ) )
//					{
//						NextPage();
//					}
//					else
//					{
//						PrePage();
//					}
//				}
//			}
//		}
		
		m_bMovePageEnable = false;
		
		
	}	
	
	bool CheckLineLockClicked(Ray inputRay)
	{
		foreach(GameObject obj in lineLock)
		{
//			if(obj.collider.bounds.IntersectRay(inputRay) == true)
			if( true == AsUtil.PtInCollider( obj.collider, inputRay))
			{
				return true;
			}
		}
		
		return false;
	}
	
	public bool SetPostBoxToStorage( AsSlot _slot, Ray inputRay )
	{
		foreach( UIStorageSlot slot in storageslots )
		{
			if( true == slot.IsIntersect( inputRay ) )
			{	
				if( null == slot.slotItem )
				{
					// send packet
					return true;
				}
				return false;
			}
		}
		
		return false;
	}
	
	public void GuiInputClickUp(Ray inputRay)
	{
		m_bMovePageEnable = false;
		
		if( false == IsUseInput() )
			return;				
			
//		if( null == moveItemSlot.slotItem && true == CheckLineLockClicked(inputRay))// && m_Popup == null)
//		{
//			StorageAdditionDlg();
//		}	
	}

	public void GuiInputDClickUp(Ray inputRay)
	{ 
		m_bMovePageEnable = false;
		
		if( false == IsUseInput() )
			return;				
			
		if( null != moveItemSlot.slotItem )
		{
			SetRestoreSlot();
		}
		else if( null != m_ClickDownItemSlot && m_ClickDownItemSlot.IsIntersect( inputRay ) )
		{
			if( null != m_ClickDownItemSlot.slotItem )
			{
//				Item.eITEM_TYPE type = m_ClickDownItemSlot.slotItem.realItem.item.ItemData.GetItemType();
//				int iSubType = m_ClickDownItemSlot.slotItem.realItem.item.ItemData.GetSubType();
					
				if( m_ClickDownItemSlot.slotItem.realItem.getSlot != m_ClickDownItemSlot.slotIndex )
				{
					Debug.LogError("m_ClickDownItemSlot.slotItem.realItem.sItem.nSlot != m_ClickDownItemSlot.slotIndex");
				}
				else
				{
					if( true == AsHudDlgMgr.Instance.IsOpenInven)
					{
						body_CS_STORAGE_MOVE move = new body_CS_STORAGE_MOVE(eSTORAGE_MOVE_TYPE.eSTORAGE_MOVE_TYPE_OUTPUT,
							(short)m_ClickDownItemSlot.slotIndex, m_ClickDownItemSlot.slotItem.realItem.sItem.nOverlapped, 0);
						AsCommonSender.Send(move.ClassToPacketBytes());
					}
				}
				
			}
			
		}		
	}
	#endregion	
	#region - tool tip -
	private void OpenTooltip()
	{
		if( null == m_ClickDownItemSlot.slotItem )
		{
			//Debug.LogError("UIStorageDlg::OpenTooltip() [ null == m_ClickDownItemSlot.slotItem ]");
			return;
		}
		switch( m_ClickDownItemSlot.slotItem.realItem.item.ItemData.GetItemType() )
		{
		case Item.eITEM_TYPE.CosEquipItem:
			OpenCosEquipItemToolTip(m_ClickDownItemSlot.slotItem.realItem);
			break;
			
		case Item.eITEM_TYPE.EquipItem:
			OpenEquipItemToolTip(m_ClickDownItemSlot.slotItem.realItem);
			break;		
			
		case Item.eITEM_TYPE.EtcItem:
		case Item.eITEM_TYPE.UseItem:
			TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.left, m_ClickDownItemSlot.slotItem.realItem );
			break;
			
		default:
			TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.left, m_ClickDownItemSlot.slotItem.realItem );		
			break;
		}
	}
	
	private void OpenCosEquipItemToolTip( RealItem _item )
	{
		RealItem haveitem = ItemMgr.HadItemManagement.Inven.GetCosEquipItem( _item.item.ItemData.GetSubType() );
		if( null == haveitem )
		{
			TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.left, _item );
		}
		else
		{
			TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.left, haveitem, _item );
		}		
	}
	
	
	private void OpenEquipItemToolTip(RealItem _item)
	{
		RealItem haveitem = ItemMgr.HadItemManagement.Inven.GetEquipItem( _item.item.ItemData.GetSubType() );
		if( null == haveitem )
		{
			TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.left, _item );
		}
		else
		{
			TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.left, haveitem, _item );
		}	
	}
	#endregion		
	#region - slot operation -
	private void SetClickDownSlot( UIStorageSlot slot )
	{
		m_ClickDownItemSlot = slot;
		
		if( null != m_ClickDownItemSlot )
		{			
			AttachFocusImg( m_ClickDownItemSlot );
		}
		else
		{
			DetachFocusImg();
		}
	}
	
	public int GetStorageSlot( Ray inputRay, RealItem realItem )
	{
		foreach(GameObject line in lineLock)
		{
//			if(line.collider.bounds.IntersectRay( inputRay ) == true)
			if( true == AsUtil.PtInCollider( line.collider, inputRay))
				return -1;
		}
		
		foreach( UIStorageSlot slot in storageslots )
		{
			if( slot.IsIntersect( inputRay ) )
			{				
				if( null != slot.slotItem  )
				{
					if( slot.slotItem.realItem.item.ItemData.GetItemType() == realItem.item.ItemData.GetItemType() &&
						slot.slotItem.realItem.item.ItemData.GetSubType() == realItem.item.ItemData.GetSubType() )
						return slot.slotItem.realItem.getSlot;
				}	
				else
				{
					return slot.slotIndex;
				}
				break;
			}
		}	
		
		return -1;
	}
	
	public int GetSlotIndex(Ray _ray)
	{
		foreach(GameObject line in lineLock)
		{
//			if(line.collider.bounds.IntersectRay( _ray ) == true)
			if( true == AsUtil.PtInCollider( line.collider, _ray))
				return -1;
		}
		
		foreach( UIStorageSlot slot in storageslots )
		{
			if(slot.IsIntersect(_ray) == true)
			{
				return slot.slotIndex;
			}
		}
		
		return -1;
	}
	
	public void SetRestoreSlot()
	{
		if( null == m_ClickDownItemSlot || null == moveItemSlot.slotItem )
			return;
		
		if( null != m_ClickDownItemSlot.slotItem )
		{
			moveItemSlot.DeleteSlotItem();
			DetachFocusImg();	
//			return;
		}
		else
		{
			if(moveItemSlot.slotIndex / pageSlotCount == page.curPage)
			{
				PlayDropSound( moveItemSlot.slotItem.realItem.item );
				m_ClickDownItemSlot.SetSlotItem( moveItemSlot.slotItem );
				m_ClickDownItemSlot.ResetSlotItemPosition();
				ResetMoveSlot();
				DetachFocusImg();		
			}
			else
			{
				moveItemSlot.DeleteSlotItem();
				DetachFocusImg();
			}				
		}		
	}
	
	public void ResetSlotItmes()
	{
		if( null == ItemMgr.HadItemManagement )
		{
			Debug.LogError("UIStorageDlg::ResetSlotItmes() [ null == ItemMgr.HadItemManagement ] " );
			return;
		}	
		
		if( null == slotItemParent)
		{
			Debug.LogError("UIStorageDlg::ResetSlotItmes() [ null == slotItemParent ]");
			return;
		}
		
		if( null == storageslots )
		{
			Debug.LogError("UIStorageDlg::ResetSlotItmes() [ null == storageslots ]");
			return;
		}
				
		int iIndex = page.curPage * Storage.useStorageSlotNumInPage ;
		
		int iMoveSlotIndex = -1;
		DetachFocusImg();
		//
		if( null != moveItemSlot && null != moveItemSlot.slotItem )
		{
			iMoveSlotIndex = moveItemSlot.slotIndex;			
		}
		
		//
		foreach( UIStorageSlot slot in storageslots )
		{
			slot.SetSlotIndex( iIndex );
			slot.DeleteSlotItem();
			
			if( iIndex == iMoveSlotIndex )
			{
				AttachFocusImg( slot );
				++iIndex;
				continue;
			}
			
			StorageSlot _storageslot = ItemMgr.HadItemManagement.Storage.GetStorageSlotItem( iIndex );			
			if( null == _storageslot || null == _storageslot.realItem )
			{
				++ iIndex;
				continue;
			}
			
			slot.CreateSlotItem( _storageslot.realItem, slotItemParent.transform );	
			slot.SetMoveLock( _storageslot.isMoveLock );
			++ iIndex;
		}
		
		PageLock();
	}
	
	public void SetSlotItem( int iSlotIndex, RealItem realItem )
	{
		int iBeginSlotIndex = storageslots[0].slotIndex;
		int iIndex = iSlotIndex - iBeginSlotIndex;
		
		if( iIndex < 0 || iIndex >= storageslots.Length )
		{
			//Debug.LogError( " UIStorageDlg::SetSlotItem()[ iSlotIndex error ] iSlotIndex : " + iSlotIndex + " beginSlotIndex : " + 
			//	iBeginSlotIndex + " iIndex : " + iIndex  + " storageslots.Length : " + storageslots.Length );
			return;
		}
		
		storageslots[iIndex].DeleteSlotItem();		
		
		if( null != realItem )
		{			
			storageslots[iIndex].CreateSlotItem( realItem, slotItemParent.transform );
		}	
		
		DetachFocusImg();
		
		Debug.Log("SetSlotItem: iSlotIndex:" + iSlotIndex + ", realItem: " + realItem); 
	}
	
	public void SetSlotItem( int iSlotIndex )
	{
		if( m_iReallyRemoveItemSlotIndex == iSlotIndex  && null != m_curReallyRemoveItemDlg )	
		{
			m_curReallyRemoveItemDlg.Close();
			m_curReallyRemoveItemDlg = null;
			m_iReallyRemoveItemSlotIndex = 0;
			m_iReallyRemoveItemCount = 0;
		}
	}
	
	
	private void ClearSlot()
	{
		if( null == storageslots )
		{
			Debug.LogError("UIStorageDlg::ClearSlot() [ null == storageslots ]");
			return;
		}
		
		foreach( UIStorageSlot slot in storageslots )
		{			
			slot.DeleteSlotItem();
		}
	}
	
	public void SetWastebasket()
	{
		if( null == moveItemSlot.slotItem)
			return;
		
		if( moveItemSlot.slotItem.realItem.sItem.nOverlapped > 1 )
		{
			AsHudDlgMgr.Instance.invenDlg.OpenRemoveItemDlg(
				 moveItemSlot.slotIndex, moveItemSlot.slotItem.realItem.sItem.nOverlapped, UIRemoveItemDlg.eDLG_TYPE.STORAGE );
		}
		else
		{
			OpenReallyRemoveItemDlg( moveItemSlot.slotIndex, moveItemSlot.slotItem.realItem.sItem.nOverlapped );
		}
	}
	
	public void OpenReallyRemoveItemDlg(int islotIndex, int iCount )
	{
//		if( true == AsHudDlgMgr.Instance.IsDontMoveState)
//			return;
		

		m_iReallyRemoveItemSlotIndex = islotIndex;
		m_iReallyRemoveItemCount = iCount;
		m_curReallyRemoveItemDlg = 
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(1099), AsTableManager.Instance.GetTbl_String(6),
				this, "ReallyRemoveItem", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);
	}
	
	private void ReallyRemoveItem()
	{
		if( 0 == m_iReallyRemoveItemSlotIndex || 0 == m_iReallyRemoveItemCount )
				return;
		
		body_CS_STORAGE_MOVE move = new body_CS_STORAGE_MOVE(eSTORAGE_MOVE_TYPE.eSTORAGE_MOVE_TYPE_REMOVE,
			(short)m_iReallyRemoveItemSlotIndex, m_iReallyRemoveItemCount, 0);
		AsCommonSender.Send(move.ClassToPacketBytes());
			
		m_iReallyRemoveItemSlotIndex = 0;
		m_iReallyRemoveItemCount  = 0;
			
	}
	#endregion		
	#region - focus -
	public void AttachFocusImg( UIStorageSlot storageslot )
	{
		if( null == focusImg )
		{
			Debug.LogError("UIStorageDlg::AttachFocusImg() [ null == focusImg ]");
			return;
		}
		
		if( null == storageslot )
			return;
		
		
		focusImg.gameObject.active = true;
		Vector3 vec3SlotPos = storageslot.transform.position;
		vec3SlotPos.z -= 1.0f;
		focusImg.transform.position = vec3SlotPos;
	}
	
	public void DetachFocusImg()
	{
		if( null == focusImg )
		{
			Debug.LogError("UIStorageDlg::AttachFocusImg() [ null == focusImg ]");
			return;
		}
		
		focusImg.gameObject.active = false;
	}
	#endregion
	#region - page -
	public void NextPage()	
	{
		if( true == page.NextPage() )
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6004_EFF_Slide", Vector3.zero, false);
			ResetSlotItmes();
			
//			PageLock();
		}
	}
	
	public void PrePage()	
	{
		if( true == page.PrePage() )
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6004_EFF_Slide", Vector3.zero, false);
			ResetSlotItmes();
			
//			PageLock();
		}
	}
	
	public void PageLock()
	{
		if(page.curPage > ItemMgr.HadItemManagement.Storage.pageOpen)
		{
			for(int i=0; i<Storage.s_StorageLineCount; ++i)
			{
				lineLock[i].SetActiveRecursively(true);
			}
			
			m_PageLocked = true;
		}
		else if(page.curPage == ItemMgr.HadItemManagement.Storage.pageOpen)
		{
			int line = ItemMgr.HadItemManagement.Storage.curLineOpen;
//			if(line == 0) line = Storage.s_StorageLineCount;
			for(int i=0; i<Storage.s_StorageLineCount; ++i)
			{
				if(i < line)
					lineLock[i].SetActiveRecursively(false);
				else
					lineLock[i].SetActiveRecursively(true);
			}
			
			m_PageLocked = false;
		}
		else
		{
			for(int i=0; i<Storage.s_StorageLineCount; ++i)
			{
				lineLock[i].SetActiveRecursively(false);
			}
			
			m_PageLocked = false;
		}
		
//		ResetSlotItmes();
		
//		if(page.curPage > ItemMgr.HadItemManagement.Storage.pageOpen)
//		{
//			imgLock.SetActiveRecursively(true);
//			m_PageLocked = true;
//		}
//		else
//		{
//			imgLock.SetActiveRecursively(false);
//			m_PageLocked = false;
//		}
	}
	
	public bool CheckPageOpened()
	{
		if(page.curPage > ItemMgr.HadItemManagement.Storage.pageOpen)
			return false;
		else
			return true;
	}
	
	public void PlayLineEffect(int _line)
	{
		effectOpen[(_line + 4) % 5].SetActiveRecursively(false);
		effectOpen[(_line + 4) % 5].SetActiveRecursively(true);
	}
	#endregion	
	#region - move lock -
	public void SetSlotMoveLock( int iSlot, bool bLock )
	{
		StorageSlot _storageslot = ItemMgr.HadItemManagement.Storage.GetStorageSlotItem( iSlot );
		if( null != _storageslot ) 
			_storageslot.SetMoveLock(bLock);		
	}

    public void ResetSlotMoveLock()
    {
        ItemMgr.HadItemManagement.Storage.ResetStorageSlotMoveLock();
    }
	
	public void ApplySlotMoveLock()
	{				
		int iIndex = page.curPage * Storage.useStorageSlotNumInPage ;			
		
		foreach( UIStorageSlot slot in storageslots )
		{			
			StorageSlot _storageslot = ItemMgr.HadItemManagement.Storage.GetStorageSlotItem( iIndex );			
			if( null == _storageslot )
			{
				++ iIndex;
				continue;
			}			
			
			slot.SetMoveLock( _storageslot.isMoveLock );
			++ iIndex;
		}
	}
	#endregion
	#region - msg box -
//	void OnMsgBox_SkillReset_Ok()
//	{
//		m_SkillResetMessageBox.Close();
//		
//		if( false == SkillBook.Instance.CheckSkillBookReset())
//		{
//			m_ClickDownItemSlot.slotItem.realItem.SendUseItem();
//		}
//		else
//		{
//			string strTitle = AsTableManager.Instance.GetTbl_String( 123);
//			string strMsg = AsTableManager.Instance.GetTbl_String( 120);
//			m_SkillResetMessageBox = AsNotify.Instance.MessageBox( strTitle, strMsg, this, "OnMsgBox_SkillReset_Err", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
//		}
//	}
//	
//	void OnMsgBox_SkillReset_Cancel()
//	{
//		m_SkillResetMessageBox.Close();
//	}
//	
//	void OnMsgBox_SkillReset_Err()
//	{
//		m_SkillResetMessageBox.Close();
//	}
//	
//	void OnMsgBox_SkillReset_Succeeded()
//	{
//		m_SkillResetMessageBox.Close();
//	}
//	
//	public void OpenSkillResetSucceeded()
//	{
//		string strTitle = AsTableManager.Instance.GetTbl_String( 123);
//		string strMsg = AsTableManager.Instance.GetTbl_String( 122);
//		m_SkillResetMessageBox = AsNotify.Instance.MessageBox( strTitle, strMsg, this, "OnMsgBox_SkillReset_Succeeded", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
//	}
	#endregion
}
