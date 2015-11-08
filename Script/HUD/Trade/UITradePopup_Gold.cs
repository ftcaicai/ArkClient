using UnityEngine;
using System.Collections;
using System;
using System.Text.RegularExpressions;

public class UITradePopup_Gold : MonoBehaviour
{
	public UIButton cancelButton;
	public UIButton okButton;
	public SpriteText titleText;
	public SpriteText textOk;
	public SpriteText textCancel;
	public UITextField goldField;
	
	private GameObject m_goRoot = null;
//	private int m_nGold = 0;

	void Start()
	{
		AsLanguageManager.Instance.SetFontFromSystemLanguage( titleText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( textOk);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( textCancel);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( goldField.spriteText);
		
		goldField.SetValidationDelegate( _goldValidator);
		okButton.SetInputDelegate( _OkBtnDelegate);
		cancelButton.SetInputDelegate( _CancelBtnDelegate);
		
		SetTitleText( AsTableManager.Instance.GetTbl_String(1112));
		textOk.Text = AsTableManager.Instance.GetTbl_String(1152);
		textCancel.Text = AsTableManager.Instance.GetTbl_String(1151);
	}

	void Update()
	{
	}

	public void Open( GameObject goRoot)
	{
//		_InitGold();
//		gameObject.SetActiveRecursively( true);
//		m_goRoot = goRoot;
	}
	
	public void Close()
	{
		gameObject.SetActiveRecursively( false);

		if( null != m_goRoot)
			Destroy( m_goRoot);
	}

	public void SetTitleText( string str)
	{
		if( null == titleText)
			return;

		titleText.Text = str;
	}

	// < private
	private string _goldValidator( UITextField field, string text, ref int insPos)
	{
		return Regex.Replace( text, "[^0-9]", "");
	}

	private void _OkBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if( null == goldField)
				return;
			
			if( true == _isDigit( goldField.Text))
			{
				ulong nGold = Convert.ToUInt64( goldField.Text);
				
				if( nGold <= AsUserInfo.Instance.SavedCharStat.nGold)
					AsCommonSender.SendTradeRegistrationGold( nGold);
				else
					AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(28), eCHATTYPE.eCHATTYPE_SYSTEM);
//					AsMessageManager.Instance.InsertMessage( AsTableManager.Instance.GetTbl_String(28));
			}
			else
			{
//				AsMessageManager.Instance.InsertMessage( AsTableManager.Instance.GetTbl_String(70));
				AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(70), eCHATTYPE.eCHATTYPE_SYSTEM);
			}

			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			Close();
		}
	}

	private void _CancelBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			Close();
		}
	}

	private void _InitGold()
	{
		if( null == goldField)
			return;
		
		goldField.Text = "";
	}
	
	private bool _isDigit( string str)
	{
		if( str == null)
			return false;
		return System.Text.RegularExpressions.Regex.IsMatch( str, "^\\d+$");
	}
	
	private bool _isInt( string str)
	{
		if( str == null)
			return false;
		return System.Text.RegularExpressions.Regex.IsMatch( str, @"^[+-]?\d*$");
	}
	 
	private bool _isDouble( string str)
	{
		if( str == null)
			return false;
		return System.Text.RegularExpressions.Regex.IsMatch( str, @"^[+-]?\d*( \.?\d*)$");
	}
	// private >
}
