using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.Xml;
using System.IO;
using System.Text;

public class Tbl_Recall_Record : AsTableRecord
{
	int m_Index;public int Index{get{return m_Index;}}
	int m_RecallMonsterID;public int RecallMonsterID{get{return m_RecallMonsterID;}}
	int m_RecallCount;public int RecallCount{get{return m_RecallCount;}}
	eRECALL_DEATH m_RecallDeath;public eRECALL_DEATH RecallDeath{get{return m_RecallDeath;}}
	int m_RecallMax;public int RecallMax{get{return m_RecallMax;}}
	int m_RecallMonsterLifeTime;public int RecallMonsterLifeTime{get{return m_RecallMonsterLifeTime;}}
	
	public Tbl_Recall_Record(XmlElement _element)// : base(_element)
	{
		try{
			XmlNode node = (XmlElement)_element;
			
			SetValue(ref m_Index, node, "Index");
			SetValue(ref m_RecallMonsterID, node, "RecallMonsterID");
			SetValue(ref m_RecallCount, node, "RecallCount");
			SetValue(ref m_RecallDeath, node, "RecallDeath");
			SetValue(ref m_RecallMax, node, "RecallMax");
			SetValue(ref m_RecallMonsterLifeTime, node, "RecallMonsterLifeTime");
		}
		catch(System.Exception e)
		{
			Debug.LogError("[Tbl_Recall_Record] 'constructor':|" + e + "| error while parsing");
		}
	}
}

public class Tbl_Recall_Table : AsTableBase {
	SortedList<int, Tbl_Recall_Record> m_ResourceTable = 
		new SortedList<int, Tbl_Recall_Record>();
	
	public Tbl_Recall_Table(string _path)
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
				Tbl_Recall_Record record = new Tbl_Recall_Record((XmlElement)node);
				m_ResourceTable.Add(record.Index, record);
			}
		}
		catch(System.Exception e)
		{
			Debug.LogError("[Tbl_Recall_Table] LoadTable:|" + e + "| error while parsing");
		}
	}
	
	public Tbl_Recall_Record GetRecord(int _id)
	{
		if(m_ResourceTable.ContainsKey(_id) == true)
		{
			return m_ResourceTable[_id];
		}
		
		Debug.LogError("[Tbl_Recall_Table]GetRecord: there is no record");
		return null;
	}
	
	public SortedList<int, Tbl_Recall_Record> GetList()
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
