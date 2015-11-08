using UnityEngine;
using System.Collections;

public class AsDeathDlgFriend : MonoBehaviour
{
	public SpriteText title = null; // ilmeda, 20120824
	public SpriteText text = null;
	public SpriteText haveMiracleTitle;
	public SpriteText consumeMiracleTitle;
	public SpriteText haveMiracle;
	public SpriteText consumeMiracle;
	public UIButton btnCancel = null;
	public UIButton btnResurrection = null;
	public UIButton btnClose = null;
	public SpriteText btnMiracle_Text_Miracle = null;
	
	AsUserEntity m_UserEntity = null;
	string m_UserName = "not set";
	
	public void Init( AsUserEntity _user)
	{
		m_UserEntity = _user;
		m_UserName = _user.GetProperty<string>( eComponentProperty.NAME);
		eCLASS usrClass = _user.GetProperty<eCLASS>( eComponentProperty.CLASS);
		int usrLevel = _user.GetProperty<int>( eComponentProperty.LEVEL);
//		consumeMiracle.Text = AsTableManager.Instance.GetTbl_Level_Record( usrClass, usrLevel).Resurrection_Cost.ToString();
		consumeMiracle.Text = AsTableManager.Instance.GetTbl_GlobalWeight_Record("Revival_Miracle_Help").Value.ToString();
	}
	
	// Use this for initialization
	void Start()
	{
		// < ilmeda, 20120824
		title.Text = "";
		text.Text = "";
		AsLanguageManager.Instance.SetFontFromSystemLanguage( title);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( text);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( btnMiracle_Text_Miracle);//$yde
		AsLanguageManager.Instance.SetFontFromSystemLanguage( haveMiracleTitle);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( consumeMiracleTitle);
		title.Text = AsTableManager.Instance.GetTbl_String(1102);
		string str = AsTableManager.Instance.GetTbl_String(1370);
		text.Text = string.Format( str, m_UserName);

		haveMiracleTitle.Text = AsTableManager.Instance.GetTbl_String(859);
		consumeMiracleTitle.Text = AsTableManager.Instance.GetTbl_String(858);
		haveMiracle.Text = AsUserInfo.Instance.nMiracle.ToString();

		// ilmeda, 20120824 >
		btnCancel.Text = AsTableManager.Instance.GetTbl_String(1151);
		btnResurrection.Text = AsTableManager.Instance.GetTbl_String(1369);

		btnCancel.SetInputDelegate( OnCancelBtn_del);
		btnClose.SetInputDelegate( OnCloseBtn_del);
	}

	// Update is called once per frame
	void Update()
	{
	}
	
	public void OnResurrection()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

//		eCLASS __class = m_UserEntity.GetProperty<eCLASS>( eComponentProperty.CLASS);
//		int level = m_UserEntity.GetProperty<int>( eComponentProperty.LEVEL);
//		long require = (long)AsTableManager.Instance.GetTbl_Level_Record( __class, level).Resurrection_Cost;

		long require = (long)AsTableManager.Instance.GetTbl_GlobalWeight_Record("Revival_Miracle_Help").Value;
		if( AsUserInfo.Instance.nMiracle < require)
		{
			string title = AsTableManager.Instance.GetTbl_String(1102);
			string content = AsTableManager.Instance.GetTbl_String(339);

			if( AsGameMain.useCashShop == true)
				AsNotify.Instance.MessageBox( title, content, this, "OnMiracleShop", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			else
				AsNotify.Instance.MessageBox( title, content, AsNotify.MSG_BOX_TYPE.MBT_OK);
		}
		else
		{
			string title = AsTableManager.Instance.GetTbl_String(1102);
			string content = AsTableManager.Instance.GetTbl_String(342);
			content = string.Format( content, require, m_UserName);
			AsNotify.Instance.MessageBox( title, content, this, "OnResurrectionConfirmed", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_QUESTION);
		}
		
		gameObject.SetActiveRecursively( false);
	}
	
	void OnMiracleShop()
	{
		// cash store required
		AsHudDlgMgr.Instance.OpenCashStore(0, eCLASS.NONE, eCashStoreMenuMode.CHARGE_MIRACLE, eCashStoreSubCategory.NONE, 0);
		
		Destroy( gameObject);
	}
	
	void OnResurrectionConfirmed()
	{
		Vector3 pos = AsUserInfo.Instance.GetCurrentUserEntity().transform.position;
		float dist = Vector3.Distance( m_UserEntity.transform.position, pos);
		
		if( dist > 10)
		{
			string title = AsTableManager.Instance.GetTbl_String(1102);
			string content = AsTableManager.Instance.GetTbl_String(343);
			AsNotify.Instance.MessageBox( title, content, AsNotify.MSG_BOX_TYPE.MBT_OK);
		}
		else if( m_UserEntity.GetProperty<bool>( eComponentProperty.LIVING) == true)
		{
			string title = AsTableManager.Instance.GetTbl_String(1102);
			string content = AsTableManager.Instance.GetTbl_String(344);
			AsNotify.Instance.MessageBox( title, content, AsNotify.MSG_BOX_TYPE.MBT_OK); 
		}
		else
		{
			AsCommonSender.SendOtherUserResurrection( m_UserEntity.SessionId);
		}
		
		Destroy( gameObject);
	}
	
	void OnCanceled()
	{
		Destroy( gameObject);
	}
	
	void OnCancelBtn_del( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			OnCanceled();
		}
	}
	
	void OnCloseBtn_del( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			OnCanceled();
		}
	}
}
