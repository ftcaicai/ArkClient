using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class AsPartyPRDlg : MonoBehaviour
{
	public UITextField m_PREdit = null;
	public UIButton m_OkBtn = null;
	public UIButton m_CancelBtn = null;
	public SpriteText m_TitleText = null;

	// Use this for initialization
	void Start()
	{
		m_OkBtn.SetInputDelegate( OkBtnDelegate);
		m_CancelBtn.SetInputDelegate( CancelBtnDelegate);

		m_PREdit.Text = "";
		m_TitleText.Text = AsTableManager.Instance.GetTbl_String(1725);
		m_OkBtn.Text = AsTableManager.Instance.GetTbl_String(1152);
		m_CancelBtn.Text = AsTableManager.Instance.GetTbl_String(1151);


		m_PREdit.spriteText.Text = AsTableManager.Instance.GetTbl_String(1726);
		m_PREdit.SetFocusDelegate( OnFocusPR);
		m_PREdit.SetValidationDelegate( OnValidateName);
	}

	// Update is called once per frame
	void Update()
	{
	}

	public void Close()
	{
		gameObject.SetActiveRecursively( false);
	}

	public bool Open()
	{
		m_PREdit.Text = "";
		m_PREdit.spriteText.Text = AsTableManager.Instance.GetTbl_String(1726);
		gameObject.SetActiveRecursively( true);
		return true;
	}

	private void OkBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			if( 0 >= m_PREdit.Text.Length)
				return;

			if( AsTableManager.Instance.TextFiltering_Name( m_PREdit.Text) == true)
			{
				AsPartySender.SendPartyNoticeOnOff( true, m_PREdit.Text);
				Close();
			}
			else
			{
				m_PREdit.Text = AsTableManager.Instance.GetTbl_String(1726);
				AsNotify.Instance.MessageBox( "", AsTableManager.Instance.GetTbl_String(364), this, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_ERROR);
			}
		}
	}

	private void CancelBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			Close();
		}
	}

	private string OnValidateName( UITextField field, string text, ref int insPos)
	{
		while( true)
		{
			int byteCount = System.Text.UTF8Encoding.UTF8.GetByteCount( text);
//			int charCount = System.Text.UTF8Encoding.UTF8.GetCharCount( System.Text.UTF8Encoding.UTF8.GetBytes( text));
			if( byteCount <= AsGameDefine.ePARTYNOTICE)
				break;

			text = text.Remove( text.Length - 1);
		}
		
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

	private void OnFocusPR( UITextField field)
	{
		m_PREdit.Text = string.Empty;
	}
}
