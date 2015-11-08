using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class AsReNameDlg : MonoBehaviour 
{
	public enum eReNameType
	{
		Character = 0,
		Guild,
		Pet//$yde
	}
	
	private eReNameType	m_renameType;
	public eReNameType ReNameType
	{
		get{return m_renameType;}
	}
	
	public UIButton 	m_BtnOk;
	public UIButton 	m_BtnCancel;
	public SpriteText 	m_TextTitle;
	public SpriteText 	m_TextDesc;
	public UITextField 	textField = null;
	
	private GameObject m_goRoot = null;
	private int m_nSlotIndex = 0;
	
	private string m_strEngChar = "abcdefghijklmnopqrstuvwxyz";
	
	void Start()
	{
	}
	
	void Update()
	{
	}
	
	public void Open(GameObject goRoot, int nSlotIndex , eReNameType renameType)
	{
		m_renameType = renameType;
		
		_Init(renameType);
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
	private void _Init(eReNameType renameType)
	{
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextTitle);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextDesc);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_BtnOk.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_BtnCancel.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( textField);
		
		switch( renameType )
		{
		case eReNameType.Character:
			ApplyControlString_Character();
			break;
			
		case eReNameType.Guild:
			ApplyControlString_Guild();
			break;
			
		case eReNameType.Pet:
			ApplyControlString_Pet();
			break;
		}

		m_BtnOk.SetInputDelegate( _BtnDelegate_Ok);
		m_BtnCancel.SetInputDelegate( _BtnDelegate_Cancel);
		textField.SetFocusDelegate( _FocusDelegate);
		textField.SetValidationDelegate( _ValidationDelegate);
		
		textField.Text = "";
		textField.spriteText.maxWidth = 20.0f;
	}
	
	private void ApplyControlString_Character()
	{
		m_TextTitle.Text = AsTableManager.Instance.GetTbl_String( 1888);
		m_TextDesc.Text = AsTableManager.Instance.GetTbl_String( 1889);
		m_BtnOk.Text = AsTableManager.Instance.GetTbl_String( 1152);
		m_BtnCancel.Text = AsTableManager.Instance.GetTbl_String( 1151);
	}
	
	private void ApplyControlString_Guild()
	{
		m_TextTitle.Text = AsTableManager.Instance.GetTbl_String( 1887);
		m_TextDesc.Text = AsTableManager.Instance.GetTbl_String( 1889);
		m_BtnOk.Text = AsTableManager.Instance.GetTbl_String( 1152);
		m_BtnCancel.Text = AsTableManager.Instance.GetTbl_String( 1151);
	}
	
	private void ApplyControlString_Pet()//$yde
	{
		m_TextTitle.Text = AsTableManager.Instance.GetTbl_String( 2210);
		m_TextDesc.Text = AsTableManager.Instance.GetTbl_String( 1889);
		m_BtnOk.Text = AsTableManager.Instance.GetTbl_String( 1152);
		m_BtnCancel.Text = AsTableManager.Instance.GetTbl_String( 1151);
		
		byteCountMax = 18;
		charCountMax = 6;
	}

	private void _BtnDelegate_Ok(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			
			if( textField.Text.Length > 0 && textField.Text.IndexOf( '/') != 0)
			{
				if( true == ChangeNameCheck( textField.Text))
				{
					switch( m_renameType )
					{
					case eReNameType.Character:
						_SendReName_Character(textField.Text);
						break;
						
					case eReNameType.Guild:
						_SendReName_Guild(textField.Text);
						break;
						
					case eReNameType.Pet:
						_SendReName_Pet(textField.Text);
						Close();
						break;
					}
				}
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
	
	private void _SendReName_Character(string strReName)
	{
		body_CS_CHAR_NAME_CHANGE changeName = new body_CS_CHAR_NAME_CHANGE( strReName , m_nSlotIndex );
		byte[] bytes = changeName.ClassToPacketBytes();
		AsCommonSender.Send(bytes);
	}

	private void _SendReName_Guild(string strReName)
	{
		body_CS_GUILD_NAME_CHANGE changeName = new body_CS_GUILD_NAME_CHANGE( strReName , m_nSlotIndex );
		byte[] bytes = changeName.ClassToPacketBytes();
		AsCommonSender.Send(bytes);
	}
	
	private void _SendReName_Pet(string strReName)//$yde
	{
		AsPetManager.Instance.Send_PetNameChange( strReName);
	}
	
	private void _FocusDelegate(UITextField field)
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6003_EFF_Input", Vector3.zero, false);
		field.Text = string.Empty;
		Input.imeCompositionMode = IMECompositionMode.On;
	}
	
	int byteCountMax = 24;
	int charCountMax = 12;
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
			if( ( byteCount <= byteCountMax) && ( charCount <= charCountMax)) //$yde
				break;

			text = text.Remove( text.Length - 1);
		}

		return text;
	}
	// private >
	private bool ChangeNameCheck( string str )
	{
		switch( m_renameType )
		{
		case eReNameType.Character:
			if( str == AsUserInfo.Instance.SavedCharStat.charName_ )
			{
				AsEventNotifyMgr.Instance.CenterNotify.AddMessage(AsTableManager.Instance.GetTbl_String(1791));
				return false;
			}
			break;
			
		case eReNameType.Guild:
			if( str == AsUserInfo.Instance.SavedCharStat.guildName_ )
			{
				AsEventNotifyMgr.Instance.CenterNotify.AddMessage(AsTableManager.Instance.GetTbl_String(1791));
				return false;
			}
			break;
			
		case eReNameType.Pet://$yde
			if( str == AsUserInfo.Instance.SavedCharStat.guildName_ )
			{
				AsEventNotifyMgr.Instance.CenterNotify.AddMessage(AsTableManager.Instance.GetTbl_String(1791));
				return false;
			}
			break;
		}
		
		return SyntaxCheck(str);
	}

	private bool SyntaxCheck( string str)
	{
		char[] arrStr = str.ToCharArray();
		foreach( char c in arrStr)
		{
			if( ' ' == c)
			{
				AsEventNotifyMgr.Instance.CenterNotify.AddMessage(AsTableManager.Instance.GetTbl_String(244));
				return false;
			}
			
			if( false == SymbolCheck(c))
			{
				AsEventNotifyMgr.Instance.CenterNotify.AddMessage(AsTableManager.Instance.GetTbl_String(243));
				return false;
			}
		}
		
		if( false == AsTableManager.Instance.TextFiltering_Name( str))
		{
			AsEventNotifyMgr.Instance.CenterNotify.AddMessage(AsTableManager.Instance.GetTbl_String(245));
			return false;
		}
		
		return true;
	}
	
	private bool SymbolCheck( char c)
	{
		return AsUtil.CheckCharFromLanguageType( c);
	}
		
}
