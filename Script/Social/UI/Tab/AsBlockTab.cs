using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsBlockTab : AsSocialTab
{
	public UIButton m_BlockBtn = null;
	public UIScrollList m_list = null;
	public GameObject m_objChoiceItem = null;
	public UIButton m_PrevPageBtn = null;
	public UIButton m_NextPageBtn = null;
	public SpriteText m_PageText = null;
	public SpriteText noBlock = null;

	private int m_CurPage = 0;
	private int m_MaxPage = 0;
	private int m_OldPage = -1;

	public override UIScrollList getList()
	{
		return m_list;
	}

	void OnDisable()
	{
	}

	void OnEnable()
	{
		m_CurPage = 0;
		m_MaxPage = 0;
		m_OldPage = -1;
		m_PageText.Text = "1/1";
		m_list.ClearList( true);

		noBlock.gameObject.SetActiveRecursively( true);
	}

	void SetLanguageText()
	{
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_BlockBtn.spriteText);
		m_BlockBtn.Text = AsTableManager.Instance.GetTbl_String(1194);
		noBlock.Text = AsTableManager.Instance.GetTbl_String(1470);
	}

	// Use this for initialization
	void Start()
	{
		SetLanguageText();

		if( m_BlockBtn != null)
			m_BlockBtn.SetInputDelegate( BlockBtnDelegate);

		m_CurPage = 0;
		m_PrevPageBtn.SetInputDelegate( PrevPageDelegate);
		m_NextPageBtn.SetInputDelegate( NextPageDelegate);
	}

	// Update is called once per frame
	void Update()
	{
	}

	override public void Init()
	{
	}

	public void RequestFirstPage()
	{
		m_CurPage = 0;
		m_OldPage = -1;
		RequestCurPageMember();
	}

	private void BlockBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			AsSocialManager.Instance.OpenBlockDlg( false);
		}
	}

	public void InsertBlockOutData( body2_SC_BLOCKOUT_LIST listData)
	{
		UIListItem item = m_list.CreateItem( m_objChoiceItem) as UIListItem;

		AsBlockOutItem baseFriendItem = item.gameObject.GetComponent<AsBlockOutItem>();
		if( null == baseFriendItem)
			return;

		baseFriendItem.SetBlockOutData( listData);
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

	public void SetBlockOutList()
	{
		m_MaxPage = AsSocialManager.Instance.SocialData.GetBlockListMaxPage();
		m_PageText.Text = string.Format( "{0}/{1}", m_CurPage + 1, m_MaxPage);

		m_list.ClearList( true);

		body2_SC_BLOCKOUT_LIST[] list = AsSocialManager.Instance.SocialData.GetBlockByPage( m_CurPage);
		foreach( body2_SC_BLOCKOUT_LIST data in list)
		{
			if( data != null)
				InsertBlockOutData( data);
		}

		m_list.ScrollListTo( 0.0f);

		noBlock.gameObject.SetActiveRecursively( 0 == m_list.Count);
	}

	public void SetBlockOutList( body1_SC_BLOCKOUT_LIST list)
	{
		m_MaxPage = list.nMaxPage;
		m_PageText.Text = string.Format( "{0}/{1}", m_CurPage + 1, m_MaxPage);

		m_list.ClearList( true);
		if( list.body == null) return;

		foreach( body2_SC_BLOCKOUT_LIST data in list.body)
		{
			if( data != null)
				InsertBlockOutData( data);
		}

		m_list.ScrollListTo( 0.0f);

		noBlock.gameObject.SetActiveRecursively( 0 == m_list.Count);
	}

	private void RequestCurPageMember()
	{
		if( m_OldPage == m_CurPage)
			return;

		SetBlockOutList();

		m_OldPage = m_CurPage;
	}
}
