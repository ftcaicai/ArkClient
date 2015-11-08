using UnityEngine;
using System;
using System.Collections;

public class AsLoginFramework : AsFrameworkBase
{
	private const int SU_LOGIN = 0;

	public AsSceneBase[] steps;
	[SerializeField]SpriteText versionText = null;
	
	void Awake()
	{
		versionText.Text = VersionManager.version;
	}
	
	//$yde
	void OnEnable()
	{
		if( null != AsUserInfo.Instance)
		{
			AsUserInfo.Instance.ClosePrivateShop();
		}
		
		if( null != AsPStoreManager.Instance)
		{
			AsPStoreManager.Instance.GameReset();
		}
	}

	// Use this for initialization
	void Start()
	{
		AsGameMain.s_gameState = GAME_STATE.STATE_LOGIN;

		AsEntityManager.Instance.RemoveAllEntities();

		steps[0].Enter();

		AsSoundManager.Instance.PlayBGM( "Sound/World/BGM/Patch01/S0001_BGM_Login");
	}

	// Update is called once per frame
	void Update()
	{
	}

	public override void PrevStep()
	{
	}

	public override void NextStep()
	{
	}

	public override void DeactivePrevStep()
	{
	}

	public override void OnNotify( NOTIFY_MSG msg)
	{
	}
}
