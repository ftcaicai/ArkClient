using UnityEngine;
using System.Collections;
using System.Collections.Generic;




public class Tbl_SE_CharacerInfo_Record 
{

	int m_Id;
	public int Id
	{
		set{ m_Id = value;}
		get{return m_Id;}
	}
	
	string m_Name;
	public string Name
	{
		set{m_Name = value;}
		get{return m_Name;}
	}
	eRACE m_RaceId;
	public eRACE Race
	{
		set{m_RaceId = value;}
		get{return m_RaceId;}
	}
	
	
	eCLASS m_Class;
	public eCLASS Class
	{
		set{m_Class = value;}
		get{return m_Class;}
	}
	
	eNPCType    m_Type;
	public eNPCType Type
	{
		set{m_Type = value;}
		get{return m_Type;}
	}
	string m_ModelingPath;
	public string ModelingPath
	{
		set{m_ModelingPath = value;}
		get{return m_ModelingPath;}
	}	
	
	string m_ModelingPath_FeMale;
	public string ModelingPath_FeMale
	{
		set{m_ModelingPath_FeMale = value;}
		get{return m_ModelingPath_FeMale;}
	}	
}

public class Tbl_SE_CharacerInfo {
	//
	SortedList<int, Tbl_SE_CharacerInfo_Record> m_ResourceTable = 
		new SortedList<int, Tbl_SE_CharacerInfo_Record>();
	
	int    m_PcTotalCount = 0; public int PcTotalCount{get{return m_PcTotalCount;}}
	int    m_NpcTotalCount = 0; public int NpcTotalCount{get{return m_NpcTotalCount;}}
	int    m_MonsterTotalCount = 0; public int MonsterTotalCount{get{return m_MonsterTotalCount;}}
	//

	public void LoadTable()
	{
		//PC
		foreach (KeyValuePair<int, Tbl_Class_Record> tmpTribe in AsTableManager.Instance.GetClassList())
		{
			Tbl_SE_CharacerInfo_Record record = new Tbl_SE_CharacerInfo_Record();
			record.Id   				 = tmpTribe.Value.Index;
			record.Race					 = tmpTribe.Value.Race;
			record.Class                 = tmpTribe.Value.Class;
			record.Name					 = tmpTribe.Value.Class.ToString();
			record.Type 				 = (eNPCType)0;
			record.ModelingPath 		 = tmpTribe.Value.ModelingPath_Male;
			record.ModelingPath_FeMale   = tmpTribe.Value.ModelingPath_Female;
			
			m_ResourceTable.Add(record.Id, record);
			m_PcTotalCount++; 
			
		}		
	
		#region -Class_All-
		Tbl_SE_CharacerInfo_Record allRecord = new Tbl_SE_CharacerInfo_Record();
		allRecord.Id   				 = m_PcTotalCount + 1;
		allRecord.Race				 = eRACE.DEMIGOD;
		allRecord.Class              = eCLASS.All;
		allRecord.Name				 = "All";
		allRecord.Type 				 = (eNPCType)0;
		allRecord.ModelingPath 		  = m_ResourceTable[1].ModelingPath;
		allRecord.ModelingPath_FeMale = m_ResourceTable[1].ModelingPath_FeMale;
		
		m_ResourceTable.Add(allRecord.Id, allRecord);
		m_PcTotalCount++; 
		#endregion

		foreach (KeyValuePair<int, Tbl_Npc_Record> tmpNpc in AsTableManager.Instance.GetNpcList())
		{
			if(tmpNpc.Value.NpcType == eNPCType.NPC || tmpNpc.Value.NpcType == eNPCType.Monster)
			{
				Tbl_SE_CharacerInfo_Record record = new Tbl_SE_CharacerInfo_Record();
				record.Id   		= tmpNpc.Value.Id;
				record.Race			= (eRACE)0;
				record.Class        = (eCLASS)0;
				record.Name			= tmpNpc.Value.NpcName;
				record.Type 		= tmpNpc.Value.NpcType;
				record.ModelingPath = tmpNpc.Value.ModelingPath;
				m_ResourceTable.Add(record.Id, record);
				
				switch((eNPCType)tmpNpc.Value.NpcType)
				{				
					case eNPCType.Monster: 
					{
						m_MonsterTotalCount++;  
						Tbl_Monster_Record monster = AsTableManager.Instance.GetTbl_Monster_Record(record.Id);
						if(null != monster)						
							record.Name			= monster.Class;						
					}
					break;
					case eNPCType.NPC: m_NpcTotalCount++; break;
					
				}
			}
			
		}	
		
	}
	
	public Tbl_SE_CharacerInfo_Record GetRecord(int _id)
	{
		if(m_ResourceTable.ContainsKey(_id) == true)
		{
			return m_ResourceTable[_id];
		}
		
		Debug.LogError("[Tbl_SE_CharacerInfo]GetRecord: there is no record");
		return null;
	}
	
	           
	
	public SortedList<int, Tbl_SE_CharacerInfo_Record> GetList()
	{
		return m_ResourceTable;
	}	

}
