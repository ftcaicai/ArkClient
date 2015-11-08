using UnityEngine;
using System.Collections;

public class AsGameTitle : AsSceneBase
{
	public AsIntroFramework parentFramework = null;
	public AsUsePersonalInfoAgreement agreementDlg = null;
	private bool nextStep = false;
	private bool m_bActiveAgreement = true;

	// Use this for initialization
	void Start()
	{
		nextStep = false;
		
		if( null != AssetbundleManager.Instance && true == AssetbundleManager.Instance.useAssetbundle)
			AssetbundleManager.Instance.SceneAssetbundleLoadCache( "Login");
	}
	
	private void CheckAgreementDlgState()
	{
		int agreement = PlayerPrefs.GetInt( "PersonalInfoAgreement");
		if( 1 == agreement)
		{
			m_bActiveAgreement = false;
			agreementDlg.gameObject.SetActive( false);
		}
		else
		{
			m_bActiveAgreement = true;
		}
	}

	// Update is called once per frame
	void Update()
	{
		if( null != AssetbundleManager.Instance && true == AssetbundleManager.Instance.useAssetbundle)
		{
			if( false == AssetbundleManager.Instance.isLoadedScene( "Login"))
				return;
		}
		
		if( true == m_bActiveAgreement)
		{
			if( ( true == agreementDlg.agreementsFlag) && ( true == agreementDlg.policyFlag))
			{
				if( ( false == nextStep))
				{
					DDOL_Tracer.BeginTrace();//$ yde
					Resources.UnloadUnusedAssets();
					parentFramework.NextStep();
					nextStep = true;
					_SetPersonalInfo();
				}
			}
		}
		else
		{
			if( ( false == nextStep))
			{
				DDOL_Tracer.BeginTrace();//$ yde
				Resources.UnloadUnusedAssets();
				parentFramework.NextStep();
				nextStep = true;
			}
		}
	}

	public override void Enter()
	{
		gameObject.SetActiveRecursively( true);
		
		CheckAgreementDlgState();
	}

	public override void Exit()
	{
		gameObject.SetActiveRecursively( false);
		Application.LoadLevel( "Login");
	}
	
	private void _SetPersonalInfo()
	{
		PlayerPrefs.SetInt( "PersonalInfoAgreement", 1);
	}
}
