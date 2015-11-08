using UnityEngine;
using System.Collections;

public class AsGuildListDlg : MonoBehaviour
{
	public SpriteText title = null;
	public UIButton closeBtn = null;
	public SpriteText createCondition = null;
	public SpriteText joinCondition = null;
	public UIButton createBtn = null;
	public UIButton prevPage = null;
	public UIButton nextPage = null;
	public SpriteText pageNum = null;
	public UIButton nameSortBtn = null;
	public UIButton levelSortBtn = null;
	public UIButton masterSortBtn = null;
	public UIScrollList guildList = null;
	public SpriteText noGuild = null;
	public GameObject listItem = null;
	public int DRAG_SENSITIVITY = 5;
	
	private GameObject requestDlg = null;
	private GameObject createDlg = null;
	private int curPage = 0;
	private int maxPage = 0;
	private int oldPage = -1;
//	private bool isTouched = false;
//	private Vector2 prevPos = Vector2.zero;
	
	// Use this for initialization
	void Start()
	{
//		noGuild.gameObject.SetActiveRecursively( false);
		noGuild.Text = AsTableManager.Instance.GetTbl_String(1472);
		title.Text = AsTableManager.Instance.GetTbl_String(1240);
		createCondition.Text = AsTableManager.Instance.GetTbl_String(1269);
		joinCondition.Text = AsTableManager.Instance.GetTbl_String(1270);
		createBtn.Text = AsTableManager.Instance.GetTbl_String(1241);
		nameSortBtn.Text = AsTableManager.Instance.GetTbl_String(1272);
		levelSortBtn.Text = AsTableManager.Instance.GetTbl_String(1242);
		masterSortBtn.Text = AsTableManager.Instance.GetTbl_String(1243);
	}
	
	// Update is called once per frame
	void Update()
	{
#if false
	#if UNITY_EDITOR
		if( true == Input.GetMouseButtonDown(0))
		{
			Vector2 pt = new Vector2( Input.mousePosition.x, Input.mousePosition.y);
			if( true == AsUtil.PtInCollider( pageNum.RenderCamera, guildList.collider, pt, true))
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
				
				RequestCurPageGuild();
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
				if( true == AsUtil.PtInCollider( pageNum.RenderCamera, guildList.collider, touch.position, true))
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
					
					RequestCurPageGuild();
				}
			}
			break;
		}
	#endif
#endif
	}
	
	void OnDisable()
	{
		CloseRequestDlg();
		CloseCreateDlg();
	}
	
	public void Init( body1_SC_GUILD_SEARCH_RESULT data)
	{
		maxPage = data.nMaxPage;
		pageNum.Text = string.Format( "{0}/{1}", curPage + 1, data.nMaxPage);
		
		guildList.ClearList( true);
		
		for( int i = 0; i < data.nCnt; i++)
		{
			UIListButton itemBtn = guildList.CreateItem( listItem) as UIListButton;
			AsGuildListItem item = itemBtn.gameObject.GetComponent<AsGuildListItem>();
			item.Data = data.search[i];
		}
		
		noGuild.gameObject.SetActiveRecursively( 0 == guildList.Count);
	}
	
	private void RequestCurPageGuild()
	{
		if( oldPage == curPage)
			return;
		
		body_CS_GUILD_UI_SCROLL scroll = new body_CS_GUILD_UI_SCROLL( eGUILD_UI_SCROLL.eGUILD_UI_SCROLL_GUILD, curPage, false);
		byte[] packet = scroll.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( packet);
		
		oldPage = curPage;
	}
	
	public void CloseRequestDlg()
	{
		GameObject.DestroyImmediate( requestDlg);
		requestDlg = null;
	}
	
	public void CloseCreateDlg()
	{
		GameObject.DestroyImmediate( createDlg);
		createDlg = null;
	}
	
	private void OnSelect()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		if( null != requestDlg)
			return;
		
		UIListButton selItem = guildList.LastClickedControl as UIListButton;
		AsGuildListItem guildListItem = selItem.gameObject.GetComponent<AsGuildListItem>();
		requestDlg = guildListItem.Select( gameObject);
	}
	
	private void OnCloseBtn()
	{
/*		
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		AsHudDlgMgr.Instance.CloseGuildListDlg();
*/		
	}
	
	private void OnCreateBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		if( null != createDlg)
			return;
		
		createDlg = Instantiate( Resources.Load( "UI/AsGUI/Guild/GUI_GuildCreate")) as GameObject;
		AsGuildCreateDlg dlg = createDlg.GetComponentInChildren<AsGuildCreateDlg>();
		dlg.Parent = gameObject;
	}
	
	private void OnPrevPageBtn()
	{
		Debug.Log( "OnPrevPageBtn");
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		curPage--;
		if( 0 > curPage)
			curPage = 0;
		
		RequestCurPageGuild();
	}
	
	private void OnNextPageBtn()
	{
		Debug.Log( "OnNextPageBtn");
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		curPage++;
		if( maxPage <= curPage + 1)
			curPage = maxPage - 1;
		
		RequestCurPageGuild();
	}
	
	private void OnNameSortBtn()
	{
		Debug.Log( "OnNameSortBtn");
	}
	
	private void OnLevelSortBtn()
	{
		Debug.Log( "OnLevelSortBtn");
	}
	
	private void OnMasterSortBtn()
	{
		Debug.Log( "OnMasterSortBtn");
	}
}
