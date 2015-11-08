
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class OtherBuffMgr : BuffBaseMgr 
{
	
	//---------------------------------------------------------------------
	/* Variable */
	//---------------------------------------------------------------------	
	protected List<BuffBaseData> m_BuffDataList = new List<BuffBaseData>();
	protected AsMonsterFsm m_MobFsm;
	protected AsOtherUserFsm m_OtherUserFsm;
	
	//---------------------------------------------------------------------
	/* Function */
	//---------------------------------------------------------------------
	protected void ResetUiBuffSlot()
	{
		if( null == uiBuffSlots ) 
		{
			Debug.LogError("null == m_BuffSlots");
			return;			
		}
		bool isShow = false;
		
		if( null != AsHUDController.Instance && true ==AsHUDController.Instance.targetInfo.gameObject.active )
			isShow = true;
		
        for (int i = 0; i < uiBuffSlots.Length; ++i)
        {
			if( null == uiBuffSlots[i] )
			{
				AsUtil.ShutDown("UIBuffSlots is null [ index : " + i );
				continue;
			}
			
            if (i < m_BuffDataList.Count && true == isShow )
			{
				uiBuffSlots[i].OpenBuffSlot(m_BuffDataList[i]);
			}
            else
			{
				uiBuffSlots[i].OffBuffSlot(); 
			}
        }
		
		if( uiBuffSlots.Length < m_BuffDataList.Count )
			SetActiveEtc(true);
		else
		{
			SetActiveEtc(false);
		}
	}
	
	void Awake()
	{
		SetActiveEtc(false);
	}
	
	
	public void InsertBuffData( body1_SC_CHAR_BUFF _data )
	{
		foreach(body2_SC_CHAR_BUFF buff in _data.body)
		{
			if( 0 == buff.nDuration )
				continue;
			
			if( true == IsMonsterSkillIndex( buff.nSkillTableIdx ) )
			{
				if( false == IsCheckHaveMobSkillIcon( buff.nSkillTableIdx, buff.nPotencyIdx ) )
					continue;
			}
			else
			{
				if( false == IsCheckHaveIcon( buff.nSkillTableIdx, buff.nPotencyIdx ) )
					continue;
			}
			
			/*if( IsSameBuffData( buff.nSkillTableIdx, buff.nPotencyIdx ) )
			{
				continue;
			}*/
		
        	m_BuffDataList.Add(new PlayerBuffData(buff));
			//Debug.Log("AddCharBuff [ PotencyTableIdx : " + buff.nSkillTableIdx + " bufftype : " + buff.eType + " nDuration : " + buff.nDuration);
		}       
	}
	
	protected void InsertBuffData(body1_SC_NPC_BUFF _data)
    {		
		foreach(body2_SC_NPC_BUFF buff in _data.body)
		{
			InsertBuffData( buff, 0f );
			
		}  
    }
	
	protected void InsertBuffData( body2_SC_NPC_BUFF buff, float _fRemainTime )
	{
		if( 0 == buff.nDuration )
				return;
			
		if( true == IsMonsterSkillIndex( buff.nSkillTableIdx ) )
		{
			if( false == IsCheckHaveMobSkillIcon( buff.nSkillTableIdx, buff.nPotencyIdx ) )
				return;
		}
		else
		{
			if( false == IsCheckHaveIcon( buff.nSkillTableIdx, buff.nPotencyIdx ) )
				return;
		}
		
		
		
		/*if( IsSameBuffData( buff.nSkillTableIdx, buff.nPotencyIdx ) )
		{
			return;
		}*/
	
    	m_BuffDataList.Add(new NpcBuffData(buff, _fRemainTime));		
	}
	/*protected bool IsSameBuffData( int iSkillTableIdx, int potencyIdx )
	{		
		foreach( BuffBaseData _data in m_BuffDataList )
		{
			if( _data.GetSkillIdx() == iSkillTableIdx && _data.GetPotencyIdx() == potencyIdx )
				return true;
		}
		
		return false;
	}*/
	protected void DeleteBuffData( body_SC_CHAR_DEBUFF data )
	{
		foreach( BuffBaseData buffdata in m_BuffDataList )
        {
			if( buffdata.GetSkillIdx() == data.nSkillTableIdx && 
				buffdata.GetPotencyIdx() == data.nPotencyIdx )
            {				
				m_BuffDataList.Remove( buffdata );
				return;
			}
		}
	}	
	
	protected void DeleteBuffData( body_SC_NPC_DEBUFF data )
	{
		foreach( BuffBaseData buffdata in m_BuffDataList )
        {
			if( buffdata.GetSkillIdx() == data.nSkillTableIdx && 
				buffdata.GetPotencyIdx() == data.nPotencyIdx )
            {				
				m_BuffDataList.Remove( buffdata );
				return;
			}
		}
	}	
	
	
	public void ReciveBuff( body1_SC_NPC_BUFF _data )
	{
        InsertBuffData(_data);
        ResetUiBuffSlot();
	}
	
	public void DeleteBuff( body_SC_NPC_DEBUFF _data )
	{
		DeleteBuffData( _data );
		ResetUiBuffSlot();
	}
	
	public void EmptyOtherUserFsm()
	{
		ClearBuff();
		m_OtherUserFsm = null;
		SetActiveEtc(false);
	}
	
	
	public void ReciveBuff( AsBaseEntity target )
	{
		m_OtherUserFsm = null;
		ClearBuff();	
		
		if( null == target )
		{			
			return;
		}		
		
		AsMonsterFsm mobFsm = target.GetComponent( eComponentType.FSM_MONSTER ) as AsMonsterFsm;			
		if( null != mobFsm )
		{				
			m_MobFsm = mobFsm;	
			m_OtherUserFsm = null;
		
			foreach( CNpcBuffTempData _data in mobFsm.getBuffTempList )
			{			
				InsertBuffData( _data.getNpcBuff, _data.getRemainCoolTime );				
			}	
		}
		else
		{
			AsOtherUserFsm otehrUserFsm = target.GetComponent( eComponentType.FSM_OTHER_USER ) as AsOtherUserFsm;
			if( null != otehrUserFsm )				
			{
				m_OtherUserFsm = otehrUserFsm;
				m_MobFsm = null;
				
				foreach( CNpcBuffTempData _data in m_OtherUserFsm.getBuffTempList )
				{			
					InsertBuffData( _data.getNpcBuff, _data.getRemainCoolTime );						
				}					
			}
		}
		
		ResetUiBuffSlot();
	}
	
	
	public void ClearBuff()
	{
		SetHideUI();	
		SetActiveEtc(false);	
		m_BuffDataList.Clear();
	}	
	
	protected void ResetEntityBuff()
	{
		if( null != m_MobFsm )			
		{		
			ClearBuff();
			foreach( CNpcBuffTempData _data in m_MobFsm.getBuffTempList )
			{				
				InsertBuffData( _data.getNpcBuff, _data.getRemainCoolTime );
			}	
			
			m_MobFsm.SetUseNeedBuffReset();	
		}
		else if( null != m_OtherUserFsm )
		{
			ClearBuff();
			foreach( CNpcBuffTempData _data in m_OtherUserFsm.getBuffTempList )
			{				
				InsertBuffData( _data.getNpcBuff, _data.getRemainCoolTime );
			}	
			
			m_OtherUserFsm.SetUseNeedBuffReset();	
		}
	}
	
	
	
	
	public override void UpdateLogic()
	{
		
		base.UpdateLogic();
		
		for (int i = 0; i < m_BuffDataList.Count; i++)
        {
            BuffBaseData buffdata = m_BuffDataList[i];
            buffdata.Update();          
        }
		
		if( null != m_MobFsm )
		{			
			if( true == m_MobFsm.isNeedBuffReset )
			{
				ResetEntityBuff();
				ResetUiBuffSlot();
			}			
		}	
		else if( null != m_OtherUserFsm )
		{
			if( true == m_OtherUserFsm.isNeedBuffReset )
			{
				ResetEntityBuff();
				ResetUiBuffSlot();
			}	
		}
	}
	
	// Update is called once per frame
	/*void Update () 
	{
		for (int i = 0; i < m_BuffDataList.Count; i++)
        {
            BuffBaseData buffdata = m_BuffDataList[i];
            buffdata.Update();          
        }
		
		if( null != m_MobFsm )
		{			
			if( true == m_MobFsm.isNeedBuffReset )
			{
				ResetEntityBuff();
				ResetUiBuffSlot();
			}			
		}	
		else if( null != m_OtherUserFsm )
		{
			if( true == m_OtherUserFsm.isNeedBuffReset )
			{
				ResetEntityBuff();
				ResetUiBuffSlot();
			}	
		}
	}*/
}