using UnityEngine;
using System.Collections;

public class Splash : MonoBehaviour
{
	public AudioSource m_AudioSource;
	public float splashWaitSecond = 0.8f;

	void Start()
	{
		#if ((UNITY_IOS || UNITY_IPHONE) && ! UNITY_EDITOR)
		AudioSettings.outputSampleRate = 44100;
		#endif
		m_AudioSource.audio.Play();

		Debug.Log("Splash start");
		StartCoroutine( LoadGameMain());
	}
	
	IEnumerator LoadGameMain()
	{
		yield return new WaitForSeconds(splashWaitSecond);
		var load = Application.LoadLevelAsync( "GameMain");
		while (!load.isDone) {
			yield return new WaitForFixedUpdate();		
		}
	}

}
