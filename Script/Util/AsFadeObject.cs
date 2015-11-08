using System;
using UnityEngine;
using System.Collections;


public enum FADEMODE
{
	FM_NONE = 0,
	FM_IN,
	FM_OUT,
}

public class AsFadeObject : MonoBehaviour
{
	private FADEMODE fadeMode = FADEMODE.FM_NONE;
	public float fadeSpeed = 1.0f;
	private string propertyName = "";
	private AsFrameworkBase parentFramework;
	private Color originColor;
	private Color currentColor;

	public bool fadeActive = false;
	public bool fadeOutToNext = false;
	public bool fadeOutToDeactiveSelf = false;
	public bool fadeOutToDeactiveStep = false;

	void Start()
	{
		GameObject go = GameObject.Find( "AsIntroFramework" );
		if( go )
			parentFramework = go.GetComponent<AsFrameworkBase>() as AsFrameworkBase;
	}

	void OnEnable()
	{
		if( fadeActive )
		{
			currentColor.a = 0.0f;

			try
			{
				if( renderer.sharedMaterials.Length > 1 )
				{
					for( int i = 0; i < renderer.sharedMaterials.Length; ++i )
					{
						renderer.materials[i].SetColor( propertyName, currentColor );
					}
				}
				else
				{
					renderer.material.SetColor( propertyName, currentColor );
				}
			}
			catch( Exception e )
			{
				Debug.Log( "FadeObject Error :: \n" + e.Message );
			}

			SetFadeMode( FADEMODE.FM_IN );
		}
	}

	void Awake()
	{
		if( renderer.material.HasProperty( "_Color" ) )
			propertyName = "_Color";
		else if( renderer.material.HasProperty( "_TintColor" ) )
			propertyName = "_TintColor";
		else if( renderer.material.HasProperty( "_MainTex" ) )
			propertyName = "_MainTex";
		
//		originColor = renderer.material.GetColor( propertyName );
		originColor = Color.white;

		currentColor = originColor;
	}

	void Update()
	{
		switch( fadeMode )
		{
		case FADEMODE.FM_IN:	FadeIn();	break;
		case FADEMODE.FM_OUT:	FadeOut();	break;
		}
	}

	public void SetFadeMode( FADEMODE _mode )
	{
		fadeMode = _mode;
	}

	void FadeIn()
	{
		if( true == Application.isEditor)
			fadeSpeed = 10.0f;

		currentColor.a += fadeSpeed * Time.deltaTime;
		if( currentColor.a > 1.0f )
		{
			currentColor.a = 1.0f;
			fadeMode = FADEMODE.FM_NONE;
		}

		if( renderer.sharedMaterials.Length > 1 )
		{
			for( int i = 0; i < renderer.sharedMaterials.Length; ++i )
			{
				renderer.sharedMaterials[i].SetColor( propertyName, currentColor );
			}
		}
		else
		{
			renderer.material.SetColor( propertyName, currentColor );
		}
	}

	void FadeOut()
	{
		//if( true == Application.isEditor)
		//	fadeSpeed = 10.0f;
		
		currentColor.a -= fadeSpeed * Time.deltaTime;
		if( currentColor.a < 0.0f )
		{
			currentColor.a = 0.0f;
			fadeMode = FADEMODE.FM_NONE;

			if( fadeOutToNext && parentFramework )
				parentFramework.NextStep();

			if( fadeOutToDeactiveSelf )
				gameObject.SetActiveRecursively( false );

			if( fadeOutToDeactiveStep )
				parentFramework.DeactivePrevStep();
		}

		if( renderer.sharedMaterials.Length > 1 )
		{
			for( int i = 0; i < renderer.sharedMaterials.Length; ++i )
			{
				renderer.sharedMaterials[i].SetColor( propertyName, currentColor );
			}
		}
		else
		{
			renderer.material.SetColor( propertyName, currentColor );
		}
	}
}
