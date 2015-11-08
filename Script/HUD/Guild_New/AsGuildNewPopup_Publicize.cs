using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class AsGuildNewPopup_Publicize : MonoBehaviour 
{
	[SerializeField] SpriteText 			m_txtTitle = null;
	[SerializeField] SpriteText 			m_txtGuide = null;
	[SerializeField] UITextField 		m_tfEdit = null;
	[SerializeField] UIButton 			m_btnConfirm = null;
	[SerializeField] UIButton 			m_btnCancel = null;

	private string 							m_prevPublicize = string.Empty;

	// Use this for initialization
	void Start () 
	{
		m_txtTitle.Text = AsTableManager.Instance.GetTbl_String(4261);
		m_txtGuide.Text = AsTableManager.Instance.GetTbl_String(1700);

		m_tfEdit.SetFocusDelegate( OnFocusPublicize);
		m_tfEdit.SetValidationDelegate( GuildPublicizeValidateCallback);
	}

	string GuildPublicizeValidateCallback( UITextField field, string text, ref int insPos)
	{
		text = Regex.Replace( text, "'", "");
		
		while( true)
		{
			int byteCount = System.Text.UTF8Encoding.UTF8.GetByteCount( text);
			int charCount = System.Text.UTF8Encoding.UTF8.GetCharCount( System.Text.UTF8Encoding.UTF8.GetBytes( text));
			if( ( byteCount <= 60) && ( charCount <= 20))
				break;
			
			text = text.Remove( text.Length - 1);
		}
		
		return text;
	}
	
	private void OnFocusPublicize( UITextField field)
	{
		#if UNITY_IPHONE	
		field.autoCorrect = false;
		#endif
		
		string szPR = AsTableManager.Instance.GetTbl_String(1700);
		if( 0 == szPR.CompareTo( field.Text))
			field.Text = "";
	}

	private void OnDisable()
	{
		Close();
	}

	public void Close()
	{
		GameObject.DestroyImmediate( gameObject );
	}

	void	OnConfirm()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		if( 0 == m_tfEdit.Text.Length)
			return;
		
		if( false == AsTableManager.Instance.TextFiltering_Guild( m_tfEdit.Text))
		{
			AsNotify.Instance.MessageBox( string.Empty, AsTableManager.Instance.GetTbl_String(364), AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			return;
		}
		
		if( 0 == string.Compare( m_tfEdit.Text, m_prevPublicize))
		{
			Debug.Log( "pr.Text == prevPR");
			return;
		}
		
		m_prevPublicize = m_tfEdit.Text;
		
		body_CS_GUILD_PUBLICIZE publicize = new body_CS_GUILD_PUBLICIZE( m_tfEdit.Text);
		byte[] packet = publicize.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( packet);
		
		AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(380), eCHATTYPE.eCHATTYPE_SYSTEM);

		Close();
	}

	void	OnCancel()
	{
		Close();
	}

	// Update is called once per frame
	void Update () 
	{
	
	}
}
