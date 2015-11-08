using UnityEngine;
using System.Collections;

public class AsSpriteBlinker : MonoBehaviour
{
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
			return;
		}
		
		curColor.a = Mathf.Clamp01( Mathf.Abs( Mathf.Sin( 2.0f * Mathf.PI * ( curTime - startTime) / period)) * multiplier);
		renderer.sharedMaterial.SetColor( "_TintColor", curColor);
		renderer.sharedMaterial.SetColor( "_Color", curColor);
	}
	
	public void Play()
	{
		curColor = orgColor;
		startTime = Time.time;
		renderer.enabled = true;
		isPlayed = true;
	}
//	
//	void OnGUI()
//	{
//		if( true == GUI.Button( new Rect( 0, 0, 100, 100), "Start"))
//			Play();
//	}
}
