using UnityEngine;
using System.Collections;

public class AsTextBlinker : MonoBehaviour
{
	public SpriteText text = null;
	public int count = 0;
	public float period = 1.0f;
	public float multiplier = 1.0f;
	public Color orgColor = Color.white;
	private Color curColor = Color.white;
	private float startTime = 0.0f;
	private bool isPlayed = false;
	public bool IsPlaying
	{
		get	{ return isPlayed; }
	}
	
	// Use this for initialization
	void Start()
	{
		s_Blinking = false; //$yde
		
		if( true == isPlayed)
			return;
		
		renderer.enabled = false;
	}
	
	// Update is called once per frame
	void Update()
	{
		if( false == renderer.enabled)
			return;
		
		float curTime = Time.time;
		if( ( ( curTime - startTime) / period) >= ( count * 0.5f))
		{
			renderer.enabled = false;
			isPlayed = false;
			s_Blinking = false; //$yde
			return;
		}
		
		curColor.a = Mathf.Clamp01( Mathf.Abs( Mathf.Sin( 2.0f * Mathf.PI * ( curTime - startTime) / period)) * multiplier);
//		renderer.sharedMaterial.SetColor( "_TintColor", curColor);
//		renderer.sharedMaterial.SetColor( "_Color", curColor);
//		text.color.a = curColor.a;
		text.SetColor( curColor);
	}
	
	public void Play()
	{
		if(isPlayed == false) //$yde
		{
			curColor = orgColor;
			startTime = Time.time;
			renderer.enabled = true;
			isPlayed = true;
			s_Blinking = true; //$yde
		}
	}
	
	static bool s_Blinking = false; public static bool Blinking{get{return s_Blinking;}}
}
