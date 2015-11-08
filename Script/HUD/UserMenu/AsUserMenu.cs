using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsUserMenu : MonoBehaviour
{
	public enum eUserMenu
	{
		NONE = -1,
		Info   = 0,
		Whisper,
		Trade,
		Party,
		Friend,
		UserRebirth,
		Mail,

		MaxUserMenu
	}

	public UIButton[] m_MenuBtn;
	ushort m_UserSessionID = 0;
	uint   m_UserUniqueId = 0;
	AsUserEntity m_UserEntity;

	bool   m_bVisible = false;

	public bool isVisible	{ get { return m_bVisible; } }
	public AsUserEntity TargetUserEntity	{ get { return m_UserEntity; } }

//	private int m_nPcSelectEff = 0;
//	private string m_strPcSelectEffPath = "FX/Effect/Decal/Character_select_Pc_Dummy";

	void Awake()
	{
		m_MenuBtn[(int)eUserMenu.Info].spriteText.Text = AsTableManager.Instance.GetTbl_String(1113);
		m_MenuBtn[(int)eUserMenu.Whisper].spriteText.Text = AsTableManager.Instance.GetTbl_String(1115);
		m_MenuBtn[(int)eUserMenu.Trade].spriteText.Text = AsTableManager.Instance.GetTbl_String(1116);
		m_MenuBtn[(int)eUserMenu.Party].spriteText.Text = AsTableManager.Instance.GetTbl_String(1163);
		m_MenuBtn[(int)eUserMenu.Friend].spriteText.Text = AsTableManager.Instance.GetTbl_String(1164);
		m_MenuBtn[(int)eUserMenu.UserRebirth].spriteText.Text = AsTableManager.Instance.GetTbl_String(1369);
		m_MenuBtn[(int)eUserMenu.Mail].spriteText.Text = AsTableManager.Instance.GetTbl_String(824);

		m_MenuBtn[(int)eUserMenu.Info].SetInputDelegate( InfoBtnDelegate);
		m_MenuBtn[(int)eUserMenu.Whisper].SetInputDelegate( WhisperBtnDelegate);
		m_MenuBtn[(int)eUserMenu.Trade].SetInputDelegate( TradeBtnDelegate);
		m_MenuBtn[(int)eUserMenu.Party].SetInputDelegate( PartyBtnDelegate);
		m_MenuBtn[(int)eUserMenu.Friend].SetInputDelegate( FriendBtnDelegate);
		m_MenuBtn[(int)eUserMenu.UserRebirth].SetInputDelegate( UserRebirthBtnDelegate);
		m_MenuBtn[(int)eUserMenu.Mail].SetInputDelegate( UserMailBtnDelegate);
	}

	void Clear()
	{
	}

	public void Close()
	{
		Clear();

		gameObject.SetActiveRecursively( false);
		m_bVisible = false;
	}

	void SetVisible( GameObject obj, bool visible)
	{
		obj.SetActiveRecursively( visible);
		obj.active = visible;
		m_bVisible = visible;

		SetVisible_UserRebirth();
	}

	void SetVisible_UserRebirth()//$yde
	{
		if( m_UserEntity != null && m_UserEntity.GetProperty<bool>( eComponentProperty.LIVING) == false)
			m_MenuBtn[(int)eUserMenu.UserRebirth].Text = AsTableManager.Instance.GetTbl_String(1369);
		else
			m_MenuBtn[(int)eUserMenu.UserRebirth].Text = Color.gray.ToString() + AsTableManager.Instance.GetTbl_String(1369);
	}

	public bool Open( ushort userSessionID)
	{
		if( true == m_bVisible)
		{
			Close();
			return false;
		}

		m_UserSessionID = userSessionID;

		List<AsUserEntity> entity = AsEntityManager.Instance.GetUserEntityBySessionId( userSessionID);
		if( null == entity)
			return false;

		m_UserEntity = entity[0];

		SetVisible( gameObject, true);

		return true;
	}

	private void InfoBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsCommonSender.SendOtherUserInfo( (int)eOTHER_INFO_TYPE.eOTHER_INFO_TYPE_PLAYER, m_UserSessionID, m_UserEntity.UniqueId, 0);
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			Close();
		}
	}

	private void WhisperBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
//			string defaultText = "/w " + m_UserEntity.GetProperty<string>( eComponentProperty.NAME) + " ";
//			AsChatManager.Instance.SetDefaultText( defaultText);
			string name = m_UserEntity.GetProperty<string>( eComponentProperty.NAME);
			AsChatFullPanel.Instance.OpenWhisperMode( name);
			Close();
		}
	}

	private void PartyBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		/*	if( false == AsPartyManager.Instance.IsPartying)
			{
				AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
				if( null == userEntity)
					return;

				string strPartyName = userEntity.GetProperty<string>( eComponentProperty.NAME) + AsTableManager.Instance.GetTbl_String(1357);


				AsPartyManager.Instance.PartyCreate( strPartyName);
			}
		*/

			if( true == AsPvpManager.Instance.CheckMatching() || true == AsPvpManager.Instance.CheckInArena())
			{
				Close();
				return;
			}

			if( true == AsInstanceDungeonManager.Instance.CheckMatching() || true == AsInstanceDungeonManager.Instance.CheckInIndun())
			{
				Close();
				return;
			}

			AsPartySender.SendPartyInvite( m_UserEntity.SessionId, m_UserEntity.UniqueId);

			Close();
		}
	}

	private void KickBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsPartySender.SendPartyBannedExit( m_UserSessionID, m_UserUniqueId);
			Close();
		}
	}

	private void CaptainBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsPartySender.SendPartyCaptain( m_UserSessionID, m_UserUniqueId);
			Close();
		}
	}

	private void TradeBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			if( true == AsPvpManager.Instance.CheckMatching() || true == AsPvpManager.Instance.CheckInArena())
			{
				Close();
				return;
			}

			if( true == AsInstanceDungeonManager.Instance.CheckMatching() || true == AsInstanceDungeonManager.Instance.CheckInIndun())
			{
				Close();
				return;
			}
			
			AsTradeManager.Instance.SetTradeTargetPlayer( m_UserSessionID);
			Close();
		}
	}

	private void FriendBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			if( true == AsPvpManager.Instance.CheckInArena())
				return;

			if( true == AsInstanceDungeonManager.Instance.CheckInIndun())
				return;

			AsCommonSender.SendFriendInvite( m_UserEntity.GetProperty<string>( eComponentProperty.NAME));
//			QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_CALL_ADD_FRIEND, new AchFriendCall(1));
			Close();
		}
	}

	private void UserRebirthBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if(TargetDecider.CheckCurrentMapIsArena() == false)
			{
				AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
				
				if( m_UserEntity != null && m_UserEntity.GetProperty<bool>( eComponentProperty.LIVING) == false)
				{
					AsNotify.Instance.DeathDlgFriend( m_UserEntity);
				}
			}
			else
			{
				AsEventNotifyMgr.Instance.CenterNotify.AddTradeMessage( AsTableManager.Instance.GetTbl_String(903));
			}
		}
	}

	private void UserMailBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			if( true == AsPvpManager.Instance.CheckInArena())
				return;

			if( true == AsInstanceDungeonManager.Instance.CheckInIndun())
				return;

			string name = AsUtil.GetRealString( m_UserEntity.GetProperty<string>( eComponentProperty.NAME));
			AsHudDlgMgr.Instance.OpenWritePostBoxDlg( name);
		}
	}

	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
	}
}
