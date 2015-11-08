using UnityEngine;
using System.Collections;


[ExecuteInEditMode]
public class CameraMgr : MonoBehaviour
{
	private static CameraMgr ms_kIstance = null;
	private static float m_MaxZoomInDistance = 24.0F;//19.5f;
	private static float m_MinZoomInDistance = 0.0F;

	public static CameraMgr Instance
	{
		get	{ return ms_kIstance; }
	}

	public static float MaxZoomInDistance
	{
		get	
		{ 
			return m_MaxZoomInDistance; 
		}
	}
	
	public static float MinZoomInDistance
	{
		get	
		{ 
			return m_MinZoomInDistance; 
		}		
	}

	public enum eCAMERA_STATE
	{
		START = 0,
		NORMAL,
		ZOOM_OUT,
	};

	private Camera m_UICamera = null; // ilmeda 20120418
	private Camera m_mainCamera = null;
	private Transform m_PlayerTransform = null;
	private CameraComponent m_CameraComponent = null;
	private CameraData[] m_CameraData = new CameraData[3];
	private float m_fZoomData = 0f;

	public float getZoomData
	{
		get	{ return m_fZoomData; }
	}

	public Camera UICamera
	{
		get { return m_UICamera; }
	}

	public void SetZoomData( float fdata)
	{
		if( MinZoomInDistance > fdata)
			fdata = MinZoomInDistance;

		if( m_MaxZoomInDistance < fdata)
			fdata = m_MaxZoomInDistance;

		m_fZoomData = fdata;
	}

	protected void InitCameraData()
	{
		// Default camera data
		CameraData cameradata = new CameraData();
		cameradata.CharHeight = 0.9f;
		cameradata.CameraHeight = 13f + 3.6f;
		cameradata.CameraDistance = -20f - 6F;
		cameradata.Fov = 20.0f;
		cameradata.YRotate = 0f;
		m_CameraData[( int)eCAMERA_STATE.NORMAL] = cameradata;
	}

	public bool AttachPlayerCamera( Transform playerTransform, float fTime)
	{
		if ( null == playerTransform)
		{
			Debug.LogError( "CameraMgr::AttachCharCamera() [ null == playerTransform ]");
			return false;
		}
		m_PlayerTransform = playerTransform;

		GameObject goMainCamera = GameObject.FindWithTag( "MainCamera");

		if( null == goMainCamera)
		{
			Debug.LogError( "CameraMgr::AttachCharCamera() [ Main camera gameobject not found ]");
			return false;
		}

		m_CameraComponent = goMainCamera.GetComponent< CameraComponent > ();
		if( null == m_CameraComponent)
			m_CameraComponent = goMainCamera.AddComponent< CameraComponent > ();

		m_mainCamera = goMainCamera.GetComponent<Camera>();
		if( null == m_mainCamera)
		{
			Debug.LogError( "CameraMgr::AttachCharCamera() [ Main camera not found ]");
			return false;
		}

		m_mainCamera.nearClipPlane = 1f;
		m_mainCamera.farClipPlane = 1000f;

		if( 0.0f >= fTime)
		{
			Debug.LogError( "CameraMgr::StartCameraZoom() [ 0.0f >= fTime] ");
			return false;
		}

		SetCameraData( m_CameraData[( int)eCAMERA_STATE.NORMAL], fTime);
		
		if( true == TerrainMgr.Instance.IsCurMapType( eMAP_TYPE.Pvp ) )
		{
			m_MinZoomInDistance = -8f;
			SetZoomData( m_MinZoomInDistance );		
			SetZoom( m_MinZoomInDistance );
		}
		else			
		{
			m_MinZoomInDistance = 0f;
			SetZoomData( getZoomData );		
			SetZoom( getZoomData );
		}

		return true;
	}

	public void SetZoomOutState( float fDistance)
	{
		if( null == m_CameraComponent)
			return;

		m_CameraComponent.UpdateZoomOut( fDistance);
	}

	public void SetZoomInState( float fDistance)
	{
		if( null == m_CameraComponent)
			return;

		m_CameraComponent.UpdateZoomIn( fDistance);
	}

	public void SetCameraData( CameraData SourceData, float fTime)
	{
		if( null == m_mainCamera)
		{
			Debug.LogError( "null == m_mainCamera");
			return;
		}

		if( null == m_CameraComponent)
		{
			Debug.LogError( "null == m_CameraComponent");
			return;
		}

		m_CameraComponent.Init( m_PlayerTransform, SourceData, fTime);
		m_mainCamera.fov = SourceData.Fov;
	}

	public void SetCurCameraData( CameraData data)
	{
		m_mainCamera.fov = data.Fov;
		//m_CameraComponent.SetCurCameraData( data);
	}

	public void SetZoom( float fZoom)
	{
		if( null == m_CameraComponent)
			return;

		m_CameraComponent.SetZoom ( fZoom);
	}

	// < ilmeda 20120418
	public Vector3 WorldToScreenPoint( Vector3 vWorldPos)
	{
		if( null == m_mainCamera)
			return Vector3.zero;

		return m_mainCamera.WorldToScreenPoint( vWorldPos);
	}

	public Vector3 ScreenPointToUIRay( Vector3 vScreenPos)
	{
		if( null == m_UICamera)
			return Vector3.zero;

		Ray ray = m_UICamera.ScreenPointToRay( vScreenPos);
		return ray.origin;
	}

	public Transform GetPlayerCharacterTransform()
	{
		return m_PlayerTransform;
	}

	public Vector3 ScreenToWorldPoint( Vector3 vScreenPos)
	{
		if( null == m_UICamera)
			return Vector3.zero;

		return m_UICamera.ScreenToWorldPoint( vScreenPos);
	}
	// ilmeda 20120418 >


	public Vector3 WorldToScreenPointByUICamera( Vector3 vWorldPos)
	{
		if( null == m_UICamera)
			return Vector3.zero;
		
		return m_UICamera.WorldToScreenPoint( vWorldPos);
	}
	// Awake
	void Awake()
	{
		ms_kIstance = this;
		InitCameraData();
		//m_fZoomData = CameraMgr.MaxZoomInDistance - ( CameraMgr.MaxZoomInDistance * 0.8f);
		m_fZoomData = 0;
	}

	// Use this for initialization
	void Start()
	{
		if( true == Application.isPlaying)
			m_UICamera = transform.parent.FindChild( "UICamera").GetComponent<Camera>(); // ilmeda 20120418
	}

	// Update is called once per frame
	void Update()
	{
	}
}
