using UnityEngine;
using System.Collections;
//Social UI
public class AsSocialUI : MonoBehaviour
{
	public int SOCIAL_NPC_ID = 999;
	public AsSocialDlg m_SocialDlg = null;
	public AsFindFriendDlg m_FindFriendDlg = null;
	public AsBlockDlg m_BlockDlg = null;
	public AsSocialCloneDlg m_SocialCloneDlg = null;
	public AsSocialStoreDlg m_SocialStoreDlg = null;

	public GameObject m_ObjectSocialDlg = null;
	public GameObject m_ObjectSocialCloneDlg = null;
	public GameObject m_ObjectFindFriendDlg = null;
	public GameObject m_ObjectBlockDlg = null;
	public GameObject m_ObjectSocialStoreDlg = null;

	public enum eLoadPortraitState
	{
		NONE = 0,
		DEFAULT,
		KAKAO_IMAGE,
		MAX
	}

	private Texture2D m_Myportrait = null;
	public Texture2D Myportrait
	{
		get { return m_Myportrait; }
		set { m_Myportrait = value; }
	}
	private eLoadPortraitState m_LoadPortraitState = eLoadPortraitState.NONE;
	public eLoadPortraitState LoadPortraitState
	{
		get { return m_LoadPortraitState; }
	}

	public AsFindFriendDlg FindFriendDlg
	{
		get { return m_FindFriendDlg; }
		set { m_FindFriendDlg = value; }
	}

	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
	}

	public void DestroyMyportrait()
	{
		if( Myportrait != null) DestroyImmediate( Myportrait);
	}
	void OnDestroy ()
	{
		DestroyMyportrait();
	}
	public bool IsOpenSocailDlg
	{
		get
		{
			if( null == m_SocialDlg)
				return false;
			return m_SocialDlg.gameObject.active;
		}
	}

	public bool IsOpenSocailCloneDlg
	{
		get
		{
			if( null == m_SocialCloneDlg)
				return false;
			return m_SocialCloneDlg.gameObject.active;
		}
	}

	public bool IsOpenFindFriendDlg
	{
		get
		{
			if( null == m_FindFriendDlg)
				return false;
			return m_FindFriendDlg.gameObject.active;
		}
	}

	public bool IsOpenBlockDlg
	{
		get
		{
			if( null == m_BlockDlg)
				return false;
			return m_BlockDlg.gameObject.active;
		}
	}

	public bool IsOpenSocailStoreDlg
	{
		get
		{
			if( null == m_SocialStoreDlg)
				return false;
			return m_SocialStoreDlg.gameObject.active;
		}
	}

	public void CloseSocailDlg()
	{
		if( null == m_SocialDlg)
			return;

		Destroy( m_ObjectSocialDlg);
		m_SocialDlg = null;
	}

	public void CloseSocailCloneDlg()
	{
		if( null == m_SocialCloneDlg)
			return;

		Destroy( m_ObjectSocialCloneDlg);
		m_SocialCloneDlg = null;
	}

	public void CloseFindFriendDlg()
	{
		if( null == m_FindFriendDlg)
			return;

		QuestTutorialMgr.Instance.ProcessQuestTutorialMsg( new QuestTutorialMsgInfo( QuestTutorialMsg.CLOSE_FINDFRIEND));

		Destroy( m_ObjectFindFriendDlg);
		m_FindFriendDlg = null;
	}

	public void CloseBlockDlg()
	{
		if( null == m_BlockDlg)
			return;

		Destroy( m_ObjectBlockDlg);
		m_BlockDlg = null;
	}

	public void CloseSocialStoreDlg()
	{
		if( null == m_SocialStoreDlg)
			return;

		Destroy( m_ObjectSocialStoreDlg);
		m_SocialStoreDlg = null;
		AsNotify.Instance.CloseAllMessageBox();
	}

	public void OpenSocialDlg()
	{
	//	if( m_FindFriendDlg != null)	m_FindFriendDlg.Close();
		if( m_BlockDlg != null)
			m_BlockDlg.Close();

		if( m_SocialDlg != null)
			m_SocialDlg.Open();
		
		AsPetManager.Instance.ClosePetDlgByOtherWindow(); //$yde
	}

	public void OpenSocialCloneDlg()
	{
		if( m_SocialCloneDlg != null)
			m_SocialCloneDlg.Open();
		else
			Debug.LogError( "OpenSocialCloneDlg[m_SocialCloneDlg = null] Err!!!");
	}

	public void OpenFindFriendDlg()
	{
		if( m_FindFriendDlg != null)
		{
			if( true == AsHudDlgMgr.Instance.IsOpenPlayerStatus)
				AsHudDlgMgr.Instance.ClosePlayerStatus();
			m_FindFriendDlg.Open();
		}
		else
		{
			Debug.LogError( "OpenFindFriendDlg[m_FindFriendDlg = null] Err!!!");
		}
	}

	public void OpenBlockDlg( bool isFriendInvite)
	{
		if( m_BlockDlg != null)
			m_BlockDlg.Open( isFriendInvite);
		else
			Debug.LogError( "OpenBlockDlg[m_BlockDlg = null] Err!!!");
	}

	public void OpenSocialStoreDlg()
	{
		if( true == AsHudDlgMgr.Instance.IsOpenPlayerStatus)
			AsHudDlgMgr.Instance.ClosePlayerStatus();

		if( true == AsHudDlgMgr.Instance.IsOpenedPostBox)
			AsHudDlgMgr.Instance.ClosePostBoxDlg();

		if( m_SocialStoreDlg != null)
		{
			m_SocialStoreDlg.InitilizeStore( SOCIAL_NPC_ID, AsEntityManager.Instance.UserEntity.GetProperty<eCLASS>( eComponentProperty.CLASS));
			QuestTutorialMgr.Instance.ProcessQuestTutorialMsg( new QuestTutorialMsgInfo( QuestTutorialMsg.OPEN_SOCIAL_STORE));

			if( ArkQuestmanager.instance.CheckHaveOpenUIType( OpenUIType.OPEN_SOCIAL_STORE) != null)
				AsCommonSender.SendClearOpneUI( OpenUIType.OPEN_SOCIAL_STORE);
		}
		else
		{
			Debug.LogError( "OpenSocailStoreDlg[m_SocialStoreDlg = null] Err!!!");
		}
	}

	public void SetSocialInfo( body_SC_SOCIAL_INFO socialInfo)
	{
		if( m_SocialDlg == null)
			return;

		AsInfoTab infoPanel = m_SocialDlg.m_Panels[ ( int)AsSocialDlg.eSocialTab.Info ].gameObject.GetComponent<AsInfoTab>();
		infoPanel.SetSocialInfo( socialInfo);
	}

	public void RequestFirstPageByFriendList()
	{
		if( m_SocialDlg == null)
			return;

		AsFriendTab friendPanel = m_SocialDlg.m_Panels[ ( int)AsSocialDlg.eSocialTab.Friend ].gameObject.GetComponent<AsFriendTab>();
		friendPanel.RequestFirstPage();
	}

	public void RequestCurrentPageByFriendList()
	{
		if( m_SocialDlg == null)
			return;

		AsFriendTab friendPanel = m_SocialDlg.m_Panels[ ( int)AsSocialDlg.eSocialTab.Friend ].gameObject.GetComponent<AsFriendTab>();
		friendPanel.RequestCurrentPage();
	}

	public void SetFriendDelete( body_SC_FRIEND_DELETE friendDelete)
	{
		if( m_SocialDlg == null)
			return;

		AsFriendTab friendPanel = m_SocialDlg.m_Panels[ ( int)AsSocialDlg.eSocialTab.Friend ].gameObject.GetComponent<AsFriendTab>();
		friendPanel.RequestFirstPage();
	}

	public void SetNotice( string notice)
	{
		if( m_SocialDlg == null)
			return;

		AsInfoTab infoPanel = m_SocialDlg.m_Panels[ ( int)AsSocialDlg.eSocialTab.Info ].gameObject.GetComponent<AsInfoTab>();
		infoPanel.SetNotice( notice);
	}

	public void SetSocialPoint( int nPoint)
	{
		if( m_SocialDlg == null)
			return;

		AsInfoTab infoPanel = m_SocialDlg.m_Panels[ ( int)AsSocialDlg.eSocialTab.Info ].gameObject.GetComponent<AsInfoTab>();
		infoPanel.SetSocialPoint( nPoint);
	}

	public void SetHistoryList( body1_SC_SOCIAL_HISTORY list)
	{
		m_SocialDlg.SetHistoryList( list);
	}

	public void SetFriendList( body1_SC_FRIEND_LIST list)
	{
		m_SocialDlg.SetFriendList( list);
	}

	public void SetBlockOutList()
	{
		if( m_SocialDlg != null)
			m_SocialDlg.SetBlockOutList();
	}

	public void SetBlockOutList( body1_SC_BLOCKOUT_LIST list)
	{
		if( m_SocialDlg != null)
			m_SocialDlg.SetBlockOutList( list);
	}

	public void SetFacebookFriendList()
	{
		if( null != m_SocialDlg)
		{
			AsInviteTab invitePanel = m_SocialDlg.m_Panels[ ( int)AsSocialDlg.eSocialTab.Invite ].gameObject.GetComponent<AsInviteTab>();
			invitePanel.m_FaceBookPanels.gameObject.SetActiveRecursively( true);
			invitePanel.m_FaceBookPanels.SetFacebookFriendList();
		}
	}

	public void SetSMSFriendList()
	{
		if( null != m_SocialDlg)
		{
			AsInviteTab invitePanel = m_SocialDlg.m_Panels[ ( int)AsSocialDlg.eSocialTab.Invite ].gameObject.GetComponent<AsInviteTab>();
			invitePanel.m_SMSPanels.gameObject.SetActiveRecursively( true);
			invitePanel.m_SMSPanels.SetSMSFriendList();
		}
	}
	
	public void SetTwitterFollowerList()
	{
		if( null != m_SocialDlg)
		{
			AsInviteTab invitePanel = m_SocialDlg.m_Panels[ ( int)AsSocialDlg.eSocialTab.Invite ].gameObject.GetComponent<AsInviteTab>();
			invitePanel.m_SMSPanels.gameObject.SetActiveRecursively( true);
			invitePanel.m_SMSPanels.SetTwitterFollowerList();
		}
	}
	
		
	public void SetGameInviteRewardList( body_SC_GAME_INVITE_LIST_RESULT list)
	{
		if( m_SocialDlg != null)
			m_SocialDlg.SetGameInviteRewardList( list);

		if( null != m_FindFriendDlg)
			m_FindFriendDlg.SetGameInviteRewardList( list);
	}

	public void SetFriendRandomList( body1_SC_FRIEND_RANDOM list)
	{
		if( null != m_FindFriendDlg)
			m_FindFriendDlg.SetFriendRandomList( list);
	}

	public void SetRecommedList( body_SC_SOCIAL_RECOMMEND list)
	{
		if( null != m_FindFriendDlg)
			m_FindFriendDlg.SetRecommedList( list);
	}

	public void SetFriendApplyList( body1_SC_FRIEND_LIST list)
	{
		if( null != m_FindFriendDlg)
			m_FindFriendDlg.SetFriendApplyList( list);
	}

	public void RequestCurPageHistory()
	{

		if( m_SocialDlg == null)
			return;

		AsInfoTab infoPanel = m_SocialDlg.m_Panels[ ( int)AsSocialDlg.eSocialTab.Info ].gameObject.GetComponent<AsInfoTab>();

		if( null != infoPanel)
			infoPanel.RequestCurPageHistory();
	}

	#region --Clone
	public void SetSocialCloneInfo( body_SC_SOCIAL_INFO socialInfo)
	{
		if( m_SocialCloneDlg == null)
			return;

		AsInfoTab infoPanel = m_SocialCloneDlg.m_Panels[ ( int)AsSocialCloneDlg.eSocialTab.Info ].gameObject.GetComponent<AsInfoTab>();
		infoPanel.SetSocialInfo( socialInfo);
	}

	public void SetSocialCloneHistoryList( body1_SC_SOCIAL_HISTORY list)
	{
		m_SocialCloneDlg.SetHistoryList( list);
	}

	public void SetSocialCloneFriendList( body1_SC_FRIEND_LIST list)
	{
		m_SocialCloneDlg.SetFriendList( list);
	}
	#endregion -Clone
}
