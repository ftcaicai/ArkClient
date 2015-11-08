using UnityEngine;
using System.Collections;

public class AsGuildNewPopup_GuildList : MonoBehaviour 
{
	[SerializeField]SpriteText	m_txtMaster = null;
	[SerializeField]UIButton 		m_btnClose = null;
	[SerializeField]UIButton 		m_btnJoin = null;
	[SerializeField]UIButton 		m_btnWhisper = null;

	private GameObject 			m_parent = null;

	body2_SC_GUILD_SEARCH_RESULT	m_data = null;

	// Use this for initialization
	void Start () 
	{
		m_btnJoin.Text = AsTableManager.Instance.GetTbl_String(1268);
		m_btnWhisper.Text = AsTableManager.Instance.GetTbl_String(1115);

		m_btnClose.SetInputDelegate( OnCloseBtn);
		m_btnJoin.SetInputDelegate( OnJoinBtn);
		m_btnWhisper.SetInputDelegate( OnWhisperBtn);
	}

	public void Open(  GameObject parent , body2_SC_GUILD_SEARCH_RESULT data )
	{
		m_parent = parent;
		m_data = data;

		m_txtMaster.Text = m_data.szMasterName;

		if( null != AsUserInfo.Instance.GuildData && null != m_btnJoin )
		{
			m_btnJoin.SetControlState( UIButton.CONTROL_STATE.DISABLED );
			m_btnJoin.spriteText.Color = Color.gray;
		}
	}

	void OnCloseBtn( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			Destroy(gameObject);
//			m_parent.BroadcastMessage( "ClosePopupGuildList", SendMessageOptions.DontRequireReceiver);
		}
	}

	void OnJoinBtn( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			
			body_CS_GUILD_SEARCH_JOIN join = new body_CS_GUILD_SEARCH_JOIN( m_data.nGuildIdx);
			byte[] packet = join.ClassToPacketBytes();
			AsNetworkMessageHandler.Instance.Send( packet);

			Destroy(gameObject);
//			m_parent.BroadcastMessage( "ClosePopupGuildList", SendMessageOptions.DontRequireReceiver);
		}
	}

	void OnWhisperBtn( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			
			//		string msg = string.Format( "/w {0} ", data.szMasterName);
			//		AsChatManager.Instance.SetFocusInputField( msg);
			
			AsChatFullPanel.Instance.OpenWhisperMode( m_data.szMasterName);

			Destroy(gameObject);
//			m_parent.BroadcastMessage( "ClosePopupGuildList", SendMessageOptions.DontRequireReceiver);
		}
	}

	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
