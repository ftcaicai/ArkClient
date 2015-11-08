using UnityEngine;
using System.Collections;

public class AsChatButtonAnimController : MonoBehaviour
{
	[SerializeField]UIButton allButton = null;
	[SerializeField]UIButton[] buttons = new UIButton[0];
	[SerializeField]SpriteText m_TextTitle_General;
	[SerializeField]SpriteText m_TextTitle_Party;
	[SerializeField]SpriteText m_TextTitle_Guild;
	[SerializeField]SpriteText m_TextTitle_Whisper;
	[SerializeField]SpriteText m_TextTitle_System;
	[SerializeField]UITextField filterField = null;
	[SerializeField]SpriteText filterText = null;
	[SerializeField]SimpleSprite filterBg = null;
	[SerializeField]UITextField inputField = null;
	[SerializeField]GameObject balloon = null;
	[SerializeField]SpriteText balloonText = null;
	
	private string m_strAll = "";
	private string m_strGeneral = "";
	private string m_strParty = "";
	private string m_strGuild = "";
	private string m_strWhisper = "";
	private string m_strSystem = "";
	
	// Use this for initialization
	void Start()
	{
		AsLanguageManager.Instance.SetFontFromSystemLanguage( allButton.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextTitle_General);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextTitle_Party);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextTitle_Guild);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextTitle_Whisper);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextTitle_System);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( balloonText);

		allButton.Text = m_strAll = AsTableManager.Instance.GetTbl_String(1173);
		m_TextTitle_General.Text = m_strGeneral = AsTableManager.Instance.GetTbl_String(1174);
		m_TextTitle_Party.Text = m_strParty = AsTableManager.Instance.GetTbl_String(1176);
		m_TextTitle_Guild.Text = m_strGuild = AsTableManager.Instance.GetTbl_String(1177);
		m_TextTitle_Whisper.Text = m_strWhisper = AsTableManager.Instance.GetTbl_String(1175);
		m_TextTitle_System.Text = m_strSystem = AsTableManager.Instance.GetTbl_String(1178);
		balloonText.Text = AsTableManager.Instance.GetTbl_String(2035);
		
		_DisableAllFocus();
		_SetColorText( allButton, AsChatManager.Instance.m_Color_All, 1);

		allButton.SetInputDelegate( OnNoFilter);
		buttons[0].SetInputDelegate( OnGeneralBtn);
		buttons[1].SetInputDelegate( OnPartyBtn);
		buttons[2].SetInputDelegate( OnGuildBtn);
		buttons[3].SetInputDelegate( OnWhisperBtn);
		buttons[4].SetInputDelegate( OnSystemBtn);
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	void OnEnable()
	{
		_DisableAllFocus();
		
		if( null == AsChatFullPanel.Instance)
		{
			_SetColorText( allButton, AsChatManager.Instance.m_Color_All, 1);
			return;
		}
		
		switch( AsChatFullPanel.Instance.filterType)
		{
		case CHAT_FILTER_TYPE.None:
			_SetColorText( allButton, AsChatManager.Instance.m_Color_All, 1);
			break;
		case CHAT_FILTER_TYPE.General:
			_SetColorText( buttons[0], AsChatManager.Instance.m_Color_General, 2);
			break;
		case CHAT_FILTER_TYPE.Party:
			_SetColorText( buttons[1], AsChatManager.Instance.m_Color_Party, 3);
			break;
		case CHAT_FILTER_TYPE.Guild:
			_SetColorText( buttons[2], AsChatManager.Instance.m_Color_Guild, 4);
			break;
		case CHAT_FILTER_TYPE.Whisper:
			_SetColorText( buttons[3], AsChatManager.Instance.m_Color_Whisper, 5);
			break;
		case CHAT_FILTER_TYPE.System:
			_SetColorText( buttons[4], AsChatManager.Instance.m_Color_System, 6);
			break;
		}
	}
	
	private void OnNoFilter( ref POINTER_INFO _ptr)
	{
		if( _ptr.evt != POINTER_INFO.INPUT_EVENT.TAP)
			return;
		
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		AsChatFullPanel.Instance.Init( CHAT_FILTER_TYPE.None);
		_DisableAllFocus();
		_SetColorText( allButton, AsChatManager.Instance.m_Color_All, 1);
	}
	
	private void OnGeneralBtn( ref POINTER_INFO _ptr)
	{
		if( _ptr.evt != POINTER_INFO.INPUT_EVENT.TAP)
			return;
		
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		AsChatFullPanel.Instance.Init( CHAT_FILTER_TYPE.General);
		_DisableAllFocus();
		_SetColorText( buttons[0], AsChatManager.Instance.m_Color_General, 2);
	}
	
	private void OnPartyBtn( ref POINTER_INFO _ptr)
	{
		if( _ptr.evt != POINTER_INFO.INPUT_EVENT.TAP)
			return;
		
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		AsChatFullPanel.Instance.Init( CHAT_FILTER_TYPE.Party);
		_DisableAllFocus();
		_SetColorText( buttons[1], AsChatManager.Instance.m_Color_Party, 3);
	}
	
	private void OnGuildBtn( ref POINTER_INFO _ptr)
	{
		if( _ptr.evt != POINTER_INFO.INPUT_EVENT.TAP)
			return;
		
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		AsChatFullPanel.Instance.Init( CHAT_FILTER_TYPE.Guild);
		_DisableAllFocus();
		_SetColorText( buttons[2], AsChatManager.Instance.m_Color_Guild, 4);
	}
	
	private void OnWhisperBtn( ref POINTER_INFO _ptr)
	{
		if( _ptr.evt != POINTER_INFO.INPUT_EVENT.TAP)
			return;
		
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		AsChatFullPanel.Instance.Init( CHAT_FILTER_TYPE.Whisper);
		_DisableAllFocus();
		_SetColorText( buttons[3], AsChatManager.Instance.m_Color_Whisper, 5);
	}
	
	private void OnSystemBtn( ref POINTER_INFO _ptr)
	{
		if( _ptr.evt != POINTER_INFO.INPUT_EVENT.TAP)
			return;
		
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		AsChatFullPanel.Instance.Init( CHAT_FILTER_TYPE.System);
		_DisableAllFocus();
		_SetColorText( buttons[4], AsChatManager.Instance.m_Color_System, 6);
	}
	
	private void _DisableAllFocus()
	{
		_SetColorText( allButton, AsChatManager.Instance.m_Color_Disable, 1);
		_SetColorText( buttons[0], AsChatManager.Instance.m_Color_Disable, 2);
		_SetColorText( buttons[1], AsChatManager.Instance.m_Color_Disable, 3);
		_SetColorText( buttons[2], AsChatManager.Instance.m_Color_Disable, 4);
		_SetColorText( buttons[3], AsChatManager.Instance.m_Color_Disable, 5);
		_SetColorText( buttons[4], AsChatManager.Instance.m_Color_Disable, 6);
	}
	
	private void _SetColorText( UIButton btn, Color color, int nFlag)
	{
		string strRes = "";

//		btn.spriteTextColor = color;
		btn.spriteText.Color = color;
		filterField.spriteText.Color = color;
		filterText.Color = color;
		inputField.spriteText.Color = color;

		switch( nFlag)
		{
		case 1:	strRes = m_strAll;	break;
		case 2:	strRes = m_strGeneral;	break;
		case 3:	strRes = m_strParty;	break;
		case 4:	strRes = m_strGuild;	break;
		case 5:	strRes = m_strWhisper;	break;
		case 6:	strRes = m_strSystem;	break;
		}

		bool isWhisper = ( 5 == nFlag) ? true : false;
//		filterField.gameObject.SetActiveRecursively( isWhisper);
		filterField.gameObject.SetActiveRecursively( true);	// must be always active
		filterField.controlIsEnabled = isWhisper;
		
		balloon.SetActiveRecursively( isWhisper);
		balloonText.gameObject.SetActiveRecursively( isWhisper);
		filterText.gameObject.SetActiveRecursively( !isWhisper);
		filterBg.gameObject.SetActiveRecursively( true);

		if( strRes.Length > 0)
		{
			btn.Text = strRes;
			if( 5 == nFlag)
			{
				string latestWhisper = PlayerPrefs.GetString( "LatestWhisper");
				if( string.Empty != latestWhisper)
					strRes = latestWhisper;
			}
			filterField.Text = strRes;
			filterText.Text = strRes;
		}
	}

	public void SetWhisperMode( string name)
	{
		AsChatFullPanel.Instance.Init( CHAT_FILTER_TYPE.Whisper);
		_DisableAllFocus();
		_SetColorText( buttons[3], AsChatManager.Instance.m_Color_Whisper, 5);
		filterField.Text = name;
	}
}
