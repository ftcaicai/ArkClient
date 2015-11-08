using UnityEngine;
using System.Collections;

[AddComponentMenu( "ArkSphere/AmbientSoundObject")]

public class AsAmbientSoundObject : MonoBehaviour
{
	private AudioSource audioSource;
	
	public float delay = 1.0f;
	public float deviation = 0.0f;
	
	// Use this for initialization
	IEnumerator Start()
	{
		float tempDeviation = deviation;
		
		audioSource = transform.audio;
		
		while( true)
		{
			if( true == audioSource.isPlaying)
				yield return null;
			
			yield return new WaitForSeconds( delay + tempDeviation);
			
			audioSource.Play();
			tempDeviation = Random.Range( -deviation, deviation);
		}
	}
	
	// Update is called once per frame
	void Update()
	{
		if( audioSource.mute == AsGameMain.GetOptionState( OptionBtnType.OptionBtnType_SoundEff))
			audioSource.mute = !audioSource.mute;
	}
}
