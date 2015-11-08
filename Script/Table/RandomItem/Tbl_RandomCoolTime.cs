using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Xml;


public class Tbl_RandomCoolTime_Record
{
	int m_Item_Index;public int Item_Index{get{return m_Item_Index;}}	
	float m_ValueTime;public float ValueTime{get{return m_ValueTime;}}
	//bool m_NeedEffect; public bool needEffect{get{return m_NeedEffect;}}
	
	public Tbl_RandomCoolTime_Record(XmlElement _element)// : base(_element)
	{
		try{
			XmlNode node = (XmlElement)_element;
			
			m_Item_Index = int.Parse(node["Item_Index"].InnerText);			
			m_ValueTime = float.Parse(node["Time"].InnerText);
			//m_NeedEffect = bool.Parse( node["Eff_Type"].InnerText );
		}
		catch(System.Exception e)
		{
			Debug.LogError("[Tbl_RandomCoolTime_Record] 'constructor': |" + e + "| error while parsing");
		}
	}
}

public class Tbl_RandomCoolTime_Table : AsTableBase {
	
	Dictionary<int, Tbl_RandomCoolTime_Record> m_TableRecordList = new Dictionary<int, Tbl_RandomCoolTime_Record>();

	
	public Tbl_RandomCoolTime_Table(string _path)
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
			Tbl_RandomCoolTime_Record record = new Tbl_RandomCoolTime_Record((XmlElement)node);
			if( true == m_TableRecordList.ContainsKey( record.Item_Index ) )
			{
				Debug.LogError("Tbl_RandomCoolTime_Table::LoadTable()[ exist item id : " + record.Item_Index );
				continue;
			}
			m_TableRecordList.Add(record.Item_Index, record);		
		}
	}
	
	public Tbl_RandomCoolTime_Record GetRecord(int _id)
	{
		if(m_TableRecordList.ContainsKey(_id) == true)
			return m_TableRecordList[_id];
		
		Debug.LogError("[Tbl_RandomCoolTime_Table]GetRecord: there is no record id " + _id );
		return null;
	}	
}
