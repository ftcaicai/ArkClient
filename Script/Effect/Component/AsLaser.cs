using UnityEngine;
using System.Collections;

public class AsLaser : MonoBehaviour {
	public Transform m_target;
	public Transform m_start = null;
	AsMeshEffect m_MeshEffect;
	
	
	Vector3[] m_verts;
	
	// Use this for initialization
	void Start () {
	
		m_MeshEffect = gameObject.GetComponent<AsMeshEffect>();
	
	}
	
	void SetPlanVertex(float fDist)
	{
		Vector3[] verts = new Vector3[8];
	
		verts[0] =  new Vector3( 0.0f,-0.5f, 0.5f+fDist); 
		verts[1] =  new Vector3( 0.0f, 0.5f, 0.5f+fDist); 		
		verts[2] =  new Vector3( 0.0f,-0.5f,-0.5f);
		verts[3] =  new Vector3( 0.0f, 0.5f,-0.5f);
		verts[4] =  new Vector3( 0.5f, 0.0f, 0.5f+fDist); 
		verts[5] =  new Vector3(-0.5f, 0.0f, 0.5f+fDist); 		
		verts[6] =  new Vector3( 0.5f, 0.0f,-0.5f);
		verts[7] =  new Vector3(-0.5f, 0.0f,-0.5f);			
			
	
        m_MeshEffect.mesh.vertices = verts; //
	
    
	}
	// Update is called once per frame
	void Update () {
		if(m_target == null ||m_start == null) return;
		
		if(m_MeshEffect == null)
		{
			m_MeshEffect = gameObject.GetComponent<AsMeshEffect>();
			
		}
	
		
		transform.parent = null;
		transform.position = m_start.position;		
		transform.rotation =  Quaternion.identity;
		
		float fDist = 	Vector3.Distance (m_target.position, transform.position);
	
		if(m_MeshEffect.m_Play)
			SetPlanVertex(fDist);
		
		
		gameObject.transform.LookAt(m_target.position);
	
	}
}
