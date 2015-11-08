using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class AsPartyEditDlg : MonoBehaviour , IEventListener
{
	public UIButton m_LeaveBtn;
	public UIButton m_CloseBtn;
	public UIButton m_EditBtn;
	public UIButton m_PartyPRBtn;
	public SpriteText m_TextTitle = null;
	public SpriteText m_PartyName = null;
	public AsPartyEditSolt[] m_PartyEditSolt;

	private bool m_IsVisible = false;
	
	void Clear()
	{
	}

	public void Close()
	{
		Clear();

		gameObject.SetActiveRecursively( false);
		gameObject.active = false;

		m_IsVisible = false;
	}

	public void Open()
	{
		Clear();
		//patrymanager of PartyOption reference
		m_PartyName.Text = AsUtil.GetRealString( AsPartyManager.Instance.PartyName);

		gameObject.SetActiveRecursively( true);
		gameObject.active = true;

		m_IsVisible = true;
	//	SetPartyNotice();
		SetPartyEditSlot();
		
		if (AsHudDlgMgr.Instance.IsOpenMainMenuDlg)
			AsHudDlgMgr.Instance.MainMenuDlg.Close();
	}

	public void SetPartyNotice()
	{
		if( AsPartyManager.Instance.IsPartyNotice)
			m_PartyPRBtn.Text = AsTableManager.Instance.GetTbl_String( 1999);
		else
			m_PartyPRBtn.Text = AsTableManager.Instance.GetTbl_String( 1725);

		if( true == AsPartyManager.Instance.IsCaptain/* && AsPartyManager.Instance.PartyOption.bPublic */
			&& AsPartyManager.Instance.GetPartyMemberList().Count < AsPartyManager.Instance.PartyOption.nMaxUser)
		{
			m_PartyPRBtn.spriteText.Color = Color.black;
			m_PartyPRBtn.SetControlState( UIButton.CONTROL_STATE.NORMAL);
			m_PartyPRBtn.controlIsEnabled = true;
		}
		else
		{
			m_PartyPRBtn.spriteText.Color = Color.gray;
			m_PartyPRBtn.SetControlState( UIButton.CONTROL_STATE.DISABLED);
			m_PartyPRBtn.controlIsEnabled = false;
		}
	}

	private void DefaultSlot()
	{
		int index = AsPartyManager.Instance.GetPartyMemberList().Count;
		for( int i = index; i < AsGameDefine.MAX_PARTY_USER; ++i)
		{
			AsPartyEditSolt slot = m_PartyEditSolt[i];
			slot.SetOffLine( false);
			slot.SetCaptainImage( false);
			slot.CloseAllClassImage(); //21034
			slot.SetPortrait( false);
			slot.SetUserLevel(false);
			slot.SetEditMenuBtn( false );

			if( AsPartyManager.Instance.IsCaptain)
				slot.SetDisableMemberBtn( false);
			else
				slot.SetDisableMemberBtn( true);

			if( ( i + 1) > AsPartyManager.Instance.PartyOption.nMaxUser)
			{
				slot.m_UserName.Text = AsTableManager.Instance.GetTbl_String( 1961);
				slot.IsOpne = false;
			}
			else
			{
				slot.m_UserName.Text = AsTableManager.Instance.GetTbl_String( 1960);
				slot.IsOpne = true;
			}
		}
	}

	private void CloseAllSlot()
	{
		for( int i = 0; i < AsGameDefine.MAX_PARTY_USER; ++i)
		{
			AsPartyEditSolt slot = m_PartyEditSolt[i];
			slot.Close();
		}
	}

	public void SetPartyEditSlot()
	{
		if( false == m_IsVisible)
			return;

		int slotIndex = 0;

		SetPartyNotice();

		m_PartyName.Text = AsUtil.GetRealString( AsPartyManager.Instance.PartyName);

		AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
		if( null == userEntity)
			return;

		if( false == AsPartyManager.Instance.IsCaptain)
		{
			m_EditBtn.spriteText.Color = Color.gray;
			m_EditBtn.SetControlState( UIButton.CONTROL_STATE.DISABLED);
		}
		else
		{
			m_EditBtn.spriteText.Color = Color.black;
			m_EditBtn.SetControlState( UIButton.CONTROL_STATE.NORMAL);
		}

		foreach( KeyValuePair<uint, AS_PARTY_USER> data in AsPartyManager.Instance.GetPartyMemberList())
		{
			AS_PARTY_USER partyUser = data.Value;
			AsPartyEditSolt slot = m_PartyEditSolt[ slotIndex];

			slot.Open();
			slot.m_nSessionIdx = partyUser.nSessionIdx;
			slot.CharUniqKey = partyUser.nCharUniqKey;

			if( slot.CharUniqKey == userEntity.UniqueId)
				partyUser.nLevel = userEntity.GetProperty<int>( eComponentProperty.LEVEL);

			slot.SetUserLevel(true);//	#21571
			StringBuilder sbLv = new StringBuilder();
			sbLv.Append( "Lv.");
			sbLv.Append( partyUser.nLevel.ToString());
			slot.m_UserLevel.Text = sbLv.ToString();

			slot.m_UserName.Text = partyUser.strCharName;
			slot.SetCaptainImage( partyUser.isCaptain);
			slot.SetDisableMemberBtn( !AsPartyManager.Instance.IsCaptain);

			if( 0 == partyUser.nSessionIdx)//Zero is OffLine User
				slot.SetOffLine( true);
			else
				slot.SetOffLine( false);

			slot.SetClassImage( partyUser.eClass-1); //21034
			slot.SetDelegateImage( partyUser.nImageTableIdx);

			slot.SetMemberBtn( true);
			
			if( slot.CharUniqKey != userEntity.UniqueId)
				slot.SetEditMenuBtn(true);
			else
				slot.SetEditMenuBtn(false);
				
			slotIndex++;
		}

		DefaultSlot();
		SetPartyNotice();
	}

	// Use this for initialization
	void Start()
	{
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_PartyName);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextTitle);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_LeaveBtn.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_EditBtn.spriteText);

		m_TextTitle.Text = AsTableManager.Instance.GetTbl_String( 1957);
		m_LeaveBtn.Text = AsTableManager.Instance.GetTbl_String( 1962);
		m_EditBtn.Text = AsTableManager.Instance.GetTbl_String( 1963);
		m_PartyPRBtn.Text = AsTableManager.Instance.GetTbl_String( 1725);

		m_CloseBtn.SetInputDelegate( CloseBtnDelegate);
		m_LeaveBtn.SetInputDelegate( LeaveBtnDelegate);
		m_EditBtn.SetInputDelegate( EditBtnDelegate);
		m_PartyPRBtn.SetInputDelegate( PartyPRBtnDelegate);
		
		for( int i = 0; i < m_PartyEditSolt.Length ; ++i)
		{
			AsPartyEditSolt slot = m_PartyEditSolt[i];
			slot.m_eventListener = gameObject.GetComponent( typeof(IEventListener) ) as IEventListener;
		}
		
	}

	// Update is called once per frame
	void Update()
	{
	}

	private void CloseBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			Close();
		}
	}

	private void LeaveBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			AsPartyManager.Instance.PartyExitMsgBox();
		}
	}

	private void EditBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			AsPartyManager.Instance.PartyUI.EditPartyCreateDlg();
			Close();
		}
	}

	private void PartyPRBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			if( true == AsPartyManager.Instance.IsCaptain /* && AsPartyManager.Instance.PartyOption.bPublic*/)
			{
				if( AsPartyManager.Instance.IsPartyNotice)
					AsPartySender.SendPartyNoticeOnOff( false, "");
				else
					AsPartyManager.Instance.PartyUI.PartyPRDlg.Open();
			}
		}
	}
	
	#region event listen function
	public void ListenEvent(AsEventHeader	eventData)
	{
		if( eventData.nType == eLISTEN_EVENT.partyMenuBtnClose )
		{
			for( int i = 0; i < m_PartyEditSolt.Length ; ++i)
			{
				AsPartyEditSolt slot = m_PartyEditSolt[i];
				slot.MenuButtonClose();
			}
		}
	}
	#endregion
}
