using UnityEngine;
using System.Collections;

public class DestroyDetector : MonoBehaviour {

	void Awake()
	{
		Debug.Log("DestroyDetector:: Awake: ");
	}

	// Use this for initialization
	void Start () {
	
		Debug.Log("DestroyDetector:: Start: ");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnDisable()
	{
		Debug.Log("DestroyDetector:: OnDisable: ");
	}

	void OnDestroy()
	{
		Debug.Log("DestroyDetector:: OnDestroy: ");
	}
}
