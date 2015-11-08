using UnityEngine;
using System.Collections;

public class AsPortraitCameraPos : MonoBehaviour 
{	
	private static AsPortraitCameraPos m_this = null;	
	private Transform m_targetTransform = null; 		
		
	public static void SetTargetTransform( Transform transform )
	{
		if( null == m_this )
		{
			Debug.LogError( "this == null" );
			return;
		}
		
		if( null == transform )
		{
			Debug.LogError(" transform == null ");
			return;
		}
				
				
		m_this.m_targetTransform = transform.FindChild("Model/Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 Head/camera_dunny" );
		if( null == m_this.m_targetTransform )
		{
			Debug.LogError(" camera_dunny == null ");
			return;
		}		
	}
	
		
	void Awake()
	{
		m_this = this;
	}
	
	// Use this for initialization
	void Start () 
	{
		transform.parent = m_targetTransform;
		transform.localRotation = Quaternion.AngleAxis( 90.0f, Vector3.up );
		transform.localPosition = Vector3.zero;
	}	
	
	// Update is called once per frame
	void Update () 
	{
		
	}
}
