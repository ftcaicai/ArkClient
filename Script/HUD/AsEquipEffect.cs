using UnityEngine;
using System.Collections;

public class AsEquipEffect : MonoBehaviour {
	
	public Animation effectAnimation;
	public Transform followTM = null;
	// Use this for initialization
	void Start () 
	{
		if( null == effectAnimation )
			effectAnimation = gameObject.GetComponentInChildren<Animation>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if( null == effectAnimation )
			return;

		if (followTM != null)
			transform.position = followTM.transform.position;
		
		if( false == effectAnimation.isPlaying )
			GameObject.Destroy( effectAnimation );
	}
}
