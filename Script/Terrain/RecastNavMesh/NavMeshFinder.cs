using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class NavMeshFinder : MonoBehaviour 
{
	//---------------------------------------------------------------------
	/* Instance */
	//---------------------------------------------------------------------	
	
	private static NavMeshFinder instance = null;
	
	public static NavMeshFinder Instance
	{
		get { return instance; }
	}
	
	void Awake()
	{
		if( null != NavMeshFinder.Instance )
		{
			Debug.LogError("NavMeshFinder::Awake()[ null != NavMeshFinder.Instance ] ");
			return;
		}
		instance = this;
		
	}
	
	void Start()
    {    		
    }
	
	
	
	
	
	//---------------------------------------------------------------------
	/* Variable */
	//---------------------------------------------------------------------		
	private Dictionary<int, DetourStatNavMesh> m_NavMeshList = new Dictionary<int, DetourStatNavMesh>();
	private DetourStatNavMesh m_curNavMesh = null;	
	
	
	public Vector3 m_polyPickExt = new Vector3( 2.0f, 100.0f, 2.0f );
	const int MAX_POLYS = 256;	
	int m_npolys = 0;
	ushort[] m_polys = new ushort[MAX_POLYS];	
	int m_nstraightPath = 0;
	Vector3[] m_straightPath = new Vector3[MAX_POLYS];
	
	//---------------------------------------------------------------------
	/* Function */
	//---------------------------------------------------------------------	
		

	public bool LoadNavMesh( int iIndex )
	{		
		if (true == m_NavMeshList.ContainsKey(iIndex))
        {
			//Debug.LogError("NavMeshFinder::LoadNavMesh()[ true == m_NavMeshList.ContainsKey(iID) ] index  : " + iIndex );
			//Debug.Log("loaded map index");
			return true;
		}
		
		string strPath = "PathFinder/" + iIndex.ToString();
		try
		{
			
			TextAsset xmlText = Resources.Load(strPath) as TextAsset;			
			if( null == xmlText )
			{
				Debug.LogError("NavMeshFinder::LoadNavMesh()[ path : " + strPath );
				return false;
			}	
			DetourStatNavMesh nav = new DetourStatNavMesh();			
	
			if( false == nav.init( xmlText.bytes, xmlText.bytes.Length, true, iIndex ) )
			{
				Debug.LogError("NavMeshFinder::LoadNavMesh()[ nav.init() == false ] index : " + iIndex );
				return false;
			}
			
			m_NavMeshList.Add( iIndex, nav );
		}
		catch(System.Exception e)
		{
			Debug.LogError("NavMeshFinder::LoadNavMesh() : " + e.ToString() );
		}
		
		
		
		return true;		
	}
	
	public void SetNavMesh( int iIndex )
	{
		if( null != m_curNavMesh )
		{
			if( m_curNavMesh.index == iIndex )
			{
				Debug.LogWarning("NavMeshFinder::SetNavMesh()[ m_curNavMesh.index == iIndex ] index : " + iIndex );
				return;
			}
		}
		
		if (false == m_NavMeshList.ContainsKey(iIndex))
        {
			Debug.LogError("NavMeshFinder::SetNavMesh()[ false == m_NavMeshList.ContainsKey(iIndex) ] index  : " + iIndex );
			return;
		}
		
		m_curNavMesh = m_NavMeshList[iIndex];	
		if( null == m_curNavMesh )
		{
			AsUtil.ShutDown("NavMeshFinder::SetNavMesh()[ null == m_curNavMesh] index : " + iIndex );
		}
	}	
	
	public Vector3[] PathFind_type1(Vector3 vec3StartPos, Vector3 vec3EndPos)
	{
		if( null == m_curNavMesh )
		{
			Debug.LogError("NavMeshFinder::PathFind() [ null == m_curNavMesh ]");
			return null;
		}
		
		Vector3 spos = vec3StartPos;
		Vector3 epos = vec3EndPos;
			
		ushort startRef = m_curNavMesh.findNearestPoly(spos, m_polyPickExt);
		ushort endRef = m_curNavMesh.findNearestPoly(epos, m_polyPickExt);	
		
		
		if ( 0!=startRef && 0!=endRef && IsCell(epos) )
		{
			float t = 0;			
			m_npolys = m_curNavMesh.raycast(startRef, spos, epos, ref t, m_polys, MAX_POLYS );
			if (t < 1)
			{
				m_npolys = m_curNavMesh.findPath(startRef, endRef, spos, epos, m_polys, MAX_POLYS);
				if (0 != m_npolys )
				{
					m_nstraightPath = m_curNavMesh.findStraightPath(spos, epos, m_polys, m_npolys, m_straightPath, MAX_POLYS);
					
					Vector3[] path = new Vector3[ m_nstraightPath ];				
					for (int i = 0; i < m_nstraightPath; ++i)
					{
						path[i] = m_straightPath[i];						
					}					
					return path;
				}
			}
			else
			{			
				Vector3[] path = new Vector3[ 1 ];	
				path[0] = epos;			
				return path;
			}
		}
		else if ( 0!=startRef )
		{
			float t = 0;			
			m_npolys = m_curNavMesh.raycast(startRef, spos, epos, ref t, m_polys, MAX_POLYS );
			if (t < 1)
			{
				Vector3[] path = new Vector3[ 1 ];								
				
				path[0].x = spos.x + (epos.x - spos.x) * (t*0.9f);
				path[0].y = spos.y + (epos.y - spos.y) * (t*0.9f);
				path[0].z = spos.z + (epos.z - spos.z) * (t*0.9f);			
				
				return path;
			}
			else
			{			
				Vector3[] path = new Vector3[ 1 ];	
				path[0] = epos;			
				return path;
			}
		}
			
		return null;		
	}
	
	public Vector3[] PathFind( Vector3 vec3StartPos, Vector3 vec3EndPos )
	{		
		if( null == m_curNavMesh )
		{
			Debug.LogError("NavMeshFinder::PathFind() [ null == m_curNavMesh ]");
			return null;
		}
		
		Vector3 spos = vec3StartPos;
		Vector3 epos = vec3EndPos;
			
		ushort startRef = m_curNavMesh.findNearestPoly(spos, m_polyPickExt);
		ushort endRef = m_curNavMesh.findNearestPoly(epos, m_polyPickExt);	
		
		
		if (0!=startRef && 0!=endRef)
		{
			float t = 0;			
			m_npolys = m_curNavMesh.raycast(startRef, spos, epos, ref t, m_polys, MAX_POLYS );
			if (t < 1)
			{
				m_npolys = m_curNavMesh.findPath(startRef, endRef, spos, epos, m_polys, MAX_POLYS);
				if (0 != m_npolys )
				{
					m_nstraightPath = m_curNavMesh.findStraightPath(spos, epos, m_polys, m_npolys, m_straightPath, MAX_POLYS);
					
					Vector3[] path = new Vector3[ m_nstraightPath ];				
					for (int i = 0; i < m_nstraightPath; ++i)
					{
						path[i] = m_straightPath[i];						
					}					
					return path;
				}
			}
			else
			{			
				Vector3[] path = new Vector3[ 1 ];	
				path[0] = epos;			
				return path;
			}
		}
		else if ( 0!=startRef )
		{
			float t = 0;			
			m_npolys = m_curNavMesh.raycast(startRef, spos, epos, ref t, m_polys, MAX_POLYS );
			if (t < 1)
			{
				Vector3[] path = new Vector3[ 1 ];								
				
				path[0].x = spos.x + (epos.x - spos.x) * (t*0.9f);
				path[0].y = spos.y + (epos.y - spos.y) * (t*0.9f);
				path[0].z = spos.z + (epos.z - spos.z) * (t*0.9f);			
				
				return path;
			}
			else
			{			
				Vector3[] path = new Vector3[ 1 ];	
				path[0] = epos;			
				return path;
			}
		}
			
		return null;	
	}
	
	
	public bool IsCell( Vector3 vec3Position )
	{
		if( null == m_curNavMesh )
		{
			Debug.LogError("NavMeshFinder::IsCell() [ null == m_curNavMesh ]");
			return false;
		}
		
		
		return m_curNavMesh.IsCell( vec3Position, m_polyPickExt );
	}	
	
	
	void OnDrawGizmos () 
	{	
		if( null == m_curNavMesh )
			return;
		
		for (int i = 0; i < m_curNavMesh.getPolyDetailCount(); ++i)
		{
			dtStatPoly p = m_curNavMesh.getPoly(i);
			dtStatPolyDetail pd = m_curNavMesh.getPolyDetail(i);
			
			
			Gizmos.color = Color.red;		
				
			for (int j = 0; j < pd.ntris; ++j)
			{					
				dtTriIdx t = m_curNavMesh.getDetailTri(pd.tbase+j);				
				Vector3 []v = new Vector3[3];				
				for (int k = 0; k < 3; ++k) 
				{
					if (t.datas[k] < p.nv)
					{
						v[k] = m_curNavMesh.getVertex(p.v[t.datas[k]]);
					}
					else
					{
						v[k] = m_curNavMesh.getDetailVertex(pd.vbase+(t.datas[k]-p.nv));
					}
				}
				
				Gizmos.DrawLine(v[0], v[1]);
				Gizmos.DrawLine(v[1], v[2]);
				Gizmos.DrawLine(v[2], v[0]);
			}
		}		
	}	
}