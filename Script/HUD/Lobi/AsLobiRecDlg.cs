using UnityEngine;
using System.Collections;

public class AsLobiRecDlg : MonoBehaviour {
	
	public enum eLobiRecMenu
	{
		NONE = -1,
		eOpenCommunity = 0,	//OpenLobiApplication
		ePresentLobiPost,
		eStartCapturing,

		
		MaxMenu
	}
	
	public enum eInCameraSetup
	{
		
		eInCamera_On = 0,
		eInCamera_Off,

		eInCameraSetup_MAX
	};
	public SpriteText m_TextTitle;
	public SpriteText m_TextInCameraSetup;
	public UIButton m_CloseBtn = null;
	public UIButton[] m_MenuBtn = null;
	public UIRadioBtn[] m_InCameraRadioBtn;
	public SpriteText   m_TextUnSupported;

	AsLobiAgreement m_AgreementDlg = null;

	void SetLanguageText()
	{
		for( int i = 0; i < (int)eLobiRecMenu.MaxMenu; ++i)
		{
			AsLanguageManager.Instance.SetFontFromSystemLanguage( m_MenuBtn[i].spriteText);
		}

		AsLanguageManager.Instance.SetFontFromSystemLanguage(m_TextTitle);
		AsLanguageManager.Instance.SetFontFromSystemLanguage(m_TextInCameraSetup);

		m_TextTitle.Text = AsTableManager.Instance.GetTbl_String(1792);
		m_TextInCameraSetup.Text = AsTableManager.Instance.GetTbl_String(19016);
		m_TextUnSupported.Text = AsTableManager.Instance.GetTbl_String(19019);

		m_MenuBtn[(int)eLobiRecMenu.eOpenCommunity ].Text = AsTableManager.Instance.GetTbl_String(19013);
		m_MenuBtn[(int)eLobiRecMenu.ePresentLobiPost ].Text = AsTableManager.Instance.GetTbl_String(19014);
		m_MenuBtn[(int)eLobiRecMenu.eStartCapturing ].Text = AsTableManager.Instance.GetTbl_String(19015);
	
		
		m_InCameraRadioBtn[(int)eInCameraSetup.eInCamera_On ].Text = AsTableManager.Instance.GetTbl_String(1460); //On.
		m_InCameraRadioBtn[(int)eInCameraSetup.eInCamera_Off].Text = AsTableManager.Instance.GetTbl_String(1461);//Off.
		
		
	}
	
	// Use this for initialization
	void Start()
	{
		SetLanguageText();
		m_MenuBtn[(int)eLobiRecMenu.eOpenCommunity ].SetInputDelegate( OpenCommunityBtnDelegate);
		m_MenuBtn[(int)eLobiRecMenu.ePresentLobiPost ].SetInputDelegate( PresentLobiPostBtnDelegate);
		m_MenuBtn[(int)eLobiRecMenu.eStartCapturing ].SetInputDelegate( StartCapturingBtnDelegate);

		m_InCameraRadioBtn[(int)eInCameraSetup.eInCamera_On ].SetInputDelegate( InCameraOnBtnDelegate);
		m_InCameraRadioBtn[(int)eInCameraSetup.eInCamera_Off].SetInputDelegate( InCameraOffBtnDelegate);
		
		m_CloseBtn.SetInputDelegate( CloseBtnDelegate);
	}
	
	void OnEnable()
	{
		#if UNITY_ANDROID
		m_InCameraRadioBtn[(int)eInCameraSetup.eInCamera_On ].gameObject.SetActiveRecursively( false);
		m_InCameraRadioBtn[(int)eInCameraSetup.eInCamera_Off].gameObject.SetActiveRecursively( false);
		m_TextInCameraSetup.gameObject.SetActiveRecursively( false);
		#endif
	
	}

	public void Close()
	{
		AsHudDlgMgr.Instance.CloseLobiRecDlg();
	}


	
	
	public void  Init()
	{
		int isInCamera = PlayerPrefs.GetInt( "InCameraRadioBtn"); //If it doesn't exist, it will return defaultValue: int = 0.
		Debug.Log("AsLobiRecDlg::Init isInCamera:" +isInCamera.ToString());

		if(AsLobiSDKManager.Instance.IsSupported())
		{
		//	AsUtil.SetButtonState(m_MenuBtn[(int)eLobiRecMenu.eOpenCommunity ], UIButton.CONTROL_STATE.NORMAL);
		//	AsUtil.SetButtonState(m_MenuBtn[(int)eLobiRecMenu.ePresentLobiPost ], UIButton.CONTROL_STATE.NORMAL);
			AsUtil.SetButtonState(m_MenuBtn[(int)eLobiRecMenu.eStartCapturing], UIButton.CONTROL_STATE.NORMAL);
			#if UNITY_IPHONE
			SetVisibleInCameraBtn( isInCamera);
			#endif
			m_TextUnSupported.gameObject.SetActiveRecursively(false);
		}
		else
		{
		//	AsUtil.SetButtonState(m_MenuBtn[(int)eLobiRecMenu.eOpenCommunity ], UIButton.CONTROL_STATE.DISABLED);
		//	AsUtil.SetButtonState(m_MenuBtn[(int)eLobiRecMenu.ePresentLobiPost ], UIButton.CONTROL_STATE.DISABLED);
			AsUtil.SetButtonState(m_MenuBtn[(int)eLobiRecMenu.eStartCapturing], UIButton.CONTROL_STATE.DISABLED);
			
			m_InCameraRadioBtn[(int)eInCameraSetup.eInCamera_On ].gameObject.SetActiveRecursively(false);
			m_InCameraRadioBtn[(int)eInCameraSetup.eInCamera_Off].gameObject.SetActiveRecursively(false);
			m_TextInCameraSetup.gameObject.SetActiveRecursively(false);

			m_TextUnSupported.gameObject.SetActiveRecursively(true);
		}

	
	}
	
	private void CloseBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			AsHudDlgMgr.Instance.CloseLobiRecDlg();
		}
	}
	
	private void OpenCommunityBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			Debug.Log("AsLobiRecDlg::OpenCommunityBtnDelegate");
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			AsLobiSDKManager.Instance.OpenLobiApplication();
			AsHudDlgMgr.Instance.CloseLobiRecDlg();
			AsPartyTrackManager.Instance.SetEvent(AsPartyTrackManager.ePartyTrackEvent.eStart_Lobi);
		}
	}
	
	private void PresentLobiPostBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			Debug.Log("AsLobiRecDlg::PresentLobiPostBtnDelegate");
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			int nState = PlayerPrefs.GetInt( "LobiAgreement"); 
			if(nState == 1 && AsLobiSDKManager.Instance.IsSupported())
				AsLobiSDKManager.Instance.PresentLobiPost();
			else
				AsLobiSDKManager.Instance.PresentLobiPlay();

			AsHudDlgMgr.Instance.CloseLobiRecDlg();
		}
	}
	
	private void StartCapturingBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			int nState = PlayerPrefs.GetInt( "LobiAgreement"); 
			if(nState == 0 )
			{
				LoadLobiAgreement();
			}
			else
			{
				Debug.Log("AsLobiRecDlg::StartCapturingBtnDelegate");
				AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
				AsHudDlgMgr.Instance.ShowLobiRecBtn();
				AsLobiSDKManager.Instance.StartCapturing();
				AsPartyTrackManager.Instance.SetEvent(AsPartyTrackManager.ePartyTrackEvent.eStart_LobiRec);
				AsHudDlgMgr.Instance.CloseLobiRecDlg();
			}

		
		}
	}

	//On  InCamera
	private void InCameraOnBtnDelegate( ref POINTER_INFO ptr) 
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6005_EFF_NextPage", Vector3.zero, false);
			if(AsLobiSDKManager.Instance.IsSupported())
			{
				#if UNITY_IPHONE
				SetVisibleInCameraBtn( (int)eInCameraSetup.eInCamera_On);
				#endif
			}

			
		}
	}
	
	//Off  InCamera
	private void InCameraOffBtnDelegate( ref POINTER_INFO ptr) 
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6005_EFF_NextPage", Vector3.zero, false);
			if(AsLobiSDKManager.Instance.IsSupported())
			{
				#if UNITY_IPHONE
				SetVisibleInCameraBtn( (int)eInCameraSetup.eInCamera_Off);
				#endif
			}

			
		}
	}
	
	private void SetVisibleInCameraBtn( int index)
	{
		foreach( UIRadioBtn radioBtn in m_InCameraRadioBtn)
			radioBtn.SetState( 1);
		
		m_InCameraRadioBtn[index].SetState( 0);
		if( index == (int)eInCameraSetup.eInCamera_On)
		{
			AsLobiSDKManager.Instance.IsInCamera = true;
			PlayerPrefs.SetInt( "InCameraRadioBtn",0);
		}
		else
		{
			AsLobiSDKManager.Instance.IsInCamera = false;
			PlayerPrefs.SetInt( "InCameraRadioBtn",1);
		}
		
	}

	void OnDestroy() {

		if( null != m_AgreementDlg)
		{
			GameObject.Destroy( m_AgreementDlg.gameObject );
			m_AgreementDlg = null;
		}
	}

	public void LoadLobiAgreement()
	{
		if( null == m_AgreementDlg)
		{
			GameObject obj = Instantiate( Resources.Load( "UI/AsGUI/GUI_RecordPopup")) as GameObject;
			m_AgreementDlg = obj.GetComponent<AsLobiAgreement>();
		}
	
		m_AgreementDlg.Open();

	}
}
