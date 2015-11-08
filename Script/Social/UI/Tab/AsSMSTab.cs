using UnityEngine;
using System.Collections;

public class AsSMSTab : AsSocialTab
{
	public UIScrollList m_list = null;
	public UIButton m_BackBtn = null;
	public GameObject m_objChoiceItem = null;
	public UIButton m_PrevPageBtn = null;
	public UIButton m_NextPageBtn = null;
	public SpriteText m_PageText = null;
	public SpriteText noFacebook = null;

	private int m_CurPage = 0;
	private int m_MaxPage = 0;
	private int m_OldPage = -1;

	override public void Init()
	{
	}

	public override UIScrollList getList()
	{
		return m_list;
	}

	// Use this for initialization
	void Start()
	{
		m_BackBtn.SetInputDelegate( BackBtnDelegate);

		m_BackBtn.Text = AsTableManager.Instance.GetTbl_String(1132);

		noFacebook.Text = AsTableManager.Instance.GetTbl_String(1470);
		noFacebook.gameObject.SetActiveRecursively( false);

		m_CurPage = 0;
		m_PrevPageBtn.SetInputDelegate( PrevPageDelegate);
		m_NextPageBtn.SetInputDelegate( NextPageDelegate);
	}

	// Update is called once per frame
	void Update()
	{
	}

	void OnEnable()
	{
	}

	private void BackBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			AsSocialManager.Instance.SocialUI.m_SocialDlg.VisibleInviteTab();
		}
	}

	public void SetHidden()
	{
		gameObject.SetActiveRecursively( false);
		m_BackBtn.gameObject.SetActiveRecursively( false);
	}

	public void RequestFirstPage()
	{
		m_CurPage = 0;
		m_OldPage = -1;
		RequestCurPageMember();
	}

	public void InsertSMSFriend( AsSMSFriend listData)
	{
		UIListItem item = m_list.CreateItem( m_objChoiceItem) as UIListItem;

		AsSMSItem baseFriendItem = item.gameObject.GetComponent<AsSMSItem>();
		if( null == baseFriendItem)
			return;

		baseFriendItem.SetSMSFriendData( listData);
	}

	private void PrevPageDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			m_CurPage--;
			if( 0 > m_CurPage)
				m_CurPage = 0;

			RequestCurPageMember();
		}
	}

	private void NextPageDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			m_CurPage++;
			if( m_MaxPage <= m_CurPage + 1)
				m_CurPage = m_MaxPage - 1;

			RequestCurPageMember();
		}
	}

	public void SetSMSFriendList()
	{
		m_MaxPage = AsSocialManager.Instance.SocialData.GetSMSFriendMaxPage();
		m_PageText.Text = string.Format( "{0}/{1}", m_CurPage + 1, m_MaxPage);

		m_list.ClearList( true);

		AsSMSFriend[] list = AsSocialManager.Instance.SocialData.GetSMSFriendByPage( m_CurPage);
		foreach( AsSMSFriend data in list)
		{
			if( data != null)
				InsertSMSFriend( data);
		}

		m_list.ScrollListTo( 0.0f);

		noFacebook.gameObject.SetActiveRecursively( 0 == m_list.Count);
	}

		
	private void RequestCurPageMember()
	{
		if( m_OldPage == m_CurPage)
			return;

		m_OldPage = m_CurPage;
		SetTwitterFollowerList();
	}
	
	public void InsertTwitterFollower( AsTwitterFollower listData)
	{
		UIListItem item = m_list.CreateItem( m_objChoiceItem) as UIListItem;

		AsTwitterItem baseFriendItem = item.gameObject.GetComponent<AsTwitterItem>();
		if( null == baseFriendItem)
			return;

		baseFriendItem.SetTwitterFollowerData( listData);
	}
	
	public void SetTwitterFollowerList()
	{
		m_MaxPage = AsSocialManager.Instance.SocialData.GetTwitterFollowerMaxPage();
		m_PageText.Text = string.Format( "{0}/{1}", m_CurPage + 1, m_MaxPage);

		m_list.ClearList( true);

		AsTwitterFollower[] list = AsSocialManager.Instance.SocialData.GetTwitterFollowerByPage( m_CurPage);
		foreach( AsTwitterFollower data in list)
		{
			if( data != null)
				InsertTwitterFollower(data);
		}

		m_list.ScrollListTo( 0.0f);

		noFacebook.gameObject.SetActiveRecursively( 0 == m_list.Count);
	}
}
