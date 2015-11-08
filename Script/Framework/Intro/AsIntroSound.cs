using UnityEngine;
using System.Collections;

public class AsIntroSound : MonoBehaviour
{
	public string soundName = "S0005_BGM_Intro";
	private bool isPlaying = false;
	
	public bool IsPlaying
	{
		get { return isPlaying; }
	}
	
	// Use this for initialization
	void Start()
	{
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	public void PlaySound()
	{
//		isPlaying = true;
//		AsSoundManager.Instance.PlayBGM( soundName);
	}
	
	public void StopSound()
	{
//		isPlaying = false;
//		AsSoundManager.Instance.StopBGM();
	}
	
//	public bool IsPlaying()
//	{
//		return AsSoundManager.Instance.IsPlayingBGM();
//	}
	
	void OnDestroy()
	{
//		AsSoundManager.Instance.StopBGM();
	}
}
