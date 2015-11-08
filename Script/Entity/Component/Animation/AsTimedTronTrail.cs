using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class AsTimedTronTrail : MonoBehaviour
{
	const float m_maxInterpolationDistance = 0.5f;
	public bool m_emit = false;
	
	private float m_emitTime = 0.00f;
	public float emitTime
	{
		get	{ return m_emitTime; }
		set	{ m_emitTime = value; }
	}
	private float m_height = 1.5f; // Size
	public float height
	{
		get	{ return m_height; }
		set	{ m_height = value; }
	}
	public float m_eraserTime = 0.8f;

	public float m_uvLengthScale = 0.01f; //Texture uv scale

	public float m_minVertexDistance = 0.10f;
	public float m_maxVertexDistance = 10.00f;
	public float m_maxAngle = 3.00f;

	private List<Point> m_pointList = new List<Point>();

	private Vector3 m_lastPositionUp;
	private Vector3 m_lastPositionDown;

	public Transform m_WeaponTrailUp = null;
	public Transform m_WeaponTrailDown = null;

	MeshFilter m_mf;
	MeshRenderer m_mr;

	public class Point
	{
		public float timeCreated = 0.00f;
		public Vector3 position;
		public Vector3 positionDown;
	}

	public void SetTexture( Texture _Texture)
	{
		if( m_mr != null)
			m_mr.material.mainTexture = _Texture;

	}

	public void SetLifeTime( float fTime)
	{
		if( fTime == 0)
			return;

		if( m_WeaponTrailUp == null || m_WeaponTrailDown == null)
			return;

		emitTime = fTime;
		m_lastPositionUp = m_WeaponTrailUp.position;//transform.position;
		m_emit = true;
	}

	void OnEnable()
	{
		m_lastPositionUp = transform.position;

		if( GetComponent<MeshFilter>() == null)
			m_mf = gameObject.AddComponent<MeshFilter>();

		if( GetComponent<MeshRenderer>() == null)
			m_mr =gameObject.AddComponent<MeshRenderer>();

		if( m_mr == null)
			m_mr = gameObject.GetComponent<MeshRenderer>();

		m_mr.material = new Material( Shader.Find ( "Mobile/Particles/Additive"));
	}

	void OnDisable()
	{
		// Destroy( m_trailgo);
	}

	void LastPositionUpdate()
	{
		( (Point)m_pointList[m_pointList.Count - 1]).position = m_WeaponTrailUp.position;
		( (Point)m_pointList[m_pointList.Count - 1]).positionDown = m_WeaponTrailDown.position;

		( (Point)m_pointList[m_pointList.Count - 1]).timeCreated = Time.time;

		m_lastPositionUp = m_WeaponTrailUp.position;
		m_lastPositionDown = m_WeaponTrailDown.position;
	}

	void Update()
	{
		if( m_emit && m_emitTime != 0)
		{
			m_emitTime -= Time.deltaTime;
			if( m_emitTime == 0)
				m_emitTime = -1.0f;

			if( m_emitTime < 0)
				m_emit = false;
		}

		if( !m_emit && m_pointList.Count == 0)
			return;

		// early out if there is no camera
		if( !Camera.main)
			return;

		if( m_WeaponTrailUp == null || m_WeaponTrailDown == null)
			return;

		// if we have moved enough, create a new vertex and make sure we rebuild the mesh
		float theDistance = ( m_lastPositionUp - m_WeaponTrailUp.position).magnitude;
		if( m_emit)
		{
			if( theDistance > m_minVertexDistance)
			{
				bool make = false;
				if( m_pointList.Count < 3)
				{
					make = true;
				}
				else
				{
					Vector3 l1 = ( (Point)m_pointList[m_pointList.Count - 2]).position - ( (Point)m_pointList[m_pointList.Count - 3]).position;
					Vector3 l2 = ( (Point)m_pointList[m_pointList.Count - 1]).position - ( (Point)m_pointList[m_pointList.Count - 2]).position;
					if( Vector3.Angle( l1, l2) > m_maxAngle || theDistance > m_maxVertexDistance)
						make = true;
				}

				if( make)
				{
					if( theDistance > m_maxInterpolationDistance)
					{
						for( int i = 0; i < 5; ++i)
						{
							Point ip = new Point();

							ip.position = Vector3.Slerp( m_lastPositionUp, m_WeaponTrailUp.position, ( i+1) * 0.2f);
							ip.positionDown = Vector3.Slerp( m_lastPositionDown, m_WeaponTrailDown.position, ( i + 1) * 0.2f);

							ip.timeCreated = Time.time;
							m_pointList.Add( ip);
						}
					}
					else
					{
						Point p = new Point();
						p.position = m_WeaponTrailUp.position;
						p.positionDown = m_WeaponTrailDown.position;
						p.timeCreated = Time.time;
						m_pointList.Add( p);
					}

					m_lastPositionUp = m_WeaponTrailUp.position;
					m_lastPositionDown = m_WeaponTrailDown.position;
				}
				else
				{
					LastPositionUpdate();
				}
			}
			else if( m_pointList.Count > 0)
			{
				LastPositionUpdate();
			}
		}

		List<Point> remove = new List<Point>();

		for(int i=0; i<m_pointList.Count; ++i)
		{
			// cull old m_pointList first
			if( Time.time - m_pointList[i].timeCreated > m_eraserTime)
				remove.Add( m_pointList[i]);
		}

		for(int i=0; i<remove.Count; ++i)
			m_pointList.Remove( remove[i]);

		remove.Clear();

		if( m_pointList.Count > 1)
		{
			Vector3[] newVertices = new Vector3[m_pointList.Count * 2];
			Vector2[] newUV = new Vector2[m_pointList.Count * 2];
			int[] newTriangles = new int[( m_pointList.Count - 1) * 6];
			Color[] newcolors = new Color[m_pointList.Count * 2];

			int i = 0;
			float curDistance = 0.00f;

			Point p;
			for(int j=0; j<m_pointList.Count; ++j)
			{
				p = m_pointList[j];

				float time = ( Time.time - p.timeCreated) / m_eraserTime;
				
				Color color = Color.Lerp( Color.white, Color.clear, time);
				
				newVertices[i * 2] = p.positionDown ;
				newVertices[( i * 2) + 1] = p.position ;
				
				newcolors[i * 2] = newcolors[( i * 2) + 1] = color;
				
				newUV[i * 2] = new Vector2( curDistance * m_uvLengthScale, 0);
				newUV[( i * 2) + 1] = new Vector2( curDistance * m_uvLengthScale, 1);
				
				if( i > 0)
				{
					curDistance += ( p.position - ( (Point)m_pointList[i - 1]).position).sqrMagnitude;

					newTriangles[( i - 1) * 6] = ( i * 2) - 2;
					newTriangles[( ( i - 1) * 6) + 1] = ( i * 2) - 1;
					newTriangles[( ( i - 1) * 6) + 2] = i * 2;
					
					newTriangles[( ( i - 1) * 6) + 3] = ( i * 2) + 1;
					newTriangles[( ( i - 1) * 6) + 4] = i * 2;
					newTriangles[( ( i - 1) * 6) + 5] = ( i * 2) - 1;
				}

				i++;
			}
			
			m_mf.mesh.Clear();
			m_mf.mesh.vertices = newVertices;
			m_mf.mesh.colors = newcolors;
			m_mf.mesh.uv = newUV;
			m_mf.mesh.triangles = newTriangles;
		}
	}
}
