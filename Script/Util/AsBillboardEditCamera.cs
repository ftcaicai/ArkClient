using UnityEngine;
using System.Collections;


[ExecuteInEditMode]
public class AsBillboardEditCamera : MonoBehaviour
{
	public float zoom = 0.0f;
	public GameObject goTargetObject = null;
	private GameObject goCurTargetObject = null;

	// Use this for initialization
	void Start ()
	{
	}

	// Update is called once per frame
	void Update ()
	{
		Setting();
		Zoom();
	}

	void Zoom()
	{
		if( null == CameraMgr.Instance)
		{
			zoom = 0.0f;
			return;
		}

		if( null == goCurTargetObject)
		{
			zoom = 0.0f;
			return;
		}

		if( 0.0f >= zoom)
			zoom = 0.0f;

		if( CameraMgr.MaxZoomInDistance <= zoom)
			zoom = CameraMgr.MaxZoomInDistance;

		CameraMgr.Instance.SetZoom ( zoom);
	}

	void Setting()
	{
		if( null == goTargetObject)
			return;

		if( goTargetObject == goCurTargetObject)
			return;

		if( null == CameraMgr.Instance)
			gameObject.AddComponent<CameraMgr>();

		goCurTargetObject = goTargetObject;
		if( false == CameraMgr.Instance.AttachPlayerCamera( goCurTargetObject.transform, 1.0f))
			Debug.LogError(" CameraMgr.Instance.AttachCharCamera() failed ");
	}
}
