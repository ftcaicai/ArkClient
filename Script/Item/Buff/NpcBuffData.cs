
using UnityEngine;
using System.Collections;

public class NpcBuffData : BuffBaseData
{	
	body2_SC_NPC_BUFF m_ServerData;	
	
	
	
	private void InitData( float nDuration )
	{
		int skillIdx = getServerData.nSkillTableIdx;
		int potencyIdx = getServerData.nPotencyIdx;
		int skillLevelIdx = getServerData.nSkillLevel;
		
		float nRemainTime = nDuration;
		
		if( 0f == nRemainTime )
		{
			nRemainTime = getServerData.nDuration;
		}
		
		SetCoolTime( nRemainTime, skillIdx, potencyIdx, skillLevelIdx );		
	}
	
	public override int GetSkillLevel()
	{
		return getServerData.nSkillLevel;
	}
	
	public NpcBuffData( body2_SC_NPC_BUFF _data, float nDuration )
	{
		if( null == _data )
		{
			AsUtil.ShutDown("PlayerBuffData::NpcBuffData()[ null == body2_SC_NPC_BUFF ]");
			return;
		}
		
		m_ServerData = _data;
		
		
		InitData( nDuration );
	}
	
	public override int GetSkillIdx()
	{
		return getServerData.nSkillTableIdx;
	}
	
	public override int GetPotencyIdx()
	{
		return getServerData.nPotencyIdx;
	}
	
	
	public body2_SC_NPC_BUFF getServerData
	{
		get
		{
			return m_ServerData;
		}
	}

	
}
