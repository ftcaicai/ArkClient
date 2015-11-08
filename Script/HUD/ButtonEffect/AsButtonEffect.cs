using UnityEngine;
using System.Collections;

public class AsButtonEffect : MonoBehaviour 
{
	private GameObject		m_NewImg;
	
	private bool 			m_isRollingEffectActive;
	private GameObject		m_goRollingEffect;
	public GameObject RollingEffect 
	{
		set{m_goRollingEffect = value;}
	}

	// Use this for initialization
	void Start () 
	{
	
	}
	
	public void SetNewImg( bool _isActive)
	{
		if( null == m_NewImg)
		{
			if( true == _isActive)
			{
				m_NewImg = ResourceLoad.CreateUI( "UI/AsGUI/GUI_NewImg", gameObject.transform , new Vector3( 0f, 1.2f, -0.1f));
				m_NewImg.transform.localScale = new Vector3(1.3f,1.3f,1);
			}

			return;
		}
		
		if( _isActive == m_NewImg.gameObject.activeSelf)
			return;
		
		m_NewImg.gameObject.SetActive( _isActive );
	}
	
	public void SetRollingEffect( bool _isActive )
	{
		m_isRollingEffectActive = _isActive;
		
		if( m_goRollingEffect == null )
			return;
		
		m_goRollingEffect.SetActive( _isActive );
		m_goRollingEffect.renderer.enabled = _isActive;
	}
	
	// Update is called once per frame
	void Update () 
	{
	}
}
