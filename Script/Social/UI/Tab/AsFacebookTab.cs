using UnityEngine;
using System.Collections;

public class AsFacebookTab : AsSocialTab
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
		m_BackBtn.Text = AsTableManager.Instance.GetTbl_String( 1132);
		noFacebook.Text = AsTableManager.Instance.GetTbl_String( 1470);
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

	public void InsertFacebookFriend( AsFaceBookFriend listData)
	{
		UIListItem item = m_list.CreateItem( m_objChoiceItem) as UIListItem;

		AsFacebookItem baseFriendItem = item.gameObject.GetComponent<AsFacebookItem>();
		if( null == baseFriendItem)
			return;

		baseFriendItem.SetFacebookFriendData( listData);
	}

	private void PrevPageDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			if( IsLoaded())
			{
				m_CurPage--;
				if( 0 > m_CurPage)
					m_CurPage = 0;

				RequestCurPageMember();
			}
		}
	}

	private void NextPageDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			if( IsLoaded())
			{
				m_CurPage++;
				if( m_MaxPage <= m_CurPage + 1)
					m_CurPage = m_MaxPage - 1;

				RequestCurPageMember();
			}
		}
	}

	private bool IsLoaded()
	{
		int count = 0;
		for( int i = 0; i < m_list.Count; i++)
		{
			UIListButton listBtn = m_list.GetItem( i) as UIListButton;
			AsFacebookItem baseFriendItem = listBtn.gameObject.GetComponent<AsFacebookItem>();
			if( baseFriendItem.IsLoaded)
				count++;
		}

		if( m_list.Count != count)
			return false;

		return true;
	}

	public void SetFacebookFriendList()
	{
		Resources.UnloadUnusedAssets();

		m_MaxPage = AsSocialManager.Instance.SocialData.GetFaceBookFriendMaxPage();
		m_PageText.Text = string.Format( "{0}/{1}", m_CurPage + 1, m_MaxPage);

		m_list.ClearList( true);

		AsFaceBookFriend[] list = AsSocialManager.Instance.SocialData.GetFaceBookFriendByPage( m_CurPage);
		foreach( AsFaceBookFriend data in list)
		{
			if( data != null)
				InsertFacebookFriend( data);
		}

		m_list.ScrollListTo( 0.0f);

		noFacebook.gameObject.SetActiveRecursively( 0 == m_list.Count);
	}

	private void RequestCurPageMember()
	{
		if( m_OldPage == m_CurPage)
			return;

		m_OldPage = m_CurPage;
		SetFacebookFriendList();
	}
}
