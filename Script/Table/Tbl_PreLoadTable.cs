using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Xml;
using System.IO;
using System.Text;

public class Tbl_PreLoad_Record
{
	int m_ID;
	string m_Data;
	
	public int ID{ get{ return m_ID;}}
	public string Data{ get{ return m_Data;}}
	
	public Tbl_PreLoad_Record(XmlElement _element)
	{
		try{
			XmlNode node = (XmlElement)_element;
			
			m_ID = int.Parse(node["MapID"].InnerText);
			m_Data = node["PreLoadID"].InnerText;
		}
		catch(System.Exception e)
		{
			Debug.LogError("[Tbl_PreLoad_Record] 'constructor': |" + e + "| error while parsing");
		}
	}
}

public class Tbl_PreLoad_Table : AsTableBase
{
	Dictionary<int, Tbl_PreLoad_Record> m_PreLoadTable = new Dictionary<int, Tbl_PreLoad_Record>();
	
	public Tbl_PreLoad_Table(string _path)
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
			Tbl_PreLoad_Record record = new Tbl_PreLoad_Record((XmlElement)node);
			m_PreLoadTable.Add(record.ID, record);
		}
	}
	
	public Tbl_PreLoad_Record GetRecord(int _id)
	{
		if(m_PreLoadTable.ContainsKey(_id) == true)
			return m_PreLoadTable[_id];
		
		Debug.LogError("[Tbl_PreLoadTable_Table]GetRecord: there is no record");
		return null;
	}
}
