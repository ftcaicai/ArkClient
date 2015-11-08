using UnityEngine;
using System.Collections;

public class AsCameraTest : MonoBehaviour 
{
	// init value
	/*public float CharHeight = 0.0f;
	public float CameraHeight = 17.0f;
	public float CameraDistance = -16.0f;
	public float Fov = 20.0f;
	public float YRotate = 0.0f;	*/
	
	public float zoom = 0.0f;
	
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
        /*if( 0.0f >= zoom )
        {
            zoom = 0.0f;
        }
		
        if( CameraMgr.MaxZoomInDistance <= zoom )
        {
            zoom = CameraMgr.MaxZoomInDistance;
        }*/
		
		
		/*CameraData cameradata = new CameraData();	
		cameradata.CharHeight = CharHeight;
		cameradata.CameraHeight = CameraHeight;
		cameradata.CameraDistance = CameraDistance;
		cameradata.Fov = Fov;
		cameradata.YRotate = YRotate;		
		CameraMgr.Instance.SetCurCameraData(cameradata);*/
		
		CameraMgr.Instance.SetZoom ( zoom  ); 
	}
}
