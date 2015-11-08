using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[AddComponentMenu( "ArkSphere/AsTrailEffect")]
public class AsTrailEffect : MonoBehaviour 
{
	
	public Dummy_Type m_DummyType   = Dummy_Type.DummyWeaponCenter_R;
	public float   m_HalfHeight  = 1.0f;
	public Texture m_Texture = null;
	
	public enum eDirection
	{
		eForworad  = 0,
		eUp,
		eRight,
	}
	public eDirection m_eDirection = eDirection.eForworad;
	public int   m_SlerpCount = 5;
	public float m_emitTime = 0.5f;
	public float EmitTime
	{
		get{return m_emitTime;}
		set{m_emitTime = value;}
	}
	
	
	private float m_curEemitTime = 0.0f;
	public float CurEmitTime
	{
		get{return m_curEemitTime;}
		set{m_curEemitTime = value;}
	}
	
	
	
	private bool m_Play = true;	
	
	private Transform m_TrailTranform = null;
	
	
	private float m_SpeedRatio = 1.0f;
	public float SpeedRatio
	{
		get{return m_SpeedRatio;}
		set{
			m_SpeedRatio = value;
			if(m_SpeedRatio != 0.0f)
				m_eraserTime = m_eraserTime / m_SpeedRatio;
		   }
	}
	public float m_eraserTime = 0.2f;   

	private float m_maxInterpolationDistance = 0.02f;
	
	List<Point> m_pointList = new List<Point>();

	private Color startColor = Color.white;
	private Color endColor = new Color(1f, 1f, 1f, 0f );
	
	MeshFilter   m_mf;
	MeshRenderer m_mr;
	

    public class Point
    {
        public float timeCreated  = 0.00f;     
		public Vector3 direction  = Vector3.zero;
		public Vector3 position   = Vector3.zero;
	
    }

    
	public void SetTexture(Texture _Texture )
	{
		if(m_mr != null) 
			m_mr.material.mainTexture = _Texture;
	
	}
	
	
	public void SetLifeTime(float fTime)
	{
		if(fTime == 0) return;
		if(m_TrailTranform == null)return;
		EmitTime = fTime;
		
		m_Play   = true;
	}
    void OnEnable()
    {
		if(m_SpeedRatio != 0.0f)
			m_curEemitTime = ( EmitTime / m_SpeedRatio);
		else
			m_curEemitTime = EmitTime;
		
		m_curEemitTime = EmitTime;
    
      
		
		
		 if (GetComponent<MeshFilter>() == null)
           m_mf = gameObject.AddComponent<MeshFilter>();
		else
		   m_mf =  gameObject.GetComponent<MeshFilter>();
			
	     if (GetComponent<MeshRenderer>() == null)
	        m_mr =gameObject.AddComponent<MeshRenderer>();
		 else
			m_mr =	gameObject.GetComponent<MeshRenderer>();	
		 
	// if(m_mr.material == null)
	  m_mr.material = new Material (Shader.Find ("Mobile/Particles/Additive"));
 
	  m_mr.material.mainTexture = m_Texture;
	
 
      

    }
	
	public void Stop()
    {
		m_Play = false;		
		m_curEemitTime = -1.0f;
	}
	
	Vector3 vecDir;
	public void Play(Transform parent, float speedRatio )
    {
		
		
						
		m_TrailTranform = ResourceLoad.SearchHierarchyTransform(parent, m_DummyType.ToString()) ;
		if(m_TrailTranform == null)
		{
			Debug.LogError("TrailTranform not found!!!");
			return;
		}			
		
		m_Play = true;		
		SpeedRatio = speedRatio;
		
		if(m_SpeedRatio != 0.0f)
			m_curEemitTime = ( EmitTime / m_SpeedRatio);
		else
			m_curEemitTime = EmitTime;
		
	
    
		
		
		 if (GetComponent<MeshFilter>() == null)
           m_mf = gameObject.AddComponent<MeshFilter>();
		else
		   m_mf =  gameObject.GetComponent<MeshFilter>();
			
	     if (GetComponent<MeshRenderer>() == null)
	        m_mr =gameObject.AddComponent<MeshRenderer>();
		 else
			m_mr =	gameObject.GetComponent<MeshRenderer>();	
		 
	     m_mr.material = null;
		 m_mr.material = new Material (Shader.Find ("Mobile/Particles/Additive")); 
	  	 m_mr.material.mainTexture = m_Texture;    
		
	
    }
	
    void OnDisable()
    {
     
    }
	
    void LateUpdate()
    {
        if (m_Play && m_curEemitTime != 0)
        {
		
            m_curEemitTime -= Time.deltaTime ;
            if (m_curEemitTime == 0) 
				m_curEemitTime = -1.0f;
            if (m_curEemitTime < 0) 
			{			
				m_curEemitTime = EmitTime;
				m_Play = false;
			}
        }
		
		m_mf.mesh.Clear();
        if (!m_Play && m_pointList.Count == 0)
		{			
        	return;
		}
        // early out if there is no camera
        if (!Camera.main) return;
		if (m_TrailTranform == null ) return;
    	
		MakeTrail();
	}	
		
	void SlerpTrail()
	{
		float theDistance = 0.0f;
	
		theDistance =  (m_pointList[0].position - m_TrailTranform.position).sqrMagnitude; 
		
		if(theDistance > m_maxInterpolationDistance )
		{		
			Vector3 vec = m_pointList[0].position;				
			Vector3 dir = m_pointList[0].direction;
			float t = (float)(1.0f / m_SlerpCount); 
			for(int i = 0; i < m_SlerpCount; ++i)
			{
				
				Point ip = new Point();			
			    ip.position =  Vector3.Slerp(vec, m_TrailTranform.position,(i+1) * t);
			 
			  
				switch(m_eDirection)
				{
					case eDirection.eForworad: ip.direction = Vector3.Slerp(dir,  m_TrailTranform.forward,(i+1) * t);	break;			
					case eDirection.eRight:    ip.direction = Vector3.Slerp(dir,  m_TrailTranform.right,(i+1) * t);	break;			
					case eDirection.eUp:       ip.direction = Vector3.Slerp(dir,  m_TrailTranform.up,(i+1) * t);	    break;			
				}					
			    ip.timeCreated = Time.time;           
				m_pointList.Insert( 0, ip );            	  					
			}		
	
		}
		
	
	}	
	void MakeTrail()
	{
        // if we have moved enough, create a new vertex and make sure we rebuild the mesh		    
        if (m_Play)
        {		
            if (m_pointList.Count == 0 )				
            {							
                Point p       = new Point();
                p.position    =  m_TrailTranform.position; 
				switch(m_eDirection)
				{
					case eDirection.eForworad: p.direction = m_TrailTranform.forward;	break;			
					case eDirection.eRight:    p.direction = m_TrailTranform.right;	break;			
					case eDirection.eUp:       p.direction = m_TrailTranform.up;	break;			
				}					
                p.timeCreated = Time.time;           
				m_pointList.Insert( 0, p );				
			   
               
            }          
						
			SlerpTrail();
		}
	
		// Remove old pointList
		for( int i = 0; i < m_pointList.Count; )
		{
			if( Time.time > m_pointList[i].timeCreated + m_eraserTime )
			{
				m_pointList.Remove( m_pointList[i++] );
			}
			else
				++i;
		}       

        if (m_pointList.Count > 1)
        {
            Vector3[] newVertices = new Vector3[m_pointList.Count * 2];
            Vector2[] newUV = new Vector2[m_pointList.Count * 2];
            int[] newTriangles = new int[(m_pointList.Count - 1) * 6];
            Color[] newcolors = new Color[m_pointList.Count * 2];

            int i = 0;		
            foreach (Point p in m_pointList)
            {			
				float u = 0.0f;		
				if (i != 0)
				u = Mathf.Clamp01 ((Time.time - p.timeCreated) / m_eraserTime);
				
				
                newVertices[i * 2] = p.position + (p.direction *-1) * m_HalfHeight;
				//newVertices[i * 2] = p.position;
                newVertices[(i * 2) + 1] =  p.position + p.direction * m_HalfHeight  ;
			
				
	            // fade colors out over time
				Color interpolateColor = Color.Lerp( startColor, endColor, u );
				newcolors[ i * 2 + 0] = interpolateColor;
				newcolors[ i * 2 + 1] = interpolateColor;

                newUV[i * 2] = new Vector2(u  , 0);
                newUV[(i * 2) + 1] = new Vector2(u  , 1);
			
                if (i > 0  )
                {					
					newTriangles[(i - 1) * 6] = (i * 2) - 2;
                    newTriangles[((i - 1) * 6) + 1] = (i * 2) - 1;
                    newTriangles[((i - 1) * 6) + 2] = i * 2;

                    newTriangles[((i - 1) * 6) + 3] = (i * 2) + 1;
                    newTriangles[((i - 1) * 6) + 4] = i * 2;
                    newTriangles[((i - 1) * 6) + 5] = (i * 2) - 1;
					
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

