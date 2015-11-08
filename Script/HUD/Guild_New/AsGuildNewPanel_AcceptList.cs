using UnityEngine;
using System.Collections;

public class AsGuildNewPanel_AcceptList : AsGuildNewPanel_Base 
{
	public SpriteText 		m_txtInfo = null;
	public SpriteText 		m_txtMember = null;

	public SpriteText 		m_uiUserName = null;
	public SpriteText 		m_uiDate = null;
	public SpriteText 		m_uiConnection = null;
	public SpriteText 		m_uiAccept = null;

	public UIScrollList 	m_acceptList = null;
	public GameObject 	m_listItem = null;

	public SpriteText 		m_pageNum = null;
	private int				m_curPage = 0;
	private int	 			m_maxPage = 0;
	private int 				m_oldPage = 0;


	public UIButton		m_btnAcceptList = null;
	public UIButton		m_btnInvite = null;
	public UIButton		m_btnContribution = null;
	public UIButton		m_btnWithdraw = null;

	// Use this for initialization
	void Awake()
	{
	}

	void Start () 
	{
		m_txtInfo.Text = "accept info";
		m_txtMember.Text = AsTableManager.Instance.GetTbl_String(1662);

		m_uiUserName.Text = AsTableManager.Instance.GetTbl_String(1261);
		m_uiDate.Text = AsTableManager.Instance.GetTbl_String(931);
		m_uiConnection.Text = "Connection";
		m_uiAccept.Text = AsTableManager.Instance.GetTbl_String(1260);


		m_btnAcceptList.Text = AsTableManager.Instance.GetTbl_String(1260);
		m_btnAcceptList.SetInputDelegate( OnAcceptList );
		m_btnAcceptList.controlIsEnabled = false;
		m_btnAcceptList.spriteText.Color = Color.gray;

		m_btnInvite.Text = AsTableManager.Instance.GetTbl_String(1263);
		m_btnInvite.SetInputDelegate( OnInvite );
		m_btnInvite.controlIsEnabled = false;
		m_btnInvite.spriteText.Color = Color.gray;

		m_btnContribution.Text = AsTableManager.Instance.GetTbl_String(1113);
		m_btnContribution.SetInputDelegate( OnContribution );
		m_btnContribution.controlIsEnabled = false;
		m_btnContribution.spriteText.Color = Color.gray;

		m_btnWithdraw.Text = AsTableManager.Instance.GetTbl_String(1264);
		m_btnWithdraw.SetInputDelegate( OnWithdraw );
		m_btnWithdraw.controlIsEnabled = false;
		m_btnWithdraw.spriteText.Color = Color.gray;
	}

	override public void Init(System.Object data)
	{
		body1_SC_GUILD_MEMBER_INFO_RESULT	acceptList = (body1_SC_GUILD_MEMBER_INFO_RESULT)data;
		UpdateList( acceptList );
	}

	override public void UpdateData( System.Object data)
	{
		Init (data);
	}

	override public void RequestCurrentPage() 
	{
		RequestCurPageAccept (true);
	}
	
	public void UpdateList( body1_SC_GUILD_MEMBER_INFO_RESULT data )
	{
		m_maxPage = data.nMaxPage;
		m_pageNum.Text = string.Format( "{0}/{1}", m_curPage + 1, data.nMaxPage);
		
		m_acceptList.ClearList( true);
		
		foreach( body2_SC_GUILD_MEMBER_INFO_RESULT member in data.infos)
		{
//			UIListButton listBtn = m_acceptList.CreateItem( m_listItem) as UIListButton;
			UIListItemContainer listBtn = m_acceptList.CreateItem( m_listItem) as UIListItemContainer;
			AsGuildNewListItem_Accept item = listBtn.gameObject.GetComponent<AsGuildNewListItem_Accept>();
			item.Init( member);
		}
	}

	private void RequestCurPageAccept(bool isForceRequest = false)
	{
		if( m_oldPage == m_curPage && isForceRequest == false )
			return;

		body_CS_GUILD_UI_SCROLL scroll = new body_CS_GUILD_UI_SCROLL( eGUILD_UI_SCROLL.eGUILD_UI_SCROLL_NOT_APPROVE_MEMBER, m_curPage, false);
		byte[] packet = scroll.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( packet);

		m_oldPage = m_curPage;
	}

	private void OnPrevPageBtn()
	{
		Debug.Log( "OnPrevPageBtn");
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		m_curPage--;
		if( 0 > m_curPage)
			m_curPage = 0;
		
		RequestCurPageAccept();
	}
	
	private void OnNextPageBtn()
	{
		Debug.Log( "OnNextPageBtn");
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		m_curPage++;
		if( m_maxPage <= m_curPage + 1)
			m_curPage = m_maxPage - 1;
		
		RequestCurPageAccept();
	}

	void OnAcceptList( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
		}
	}
	
	void OnInvite( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
		}
	}
	
	void OnContribution( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
		}
	}
	
	void OnWithdraw( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
		}
	}

	// Update is called once per frame
	void Update () {
	
	}
}
