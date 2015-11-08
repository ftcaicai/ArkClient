using UnityEngine;
using System.Collections;

public class AsLobiAgreement : MonoBehaviour {

	public UIButton m_OkBtn = null;
	public UIButton m_CancelBtn = null;
	public UIButton m_CloseBtn = null;
	public UIButton m_ShowTermsBtn = null;

	public SpriteText m_TitleText = null;
	public SpriteText m_MessageText = null;

	// Use this for initialization
	void Start () {
		m_TitleText.Text = AsTableManager.Instance. GetTbl_String(19020);
		m_MessageText.Text = AsTableManager.Instance. GetTbl_String(19021);

		m_OkBtn.Text = AsTableManager.Instance. GetTbl_String(19023);
		m_CancelBtn.Text = AsTableManager.Instance. GetTbl_String(1151);
		m_ShowTermsBtn.Text = AsTableManager.Instance. GetTbl_String(19020);
	

		m_OkBtn.SetInputDelegate( OkBtnDelegate);
		m_CancelBtn.SetInputDelegate( CancelBtnDelegate);
		m_CloseBtn.SetInputDelegate( CancelBtnDelegate);
		m_ShowTermsBtn.SetInputDelegate( ShowTermsBtnDelegate);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Close()
	{
		gameObject.SetActiveRecursively(false);
	}

	public bool Open( )
	{
		gameObject.SetActiveRecursively(true);
		Debug.Log("AsLobiAgreement Open");
		return true;
	}

	private void OkBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			Close();
			PlayerPrefs.SetInt( "LobiAgreement", 1);

			AsHudDlgMgr.Instance.ShowLobiRecBtn();
			AsLobiSDKManager.Instance.StartCapturing();
			AsHudDlgMgr.Instance.CloseLobiRecDlg();

			AsPartyTrackManager.Instance.SetEvent(AsPartyTrackManager.ePartyTrackEvent.eStart_LobiRec);

		}
	}

	private void CancelBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			Close();
		}
	}

	private void ShowTermsBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			#if UNITY_IPHONE || UNITY_ANDROID
			string url = string.Empty;
			url = AsTableManager.Instance.GetTbl_String(19022);
			WemeSdkManager.GetMainGameObject.showPlatformViewWeb( url);
			#endif
		}
	}

}
