using UnityEngine;
using System.Collections;

public class AsUIFxController : MonoBehaviour
{
	// Use this for initialization
	void Start()
	{
		Stop();
		SetWrapMode( WrapMode.Once);
	}
	
	// Update is called once per frame
	void Update()
	{
	}

	public void Play()
	{
		Camera fxCam = gameObject.GetComponentInChildren<Camera>();
		fxCam.enabled = true;

		animation.Play();

		Animation[] anims = gameObject.GetComponentsInChildren<Animation>();
		foreach( Animation ani in anims )
		{
			ani.Play();
		}
	}

	public void Stop()
	{
		Camera fxCam = gameObject.GetComponentInChildren<Camera>();
		fxCam.enabled = false;

		animation.Stop();

		Animation[] anims = gameObject.GetComponentsInChildren<Animation>();
		foreach( Animation ani in anims )
		{
			ani.Stop();
		}
	}

	public void SetWrapMode( WrapMode mode)
	{
		animation.wrapMode = mode;

		Animation[] anims = gameObject.GetComponentsInChildren<Animation>();
		foreach( Animation ani in anims )
		{
			ani.wrapMode = mode;
		}
	}
}
