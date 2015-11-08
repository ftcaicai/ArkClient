using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class AsChatServerDlg : MonoBehaviour
{
	public UIButton m_BtnOk;
	public UIButton m_BtnCancel;
	public SpriteText m_TextTitle;
	public SpriteText m_TextDesc;
	public UITextField textField = null;
	
	private GameObject m_goRoot = null;
	private int m_nSlotIndex = 0;
	
	void Start()
	{
	}
	
	void Update()
	{
	}
	
	public void Open(GameObject goRoot, int nSlotIndex)
	{
		_Init();
		gameObject.SetActiveRecursively( true);
		m_goRoot = goRoot;
		m_nSlotIndex = nSlotIndex;
	}
	
	public void Close()
	{
		gameObject.SetActiveRecursively( false);
		if( null != m_goRoot)
			Destroy( m_goRoot);
	}
	
	// < private
	private void _Init()
	{
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextTitle);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextDesc);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_BtnOk.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_BtnCancel.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( textField);
		
		m_TextTitle.Text = AsTableManager.Instance.GetTbl_String( 2120);
		m_TextDesc.Text = AsTableManager.Instance.GetTbl_String( 2121);
		m_BtnOk.Text = AsTableManager.Instance.GetTbl_String( 1152);
		m_BtnCancel.Text = AsTableManager.Instance.GetTbl_String( 1027);

		m_BtnOk.SetInputDelegate( _BtnDelegate_Ok);
		m_BtnCancel.SetInputDelegate( _BtnDelegate_Cancel);
		textField.SetFocusDelegate( _FocusDelegate);
		textField.SetValidationDelegate( _ValidationDelegate);
		
		textField.Text = "";
		textField.spriteText.maxWidth = 20.0f;
	}

	private void _BtnDelegate_Ok(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			
			if( textField.Text.Length > 0 && textField.Text.IndexOf( '/') != 0)
			{
				_SendWorldChat( textField.Text);
				Close();
			}
		}
	}

	private void _BtnDelegate_Cancel(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			Close();
		}
	}
	
	private void _SendWorldChat(string strChat)
	{
		body_CS_CHAT_MESSAGE chat = new body_CS_CHAT_MESSAGE( strChat, eCHATTYPE.eCHATTYPE_SERVER, true, m_nSlotIndex);
		byte[] data = chat.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}

	private void _FocusDelegate(UITextField field)
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		field.Text = string.Empty;
		Input.imeCompositionMode = IMECompositionMode.On;
	}
	
	private string _ValidationDelegate(UITextField field, string text, ref int insPos)
	{
//		int nIndex = text.IndexOf( '\n');
//		if( -1 != nIndex)
//			text = text.Remove( nIndex);
//		
//		return text;
		text = Regex.Replace( text, "\n", "");
		
		while( true)
		{
			int byteCount = System.Text.UTF8Encoding.UTF8.GetByteCount( text);
			int charCount = System.Text.UTF8Encoding.UTF8.GetCharCount( System.Text.UTF8Encoding.UTF8.GetBytes( text));
			if( ( byteCount <= 102) && ( charCount <= 34))
				break;

			text = text.Remove( text.Length - 1);
		}

		return text;
	}
	// private >
}
