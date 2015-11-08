using UnityEngine;
using System.Collections;

public class AsGuildApplicantPopup : MonoBehaviour
{
	public SpriteText title = null;
	public UIButton userInfoBtn = null;
	public UIButton whisperBtn = null;
	public UIButton inviteFriendBtn = null;
	public UIButton invitePartyBtn = null;
	public UIButton approveBtn = null;
	public UIButton deleteBtn = null;
	public UIButton closeBtn = null;
	private body2_SC_GUILD_MEMBER_INFO_RESULT data = null;
	private GameObject parentObj = null;
	
	// Use this for initialization
	void Start()
	{
		userInfoBtn.Text = AsTableManager.Instance.GetTbl_String(1161);
		whisperBtn.Text = AsTableManager.Instance.GetTbl_String(1162);
		inviteFriendBtn.Text = AsTableManager.Instance.GetTbl_String(1164);
		invitePartyBtn.Text = AsTableManager.Instance.GetTbl_String(1163);
		approveBtn.Text = AsTableManager.Instance.GetTbl_String(1260);
		deleteBtn.Text = AsTableManager.Instance.GetTbl_String(1302);
		closeBtn.Text = AsTableManager.Instance.GetTbl_String(1317);

		if( eGUILDPERMISSION.eGUILDPERMISSION_INVITE != ( eGUILDPERMISSION.eGUILDPERMISSION_INVITE & AsUserInfo.Instance.GuildData.ePermission))
		{
			deleteBtn.SetControlState( UIButton.CONTROL_STATE.DISABLED);
			deleteBtn.spriteText.Color = Color.gray;
		}
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	public void Init( body2_SC_GUILD_MEMBER_INFO_RESULT data, GameObject parent)
	{
		this.data = data;
		parentObj = parent;

		title.Text = data.szCharName;
	}
	
	private void OnUserInfoBtn()
	{
		Debug.Log( "OnUserInfoBtn");
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		//dopamin
		body_CS_OTHER_INFO memberInfoDetail = new body_CS_OTHER_INFO((int)eOTHER_INFO_TYPE.eOTHER_INFO_TYPE_GUILD_MEMBER, data.nSessionIdx,data.nCharUniqKey, 0 ); 
	//	body_CS_GUILD_MEMBER_INFO_DETAIL memberInfoDetail = new body_CS_GUILD_MEMBER_INFO_DETAIL( data.nCharUniqKey);
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
		AsPartySender.SendPartyInvite( data.nSessionIdx, data.nCharUniqKey);
	}
	
	private void OnApproveBtn()
	{
		Debug.Log( "OnApproveBtn");
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		body_CS_GUILD_SEARCH_JOIN_APPROVE joinApprove = new body_CS_GUILD_SEARCH_JOIN_APPROVE( data.nCharUniqKey, eGUILDJOINTYPE.eGUILDJOINTYPE_APPROVE);
		byte[] packet = joinApprove.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( packet);
		
//		parentObj.BroadcastMessage( "RemoveApprovedUser", data.nCharUniqKey, SendMessageOptions.DontRequireReceiver);
		
		OnCloseBtn();
	}
	
	private void OnDeleteBtn()
	{
		Debug.Log( "OnDeleteBtn");
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		body_CS_GUILD_SEARCH_JOIN_APPROVE joinApprove = new body_CS_GUILD_SEARCH_JOIN_APPROVE( data.nCharUniqKey, eGUILDJOINTYPE.eGUILDJOINTYPE_NOT_APPROVE);
		byte[] packet = joinApprove.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( packet);
		
//		parentObj.BroadcastMessage( "RemoveApprovedUser", data.nCharUniqKey, SendMessageOptions.DontRequireReceiver);

		OnCloseBtn();
	}
	
	private void OnCloseBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		parentObj.BroadcastMessage( "ClosePupup", SendMessageOptions.DontRequireReceiver);
	}
}
