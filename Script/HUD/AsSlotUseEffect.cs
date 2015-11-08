using UnityEngine;
using System.Collections;

public class AsSlotUseEffect : SimpleSprite
{
	private Color orgColor;
	private Color curColor;
	private bool m_isLoop = false;
	public float effectSpeed = 2f;

	
	public void SetLoop( bool isLoop )
	{
		m_isLoop = isLoop;;
	}
	
	
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
			if( true == m_isLoop )
			{
				curColor.a = 1.0f;
			}
			else
			{				
				curColor.a = 0.0f;
				gameObject.SetActiveRecursively( false);
				return;
			}
		}
		
		curColor.a -= ( Time.deltaTime * effectSpeed);
		renderer.sharedMaterial.SetColor( "_TintColor", curColor);
	}
	
	public void Draw( Vector3 pos)
	{
		gameObject.SetActiveRecursively( true);
		curColor = new Color( 1, 1, 1, 1);
		renderer.sharedMaterial.SetColor( "_TintColor", curColor);
		transform.position = new Vector3( pos.x, pos.y, pos.z - 2.0f);
	}
}
