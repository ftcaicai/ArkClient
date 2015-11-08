using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class AsGuildPanel_Info : MonoBehaviour
{
	public SpriteText NAME = null;
	public SpriteText MASTER = null;
	public SpriteText MEMBER = null;
	public SpriteText LEVEL = null;
	public SpriteText NOTICE = null;
	public SpriteText guildName = null;
	public SpriteText guildMaster = null;
	public SpriteText guildMember = null;
	public SpriteText guildLevel = null;
	public UITextField guildNotice = null;
	public UIButton detailInfoBtn = null;
	public UIButton assignBtn = null;
	public UIButton inviteBtn = null;
	public UIButton withdrawBtn = null;
//	private int guildID = -1;
	private int curLevel = 0;
	private int curConnetCnt = 0;
	private GameObject inviteDlgObj = null;
	private string prevNotice = string.Empty;
	
	// Use this for initialization
	void Start()
	{
		NAME.Text = AsTableManager.Instance.GetTbl_String(1272);
		MASTER.Text = AsTableManager.Instance.GetTbl_String(1243);
		MEMBER.Text = AsTableManager.Instance.GetTbl_String(1244);
		LEVEL.Text = AsTableManager.Instance.GetTbl_String(1242);
		NOTICE.Text = AsTableManager.Instance.GetTbl_String(1245);
		detailInfoBtn.Text = AsTableManager.Instance.GetTbl_String(1265);
		assignBtn.Text = AsTableManager.Instance.GetTbl_String(1238);
		inviteBtn.Text = AsTableManager.Instance.GetTbl_String(1263);
		withdrawBtn.Text = AsTableManager.Instance.GetTbl_String(1264);
		
		guildNotice.SetFocusDelegate( OnFocusNotice);
		guildNotice.SetValidationDelegate( GuildNoticeValidateCallback);
		AsUtil.SetRenderingState( gameObject, false);
	}
	
	// Update is called once per frame
	void Update()
	{
	}

	string GuildNoticeValidateCallback( UITextField field, string text, ref int insPos)
	{
		text = Regex.Replace( text, "'", "");
		
		while( true)
		{
			int byteCount = System.Text.UTF8Encoding.UTF8.GetByteCount( text);
			int charCount = System.Text.UTF8Encoding.UTF8.GetCharCount( System.Text.UTF8Encoding.UTF8.GetBytes( text));
			if( ( byteCount <= 240) && ( charCount <= 80))
				break;

			text = text.Remove( text.Length - 1);
		}

		return text;
	}

	private void OnFocusNotice( UITextField field)
	{
//		field.autoCorrect = false;
		
		string szNotice = AsTableManager.Instance.GetTbl_String(1701);
		if( 0 == szNotice.CompareTo( field.Text))
			field.Text = "";
	}
	
	private void OnDisable()
	{
		if( null != AsHudDlgMgr.Instance.GuildDetailInfoDlg)
		{
			GameObject.DestroyImmediate( AsHudDlgMgr.Instance.GuildDetailInfoDlg);
			AsHudDlgMgr.Instance.GuildDetailInfoDlg = null;
		}
		
		if( null != inviteDlgObj)
		{
			GameObject.DestroyImmediate( inviteDlgObj);
			inviteDlgObj = null;
		}
	}
	
	public void Init( System.Object data)
	{
		AsUtil.SetRenderingState( gameObject, true);
		
		if( null == data)
			return;
		
		body_SC_GUILD_INFO_RESULT info = (body_SC_GUILD_INFO_RESULT)data;
		
		guildName.Text = info.szGuildName;
		guildMaster.Text = info.szMasterName;
		string msg = string.Format( "{0}/{1}", info.nConnectCnt, info.nLevel * 10);
		guildMember.Text = msg;
		guildLevel.Text = string.Format( "{0}{1}", AsTableManager.Instance.GetTbl_String(1094), info.nLevel);
//		guildID = info.nGuildIdx;
		curLevel = info.nLevel;
		curConnetCnt = info.nConnectCnt;
		string szNotice = AsUtil.GetRealString( info.szGuildNotice);
		if( 0 == szNotice.Length)
		{
			guildNotice.Text = AsTableManager.Instance.GetTbl_String(1701);
			prevNotice = guildNotice.Text;
		}
		else
		{
			guildNotice.Text = szNotice;
			prevNotice = szNotice;
		}

		eGUILDPERMISSION permission = AsUserInfo.Instance.GuildData.ePermission;
		if( eGUILDPERMISSION.eGUILDPERMISSION_NOTICE != ( eGUILDPERMISSION.eGUILDPERMISSION_NOTICE & permission))
		{
			assignBtn.SetControlState( UIButton.CONTROL_STATE.DISABLED);
			assignBtn.spriteText.Color = Color.gray;
			
			guildNotice.controlIsEnabled = false;
		}
		
		if( eGUILDPERMISSION.eGUILDPERMISSION_INVITE != ( eGUILDPERMISSION.eGUILDPERMISSION_INVITE & permission))
		{
			inviteBtn.SetControlState( UIButton.CONTROL_STATE.DISABLED);
			inviteBtn.spriteText.Color = Color.gray;
		}
	}
	
	public void UpdateGuildLevel( body_SC_GUILD_LEVEL_UP_RESULT data)
	{
		curLevel = data.nLevel;
		guildLevel.Text = string.Format( "{0}{1}", AsTableManager.Instance.GetTbl_String(1094), data.nLevel);
		guildMember.Text = string.Format( "{0}/{1}", curConnetCnt, data.nMaxMember);
	}
	
	private void OnInfoBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		if( null != AsHudDlgMgr.Instance.GuildDetailInfoDlg)
			return;

		GameObject go = Instantiate( Resources.Load( "UI/AsGUI/Guild/GUI_GuildDetailInfo")) as GameObject;
		AsHudDlgMgr.Instance.GuildDetailInfoDlg = go;
		AsGuildDetailInfo detailInfoDlg = go.GetComponentInChildren<AsGuildDetailInfo>();
		detailInfoDlg.SavedLevel = curLevel;
		
		body_CS_GUILD_INFO_DETAIL detailInfo = new body_CS_GUILD_INFO_DETAIL();
		byte[] data = detailInfo.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}
	
	private void OnAssignBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		if( false == AsTableManager.Instance.TextFiltering_Guild( guildNotice.Text))
		{
			AsNotify.Instance.MessageBox( string.Empty, AsTableManager.Instance.GetTbl_String(364), AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			return;
		}
		
		if( prevNotice == guildNotice.Text)
			return;
		
		prevNotice = guildNotice.Text;
		
		body_CS_GUILD_NOTICE notice = new body_CS_GUILD_NOTICE( guildNotice.Text);
		byte[] packet = notice.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( packet);
		
		AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(380), eCHATTYPE.eCHATTYPE_SYSTEM);
	}
	
	private void OnInviteBtn()
	{
		Debug.Log( "OnInviteBtn");
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		if( null != inviteDlgObj)
			return;
		
		inviteDlgObj = Instantiate( Resources.Load( "UI/AsGUI/Guild/GUI_GuildInvite")) as GameObject;
		AsGuildInviteDlg inviteDlg = inviteDlgObj.gameObject.GetComponentInChildren<AsGuildInviteDlg>();
		inviteDlg.Parent = gameObject;
	}
	
	private void OnWithdrawBtn()
	{
		Debug.Log( "OnWithdrawBtn");
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		string msg = string.Format( AsTableManager.Instance.GetTbl_String(237), AsUtil.GetRealString( guildName.Text));
		AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(1240), msg, 
			AsTableManager.Instance.GetTbl_String(1264), AsTableManager.Instance.GetTbl_String(1151),
			this, "WithdrawConfirm", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);
	}
	
	private void WithdrawConfirm()
	{
		body_CS_GUILD_EXIT guildExit = new body_CS_GUILD_EXIT();
		byte[] packet = guildExit.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( packet);
	}
	
	public void CloseInviteDlg()
	{
		GameObject.DestroyImmediate( inviteDlgObj);
		inviteDlgObj = null;
	}
	
	public void DisplayInviteErrorMsg( string msg)
	{
		if( null == inviteDlgObj)
		{
			Debug.LogWarning( "Invite dlg not exist");
			return;
		}
		
		AsGuildInviteDlg inviteDlg = inviteDlgObj.gameObject.GetComponentInChildren<AsGuildInviteDlg>();
		inviteDlg.DisplayErrorMsg( msg);
	}
}
