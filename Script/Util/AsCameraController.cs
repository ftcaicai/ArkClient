using UnityEngine;
using System.Collections;

public class AsCameraController : MonoBehaviour
{
	public float velocity = 0.1f;
	public float angularVelocity = 60.0f;

	void Update()
	{
		transform.Translate( Input.GetAxis( "Horizontal") * velocity, 0, Input.GetAxis( "Vertical") * velocity);

		if( Input.GetMouseButton(1))
		{
			transform.Rotate( transform.right * Input.GetAxis( "Mouse Y") * Time.deltaTime * -angularVelocity, Space.World);
			transform.Rotate( Vector3.up * Input.GetAxis( "Mouse X") * Time.deltaTime * angularVelocity, Space.World);
		}
	}
}
