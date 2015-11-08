using UnityEngine;
using System.Collections;

public enum BILLBOARD_AXIS
{
	BA_INVALID = -1,
	ALL_AXIS,
	X_AXIS,
	Y_AXIS,
	Z_AXIS
};

[AddComponentMenu( "ArkSphere/Billboard")]
[ExecuteInEditMode]
public class AsBillboard : MonoBehaviour
{
	public BILLBOARD_AXIS axis;
	
	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
		if( true == Application.isPlaying)
			return;
//		if( true == Application.isPlaying)
//		{
//			AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
//			if( null != userEntity)
//			{
//				Vector3 distance = userEntity.transform.position - gameObject.transform.position;
//				if( 6.0f < distance.magnitude)
//					return;
//			}
//		}
		
		Transform target = Camera.main.transform;
	
		switch( axis)
		{
		case BILLBOARD_AXIS.ALL_AXIS:
			transform.forward = -target.forward;
			break;
		case BILLBOARD_AXIS.X_AXIS:
			{
				Vector3 p1 = transform.InverseTransformDirection( target.position - transform.position);
				p1.x = 0.0f;
				p1 = transform.TransformDirection( p1);
				p1.Normalize();
				transform.forward = p1;
			}
			break;
		case BILLBOARD_AXIS.Y_AXIS:
			{
				Vector3 dir = target.position - transform.position;
				dir.y = 0.0f;
				transform.forward = dir;
			}
			break;
		case BILLBOARD_AXIS.Z_AXIS:
			{
				Vector3 dir = target.position - transform.position;
				dir.z = 0.0f;
				transform.forward = dir;
			}
			break;
		}
	}
}
