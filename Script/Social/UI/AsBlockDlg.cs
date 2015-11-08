using UnityEngine;
using System.Collections;

public class AsBlockDlg : MonoBehaviour
{
	public UITextField m_NameEdit = null;
	public UIButton m_OkBtn = null;
	public UIButton m_CancelBtn = null;
	public SpriteText m_TitleText = null;
	public SpriteText m_MessageText = null;
	public SpriteText m_WarnningText = null;

	private bool m_isFriendInvite = false;

	// Use this for initialization
	void Start()
	{
		m_OkBtn.SetInputDelegate( OkBtnDelegate);
		m_CancelBtn.SetInputDelegate( CancelBtnDelegate);
		m_NameEdit.SetValidationDelegate( OnValidateName);
		m_NameEdit.Text = "";
	}

	// Update is called once per frame
	void Update()
	{
	}

	void Clear()
	{
	}

	void SetVisible( GameObject obj, bool visible)
	{
		obj.SetActiveRecursively( visible);
		obj.active = visible;
	}

	public void Close()
	{
		Clear();
		gameObject.SetActiveRecursively( false);
	}

	public bool Open( bool isFriendInvite)
	{
		m_isFriendInvite = isFriendInvite;

		if( isFriendInvite)
		{
			m_TitleText.Text = AsTableManager.Instance. GetTbl_String(1203);
			m_MessageText.Text = AsTableManager.Instance. GetTbl_String(1200);
			m_WarnningText.Text = AsTableManager.Instance. GetTbl_String(1265);
			m_OkBtn.Text = AsTableManager.Instance. GetTbl_String(1198);
			m_CancelBtn.Text = AsTableManager.Instance. GetTbl_String(1151);
		}
		else
		{
			m_TitleText.Text = AsTableManager.Instance. GetTbl_String(1203);
			m_MessageText.Text = AsTableManager.Instance. GetTbl_String(1195);
			m_WarnningText.Text = AsTableManager.Instance. GetTbl_String(1265);
			m_OkBtn.Text = AsTableManager.Instance. GetTbl_String(1197);
			m_CancelBtn.Text = AsTableManager.Instance. GetTbl_String(1151);
		}

		m_NameEdit.Text = "";

		gameObject.SetActiveRecursively( true);
		SetVisible ( m_WarnningText.gameObject, false);

		return true;
	}

	private void OkBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			if( m_NameEdit.Text.Length == 0)
			{
				Close();
				return;
			}
			if( false == AsUtil.CheckCharacterName( m_NameEdit.Text))
				return;

			if( m_isFriendInvite)
				AsCommonSender.SendFriendInvite( m_NameEdit.Text);
			else
				AsCommonSender.SendBlockOutInsert( m_NameEdit.Text);

			Close();
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
			if( byteCount <= AsGameDefine.MAX_NAME_LEN)
				break;
			
			text = text.Remove( text.Length - 1);
		}
		// #22671
		int index =  text.IndexOf('\'');
		if(-1 != index)
			text = text.Remove( index);	
		
		return text;
	}
}
