using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CameraAspectReset : MonoBehaviour
{
	// Use this for initialization
	void Start()
	{
		Camera curCamera = GetComponent<Camera>();
		curCamera.aspect = Camera.mainCamera.aspect;
		curCamera.fov = Camera.mainCamera.fov;
	}
	
	// Update is called once per frame
	void Update()
	{
		//Camera curCamera = GetComponent<Camera>();
		//Debug.Log( curCamera.aspect );
	}
}
