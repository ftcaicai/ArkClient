
//#define _USE_ASSETBUNDLE

using UnityEngine;
using System.Collections;

public class AsPatcher : AsSceneBase
{
	public AsIntroFramework parentFramework = null;
	
	private float accumeTime = 0.0f;
	private bool nextScene = false;
	private bool m_bStartPatch = false;
	
	void Start()
	{
		accumeTime = 0.0f;
		nextScene = false;
		m_bStartPatch = false;
	}
	
	void Update()
	{
		if( true == gameObject.active)
		{
			if( null != AssetbundleManager.Instance && true == AssetbundleManager.Instance.useAssetbundle)
			{
				if( false == nextScene && true == m_bStartPatch)
				{
					accumeTime += Time.deltaTime;
					
					if( true == AssetbundleManager.Instance.bLoaded && true == AssetbundleManager.Instance.bPatchFinishedBtnOk)
					{
						Debug.Log( "PatchTime: " + accumeTime);
						nextScene = true;
						AssetbundleManager.Instance.SetActiveAssetbundleInfo( true, Color.white);
						AssetbundleManager.Instance.SetActiveLoadingTip( true);
						AssetbundleManager.Instance.SetLoadingText( "", Color.white);
						StartCoroutine( _FinishPatch());
						
						AsPStoreManager.Instance.LoadShopModel_Coroutine();
						AsPreloadManager.Instance.LoadTransformModel();
						AsSoundManager.Instance.StopBGM();
						AsPartyTrackManager.Instance.SetEvent(AsPartyTrackManager.ePartyTrackEvent.eCompletion_Patch);
						
					}
				}
			}
			else
			{
				if( false == nextScene)
				{
					parentFramework.NextStep();
					nextScene = true;
				}
			}
		}
	}

	public override void Enter()
	{
		gameObject.SetActiveRecursively( true);
		StartCoroutine( _StartPatch());
	}

	public override void Exit()
	{
		AsFadeObject fo = gameObject.GetComponent<AsFadeObject>();
		if( null != fo)
			fo.SetFadeMode( FADEMODE.FM_OUT);
//		gameObject.SetActiveRecursively( false);
	}
	
	// < private
	private IEnumerator _StartPatch()
	{
		yield return new WaitForSeconds( 1.0f);

		if( null != AssetbundleManager.Instance && true == AssetbundleManager.Instance.useAssetbundle)
		{
			AssetbundleManager.Instance.DownloadAssets();
			m_bStartPatch = true;
		}
	}
	
	private IEnumerator _FinishPatch()
	{
		float fTime = Time.realtimeSinceStartup;
		float fTime_Tip = fTime;
		bool bFirstTip = true;
		
#if _USE_ASSETBUNDLE
		StartCoroutine( AsTableManager.Instance.LoadTable());
#endif
		
		while( false == AsTableManager.Instance.s_bTableLoaded)
		{
			if( Time.realtimeSinceStartup - fTime > 0.1f)
			{
				fTime = Time.realtimeSinceStartup;
				
				long lPercent = ( AsTableManager.Instance.m_nCurTableLoadCount * 100) / AsTableManager.sTableLoadCount;
				if( lPercent > 100)
					lPercent = 100;
				AssetbundleManager.Instance.SetAssetbundleInfoText( "Loading..." + lPercent.ToString() + "%");
				AssetbundleManager.Instance.SetAssetbundleInfoProgress( (float)lPercent / 100.0f);
				
				// update loading tip
				if( AsTableManager.Instance.m_nCurTableLoadCount > 3)
				{
//					float fTipTimeDelay = 4.0f;
//					Tbl_GlobalWeight_Record record = AsTableManager.Instance.GetTbl_GlobalWeight_Record( 73);
//					if( null != record )
//						fTipTimeDelay = record.Value;
					
					if( Time.realtimeSinceStartup - fTime_Tip > 7.0f || true == bFirstTip)
					{
						bFirstTip = false;
						fTime_Tip = Time.realtimeSinceStartup;
						AssetbundleManager.Instance.SetLoadingText( AsLoadingTipManager.Instance.GetLoadingTip_RandAll(), Color.white);
					}
				}
			}

			yield return null;
		}
		
		AssetbundleManager.Instance.SetActiveAssetbundleInfo( false);
		AssetbundleManager.Instance.SetActiveLoadingTip( false);
		parentFramework.NextStep();
	}

	// private >
}
