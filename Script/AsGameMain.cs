#define NEW_DELEGATE_IMAGE
using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System.Xml;


public enum GAME_STATE
{
	STATE_INVALID = -1,
	STATE_INTRO,
	STATE_LOGIN,
	STATE_CHARACTER_SELECT,
	STATE_CHARACTER_CREATE,
	STATE_INGAME,
	STATE_LOADING,
	STATE_LOAD_END,
	STATE_WARP_SEND,

	STATE_MAX
};

// < ilmeda, 20120824
public enum GAME_LANGUAGE
{
//	LANGUAGE_AUTO = 0,
	LANGUAGE_KOR = 0,
	LANGUAGE_JAP
//	LANGUAGE_CHN
};
// ilmeda, 20120824 >

public enum OptionBtnType
{
	OptionBtnType_Push = 0,
	OptionBtnType_EffectShow,
	OptionBtnType_ResourceShow,
	OptionBtnType_Chat,
	OptionBtnType_SoundBG,
	OptionBtnType_SoundEff,
	OptionBtnType_AutoChat,
	OptionBtnType_Vibrate,
	OptionBtnType_SubTitleName,
	OptionBtnType_MonsterName,
	OptionBtnType_NpcName,
	OptionBtnType_FriendOnlineAlarm,
	OptionBtnType_FriendInviteRefuse,
	OptionBtnType_PartyInviteRefuse,
	OptionBtnType_GuildInviteRefuse,
	OptionBtnType_SkillCoolAlram,
	OptionBtnType_QuestPartyMatching,
#if !NEW_DELEGATE_IMAGE
	OptionBtnType_RankMark,
	OptionBtnType_PvpGrade,
#endif
	OptionBtnType_Filtering,
	OptionBtnType_VoiceBattle,
	OptionBtnType_TargetChange,

//	OptionBtnType_CharacterQuality,
//	OptionBtnType_EffectQuality,
//	OptionBtnType_HelmView,
//	OptionBtnType_PushNotification,
	OptionBtnType_Max
};

public enum PersonalInfoType
{
	PersonalInfoType_Agreement = 0,
	PersonalInfoType_MOVIE_PLAYED = 1,

	PersonalInfoType_Max
};

public class AsGameMain : MonoBehaviour
{
	static public GAME_STATE s_gameState = GAME_STATE.STATE_INVALID;
	static public int createCharacterSlot = 1;
	static public bool isPopupExist = false;
	static public bool[] s_optionState = new bool[(int)(OptionBtnType.OptionBtnType_Max)];
	static public byte[] s_personalInfo = new byte[(int)(PersonalInfoType.PersonalInfoType_Max)];
	public GAME_LANGUAGE gameLanguage = GAME_LANGUAGE.LANGUAGE_KOR; // ilmeda, 20120824
	public bool useAssetbundle = false; // ilmeda, 20120829
	static public bool useCashShop = true;
	static public long serverTickGap = 0;
	public FPSCounter fps = null;
	private bool showFPS = false;
	[SerializeField]int firstResetAlarmHour = 0;
	[SerializeField]int firstResetAlarmMinute = 0;
	[SerializeField]int firstResetAlarmSecond = 0;
	[SerializeField]int secondResetAlarmHour = 0;
	[SerializeField]int secondResetAlarmMinute = 0;
	[SerializeField]int secondResetAlarmSecond = 0;
	[SerializeField]int thirdResetAlarmHour = 0;
	[SerializeField]int thirdResetAlarmMinute = 0;
	[SerializeField]int thirdResetAlarmSecond = 0;
	//public bool PvpMode = false;
	public bool UseReadBinary = false;

	void Awake()
	{
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		DontDestroyOnLoad( gameObject);
		DDOL_Tracer.RegisterDDOL( this, gameObject);//$ yde

		//FTInit ();

		_InitOptionState();
		_InitPersonalInfo();
		
		UIWorldMapSettingDlg.s_alphaValue = PlayerPrefs.GetInt("worldalpha", 0);
        UIWorldMapSettingDlg.s_bigMap = PlayerPrefs.GetInt("worldbig", 1 ) == 1 ? true : false;
	}

	void FTInit () {
		if (Application.platform == RuntimePlatform.Android) {
			AsNetworkDefine.PATCH_SERVER_ADDRESS = "jar:file://" + Application.dataPath + "!/assets/";	
		} else {
			AsNetworkDefine.PATCH_SERVER_ADDRESS = "file:///" + Application.streamingAssetsPath + "/";
		}
	}

	// Use this for initialization
//	void Start()
	IEnumerator Start()
	{
		Application.targetFrameRate = 60;
		DDOL_Tracer.BeginTrace();//$ yde

		AsShaderManager.Instance.Init();
		ResetAlarmResigration();
		HackCheck();
		yield return Application.LoadLevelAsync( "Intro");
	}

	void OnApplicationPause( bool pause)
	{
		#region - additional process -
		if( AsPStoreManager.Instance != null)
			AsPStoreManager.Instance.ApplicationPause_GameMain( pause);
		#endregion

		SetBackGroundProc( pause);

		if( null != AsEntityManager.Instance)
		{
			AsEntityManager.Instance.RemoveAllMonster();
			AsEntityManager.Instance.RemoveAllNpc();
			AsEntityManager.Instance.RemoveAllOtherUser();
		}

		if( null != ItemMgr.DropItemManagement)
			ItemMgr.DropItemManagement.Clear();
		
		if( null != AsPvpManager.Instance)
			AsPvpManager.Instance.SetBackGroundProc( pause);
		
		if( false == pause)
			HackCheck();
	}
	
	void HackCheck()
	{
		HackChecker hackChecker = gameObject.GetComponent<HackChecker>();
		Debug.Assert( null != hackChecker);
		hackChecker.Check();
	}

	private static bool m_isBackGroundPause = false;
	public static bool isBackGroundPause
	{
		get	{ return m_isBackGroundPause; }
	}

	public static void SetBackGroundProc( bool pause)
	{
		//Debug.Log( "OnApplicationPause : " + pause);

		if( true == pause && null != AsHudDlgMgr.Instance)
			AsHudDlgMgr.Instance.lobiRecBtn.gameObject.SetActiveRecursively(false);

		if( null != AsEntityManager.Instance && null != AsEntityManager.Instance.GetPlayerCharFsm())
		{
			AsPlayerFsm.ePlayerFsmStateType curType = AsEntityManager.Instance.GetPlayerCharFsm().CurrnetFsmStateType;

			if( AsPlayerFsm.ePlayerFsmStateType.RUN_COLLECT == curType ||
				AsPlayerFsm.ePlayerFsmStateType.READY_COLLECT == curType ||
				AsPlayerFsm.ePlayerFsmStateType.WORK_COLLECT == curType)
			{
				AsEntityManager.Instance.GetPlayerCharFsm().SetPlayerFsmState( AsPlayerFsm.ePlayerFsmStateType.IDLE);
			}
		}

		m_isBackGroundPause = pause;
		AsCommonSender.SendBackgroundProc( pause);
	}

#if UNITY_ANDROID
	AsMessageBox quitDlg = null;
	void Quit()
	{
		AsUtil.Quit();
	}

	void CancelQuit()
	{
		quitDlg = null;
	}	
#endif

	void ResetAlarmResigration()
	{
		System.DateTime curTime = System.DateTime.Now;
		float curTimeToSec = ( curTime.Hour * 3600) + ( curTime.Minute * 60) + curTime.Second;

		float firstAlarmSeconds = ( ( firstResetAlarmHour * 3600) + ( firstResetAlarmMinute * 60) + firstResetAlarmSecond) - curTimeToSec;
		if( 0.0f <= firstAlarmSeconds)
			Invoke( "FirstResetAlarm", firstAlarmSeconds);

		float secondAlarmSeconds = ( ( secondResetAlarmHour * 3600) + ( secondResetAlarmMinute * 60) + secondResetAlarmSecond) - curTimeToSec;
		if( 0.0f <= secondAlarmSeconds)
			Invoke( "SecondResetAlarm", secondAlarmSeconds);

		float thirdAlarmSeconds = ( ( thirdResetAlarmHour * 3600) + ( thirdResetAlarmMinute * 60) + thirdResetAlarmSecond) - curTimeToSec;
		if( 0.0f <= thirdAlarmSeconds)
			Invoke( "ThirdResetAlarm", thirdAlarmSeconds);
	}

	void FirstResetAlarm()
	{
		if( GAME_STATE.STATE_INGAME != s_gameState)
			return;

		AsEventNotifyMgr.Instance.CenterNotify.AddResetAlarmMessage( AsTableManager.Instance.GetTbl_String(2021));
		AsEventNotifyMgr.Instance.CenterNotify.AddResetAlarmMessage( AsTableManager.Instance.GetTbl_String(2022));
		AsEventNotifyMgr.Instance.CenterNotify.AddResetAlarmMessage( AsTableManager.Instance.GetTbl_String(2023));
		AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(2021), eCHATTYPE.eCHATTYPE_SYSTEM);
		AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(2022), eCHATTYPE.eCHATTYPE_SYSTEM);
		AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(2023), eCHATTYPE.eCHATTYPE_SYSTEM);
	}

	void SecondResetAlarm()
	{
		if( GAME_STATE.STATE_INGAME != s_gameState)
			return;

		AsEventNotifyMgr.Instance.CenterNotify.AddResetAlarmMessage( AsTableManager.Instance.GetTbl_String(2024));
		AsEventNotifyMgr.Instance.CenterNotify.AddResetAlarmMessage( AsTableManager.Instance.GetTbl_String(2025));
		AsEventNotifyMgr.Instance.CenterNotify.AddResetAlarmMessage( AsTableManager.Instance.GetTbl_String(2026));
		AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(2024), eCHATTYPE.eCHATTYPE_SYSTEM);
		AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(2025), eCHATTYPE.eCHATTYPE_SYSTEM);
		AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(2026), eCHATTYPE.eCHATTYPE_SYSTEM);
	}

	void ThirdResetAlarm()
	{
		if( GAME_STATE.STATE_INGAME != s_gameState)
			return;

		AsEventNotifyMgr.Instance.CenterNotify.AddResetAlarmMessage( AsTableManager.Instance.GetTbl_String(2027));
		AsEventNotifyMgr.Instance.CenterNotify.AddResetAlarmMessage( AsTableManager.Instance.GetTbl_String(2028));
		AsEventNotifyMgr.Instance.CenterNotify.AddResetAlarmMessage( AsTableManager.Instance.GetTbl_String(2029));
		AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(2027), eCHATTYPE.eCHATTYPE_SYSTEM);
		AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(2028), eCHATTYPE.eCHATTYPE_SYSTEM);
		AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(2029), eCHATTYPE.eCHATTYPE_SYSTEM);
	}


	void LateUpdate()
	{
		AsFrameSkipManager.Instance.LateUpdate ();
	}

	// Update is called once per frame
	void Update()
	{
		AsFrameSkipManager.Instance.Update ();

#if UNITY_ANDROID
		if( ( null == quitDlg) && ( true == Input.GetKeyDown( KeyCode.Escape)))
		{
			if( null == AsHudDlgMgr.Instance)
			{
				if( false == _CheckOpenUI())
				{
					quitDlg = AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(1583), AsTableManager.Instance.GetTbl_String(1582),
						this, "Quit", "CancelQuit", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);
				}
			}
			else
			{
				if( false == _CheckOpenUI() && false == AsHudDlgMgr.Instance.CheckOpenUI())
				{
					quitDlg = AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(1583), AsTableManager.Instance.GetTbl_String(1582),
						this, "Quit", "CancelQuit", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);
				}
			}				
		}
		else if( ( null != quitDlg) && ( true == Input.GetKeyDown( KeyCode.Escape)))
		{
			quitDlg.Close();
		}
#endif
	}

	void OnEnable()
	{
		fps.gameObject.SetActiveRecursively( showFPS);
	}

	// < ilmeda, 20120824
	public GAME_LANGUAGE GetGameLanguage()
	{
		return gameLanguage;
	}

	public bool GetUseAssetbundle()
	{
		return useAssetbundle;
	}
	// ilmeda, 20120824 >

	public void ShowFPS()
	{
		showFPS = !showFPS;
		fps.gameObject.SetActiveRecursively( showFPS);
	}

	static public bool GetOptionState( OptionBtnType eOptionBtnType)
	{
		return s_optionState[ (int)eOptionBtnType];
	}

	static public void SetOptionState( OptionBtnType eOptionBtnType, bool bState)
	{
		s_optionState[ (int)eOptionBtnType] = bState;
	}

	static public void SetPersonalInfo( PersonalInfoType ePersonalInfoType, byte PersonalInfo)
	{
		s_personalInfo[ (int)ePersonalInfoType] = PersonalInfo;
	}

	static public byte GetPersonalInfo( PersonalInfoType ePersonalInfoType)
	{
		return s_personalInfo[ (int)ePersonalInfoType];
	}

	static public string strOptionFileName
	{
		get { return "asoption.txt"; }
	}

	static public string strPersonalInfoName
	{
		get { return "aspersonalinfo.txt"; }
	}

	static public void SaveOption()
	{
		StringBuilder sb = new StringBuilder( AsUtil.strSaveDataPath);
		sb.Append( '/');
		sb.Append( strOptionFileName);

		FileStream fs = new FileStream( sb.ToString(), FileMode.OpenOrCreate);
		fs.Seek( 0, SeekOrigin.Begin);

		byte[] byteData = new byte[(int)OptionBtnType.OptionBtnType_Max];
		for( int i = 0; i < (int)OptionBtnType.OptionBtnType_Max; i++)
			byteData[i] = s_optionState[i] ? (byte)'1' : (byte)'0';

		fs.Write( byteData, 0, byteData.Length);
		fs.Close();
	}

	static public void SavePersonalInfo()
	{
		StringBuilder sb = new StringBuilder( AsUtil.strSaveDataPath);
		sb.Append( '/');
		sb.Append( strPersonalInfoName);

		FileStream fs = new FileStream( sb.ToString(), FileMode.OpenOrCreate);
		fs.Seek( 0, SeekOrigin.Begin);

		byte[] byteData = new byte[(int)PersonalInfoType.PersonalInfoType_Max];
		for( int i = 0; i < (int)PersonalInfoType.PersonalInfoType_Max; i++)
			byteData[i] = s_personalInfo[i];

		fs.Write( byteData, 0, byteData.Length);
		fs.Close();
	}

	private void _InitOptionState()
	{
		// default option
		for( int i = 0; i < (int)OptionBtnType.OptionBtnType_Max; i++)
		{
			s_optionState[i] = true;
			
			if((OptionBtnType)i == OptionBtnType.OptionBtnType_VoiceBattle || 
			   (OptionBtnType)i == OptionBtnType.OptionBtnType_TargetChange) //$yde
				s_optionState[i] = false;
		}

		// load optionfile
		System.String[] res = System.IO.Directory.GetFiles( AsUtil.strSaveDataPath, strOptionFileName);

		if( res.Length > 0)
		{
			StringBuilder sb = new StringBuilder( AsUtil.strSaveDataPath);
			sb.Append( '/');
			sb.Append( strOptionFileName);
//			string strPath = Application.persistentDataPath + "/" + strOptionFileName;
			FileStream fs = new FileStream( sb.ToString(), FileMode.Open);

			if( (long)OptionBtnType.OptionBtnType_Max <= fs.Length)
			{
				byte[] buffer = new byte[fs.Length];
				fs.Read( buffer, 0, (int)( fs.Length));

				for( int i = 0; i < (int)OptionBtnType.OptionBtnType_Max; i++)
				{
					s_optionState[i] = ( '1' == buffer[i]) ? true : false;
				}
			}

			fs.Close();
		}
	}

	private void _InitPersonalInfo()
	{
		// default setting
		for( int i = 0; i < (int)PersonalInfoType.PersonalInfoType_Max; i++)
			s_personalInfo[i] = (byte)'0';

//#if !UNITY_EDITOR
		// load personalinfo file
		System.String[] res = System.IO.Directory.GetFiles( AsUtil.strSaveDataPath, strPersonalInfoName);

		if( res.Length > 0)
		{
			StringBuilder sb = new StringBuilder( AsUtil.strSaveDataPath);
			sb.Append( '/');
			sb.Append( strPersonalInfoName);

			FileStream fs = new FileStream( sb.ToString(), FileMode.Open);

			if( (long)PersonalInfoType.PersonalInfoType_Max == fs.Length)
			{
				byte[] buffer = new byte[fs.Length];
				fs.Read( buffer, 0, (int)( fs.Length));

				for( int i = 0; i < (int)PersonalInfoType.PersonalInfoType_Max; i++)
				{
					s_personalInfo[i] = buffer[i];
				}
			}

			fs.Close();
		}
//#endif
	}

#if UNITY_ANDROID
	private bool _CheckOpenUI()
	{
		bool isClose = false;
		
		if( true == AsNotify.Instance.IsOpenMessageBox())
		{
			AsNotify.Instance.CloseAllMessageBox();
			return true;
		}
		
		if( null != AsNotify.Instance.CashStoreRef)
		{
			AsNotify.Instance.CashStoreRef.Close();
			isClose = true;
		}

		GameObject goServerList = GameObject.Find( "ServerList");

		if( null != goServerList)
		{
			AsServerListFramework serverList = goServerList.GetComponentInChildren<AsServerListFramework>();
			if( null != serverList)
			{
				if( true == serverList.IsOpenServerListFrameworkDlg())
				{
					serverList.CloseServerListFrameworkDlg();
					isClose = true;
				}
			}
		}
		
		GameObject goCharacterCreateFramework = GameObject.Find( "CharacterCreateFramework");
		if( null != goCharacterCreateFramework)
		{
			AsCharacterCreateFramework characterCreateFramework = goCharacterCreateFramework.GetComponentInChildren<AsCharacterCreateFramework>();
			if( null != characterCreateFramework)
			{
				if( true == characterCreateFramework.IsOpenRecommendDlg())
				{
					characterCreateFramework.CancelRecommendDlg();
					isClose = true;
				}
			}
		}

		return isClose;
	}
#endif
}
