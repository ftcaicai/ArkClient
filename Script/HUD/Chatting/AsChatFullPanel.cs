using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class AsChatFullPanel : MonoBehaviour
{
	private const int MAX_CHAT_HISTORY = 30;
	private const int MAX_CHAT_SHOWLIST = 11;
	private string TOKEN_NORMAL_CHAT1 = "/n";
	private string TOKEN_NORMAL_CHAT2 = "/n";
	private string TOKEN_NORMAL_CHAT3 = "/n";
	private string TOKEN_WHISPER_CHAT1 = "/w";
	private string TOKEN_WHISPER_CHAT2 = "/w";
	private string TOKEN_WHISPER_CHAT3 = "/w";
	private string TOKEN_PARTY_CHAT1 = "/p";
	private string TOKEN_PARTY_CHAT2 = "/p";
	private string TOKEN_PARTY_CHAT3 = "/p";
	private string TOKEN_GUILD_CHAT1 = "/g";
	private string TOKEN_GUILD_CHAT2 = "/g";
	private string TOKEN_GUILD_CHAT3 = "/g";
	private string TOKEN_SHOUT_CHAT1 = "/s";
	private string TOKEN_SHOUT_CHAT2 = "/s";
	private string TOKEN_SHOUT_CHAT3 = "/s";
	private string TOKEN_REPLY1 = "/r";
	private string TOKEN_REPLY2 = "/r";
	private string TOKEN_REPLY3 = "/r";
	private const char TOKEN_SPLIT = ' ';
	private const string TOKEN_GUILD_INVITE = "/guildinvite";
	private const string TOKEN_GUILD_LEAVE = "/guildleave";
	private const string TOKEN_GUILD_KICK = "/guildkick";
	
	public AsDlgBase background = null;
	[SerializeField] Color bgColor = Color.white;
	public UIScrollList allList = null;
	public UITextField textField = null;
	[SerializeField] SimpleSprite textFieldBg = null;
	[SerializeField] UITextField filterField = null;
	[SerializeField] SimpleSprite filterFieldBg = null;
	[SerializeField] SpriteText filterText = null;
	[SerializeField] GameObject whisperBalloon = null;
	public GameObject chatListItem = null;
	public AsChatBalloonBase chatBalloon = null;
	public AsNewChatAlarm[] news = new AsNewChatAlarm[0];
	public UIButton btnClose;
	[HideInInspector]
	public CHAT_FILTER_TYPE filterType = CHAT_FILTER_TYPE.None;

	private string latestWhisperer = null;
	private string m_strDefaultText = "";
	
	private float m_fLatestMsgSendTime = 0.0f;
	private float m_fSendMsgLockTimeGab = 0.0f;
	private int m_nSendMsgLockCount = 0;
	private float m_fSendMsgLockTime = 0.0f;
	private int m_nSendMsgLockCountBuf = 0;
	private float m_fSendMsgLockTimeBuf = 0.0f;
	private bool m_bLocked = false;
	[SerializeField] AsChatButtonAnimController btnController = null;
	
	private List<string> chatBuff_all = new List<string>();
	private List<string> chatBuff_normal = new List<string>();
	private List<string> chatBuff_party = new List<string>();
	private List<string> chatBuff_guild = new List<string>();
	private List<string> chatBuff_whisper = new List<string>();
	private List<string> chatBuff_system = new List<string>();
	private int m_nScroll = 0;
	public UIButton btnChatTopEnd;
	public UIButton btnChatTop;
	public UIButton btnChatBottom;
	public UIButton btnChatBottomEnd;
	
#if !UNITY_EDITOR
	private bool isFocused = false;
#endif
	
	private static AsChatFullPanel instance = null;
	public static AsChatFullPanel Instance
	{
		get	{ return instance; }
	}
	
	void Awake()
	{
		instance = this;

		Maximize();
	}
		
	// Use this for initialization
	void Start()
	{
		m_strDefaultText = AsTableManager.Instance.GetTbl_String(1475);
		
		Init( CHAT_FILTER_TYPE.None);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( textField);
		textField.SetFocusDelegate( OnInputFocused);
		textField.collider.enabled = false;
		textField.Text = m_strDefaultText;
//		textField.SetCommitDelegate( OnCommit);

		btnClose.SetInputDelegate( _CloseBtnDelegate);
		
		_InitTokens();
		
		m_fSendMsgLockTimeGab = AsTableManager.Instance.GetTbl_GlobalWeight_Record( 40).Value / 1000.0f;
		m_nSendMsgLockCount = (int)( AsTableManager.Instance.GetTbl_GlobalWeight_Record( 41).Value);
		m_fSendMsgLockTime = AsTableManager.Instance.GetTbl_GlobalWeight_Record( 42).Value / 1000.0f;
		
		//InitChatListItems();
		btnChatTopEnd.SetInputDelegate( _BtnDelegate_ChatTopEnd);
		btnChatTop.SetInputDelegate( _BtnDelegate_ChatTop);
		btnChatBottom.SetInputDelegate( _BtnDelegate_ChatBottom);
		btnChatBottomEnd.SetInputDelegate( _BtnDelegate_ChatBottomEnd);
	}

	void OnEnable()
	{
		Init( filterType);
		Input.imeCompositionMode = IMECompositionMode.On;
		StartCoroutine( MaximizeProc());
	}
	
	void OnDisable()
	{
		Input.imeCompositionMode = IMECompositionMode.Auto;
	}
	
	public void ClearAllChat()
	{
		InitChatListItems();
	}

	private void _activateWhisperBalloon()
	{
		whisperBalloon.SetActiveRecursively( true);
		Invoke( "_inactivateWhisperBalloon", 3.0f);
	}

	private void _inactivateWhisperBalloon()
	{
		if( true == IsInvoking( "_inactivateWhisperBalloon"))
			CancelInvoke( "_inactivateWhisperBalloon");

		whisperBalloon.SetActiveRecursively( false);
	}
	
	public void Init( CHAT_FILTER_TYPE type)
	{
		filterType = type;
		
		switch( type)
		{
		case CHAT_FILTER_TYPE.None:
			foreach( AsNewChatAlarm alram in news)
				alram.NewMsgExist = false;
			_inactivateWhisperBalloon();
			break;
		case CHAT_FILTER_TYPE.General:
			news[ (int)type].NewMsgExist = false;
			_inactivateWhisperBalloon();
			break;
		case CHAT_FILTER_TYPE.Party:
			news[ (int)type].NewMsgExist = false;
			_inactivateWhisperBalloon();
			break;
		case CHAT_FILTER_TYPE.Guild:
			news[ (int)type].NewMsgExist = false;
			_inactivateWhisperBalloon();
			break;
		case CHAT_FILTER_TYPE.Whisper:
			news[ (int)type].NewMsgExist = false;
			_activateWhisperBalloon();
			break;
		case CHAT_FILTER_TYPE.System:
			news[ (int)type].NewMsgExist = false;
			_inactivateWhisperBalloon();
			break;
		default:
			Debug.LogError( "Invalid chat filter type");
			return;
		}

		textField.spriteText.transform.localPosition = new Vector3( 0.0f, 0.0f, -0.2f);
		textField.Text = m_strDefaultText;
		
		_InitScroll();
	}
	
	private void OnInputFocused( UITextField field)
	{
		Debug.Log( "OnInputFocused");
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
	}

	private void OnCommit( IKeyFocusable field)
	{
		textField.spriteText.transform.localPosition = new Vector3( 0.0f, 0.0f, -0.2f);
		textField.Text = m_strDefaultText;
	}
	
	// Update is called once per frame
	void Update()
	{
#if !UNITY_EDITOR
		if( 1 > Input.touchCount)
			return;
		
		Touch touch = Input.GetTouch( 0);
		
		switch( touch.phase)
		{
		case TouchPhase.Began:
			if( false == AsUtil.PtInCollider( textField.RenderCamera, collider, Input.GetTouch( 0).position))
			{
				isFocused = false;
				break;
			}
			
			isFocused = true;
			break;
			
		case TouchPhase.Ended:
			if( ( false == AsUtil.PtInCollider( textField.RenderCamera, collider, Input.GetTouch( 0).position)) || ( false == isFocused))
				break;
			
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			SetFocusInputField( string.Empty);
			isFocused = false;
			break;
		}
#endif
	}

#if ( UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN)
	void OnMouseUpAsButton()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		SetFocusInputField( string.Empty);
	}
#endif
	
	private bool CommandJudgment( string msg)
	{
		if( "/ShowFPS" == msg)
		{
			GameObject go = GameObject.Find( "GameMain");
			AsGameMain gameMain = go.GetComponent<AsGameMain>();
			gameMain.ShowFPS();
			return true;
		}

		#if ( UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN)
		if( true == AsUserInfo.Instance.isGM)
		{
			if( "/Cheat" == msg)
			{
				GameObject go = ResourceLoad.CreateGameObject( "UI/Cheat/CheatManager");
				Debug.Assert( null != go);
				Close();
				return true;
			}
		}
		#endif
		
/*		
		//	not use guild chat command
		if( true == msg.Contains( TOKEN_GUILD_INVITE))
		{
			string[] tokens = msg.Split( TOKEN_SPLIT);
			if( 1 >= tokens.Length)
				return true;
		
			body_CS_GUILD_INVITE guildInvite = new body_CS_GUILD_INVITE( tokens[1]);
			byte[] packet = guildInvite.ClassToPacketBytes();
			AsNetworkMessageHandler.Instance.Send( packet);
			
			return true;
		}
		
		if( true == msg.Contains( TOKEN_GUILD_LEAVE))
		{
			if( null == AsUserInfo.Instance.GuildData)
				return true;
			
			string text = string.Format( AsTableManager.Instance.GetTbl_String(237), AsUserInfo.Instance.GuildData.szGuildName);
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(1240), text, 
				AsTableManager.Instance.GetTbl_String(1264), AsTableManager.Instance.GetTbl_String(1151),
				this, "WithdrawConfirm", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);
			
			return true;
		}
*/
		
		return false;
	}
	
	private void WithdrawConfirm()
	{
		body_CS_GUILD_EXIT guildExit = new body_CS_GUILD_EXIT();
		byte[] packet = guildExit.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( packet);
	}
	
	public void SendChat()
	{
		if( true == CommandJudgment( textField.Text))
		{
			textField.spriteText.transform.localPosition = new Vector3( 0.0f, 0.0f, -0.2f);
			textField.Text = m_strDefaultText;
		}
		
		if( 0 >= textField.Text.Length || textField.Text.Equals( m_strDefaultText))
		{
			textField.spriteText.transform.localPosition = new Vector3( 0.0f, 0.0f, -0.2f);
			textField.Text = m_strDefaultText;
			return;
		}
		
		AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
		if( null == userEntity)
			return;
		
		string name = userEntity.GetProperty<string>( eComponentProperty.NAME);
		string msg = textField.Text;
		
		eCHATTYPE chatType = GetChatType( msg);

		if( eCHATTYPE.eCHATTYPE_GUILD == chatType)
		{
			if( true == AsInstanceDungeonManager.Instance.CheckInIndun() || true == AsPvpManager.Instance.CheckInArena())
				return;
		}

		switch( chatType)
		{
		case eCHATTYPE.eCHATTYPE_PUBLIC:
			{
				string strRes = _Substring_TokenNormalChat( msg);
			
				if( strRes.Length > 0)
				{
					if( false == _isChatLock())
					{
						body_CS_CHAT_MESSAGE chat = new body_CS_CHAT_MESSAGE( strRes, chatType);
						byte[] data = chat.ClassToPacketBytes();
						AsNetworkMessageHandler.Instance.Send( data);
					}
				}
			}
			break;
		case eCHATTYPE.eCHATTYPE_SERVER:
		case eCHATTYPE.eCHATTYPE_SYSTEM:
			{
				if( false == _isChatLock())
				{
					body_CS_CHAT_MESSAGE chat = new body_CS_CHAT_MESSAGE( msg, chatType);
					byte[] data = chat.ClassToPacketBytes();
					AsNetworkMessageHandler.Instance.Send( data);
				}
			}
			break;
		case eCHATTYPE.eCHATTYPE_GUILD:
		case eCHATTYPE.eCHATTYPE_MAP:
		case eCHATTYPE.eCHATTYPE_PARTY:
			{
				if( 0 >= msg.Length)
					break;

				string[] tokens = msg.Split( TOKEN_SPLIT);
				string typeToken = tokens[0];

				if( ( 1 < tokens.Length) && ( ( 0 == string.Compare( typeToken, TOKEN_GUILD_CHAT1)) || ( 0 == string.Compare( typeToken, TOKEN_PARTY_CHAT1))))
				{
					msg = msg.Remove( 0, typeToken.Length + 1);
				}

				if( eCHATTYPE.eCHATTYPE_PARTY == chatType)
				{
					if( null == AsPartyManager.Instance.GetPartyMember( userEntity.UniqueId))
					{
						AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(1477), eCHATTYPE.eCHATTYPE_SYSTEM);
						break;
					}
				}

				if( eCHATTYPE.eCHATTYPE_GUILD == chatType)
				{
					if( null == AsUserInfo.Instance.GuildData)
					{
						AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(1478), eCHATTYPE.eCHATTYPE_SYSTEM);
						break;
					}
				}
			
				if( false == _isChatLock())
				{
					body_CS_CHAT_MESSAGE chat = new body_CS_CHAT_MESSAGE( msg, chatType);
					byte[] data = chat.ClassToPacketBytes();
					AsNetworkMessageHandler.Instance.Send( data);
				}
			}
			break;
		case eCHATTYPE.eCHATTYPE_PRIVATE:
			{
				if( 0 >= msg.Length)
					break;

				string[] tokens = msg.Split( TOKEN_SPLIT);
				string typeToken = tokens[0];

				if( ( 2 >= tokens.Length) || ( 0 != string.Compare( typeToken, TOKEN_WHISPER_CHAT1)))
				{
					if( 0 >= filterField.Text.Length)
						return;

					if( false == _isChatLock())
					{
						string receiver = AsUtil.GetRealString( filterField.Text);
						PlayerPrefs.SetString( "LatestWhisper", receiver);
						PlayerPrefs.Save();

						body_CS_CHAT_PRIVATE chat = new body_CS_CHAT_PRIVATE( receiver, msg);
						byte[] data = chat.ClassToPacketBytes();
						AsNetworkMessageHandler.Instance.Send( data);
					}
				}
				else
				{
					string receiverToken = tokens[1];

					if( receiverToken == name)
					{
						AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(112), eCHATTYPE.eCHATTYPE_SYSTEM);
						break;
					}
	
					//dopamin #16068
					if( 0 >= receiverToken.Length)
					{
						string format = AsTableManager.Instance.GetTbl_String(113);
						string sysMsg = string.Format( format, receiverToken);
						AsChatManager.Instance.InsertChat( sysMsg, eCHATTYPE.eCHATTYPE_SYSTEM);
						break;
					}
	
					msg = msg.Remove( 0, typeToken.Length + receiverToken.Length + 2);
					if( 0 >= msg.Length)
						return;
				
					if( false == _isChatLock())
					{
						PlayerPrefs.SetString( "LatestWhisper", receiverToken);
						PlayerPrefs.Save();
					
						body_CS_CHAT_PRIVATE chat = new body_CS_CHAT_PRIVATE( receiverToken, msg);
						byte[] data = chat.ClassToPacketBytes();
						AsNetworkMessageHandler.Instance.Send( data);
					}
				}
			}
			break;
		case eCHATTYPE.eCHATTYPE_REPLY:
			{
				if( null == latestWhisperer)
					break;

				if( latestWhisperer == name)
				{
					AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(112), eCHATTYPE.eCHATTYPE_SYSTEM);
					break;
				}

				string[] tokens = msg.Split( TOKEN_SPLIT);
				if( 1 < tokens.Length)
				{
					string typeToken = tokens[0];
					msg = msg.Remove( 0, typeToken.Length + 1);
				}

				if( 0 >= msg.Length)
					return;

				if( false == _isChatLock())
				{
					body_CS_CHAT_PRIVATE chat = new body_CS_CHAT_PRIVATE( latestWhisperer, msg);
					byte[] data = chat.ClassToPacketBytes();
					AsNetworkMessageHandler.Instance.Send( data);
				}
			}
			break;
		}
		
		textField.spriteText.transform.localPosition = new Vector3( 0.0f, 0.0f, -0.2f);
		textField.Text = m_strDefaultText;
	}
	
	private eCHATTYPE GetChatType( string chat)
	{
		eCHATTYPE ret = eCHATTYPE.eCHATTYPE_PUBLIC;

		string[] tokens = chat.Split( TOKEN_SPLIT);
		
		if( tokens[0].Equals( TOKEN_NORMAL_CHAT1) || tokens[0].Equals( TOKEN_NORMAL_CHAT2) || tokens[0].Equals( TOKEN_NORMAL_CHAT3))
		{
			ret = eCHATTYPE.eCHATTYPE_PUBLIC;
		}
		else if( tokens[0].Equals( TOKEN_WHISPER_CHAT1) || tokens[0].Equals( TOKEN_WHISPER_CHAT2) || tokens[0].Equals( TOKEN_WHISPER_CHAT3))
		{
			ret = eCHATTYPE.eCHATTYPE_PRIVATE;
		}
		else if( tokens[0].Equals( TOKEN_PARTY_CHAT1) || tokens[0].Equals( TOKEN_PARTY_CHAT2) || tokens[0].Equals( TOKEN_PARTY_CHAT3))
		{
			ret = eCHATTYPE.eCHATTYPE_PARTY;
		}
		else if( tokens[0].Equals( TOKEN_GUILD_CHAT1) || tokens[0].Equals( TOKEN_GUILD_CHAT2) || tokens[0].Equals( TOKEN_GUILD_CHAT3))
		{
			ret = eCHATTYPE.eCHATTYPE_GUILD;
		}
		else if( tokens[0].Equals( TOKEN_SHOUT_CHAT1) || tokens[0].Equals( TOKEN_SHOUT_CHAT2) || tokens[0].Equals( TOKEN_SHOUT_CHAT3))
		{
			ret = eCHATTYPE.eCHATTYPE_MAP;
		}
		else if( tokens[0].Equals( TOKEN_REPLY1) || tokens[0].Equals( TOKEN_REPLY2) || tokens[0].Equals( TOKEN_REPLY3))
		{
			ret = eCHATTYPE.eCHATTYPE_REPLY;
		}
		else
		{
			switch( filterType)
			{
			case CHAT_FILTER_TYPE.General:
				ret = eCHATTYPE.eCHATTYPE_PUBLIC;
				break;
			case CHAT_FILTER_TYPE.Party:
				ret = eCHATTYPE.eCHATTYPE_PARTY;
				break;
			case CHAT_FILTER_TYPE.Guild:
				ret = eCHATTYPE.eCHATTYPE_GUILD;
				break;
			case CHAT_FILTER_TYPE.Whisper:
				ret = eCHATTYPE.eCHATTYPE_PRIVATE;
				break;
			case CHAT_FILTER_TYPE.System:
//				ret = eCHATTYPE.eCHATTYPE_SYSTEM;
				ret = eCHATTYPE.eCHATTYPE_PUBLIC;
				break;
			}
		}

		Debug.Log( "ChatType : " + ret);

		return ret;
	}
	
	private void NewMsgAlarm( eCHATTYPE type, bool isMe)
	{
		if( false == gameObject.active)
			return;
		
		switch( filterType)
		{
		case CHAT_FILTER_TYPE.General:
			{
				switch( type)
				{
				case eCHATTYPE.eCHATTYPE_PRIVATE:
					news[ (int)CHAT_FILTER_TYPE.Whisper].NewMsgExist = true;
					break;
				case eCHATTYPE.eCHATTYPE_PARTY:
					news[ (int)CHAT_FILTER_TYPE.Party].NewMsgExist = true;
					break;
				case eCHATTYPE.eCHATTYPE_GUILD:
					news[ (int)CHAT_FILTER_TYPE.Guild].NewMsgExist = true;
					break;
				case eCHATTYPE.eCHATTYPE_SYSTEM:
					news[ (int)CHAT_FILTER_TYPE.System].NewMsgExist = true;
					break;
				}
			}
			break;
		case CHAT_FILTER_TYPE.Party:
			{
				switch( type)
				{
				case eCHATTYPE.eCHATTYPE_PUBLIC:
				case eCHATTYPE.eCHATTYPE_MAP:
				case eCHATTYPE.eCHATTYPE_SERVER:
					if( true == isMe)
						break;
					news[ (int)CHAT_FILTER_TYPE.General].NewMsgExist = true;
					break;
				case eCHATTYPE.eCHATTYPE_PRIVATE:
					news[ (int)CHAT_FILTER_TYPE.Whisper].NewMsgExist = true;
					break;
				case eCHATTYPE.eCHATTYPE_GUILD:
					news[ (int)CHAT_FILTER_TYPE.Guild].NewMsgExist = true;
					break;
				case eCHATTYPE.eCHATTYPE_SYSTEM:
					news[ (int)CHAT_FILTER_TYPE.System].NewMsgExist = true;
					break;
				}
			}
			break;
		case CHAT_FILTER_TYPE.Guild:
			{
				switch( type)
				{
				case eCHATTYPE.eCHATTYPE_PUBLIC:
				case eCHATTYPE.eCHATTYPE_MAP:
				case eCHATTYPE.eCHATTYPE_SERVER:
					if( true == isMe)
						break;
					news[ (int)CHAT_FILTER_TYPE.General].NewMsgExist = true;
					break;
				case eCHATTYPE.eCHATTYPE_PARTY:
					news[ (int)CHAT_FILTER_TYPE.Party].NewMsgExist = true;
					break;
				case eCHATTYPE.eCHATTYPE_PRIVATE:
					news[ (int)CHAT_FILTER_TYPE.Whisper].NewMsgExist = true;
					break;
				case eCHATTYPE.eCHATTYPE_SYSTEM:
					news[ (int)CHAT_FILTER_TYPE.System].NewMsgExist = true;
					break;
				}
			}
			break;
		case CHAT_FILTER_TYPE.Whisper:
			{
				switch( type)
				{
				case eCHATTYPE.eCHATTYPE_PUBLIC:
				case eCHATTYPE.eCHATTYPE_MAP:
				case eCHATTYPE.eCHATTYPE_SERVER:
					if( true == isMe)
						break;
					news[ (int)CHAT_FILTER_TYPE.General].NewMsgExist = true;
					break;
				case eCHATTYPE.eCHATTYPE_PARTY:
					news[ (int)CHAT_FILTER_TYPE.Party].NewMsgExist = true;
					break;
				case eCHATTYPE.eCHATTYPE_GUILD:
					news[ (int)CHAT_FILTER_TYPE.Guild].NewMsgExist = true;
					break;
				case eCHATTYPE.eCHATTYPE_SYSTEM:
					news[ (int)CHAT_FILTER_TYPE.System].NewMsgExist = true;
					break;
				}
			}
			break;
		case CHAT_FILTER_TYPE.System:
			{
				switch( type)
				{
				case eCHATTYPE.eCHATTYPE_PUBLIC:
				case eCHATTYPE.eCHATTYPE_MAP:
				case eCHATTYPE.eCHATTYPE_SERVER:
					if( true == isMe)
						break;
					news[ (int)CHAT_FILTER_TYPE.General].NewMsgExist = true;
					break;
				case eCHATTYPE.eCHATTYPE_PARTY:
					news[ (int)CHAT_FILTER_TYPE.Party].NewMsgExist = true;
					break;
				case eCHATTYPE.eCHATTYPE_GUILD:
					news[ (int)CHAT_FILTER_TYPE.Guild].NewMsgExist = true;
					break;
				case eCHATTYPE.eCHATTYPE_PRIVATE:
					news[ (int)CHAT_FILTER_TYPE.Whisper].NewMsgExist = true;
					break;
				}
			}
			break;
		}
	}
	
	public void InsertSystemChat( string msg, eCHATTYPE type=eCHATTYPE.eCHATTYPE_PUBLIC, bool isMe=false)
	{
		UIScrollList destList = allList;
		m_nScroll = 0;
		
		NewMsgAlarm( type, isMe);
		
		switch( type)
		{
		case eCHATTYPE.eCHATTYPE_PUBLIC:
			//
			chatBuff_normal.Add( msg);
			if( chatBuff_normal.Count > MAX_CHAT_HISTORY)
				chatBuff_normal.RemoveAt( 0);
			for( int i = 0; i < MAX_CHAT_SHOWLIST; i++)
			{
				UIListItem chatItem = destList.GetItem( i) as UIListItem;
				chatItem.Text = chatBuff_normal[chatBuff_normal.Count - MAX_CHAT_SHOWLIST + i];
				ActivateChatList( type, chatItem, isMe);
			}
			//
			break;
		case eCHATTYPE.eCHATTYPE_PRIVATE:
			string[] tokens = msg.Split( TOKEN_SPLIT);
			latestWhisperer = tokens[1];
			//
			chatBuff_whisper.Add( msg);
			if( chatBuff_whisper.Count > MAX_CHAT_HISTORY)
				chatBuff_whisper.RemoveAt( 0);
			for( int i = 0; i < MAX_CHAT_SHOWLIST; i++)
			{
				UIListItem chatItem = destList.GetItem( i) as UIListItem;
				chatItem.Text = chatBuff_whisper[chatBuff_whisper.Count - MAX_CHAT_SHOWLIST + i];
				ActivateChatList( type, chatItem, isMe);
			}
			//
			break;
		case eCHATTYPE.eCHATTYPE_PARTY:
			//
			chatBuff_party.Add( msg);
			if( chatBuff_party.Count > MAX_CHAT_HISTORY)
				chatBuff_party.RemoveAt( 0);
			for( int i = 0; i < MAX_CHAT_SHOWLIST; i++)
			{
				UIListItem chatItem = destList.GetItem( i) as UIListItem;
				chatItem.Text = chatBuff_party[chatBuff_party.Count - MAX_CHAT_SHOWLIST + i];
				ActivateChatList( type, chatItem, isMe);
			}
			//
			break;
		case eCHATTYPE.eCHATTYPE_GUILD:
			//
			chatBuff_guild.Add( msg);
			if( chatBuff_guild.Count > MAX_CHAT_HISTORY)
				chatBuff_guild.RemoveAt( 0);
			for( int i = 0; i < MAX_CHAT_SHOWLIST; i++)
			{
				UIListItem chatItem = destList.GetItem( i) as UIListItem;
				chatItem.Text = chatBuff_guild[chatBuff_guild.Count - MAX_CHAT_SHOWLIST + i];
				ActivateChatList( type, chatItem, isMe);
			}
			//
			break;
		case eCHATTYPE.eCHATTYPE_MAP:
			//
			chatBuff_normal.Add( msg);
			if( chatBuff_normal.Count > MAX_CHAT_HISTORY)
				chatBuff_normal.RemoveAt( 0);
			for( int i = 0; i < MAX_CHAT_SHOWLIST; i++)
			{
				UIListItem chatItem = destList.GetItem( i) as UIListItem;
				chatItem.Text = chatBuff_normal[chatBuff_normal.Count - MAX_CHAT_SHOWLIST + i];
				ActivateChatList( type, chatItem, isMe);
			}
			//
			break;
		case eCHATTYPE.eCHATTYPE_SERVER:
			//
			chatBuff_normal.Add( msg);
			if( chatBuff_normal.Count > MAX_CHAT_HISTORY)
				chatBuff_normal.RemoveAt( 0);
			for( int i = 0; i < MAX_CHAT_SHOWLIST; i++)
			{
				UIListItem chatItem = destList.GetItem( i) as UIListItem;
				chatItem.Text = chatBuff_normal[chatBuff_normal.Count - MAX_CHAT_SHOWLIST + i];
				ActivateChatList( type, chatItem, isMe);
			}
			//
			break;
		case eCHATTYPE.eCHATTYPE_SYSTEM:
			//
			chatBuff_system.Add( msg);
			if( chatBuff_system.Count > MAX_CHAT_HISTORY)
				chatBuff_system.RemoveAt( 0);
			for( int i = 0; i < MAX_CHAT_SHOWLIST; i++)
			{
				UIListItem chatItem = destList.GetItem( i) as UIListItem;
				chatItem.Text = chatBuff_system[chatBuff_system.Count - MAX_CHAT_SHOWLIST + i];
				ActivateChatList( type, chatItem, isMe);
			}
			//
			break;
		case eCHATTYPE.eCHATTYPE_SYSTEM_ITEM:
			//
			chatBuff_system.Add( msg);
			if( chatBuff_system.Count > MAX_CHAT_HISTORY)
				chatBuff_system.RemoveAt( 0);
			for( int i = 0; i < MAX_CHAT_SHOWLIST; i++)
			{
				UIListItem chatItem = destList.GetItem( i) as UIListItem;
				chatItem.Text = chatBuff_system[chatBuff_system.Count - MAX_CHAT_SHOWLIST + i];
				ActivateChatList( type, chatItem, isMe);
			}
			//
			break;
		case eCHATTYPE.eCHATTYPE_SYSTEM_QUEST:
			//
			chatBuff_system.Add( msg);
			if( chatBuff_system.Count > MAX_CHAT_HISTORY)
				chatBuff_system.RemoveAt( 0);
			for( int i = 0; i < MAX_CHAT_SHOWLIST; i++)
			{
				UIListItem chatItem = destList.GetItem( i) as UIListItem;
				chatItem.Text = chatBuff_system[chatBuff_system.Count - MAX_CHAT_SHOWLIST + i];
				ActivateChatList( type, chatItem, isMe);
			}
			//
			break;
		default:
			Debug.LogWarning( "Invalid chatting type");
			destList = null;
			break;
		}

		chatBuff_all.Add( msg);
		if( chatBuff_all.Count > MAX_CHAT_HISTORY)
			chatBuff_all.RemoveAt( 0);
		for( int i = 0; i < MAX_CHAT_SHOWLIST; i++)
		{
			UIListItem chatItem = destList.GetItem( i) as UIListItem;
			chatItem.Text = chatBuff_all[chatBuff_all.Count - MAX_CHAT_SHOWLIST + i];
			ActivateChatList( type, chatItem, isMe);
		}
	}
	
	public void InsertChat( string msg, eCHATTYPE type=eCHATTYPE.eCHATTYPE_PUBLIC, bool isMe=false, bool useFiltering = true)
	{
//		if( msg.IndexOf( '[') < msg.IndexOf( ']') && msg.IndexOf( ']') < msg.IndexOf( ':') && msg.IndexOf( ':') < msg.IndexOf( '/'))
//			return;
		
		if( msg.IndexOf(':') + 2 == msg.IndexOf( '/'))
			return;

		UIScrollList destList = allList;
		m_nScroll = 0;
		
		NewMsgAlarm( type, isMe);
		
		Color strColor = AsChatManager.Instance.m_Color_All;
		switch( type)
		{
		case eCHATTYPE.eCHATTYPE_PUBLIC:
			strColor = AsChatManager.Instance.m_Color_All;
			//
			chatBuff_normal.Add( msg);
			if( chatBuff_normal.Count > MAX_CHAT_HISTORY)
				chatBuff_normal.RemoveAt( 0);
			for( int i = 0; i < MAX_CHAT_SHOWLIST; i++)
			{
				UIListItem chatItem = destList.GetItem( i) as UIListItem;
				chatItem.Text = chatBuff_normal[chatBuff_normal.Count - MAX_CHAT_SHOWLIST + i];
				ActivateChatList( type, chatItem, isMe);
			}
			//
			break;
		case eCHATTYPE.eCHATTYPE_PRIVATE:
			strColor = AsChatManager.Instance.m_Color_Whisper;
			string[] tokens = msg.Split( TOKEN_SPLIT);
			latestWhisperer = tokens[1];
			//
			chatBuff_whisper.Add( msg);
			if( chatBuff_whisper.Count > MAX_CHAT_HISTORY)
				chatBuff_whisper.RemoveAt( 0);
			for( int i = 0; i < MAX_CHAT_SHOWLIST; i++)
			{
				UIListItem chatItem = destList.GetItem( i) as UIListItem;
				chatItem.Text = chatBuff_whisper[chatBuff_whisper.Count - MAX_CHAT_SHOWLIST + i];
				ActivateChatList( type, chatItem, isMe);
			}
			//
			break;
		case eCHATTYPE.eCHATTYPE_PARTY:
			strColor = AsChatManager.Instance.m_Color_Party;
			//
			chatBuff_party.Add( msg);
			if( chatBuff_party.Count > MAX_CHAT_HISTORY)
				chatBuff_party.RemoveAt( 0);
			for( int i = 0; i < MAX_CHAT_SHOWLIST; i++)
			{
				UIListItem chatItem = destList.GetItem( i) as UIListItem;
				chatItem.Text = chatBuff_party[chatBuff_party.Count - MAX_CHAT_SHOWLIST + i];
				ActivateChatList( type, chatItem, isMe);
			}
			//
			break;
		case eCHATTYPE.eCHATTYPE_GUILD:
			strColor = AsChatManager.Instance.m_Color_Guild;
			//
			chatBuff_guild.Add( msg);
			if( chatBuff_guild.Count > MAX_CHAT_HISTORY)
				chatBuff_guild.RemoveAt( 0);
			for( int i = 0; i < MAX_CHAT_SHOWLIST; i++)
			{
				UIListItem chatItem = destList.GetItem( i) as UIListItem;
				chatItem.Text = chatBuff_guild[chatBuff_guild.Count - MAX_CHAT_SHOWLIST + i];
				ActivateChatList( type, chatItem, isMe);
			}
			//
			break;
		case eCHATTYPE.eCHATTYPE_MAP:
			strColor = new Color( 0.85f, 0.07f, 0.0f, 1.0f);
			//
			chatBuff_normal.Add( msg);
			if( chatBuff_normal.Count > MAX_CHAT_HISTORY)
				chatBuff_normal.RemoveAt( 0);
			for( int i = 0; i < MAX_CHAT_SHOWLIST; i++)
			{
				UIListItem chatItem = destList.GetItem( i) as UIListItem;
				chatItem.Text = chatBuff_normal[chatBuff_normal.Count - MAX_CHAT_SHOWLIST + i];
				ActivateChatList( type, chatItem, isMe);
			}
			//
			break;
		case eCHATTYPE.eCHATTYPE_SERVER:
			strColor = new Color( 0.5f, 0.0f, 1.0f, 1.0f);
			//
			chatBuff_normal.Add( msg);
			if( chatBuff_normal.Count > MAX_CHAT_HISTORY)
				chatBuff_normal.RemoveAt( 0);
			for( int i = 0; i < MAX_CHAT_SHOWLIST; i++)
			{
				UIListItem chatItem = destList.GetItem( i) as UIListItem;
				chatItem.Text = chatBuff_normal[chatBuff_normal.Count - MAX_CHAT_SHOWLIST + i];
				ActivateChatList( type, chatItem, isMe);
			}
			//
			break;
		case eCHATTYPE.eCHATTYPE_SYSTEM:
			strColor = AsChatManager.Instance.m_Color_System;
			//
			chatBuff_system.Add( msg);
			if( chatBuff_system.Count > MAX_CHAT_HISTORY)
				chatBuff_system.RemoveAt( 0);
			for( int i = 0; i < MAX_CHAT_SHOWLIST; i++)
			{
				UIListItem chatItem = destList.GetItem( i) as UIListItem;
				chatItem.Text = chatBuff_system[chatBuff_system.Count - MAX_CHAT_SHOWLIST + i];
				ActivateChatList( type, chatItem, isMe);
			}
			//
			break;
		case eCHATTYPE.eCHATTYPE_SYSTEM_ITEM:
			strColor = new Color( 1.0f, 0.6f, 0.6f, 1.0f);
			//
			chatBuff_system.Add( msg);
			if( chatBuff_system.Count > MAX_CHAT_HISTORY)
				chatBuff_system.RemoveAt( 0);
			for( int i = 0; i < MAX_CHAT_SHOWLIST; i++)
			{
				UIListItem chatItem = destList.GetItem( i) as UIListItem;
				chatItem.Text = chatBuff_system[chatBuff_system.Count - MAX_CHAT_SHOWLIST + i];
				ActivateChatList( type, chatItem, isMe);
			}
			//
			break;
		case eCHATTYPE.eCHATTYPE_SYSTEM_QUEST:
			strColor = new Color( 0.6f, 0.6f, 1.0f, 1.0f);
			//
			chatBuff_system.Add( msg);
			if( chatBuff_system.Count > MAX_CHAT_HISTORY)
				chatBuff_system.RemoveAt( 0);
			for( int i = 0; i < MAX_CHAT_SHOWLIST; i++)
			{
				UIListItem chatItem = destList.GetItem( i) as UIListItem;
				chatItem.Text = chatBuff_system[chatBuff_system.Count - MAX_CHAT_SHOWLIST + i];
				ActivateChatList( type, chatItem, isMe);
			}
			//
			break;
		default:
			Debug.LogWarning( "Invalid chatting type");
			destList = null;
			break;
		}

		chatBuff_all.Add( msg);
		if( chatBuff_all.Count > MAX_CHAT_HISTORY)
			chatBuff_all.RemoveAt( 0);
		for( int i = 0; i < MAX_CHAT_SHOWLIST; i++)
		{
			UIListItem chatItem = destList.GetItem( i) as UIListItem;
			chatItem.Text = chatBuff_all[chatBuff_all.Count - MAX_CHAT_SHOWLIST + i];
			ActivateChatList( type, chatItem, isMe);
		}
			
		_InitScroll();			
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
					break;
				}
			}
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
					break;
				}
			}
		}
	}
	
	public void ShowChatBalloon( UInt32 uniqueIdx, string msg, eCHATTYPE type, bool useFiltering = true)
	{
		if( msg.IndexOf( '/') == 0)
		{
			AsEmotionManager.Instance.SystemProcess( uniqueIdx, msg);
			return;
		}
		
		AsUserEntity entity = AsEntityManager.Instance.GetUserEntityByUniqueId( uniqueIdx);
		if( null == entity)
			return;
		
		string colorTag = "RGBA( 0, 0, 0, 1.0)";
		switch( type)
		{
		case eCHATTYPE.eCHATTYPE_PUBLIC:
			colorTag = AsChatManager.Instance.m_Color_All.ToString();
			break;
		case eCHATTYPE.eCHATTYPE_PRIVATE:
			colorTag = AsChatManager.Instance.m_Color_Whisper.ToString();
			break;
		case eCHATTYPE.eCHATTYPE_PARTY:
			colorTag = AsChatManager.Instance.m_Color_Party.ToString();
			break;
		case eCHATTYPE.eCHATTYPE_GUILD:
			colorTag = AsChatManager.Instance.m_Color_Guild.ToString();
			break;
		case eCHATTYPE.eCHATTYPE_MAP:
			colorTag = AsChatManager.Instance.m_Color_Map.ToString();
			break;
		case eCHATTYPE.eCHATTYPE_SERVER:
			colorTag = AsChatManager.Instance.m_Color_World.ToString();
			break;
		case eCHATTYPE.eCHATTYPE_SYSTEM:
			colorTag = AsChatManager.Instance.m_Color_System.ToString();
			break;
		default:
			Debug.LogWarning( "Invalid chatting type");
			break;
		}
		
		//$yde - 20130108
		if (useFiltering == true)
			msg = AsTableManager.Instance.TextFiltering_Balloon( msg);
		
		entity.AddChatBalloon( chatBalloon, colorTag + msg);
	}
	
	public void SetDefaultText( string msg)
	{
		SetFocusInputField( msg);
	}
	
	public void SetFocusInputField( string defaultMsg)
	{
#if UNITY_IPHONE
		textField.autoCorrect = false;
#endif
		
#if false
		StringBuilder sb = new StringBuilder();

		switch( filterType)
		{
		case CHAT_FILTER_TYPE.Party:
			sb.AppendFormat( "{0} ", TOKEN_PARTY_CHAT1);
			break;
		case CHAT_FILTER_TYPE.Guild:
			sb.AppendFormat( "{0} ", TOKEN_GUILD_CHAT1);
			break;
		case CHAT_FILTER_TYPE.Whisper:
			{
				string latestWhisper = PlayerPrefs.GetString( "LatestWhisper");
				if( string.Empty == latestWhisper)
					sb.AppendFormat( "{0} ", TOKEN_WHISPER_CHAT1);
				else
					sb.AppendFormat( "{0} {1} ", TOKEN_WHISPER_CHAT1, latestWhisper);
			}
			break;
		default:
			break;
		}

		if( true == defaultMsg.Contains( TOKEN_WHISPER_CHAT1))
		{
			sb.Remove( 0, sb.Length);
			sb.Append( defaultMsg);
		}
#endif

//		textField.Text = sb.ToString();
		textField.Text = defaultMsg;
//		textField.hideInput = true;

		UIManager.instance.FocusObject = textField;
	}
	
	public void Open()
	{
		gameObject.SetActiveRecursively( true);
//		UIManager.instance.FocusObject = textField;
	}
	
	public void Close()
	{
		gameObject.SetActiveRecursively( false);
	}
	
	public bool IsOpen()
	{
		return gameObject.active;
	}

	private void _CloseBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			Close();
		}
	}
	
	private void _InitTokens()
	{
		TOKEN_NORMAL_CHAT1 = AsTableManager.Instance.GetTbl_String(1479);
		TOKEN_NORMAL_CHAT2 = AsTableManager.Instance.GetTbl_String(1485);
		TOKEN_NORMAL_CHAT3 = AsTableManager.Instance.GetTbl_String(1491);
		TOKEN_WHISPER_CHAT1 = AsTableManager.Instance.GetTbl_String(1480);
		TOKEN_WHISPER_CHAT2 = AsTableManager.Instance.GetTbl_String(1486);
		TOKEN_WHISPER_CHAT3 = AsTableManager.Instance.GetTbl_String(1492);
		TOKEN_PARTY_CHAT1 = AsTableManager.Instance.GetTbl_String(1482);
		TOKEN_PARTY_CHAT2 = AsTableManager.Instance.GetTbl_String(1488);
		TOKEN_PARTY_CHAT3 = AsTableManager.Instance.GetTbl_String(1494);
		TOKEN_GUILD_CHAT1 = AsTableManager.Instance.GetTbl_String(1483);
		TOKEN_GUILD_CHAT2 = AsTableManager.Instance.GetTbl_String(1489);
		TOKEN_GUILD_CHAT3 = AsTableManager.Instance.GetTbl_String(1495);
		TOKEN_SHOUT_CHAT1 = AsTableManager.Instance.GetTbl_String(1484);
		TOKEN_SHOUT_CHAT2 = AsTableManager.Instance.GetTbl_String(1490);
		TOKEN_SHOUT_CHAT3 = AsTableManager.Instance.GetTbl_String(1496);
		TOKEN_REPLY1 = AsTableManager.Instance.GetTbl_String(1481);
		TOKEN_REPLY2 = AsTableManager.Instance.GetTbl_String(1487);
		TOKEN_REPLY3 = AsTableManager.Instance.GetTbl_String(1493);
	}
	
	private string _Substring_TokenNormalChat( string msg)
	{
		string strRes = "";
		string strBuf = "";
		int nLength = 0;
		
		if( msg.Contains( TOKEN_NORMAL_CHAT1))
		{
			strBuf = TOKEN_NORMAL_CHAT1 + TOKEN_SPLIT;
			nLength = msg.Length - strBuf.Length;

			if( 0 >= nLength)
				return strRes;

			strRes = msg.Substring( strBuf.Length, nLength);
		}
		else if( msg.Contains( TOKEN_NORMAL_CHAT2))
		{
			strBuf = TOKEN_NORMAL_CHAT2 + TOKEN_SPLIT;
			nLength = msg.Length - strBuf.Length;
			
			if( 0 >= nLength)
				return strRes;

			strRes = msg.Substring( strBuf.Length, nLength);
		}
		else if( msg.Contains( TOKEN_NORMAL_CHAT3))
		{
			strBuf = TOKEN_NORMAL_CHAT3 + TOKEN_SPLIT;
			nLength = msg.Length - strBuf.Length;
			
			if( 0 >= nLength)
				return strRes;

			strRes = msg.Substring( strBuf.Length, nLength);
		}
		else
			strRes = msg;
		
		return strRes;
	}
	
	private bool _isChatLock()
	{
		float fCurTime = Time.realtimeSinceStartup;
		
		if( ( ( fCurTime - m_fLatestMsgSendTime) < m_fSendMsgLockTimeGab) || true == m_bLocked)
		{
			m_nSendMsgLockCountBuf++;
			
			if( m_nSendMsgLockCount <= m_nSendMsgLockCountBuf)
			{
				if( false == m_bLocked)
				{
					m_fSendMsgLockTimeBuf = fCurTime;
					m_bLocked = true;
				}
				
				float fTime = m_fSendMsgLockTime - ( fCurTime - m_fSendMsgLockTimeBuf);
				
				if( fTime <= 0.0f)
				{
					m_fSendMsgLockTimeBuf = fCurTime;
					m_fLatestMsgSendTime = fCurTime;
					m_nSendMsgLockCountBuf = 0;
					m_bLocked = false;
					return false;
				}
				
				string strMsg = string.Format( AsTableManager.Instance.GetTbl_String(1476), ( fTime < 1.0f) ? 1 : (int)fTime);
				AsChatManager.Instance.InsertChat( strMsg, eCHATTYPE.eCHATTYPE_SYSTEM);

				return true;
			}
		}
		
		m_fSendMsgLockTimeBuf = fCurTime;
		m_fLatestMsgSendTime = fCurTime;
		
		return false;
	}
	
	//$yde
	public bool CheckChatLocked()
	{
		return _isChatLock();
	}
	
	public void EmoticonPanelCreated()
	{
		m_fSendMsgLockTimeGab = AsTableManager.Instance.GetTbl_GlobalWeight_Record( 40).Value / 1000.0f;
		m_nSendMsgLockCount = (int)( AsTableManager.Instance.GetTbl_GlobalWeight_Record( 41).Value);
		m_fSendMsgLockTime = AsTableManager.Instance.GetTbl_GlobalWeight_Record( 42).Value / 1000.0f;
	}

	public void OpenWhisperMode( string name)
	{
		Open();
		StartCoroutine( "ToggleWhisperMode", name);
	}

	IEnumerator ToggleWhisperMode( string name)
	{
		yield return null;

		btnController.SetWhisperMode( name);
	}

	IEnumerator MaximizeProc()
	{
		yield return null;

		Maximize();
	}

	Vector3 listPos = new Vector3( 2.37f, -4.4f, 0.0f);
	void Maximize()
	{
		float screenWidth = background.center.RenderCamera.orthographicSize * background.center.RenderCamera.aspect * 2.0f;

		filterFieldBg.width = 10.0f;
		filterFieldBg.CalcSize();
		Vector3 orgPos = filterField.transform.localPosition;
		filterField.width = 10.0f;
		filterField.transform.localPosition = new Vector3( 5.0f, orgPos.y, orgPos.z);
		filterField.CalcSize();

		textFieldBg.width = screenWidth - 10.0f;
		textFieldBg.CalcSize();
		orgPos = textField.transform.localPosition;
		textField.width = screenWidth - 10.0f;
		textField.transform.localPosition = new Vector3( -( screenWidth - 10.0f) * 0.5f, orgPos.y, orgPos.z);
		textField.CalcSize();

		allList.transform.localPosition = listPos;
		allList.viewableArea.Set( screenWidth - 8.4f, background.center.height + 0.5f);
		allList.UpdateCamera();
		
		background.SetColor( bgColor);
	}
	
	public void InitChatListItems()
	{
		allList.ClearList( true);

		chatBuff_all.Clear();
		chatBuff_normal.Clear();
		chatBuff_party.Clear();
		chatBuff_guild.Clear();
		chatBuff_whisper.Clear();
		chatBuff_system.Clear();

		for( int i = 0; i < MAX_CHAT_SHOWLIST; i++)
		{
			_CreateListItem( allList);

			chatBuff_all.Add( "");
			chatBuff_normal.Add( "");
			chatBuff_party.Add( "");
			chatBuff_guild.Add( "");
			chatBuff_whisper.Add( "");
			chatBuff_system.Add( "");
		}
	}
	
	private void _CreateListItem(UIScrollList scrollList)
	{
		UIListItem chatItem = scrollList.CreateItem( chatListItem) as UIListItem;
		chatItem.collider.enabled = false;
		chatItem.width = scrollList.viewableArea.x;
		chatItem.spriteText.maxWidth = scrollList.viewableArea.x;
		chatItem.CalcSize();
		AsLanguageManager.Instance.SetFontFromSystemLanguage( chatItem.spriteText);
		chatItem.Text = "";
	}

	private void _BtnDelegate_ChatTopEnd( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			int nScrollEnd = _GetScrollTopEnd();
			if( nScrollEnd > 0)
			{
				m_nScroll = nScrollEnd;
				_UpdateScroll();
			}
		}
	}

	private void _BtnDelegate_ChatTop( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			m_nScroll++;
			if( false == _UpdateScroll())
				m_nScroll--;
		}
	}

	private void _BtnDelegate_ChatBottom( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			m_nScroll--;
			if( m_nScroll < 0)
				m_nScroll = 0;
			else
				_UpdateScroll();
		}
	}

	private void _BtnDelegate_ChatBottomEnd( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			m_nScroll = 0;
			_UpdateScroll();
		}
	}
	
	private void _InitScroll()
	{
		m_nScroll = 0;
		
		if( null != allList && allList.Count > 0)
		{
			if( CHAT_FILTER_TYPE.None == filterType)
			{
				for( int i = 0; i < MAX_CHAT_SHOWLIST; i++)
				{
					UIListItem chatItem = allList.GetItem( i) as UIListItem;
					
					int nIndex = chatBuff_all.Count - MAX_CHAT_SHOWLIST + i - m_nScroll;
					if( 0 <= nIndex && chatBuff_all.Count > nIndex && chatBuff_all[nIndex].Length > 0)
						chatItem.Text = chatBuff_all[nIndex];
					else
						chatItem.Text = "";
				}
			}
			else if( CHAT_FILTER_TYPE.General == filterType)
			{
				for( int i = 0; i < MAX_CHAT_SHOWLIST; i++)
				{
					UIListItem chatItem = allList.GetItem( i) as UIListItem;

					int nIndex = chatBuff_normal.Count - MAX_CHAT_SHOWLIST + i - m_nScroll;
					if( 0 <= nIndex && chatBuff_normal.Count > nIndex && chatBuff_normal[nIndex].Length > 0)
						chatItem.Text = chatBuff_normal[nIndex];
					else
						chatItem.Text = "";
				}
			}
			else if( CHAT_FILTER_TYPE.Party == filterType)
			{
				for( int i = 0; i < MAX_CHAT_SHOWLIST; i++)
				{
					UIListItem chatItem = allList.GetItem( i) as UIListItem;

					int nIndex = chatBuff_party.Count - MAX_CHAT_SHOWLIST + i - m_nScroll;
					if( 0 <= nIndex && chatBuff_party.Count > nIndex && chatBuff_party[nIndex].Length > 0)
						chatItem.Text = chatBuff_party[nIndex];
					else
						chatItem.Text = "";
				}
			}
			else if( CHAT_FILTER_TYPE.Guild == filterType)
			{
				for( int i = 0; i < MAX_CHAT_SHOWLIST; i++)
				{
					UIListItem chatItem = allList.GetItem( i) as UIListItem;

					int nIndex = chatBuff_guild.Count - MAX_CHAT_SHOWLIST + i - m_nScroll;
					if( 0 <= nIndex && chatBuff_guild.Count > nIndex && chatBuff_guild[nIndex].Length > 0)
						chatItem.Text = chatBuff_guild[nIndex];
					else
						chatItem.Text = "";
				}
			}
			else if( CHAT_FILTER_TYPE.Whisper == filterType)
			{
				for( int i = 0; i < MAX_CHAT_SHOWLIST; i++)
				{
					UIListItem chatItem = allList.GetItem( i) as UIListItem;

					int nIndex = chatBuff_whisper.Count - MAX_CHAT_SHOWLIST + i - m_nScroll;
					if( 0 <= nIndex && chatBuff_whisper.Count > nIndex && chatBuff_whisper[nIndex].Length > 0)
						chatItem.Text = chatBuff_whisper[nIndex];
					else
						chatItem.Text = "";
				}
			}
			else if( CHAT_FILTER_TYPE.System == filterType)
			{
				for( int i = 0; i < MAX_CHAT_SHOWLIST; i++)
				{
					UIListItem chatItem = allList.GetItem( i) as UIListItem;
					chatItem.Text = chatBuff_system[chatBuff_system.Count - MAX_CHAT_SHOWLIST + i - m_nScroll];

					int nIndex = chatBuff_system.Count - MAX_CHAT_SHOWLIST + i - m_nScroll;
					if( 0 <= nIndex && chatBuff_system.Count > nIndex && chatBuff_system[nIndex].Length > 0)
						chatItem.Text = chatBuff_system[nIndex];
					else
						chatItem.Text = "";
				}
			}
		}
	}

	private bool _UpdateScroll()
	{
		if( null != allList)
		{
			if( CHAT_FILTER_TYPE.None == filterType)
			{
				for( int i = 0; i < MAX_CHAT_SHOWLIST; i++)
				{
					UIListItem chatItem = allList.GetItem( i) as UIListItem;
					
					int nIndex = chatBuff_all.Count - MAX_CHAT_SHOWLIST + i - m_nScroll;
					if( 0 <= nIndex && chatBuff_all.Count > nIndex && chatBuff_all[nIndex].Length > 0)
						chatItem.Text = chatBuff_all[nIndex];
					else
						return false;
				}
			}
			else if( CHAT_FILTER_TYPE.General == filterType)
			{
				for( int i = 0; i < MAX_CHAT_SHOWLIST; i++)
				{
					UIListItem chatItem = allList.GetItem( i) as UIListItem;

					int nIndex = chatBuff_normal.Count - MAX_CHAT_SHOWLIST + i - m_nScroll;
					if( 0 <= nIndex && chatBuff_normal.Count > nIndex && chatBuff_normal[nIndex].Length > 0)
						chatItem.Text = chatBuff_normal[nIndex];
					else
						return false;
				}
			}
			else if( CHAT_FILTER_TYPE.Party == filterType)
			{
				for( int i = 0; i < MAX_CHAT_SHOWLIST; i++)
				{
					UIListItem chatItem = allList.GetItem( i) as UIListItem;

					int nIndex = chatBuff_party.Count - MAX_CHAT_SHOWLIST + i - m_nScroll;
					if( 0 <= nIndex && chatBuff_party.Count > nIndex && chatBuff_party[nIndex].Length > 0)
						chatItem.Text = chatBuff_party[nIndex];
					else
						return false;
				}
			}
			else if( CHAT_FILTER_TYPE.Guild == filterType)
			{
				for( int i = 0; i < MAX_CHAT_SHOWLIST; i++)
				{
					UIListItem chatItem = allList.GetItem( i) as UIListItem;

					int nIndex = chatBuff_guild.Count - MAX_CHAT_SHOWLIST + i - m_nScroll;
					if( 0 <= nIndex && chatBuff_guild.Count > nIndex && chatBuff_guild[nIndex].Length > 0)
						chatItem.Text = chatBuff_guild[nIndex];
					else
						return false;
				}
			}
			else if( CHAT_FILTER_TYPE.Whisper == filterType)
			{
				for( int i = 0; i < MAX_CHAT_SHOWLIST; i++)
				{
					UIListItem chatItem = allList.GetItem( i) as UIListItem;

					int nIndex = chatBuff_whisper.Count - MAX_CHAT_SHOWLIST + i - m_nScroll;
					if( 0 <= nIndex && chatBuff_whisper.Count > nIndex && chatBuff_whisper[nIndex].Length > 0)
						chatItem.Text = chatBuff_whisper[nIndex];
					else
						return false;
				}
			}
			else if( CHAT_FILTER_TYPE.System == filterType)
			{
				for( int i = 0; i < MAX_CHAT_SHOWLIST; i++)
				{
					UIListItem chatItem = allList.GetItem( i) as UIListItem;
					chatItem.Text = chatBuff_system[chatBuff_system.Count - MAX_CHAT_SHOWLIST + i - m_nScroll];

					int nIndex = chatBuff_system.Count - MAX_CHAT_SHOWLIST + i - m_nScroll;
					if( 0 <= nIndex && chatBuff_system.Count > nIndex && chatBuff_system[nIndex].Length > 0)
						chatItem.Text = chatBuff_system[nIndex];
					else
						return false;
				}
			}
			
			return true;
		}
		
		return false;
	}
	
	private int _GetScrollTopEnd()
	{
		if( null != allList)
		{
			int nScroll = 0;
			
			if( CHAT_FILTER_TYPE.None == filterType)
			{
				for( int i = 0; i < MAX_CHAT_HISTORY; i++)
				{
					int nIndex = chatBuff_all.Count - MAX_CHAT_SHOWLIST - i;
					if( 0 <= nIndex && chatBuff_all.Count > nIndex && chatBuff_all[nIndex].Length > 0)
						nScroll = i;
					else
						return nScroll;
				}
			}
			else if( CHAT_FILTER_TYPE.General == filterType)
			{
				for( int i = 0; i < MAX_CHAT_HISTORY; i++)
				{
					int nIndex = chatBuff_normal.Count - MAX_CHAT_SHOWLIST - i;
					if( 0 <= nIndex && chatBuff_normal.Count > nIndex && chatBuff_normal[nIndex].Length > 0)
						nScroll = i;
					else
						return nScroll;
				}
			}
			else if( CHAT_FILTER_TYPE.Party == filterType)
			{
				for( int i = 0; i < MAX_CHAT_HISTORY; i++)
				{
					int nIndex = chatBuff_party.Count - MAX_CHAT_SHOWLIST - i;
					if( 0 <= nIndex && chatBuff_party.Count > nIndex && chatBuff_party[nIndex].Length > 0)
						nScroll = i;
					else
						return nScroll;
				}
			}
			else if( CHAT_FILTER_TYPE.Guild == filterType)
			{
				for( int i = 0; i < MAX_CHAT_HISTORY; i++)
				{
					int nIndex = chatBuff_guild.Count - MAX_CHAT_SHOWLIST - i;
					if( 0 <= nIndex && chatBuff_guild.Count > nIndex && chatBuff_guild[nIndex].Length > 0)
						nScroll = i;
					else
						return nScroll;
				}
			}
			else if( CHAT_FILTER_TYPE.Whisper == filterType)
			{
				for( int i = 0; i < MAX_CHAT_HISTORY; i++)
				{
					int nIndex = chatBuff_whisper.Count - MAX_CHAT_SHOWLIST - i;
					if( 0 <= nIndex && chatBuff_whisper.Count > nIndex && chatBuff_whisper[nIndex].Length > 0)
						nScroll = i;
					else
						return nScroll;
				}
			}
			else if( CHAT_FILTER_TYPE.System == filterType)
			{
				for( int i = 0; i < MAX_CHAT_HISTORY; i++)
				{
					int nIndex = chatBuff_system.Count - MAX_CHAT_SHOWLIST - i;
					if( 0 <= nIndex && chatBuff_system.Count > nIndex && chatBuff_system[nIndex].Length > 0)
						nScroll = i;
					else
						return nScroll;
				}
			}
			
			return 0;
		}
		
		return 0;
	}
}
