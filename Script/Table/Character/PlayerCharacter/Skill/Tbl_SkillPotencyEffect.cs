using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.Xml;
using System.IO;
using System.Text;

public class Tbl_SkillPotencyEffect_Record : AsTableRecord
{
	int m_Index;public int Index{get{return m_Index;}}
	string m_PotencyEffect_FileName;public string PotencyEffect_FileName{get{return m_PotencyEffect_FileName;}}
		string m_PotencyEffect_FileName_Full;public string PotencyEffect_FileName_Full{get{return m_PotencyEffect_FileName_Full;}}
	string m_PotencyCriticalEffect_FileName;public string PotencyCriticalEffect_FileName{get{return m_PotencyCriticalEffect_FileName;}}
	eEFFECT_TIMING m_PotencyEffect_Timing;public eEFFECT_TIMING PotencyEffect_Timing{get{return m_PotencyEffect_Timing;}}
	
	string m_PotencySound_FileName;public string PotencySound_FileName{get{return m_PotencySound_FileName;}}
	string m_PotencyCriticalSound_FileName;public string PotencyCriticalSound_FileName{get{return m_PotencyCriticalSound_FileName;}}
	eSOUND_TIMING m_PotencySound_Timing;public eSOUND_TIMING PotencySound_Timing{get{return m_PotencySound_Timing;}}
	float m_Effect_Size;public float Effect_Size{get{return m_Effect_Size;}}
	float m_HitShake;public float HitShake{get{return m_HitShake;}}
	
	public Tbl_SkillPotencyEffect_Record(XmlElement _element)// : base(_element)
	{
		try{
			XmlNode node = (XmlElement)_element;
			
			SetValue(ref m_Index, node, "Index");
			SetValue(ref m_PotencyEffect_FileName, node, "PotencyEffect_FileName");
				SetStringFullPath(ref m_PotencyEffect_FileName_Full, node, "PotencyEffect_FileName");
			SetValue(ref m_PotencyCriticalEffect_FileName, node, "PotencyCriticalEffect_FileName");
			SetValue<eEFFECT_TIMING>(ref m_PotencyEffect_Timing, node, "PotencyEffect_Timing");
			
			SetValue(ref m_PotencySound_FileName, node, "PotencySound_FileName");
			SetValue(ref m_PotencyCriticalSound_FileName, node, "PotencyCriticalSound_FileName");
			SetValue<eSOUND_TIMING>(ref m_PotencySound_Timing, node, "PotencySound_Timing");
			SetValue(ref m_Effect_Size, node, "Effect_Size");
			SetValue(ref m_HitShake, node, "HitShake");
		}
		catch(System.Exception e)
		{
			Debug.LogError("[Tbl_SkillPotencyEffect_Record] 'constructor':|" + e + "| error while parsing");
		}
	}
}

public class Tbl_SkillPotencyEffect_Table : AsTableBase {
	SortedList<int, Tbl_SkillPotencyEffect_Record> m_ResourceTable = 
		new SortedList<int, Tbl_SkillPotencyEffect_Record>();
	
	public Tbl_SkillPotencyEffect_Table(string _path)
	{
//		m_TableName = "CharacterResource";
		m_TableType = eTableType.NPC;
		
		LoadTable(_path);
	}
	
	public override void LoadTable(string _path)
	{
		try{
			XmlElement root = GetXmlRootElement(_path);
			XmlNodeList nodes = root.ChildNodes;
			
			foreach(XmlNode node in nodes)
			{
				Tbl_SkillPotencyEffect_Record record = new Tbl_SkillPotencyEffect_Record((XmlElement)node);
				m_ResourceTable.Add(record.Index, record);
			}
		}
		catch(System.Exception e)
		{
			Debug.LogError("[Tbl_SkillPotencyEffect_Table] LoadTable:|" + e + "| error while parsing");
		}
	}
	
	public Tbl_SkillPotencyEffect_Record GetRecord(int _id)
	{
		if(m_ResourceTable.ContainsKey(_id) == true)
		{
			return m_ResourceTable[_id];
		}
		
//		Debug.LogError("[Tbl_SkillPotencyEffect_Table]GetRecord: there is no record");
		return null;
	}
	
	public SortedList<int, Tbl_SkillPotencyEffect_Record> GetList()
	{
		return m_ResourceTable;
	}
	
//	public Tbl_Npc_Record GetRecordByTribeAndClass(int _tribe, int _class)
//	{
//		foreach(KeyValuePair<int, Tbl_Npc_Record> pair in m_ResourceTable)
//		{
//			if(pair.Value.Race == _tribe)
//			{
//				if(pair.Value.Class == _class)
//				{
//					return pair.Value;
//				}
//			}
//		}
//		
//		Debug.LogError("Tbl_Npc_Table::GetRecordByTribeAndClass: no record");
//		return null;
//	}
}
