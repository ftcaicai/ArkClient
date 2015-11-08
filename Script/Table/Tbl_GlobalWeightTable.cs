using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Xml;
using System.IO;
using System.Text;

//public enum ePlayerClass {DEVINE_KNIGHT = 1, ARK_TECHNICAL, MAGICIAN, MAX}
public class Tbl_GlobalWeight_Record
{
	int m_ID;public int ID{get{return m_ID;}}
	string m_Data;public string Data{get{return m_Data;}}
	float m_Value;public float Value{get{return m_Value;}}
	
	public Tbl_GlobalWeight_Record(XmlElement _element)// : base(_element)
	{
		try{
			XmlNode node = (XmlElement)_element;
			
			m_ID = int.Parse(node["ID"].InnerText);
			m_Data = node["Data"].InnerText;
			m_Value = float.Parse(node["Value"].InnerText);
		}
		catch(System.Exception e)
		{
			Debug.LogError("[Tbl_GlobalWeight_Eng_Record] 'constructor': |" + e + "| error while parsing");
		}
	}
}

public class Tbl_GlobalWeight_Table : AsTableBase {
	
	Dictionary<int, Tbl_GlobalWeight_Record> m_GlobalWeightTable = 
		new Dictionary<int, Tbl_GlobalWeight_Record>();
	Dictionary<string, Tbl_GlobalWeight_Record> m_GlobalWeightTableByString = 
		new Dictionary<string, Tbl_GlobalWeight_Record>();
	
	public Tbl_GlobalWeight_Table(string _path)
	{
		m_TableType = eTableType.STRING;
		
		LoadTable(_path);
	}
	
	public override void LoadTable(string _path)
	{
		XmlElement root = GetXmlRootElement(_path);
		XmlNodeList nodes = root.ChildNodes;
		
		foreach(XmlNode node in nodes)
		{
			Tbl_GlobalWeight_Record record = new Tbl_GlobalWeight_Record((XmlElement)node);
			m_GlobalWeightTable.Add(record.ID, record);
			m_GlobalWeightTableByString.Add(record.Data, record);
		}
	}
	
	public Tbl_GlobalWeight_Record GetRecord(int _id)
	{
		if(m_GlobalWeightTable.ContainsKey(_id) == true)
			return m_GlobalWeightTable[_id];
		
		Debug.LogError("[Tbl_GlobalWeight_Base_Table]GetRecord: there is no record [id:" + _id + "]");
		return null;
	}
	
	public Tbl_GlobalWeight_Record GetRecord(string _str)
	{
		if(m_GlobalWeightTableByString.ContainsKey(_str) == true)
			return m_GlobalWeightTableByString[_str];
		
		Debug.LogError("[Tbl_GlobalWeight_Base_Table]GetRecord(string): there is no record [" + _str + "]");
		return null;
	}
}
