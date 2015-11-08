using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CameraComponent : MonoBehaviour
{
	class MoveDirData
	{
		public float maxZoomSpeed = 1.0f;
		public float maxZoomDistance = 0.0f;
	}

	private Transform m_PlayerTransform = null;
	private CameraData m_CameraPosition = null;
	private Vector3 m_vec3CameraLocalPos = Vector3.zero;
	private Vector3 m_vec3MoveDirection = Vector3.zero;
	private Vector3 m_vec3SourcePos = Vector3.zero;

	public void Init( Transform playerTransform, CameraData cameraPosition, float fCompleteTime)
	{
		if ( null == playerTransform)
		{
			Debug.LogError( "playerTransform == null ");
			return;
		}

		if ( null == cameraPosition)
		{
			Debug.LogError( "cameraPosition == null ");
			return;
		}

		m_PlayerTransform = playerTransform;
		m_CameraPosition = cameraPosition;
		m_vec3MoveDirection = GetCameraMoveDirection( m_CameraPosition);

		m_vec3SourcePos = GetPosition( cameraPosition.CameraHeight, cameraPosition.CameraDistance, cameraPosition.YRotate);
		m_vec3SourcePos = m_vec3SourcePos - ( m_vec3MoveDirection * -8.0f);

		ResetCameraLocalPos();
	}

	protected void ResetCameraLocalPos()
	{
		m_vec3CameraLocalPos = m_vec3SourcePos - ( m_vec3MoveDirection * CameraMgr.Instance.getZoomData);
	}

	// Reset camera move direction
	protected Vector3 GetCameraMoveDirection( CameraData cameraPosData)
	{
		Vector3 vec3Direction = GetPosition( cameraPosData.CameraHeight, cameraPosData.CameraDistance, cameraPosData.YRotate);
		vec3Direction.y -= cameraPosData.CharHeight;
		return vec3Direction.normalized;
	}


	// Get Camera position
	protected Vector3 GetPosition( float fCameraHeight, float fCameraDistance, float fRotate)
	{
		Vector3 vec3Position = Vector3.zero;
		vec3Position.y = fCameraHeight;
		vec3Position.z = fCameraDistance;
		vec3Position = Quaternion.AngleAxis( fRotate, Vector3.up) * vec3Position;
		return vec3Position;
	}

	// Get player Postion
	protected Vector3 GetPlayerPosition()
	{
		Vector3 vec3Temp = m_PlayerTransform.position;
		vec3Temp.y += m_CameraPosition.CharHeight;
	
		return vec3Temp;
	}

	// Get Max Speed
	protected float GetMaxSpeed( float fTime, float fDistance)
	{
		if ( 0 == fTime)
			fTime = 1.0f;

		return fDistance / fTime;
	}

	// Update ZoomIn
	public void UpdateZoomIn( float fDistance)
	{
		float fSpeed = fDistance * Time.deltaTime;
		CameraMgr.Instance.SetZoomData( CameraMgr.Instance.getZoomData + fSpeed);
		ResetCameraLocalPos();
	}

	public void UpdateZoomOut( float fDistance)
	{
		float fSpeed = fDistance * Time.deltaTime;
		CameraMgr.Instance.SetZoomData( CameraMgr.Instance.getZoomData - fSpeed);
		ResetCameraLocalPos();
	}

	// Update Normal
	protected void UpdateNormal()
	{
		if ( null == m_PlayerTransform || null == m_CameraPosition)
			return;

		transform.position = m_PlayerTransform.position + m_vec3CameraLocalPos;

		// char look pos
		Vector3 vec3CharLookPos = GetPlayerPosition();

		// set rotate
		Vector3 relativePos = vec3CharLookPos - transform.position;
		Quaternion rotation = Quaternion.LookRotation( relativePos);
		transform.rotation = rotation;
	}

	public void SetZoom( float fZoom)
	{
		m_vec3CameraLocalPos = m_vec3SourcePos - m_vec3MoveDirection * fZoom;
	}

	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
	}

	// Update is called once per frame
	void LateUpdate()
	{
		UpdateNormal();
	}
}
