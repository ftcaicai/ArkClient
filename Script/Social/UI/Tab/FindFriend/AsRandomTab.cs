using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsRandomTab : AsSocialTab
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

	void OnEnable()
	{
		m_OldPage = 0;
		m_CurPage = 0;
		m_PageText.Text = "1/1";
		m_list.ClearList( true);
		noFriend.gameObject.SetActiveRecursively( true);
	}

	// Use this for initialization
	void Start()
	{
		noFriend.gameObject.SetActiveRecursively( true);
		noFriend.Text = AsTableManager.Instance.GetTbl_String( 1470);
		m_FindFriendBtn.Text = AsTableManager.Instance.GetTbl_String( 1202);
		m_FindFriendBtn.SetInputDelegate( FindFriendBtnDelegate);
		m_PrevPageBtn.SetInputDelegate( PrevPageDelegate);
		m_NextPageBtn.SetInputDelegate( NextPageDelegate);
	}

	// Update is called once per frame
	void Update()
	{
	}

	public void InsertFriendData( body2_SC_FRIEND_RANDOM  listData)
	{
		UIListItem item = m_list.CreateItem( m_objChoiceItem) as UIListItem;

		AsRandomItem baseFriendItem = item.gameObject.GetComponent<AsRandomItem>();
		if( null == baseFriendItem)
			return;

		baseFriendItem.SetRandomFriendData( listData);
	}

	public void OnSelect()
	{
	}

	public void SetRandomFriendList( body1_SC_FRIEND_RANDOM  list)
	{
		m_MaxPage =  list.nMaxPage;
		m_PageText.Text = string.Format( "{0}/{1}", m_CurPage + 1, m_MaxPage);

		m_list.ClearList( true);

		if( list.body != null)
		{
			foreach( body2_SC_FRIEND_RANDOM data in list.body)
				InsertFriendData( data);
		}
		m_list.ScrollListTo( 0.0f);
		noFriend.gameObject.SetActiveRecursively( 0 == m_list.Count);
	}

	private void PrevPageDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			Debug.Log( "PrevPageDelegate");

			m_CurPage--;
			if( 0 > m_CurPage)
				m_CurPage = 0;

			RequestCurPageMember();
		}
	}

	private void FindFriendBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			AsSocialManager.Instance.LoadBlockDlg( true);
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

		AsCommonSender.SendSocialUiScroll( AsUserInfo.Instance.LoginUserUniqueKey, eSOCIAL_UI_TYPE.eSOCIAL_UI_FRIEND_RANDOM, m_CurPage, false);	//2013.02.22
		m_OldPage = m_CurPage;
	}
}
