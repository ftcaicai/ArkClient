using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public enum eCHATTYPE
{
	eCHATTYPE_NOTHING = 0,

	eCHATTYPE_PUBLIC,
	eCHATTYPE_PRIVATE,
	eCHATTYPE_PARTY,
	eCHATTYPE_GUILD,
	eCHATTYPE_MAP,
	eCHATTYPE_SERVER,
	eCHATTYPE_SYSTEM,
	eCHATTYPE_SYSTEM_ITEM,
	eCHATTYPE_SYSTEM_QUEST,
	eCHATTYPE_REPLY,
};

public enum CHAT_FILTER_TYPE : int
{
	None = -1,
	General,
	Party,
	Guild,
	Whisper,
	System,
};

public class AsChatManager : MonoBehaviour
{
	private const int MAX_CHAT_HISTORY = 6;
	private const int MIN_STATE_TEXT_WIDTH = 370;

	public AsDlgBase background = null;
	public UIScrollList allList = null;
	public GameObject chatListItem = null;
	[SerializeField] UIButton m_btnEmoticon = null;
	public UIButton m_btnChatFull;
	public SpriteText m_textSystemGM = null;
	public Color m_Color_Disable;
	public Color m_Color_All;
	public Color m_Color_General;
	public Color m_Color_Party;
	public Color m_Color_Guild;
	public Color m_Color_Whisper;
	public Color m_Color_System;
	public Color m_Color_System_Item;
	public Color m_Color_System_Quest;
	public Color m_Color_Map;
	public Color m_Color_World;

	[HideInInspector]
	public CHAT_FILTER_TYPE filterType = CHAT_FILTER_TYPE.None;

//	private float m_fTime_GMChat = 0.0f;
//	private float m_fTime_GMChat_Duration = 0.0f;
//	private Vector3 m_vGMChatPos = Vector3.zero;
	private Color bgColor = new Color( 1.0f, 1.0f, 1.0f, 0.0f);
	private List<body_SC_CHAT_WITH_BALLOON_RESULT> chatRawData = new List<body_SC_CHAT_WITH_BALLOON_RESULT>();
	private List<string> chatBuff = new List<string>();

	private static AsChatManager instance = null;
	public static AsChatManager Instance
	{
		get	{ return instance; }
	}
	
	void Awake()
	{
		instance = this;
	}

	// Use this for initialization
	void Start()
	{
		Init( CHAT_FILTER_TYPE.None);
		ModifyColor();

		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_btnChatFull.spriteText);
		m_btnChatFull.Text = AsTableManager.Instance.GetTbl_String(1474);
		m_btnChatFull.SetInputDelegate( _ChatFullBtnDelegate);

		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_btnEmoticon.spriteText);
		m_btnEmoticon.Text = AsTableManager.Instance.GetTbl_String(1933);
		m_btnEmoticon.SetInputDelegate( _EmoticonBtnDelegate);

		m_textSystemGM.Hide( true);
		
		_InitChatListItems();
		AsChatFullPanel.Instance.InitChatListItems();
	}

	void OnEnable()
	{
		Init( filterType);
	}

	public void InsertChatRawData( body_SC_CHAT_WITH_BALLOON_RESULT data)
	{
		if( 10 <= chatRawData.Count)
			chatRawData.Clear();

		chatRawData.Add( data);
	}

	IEnumerator InsertChatFromRawData()
	{
		while( true)
		{
			if( 0 < chatRawData.Count)
			{
				body_SC_CHAT_WITH_BALLOON_RESULT data = chatRawData[0];

				if( null == AsSocialManager.Instance.SocialData.GetBlockOutUser( data.nUserUniqKey))
				{
					data.kName.szName = data.kName.szName.Remove( data.kName.szName.Length - 1);

					StringBuilder sb = new StringBuilder();
					sb.AppendFormat( "[{0}]: {1}", data.kName.szName, data.kMessage.szMsg);

					bool isMe = ( AsUserInfo.Instance.SavedCharStat.uniqKey_ == data.nCharUniqKey) ? true : false;
					AsChatManager.Instance.InsertChat( sb.ToString(), eCHATTYPE.eCHATTYPE_PUBLIC, isMe, data.kMessage.bUseFiltering);
					AsChatManager.Instance.ShowChatBalloon( data.nCharUniqKey, data.kMessage.szMsg, eCHATTYPE.eCHATTYPE_PUBLIC, data.kMessage.bUseFiltering);
					AsEmotionManager.Instance.EmotionProcess( data); //$ yde

					chatRawData.RemoveAt(0);
				}
			}

			yield return null;
		}
	}

	public void ClearAllChat()
	{
		//allList.ClearList( true);
		//chatBuff.Clear();
		_InitChatListItems();

		if( null != AsChatFullPanel.Instance)
			AsChatFullPanel.Instance.ClearAllChat();
	}

	public void Init( CHAT_FILTER_TYPE type)
	{
		allList.gameObject.SetActiveRecursively( true);
		allList.ScrollListTo( 1.0f);

		filterType = type;

		StopCoroutine( "InsertChatFromRawData");
		StartCoroutine( "InsertChatFromRawData");
	}

	private void ModifyColor()
	{
		background.SetColor( bgColor);

		for( int i = 0; i < allList.Count; i++)
		{
			UIListItem item = allList.GetItem(i) as UIListItem;
			Color color = item.spriteText.Color;
			color.a = bgColor.a;
			item.spriteText.Color = color;
		}
	}

	public void StartModifyTransparency()
	{
		if( false == gameObject.active)
			return;

		StopCoroutine( "ModifyTransparency");

		bgColor.a = 0.6f;
		ModifyColor();

		StartCoroutine( "ModifyTransparency", null);
	}

	IEnumerator ModifyTransparency()
	{
		yield return new WaitForSeconds( 5.0f);

		while( true)
		{
			bgColor.a -= ( Time.deltaTime);

			ModifyColor();

			if( 0.0f >= bgColor.a)
				break;

			yield return null;
		}
	}
	
	Vector3 chatBtnPos1 = new Vector3( -18.21698f, -8.442736f, -0.2f);
	Vector3 chatBtnPos2 = new Vector3( -18.21698f, -8.442736f, -24.0f);
	Vector3 iconBtnPos1 = new Vector3( -18.21698f, -5.14945f, -0.2f);
	Vector3 iconBtnPos2 = new Vector3( -18.21698f, -5.14945f, -24.0f);
	bool isFirst = true;
	// Update is called once per frame
	void Update()
	{
		if( true == isFirst)
		{
			ToggleWide( AsGameMain.GetOptionState( OptionBtnType.OptionBtnType_Chat));
			isFirst = false;
		}
		
		if( true == AsNotify.Instance.IsOpenDeathDlg)
		{
			if (true == AsHudDlgMgr.Instance.IsOpenCashStore)
			{
				m_btnChatFull.gameObject.transform.localPosition = chatBtnPos1;
				m_btnEmoticon.gameObject.transform.localPosition = iconBtnPos1;
			}
			else
			{
				m_btnChatFull.gameObject.transform.localPosition = chatBtnPos2;
				m_btnEmoticon.gameObject.transform.localPosition = iconBtnPos2;
			}
		}
		else
		{
			m_btnChatFull.gameObject.transform.localPosition = chatBtnPos1;
			m_btnEmoticon.gameObject.transform.localPosition = iconBtnPos1;
		}
	}
	
	//begin kij
	System.Text.StringBuilder m_sbInsertSystemChatTemp = new System.Text.StringBuilder();
	
	/*
	public void InsertSystemChat( string msg, eCHATTYPE type=eCHATTYPE.eCHATTYPE_PUBLIC, bool isMe=false)
	{
		UIListItem chatItem = allList.CreateItem( chatListItem) as UIListItem;
		chatItem.collider.enabled = false;
		chatItem.transform.localPosition = new Vector3( -allList.viewableArea.x * 0.5f, chatItem.transform.localPosition.y, chatItem.transform.localPosition.z);
		chatItem.SetSize( allList.viewableArea.x, 1.0f);
		chatItem.spriteText.maxWidth = background.center.PixelSize.x - m_btnChatFull.PixelSize.x;
		chatItem.UpdateCamera();

		m_sbInsertSystemChatTemp.Remove( 0, m_sbInsertSystemChatTemp.Length);
		m_sbInsertSystemChatTemp.Append( GetChatTypeColor( type));
		m_sbInsertSystemChatTemp.Append( msg);

		chatItem.Text = m_sbInsertSystemChatTemp.ToString();
		ActivateChatList( type, chatItem, isMe);
		allList.ScrollListTo( 1.0f);
		allList.UpdateCamera();

		AsChatFullPanel.Instance.InsertSystemChat( m_sbInsertSystemChatTemp.ToString(), type, isMe);
	}
	*/
	public void InsertSystemChat( string msg, eCHATTYPE type=eCHATTYPE.eCHATTYPE_PUBLIC, bool isMe=false)
	{
		m_sbInsertSystemChatTemp.Remove( 0, m_sbInsertSystemChatTemp.Length);
		m_sbInsertSystemChatTemp.Append( GetChatTypeColor( type));
		m_sbInsertSystemChatTemp.Append( msg);

		chatBuff.Add( m_sbInsertSystemChatTemp.ToString());
		
		if( chatBuff.Count > MAX_CHAT_HISTORY)
			chatBuff.RemoveAt( 0);

		for( int i = 0; i < MAX_CHAT_HISTORY - 1; i++)
		{
			UIListItem chatItemBuff = allList.GetItem( i) as UIListItem;
			chatItemBuff.Text = chatBuff[i];
		}
		
		UIListItem chatItem = allList.GetItem( MAX_CHAT_HISTORY - 1) as UIListItem;

		chatItem.Text = m_sbInsertSystemChatTemp.ToString();
		ActivateChatList( type, chatItem, isMe);
		allList.ScrollListTo( 1.0f);
		allList.UpdateCamera();

		AsChatFullPanel.Instance.InsertSystemChat( m_sbInsertSystemChatTemp.ToString(), type, isMe);
	}

	public Color GetChatTypeColor( eCHATTYPE type)
	{
		switch( type)
		{
		case eCHATTYPE.eCHATTYPE_PUBLIC:	return m_Color_All;
		case eCHATTYPE.eCHATTYPE_PRIVATE:	return m_Color_Whisper;
		case eCHATTYPE.eCHATTYPE_PARTY:	return m_Color_Party;
		case eCHATTYPE.eCHATTYPE_GUILD:	return m_Color_Guild;
		case eCHATTYPE.eCHATTYPE_MAP:	return m_Color_Map;
		case eCHATTYPE.eCHATTYPE_SERVER:	return m_Color_World;
		case eCHATTYPE.eCHATTYPE_SYSTEM:	return m_Color_System;
		case eCHATTYPE.eCHATTYPE_SYSTEM_ITEM:	return m_Color_System_Item;
		case eCHATTYPE.eCHATTYPE_SYSTEM_QUEST:	return m_Color_System_Quest;
		default:
			Debug.LogWarning( "Invalid chatting type");
			break;
		}

		return m_Color_All;
	}
	//end kij
	
	/*
	public void InsertChat( string msg, eCHATTYPE type=eCHATTYPE.eCHATTYPE_PUBLIC, bool isMe=false, bool useFiltering = true)
	{
		if( msg.IndexOf('[') < msg.IndexOf(']') && msg.IndexOf(']') < msg.IndexOf(':') && msg.IndexOf(':') < msg.IndexOf('/'))
			return;

		if( MAX_CHAT_HISTORY <= allList.Count)
			allList.RemoveItem( 0, true);

		UIListItem chatItem = allList.CreateItem( chatListItem) as UIListItem;
		chatItem.collider.enabled = false;
		chatItem.transform.localPosition = new Vector3( -allList.viewableArea.x * 0.5f, chatItem.transform.localPosition.y, chatItem.transform.localPosition.z);
		chatItem.SetSize( allList.viewableArea.x, 1.0f);
		chatItem.spriteText.maxWidth = background.center.PixelSize.x - m_btnChatFull.PixelSize.x;
		chatItem.UpdateCamera();

		if (useFiltering == true)
			msg = AsTableManager.Instance.TextFiltering_Chat(msg);

		m_sbInsertSystemChatTemp.Remove( 0, m_sbInsertSystemChatTemp.Length);
		m_sbInsertSystemChatTemp.Append( GetChatTypeColor( type));
		m_sbInsertSystemChatTemp.Append( msg);

		chatItem.Text = m_sbInsertSystemChatTemp.ToString();
		ActivateChatList( type, chatItem, isMe);
		allList.ScrollListTo( 1.0f);
		allList.UpdateCamera();

		AsChatFullPanel.Instance.InsertChat( msg, type, isMe, useFiltering);
	}
	*/

	public void InsertChat( string msg, eCHATTYPE type=eCHATTYPE.eCHATTYPE_PUBLIC, bool isMe=false, bool useFiltering = true)
	{
//		if( msg.IndexOf('[') < msg.IndexOf(']') && msg.IndexOf(']') < msg.IndexOf(':') && msg.IndexOf(':') < msg.IndexOf('/'))
//			return;
		
		if( msg.IndexOf(':') + 2 == msg.IndexOf( '/'))
			return;
		
		if (useFiltering == true)
			msg = AsTableManager.Instance.TextFiltering_Chat(msg);

		m_sbInsertSystemChatTemp.Remove( 0, m_sbInsertSystemChatTemp.Length);
		m_sbInsertSystemChatTemp.Append( GetChatTypeColor( type));
		m_sbInsertSystemChatTemp.Append( msg);

		chatBuff.Add( m_sbInsertSystemChatTemp.ToString());
		
		if( chatBuff.Count > MAX_CHAT_HISTORY)
			chatBuff.RemoveAt( 0);
		
		for( int i = 0; i < MAX_CHAT_HISTORY - 1; i++)
		{
			UIListItem chatItemBuff = allList.GetItem( i) as UIListItem;
			chatItemBuff.Text = chatBuff[i];
		}
		
		UIListItem chatItem = allList.GetItem( MAX_CHAT_HISTORY - 1) as UIListItem;

		chatItem.Text = m_sbInsertSystemChatTemp.ToString();
		ActivateChatList( type, chatItem, isMe);
		allList.ScrollListTo( 1.0f);
		allList.UpdateCamera();

		AsChatFullPanel.Instance.InsertChat( m_sbInsertSystemChatTemp.ToString(), type, isMe, useFiltering);
	}

	public void InsertGMChat( string msg)
	{
//		m_fTime_GMChat = 0.0f;

		StringBuilder sb = new StringBuilder( AsTableManager.Instance.GetTbl_String(4030));
		sb.Append( msg);
		InsertChat( sb.ToString(), eCHATTYPE.eCHATTYPE_SYSTEM, false, false);
		AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage( sb.ToString());
	}

	private void ActivateChatList( eCHATTYPE type, UIListItem chatItem, bool isMe)
	{
		if( CHAT_FILTER_TYPE.General != filterType)
		{
			if( CHAT_FILTER_TYPE.None != filterType)
			{
				switch( type)
				{
				case eCHATTYPE.eCHATTYPE_PUBLIC:
				case eCHATTYPE.eCHATTYPE_MAP:
				case eCHATTYPE.eCHATTYPE_SERVER:
					chatItem.gameObject.SetActiveRecursively( false);
					break;
				default:
					StartModifyTransparency();
					break;
				}
			}
			else
				StartModifyTransparency();
		}
		else
		{
			if( false == isMe)
			{
				switch( type)
				{
				case eCHATTYPE.eCHATTYPE_PUBLIC:
				case eCHATTYPE.eCHATTYPE_MAP:
				case eCHATTYPE.eCHATTYPE_SERVER:
					chatItem.spriteText.Color = new Color( 1.0f, 1.0f, 1.0f, bgColor.a);
					break;
				}
			}
			else
			{
				switch( type)
				{
				case eCHATTYPE.eCHATTYPE_PUBLIC:
				case eCHATTYPE.eCHATTYPE_MAP:
				case eCHATTYPE.eCHATTYPE_SERVER:
					StartModifyTransparency();
					break;
				}
			}
		}
	}

	public void ShowChatBalloon( UInt32 uniqueIdx, string msg, eCHATTYPE type, bool useFiltering = true)
	{
		AsChatFullPanel.Instance.ShowChatBalloon( uniqueIdx, msg, type, useFiltering);
	}

	public void SetDefaultText( string msg)
	{
		AsChatFullPanel.Instance.Open();
		AsChatFullPanel.Instance.SetDefaultText( msg);
	}

	public void SetFocusInputField( string defaultMsg)
	{
		AsChatFullPanel.Instance.Open();
		AsChatFullPanel.Instance.SetDefaultText( defaultMsg);
	}

	private void _ChatFullBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			AsChatFullPanel.Instance.Open();
		}
	}

	private void _EmoticonBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			AsEmotionManager.Instance.ToggleEmoticonPanel();
		}
	}

	public void ToggleWide( bool isMax)
	{
		float screenWidth = background.center.RenderCamera.orthographicSize * background.center.RenderCamera.aspect;

		if( true == isMax)
		{
			background.transform.localPosition = new Vector3( 0.0f, 7.9f, 8.0f);
			background.AssignRatio();
		}
		else
		{
			background.transform.localPosition = new Vector3( -( screenWidth - 10.0f), 7.9f, 8.0f);
			background.width = 19;
			background.Assign();
		}

		//	all
		allList.transform.position = new Vector3( background.transform.position.x + 1.642515f, background.transform.position.y, 8.0f);
		allList.viewableArea.Set( background.width - 2.7f, 6.2f);
		for( int i = 0; i < allList.Count; i++)
		{
			UIListItem item = allList.GetItem(i) as UIListItem;
			item.transform.localPosition = new Vector3( -allList.viewableArea.x * 0.5f, item.transform.localPosition.y, item.transform.localPosition.z);
			item.SetSize( allList.viewableArea.x, 1.0f);
			item.spriteText.maxWidth = background.center.PixelSize.x - m_btnChatFull.PixelSize.x;
			item.Text = item.Text;
			item.UpdateCamera();
		}
		allList.ScrollListTo( 1.0f);
		allList.UpdateCamera();
	}
	
	private void _InitChatListItems()
	{
		//dopamin ios error 	allList.ClearList( true);
		allList.ClearListSync( true);
		chatBuff.Clear();
		
		for( int i = 0; i < MAX_CHAT_HISTORY; i++)
		{
			UIListItem chatItem = allList.CreateItem( chatListItem) as UIListItem;
			chatItem.collider.enabled = false;
			chatItem.transform.localPosition = new Vector3( -allList.viewableArea.x * 0.5f, chatItem.transform.localPosition.y, chatItem.transform.localPosition.z);
			chatItem.SetSize( allList.viewableArea.x, 1.0f);
			chatItem.spriteText.maxWidth = background.center.PixelSize.x - m_btnChatFull.PixelSize.x;
			chatItem.UpdateCamera();
			
			chatBuff.Add( "");
		}
	}
}
