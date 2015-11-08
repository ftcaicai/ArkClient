
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PlayerBuffMgr : BuffBaseMgr 
{
	//---------------------------------------------------------------------
	/* Static */
	//---------------------------------------------------------------------	
	private static PlayerBuffMgr ms_kIstance = null;
	
	public static PlayerBuffMgr Instance
	{
		get
		{
			return ms_kIstance;
		}
	}
	
	
	//---------------------------------------------------------------------
	/* Variable */
	//---------------------------------------------------------------------	
	private List<PlayerBuffData> m_BuffDataList = new List<PlayerBuffData>();
	
	
	
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
		
        for (int i = 0; i < uiBuffSlots.Length; ++i)
        {
			if( null == uiBuffSlots[i] )
			{
				AsUtil.ShutDown("UIBuffSlots is null [ index : " + i );
				continue;
			}
			
            if (i < m_BuffDataList.Count)
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
		
        	m_BuffDataList.Add(new PlayerBuffData(buff));
			//Debug.Log("AddCharBuff [ PotencyTableIdx : " + buff.nSkillTableIdx + " bufftype : " + buff.eType + " nDuration : " + buff.nDuration);
		}       
	}
	
	
	protected void DeleteBuffData( body_SC_CHAR_DEBUFF data )
	{
		foreach( PlayerBuffData buffdata in m_BuffDataList )
        {
			if( buffdata.GetSkillIdx() == data.nSkillTableIdx && 
				buffdata.GetPotencyIdx() == data.nPotencyIdx )
            {				
				m_BuffDataList.Remove( buffdata );
				return;
			}
		}
	}
	
	public void ReciveBuff( body1_SC_CHAR_BUFF _data )
	{
        InsertBuffData(_data);
        ResetUiBuffSlot();
	}
	
	public void DeleteBuff( body_SC_CHAR_DEBUFF _data )
	{
		DeleteBuffData( _data );
		ResetUiBuffSlot();
	}
	
	
	//---------------------------------------------------------------------
	/* Function */
	//---------------------------------------------------------------------	
	
	
	//$yde
	public List<body2_SC_CHAR_BUFF> CharBuffDataList
	{
		get
		{
			List<body2_SC_CHAR_BUFF> list = new List<body2_SC_CHAR_BUFF>();
			foreach(PlayerBuffData data in m_BuffDataList)
			{
				list.Add(data.getServerData);
			}
			
			return list;
		}
	}
	
	
	public void SetShowUI()
	{
		ResetUiBuffSlot();
	}
	
	public void Clear()
	{
		m_BuffDataList.Clear();
		ResetUiBuffSlot();		
	}
	
	
	
	 // Awake
    void Awake()
	{
		ms_kIstance = this;
        DontDestroyOnLoad(gameObject);
		DDOL_Tracer.RegisterDDOL(this, gameObject);//$ yde
		SetActiveEtc( false );
	}

	
	
	public override void UpdateLogic()
	{
		base.UpdateLogic();
		
		for (int i = 0; i < m_BuffDataList.Count; i++)
        {
            PlayerBuffData buffdata = m_BuffDataList[i];
            buffdata.Update();          
        }		
	}
	// Update is called once per frame
	/*void Update () 
	{    
        for (int i = 0; i < m_BuffDataList.Count; i++)
        {
            PlayerBuffData buffdata = m_BuffDataList[i];
            buffdata.Update();          
        }
	}*/
}
