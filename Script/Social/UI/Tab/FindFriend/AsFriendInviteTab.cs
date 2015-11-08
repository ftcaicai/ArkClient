using UnityEngine;
using System.Collections;
using System;

public delegate void FriendAcceptBtnFunc( System.Object _obj);

public class AsFriendInviteTab : AsSocialTab
{
	public UIScrollList m_list = null;
	public GameObject m_objChoiceItem = null;
	public SpriteText noFriend = null;
	public UIButton m_FindFriendBtn = null;
	public UIButton m_PrevPageBtn = null;
	public UIButton m_NextPageBtn = null;
	public int DRAG_SENSITIVITY = 5;
	public SpriteText m_PageText = null;

	private int m_CurPage = 0;
	private int m_MaxPage = 0;
	private int m_OldPage = -1;

	override public void Init()
	{
		m_CurPage = 0;
	}

	public override UIScrollList getList()
	{
		return m_list;
	}

	// Use this for initialization
	void Start()
	{
		noFriend.gameObject.SetActiveRecursively( true);
		noFriend.Text = AsTableManager.Instance.GetTbl_String(1470);
		m_FindFriendBtn.Text = AsTableManager.Instance.GetTbl_String(1202);

		m_FindFriendBtn.SetInputDelegate( FindFriendBtnDelegate);
		m_PrevPageBtn.SetInputDelegate( PrevPageDelegate);
		m_NextPageBtn.SetInputDelegate( NextPageDelegate);
	}

	public void FriendAcceptBtnChangeDel( System.Object _obj)
	{
		if( m_list.Count == 1 && m_CurPage == 0)
			Initialize();
		else
		{
			if( m_list.Count == 1)
				AsCommonSender.SendSocialUiScroll( AsUserInfo.Instance.LoginUserUniqueKey, eSOCIAL_UI_TYPE.eSOCIAL_UI_FRIEND_APPLY, m_CurPage-1, false);	//#20206.
			else
				AsCommonSender.SendSocialUiScroll( AsUserInfo.Instance.LoginUserUniqueKey, eSOCIAL_UI_TYPE.eSOCIAL_UI_FRIEND_APPLY, m_CurPage, false);	//#20206.
		}
	}

	// Update is called once per frame
	void Update()
	{
	}

	private void FindFriendBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			AsSocialManager.Instance.LoadBlockDlg( true);
		}
	}

	void Initialize()
	{
		m_OldPage = 0;
		m_CurPage = 0;
		m_PageText.Text = "1/1";
		m_list.ClearList( true);

		noFriend.gameObject.SetActiveRecursively( true);
	}

	void OnEnable()
	{
		Initialize();
	}

	public void SetFriendList( body1_SC_FRIEND_LIST list)
	{
		m_MaxPage = list.nMaxPage;
		if( m_CurPage + 1 > m_MaxPage)
		{
			m_CurPage = m_MaxPage -1;
			AsCommonSender.SendSocialUiScroll( AsUserInfo.Instance.LoginUserUniqueKey, eSOCIAL_UI_TYPE.eSOCIAL_UI_FRIEND_APPLY, m_CurPage, false);
			return;
		}
		m_PageText.Text = string.Format( "{0}/{1}", m_CurPage + 1, m_MaxPage);

		m_list.ClearList( true);
		if( list.body == null) return;

		foreach( body2_SC_FRIEND_LIST data in list.body)
		{
			UIListItem item = m_list.CreateItem( m_objChoiceItem) as UIListItem;

			AsFriendInvite friendItem = item.gameObject.GetComponent<AsFriendInvite>();
			friendItem.SetFriendData( data, FriendAcceptBtnChangeDel);
		}

		noFriend.gameObject.SetActiveRecursively( 0 == list.nCnt);
	}

	private void PrevPageDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			Debug.Log( "OnOnlineFilterBtn");

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
			Debug.Log( "OnNextPageBtn");

			m_CurPage++;
			if( m_MaxPage <= m_CurPage + 1)
				m_CurPage = m_MaxPage - 1;

			RequestCurPageMember();
		}
	}

	private void RequestCurPageMember()
	{
		if( m_OldPage == m_CurPage)
			return;

		AsCommonSender.SendSocialUiScroll( AsUserInfo.Instance.LoginUserUniqueKey, eSOCIAL_UI_TYPE.eSOCIAL_UI_FRIEND_APPLY, m_CurPage, false);
		m_OldPage = m_CurPage;
	}
}
