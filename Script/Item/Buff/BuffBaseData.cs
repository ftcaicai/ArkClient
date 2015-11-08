
using UnityEngine;
using System.Collections;

public abstract class BuffBaseData
{
	protected string m_strIconPath;
	protected bool m_bNeedDelete = false;
	
	private float m_fMaxCoolTime = 1.0f;
    private float m_fCurCoolTime = 0.0f;
	private float m_fRemainCoolTime = 0.0f;
	private float m_fStartTime = 0f;
	
	public string getIconPath
	{
		get
		{
			return m_strIconPath;
		}
	}
	
	public float getRemainCoolTime
	{
		get
		{
			return m_fRemainCoolTime;	
		}
	}
	
	public float getCurCoolTime 
	{
		get
		{
			return m_fCurCoolTime;
		}
	}
	
	public bool isNeedDelete
    {
		get
		{
        	return m_bNeedDelete;
		}
    }
	
	public abstract int GetSkillIdx();
	public abstract int GetPotencyIdx();
	public abstract int GetSkillLevel();
	
	
	public BuffBaseData()
	{
		
	}
	
	protected void SetCoolTime( float nDuration, int skillIdx, int potencyIdx, int skillLevelIdx, int chargeStep = 0 )
	{		
		float fMaxBuffCooltim = 1f;
		
		if(BuffBaseMgr.s_MonsterSkillIndexRange_Min <= skillIdx && skillIdx <= BuffBaseMgr.s_MonsterSkillIndexRange_Max)
		{
			Tbl_MonsterSkill_Record skill = AsTableManager.Instance.GetTbl_MonsterSkill_Record(skillIdx);
			m_strIconPath = skill.listSkillPotency[potencyIdx].Potency_BuffIcon;
			
			// skill cooltime max
			Tbl_MonsterSkillLevel_Record mobSkillLevel = AsTableManager.Instance.GetTbl_MonsterSkillLevel_Record( skillLevelIdx, skillIdx );
			
			if( null == mobSkillLevel )
			{
				AsUtil.ShutDown( "playerBuffData()[ null == skillLevel ) skill id : " + skillIdx + " level id : " + skillLevelIdx );
				return;
			}
			
			if( mobSkillLevel.listSkillLevelPotency.Count <= potencyIdx )
			{
				AsUtil.ShutDown( "playerBuffData()[ skillLevel.listSkillLevelPotency.Count <= potencyIdx ) skill id : " 
					+ skillIdx + " level id : " + skillLevelIdx + " potencyidx : " + potencyIdx );
				return;
			}
			fMaxBuffCooltim = mobSkillLevel.listSkillLevelPotency[potencyIdx].Potency_Duration;
		}
		else
		{
			// icon path
			Tbl_Skill_Record skill = AsTableManager.Instance.GetTbl_Skill_Record(skillIdx);						
			m_strIconPath = skill.listSkillPotency[potencyIdx].Potency_BuffIcon;
			
			// skill cooltime max
			Tbl_SkillLevel_Record skillLevel = null;
			
			if( 0 == chargeStep) 
			{
				skillLevel = AsTableManager.Instance.GetTbl_SkillLevel_Record( skillLevelIdx, skillIdx ); 	
			}
			else
			{
				skillLevel = AsTableManager.Instance.GetTbl_SkillLevel_Record( skillLevelIdx, skillIdx, chargeStep ); 	
			}
			
			if( null == skillLevel )
			{
				AsUtil.ShutDown( "playerBuffData()[ null == skillLevel ) skill id : " + skillIdx + " level id : " + skillLevelIdx );
				return;
			}
			
			if( skillLevel.listSkillLevelPotency.Count <= potencyIdx )
			{
				AsUtil.ShutDown( "playerBuffData()[ skillLevel.listSkillLevelPotency.Count <= potencyIdx ) skill id : " 
					+ skillIdx + " level id : " + skillLevelIdx + " potencyidx : " + potencyIdx );
				return;
			}
			fMaxBuffCooltim = skillLevel.listSkillLevelPotency[potencyIdx].Potency_Duration;
		}	
		
		StartCoolTime( nDuration, fMaxBuffCooltim );	
	}
	
	
	protected void StartCoolTime(float fDuration, float maxCooltime )
    {       
	    m_fRemainCoolTime = fDuration;
	    m_fMaxCoolTime = maxCooltime;   
		m_fStartTime = Time.realtimeSinceStartup;
    }
	
	
	public void SetMinusRemainTime()
    {
		float fMinTime = Time.realtimeSinceStartup - m_fStartTime; 
		m_fStartTime = Time.realtimeSinceStartup;
		
        m_fRemainCoolTime -= fMinTime;
		if (0.0f > m_fRemainCoolTime)
			m_fRemainCoolTime = 0.0f;
		
		
		
		if( m_fMaxCoolTime <= 0f )
		{
			m_fMaxCoolTime = 1f;
			Debug.LogError("BuffBaseData::SetMinusRemainTime()[m_fMaxCoolTime >= 0f] : " + m_fMaxCoolTime);
		}

		m_fCurCoolTime = (m_fMaxCoolTime - m_fRemainCoolTime) / m_fMaxCoolTime;
    }
	
	public void Update()
    {
        if (0.0f < m_fRemainCoolTime)
        {
            SetMinusRemainTime();            
        }        
    }
}
