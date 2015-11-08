using UnityEngine;
using System.Collections;

// Just a conversion from JavaScript to C# of FPSCounter.js
public class FPSCounter : MonoBehaviour
{
	public SpriteText txt = null;
	public float updateInterval = 1.0f;
	private float accum = 0.0f; // FPS accumulated over the interval
	private int frames = 0; // Frames drawn over the interval
	private float timeleft; // Left time for current interval
	private float fps = 15.0f; // Current FPS
	private float lastSample;
	private int gotIntervals = 0;

	// Use this for initialization
	void Start()
	{
		timeleft = updateInterval;
//		lastSample = Time.realtimeSinceStartup;
		lastSample = Time.time;
//		StartCoroutine( DisplayFPS());
	}

	public float GetFPS()
	{
		return fps;
	}

	public bool HasFPS()
	{
		return ( gotIntervals > 2) ? true : false;
	}
	
	IEnumerator DisplayFPS()
	{
		while( true)
		{
			yield return null;
			txt.Text = string.Format( "FPS :{0:F2}", fps);
		}
	}

	// Update is called once per frame
	void Update()
	{
		++frames;
		float newSample = Time.time;
		float deltaTime = newSample - lastSample;
		lastSample = newSample;

		timeleft -= deltaTime;
		accum += 1.0f / deltaTime;

		// Interval ended - update GUI text and start new interval
		if( timeleft <= 0.0f)
		{
			// display two fractional digits (f2 format)
			fps = accum / frames;
			timeleft = updateInterval;
			accum = 0.0f;
			frames = 0;
			++gotIntervals;
		}

		txt.Text = string.Format( "FPS :{0:F2}", fps);
	}
}
