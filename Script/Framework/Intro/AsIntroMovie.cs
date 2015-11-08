using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsIntroMovie : AsSceneBase
{
//	private float accumeTime = 0.0f;
//	private bool nextScene = false;

	[SerializeField] AsIntroFramework parentFramework_ = null;
//	[SerializeField] MovieTexture movie_ = null;

	// Use this for initialization
	void Start()
	{
#if UNITY_EDITOR
//		movie_.Play();
		parentFramework_.NextStep();
#else
//		if(AsGameMain.GetPersonalInfo(PersonalInfoType.PersonalInfoType_MOVIE_PLAYED) == (byte)'0')
//		{
//			AsGameMain.SetPersonalInfo(PersonalInfoType.PersonalInfoType_MOVIE_PLAYED, (byte)'1');
//			AsGameMain.SavePersonalInfo();
//			Handheld.PlayFullScreenMovie( "Intro.mp4", Color.black, FullScreenMovieControlMode.CancelOnInput, FullScreenMovieScalingMode.AspectFit);
//			
//		}
		parentFramework_.NextStep();
//		StartCoroutine( NextStep());
#endif
	}
	
	IEnumerator NextStep()
	{
		yield return new WaitForSeconds( 0.5f);
		
		parentFramework_.NextStep();
	}

	// Update is called once per frame
	void Update()
	{
	}

	public override void Enter()
	{
		gameObject.SetActiveRecursively( true);
	}

	public override void Exit()
	{
		gameObject.SetActiveRecursively( false);
	}
	
	void OnMouseUpAsButton()
	{
		parentFramework_.NextStep();
	}
	
	public static void ResetMoviePlayedRecord()
	{
		AsGameMain.SetPersonalInfo(PersonalInfoType.PersonalInfoType_MOVIE_PLAYED, (byte)'0');
		AsGameMain.SavePersonalInfo();
	}
	
//	public static void PlayOpeningMovie()
//	{
//		Handheld.PlayFullScreenMovie("test_movie.mp4", Color.black, FullScreenMovieControlMode.CancelOnInput, FullScreenMovieScalingMode.AspectFit);
//	}

//	void OnGUI()
//	{
//		if(GUI.Button(new Rect(300, 50, 30, 15), "movie") == true)
//		{
//			Handheld.PlayFullScreenMovie("test_movie.mp4", Color.black, FullScreenMovieControlMode.CancelOnInput, FullScreenMovieScalingMode.AspectFit);
//		}
//	}
}
