using UnityEngine;
using System.Collections;

public class AsGuildNewPanel_Info : AsGuildNewPanel_Base 
{
	public SpriteText 		m_guildName = null;
	public SpriteText 		m_guildDescription = null;

	public SpriteText 		m_titleGuildNotice = null;
	public UIButton		m_btnEditGuildNotice = null;
	public SpriteText 		m_guildNotice = null;

	public SpriteText 		m_titleGuildInfo = null;
	public SpriteText 		m_titleGuildMaster = null;
	public SpriteText 		m_guildMaster = null;
	public SpriteText 		m_titleGuildMember = null;
	public SpriteText 		m_guildMember = null;
	public SpriteText 		m_titleGuildGold = null;
	public SpriteText 		m_guildGold = null;
	public SpriteText 		m_titleStackPoint = null;
	public SpriteText 		m_stackPoint = null;
	public SpriteText 		m_titleAblePoint = null;
	public SpriteText 		m_ablePoint = null;

	public SpriteText 		m_titleBuff = null;
	public SpriteText[] 	m_buffInfo;
	public SpriteText[] 	m_buffTime;

	public SpriteText 		m_titleGuildQuest = null;
	public SpriteText[] 	m_guildQuestName;
	public UIButton[] 		m_btnGuildQuest;

	public UIButton 		m_btnGuildLevelUp;
	public UIButton 		m_btnBuffSetting;
	public UIButton 		m_btnEditRecruitText;
	public UIButton 		m_btnGuildList;

	// Use this for initialization
	void Awake()
	{
	}

	void Start () 
	{
		m_titleGuildNotice.Text = AsTableManager.Instance.GetTbl_String(1936);

		m_titleBuff.Text = AsTableManager.Instance.GetTbl_String(2212);

		m_titleGuildInfo.Text = AsTableManager.Instance.GetTbl_String(1113);
		m_titleGuildMaster.Text = AsTableManager.Instance.GetTbl_String(1243);
		m_titleGuildMember.Text = AsTableManager.Instance.GetTbl_String(1244);
		m_titleGuildGold.Text = AsTableManager.Instance.GetTbl_String(1580);
		m_titleStackPoint.Text = AsTableManager.Instance.GetTbl_String(1036);
		m_titleAblePoint.Text = AsTableManager.Instance.GetTbl_String(1036);

		m_titleGuildQuest.Text = AsTableManager.Instance.GetTbl_String(819);

		m_btnEditGuildNotice.SetInputDelegate(  OnEditGuildNotice );

		for (int i=0; i<m_btnGuildQuest.Length; i++) 
		{
			m_btnGuildQuest[i].SetInputDelegate(  OnGuildQuest );
			m_btnGuildQuest[i].Text = "show";
		}

		m_btnGuildLevelUp.Text = AsTableManager.Instance.GetTbl_String(1279);
		m_btnBuffSetting.Text = AsTableManager.Instance.GetTbl_String(2212);
		m_btnEditRecruitText.Text = AsTableManager.Instance.GetTbl_String(4261);
		m_btnGuildList.Text = AsTableManager.Instance.GetTbl_String(1004);

		m_btnGuildLevelUp.SetInputDelegate(  OnGuildLevelUp );
		m_btnBuffSetting.SetInputDelegate(  OnBuffSetting );
		m_btnEditRecruitText.SetInputDelegate(  OnEditRecruitText );
		m_btnGuildList.SetInputDelegate(  OnGuildList );
	}

	override public void Init(System.Object data)
	{
		body_SC_GUILD_INFO_RESULT info = (body_SC_GUILD_INFO_RESULT)data;

		m_guildName.Text = info.szGuildName;
		m_guildDescription.Text = string.Format ("{0}{1} {2}{3}", AsTableManager.Instance.GetTbl_String (1094),
		                                   												info.nLevel,
		                                   												"night mare",
		                                   												"[buff buff]");

		//	guild notice
		string szNotice = AsUtil.GetRealString (info.szGuildNotice);
		if (szNotice.Length == 0) 
		{
			m_guildNotice.Text = AsTableManager.Instance.GetTbl_String(1701);
		} 
		else 
		{
			m_guildNotice.Text = szNotice;
		}

		//	guild info
		m_guildMaster.Text = info.szMasterName;
		m_guildMember.Text = string.Format ("{0}/{1}", info.nConnectCnt, info.nLevel * 10);
		m_guildGold.Text = "0";
		m_stackPoint.Text = "0";
		m_ablePoint.Text = "0";


		//	guild buff
		for (int i=0; i<m_buffInfo.Length; i++) 
		{
			m_buffInfo[i].Text = "[buff buff]" + i;
		}

		for (int i=0; i<m_buffTime.Length; i++) 
		{
			m_buffTime[i].Text = "[buff time]" + i;
		}

		//	guild quest
		for (int i=0; i<m_guildQuestName.Length; i++) 
		{
			m_guildQuestName[i].Text = "guild quest " + i;
		}


		//	permission
		eGUILDPERMISSION permission = AsUserInfo.Instance.GuildData.ePermission;
		if (eGUILDPERMISSION.eGUILDPERMISSION_NOTICE != (eGUILDPERMISSION.eGUILDPERMISSION_NOTICE & permission)) 
		{
			m_btnEditGuildNotice.controlIsEnabled = false;
			m_btnEditGuildNotice.spriteText.Color = Color.gray;
		}

		if (eGUILDPERMISSION.eGUILDPERMISSION_INVITE != (eGUILDPERMISSION.eGUILDPERMISSION_INVITE & permission)) 
		{
		}
	}

	void OnEditGuildNotice( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			Debug.LogError("edit guild notice button press " );
		}
	}

	void OnGuildQuest( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			for (int i=0; i<m_btnGuildQuest.Length; i++) 
			{
				if( ptr.targetObj ==  m_btnGuildQuest[i] )
				{
					Debug.LogError("quest button press [" + i + "]" );
					break;
				}
			}
		}
	}

	void OnGuildLevelUp( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			Debug.LogError("OnGuildLevelUp button press " );
		}
	}

	void OnBuffSetting( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			Debug.LogError("OnBuffSetting button press " );
		}
	}

	void OnEditRecruitText( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			Debug.LogError("OnEditRecruitText button press " );
		}
	}

	void OnGuildList( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			body_CS_GUILD_UI_SCROLL scroll = new body_CS_GUILD_UI_SCROLL( eGUILD_UI_SCROLL.eGUILD_UI_SCROLL_GUILD, 0 , false);
			byte[] packet = scroll.ClassToPacketBytes();
			AsNetworkMessageHandler.Instance.Send( packet);
		}
	}

	// Update is called once per frame
	void Update () 
	{
	
	}
}
