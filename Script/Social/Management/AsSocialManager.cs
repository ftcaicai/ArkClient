#define _SOCIAL_LOG_
//#define _SOCIAL_ERROR_LOG_

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using WmWemeSDK.JSON;
using System.Text;

// trick when using both the iOS and Android version of the plugin in the same project. Add this block to the
// top of the file you are calling the Facebook methods from so they can share code. Note that it will only work
// when calling methods that are common to both platforms!
/*
#if UNITY_ANDROID
using FacebookAccess = FacebookAndroid;
#elif UNITY_IPHONE
using FacebookAccess = FacebookBinding;
#endif
*/



//Social Data
public class AsSocialManager : MonoBehaviour
{
	GameObject m_RootObject;
	AsSocialData m_SocialData;
	AsSocialUI m_SocialUI;

	private static AsSocialManager ms_kIstance = null;
	public static AsSocialManager Instance
	{
		get { return ms_kIstance; }
	}

	public AsSocialData SocialData
	{
		get { return m_SocialData; }
	}

	public AsSocialUI SocialUI
	{
		get { return m_SocialUI; }
	}

	public string ServerName;

	body_SC_FRIEND_INVITE m_invite;
	private AsMessageBox m_MsgBox_FriendInvite = null;
	private AsMessageBox m_MsgBox_GameInviteKakao = null;
	private AsMessageBox m_MsgBox_GameInviteSMS = null;
	private string m_SmsID = null;
	
	private string strFacebook_id = "";
	private bool m_bFaceBookLogin = false;
	private bool   m_bPostMessage = false;
	private int    m_nFbSubTitleIdx;
	private UInt32 m_nFbCharUniqKey;
	private string m_FbUserId = string.Empty;

	public void InitUI()
	{
		m_MsgBox_FriendInvite = null;
		m_MsgBox_GameInviteKakao = null;
		m_MsgBox_GameInviteSMS  = null;
	}

	void Awake()
	{
		if( ms_kIstance == null)
			ms_kIstance = this;

		m_SocialData = new AsSocialData();
		m_SocialUI = gameObject.AddComponent<AsSocialUI>();
	}

	// Use this for initialization
	void Start()
	{

	}

	public void Clear()
	{
		if( null != m_MsgBox_FriendInvite)
			DestroyImmediate( m_MsgBox_FriendInvite.gameObject);
		if( null != m_MsgBox_GameInviteKakao)
			DestroyImmediate( m_MsgBox_GameInviteKakao.gameObject);
		if( null != m_MsgBox_GameInviteSMS)
			DestroyImmediate( m_MsgBox_GameInviteSMS.gameObject);
	}

	public void Initilize()
	{
		m_SocialData.ClearList();
	}

	public void LogOut()
	{

	}

	#region -- Facebook
	public bool IsLoginFacebook()
	{
		return false;
	}

	public void LoginFacebook()
	{
		m_bFaceBookLogin = true;
	}

	public void reauthorizeWithPublishPermissions()
	{

	}

	public bool  getSessionPermissions()
	{
		bool re = false;

		return re;
	}

	public void PostMessageFacebook( string message, UInt32 nFbCharUniqKey, int nFbSubTitleIdx)
	{
		m_nFbCharUniqKey = nFbCharUniqKey;
		m_nFbSubTitleIdx = nFbSubTitleIdx;
		m_bPostMessage = true;
	}

	public void GetFaceBookFriends()
	{

	}

	public void GetSMSFriends()
	{

	}

	private void KakaoMsg()
	{

	}
	
	
	private void LobiMsg()
	{

		AsCommonSender.SendGameInvite( false, (int)eGAME_INVITE_PLATFORM.eGAME_INVITE_PLATFORM_LOBI, "12345");
		AsCommonSender.SendGameInviteList(); //#19733	

	}
	
	private void LineMsg()
	{

		AsCommonSender.SendGameInvite( false, (int)eGAME_INVITE_PLATFORM.eGAME_INVITE_PLATFORM_LINE, "12345");
		AsCommonSender.SendGameInviteList(); //#19733	


//#if (!UNITY_EDITOR && !UNITY_STANDALONE_OSX && !UNITY_STANDALONE_WIN)
		AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
		string charName = userEntity.GetProperty<string>( eComponentProperty.NAME);
	//	AsLoadingIndigator.Instance.ShowIndigator( "");
		
	//	string url = "http://line.me/R/msg/text/?{0}";#26939
		string msg =  string.Format( AsTableManager.Instance.GetTbl_String( 1578 ),ServerName,charName);

		Application.OpenURL(System.Uri.EscapeUriString(msg)); //#27312
	
//#endif
	}
	
	public void GameInviteResult( body_SC_GAME_INVITE_RESULT gameInviteResult)
	{
		switch ( (eGAME_INVITE_PLATFORM)gameInviteResult.ePlatform)
		{
		case eGAME_INVITE_PLATFORM.eGAME_INVITE_PLATFORM_FACEBOOK:
			GameFacebookInviteMsg( AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( gameInviteResult.szKeyword)));
			break;
		case eGAME_INVITE_PLATFORM.eGAME_INVITE_PLATFORM_KAKAOTALK:
			GameInviteKakao();  //#19799.
			break;
		case eGAME_INVITE_PLATFORM.eGAME_INVITE_PLATFORM_SMS:
			GameInviteSMS( AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( gameInviteResult.szKeyword)));    //#19799.
			break;
		case eGAME_INVITE_PLATFORM.eGAME_INVITE_PLATFORM_LOBI:
			LobiMsg();
			break;
		case eGAME_INVITE_PLATFORM.eGAME_INVITE_PLATFORM_LINE:
			LineMsg();
			break;	
		case eGAME_INVITE_PLATFORM.eGAME_INVITE_PLATFORM_TWITTER:
			GameInviteTwitter( AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( gameInviteResult.szKeyword))); 
			break;		
		}
	}

	public void GamemSmsInviteMsg( string strID)
	{

	}
    public void GameFacebookInviteMsg(string strID)
    {
        //
        UInt64 convert_id = 0;
        if (UInt64.TryParse(strID, out convert_id) != true)
            return;
        AsFaceBookFriend facebookfriend = m_SocialData.GetFacebookFriend(convert_id);
        strFacebook_id = facebookfriend.str_id;

        AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
        string charName = userEntity.GetProperty<string>(eComponentProperty.NAME);  

        var parameters = new Dictionary<string, string>
		{
			{ "to", strID },
			{ "message",  string.Format(AsTableManager.Instance.GetTbl_String(1732), ServerName, charName) }
		};


    }
		
		

/*	public void GameFacebookInviteMsg( string strID)
	{
		UInt64 convert_id = 0;
		if( UInt64.TryParse( strID, out convert_id) != true)
			return;
		AsFaceBookFriend facebookfriend = m_SocialData.GetFacebookFriend( convert_id);
		strFacebook_id = facebookfriend.str_id;
		AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
		string charName = userEntity.GetProperty<string>( eComponentProperty.NAME);
		var parameters = new Dictionary<string,string>
			{
				{ "target_id", strID },
				{ "link", AsTableManager.Instance.GetTbl_String( 1733) },
				{ "name", AsTableManager.Instance.GetTbl_String( 1655) },
				{ "message", string.Format( AsTableManager.Instance.GetTbl_String( 4062),ServerName, charName) },
				{ "picture", "http://wemade-image.gscdn.com/social/2012_v2/web/game/icon_game_19.png" },
				{ "caption", string.Format( AsTableManager.Instance.GetTbl_String( 4062),ServerName, charName) }
			};

#if UNITY_ANDROID
		FacebookAndroid.showDialog( "stream.publish", parameters);
#endif

#if UNITY_IPHONE
		FacebookBinding.showDialog( "stream.publish", parameters);
#endif
	}
*/
	#endregion

	#region -- Twitter
	public bool IsLoginTwitter()
	{
		return false;
	}

	public void LoginTwitter()
	{
		//	AsCommonSender.SendBackgroundProc( false);
	}

	public void PostMessageTwitter( string message)
	{

	}
	#endregion

	#region -- Socail UI
	public void CloseAllSocailUI()
	{
		SocialUI.CloseSocailDlg();
		SocialUI.CloseSocialStoreDlg();
		SocialUI.CloseFindFriendDlg();
		SocialUI.CloseSocailCloneDlg();
		SocialUI.CloseBlockDlg();
	}

	/// <summary>
	/// Opens the socail dlg.
	/// </summary>
	public void OpenSocailDlg()
	{
		if( null != m_SocialUI)
		{
			if( false == m_SocialUI.IsOpenSocailDlg)
			{
				if( null == m_SocialUI.m_SocialDlg)
					LoadSocialDlg();
			}
			else
			{
				m_SocialUI.CloseSocailDlg();
			}
		}
		else
		{
			Debug.LogError( "AsSocialManager::OpenSocailDlg() m_SocialUI is Null!!!");
		}
	}

	public bool IsOpenSocialDlg()
	{
		if( null == m_SocialUI)
		{
			Debug.LogWarning( "close1");
			return false;
		}

		Debug.LogWarning( "close = " + m_SocialUI.IsOpenSocailDlg);

		return m_SocialUI.IsOpenSocailDlg;
	}

	public void OpenSocailCloneDlg()
	{
		AsHudDlgMgr.Instance.CloseRecommendInfoDlg();  //#17479.
		AsHudDlgMgr.Instance.ClosePlayerStatus();     //#19720.
		//#13335
		if( null != m_SocialUI.m_FindFriendDlg)
			m_SocialUI.m_FindFriendDlg.Close();

		if( null != m_SocialUI.m_SocialStoreDlg)
			m_SocialUI.CloseSocialStoreDlg();

		if( null != m_SocialUI)
		{
			if( false == m_SocialUI.IsOpenSocailCloneDlg)
			{
				if( null == m_SocialUI.m_SocialCloneDlg)
					LoadSocialCloneDlg();
			}
			else
			{
				m_SocialUI.m_SocialCloneDlg.Open();
			}
		}
		else
		{
			Debug.LogError( "AsSocialManager::OpenSocailCloneDlg() m_SocialUI is Null!!!");
		}
	}

	public void OpenFindFriendDlg()
	{
		//#13335
		if( null != m_SocialUI.m_SocialCloneDlg)
			m_SocialUI.m_SocialCloneDlg.Close();

		if( null != m_SocialUI.m_SocialStoreDlg)
			m_SocialUI.CloseSocialStoreDlg();

		if( null != m_SocialUI)
		{
			if( false == m_SocialUI.IsOpenFindFriendDlg)
			{
				if( null == m_SocialUI.m_FindFriendDlg)
					LoadFindFriendDlg();
				else
					m_SocialUI.OpenFindFriendDlg();
			}
			else
			{
				m_SocialUI.CloseFindFriendDlg();
			}
		}
	}

	public void OpenBlockDlg( bool isFriendInvite)
	{
		if( null != m_SocialUI)
		{
			if( false == m_SocialUI.IsOpenBlockDlg)
			{
				if( null == m_SocialUI.m_BlockDlg)
					LoadBlockDlg( isFriendInvite);
				else
					m_SocialUI.OpenBlockDlg( isFriendInvite);
			}
			else
			{
				m_SocialUI.CloseBlockDlg();
			}
		}
	}

	public void OpenSocialStoreDlg()
	{
		AsHudDlgMgr.Instance.CloseRecommendInfoDlg();	// #17479

		//#13335
		if( null != m_SocialUI.m_FindFriendDlg)
			m_SocialUI.m_FindFriendDlg.Close();

		if( null != m_SocialUI.m_SocialCloneDlg)
			m_SocialUI.m_SocialCloneDlg.Close();

		if( null != m_SocialUI)
		{
			if( false == m_SocialUI.IsOpenSocailStoreDlg)
			{
				if( null == m_SocialUI.m_SocialStoreDlg)
					LoadSocialStoreDlg();
				else
					m_SocialUI.OpenSocialStoreDlg();
			}
			else
			{
				m_SocialUI.CloseSocialStoreDlg();
			}
		}
	}

	public void LoadSocialDlg()
	{
		if( null == m_SocialUI.m_SocialDlg)
		{
			GameObject obj = ResourceLoad.LoadGameObject( "UI/AsGUI/Social_new/GUI_Social");

			m_SocialUI.m_ObjectSocialDlg = GameObject.Instantiate( obj) as GameObject;
			m_SocialUI.m_SocialDlg = m_SocialUI.m_ObjectSocialDlg.GetComponentInChildren<AsSocialDlg>();
			m_SocialUI.OpenSocialDlg();
		}
	}

	public void LoadSocialCloneDlg()
	{
		if( null == m_SocialUI.m_SocialCloneDlg)
		{
			GameObject obj = ResourceLoad.LoadGameObject( "UI/AsGUI/Social_new/GUI_Social_Clone");

			m_SocialUI.m_ObjectSocialCloneDlg = GameObject.Instantiate( obj) as GameObject;
			m_SocialUI.m_SocialCloneDlg = m_SocialUI.m_ObjectSocialCloneDlg.GetComponentInChildren<AsSocialCloneDlg>();
			m_SocialUI.OpenSocialCloneDlg();
		}
	}

	public void LoadFindFriendDlg()
	{
		if( null == m_SocialUI.m_FindFriendDlg)
		{
			GameObject obj = ResourceLoad.LoadGameObject( "UI/AsGUI/Social_new/GUI_Social_Search");
	
			m_SocialUI.m_ObjectFindFriendDlg = GameObject.Instantiate( obj) as GameObject;
			m_SocialUI.m_FindFriendDlg = m_SocialUI.m_ObjectFindFriendDlg.GetComponentInChildren<AsFindFriendDlg>();
			m_SocialUI.OpenFindFriendDlg();
		}
	}

	public void LoadBlockDlg( bool isFriendInvite)
	{
		if( null == m_SocialUI.m_BlockDlg)
		{
			GameObject obj = ResourceLoad.LoadGameObject( "UI/AsGUI/Social_new/GUI_Social_Popup");
	
			m_SocialUI.m_ObjectBlockDlg = GameObject.Instantiate( obj) as GameObject;
			m_SocialUI.m_BlockDlg = m_SocialUI.m_ObjectBlockDlg.GetComponentInChildren<AsBlockDlg>();
			m_SocialUI.OpenBlockDlg( isFriendInvite);
		}
		else
		{
			m_SocialUI.OpenBlockDlg( isFriendInvite);//#19656
		}
	}

	public void LoadSocialStoreDlg()
	{
		if( null == m_SocialUI.m_SocialStoreDlg)
		{
			GameObject obj = ResourceLoad.LoadGameObject( "UI/AsGUI/Social_new/GUI_Social_Store");
	
			m_SocialUI.m_ObjectSocialStoreDlg = GameObject.Instantiate( obj) as GameObject;
			m_SocialUI.m_SocialStoreDlg = m_SocialUI.m_ObjectSocialStoreDlg.GetComponentInChildren<AsSocialStoreDlg>();
			m_SocialUI.OpenSocialStoreDlg();
		}
	}
	#endregion


	public void FriendInvite( body_SC_FRIEND_INVITE invite)
	{
		string userName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( invite.szCharName));
		if( m_MsgBox_FriendInvite == null)
		{
			string strMsg = string.Format( AsTableManager.Instance.GetTbl_String( 165), userName);
			m_MsgBox_FriendInvite = AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String( 1203), strMsg, this, "OnMsgBox_FriendInvite_Ok", "OnMsgBox_FriendInvite_Cancel", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);
			m_invite = invite;
		}
		else
		{
			AsCommonSender.SendFriendJoin( userName, invite.nUserUniqKey, eFRIEND_JOIN_TYPE.eFRIEND_JOIN_BUSY);
		}
	}

	public void OnMsgBox_FriendInvite_Ok()
	{
		string userName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( m_invite.szCharName));
		AsCommonSender.SendFriendJoin( userName, m_invite.nUserUniqKey, eFRIEND_JOIN_TYPE.eFRIEND_JOIN_ACCEPT);
	}

	public void OnMsgBox_FriendInvite_Cancel()
	{
		string userName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( m_invite.szCharName));
		AsCommonSender.SendFriendJoin( userName, m_invite.nUserUniqKey, eFRIEND_JOIN_TYPE.eFRIEND_JOIN_REFUSE);
	}

	public void GameInviteKakao()
	{
		if( m_MsgBox_GameInviteKakao == null)
			m_MsgBox_GameInviteKakao = AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String( 126), AsTableManager.Instance.GetTbl_String( 4060),
				this, "OnMsgBox_GameInviteKakao_Ok", "OnMsgBox_GameInviteKakao_Cancel", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);
	}

	public void OnMsgBox_GameInviteKakao_Ok()
	{
		KakaoMsg();
	}

	public void OnMsgBox_GameInviteKakao_Cancel()
	{
	}

	public void GameInviteSMS( string strID)
	{
		if( m_MsgBox_GameInviteSMS == null)
		{
			m_MsgBox_GameInviteSMS = AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String( 126), AsTableManager.Instance.GetTbl_String( 4068),
					this, "OnMsgBox_GameInviteSMS_Ok", "OnMsgBox_GameInviteSMS_Cancel", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);
			m_SmsID = strID;
		}
		else
		{
			DestroyImmediate( m_MsgBox_GameInviteSMS.gameObject);
			m_MsgBox_GameInviteSMS = AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String( 126), AsTableManager.Instance.GetTbl_String( 4068),
					this, "OnMsgBox_GameInviteSMS_Ok", "OnMsgBox_GameInviteSMS_Cancel", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);
			m_SmsID = strID;
		}
	}

	public void OnMsgBox_GameInviteSMS_Ok()
	{
		GamemSmsInviteMsg( m_SmsID);
	}

	public void OnMsgBox_GameInviteSMS_Cancel()
	{
	}

	public void InputDown( Ray inputRay)
	{
		if( m_SocialUI.m_SocialStoreDlg != null)
			m_SocialUI.m_SocialStoreDlg.InputDown( inputRay);
	}

	public void InputUp( Ray inputRay)
	{
		if( m_SocialUI.m_SocialStoreDlg != null)
			m_SocialUI.m_SocialStoreDlg.InputUp( inputRay);
	}
	
	
	public  void GameInviteTwitter(  string strID )
	{
		
		UInt64 convert_id = 0;
       if (UInt64.TryParse(strID, out convert_id) != true)
            return;
		 
		AsTwitterFollower twitterFollower = m_SocialData.GetTwitterFollower(convert_id);
		
		AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
        string charName = userEntity.GetProperty<string>(eComponentProperty.NAME);  

		StringBuilder sb = new StringBuilder("@");
		sb.Append(twitterFollower.screen_name);
		sb.Append(" ");
		sb.Append( string.Format(AsTableManager.Instance.GetTbl_String(1895), ServerName, charName));
		
		var param = new Dictionary<string, string>()
		{
			{ "status", sb.ToString() }
		};
	
		AsCommonSender.SendGameInvite( false, (int)eGAME_INVITE_PLATFORM.eGAME_INVITE_PLATFORM_TWITTER, strID);
		AsCommonSender.SendGameInviteList(); 
	
		AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), 
		                             AsTableManager.Instance.GetTbl_String(4208), null, null, AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE); //#27107
	}
	
	 void getTwitterFollowerList()
    {
        string str = ( ( TextAsset)Resources.Load( "JSONRequests/weme/TwitterList")).text;
        Debug.Log( str);
       

		JSONObject jsonObject = JSONObject.Parse( str);
		Debug.Log( "GetTwitterFollowersHandler" + jsonObject.ToString());
			JSONArray friendJson = jsonObject.GetArray( "users");
			if(friendJson == null)
				return;
		
			m_SocialData.TwitterFollowersLoad = true;
			Debug.Log( "users" + friendJson.ToString());
				      			
			IEnumerator<JSONValue> e = friendJson.GetEnumerator();
			while ( e.MoveNext())
			{
				JSONObject friendData = e.Current.Obj;

				Debug.Log( "friendData" + friendData.ToString());
				
				m_SocialData.TwitterFollowerInsert( friendData["name"].Str, friendData["id"].Number, friendData["screen_name"].Str, friendData["profile_image_url_https"].Str);
			}
			m_SocialUI.SetTwitterFollowerList();
    }
	// Gets a list of the users followers
	public static void getFollowers()
	{
		var param = new Dictionary<string, string>()
		{ //{ "cursor", "-1" },
			{ "screen_name", "twitterapi" },
		//	{ "skip_status", "true" },
		//	{ "include_user_entities", "false" },
		};
	}
	
	public void GetTwitterFollowers()
	{

	}
	
	
	void requestDidFailEvent( string error )
	{
		Debug.Log( "requestDidFailEvent: " + error );
	}
	
	
	void requestDidFinishEvent( object result )
	{
		if( result != null )
		{
			Debug.Log( "requestDidFinishEvent" );
			//Prime31.Utils.logObject( result );
		}
		
			
			//JSONObject jsonObject = JSONObject.Parse( Prime31.Json.encode( result));
		JSONObject jsonObject = JSONObject.Parse( result.ToString() );

			Debug.Log( "GetTwitterFollowersHandler" + jsonObject.ToString());
			JSONArray friendJson = jsonObject.GetArray( "users");
			if(friendJson == null)
				return;
		
			m_SocialData.TwitterFollowersLoad = true;
			Debug.Log( "users" + friendJson.ToString());
				      			
			IEnumerator<JSONValue> e = friendJson.GetEnumerator();
			while ( e.MoveNext())
			{
				JSONObject friendData = e.Current.Obj;

				Debug.Log( "friendData" + friendData.ToString());
				
				m_SocialData.TwitterFollowerInsert( friendData["name"].Str, friendData["id"].Number, friendData["screen_name"].Str, friendData["profile_image_url_https"].Str);
			}
			m_SocialUI.SetTwitterFollowerList();
	}
	
	
	

}
