using UnityEngine;
using System.Collections;

public class AsSocialCloneDlg : MonoBehaviour
{
	public enum eSocialTab
	{
		NONE = -1,
		Info = 0,
		Friend,
		Max
	}

	public SpriteText m_DlgTitle = null;
	public UIButton m_CloseBtn = null;
	public UIPanelTab[] m_TabBtn = null;
	public AsSocialTab[] m_Panels = null;
	private eSocialTab m_TabState = eSocialTab.NONE;
	private uint m_nUserUniqKey;

	void SetLanguageText()
	{
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_DlgTitle);
		m_DlgTitle.Text = AsTableManager.Instance.GetTbl_String(1203);
		for( int i = 0; i < (int)eSocialTab.Max; ++i)
		{
			AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TabBtn[i].spriteText);
		}
		m_TabBtn[(int)eSocialTab.Info].Text = AsTableManager.Instance.GetTbl_String(1180);
		m_TabBtn[(int)eSocialTab.Friend].Text = AsTableManager.Instance.GetTbl_String(1181);
	}

	// Use this for initialization
	void Start()
	{
		SetLanguageText();
		m_TabBtn[(int)eSocialTab.Info].SetInputDelegate( InfoBtnDelegate);
		m_TabBtn[(int)eSocialTab.Friend].SetInputDelegate( FriendBtnDelegate);

		m_CloseBtn.SetInputDelegate( CloseBtnDelegate);

		//initialize UIPanelTab( True( 0),False( 1),Disabled( 2))
		foreach( UIPanelTab tab in m_TabBtn)
			tab.SetState(1);

		m_TabBtn[0].SetState(0);

		SetVisibleTab( (int)eSocialTab.Info);
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
		AsSocialManager.Instance.SocialUI.CloseSocailCloneDlg();
	}

	public bool Open()
	{
		foreach( AsSocialTab panels in m_Panels)
		{
			panels.IsClone = true;
			panels.Init();
		}
		m_nUserUniqKey = AsSocialManager.Instance.SocialData.FriendItem.nUserUniqKey;
		AsCommonSender.SendSocialInfo( m_nUserUniqKey);
		gameObject.SetActiveRecursively( true);
		SetVisibleTab( (int)eSocialTab.Info);

		return true;
	}

	private void InfoBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP && m_TabState != eSocialTab.Info)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			AsCommonSender.SendSocialInfo( m_nUserUniqKey); //2013.02.26
			SetVisibleTab( (int)eSocialTab.Info);
		}
	}

	private void FriendBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP && m_TabState != eSocialTab.Friend)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			SetVisibleTab( (int)eSocialTab.Friend);
			AsFriendTab friendPanel = m_Panels[ (int)eSocialTab.Friend ].gameObject.GetComponent<AsFriendTab>();
			AsCommonSender.SendFriendList( m_nUserUniqKey,friendPanel.OnlineMember ); //2013.11.25.
			friendPanel.SetVisibleSocialMenu( false);
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

	private void SetVisibleTab( int index)
	{
		foreach( AsSocialTab panel in m_Panels)
			panel.gameObject.SetActiveRecursively( false);

		m_Panels[index].gameObject.SetActiveRecursively( true);
		m_TabState = ( eSocialTab)index;
		
		//initialize UIPanelTab( True( 0),False( 1),Disabled( 2))
		foreach( UIPanelTab tab in m_TabBtn) //#21546
			tab.SetState(1);

		m_TabBtn[index].SetState(0);
		
	}

	public void SetHistoryList( body1_SC_SOCIAL_HISTORY list)
	{
		AsInfoTab infoPanel = m_Panels[ (int)eSocialTab.Info ].gameObject.GetComponent<AsInfoTab>();
		infoPanel.SetHistoryList( list);
	}

	public void SetFriendList( body1_SC_FRIEND_LIST list)
	{
		AsFriendTab friendPanel = m_Panels[ (int)eSocialTab.Friend ].gameObject.GetComponent<AsFriendTab>();
		friendPanel.SetFriendList( list);
	}
}
