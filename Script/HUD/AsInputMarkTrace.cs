using UnityEngine;
using System.Collections;

public class AsInputMarkTrace : MonoBehaviour
{
	public Color c1 = Color.yellow;
	public Color c2 = Color.red;
	public int lengthOfLineRenderer = 20;
	public float fStartWidth = 0.1f;
	public float fEndWidth = 3.0f;
	public Material material;
	
	private LineRenderer m_LineLenderer;
	private Vector3[] m_vPos;
	private int m_nTraceCnt = 0;
	private bool m_bFirst = true;
	private Vector3 m_vEndPos;
	private int m_nTraceFinishCnt = 0;
	private bool m_bActive = false;
	private float m_fTimeBuf = 0.0f;
	
	void Start()
	{
		m_vPos = new Vector3[lengthOfLineRenderer];
		
		if( null == material)
			material = new Material( Shader.Find( "Particles/Additive"));
		
		m_LineLenderer = gameObject.AddComponent<LineRenderer>();
		m_LineLenderer.material = material;
		m_LineLenderer.SetColors( c1, c2);
		m_LineLenderer.SetWidth( fStartWidth, fEndWidth);
		m_LineLenderer.SetVertexCount( lengthOfLineRenderer);
	}
	
	void Update()
	{
		if( m_bFirst)
		{
			if( m_nTraceFinishCnt > 0)
			{
				_SetTracePos( m_vEndPos);
				m_nTraceFinishCnt--;
			}
			else
				m_bActive = false;
		}
		
		if( true == m_bActive)
		{
			//LineRenderer lineRenderer = GetComponent<LineRenderer>();
			for( int i=0 ; i<lengthOfLineRenderer ; i++)
				m_LineLenderer.SetPosition(i, m_vPos[i]);
		}
	}

	public void EndTrace()
	{
		m_bFirst = true;
		m_nTraceFinishCnt = lengthOfLineRenderer;
	}

	public void SetTracePos(Vector3 vScreenPos)
	{
		if( Time.time - m_fTimeBuf > 0.016f)
			m_fTimeBuf = Time.time;
		else
			return;
		
		if( true == m_bFirst)
		{
			_ResetTracePos();
			m_bFirst = false;
			m_bActive = true;
		}
		
		_SetTracePos( vScreenPos);
	}
	
	public bool isActive()
	{
		return m_bActive;
	}
	
	// < private
	private void _ResetTracePos()
	{
		//LineRenderer lineRenderer = GetComponent<LineRenderer>();
		for( int i = 0; i < lengthOfLineRenderer; i++)
		{
			m_vPos[i] = Vector3.zero;
			m_LineLenderer.SetPosition(i, m_vPos[i]);
		}
		
		m_nTraceCnt = 0;
	}
	
	private void _SetTracePos(Vector3 vScreenPos)
	{
		Vector3 vRes = CameraMgr.Instance.ScreenPointToUIRay( vScreenPos);
		vRes.z = 2.0f;

		m_vPos[m_nTraceCnt] = vRes;
		m_nTraceCnt++;
		
		for( int i = m_nTraceCnt; i < lengthOfLineRenderer; i++)
			m_vPos[i] = vRes;
		
		if( lengthOfLineRenderer <= m_nTraceCnt)
		{
			for( int k = 0; k < lengthOfLineRenderer-1; k++)
				m_vPos[k] = m_vPos[k+1];
			
			m_nTraceCnt--;
		}
		
		m_vEndPos = vScreenPos;
	}
	// private >
}
