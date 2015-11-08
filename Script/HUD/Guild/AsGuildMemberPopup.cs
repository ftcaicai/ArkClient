using UnityEngine;
using System.Collections;

public class AsGuildMemberPopup : MonoBehaviour
{
	[SerializeField]SpriteText nameField = null;
	[SerializeField]UIButton userInfoBtn = null;
	[SerializeField]UIButton whisperBtn = null;
	[SerializeField]UIButton inviteFriendBtn = null;
	[SerializeField]UIButton invitePartyBtn = null;
	[SerializeField]UIButton guildBanBtn = null;
	[SerializeField]UIButton editAuthorityBtn = null;
	[SerializeField]UIButton masterHandoverBtn = null;
	[SerializeField]UIButton sendMailBtn = null;
	[SerializeField]UIButton closeBtn = null;
	private body2_SC_GUILD_MEMBER_INFO_RESULT data = null;
	private GameObject parentObj = null;
	private GameObject authorityDlg = null;
	
	// Use this for initialization
	void Start()
	{
		userInfoBtn.Text = AsTableManager.Instance.GetTbl_String(1161);
		whisperBtn.Text = AsTableManager.Instance.GetTbl_String(1162);
		inviteFriendBtn.Text = AsTableManager.Instance.GetTbl_String(1164);
		invitePartyBtn.Text = AsTableManager.Instance.GetTbl_String(1163);
		guildBanBtn.Text = AsTableManager.Instance.GetTbl_String(1252);
		editAuthorityBtn.Text = AsTableManager.Instance.GetTbl_String(1253);
		masterHandoverBtn.Text = AsTableManager.Instance.GetTbl_String(1254);
		sendMailBtn.Text = AsTableManager.Instance.GetTbl_String(824);
		closeBtn.Text = AsTableManager.Instance.GetTbl_String(1317);
	}
	
	void OnDisable()
	{
		ClosePopup();
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	public void Init( body2_SC_GUILD_MEMBER_INFO_RESULT data, GameObject parent)
	{
		this.data = data;
		parentObj = parent;
		
		nameField.Text = data.szCharName;
		
		string masterName = AsUserInfo.Instance.GuildData.szGuildMaster;
		
		eGUILDPERMISSION permission = AsUserInfo.Instance.GuildData.ePermission;
		
		if( ( eGUILDPERMISSION.eGUILDPERMISSION_EXILE != ( eGUILDPERMISSION.eGUILDPERMISSION_EXILE & permission))
			|| ( data.szCharName == masterName))
		{
			guildBanBtn.SetControlState( UIButton.CONTROL_STATE.DISABLED);
			guildBanBtn.spriteText.Color = Color.gray;
		}
		
		if( ( eGUILDPERMISSION.eGUILDPERMISSION_AUTHORIZE != ( eGUILDPERMISSION.eGUILDPERMISSION_AUTHORIZE & permission))
			|| ( data.szCharName == masterName))
		{
			editAuthorityBtn.SetControlState( UIButton.CONTROL_STATE.DISABLED);
			editAuthorityBtn.spriteText.Color = Color.gray;
		}
		
		if( ( eGUILDPERMISSION.eGUILDPERMISSION_ALL != permission)
			|| ( data.szCharName == masterName)
			|| ( masterName != AsUserInfo.Instance.SavedCharStat.charName_))
		{
			masterHandoverBtn.SetControlState( UIButton.CONTROL_STATE.DISABLED);
			masterHandoverBtn.spriteText.Color = Color.gray;
		}
	}
	
	public void ClosePopup()
	{
		GameObject.DestroyImmediate( authorityDlg);
		authorityDlg = null;
	}
	
	private void OnUserInfoBtn()
	{
		Debug.Log( "OnUserInfoBtn");
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		//dopamin
		body_CS_OTHER_INFO memberInfoDetail = new body_CS_OTHER_INFO((int)eOTHER_INFO_TYPE.eOTHER_INFO_TYPE_GUILD_MEMBER, data.nSessionIdx, data.nCharUniqKey, 0 ); 
		//body_CS_GUILD_MEMBER_INFO_DETAIL memberInfoDetail = new body_CS_GUILD_MEMBER_INFO_DETAIL( data.nCharUniqKey);
		byte[] packet = memberInfoDetail.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( packet);
	}

	private void OnWhisperBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
//		string msg = string.Format( "/w {0} ", data.szCharName);
//		AsChatManager.Instance.SetFocusInputField( msg);

		AsChatFullPanel.Instance.OpenWhisperMode( data.szCharName);
	}
	
	private void OnInviteFriendBtn()
	{
		Debug.Log( "OnInviteFriendBtn");
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		AsCommonSender.SendFriendInvite( data.szCharName);
	}
	
	private void OnInvitePartyBtn()
	{
		Debug.Log( "OnInvitePartyBtn");
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		if( false == data.bConnect)
		{
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126),
				AsTableManager.Instance.GetTbl_String(182), AsNotify.MSG_BOX_TYPE.MBT_OK,
				AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			return;
		}
		
		AsPartySender.SendPartyInvite( data.nSessionIdx, data.nCharUniqKey);
	}
	
	private void OnGuildBanBtn()
	{
		Debug.Log( "OnGuildBanBtn");
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		string strMsg = string.Format( AsTableManager.Instance.GetTbl_String(2190) , nameField.Text );
		AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126),
			strMsg,
			AsTableManager.Instance.GetTbl_String(1152),
			AsTableManager.Instance.GetTbl_String(1151),
			this,
			"processGuildBanOK",
			"processGuildBanCancel",
			AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL,
			AsNotify.MSG_BOX_ICON.MBI_QUESTION);
	}
	
	private void processGuildBanOK()
	{
		body_CS_GUILD_EXILE guildExile = new body_CS_GUILD_EXILE( data.nCharUniqKey);
		byte[] packet = guildExile.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( packet);
		
		parentObj.BroadcastMessage( "RemoveExileMember", data.nCharUniqKey, SendMessageOptions.DontRequireReceiver);
		
		OnCloseBtn();
	}

	private void processGuildBanCancel()
	{
		
	}
	
	private void OnEditAuthorityBtn()
	{
		Debug.Log( "OnEditAuthorityBtn");
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		if( null != authorityDlg)
			return;
		
		authorityDlg = Instantiate( Resources.Load( "UI/AsGUI/Guild/GUI_GuildAuthorityEditDlg")) as GameObject;
		AsGuildAuthorityEditDlg authorityEditDlg = authorityDlg.GetComponentInChildren<AsGuildAuthorityEditDlg>();
		authorityEditDlg.Init( data, gameObject);
	}
	
	private void OnMasterHandoverBtn()
	{
		Debug.Log( "OnMasterHandoverBtn");
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		string msg = string.Format( AsTableManager.Instance.GetTbl_String(188), data.szCharName);
		AsMessageBox msgBox = AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), msg, AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);
		msgBox.SetOkDelegate = _confirmMasterHandOver;
	}
	
	void _confirmMasterHandOver()
	{
		body_CS_GUILD_MASTER_CHANGE masterChange = new body_CS_GUILD_MASTER_CHANGE( data.nCharUniqKey);
		byte[] packet = masterChange.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( packet);
	}

	private void OnSendMailBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		AsHudDlgMgr.Instance.OpenWritePostBoxDlg( data.szCharName);

		parentObj.BroadcastMessage( "ClosePupup", SendMessageOptions.DontRequireReceiver);
	}
	
	private void OnCloseBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		parentObj.BroadcastMessage( "ClosePupup", SendMessageOptions.DontRequireReceiver);
	}
	
	private void ModifyUserPermission( eGUILDPERMISSION permission)	// #11380
	{
		data.ePermission = permission;
	}
}
