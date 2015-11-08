using UnityEngine;
using System;
using System.Collections;
using System.Text.RegularExpressions;

public class AsRecommendDlg : MonoBehaviour
{
	public SpriteText m_Title = null;
	public SpriteText m_RewardItemLabel = null;
	public SpriteText m_RewardItemName = null;
	public UITextField m_NameEdit = null;
	public UIButton m_OkBtn = null;
	public UIButton m_CancelBtn = null;

	private GameObject goParent = null;
	public GameObject ParentObject
	{
		set	{ goParent = value; }
	}

	public SimpleSprite itemImgPos;
	public Vector2 minusItemSize;

	private UISlotItem m_SlotItem = null;
	public BoxCollider m_IconCollider;
	private Item m_Item;


	// Use this for initialization
	void Start()
	{
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_Title);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_RewardItemLabel);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_RewardItemName);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_NameEdit);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_OkBtn.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_CancelBtn.spriteText);

		m_Title.Text = AsTableManager.Instance.GetTbl_String(1434);
		m_RewardItemLabel.Text = AsTableManager.Instance.GetTbl_String(4033);

		m_OkBtn.Text = AsTableManager.Instance.GetTbl_String(1152);
		m_CancelBtn.Text = AsTableManager.Instance.GetTbl_String(1151);

		m_NameEdit.spriteText.Text = AsTableManager.Instance.GetTbl_String(4034);
		m_NameEdit.SetFocusDelegate( OnFocusID);
		SetItem();

		m_NameEdit.SetValidationDelegate( OnValidateName);	//#18178
	}

	private string OnValidateName( UITextField field, string text, ref int insPos)
	{
		while( true)
		{
			int byteCount = System.Text.UTF8Encoding.UTF8.GetByteCount( text);
			int charCount = System.Text.UTF8Encoding.UTF8.GetCharCount( System.Text.UTF8Encoding.UTF8.GetBytes( text));
			if( ( byteCount <= 24) && ( charCount <= 12))
				break;

			text = text.Remove( text.Length - 1);
		}

		return Regex.Replace( text, "\\s+", "");
	}

	// Update is called once per frame
	void Update()
	{
#if UNITY_EDITOR
		if( Input.GetMouseButtonUp( 0) == true)
		{
			if( null != TooltipMgr.Instance)
				TooltipMgr.Instance.Clear();
		}

		if( true == Input.GetMouseButtonDown( 0))
		{
			Ray ray = UIManager.instance.rayCamera.ScreenPointToRay( Input.mousePosition);

			if( m_SlotItem == null || m_Item == null)
				return;

			if( m_IconCollider == null)
				return;

			if( true == AsUtil.PtInCollider( m_IconCollider, ray))
			{
				AsHUDController.Instance.m_TooltipMgr.gameObject.SetActiveRecursively( true);
				if( null != TooltipMgr.Instance)
					TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.normal, m_Item, false, -10.0f);
			}
		}
#else
		if( 0 < Input.touchCount)
		{
			if( Input.GetTouch( 0).phase == TouchPhase.Ended || Input.GetTouch( 0).phase == TouchPhase.Canceled)
			{
				if( null != TooltipMgr.Instance)
					TooltipMgr.Instance.Clear();
			}
		}

		if( 0 < Input.touchCount)
		{
			Touch touch = Input.GetTouch( 0);
			Ray ray = UIManager.instance.rayCamera.ScreenPointToRay( touch.position);
			if( TouchPhase.Began == touch.phase)
			{
				if( m_SlotItem == null || m_Item == null)
					return;

				if( m_IconCollider == null)
					return;

				if( true == AsUtil.PtInCollider( m_IconCollider, ray))
				{
					AsHUDController.Instance.m_TooltipMgr.gameObject.SetActiveRecursively( true);
					if( null != TooltipMgr.Instance)
						TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.normal, m_Item, false, -10.0f);
				}
			}
		}
#endif
	}

	public bool IsIconRect( Ray inputRay)
	{
		if( null == m_IconCollider)
			return false;

		return AsUtil.PtInCollider( m_IconCollider, inputRay);
	}

	public void SetItem()
	{
		m_Item = ItemMgr.ItemManagement.GetItem( AsEventUIMgr.Instance.RecommendItemTableIdx);
		if( null == m_Item)
			return;

		m_RewardItemName.Text = AsTableManager.Instance.GetTbl_String( m_Item.ItemData.nameId);

		if( null != m_SlotItem)
			GameObject.Destroy( m_SlotItem.gameObject);

		m_SlotItem = ResourceLoad.CreateItemIcon( m_Item.GetIcon(), itemImgPos, Vector3.back, minusItemSize);
	}

	private void OkBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		string name = m_NameEdit.Text;

		if( 0 == name.Length)
		{
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(1434), AsTableManager.Instance.GetTbl_String(1438),
										AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);

			return;
		}

		//#20503
		if( AsEventUIMgr.Instance.UserName.CompareTo( name)== 0)
		{
			//m_NameEdit.Text = AsTableManager.Instance.GetTbl_String(1920);
			m_RewardItemLabel.Text = AsTableManager.Instance.GetTbl_String(1920);
			return;
		}

		string msg = string.Format (AsTableManager.Instance.GetTbl_String (2345), name);
		AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(131), msg,
		                             AsTableManager.Instance.GetTbl_String(1152), AsTableManager.Instance.GetTbl_String(1151),
		                             this, "ProcessRecommend", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);
	}

	void ProcessRecommend()
	{
		string name = m_NameEdit.Text;

		body_CG_RECOMMEND recommend = new body_CG_RECOMMEND( true, name);
		byte[] data = recommend.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
		goParent.BroadcastMessage( "CloseRecommendDlg", SendMessageOptions.DontRequireReceiver);
	}

	private void CancelBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(131), AsTableManager.Instance.GetTbl_String (2344),
		                             AsTableManager.Instance.GetTbl_String(1152), AsTableManager.Instance.GetTbl_String(1151),
		                             this, "ProcessRecommendCancel", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);
	}

	void ProcessRecommendCancel()
	{
		body_CG_RECOMMEND recommend = new body_CG_RECOMMEND( false, "");
		byte[] data = recommend.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
		goParent.BroadcastMessage( "CloseRecommendDlg", SendMessageOptions.DontRequireReceiver);
	}

	private void OnFocusID( UITextField field)
	{
		m_NameEdit.Text = string.Empty;
	}
	
	public void Close()
	{
		CancelBtn();
	}
}
