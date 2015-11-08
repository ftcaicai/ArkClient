using UnityEngine;
using System.Collections;

public class AsSocialDlg : MonoBehaviour
{
	public enum eSocialTab
	{
		NONE = -1,

		Friend = 0,
		Invite,
		Block,
		Info,
		Max
	}

	public SpriteText m_DlgTitle = null;
	public UIButton m_CloseBtn = null;
	public UIPanelTab[] m_TabBtn = null;
	public AsSocialTab[] m_Panels = null;
	private eSocialTab m_TabState = eSocialTab.NONE;

	void SetLanguageText()
	{
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_DlgTitle);
		m_DlgTitle.Text  =  AsTableManager.Instance.GetTbl_String(1203);
		for( int i = 0; i < (int)eSocialTab.Max; ++i)
		{
			AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TabBtn[i].spriteText);
		}

		m_TabBtn[(int)eSocialTab.Friend].Text = AsTableManager.Instance.GetTbl_String(1181);
		m_TabBtn[(int)eSocialTab.Invite].Text = AsTableManager.Instance.GetTbl_String(1263);
		m_TabBtn[(int)eSocialTab.Block].Text = AsTableManager.Instance.GetTbl_String(1182);
		m_TabBtn[(int)eSocialTab.Info].Text = AsTableManager.Instance.GetTbl_String(1180);
	}

	// Use this for initialization
	void Start()
	{
		SetLanguageText();
	//	m_TabBtn[(int)eSocialTab.Invite].controlIsEnabled = false;
		m_TabBtn[(int)eSocialTab.Friend].SetInputDelegate( FriendBtnDelegate);
		m_TabBtn[(int)eSocialTab.Invite].SetInputDelegate( InviteBtnDelegate);
		m_TabBtn[(int)eSocialTab.Block].SetInputDelegate( BlockBtnDelegate);
		m_TabBtn[(int)eSocialTab.Info].SetInputDelegate( InfoBtnDelegate);

		m_CloseBtn.SetInputDelegate( CloseBtnDelegate);
	}

	// Update is called once per frame
	void Update()
	{
	}

	void Clear()
	{
	}

	public void Close()
	{
		AsSocialManager.Instance.SocialUI.CloseSocailDlg();
		AsSocialManager.Instance.SocialUI.CloseSocailCloneDlg();
		AsSocialManager.Instance.SocialUI.CloseFindFriendDlg();
		AsSocialManager.Instance.SocialUI.CloseBlockDlg();
		AsSocialManager.Instance.SocialUI.CloseSocialStoreDlg();
		QuestTutorialMgr.Instance.ProcessQuestTutorialMsg( new QuestTutorialMsgInfo( QuestTutorialMsg.CLOSE_SOCIAL_DLG));
	}

	public bool Open()
	{
		foreach( AsSocialTab panels in m_Panels)
		{
			panels.IsClone = false;
			panels.Init();
		}

		AsCommonSender.SendFriendList( AsUserInfo.Instance.LoginUserUniqueKey, false); //2013.11.25.
		//AsCommonSender.SendSocialInfo( AsUserInfo.Instance.LoginUserUniqueKey); //2013.02.28
		gameObject.SetActiveRecursively( true);

		int		nOpenVisibleTab = (int)eSocialTab.Friend;

		SetVisibleTab( nOpenVisibleTab );
		TabStateUpdate( nOpenVisibleTab );

		QuestTutorialMgr.Instance.ProcessQuestTutorialMsg( new QuestTutorialMsgInfo( QuestTutorialMsg.OPEN_SOCIAL));

		return true;
	}

	private void InfoBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP && m_TabState != eSocialTab.Info)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			SetVisibleTab( (int)eSocialTab.Info);

			AsCommonSender.SendSocialInfo( AsUserInfo.Instance.LoginUserUniqueKey);
		}
	}

	private void FriendBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP && m_TabState != eSocialTab.Friend)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			SetVisibleTab( (int)eSocialTab.Friend);

			AsFriendTab friendPanel = m_Panels[ (int)eSocialTab.Friend ].gameObject.GetComponent<AsFriendTab>();
			AsCommonSender.SendFriendList( AsUserInfo.Instance.LoginUserUniqueKey, friendPanel.OnlineMember); //2013.11.25.

			friendPanel.SetVisibleSocialMenu( false);
		}
	}

	private void InviteBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP && m_TabState != eSocialTab.Invite)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			SetVisibleTab( (int)eSocialTab.Invite);
			AsCommonSender.SendGameInviteList();
		}
	}

	private void BlockBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP && m_TabState != eSocialTab.Block)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			SetVisibleTab( (int)eSocialTab.Block);
			SetBlockOutList();
		}
	}

	private void CloseBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			Close();
		}
	}

	void SetVisible( GameObject obj, bool visible)
	{
		obj.SetActiveRecursively( visible);
		obj.active = visible;
	}

	private void SetVisibleTab( int index)
	{
		AsInviteTab invitePanel = m_Panels[ (int)eSocialTab.Invite].gameObject.GetComponent<AsInviteTab>();
		invitePanel.m_FaceBookPanels.SetHidden();
		invitePanel.m_SMSPanels.SetHidden();

		foreach( AsSocialTab panel in m_Panels)
			panel.gameObject.SetActive( false);

		m_Panels[index].gameObject.SetActive( true);

		m_TabState = ( eSocialTab)index;
	}

	private void TabStateUpdate( int index)
	{
		foreach( UIPanelTab tab in m_TabBtn)
			tab.SetState( 1);

		m_TabBtn[ index].SetState( 0);
	}

	public void SetHistoryList( body1_SC_SOCIAL_HISTORY list)
	{
		if( m_TabState == eSocialTab.Info)
		{
			AsInfoTab infoPanel = m_Panels[ (int)eSocialTab.Info ].gameObject.GetComponent<AsInfoTab>();
			infoPanel.SetHistoryList( list);
		}
	}

	public void SetFriendList( body1_SC_FRIEND_LIST list)
	{
		if( m_TabState == eSocialTab.Friend)
		{
			AsFriendTab friendPanel = m_Panels[ (int)eSocialTab.Friend ].gameObject.GetComponent<AsFriendTab>();
			friendPanel.SetFriendList( list);
		}
	}

	public void SetBlockOutList()
	{
		if( m_TabState == eSocialTab.Block)
		{
			AsBlockTab blockOutPanel = m_Panels[ (int)eSocialTab.Block ].gameObject.GetComponent<AsBlockTab>();
			blockOutPanel.SetBlockOutList();
		}
	}

	public void SetBlockOutList( body1_SC_BLOCKOUT_LIST list)
	{
		if( m_TabState == eSocialTab.Block)
		{
			AsBlockTab blockOutPanel = m_Panels[ (int)eSocialTab.Block ].gameObject.GetComponent<AsBlockTab>();
			blockOutPanel.SetBlockOutList( list);
		}
	}

	public void SetGameInviteRewardList( body_SC_GAME_INVITE_LIST_RESULT list)
	{
		if( m_TabState == eSocialTab.Invite)
		{
			AsInviteTab invitePanel = m_Panels[ (int)eSocialTab.Invite ].gameObject.GetComponent<AsInviteTab>();
			invitePanel.SetGameInviteRewardList( list);
		}
	}

	public void VisibleInviteTab()
	{
		SetVisibleTab( (int)eSocialTab.Invite);
		TabStateUpdate( (int)eSocialTab.Invite);
	}
}
