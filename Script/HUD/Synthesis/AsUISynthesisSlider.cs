using UnityEngine;
using System.Collections;

public class AsUISynthesisSlider : MonoBehaviour 
{
	public delegate void ActionDelegate(); 
	
	public UIProgressBar probarComplete;
	public float maxProgressTime = 2.0f;	
	private float m_fProgressTime = 0.0f;
	private bool m_bProgressStated = false;
	private ActionDelegate m_actionDelegate = null;
	
	public bool isStop
	{
		get
		{
			return m_bProgressStated == false;
		}
	}
	
	public void SetAction( ActionDelegate _del )
	{
		m_actionDelegate = _del;
	}
	
	public void Reset()
	{
		m_bProgressStated = false;
		probarComplete.Value = 0.0f;
	}
	
	public void ActionStart()
	{
		if( true == AsCommonSender.isSendItemMix )
			return;
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6013_EFF_SocketProgress", Vector3.zero, false);
		m_bProgressStated = true;
		m_fProgressTime = Time.time;
	}
	
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if( true == m_bProgressStated )
		{	
			float fValue = Time.time - m_fProgressTime;
			probarComplete.Value = fValue/maxProgressTime;			
			
			if( 1.0f <= probarComplete.Value )
			{
				m_bProgressStated = false;
				probarComplete.Value = 0.0f;	
				if( null != m_actionDelegate )
					m_actionDelegate();						
			}
		}		
	}
}
