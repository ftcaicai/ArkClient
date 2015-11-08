
using UnityEngine;
using System.Collections;


public class PlayerBuffData : BuffBaseData
{	
	body2_SC_CHAR_BUFF m_ServerData;	
	
	private void InitData()
	{
		int skillIdx = getServerData.nSkillTableIdx;
		int potencyIdx = getServerData.nPotencyIdx;
		int skillLevelIdx = getServerData.nSkillLevel;
		int chargeStep = getServerData.nChargeStep;
		
		SetCoolTime( getServerData.nDuration*0.001f, skillIdx, potencyIdx, skillLevelIdx, chargeStep );	
	}
	
	public override int GetSkillLevel()
	{
		return getServerData.nSkillLevel;
	}
	
	public PlayerBuffData( body2_SC_CHAR_BUFF _data )
	{
		if( null == _data )
		{
			AsUtil.ShutDown("PlayerBuffData::PlayerBuffData()[ null == body2_SC_CHAR_BUFF ]");
			return;
		}
		
		m_ServerData = _data;
		
		
		InitData();
	}
	
	public override int GetSkillIdx()
	{
		return getServerData.nSkillTableIdx;
	}
	
	public override int GetPotencyIdx()
	{
		return getServerData.nPotencyIdx;
	}
	
	
	public body2_SC_CHAR_BUFF getServerData
	{
		get
		{
			return m_ServerData;
		}
	}

	
}