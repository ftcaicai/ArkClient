using UnityEngine;
using System.Collections;

public class AsSkillTargetMark : MonoBehaviour
{
	private Color orgColor;
	private Color curColor;
	private float multiplier = 1.0f;

	public bool Enable
	{
		set { gameObject.SetActiveRecursively( value); }
	}

	// Use this for initialization
	void Start()
	{
		orgColor = renderer.sharedMaterial.GetColor( "_TintColor");
		curColor = orgColor;
	}
	
	// Update is called once per frame
	void Update()
	{
		if( 0.0f > curColor.a)
		{
			multiplier = 1.0f;
		}
		else if( 1.0f < curColor.a)
		{
			multiplier = -1.0f;
		}
		
		curColor.a += ( Time.deltaTime * multiplier);
	}
}
