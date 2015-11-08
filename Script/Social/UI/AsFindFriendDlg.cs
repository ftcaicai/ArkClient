using UnityEngine;
using System.Collections;


public class AsFindFriendDlg : MonoBehaviour
{
	public enum eFindFriendTab
	{
		NONE = -1,
		Reward = 0,
		Recommend,
		RandomFriend,
		FriendInvite,
		Max
	}

	public SpriteText m_DlgTitle = null;
	public UIButton m_CloseBtn = null;
	public UIPanelTab[] m_TabBtn = null;
	public AsSocialTab[] m_Panels = null;

	private eFindFriendTab m_TabState = eFindFriendTab.NONE;

	void SetLanguageText()
	{
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_DlgTitle);
		m_DlgTitle.Text = AsTableManager.Instance. GetTbl_String(1189);

		for( int i = 0; i < (int)eFindFriendTab.Max; ++i)
		{
			AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TabBtn[i].spriteText);
		}

		m_TabBtn[(int)eFindFriendTab.Reward].Text = AsTableManager.Instance. GetTbl_String(1560);
		m_TabBtn[(int)eFindFriendTab.Recommend].Text = AsTableManager.Instance. GetTbl_String(1601);
		m_TabBtn[(int)eFindFriendTab.RandomFriend].Text = AsTableManager.Instance. GetTbl_String(1184);
		m_TabBtn[(int)eFindFriendTab.FriendInvite].Text = AsTableManager.Instance. GetTbl_String(1706);
	}

	// Use this for initialization
	void Start()
	{
		SetLanguageText();
		m_CloseBtn.SetInputDelegate( CloseBtnDelegate);

		m_TabBtn[(int)eFindFriendTab.Reward].SetInputDelegate( RewardBtnDelegate);
		m_TabBtn[(int)eFindFriendTab.RandomFriend].SetInputDelegate( RandomFriendBtnDelegate);
		m_TabBtn[(int)eFindFriendTab.FriendInvite].SetInputDelegate( FriendInviteBtnDelegate);
		m_TabBtn[(int)eFindFriendTab.Recommend].SetInputDelegate( RecommendBtnDelegate);
	}

	void OnDisable()
	{
		m_TabState = eFindFriendTab.NONE;
	}

	private void FriendInviteBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP && m_TabState != eFindFriendTab.FriendInvite)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			SetVisibleTab( (int)eFindFriendTab.FriendInvite);
			AsCommonSender.SendSocialUiScroll( AsUserInfo.Instance.LoginUserUniqueKey, eSOCIAL_UI_TYPE.eSOCIAL_UI_FRIEND_APPLY, 0, false);
		}
	}

	private void RecommendBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP && m_TabState != eFindFriendTab.Recommend)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			SetVisibleTab( (int)eFindFriendTab.Recommend);
			AsCommonSender.SendSocialUiScroll( AsUserInfo.Instance.LoginUserUniqueKey, eSOCIAL_UI_TYPE.eSOCIAL_UI_RECOMMEND, 0, false);
		}
	}

	private void RandomFriendBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP && m_TabState != eFindFriendTab.RandomFriend)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			SetVisibleTab( (int)eFindFriendTab.RandomFriend);

			QuestTutorialMgr.Instance.ProcessQuestTutorialMsg( new QuestTutorialMsgInfo( QuestTutorialMsg.TAP_RANDOM_FRIEND));

			AsCommonSender.SendFriendRandom();

			if( ArkQuestmanager.instance.CheckHaveOpenUIType( OpenUIType.OPEN_FRIEND_RANDOM) != null)
				AsCommonSender.SendClearOpneUI( OpenUIType.OPEN_FRIEND_RANDOM);
		}
	}

	private void RewardBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP && m_TabState != eFindFriendTab.Reward)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			SetVisibleTab( (int)eFindFriendTab.Reward);
			AsCommonSender.SendGameInviteList();
		}
	}

	private void FindFriendBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			AsSocialManager.Instance.OpenBlockDlg( true);
		}
	}

	private void SetVisibleTab( int index)
	{
		foreach( AsSocialTab panel in m_Panels)
			panel.gameObject.SetActiveRecursively( false);

		m_Panels[index].gameObject.SetActiveRecursively( true);
		m_TabState = ( eFindFriendTab)index;
	}

	private void TabStateUpdate( int index)
	{
		foreach( UIPanelTab tab in m_TabBtn)
			tab.SetState( 1);

		m_TabBtn[ index].SetState( 0);
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
		QuestTutorialMgr.Instance.ProcessQuestTutorialMsg( new QuestTutorialMsgInfo( QuestTutorialMsg.CLOSE_FINDFRIEND));
		Clear();
		gameObject.SetActiveRecursively( false);
	}

	public bool Open()
	{
		gameObject.SetActiveRecursively( true);

		int		nOpenVisibleTab = (int)eFindFriendTab.Reward;

		if (AsReviewConditionManager.Instance.IsReviewCondition (eREVIEW_CONDITION.FRIEND_REWARD) == false) 
		{
			nOpenVisibleTab = (int)eFindFriendTab.RandomFriend;

			m_TabBtn[(int)eFindFriendTab.RandomFriend].gameObject.transform.localPosition = m_TabBtn[(int)eFindFriendTab.Reward].gameObject.transform.localPosition;
			m_TabBtn[(int)eFindFriendTab.FriendInvite].gameObject.transform.localPosition = m_TabBtn[(int)eFindFriendTab.Recommend].gameObject.transform.localPosition;
			
			m_TabBtn[(int)eFindFriendTab.Reward].gameObject.SetActive(false);
			m_TabBtn[(int)eFindFriendTab.Recommend].gameObject.SetActive(false);
		}

		SetVisibleTab( nOpenVisibleTab );
		TabStateUpdate( nOpenVisibleTab );
		AsCommonSender.SendGameInviteList();
		QuestTutorialMgr.Instance.ProcessQuestTutorialMsg( new QuestTutorialMsgInfo( QuestTutorialMsg.OPEN_FINDFRIEND));

		if (AsHudDlgMgr.Instance.IsOpenMainMenuDlg)
			AsHudDlgMgr.Instance.MainMenuDlg.Close();

		return true;
	}

	private void CloseBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			AsSocialManager.Instance.SocialUI.CloseBlockDlg();
			Close();
		}
	}

	public void SetFriendRandomList( body1_SC_FRIEND_RANDOM list)
	{
		if( m_TabState == eFindFriendTab.RandomFriend)
		{
			AsRandomTab randomPanel = m_Panels[ (int)eFindFriendTab.RandomFriend].gameObject.GetComponent<AsRandomTab>();
			randomPanel.SetRandomFriendList( list);
		}
	}

	public void SetRecommedList( body_SC_SOCIAL_RECOMMEND list)
	{
		if( m_TabState == eFindFriendTab.Recommend)
		{
			AsRecommendTab recommendPanel = m_Panels[ (int)eFindFriendTab.Recommend].gameObject.GetComponent<AsRecommendTab>();
			recommendPanel.SetData( list);
		}
	}

	public void SetFriendApplyList( body1_SC_FRIEND_LIST list)
	{
		if( m_TabState == eFindFriendTab.FriendInvite)
		{
			AsFriendInviteTab friendInvitePanel = m_Panels[ (int)eFindFriendTab.FriendInvite].gameObject.GetComponent<AsFriendInviteTab>();
			friendInvitePanel.SetFriendList( list);
		}
	}

	public void SetGameInviteRewardList( body_SC_GAME_INVITE_LIST_RESULT list)
	{
		if( m_TabState == eFindFriendTab.Reward)
		{
			AsInviteRewardTab rewardPanel = m_Panels[ (int)AsFindFriendDlg.eFindFriendTab.Reward].gameObject.GetComponent<AsInviteRewardTab>();
			rewardPanel.SetRewardList( list);
		}
	}
}
