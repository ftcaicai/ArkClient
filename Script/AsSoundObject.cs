using UnityEngine;
using System.Collections;

public class AsSoundObject : MonoBehaviour
{
	private AudioSource audioSource;
	private bool isBGM = false;

	void Start()
	{
		audioSource = transform.audio;
		isBGM = transform.name.Contains( "BGM");
	}

	// Update is called once per frame
	void Update()
	{
		if( ( false == isBGM) && ( false == audioSource.isPlaying))
			Destroy( gameObject);
	}
}
