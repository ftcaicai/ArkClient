using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

public enum ePostBoxDlgState
{
	Invalid,
	ListTab,
	ReadTab,
	WriteTab
};

public class AsPostBoxDlg : MonoBehaviour
{
	public delegate void CloseDelegate();
	public CloseDelegate closeDel;

	public GameObject listTab = null;
	public GameObject readTab = null;
	public GameObject writeTab = null;
	
	[SerializeField] UIPanelTab tabReceive = null;
	[SerializeField] UIPanelTab tabWrite = null;

	//	List
	public UIScrollList list = null;
	public GameObject itemPrefab = null;
	public int DRAG_SENSITIVITY = 50;
	public SpriteText textTitle = null;
	public SpriteText noMail = null;
	private const int MAIL_LIST_MAX_PAGE = 10;
	private int curPage = 0;
	private bool isTouched = false;
	private Vector2 prevPos = Vector2.zero;
	private List<UInt64> selectedPostSerials = new List<UInt64>();
	[SerializeField] UIButton btnAll = null;
	[SerializeField] UIButton btnReceive = null;
	[SerializeField] UIButton btnDel = null;
	[SerializeField] SpriteText txtPage = null;
	bool allSelect = false;

	//	Read
	public SpriteText recvFrom = null;
	public SpriteText recvTitle = null;
	public UIButton recvGold = null;
	public UIButton btnReply = null;
	public UIButton btnTakeAll = null;
	public SpriteText textReadFrom = null;
	public SpriteText textReadTitle = null;
	public SpriteText textReadReply = null;
	public SpriteText textReadTakeAll = null;
	public UIScrollList scrollListReadContent = null;
	public GameObject listItemReadContent = null;

	public AsSlot[] recvItemSlots = null;
	private body2_SC_POST_LIST_RESULT readMailInfo = null;

	//	Send
	public UITextField sendTo = null;
	[SerializeField] UIButton searchBtn = null;
	public UITextField sendTitle = null;
	public UITextField sendContent = null;
	public UITextField sendGold = null;
	public SpriteText textSendFrom = null;
	public SpriteText textSendTitle = null;
	public SpriteText textSend = null;
	public SpriteText textSendCost = null;
	public AsSlot[] sendItemSlots = null;
	[SerializeField]AsPostReceiverCandidateDlg candidateDlg = null;

	private AsSlot m_curSlotUseOpenDlg = null;
	private ePostBoxDlgState state = ePostBoxDlgState.ListTab;
	public ePostBoxDlgState TabState
	{
		set	{ state = value; }
		get	{ return state; }
	}

	// begin kij move icon
	public float m_fMaxItemMoveTime = 0.5f;
	private float m_fItemMoveTime = 0.0f;
	private GameObject m_goMoveIcon;
	private AsSlot m_ClickDownSlot;
	// end kij

	private AsMessageBox m_msgboxItem = null;
	private AsSlot m_slotBuf;
	private byte m_slotIndexBuf;
	public SimpleSprite mailBubble = null;

	public float Depth
	{
		set
		{
			gameObject.transform.position = new Vector3( gameObject.transform.position.x, gameObject.transform.position.y, value);
		}
	}

	// Use this for initialization
	void Start()
	{
		AsLanguageManager.Instance.SetFontFromSystemLanguage( textTitle);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( recvFrom);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( recvTitle);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( textReadFrom);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( textReadTitle);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( textReadReply);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( textReadTakeAll);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( sendTo);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( sendTitle);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( sendContent);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( textSendFrom);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( textSendTitle);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( textSend);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( textSendCost);
		sendGold.SetValidationDelegate( SendGoldValidator);
		sendTitle.SetValidationDelegate( SendTitleValidator);
		sendContent.SetValidationDelegate( sendContentValidator);
		
		tabReceive.Text = AsTableManager.Instance.GetTbl_String(34);
		tabWrite.Text = AsTableManager.Instance.GetTbl_String(1138);
		textTitle.Text = AsTableManager.Instance.GetTbl_String(1392);
		textReadFrom.Text = AsTableManager.Instance.GetTbl_String(1143);
		textReadTitle.Text = AsTableManager.Instance.GetTbl_String(1144);
		textReadReply.Text = AsTableManager.Instance.GetTbl_String(1139);
		textReadTakeAll.Text = AsTableManager.Instance.GetTbl_String(857);
		textSendFrom.Text = AsTableManager.Instance.GetTbl_String(1142);
		textSendTitle.Text = AsTableManager.Instance.GetTbl_String(1144);
		textSend.Text = AsTableManager.Instance.GetTbl_String(1141);
		textSendCost.Text = _GetPostCost().ToString( "#,#0", CultureInfo.InvariantCulture);
		noMail.Text = AsTableManager.Instance.GetTbl_String(1463);
		searchBtn.Text = AsTableManager.Instance.GetTbl_String(1148);
		btnAll.Text = AsTableManager.Instance.GetTbl_String(35);
		btnReceive.Text = AsTableManager.Instance.GetTbl_String(36);
		btnDel.Text = AsTableManager.Instance.GetTbl_String(37);
		
		StringBuilder sb = new StringBuilder();
		sb.AppendFormat( "1/{0}", MAIL_LIST_MAX_PAGE);
		txtPage.Text = sb.ToString();

		noMail.gameObject.SetActiveRecursively( false);
		candidateDlg.SetReceiverDelegate = SetMailSender;
		
		AsUtil.SetButtonState( btnReceive , UIButton.CONTROL_STATE.DISABLED );
		AsUtil.SetButtonState( btnDel , UIButton.CONTROL_STATE.DISABLED );
	}

	string SendTitleValidator( UITextField field, string text, ref int insPos)
	{
		while( true)
		{
			int length = System.Text.UTF8Encoding.UTF8.GetByteCount( text);
			if( length <= AsGameDefine.MAX_MAILT_TITLE_LENGTH)
				break;

			text = text.Remove( text.Length - 1);
		}
		// #22671
		int index =  text.IndexOf('\'');
		if(-1 != index)
			text = text.Remove( index);	
		
		return text;
	
	}

	string sendContentValidator( UITextField field, string text, ref int insPos)
	{		
		// #22671
		int index =  text.IndexOf('\'');
		if(-1 != index)
			text = text.Remove( index);	
		
		return text;	
	}
	

	string SendGoldValidator( UITextField field, string text, ref int insPos)
	{
		return Regex.Replace( text, "[^0-9]", "");
	}

	void UpdateBubble()
	{
		AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
		if( null == userEntity)
			return;

		Vector3 screenPos = Vector3.zero;
		Transform dummyLeadTop = userEntity.GetDummyTransform( "DummyLeadTop");
		if( null != dummyLeadTop)
			screenPos = CameraMgr.Instance.WorldToScreenPoint( dummyLeadTop.position);
		else
		{
			if( true == userEntity.isKeepDummyObj)
			{
				Vector3 vPos = userEntity.transform.position;
				vPos.y += userEntity.characterController.height;
				screenPos = CameraMgr.Instance.WorldToScreenPoint( vPos);
			}
		}

		Vector3 vRes = CameraMgr.Instance.ScreenPointToUIRay( screenPos);
		vRes.z = 9.0f;//-2.0f;
		mailBubble.transform.position =  vRes;
	}

	// Update is called once per frame
	void Update()
	{
		UpdateBubble();
	}

	public void Open( ePostBoxDlgState state=ePostBoxDlgState.ListTab)
	{
		RequestMailList();

		gameObject.SetActiveRecursively( true);
		TabChange( state);

		ItemMgr.HadItemManagement.Inven.ResetInvenSlotMoveLock();
		m_curSlotUseOpenDlg = null;
	}

	public void OnClickClose()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		Close();
	}

	public void Close()
	{
#if false
		switch( state)
		{
		case ePostBoxDlgState.ListTab:
			closeDel();
			break;
		case ePostBoxDlgState.ReadTab:
			TabChange( ePostBoxDlgState.ListTab);
			break;
		case ePostBoxDlgState.WriteTab:
			TabChange( ePostBoxDlgState.ListTab);
			break;
		}
#endif

		ItemMgr.HadItemManagement.Inven.ResetInvenSlotMoveLock();
		if( true == AsHudDlgMgr.Instance.IsOpenInven)
			AsHudDlgMgr.Instance.invenDlg.ApplySlotMoveLock();

		if( true == _isOpenMsgBox())
			m_msgboxItem.Close();

		AsCommonSender.isSendPostList = false;
		
		closeDel();
	}

	public void CloseAll()
	{
		Close();
		gameObject.SetActiveRecursively( false);
	}

	public void ClearMailList()
	{
		list.ClearList( true);
		noMail.gameObject.SetActiveRecursively( true);
		btnAll.Text = AsTableManager.Instance.GetTbl_String(35);
		AsUtil.SetButtonState( btnReceive , UIButton.CONTROL_STATE.DISABLED );
		AsUtil.SetButtonState( btnDel , UIButton.CONTROL_STATE.DISABLED );
	}

	public void InsertMailInfo( body2_SC_POST_LIST_RESULT[] infos)
	{
		for( int i = 0; i < infos.Length; i++)
		{
			body2_SC_POST_LIST_RESULT info = infos[i];
			
			UIListItemContainer itemContainer = list.CreateItem( itemPrefab) as UIListItemContainer;
			AsPostBoxListItem listItem = itemContainer.gameObject.GetComponent<AsPostBoxListItem>();
			listItem.Parent = this;
			listItem.SetInfo( info, _GetPostTitle( info, true));
			listItem.CheckCallback = PostItemCheckCallback;
	
			if( ePostBoxDlgState.ListTab != state)
				listItem.gameObject.SetActiveRecursively( false);
	
			noMail.gameObject.SetActiveRecursively( false);
		}
		
		_initListPanel();
	}
	
#if false
	public void WriteMail()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		TabChange( ePostBoxDlgState.WriteTab);
	}
#endif
	
	void OnReceiveTab()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		TabChange( ePostBoxDlgState.ListTab);
		
		_initListPanel();
	}
	
	void OnWriteTab()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		TabChange( ePostBoxDlgState.WriteTab);
	}

	public void TabChange( ePostBoxDlgState state)
	{
		candidateDlg.gameObject.SetActiveRecursively( false);

		this.state = state;

		switch( state)
		{
		case ePostBoxDlgState.ListTab:
			tabReceive.Value = true;
			tabWrite.Value = false;
			InitListTab();
			break;
		case ePostBoxDlgState.ReadTab:
			InitReadTab();
			break;
		case ePostBoxDlgState.WriteTab:
			tabReceive.Value = false;
			tabWrite.Value = true;
			InitWriteTab();
			break;
		}
	}

	private void InitListTab()
	{
		listTab.SetActiveRecursively( true);
		readTab.SetActiveRecursively( false);
		writeTab.SetActiveRecursively( false);
		readMailInfo = null;

		recvItemSlots[0].SetEmpty();
		recvItemSlots[1].SetEmpty();
		recvItemSlots[2].SetEmpty();
		recvItemSlots[3].SetEmpty();
		sendItemSlots[0].SetEmpty();
		sendItemSlots[1].SetEmpty();
		sendItemSlots[2].SetEmpty();
		sendItemSlots[3].SetEmpty();
		
		ItemMgr.HadItemManagement.Inven.ResetInvenSlotMoveLock();
		if( true == AsHudDlgMgr.Instance.IsOpenInven)
			AsHudDlgMgr.Instance.invenDlg.ApplySlotMoveLock();

		for( int i = 0; i < list.Count; i++)
		{
			UIListItemContainer itemContainer = list.GetItem(i) as UIListItemContainer;
			AsPostBoxListItem listItem = itemContainer.gameObject.GetComponent<AsPostBoxListItem>();
			listItem.RefreshInfo();
			listItem.CheckCallback = PostItemCheckCallback;
		}

		noMail.gameObject.SetActiveRecursively( 0 == list.Count);
		
		foreach( AsSlot _slot in recvItemSlots )
		{
			_slot.SetType( AsSlot.SLOT_TYPE.SLT_IT_POST );
		}
		foreach( AsSlot _slot in sendItemSlots )
		{
			_slot.SetType( AsSlot.SLOT_TYPE.SLT_IT_POST );
		}
	}

	private void InitReadTab()
	{
		listTab.SetActiveRecursively( false);
		readTab.SetActiveRecursively( true);
		writeTab.SetActiveRecursively( false);
		noMail.gameObject.SetActiveRecursively( false);

		recvGold.gameObject.SetActiveRecursively( false);
	}

	private void InitWriteTab()
	{
		Debug.Log( "InitWriteTab");
		listTab.SetActiveRecursively( false);
		readTab.SetActiveRecursively( false);
		writeTab.SetActiveRecursively( true);
		noMail.gameObject.SetActiveRecursively( false);
		sendTo.Text = "";
		sendTitle.Text = "";
		sendContent.Text = "";
		sendGold.Text = "";
		readMailInfo = null;
		textSendCost.Text = _GetPostCost().ToString( "#,#0", CultureInfo.InvariantCulture);

		sendGold.gameObject.SetActiveRecursively( false);
	}

	public void GuiInputDown( Ray inputRay)
	{
		if( true == _isOpenMsgBox())
			return;

		if( null!= m_ClickDownSlot && null != m_goMoveIcon)
		{
			m_goMoveIcon.transform.position = new Vector3( m_ClickDownSlot.transform.position.x ,
				m_ClickDownSlot.transform.position.y,
				m_ClickDownSlot.transform.position.z - 0.5f);
		}
		m_ClickDownSlot = null;
		m_goMoveIcon = null;
		m_fItemMoveTime = 0.0f;

		foreach( AsSlot _slot in recvItemSlots)
		{
//			if( true == _slot.collider.bounds.IntersectRay( inputRay))
			if( true == AsUtil.PtInCollider( _slot.collider, inputRay))
			{
				if( 0 != _slot.getItemID)
				{
					AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
					m_ClickDownSlot = _slot;
				}

				return;
			}
		}

		foreach( AsSlot _slot in sendItemSlots)
		{
//			if( true == _slot.collider.bounds.IntersectRay( inputRay))
			if( true == AsUtil.PtInCollider( _slot.collider, inputRay))
			{
				if( 0 != _slot.getItemID)
				{
					AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
					m_ClickDownSlot = _slot;
				}

				return;
			}
		}
	}

	public void InputMove(Ray inputRay)
	{
		if( true == _isOpenMsgBox())
			return;

		if( null != m_goMoveIcon)
		{
			Vector3 vec3Temp = inputRay.origin;
			vec3Temp.z = -10;

			m_goMoveIcon.transform.position = vec3Temp;
		}
	}

	public void InputUp( Ray inputRay)
	{
		if( true == _isOpenMsgBox())
			return;

		if( null != m_ClickDownSlot && null != m_goMoveIcon)
		{
			RaycastHit hit;
			if(Physics.Raycast(inputRay, out hit, 10000f, 1<<LayerMask.NameToLayer("GUI")) == false)
			{
				m_goMoveIcon.transform.position = new Vector3( m_ClickDownSlot.transform.position.x ,
					m_ClickDownSlot.transform.position.y,
					m_ClickDownSlot.transform.position.z - 0.5f);
				m_ClickDownSlot = null;
				m_goMoveIcon = null;
			}
		}
	}

	public void GuiInputMove( Ray inputRay)
	{
		if( true == _isOpenMsgBox())
			return;

		if( null == m_goMoveIcon && null != m_ClickDownSlot)
		{
			if( m_fMaxItemMoveTime <= m_fItemMoveTime)
			{
				//m_goMoveIcon = m_ClickDownSlot.getIcon;
				m_fItemMoveTime = 0.0f;
				GuiInputDClickUp( inputRay);
			}

			m_fItemMoveTime += Time.deltaTime;
		}
	}

	public void GuiInputDClickUp(Ray inputRay)
	{
		if( true == _isOpenMsgBox())
			return;

		if( ePostBoxDlgState.WriteTab == state)
		{
			foreach( AsSlot _slot in sendItemSlots)
			{
				if( true == AsUtil.PtInCollider( _slot.collider, inputRay))
				{
					if( true == _slot.IsEnableItem)
					{
						if( -1 == ItemMgr.HadItemManagement.Inven.GetEmptyInvenSlot())
						{
							AsMessageBox msgBox = AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(103), AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
							msgBox.SetOkText( AsTableManager.Instance.GetTbl_String(1152));
						}
						else
						{
							_AppendItemCancel( _slot);
							textSendCost.Text = _GetPostCost().ToString( "#,#0", CultureInfo.InvariantCulture);
						}
					}

					return;
				}
			}
		}
		else if( ePostBoxDlgState.ReadTab == state)
		{
			byte slotIndex = 0;
			foreach( AsSlot _slot in recvItemSlots)
			{
				if( true == AsUtil.PtInCollider( _slot.collider, inputRay))
				{
					if( 0 != _slot.getItemID)
					{
						if( -1 == ItemMgr.HadItemManagement.Inven.GetEmptyInvenSlot())
						{
							AsMessageBox msgBox = AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(103), AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
							msgBox.SetOkText( AsTableManager.Instance.GetTbl_String(1152));
						}
						else
						{
							_AppendItemGet( _slot, slotIndex);
						}
					}

					return;
				}

				slotIndex++;
			}
		}
	}

	public void GuiInputUp(Ray inputRay)
	{
		if( true == _isOpenMsgBox())
			return;

		if( null != m_ClickDownSlot && null == m_goMoveIcon)
		{
			foreach( AsSlot _slot in sendItemSlots)
			{
				if( true == AsUtil.PtInCollider( _slot.collider, inputRay))
				{
					if( true == _slot.IsEnableItem)
					{
						TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.left, _slot.getRealItem);
						return;
					}
				}
			}

			foreach( AsSlot _slot in recvItemSlots)
			{
				if( true == AsUtil.PtInCollider( _slot.collider, inputRay))
				{
					if( null != _slot.getSItem)
					{
						TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.left, _slot.getSItem, 0f);
						return;
					}
				}
			}
		}
		else if( null != m_ClickDownSlot && null != m_goMoveIcon)
		{
			if( TabState == ePostBoxDlgState.ReadTab)
			{
				m_goMoveIcon.transform.position = new Vector3( 	m_ClickDownSlot.transform.position.x ,
																m_ClickDownSlot.transform.position.y,
																m_ClickDownSlot.transform.position.z - 0.5f);
			}
			else if( true == AsHudDlgMgr.Instance.IsOpenInven && true == AsHudDlgMgr.Instance.invenDlg.SetPostBoxToInven( m_ClickDownSlot, inputRay))
			{
				m_ClickDownSlot.SetEmpty();
				textSendCost.Text = _GetPostCost().ToString( "#,#0", CultureInfo.InvariantCulture);
			}
			else
			{
				m_goMoveIcon.transform.position = new Vector3( 	m_ClickDownSlot.transform.position.x ,
																m_ClickDownSlot.transform.position.y,
																m_ClickDownSlot.transform.position.z - 0.5f);
			}

			m_goMoveIcon = null;
			m_ClickDownSlot = null;
		}
	}

	public bool SetMoveInvenSlotInItemSlot( Ray inputRay, UIInvenSlot _InvenSlot)
	{
		if( null == _InvenSlot || null == _InvenSlot.slotItem || TabState != ePostBoxDlgState.WriteTab)
			return false;

		foreach( AsSlot _slot in sendItemSlots)
		{
			if( true == AsUtil.PtInCollider( _slot.collider, inputRay))
			{
				SetSlotItem( _slot, _InvenSlot);
				return true;
			}
		}

		return false;
	}

	public void SetDClickSlotItem( UIInvenSlot _InvenSlot)
	{
		if( null == _InvenSlot || null == _InvenSlot.slotItem || TabState != ePostBoxDlgState.WriteTab)
			return;

		foreach( AsSlot _slot in sendItemSlots)
		{
			if( false == _slot.IsEnableItem)
			{
				SetSlotItem( _slot, _InvenSlot);
				return;
			}
		}
	}

	public void SetUseOpenDlgItemSlot( int iSlot, int iCount)
	{
		if( null == m_curSlotUseOpenDlg)
			return;

		RealItem _realItem = ItemMgr.HadItemManagement.Inven.GetRealItemInSlot( iSlot);
		if( null == _realItem)
		{
			Debug.LogError("AsPostBoxDlg::SetUserOpenDlgItemSlot() [ null == realItem ] slot : " + iSlot);
			return;
		}

		if( true == m_curSlotUseOpenDlg.IsEnableItem )
		{
			ItemMgr.HadItemManagement.Inven.SetSlotMoveLock( m_curSlotUseOpenDlg.getRealItem.getSlot, false);
		}
		
		m_curSlotUseOpenDlg.SetType( AsSlot.SLOT_TYPE.SLT_IT_POST );
		m_curSlotUseOpenDlg.SetItem( _realItem, iCount);


		ItemMgr.HadItemManagement.Inven.SetSlotMoveLock(iSlot, true);
		if (true == AsHudDlgMgr.Instance.IsOpenInven)
			AsHudDlgMgr.Instance.invenDlg.ApplySlotMoveLock();

		textSendCost.Text = _GetPostCost().ToString( "#,#0", CultureInfo.InvariantCulture);
		AsSoundManager.Instance.PlaySound( _realItem.item.ItemData.getStrDropSound, Vector3.zero, false);
	}

	private void SetSlotItem( AsSlot _slot, UIInvenSlot _InvenSlot)
	{
		if( true == _InvenSlot.slotItem.realItem.item.ItemData.isTradeLimit ||
			true == _InvenSlot.slotItem.realItem.item.ItemData.isDump ||
			false == _InvenSlot.slotItem.realItem.sItem.IsTradeEnable() )
		{
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(109), AsTableManager.Instance.GetTbl_String(29),
				null, null, AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			return;
		}

		if( 1 < _InvenSlot.slotItem.realItem.sItem.nOverlapped)
		{
			if( true == AsHudDlgMgr.Instance.IsOpenInven)
			{
				m_curSlotUseOpenDlg = _slot;
				AsHudDlgMgr.Instance.invenDlg.OpenRemoveItemDlg( _InvenSlot.slotItem.realItem.getSlot, _InvenSlot.slotItem.realItem.sItem.nOverlapped,
					UIRemoveItemDlg.eDLG_TYPE.POSTBOX);
			}
			return;
		}
		else
		{
			if( true == _slot.IsEnableItem )
			{
				ItemMgr.HadItemManagement.Inven.SetSlotMoveLock( _slot.getRealItem.getSlot, false);
			}
			
			_slot.SetType( AsSlot.SLOT_TYPE.SLT_IT_POST );
			_slot.SetItem( _InvenSlot.slotItem.realItem, 1);
			ItemMgr.HadItemManagement.Inven.SetSlotMoveLock( _InvenSlot.slotItem.realItem.getSlot, true);

			textSendCost.Text = _GetPostCost().ToString( "#,#0", CultureInfo.InvariantCulture);
			AsSoundManager.Instance.PlaySound( _InvenSlot.slotItem.realItem.item.ItemData.getStrDropSound, Vector3.zero, false);
			return;
		}
	}

	void OnFriendSearchBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		Vector3 dlgPos = candidateDlg.gameObject.transform.position;
		candidateDlg.gameObject.transform.position = new Vector3( dlgPos.x, dlgPos.y, gameObject.transform.position.z - 3.0f);
		candidateDlg.Show();

		body_CS_POST_ADDRESS_BOOK addressBook = new body_CS_POST_ADDRESS_BOOK( ePOST_ADDRESS_BOOK_TYPE.ePOST_ADDRESS_BOOK_TYPE_FRIEND, 0, false);
		byte[] packet = addressBook.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( packet);
	}

	public void InsertAddress( body1_SC_POST_ADDRESS_BOOK addressBook)
	{
		candidateDlg.InsertAddress( addressBook);
	}

	public void ClearAddress()
	{
		candidateDlg.ClearAddress();
	}

	private void OnMailSend()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		// filtering
		if( false == AsTableManager.Instance.TextFiltering_Post( sendTitle.Text) || false == AsTableManager.Instance.TextFiltering_Post( sendContent.Text))
		{
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(109), AsTableManager.Instance.GetTbl_String(364),
				null, null, AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			return;
		}

		// check gold
		ulong uiGold = 0;
		if( sendGold.Text.Length > 0)
			uiGold = System.UInt64.Parse( sendGold.Text);

		if( AsUserInfo.Instance.SavedCharStat.nGold < ( (ulong)_GetPostCost() + uiGold))
		{
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(1575), AsTableManager.Instance.GetTbl_String(97),
				null, null, AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			return;
		}

		if( 0 == sendTo.Text.Length)
		{
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(109), AsTableManager.Instance.GetTbl_String(92),
				null, null, AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			return;
		}

		if( 0 == sendTitle.Text.Length)
		{
			if( 0 == sendContent.Text.Length)
			{
				AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(109), AsTableManager.Instance.GetTbl_String(93),
					null, null, AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
				return;
			}
			else
			{
				sendTitle.Text = AsTableManager.Instance.GetTbl_String(1993);
			}
		}

		List<sPOSTITEM> items = new List<sPOSTITEM>();
		for( int i = 0; i < (int)ePOST_COMMON.ePOST_MAX_ITEM; i++)
		{
			sPOSTITEM tempItem = new sPOSTITEM();
			if( null == sendItemSlots[i].getRealItem)
			{
				tempItem.nInentorySlot = 0;
				tempItem.nOverlapped = 0;
			}
			else
			{
				tempItem.nInentorySlot = sendItemSlots[i].getRealItem.getSlot;
				tempItem.nOverlapped = sendItemSlots[i].getItemCount;
			}
			items.Add( tempItem);
		}

		string strSendTo = sendTo.Text;

		while( _GetStringSize( strSendTo) > 24)
		{
			strSendTo = strSendTo.Substring( 0, strSendTo.Length - 1);
		}

		string strSendTitle = sendTitle.Text;

		while( _GetStringSize( strSendTitle) > 24)
		{
			strSendTitle = strSendTitle.Substring( 0, strSendTitle.Length - 1);
		}

		body_CS_POST_SEND postSend = new body_CS_POST_SEND( strSendTo, strSendTitle, sendContent.Text, uiGold, items);
		byte[] data = postSend.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
		ItemMgr.HadItemManagement.Inven.ResetInvenSlotMoveLock();
		if( true == AsHudDlgMgr.Instance.IsOpenInven )
        {
            AsHudDlgMgr.Instance.invenDlg.ApplySlotMoveLock();
        }
		foreach( AsSlot slot in sendItemSlots)
			slot.SetEmpty();
		
		TabChange( ePostBoxDlgState.ListTab);
	}
	
	private void RequestMailList()
	{
		curPage = 0;

		body_CS_POST_LIST postList = new body_CS_POST_LIST();
		byte[] data = postList.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
		AsCommonSender.isSendPostList = true;
	}

	public void RequestNextPageMailList()
	{
		body_CS_POST_PAGE postPage = new body_CS_POST_PAGE( curPage);
		byte[] data = postPage.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}

	public void SetMailDetailInfo( body2_SC_POST_LIST_RESULT info)
	{
		if( ePOSTTYPE.ePOSTTYPE_NOTHING == (ePOSTTYPE)(info.nPostType))
		{
			btnReply.SetControlState( UIButton.CONTROL_STATE.NORMAL);
			btnReply.spriteText.Color = Color.black;
		}
		else
		{
			btnReply.SetControlState( UIButton.CONTROL_STATE.DISABLED);
			btnReply.spriteText.Color = Color.gray;
		}

		recvFrom.Text = _GetPostSenderName( info);
		recvTitle.Text = _GetPostTitle( info, false);
		recvGold.Text = info.nGold.ToString( "#,#0", CultureInfo.InvariantCulture);

		if( scrollListReadContent.Count > 0)
			scrollListReadContent.RemoveItem( 0, true);

		scrollListReadContent.CreateItem( listItemReadContent, _GetPostContent( info));

		if( 0 != info.sRecievItem1.nOverlapped)
			recvItemSlots[0].SetItem( info.sRecievItem1.nItemTableIdx, info.sRecievItem1.nOverlapped, info.sRecievItem1);
		if( 0 != info.sRecievItem2.nOverlapped)
			recvItemSlots[1].SetItem( info.sRecievItem2.nItemTableIdx, info.sRecievItem2.nOverlapped, info.sRecievItem2);
		if( 0 != info.sRecievItem3.nOverlapped)
			recvItemSlots[2].SetItem( info.sRecievItem3.nItemTableIdx, info.sRecievItem3.nOverlapped, info.sRecievItem3);
		if( 0 != info.sRecievItem4.nOverlapped)
			recvItemSlots[3].SetItem( info.sRecievItem4.nItemTableIdx, info.sRecievItem4.nOverlapped, info.sRecievItem4);

		readMailInfo = info;

		if( true == _isGetItemInReadMailInfo())
		{
			btnTakeAll.SetControlState( UIButton.CONTROL_STATE.NORMAL);
			btnTakeAll.spriteText.Color = new Color(0.1f,0.4f,1.0f,1.0f);
		}
		else
		{
			btnTakeAll.SetControlState( UIButton.CONTROL_STATE.DISABLED);
			btnTakeAll.spriteText.Color = Color.gray;
		}
	}
	
	private void PostItemCheckCallback( bool isChecked, UInt64 serial)
	{
		if( true == isChecked)
		{
			if( false == selectedPostSerials.Contains( serial))
				selectedPostSerials.Add( serial);
		}
		else
		{
			if( true == selectedPostSerials.Contains( serial))
				selectedPostSerials.Remove( serial);
		}
		
		allSelect = ( list.Count != selectedPostSerials.Count) ? false : true;
		if( true == allSelect)
			btnAll.Text = AsTableManager.Instance.GetTbl_String(40);
		else
			btnAll.Text = AsTableManager.Instance.GetTbl_String(35);
		
		if( 0 == selectedPostSerials.Count)
		{
			AsUtil.SetButtonState( btnReceive , UIButton.CONTROL_STATE.DISABLED );
			AsUtil.SetButtonState( btnDel , UIButton.CONTROL_STATE.DISABLED );
		}
		else
		{
			bool itemHas = false;
			for( int i = 0; i < list.Count; i++)
			{
				UIListItemContainer itemContainer = list.GetItem(i) as UIListItemContainer;
				AsPostBoxListItem listItem = itemContainer.gameObject.GetComponent<AsPostBoxListItem>();
				body2_SC_POST_LIST_RESULT postInfo = listItem.Info;
				if( true == selectedPostSerials.Contains( postInfo.nPostSerial))
				{
					bool showHasItem = ( 0 != postInfo.sRecievItem1.nOverlapped) || ( 0 != postInfo.sRecievItem2.nOverlapped) || ( 0 != postInfo.sRecievItem3.nOverlapped) || ( 0 != postInfo.sRecievItem4.nOverlapped);
					itemHas |= showHasItem;
				}
			}
			
			if( true == itemHas)
				AsUtil.SetButtonState( btnReceive , UIButton.CONTROL_STATE.NORMAL );
			else
				AsUtil.SetButtonState( btnReceive , UIButton.CONTROL_STATE.DISABLED );

			AsUtil.SetButtonState( btnDel , UIButton.CONTROL_STATE.NORMAL );
		}
	}
	
	void _deleteSelectedPos()
	{
		int count = selectedPostSerials.Count;
		
		if( 0 == count)
			return;
		
		List<body2_CS_POST_DELETE> bodies = new List<body2_CS_POST_DELETE>();
		for( int i = 0; i < count; i++)
		{
			bodies.Add( new body2_CS_POST_DELETE( selectedPostSerials[i]));
		}
		
		body1_CS_POST_DELETE postDelete = new body1_CS_POST_DELETE( curPage, bodies.ToArray());
		byte[] data = postDelete.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	
		for( int i = 0; i < list.Count; )
		{
			UIListItemContainer itemContainer = list.GetItem(i) as UIListItemContainer;
			AsPostBoxListItem listItem = itemContainer.gameObject.GetComponent<AsPostBoxListItem>();
			if( true == selectedPostSerials.Contains( listItem.Info.nPostSerial))
			{
				list.RemoveItem( itemContainer, true);
				continue;
			}
			
			i++;
		}

		noMail.gameObject.SetActiveRecursively( 0 == list.Count);
		
		selectedPostSerials.Clear();
	}
	
	private void OnSelectedDeleteBtn()
	{
		int count = selectedPostSerials.Count;
		
		if( 0 == count)
			return;
		
		AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(109), AsTableManager.Instance.GetTbl_String(41), this, "_deleteSelectedPos", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_WARNING);
#if false
		List<body2_CS_POST_DELETE> bodies = new List<body2_CS_POST_DELETE>();
		for( int i = 0; i < count; i++)
		{
			bodies.Add( new body2_CS_POST_DELETE( selectedPostSerials[i]));
		}
		
		body1_CS_POST_DELETE postDelete = new body1_CS_POST_DELETE( curPage, bodies.ToArray());
		byte[] data = postDelete.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
		
		for( int i = 0; i < list.Count; )
		{
			UIListItemContainer itemContainer = list.GetItem(i) as UIListItemContainer;
			AsPostBoxListItem listItem = itemContainer.gameObject.GetComponent<AsPostBoxListItem>();
			if( true == selectedPostSerials.Contains( listItem.Info.nPostSerial))
			{
				list.RemoveItem( itemContainer, true);
				continue;
			}
			
			i++;
		}
		
		selectedPostSerials.Clear();
#endif
	}
	
	private void OnAllSelectBtn()
	{
		allSelect = !allSelect;
		
		selectedPostSerials.Clear();
		
		for( int i = 0; i < list.Count; i++)
		{
			UIListItemContainer itemContainer = list.GetItem(i) as UIListItemContainer;
			AsPostBoxListItem listItem = itemContainer.gameObject.GetComponent<AsPostBoxListItem>();
			listItem.Check( allSelect);
			if( true == allSelect)
				selectedPostSerials.Add( listItem.Info.nPostSerial);
		}
		
		if( true == allSelect)
		{
			btnAll.Text = AsTableManager.Instance.GetTbl_String(40);
			bool itemHas = false;
			for( int i = 0; i < list.Count; i++)
			{
				UIListItemContainer itemContainer = list.GetItem(i) as UIListItemContainer;
				AsPostBoxListItem listItem = itemContainer.gameObject.GetComponent<AsPostBoxListItem>();
				body2_SC_POST_LIST_RESULT postInfo = listItem.Info;
				if( true == selectedPostSerials.Contains( postInfo.nPostSerial))
				{
					bool showHasItem = ( 0 != postInfo.sRecievItem1.nOverlapped) || ( 0 != postInfo.sRecievItem2.nOverlapped) || ( 0 != postInfo.sRecievItem3.nOverlapped) || ( 0 != postInfo.sRecievItem4.nOverlapped);
					itemHas |= showHasItem;
				}
			}
			
			if( true == itemHas)
				AsUtil.SetButtonState( btnReceive , UIButton.CONTROL_STATE.NORMAL );
			else
				AsUtil.SetButtonState( btnReceive , UIButton.CONTROL_STATE.DISABLED );

			AsUtil.SetButtonState( btnDel , UIButton.CONTROL_STATE.NORMAL );
		}
		else
		{
			btnAll.Text = AsTableManager.Instance.GetTbl_String(35);
			AsUtil.SetButtonState( btnReceive , UIButton.CONTROL_STATE.DISABLED );
			AsUtil.SetButtonState( btnDel , UIButton.CONTROL_STATE.DISABLED );
		}
	}
	
	private void OnPrevPageBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		curPage--;
		if( 0 > curPage)
			curPage = MAIL_LIST_MAX_PAGE - 1;
		
		StringBuilder sb = new StringBuilder();
		sb.AppendFormat( "{0}/{1}", curPage + 1, MAIL_LIST_MAX_PAGE);
		txtPage.Text = sb.ToString();
		
		RequestNextPageMailList();
	}
	
	private void OnNextPageBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		curPage++;
		if( MAIL_LIST_MAX_PAGE <= curPage)
			curPage = 0;
		
		StringBuilder sb = new StringBuilder();
		sb.AppendFormat( "{0}/{1}", curPage + 1, MAIL_LIST_MAX_PAGE);
		txtPage.Text = sb.ToString();
		
		RequestNextPageMailList();
	}
	
	void _initListPanel()
	{
		selectedPostSerials.Clear();
		allSelect = false;
		btnAll.Text = AsTableManager.Instance.GetTbl_String(35);
		AsUtil.SetButtonState( btnReceive , UIButton.CONTROL_STATE.DISABLED );
		AsUtil.SetButtonState( btnDel , UIButton.CONTROL_STATE.DISABLED );
		
		for( int i = 0; i < list.Count; i++)
		{
			UIListItemContainer itemContainer = list.GetItem(i) as UIListItemContainer;
			AsPostBoxListItem listItem = itemContainer.gameObject.GetComponent<AsPostBoxListItem>();
			listItem.Check( false);
		}

		noMail.gameObject.SetActiveRecursively( 0 == list.Count);
	}
	
#if false
	public void DeleteMailListItem( body2_SC_POST_LIST_RESULT info)
	{
		List<body2_CS_POST_DELETE> bodies = new List<body2_CS_POST_DELETE>();
		bodies.Add( new body2_CS_POST_DELETE( info.nPostSerial));
		body1_CS_POST_DELETE postDelete = new body1_CS_POST_DELETE( curPage, bodies.ToArray());
		byte[] data = postDelete.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);

		for( int i = 0; i < list.Count; i++)
		{
			UIListItemContainer itemContainer = list.GetItem(i) as UIListItemContainer;
			AsPostBoxListItem listItem = itemContainer.gameObject.GetComponent<AsPostBoxListItem>();
			if( listItem.Info.nPostSerial == info.nPostSerial)
			{
				list.RemoveItem( itemContainer, true);
				break;
			}
		}

		if( 0 == list.Count)
			noMail.gameObject.SetActiveRecursively( true);
	}
#endif

	public void SetMailSender( string receiver)
	{
		sendTo.Text = receiver;
	}

	private void OnReply()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		TabChange( ePostBoxDlgState.WriteTab);
		sendTo.Text = recvFrom.Text;

		string strRe = "Re:";
		string strTitle = "";

		if( recvTitle.Text.Length >= strRe.Length)
		{
			string strBuf = recvTitle.Text.Substring( 0, strRe.Length);
			if( true == strBuf.Equals( strRe))
			{
				if( recvTitle.Text.Length > 24)
					sendTitle.Text = recvTitle.Text.Substring( 0, 24);
				else
					sendTitle.Text = recvTitle.Text;
				return;
			}
		}

		if( recvTitle.Text.Length > 24 - strRe.Length)
			strTitle = recvTitle.Text.Substring( 0, 24 - strRe.Length);
		else
			strTitle = recvTitle.Text;

		sendTitle.Text = strRe + strTitle;

		while( _GetStringSize( sendTitle.Text) > 24)
		{
			sendTitle.Text = sendTitle.Text.Substring( 0, sendTitle.Text.Length - 1);
		}
	}

	private void OnTakeAll()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		_AppendItemGetAll();
	}

	private void OnRecvGold()
	{
		_AppendGoldGet();
	}
	
	void _receiveItemsConfirm()
	{
		int count = selectedPostSerials.Count;
		
		if( 0 == count)
			return;

		if( GetItemCountSelected() > ItemMgr.HadItemManagement.Inven.GetEmptyInvenSlotCount())
		{
			AsMessageBox msgBox = AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(103), AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			msgBox.SetOkText( AsTableManager.Instance.GetTbl_String(1152));
			return;
		}

		List<body2_CS_POST_ITEM_RECEIVE> bodies = new List<body2_CS_POST_ITEM_RECEIVE>();
		for( int i = 0; i < count; i++)
		{
			bodies.Add( new body2_CS_POST_ITEM_RECEIVE( selectedPostSerials[i]));
		}
		body1_CS_POST_ITEM_RECEIVE itemReceive = new body1_CS_POST_ITEM_RECEIVE( (Byte)( ePOST_COMMON.ePOST_MAX_ITEM), bodies.ToArray());
		byte[] data = itemReceive.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}
	
	private void OnSelectedItemReceiveBtn()
	{
		int count = selectedPostSerials.Count;
		
		if( 0 == count)
			return;

		AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(1576), AsTableManager.Instance.GetTbl_String(38), this, "_receiveItemsConfirm", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);
	}

	private void RecieveMailItem( byte slot, bool bAllSlot = false)
	{
		List<body2_CS_POST_ITEM_RECEIVE> itemReceiveBodys = new List<body2_CS_POST_ITEM_RECEIVE>();
		itemReceiveBodys.Add( new body2_CS_POST_ITEM_RECEIVE( readMailInfo.nPostSerial));
		body1_CS_POST_ITEM_RECEIVE itemReceive = new body1_CS_POST_ITEM_RECEIVE( slot, itemReceiveBodys.ToArray());
		byte[] data = itemReceive.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);

		if( true == bAllSlot)
		{
			readMailInfo.sRecievItem1.nOverlapped = 0;
			readMailInfo.sRecievItem2.nOverlapped = 0;
			readMailInfo.sRecievItem3.nOverlapped = 0;
			readMailInfo.sRecievItem4.nOverlapped = 0;
		}
		else
		{
			switch( slot)
			{
			case 0:	readMailInfo.sRecievItem1.nOverlapped = 0;	break;
			case 1:	readMailInfo.sRecievItem2.nOverlapped = 0;	break;
			case 2:	readMailInfo.sRecievItem3.nOverlapped = 0;	break;
			case 3:	readMailInfo.sRecievItem4.nOverlapped = 0;	break;
			}
		}
	}

	private string _GetPostSenderName(body2_SC_POST_LIST_RESULT info)
	{
		string strSenderName = "";

		if( ePOSTTYPE.ePOSTTYPE_NOTHING == (ePOSTTYPE)(info.nPostType))
		{
			//strSenderName = info.senderName;

			if( 24 < _GetStringSize( info.senderName))
				strSenderName = info.senderName.Substring( 0, info.senderName.Length - 1);
			else
				strSenderName = info.senderName;
		}
		else
			strSenderName = AsTableManager.Instance.GetTbl_String(1450);

		return strSenderName;
	}

	private string _GetPostTitle(body2_SC_POST_LIST_RESULT info, bool bUseColor)
	{
		string strTitle = "";
		string strColor = "";

		if( true == bUseColor)
		{
			if( true == info.bAccount)
				strColor = Color.yellow.ToString();
			else
				strColor = Color.white.ToString();
		}

		switch( (ePOSTTYPE)info.nPostType)
		{
		case ePOSTTYPE.ePOSTTYPE_NOTHING: strTitle = strColor + info.title; break;
		case ePOSTTYPE.ePOSTTYPE_PRIVATE_SHOP: strTitle = strColor + AsTableManager.Instance.GetTbl_String(1353); break;
		case ePOSTTYPE.ePOSTTYPE_ATTENDANCE: strTitle = strColor + AsTableManager.Instance.GetTbl_String(1451); break;
		case ePOSTTYPE.ePOSTTYPE_RETURN: strTitle = strColor + AsTableManager.Instance.GetTbl_String(1452); break;
		case ePOSTTYPE.ePOSTTYPE_INVITE_FRIEND: strTitle = strColor + AsTableManager.Instance.GetTbl_String(1455); break;
		case ePOSTTYPE.ePOSTTYPE_GUILD_EXPULSION: strTitle = strColor + AsTableManager.Instance.GetTbl_String(1457); break;
		case ePOSTTYPE.ePOSTTYPE_HELLO_SEND: strTitle = strColor + AsTableManager.Instance.GetTbl_String(1500); break;
		case ePOSTTYPE.ePOSTTYPE_HELLO_RECV: strTitle = strColor + AsTableManager.Instance.GetTbl_String(1502); break;
		case ePOSTTYPE.ePOSTTYPE_HELLO_SEND_RECV: strTitle = strColor + AsTableManager.Instance.GetTbl_String(1504); break;
		case ePOSTTYPE.ePOSTTYPE_RECALL_SEND: strTitle = strColor + AsTableManager.Instance.GetTbl_String(1506); break;
		case ePOSTTYPE.ePOSTTYPE_RECALL_RECV: strTitle = strColor + AsTableManager.Instance.GetTbl_String(1508); break;
		case ePOSTTYPE.ePOSTTYPE_CONNECT_EVENT: strTitle = strColor + AsTableManager.Instance.GetTbl_String(1510); break;
		case ePOSTTYPE.ePOSTTYPE_RECOMMEND_1: strTitle = strColor + AsTableManager.Instance.GetTbl_String(1512); break;
		case ePOSTTYPE.ePOSTTYPE_RECOMMEND_2: strTitle = strColor + AsTableManager.Instance.GetTbl_String(1512); break;
		case ePOSTTYPE.ePOSTTYPE_RECOMMEND_REWARD_1: strTitle = strColor + AsTableManager.Instance.GetTbl_String(1512); break;
		case ePOSTTYPE.ePOSTTYPE_RECOMMEND_REWARD_2: strTitle = strColor + AsTableManager.Instance.GetTbl_String(1512); break;
		case ePOSTTYPE.ePOSTTYPE_RECOMMEND_ACCRUE: strTitle = strColor + AsTableManager.Instance.GetTbl_String(1512); break;
		case ePOSTTYPE.ePOSTTYPE_CASH_GIFT: strTitle = strColor + AsTableManager.Instance.GetTbl_String(1514); break;
		case ePOSTTYPE.ePOSTTYPE_COUPON: strTitle = strColor + AsTableManager.Instance.GetTbl_String(1518); break;
//		case ePOSTTYPE.ePOSTTYPE_GUILD_EVENT_GIFT: strTitle = strColor + AsTableManager.Instance.GetTbl_String(1516); break;
		case ePOSTTYPE.ePOSTTYPE_LEVEL_COMPLETE: strTitle = strColor + AsTableManager.Instance.GetTbl_String(1521); break;
		case ePOSTTYPE.ePOSTTYPE_GM_SEND: strTitle = strColor + info.title; break;
		case ePOSTTYPE.ePOSTTYPE_WEME_EVENT: strTitle = strColor + AsTableManager.Instance.GetTbl_String(1523); break;
		case ePOSTTYPE.ePOSTTYPE_WEME_COMPENSATION: strTitle = strColor + AsTableManager.Instance.GetTbl_String(1525); break;
		case ePOSTTYPE.ePOSTTYPE_DAILY_REWARD: strTitle = strColor + AsTableManager.Instance.GetTbl_String(2352); break;
		case ePOSTTYPE.ePOSTTYPE_INDUN_FIRST_REWARD: strTitle = strColor + AsTableManager.Instance.GetTbl_String(2350); break;
		}

		return strTitle;
	}

	private string _GetPostContent(body2_SC_POST_LIST_RESULT info)
	{
		string strContent = "";

		switch( (ePOSTTYPE)info.nPostType)
		{
		case ePOSTTYPE.ePOSTTYPE_NOTHING:
			strContent = info.content;
			break;

		case ePOSTTYPE.ePOSTTYPE_PRIVATE_SHOP:
			{
				Item item = ItemMgr.ItemManagement.GetItem( (int)info.nPostSubValue1);
				if( null != item && null != item.ItemData)
				{
					string strItem = AsTableManager.Instance.GetTbl_String(item.ItemData.nameId);
					strContent = string.Format( AsTableManager.Instance.GetTbl_String(1354), info.content, strItem, info.nPostSubValue2, info.nPostSubValue3);
				}
			}
			break;

		case ePOSTTYPE.ePOSTTYPE_ATTENDANCE:
			strContent = AsTableManager.Instance.GetTbl_String(1453);
			break;
		case ePOSTTYPE.ePOSTTYPE_RETURN:
			strContent = AsTableManager.Instance.GetTbl_String(1454);
			break;
		case ePOSTTYPE.ePOSTTYPE_INVITE_FRIEND:
			strContent = AsTableManager.Instance.GetTbl_String(1456);
			break;
		case ePOSTTYPE.ePOSTTYPE_GUILD_EXPULSION:
			strContent = AsTableManager.Instance.GetTbl_String(1458);
			break;
		case ePOSTTYPE.ePOSTTYPE_HELLO_SEND:
			strContent = string.Format( AsTableManager.Instance.GetTbl_String(1501), info.content);
			break;
		case ePOSTTYPE.ePOSTTYPE_HELLO_RECV:
			strContent = string.Format( AsTableManager.Instance.GetTbl_String(1503), info.content);
			break;
		case ePOSTTYPE.ePOSTTYPE_HELLO_SEND_RECV:
			strContent = string.Format( AsTableManager.Instance.GetTbl_String(1505), info.content);
			break;
		case ePOSTTYPE.ePOSTTYPE_RECALL_SEND:
			strContent = string.Format( AsTableManager.Instance.GetTbl_String(1507), info.content);
			break;
		case ePOSTTYPE.ePOSTTYPE_RECALL_RECV:
			strContent = string.Format( AsTableManager.Instance.GetTbl_String(1509), info.content);
			break;
		case ePOSTTYPE.ePOSTTYPE_CONNECT_EVENT:
			strContent = AsTableManager.Instance.GetTbl_String(1511);
			break;
		case ePOSTTYPE.ePOSTTYPE_RECOMMEND_1:
			strContent = AsTableManager.Instance.GetTbl_String(1743);
			break;
		case ePOSTTYPE.ePOSTTYPE_RECOMMEND_2:
			strContent = string.Format( AsTableManager.Instance.GetTbl_String(1680), info.content);
			break;
		case ePOSTTYPE.ePOSTTYPE_RECOMMEND_REWARD_1:
			strContent = AsTableManager.Instance.GetTbl_String(1513);
			break;
		case ePOSTTYPE.ePOSTTYPE_RECOMMEND_REWARD_2:
			strContent = string.Format( AsTableManager.Instance.GetTbl_String(1677), info.content);
			break;
		case ePOSTTYPE.ePOSTTYPE_RECOMMEND_ACCRUE:
			strContent = AsTableManager.Instance.GetTbl_String(1744);
			break;
		case ePOSTTYPE.ePOSTTYPE_CASH_GIFT:
			strContent = string.Format( AsTableManager.Instance.GetTbl_String(1515), info.content);
			break;
		case ePOSTTYPE.ePOSTTYPE_COUPON:
			strContent = AsTableManager.Instance.GetTbl_String(1519);
			break;
//		case ePOSTTYPE.ePOSTTYPE_GUILD_EVENT_GIFT:
//			strContent = string.Format( AsTableManager.Instance.GetTbl_String(1517));
//			break;
		case ePOSTTYPE.ePOSTTYPE_LEVEL_COMPLETE:
			strContent = AsTableManager.Instance.GetTbl_String(1522);
			break;
		case ePOSTTYPE.ePOSTTYPE_GM_SEND:
			strContent = info.content;
			break;
		case ePOSTTYPE.ePOSTTYPE_WEME_EVENT:
			strContent = AsTableManager.Instance.GetTbl_String(1524);
			break;
		case ePOSTTYPE.ePOSTTYPE_WEME_COMPENSATION:
			strContent = AsTableManager.Instance.GetTbl_String(1526);
			break;
		case ePOSTTYPE.ePOSTTYPE_DAILY_REWARD:
			{
				Item item = ItemMgr.ItemManagement.GetItem( (int)info.nPostSubValue1);
				if( null != item && null != item.ItemData)
				{
					string strItem = AsTableManager.Instance.GetTbl_String(item.ItemData.nameId);
					int nTotalHor = (int)( info.nPostSubValue2 / 3600);
					int nDay = (int)( nTotalHor / 24);
					int nHor = (int)( nTotalHor % 24);
					strContent = string.Format( AsTableManager.Instance.GetTbl_String(2353), strItem, nDay.ToString(), nHor.ToString());
				}
			}
			break;
		case ePOSTTYPE.ePOSTTYPE_INDUN_FIRST_REWARD:
			strContent = AsTableManager.Instance.GetTbl_String(2351);
			break;
		}

		return strContent;
	}

	private int _GetPostCost()
	{
		int nPostCost = (int)( AsTableManager.Instance.GetTbl_GlobalWeight_Record( 34).Value);

		float fCommission = 0;

		foreach( AsSlot _slot in sendItemSlots)
		{
			if( 0 != _slot.getItemID)
			{
				int nSellAmount = _slot.getRealItem.item.ItemData.sellAmount;
				if( nSellAmount > 0)
				{
					float fCommissionBuf = nSellAmount * ( AsTableManager.Instance.GetTbl_GlobalWeight_Record( 74).Value * 0.001f);
					if( fCommissionBuf > 0.0f)
						fCommission += fCommissionBuf * _slot.getItemCount;
				}
			}
		}

		return ( nPostCost + (int)fCommission);
	}

	private void _AppendItemCancel(AsSlot slot)
	{
		ItemMgr.HadItemManagement.Inven.SetSlotMoveLock(slot.getRealItem.getSlot, false);
		if (true == AsHudDlgMgr.Instance.IsOpenInven)
			AsHudDlgMgr.Instance.invenDlg.ApplySlotMoveLock();
		slot.SetEmpty();
	}

	private void _AppendItemGet(AsSlot slot, byte slotIndex)
	{
		if( true == _isOpenMsgBox())
			return;

		if( true == readMailInfo.bAccount)
		{
			Color titleColor = new Color( 1.0f, 0.494f, 0.0f, 1.0f);
			string title = titleColor.ToString() + AsTableManager.Instance.GetTbl_String(1442);
			string msg = AsTableManager.Instance.GetTbl_String(1443);
			m_msgboxItem = AsNotify.Instance.MessageBox( title, msg, this, "OnMsgBox_AppendItemGet_Ok", "OnMsgBox_AppendItemGet_Cancel", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);
			m_slotBuf = slot;
			m_slotIndexBuf = slotIndex;
		}
		else
		{
			// server packet
			RecieveMailItem( slotIndex);
			slot.SetEmpty();
		}

		if( false == _isGetItemInReadMailInfo())
		{
			btnTakeAll.SetControlState( UIButton.CONTROL_STATE.DISABLED);
			btnTakeAll.spriteText.Color = Color.gray;
		}
	}

	private void _AppendItemGetAll()
	{
		if( false == _isGetItemInReadMailInfo())
			return;

		if( true == _isOpenMsgBox())
			return;

		//if( -1 == ItemMgr.HadItemManagement.Inven.GetEmptyInvenSlot())
		if( _isGetItemCountInReadMailInfo() > ItemMgr.HadItemManagement.Inven.GetEmptyInvenSlotCount())
		{
			AsMessageBox msgBox = AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(103), AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			msgBox.SetOkText( AsTableManager.Instance.GetTbl_String(1152));
			return;
		}

		if( true == readMailInfo.bAccount)
		{
			Color titleColor = new Color( 1.0f, 0.494f, 0.0f, 1.0f);
			string title = titleColor.ToString() + AsTableManager.Instance.GetTbl_String(1442);
			string msg = AsTableManager.Instance.GetTbl_String(1443);
			m_msgboxItem = AsNotify.Instance.MessageBox( title, msg, this, "OnMsgBox_AppendItemGetAll_Ok", "OnMsgBox_AppendItemGetAll_Cancel", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);
			m_slotBuf = null;
			m_slotIndexBuf = 0;
		}
		else
		{
			// server packet
			RecieveMailItem( 4, true);

			for( int i = 0; i < 4; i++)
				recvItemSlots[i].SetEmpty();

			if( readMailInfo.nGold > 0)
			{
				recvGold.Text = "0";
				readMailInfo.nGold = 0;
			}
		}

		if( false == _isGetItemInReadMailInfo())
		{
			btnTakeAll.SetControlState( UIButton.CONTROL_STATE.DISABLED);
			btnTakeAll.spriteText.Color = Color.gray;
			return;
		}
	}

	private void _AppendGoldGet()
	{
		if( true == _isOpenMsgBox())
			return;

		if( null == readMailInfo || 0 == readMailInfo.nGold)
			return;

		if( true == readMailInfo.bAccount)
		{
			Color titleColor = new Color( 1.0f, 0.494f, 0.0f, 1.0f);
			string title = titleColor.ToString() + AsTableManager.Instance.GetTbl_String(1442);
			string msg = AsTableManager.Instance.GetTbl_String(1443);
			m_msgboxItem = AsNotify.Instance.MessageBox( title, msg, this, "OnMsgBox_AppendGoldGet_Ok", "OnMsgBox_AppendGoldGet_Cancel", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);
		}
		else
		{
			body_CS_POST_GOLD_RECIEVE goldRecieve = new body_CS_POST_GOLD_RECIEVE( readMailInfo.nPostSerial);
			byte[] data = goldRecieve.ClassToPacketBytes();
			AsNetworkMessageHandler.Instance.Send( data);

			recvGold.Text = "0";
			readMailInfo.nGold = 0;
		}
	}

	private bool _isOpenMsgBox()
	{
		if( null != m_msgboxItem)
			return true;
		return false;
	}

	public void OnMsgBox_AppendItemGet_Ok()
	{
		if( null == m_slotBuf)
		{
			Debug.LogError( "OnMsgBox_AppendItemGet_Ok(): null == m_slotBuf");
			return;
		}

		// server packet
		RecieveMailItem( m_slotIndexBuf);
		m_slotBuf.SetEmpty();

		if( false == _isGetItemInReadMailInfo())
		{
			btnTakeAll.SetControlState( UIButton.CONTROL_STATE.DISABLED);
			btnTakeAll.spriteText.Color = Color.gray;
		}
	}

	public void OnMsgBox_AppendItemGet_Cancel()
	{
		m_slotBuf = null;
		m_slotIndexBuf = 0;
	}

	public void OnMsgBox_AppendItemGetAll_Ok()
	{
		RecieveMailItem( 4, true);

		for( int i = 0; i < 4; i++)
			recvItemSlots[i].SetEmpty();

		if( readMailInfo.nGold > 0)
		{
			recvGold.Text = "0";
			readMailInfo.nGold = 0;
		}

		btnTakeAll.SetControlState( UIButton.CONTROL_STATE.DISABLED);
		btnTakeAll.spriteText.Color = Color.gray;
	}

	public void OnMsgBox_AppendItemGetAll_Cancel()
	{
		m_slotBuf = null;
		m_slotIndexBuf = 0;
	}

	public void OnMsgBox_AppendGoldGet_Ok()
	{
		body_CS_POST_GOLD_RECIEVE goldRecieve = new body_CS_POST_GOLD_RECIEVE( readMailInfo.nPostSerial);
		byte[] data = goldRecieve.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);

		recvGold.Text = "0";
		readMailInfo.nGold = 0;
	}

	public void OnMsgBox_AppendGoldGet_Cancel()
	{
	}

	private int	GetItemCountSelected()
	{
		int nSelectedItemCount = 0;

		for( int i = 0; i < list.Count; i++)
		{
			UIListItemContainer itemContainer = list.GetItem(i) as UIListItemContainer;
			AsPostBoxListItem listItem = itemContainer.gameObject.GetComponent<AsPostBoxListItem>();
			body2_SC_POST_LIST_RESULT postInfo = listItem.Info;
			if( true == selectedPostSerials.Contains( postInfo.nPostSerial))
			{
				if( postInfo.sRecievItem1.nOverlapped > 0 )
					nSelectedItemCount++;
				if( postInfo.sRecievItem2.nOverlapped > 0 )
					nSelectedItemCount++;
				if( postInfo.sRecievItem3.nOverlapped > 0 )
					nSelectedItemCount++;
				if( postInfo.sRecievItem4.nOverlapped > 0 )
					nSelectedItemCount++;
			}
		}

		return nSelectedItemCount;
	}

	private bool _isGetItemInReadMailInfo()
	{
		if( 0 == readMailInfo.sRecievItem1.nOverlapped
			&& 0 == readMailInfo.sRecievItem2.nOverlapped
			&& 0 == readMailInfo.sRecievItem3.nOverlapped
			&& 0 == readMailInfo.sRecievItem4.nOverlapped
			&& 0 == readMailInfo.nGold)
			return false;

		return true;
	}

	private int _isGetItemCountInReadMailInfo()
	{
		int nCount = 0;

		if( readMailInfo.sRecievItem1.nOverlapped > 0)
			nCount++;
		if( readMailInfo.sRecievItem2.nOverlapped > 0)
			nCount++;
		if( readMailInfo.sRecievItem3.nOverlapped > 0)
			nCount++;
		if( readMailInfo.sRecievItem4.nOverlapped > 0)
			nCount++;

		return nCount;
	}

	private int _GetStringSize(string str)
	{
		int length = System.Text.UTF8Encoding.UTF8.GetByteCount( str);
		return length;
	}
}
