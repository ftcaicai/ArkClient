using UnityEngine;
using System.Collections;

public class AsGuildNewPanel_GuildList : AsGuildNewPanel_Base 
{
	public SpriteText 		m_guildName = null;
	public SpriteText 		m_guildMaster = null;
	public SpriteText 		m_guildLevel = null;
	public SpriteText 		m_guildMember = null;
	public SpriteText 		m_guildNotice = null;

	public SpriteText 		m_pageNum = null;

	public UIScrollList 	m_guildList = null;
	public GameObject 	m_listItem = null;


	private int				m_curPage = 0;
	private int	 			m_maxPage = 0;
	private int 				m_oldPage = 0;

	private GameObject 	m_guildListMenuPopup = null;

	// Use this for initialization
	void Awake()
	{
	}

	void Start () 
	{
		m_guildName.Text = AsTableManager.Instance.GetTbl_String(1168);
		m_guildMaster.Text = AsTableManager.Instance.GetTbl_String(1243);
		m_guildLevel.Text = AsTableManager.Instance.GetTbl_String(1724);
		m_guildMember.Text = AsTableManager.Instance.GetTbl_String(1949);
		m_guildNotice.Text = AsTableManager.Instance.GetTbl_String(4261);
	}


	void OnDisable()
	{
		ClosePopupGuildList ();
	}	

	override public void Init(System.Object data)
	{
		body1_SC_GUILD_SEARCH_RESULT guildList = (body1_SC_GUILD_SEARCH_RESULT)data;
		UpdateList( guildList );
	}

	override public void UpdateData( System.Object data)
	{
		Init (data);
	}

	public void UpdateList( body1_SC_GUILD_SEARCH_RESULT data )
	{
		m_maxPage = data.nMaxPage;
		m_pageNum.Text = string.Format( "{0}/{1}",m_curPage + 1, data.nMaxPage);
		
		m_guildList.ClearList( true);
		
		for( int i = 0; i < data.nCnt; i++)
		{
			UIListButton itemBtn = m_guildList.CreateItem( m_listItem) as UIListButton;
			AsGuildNewListItem_Guild item = itemBtn.gameObject.GetComponent<AsGuildNewListItem_Guild>();
			item.Data = data.search[i];
		}
	}

	private void RequestCurPageGuild()
	{
		if( m_oldPage == m_curPage)
			return;
		
		body_CS_GUILD_UI_SCROLL scroll = new body_CS_GUILD_UI_SCROLL( eGUILD_UI_SCROLL.eGUILD_UI_SCROLL_GUILD, m_curPage, false);
		byte[] packet = scroll.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( packet);
		
		m_oldPage = m_curPage;
	}

	private void OnSelect()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		if (m_guildListMenuPopup != null)
			return;

		UIListButton selItem = m_guildList.LastClickedControl as UIListButton;
		AsGuildNewListItem_Guild guildListItem = selItem.gameObject.GetComponent<AsGuildNewListItem_Guild>();
		m_guildListMenuPopup = guildListItem.Select( gameObject);
	}	

	private void OnPrevPageBtn()
	{
		Debug.Log( "OnPrevPageBtn");
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		m_curPage--;
		if( 0 > m_curPage)
			m_curPage = 0;
		
		RequestCurPageGuild();
	}
	
	private void OnNextPageBtn()
	{
		Debug.Log( "OnNextPageBtn");
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		m_curPage++;
		if( m_maxPage <= m_curPage + 1)
			m_curPage = m_maxPage - 1;
		
		RequestCurPageGuild();
	}

	public void ClosePopupGuildList()
	{
		GameObject.DestroyImmediate( m_guildListMenuPopup);
		m_guildListMenuPopup = null;
	}



	// Update is called once per frame
	void Update () {
	
	}
}
