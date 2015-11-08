using UnityEngine;
using System.Collections;

public delegate void AsCheckTapDelegate(int nTapCount);

public class AsCheckTap : MonoBehaviour
{
	private	AsCheckTapDelegate		m_tapDelegate;
	
	private	bool					m_isTapCheck = false;
	private	int						m_nTapCount;
	private float					m_fTime;
	private	float					m_fTapDelay = 0.5f;
	
	public float TapDelay
	{
		get{return m_fTapDelay;}
		set{m_fTapDelay = value;}
	}
	
	public void SetTapDelegate(AsCheckTapDelegate	del)
	{
		m_tapDelegate = del;
	}

	public void Tap()
	{
		m_nTapCount++;

		if( m_isTapCheck == false )
		{
			StartCoroutine( "ProcessTap");
		}
		else
		{
			if( m_nTapCount < 2 )
			{
				m_fTime = Time.time;
			}
			else
			{
				ExcuteAndReset();
			}
		}
	}
	
	public void Cancel()
	{
		m_isTapCheck = false;
	}
	
	IEnumerator ProcessTap()
	{
		m_isTapCheck = true;
		
		m_fTime = Time.time;
		
		while( Time.time < m_fTime + m_fTapDelay )
		{
			yield return null;
		}
		
		ExcuteAndReset();
	}
	
	void ExcuteAndReset()
	{
		if( m_isTapCheck == false )
			return;
		
		if( m_tapDelegate != null )
			m_tapDelegate( m_nTapCount );
		
		m_isTapCheck = false;
		m_nTapCount = 0;
	}
}
