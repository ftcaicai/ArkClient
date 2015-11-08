using UnityEngine;
using System.Collections;

public class AsGuildNewPanel_MemberList : AsGuildNewPanel_Base 
{
	public SpriteText 		m_uiNum = null;
	public SpriteText 		m_uiTitle = null;
	public SpriteText 		m_uiName = null;
	public SpriteText 		m_uiAttendance = null;
	public SpriteText 		m_uiConnection = null;
	public SpriteText 		m_uiGoldSupport = null;
	public SpriteText 		m_uiGuildPoint = null;

	public UIRadioBtn		m_rdoConnection = null;
	public UIRadioBtn		m_rdoGoldSupport = null;
	public UIRadioBtn		m_rdoGuildPoint = null;

	private bool 			m_isAscendingByConnection = false;
	private bool 			m_isAscendingByGoldSupport = false;
	private bool 			m_isAscendingByGuildPoint = false;
	
	public SpriteText 		m_pageNum = null;
	
	public UIScrollList 	m_memberList = null;
	public GameObject 	m_listItem = null;
	
	
	private int				m_curPage = 0;
	private int	 			m_maxPage = 0;
	private int 				m_oldPage = 0;

	public UIButton		m_btnAcceptList = null;
	public UIButton		m_btnInvite = null;
	public UIButton		m_btnContribution = null;
	public UIButton		m_btnWithdraw = null;
	
	private GameObject 	m_memberListMenuPopup = null;

	// Use this for initialization
	void Awake()
	{
	}

	void Start () 
	{
		m_uiNum.Text = AsTableManager.Instance.GetTbl_String(1662);
		m_uiTitle.Text = AsTableManager.Instance.GetTbl_String(1662);
		m_uiName.Text = AsTableManager.Instance.GetTbl_String(1168);
		m_uiAttendance.Text = AsTableManager.Instance.GetTbl_String(1662);
		m_uiConnection.Text = AsTableManager.Instance.GetTbl_String(1662);
		m_uiGoldSupport.Text = AsTableManager.Instance.GetTbl_String(1662);
		m_uiGuildPoint.Text = AsTableManager.Instance.GetTbl_String(1662);

		m_rdoConnection.SetInputDelegate ( OnRadioConnection );
		m_rdoGoldSupport.SetInputDelegate ( OnRadioGoldSupport );
		m_rdoGuildPoint.SetInputDelegate ( OnRadioGuildPoint );

		m_btnAcceptList.Text = AsTableManager.Instance.GetTbl_String(1260);
		m_btnAcceptList.SetInputDelegate( OnAcceptList );

		m_btnInvite.Text = AsTableManager.Instance.GetTbl_String(1263);
		m_btnInvite.SetInputDelegate( OnInvite );

		m_btnContribution.Text = AsTableManager.Instance.GetTbl_String(1113);
		m_btnContribution.SetInputDelegate( OnContribution );

		m_btnWithdraw.Text = AsTableManager.Instance.GetTbl_String(1264);
		m_btnWithdraw.SetInputDelegate( OnWithdraw );
	}

	void OnDisable()
	{
		ClosePopupMemberList ();
	}	

	public void ClosePopupMemberList()
	{
		GameObject.DestroyImmediate( m_memberListMenuPopup);
		m_memberListMenuPopup = null;
	}

	override public void Init(System.Object data)
	{
		body1_SC_GUILD_MEMBER_INFO_RESULT	memberList = (body1_SC_GUILD_MEMBER_INFO_RESULT)data;
		UpdateList( memberList );
	}

	override public void UpdateData( System.Object data)
	{
		Init (data);
	}

	override public void RequestCurrentPage() 
	{
		RequestCurPageMember (true);
	}

	public void UpdateList( body1_SC_GUILD_MEMBER_INFO_RESULT data )
	{
		m_maxPage = data.nMaxPage;
		m_pageNum.Text = string.Format( "{0}/{1}",m_curPage + 1, data.nMaxPage);
		
		m_memberList.ClearList( true);

		int nStartNumber = m_curPage * data.infos.Length;

		foreach( body2_SC_GUILD_MEMBER_INFO_RESULT member in data.infos )
		{
//			UIListButton itemBtn = m_memberList.CreateItem( m_listItem) as UIListButton;
			UIListItemContainer itemBtn = m_memberList.CreateItem( m_listItem) as UIListItemContainer;
			AsGuildNewListItem_Member item = itemBtn.gameObject.GetComponent<AsGuildNewListItem_Member>();
			item.Init( nStartNumber , member );
			nStartNumber++;
		}
	}
	
	private void RequestCurPageMember(bool isForceRequest = false)
	{
		if( m_oldPage == m_curPage && isForceRequest == false )
			return;
		
		body_CS_GUILD_UI_SCROLL scroll = new body_CS_GUILD_UI_SCROLL( eGUILD_UI_SCROLL.eGUILD_UI_SCROLL_APPROVE_MEMBER , m_curPage, false);
		byte[] packet = scroll.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( packet);
		
		m_oldPage = m_curPage;
	}
	
	private void OnSelect()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		if (m_memberListMenuPopup != null)
			return;
		
		UIListButton selItem = m_memberList.LastClickedControl as UIListButton;
		AsGuildNewListItem_Member memberListItem = selItem.gameObject.GetComponent<AsGuildNewListItem_Member>();
		m_memberListMenuPopup = memberListItem.PromptPopup( gameObject);
	}	
	
	private void OnPrevPageBtn()
	{
		Debug.Log( "OnPrevPageBtn");
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		m_curPage--;
		if( 0 > m_curPage)
			m_curPage = 0;
		
		RequestCurPageMember();
	}
	
	private void OnNextPageBtn()
	{
		Debug.Log( "OnNextPageBtn");
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		m_curPage++;
		if( m_maxPage <= m_curPage + 1)
			m_curPage = m_maxPage - 1;
		
		RequestCurPageMember();
	}


	void OnRadioConnection( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			Debug.LogError("OnRadioConnection button press " );

			m_isAscendingByConnection = !m_isAscendingByConnection;
			m_rdoConnection.Value = m_isAscendingByConnection;
		}
	}

	void OnRadioGoldSupport( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			Debug.LogError("OnRadioGoldSupport button press " );
			
			m_isAscendingByGoldSupport = !m_isAscendingByGoldSupport;
			m_rdoGoldSupport.Value = m_isAscendingByGoldSupport;
		}
	}

	void OnRadioGuildPoint( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			Debug.LogError("OnRadioGuildPoint button press " );
			
			m_isAscendingByGuildPoint = !m_isAscendingByGuildPoint;
			m_rdoGuildPoint.Value = m_isAscendingByGuildPoint;
		}
	}

	void OnAcceptList( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			body_CS_GUILD_UI_SCROLL scroll = new body_CS_GUILD_UI_SCROLL( eGUILD_UI_SCROLL.eGUILD_UI_SCROLL_NOT_APPROVE_MEMBER, 0 , false );
			byte[] packet = scroll.ClassToPacketBytes();
			AsNetworkMessageHandler.Instance.Send( packet);
		}
	}

	void OnInvite( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			Debug.LogError("OnInvite button press " );
		}
	}

	void OnContribution( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			Debug.LogError("OnContribution button press " );
		}
	}

	void OnWithdraw( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			Debug.Log( "OnWithdrawBtn");
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			
			string msg = string.Format( AsTableManager.Instance.GetTbl_String(237), AsUtil.GetRealString( AsUserInfo.Instance.GuildData.szGuildName ));
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(1240), msg, 
			                             AsTableManager.Instance.GetTbl_String(1264), AsTableManager.Instance.GetTbl_String(1151),
			                             this, "WithdrawConfirm", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);
		}
	}

	private void WithdrawConfirm()
	{
		body_CS_GUILD_EXIT guildExit = new body_CS_GUILD_EXIT();
		byte[] packet = guildExit.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( packet);
	}


	// Update is called once per frame
	void Update () {
	
	}
}




































