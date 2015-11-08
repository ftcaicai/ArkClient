using UnityEngine;
using System.Collections;

public class AsRankPopup : MonoBehaviour
{
	[SerializeField] SpriteText title = null;
	[SerializeField] UIButton closeBtn = null;
	[SerializeField] UIButton userInfoBtn = null;
	[SerializeField] UIButton whisperBtn = null;
	[SerializeField] UIButton inviteFriendBtn = null;
//	[SerializeField] UIButton invitePartyBtn = null; #27460.
//	[SerializeField] UIButton inviteGuildBtn = null; #27460.
	[SerializeField] UIButton mailBtn = null;

	private sRANKINFO rankInfo = null;
	public sRANKINFO RankInfo	{ set { rankInfo = value; } }

	// Use this for initialization
	void Start()
	{
		Debug.Assert( null != rankInfo);
		
		// #24545 ilmeda
		//	inviteFriendBtn.spriteText.color = Color.gray;  #27460.
		//	invitePartyBtn.spriteText.color = Color.gray; #27460.
		//	inviteGuildBtn.spriteText.color = Color.gray; #27460.
		
		title.Text = rankInfo.szCharName;
		userInfoBtn.Text = AsTableManager.Instance.GetTbl_String(1161);
		whisperBtn.Text = AsTableManager.Instance.GetTbl_String(1162);
		inviteFriendBtn.Text = AsTableManager.Instance.GetTbl_String(1164);
		//	invitePartyBtn.Text = AsTableManager.Instance.GetTbl_String(1163); #27460.
		//	inviteGuildBtn.Text = AsTableManager.Instance.GetTbl_String(1165);#27460.
		mailBtn.Text = AsTableManager.Instance.GetTbl_String(824);
		// #22495 dopamin..
		//	inviteFriendBtn.SetControlState(UIButton.CONTROL_STATE.DISABLED);#27460.
		//	invitePartyBtn.SetControlState(UIButton.CONTROL_STATE.DISABLED);#27460.
		//	inviteGuildBtn.SetControlState(UIButton.CONTROL_STATE.DISABLED);#27460.
	}
	
	// Update is called once per frame
	void Update()
	{
	}

	public void Close()
	{
		GameObject.Destroy( gameObject);
	}

	private void OnCloseBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		Close();
	}

	private void OnUserInfoBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		body_CS_OTHER_INFO memberInfoDetail = new body_CS_OTHER_INFO( (int)eOTHER_INFO_TYPE.eOTHER_INFO_TYPE_RANK, rankInfo.nSessionIdx, rankInfo.nCharUniqKey, rankInfo.nUserUniqKey);
		byte[] packet = memberInfoDetail.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( packet);

		Close();
	}

	private void OnWhisperBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		AsChatFullPanel.Instance.OpenWhisperMode( rankInfo.szCharName);

		Close();
	}

	private void OnInviteFriendBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		AsCommonSender.SendFriendInvite( rankInfo.szCharName);

		Close();
	}
//#27460.
/*	private void OnInvitePartyBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		AsPartySender.SendPartyInvite( rankInfo.nSessionIdx, rankInfo.nCharUniqKey);

		Close();
	}

	private void OnInviteGuildBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		if( AsUserInfo.Instance.GuildData != null)
		{
			bool enableInvite = ( eGUILDPERMISSION.eGUILDPERMISSION_INVITE == ( eGUILDPERMISSION.eGUILDPERMISSION_INVITE & AsUserInfo.Instance.GuildData.ePermission)) ? true : false;
			if( true == enableInvite)
			{
				body_CS_GUILD_INVITE guildInvite = new body_CS_GUILD_INVITE( rankInfo.szCharName);
				byte[] packet = guildInvite.ClassToPacketBytes();
				AsNetworkMessageHandler.Instance.Send( packet);
			}
		}

		Close();
	}
*/
	private void OnMailBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		AsSocialManager.Instance.SocialUI.CloseFindFriendDlg();
		AsSocialManager.Instance.SocialUI.CloseSocialStoreDlg();
		AsSocialManager.Instance.SocialUI.CloseSocailCloneDlg();

		AsHudDlgMgr.Instance.OpenWritePostBoxDlg( rankInfo.szCharName);
		AsHudDlgMgr.Instance.postBoxDlg.Depth = gameObject.transform.position.z - 1.0f;

		Close();
	}
}
