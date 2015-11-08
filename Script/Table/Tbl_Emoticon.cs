using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.Xml;
using System.IO;
using System.Text;

#region - enum -
public enum eEmoticonType {Hunt, Normal}
public enum eActivationType {NONE, Auto, Choice}
public enum eAutoCondition
{
	pCondition_PartyHP40,
	pCondition_PotionRunOut,
	Condition_FirstAttack,
	Condition_PartyReject,
	pCondition_PartyMP10,
	pCondition_BuffCancelALL,
	Condition_Death,
	Condition_GetRareItem,
	Condition_LevelUp,
	Condition_Trade,
	Condition_GetBuff,
	NONE = 12
}
#endregion

public class EmoticonCondition
{
	eActivationType m_ActivationType; public eActivationType ActivationType{get{return m_ActivationType;}}
	eAutoCondition m_AutoCondition; public eAutoCondition AutoCondition{get{return m_AutoCondition;}}
	int m_Prob; public int Prob{get{return m_Prob;}}
	int m_Cool; public int Cool{get{return m_Cool;}}
	
	public EmoticonCondition(eActivationType _type, eAutoCondition _condition, int _prob, int _cool)
	{
		m_ActivationType = _type;
		m_AutoCondition = _condition;
		m_Prob = _prob;
		m_Cool = _cool;
	}
}

public class Tbl_Emoticon_Record : AsTableRecord//
{
	public static readonly int s_ConditionSize = 3;
	
	int m_Index; public int Index{get{return m_Index;}}
	string m_EmoticonPath; public string EmoticonPath{get{return m_EmoticonPath;}}
	eEmoticonType m_Section; public eEmoticonType Section{get{return m_Section;}}
	int m_ButtonString; public int ButtonString{get{return m_ButtonString;}}
	int[] m_ChatString = new int[6]; public int[] ChatString{get{return m_ChatString;}}
	
	List<EmoticonCondition> m_listCondition = new List<EmoticonCondition>(); public List<EmoticonCondition> listCondition{get{return m_listCondition;}}
	
	public Tbl_Emoticon_Record(XmlElement _element)// : base(_element)
	{
		try{
			XmlNode node = (XmlElement)_element;
			
			SetValue(ref m_Index, node, "Index");
			SetValue(ref m_EmoticonPath, node, "EmoticonPath");
			SetValue<eEmoticonType>(ref m_Section, node, "Section");
			SetValue(ref m_ButtonString, node, "ButtonString");
			int strIdx = 0;
			SetValue(ref strIdx, node, "ChatString1"); m_ChatString[0] = strIdx;
			SetValue(ref strIdx, node, "ChatString2"); m_ChatString[1] = strIdx;
			SetValue(ref strIdx, node, "ChatString3"); m_ChatString[2] = strIdx;
			SetValue(ref strIdx, node, "ChatString4"); m_ChatString[3] = strIdx;
			SetValue(ref strIdx, node, "ChatString5"); m_ChatString[4] = strIdx;
			SetValue(ref strIdx, node, "ChatString6"); m_ChatString[5] = strIdx;
			
			for(int i=1; i<=s_ConditionSize; ++i)
			{
				eActivationType activationType = eActivationType.NONE;
				eAutoCondition autoCondition = eAutoCondition.NONE;
				int prob = 0;
				int cool = 0;
				
				SetValue<eActivationType>(ref activationType, node, "Type" + i);
				SetValue<eAutoCondition>(ref autoCondition, node, "AutoCondition" + i);
				SetValue(ref prob, node, "Prob" + i);
				SetValue(ref cool, node, "Cool" + i);
				
				EmoticonCondition condition = new EmoticonCondition(activationType, autoCondition, prob, cool);
				m_listCondition.Add(condition);
			}
		}
		catch(System.Exception e)
		{
			Debug.LogError(e);
		}
	}
	
//	public int GetRandomString()
//	{
//		return m_ChatString[UnityEngine.Random.Range(0, 3)];
//	}
	
	public int GetRandomString(eGENDER _gender)
	{
		switch(_gender)
		{
		case eGENDER.eGENDER_MALE:
			return m_ChatString[UnityEngine.Random.Range(0, 3)];
		case eGENDER.eGENDER_FEMALE:
			return m_ChatString[UnityEngine.Random.Range(3, 6)];
		default:
			Debug.LogError("Tbl_Emoticon_Record::GetRandomString: invalid gender = " + _gender);
			return -1;
//			break;
		}
	}
	
//	eAutoCondition m_AutoCondition;
//	public eAutoCondition AutoCondition
//	{
//		get
//		{
//			foreach(EmoticonCondition node in m_listCondition)
//			{
//				
//			}
//			return m_AutoCondition;
//		}
//	}
	
	public bool CheckContainCondition(eAutoCondition _condition)
	{
		foreach(EmoticonCondition node in m_listCondition)
		{
			if(node.AutoCondition == _condition)
				return true;
		}
		
		return false;
	}
	
	public int GetConditionCool(eAutoCondition _condition)
	{
		foreach(EmoticonCondition node in m_listCondition)
		{
			if(node.AutoCondition == _condition)
				return node.Cool;
		}
		
		return 0;
	}
	public int GetConditionProb(eAutoCondition _condition)
	{
		foreach(EmoticonCondition node in m_listCondition)
		{
			if(node.AutoCondition == _condition)
				return node.Prob;
		}
		
		return 0;
	}
	public eActivationType GetConditionActivation(eAutoCondition _condition)
	{
		foreach(EmoticonCondition node in m_listCondition)
		{
			if(node.AutoCondition == _condition)
				return node.ActivationType;
		}
		
		return eActivationType.NONE;
	}
}

public class Tbl_Emoticon_Table : AsTableBase {
		
	SortedList<int, Tbl_Emoticon_Record> m_ResourceTable = new SortedList<int, Tbl_Emoticon_Record>();
	List<Tbl_Emoticon_Record> m_listHunt = new List<Tbl_Emoticon_Record>(); public List<Tbl_Emoticon_Record> listHunt{get{return m_listHunt;}}
	List<Tbl_Emoticon_Record> m_listNormal = new List<Tbl_Emoticon_Record>(); public List<Tbl_Emoticon_Record> listNormal{get{return m_listNormal;}}
//	Dictionary<eAutoCondition, Tbl_Emoticon_Record> m_dicCondition = new Dictionary<eAutoCondition, Tbl_Emoticon_Record>();
//	public Dictionary<eAutoCondition, Tbl_Emoticon_Record> dicCondition{get{return m_dicCondition;}}
	MultiDictionary<eAutoCondition, Tbl_Emoticon_Record> m_mdicCondition = new MultiDictionary<eAutoCondition, Tbl_Emoticon_Record>();
	public MultiDictionary<eAutoCondition, Tbl_Emoticon_Record> dicCondition{get{return m_mdicCondition;}}
	
	public Tbl_Emoticon_Table(string _path)
	{
		LoadTable(_path);
	}
	
	public override void LoadTable(string _path)
	{
		try{
			XmlElement root = GetXmlRootElement(_path);
			XmlNodeList nodes = root.ChildNodes;
			
			foreach(XmlNode node in nodes)
			{
				Tbl_Emoticon_Record record = new Tbl_Emoticon_Record((XmlElement)node);
				m_ResourceTable.Add(record.Index, record);
				
				switch(record.Section)
				{
				case eEmoticonType.Hunt:
					m_listHunt.Add(record);
					break;
				case eEmoticonType.Normal:
					m_listNormal.Add(record);
					break;
				}
				
				foreach(EmoticonCondition node2 in record.listCondition)
				{
					if(node2.ActivationType != eActivationType.NONE)
						m_mdicCondition.Add(node2.AutoCondition, record);
				}
			}
		}
		catch(System.Exception e)
		{
			Debug.LogError(e);
		}
	}
	
	public Tbl_Emoticon_Record GetRecord(int _idx)
	{
		return m_ResourceTable[_idx];
	}
	
	public Tbl_Emoticon_Record GetRecordByCondition(eAutoCondition _condition)
	{
		if(m_mdicCondition.ContainsKey(_condition) == true && m_mdicCondition[_condition].Count > 0)
		{
			int idx = UnityEngine.Random.Range(0, m_mdicCondition[_condition].Count);
			return m_mdicCondition[_condition][idx];
		}
		else
		{
			Debug.LogWarning("Tbl_Emoticon_Table::GetRecordByCondition: " + _condition + " is not found.");
			return null;
		}
		
//		return null;
//		if(m_dicCondition.ContainsKey(_condition) == true)
//			return m_dicCondition[_condition];
//		else
//		{
//			Debug.LogError("Tbl_Emoticon_Table::GetRecordByCondition: " + _condition + " is not found.");
//			return null;
//		}
	}
}
