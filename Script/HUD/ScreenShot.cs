using UnityEngine;
using System.Collections;

public class ScreenShot : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		  if (Input.GetKeyDown ("space"))
			AsUtil.Capture();
	}
	
	
}
