using UnityEngine;
using System.Collections;

public class AsWemadeCI : AsSceneBase
{
	private float accumeTime = 0.0f;
	private bool nextScene = false;

	public AsIntroFramework parentFramework = null;

	// Use this for initialization
	void Start()
	{
		accumeTime = 0.0f;
	}

	// Update is called once per frame
	void Update()
	{
		if( true == gameObject.active)
		{
			accumeTime += Time.deltaTime;
			
#if UNITY_EDITOR
			if( ( false == nextScene) && ( 0.5f < accumeTime))
#else
			if( ( false == nextScene) && ( 3.0f < accumeTime))
#endif
			{
				parentFramework.NextStep();
				nextScene = true;
			}
		}
	}

	public override void Enter()
	{
		gameObject.SetActiveRecursively( true);
	}

	public override void Exit()
	{
		AsFadeObject fo = gameObject.GetComponent<AsFadeObject>();
		if( null != fo)
			fo.SetFadeMode( FADEMODE.FM_OUT);
	}
}
