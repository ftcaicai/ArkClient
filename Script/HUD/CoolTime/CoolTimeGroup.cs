using UnityEngine;
using System.Collections;

public class CoolTimeGroup 
{	
	private int m_iCoolTimeGroup = 0;	
	private float m_initialCoolTime = 1.0f;
	
	private float m_fMaxCoolTime = 1.0f;
	private float m_fValueCoolTime = 0.0f;
	private float m_fRemainCoolTime = 0.0f; 	
	private bool m_isActiveCooltime = false;
	
	private float m_fCollTimeRealTime = 0f;
	
	/*public CoolTimeGroup( int iSkillID, int iSkillLevel )
	{
		Tbl_SkillLevel_Record skillData = AsTableManager.Instance.GetTbl_SkillLevel_Record( iSkillLevel, iSkillID );
		if( null == skillData )
		{
			Debug.LogError("CoolTimeItem::SetCoolTime() [ null == skillData ] skill id : " + iSkillID + " skilllevel id : " + iSkillLevel );
			return;
		}
		m_initialCoolTime = skillData.CoolTime * 0.001f;
		if( 0 >= m_initialCoolTime )
		{
			Debug.LogError("RealItem::SetItem() [ 0 >= m_fMaxCoolTime ] item id : " + iSkillID );
			m_initialCoolTime = 1.0f;
		}
	}*/
	
	public CoolTimeGroup( int iCoolTimeGroup, float iCoolTime )
	{
		m_iCoolTimeGroup = iCoolTimeGroup;
		m_initialCoolTime = iCoolTime;		
	}
	
	public void SetInitcoolTime( float iCoolTime )
	{
		m_initialCoolTime = iCoolTime;
	}
	
	
	
	public int getCoolTimeGroupID
	{
		get
		{
			return m_iCoolTimeGroup;
		}
	}
	
	public float getCoolTimeValue
	{
		get
		{
			return m_fValueCoolTime;
		}
	}
	
	public float getRemainTime
	{
		get 
		{
			return m_fRemainCoolTime;
		}
	}
	
	public float getMaxTime
	{
		get
		{
			return m_fMaxCoolTime;	
		}
	}
	
	public bool isCoolTimeActive
	{
		get
		{
			return m_isActiveCooltime;
		}
	}
	
	public void ActionCoolTime()
	{
		StartCoolTime( m_initialCoolTime, m_initialCoolTime );
	}
	
	public void RemainCooltime( float time )
	{
		time = time * 0.001f;
		
		if( m_initialCoolTime < time )
			m_initialCoolTime = time;
		
		StartCoolTime( m_initialCoolTime, time );
	}
	
	public void RemainCooltime( float time, float maxTime )
	{
		time = time * 0.001f;		
		StartCoolTime( maxTime* 0.001f, time );
	}
	
	public void SetRemainCoolTime( float time )
	{
		m_fRemainCoolTime = time * 0.001f;
	}
	
	private void StartCoolTime( float fMaxValue, float fRemainCooltime )
	{	
		// < ilmeda, 20121122
		// for cheat command
		/*if( RuntimePlatform.OSXEditor == Application.platform || RuntimePlatform.WindowsEditor == Application.platform)
		{
			AsPlayerFsm playerFsm = AsEntityManager.Instance.GetPlayerCharFsm();
			if( null == playerFsm)
				return;

			if( false == playerFsm.bUseCoolTime)
				return;
		}*/
		// ilmeda, 20121122 >
		
		if( 0.0f >= fMaxValue )
		{
			Debug.LogError("UISlotItem::SetCoolTime() [ 0.0f >= fMaxValue");
			return;
		}		
		m_fRemainCoolTime = fRemainCooltime;
		m_fMaxCoolTime = fMaxValue;	
		m_isActiveCooltime = true;
		
		m_fCollTimeRealTime = Time.realtimeSinceStartup;
		
		ResetCoolTime();
	}
	
	private void ResetCoolTime()
	{
		if( false == m_isActiveCooltime )
			return;
		
		if (0.0f >= m_fRemainCoolTime)
		{
			m_fRemainCoolTime = 0.0f;
			m_isActiveCooltime = false;	
		}

		m_fValueCoolTime = (m_fMaxCoolTime - m_fRemainCoolTime) / m_fMaxCoolTime;			
	}
	
	public void Update() 
	{
		if( false == m_isActiveCooltime )
			return;		
		
		//m_fRemainCoolTime -= Time.deltaTime;
		m_fRemainCoolTime -= (Time.realtimeSinceStartup - m_fCollTimeRealTime);
		m_fCollTimeRealTime = Time.realtimeSinceStartup;
		ResetCoolTime();
	}
}
