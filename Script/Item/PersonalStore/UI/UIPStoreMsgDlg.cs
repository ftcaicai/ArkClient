using UnityEngine;
using System.Collections;

public class UIPStoreMsgDlg : MonoBehaviour {
	
	public UIButton confirm_;
	public UIButton cancel_;
	public SpriteText textTitle_;
	public UITextField textFieldContent_;
	public SpriteText textContent_;
	public SpriteText textSubTitle;
	public SpriteText textCancel;
	public SpriteText textConfirm;
	
	AsMessageBox m_MsgBox = null;

	void Awake()
	{
		confirm_.SetInputDelegate(ConfirmBtnDelegate);
		cancel_.SetInputDelegate(CancelBtnDelegate);
	}
	
	public void Open(string _str)
    {
		AsLanguageManager.Instance.SetFontFromSystemLanguage( textTitle_);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( textContent_);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( textSubTitle);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( textCancel);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( textConfirm);

		gameObject.SetActiveRecursively(true);
		
		textFieldContent_.Text = _str;
		
//		string name = 
//			AsUserInfo.Instance.GetCurrentUserEntity().GetProperty<string>(eComponentProperty.NAME);
		//textTitle_.Text = string.Format(AsTableManager.Instance.GetTbl_String(1231), name);
		textTitle_.Text = AsTableManager.Instance.GetTbl_String( 1232);
		textSubTitle.Text = AsTableManager.Instance.GetTbl_String( 1233);
		textCancel.Text = AsTableManager.Instance.GetTbl_String( 1151);
		textConfirm.Text = AsTableManager.Instance.GetTbl_String( 1152);
		textFieldContent_.SetValidationDelegate(ContentValidator);
    }

    public void Close()
    {
        gameObject.SetActiveRecursively(false);
//		ClearMsgBox();
    }
		
	void ConfirmBtnDelegate( ref POINTER_INFO ptr )
	{
		if(CheckMsgBoxPopUp() == true)
			return;
		
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			if(textContent_.text == "")
			{
//				string name = AsUserInfo.Instance.GetCurrentUserEntity().GetProperty<string>(eComponentProperty.NAME);
//				textContent_.text = string.Format(AsTableManager.Instance.GetTbl_String(1231), name);
			}
			
			if(AsTableManager.Instance.TextFiltering_PStoreContent(textContent_.text) == true)
			{
				AsPStoreManager.Instance.SetContentText(textContent_.text);
				AsPStoreManager.Instance.Request_Modify();
				Close();
				ClearMsgBox();
			}
			else
			{
				m_MsgBox = AsNotify.Instance.MessageBox("", AsTableManager.Instance.GetTbl_String(364), this, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_ERROR);
			}
		}
	}
	
	void CancelBtnDelegate( ref POINTER_INFO ptr )
	{
		if(CheckMsgBoxPopUp() == true)
			return;
		
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			Close();
			ClearMsgBox();
		}
	}
	
	bool CheckMsgBoxPopUp()
	{
		if(m_MsgBox != null)
			return true;
		else
			return false;
	}
	
	void ClearMsgBox()
	{
		if(m_MsgBox != null)
		{
			Destroy(m_MsgBox.gameObject);
		}
		m_MsgBox = null;
	}
	
	string ContentValidator( UITextField field, string text, ref int insPos)
	{		
		// #22671
		int index = 0;
		
		index =  text.IndexOf('\'');
		if(-1 != index)
			text = text.Remove( index);
		
		index =  text.IndexOf('\n');
		if(-1 != index)
			text = text.Remove( index);
		
		index =  text.IndexOf('\r');
		if(-1 != index)
			text = text.Remove( index);
		
		return text;	
	}
}
