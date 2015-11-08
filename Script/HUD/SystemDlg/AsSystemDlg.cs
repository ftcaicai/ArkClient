
//#define _FACEBOOK_VER 
//#define USE_WEME_APP
#define NEW_DELEGATE_IMAGE
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class AsSystemDlg : MonoBehaviour
{
	[SerializeField] SpriteText m_TextTitle;
	[SerializeField] UIButton m_btnGameInfo;
	[SerializeField] UIButton m_btnHomePage;
	[SerializeField] UIButton m_btnWeme;
	[SerializeField] UIScrollList list = null;
	[SerializeField] GameObject listItem = null;
	
	// Use this for initialization
	void Start()
	{
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextTitle);

		m_TextTitle.Text = AsTableManager.Instance.GetTbl_String(1376);

		Init();

		QuestTutorialMgr.Instance.ProcessQuestTutorialMsg(new QuestTutorialMsgInfo(QuestTutorialMsg.OPEN_SYSTEM));

		m_btnHomePage.SetInputDelegate( OnHomePageBtn );
		m_btnHomePage.Text = AsTableManager.Instance.GetTbl_String(443);

		m_btnGameInfo.SetInputDelegate( OnGameInfo );
		m_btnGameInfo.Text = AsTableManager.Instance.GetTbl_String(445);

		m_btnWeme.SetInputDelegate( OnWemeBtn );
		m_btnWeme.Text = AsTableManager.Instance.GetTbl_String(444);

		if (AsReviewConditionManager.Instance.IsReviewCondition (eREVIEW_CONDITION.PLATFORM_VIEW) == false) 
		{
			m_btnHomePage.gameObject.SetActive(false);
			m_btnGameInfo.gameObject.SetActive(false);
			m_btnWeme.gameObject.SetActive(false);
		}
	}

	private void Init()
	{
		// Option
		UIListItemContainer con = list.CreateItem( listItem) as UIListItemContainer;
		con.Text = AsTableManager.Instance.GetTbl_String(810);
		SystemListItem item = con.gameObject.GetComponent<SystemListItem>();
		Debug.Assert( null != item);
		item.SelectCallback = OnOpenGameOptionDlgBtn;

		// Channel select
		con = list.CreateItem( listItem) as UIListItemContainer;
		con.Text = AsTableManager.Instance.GetTbl_String(808);
		item = con.gameObject.GetComponent<SystemListItem>();
		Debug.Assert( null != item);
		item.SelectCallback = OnOpenChannelDlgBtn;

		// Character select
		con = list.CreateItem( listItem) as UIListItemContainer;
		con.Text = AsTableManager.Instance.GetTbl_String(1377);
		item = con.gameObject.GetComponent<SystemListItem>();
		Debug.Assert( null != item);
		item.SelectCallback = OnCharacterSelectBtn;

		// Guide
		con = list.CreateItem( listItem) as UIListItemContainer;
		con.gameObject.name = "Guide";
		con.Text = AsTableManager.Instance.GetTbl_String(1992);
		item = con.gameObject.GetComponent<SystemListItem>();
		Debug.Assert( null != item);
		item.SelectCallback = OnGuideBtn;
		
#if false
		// Notice
		con = list.CreateItem( listItem) as UIListItemContainer;
		con.Text = AsTableManager.Instance.GetTbl_String(1245);
		item = con.gameObject.GetComponent<SystemListItem>();
		Debug.Assert( null != item);
		item.SelectCallback = OnNoticeBtn;
#endif

		// Policy
		con = list.CreateItem( listItem) as UIListItemContainer;
		con.Text = AsTableManager.Instance.GetTbl_String(1746);
		item = con.gameObject.GetComponent<SystemListItem>();
		Debug.Assert( null != item);
		item.SelectCallback = OnPolicyBtn;

#if !NEW_DELEGATE_IMAGE
		// Gender
		con = list.CreateItem( listItem) as UIListItemContainer;
		con.Text = AsTableManager.Instance.GetTbl_String(4202);
		item = con.gameObject.GetComponent<SystemListItem>();
		Debug.Assert( null != item);
		item.SelectCallback = OnGenderBtn;
#endif

		// Logout
		con = list.CreateItem( listItem) as UIListItemContainer;
		con.Text = AsTableManager.Instance.GetTbl_String(809);
		item = con.gameObject.GetComponent<SystemListItem>();
		Debug.Assert( null != item);
		item.SelectCallback = OnLogoutBtn;
	}

	// Update is called once per frame
	void Update()
	{
	}

	void OnEnable()
	{		
//		m_FaceBookBtn.gameObject.SetActiveRecursively( false);
//		m_TwitterBtn.gameObject.SetActiveRecursively( false);
	}

	private void OnOpenChannelDlgBtn()
	{
		if (AsLogoutManager.Instance.ActiveLogoutCoroutine == true)
			return;

		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		//2014.05.16 dopamin
		//if( true == AsUtil.CheckGuestUser())
		//	return;

		AsHudDlgMgr.Instance.CloseSystemDlg();
		
		AsHudDlgMgr.Instance.OpenChannelSelectDlg();
	}

	private void OnOpenGameOptionDlgBtn()
	{
		if (AsLogoutManager.Instance.ActiveLogoutCoroutine == true)
			return;

		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		AsHudDlgMgr.Instance.CloseSystemDlg();
		
		if( false == AsHudDlgMgr.Instance.IsOpenOptionDlg)
			AsHudDlgMgr.Instance.OpenOption();
	}
	
	private void OnCharacterSelectBtn()
	{
		if (AsLogoutManager.Instance.ActiveLogoutCoroutine == true)
			return;

		//2014.05.16
		if( true == WemeSdkManager.Instance.IsServiceGuest)
		{
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(1923), AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			return;
		}
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		if( true == AsInstanceDungeonManager.Instance.CheckInIndun() || true == AsPvpManager.Instance.CheckInArena())
			return;

		AsHudDlgMgr.Instance.CloseSystemDlg();
		
		//$yde
		AsHudDlgMgr.Instance.CollapseMenuBtn();	// #10694
		AsUserInfo.Instance.ApplyInGameDataOnSelectInfo();
		AutoCombatManager.Instance.ExitInGame();//$yde
//		AsPartyManager.Instance.PartyDiceRemoveAll();//#11954
//		AsPartyManager.Instance.PartyUserRemoveAll();
//		ArkQuestmanager.instance.ResetQuestManager();
		AsSocialManager.Instance.Clear();//- 친구 요청에 대한 수락에 캐릭터 이름 누락.
		
		if( AsUserInfo.Instance.IsBattle() )			
		{
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(1633),
											null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			
			AsSoundManager.Instance.PlaySound_VoiceBattle( eVoiceBattle.str1633_Cannot_Use_In_Combat );
			
			return;
		}		
		
		AS_CG_RETURN_CHARSELECT returnCharSelect = new AS_CG_RETURN_CHARSELECT();
		byte[] data = returnCharSelect.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}

	private void OnGuideBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		GameObject go = ResourceLoad.CreateGameObject( "UI/AsGUI/GUI_Guide");
		Debug.Assert( null != go);

		AsHudDlgMgr.Instance.CloseSystemDlg();
		
		AsGameGuideManager.Instance.SetGuideList( go);

		if (ArkQuestmanager.instance.CheckHaveOpenUIType(OpenUIType.OPEN_GUIDE) != null)
			AsCommonSender.SendClearOpneUI(OpenUIType.OPEN_GUIDE);
	}

	private void OnLogoutBtn()
	{
		if( AsLogoutManager.Instance.ActiveLogoutCoroutine == true )
			return;

		AsLogoutManager.Instance.ProcessLogout ();
	}

	private void OnWemeBtn(ref POINTER_INFO ptr)
	{
		if (ptr.evt != POINTER_INFO.INPUT_EVENT.TAP)
			return;
		
		if( AsLogoutManager.Instance.ActiveLogoutCoroutine == true )
			return;

		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		#if UNITY_IPHONE || UNITY_ANDROID
		WemeSdkManager.GetMainGameObject.showPlatformViewMain();	
		#endif
	}
	
	private void OnGameInfo(ref POINTER_INFO ptr)
	{
		if (ptr.evt != POINTER_INFO.INPUT_EVENT.TAP)
			return;
		
		string url =  AsTableManager.Instance.GetTbl_String(446);
		#if ( UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN)
		Application.OpenURL(url);
		#else	
		WemeSdkManager.GetMainGameObject.showPlatformViewWeb(url);
		#endif
		
/*		
		string url = string.Empty;

		#if ( UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN)
		if( ( "_Dev" == AsNetworkDefine.ENVIRONMENT) || ( "_Alpha" == AsNetworkDefine.ENVIRONMENT))
		{
			url = AsTableManager.Instance.GetTbl_String(425);
		}
		else if( "_Beta" == AsNetworkDefine.ENVIRONMENT)
		{
			url = AsTableManager.Instance.GetTbl_String(427);
		}
		else if( "" == AsNetworkDefine.ENVIRONMENT)
		{
			url = AsTableManager.Instance.GetTbl_String(429);
		}

		Application.OpenURL( url);
		#else		
		if( ( "_Dev" == AsNetworkDefine.ENVIRONMENT) || ( "_Alpha" == AsNetworkDefine.ENVIRONMENT))
		{
			url = AsTableManager.Instance.GetTbl_String(426);
		}
		else if( "_Beta" == AsNetworkDefine.ENVIRONMENT)
		{
			url = AsTableManager.Instance.GetTbl_String(428);
		}
		else if( "" == AsNetworkDefine.ENVIRONMENT)
		{
			url = AsTableManager.Instance.GetTbl_String(430);
		}

		WemeSdkManager.GetMainGameObject.showPlatformViewWeb( url);
		#endif
*/
		if( ArkQuestmanager.instance.CheckHaveOpenUIType( OpenUIType.OPEN_INVEN_WEB) != null)
			AsCommonSender.SendClearOpneUI( OpenUIType.OPEN_INVEN_WEB);
	}
	
	private void OnHomePageBtn(ref POINTER_INFO ptr)
	{
		if (ptr.evt != POINTER_INFO.INPUT_EVENT.TAP)
			return;
		
		//419 : 개발팀 블로그 주소.
		string url =  AsTableManager.Instance.GetTbl_String(439);
		#if ( UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN)
		Application.OpenURL(url);
		#else	
		WemeSdkManager.GetMainGameObject.showPlatformViewWeb(url);
		#endif
	}
	
	private void OnCloseBtn()
	{
		if( AsLogoutManager.Instance.ActiveLogoutCoroutine == true )
			return;

		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		AsHudDlgMgr.Instance.CloseSystemDlg();
	}
	
	void DownloadWeme()
	{
		//KakaoManager.Instance.DownloadWemeApp();
	}
	
	private void OnNoticeBtn()
	{
		if( AsLogoutManager.Instance.ActiveLogoutCoroutine == true )
			return;

//		GameObject noticesDlg = GameObject.Instantiate( ResourceLoad.LoadGameObject( "UI/AsGUI/GUI_IntroEvent")) as GameObject;
//		AsNoticesDlg dlg = noticesDlg.GetComponentInChildren<AsNoticesDlg>();
//		Debug.Assert( null != dlg);
		GameObject noticesDlg = GameObject.Instantiate( ResourceLoad.LoadGameObject( "UI/AsGUI/GUI_IntroEvent")) as GameObject;
		AsHudDlgMgr.Instance.NoticeDlg = noticesDlg.GetComponentInChildren<AsNoticesDlg>();
		Debug.Assert( null != AsHudDlgMgr.Instance.NoticeDlg);
		
		AsHudDlgMgr.Instance.NoticeDlg.ReOpen();
	}

	private void OnPolicyBtn()
	{
		string url =  AsTableManager.Instance.GetTbl_String(447);
		#if ( UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN)
		Application.OpenURL(url);
		#else	
		WemeSdkManager.GetMainGameObject.showPlatformViewWeb(url);
		#endif

/*
#if ( UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN)
		Application.OpenURL( "http://m.weme.wemade.com/policy/policy.asp?tab=1");
#else
		WemeSdkManager.GetMainGameObject.showTermsView();	//dopamin
		
		//AndroidJavaClass webView = new AndroidJavaClass( "com.wemadecreative.arksphere.WebViewManager");
		//webView.CallStatic( "showWebPop", "http://m.weme.wemade.com/policy/policy.asp?tab=1");
#endif
*/
	}

	private void OnGenderBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		if( true == AsInstanceDungeonManager.Instance.CheckInIndun() || true == AsPvpManager.Instance.CheckInArena())
			return;
		
		string ticks = PlayerPrefs.GetString( "LatestGenderSetTime");
		if( false == string.IsNullOrEmpty( ticks))
		{
			System.DateTime savedTime = new System.DateTime( long.Parse( ticks));
			System.TimeSpan elapsedSpan = new System.TimeSpan( System.DateTime.Now.Ticks - savedTime.Ticks);
			if( 60.0f > elapsedSpan.TotalSeconds)
			{
				AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(1660), AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE));
				return;
			}
		}

//		GameObject genderMsgBox = ResourceLoad.CreateGameObject( "UI/Optimization/Prefab/MessageBox_Gender");
//		Debug.Assert( null != genderMsgBox);
		GameObject genderMsgBox = ResourceLoad.CreateGameObject( "UI/Optimization/Prefab/MessageBox_Gender");
		AsHudDlgMgr.Instance.GenderDlg = genderMsgBox.GetComponentInChildren<AsMessageBoxGender>();
		Debug.Assert( null != AsHudDlgMgr.Instance.GenderDlg);
	}
}
