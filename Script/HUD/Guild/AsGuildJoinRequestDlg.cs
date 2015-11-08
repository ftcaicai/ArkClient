using UnityEngine;
using System.Collections;

public class AsGuildJoinRequestDlg : MonoBehaviour
{
	public SpriteText title = null;
	public UIButton closeBtn = null;
	public UIButton nameBtn = null;
	public UIButton levelBtn = null;
	public UIButton masterBtn = null;
	public SpriteText guildName = null;
	public SpriteText guildLevel = null;
	public SpriteText guildMaster = null;
	public SpriteText pr = null;
	public UIButton subscribeBtn = null;
	public UIButton whisperBtn = null;
	
	private body2_SC_GUILD_SEARCH_RESULT data = null;
	private GameObject parent = null;
	
	// Use this for initialization
	void Start()
	{
		title.Text = AsTableManager.Instance.GetTbl_String(1246);
		nameBtn.Text = AsTableManager.Instance.GetTbl_String(1272);
		levelBtn.Text = AsTableManager.Instance.GetTbl_String(1242);
		masterBtn.Text = AsTableManager.Instance.GetTbl_String(1243);
		subscribeBtn.Text = AsTableManager.Instance.GetTbl_String(1268);
		whisperBtn.Text = AsTableManager.Instance.GetTbl_String(1115);
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	public void Open( body2_SC_GUILD_SEARCH_RESULT data, GameObject parent)
	{
		this.data = data;
		this.parent = parent;
		
		guildName.Text = data.szGuildName;
		guildLevel.Text = data.nLevel.ToString();
		guildMaster.Text = data.szMasterName;
		pr.Text = data.szGuildPublicize;
		
		if( null != AsUserInfo.Instance.GuildData && null != subscribeBtn )
		{
			subscribeBtn.SetControlState( UIButton.CONTROL_STATE.DISABLED );
			subscribeBtn.spriteText.Color = Color.gray;
		}
	}

	private void OnSubscribeBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		body_CS_GUILD_SEARCH_JOIN join = new body_CS_GUILD_SEARCH_JOIN( data.nGuildIdx);
		byte[] packet = join.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( packet);
		
		parent.BroadcastMessage( "CloseRequestDlg", SendMessageOptions.DontRequireReceiver);
	}
	
	private void OnWhisperBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

//		string msg = string.Format( "/w {0} ", data.szMasterName);
//		AsChatManager.Instance.SetFocusInputField( msg);

		AsChatFullPanel.Instance.OpenWhisperMode( data.szMasterName);
	}
	
	private void OnCloseBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		parent.BroadcastMessage( "CloseRequestDlg", SendMessageOptions.DontRequireReceiver);
	}
}
