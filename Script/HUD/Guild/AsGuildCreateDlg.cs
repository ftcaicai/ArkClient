using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class AsGuildCreateDlg : MonoBehaviour
{
	[SerializeField]SpriteText title = null;
	[SerializeField]SpriteText guide1 = null;
	[SerializeField]SpriteText guide2 = null;
	[SerializeField]UITextField nameField = null;
	[SerializeField]UIButton cancel = null;
	[SerializeField]UIButton confirm = null;
	
	private string m_strEngChar = "abcdefghijklmnopqrstuvwxyz";
	private GameObject parent = null;
	public GameObject Parent
	{
		set	{ parent = value; }
	}
	
	// Use this for initialization
	void Start()
	{
		title.Text = AsTableManager.Instance.GetTbl_String(1267);
		guide1.Text = AsTableManager.Instance.GetTbl_String(4262);
		guide2.Text = AsTableManager.Instance.GetTbl_String(1271);
		cancel.Text = AsTableManager.Instance.GetTbl_String(1151);
		confirm.Text = AsTableManager.Instance.GetTbl_String(1152);
		nameField.Text = string.Empty;
		nameField.SetValidationDelegate( GuildNameValidateCallback );
		nameField.maxLength = 0;
	}
	
	string GuildNameValidateCallback( UITextField field , string text , ref int insPos)
	{
		text = Regex.Replace( text , "'" , "" );
		
		while(true)
		{
			int byteCount = System.Text.UTF8Encoding.UTF8.GetByteCount( text );
			int charCount = System.Text.UTF8Encoding.UTF8.GetCharCount( System.Text.UTF8Encoding.UTF8.GetBytes(text) );
			if( (byteCount <= 24) && (charCount <= 12) )
				break;
			
			text = text.Remove( text.Length - 1 );
		}
		
		return text;
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	private bool SyntaxCheck( string str)
	{
		char[] arrStr = str.ToCharArray();
		foreach( char c in arrStr)
		{
			if( ' ' == c)
			{
				AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(244), eCHATTYPE.eCHATTYPE_SYSTEM);
				return false;
			}
			
			if( false == SymbolCheck(c))
			{
				AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(243), eCHATTYPE.eCHATTYPE_SYSTEM);
				return false;
			}
		}
		
		if( false == AsTableManager.Instance.TextFiltering_Name( str))
		{
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(245), eCHATTYPE.eCHATTYPE_SYSTEM);
			return false;
		}
		
		return true;
	}
	
	private bool SymbolCheck( char c)
	{
		return AsUtil.CheckCharFromLanguageType( c);
	}
	
	private void OnCancelBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		Close();
	}
	
	private void OnConfirmBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		if( 0 == nameField.Text.Length)
			return;
		
		if( false == SyntaxCheck( nameField.Text))
			return;

		body_CS_GUILD_CREATE guildCreate = new body_CS_GUILD_CREATE( nameField.Text);
		byte[] packet = guildCreate.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( packet);
	
		Close();
	}
	
	public void Close()
	{
		parent.BroadcastMessage( "CloseCreateDlg", SendMessageOptions.DontRequireReceiver);
	}
}
