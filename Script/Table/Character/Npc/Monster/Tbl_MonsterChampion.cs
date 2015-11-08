using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.Xml;
using System.IO;
using System.Text;

public class Tbl_MonsterChampion_Record : AsTableRecord
{
	int m_Id;public int Id{get{return m_Id;}}
	string m_Class;public string MonsterName{get{return m_Class;}}
	float m_Scale;public float Scale{get{return m_Scale;}}
	int m_MaxCount;public int MaxCount{get{return m_MaxCount;}}
	float m_AppearProb;public float AppearProb{get{return m_AppearProb;}}
	
	public Tbl_MonsterChampion_Record(XmlElement _element)// : base(_element)
	{
		try{
			XmlNode node = (XmlElement)_element;
						
			SetValue(ref m_Id, node, "Index");
			SetValue(ref m_Class, node, "MonsterName");
            SetValue(ref m_Scale, node, "Scale");
			SetValue(ref m_MaxCount, node, "MaxCount");
			SetValue(ref m_AppearProb, node, "AppearProb");
		}
		catch(System.Exception e)
		{
			Debug.LogError(e);
			Debug.Log(_element.Name);
		}
	}
}

public class Tbl_MonsterChampion_Table : AsTableBase {
	SortedList<int, Tbl_MonsterChampion_Record> m_ResourceTable = 
		new SortedList<int, Tbl_MonsterChampion_Record>();
	
	public Tbl_MonsterChampion_Table(string _path)
	{
//		m_TableName = "CharacterResource";
		m_TableType = eTableType.MONSTER;
		
		LoadTable(_path);
	}
	
	public override void LoadTable(string _path)
	{
		try{
			XmlElement root = GetXmlRootElement(_path);
			XmlNodeList nodes = root.ChildNodes;
			
			foreach(XmlNode node in nodes)
			{
				Tbl_MonsterChampion_Record record = new Tbl_MonsterChampion_Record((XmlElement)node);
				m_ResourceTable.Add(record.Id, record);
			}
		}
		catch(System.Exception e)
		{
			Debug.LogError(e);
		}
	}
	
	public Tbl_MonsterChampion_Record GetRecord(int _id)
	{
		if(m_ResourceTable.ContainsKey(_id) == true)
		{
			return m_ResourceTable[_id];
		}
		
		Debug.LogError("[Tbl_MonsterChampion_Table]GetRecord: there is no id[" + _id + "] record");
		return null;
	}
}
