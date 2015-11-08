using UnityEngine;
using System;
using System.Collections;

public delegate void CheckDelegate( bool isChecked, UInt64 serial);

public class AsPostBoxListItem : MonoBehaviour
{
	[SerializeField] SimpleSprite hasItem = null;
	[SerializeField] SimpleSprite isReaded = null;
	[SerializeField] SpriteText title = null;
	[SerializeField] SpriteText period = null;
	[SerializeField] UIRadioBtn check = null;
	
	private bool isChecked = false;
	
	private CheckDelegate checkDelegate = null;
	public CheckDelegate CheckCallback
	{
		set	{ checkDelegate = value; }
	}
	
	private body2_SC_POST_LIST_RESULT info = null;
	public body2_SC_POST_LIST_RESULT Info
	{
		get	{ return info; }
	}
	private MonoBehaviour parent = null;
	public MonoBehaviour Parent
	{
		set	{ parent = value; }
	}
	
	// Use this for initialization
	void Start()
	{
		AsLanguageManager.Instance.SetFontFromSystemLanguage( title);
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	public void SetInfo( body2_SC_POST_LIST_RESULT info, string strTitle = "")
	{
		this.info = info;
		
		if( strTitle.Length > 0)
			title.Text = strTitle;
		
		period.Text = ( info.nDeleteTime / ( 3600 * 24)).ToString() + AsTableManager.Instance.GetTbl_String(91);
		
		bool showHasItem = ( 0 != info.sRecievItem1.nOverlapped) || ( 0 != info.sRecievItem2.nOverlapped)
			|| ( 0 != info.sRecievItem3.nOverlapped) || ( 0 != info.sRecievItem4.nOverlapped);
		hasItem.gameObject.SetActiveRecursively( showHasItem);
		isReaded.gameObject.SetActiveRecursively( !info.bRead);
	}
	
	public void ChangeItemIconState()
	{
		hasItem.Hide( true);
		isReaded.Hide( true);
		info.sRecievItem1.nOverlapped = 0;
		info.sRecievItem2.nOverlapped = 0;
		info.sRecievItem3.nOverlapped = 0;
		info.sRecievItem4.nOverlapped = 0;
	}
	
	public void RefreshInfo()
	{
		SetInfo( this.info);
	}
	
	private void OnClick()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		if( null == parent)
			return;
		
		info.bRead = true;
		parent.SendMessage( "TabChange", ePostBoxDlgState.ReadTab);
		parent.SendMessage( "SetMailDetailInfo", info);
		
		body_CS_POST_READ postRead = new body_CS_POST_READ( info.nPostSerial);
		byte[] data = postRead.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}
	
	private void OnCheck()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		if( null == info)
			return;
	
		isChecked = !isChecked;
		check.Value = isChecked;
		
		if( null != checkDelegate)
			checkDelegate( isChecked, info.nPostSerial);
	}
	
#if false
	private void OnDelete()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		if( null == info)
			return;
	
		isChecked = !isChecked;
		
		if( null != checkDelegate)
			checkDelegate( isChecked, info.nPostSerial);
#if false
		AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(1425), AsTableManager.Instance.GetTbl_String(94),
			this, "DeleteItem", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);
#endif
	}
#endif
	
	private void DeleteItem()
	{
		parent.SendMessage( "DeleteMailListItem", info);
	}
	
	public void Check( bool bCheck)
	{
		isChecked = bCheck;
		
		check.Value = bCheck;
//		if( true == bCheck)
//			del.SetControlState( UIButton.CONTROL_STATE.ACTIVE);
//		else
//			del.SetControlState( UIButton.CONTROL_STATE.NORMAL);
	}
}
