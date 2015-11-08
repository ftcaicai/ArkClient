using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;



//[ExecuteInEditMode]
public class AsMeshEffect : MonoBehaviour
{
	public enum EffectBillBoardType
	{
		EBT_NONE = 0,
		EBT_NORMAL,
		EBT_VERTICAL,
		EBT_HORIZONTAL,
	}

    public bool m_Play = true;
	/*public bool play//
	{
		get{return m_play;}	
		set{m_play = value;}
	}*/
	public EffectBillBoardType m_billboardType = EffectBillBoardType.EBT_NONE;
	Vector3 cameraPos = Vector3.zero;
	private float m_startSize = 1f;
	public float StartSize
	{		
        get { return m_startSize; }
        set { m_startSize = value; }
	}
	
    [Serializable]
    public class AsMeshEffect_Transform
    {
        [HideInInspector]
        public bool m_Init = false;
        [SerializeField]
        public float m_Time;
        [SerializeField]
        public Vector3 m_Scale = Vector2.one;
        [SerializeField]
        public Vector3 m_Position;
        [SerializeField]
        public Vector3 m_Rotation;
        [SerializeField]
        public Color m_Color;

        public void Init()
        {
            m_Init = true;
            m_Time = 0f;
            m_Scale = Vector3.one;
            m_Position = Vector3.zero;
            m_Rotation = Vector3.zero;
            m_Color = Color.white;
        }
    }


    private MeshFilter m_MeshFilter = null;
    private Mesh m_Mesh = null;
	public Mesh mesh {
		get{return m_Mesh;}
		set{m_Mesh = value;}
	}

    private Vector3[] m_verts;
    private Color[] m_vertsColors;



    public List<AsMeshEffect_Transform> m_TransformLayer = new List<AsMeshEffect_Transform>();

    public float m_LifeTime = 0f;
    public float m_CycleTime = 1.00f;
    float m_CreateTime = 0.0f;
    float m_fProgress = 0.0f;


    ///////////////////////////////////////////////////////////////
    public bool m_random = false;

    public int m_uvTieX = 1;
    public int m_uvTieY = 1;
    public int m_cycles = 1;

    private float m_fps = 10f;

    private Vector2 m_size;
    private int m_lastIndex = 0;

    private float m_changeTime = 0f;
    ///////////////////////////////////////////////////////////////


    // Use this for initialization
    void Start()
    {

        m_CreateTime = Time.time;

        if (m_MeshFilter == null)
        {
            m_MeshFilter = gameObject.GetComponent<MeshFilter>();
        }

        if (m_Mesh == null)
        {
            if (m_MeshFilter != null)
                m_Mesh = m_MeshFilter.mesh;
            if (m_Mesh != null)
            {
                m_verts = m_Mesh.vertices;
                m_vertsColors = new Color[m_verts.Length];
            }
        }
        if (m_Mesh != null)
        {
            m_verts = m_Mesh.vertices;
            m_vertsColors = new Color[m_verts.Length];
        }

        m_size = new Vector2(1.0f / m_uvTieX, 1.0f / m_uvTieY);

        m_changeTime = 0;
        m_lastIndex = 0;

        if (renderer == null)
            enabled = false;


    }

    void MeshEffectUpdate()
    {
        if (m_Mesh == null) return;
        if (m_LifeTime != 0 && Time.time - m_CreateTime >= m_LifeTime)
        {
            m_Play = false;
            //		enabled = false;
            m_MeshFilter.renderer.enabled = false;

            return;
        }
        if (m_TransformLayer.Count == 0) return;


        m_fProgress = (float)((Time.time - m_CreateTime) % m_CycleTime) / (float)m_CycleTime;
        float fProgress = 0.0f;
        Color color = Color.white;
        Vector3 _Scale = Vector3.one;
        Vector3 _Position = Vector3.zero;
        Vector3 _Rotation = Vector3.zero;
        float fLayerTerm = 0f;
        float fCycleTerm = 0f;
        for (int i = 0; i < m_TransformLayer.Count; i++)
        {

            if (m_TransformLayer.Count - 1 == i) break;
            if (m_TransformLayer[i].m_Time < m_fProgress && m_TransformLayer[i + 1].m_Time > m_fProgress)
            {

                fLayerTerm = m_TransformLayer[i + 1].m_Time - m_TransformLayer[i].m_Time;
                fCycleTerm = m_fProgress - m_TransformLayer[i].m_Time;
                fProgress = fCycleTerm / fLayerTerm;


                color = Color.Lerp(m_TransformLayer[i].m_Color, m_TransformLayer[i + 1].m_Color, fProgress);

                _Scale = Vector3.Lerp(m_TransformLayer[i].m_Scale, m_TransformLayer[i + 1].m_Scale, fProgress);
                _Position = Vector3.Lerp(m_TransformLayer[i].m_Position, m_TransformLayer[i + 1].m_Position, fProgress);
                _Rotation = Vector3.Lerp(m_TransformLayer[i].m_Rotation, m_TransformLayer[i + 1].m_Rotation, fProgress);
            }

        }

        //	Debug.Log("fProgress" + fProgress + "size y" +Scale.x + "size y"+ Scale.y);
        Vector3[] verts = new Vector3[m_verts.Length];
        for (int i = 0; i < m_verts.Length; i++)
        {
            m_vertsColors[i] = color;

            verts[i].x = m_verts[i].x * _Scale.x * m_startSize;
            verts[i].y = m_verts[i].y * _Scale.y * m_startSize;
            verts[i].z = m_verts[i].z * _Scale.z * m_startSize;

        }
        Quaternion rotate = Quaternion.identity;
        rotate.eulerAngles = _Rotation;


        transform.localRotation = rotate;
        transform.localPosition = _Position;

        if (Application.isPlaying)
        {
            m_Mesh.colors = m_vertsColors;
            m_Mesh.vertices = verts; //Scale
        }




    }

    void SetSpriteSheet(int index)
    {
        // split into horizontal and vertical index
        int uIndex = index % m_uvTieX;
        int vIndex = index / m_uvTieX;

        // build offset
        // v coordinate is the bottom of the image in opengl so we need to invert.
        Vector2 offset = new Vector2(uIndex * m_size.x, 1.0f - m_size.y - vIndex * m_size.y);

        renderer.material.SetTextureOffset("_MainTex", offset);
        renderer.material.SetTextureScale("_MainTex", m_size);

        // m_lastIndex = index;
    }


    // Update is called once per frame
    void Update()
    {
        if (m_Play)
        {
            if (!m_MeshFilter.renderer.enabled)
            {
                m_CreateTime = Time.time;
                m_MeshFilter.renderer.enabled = true;
                m_changeTime = 0;
                m_lastIndex = 0;
            }

        }
        else
        {
            m_MeshFilter.renderer.enabled = false;
        }




        m_fps = m_CycleTime / (m_uvTieX * m_uvTieY * m_cycles);
        int total_index = (m_uvTieX * m_uvTieY) - 1;
        int index = 0;
        if (Time.time - m_changeTime > m_fps)
        {
            m_changeTime = Time.time;
            if (m_random)
            {
                // random index
                index = UnityEngine.Random.Range(0, total_index);
            }
            else
            {
                index = m_lastIndex++;
                if (index > total_index)
                {
                    index = 0;
                    m_lastIndex = 0;
                }
            }
            SetSpriteSheet(index);
        }


        MeshEffectUpdate();
		//
		cameraPos = Camera.main.transform.position;
		switch( m_billboardType )
		{
		case EffectBillBoardType.EBT_NORMAL :
			BillBoardNormal();
			break;
		case EffectBillBoardType.EBT_VERTICAL :
			BillBoardVertical();
			break;
		case EffectBillBoardType.EBT_HORIZONTAL :
			BillBoardHorizontal();
			break;
				
			
		}
    }


	
	

	void BillBoardNormal()
	{
		transform.LookAt( cameraPos );
	}
	
	void BillBoardVertical()
	{
		transform.LookAt( new Vector3( cameraPos.x, transform.position.y, cameraPos.z) );
	}
	
	void BillBoardHorizontal()
	{
		transform.LookAt( new Vector3( transform.position.x, cameraPos.y, transform.position.z) );
	}
}
