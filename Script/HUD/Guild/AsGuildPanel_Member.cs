using UnityEngine;
using System.Collections;

public class AsGuildPanel_Member : MonoBehaviour
{
	public UIButton nameBtn = null;
	public UIButton levelBtn = null;
	public UIButton classBtn = null;
	public UIScrollList list = null;
	public UIButton prevPageBtn = null;
	public UIButton nextPageBtn = null;
	public SpriteText pageText = null;
	public UIRadioBtn onlineFilter = null;
	public GameObject listItem = null;
	public int DRAG_SENSITIVITY = 5;
	
	private bool onlineMember = false;
	private GameObject popupMenu = null;
	private int curPage = 0;
	private int maxPage = 0;
	private int oldPage = -1;
//	private bool isTouched = false;
//	private Vector2 prevPos = Vector2.zero;
	
	// Use this for initialization
	void Start()
	{
		curPage = 0;
		nameBtn.Text = AsTableManager.Instance.GetTbl_String(1168);
		levelBtn.Text = AsTableManager.Instance.GetTbl_String(1249);
		classBtn.Text = AsTableManager.Instance.GetTbl_String(1250);
		onlineFilter.Text = AsTableManager.Instance.GetTbl_String(1251);
	}
	
	// Update is called once per frame
	void Update()
	{
#if false
	#if UNITY_EDITOR
		if( true == Input.GetMouseButtonDown(0))
		{
			Vector2 pt = new Vector2( Input.mousePosition.x, Input.mousePosition.y);
			if( true == AsUtil.PtInCollider( pageText.RenderCamera, list.collider, pt, true))
			{
				isTouched = true;
				prevPos = pt;
			}
		}
		else if( ( true == isTouched) && ( true == Input.GetMouseButtonUp(0)))
		{
			isTouched = false;
			Vector2 curPos = new Vector2( Input.mousePosition.x, Input.mousePosition.y);
			
			if( DRAG_SENSITIVITY <= Mathf.Abs( curPos.x - prevPos.x))
			{
				if( curPos.x < prevPos.x)
				{
					curPage++;
					if( maxPage <= curPage + 1)
						curPage = maxPage - 1;
				}
				else if( prevPos.x < curPos.x)
				{
					curPage--;
					if( 0 > curPage)
						curPage = 0;
				}
				
				RequestCurPageMember();
			}
		}
	#else
		if( 0 >= Input.touchCount)
			return;
		
		Touch touch = Input.GetTouch(0);
		switch( touch.phase)
		{
		case TouchPhase.Began:
			{
				if( true == AsUtil.PtInCollider( pageText.RenderCamera, list.collider, touch.position, true))
				{
					isTouched = true;
					prevPos = touch.position;
				}
			}
			break;
		case TouchPhase.Ended:
			{
				if( false == isTouched)
					break;
				
				if( DRAG_SENSITIVITY <= Mathf.Abs( touch.position.x - prevPos.x))
				{
					if( touch.position.x < prevPos.x)
					{
						curPage++;
						if( maxPage <= curPage + 1)
							curPage = maxPage - 1;
					}
					else if( prevPos.x < touch.position.x)
					{
						curPage--;
						if( 0 > curPage)
							curPage = 0;
					}
					
					RequestCurPageMember();
				}
			}
			break;
		}
	#endif
#endif
	}
	
	public void Init( System.Object data)
	{
		body1_SC_GUILD_MEMBER_INFO_RESULT info = (body1_SC_GUILD_MEMBER_INFO_RESULT)data;
		
		curPage = 0;
		oldPage = -1;
		InsertMemberList( info);
	}
	
	public void InsertMemberList( body1_SC_GUILD_MEMBER_INFO_RESULT data)
	{
		maxPage = data.nMaxPage;
		pageText.Text = string.Format( "{0}/{1}", curPage + 1, data.nMaxPage);
		
		list.ClearList( true);
		
		foreach( body2_SC_GUILD_MEMBER_INFO_RESULT member in data.infos)
		{
			if( ( true == onlineMember) && ( false == member.bConnect))
				continue;
				
		 	UIListButton listBtn = list.CreateItem( listItem) as UIListButton;
			AsGuildMemberListItem item = listBtn.gameObject.GetComponent<AsGuildMemberListItem>();
			item.Init( member);
		}
	}
	
	private void OnDisable()
	{
		ClosePupup();
	}
	
	private void RequestCurPageMember()
	{
		if( oldPage == curPage)
			return;
		
		body_CS_GUILD_UI_SCROLL scroll = new body_CS_GUILD_UI_SCROLL( eGUILD_UI_SCROLL.eGUILD_UI_SCROLL_APPROVE_MEMBER, curPage, onlineFilter.Value);
		byte[] packet = scroll.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( packet);
		
		oldPage = curPage;
	}
	
	public void ClosePupup()
	{
		Debug.Log( "ClosePupup");
		
		GameObject.DestroyImmediate( popupMenu);
		popupMenu = null;
	}
	
	private void RemoveExileMember( uint uniqKey)
	{
		body_CS_GUILD_UI_SCROLL scroll = new body_CS_GUILD_UI_SCROLL( eGUILD_UI_SCROLL.eGUILD_UI_SCROLL_APPROVE_MEMBER, curPage, onlineFilter.Value);
		byte[] packet = scroll.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( packet);
		return;
		
		for( int i = 0; i < list.Count; i++)
		{
			UIListButton listBtn = list.GetItem(i) as UIListButton;
			AsGuildMemberListItem item = listBtn.gameObject.GetComponent<AsGuildMemberListItem>();
			if( uniqKey == item.Data.nCharUniqKey)
			{
				list.RemoveItem( i, true);
				return;
			}
		}
	}
	
	private void OnSelect()
	{
		Debug.Log( "OnSelect");
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		if( null != popupMenu)
			return;
		
		UIListButton curSel = list.LastClickedControl as UIListButton;
		AsGuildMemberListItem listItem = curSel.gameObject.GetComponent<AsGuildMemberListItem>();
		popupMenu = listItem.PromptPopup( gameObject);
	}
	
	private void OnNameBtn()
	{
		Debug.Log( "OnNameBtn");
	}
	
	private void OnLevelBtn()
	{
		Debug.Log( "OnLevelBtn");
	}
	
	private void OnClassBtn()
	{
		Debug.Log( "OnClassBtn");
	}
	
	private void OnPrevPageBtn()
	{
		Debug.Log( "OnPrevPageBtn");
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		curPage--;
		if( 0 > curPage)
			curPage = 0;
		
		RequestCurPageMember();
	}
	
	private void OnNextPageBtn()
	{
		Debug.Log( "OnNextPageBtn");
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		curPage++;
		if( maxPage <= curPage + 1)
			curPage = maxPage - 1;
		
		RequestCurPageMember();
	}
	
	private void OnOnlineFilterBtn()
	{
		Debug.Log( "OnOnlineFilterBtn");
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		onlineMember = !onlineMember;
		onlineFilter.Value = onlineMember;
		curPage = 0;
		oldPage = -1;
		RequestCurPageMember();
	}
}
