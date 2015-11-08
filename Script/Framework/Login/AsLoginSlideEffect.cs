using UnityEngine;
using System.Collections;

public class AsLoginSlideEffect : MonoBehaviour
{
	// Use this for initialization
	void Start()
	{
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	public bool Enable
	{
		set
		{
			renderer.enabled = value;
		}
	}
}
