using UnityEngine;
using System.Collections;

public class AsInputMarkSingle : SimpleSprite
{
	private Color m_curColor;
	private Vector3 m_curScale;
	private bool m_bShowCommand = false;
	
	// Use this for initialization
	public override void Start()
	{
		if( false == m_bShowCommand)
			gameObject.SetActiveRecursively( false);
	}
	
	// Update is called once per frame
	void Update()
	{
		if( 0.0f >= m_curColor.a)
		{
			_Hide();
			return;
		}
		
		m_curColor.a -= ( Time.deltaTime * 5.0f);
		renderer.sharedMaterial.SetColor( "_TintColor", m_curColor);
		
		float fBuf = ( Time.deltaTime * 2.0f);
		m_curScale.x += fBuf;
		m_curScale.y += fBuf;
		m_curScale.z += fBuf;
		transform.localScale = m_curScale;
	}

	protected override void OnEnable()
	{
		if( false == m_bShowCommand)
			gameObject.SetActiveRecursively( false);
	}
	
	private void _Hide()
	{
		if( true == m_bShowCommand)
		{
			m_bShowCommand = false;
			gameObject.SetActiveRecursively( false);
		}
	}

	public void Show(Vector3 vScreenPos)
	{
		if( true == gameObject.active)
			_Hide();

		m_bShowCommand = true;
		gameObject.SetActiveRecursively( true);
		
		m_curColor = new Color( 1, 1, 1, 1);
		renderer.sharedMaterial.SetColor( "_TintColor", m_curColor);
		
		Vector3 vRes = CameraMgr.Instance.ScreenPointToUIRay( vScreenPos);
		vRes.z = 1.0f;
		transform.position = vRes;
		
		m_curScale = new Vector3( 0.3f, 0.3f, 0.3f);
		transform.localScale = m_curScale;
	}
}
