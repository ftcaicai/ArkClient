using UnityEngine;
using System.Collections;

public class AsGuildNewDlg : MonoBehaviour 
{
	public SpriteText 		m_MainTitle = null;

	public UIPanelTab 	m_guildInfoTabBtn = null;
	public UIPanelTab 	m_guildMemberTabBtn = null;
	public UIPanelTab 	m_guildRankingTabBtn = null;

	public AsGuildNewPanelManager	m_guildPanelManager = null;

	// Use this for initialization
	void Start () 
	{
		m_MainTitle.Text = AsTableManager.Instance.GetTbl_String(1177);

		m_guildInfoTabBtn.Text = AsTableManager.Instance.GetTbl_String(1246);
		m_guildMemberTabBtn.Text = AsTableManager.Instance.GetTbl_String(1247);
		m_guildRankingTabBtn.Text = AsTableManager.Instance.GetTbl_String(1662);

		m_guildInfoTabBtn.SetInputDelegate( OnGuildInfoTabBtnClick);
		m_guildMemberTabBtn.SetInputDelegate( OnGuildMemberTabBtnClick);
		m_guildRankingTabBtn.SetInputDelegate( OnGuildRankingTabBtnClick);

		m_guildInfoTabBtn.Value = true;
		m_guildInfoTabBtn.panelShowingAtStart = true;
		m_guildMemberTabBtn.Value = false;
		m_guildRankingTabBtn.Value = false;

		//	L03 build is not contained ranking
		m_guildRankingTabBtn.controlIsEnabled = false;
		m_guildRankingTabBtn.spriteText.Color = Color.gray;
		m_guildRankingTabBtn.Value = false;

		if( null == AsUserInfo.Instance.GuildData)
		{
			m_guildMemberTabBtn.controlIsEnabled = false;
			m_guildMemberTabBtn.spriteText.Color = Color.gray;
			m_guildMemberTabBtn.Value = false;
		}
	}


	void OnCloseBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		AsHudDlgMgr.Instance.CloseGuildDlg();
		AsNotify.Instance.CloseAllMessageBox();
	}

	public void Init()
	{
		if (null == AsUserInfo.Instance.GuildData) 
		{
			InitTab( eGuildNewPanelState.Main , null );
		} 
		else
		{
			body_CS_GUILD_INFO guildInfo = new body_CS_GUILD_INFO(false);
			byte[] data = guildInfo.ClassToPacketBytes();
			AsNetworkMessageHandler.Instance.Send( data);
		}
	}

	public void InitTab( eGuildNewPanelState	state , System.Object	data )
	{
		m_guildPanelManager.Init (state, data);
	}

	public void RequestCurrentPage(eGuildNewPanelState	state)
	{
		m_guildPanelManager.RequestCurrentPage (state);
	}

	void OnGuildInfoTabBtnClick( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			Debug.LogError("btn click OnGuildInfoTabBtnClick");

			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			Init ();
		}
	}

	void OnGuildMemberTabBtnClick( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			
			body_CS_GUILD_MEMBER_INFO memberInfo = new body_CS_GUILD_MEMBER_INFO();
			byte[] packet = memberInfo.ClassToPacketBytes();
			AsNetworkMessageHandler.Instance.Send( packet);
		}
	}

	void OnGuildRankingTabBtnClick( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			Debug.LogError("btn click OnGuildRankingTabBtnClick");
		}
	}

	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
