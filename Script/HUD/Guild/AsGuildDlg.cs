using UnityEngine;
using System.Collections;

public class AsGuildDlg : MonoBehaviour
{
	public SpriteText title = null;
	public UIPanelTab infoTabBtn = null;
	public UIPanelTab memberTabBtn = null;
	public UIPanelTab manageTabBtn = null;
	public UIPanelTab listTabBtn = null;
	public AsGuildPanelManager panelManager = null;
//	private body_SC_GUILD_INFO_RESULT guildData = null;
	
	// Use this for initialization
	void Start()
	{
		title.Text = AsTableManager.Instance.GetTbl_String(1177);
		infoTabBtn.Text = AsTableManager.Instance.GetTbl_String(1246);
		memberTabBtn.Text = AsTableManager.Instance.GetTbl_String(1247);
		manageTabBtn.Text = AsTableManager.Instance.GetTbl_String(1248);
		listTabBtn.Text = AsTableManager.Instance.GetTbl_String(4261);
		
		if( null == AsUserInfo.Instance.GuildData)
		{
			manageTabBtn.controlIsEnabled = false;
			manageTabBtn.spriteText.Color = Color.gray;
			
			infoTabBtn.controlIsEnabled = false;
			infoTabBtn.spriteText.Color = Color.gray;
			
			memberTabBtn.controlIsEnabled = false;
			memberTabBtn.spriteText.Color = Color.gray;
			
			infoTabBtn.Value 	= false;
			manageTabBtn.Value 	= false;
			memberTabBtn.Value 	= false;
			listTabBtn.Value 	= true;
			listTabBtn.panelShowingAtStart = true;
		}
#if false
		else
		{
			eGUILDPERMISSION permission = AsUserInfo.Instance.GuildData.ePermission;
			if( eGUILDPERMISSION.eGUILDPERMISSION_ALL != permission)
			{
				manageTabBtn.controlIsEnabled = false;
				manageTabBtn.spriteText.Color = Color.gray;
			}
			else
			{
				manageTabBtn.controlIsEnabled = true;
				manageTabBtn.spriteText.Color = Color.white;
			}
		}
#endif
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	public void Init()
	{
		panelManager.Init( AsGuildPanelManager.eGuildPanelState.Information, null);
	}
	
	public void InitInfoTab( body_SC_GUILD_INFO_RESULT data)
	{
//		guildData = data;
		panelManager.Init( AsGuildPanelManager.eGuildPanelState.Information, data);
	}
	
	public void InitMemberTab( body1_SC_GUILD_MEMBER_INFO_RESULT data)
	{
		panelManager.Init( AsGuildPanelManager.eGuildPanelState.MemberInfo, data);
	}
	
	public void InitManageTab( body_SC_GUILD_SUPERVISE_RESULT data)
	{
		panelManager.Init( AsGuildPanelManager.eGuildPanelState.Managing, data);
	}

	public void InitListTab( body1_SC_GUILD_SEARCH_RESULT data)
	{
		panelManager.Init( AsGuildPanelManager.eGuildPanelState.List, data);
	}
	
	public void InsertMemberList( body1_SC_GUILD_MEMBER_INFO_RESULT data)
	{
		panelManager.InsertMemberList( data);
	}
	
	public void InsertApplicantList( body1_SC_GUILD_MEMBER_INFO_RESULT data)
	{
		panelManager.InsertApplicantList( data);
	}
	
	public void DisplayInviteErrorMsg( string msg)
	{
		panelManager.DisplayInviteErrorMsg( msg);
	}
	
	public void UpdateGuildLevel( body_SC_GUILD_LEVEL_UP_RESULT data)
	{
		panelManager.UpdateGuildLevel( data);
	}
	
	public void UpdateGuildList( body1_SC_GUILD_SEARCH_RESULT data )
	{
		panelManager.UpdateGuildList( data );
	}
	
	public void RequestCurPageApplicant( bool isForced)
	{
		panelManager.RequestCurPageApplicant( isForced);
	}
	
	public void CloseInviteDlg()
	{
		panelManager.CloseInviteDlg();
	}
	
	private void OnCloseBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		AsHudDlgMgr.Instance.CloseGuildDlg();
		AsNotify.Instance.CloseAllMessageBox();
	}
	
	private void OnInfoTab()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		body_CS_GUILD_INFO guildInfo = new body_CS_GUILD_INFO(false);
		byte[] data = guildInfo.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}
	
	private void OnMemberTab()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		body_CS_GUILD_MEMBER_INFO memberInfo = new body_CS_GUILD_MEMBER_INFO();
		byte[] packet = memberInfo.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( packet);
	}
	
	private void OnManageTab()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		body_CS_GUILD_SUPERVISE supervise = new body_CS_GUILD_SUPERVISE();
		byte[] packet = supervise.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( packet);
	}
	
	private void OnListTab()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		body_CS_GUILD_INFO guildInfo = new body_CS_GUILD_INFO(true);
		byte[] data = guildInfo.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
		AsCommonSender.isSendGuild = true;
	}
	
}
