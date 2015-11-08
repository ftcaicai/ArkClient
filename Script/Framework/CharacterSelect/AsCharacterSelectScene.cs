using UnityEngine;
using System.Collections;

public class AsCharacterSelectScene : AsSceneBase
{
	void Awake()
	{
		gameObject.SetActiveRecursively( false );
	}

	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
	}

	public override void Enter()
	{
		gameObject.SetActiveRecursively( true );
	}

	public override void Exit()
	{
		AsFadeObject fo = gameObject.GetComponent<AsFadeObject>();
		if( null != fo )
			fo.SetFadeMode( FADEMODE.FM_OUT );

		//AsSoundManager.Instance.StopBGM();
	}
}
