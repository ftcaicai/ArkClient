using UnityEngine;
using System.Collections;

public class AsPvpTimeDlg : MonoBehaviour
{
	private GameObject m_goRoot = null;
	private bool m_bUpdateTime = false;
	private float m_fStartTime = 0.0f;
	private float m_fDuration = 0.0f;
	private int m_nSec = 0;
	private char[] m_YellowFontBuf = new char[10]{ 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j'};
	
	public SpriteText m_TextSec = null;
	public SimpleSprite m_ImgEff = null;

	void Start()
	{
	}
	
	void Update()
	{
		if( true == m_bUpdateTime)
		{
			float fCurTime = Time.realtimeSinceStartup;
			
			// update time
			float fTime = fCurTime - m_fStartTime;
			if( fTime >= 1.0f)
			{
				m_nSec = (int)( ( m_fDuration - fTime) + 0.5f);

				if( m_nSec <= 30)
				{
					if( false == m_ImgEff.gameObject.active)
						m_ImgEff.gameObject.SetActiveRecursively( true);
					
					UIPanel panel = m_ImgEff.gameObject.GetComponent<UIPanel>();
					if( false == panel.IsTransitioning)
						panel.BringIn();
				}
			}

			if( m_nSec < 1)
			{
				m_nSec = 0;
				
				m_bUpdateTime = false;
				Close();
			}
			
			m_TextSec.Text = _GetCustomFont( m_nSec);
		}
	}

	public void Open(GameObject goRoot, int nTimeDurationSec)
	{
		gameObject.SetActiveRecursively( true);
		m_goRoot = goRoot;
		
		_StartTime( nTimeDurationSec);
	}

	public void Close()
	{
		gameObject.SetActiveRecursively( false);

		if( null != m_goRoot)
			Destroy( m_goRoot);
	}
	
	public void SetBackGroundDelayTime(int nBackGroundDelayTime)
	{
		m_fStartTime = Time.realtimeSinceStartup - nBackGroundDelayTime;
	}
	
	private void _StartTime(int nTimeDurationSec)
	{
		m_bUpdateTime = true;
		m_fStartTime = Time.realtimeSinceStartup;
		m_fDuration = 0.0f;
		m_nSec = 0;
		
		if( nTimeDurationSec < 1000)
		{
			m_fDuration = (float)nTimeDurationSec;
			m_nSec = nTimeDurationSec;
			m_ImgEff.gameObject.SetActiveRecursively( false);
		}
		else
		{
			Debug.Log( "AsPvpTimeDlg::_StartTime(), TimeDurationSec: " + nTimeDurationSec);
		}
		
		m_TextSec.Text = _GetCustomFont( m_nSec);
	}

	private string _GetCustomFont(int nValue)
	{
		if( nValue < 0)
			nValue = 0;

		int nBuf = 0;
		string str = nValue.ToString();
		char[] strBuf = new char[str.Length];

		for( int i = 0; i < str.Length; i++)
		{
			nBuf = int.Parse( str[i].ToString());
			strBuf[i] = m_YellowFontBuf[nBuf];
		}
		
		string strRes = new string( strBuf);
		return strRes;
	}
}
