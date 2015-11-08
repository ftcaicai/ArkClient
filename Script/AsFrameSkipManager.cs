using UnityEngine;
using System.Collections;

public class AsFrameSkipManager 
{
	static private AsFrameSkipManager instance = null;
	static public AsFrameSkipManager Instance
	{
		get
		{
			if( null == instance)
				instance = new AsFrameSkipManager();
			
			return instance;
		}
	}

	protected int		m_nUpdateFrameSkipCount = 0;
	protected int		m_nLateUpdateFrameSkipCount = 0;

	protected int 		m_nFrameSkipValue = 3;
	
	private AsFrameSkipManager()
	{

	}

	public bool IsFrameSkip_Update()
	{
		if (m_nUpdateFrameSkipCount > 0)
			return true;

		return false;
	}

	public bool IsFrameSkip_LateUpdate()
	{
		if (m_nLateUpdateFrameSkipCount > 0)
			return true;
		
		return false;
	}

	public void Update () 
	{
		m_nUpdateFrameSkipCount--;

		if (m_nUpdateFrameSkipCount < 0)
			m_nUpdateFrameSkipCount = m_nFrameSkipValue;
	}

	public void LateUpdate () 
	{
		m_nLateUpdateFrameSkipCount--;

		if (m_nLateUpdateFrameSkipCount < 0)
			m_nLateUpdateFrameSkipCount = m_nFrameSkipValue;
	}

}








