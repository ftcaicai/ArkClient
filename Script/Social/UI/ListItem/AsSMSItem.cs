using UnityEngine;
using System.Collections;

public class AsSMSItem : MonoBehaviour
{
	public SpriteText desc = null;
	private AsSMSFriend m_friendData = null;

	private bool isLoaded = false;
	public bool  IsLoaded
	{
		get { return isLoaded; }
		set { isLoaded = value; }
	}
	public UIButton m_InviteMsgBtn;

	void OnDestory()
	{
	}

	// Use this for initialization
	void Start()
	{
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_InviteMsgBtn.spriteText);
		m_InviteMsgBtn.SetInputDelegate( m_InviteMsgBtnDelegate);
		m_InviteMsgBtn.Text = AsTableManager.Instance.GetTbl_String(1263);
	}

	// Update is called once per frame
	void Update()
	{
	}

	public void SetSMSFriendData( AsSMSFriend friend)
	{
		m_friendData = friend;
		desc.Text = friend.name;
	}

	private void m_InviteMsgBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if(m_friendData.phonenumber.Length > 0)
			{	
				AsSoundManager.Instance.PlaySound("Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
				Debug.Log("AsSMSItem InviteMsgBtnDelegate");
				AsCommonSender.SendGameInvite(true, (int)eGAME_INVITE_PLATFORM.eGAME_INVITE_PLATFORM_SMS, m_friendData.phonenumber.ToString());
			}
		}
	}
}
