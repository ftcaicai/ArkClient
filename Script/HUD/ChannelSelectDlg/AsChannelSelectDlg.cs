using UnityEngine;
using System.Collections;
using System.Text;

public class AsChannelSelectDlg : MonoBehaviour
{
	[SerializeField]UIScrollList listLeft = null;
	[SerializeField]UIScrollList listRight = null;
	[SerializeField]SpriteText m_TextTitle;
	[SerializeField]SpriteText curChannel = null;
	[SerializeField]SpriteText coolTimeText = null;
	[SerializeField]SpriteText desc = null;
	public GameObject item = null;
	
	private UIListButton prevSelect = null;
	private float prevClickTime = 0.0f;
	private int timeGap = 0;
	
	// Use this for initialization
	void Start()
	{
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextTitle);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( curChannel);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( coolTimeText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( desc);
		
		m_TextTitle.Text = AsTableManager.Instance.GetTbl_String(4012);
		curChannel.Text = string.Format( AsTableManager.Instance.GetTbl_String(4098), AsUserInfo.Instance.currentChannelName);
		desc.Text = AsTableManager.Instance.GetTbl_String(4099);

		InitData();
	}
	
	public void InitData()
	{
		if (ArkQuestmanager.instance.CheckHaveOpenUIType(OpenUIType.OPEN_CHANNEL) != null)
			AsCommonSender.SendClearOpneUI(OpenUIType.OPEN_CHANNEL);

		StringBuilder sb = new StringBuilder();

		int i = 0;
		int count = AsChannelListData.Instance.Count;
		for( i = 0; i < count; i++)
		{
			body2_GC_CHANNEL_LIST channel = AsChannelListData.Instance.GetData(i);
			if( null == channel)
				continue;

			sb.Remove( 0, sb.Length);
			if( ( 0 == channel.nMinLevel) && ( 0 == channel.nMaxLevel))
				sb.Append( channel.szChannelName);
			else
				sb.AppendFormat( "{0} (Lv {1} ~ {2})", channel.szChannelName, channel.nMinLevel, channel.nMaxLevel);
			
			switch( channel.eConfusion)
			{
			case eCONFUSIONTYPE.eCONFUSION_NORMAL:
				sb.AppendFormat( " RGBA(0.0, 1.0, 0.0, 1.0)[{0}]", AsTableManager.Instance.GetTbl_String(1096));
				break;
			case eCONFUSIONTYPE.eCONFUSION_BUSY:
				sb.AppendFormat( " RGBA(1.0, 1.0, 0.0, 1.0)[{0}]", AsTableManager.Instance.GetTbl_String(1098));
				break;
			case eCONFUSIONTYPE.eCONFUSION_SATURATION:
				sb.AppendFormat( " RGBA(1.0, 0.0, 0.0, 1.0)[{0}]", AsTableManager.Instance.GetTbl_String(1097));
				break;
			default:
				Debug.Log( "Channel confusion : " + channel.eConfusion);
				break;
			}
			
			UIScrollList list = ( 0 == ( i & 0x01)) ? listLeft : listRight;
			UIListButton listBtn = list.CreateItem( item, sb.ToString()) as UIListButton;
			listBtn.width = list.viewableArea.x;
			if( ( AsUserInfo.Instance.currentChannel == channel.nChannel) || ( eCONFUSIONTYPE.eCONFUSION_INSPECTION == channel.eConfusion))
			{
				listBtn.SetControlState( UIButton.CONTROL_STATE.DISABLED);
				listBtn.spriteText.Color = Color.gray;
			}
			listBtn.UpdateCamera();
			
			AsChannelListBtn dataContainer = listBtn.gameObject.GetComponent<AsChannelListBtn>();
			dataContainer.channelData = channel;
		}

		if( 0 != ( i & 0x01))
		{
			UIListButton listBtn = listRight.CreateItem( item) as UIListButton;
			listBtn.width = listRight.viewableArea.x;
			listBtn.Text = "";
			listBtn.SetControlState( UIButton.CONTROL_STATE.DISABLED);
			listBtn.spriteText.Color = Color.gray;
			listBtn.UpdateCamera();

			AsChannelListBtn dataContainer = listBtn.gameObject.GetComponent<AsChannelListBtn>();
			dataContainer.channelData = null;
		}

		AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
		StringBuilder sbKey = new StringBuilder( "ChannelCooltime_");
		sbKey.Append( userEntity.UniqueId);
		
		string ticks = PlayerPrefs.GetString( sbKey.ToString());
		if( ( null != ticks) && ( string.Empty != ticks))
		{
			System.DateTime savedTime = new System.DateTime( long.Parse( ticks));
			System.TimeSpan elapsedSpan = new System.TimeSpan( System.DateTime.Now.Ticks - savedTime.Ticks);
			
			if( 30.0f > elapsedSpan.TotalSeconds)
			{
				timeGap = 30 - (int)elapsedSpan.TotalSeconds;
				SetChannelBtnState( false);
				CancelInvoke( "CooltimeUpdate");
				InvokeRepeating( "CooltimeUpdate", 0.0f, 1.0f);
			}
			else
			{
				coolTimeText.Text = string.Format( AsTableManager.Instance.GetTbl_String(4072), 0);
			}
		}
		else
		{
			coolTimeText.Text = string.Format( AsTableManager.Instance.GetTbl_String(4072), 0);
		}
	}
	
	void SetChannelBtnState( bool flag)
	{
		for( int i = 0; i < listLeft.Count; i++)
		{
			UIListButton listBtn = listLeft.GetItem(i) as UIListButton;

			AsChannelListBtn dataContainer = listBtn.gameObject.GetComponent<AsChannelListBtn>();

			if( null == dataContainer.channelData)
				continue;

			if( ( true == flag) && ( AsUserInfo.Instance.currentChannel != dataContainer.channelData.nChannel) && ( eCONFUSIONTYPE.eCONFUSION_INSPECTION != dataContainer.channelData.eConfusion))
			{
				listBtn.SetControlState( UIListButton.CONTROL_STATE.NORMAL);
				listBtn.spriteText.Color = Color.white;
			}
			else
			{
				listBtn.SetControlState( UIListButton.CONTROL_STATE.DISABLED);
				listBtn.spriteText.Color = Color.gray;
			}
		}

		for( int i = 0; i < listRight.Count; i++)
		{
			UIListButton listBtn = listRight.GetItem(i) as UIListButton;

			AsChannelListBtn dataContainer = listBtn.gameObject.GetComponent<AsChannelListBtn>();
			
			if( null == dataContainer.channelData)
				continue;
			
			if( ( true == flag) && ( AsUserInfo.Instance.currentChannel != dataContainer.channelData.nChannel) && ( eCONFUSIONTYPE.eCONFUSION_INSPECTION != dataContainer.channelData.eConfusion))
			{
				listBtn.SetControlState( UIListButton.CONTROL_STATE.NORMAL);
				listBtn.spriteText.Color = Color.white;
			}
			else
			{
				listBtn.SetControlState( UIListButton.CONTROL_STATE.DISABLED);
				listBtn.spriteText.Color = Color.gray;
			}
		}
	}
	
	void CooltimeUpdate()
	{
		timeGap--;
		if( 0 >= timeGap)
		{
			timeGap = 0;
			SetChannelBtnState( true);
			CancelInvoke( "CooltimeUpdate");
		}
		
		coolTimeText.Text = string.Format( AsTableManager.Instance.GetTbl_String(4072), timeGap);
	}
	
	// Update is called once per frame
	void Update()
	{
		if( true == listLeft.IsScrolling)
		{
			listRight.ScrollPosition = listLeft.ScrollPosition;
		}
		else if( true == listRight.IsScrolling)
		{
			listLeft.ScrollPosition = listRight.ScrollPosition;
		}
	}
	
	private void OnSelectLeft()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		UIListButton selItem = listLeft.LastClickedControl as UIListButton;
		
		float curClickTime = Time.time;
		if( ( 0.4f >= ( curClickTime - prevClickTime)) && ( prevSelect == selItem))
		{
			AsUserInfo.Instance.ApplyInGameDataOnChannelInfo();
			AsChannelListBtn dataContainer = selItem.gameObject.GetComponent<AsChannelListBtn>();
			dataContainer.EnterChannel();
		}
		
		prevSelect = selItem;
		prevClickTime = curClickTime;
	}
	
	private void OnSelectRight()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		UIListButton selItem = listRight.LastClickedControl as UIListButton;
		
		float curClickTime = Time.time;
		if( ( 0.4f >= ( curClickTime - prevClickTime)) && ( prevSelect == selItem))
		{
			AsUserInfo.Instance.ApplyInGameDataOnChannelInfo();
			AsChannelListBtn dataContainer = selItem.gameObject.GetComponent<AsChannelListBtn>();
			dataContainer.EnterChannel();
		}
		
		prevSelect = selItem;
		prevClickTime = curClickTime;
	}
	
	private void OnCloseBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		AsHudDlgMgr.Instance.CloseChannelSelectDlg();
	}
}
