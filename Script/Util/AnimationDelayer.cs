using UnityEngine;
using System.Collections;

public class AnimationDelayer : MonoBehaviour
{
	void Start()
	{
		AnimationState animState = null;
		
		foreach( AnimationState state in animation)
		{
			animState = state;
			break;
		}
		
		if( animState == null)
		{
			Debug.LogWarning( "AnimationDelayer::Start: There is no Animation component attached on " + name);
			return;
		}
		
		animState.time = Random.Range( 0, animState.length);
	}
	
	void Update()
	{
	}
}
