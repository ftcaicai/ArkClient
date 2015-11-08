using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class AsGuildPanel_Manage : MonoBehaviour
{
	public SpriteText PR = null;
	public UIButton assignBtn = null;
	public UITextField pr = null;
	public SpriteText APPLICANT = null;
	public UIButton nameBtn = null;
	public UIButton levelBtn = null;
	public UIButton classBtn = null;
	public UIScrollList applicantList = null;
	public UIButton prevBtn = null;
	public SpriteText page = null;
	public UIButton nextBtn = null;
	public UIRadioBtn onlineBtn = null;
	public GameObject listItemObj = null;
	public int DRAG_SENSITIVITY = 5;
	public SpriteText noApplicant = null;

	private bool onlyOnline = false;
	private GameObject popupMenu = null;
	private int curPage = 0;
	private int maxPage = 0;
	private int oldPage = -1;
//	private bool isTouched = false;
//	private Vector2 prevPos = Vector2.zero;
	private string prevPR = string.Empty;
	
	// Use this for initialization
	void Start()
	{
		curPage = 0;
		noApplicant.Text = AsTableManager.Instance.GetTbl_String(1473);
		PR.Text = AsTableManager.Instance.GetTbl_String(1256);
		assignBtn.Text = AsTableManager.Instance.GetTbl_String(1238);
		APPLICANT.Text = AsTableManager.Instance.GetTbl_String(1261);
		nameBtn.Text = AsTableManager.Instance.GetTbl_String(1168);
		levelBtn.Text = AsTableManager.Instance.GetTbl_String(1249);
		classBtn.Text = AsTableManager.Instance.GetTbl_String(1250);
		onlineBtn.Text = AsTableManager.Instance.GetTbl_String(1251);
		
		pr.SetFocusDelegate( OnFocusPR);
		pr.SetValidationDelegate( GuildPRValidateCallback);
	}

	string GuildPRValidateCallback( UITextField field, string text, ref int insPos)
	{
		text = Regex.Replace( text, "'", "");
		
		while( true)
		{
			int byteCount = System.Text.UTF8Encoding.UTF8.GetByteCount( text);
			int charCount = System.Text.UTF8Encoding.UTF8.GetCharCount( System.Text.UTF8Encoding.UTF8.GetBytes( text));
			if( ( byteCount <= 60) && ( charCount <= 20))
				break;

			text = text.Remove( text.Length - 1);
		}

		return text;
	}

	private void OnFocusPR( UITextField field)
	{
		#if UNITY_IPHONE	
		field.autoCorrect = false;
		#endif
		
		string szPR = AsTableManager.Instance.GetTbl_String(1700);
		if( 0 == szPR.CompareTo( field.Text))
			field.Text = "";
	}
	
	// Update is called once per frame
	void Update()
	{
#if false
	#if UNITY_EDITOR
		if( true == Input.GetMouseButtonDown(0))
		{
			Vector2 pt = new Vector2( Input.mousePosition.x, Input.mousePosition.y);
			if( true == AsUtil.PtInCollider( page.RenderCamera, applicantList.collider, pt, true))
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
				
				RequestCurPageApplicant();
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
				if( true == AsUtil.PtInCollider( page.RenderCamera, applicantList.collider, touch.position, true))
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
					
					RequestCurPageApplicant();
				}
			}
			break;
		}
	#endif
#endif
	}
	
	private void OnDisable()
	{
		ClosePupup();
	}
	
	public void Init( System.Object data)
	{
		curPage = 0;
		oldPage = -1;
		body_SC_GUILD_SUPERVISE_RESULT supervise = (body_SC_GUILD_SUPERVISE_RESULT)data;
		
		string szPR = AsUtil.GetRealString( supervise.szPublicize);
		if( 0 == szPR.Length)
			pr.Text = AsTableManager.Instance.GetTbl_String(1700);
		else
			pr.Text = szPR;
		prevPR = pr.Text;
		
		body_SC_GUILD_LOAD_RESULT guildData = AsUserInfo.Instance.GuildData;
		if( null != guildData)
		{
			eGUILDPERMISSION permission = guildData.ePermission;
			
			if( 0 == ( eGUILDPERMISSION.eGUILDPERMISSION_PUBLICIZE & permission))
			{
				pr.controlIsEnabled = false;
				assignBtn.spriteText.Color = Color.gray;
				assignBtn.SetControlState( UIButton.CONTROL_STATE.DISABLED);
			}
		}
	}
	
	public void InsertApplicantList( body1_SC_GUILD_MEMBER_INFO_RESULT data)
	{
		maxPage = data.nMaxPage;
		page.Text = string.Format( "{0}/{1}", curPage + 1, data.nMaxPage);
		
		applicantList.ClearList( true);
		
		foreach( body2_SC_GUILD_MEMBER_INFO_RESULT member in data.infos)
		{
			if( ( true == onlyOnline) && ( false == member.bConnect))
				continue;
			
		 	UIListButton listBtn = applicantList.CreateItem( listItemObj) as UIListButton;
			AsGuildApplicantListItem item = listBtn.gameObject.GetComponent<AsGuildApplicantListItem>();
			item.Init( member);
		}
		
		noApplicant.gameObject.SetActiveRecursively( 0 == applicantList.Count);
	}
	
#if false
	private void RemoveApprovedUser( uint uniqKey)
	{
		for( int i = 0; i < applicantList.Count; i++)
		{
			UIListButton listBtn = applicantList.GetItem(i) as UIListButton;
			AsGuildApplicantListItem item = listBtn.gameObject.GetComponent<AsGuildApplicantListItem>();
			if( uniqKey == item.Data.nCharUniqKey)
			{
				applicantList.RemoveItem( i, true);
				noApplicant.gameObject.SetActiveRecursively( 0 == applicantList.Count);
				return;
			}
		}
	}
#endif
	
	public void RequestCurPageApplicant( bool isForced=false)
	{
		if( ( oldPage == curPage) && ( false == isForced))
			return;
		
		body_CS_GUILD_UI_SCROLL scroll = new body_CS_GUILD_UI_SCROLL( eGUILD_UI_SCROLL.eGUILD_UI_SCROLL_NOT_APPROVE_MEMBER, curPage, onlineBtn.Value);
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
	
	private void OnSelect()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		body_SC_GUILD_LOAD_RESULT guildData = AsUserInfo.Instance.GuildData;
		if( null != guildData)
		{
			eGUILDPERMISSION permission = guildData.ePermission;
			
			if( 0 == ( eGUILDPERMISSION.eGUILDPERMISSION_INVITE & permission))
				return;
		}
		
		if( null != popupMenu)
			return;
		
		UIListButton curSel = applicantList.LastClickedControl as UIListButton;
		AsGuildApplicantListItem listItem = curSel.gameObject.GetComponent<AsGuildApplicantListItem>();
		popupMenu = listItem.PromptPopup( gameObject);
	}
	
	private void OnAssignBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		if( 0 == pr.Text.Length)
			return;
		
		if( false == AsTableManager.Instance.TextFiltering_Guild( pr.Text))
		{
			AsNotify.Instance.MessageBox( string.Empty, AsTableManager.Instance.GetTbl_String(364), AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			return;
		}
		
		if( 0 == string.Compare( pr.Text, prevPR))
		{
			Debug.Log( "pr.Text == prevPR");
			return;
		}
		
		prevPR = pr.Text;
		
		body_CS_GUILD_PUBLICIZE publicize = new body_CS_GUILD_PUBLICIZE( pr.Text);
		byte[] packet = publicize.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( packet);
		
		AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(380), eCHATTYPE.eCHATTYPE_SYSTEM);
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
	
	private void OnPrevBtn()
	{
		Debug.Log( "OnPrevBtn");
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		curPage--;
		if( 0 > curPage)
			curPage = 0;

		RequestCurPageApplicant();
	}
	
	private void OnNextBtn()
	{
		Debug.Log( "OnNextBtn");
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		curPage++;
		if( maxPage <= curPage + 1)
			curPage = maxPage - 1;
		
		RequestCurPageApplicant();
	}
	
	private void OnOnlineBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		onlyOnline = !onlyOnline;
		onlineBtn.Value = onlyOnline;
		curPage = 0;
		oldPage = -1;
		RequestCurPageApplicant();
	}
}
