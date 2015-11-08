using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.Xml;
using System.IO;
using System.Text;

//public class Tbl_Skill_Potency
//{
//	ePotency_UsableCheck_Target m_Potency_UsableCheck_Target;public ePotency_UsableCheck_Target Potency_UsableCheck_Target{get{return m_Potency_UsableCheck_Target;}}
//	ePotency_UsableCheck_Condition m_Potency_UsableCheck_Condition;public ePotency_UsableCheck_Condition Potency_UsableCheck_Condition{get{return m_Potency_UsableCheck_Condition;}}
//	float m_Potency_UsableCheck_ConditionValue;public float Potency_UsableCheck_ConditionValue{get{return m_Potency_UsableCheck_ConditionValue;}}
//	ePotency_Type m_Potency_Type;public ePotency_Type Potency_Type{get{return m_Potency_Type;}}
//	ePotency_Target m_Potency_Target;public ePotency_Target Potency_Target{get{return m_Potency_Target;}}
//	ePotency_DurationType m_Potency_DurationType;public ePotency_DurationType Potency_Duration{get{return m_Potency_DurationType;}}
//	ePotency_Attribute m_Potency_Attribute;public ePotency_Attribute Potency_Attribute{get{return m_Potency_Attribute;}}
//	ePotency_Element m_Potency_Element;public ePotency_Element Potency_Element{get{return m_Potency_Element;}}
//	string m_Potency_BuffIcon;public string Potency_BuffIcon{get{return m_Potency_BuffIcon;}}
//	
//	public Tbl_Skill_Potency(ePotency_UsableCheck_Target _checkTarget, ePotency_UsableCheck_Condition _checkCondition, float _checkConditionValue, ePotency_Type _type,
//		 ePotency_Target _target, ePotency_DurationType _duration, ePotency_Attribute _attribute, ePotency_Element _element, string _buff)
//	{
//		m_Potency_Target = _target;
//		m_Potency_DurationType = _duration;
//		m_Potency_Attribute = _attribute;
//		m_Potency_Element = _element;
//		m_Potency_BuffIcon = _buff;
//	}
//}

public class Tbl_MonsterPassive_Record : AsTableRecord
{
//	readonly int m_PassiveCount = 5;
	
	int m_Index;public int Index{get{return m_Index;}}
	
	int m_PassiveName_1;public int PassiveName_1{get{return m_PassiveName_1;}}
	int m_PassiveIndex_1;public int PassiveIndex_1{get{return m_PassiveIndex_1;}}
	int m_PassiveName_2;public int PassiveName_2{get{return m_PassiveName_2;}}
	int m_PassiveIndex_2;public int PassiveIndex_2{get{return m_PassiveIndex_2;}}
	int m_PassiveName_3;public int PassiveName_3{get{return m_PassiveName_3;}}
	int m_PassiveIndex_3;public int PassiveIndex_3{get{return m_PassiveIndex_3;}}
	int m_PassiveName_4;public int PassiveName_4{get{return m_PassiveName_4;}}
	int m_PassiveIndex_4;public int PassiveIndex_4{get{return m_PassiveIndex_4;}}
	int m_PassiveName_5;public int PassiveName_5{get{return m_PassiveName_5;}}
	int m_PassiveIndex_5;public int PassiveIndex_5{get{return m_PassiveIndex_5;}}

	public Tbl_MonsterPassive_Record(XmlElement _element)// : base(_element)
	{
		try{
			XmlNode node = (XmlElement)_element;
			
			SetValue(ref m_Index, node, "Index");
			
			SetValue(ref m_PassiveName_1, node, "PassiveName_1");
			SetValue(ref m_PassiveIndex_1, node, "PassiveIndex_1");
			SetValue(ref m_PassiveName_2, node, "PassiveName_2");
			SetValue(ref m_PassiveIndex_2, node, "PassiveIndex_2");
			SetValue(ref m_PassiveName_3, node, "PassiveName_3");
			SetValue(ref m_PassiveIndex_3, node, "PassiveIndex_3");
			SetValue(ref m_PassiveName_4, node, "PassiveName_4");
			SetValue(ref m_PassiveIndex_4, node, "PassiveIndex_4");
			SetValue(ref m_PassiveName_5, node, "PassiveName_5");
			SetValue(ref m_PassiveIndex_5, node, "PassiveIndex_5");
		}
		catch(System.Exception e)
		{
			Debug.LogError("Error while [" + m_CurrentColumn + "] parsing" + "\n" + e);
		}
	}
}

public class Tbl_MonsterPassive_Table : AsTableBase {
	SortedList<int, Tbl_MonsterPassive_Record> m_ResourceTable = 
		new SortedList<int, Tbl_MonsterPassive_Record>();
	
	public Tbl_MonsterPassive_Table(string _path)
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
				Tbl_MonsterPassive_Record record = new Tbl_MonsterPassive_Record((XmlElement)node);
				m_ResourceTable.Add(record.Index, record);
			}
		}
		catch(System.Exception e)
		{
			Debug.LogError("[Tbl_MonsterSkill_Table] LoadTable:|" + e + "| error while parsing");
		}
	}
	
	public Tbl_MonsterPassive_Record GetRecord(int _id)
	{
		if(m_ResourceTable.ContainsKey(_id) == true)
		{
			return m_ResourceTable[_id];
		}
		
		Debug.LogError("[Tbl_MonsterSkill_Table]GetRecord: there is no record");
		return null;
	}
	
	public SortedList<int, Tbl_MonsterPassive_Record> GetList()
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
