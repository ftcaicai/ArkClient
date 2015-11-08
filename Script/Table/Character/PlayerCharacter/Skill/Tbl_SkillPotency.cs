//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//
//using System;
//using System.Xml;
//using System.IO;
//using System.Text;
//
//public class Tbl_SkillPotency_Record : AsTableRecord
//{
//	int m_Index;public int Index{get{return m_Index;}}
////	string m_Des_SkillName;public string Des_SkillName{get{return m_Des_SkillName;}}
//	ePotency_Type m_Potency_Type;public ePotency_Type Potency_Type{get{return m_Potency_Type;}}
//	ePotency_DurationType m_Potency_DurationType;public ePotency_DurationType Potency_Duration{get{return m_Potency_DurationType;}}
//	string m_Potency_BuffIcon;public string Potency_BuffIcon{get{return m_Potency_BuffIcon;}}
//	int m_Recall_Index;public int Recall_Index{get{return m_Recall_Index;}}
//	
//	public Tbl_SkillPotency_Record(XmlElement _element)// : base(_element)
//	{
//		try{
//			XmlNode node = (XmlElement)_element;
//			
//			SetValue(ref m_Index, node, "Index");
////			SetValue(ref m_Des_SkillName, node, "Des_SkillName");
//			SetValue<ePotency_Type>(ref m_Potency_Type, node, "Potency_Type");
//			SetValue<ePotency_DurationType>(ref m_Potency_DurationType, node, "Potency_DurationType");
//			SetValue(ref m_Potency_BuffIcon, node, "Potency_BuffIcon");
//			SetValue(ref m_Recall_Index, node, "Recall_Index");
//		}
//		catch(System.Exception e)
//		{
//			Debug.LogError("[Tbl_SkillPotency_Record] 'constructor':|" + e + "| error while parsing");
//		}
//	}
//}
//
//public class Tbl_SkillPotency_Table : AsTableBase {
//	SortedList<int, Tbl_SkillPotency_Record> m_ResourceTable = 
//		new SortedList<int, Tbl_SkillPotency_Record>();
//	
//	public Tbl_SkillPotency_Table(string _path)
//	{
////		m_TableName = "CharacterResource";
//		m_TableType = eTableType.NPC;
//		
//		LoadTable(_path);
//	}
//	
//	public override void LoadTable(string _path)
//	{
//		try{
//			XmlElement root = GetXmlRootElement(_path);
//			XmlNodeList nodes = root.ChildNodes;
//			
//			foreach(XmlNode node in nodes)
//			{
//				Tbl_SkillPotency_Record record = new Tbl_SkillPotency_Record((XmlElement)node);
//				m_ResourceTable.Add(record.Index, record);
//			}
//		}
//		catch(System.Exception e)
//		{
//			Debug.LogError("[Tbl_SkillPotency_Table] LoadTable:|" + e + "| error while parsing");
//		}
//	}
//	
//	public Tbl_SkillPotency_Record GetRecord(int _id)
//	{
//		if(m_ResourceTable.ContainsKey(_id) == true)
//		{
//			return m_ResourceTable[_id];
//		}
//		
////		Debug.LogWarning("[Tbl_SkillPotency_Table]GetRecord: there is no " + _id + " record");
//		return null;
//	}
//	
//	public SortedList<int, Tbl_SkillPotency_Record> GetList()
//	{
//		return m_ResourceTable;
//	}
//	
////	public Tbl_Npc_Record GetRecordByTribeAndClass(int _tribe, int _class)
////	{
////		foreach(KeyValuePair<int, Tbl_Npc_Record> pair in m_ResourceTable)
////		{
////			if(pair.Value.Race == _tribe)
////			{
////				if(pair.Value.Class == _class)
////				{
////					return pair.Value;
////				}
////			}
////		}
////		
////		Debug.LogError("Tbl_Npc_Table::GetRecordByTribeAndClass: no record");
////		return null;
////	}
//}
