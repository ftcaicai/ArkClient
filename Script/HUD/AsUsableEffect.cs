using UnityEngine;
using System.Collections;

public class AsUsableEffect : SimpleSprite
{
	private Color orgColor;
	private Color curColor;

	public bool Enable
	{
		set { gameObject.SetActiveRecursively( value); }
	}

	// Use this for initialization
	public override void Start()
	{
		orgColor = renderer.sharedMaterial.GetColor( "_TintColor");
		curColor = orgColor;
	}
	
	// Update is called once per frame
	void Update()
	{
		if( 0.0f >= curColor.a)
		{
			curColor.a = 0.0f;
			gameObject.SetActiveRecursively( false);
			return;
		}
		
		curColor.a -= ( Time.deltaTime * 2.0f);
//		renderer.sharedMaterial.SetColor( "_TintColor", curColor);
		renderer.sharedMaterial.SetColor( "_Color", curColor);
	}
	
	public void Draw( Vector3 pos)
	{
		gameObject.SetActiveRecursively( true);
		curColor = new Color( 1, 1, 1, 1);
//		renderer.sharedMaterial.SetColor( "_TintColor", curColor);
		renderer.sharedMaterial.SetColor( "_Color", curColor);
		transform.position = new Vector3( pos.x, pos.y, pos.z - 2.0f);
	}
}
