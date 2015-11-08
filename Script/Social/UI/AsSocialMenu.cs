//#define _SOCIAL_KAKAO_VER
using UnityEngine;
using System.Collections;


public class AsSocialMenu : MonoBehaviour
{
	public enum eSocailMenu
	{
		NONE = -1,
		UserInfo = 0,	/// <summary>
		/// 대상 정보 창 출력
		/// </summary>/
		SocialInfo,/// <summary>
		/// 대상 소셜 창 출력
		/// </summary>
		Whisper,
		PartyInvite,
		GuildInvite,
		DeleteFriend,
		MoveToFriend,
		SendMail,

		MaxMenu
	}

	const int GRADE_MOVETOFRIEND = 4;
	public 	SpriteText m_CharNameText = null;
	public UIButton m_CloseBtn = null;
	public UIButton[] m_MenuBtn = null;

	private body2_SC_FRIEND_LIST m_data = null;

	void SetLanguageText()
	{
		for( int i = 0; i < (int)eSocailMenu.MaxMenu; ++i)
		{
			AsLanguageManager.Instance.SetFontFromSystemLanguage( m_MenuBtn[i].spriteText);
		}

		m_MenuBtn[(int)eSocailMenu.UserInfo ].Text = AsTableManager.Instance.GetTbl_String(1161);
		m_MenuBtn[(int)eSocailMenu.SocialInfo ].Text = AsTableManager.Instance.GetTbl_String(1191);
		m_MenuBtn[(int)eSocailMenu.Whisper ].Text = AsTableManager.Instance.GetTbl_String(1162);
		m_MenuBtn[(int)eSocailMenu.PartyInvite ].Text = AsTableManager.Instance.GetTbl_String(1163);
		m_MenuBtn[(int)eSocailMenu.GuildInvite ].Text = AsTableManager.Instance.GetTbl_String(1165);
		m_MenuBtn[(int)eSocailMenu.DeleteFriend].Text = AsTableManager.Instance.GetTbl_String(1192);
		m_MenuBtn[(int)eSocailMenu.MoveToFriend].Text = AsTableManager.Instance.GetTbl_String(1193);
		m_MenuBtn[(int)eSocailMenu.SendMail].Text = AsTableManager.Instance.GetTbl_String(824);
	}

	// Use this for initialization
	void Start()
	{
		SetLanguageText();
		m_MenuBtn[(int)eSocailMenu.UserInfo ].SetInputDelegate( UserInfoBtnDelegate);
		m_MenuBtn[(int)eSocailMenu.SocialInfo ].SetInputDelegate( SocialInfoBtnDelegate);
		m_MenuBtn[(int)eSocailMenu.Whisper ].SetInputDelegate( WhisperBtnDelegate);
		m_MenuBtn[(int)eSocailMenu.PartyInvite ].SetInputDelegate( PartyInviteBtnDelegate);
		m_MenuBtn[(int)eSocailMenu.GuildInvite ].SetInputDelegate( GuildInviteBtnDelegate);
		m_MenuBtn[(int)eSocailMenu.DeleteFriend].SetInputDelegate( DeleteFriendBtnDelegate);
		m_MenuBtn[(int)eSocailMenu.MoveToFriend].SetInputDelegate( MoveToFriendBtnDelegate);
		m_MenuBtn[(int)eSocailMenu.SendMail].SetInputDelegate( SendMailBtnDelegate);

		m_CloseBtn.SetInputDelegate( CloseBtnDelegate);
	}

	// Update is called once per frame
	void Update()
	{
	}

	//Button Setting
	private void SetButtonState()
	{
		for( int i = 0; i < (int)eSocailMenu.MaxMenu; ++i)
		{
			AsUtil.SetButtonState(m_MenuBtn[i], UIButton.CONTROL_STATE.NORMAL);
			//m_MenuBtn[i].SetControlState( UIButton.CONTROL_STATE.NORMAL);
			//m_MenuBtn[i].controlIsEnabled = true;
		}

		//IsOnline
		if( m_data.nSessionIdx != 0)
		{
			if( m_data.nFriendlyLevel <= GRADE_MOVETOFRIEND)
				AsUtil.SetButtonState(m_MenuBtn[(int)eSocailMenu.MoveToFriend], UIButton.CONTROL_STATE.DISABLED);
				//m_MenuBtn[(int)eSocailMenu.MoveToFriend].SetControlState( UIButton.CONTROL_STATE.DISABLED);

			if( AsUserInfo.Instance.GuildData != null)
			{
				bool bGuildInvite = ( eGUILDPERMISSION.eGUILDPERMISSION_INVITE == ( eGUILDPERMISSION.eGUILDPERMISSION_INVITE & AsUserInfo.Instance.GuildData.ePermission)) ? true : false;
				if( !bGuildInvite)
					AsUtil.SetButtonState(m_MenuBtn[(int)eSocailMenu.GuildInvite], UIButton.CONTROL_STATE.DISABLED);
				//m_MenuBtn[(int)eSocailMenu.GuildInvite].SetControlState( UIButton.CONTROL_STATE.DISABLED);
			}
			else
			{
				AsUtil.SetButtonState(m_MenuBtn[(int)eSocailMenu.GuildInvite], UIButton.CONTROL_STATE.DISABLED);
				//m_MenuBtn[(int)eSocailMenu.GuildInvite].SetControlState( UIButton.CONTROL_STATE.DISABLED);
			}
		}
		else
		{
			AsUtil.SetButtonState(m_MenuBtn[(int)eSocailMenu.Whisper], UIButton.CONTROL_STATE.DISABLED);
			AsUtil.SetButtonState(m_MenuBtn[(int)eSocailMenu.PartyInvite], UIButton.CONTROL_STATE.DISABLED);
			AsUtil.SetButtonState(m_MenuBtn[(int)eSocailMenu.GuildInvite], UIButton.CONTROL_STATE.DISABLED);
			AsUtil.SetButtonState(m_MenuBtn[(int)eSocailMenu.MoveToFriend], UIButton.CONTROL_STATE.DISABLED);

			//m_MenuBtn[(int)eSocailMenu.Whisper].SetControlState( UIButton.CONTROL_STATE.DISABLED);
			//m_MenuBtn[(int)eSocailMenu.PartyInvite].SetControlState( UIButton.CONTROL_STATE.DISABLED);
			//m_MenuBtn[(int)eSocailMenu.GuildInvite].SetControlState( UIButton.CONTROL_STATE.DISABLED);
			//m_MenuBtn[(int)eSocailMenu.MoveToFriend].SetControlState( UIButton.CONTROL_STATE.DISABLED);
		}
	}

	public void SetFriendItem( body2_SC_FRIEND_LIST data)
	{
		m_data = data;
		m_CharNameText.Text = data.szCharName;

		SetButtonState();
	}

	public void Close()
	{
		gameObject.SetActiveRecursively( false);
	//	AsHudDlgMgr.Instance.CloseRecommendInfoDlg();	// #17479
	}

	public bool IsOnline()
	{
		if( m_data.nSessionIdx == 0)			// 0. offline, !0. online
		{
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126),AsTableManager.Instance.GetTbl_String(182));
			return false;
		}

		return true;
	}

	public bool Open()
	{
		gameObject.SetActiveRecursively( true);
		return true;
	}

	private void CloseBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			Close();
		}
	}

	private void UserInfoBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsCommonSender.SendOtherUserInfo( (int)eOTHER_INFO_TYPE.eOTHER_INFO_TYPE_SOCIAL_FRIEND,m_data.nSessionIdx, m_data.nCharUniqKey, m_data.nUserUniqKey);
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			Close();
		}
	}

	private void SocialInfoBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			//setp01.datasetting
			AsSocialManager.Instance.SocialData.FriendItem = m_data;
			//setp02.Ui Open
			AsSocialManager.Instance.OpenSocailCloneDlg();
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			Close();
		}
	}

	private void WhisperBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if( IsOnline())
			{
				AsChatFullPanel.Instance.OpenWhisperMode( m_data.szCharName);

//				string defaultText = "/w " + m_data.szCharName + " ";
//				AsChatManager.Instance.SetDefaultText( defaultText);
			}

			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			Close();
		}
	}

	private void PartyInviteBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if( IsOnline())
				AsPartySender.SendPartyInvite( m_data.nSessionIdx, m_data.nCharUniqKey);

			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			Close();
		}
	}

	private void GuildInviteBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if( IsOnline() && AsUserInfo.Instance.GuildData != null)
			{
				bool bGuildInvite	 = ( eGUILDPERMISSION.eGUILDPERMISSION_INVITE == ( eGUILDPERMISSION.eGUILDPERMISSION_INVITE & AsUserInfo.Instance.GuildData.ePermission)) ? true : false;
				if( bGuildInvite)
				{
					body_CS_GUILD_INVITE guildInvite = new body_CS_GUILD_INVITE( m_data.szCharName);
					byte[] packet = guildInvite.ClassToPacketBytes();
					AsNetworkMessageHandler.Instance.Send( packet);
				}

				AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
				Close();
			}
		}
	}

	private void DeleteFriendBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			FriendDeleteMessage();

			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			Close();
		}
	}

	private void MoveToFriendBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if( IsOnline())
			{
				AsCommonSender.SendFriendWarp( m_data.nUserUniqKey);
			}

			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			Close();
		}
	}

	private void SendMailBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			AsSocialManager.Instance.SocialUI.CloseFindFriendDlg();
			AsSocialManager.Instance.SocialUI.CloseSocialStoreDlg();
			AsSocialManager.Instance.SocialUI.CloseSocailCloneDlg();//#19720

			AsHudDlgMgr.Instance.OpenWritePostBoxDlg( m_data.szCharName);

			Close();
		}
	}

	public void FriendDeleteMessage()
	{
		string strMsg = string.Format( AsTableManager.Instance.GetTbl_String(175), m_data.szCharName);
		AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(1203),strMsg, this, "OnMsgBox_FriendDelete_Ok", "OnMsgBox_FriendDelete_Cancel", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);
	}

	public void OnMsgBox_FriendDelete_Ok()
	{
		AsCommonSender.SendFriendClear( m_data.nUserUniqKey);
	}

	public void OnMsgBox_FriendDelete_Cancel()
	{
	}
}
