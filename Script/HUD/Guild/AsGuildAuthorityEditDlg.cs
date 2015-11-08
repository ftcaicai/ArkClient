using UnityEngine;
using System.Collections;

public class AsGuildAuthorityEditDlg : MonoBehaviour
{
	public SpriteText title = null;
	public UIRadioBtn noticeBtn = null;
	public UIRadioBtn publicityBtn = null;
	public UIRadioBtn joinBtn = null;
	public UIRadioBtn banishBtn = null;
	public UIRadioBtn depositBtn = null;
	public UIRadioBtn withdrawBtn = null;
	public UIRadioBtn authorityBtn = null;
	public UIButton defaultBtn = null;
	public UIButton assignBtn = null;
	private body2_SC_GUILD_MEMBER_INFO_RESULT data = null;
	private bool noticeFlag = true;
	private bool publicityFlag = true;
	private bool joinFlag = true;
	private bool banishFlag = true;
	private bool depositFlag = true;
	private bool withdrawFlag = true;
	private bool authorityFlag = true;
	private GameObject parent = null;
	
	// Use this for initialization
	void Start()
	{
		title.Text = AsTableManager.Instance.GetTbl_String(1253);
		noticeBtn.Text = AsTableManager.Instance.GetTbl_String(1255);
		publicityBtn.Text = AsTableManager.Instance.GetTbl_String(1256);
		joinBtn.Text = AsTableManager.Instance.GetTbl_String(1280);
		banishBtn.Text = AsTableManager.Instance.GetTbl_String(1252);
		depositBtn.Text = AsTableManager.Instance.GetTbl_String(1257);
		withdrawBtn.Text = AsTableManager.Instance.GetTbl_String(1258);
		authorityBtn.Text = AsTableManager.Instance.GetTbl_String(1259);
		defaultBtn.Text = AsTableManager.Instance.GetTbl_String(1262);
		assignBtn.Text = AsTableManager.Instance.GetTbl_String(1238);
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	public void Init( body2_SC_GUILD_MEMBER_INFO_RESULT data, GameObject parent)
	{
		this.data = data;
		this.parent = parent;
		
		noticeBtn.Value = ( eGUILDPERMISSION.eGUILDPERMISSION_NOTICE == ( eGUILDPERMISSION.eGUILDPERMISSION_NOTICE & data.ePermission)) ? true : false;
		publicityBtn.Value = ( eGUILDPERMISSION.eGUILDPERMISSION_PUBLICIZE == ( eGUILDPERMISSION.eGUILDPERMISSION_PUBLICIZE & data.ePermission)) ? true : false;
		joinBtn.Value = ( eGUILDPERMISSION.eGUILDPERMISSION_INVITE == ( eGUILDPERMISSION.eGUILDPERMISSION_INVITE & data.ePermission)) ? true : false;
		banishBtn.Value = ( eGUILDPERMISSION.eGUILDPERMISSION_EXILE == ( eGUILDPERMISSION.eGUILDPERMISSION_EXILE & data.ePermission)) ? true : false;
		depositBtn.Value = ( eGUILDPERMISSION.eGUILDPERMISSION_DEPOSIT == ( eGUILDPERMISSION.eGUILDPERMISSION_DEPOSIT & data.ePermission)) ? true : false;
		withdrawBtn.Value = ( eGUILDPERMISSION.eGUILDPERMISSION_WITHDRAW == ( eGUILDPERMISSION.eGUILDPERMISSION_WITHDRAW & data.ePermission)) ? true : false;
		authorityBtn.Value = ( eGUILDPERMISSION.eGUILDPERMISSION_AUTHORIZE == ( eGUILDPERMISSION.eGUILDPERMISSION_AUTHORIZE & data.ePermission)) ? true : false;

		noticeFlag = noticeBtn.Value;
		publicityFlag = publicityBtn.Value;
		joinFlag = joinBtn.Value;
		banishFlag = banishBtn.Value;
		depositFlag = depositBtn.Value;
		withdrawFlag = withdrawBtn.Value;
		authorityFlag = authorityBtn.Value;
	}
	
	private void OnNoticeBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		noticeFlag = !noticeFlag;
		noticeBtn.Value = noticeFlag;
	}
	
	private void OnPublicityBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		publicityFlag = !publicityFlag;
		publicityBtn.Value = publicityFlag;
	}
	
	private void OnJoinBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		joinFlag = !joinFlag;
		joinBtn.Value = joinFlag;
	}
	
	private void OnBanishBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		banishFlag = !banishFlag;
		banishBtn.Value = banishFlag;
	}
	
	private void OnDepositBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		depositFlag = !depositFlag;
		depositBtn.Value = depositFlag;
	}
	
	private void OnWithdrawBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		withdrawFlag = !withdrawFlag;
		withdrawBtn.Value = withdrawFlag;
	}
	
	private void OnAuthorityBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		authorityFlag = !authorityFlag;
		authorityBtn.Value = authorityFlag;
	}
	
	private void OnDefaultBtn()
	{
		Debug.Log( "OnDefaultBtn");
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		noticeBtn.Value = noticeFlag = false;
		publicityBtn.Value = publicityFlag = false;
		joinBtn.Value = joinFlag = false;
		banishBtn.Value = banishFlag = false;
		depositBtn.Value = depositFlag = true;
		withdrawBtn.Value = withdrawFlag = false;
		authorityBtn.Value = authorityFlag = false;
	}
	
	private void OnAssignBtn()
	{
		Debug.Log( "OnAssignBtn");
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		eGUILDPERMISSION permission = eGUILDPERMISSION.eGUILDPERMISSION_NOTHING;
		
		if( true == noticeBtn.Value)
			permission |= eGUILDPERMISSION.eGUILDPERMISSION_NOTICE;
		if( true == publicityBtn.Value)
			permission |= eGUILDPERMISSION.eGUILDPERMISSION_PUBLICIZE;
		if( true == joinBtn.Value)
			permission |= eGUILDPERMISSION.eGUILDPERMISSION_INVITE;
		if( true == banishBtn.Value)
			permission |= eGUILDPERMISSION.eGUILDPERMISSION_EXILE;
		if( true == depositBtn.Value)
			permission |= eGUILDPERMISSION.eGUILDPERMISSION_DEPOSIT;
		if( true == withdrawBtn.Value)
			permission |= eGUILDPERMISSION.eGUILDPERMISSION_WITHDRAW;
		if( true == authorityBtn.Value)
			permission |= eGUILDPERMISSION.eGUILDPERMISSION_AUTHORIZE;
		
		Debug.Log( "permission : " + permission);
		
		body_CS_GUILD_CHANGE_PERMISSION changePermission = new body_CS_GUILD_CHANGE_PERMISSION( data.nCharUniqKey, permission);
		byte[] packet = changePermission.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( packet);
		
		parent.BroadcastMessage( "ClosePopup", SendMessageOptions.DontRequireReceiver);
		parent.BroadcastMessage( "ModifyUserPermission", permission, SendMessageOptions.DontRequireReceiver);	// #11380
	}
}
