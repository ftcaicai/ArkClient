using UnityEngine;
using System.Collections;

public class CameraClientTest
{
	public bool Loading( Map map )	
	{		
		Transform transform = GameObject.Find( "PlayerChar" ).transform;
		if( false == CameraMgr.Instance.AttachPlayerCamera( transform, map.MapData.GetMapNameShowTime() ) )
			Debug.LogError(" CameraMgr.Instance.AttachCharCamera() failed ");	
		
			
		return true;
	}
}
