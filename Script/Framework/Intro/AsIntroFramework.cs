using UnityEngine;
using System.Collections;

public class AsIntroFramework : AsFrameworkBase
{
	public AsSceneBase[] steps;
	public GameObject bg = null;
	[SerializeField]SpriteText versionText = null;

	private int currentStep = 0;
//	private bool isFirst = true;

	void Awake()
	{
		gameObject.SetActiveRecursively( false);
		gameObject.active = true;

	}

	// Use this for initialization
	void Start()
	{
		bg.SetActiveRecursively( true);
		steps[0].Enter();

		AsGameMain.s_gameState = GAME_STATE.STATE_INTRO;
//		isFirst = true;
	}
	
	void OnDestroy()
	{
	}
	
	// Update is called once per frame
	void UpdateX()
	{
//		if( ( string.Empty != VersionManager.version) && ( true == isFirst))
//		{
			versionText.Text = VersionManager.version;
//			isFirst = false;
//		}
	}

	public override void PrevStep()
	{
		steps[ currentStep].Exit();
		currentStep = 1;
		steps[ currentStep].Enter();
	}

	public override void NextStep()
	{
		if( currentStep < steps.Length - 1)
		{
			steps[ currentStep].Exit();
			++currentStep;
			steps[ currentStep].Enter();
		}
		else
		{
			steps[ currentStep].Exit();
			++currentStep;
		}
	}

	public override void DeactivePrevStep()
	{
		if( ( 0 < currentStep) && ( true == steps[ currentStep - 1].gameObject.active))
			steps[ currentStep - 1].gameObject.SetActiveRecursively( false);
	}

	public override void OnNotify( NOTIFY_MSG msg )
	{
	}
}
